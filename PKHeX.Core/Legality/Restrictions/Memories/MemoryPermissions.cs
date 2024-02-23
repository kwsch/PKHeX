using System;

using static PKHeX.Core.Move;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Validation logic for specific memory conditions
/// </summary>
public static class MemoryPermissions
{
    public static bool IsMemoryOfKnownMove(int memory) => memory is 48 or 80 or 81;

    public static bool CanWinLotoID(EntityContext context, int item)
    {
        var mem = Memories.GetContext(context);
        return mem.CanWinLotoID(item);
    }

    public static bool CanHoldItem(EntityContext context, int item)
    {
        var mem = Memories.GetContext(context);
        return mem.CanHoldItem(item);
    }

    public static bool CanPlantBerry(EntityContext context, int item)
    {
        var mem = Memories.GetContext(context);
        return mem.CanPlantBerry(item);
    }

    public static bool CanUseItemGeneric(EntityContext context, int item)
    {
        var mem = Memories.GetContext(context);
        return mem.CanUseItemGeneric(item);
    }

    public static bool CanUseItem(EntityContext context, int item, ushort species)
    {
        if (IsUsedKeyItemUnspecific(context, item))
            return true;
        if (IsUsedKeyItemSpecific(context, item, species))
            return true;
        return true; // todo
    }

    private static bool IsUsedKeyItemUnspecific(EntityContext context, int item)
    {
        var mem = Memories.GetContext(context);
        return mem.IsUsedKeyItemUnspecific(item);
    }

    private static bool IsUsedKeyItemSpecific(EntityContext context, int item, ushort species)
    {
        var mem = Memories.GetContext(context);
        return mem.IsUsedKeyItemSpecific(item, species);
    }

    public static bool CanBuyItem(EntityContext context, int item, GameVersion version = GameVersion.Any)
    {
        var mem = Memories.GetContext(context);
        return mem.CanBuyItem(item, version);
    }

    public static bool CanKnowMove(PKM pk, MemoryVariableSet memory, EntityContext gen, LegalInfo info, bool battleOnly = false)
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
            var learn = GameData.GetLearnSource(enc.Version);
            var em = learn.GetEggMoves(enc.Species, enc.Form);
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
            if (gen == EntityContext.Gen8 && MoveInfo.IsMoveDynamax(move))
                return true;
            if (pk.Species == (int)Ditto)
            {
                if (move == (int)Struggle)
                    return false;
                return gen switch
                {
                    EntityContext.Gen8 => move <= Legal.MaxMoveID_8_R2 && !MoveInfo.IsDummiedMove(pk, move),
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

    public static bool GetCanRelearnMove(PKM pk, ushort move, EntityContext context, EvolutionHistory history, IEncounterTemplate enc)
    {
        if (context == EntityContext.Gen6)
        {
            Span<MoveResult> result = stackalloc MoveResult[1];
            Span<ushort> moves = [move];
            LearnGroup6.Instance.Check(result, moves, pk, history, enc, MoveSourceType.Reminder, LearnOption.AtAnyTime);
            return result[0].Valid;
        }
        if (context == EntityContext.Gen8)
        {
            Span<MoveResult> result = stackalloc MoveResult[1];
            Span<ushort> moves = [move];
            LearnGroup8.Instance.Check(result, moves, pk, history, enc, MoveSourceType.Reminder, LearnOption.AtAnyTime);
            return result[0].Valid;
        }
        return false;
    }

    private static bool GetCanKnowMove(PKM pk, ushort move, EntityContext context, EvolutionHistory history, IEncounterTemplate enc)
    {
        if (pk.Species == (int)Smeargle)
            return MoveInfo.IsSketchValid(move, context);

        ILearnGroup game;
        if (context == EntityContext.Gen6)
            game = LearnGroup6.Instance;
        else if (context == EntityContext.Gen8)
            game = LearnGroup8.Instance;
        else
            return false;

        return GetCanKnowMove(enc, move, history, pk, game);
    }

    public static bool GetCanKnowMove(IEncounterTemplate enc, ushort move, EvolutionHistory history, PKM pk, ILearnGroup game)
    {
        Span<MoveResult> result = stackalloc MoveResult[1];
        Span<ushort> moves = [move];
        LearnVerifierHistory.MarkAndIterate(result, moves, enc, pk, history, game, MoveSourceType.All, LearnOption.AtAnyTime);
        return result[0].Valid;
    }

    public static bool GetCanBeCaptured(ushort species, EntityContext context, GameVersion version) => context switch
    {
        EntityContext.Gen6 => MemoryContext6.GetCanBeCaptured(species, version),
        EntityContext.Gen8 => MemoryContext8.GetCanBeCaptured(species, version),
        _ => false,
    };

    public static bool GetCanDynamaxTrainer(ushort species, byte generation, GameVersion version)
    {
        if (generation != 8)
            return false;

        return version switch
        {
            GameVersion.SW => IsDynamaxSWSH(species) || IsDynamaxSW(species),
            GameVersion.SH => IsDynamaxSWSH(species) || IsDynamaxSH(species),
            _ => IsDynamaxSWSH(species) || IsDynamaxSW(species) || IsDynamaxSH(species),
        };
    }

    /// <summary>
    /// Checks if the <see cref="species"/> can be Dynamaxed by a trainer available exclusively in Sword.
    /// </summary>
    private static bool IsDynamaxSW(ushort species) => species is (int)Machamp or (int)Gigalith or (int)Conkeldurr or (int)Coalossal or (int)Flapple;

    /// <summary>
    /// Checks if the <see cref="species"/> can be Dynamaxed by a trainer available exclusively in Sword.
    /// </summary>
    private static bool IsDynamaxSH(ushort species) => species is (int)Gengar or (int)Lapras or (int)Dusknoir or (int)Froslass or (int)Appletun;

    /// <summary>
    /// Checks if the <see cref="species"/> can be Dynamaxed by a trainer available in both Sword and Shield.
    /// </summary>
    private static bool IsDynamaxSWSH(ushort species) => species is
        (int)Venusaur or
        (int)Blastoise or
        (int)Charizard or
        (int)Slowbro or
        (int)Gyarados or
        (int)Snorlax or
        (int)Slowking or
        (int)Torkoal or
        (int)Vespiquen or
        (int)Regigigas or
        (int)Garbodor or
        (int)Haxorus or
        (int)Tsareena or
        (int)Rillaboom or
        (int)Inteleon or
        (int)Cinderace or
        (int)Greedent or
        (int)Corviknight or
        (int)Eldegoss or
        (int)Drednaw or
        (int)Centiskorch or
        (int)Hatterene or
        (int)Grimmsnarl or
        (int)Alcremie or
        (int)Copperajah or
        (int)Duraludon or
        (int)Urshifu
    ;

    public static bool GetCanFishSpecies(ushort species, EntityContext context, GameVersion version) => context switch
    {
        EntityContext.Gen6 => version switch
        {
            GameVersion.Any => IsFishingSpeciesXY(species) || IsFishingSpeciesAO(species)
                             || IsFishingSpeciesX(species) || IsFishingSpeciesY(species),

            GameVersion.X => IsFishingSpeciesXY(species) || IsFishingSpeciesX(species),
            GameVersion.Y => IsFishingSpeciesXY(species) || IsFishingSpeciesY(species),

            GameVersion.OR or GameVersion.AS => IsFishingSpeciesAO(species),
            _ => false,
        },
        EntityContext.Gen8 => version switch
        {
            GameVersion.Any or GameVersion.SW or GameVersion.SH => IsFishingSpeciesSWSH(species),
            _ => false,
        },
        _ => false,
    };

    /// <summary>
    /// Checks if the <see cref="species"/> can be fished in either <see cref="GameVersion.SW"/> or <see cref="GameVersion.SH"/>.
    /// </summary>
    private static bool IsFishingSpeciesSWSH(ushort species) => species is
        (int)Shellder or (int)Cloyster or
        (int)Krabby or
        (int)Goldeen or
        (int)Magikarp or (int)Gyarados or
        (int)Lapras or
        (int)Dratini or
        (int)Chinchou or (int)Lanturn or
        (int)Qwilfish or
        (int)Remoraid or (int)Octillery or
        (int)Carvanha or (int)Sharpedo or
        (int)Wailmer or (int)Wailord or
        (int)Barboach or (int)Whiscash or
        (int)Corphish or
        (int)Lileep or
        (int)Feebas or
        (int)Mantyke or (int)Mantine or
        (int)Basculin or
        (int)Wishiwashi or
        (int)Mareanie or
        (int)Pyukumuku or
        (int)Dhelmise or
        (int)Chewtle or (int)Drednaw or
        (int)Arrokuda or (int)Barraskewda
    ;

    /// <summary>
    /// Checks if the <see cref="species"/> can be fished in either <see cref="GameVersion.AS"/> or <see cref="GameVersion.OR"/>.
    /// </summary>
    public static bool IsFishingSpeciesAO(ushort species) => species is
        (int)Tentacool or
        (int)Horsea or (int)Seadra or
        (int)Goldeen or (int)Seaking or
        (int)Staryu or
        (int)Magikarp or (int)Gyarados or
        (int)Corsola or
        (int)Remoraid or (int)Octillery or
        (int)Carvanha or (int)Sharpedo or
        (int)Wailmer or
        (int)Barboach or (int)Whiscash or
        (int)Corphish or (int)Crawdaunt or
        (int)Feebas or
        (int)Luvdisc
    ;

    /// <summary>
    /// Checks if the <see cref="species"/> can be fished in only <see cref="GameVersion.X"/>.
    /// </summary>
    public static bool IsFishingSpeciesX(ushort species) => species is (int)Staryu or (int)Starmie or (int)Huntail or (int)Clauncher or (int)Clawitzer;

    /// <summary>
    /// Checks if the <see cref="species"/> can be fished in only <see cref="GameVersion.Y"/>.
    /// </summary>
    public static bool IsFishingSpeciesY(ushort species) => species is (int)Shellder or (int)Cloyster or (int)Gorebyss or (int)Skrelp or (int)Dragalge;

    /// <summary>
    /// Checks if the <see cref="species"/> can be fished in both <see cref="GameVersion.X"/> and <see cref="GameVersion.Y"/>.
    /// </summary>
    public static bool IsFishingSpeciesXY(ushort species) => species is
        (int)Poliwag or (int)Poliwhirl or (int)Poliwrath or (int)Politoed or
        (int)Horsea or (int)Seadra or
        (int)Goldeen or (int)Seaking or
        (int)Magikarp or (int)Gyarados or
        (int)Dratini or (int)Dragonair or
        (int)Chinchou or (int)Lanturn or
        (int)Qwilfish or
        (int)Corsola or
        (int)Remoraid or (int)Octillery or
        (int)Carvanha or (int)Sharpedo or
        (int)Barboach or (int)Whiscash or
        (int)Corphish or (int)Crawdaunt or
        (int)Clamperl or
        (int)Relicanth or
        (int)Luvdisc or
        (int)Basculin or
        (int)Alomomola
    ;
}
