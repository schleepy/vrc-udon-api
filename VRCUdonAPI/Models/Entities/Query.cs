using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace VRCUdonAPI.Models.Entities
{
    public class Query : Identity
    {
        public IPAddress Address { get; set; }

        public int Calls { get; set; } = 0;

        public string Result { get; set; }
    }
}
