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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;
using Web1.Services;
using Web1.SMTP;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Common;
using AspNetCoreGeneratedDocument;

namespace Web1.Controllers
{
    public class AccountController(UserManager<UserEntity> userManager,
        SignInManager<UserEntity> signInManager,IImageService imageService,
        IMapper mapper,ISMTPService sMTPService) : Controller
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

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Користувача з такою поштою не існує");
                return View(model);
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var resetUrl = Url.Action(
                "ResetPassword",// назва дії
                "Account", // назва контролера
                new { email = user.Email, token }, // параметри get,  WebUtility.UrlEncode(user.Email) -- щоби в url адресі пропускало всі символи, бо можливо @ заборонений у прямому використанні
                protocol: Request.Scheme // http, https
                );

            Message msgEmail = new Message
            {
                Body = $"Для скидання паролю перейдіть за посиланням: <a href='{resetUrl}'>скинути пароль</a>",
                Subject = $"Скидання паролю",
                To = model.Email
            };
            var result = await sMTPService.SendMessageAsync(msgEmail);

            if (!result)
            {
                ModelState.AddModelError("", "Помилка надсилання листа. Зверніться у підтримку");
                return View(model);
            }

            return RedirectToAction(nameof(ForgotPasswordSend));
        }

        // відновлення паролю, надіслати лист
        [HttpGet]
        public IActionResult ForgotPasswordSend()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            
            return View(new ResetPasswordViewModel() { Email = email, Token = token });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if(!ModelState.IsValid)
                return View();
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Користувача з такою поштою не існує");
                return View(model);
            }
            if (await userManager.CheckPasswordAsync(user, model.Password))
            {
                ModelState.AddModelError("", "Введіть новий пароль. Ви використали старий");
                return View(model);
            }

            var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if(result.Succeeded)
                return RedirectToAction(nameof(SuccessResetPassword));
            else
            {
                foreach(var error in result.Errors)
                {
                    if(error.Code.Contains("PasswordTooShort"))
                    {
                        ModelState.AddModelError("", "Пароль закороткий");
                        return View(model);
                    }
                }
                ModelState.AddModelError("", "Не вдалося змінити пароль");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult SuccessResetPassword()
        {
            return View();
        }
    }
}
