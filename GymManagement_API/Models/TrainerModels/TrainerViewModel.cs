﻿namespace GymManagement.Models.TrainerModels;

public class TrainerViewModel
{
	public int TrainerId { get; set; }

	public string Email { get; set; } = null!;

	public string UserName { get; set; } = null!;

	public string? FirtName { get; set; }

	public string? LastName { get; set; }

	public string? PhoneNumber { get; set; }

	public string Password { get; set; } = null!;

	public string? UserImg { get; set; }

	public string? Bio { get; set; }

	public DateTime? CreateDate { get; set; }

	public string? Status { get; set; }
}
