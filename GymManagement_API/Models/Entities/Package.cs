using System;
using System.Collections.Generic;

namespace GymManagement_API.Models.Entities;

public partial class Package
{
    public int PackageId { get; set; }

    public string PackageName { get; set; } = null!;

    public decimal? Cost { get; set; }

    public int? Duration { get; set; }

    public decimal? Discount { get; set; }

    public DateTime? CreateDate { get; set; }

    public virtual ICollection<MemberPackage> MemberPackages { get; set; } = new List<MemberPackage>();
}
