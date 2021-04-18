using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace VRCUdonAPI.Models.Dtos
{
    public class QueryDto
    {
        public IPAddress Address { get; set; }

        public int Calls { get; set; }

        public string Result { get; set; }
    }
}
