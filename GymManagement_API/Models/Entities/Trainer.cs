using System;
using System.Collections.Generic;

namespace GymManagement_API.Models.Entities;

public partial class Trainer
{
    public int TrainerId { get; set; }

    public string Email { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string? FirtName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public string Password { get; set; } = null!;

    public string? Bio { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();
}
