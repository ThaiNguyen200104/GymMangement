using GymManagement.Models.AccountModels;
using GymManagement_API.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagement_API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class FeedbackController : ControllerBase
{
	private readonly GymManagementContext  context;

	public async Task<IActionResult> Get() => Ok(await context.Feedbacks.ToListAsync());

	[HttpPost("create")]
	public async Task<IActionResult> Post(FeedbackModel model)
	{
		var member = HttpContext.Session.GetString("MemberID");
		if (member == null) return Unauthorized("User is not authenticated");

		if (ModelState.IsValid)
		{
			Feedback fb = new()
			{
				TrainerId = model.TrainerId,
				Feedback1 = model.Feedback1,
				FeedbackDate = DateTime.UtcNow
			};
			context.Add(fb);
			await context.SaveChangesAsync();

			return RedirectToAction(nameof(Get));
		}
		return BadRequest();
	}


}
