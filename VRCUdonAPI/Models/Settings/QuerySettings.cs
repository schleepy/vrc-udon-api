using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VRCUdonAPI.Models.Settings
{
    public class QuerySettings
    {
        public string EOQMarker { get; set; } = "]"; // End of query marker
        public int RemovalThresholdInSeconds { get; set; } = 30; // How old has the query to be to have it removed?
    }
}
