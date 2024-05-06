namespace GymManagement.Models.PackageModels;

public class PaymentModel
{
    public int PaymentId { get; set; }

    public string CardNumber { get; set; } = null!;

    public string CardName { get; set; } = null!;

    public string? TransactionName { get; set; }

    public int Amount { get; set; } = 0;

    public DateTime? TransactionDate { get; set; }

    public DateTime ExpiryDate { get; set; }
}
