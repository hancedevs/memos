using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dto
{
    public class MediaFileDto
    {
        public Guid WeddingId { get; set; }
        public IFormFile File { get; set; }
    }
    public class MediaFileResponseDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Type { get; set; } // "image" or "video"
        public bool IsCoverImage { get; set; } = false;
    }
}