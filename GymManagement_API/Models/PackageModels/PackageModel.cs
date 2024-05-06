using System.ComponentModel.DataAnnotations;

namespace GymManagement.Models.PackageModel;

public class PackageModel
{
    public int PackageId { get; set; }

    [Required]
    [Display(Name = "Package Name")]
    public string PackageName { get; set; }

    [Required]
    [Display(Name = "Price")]
    [DataType(DataType.Currency)]
    public decimal? Cost { get; set; }

    [Display(Name = "Duration")]
    public string? Duration { get; set; }

    [Display(Name = "Discount")]
    public decimal? Discount { get; set; } = 0;

    [Display(Name = "Create Date")]
    [DataType(DataType.Date)]
    public DateTime? CreateDate { get; set; }
}
