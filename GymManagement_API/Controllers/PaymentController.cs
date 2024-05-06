using GymManagement.Models.PackageModels;
using GymManagement_API.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GymManagement_API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PaymentController(GymManagementContext context) : Controller
{
	private readonly GymManagementContext _context = context;
	public async Task<IActionResult> GetPayments() => Ok(await _context.Payments.ToListAsync());

	[HttpPost]
	public async Task<IActionResult> PostPayment([FromForm] PaymentModel model)
	{
		var admin = HttpContext.Session.GetString("Admin_id");
		if (admin == null) return RedirectToAction("LoginAdmin", "Admin");

		if (ModelState.IsValid) return BadRequest(ModelState);

		try
		{
			Payment pay = new()
			{
				CardNumber = model.CardNumber,
				CardName = model.CardName,
				Amount = model.Amount,
				ExpiryDate = model.ExpiryDate
			};
			_context.Add(pay);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetPayments), new { id = pay.PaymentId }, pay);
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"Internal server error: {ex.Message}");
		}
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> EditAmount(int id, [FromBody] PaymentModel model)
	{
		try
		{
			var member = HttpContext.Session.GetString("Member_id");
			if (member == null) return RedirectToAction("login", "account");

			var payment = await _context.Payments.FindAsync(id);
			if (payment == null) return NotFound();

			payment.Amount += model.Amount;
			_context.Update(payment);

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!PaymentExists(model.PaymentId))
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
	public async Task<IActionResult> DeletePayment(int id)
	{
		try
		{
			var pay = await _context.Payments.FindAsync(id);
			if (pay == null) return NotFound();

			_context.Payments.Remove(pay);
			await _context.SaveChangesAsync();

			return Ok(pay);
		}
		catch (DbUpdateException ex)
		{
			return HandleDbUpdateException(ex);
		}
	}

	private bool PaymentExists(int id) => _context.Payments.Any(e => e.PaymentId == id);
	private IActionResult HandleDbUpdateException(DbUpdateException ex)
	{
		if (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 547 || sqlEx.Number == 2627))
		{
			return BadRequest("Operation failed due to a database constraint. Details: " + sqlEx.Message);
		}
		return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Details: " + ex.Message);
	}

}
