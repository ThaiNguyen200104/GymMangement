using System;
using System.Collections.Generic;

namespace GymManagement_API.Models.Entities;

public partial class Image
{
    public int ImagesId { get; set; }

    public int? TrainerId { get; set; }

    public string ImagesName { get; set; } = null!;

    public virtual Trainer? Trainer { get; set; }
}
