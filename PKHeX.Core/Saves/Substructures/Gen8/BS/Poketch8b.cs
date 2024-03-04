using System;
using System.ComponentModel;

namespace PKHeX.Core;

/// <summary>
/// Details about the Poketch corner app.
/// </summary>
/// <remarks>size: 0x19C</remarks>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class Poketch8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw)
{
    public const int APP_REGIST_MAX = 20; // bool array unlock flags
    public const int POKETCH_MAP_MARK_MAX = 6; // mark_map_pos[6]
    public const int POKETCH_DOTART_DATA_BYTESIZE = 192; // dotart data bytes
    public const int POKETCH_POKE_HISTORY_COUNT_MAX = 12; // poke_history[12]
    public const int POKETCH_PEDOMETER_MAX = 99999;
    public const int POKETCH_CALENDER_MONTH_MAX = 12; // calendar markbit uint[12]

    public const int SIZE = 0x19C;
}

public enum PoketchApp8b
{
    DWATCH = 0,
    CALC = 1,
    MEMO = 2,
    PEDOMETER = 3,
    POKELIST = 4,
    NATSUKI_CHECK = 5,
    DOWSING = 6,
    SODATEYA_CAMERA = 7,
    POKEMON_HISTORY = 8,
    COUNTER = 9,
    AWATCH = 10,
    MAP_MARKING = 11,
    COINTOSS = 12,
    CALENDER = 13,
    DOTART = 14,
    ROULETTE = 15,
    POKEMON_COUNTER = 16,
    KITCHEN_TIMER = 17,
    COLOR_CHANGER = 18,
    HIDEN_WAZA = 19,
}
