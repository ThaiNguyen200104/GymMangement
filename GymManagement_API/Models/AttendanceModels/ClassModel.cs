namespace GymManagement.Models.AttendanceModels;

public class ClassModel
{
    public int ClassId { get; set; }
    public int? TrainerId { get; set; }
    public int? MemberId { get; set; }
    public string? ClassName { get; set; }
}
