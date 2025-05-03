
using Microsoft.AspNetCore.Mvc;
namespace Web1.Areas.Admin.Controllers;

[Area("Admin")]
public class CardsController : Controller
{
  public IActionResult Basic() => View();
}
