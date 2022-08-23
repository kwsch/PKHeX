using System;

namespace PKHeX.Core;

/// <summary>
/// Stores an IV template.
/// </summary>
/// <param name="HP "><see cref="PKM.IV_HP "/></param>
/// <param name="ATK"><see cref="PKM.IV_ATK"/></param>
/// <param name="DEF"><see cref="PKM.IV_DEF"/></param>
/// <param name="SPE"><see cref="PKM.IV_SPE"/></param>
/// <param name="SPA"><see cref="PKM.IV_SPA"/></param>
/// <param name="SPD"><see cref="PKM.IV_SPD"/></param>
/// <param name="Type">Differentiate between different IV templates, or lack thereof (0).</param>
public readonly record struct IndividualValueSet(sbyte HP, sbyte ATK, sbyte DEF, sbyte SPE, sbyte SPA, sbyte SPD, byte Type = 1)
{
    // 8 BYTES MAX STRUCTURE

    // Default struct will be zero type.
    public bool IsSpecified => Type != 0;

    public sbyte this[int index] => index switch
    {
        0 => HP,
        1 => ATK,
        2 => DEF,
        3 => SPE,
        4 => SPA,
        5 => SPD,
        _ => throw new ArgumentOutOfRangeException(nameof(index), index, null),
    };

    public void CopyToSpeedLast(Span<int> span)
    {
        span[5] = SPE;
        span[4] = SPD;
        span[3] = SPA;
        span[2] = DEF;
        span[1] = ATK;
        span[0] = HP;
    }
}
