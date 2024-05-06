using GymManagement.Models.AttendanceModels;
using GymManagement_API.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GymManagement_API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AttendancesController : ControllerBase
{
	private readonly GymManagementContext context;

	[HttpGet("list")]
	public async Task<IActionResult> List()
	{
		var admin = HttpContext.Session.GetString("Admin_id");
		if (admin == null)
		{
			return Unauthorized("User is not authenticated.");
		}
		var attendance = await context.Attendances.ToListAsync();
		return Ok(attendance);
	}

	[HttpPost("Create")]
	public async Task<IActionResult> AttendanceCreate(AttendancesModel model)
	{
		var trainer = HttpContext.Session.GetString("Trainer_id");
		if (trainer == null)
		{
			return Unauthorized("User is not authenticated");
		}

		if (ModelState.IsValid)
		{
			Attendance at = new()
			{
				MemberId = model.MemberId,
				ClassId = model.ClassId,
				AttendanceDate = model.AttendanceDate
			};
			context.Attendances.Add(at);
			await context.SaveChangesAsync();
			return RedirectToAction(nameof(List));
		}
		return BadRequest(ModelState);
	}

	[HttpGet("{attendanceId}")]
	public async Task<IActionResult> Edit(int attendanceId)
	{
		if (attendanceId == 0)
		{
			return NotFound();
		}
		var attend = await context.Attendances.Where(a => a.AttendanceId == attendanceId).Select(a => new AttendanceEditModel
		{
			AttendanceId = a.AttendanceId,
			MemberId = a.MemberId,
			ClassId = a.ClassId,
			AttendanceDate = a.AttendanceDate,
			ClassName = a.Class.ClassName,
			MemberName = a.Member.UserName
		}).FirstOrDefaultAsync();

		if (attend == null)
		{
			return NotFound();
		}
		return Ok(attend);
	}

	[HttpPut("edit")]
	public async Task<IActionResult> Edit(int attendanceId, [FromForm] Attendance model)
	{
		try
		{
			var attendance = await context.Attendances.FindAsync(attendanceId);
			if (attendance == null)
			{
				return NotFound();
			}

			attendance.MemberId = model.MemberId;
			attendance.ClassId = model.ClassId;
			attendance.AttendanceDate = model.AttendanceDate;
			try
			{
				await context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException){
				if (!AttandanceExists(attendanceId))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
			return NoContent();
		}
		catch (DbUpdateException ex)
		{
			return HandleDbUpdateException(ex);
		}
	}

	private bool AttandanceExists(int id)
	{
		return context.Attendances.Any(e => e.AttendanceId == id);
	}

	private IActionResult HandleDbUpdateException(DbUpdateException ex)
	{
		if (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 547 || sqlEx.Number == 2627))
		{
			return BadRequest("Operation failed due to a database constraint. Details: " + sqlEx.Message);
		}
		else
		{
			return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Details: " + ex.Message);
		}
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteProduct(string id)
	{
		try
		{
			var product = await context.Attendances.FindAsync(id);
			if (product == null)
			{
				return NotFound();
			}

			context.Attendances.Remove(product);
			await context.SaveChangesAsync();

			return Ok(product);
		}
		catch (DbUpdateException ex)
		{
			return HandleDbUpdateException(ex);
		}
	}
}
