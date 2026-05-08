using AutoMapper;
using Todo.Application.DTOs.Request;
using Todo.Domain.DomainEntities;

namespace Todo.Application.Mappers
{
    public class UserMappingExtension : Profile
    {

        public UserMappingExtension()
        {
            CreateMap<CreateUserDto, UserDomain>()
                .ForMember(dest => dest.PasswordHash, opt =>
                        opt.MapFrom(src => src.Password));
        }
    }
}
