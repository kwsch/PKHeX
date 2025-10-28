using System;

namespace PKHeX.Core;

/// <summary>
/// Stores an IV template.
/// </summary>
/// <param name="HP "><see cref="PKM.IV_HP "/>; -1 indicates "random".</param>
/// <param name="ATK"><see cref="PKM.IV_ATK"/>; -1 indicates "random".</param>
/// <param name="DEF"><see cref="PKM.IV_DEF"/>; -1 indicates "random".</param>
/// <param name="SPE"><see cref="PKM.IV_SPE"/>; -1 indicates "random".</param>
/// <param name="SPA"><see cref="PKM.IV_SPA"/>; -1 indicates "random".</param>
/// <param name="SPD"><see cref="PKM.IV_SPD"/>; -1 indicates "random".</param>
/// <param name="Type">Differentiate between different IV templates, or lack thereof.</param>
public readonly record struct IndividualValueSet(sbyte HP, sbyte ATK, sbyte DEF, sbyte SPE, sbyte SPA, sbyte SPD, IndividualValueSetType Type = IndividualValueSetType.Specified)
{
    // 8 BYTES MAX STRUCTURE

    /// <summary>
    /// Indicates if this has a value or not.
    /// </summary>
    /// <remarks>
    /// The default value of this struct will be all zero with <see cref="IndividualValueSetType"/> of 0.
    /// </remarks>
    public bool IsSpecified => Type != IndividualValueSetType.Unspecified;

    /// <summary>
    /// Gets the IV at the requested <see cref="index"/>.
    /// </summary>
    /// <param name="index">Internal ordered index of the IV to get.</param>
    /// <remarks>Speed is at index 3, not 5.</remarks>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
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

    public uint GetIV32() => (uint)(HP & 0x1F) | ((uint)(ATK & 0x1F) << 5) | ((uint)(DEF & 0x1F) << 10) | ((uint)(SPE & 0x1F) << 15) | ((uint)(SPA & 0x1F) << 20) | ((uint)(SPD & 0x1F) << 25);
}

/// <summary>
/// Used by <see cref="IndividualValueSet"/> to indicate if a value is present.
/// </summary>
public enum IndividualValueSetType : byte
{
    Unspecified = 0,
    Specified = 1,
}
