using System;
using System.Collections.Generic;
using System.Linq;

using static PKHeX.Core.EncounterGeneratorUtil;
using static PKHeX.Core.EncounterTypeGroup;
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
        var iterator = new EncounterEnumerator3(pk, chain, game);
        EncounterSlot3? deferSlot = null;
        List<Frame>? frames = null;
        foreach (var enc in iterator)
        {
            var e = enc.Encounter;
            if (e is not EncounterSlot3 s3)
            {
                yield return e;
                continue;
            }

            var wildFrames = frames ?? AnalyzeFrames(pk, info);
            var frame = wildFrames.Find(s => s.IsSlotCompatibile(s3, pk));
            if (frame != null)
                yield return s3;
            deferSlot ??= s3;
        }
        if (deferSlot != null)
            yield return deferSlot;
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
