using AutoMapper;
using Web1.Data.Entities;
using Web1.Models.Category;
using Web1.Models.Seeder;
using Web1.Models.User;

namespace Web1.Mapper;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<UserCreateViewModel, UserEntity>()
            .ForMember(x=>x.ImageUrl,opt=>opt.Ignore())
            .ForMember(x=>x.Role,opt=>opt.MapFrom(u=>"User"))
            .ReverseMap();
    }
}
