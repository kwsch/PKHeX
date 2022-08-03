//#define VERIFY_GEN
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PKHeX.Core;

/// <summary>
/// Generates weakly matched <see cref="IEncounterable"/> objects for an input <see cref="PKM"/> (and/or criteria).
/// </summary>
public static class EncounterMovesetGenerator
{
    /// <summary>
    /// Order in which <see cref="IEncounterable"/> objects are yielded from the <see cref="GenerateVersionEncounters"/> generator.
    /// </summary>
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public static IReadOnlyCollection<EncounterOrder> PriorityList { get; set; } = PriorityList = (EncounterOrder[])Enum.GetValues(typeof(EncounterOrder));

    /// <summary>
    /// Resets the <see cref="PriorityList"/> to the default values.
    /// </summary>
    public static void ResetFilters() => PriorityList = (EncounterOrder[])Enum.GetValues(typeof(EncounterOrder));

    /// <summary>
    /// Gets possible <see cref="PKM"/> objects that allow all moves requested to be learned.
    /// </summary>
    /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
    /// <param name="info">Trainer information of the receiver.</param>
    /// <param name="moves">Moves that the resulting <see cref="IEncounterable"/> must be able to learn.</param>
    /// <param name="versions">Any specific version(s) to iterate for. If left blank, all will be checked.</param>
    /// <returns>A consumable <see cref="PKM"/> list of possible results.</returns>
    /// <remarks>When updating, update the sister <see cref="GenerateEncounters(PKM,ITrainerInfo,int[],GameVersion[])"/> method.</remarks>
    public static IEnumerable<PKM> GeneratePKMs(PKM pk, ITrainerInfo info, int[]? moves = null, params GameVersion[] versions)
    {
        pk.TID = info.TID;
        var m = moves ?? pk.Moves;
        var vers = versions.Length >= 1 ? versions : GameUtil.GetVersionsWithinRange(pk, pk.Format);
        foreach (var ver in vers)
        {
            var encounters = GenerateVersionEncounters(pk, m, ver);
            foreach (var enc in encounters)
            {
                var result = enc.ConvertToPKM(info);
#if VERIFY_GEN
                    var la = new LegalityAnalysis(result);
                    if (!la.Valid)
                        throw new Exception("Legality analysis of generated Pokémon is invalid");
#endif
                yield return result;
            }
        }
    }

    /// <summary>
    /// Gets possible <see cref="IEncounterable"/> objects that allow all moves requested to be learned.
    /// </summary>
    /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
    /// <param name="info">Trainer information of the receiver.</param>
    /// <param name="moves">Moves that the resulting <see cref="IEncounterable"/> must be able to learn.</param>
    /// <param name="versions">Any specific version(s) to iterate for. If left blank, all will be checked.</param>
    /// <returns>A consumable <see cref="IEncounterable"/> list of possible results.</returns>
    /// <remarks>When updating, update the sister <see cref="GeneratePKMs(PKM,ITrainerInfo,int[],GameVersion[])"/> method.</remarks>
    public static IEnumerable<IEncounterable> GenerateEncounters(PKM pk, ITrainerInfo info, int[]? moves = null, params GameVersion[] versions)
    {
        pk.TID = info.TID;
        var m = moves ?? pk.Moves;
        var vers = versions.Length >= 1 ? versions : GameUtil.GetVersionsWithinRange(pk, pk.Format);
        foreach (var ver in vers)
        {
            var encounters = GenerateVersionEncounters(pk, m, ver);
            foreach (var enc in encounters)
                yield return enc;
        }
    }

    /// <summary>
    /// Gets possible <see cref="PKM"/> objects that allow all moves requested to be learned within a specific generation.
    /// </summary>
    /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
    /// <param name="info">Trainer information of the receiver.</param>
    /// <param name="generation">Specific generation to iterate versions for.</param>
    /// <param name="moves">Moves that the resulting <see cref="IEncounterable"/> must be able to learn.</param>
    public static IEnumerable<PKM> GeneratePKMs(PKM pk, ITrainerInfo info, int generation, int[]? moves = null)
    {
        var vers = GameUtil.GetVersionsInGeneration(generation, pk.Version);
        return GeneratePKMs(pk, info, moves, vers);
    }

    /// <summary>
    /// Gets possible encounters that allow all moves requested to be learned.
    /// </summary>
    /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
    /// <param name="generation">Specific generation to iterate versions for.</param>
    /// <param name="moves">Moves that the resulting <see cref="IEncounterable"/> must be able to learn.</param>
    /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
    public static IEnumerable<IEncounterable> GenerateEncounter(PKM pk, int generation, int[]? moves = null)
    {
        var vers = GameUtil.GetVersionsInGeneration(generation, pk.Version);
        return GenerateEncounters(pk, moves, vers);
    }

    /// <summary>
    /// Gets possible encounters that allow all moves requested to be learned.
    /// </summary>
    /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
    /// <param name="moves">Moves that the resulting <see cref="IEncounterable"/> must be able to learn.</param>
    /// <param name="versions">Any specific version(s) to iterate for. If left blank, all will be checked.</param>
    /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
    public static IEnumerable<IEncounterable> GenerateEncounters(PKM pk, int[]? moves = null, params GameVersion[] versions)
    {
        moves ??= pk.Moves;
        if (versions.Length > 0)
            return GenerateEncounters(pk, moves, (IReadOnlyList<GameVersion>)versions);

        var vers = GameUtil.GetVersionsWithinRange(pk, pk.Format);
        return vers.SelectMany(ver => GenerateVersionEncounters(pk, moves, ver));
    }

    /// <summary>
    /// Gets possible encounters that allow all moves requested to be learned.
    /// </summary>
    /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
    /// <param name="moves">Moves that the resulting <see cref="IEncounterable"/> must be able to learn.</param>
    /// <param name="vers">Any specific version(s) to iterate for. If left blank, all will be checked.</param>
    /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
    public static IEnumerable<IEncounterable> GenerateEncounters(PKM pk, int[]? moves, IReadOnlyList<GameVersion> vers)
    {
        moves ??= pk.Moves;
        return vers.SelectMany(ver => GenerateVersionEncounters(pk, moves, ver));
    }

    /// <summary>
    /// Gets possible encounters that allow all moves requested to be learned.
    /// </summary>
    /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
    /// <param name="moves">Moves that the resulting <see cref="IEncounterable"/> must be able to learn.</param>
    /// <param name="version">Specific version to iterate for.</param>
    /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
    public static IEnumerable<IEncounterable> GenerateVersionEncounters(PKM pk, ReadOnlySpan<int> moves, GameVersion version)
    {
        if (pk.Species == 0) // can enter this method after failing to set a species ID that cannot exist in the format
            return Array.Empty<IEncounterable>();
        if (pk.Species is (int)Species.Smeargle && !IsPlausibleSmeargleMoveset(pk, moves))
            return Array.Empty<IEncounterable>();

        pk.Version = (int)version;
        var context = pk.Context;
        if (context is EntityContext.Gen2 && version is GameVersion.RD or GameVersion.GN or GameVersion.BU or GameVersion.YW)
            context = EntityContext.Gen1; // try excluding baby pokemon from our evolution chain, for move learning purposes.
        var et = EvolutionTree.GetEvolutionTree(context);
        var chain = et.GetValidPreEvolutions(pk, levelMax: 100, skipChecks: true);
        int[] needs = GetNeededMoves(pk, moves);

        return PriorityList.SelectMany(type => GetPossibleOfType(pk, needs, version, type, chain));
    }

    private static bool IsPlausibleSmeargleMoveset(PKM pk, ReadOnlySpan<int> moves)
    {
        foreach (var move in moves)
        {
            if (!Legal.IsValidSketch(move, pk.Format))
                return false;
        }
        return true;
    }

    private static int[] GetNeededMoves(PKM pk, ReadOnlySpan<int> moves)
    {
        if (pk.Species == (int)Species.Smeargle)
            return Array.Empty<int>();

        // Roughly determine the generation the PKM is originating from
        var ver = pk.Version;
        int origin = pk.Generation;
        if (origin < 0)
            origin = ((GameVersion)ver).GetGeneration();

        // Temporarily replace the Version for VC1 transfers, so that they can have VC2 moves if needed.
        bool vcBump = origin == 1 && pk.Format >= 7;
        if (vcBump)
            pk.Version = (int)GameVersion.C;

        var length = pk.MaxMoveID + 1;
        var rent = ArrayPool<bool>.Shared.Rent(length);
        var permitted = rent.AsSpan(0, length);
        var enc = new EvolutionOrigin(0, (byte)ver, (byte)origin, 1, 100, true);
        var history = EvolutionChain.GetEvolutionChainsSearch(pk, enc);
        LearnPossible.Get(pk, EncounterInvalid.Default, history, permitted);

        int ctr = 0; // count of moves that can be learned
        Span<int> result = stackalloc int[moves.Length];
        foreach (var move in moves)
        {
            if (move == 0)
                continue;
            if (!permitted[move])
                result[ctr++] = move;
        }

        permitted.Clear();
        ArrayPool<bool>.Shared.Return(rent);

        if (vcBump)
            pk.Version = ver;

        if (ctr == 0)
            return Array.Empty<int>();
        return result[..ctr].ToArray();
    }

    private static IEnumerable<IEncounterable> GetPossibleOfType(PKM pk, int[] needs, GameVersion version, EncounterOrder type, EvoCriteria[] chain)
    {
        return type switch
        {
            EncounterOrder.Egg => GetEggs(pk, needs, chain, version),
            EncounterOrder.Mystery => GetGifts(pk, needs, chain, version),
            EncounterOrder.Static => GetStatic(pk, needs, chain, version),
            EncounterOrder.Trade => GetTrades(pk, needs, chain, version),
            EncounterOrder.Slot => GetSlots(pk, needs, chain, version),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
    }

    /// <summary>
    /// Gets possible encounters that allow all moves requested to be learned.
    /// </summary>
    /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
    /// <param name="needs">Moves which cannot be taught by the player.</param>
    /// <param name="chain">Origin possible evolution chain</param>
    /// <param name="version">Specific version to iterate for. Necessary for retrieving possible Egg Moves.</param>
    /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
    private static IEnumerable<EncounterEgg> GetEggs(PKM pk, int[] needs, EvoCriteria[] chain, GameVersion version)
    {
        if (!Breeding.CanGameGenerateEggs(version))
            yield break; // no eggs from these games
        int gen = version.GetGeneration();
        var eggs = gen == 2
            ? EncounterEggGenerator2.GenerateEggs(pk, chain, all: true)
            : EncounterEggGenerator.GenerateEggs(pk, chain, gen, all: true);
        foreach (var egg in eggs)
        {
            if (needs.Length == 0)
            {
                yield return egg;
                continue;
            }

            IEnumerable<int> em = MoveEgg.GetEggMoves(egg.Species, egg.Form, egg.Version, egg.Generation);
            if (egg.Generation <= 2)
                em = em.Concat(MoveLevelUp.GetEncounterMoves(egg.Species, 0, egg.Level, egg.Version));
            else if (egg.Species is (int)Species.Pichu && needs.Contains((int)Move.VoltTackle) && egg.CanHaveVoltTackle)
                em = em.Concat(new[] { (int)Move.VoltTackle });

            if (HasAllMoves(needs, em))
                yield return egg;
        }
    }

    /// <summary>
    /// Gets possible encounters that allow all moves requested to be learned.
    /// </summary>
    /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
    /// <param name="needs">Moves which cannot be taught by the player.</param>
    /// <param name="chain">Origin possible evolution chain</param>
    /// <param name="version">Specific version to iterate for.</param>
    /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
    private static IEnumerable<MysteryGift> GetGifts(PKM pk, int[] needs, EvoCriteria[] chain, GameVersion version)
    {
        var format = pk.Format;
        var gifts = MysteryGiftGenerator.GetPossible(pk, chain, version);
        foreach (var gift in gifts)
        {
            if (gift is WC3 {NotDistributed: true})
                continue;
            if (!IsSane(chain, gift, format))
                continue;
            if (needs.Length == 0)
            {
                yield return gift;
                continue;
            }
            var em = gift.Moves.Concat(gift.Relearn);
            if (HasAllMoves(needs, em))
                yield return gift;
        }
    }

    /// <summary>
    /// Gets possible encounters that allow all moves requested to be learned.
    /// </summary>
    /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
    /// <param name="needs">Moves which cannot be taught by the player.</param>
    /// <param name="chain">Origin possible evolution chain</param>
    /// <param name="version">Specific version to iterate for.</param>
    /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
    private static IEnumerable<EncounterStatic> GetStatic(PKM pk, int[] needs, EvoCriteria[] chain, GameVersion version)
    {
        var format = pk.Format;
        var encounters = EncounterStaticGenerator.GetPossible(pk, chain, version);
        foreach (var enc in encounters)
        {
            if (!IsSane(chain, enc, format))
                continue;
            if (needs.Length == 0)
            {
                yield return enc;
                continue;
            }

            // Some rare encounters have special moves hidden in the Relearn section (Gen7 Wormhole Ho-Oh). Include relearn moves
            IEnumerable<int> em = enc.Moves;
            if (enc is IRelearn { Relearn: int[] {Length: not 0} r})
                em = em.Concat(r);
            if (enc.Generation <= 2)
                em = em.Concat(MoveLevelUp.GetEncounterMoves(enc.Species, 0, enc.Level, enc.Version));

            if (HasAllMoves(needs, em))
                yield return enc;
        }

        int gen = version.GetGeneration();
        if ((uint)gen >= 3)
            yield break;

        var gifts = EncounterStaticGenerator.GetPossibleGBGifts(chain, version);
        foreach (var enc in gifts)
        {
            if (needs.Length == 0)
            {
                yield return enc;
                continue;
            }

            var em = enc.Moves;
            if (HasAllMoves(needs, em))
                yield return enc;
        }
    }

    /// <summary>
    /// Gets possible encounters that allow all moves requested to be learned.
    /// </summary>
    /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
    /// <param name="needs">Moves which cannot be taught by the player.</param>
    /// <param name="chain">Origin possible evolution chain</param>
    /// <param name="version">Specific version to iterate for.</param>
    /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
    private static IEnumerable<EncounterTrade> GetTrades(PKM pk, int[] needs, EvoCriteria[] chain, GameVersion version)
    {
        var format = pk.Format;
        var trades = EncounterTradeGenerator.GetPossible(pk, chain, version);
        foreach (var trade in trades)
        {
            if (!IsSane(chain, trade, format))
                continue;
            if (needs.Length == 0)
            {
                yield return trade;
                continue;
            }
            IEnumerable<int> em = trade.Moves;
            if (trade.Generation <= 2)
                em = em.Concat(MoveLevelUp.GetEncounterMoves(trade.Species, 0, trade.Level, trade.Version));
            else if (trade is IRelearn { Relearn: int[] { Length: not 0 } r })
                em = em.Concat(r);
            if (HasAllMoves(needs, em))
                yield return trade;
        }
    }

    /// <summary>
    /// Gets possible encounters that allow all moves requested to be learned.
    /// </summary>
    /// <param name="pk">Rough Pokémon data which contains the requested species, gender, and form.</param>
    /// <param name="needs">Moves which cannot be taught by the player.</param>
    /// <param name="chain">Origin possible evolution chain</param>
    /// <param name="version">Origin version</param>
    /// <returns>A consumable <see cref="IEncounterable"/> list of possible encounters.</returns>
    private static IEnumerable<EncounterSlot> GetSlots(PKM pk, int[] needs, EvoCriteria[] chain, GameVersion version)
    {
        var format = pk.Format;
        var slots = EncounterSlotGenerator.GetPossible(pk, chain, version);
        foreach (var slot in slots)
        {
            if (!IsSane(chain, slot, format))
                continue;

            if (needs.Length == 0)
            {
                yield return slot;
                continue;
            }

            if (slot is IMoveset m && HasAllMoves(needs, m.Moves))
                yield return slot;
            else if (needs.Length == 1 && slot is EncounterSlot6AO {CanDexNav: true} dn && dn.CanBeDexNavMove(needs[0]))
                yield return slot;
            else if (needs.Length == 1 && slot is EncounterSlot8b {IsUnderground: true} ug && ug.CanBeUndergroundMove(needs[0]))
                yield return slot;
            else if (slot.Generation <= 2 && HasAllMoves(needs, MoveLevelUp.GetEncounterMoves(slot.Species, 0, slot.LevelMin, slot.Version)))
                yield return slot;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSane(ReadOnlySpan<EvoCriteria> chain, IEncounterTemplate enc, int format)
    {
        foreach (var evo in chain)
        {
            if (evo.Species != enc.Species)
                continue;
            if (evo.Form == enc.Form)
                return true;
            if (FormInfo.IsFormChangeable(enc.Species, enc.Form, evo.Form, enc.Generation))
                return true;
            if (enc is EncounterSlot {IsRandomUnspecificForm: true} or EncounterStatic {IsRandomUnspecificForm: true})
                return true;
            if (enc is EncounterStatic7 {IsTotem: true} && evo.Form == 0 && format > 7) // totems get form wiped
                return true;
            break;
        }
        return false;
    }

    private static bool HasAllMoves(ReadOnlySpan<int> needs, IEnumerable<int> extra)
    {
        // Flag each present index; having all moves will have all bitflags.
        int flags = 0;
        foreach (var move in extra)
        {
            var index = needs.IndexOf(move);
            if (index == -1)
                continue;
            flags |= 1 << index;
        }
        return flags == (1 << needs.Length) - 1;
    }
}
