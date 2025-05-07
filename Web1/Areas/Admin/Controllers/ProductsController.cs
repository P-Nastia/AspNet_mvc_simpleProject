using AutoMapper;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using Web1.Areas.Admin.Models.Products;
using Web1.Data;
using Web1.Data.Entities;
using Web1.Interfaces;

namespace Web1.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductsController(AppDbContext context, 
    IMapper mapper, IImageService imageService) : Controller
{

    public IActionResult Index()
    {
        ViewBag.Title = "Продукти";
        var model = mapper.ProjectTo<ProductItemViewModel>(context.Products).ToList();
        return View(model);
    }

    [HttpGet] 
    public IActionResult Create()
    {
        ViewBag.Title = "Створити продукт";
        ViewBag.Categories = context.Categories.ToList();
        return View();
    }

    [HttpPost] 
    public async Task<IActionResult> Create(CreateProductViewModel model)
    {
        var existingProduct = await context.Products.SingleOrDefaultAsync(x => x.Name == model.Name);

        if (existingProduct != null)
        {
            ModelState.AddModelError("Name", "Такий продукт вже є!!!");
            return View(model);
        }

        var productEntity = mapper.Map<ProductEntity>(model);

        productEntity.Category = await context.Categories
            .SingleOrDefaultAsync(x => x.Name == model.CategoryName);

        var savedImages = await Task.WhenAll(
            model.Images.Select(async image => new ProductImageEntity
            {
                Name = await imageService.SaveImageFromBase64Async(image.Name),
                Priotity = image.Priority
            })
        );

        productEntity.ProductImages = savedImages.ToList();

        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(productEntity.Description);

        var imgNodes = doc.DocumentNode.SelectNodes("//img");

        if (imgNodes != null)
        {
            foreach (var img in imgNodes)
            {
                var oldSrc = img.GetAttributeValue("src", "");
                
                img.SetAttributeValue("src", $"/images/800_{await imageService.SaveImageFromUrlAsync(oldSrc)}");
            }
        }

        productEntity.Description = doc.DocumentNode.OuterHtml;

        context.Products.Add(productEntity);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var productEntity = await context.Products.Include(x=>x.ProductImages).FirstOrDefaultAsync(x => x.Id == id);
        if (productEntity != null)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(productEntity.Description);

            var imgNodes = doc.DocumentNode.SelectNodes("//img");

            if (imgNodes != null)
            {
                foreach (var img in imgNodes)
                {
                    var src = img.GetAttributeValue("src", "");

                    var substr = src.Substring(src.IndexOf('_') + 1, (src.Length - src.IndexOf('_') - 1));
                    await imageService.DeleteImageAsync(substr);
                }
            }
            foreach(var img in productEntity.ProductImages)
            {
                await imageService.DeleteImageAsync(img.Name);
                context.ProductImages.Remove(img);
            }
            context.Products.Remove(productEntity);
            await context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
