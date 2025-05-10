using System.Security.Cryptography;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using Web1.Constants;
using Web1.Data;
using Web1.Data.Entities;
using Web1.Interfaces;
using Web1.Models.Category;
using Web1.Models.Helpers;
using Web1.Models.Product;

namespace Web1.Controllers;

public class ProductsController(AppDbContext context, 
    IMapper mapper) : Controller
{

    public async Task<IActionResult> Index()
    {
        ViewBag.Title = "Продукти";
        var searchModel = new ProductSearchViewModel();

        searchModel.Categories = await mapper.ProjectTo<SelectItemViewModel>(context.Categories).ToListAsync();

        searchModel.Categories.Insert(0, new SelectItemViewModel
        {
            Id = 0,
            Name = "Оберіть категорію"
        });

        var model = new ProductListViewModel();
        model.Products = mapper.ProjectTo<ProductItemViewModel>(context.Products).ToList();
        model.Search = searchModel;
        return View(model);
    }

    
}
