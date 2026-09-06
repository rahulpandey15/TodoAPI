using AutoMapper;
using Todo.Domain.DomainEntities;
using Todo.Infrastructure.Persistence.Entities;

namespace Todo.Infrastructure.Mappers
{
    /// <summary>
    /// AutoMapper profile for RefreshToken entity and domain model mapping.
    /// </summary>
    public class RefreshTokenMappingExtension : Profile
    {
        public RefreshTokenMappingExtension()
        {
            CreateMap<RefreshTokenDomain, RefreshToken>()
                .ReverseMap();
        }
    }
}
