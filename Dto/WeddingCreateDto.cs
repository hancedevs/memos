using backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dto
{
    
        public class WeddingCreateDto
{
        public string BrideName { get; set; }
        public string GroomName { get; set; }
        public string BrideVows { get; set; }
        public string HowWeMet { get; set; }
        public string GroomVows { get; set; }
        public string Proposal { get; set; }
        public string ThankYouMessage { get; set; }
        //public IFormFile[] Gallery { get; set; }
        //public IFormFile CoverImage { get; set; }

    }
    public class WeddingResponseDto
    {
        public Guid Id { get; set; }
        public string BrideName { get; set; }
        public string GroomName { get; set; }
        public string BrideVows { get; set; }
        public string HowWeMet { get; set; }
        public string GroomVows { get; set; }
        public string Proposal { get; set; }
        public string ThankYouMessage { get; set; }
        public List<MediaFileResponseDto> Gallery { get; set; } = new List<MediaFileResponseDto>();
        public MediaFileResponseDto CoverImage { get; set; } = new MediaFileResponseDto();
        public WQRCodeResponse QrCode { get; set; } = new WQRCodeResponse();
    }
    public class WQRCodeResponse
    {
        public Guid Id { get; set; }
        public Guid WeddingId { get; set; }
        public string Url { get; set; }
        public string AssetUrl { get; set; }

        public int Scans { get; set; }
    }
    public class WeddingUpdateDto
    {
        public Guid Id { get; set; }
        public string BrideName { get; set; }
        public string GroomName { get; set; }
        public string BrideVows { get; set; }
        public string HowWeMet { get; set; }
        public string GroomVows { get; set; }
        public string Proposal { get; set; }
        public string ThankYouMessage { get; set; }
    }

}