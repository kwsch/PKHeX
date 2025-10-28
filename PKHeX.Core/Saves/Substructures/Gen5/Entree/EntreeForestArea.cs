using System;

namespace PKHeX.Core;

[Flags]
public enum EntreeForestArea
{
    None,
    Deepest = 1 << 0,
    First =   1 << 1,
    Second =  1 << 2,
    Third =   1 << 3,
    Fourth =  1 << 4,
    Fifth =   1 << 5,
    Sixth =   1 << 6,
    Seventh = 1 << 7,
    Eighth =  1 << 8,
    Ninth =   1 << 9,

    Left =    1 << 10,
    Right =   1 << 11,
    Center =  1 << 12,
}
