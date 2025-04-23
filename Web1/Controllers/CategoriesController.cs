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
            ViewBag.Title = "Категорії";// витягується в html коді
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
        public async Task<IActionResult> Edit(int id)
        {
            var entity = await context.Categories.SingleOrDefaultAsync(x => x.Id == id);
            if(entity == null)
            {
                return NotFound();
            }
            // динамічна колекція, яка зберігає динамічні дані, які можна використати на View
            //ViewBag.ImageName = category.ImageUrl;

            //TempData["ImageUrl"] = category.ImageUrl;

            var model = mapper.Map<CategoryEditViewModel>(entity);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CategoryEditViewModel model)
        {

            if (!ModelState.IsValid) // перевірка чи всі дані введено
            {
                return View(model);
            }
            var existing = await context.Categories.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (existing == null)
            {
                return NotFound();
            }

            var duplicate = await context.Categories.FirstOrDefaultAsync(x => x.Id != model.Id && x.Name == model.Name);
            if (duplicate != null)
            {
                ModelState.AddModelError("Name", "Така категорія уже існує");
                return View(model);
            }

            existing = mapper.Map(model, existing);

            if (model.ImageFile != null)
            {
                await imageService.DeleteImageAsync(existing.ImageUrl);
                existing.ImageUrl = await imageService.SaveImageAsync(model.ImageFile);
            }
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await context.Categories.SingleOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(category.ImageUrl))
            {
                await imageService.DeleteImageAsync(category.ImageUrl);
            }

            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
