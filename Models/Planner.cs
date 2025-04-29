using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class Planner
    {
        public string Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
     public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
    }
}