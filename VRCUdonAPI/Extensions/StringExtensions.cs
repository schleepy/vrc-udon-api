using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCUdonAPI.Extensions
{
    public static class StringExtensions
    {
        public static string ToBinary(this string input)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in input.ToCharArray())
            {
                sb.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            return sb.ToString();
        }
    }
}
