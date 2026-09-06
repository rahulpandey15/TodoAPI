
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.IdentityModel.Tokens;
using Polly;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;
using Todo.API.Middlewares;
using Todo.Application.Constants;
using Todo.Application.Contracts;

namespace Todo.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

            builder.Services.AddLogging(options =>
            {
                options.AddSeq();
            });

            // builder.Services.AddMemoryCache();


            builder.Services.AddStackExchangeRedisCache(options =>
            {

                options.Configuration = "127.0.0.1:6379";
                options.InstanceName = "redis-instance";
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient(
                    ApplicationConstants.EmailServiceClient,
                    client =>
                    {
                        client.BaseAddress = new Uri(
                            builder.Configuration.GetSection(
                                "EmailService:BaseUrl").Value!);
                    })
                //.AddStandardResilienceHandler();
                .AddResilienceHandler("retry", (pipeline, context) =>
                {
                    var loggerFactory = context.ServiceProvider?.GetService<ILoggerFactory>();
                    var logger = loggerFactory?.CreateLogger("EmailServiceResilience");


                    // 1. RATE LIMITER (Outermost - Control request flow)
                    pipeline.AddRateLimiter(new HttpRateLimiterStrategyOptions
                    {
                        // Define the RateLimiter algorithm
                        DefaultRateLimiterOptions = new ConcurrencyLimiterOptions
                        {
                            PermitLimit = 10,        // Maximum 10 concurrent HTTP requests active at once
                            QueueLimit = 5,          // Up to 5 additional requests can wait in queue
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                        },
                        OnRejected = args =>
                        {
                            logger.LogWarning("[RateLimiter] Request rejected! Concurrency/Queue limit exceeded.");
                            return ValueTask.CompletedTask;
                        }
                    });

                    // 2. TOTAL REQUEST TIMEOUT (Global cap across all retries)
                    pipeline.AddTimeout(new HttpTimeoutStrategyOptions
                    {
                        Timeout = TimeSpan.FromSeconds(30),
                        OnTimeout = args =>
                        {
                            logger.LogError("[TotalTimeout] Entire operation exceeded global deadline of {Timeout}s.",
                                args.Timeout.TotalSeconds);
                            return ValueTask.CompletedTask;
                        }
                    });

                    // 3. RETRY (Recover from transient failures)
                    pipeline.AddRetry(new HttpRetryStrategyOptions
                    {
                        MaxRetryAttempts = 3,
                        Delay = TimeSpan.FromSeconds(2),
                        UseJitter = true,
                        BackoffType = DelayBackoffType.Exponential,
                        OnRetry = args =>
                        {
                            // Log as Warning (not Error), because a retry means we are still attempting to recover
                            logger?.LogWarning(
                                "Retry attempt {Attempt}. Delay: {Delay}ms. Cause: {Reason}",
                                args.AttemptNumber + 1,
                                args.RetryDelay.TotalMilliseconds,
                                args.Outcome.Exception?.Message ?? args.Outcome.Result?.StatusCode.ToString());

                            return ValueTask.CompletedTask;
                        }
                    });

                    // 4. CIRCUIT BREAKER (Evaluates EACH individual attempt inside the retry loop)
                    pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
                    {
                        FailureRatio = 0.5,
                        MinimumThroughput = 3,
                        SamplingDuration = TimeSpan.FromSeconds(30),
                        BreakDuration = TimeSpan.FromSeconds(10),
                        OnOpened = args =>
                        {
                            logger.LogCritical("[CircuitBreaker] Circuit TRIPPED OPEN for {BreakDuration}s due to high failure rate.",
                                args.BreakDuration.TotalSeconds);
                            return ValueTask.CompletedTask;
                        },
                        OnClosed = args =>
                        {
                            logger.LogInformation("[CircuitBreaker] Circuit CLOSED. Service recovered.");
                            return ValueTask.CompletedTask;
                        },
                        OnHalfOpened = args =>
                        {
                            logger.LogInformation("[CircuitBreaker] Circuit HALF-OPEN. Testing service health with next request...");
                            return ValueTask.CompletedTask;
                        }
                    });

                    // 5. ATTEMPT TIMEOUT (Innermost - Stops a single hanging network attempt after 10s)
                    pipeline.AddTimeout(new HttpTimeoutStrategyOptions
                    {
                        Timeout = TimeSpan.FromSeconds(10),
                        OnTimeout = args =>
                        {
                            logger.LogWarning("[AttemptTimeout] Single HTTP request attempt timed out after {Timeout}s.",
                                args.Timeout.TotalSeconds);
                            return ValueTask.CompletedTask;
                        }
                    });
                });

            builder.Services.AddHealthChecks();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt:Secret").Value)),
                        ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value,
                        ValidAudience = builder.Configuration.GetSection("Jwt:Audience").Value,
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ClockSkew = TimeSpan.Zero

                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            var sidClaim = context.Principal?.FindFirst("sid");
                            if (sidClaim == null || !Guid.TryParse(sidClaim.Value, out var sessionId))
                            {
                                context.Fail("Invalid token: missing session reference");
                                return;
                            }

                            var revocationService = context.HttpContext.RequestServices
                                .GetRequiredService<ITokenRevocationService>();

                            bool isRevoked = await revocationService.IsSessionRevokedAsync(sessionId);

                            if (isRevoked)
                            {
                                context.Fail("Session has been revoked");
                            }
                        }

                    };
                    
                });


            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    var response
                     = new
                     {
                         status = report.Status.ToString(),
                         totalDuration = report.TotalDuration.TotalMilliseconds,
                         checks = report.Entries.Select(entry => new
                         {
                             name = entry.Key,
                             status = entry.Value.Status.ToString(),
                             duration = entry.Value.Duration.TotalMilliseconds,
                             description = entry.Value.Description,
                         })
                     };


                    await context.Response.WriteAsync(
                        JsonSerializer.Serialize(response, new JsonSerializerOptions
                        {
                            WriteIndented = true
                        }));
                }
            });

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<UserContextMiddleware>();
            app.MapControllers();
            app.Run();
        }
    }
}
