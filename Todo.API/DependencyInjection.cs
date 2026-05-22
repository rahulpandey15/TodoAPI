using Microsoft.EntityFrameworkCore;
using Todo.Application;
using Todo.Application.Contracts;
using Todo.Application.Implementation;
using Todo.Domain.RepositoryInterface;
using Todo.Infrastructure;
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


            services.AddAutoMapper(typeof(InfraAssemblyMarker).Assembly); // register automapper only at infra layer


            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }

        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddAutoMapper(
                typeof(ApplicationLayerMarker).Assembly); // register automapper only at service layer


            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPasswordHasher,PasswordHasher>();


            return services;
        }
    }
}
