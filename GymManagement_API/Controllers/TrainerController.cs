using GymManagement.Models.TrainerModels;
using GymManagement_API.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagement_API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TrainerController : ControllerBase
{
	private readonly GymManagementContext context;

	public async Task<IActionResult> Get() => Ok(await context.Trainers.ToListAsync());

	[HttpPost("Edit")]
	public async Task<IActionResult> StatusEdit(TrainerViewModel model)
	{
		var adminId = HttpContext.Session.GetString("Admin_id");
		if (string.IsNullOrEmpty(adminId) || !int.TryParse(adminId, out int userId))
		{
			return Unauthorized(new { message = "Admin is not authenticated" });
		}
		if (ModelState.IsValid)
		{
			var trainer = await context.Trainers.FindAsync(adminId);
			if (trainer == null) return NotFound();
			trainer.Status = model.Status;
			try
			{
				context.Update(trainer);
				await context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!TrainerExists(model.TrainerId))
				{
					return NotFound();
				}
				throw;
			}
			return RedirectToAction(nameof(Get));
		}
		return BadRequest();
	}

	private bool TrainerExists(int id) => context.Trainers.Any(e => e.TrainerId== id);

	public async Task<IActionResult> GetMemberByClass()
	{
		var trainerId = int.Parse(HttpContext.Session.GetString("Trainer_id"));
		var trainer = await context.Trainers.FindAsync(trainerId);
		if (trainerId == 0) return RedirectToAction("login", "acccount");
		var classes = await context.Classes.Where(c => c.TrainerId == trainerId).ToListAsync();

		var membersInClasses = new List<Member>();

		foreach (var cls in classes)
		{
			var members = context.Attendances
				.Where(a => a.ClassId == cls.ClassId)
				.Select(a => a.Member)
				.ToList();

			membersInClasses.AddRange(members);
		}
		return Ok(membersInClasses);
	}
}
