using AutoMapper;
using Web1.Data.Entities.Identity;
using Web1.Areas.Admin.Models;

namespace Web1.Areas.Admin.Mapper;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<UserEntity, UserItemViewModel>()
            .ForMember(x => x.Image, opt=>opt.MapFrom(x=>x.Image))
            .ReverseMap();

    }
}
