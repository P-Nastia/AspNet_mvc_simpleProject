using AutoMapper;
using Web1.Data.Entities;
using Web1.Models.Category;
using Web1.Models.Helpers;
using Web1.Models.Seeder;

namespace Web1.Mapper;

public class CategoryMapper : Profile
{
    public CategoryMapper()
    {
        CreateMap<CategoryEntity, CategoryItemViewModel>()
            .ForMember(x => x.Image, opt => opt.MapFrom(x => x.ImageUrl));
        CreateMap<CategoryCreateViewModel, CategoryEntity>()
            .ForMember(x=>x.ImageUrl,opt=>opt.Ignore()); // для створення нової категорії

        CreateMap<CategoryEntity, CategoryEditViewModel>()
            .ForMember(x => x.ViewImage, opt => opt.MapFrom(x => 
                 string.IsNullOrEmpty(x.ImageUrl)?"/pictures/default.png":$"/images/400_{x.ImageUrl}"))
            .ForMember(x=>x.ImageFile,opt=>opt.Ignore())
            .ReverseMap(); // і туди і назад mapping;

        CreateMap<CategoryEntity, SelectItemViewModel>();
    }
}
