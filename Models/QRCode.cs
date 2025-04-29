using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class QRScan
    {
        public string Id { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
    public string StoryId { get; set; }
    public string Url { get; set; }
    public int Scans { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}