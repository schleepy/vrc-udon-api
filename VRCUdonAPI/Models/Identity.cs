using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VRCUdonAPI.Models
{
    public class Identity
    {
        public DateTime WhenCreated { get; set; } = DateTime.UtcNow;
        public DateTime WhenUpdated { get; set; } = DateTime.UtcNow;
    }
}
