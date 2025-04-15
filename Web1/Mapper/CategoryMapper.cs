using AutoMapper;
using Web1.Data.Entities;
using Web1.Models.Category;
using Web1.Models.Seeder;

namespace Web1.Mapper;

public class CategoryMapper : Profile
{
    public CategoryMapper()
    {
        CreateMap<CategoryEntity, CategoryItemViewModel>()
            .ForMember(x => x.ImageUrl, opt => opt.MapFrom(x => x.ImageUrl));
        CreateMap<CategoryCreateViewModel, CategoryEntity>(); // для створення нової категорії
        CreateMap<CategoryItemViewModel, CategoryEditViewModel>(); 
        CreateMap<CategoryEditViewModel, CategoryItemViewModel>(); 
        CreateMap<CategoryEntity, CategoryEditViewModel>(); 
    }
}
