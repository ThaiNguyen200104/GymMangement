﻿using GymManagement.Models.PackageModel;
using GymManagement_API.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GymManagement_API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PackageController(GymManagementContext context) : Controller
{
	private readonly GymManagementContext _context = context;

	[HttpGet]
	public async Task<IActionResult> GetPackages() => Ok(await _context.Packages.ToListAsync());

	[HttpGet("{id}")]
	public async Task<IActionResult> GetPackage(int id)
	{
		try
		{
			var pack = await _context.Packages.FirstOrDefaultAsync(p => p.PackageId == id);
			if (pack == null) return NotFound();

			return Ok(pack);
		}
		catch (DbUpdateException ex)
		{
			return HandleDbUpdateException(ex);
		}
	}

	[HttpPost]
	public async Task<IActionResult> PostPackage([FromForm] PackageModel model)
	{
		var admin = HttpContext.Session.GetString("Admin_id");
		if (admin == null) return RedirectToAction("LoginAdmin", "Admin");

		if (!ModelState.IsValid) return BadRequest(ModelState);

		try
		{
			Package pack = new()
			{
				PackageName = model.PackageName,
				Cost = model.Cost,
				Duration = model.Duration
			};
			_context.Add(pack);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(GetPackage));
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"Internal server error: {ex.Message}");
		}
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> PutPackage(int id, [FromBody] PackageModel model)
	{
		try
		{
			var pack = await _context.Packages.FindAsync(id);
			if (pack == null) return NotFound();

			pack.PackageName = model.PackageName;
			pack.Cost = model.Cost;
			pack.Duration = model.Duration;
			pack.Discount = model.Discount;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!PackageExists(model.PackageId))
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
	public async Task<IActionResult> DeletePackage(int id)
	{
		try
		{
			var pack = await _context.Packages.FindAsync(id);
			if (pack == null) return NotFound();
			
			_context.Packages.Remove(pack);
			await _context.SaveChangesAsync();

			return Ok(pack);
		}
		catch (DbUpdateException ex)
		{
			return HandleDbUpdateException(ex);
		}
	}

	private bool PackageExists(int id) => _context.Packages.Any(e => e.PackageId == id);

	private IActionResult HandleDbUpdateException(DbUpdateException ex)
	{
		if (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 547 || sqlEx.Number == 2627))
		{
			return BadRequest("Operation failed due to a database constraint. Details: " + sqlEx.Message);
		}
		return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Details: " + ex.Message);
	}
}

