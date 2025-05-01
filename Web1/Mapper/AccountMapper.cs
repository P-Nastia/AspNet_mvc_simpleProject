using AutoMapper;
using Web1.Models.Account;
using Web1.Data.Entities.Identity;
using Web1.Data.Entities;
using Web1.Models.Category;


namespace Web1.Mapper;

public class AccountMapper : Profile
{
    public AccountMapper()
    {
        CreateMap<CreateViewModel, UserEntity>()
            .ForMember(x => x.Image, opt => opt.Ignore())
            .ForMember(x => x.PasswordHash, opt => opt.Ignore());

        CreateMap<UserEntity, ProfileViewModel>()
            .ForMember(x => x.ViewImage, opt => opt.MapFrom(x =>
            string.IsNullOrEmpty(x.Image) ? "/pictures/default.png" : $"/images/400_{x.Image}"))
            .ForMember(x => x.ImageFile, opt => opt.Ignore())
            .ReverseMap(); // і туди і назад mapping;

        CreateMap<UserEntity, UserLinkViewModel>()
                .ForMember(x => x.Name, opt => opt.MapFrom(x => $"{x.LastName} {x.FirstName}"))
                .ForMember(x => x.Image, opt => opt.MapFrom(x => x.Image ?? $"default.webp"));
    }
}
