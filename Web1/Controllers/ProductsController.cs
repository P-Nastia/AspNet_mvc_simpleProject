﻿using System.Security.Cryptography;
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
    [HttpGet]

    public async Task<IActionResult> Index(ProductSearchViewModel searchModel)
    {
        ViewBag.Title = "Продукти";

        searchModel.Categories = await mapper.ProjectTo<SelectItemViewModel>(context.Categories).ToListAsync();

        searchModel.Categories.Insert(0, new SelectItemViewModel
        {
            Id = 0,
            Name = "Оберіть категорію"
        });

        var query = context.Products.AsQueryable();

        if (!string.IsNullOrEmpty(searchModel.Name))
        {
            string textSearch = searchModel.Name.Trim();
            query = query.Where(x => x.Name.ToLower().Contains(textSearch.ToLower()));
        }

        if (!string.IsNullOrEmpty(searchModel.Description))
        {
            string textSearch = searchModel.Description.Trim();
            query = query.Where(x => x.Description.ToLower().Contains(textSearch.ToLower()));
        }

        if (searchModel.CategoryId != 0) 
        {
            query = query.Where(x => x.CategoryId == searchModel.CategoryId);
        }

        // відбір тих елементів, які будуть відображатися на сторінці
        var model = new ProductListViewModel();
        model.Count = query.Count();

        model.Products = mapper.ProjectTo<ProductItemViewModel>(query).ToList();
        model.Search = searchModel;

       

        return View(model);
    }

    
}
