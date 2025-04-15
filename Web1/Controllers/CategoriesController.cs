using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web1.Data;
using Web1.Data.Entities;
using Web1.Models.Category;

namespace Web1.Controllers
{
    public class CategoriesController(AppDbContext context,IMapper mapper) : Controller
    {
        //public string Index() //IActionResult
        //{
        //    return "тут повертатимуться категорії "; // View()
        //}

        //List<CategoryItemViewModel> categories = new List<CategoryItemViewModel>
        //{
        //    new CategoryItemViewModel
        //    {
        //        Id = 1,
        //        Name = "Пригодницькі",
        //        Description = "Мультфільми, сповнені захопливих пригод та подорожей.",
        //        Image = "https://uaserial.com/images/serials/65/6569f80a5a17b120847827.webp"
        //    },
        //    new CategoryItemViewModel
        //    {
        //        Id = 2,
        //        Name = "Комедійні",
        //        Description = "Мультфільми, що піднімуть настрій та розсмішать.",
        //        Image = "https://inlviv.in.ua/wp-content/uploads/2018/09/multfylm-korporatsyya-monstrov.jpg"
        //    },
        //    new CategoryItemViewModel
        //    {
        //        Id = 3,
        //        Name = "Фентезі",
        //        Description = "Магічні світи та чарівні істоти чекають на вас.",
        //        Image = "https://www.ukraine-is.com/wp-content/uploads/2023/12/406421527_18311026873190510_5952803201063321047_n.jpg"
        //    },
        //    new CategoryItemViewModel
        //    {
        //        Id = 4,
        //        Name = "Наукова фантастика",
        //        Description = "Подорожі в космос та футуристичні технології.",
        //        Image = "https://fact-news.com.ua/wp-content/uploads/1-585.jpg"
        //    },
        //    new CategoryItemViewModel
        //    {
        //        Id = 5,
        //        Name = "Класика Disney",
        //        Description = "Найкращі класичні мультфільми від студії Disney.",
        //        Image = "https://mini-rivne.com/wp-content/uploads/2018/11/%D0%B1%D0%B5%D0%BC%D0%B1%D1%96.jpg"
        //    }
        //};

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
            var item = await context.Categories.SingleOrDefaultAsync(x => x.Name == model.Name);
            if (item != null)
            {
                ModelState.AddModelError("Name", "Така категорія уже є"); // виводить помилку, що така категорія вже існує
                return View(model);
            }
            item = mapper.Map<CategoryEntity>(model);
            await context.Categories.AddAsync(item);
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
