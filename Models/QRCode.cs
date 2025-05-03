using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class WQRCode
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
     public Guid WeddingId { get; set; }
    public string Url { get; set; }
    public string AssetUrl { get; set; }
    public virtual WeddingStory Wedding { get; set; }
    public int Scans { get; set; }
    }
}