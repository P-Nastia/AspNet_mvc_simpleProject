using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web1.Areas.Admin.Models;
using Web1.Data.Entities.Identity;
using Web1.Interfaces;

namespace Web1.Areas.Admin.Controllers;

[Area("Admin")]
public class UsersController(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager, IImageService imageService, IMapper mapper) : Controller
{
    public async Task< IActionResult> Index()
    {
        var allUsers = userManager.Users.ToList();
        List<UserEntity> users = new List<UserEntity>();
        foreach(var user in allUsers)
        {
            var isAdmin = false;
            var roles = await userManager.GetRolesAsync(user);
            foreach(var role in roles)
            {
                if (role.Contains("admin"))
                    isAdmin=true;
            }
            if (!isAdmin)
            {
                users.Add(user);
            }
        }
        users = users.OrderBy(u => u.Id).ToList();
        var model = mapper.Map<List<UserItemViewModel>>(users);
        
        return View(model);
    }
}
