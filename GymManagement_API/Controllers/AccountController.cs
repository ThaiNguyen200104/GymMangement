using GymManagement_API.Models.AccountModels;
using GymManagement_API.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace GymManagement_API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class MemberController : ControllerBase
{
	private readonly GymManagementContext _context;

	[HttpPost("signup")]
	public async Task<IActionResult> SignUp([FromBody] SignupModel model)
	{
		if (ModelState.IsValid)
		{
			var isTrainer = isTrainerEmail(model.Email);
			var emailExists = await _context.Members.AnyAsync(m => m.Email == model.Email);
			var emailTrainerExists = await _context.Trainers.AnyAsync(t => t.Email == model.Email);
			var usernameExists = await _context.Members.AnyAsync(m => m.UserName == model.Username);
			var phoneExists = await _context.Members.AnyAsync(m => m.PhoneNumber == model.PhoneNumber);

			if (emailExists || usernameExists || phoneExists || emailTrainerExists)
			{
				return BadRequest(new { message = "Some of member information are already exsists, please try again!" });
			}

			if (!isTrainer)
			{
				var member = new Member
				{
					Email = model.Email,
					UserName = model.Username,
					PhoneNumber = model.PhoneNumber,
					Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
					CreateDate = DateTime.UtcNow
				};
				_context.Members.Add(member);
				await _context.SaveChangesAsync();

				return CreatedAtAction(nameof(Profile), new { memberId = member.MemberId }, member);
			}

			var trainer = new Trainer
			{
				Email = model.Email,
				UserName = model.Username,
				PhoneNumber = model.PhoneNumber,
				Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
				CreateDate = DateTime.UtcNow
			};
			_context.Trainers.Add(trainer);
			await _context.SaveChangesAsync();

		}
		return BadRequest(ModelState);
	}

	public bool isTrainerEmail(string email)
	{
		return email.EndsWith("@trainer.com");
	}


	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] SigninModel model)
	{
		if (ModelState.IsValid)
		{
			var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == model.Email);
			var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.Email == model.Email);
			if (member != null && BCrypt.Net.BCrypt.Verify(model.Password, member.Password))
			{
				HttpContext.Session.SetString("Member_id", member.MemberId.ToString());
				HttpContext.Session.SetString("Email", member.Email);
				HttpContext.Session.SetString("UserName", member.UserName);

				return Ok(new { message = "Member login successfully" });
			}
			else if (trainer != null && BCrypt.Net.BCrypt.Verify(model.Password, trainer.Password))
			{
				HttpContext.Session.SetString("Trainer_id", trainer.TrainerId.ToString());
				HttpContext.Session.SetString("Email", trainer.Email);
				HttpContext.Session.SetString("UserName", trainer.UserName);

				return Ok(new { message = "Trainer login successfully" });
			}
			return Unauthorized(new { message = "Failed to login" });
		}
		return BadRequest(ModelState);
	}

	[HttpGet("profile")]
	public async Task<IActionResult> Profile()
	{
		Member member = null;
		var memberIdString = HttpContext.Session.GetString("Member_id");
		if (!string.IsNullOrEmpty(memberIdString) && int.TryParse(memberIdString, out int memberId))
		{
			member = await _context.Members.FindAsync(memberId);
		}
		else if (User.Identity.IsAuthenticated)
		{
			var memberEmail = User.FindFirst(ClaimTypes.Email)?.Value;
			if (!string.IsNullOrEmpty(memberEmail))
			{
				member = await _context.Members.FirstOrDefaultAsync(c => c.Email == memberEmail);
			}
		}

		if (member == null)
		{
			return Unauthorized(new { message = "Member is not authenticated" });
		}

		var memPack = await _context.MemberPackages.Where(p => p.MemberId == int.Parse(memberIdString))
													.Include(p => p.Package)
													.Select(p => new ProfileModel.MemberPackage
													{
														PackageId = p.PackageId,
														StartDate = p.StartDate,
														EndDate = p.EndDate,
													}).ToListAsync();

		var model = new ProfileModel
		{
			UserName = member.UserName,
			Email = member.Email,
			Firstname = member.FirtName,
			Lastname = member.LastName,
			PhoneNumber = member.PhoneNumber
		};

		return Ok(model);
	}

	[HttpPost("logout")]
	public IActionResult Logout()
	{
		HttpContext.Session.Clear(); // clears all data stored in session
		return Ok(new { message = "You have been logged out successfully" });
	}
}
