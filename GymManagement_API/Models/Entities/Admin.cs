using System;
using System.Collections.Generic;

namespace GymManagement_API.Models.Entities;

public partial class Admin
{
    public int AdminId { get; set; }

    public string Email { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;
}
