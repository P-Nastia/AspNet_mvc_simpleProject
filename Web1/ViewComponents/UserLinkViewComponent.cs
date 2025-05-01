using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web1.Data.Entities.Identity;
using Web1.Models.Account;

namespace Web1.ViewComponents;

public class UserLinkViewComponent(UserManager<UserEntity> userManager,
    IMapper mapper) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var userName = User.Identity?.Name;
        var model = new UserLinkViewModel();

        if (userName != null)
        {
            var user = userManager.FindByNameAsync(userName).Result;
            model = mapper.Map<UserLinkViewModel>(user);
        }
        return View(model);
    }
}
