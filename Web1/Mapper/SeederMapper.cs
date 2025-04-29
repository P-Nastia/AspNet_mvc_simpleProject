using AutoMapper;
using Web1.Data.Entities;
using Web1.Models.Seeder;
using Web1.Data.Entities.Identity;

namespace Web1.Mapper;

public class SeederMapper : Profile
{
    public SeederMapper()
    {
        CreateMap<SeederCategoryModel, CategoryEntity>()
            .ForMember(x => x.ImageUrl, opt => opt.MapFrom(x => x.Image));
        CreateMap<SeederUserModel, UserEntity>()
            .ForMember(x => x.Image, opt => opt.MapFrom(x => x.Image));
    }
}
