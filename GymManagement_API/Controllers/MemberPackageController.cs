using GymManagement.Models.PackageModels;
using GymManagement_API.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GymManagement_API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class MemberPackageController(GymManagementContext context) : Controller
{
	private readonly GymManagementContext _context = context;

	public async Task<IActionResult> GetMemPacks() => Ok(await _context.MemberPackages.Where(m => m.EndDate < DateTime.Now).ToListAsync());

	[HttpGet("{id}")]
	public async Task<IActionResult> GetMemPack(int id)
	{
		try
		{
			var cl = await _context.MemberPackages.FirstOrDefaultAsync(p => p.Id == id);
			if (cl == null) return NotFound();

			return Ok(cl);
		}
		catch (DbUpdateException ex)
		{
			return HandleDbUpdateException(ex);
		}
	}

	[HttpPost]
	public async Task<IActionResult> PostMemPack([FromForm] MemberPackageModel model)
	{
		var admin = HttpContext.Session.GetString("Admin_id");
		if (admin == null) return RedirectToAction("LoginAdmin", "Admin");

		if (ModelState.IsValid) return BadRequest(ModelState);
		try
		{
			MemberPackage mp = new()
			{
				PackageId = model.PackageId,
				MemberId = model.MemberId,
				StartDate = model.StartDate,
				EndDate = model.EndDate,
			};
			_context.Add(mp);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(GetMemPacks));
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"Internal server error: {ex.Message}");
		}
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> PutMemPack(int id, [FromBody] MemberPackageModel model)
	{
		try
		{
			var mp = await _context.MemberPackages.FindAsync(id);
			if (mp == null) return NotFound();

			mp.PackageId = model.PackageId;
			mp.MemberId = model.MemberId;
			mp.StartDate = model.StartDate;
			mp.EndDate = model.EndDate;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!MemberPackageExists(id))
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
	public async Task<IActionResult> DeleteMemPack(int id)
	{
		try
		{
			var mp = await _context.MemberPackages.FindAsync(id);
			if (mp == null) return NotFound();

			_context.MemberPackages.Remove(mp);
			await _context.SaveChangesAsync();

			return Ok(mp);
		}
		catch (DbUpdateException ex)
		{
			return HandleDbUpdateException(ex);
		}
	}

	private bool MemberPackageExists(int id) => _context.MemberPackages.Any(e => e.Id == id);

	private IActionResult HandleDbUpdateException(DbUpdateException ex)
	{
		if (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 547 || sqlEx.Number == 2627))
		{
			return BadRequest("Operation failed due to a database constraint. Details: " + sqlEx.Message);
		}
		return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Details: " + ex.Message);
	}
}
