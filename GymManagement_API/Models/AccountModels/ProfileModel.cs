namespace GymManagement.Models.AccountModels;

public class ProfileModel
{
	public string UserName { get; set; }
	public string Email { get; set; }
	public string Firstname { get; set; }
	public string Lastname { get; set; }
	public string PhoneNumber { get; set; }
	public string UserImg { get; set; }
	public string PackageName { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
	public List<MemberPackage> memPack {  get; set; } = new List<MemberPackage>();
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
	public List<MemberPackage> memPack { get; set; } = new List<MemberPackage>();

	public class MemberPackage
	{
		public int? PackageId { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}
}
