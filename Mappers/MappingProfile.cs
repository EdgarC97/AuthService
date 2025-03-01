using AutoMapper;
using AuthService.Models;
using AuthService.Models.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AuthService.Mappers
{
    // MappingProfile acts as our translator, converting full blueprints to simplified ones and vice versa.
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map User to UserDto, including a custom mapping for FullName.
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            // Map CreateUserDto to User (for registration)
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // We'll set this in the service
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // Map UpdateUserDto to User (for updates)
            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
