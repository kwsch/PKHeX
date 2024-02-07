using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

public sealed class EncounterGenerator4 : IEncounterGenerator
{
    public static readonly EncounterGenerator4 Instance = new();

    // Utility
    internal static readonly PGT RangerManaphy = new() { Data = { [0] = 7, [8] = 1 } };

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info)
    {
        var chain = EncounterOrigin.GetOriginChain(pk, 4);
        if (chain.Length == 0)
            return [];
        return GetEncounters(pk, chain, info);
    }

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        var iterator = new EncounterPossible4(chain, groups, game, pk);
        foreach (var enc in iterator)
            yield return enc;
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        info.PIDIV = MethodFinder.Analyze(pk);
        var deferredPIDIV = new List<IEncounterable>();
        var deferredEType = new List<IEncounterable>();

        foreach (var z in GetEncountersInner(pk, chain, info))
        {
            if (!IsTypeCompatible(z, pk, info.PIDIV.Type))
                deferredPIDIV.Add(z);
            else if (!IsTileCompatible(z, pk))
                deferredEType.Add(z);
            else
                yield return z;
        }

        static bool IsTileCompatible(IEncounterable enc, PKM pk)
        {
            if (pk is not IGroundTile e)
                return true; // No longer has the data to check
            if (enc is not IGroundTypeTile t)
                return e.GroundTile == 0;
            return t.GroundTile.Contains(e.GroundTile);
        }

        static bool IsTypeCompatible(IEncounterTemplate enc, PKM pk, PIDType type)
        {
            if (enc is IRandomCorrelation r)
                return r.IsCompatible(type, pk);
            return type == PIDType.None;
        }

        foreach (var z in deferredEType)
            yield return z;

        if (deferredPIDIV.Count == 0)
            yield break;

        info.PIDIVMatches = false;
        foreach (var z in deferredPIDIV)
            yield return z;
    }

    private static IEnumerable<IEncounterable> GetEncountersInner(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        var game = (GameVersion)pk.Version;
        var iterator = new EncounterEnumerator4(pk, chain, game);
        EncounterSlot4? deferSlot = null;
        foreach (var enc in iterator)
        {
            var e = enc.Encounter;
            if (e is not EncounterSlot4 s4)
            {
                yield return e;
                continue;
            }

            var evo = GetLevelConstraint(pk, chain, s4);
            var leadInfo = GetLeadInfo(pk, s4, info.PIDIV, evo);
            if (leadInfo.Lead != LeadRequired.Fail)
                yield return s4;
            deferSlot ??= s4;
        }
        if (deferSlot != null)
            yield return deferSlot;
    }

    private static (uint Seed, LeadRequired Lead) GetLeadInfo(PKM pk, EncounterSlot4 s4, in PIDIV pv, in EvoCriteria evo)
    {
        var type = pv.Type;
        if (type is PIDType.Method_1)
        {
            var seed = pv.OriginSeed;
            var result = pk.HGSS
                ? MethodK.GetSeed(s4, seed, evo)
                : MethodJ.GetSeed(s4, seed, evo);
            if (result.Lead != LeadRequired.Fail)
                return result;

            // There's a very-very rare chance that the PID-IV can be from Cute Charm too.
            // It may match Method 1, but since we early-return, we don't check for Cute Charm.
            // So, we check for Cute Charm here and try checking Cute Charm frames if it matches.
            if (MethodFinder.IsCuteCharm(pk, pk.EncryptionConstant))
                type = PIDType.CuteCharm;
        }
        if (type is PIDType.CuteCharm)
        {
            // Needs to fetch all possible seeds for IVs.
            // Kinda sucks to do this every encounter, but it's relatively rare -- still good enough perf.
            var result = TryGetMatchCuteCharm(s4, pk, evo, out var seed);
            if (result)
                return (seed, LeadRequired.CuteCharm);
        }
        return (default, LeadRequired.Fail);
    }

    private static bool TryGetMatchCuteCharm(EncounterSlot4 s4, PKM pk, in EvoCriteria evo, out uint seed)
    {
        // Can be one of many seeds.
        Span<uint> seeds = stackalloc uint[LCRNG.MaxCountSeedsIV];
        int ctr = LCRNGReversal.GetSeedsIVs(seeds, (uint)pk.IV_HP, (uint)pk.IV_ATK, (uint)pk.IV_DEF, (uint)pk.IV_SPA, (uint)pk.IV_SPD, (uint)pk.IV_SPE);
        seeds = seeds[..ctr];

        var nature = (byte)(pk.EncryptionConstant % 25);
        byte min = evo.LevelMin, max = evo.LevelMax;
        bool result = pk.HGSS
            ? MethodK.TryGetMatchCuteCharm(s4, seeds, nature, min, max, out seed)
            : MethodJ.TryGetMatchCuteCharm(s4, seeds, nature, min, max, out seed);
        return result;
    }

    private static EvoCriteria GetLevelConstraint(PKM pk, EvoCriteria[] chain, EncounterSlot4 s4)
    {
        if (pk.Format == 4)
            return new EvoCriteria { Species = s4.Species, LevelMin = (byte)pk.Met_Level, LevelMax = (byte)pk.Met_Level };
        return chain.First(x => x.Species == s4.Species);
    }

    private const int Generation = 4;
    private const EntityContext Context = EntityContext.Gen4;
    private const byte EggLevel = 1;

    private static EncounterEgg CreateEggEncounter(ushort species, byte form, GameVersion version)
    {
        if (FormInfo.IsBattleOnlyForm(species, form, Generation) || species is (int)Species.Rotom or (int)Species.Castform)
            form = FormInfo.GetOutOfBattleForm(species, form, Generation);
        return new EncounterEgg(species, form, EggLevel, Generation, version, Context);
    }

    private static (ushort Species, byte Form) GetBaby(EvoCriteria lowest)
    {
        return EvolutionTree.Evolves4.GetBaseSpeciesForm(lowest.Species, lowest.Form);
    }

    public static bool TryGetEgg(ReadOnlySpan<EvoCriteria> chain, GameVersion version, [NotNullWhen(true)] out EncounterEgg? result)
    {
        result = null;
        var devolved = chain[^1];
        if (!devolved.InsideLevelRange(EggLevel))
            return false;

        // Ensure most devolved species is the same as the egg species.
        var (species, form) = GetBaby(devolved);
        if (species != devolved.Species && !Breeding.IsSplitBreedNotBabySpecies4(devolved.Species))
            return false; // not a split-breed.

        // Sanity Check 1
        if (!Breeding.CanHatchAsEgg(species))
            return false;
        // Sanity Check 2
        if (!Breeding.CanHatchAsEgg(species, form, Context))
            return false;
        // Sanity Check 3
        if (!PersonalTable.HGSS.IsPresentInGame(species, form))
            return false;

        result = CreateEggEncounter(species, form, version);
        return true;
    }

    // Version is not updated when hatching an Egg in Gen4. Version is a clear indicator of the game it originated on.

    public static bool TryGetSplit(EncounterEgg other, ReadOnlySpan<EvoCriteria> chain, [NotNullWhen(true)] out EncounterEgg? result)
    {
        result = null;
        // Check for split-breed
        var devolved = chain[^1];
        if (other.Species == devolved.Species)
        {
            if (chain.Length < 2)
                return false; // no split-breed
            devolved = chain[^2];
        }
        if (!Breeding.IsSplitBreedNotBabySpecies4(devolved.Species))
            return false;

        result = other with { Species = devolved.Species, Form = devolved.Form };
        return true;
    }
}
