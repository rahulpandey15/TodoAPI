using Microsoft.EntityFrameworkCore;
using Todo.Application.Implementation;
using Todo.Application.Interface;
using Todo.Domain.DomainEntities;
using Todo.Domain.RepositoryInterfaces;
using Todo.Infrastructure.Mappers;
using Todo.Infrastructure.Mapping;
using Todo.Infrastructure.Mapping.Mapper;
using Todo.Infrastructure.Persistence.Entities;
using Todo.Infrastructure.Repositories;

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

            services.AddScoped<IMappingService, MappingService>();

            services.AddScoped<IMapper<UserDomain, User>, UserEntityMapper>();

            services.AddScoped<IUserRepository, UserRepository>();


            return services;
        }


        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {

            services.AddScoped<IUserService, UserService>();
                

            return services;
        }
    }
}
