
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Todo.API.Middlewares;
using Todo.Application.Constants;

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

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient(
                    ApplicationConstants.EmailServiceClient,
                    client =>
                    {
                        client.BaseAddress = new Uri(
                            builder.Configuration.GetSection(
                                "EmailService:BaseUrl").Value!);
                    })
                .AddResilienceHandler("retry", (pipeline, context) =>
                {
                    var loggerFactory = context.ServiceProvider.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger("EmailServiceResilience");
                    
                    // 1. TIMEOUT - Prevent hanging requests
                    pipeline.AddTimeout(new HttpTimeoutStrategyOptions
                    {
                        Timeout = TimeSpan.FromSeconds(10)
                    });

                    // 2. CIRCUIT BREAKER - Prevent cascading failures
                    pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
                    {
                        FailureRatio = 0.5,
                        MinimumThroughput = 3,
                        SamplingDuration = TimeSpan.FromSeconds(30),
                        BreakDuration = TimeSpan.FromSeconds(10)
                    });

                    // 3. RATE LIMITER - Control request flow
                    pipeline.AddRateLimiter(new HttpRateLimiterStrategyOptions());

                    // 4. RETRY - Recover from transient failures
                    pipeline.AddRetry(new HttpRetryStrategyOptions
                    {
                        MaxRetryAttempts = 3,
                        Delay = TimeSpan.FromSeconds(2),
                        UseJitter = true,
                        BackoffType = DelayBackoffType.Exponential,
                        OnRetry = args =>
                        {
                            logger.LogError(
                                "Retry attempt {Attempt}/{MaxAttempts}. Delay: {Delay}ms",
                                args.AttemptNumber + 1,
                                3,
                                args.RetryDelay.TotalMilliseconds);

                            return ValueTask.CompletedTask;
                        }
                    });
                });
            
            builder.Services.AddHealthChecks();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt:Secret").Value)),
                        ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value,
                        ValidAudience = builder.Configuration.GetSection("Jwt:Audience").Value,
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ClockSkew = TimeSpan.Zero

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
