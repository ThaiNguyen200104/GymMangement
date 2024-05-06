using GymManagement_API.Models.AccountModels;
using GymManagement_API.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagement_API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AdminController(GymManagementContext context) : Controller
{
	private readonly GymManagementContext _context = context;

	[HttpGet]
	public async Task<IActionResult> Login(SigninModel model)
	{
		if (ModelState.IsValid) return BadRequest("Invalid login attempt.");

		var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == model.Email);
		if (admin != null && BCrypt.Net.BCrypt.Verify(model.Password, admin.Password))
		{
			HttpContext.Session.SetString("Admin_id", admin.Id.ToString());
			HttpContext.Session.SetString("Email", admin.Email.ToString());
			HttpContext.Session.SetString("Password", admin.Password.ToString());
			return RedirectToAction("Index", "Home");
		}
		return Unauthorized("You dont have permisson to enter.");
	}

	[HttpPost]
	public IActionResult Logout()
	{
		HttpContext.Session.Clear();
		return RedirectToAction("Login");
	}
}
