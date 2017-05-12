using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PKHeX.Core
{
    public partial class Util
    {
        public static int ToInt32(string value)
        {
            string val = value?.Replace(" ", "").Replace("_", "").Trim();
            return string.IsNullOrWhiteSpace(val) ? 0 : int.Parse(val);
        }

        public static uint ToUInt32(string value)
        {
            string val = value?.Replace(" ", "").Replace("_", "").Trim();
            return string.IsNullOrWhiteSpace(val) ? 0 : uint.Parse(val);
        }

        public static uint getHEXval(string s)
        {
            string str = getOnlyHex(s);
            return string.IsNullOrWhiteSpace(str) ? 0 : Convert.ToUInt32(str, 16);
        }

        public static string getOnlyHex(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? "0" : s.Select(char.ToUpper).Where("0123456789ABCDEF".Contains).Aggregate("", (str, c) => str + c);
        }

        public static string toTitleCase(string s)
        {
            return string.Join(" ", s.Split(' ').Select(x => x[0].ToString().ToUpper() + x.Substring(1, x.Length - 1).ToLower()));
        }
    }
}
