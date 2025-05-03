using Microsoft.AspNetCore.Mvc;

namespace Web1.Areas.Admin.Controllers;

[Area("Admin")]
public class FormsController : Controller
{
  public IActionResult BasicInputs() => View();
  public IActionResult InputGroups() => View();
}
