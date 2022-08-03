using System;
using System.Collections.Generic;
using System.Linq;

using static PKHeX.Core.Encounters6;
using static PKHeX.Core.Encounters8;
using static PKHeX.Core.Move;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Validation logic for specific memory conditions
/// </summary>
public static class MemoryPermissions
{
    public static bool IsMemoryOfKnownMove(int memory) => memory is 48 or 80 or 81;

    public static bool CanWinLotoID(int generation, int item)
    {
        var context = Memories.GetContext(generation);
        return context.CanWinLotoID(item);
    }

    public static bool CanHoldItem(int generation, int item)
    {
        var context = Memories.GetContext(generation);
        return context.CanHoldItem(item);
    }

    public static bool CanPlantBerry(int generation, int item)
    {
        var context = Memories.GetContext(generation);
        return context.CanPlantBerry(item);
    }

    public static bool CanUseItemGeneric(int generation, int item)
    {
        var context = Memories.GetContext(generation);
        return context.CanUseItemGeneric(item);
    }

    public static bool CanUseItem(int generation, int item, int species)
    {
        if (IsUsedKeyItemUnspecific(generation, item))
            return true;
        if (IsUsedKeyItemSpecific(generation, item, species))
            return true;
        return true; // todo
    }

    private static bool IsUsedKeyItemUnspecific(int generation, int item)
    {
        var context = Memories.GetContext(generation);
        return context.IsUsedKeyItemUnspecific(item);
    }

    private static bool IsUsedKeyItemSpecific(int generation, int item, int species)
    {
        var context = Memories.GetContext(generation);
        return context.IsUsedKeyItemSpecific(item, species);
    }

    public static bool CanBuyItem(int generation, int item, GameVersion version = GameVersion.Any)
    {
        var context = Memories.GetContext(generation);
        return context.CanBuyItem(item, version);
    }

    public static bool CanKnowMove(PKM pk, MemoryVariableSet memory, int gen, LegalInfo info, bool battleOnly = false)
    {
        var move = memory.Variable;
        if (move == 0)
            return false;

        if (pk.HasMove(move))
            return true;

        if (pk.IsEgg)
            return false;

        if (GetCanKnowMove(pk, move, gen, info.EvoChainsAllGens, info.EncounterOriginal))
            return true;

        var enc = info.EncounterMatch;
        if (enc is IMoveset ms && ms.Moves.Contains(move))
            return true;
        if (enc is IRelearn r && r.Relearn.Contains(move))
            return true;

        // Relearns can be wiped via Battle Version. Check for eggs too.
        if (IsSpecialEncounterMoveEggDeleted(pk, enc))
        {
            var em = MoveEgg.GetEggMoves(enc.Generation, enc.Species, enc.Form, enc.Version);
            if (em.Contains(move))
                return true;
        }

        if (battleOnly)
        {
            // Some moves can only be known in battle; outside of battle they aren't obtainable as a memory parameter.
            switch (move)
            {
                case (int)BehemothBlade when pk.Species == (int)Zacian:
                case (int)BehemothBash when pk.Species == (int)Zamazenta:
                    return true;
            }
            if (gen == 8 && MoveInfo.IsDynamaxMove(move))
                return true;
            if (pk.Species == (int)Ditto)
            {
                if (move == (int)Struggle)
                    return false;
                return gen switch
                {
                    8 => move <= Legal.MaxMoveID_8_R2 && !MoveInfo.IsDummiedMove(pk, move),
                    _ => move <= Legal.MaxMoveID_6_AO,
                };
            }
        }

        return false;
    }

    private static bool IsSpecialEncounterMoveEggDeleted(PKM pk, IEncounterTemplate enc)
    {
        if (pk.IsOriginalMovesetDeleted())
            return true;
        return enc is EncounterEgg { Generation: < 6 }; // egg moves that are no longer in the movepool
    }

    public static bool GetCanRelearnMove(PKM pk, int move, int generation, EvolutionHistory history, IEncounterTemplate enc)
    {
        if (generation == 6)
        {
            Span<MoveResult> result = stackalloc MoveResult[1];
            Span<int> moves = stackalloc int[] { move };
            LearnGroup6.Instance.Check(result, moves, pk, history, enc, MoveSourceType.Reminder, LearnOption.AtAnyTime);
            return result[0].Valid;
        }
        if (generation == 8)
        {
            Span<MoveResult> result = stackalloc MoveResult[1];
            Span<int> moves = stackalloc int[] { move };
            LearnGroup8.Instance.Check(result, moves, pk, history, enc, MoveSourceType.Reminder, LearnOption.AtAnyTime);
            return result[0].Valid;
        }
        return false;
    }

    private static bool GetCanKnowMove(PKM pk, int move, int generation, EvolutionHistory history, IEncounterTemplate enc)
    {
        if (pk.Species == (int)Smeargle)
            return Legal.IsValidSketch(move, generation);

        ILearnGroup game;
        if (generation == 6)
            game = LearnGroup6.Instance;
        else if (generation == 8)
            game = LearnGroup8.Instance;
        else
            return false;

        Span<MoveResult> result = stackalloc MoveResult[1];
        Span<int> moves = stackalloc int[] { move };
        LearnVerifierHistory.MarkAndIterate(result, moves, enc, pk, history, game, MoveSourceType.All, LearnOption.AtAnyTime);
        return result[0].Valid;
    }

    public static bool GetCanBeCaptured(int species, int gen, GameVersion version) => gen switch
    {
        6 => version switch
        {
            GameVersion.Any => GetCanBeCaptured(species, SlotsX, StaticX) || GetCanBeCaptured(species, SlotsY, StaticY)
                                                                          || GetCanBeCaptured(species, SlotsA, StaticA) || GetCanBeCaptured(species, SlotsO, StaticO),

            GameVersion.X => GetCanBeCaptured(species, SlotsX, StaticX),
            GameVersion.Y => GetCanBeCaptured(species, SlotsY, StaticY),

            GameVersion.AS => GetCanBeCaptured(species, SlotsA, StaticA),
            GameVersion.OR => GetCanBeCaptured(species, SlotsO, StaticO),
            _ => false,
        },
        8 => version switch
        {
            GameVersion.Any => GetCanBeCaptured(species, SlotsSW.Concat(SlotsSH), StaticSW.Concat(StaticSH)),
            GameVersion.SW => GetCanBeCaptured(species, SlotsSW, StaticSW),
            GameVersion.SH => GetCanBeCaptured(species, SlotsSH, StaticSH),
            _ => false,
        },
        _ => false,
    };

    private static bool GetCanBeCaptured(int species, IEnumerable<EncounterArea> area, IEnumerable<EncounterStatic> statics)
    {
        if (area.Any(loc => loc.HasSpecies(species)))
            return true;
        if (statics.Any(enc => enc.Species == species && !enc.Gift))
            return true;
        return false;
    }

    public static bool GetCanDynamaxTrainer(int species, int gen, GameVersion version)
    {
        if (gen != 8)
            return false;

        return version switch
        {
            GameVersion.SW => DynamaxTrainer_SWSH.Contains(species) || IsDynamaxSW(species),
            GameVersion.SH => DynamaxTrainer_SWSH.Contains(species) || IsDynamaxSH(species),
            _              => DynamaxTrainer_SWSH.Contains(species) || IsDynamaxSW(species) || IsDynamaxSH(species),
        };
    }

    // exclusive to version
    private static bool IsDynamaxSW(int species) => species is (int)Machamp or (int)Gigalith or (int)Conkeldurr or (int)Coalossal or (int)Flapple;
    private static bool IsDynamaxSH(int species) => species is (int)Gengar or (int)Lapras or (int)Dusknoir or (int)Froslass or (int)Appletun;

    // common to SW & SH
    private static readonly HashSet<int> DynamaxTrainer_SWSH = new()
    {
        (int)Venusaur,
        (int)Blastoise,
        (int)Charizard,
        (int)Slowbro,
        (int)Gyarados,
        (int)Snorlax,
        (int)Slowking,
        (int)Torkoal,
        (int)Vespiquen,
        (int)Regigigas,
        (int)Garbodor,
        (int)Haxorus,
        (int)Tsareena,
        (int)Rillaboom,
        (int)Inteleon,
        (int)Cinderace,
        (int)Greedent,
        (int)Corviknight,
        (int)Eldegoss,
        (int)Drednaw,
        (int)Centiskorch,
        (int)Hatterene,
        (int)Grimmsnarl,
        (int)Alcremie,
        (int)Copperajah,
        (int)Duraludon,
        (int)Urshifu,
    };

    public static bool GetCanFishSpecies(int species, int gen, GameVersion version) => gen switch
    {
        6 => version switch
        {
            GameVersion.Any => FishingSpecies_XY.Contains(species) || FishingSpecies_AO.Contains(species)
                                                                   || IsFishingSpeciesX(species) || IsFishingSpeciesY(species),

            GameVersion.X => FishingSpecies_XY.Contains(species) || IsFishingSpeciesX(species),
            GameVersion.Y => FishingSpecies_XY.Contains(species) || IsFishingSpeciesY(species),

            GameVersion.OR or GameVersion.AS => FishingSpecies_AO.Contains(species),
            _ => false,
        },
        8 => version switch
        {
            GameVersion.Any or GameVersion.SW or GameVersion.SH => FishingSpecies_SWSH.Contains(species),
            _ => false,
        },
        _ => false,
    };

    private static readonly HashSet<int> FishingSpecies_SWSH = new()
    {
        (int)Shellder, (int)Cloyster,
        (int)Krabby,
        (int)Goldeen,
        (int)Magikarp, (int)Gyarados,
        (int)Lapras,
        (int)Dratini,
        (int)Chinchou, (int)Lanturn,
        (int)Qwilfish,
        (int)Remoraid, (int)Octillery,
        (int)Carvanha, (int)Sharpedo,
        (int)Wailmer, (int)Wailord,
        (int)Barboach, (int)Whiscash,
        (int)Corphish,
        (int)Lileep,
        (int)Feebas,
        (int)Mantyke, (int)Mantine,
        (int)Basculin,
        (int)Wishiwashi,
        (int)Mareanie,
        (int)Pyukumuku,
        (int)Dhelmise,
        (int)Chewtle, (int)Drednaw,
        (int)Arrokuda, (int)Barraskewda,
    };

    private static readonly HashSet<int> FishingSpecies_AO = new()
    {
        (int)Tentacool,
        (int)Horsea, (int)Seadra,
        (int)Goldeen, (int)Seaking,
        (int)Staryu,
        (int)Magikarp, (int)Gyarados,
        (int)Corsola,
        (int)Remoraid, (int)Octillery,
        (int)Carvanha, (int)Sharpedo,
        (int)Wailmer,
        (int)Barboach, (int)Whiscash,
        (int)Corphish, (int)Crawdaunt,
        (int)Feebas,
        (int)Luvdisc,
    };

    // exclusive to version
    private static bool IsFishingSpeciesX(int species) => species is (int)Staryu or (int)Starmie or (int)Huntail or (int)Clauncher or (int)Clawitzer;
    private static bool IsFishingSpeciesY(int species) => species is (int)Shellder or (int)Cloyster or (int)Gorebyss or (int)Skrelp or (int)Dragalge;

    // common to X & Y
    private static readonly HashSet<int> FishingSpecies_XY = new()
    {
        (int)Poliwag, (int)Poliwhirl, (int)Poliwrath, (int)Politoed,
        (int)Horsea, (int)Seadra,
        (int)Goldeen, (int)Seaking,
        (int)Magikarp, (int)Gyarados,
        (int)Dratini, (int)Dragonair,
        (int)Chinchou, (int)Lanturn,
        (int)Qwilfish,
        (int)Corsola,
        (int)Remoraid, (int)Octillery,
        (int)Carvanha, (int)Sharpedo,
        (int)Barboach, (int)Whiscash,
        (int)Corphish, (int)Crawdaunt,
        (int)Clamperl,
        (int)Relicanth,
        (int)Luvdisc,
        (int)Basculin,
        (int)Alomomola,
    };
}
