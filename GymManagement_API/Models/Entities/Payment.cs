using System;
using System.Collections.Generic;

namespace GymManagement_API.Models.Entities;

public partial class Payment
{
    public int PaymentId { get; set; }

    public string CardNumber { get; set; } = null!;

    public string CardName { get; set; } = null!;

    public int Amount { get; set; }

    public DateTime ExpiryDate { get; set; }

    public virtual MemberPackage PaymentNavigation { get; set; } = null!;
}
