using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dto
{
    
        public class WeddingCreateDto
{
    public string CoupleName { get; set; }
    public string PlannerId { get; set; } // Required planner ID
    public string HowWeMet { get; set; }
    public string Proposal { get; set; }
    public string ThankYouMessage { get; set; }
    public IFormFile[] MediaFiles { get; set; }

    }
}