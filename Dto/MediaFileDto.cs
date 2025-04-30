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
}