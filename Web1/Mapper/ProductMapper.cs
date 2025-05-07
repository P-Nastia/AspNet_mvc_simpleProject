using AutoMapper;
using Web1.Data.Entities;
using Web1.Models.Product;
namespace WebSmonder.Mapper;

public class ProductMapper : Profile
{
    public ProductMapper() 
    {
        CreateMap<ProductEntity, ProductItemViewModel>()
            .ForMember(x => x.CategoryName, opt => opt.MapFrom(x => x.Category.Name))
            .ForMember(x => x.Images, opt => opt.MapFrom(x => x.ProductImages.Select(x=>x.Name)));


    }
}
