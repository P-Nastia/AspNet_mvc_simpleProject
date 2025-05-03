using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web1.Areas.Admin.Models.Users;
using Web1.Data.Entities.Identity;
using Web1.Models.Account;

namespace Web1.Areas.Admin.Controllers;

[Area("Admin")]
public class UsersController(UserManager<UserEntity> userManager, 
    IMapper mapper) : Controller
{
    public async Task<IActionResult> Index()
    {
        var model = await userManager.Users.ProjectTo<UserItemViewModel>(mapper.ConfigurationProvider).ToListAsync();
        
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var entity = await userManager.FindByIdAsync(id.ToString());
        if (entity == null)
        {
            return NotFound();
        }

        var model = mapper.Map<UserEditViewModel>(entity);
        var roles = await userManager.GetRolesAsync(entity);
        foreach (var role in roles)
        {
            model.SelectedRoles.Add(role);
        }
        
        return View(model);
    }

    [HttpPost]
    public IActionResult Edit(UserEditViewModel model)
    {
        return View();
    }
}