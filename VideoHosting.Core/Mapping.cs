using AutoMapper;
using Microsoft.Extensions.Configuration;
using VideoHosting.Abstractions.Dto;
using VideoHosting.Core.Models;
using VideoHosting.Services.Mapper;

namespace VideoHosting.Core
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<UserRegistrationModel, UserLoginDto>()
                .ForMember(x=>x.Email, opt => opt.MapFrom(c => c.Email))
                .ForMember(x=>x.Password, opt => opt.MapFrom(c => c.Password));

            CreateMap<UserRegistrationModel, UserDto>()
                .ForMember(x => x.Name, opt => opt.MapFrom(c => c.Name))
                .ForMember(x => x.Surname, opt => opt.MapFrom(c => c.Surname))
                .ForMember(x => x.Group, opt => opt.MapFrom(c => c.Group));
        }

        public static IMapper GetMapper(IConfiguration configuration )
        {
            var conf = new MapperConfiguration(opt =>
	            {
		            opt.AddProfiles(new Profile[] {new Mapping(), new MapperService(configuration) });
	            });

            return conf.CreateMapper();
        }
    }
}
