using System;
using System.Collections.Generic;

namespace GymManagement_API.Models.Entities;

public partial class Payment
{
    public int PaymentId { get; set; }

    public string CardNumber { get; set; } = null!;

    public string CardName { get; set; } = null!;

    public string? TransactionName { get; set; }

    public int Amount { get; set; }

    public DateTime? TransactionDate { get; set; }

    public DateOnly ExpiryDate { get; set; }

    public virtual MemberPackage PaymentNavigation { get; set; } = null!;
}
