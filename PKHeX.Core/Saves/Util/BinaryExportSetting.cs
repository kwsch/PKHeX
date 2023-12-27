using System;

namespace PKHeX.Core;

[Flags]
public enum BinaryExportSetting
{
    None,
    IncludeFooter = 1 << 0,
    IncludeHeader = 1 << 1,
}
