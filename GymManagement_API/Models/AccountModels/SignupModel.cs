using System.ComponentModel.DataAnnotations;

namespace GymManagement.Models.AccountModels;

public class SignupModel
{
	[Required, EmailAddress, Display(Name = "Email")]
	public string Email { get; set; }

	[Required, Display(Name = "UserName")]
	public string Username { get; set; }

	[Display(Name = "Phone Number")]
	public string PhoneNumber { get; set; }

	[Required, DataType(DataType.Password), Display(Name = "Password")]
	public string Password { get; set; }

	[DataType(DataType.Password), Compare("Password", ErrorMessage = "The password and confirmPassword do not match.")]
	[Display(Name = "Confirm Password")]
	public string ConfirmPassword { get; set; }
}
