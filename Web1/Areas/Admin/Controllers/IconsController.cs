using Microsoft.AspNetCore.Mvc;

namespace Web1.Areas.Admin.Controllers;

[Area("Admin")]
public class IconsController : Controller
{
  public IActionResult RiIcons() => View();
}
