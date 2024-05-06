using GymManagement.Models.AttendanceModels;
using GymManagement_API.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GymManagement_API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ClassController(GymManagementContext context) : Controller
{
	private readonly GymManagementContext _context = context;

	[HttpGet]
	public async Task<IActionResult> GetClasses() => Ok(await _context.Classes.ToListAsync());

	[HttpGet("{id}")]
	public async Task<IActionResult> GetClass(int id)
	{
		try
		{
			var cl = await _context.Classes.FirstOrDefaultAsync(p => p.ClassId == id);
			if (cl == null) return NotFound();

			return Ok(cl);
		}
		catch (DbUpdateException ex)
		{
			return HandleDbUpdateException(ex);
		}
	}

	[HttpPost]
	public async Task<IActionResult> PostClass([FromForm] ClassModel model)
	{
		var trainer = HttpContext.Session.GetString("Trainer_Id");
		if (trainer == null) return RedirectToAction("Login", "Account");

		if (ModelState.IsValid) return BadRequest(ModelState);

		try
		{
			Class cl = new()
			{
				TrainerId = model.TrainerId,
				MemberId = model.MemberId,
				ClassName = model.ClassName
			};
			_context.Add(cl);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetClass), new { id = cl.ClassId }, cl);
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"Internal server error: {ex.Message}");
		}
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> PutClass(int id, [FromBody] ClassModel model)
	{
		try
		{
			var cl = await _context.Classes.FindAsync(id);
			if (cl == null) return NotFound();

			cl.TrainerId = model.TrainerId;
			cl.MemberId = model.MemberId;
			cl.ClassName = model.ClassName;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ClassExists(id))
				{
					return NotFound();
				}
				throw;
			}
			return NoContent();
		}
		catch (DbUpdateException ex)
		{
			return HandleDbUpdateException(ex);
		}
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteClass(int id)
	{
		try
		{
			var cl = await _context.Classes.FindAsync(id);
			if (cl == null) return NotFound();
			
			_context.Classes.Remove(cl);
			await _context.SaveChangesAsync();

			return Ok(cl);
		}
		catch (DbUpdateException ex)
		{
			return HandleDbUpdateException(ex);
		}
	}

	private bool ClassExists(int id) => _context.Classes.Any(e => e.ClassId == id);

	private IActionResult HandleDbUpdateException(DbUpdateException ex)
	{
		if (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 547 || sqlEx.Number == 2627))
		{
			return BadRequest("Operation failed due to a database constraint. Details: " + sqlEx.Message);
		}
		return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Details: " + ex.Message);
	}

}
