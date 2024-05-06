using System.ComponentModel.DataAnnotations;

namespace GymManagement.Models.AccountModels;

public class AdminLoginModel
{
    [Required, Display(Name = "Email")]
    public string Email { get; set; }
    [Required, DataType(DataType.Password), Display(Name = "Password")]
    public string Password { get; set; }
}
