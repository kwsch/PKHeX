using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

public sealed class EncounterGenerator4 : IEncounterGenerator
{
    public static readonly EncounterGenerator4 Instance = new();
    public bool CanGenerateEggs => true;

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

    private enum DeferralType
    {
        None,
        PIDIV,
        Tile,
        Ball,
        SlotNumber,
    }

    private struct Deferral
    {
        public DeferralType Type;
        public IEncounterable? Encounter;

        public void Update(DeferralType type, IEncounterable enc)
        {
            if (Type >= type)
                return;
            Type = type;
            Encounter = enc;
        }
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        info.PIDIV = MethodFinder.Analyze(pk);
        var game = pk.Version;
        var iterator = new EncounterEnumerator4(pk, chain, game);
        Deferral defer = default;
        var leadQueue = new LeadEncounterQueue<EncounterSlot4>();

        foreach (var enc in iterator)
        {
            var e = enc.Encounter;
            if (!IsTileCompatible(e, pk))
            {
                defer.Update(DeferralType.Tile, e);
                continue;
            }
            if (!IsTypeCompatible(e, pk, info.PIDIV.Type))
            {
                defer.Update(DeferralType.PIDIV, e);
                continue;
            }
            if (!IsBallCompatible(e, pk))
            {
                defer.Update(DeferralType.Ball, e);
                continue;
            }
            if (e is not EncounterSlot4 slot)
            {
                if (pk.Ball is (byte)Ball.Safari or (byte)Ball.Sport)
                    defer.Update(DeferralType.Ball, e);
                else
                    yield return e;
                continue;
            }

            var evo = LeadFinder.GetLevelConstraint(pk, chain, slot, 4);
            var lead = LeadFinder.GetLeadInfo4(pk, slot, info.PIDIV, evo);
            if (!lead.IsValid())
            {
                defer.Update(DeferralType.SlotNumber, slot);
                continue;
            }
            leadQueue.Insert(lead, slot);
        }

        foreach (var cache in leadQueue.List)
        {
            info.PIDIV = info.PIDIV.AsEncounteredVia(cache.Lead);
            yield return cache.Encounter;
        }
        if (leadQueue.List.Count != 0)
            yield break;

        // Errors will be flagged later for those not manually handled below.
        if (defer.Encounter is not { } lastResort)
            yield break;
        if (defer.Type is DeferralType.PIDIV)
            info.ManualFlag = EncounterYieldFlag.InvalidPIDIV;
        else if (defer.Type is DeferralType.SlotNumber)
            info.ManualFlag = EncounterYieldFlag.InvalidFrame;
        yield return lastResort;
    }

    private static bool IsBallCompatible(IFixedBall e, PKM pk) => e.FixedBall switch
    {
        Ball.Safari when pk.Ball is (byte)Ball.Safari => true,
        Ball.Sport when pk.Ball is (byte)Ball.Sport => true,
        _ => pk.Ball is not ((byte)Ball.Safari or (byte)Ball.Sport),
    };

    private static bool IsTileCompatible(IEncounterTemplate enc, PKM pk)
    {
        if (pk is not IGroundTile e)
            return true; // No longer has the data to check
        if (enc is not IGroundTypeTile t)
            return e.GroundTile == 0;
        return t.GroundTile.Contains(e.GroundTile);
    }

    private static bool IsTypeCompatible(IEncounterTemplate enc, PKM pk, PIDType type)
    {
        if (enc is IRandomCorrelation r)
            return r.IsCompatible(type, pk);
        return type == PIDType.None;
    }

    private const byte Generation = 4;
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
