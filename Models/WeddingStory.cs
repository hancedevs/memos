using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class WeddingStory
    {
    public Guid Id { get; set; } = Guid.NewGuid();
   public string CoupleName { get; set; }
    public string PlannerId { get; set; } // Foreign key to Planner
    public string HowWeMet { get; set; }
    public string Proposal { get; set; }
    public string ThankYouMessage { get; set; }
    public Planner Planner { get; set; }
    public List<Media> Gallery { get; set; }
    public WQRCode QRCode { get; set; } // One-to-one with QRCode

    }
}