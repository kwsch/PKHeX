using System.Runtime.InteropServices;

namespace PKHeX.Core;

/// <summary>
/// Stores details about a <see cref="PKM.EncryptionConstant"/> (PID) and any associated details being traced to a known correlation.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 10)]
public readonly struct PIDIV
{
    internal static readonly PIDIV None = new();
    internal static readonly PIDIV CuteCharm = new(PIDType.CuteCharm); // can be one of many seeds!
    internal static readonly PIDIV Pokewalker = new(PIDType.Pokewalker);
    internal static readonly PIDIV G5MGShiny = new(PIDType.G5MGShiny);

    /// <summary>The RNG seed which immediately generates the PIDIV (starting with PID or IVs, whichever comes first)</summary>
    [field: FieldOffset(0)]
    public uint OriginSeed { get; }
    /// <summary>The RNG seed which starts the encounter generation routine.</summary>
    [field: FieldOffset(4)]
    public uint EncounterSeed { get; init; }

    /// <summary>The RNG seed which immediately generates the PIDIV (starting with PID or IVs, whichever comes first)</summary>
    [field: FieldOffset(0)]
    public ulong Seed64 { get; }

    /// <summary>Type of PIDIV correlation</summary>
    [field: FieldOffset(8)]
    public PIDType Type { get; }

    [field: FieldOffset(9)]
    public LeadRequired Lead { get; init; }

    public PIDIV(PIDType type, uint seed = 0)
    {
        Type = type;
        OriginSeed = seed;
    }

    public PIDIV(PIDType type, ulong seed)
    {
        Type = type;
        Seed64 = seed;
    }

    /// <remarks> Some PIDIVs may be generated without a single seed, but may follow a traceable pattern. </remarks>
    /// <summary> Indicates that there is no <see cref="OriginSeed"/> to refer to. </summary>
    public bool NoSeed => Type is PIDType.None or PIDType.Pokewalker or PIDType.G5MGShiny;

#if DEBUG
    public override string ToString() => NoSeed ? Type.ToString() : $"{Type} - 0x{OriginSeed:X8}";
#endif
    public bool IsClassicMethod() => Type.IsClassicMethod();

    public bool IsSeed64() => Type is PIDType.Xoroshiro;

    public PIDIV AsEncounteredVia(LeadSeed condition) => this with { Lead = condition.Lead, EncounterSeed = condition.Seed };
}
