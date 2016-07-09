using System;
using System.Linq;

namespace PKHeX
{
    public partial class Util
    {
        internal static int ToInt32(string value)
        {
            string val = value?.Replace(" ", "").Replace("_", "").Trim();
            return String.IsNullOrWhiteSpace(val) ? 0 : Int32.Parse(val);
        }

        internal static uint ToUInt32(string value)
        {
            string val = value?.Replace(" ", "").Replace("_", "").Trim();
            return String.IsNullOrWhiteSpace(val) ? 0 : UInt32.Parse(val);
        }

        internal static uint getHEXval(string s)
        {
            string str = getOnlyHex(s);
            return String.IsNullOrWhiteSpace(str) ? 0 : Convert.ToUInt32(str, 16);
        }

        internal static string getOnlyHex(string s)
        {
            return String.IsNullOrWhiteSpace(s) ? "0" : s.Select(Char.ToUpper).Where("0123456789ABCDEF".Contains).Aggregate("", (str, c) => str + c);
        }
    }
}
