using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.RandomCorrelationRating;

namespace PKHeX.Core;

public sealed class EncounterGenerator3 : IEncounterGenerator
{
    public static readonly EncounterGenerator3 Instance = new();
    public bool CanGenerateEggs => true;

    public IEnumerable<IEncounterable> GetPossible(PKM _, EvoCriteria[] chain, GameVersion version, EncounterTypeGroup groups)
    {
        var iterator = new EncounterPossible3(chain, groups, version);
        foreach (var enc in iterator)
            yield return enc;
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info)
    {
        var chain = EncounterOrigin.GetOriginChain(pk, 3, Context);
        return GetEncounters(pk, chain, info);
    }

    private enum DeferralType
    {
        // Legal
        None,
        PIDIVDefer,
        SlotNumber,

        // Illegal
        Ball,
        PIDIV,
    }

    private struct Deferral
    {
        public DeferralType Type;
        public IEncounterable? Encounter;

        public void Update(DeferralType type, IEncounterable enc)
        {
            if (type >= Type && Encounter is not null)
                return;
            Type = type;
            Encounter = enc;
        }
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        if (chain.Length == 0)
            yield break;

        info.PIDIV = MethodFinder.Analyze(pk);
        var game = pk.Version;
        var iterator = new EncounterEnumerator3(pk, chain, game);
        Deferral defer = default;
        var leadQueue = new LeadEncounterQueue<EncounterSlot3>();

        bool emerald = pk.E;
        byte gender = pk.Gender;
        if (pk.Species is (int)Species.Marill or (int)Species.Azumarill)
            gender = EntityGender.GetFromPID(pk.EncryptionConstant, EntityGender.MM);

        foreach (var enc in iterator)
        {
            var e = enc.Encounter;
            var typeCheck = IsTypeCompatible(e, pk, ref info.GetPIDIVRef());
            if (typeCheck is not Match)
            {
                var rating = typeCheck == NotIdeal
                    ? DeferralType.PIDIVDefer
                    : DeferralType.PIDIV;
                defer.Update(rating, e);
                continue;
            }
            if (!IsBallCompatible(e, pk))
            {
                defer.Update(DeferralType.Ball, e);
                continue;
            }
            if (e is not EncounterSlot3 slot)
            {
                yield return e;
                continue;
            }

            var evo = LeadFinder.GetLevelConstraint(pk, chain, slot, 3);
            var lead = LeadFinder.GetLeadInfo3(slot, info.PIDIV, evo, emerald, gender, pk.Format);
            if (!lead.IsValid)
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
        // vBlanks lead to imperfect lead analysis; just yield whatever else we have instead of early returning nothing.
        //if (leadQueue.List.Count != 0)
        //    yield break;

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
        _ => pk.Ball is not (byte)Ball.Safari,
    };

    private static RandomCorrelationRating IsTypeCompatible(IEncounterTemplate enc, PKM pk, ref PIDIV pidiv)
    {
        if (enc is IRandomCorrelationEvent3 revise)
            return revise.IsCompatibleReviseReset(ref pidiv, pk);
        var type = pidiv.Type;
        if (enc is IRandomCorrelation r)
            return r.IsCompatible(type, pk);
        return type is PIDType.None ? Match : Mismatch;
    }

    private const EntityContext Context = EntityContext.Gen3;
    private const byte EggLevel = EncounterEgg3.Level;

    private static EncounterEgg3 CreateEggEncounter(ushort species, GameVersion version) => new(species, version);

    private static (ushort Species, byte Form) GetBaby(EvoCriteria lowest)
    {
        return EvolutionTree.Evolves3.GetBaseSpeciesForm(lowest.Species, lowest.Form);
    }

    public static bool TryGetEgg(ReadOnlySpan<EvoCriteria> chain, GameVersion version, [NotNullWhen(true)] out EncounterEgg3? result)
    {
        result = null;
        var devolved = chain[^1];
        if (!devolved.InsideLevelRange(EggLevel))
            return false;

        // Ensure most devolved species is the same as the egg species.
        var (species, form) = GetBaby(devolved);
        if (species != devolved.Species && !Breeding.IsSplitBreedNotBabySpecies3(devolved.Species))
            return false; // not a split-breed.

        // Sanity Check 1
        if (!Breeding.CanHatchAsEgg(species))
            return false;
        // Sanity Check 2
        if (!Breeding.CanHatchAsEgg(species, form, Context))
            return false;
        // Sanity Check 3
        if (!PersonalTable.E.IsPresentInGame(species, form))
            return false;

        result = CreateEggEncounter(species, version);
        return true;
    }

    // Version is not updated when hatching an Egg in Gen3. Version is a clear indicator of the game it originated on.

    public static bool TryGetSplit(EncounterEgg3 other, ReadOnlySpan<EvoCriteria> chain, [NotNullWhen(true)] out EncounterEgg3? result)
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
        if (!Breeding.IsSplitBreedNotBabySpecies3(devolved.Species))
            return false;

        result = other with { Species = devolved.Species };
        return true;
    }
}
