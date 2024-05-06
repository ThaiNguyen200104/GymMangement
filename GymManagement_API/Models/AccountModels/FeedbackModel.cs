namespace GymManagement.Models.AccountModels;

public class FeedbackModel
{
    public int FeedbackId { get; set; }
    public int? TrainerId { get; set; }
    public string? Feedback1 { get; set; }
    public DateTime FeedbackDate { get; set; }
}
