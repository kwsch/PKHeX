using System;

namespace PKHeX.Core
{
    [Flags]
    public enum ExportFlags
    {
        None,
        IncludeFooter = 1 << 0,
        IncludeHeader = 1 << 1,
    }

    public static class ExportFlagsExtensions
    {
        public static bool HasFlagFast(this ExportFlags value, ExportFlags flag) => (value & flag) != 0;
    }
}