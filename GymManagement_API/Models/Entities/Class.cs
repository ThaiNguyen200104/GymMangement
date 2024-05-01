using System;
using System.Collections.Generic;

namespace GymManagement_API.Models.Entities;

public partial class Class
{
    public int ClassId { get; set; }

    public int? TrainerId { get; set; }

    public int? MemberId { get; set; }

    public string? ClassName { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual Trainer? Trainer { get; set; }
}
