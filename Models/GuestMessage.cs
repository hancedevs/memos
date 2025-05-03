using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class GuestMessage
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid WeddingId { get; set; }
        public string Message { get; set; }
        public string SenderName { get; set; }
        public string RelationToCouple { get; set; }

        public virtual WeddingStory Wedding { get; set; } // Navigation property to WeddingStory
    }
}
