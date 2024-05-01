using System.ComponentModel.DataAnnotations;

namespace GymManagement_API.Models.AccountModels;

public class SigninModel
{
	[Required, Display(Name ="Email")]
	public string Email { get; set; }
	[Required, DataType(DataType.Password), Display(Name = "Password")]
	public string Password { get; set; }
}
