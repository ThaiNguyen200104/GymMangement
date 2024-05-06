using GymManagement.Models.AccountModels;
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
			if (isTrainer(model.Email))
			{
				var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.Email == model.Email);
				if (trainer != null && BCrypt.Net.BCrypt.Verify(model.Password, trainer.Password))
				{
					HttpContext.Session.SetString("Trainer_id", trainer.TrainerId.ToString());
					HttpContext.Session.SetString("Email", trainer.Email);
					HttpContext.Session.SetString("UserName", trainer.UserName);
					return Ok(new { message = "Login successful", trainerId = trainer.TrainerId, username = trainer.UserName });
				}
				return Unauthorized(new { message = "Invalid login attempt" });
			}
			var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == model.Email);
			if (member != null && BCrypt.Net.BCrypt.Verify(model.Password, member.Password))
			{
				HttpContext.Session.SetString("Member_id", member.MemberId.ToString());
				HttpContext.Session.SetString("Email", member.Email);
				HttpContext.Session.SetString("UserName", member.UserName);
				return Ok(new { message = "Login successful", MemberId = member.MemberId, username = member.UserName });
			}
			ModelState.AddModelError(string.Empty, "Invalid login attempt.");
		}
		return BadRequest(ModelState);
	}

	public bool isTrainer(string email)
	{
		return email.EndsWith("@trainer.com");
	}

	[HttpGet("profile")]
	public async Task<IActionResult> Profile()
	{
		var email = HttpContext.Session.GetString("Email");
		var trainerId = int.Parse(HttpContext.Session.GetString("Trainer_id"));
		var memberId = int.Parse(HttpContext.Session.GetString("Member_id"));
		Member member = null;
		if (!string.IsNullOrEmpty(email) && isTrainerEmail(email))
		{
			var trainer = await _context.Trainers.FindAsync(trainerId);
			if (trainer == null)
			{
				return Unauthorized(new { message = "Trainer is not authenticated" });
			}

			var model = new ProfileModel
			{
				UserName = trainer.UserName,
				Email = trainer.Email,
				Firstname = trainer.FirtName,
				Lastname = trainer.LastName,
				PhoneNumber = trainer.PhoneNumber
			};
			return Ok(model);
		}
		else if (memberId != 0 || User.Identity.IsAuthenticated)
		{
			var memberEmail = User.FindFirst(ClaimTypes.Email)?.Value;
			if (!string.IsNullOrEmpty(memberEmail))
			{
				member = await _context.Members.FirstOrDefaultAsync(c => c.Email == memberEmail);
			}
			else
			{
				member = await _context.Members.FirstOrDefaultAsync(c => c.MemberId == memberId);
			}
			if (member == null)
			{
				return Unauthorized(new { message = "Trainer is not authenticated" });
			}
			var memPack = await _context.MemberPackages.Where(p => p.MemberId == memberId)
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
		return BadRequest();
	}

	[HttpPost("logout")]
	public IActionResult Logout()
	{
		HttpContext.Session.Clear(); // clears all data stored in session
		return Ok(new { message = "You have been logged out successfully" });
	}

	[HttpPut("profileEdit")]
	public async Task<IActionResult> ProfileEdit(ProfileEditModel model, IFormFile file)
	{
		if (string.IsNullOrEmpty(HttpContext.Session.GetString("Member_id")) || string.IsNullOrEmpty(HttpContext.Session.GetString("Trainer_id")))
		{
			return RedirectToAction("Login", "Account");
		}

		var memberId = int.Parse(HttpContext.Session.GetString("Member_id"));
		var trainerId = int.Parse(HttpContext.Session.GetString("Trainer_id"));

		if (memberId != 0)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var existsingMemberWithUsername = await _context.Members.AnyAsync(m => m.UserName == model.UserName);
					var existsingMemberPhonenumber = await _context.Members.AnyAsync(m => m.PhoneNumber == model.PhoneNumber);
					if (existsingMemberWithUsername)
					{
						return BadRequest(new { message = "Username already in use by another account." });
					}
					else if (existsingMemberPhonenumber)
					{
						return BadRequest(new { message = "Phone number already in use by another account." });
					}

					var member = await _context.Members.FindAsync(memberId);
					if (member == null)
					{
						return NotFound();
					}

					member.UserName = model.UserName;
					member.FirtName = model.Firstname;
					member.LastName = model.Lastname;
					member.PhoneNumber = model.PhoneNumber;
					member.UserImg = model.UserImg;
					await _context.SaveChangesAsync();
					return Ok(new { message = "Profile updated successfully" });
				}
				return BadRequest(ModelState);

			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while updating the profile. Please try again." });
			}
		}
		else if (trainerId != 0)
		{
			try
			{
				var existsingTrainerWithUsername = await _context.Trainers.AnyAsync(m => m.UserName == model.UserName);
				var existsingTrainerPhonenumber = await _context.Trainers.AnyAsync(m => m.PhoneNumber == model.PhoneNumber);
				if (existsingTrainerWithUsername)
				{
					return BadRequest(new { message = "Username already in use by another account." });
				}
				else if (existsingTrainerPhonenumber)
				{
					return BadRequest(new { message = "Phone number already in use by another account." });
				}

				var trainer = await _context.Trainers.FindAsync(trainerId);
				if (trainer == null)
				{
					return NotFound();
				}
				
				trainer.UserName = model.UserName;
				trainer.FirtName = model.Firstname;
				trainer.LastName = model.Lastname;
				trainer.PhoneNumber = model.PhoneNumber;
				trainer.UserImg = model.UserImg;
				trainer.Bio = model.Bio;
				await _context.SaveChangesAsync();
				return Ok(new { message = "Profile updated successfully" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while updating the profile. Please try again." });
			}
		}
		return BadRequest();
	}

	[HttpPost("SignUpForTrainer")]
	public async Task<IActionResult> SignupForTrainer(SignupModel model)
	{
		var admin = HttpContext.Session.GetString("Admin_id");
		if (admin == null)
		{
			return RedirectToAction("Login", "Admin");
		}
		if (ModelState.IsValid)
		{
			var emailExists = await _context.Trainers.AnyAsync(t => t.Email == model.Email);
			var usernameExists = await _context.Trainers.AnyAsync(t => t.UserName == model.Username);
			var phoneNumerExists = await _context.Trainers.AnyAsync(t => t.PhoneNumber == model.PhoneNumber);

			if (emailExists || usernameExists || phoneNumerExists)
			{
				throw new ArgumentException("Some of the information is already exists.");
			}

			var trainer = new Trainer
			{
				Email = model.Email,
				UserName = model.Username,
				Password = BCrypt.Net.BCrypt.HashPassword(model.Password)
			};
			_context.Trainers.Add(trainer);
			await _context.SaveChangesAsync();
			return Ok(new { message = "Signup successfully" });
		}
		return BadRequest();
	}
}
