using System;
using System.Collections.Generic;
using System.Linq;

using static PKHeX.Core.EncounterGeneratorUtil;
using static PKHeX.Core.EncounterStateUtil;
using static PKHeX.Core.EncounterTypeGroup;
using static PKHeX.Core.EncounterMatchRating;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

public sealed class EncounterGenerator3 : IEncounterGenerator
{
    public static readonly EncounterGenerator3 Instance = new();

    public IEnumerable<IEncounterable> GetPossible(PKM _, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        if (chain.Length == 0)
            yield break;

        if (groups.HasFlag(Mystery))
        {
            var table = EncountersWC3.Encounter_WC3;
            foreach (var enc in GetPossibleGifts(chain, table, game))
                yield return enc;
        }

        if (groups.HasFlag(Trade))
        {
            if (game is GameVersion.FR or GameVersion.LG)
            {
                var table = Encounters3FRLG.TradeGift_FRLG;
                foreach (var enc in GetPossibleAll(chain, table))
                    yield return enc;
                var specific = game is GameVersion.FR ? Encounters3FRLG.TradeGift_FR : Encounters3FRLG.TradeGift_LG;
                foreach (var enc in GetPossibleAll(chain, specific))
                    yield return enc;
            }
            else
            {
                var specific = game is GameVersion.E ? Encounters3RSE.TradeGift_E : Encounters3RSE.TradeGift_RS;
                foreach (var enc in GetPossibleAll(chain, specific))
                    yield return enc;
            }
        }
        if (groups.HasFlag(Egg))
        {
            if (TryGetEgg(chain, game, out var egg))
            {
                yield return egg;
                if (TryGetSplit(egg, chain, out var split))
                    yield return split;
            }
        }
        if (groups.HasFlag(Slot))
        {
            var areas = GetAreas(game);
            foreach (var enc in GetPossibleSlots<EncounterArea3, EncounterSlot3>(chain, areas))
                yield return enc;
        }
        if (groups.HasFlag(Static))
        {
            var group = game is GameVersion.FR or GameVersion.LG ? Encounters3FRLG.StaticFRLG : Encounters3RSE.StaticRSE;
            foreach (var enc in GetPossibleAll(chain, group))
                yield return enc;
            var table = GetStatic(game);
            foreach (var enc in GetPossibleAll(chain, table))
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

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, LegalInfo info)
    {
        var chain = EncounterOrigin.GetOriginChain(pk, 3);
        return GetEncounters(pk, chain, info);
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        if (chain.Length == 0)
            yield break;

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
        foreach (var enc in EncountersWC3.Encounter_WC3)
        {
            if (!enc.Version.Contains(game))
                continue;

            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                if (!enc.IsMatchExact(pk, evo))
                    break;

                // Don't bother deferring matches.
                var match = enc.GetMatchRating(pk);
                if (match < PartialMatch)
                    yield return enc;
                break;
            }
        }

        // Trades

        if (game is GameVersion.FR or GameVersion.LG)
        {
            foreach (var enc in Encounters3FRLG.TradeGift_FRLG)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != enc.Species)
                        continue;
                    if (!enc.IsMatchExact(pk, evo))
                        break;

                    // Don't bother deferring matches.
                    var match = enc.GetMatchRating(pk);
                    if (match < PartialMatch)
                        yield return enc;
                    break;
                }
            }
            var specific = game is GameVersion.FR ? Encounters3FRLG.TradeGift_FR : Encounters3FRLG.TradeGift_LG;
            foreach (var enc in specific)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != enc.Species)
                        continue;
                    if (!enc.IsMatchExact(pk, evo))
                        break;

                    // Don't bother deferring matches.
                    var match = enc.GetMatchRating(pk);
                    if (match < PartialMatch)
                        yield return enc;
                    break;
                }
            }
        }
        else
        {
            var specific = game is GameVersion.E ? Encounters3RSE.TradeGift_E : Encounters3RSE.TradeGift_RS;
            foreach (var enc in specific)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != enc.Species)
                        continue;
                    if (!enc.IsMatchExact(pk, evo))
                        break;

                    // Don't bother deferring matches.
                    var match = enc.GetMatchRating(pk);
                    if (match < PartialMatch)
                        yield return enc;
                    break;
                }
            }
        }

        IEncounterable? partial = null;

        // Static Encounter
        // Defer everything if Safari Ball
        bool safari = pk.Ball == (byte)Ball.Safari; // never static encounters
        if (!safari)
        {
            var group = game is GameVersion.FR or GameVersion.LG ? Encounters3FRLG.StaticFRLG : Encounters3RSE.StaticRSE;
            foreach (var enc in group)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != enc.Species)
                        continue;
                    if (!enc.IsMatchExact(pk, evo))
                        break;

                    var match = enc.GetMatchRating(pk);
                    if (match < PartialMatch)
                        yield return enc;
                    else
                        partial ??= enc;
                    break;
                }
            }
            var table = GetStatic(game);
            foreach (var enc in table)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != enc.Species)
                        continue;
                    if (!enc.IsMatchExact(pk, evo))
                        break;

                    var match = enc.GetMatchRating(pk);
                    if (match < PartialMatch)
                        yield return enc;
                    else
                        partial ??= enc;
                    break;
                }
            }
        }

        // Encounter Slots
        List<Frame>? wildFrames = null;
        if (CanBeWildEncounter(pk))
        {
            IEncounterable? deferred = null;
            wildFrames = AnalyzeFrames(pk, info);
            var areas = GetAreas(game);
            foreach (var area in areas)
            {
                var slots = area.GetMatchingSlots(pk, chain);
                foreach (var enc in slots)
                {
                    var match = enc.GetMatchRating(pk);
                    if (match >= PartialMatch)
                    {
                        partial ??= enc;
                        continue;
                    }

                    var frame = wildFrames.Find(s => s.IsSlotCompatibile(enc, pk));
                    if (frame == null)
                    {
                        deferred ??= enc;
                        continue;
                    }
                    yield return enc;
                }
            }

            info.FrameMatches = false;
            if (deferred is EncounterSlot3 x)
                yield return x;
        }

        // Due to the lack of Egg Met Location, eggs can be confused with Slots. Yield them now.
        if (TryGetEgg(chain, game, out var egg))
        {
            yield return egg;
            if (TryGetSplit(egg, chain, out var split))
                yield return split;
        }

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

        var encStatic = game is GameVersion.FR or GameVersion.LG ? Encounters3FRLG.StaticFRLG : Encounters3RSE.StaticRSE;
        foreach (var enc in encStatic)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                if (!enc.IsMatchExact(pk, evo))
                    break;

                var match = enc.GetMatchRating(pk);
                if (match < PartialMatch)
                    yield return enc;
                else
                    partial ??= enc;
                break;
            }
        }
        var specificStatic = GetStatic(game);
        foreach (var enc in specificStatic)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                if (!enc.IsMatchExact(pk, evo))
                    break;

                var match = enc.GetMatchRating(pk);
                if (match < PartialMatch)
                    yield return enc;
                else
                    partial ??= enc;
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

    private const int Generation = 3;
    private const EntityContext Context = EntityContext.Gen3;
    private const byte EggLevel = 5;

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

    public static bool TryGetEgg(ReadOnlySpan<EvoCriteria> chain, GameVersion version, [NotNullWhen(true)] out EncounterEgg? result)
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

        result = CreateEggEncounter(species, form, version);
        return true;
    }

    // Version is not updated when hatching an Egg in Gen3. Version is a clear indicator of the game it originated on.

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
        if (!Breeding.IsSplitBreedNotBabySpecies3(devolved.Species))
            return false;

        result = other with { Species = devolved.Species, Form = devolved.Form };
        return true;
    }
}
