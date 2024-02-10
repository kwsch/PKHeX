using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.PIDType;

namespace PKHeX.Core;

/// <summary>
/// High-level wrappers for the Golden Era of RNG manipulation.
/// </summary>
public static class LeadFinder
{
    /// <inheritdoc cref="GetLeadInfo4{TEnc,TEvo}"/>
    public static LeadSeed GetLeadInfo3<TEnc, TEvo>(TEnc enc, in PIDIV pv, TEvo evo, bool emerald, int gender, int format)
        where TEnc : IEncounterSlot3
        where TEvo : ILevelRange
    {
        var type = pv.Type;
        if (type.IsClassicMethod())
            return MethodH.GetSeed(enc, pv.OriginSeed, evo, emerald, gender, (byte)format);
        return LeadSeed.Invalid;
    }

    /// <summary>
    /// Gets the lead information for the given <see cref="PKM"/> and <see cref="PIDIV"/>.
    /// </summary>
    /// <param name="pk">Entity to check</param>
    /// <param name="enc">Encounter slot</param>
    /// <param name="pv">PID/IV information</param>
    /// <param name="evo">Level range</param>
    /// <returns>If found, origin seed and lead conditions.</returns>
    public static LeadSeed GetLeadInfo4<TEnc, TEvo>(PKM pk, TEnc enc, in PIDIV pv, TEvo evo)
        where TEnc : IEncounterSlot4
        where TEvo : ILevelRange
    {
        var type = pv.Type;
        if (type is Method_1)
        {
            if (TryGetLeadInfo4(enc, evo, pk.HGSS, pv.OriginSeed, (byte)pk.Format, out var result))
                return result;

            // There's a very-very rare chance that the PID-IV can be from Cute Charm too.
            // It may match Method 1, but since we early-return, we don't check for Cute Charm.
            // So, we check for Cute Charm here and try checking Cute Charm frames if it matches.
            if (MethodFinder.IsCuteCharm(pk, pk.EncryptionConstant))
                type = CuteCharm;
        }
        if (type is CuteCharm)
        {
            // Needs to fetch all possible seeds for IVs.
            // Kinda sucks to do this every encounter, but it's relatively rare -- still good enough perf.
            var result = TryGetMatchCuteCharm4(enc, pk, evo, out var seed);
            if (result)
                return new(seed, LeadRequired.CuteCharm);
        }
        return LeadSeed.Invalid;
    }

    /// <summary>
    /// Tries to get the lead information for a Generation 4 encounter.
    /// </summary>
    /// <returns>If found, origin seed and lead conditions.</returns>
    public static bool TryGetLeadInfo4<TEnc, TEvo>(TEnc enc, TEvo evo, bool hgss, uint seed, byte format, out LeadSeed result)
        where TEnc : IEncounterSlot4
        where TEvo : ILevelRange
    {
        result = hgss
            ? MethodK.GetSeed(enc, seed, evo, format)
            : MethodJ.GetSeed(enc, seed, evo, format);
        return result.IsValid();
    }

    private static bool TryGetMatchCuteCharm4<TEnc, TEvo>(TEnc enc, PKM pk, TEvo evo, out uint seed)
        where TEnc : IEncounterSlot4
        where TEvo : ILevelRange
    {
        // Can be one of many seeds.
        Span<uint> seeds = stackalloc uint[LCRNG.MaxCountSeedsIV];
        var ctr = GetSeedsIVs(pk, seeds);
        seeds = seeds[..ctr];

        var nature = (byte)(pk.EncryptionConstant % 25);
        byte min = evo.LevelMin, max = evo.LevelMax;
        return pk.HGSS
            ? MethodK.TryGetMatchCuteCharm(enc, seeds, nature, min, max, out seed)
            : MethodJ.TryGetMatchCuteCharm(enc, seeds, nature, min, max, out seed);
    }

    private static int GetSeedsIVs(PKM pk, Span<uint> seeds)
    {
        var hp = (uint)pk.IV_HP;
        var atk = (uint)pk.IV_ATK;
        var def = (uint)pk.IV_DEF;
        var spa = (uint)pk.IV_SPA;
        var spd = (uint)pk.IV_SPD;
        var spe = (uint)pk.IV_SPE;
        return LCRNGReversal.GetSeedsIVs(seeds, hp, atk, def, spa, spd, spe);
    }

    public static EvoCriteria GetLevelConstraint<TEnc>(PKM pk, ReadOnlySpan<EvoCriteria> chain, TEnc enc, [ConstantExpected] int generation)
        where TEnc : IEncounterSlot34, ISpeciesForm
    {
        if (pk.Format == generation)
            return new EvoCriteria { Species = enc.Species, LevelMin = (byte)pk.Met_Level, LevelMax = (byte)pk.Met_Level };
        foreach (var evo in chain)
        {
            if (evo.Species == enc.Species)
                return evo;
        }
        throw new ArgumentException("No matching species found in the evolution chain.");
    }
}
