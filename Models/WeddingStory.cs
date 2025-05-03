using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class WeddingStory
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
   public string BrideName { get; set; }
   public string GroomName { get; set; }
        public string BrideVows { get; set; }
        public string HowWeMet { get; set; }
        public string GroomVows { get; set; }
        public string Proposal { get; set; }
    public string ThankYouMessage { get; set; }
    public virtual List<Media> Gallery { get; set; }
        public virtual  List<GuestMessage> GuestMessages { get; set; }
        public WQRCode QRCode { get; set; } // One-to-one with QRCode

    }
}