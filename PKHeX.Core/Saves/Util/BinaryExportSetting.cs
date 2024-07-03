using System;

namespace PKHeX.Core;

[Flags]
public enum BinaryExportSetting
{
    None,
    ExcludeFooter = 1 << 0,
    ExcludeHeader = 1 << 1,
    ExcludeFinalize = 1 << 2,
}
