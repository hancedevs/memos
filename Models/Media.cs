using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class Media
{
    public Guid Id { get; set; }
    public Guid WeddingId { get; set; }
    public string Url { get; set; }
    public string Type { get; set; } // "image" or "video"
    public WeddingStory Wedding { get; set; }
}
}