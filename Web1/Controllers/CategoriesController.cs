using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Web1.Data;
using Web1.Data.Entities;
using Web1.Interfaces;
using Web1.Models.Category;

namespace Web1.Controllers
{
    public class CategoriesController(AppDbContext context,IMapper mapper,
        IImageService imageService) : Controller
    {

        public IActionResult Index() //IActionResult -- будь який web-результат (сторінки html - View, файл, PDF, Excel
        {
            var model = mapper.ProjectTo<CategoryItemViewModel>(context.Categories).ToList();
            model = model.OrderBy(x => x.Id).ToList();
            return View(model); // йде в папку View/Categories/Index.cshtml і виводить там прописаний html
            
        }

        [HttpGet] // тепер функція працює методом GET, щоби побачити форму,якщо не вказувати, то він може бути будь-яким методом
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateViewModel model)
        {
            var entity = await context.Categories.SingleOrDefaultAsync(x => x.Name == model.Name);
            
            if (entity != null)
            {
                ModelState.AddModelError("Name", "Така категорія уже є!"); // виводить помилку, що така категорія вже існує
                return View(model);
            }

            ////////////// для записування фото до БД
            /// використовуємо ImageService
            ////////////////
            entity = mapper.Map<CategoryEntity>(model);
            entity.ImageUrl = await imageService.SaveImageAsync(model.ImageFile);
            await context.Categories.AddAsync(entity);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); // переадресація на головну сторінку
        }


        [HttpGet]
        public async Task<IActionResult> Edit(CategoryEntity entity)
        {
            var item = await context.Categories.SingleOrDefaultAsync(x => x.Id == entity.Id);
            var model = mapper.Map<CategoryEditViewModel>(entity);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CategoryEditViewModel model)
        {
            var item = await context.Categories.SingleOrDefaultAsync(x => x.Name == model.Name && x.Id != model.Id);
            if (item != null)
            {
                ModelState.AddModelError("Name", "Така категорія уже є");
                return View(model);
            }
            var toEdit = await context.Categories.SingleOrDefaultAsync(x => x.Id == model.Id);
            if(toEdit != null)
            {
                toEdit.Name = model.Name;
                toEdit.Description = model.Description;
                toEdit.ImageUrl = model.ImageUrl;
                await context.SaveChangesAsync();
            }
            else
                return View(model);
            return RedirectToAction(nameof(Index)); 
        }
        public async Task<IActionResult> Delete(CategoryEntity entity)
        {
            context.Categories.Remove(entity);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
