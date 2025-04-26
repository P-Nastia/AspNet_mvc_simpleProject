using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web1.Data;
using Web1.Data.Entities;
using Web1.Interfaces;
using Web1.Models.Category;
using Web1.Models.User;

namespace Web1.Controllers
{
    public class UsersController(AppDbContext context, IMapper mapper,
        IImageService imageService) : Controller
    {
        public IActionResult Index()
        {
            return View("Create");
        }
        [HttpGet] 
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var uniqueEmail = await context.Users.SingleOrDefaultAsync(x => x.Email == model.Email);

            if (uniqueEmail != null)
            {
                ModelState.AddModelError("Email", "Така пошта уже є!"); 
                return View(model);
            }

            var uniquePhone = await context.Users.SingleOrDefaultAsync(x => x.Phone == model.Phone);

            if (uniquePhone != null)
            {
                ModelState.AddModelError("Email", "Такий номер телефону уже є!");
                return View(model);
            }

            var uniqueNickname = await context.Users.SingleOrDefaultAsync(x => x.Nickname == model.Nickname);

            if (uniqueNickname != null)
            {
                ModelState.AddModelError("Nickname", "Такий псевдонім уже є!");
                return View(model);
            }

            var uniquePassword = await context.Users.SingleOrDefaultAsync(x => x.Password == model.Password);

            if (uniqueNickname != null)
            {
                ModelState.AddModelError("Nickname", "Такий псевдонім уже є!");
                return View(model);
            }

            uniqueEmail = mapper.Map<UserEntity>(model);
            uniqueEmail.ImageUrl = await imageService.SaveImageAsync(model.ImageFile!);
            await context.Users.AddAsync(uniqueEmail);
            await context.SaveChangesAsync();
            return RedirectToAction("UserIndex","Categories");
            
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await context.Users.SingleOrDefaultAsync(x => x.Password == model.Password && x.Nickname == model.Nickname);

            if (user == null)
            {
                ModelState.AddModelError("Password", "Користувача не знайдено");
                return View(model);
            }

            if (user.Role == "user")
            {
                return RedirectToAction("UserIndex", "Categories");
            }
            else if (user.Role == "admin")
            {
                return RedirectToAction("Index", "Categories");

            }
            return View(model);
        }
    }
}
