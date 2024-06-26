﻿using System;
using System.Collections.Generic;

namespace GymManagement_API.Models.Entities;

public partial class MemberPackage
{
    public int Id { get; set; }

    public int? PackageId { get; set; }

    public int? MemberId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public virtual Member? Member { get; set; }

    public virtual Package? Package { get; set; }

    public virtual Payment? Payment { get; set; }
}
