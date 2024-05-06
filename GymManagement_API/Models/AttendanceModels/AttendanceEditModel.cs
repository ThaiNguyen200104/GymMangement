using System.ComponentModel.DataAnnotations;

namespace GymManagement.Models.AttendanceModels;

public class AttendanceEditModel
{
	public int AttendanceId { get; set; }

	[Required]
	public int? MemberId { get; set; }

	[Required]
	public int? ClassId { get; set; }

	[Required]
	public DateTime AttendanceDate { get; set; }

	public string MemberName { get; set; }

	public string ClassName { get; set; }

}
