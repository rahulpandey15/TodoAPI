using Hangfire;
using Microsoft.EntityFrameworkCore;
using Todo.Application.Contracts;
using Todo.Application.Implementation;
using Todo.Domain.RepositoryInterface;
using Todo.Infrastructure;
using Todo.Infrastructure.BackgroundJobs;
using Todo.Infrastructure.Persistence.Entities;
using Todo.Infrastructure.Repository;

namespace Todo.API
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, IConfiguration configuration)
        {
            
            var connectionString
                 = configuration.GetConnectionString("DatabaseConnection");

            services.AddDbContext<TodoAppDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });


            services.AddHealthChecks().AddDbContextCheck<TodoAppDbContext>();
            
            services.AddAutoMapper(typeof(InfraAssemblyMarker).Assembly); // register automapper only at infra layer


            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITodoRepository, TodoRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();


            services.AddHangfire(config => config
                 .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                 .UseSimpleAssemblyNameTypeSerializer()
                 .UseRecommendedSerializerSettings()
                 .UseSqlServerStorage(connectionString, new Hangfire.SqlServer.SqlServerStorageOptions
                 {

                     CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                     SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),  
                     QueuePollInterval = TimeSpan.FromMinutes(5),
                     UseRecommendedIsolationLevel = true,
                     DisableGlobalLocks = true,
                     PrepareSchemaIfNecessary = true // create hangfire table

                 }));


            services.AddHangfireServer(options =>
            {
                options.WorkerCount = Math.Min(Environment.ProcessorCount * 2, 10);
                options.Queues = new[] { "default" };
            });


            services.AddScoped<IBackgroundJobService, HanfireBackgroundJobService>();

            return services;
        }

        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
        
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ITodoService, TodoService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITokenRevocationService, TokenRevokationService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();



            return services;
        }
    }
}
