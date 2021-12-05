namespace PKHeX.Core;

/// <summary>
/// Stores details about a <see cref="PKM.EncryptionConstant"/> (PID) and any associated details being traced to a known correlation.
/// </summary>
/// <param name="Type">Type of PIDIV correlation</param>
/// <param name="OriginSeed">The RNG seed which immediately generates the PIDIV (starting with PID or IVs, whichever comes first)</param>
public readonly record struct PIDIV(PIDType Type, uint OriginSeed = 0)
{
    internal static readonly PIDIV None = new();
    internal static readonly PIDIV CuteCharm = new(PIDType.CuteCharm);
    internal static readonly PIDIV Pokewalker = new(PIDType.Pokewalker);
    internal static readonly PIDIV G5MGShiny = new(PIDType.G5MGShiny);

    /// <summary> Indicates that there is no <see cref="OriginSeed"/> to refer to. </summary>
    /// <remarks> Some PIDIVs may be generated without a single seed, but may follow a traceable pattern. </remarks>
    public bool NoSeed => Type is PIDType.None or PIDType.CuteCharm or PIDType.Pokewalker or PIDType.G5MGShiny;

    /// <summary> Type of PIDIV correlation </summary>

#if DEBUG
    public override string ToString() => NoSeed ? Type.ToString() : $"{Type} - 0x{OriginSeed:X8}";
#endif
}
