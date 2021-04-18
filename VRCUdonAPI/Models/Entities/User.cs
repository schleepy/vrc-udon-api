using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VRCUdonAPI.Models.Entities
{
    public class User : Identity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
