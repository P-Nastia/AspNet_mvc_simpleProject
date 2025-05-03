using AutoMapper;
using Web1.Areas.Admin.Models.Users;
using Web1.Data.Entities.Identity;

namespace Web1.Areas.Admin.Mapper;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<UserEntity, UserItemViewModel>()
            .ForMember(x => x.Image, opt => opt.MapFrom(x => x.Image))
            .ReverseMap();

        CreateMap<UserEntity, UserEditViewModel>()
            .ForMember(x => x.ImageFile, opt => opt.Ignore())
            .ForMember(x => x.ViewImage, opt => opt.MapFrom(x =>
            string.IsNullOrEmpty(x.Image) ? "/pictures/default.png" : $"/images/400_{x.Image}"))
            .ForMember(x => x.Roles, opt => opt.Ignore()).
            ForMember(x => x.SelectedRoles, opt => opt.Ignore())
            .ReverseMap();

    }
}
