using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Web1.Areas.Admin.Models.Users;
using Web1.Data.Entities.Identity;
using Web1.Interfaces;
using Web1.Models.Account;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Web1.Areas.Admin.Controllers;

[Area("Admin")]
public class UsersController(UserManager<UserEntity> userManager, 
    IMapper mapper,IImageService imageService) : Controller
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
    public async Task<IActionResult> Edit(UserEditViewModel model)
    {
        
        if (!ModelState.IsValid) {
            var entity = await userManager.Users.Where(u => u.Id == model.Id).FirstOrDefaultAsync();
            if ((ModelState["Password"]!.Errors.Count == 1 && ModelState.ErrorCount > 1) || ModelState["Password"]!.Errors.Count == 0)
            {
                model.ViewImage = $"/images/400_{entity.Image}";
                return View(model);
            }
            else
            {
                if (entity != null)
                {
                    if (model.Password != null)
                    {
                        var token = await userManager.GeneratePasswordResetTokenAsync(entity);
                        var result = await userManager.ResetPasswordAsync(entity, token, model.Password);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", "Не вдалося змінити пароль");
                        }
                    }
                    if (model.ImageFile != null)
                    {
                        await imageService.DeleteImageAsync(entity.Image!);
                        entity.Image = await imageService.SaveImageAsync(model.ImageFile!);
                    }
                    entity.LastName = model.LastName;
                    entity.FirstName = model.FirstName;
                    entity.Email = model.Email;
                    entity.UserName = model.UserName;
                    entity.PhoneNumber = model.PhoneNumber;
                    await userManager.UpdateAsync(entity);

                    var currentRoles = await userManager.GetRolesAsync(entity);

                    var removeResult = await userManager.RemoveFromRolesAsync(entity, currentRoles);
                    if (!removeResult.Succeeded)
                    {
                        return BadRequest(removeResult.Errors);
                    }

                    var addResult = await userManager.AddToRolesAsync(entity, model.SelectedRoles);
                    if (!addResult.Succeeded)
                    {
                        return BadRequest(addResult.Errors);
                    }

                    return RedirectToAction(nameof(Index));
                }
            }
        }
        
        return View(model);
    }
}