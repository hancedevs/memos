using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace backend.Models
{
    public class WeddingStory
    {
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    public string CoupleId { get; set; }
    public string PlannerId { get; set; }
    public string Title { get; set; }
    public Dictionary<string, string> Sections { get; set; }
    public List<Media> Media { get; set; }
    public string QRCodeId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}