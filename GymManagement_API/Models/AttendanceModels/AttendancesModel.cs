using System.ComponentModel.DataAnnotations;

namespace GymManagement.Models.AttendanceModels;

public class AttendancesModel
{
	public int AttendanceId { get; set; }

	[Required]
	public int? MemberId { get; set; }

	[Required]
	public int? ClassId { get; set; }

	[Required]
	public DateTime AttendanceDate { get; set; }
}
