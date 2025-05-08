using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Web1.Interfaces;
using Web1.Data.Entities.Identity;
using Web1.Models.Account;
using Web1.Constants;
using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Web1.Models.Category;

namespace Web1.Controllers
{
    public class AccountController(UserManager<UserEntity> userManager,SignInManager<UserEntity> signInManager,IImageService imageService, IMapper mapper) : Controller
    {
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                user = await userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    user = mapper.Map<UserEntity>(model);

                    user.Image = await imageService.SaveImageAsync(model.ImageFile!);

                    var result = await userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        Console.WriteLine($"Користувача успішно створено {user.LastName} {user.FirstName}!");
                        await userManager.AddToRoleAsync(user, Roles.User);
                    }
                    else
                    {
                        Console.WriteLine($"Помилка створення користувача:");
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"- {error.Code}: {error.Description}");
                        }
                    }


                    // валідація пароля
                    var res = await signInManager.PasswordSignInAsync(user, model.Password, false, false);// false - 1 чи запам'ятовувати і 2 чи заблокований 
                    if (res.Succeeded)
                    {
                        await signInManager.SignInAsync(user, isPersistent: false);
                        return Redirect("/");// перехід на головну сторінку
                    }
                }
                ModelState.AddModelError("", "Такий нікнейм вже використовується!");
                return View(model);
            }
            ModelState.AddModelError("", "Така пошта вже використовується!");
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                // валідація пароля
                var res = await signInManager.PasswordSignInAsync(user, model.Password, false, false);// false - 1 чи запам'ятовувати і 2 чи заблокований 
                if (res.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return Redirect("/");// перехід на головну сторінку
                }
            }
            ModelState.AddModelError("", "Дані вказано не правильно!");
            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            //вихід з акаунту
            await signInManager.SignOutAsync();
            return Redirect("/");
        }

        [HttpGet]
        public async Task<IActionResult> Profile(string id)
        {
            var entity = await userManager.FindByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            var model = mapper.Map<ProfileViewModel>(entity);
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
    }
}
