using System;
using System.Collections.Generic;
using System.Linq;

using static PKHeX.Core.EncounterStateUtil;
using static PKHeX.Core.EncounterTypeGroup;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core;

public sealed class EncounterGenerator4 : IEncounterGenerator
{
    public static readonly EncounterGenerator4 Instance = new();

    // Utility
    private static readonly PGT RangerManaphy = new() { Data = { [0] = 7, [8] = 1 } };

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info)
    {
        var chain = EncounterOrigin.GetOriginChain(pk);
       return GetEncounters(pk, chain, info);
    }

    public IEnumerable<IEncounterable> GetPossible(PKM _, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        if (groups.HasFlag(Mystery))
        {
            if (chain[^1].Species == (int)Species.Manaphy)
                yield return RangerManaphy;

            var table = EncounterEvent.MGDB_G4;
            foreach (var enc in GetPossibleGifts(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Egg))
        {
            var eggs = GetEggs(chain, game);
            foreach (var enc in eggs)
                yield return enc;
        }
        if (groups.HasFlag(Static))
        {
            var table = GetStatic(game);
            foreach (var enc in GetPossibleStatic(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Slot))
        {
            var areas = GetAreas(game);
            foreach (var enc in GetPossibleSlots(chain, areas))
                yield return enc;
        }
        if (groups.HasFlag(Trade))
        {
            var table = GetTrades(game);
            foreach (var enc in GetPossibleTrades(chain, table))
                yield return enc;
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleGifts(EvoCriteria[] chain, IReadOnlyList<PCD> table)
    {
        foreach (var enc in table)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                yield return enc;
                break;
            }
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleStatic(EvoCriteria[] chain, EncounterStatic[] table)
    {
        foreach (var enc in table)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                yield return enc;
                break;
            }
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleSlots(EvoCriteria[] chain, EncounterArea4[] areas)
    {
        foreach (var area in areas)
        {
            foreach (var slot in area.Slots)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != slot.Species)
                        continue;
                    yield return slot;
                    break;
                }
            }
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleTrades(EvoCriteria[] chain, EncounterTrade[] table)
    {
        foreach (var enc in table)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                yield return enc;
                break;
            }
        }
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        info.PIDIV = MethodFinder.Analyze(pk);
        var deferredPIDIV = new List<IEncounterable>();
        var deferredEType = new List<IEncounterable>();

        foreach (var z in GetEncountersInner(pk, chain, info))
        {
            if (!info.PIDIV.Type.IsCompatible4(z, pk))
                deferredPIDIV.Add(z);
            else if (pk is IGroundTile e && !(z is IGroundTypeTile t ? t.GroundTile.Contains(e.GroundTile) : e.GroundTile == 0))
                deferredEType.Add(z);
            else
                yield return z;
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
        if (pk.FatefulEncounter)
        {
            if (PGT.IsRangerManaphy(pk))
            {
                yield return RangerManaphy;
                yield break;
            }

            bool yielded = false;
            foreach (var mg in EncounterEvent.MGDB_G4)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != mg.Species)
                        continue;
                    if (mg.IsMatchExact(pk, evo))
                    {
                        yield return mg;
                        yielded = true;
                    }
                    break;
                }
            }
            if (yielded)
                yield break;
        }
        if (Locations.IsEggLocationBred4(pk.Egg_Location, game))
        {
            var eggs = GetEggs(chain, game);
            foreach (var egg in eggs)
                yield return egg;
        }

        var encTrade = GetTrades(game);
        foreach (var enc in encTrade)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                if (enc.IsMatchExact(pk, evo))
                    yield return enc;
                break;
            }
        }

        IEncounterable? deferred = null;
        IEncounterable? partial = null;

        bool safariSport = pk.Ball is (int)Ball.Sport or (int)Ball.Safari; // never static encounters
        if (!safariSport)
        {
            var encStatic = GetStatic(game);
            foreach (var enc in encStatic)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != enc.Species)
                        continue;
                    if (!enc.IsMatchExact(pk, evo))
                        break;

                    var match = enc.GetMatchRating(pk);
                    if (match == PartialMatch)
                        partial ??= enc;
                    else
                        yield return enc;
                    break;
                }
            }
        }

        if (CanBeWildEncounter(pk))
        {
            var wildFrames = AnalyzeFrames(pk, info);
            var areas = GetAreas(game);
            foreach (var area in areas)
            {
                var slots = area.GetMatchingSlots(pk, chain);
                foreach (var slot in slots)
                {
                    var match = slot.GetMatchRating(pk);
                    if (match == PartialMatch)
                    {
                        partial ??= slot;
                        continue;
                    }

                    // Can use Radar to force the encounter slot to stay consistent across encounters.
                    if (slot.CanUseRadar)
                    {
                        yield return slot;
                        continue;
                    }

                    var frame = wildFrames.Find(s => s.IsSlotCompatibile(slot, pk));
                    if (frame == null)
                    {
                        deferred ??= slot;
                        continue;
                    }
                    yield return slot;
                }
            }

            info.FrameMatches = false;
            if (deferred is EncounterSlot4 x)
                yield return x;

            if (partial is EncounterSlot4 y)
            {
                var frame = wildFrames.Find(s => s.IsSlotCompatibile(y, pk));
                info.FrameMatches = frame != null;
                yield return y;
            }
        }

        // do static encounters if they were deferred to end, spit out any possible encounters for invalid pk
        if (!safariSport)
            yield break;

        var encStatic2 = GetStatic(game);
        foreach (var enc in encStatic2)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                if (!enc.IsMatchExact(pk, evo))
                    break;

                var match = enc.GetMatchRating(pk);
                if (match == PartialMatch)
                    partial ??= enc;
                else
                    yield return enc;
                break;
            }
        }

        if (partial is not null)
            yield return partial;
    }

    private static List<Frame> AnalyzeFrames(PKM pk, LegalInfo info)
    {
        return FrameFinder.GetFrames(info.PIDIV, pk).ToList();
    }

    private static EncounterStatic[] GetStatic(GameVersion gameSource) => gameSource switch
    {
        GameVersion.D => Encounters4DPPt.StaticD,
        GameVersion.P => Encounters4DPPt.StaticP,
        GameVersion.Pt => Encounters4DPPt.StaticPt,
        GameVersion.HG => Encounters4HGSS.StaticHG,
        GameVersion.SS => Encounters4HGSS.StaticSS,
        _ => throw new ArgumentOutOfRangeException(nameof(gameSource), gameSource, null),
    };

    private static EncounterArea4[] GetAreas(GameVersion gameSource) => gameSource switch
    {
        GameVersion.D => Encounters4DPPt.SlotsD,
        GameVersion.P => Encounters4DPPt.SlotsP,
        GameVersion.Pt => Encounters4DPPt.SlotsPt,
        GameVersion.HG => Encounters4HGSS.SlotsHG,
        GameVersion.SS => Encounters4HGSS.SlotsSS,
        _ => throw new ArgumentOutOfRangeException(nameof(gameSource), gameSource, null),
    };

    private static EncounterTrade[] GetTrades(GameVersion gameSource) => gameSource switch
    {
        GameVersion.D => Encounters4DPPt.TradeGift_DPPt,
        GameVersion.P => Encounters4DPPt.TradeGift_DPPt,
        GameVersion.Pt => Encounters4DPPt.TradeGift_DPPt,
        GameVersion.HG => Encounters4HGSS.TradeGift_HGSS,
        GameVersion.SS => Encounters4HGSS.TradeGift_HGSS,
        _ => throw new ArgumentOutOfRangeException(nameof(gameSource), gameSource, null),
    };

    private const int Generation = 4;
    private const EntityContext Context = EntityContext.Gen4;
    private const byte EggLevel = 1;

    private static IEnumerable<EncounterEgg> GetEggs(EvoCriteria[] chain, GameVersion version)
    {
        var devolved = chain[^1];

        // Ensure most devolved species is the same as the egg species.
        var (species, form) = GetBaby(devolved);
        if (species != devolved.Species && !IsValidBabySpecies(devolved.Species))
            yield break; // not a split-breed.

        // Sanity Check 1
        if (!Breeding.CanHatchAsEgg(species))
            yield break;
        // Sanity Check 2
        if (!Breeding.CanHatchAsEgg(species, form, Context))
            yield break;
        // Sanity Check 3
        if (!PersonalTable.HGSS.IsPresentInGame(species, form))
            yield break;

        yield return CreateEggEncounter(species, form, version);
        // Version is not updated when hatching an Egg in Gen4. Version is a clear indicator of the game it originated on.

        // Check for split-breed
        if (species == devolved.Species)
        {
            if (chain.Length < 2)
                yield break; // no split-breed
            devolved = chain[^2];
        }
        var splitSet = Breeding.GetSplitBreedGeneration(Generation);
        if (splitSet is null)
            yield break; // Shouldn't happen.
        if (!splitSet.Contains(devolved.Species))
            yield break;

        species = devolved.Species;
        form = devolved.Form;
        yield return CreateEggEncounter(species, form, version);
    }

    private static bool IsValidBabySpecies(ushort species)
    {
        var split = Breeding.GetSplitBreedGeneration(Generation);
        return split is not null && split.Contains(species);
    }

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
}
