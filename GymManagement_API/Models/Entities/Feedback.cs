using System;
using System.Collections.Generic;

namespace GymManagement_API.Models.Entities;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int? TrainerId { get; set; }

    public string? Feedback1 { get; set; }

    public DateOnly? FeedbackDate { get; set; }

    public virtual Trainer? Trainer { get; set; }
}
