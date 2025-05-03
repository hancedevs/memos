using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class Media
{
        [Key]
        public Guid Id { get; set; }=Guid.NewGuid();
    public Guid WeddingId { get; set; }
    public string Url { get; set; }
    public string Type { get; set; } // "image" or "video"
        public Boolean IsCoverImage { get; set; } = false;
    public virtual WeddingStory Wedding { get; set; }
}
}