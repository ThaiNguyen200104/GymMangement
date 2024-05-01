using System;
using System.Collections.Generic;

namespace GymManagement_API.Models.Entities;

public partial class Member
{
    public int MemberId { get; set; }

    public string Email { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string? FirtName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public string Password { get; set; } = null!;

    public string? UserImg { get; set; }

    public DateTime? CreateDate { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual ICollection<MemberPackage> MemberPackages { get; set; } = new List<MemberPackage>();
}
