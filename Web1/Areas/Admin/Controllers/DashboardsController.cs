using Microsoft.AspNetCore.Mvc;

namespace Web1.Areas.Admin.Controllers;

[Area("Admin")]
public class DashboardsController : Controller
{
  public IActionResult Index() => View();
}
