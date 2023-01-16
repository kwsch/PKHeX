using System;
using System.Collections.Generic;
using System.Linq;

using static PKHeX.Core.EncounterStateUtil;
using static PKHeX.Core.EncounterTypeGroup;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core;

public sealed class EncounterGenerator3 : IEncounterGenerator
{
    public static readonly EncounterGenerator3 Instance = new();

    public IEnumerable<IEncounterable> GetPossible(PKM _, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        if (groups.HasFlag(Mystery))
        {
            var table = EncountersWC3.Encounter_WC3;
            foreach (var enc in GetPossibleGifts(chain, table, game))
                yield return enc;
        }

        if (groups.HasFlag(Trade))
        {
            var table = GetTrades(game);
            foreach (var enc in GetPossibleTrades(chain, table, game))
                yield return enc;
        }
        if (groups.HasFlag(Egg))
        {
            var eggs = GetEggs(chain, game);
            foreach (var egg in eggs)
                yield return egg;
        }
        if (groups.HasFlag(Slot))
        {
            var areas = GetAreas(game);
            foreach (var enc in GetPossibleAreas(chain, areas))
                yield return enc;
        }
        if (groups.HasFlag(Static))
        {
            var table = GetStatic(game);
            foreach (var enc in GetPossibleStatic(chain, table))
                yield return enc;
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleGifts(EvoCriteria[] chain, WC3[] table, GameVersion game)
    {
        foreach (var enc in table)
        {
            if (!enc.Version.Contains(game))
                continue;
            if (enc.NotDistributed)
                continue;

            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                yield return enc;
                break;
            }
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleTrades(EvoCriteria[] chain, EncounterTrade3[] table,
        GameVersion game)
    {
        foreach (var enc in table)
        {
            if (!enc.Version.Contains(game))
                continue;

            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                yield return enc;
                break;
            }
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleAreas(EvoCriteria[] chain, EncounterArea3[] areas)
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

    private static IEnumerable<IEncounterable> GetPossibleStatic(EvoCriteria[] chain, EncounterStatic3[] table)
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

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info)
    {
        var chain = EncounterOrigin.GetOriginChain(pk);
        return GetEncounters(pk, chain, info);
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        info.PIDIV = MethodFinder.Analyze(pk);
        IEncounterable? partial = null;

        foreach (var z in GetEncountersInner(pk, chain, info))
        {
            if (info.PIDIV.Type.IsCompatible3(z, pk))
                yield return z;
            else
                partial ??= z;
        }
        if (partial == null)
            yield break;

        info.PIDIVMatches = false;
        yield return partial;
    }

    private static IEnumerable<IEncounterable> GetEncountersInner(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        var game = (GameVersion)pk.Version;

        // Mystery Gifts
        foreach (var z in EncountersWC3.Encounter_WC3)
        {
            if (!z.Version.Contains(game))
                continue;

            foreach (var evo in chain)
            {
                if (evo.Species != z.Species)
                    continue;
                if (!z.IsMatchExact(pk, evo))
                    break;

                // Don't bother deferring matches.
                var match = z.GetMatchRating(pk);
                if (match != PartialMatch)
                    yield return z;
                break;
            }
        }

        // Trades
        var trades = GetTrades(game);
        foreach (var z in trades)
        {
            if (!z.Version.Contains(game))
                continue;

            foreach (var evo in chain)
            {
                if (evo.Species != z.Species)
                    continue;
                if (!z.IsMatchExact(pk, evo))
                    break;

                // Don't bother deferring matches.
                var match = z.GetMatchRating(pk);
                if (match != PartialMatch)
                    yield return z;
                break;
            }
        }

        IEncounterable? deferred = null;
        IEncounterable? partial = null;

        // Static Encounter
        // Defer everything if Safari Ball
        bool safari = pk.Ball == (byte)Ball.Safari; // never static encounters
        if (!safari)
        {
            var table = GetStatic(game);
            foreach (var z in table)
            {
                if (!z.Version.Contains(game))
                    continue;

                foreach (var evo in chain)
                {
                    if (evo.Species != z.Species)
                        continue;
                    if (!z.IsMatchExact(pk, evo))
                        break;

                    var match = z.GetMatchRating(pk);
                    if (match == PartialMatch)
                        partial ??= z;
                    else
                        yield return z;
                    break;
                }
            }
        }

        // Encounter Slots
        List<Frame>? wildFrames = null;
        if (CanBeWildEncounter(pk))
        {
            wildFrames = AnalyzeFrames(pk, info);
            var areas = GetAreas(game);
            foreach (var area in areas)
            {
                var all = area.GetMatchingSlots(pk, chain);
                foreach (var z in all)
                {
                    var match = z.GetMatchRating(pk);
                    if (match == PartialMatch)
                    {
                        partial ??= z;
                        continue;
                    }

                    var frame = wildFrames.Find(s => s.IsSlotCompatibile(z, pk));
                    if (frame == null)
                    {
                        deferred ??= z;
                        continue;
                    }
                    yield return z;
                }
            }

            info.FrameMatches = false;
            if (deferred is EncounterSlot3 x)
                yield return x;
        }

        // Due to the lack of Egg Met Location, eggs can be confused with Slots. Yield them now.
        var eggs = GetEggs(chain, game);
        foreach (var z in eggs)
            yield return z;

        if (partial is EncounterSlot3 y)
        {
            wildFrames ??= AnalyzeFrames(pk, info);
            var frame = wildFrames.Find(s => s.IsSlotCompatibile(y, pk));
            info.FrameMatches = frame != null;
            yield return y;
        }

        // Do static encounters if they were deferred to end, spit out any possible encounters for invalid pk
        if (!safari)
            yield break;

        partial = null;

        var encStatic = GetStatic(game);
        foreach (var z in encStatic)
        {
            if (!z.Version.Contains(game))
                continue;

            foreach (var evo in chain)
            {
                if (evo.Species != z.Species)
                    continue;
                if (!z.IsMatchExact(pk, evo))
                    break;

                var match = z.GetMatchRating(pk);
                if (match == PartialMatch)
                    partial ??= z;
                else
                    yield return z;
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

    private static EncounterStatic3[] GetStatic(GameVersion gameSource) => gameSource switch
    {
        GameVersion.R => Encounters3RSE.StaticR,
        GameVersion.S => Encounters3RSE.StaticS,
        GameVersion.E => Encounters3RSE.StaticE,
        GameVersion.FR => Encounters3FRLG.StaticFR,
        GameVersion.LG => Encounters3FRLG.StaticLG,
        _ => throw new ArgumentOutOfRangeException(nameof(gameSource), gameSource, null),
    };

    private static EncounterArea3[] GetAreas(GameVersion gameSource) => gameSource switch
    {
        GameVersion.R => Encounters3RSE.SlotsR,
        GameVersion.S => Encounters3RSE.SlotsS,
        GameVersion.E => Encounters3RSE.SlotsE,
        GameVersion.FR => Encounters3FRLG.SlotsFR,
        GameVersion.LG => Encounters3FRLG.SlotsLG,
        _ => throw new ArgumentOutOfRangeException(nameof(gameSource), gameSource, null),
    };

    private static EncounterTrade3[] GetTrades(GameVersion gameSource) => gameSource switch
    {
        GameVersion.R => Encounters3RSE.TradeGift_RSE,
        GameVersion.S => Encounters3RSE.TradeGift_RSE,
        GameVersion.E => Encounters3RSE.TradeGift_RSE,
        GameVersion.FR => Encounters3FRLG.TradeGift_FRLG,
        GameVersion.LG => Encounters3FRLG.TradeGift_FRLG,
        _ => throw new ArgumentOutOfRangeException(nameof(gameSource), gameSource, null),
    };

    private const int Generation = 3;
    private const EntityContext Context = EntityContext.Gen3;
    private const byte EggLevel = 5;

    private static IEnumerable<EncounterEgg> GetEggs(EvoCriteria[] chain, GameVersion version)
    {
        var devolved = chain[^1];

        // Ensure most devolved species is the same as the egg species.
        var (species, form) = GetBaby(devolved);
        if (species != devolved.Species && !IsValidBabySpecies(devolved.Species))
            yield break; // no split-breed.

        // Sanity Check 1
        if (!Breeding.CanHatchAsEgg(species))
            yield break;
        // Sanity Check 2
        if (!Breeding.CanHatchAsEgg(species, form, Context))
            yield break;
        // Sanity Check 3
        if (!PersonalTable.E.IsPresentInGame(species, form))
            yield break;

        yield return CreateEggEncounter(species, form, version);
        // Version is not updated when hatching an Egg in Gen3. Version is a clear indicator of the game it originated on.

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
        if (FormInfo.IsBattleOnlyForm(species, form, Generation) || species is (int)Species.Castform)
            form = FormInfo.GetOutOfBattleForm(species, form, Generation);
        return new EncounterEgg(species, form, EggLevel, Generation, version, Context);
    }

    private static (ushort Species, byte Form) GetBaby(EvoCriteria lowest)
    {
        return EvolutionTree.Evolves3.GetBaseSpeciesForm(lowest.Species, lowest.Form);
    }
}
