namespace GymManagement.Models.PackageModels;

public class MemberPackageModel
{
    public int Id { get; set; }
    public int? PackageId { get; set; }
    public int? MemberId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
