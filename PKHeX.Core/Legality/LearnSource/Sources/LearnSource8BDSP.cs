using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="BDSP"/>.
/// </summary>
public sealed class LearnSource8BDSP : ILearnSource, IEggSource
{
    public static readonly LearnSource8BDSP Instance = new();
    private static readonly PersonalTable8BDSP Personal = PersonalTable.BDSP;
    private static readonly Learnset[] Learnsets = Legal.LevelUpBDSP;
    private static readonly EggMoves6[] EggMoves = Legal.EggMovesBDSP;
    private const int MaxSpecies = Legal.MaxSpeciesID_8b;
    private const LearnEnvironment Game = BDSP;

    public Learnset GetLearnset(ushort species, byte form) => Learnsets[Personal.GetFormIndex(species, form)];

    public bool TryGetPersonal(ushort species, byte form, [NotNullWhen(true)] out PersonalInfo? pi)
    {
        pi = null;
        if ((uint)species > MaxSpecies)
            return false;
        pi = Personal[species, form];
        return true;
    }

    public bool GetIsEggMove(ushort species, byte form, ushort move)
    {
        // Array is optimized to not have entries for species above 460 (not able to breed / no egg moves).
        var arr = EggMoves;
        if (species >= arr.Length)
            return false;
        var moves = arr[species];
        return moves.GetHasEggMove(move);
    }

    public ReadOnlySpan<ushort> GetEggMoves(ushort species, byte form)
    {
        // Array is optimized to not have entries for species above 460 (not able to breed / no egg moves).
        var arr = EggMoves;
        if (species >= arr.Length)
            return ReadOnlySpan<ushort>.Empty;
        return arr[species].Moves;
    }

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo pi, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlagFast(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1 && level <= evo.LevelMax)
                return new(LevelUp, Game, (byte)level);
        }

        if (types.HasFlagFast(MoveSourceType.SharedEggMove) && GetIsSharedEggMove(pi, move))
            return new(Shared, Game);

        if (types.HasFlagFast(MoveSourceType.Machine) && GetIsTM(pi, move))
            return new(TMHM, Game);

        if (types.HasFlagFast(MoveSourceType.TypeTutor) && GetIsTypeTutor(pi, move))
            return new(Tutor, Game);

        if (types.HasFlagFast(MoveSourceType.EnhancedTutor) && GetIsEnhancedTutor(evo, pk, move, option))
            return new(Tutor, Game);

        return default;
    }

    private static bool GetIsEnhancedTutor(EvoCriteria evo, ISpeciesForm current, ushort move, LearnOption option) => evo.Species is (int)Species.Rotom && move switch
    {
        (int)Move.Overheat  => option == LearnOption.AtAnyTime || current.Form == 1,
        (int)Move.HydroPump => option == LearnOption.AtAnyTime || current.Form == 2,
        (int)Move.Blizzard  => option == LearnOption.AtAnyTime || current.Form == 3,
        (int)Move.AirSlash  => option == LearnOption.AtAnyTime || current.Form == 4,
        (int)Move.LeafStorm => option == LearnOption.AtAnyTime || current.Form == 5,
        _ => false,
    };

    private bool GetIsSharedEggMove(PersonalInfo pi, ushort move)
    {
        var entry = (PersonalInfo8BDSP)pi;
        var baseSpecies = entry.HatchSpecies;
        var baseForm = entry.HatchFormIndex;
        return GetEggMoves(baseSpecies, baseForm).IndexOf(move) != -1;
    }

    private static bool GetIsTypeTutor(PersonalInfo pi, ushort move)
    {
        var index = Array.IndexOf(TypeTutor8b, move);
        if (index == -1)
            return false;
        return pi.TypeTutors[index];
    }

    private static bool GetIsTM(PersonalInfo info, ushort move)
    {
        var index = Array.IndexOf(TMHM_BDSP, move);
        if (index == -1)
            return false;
        return info.TMHM[index];
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types = MoveSourceType.All)
    {
        if (!TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        if (types.HasFlagFast(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            (bool hasMoves, int start, int end) = learn.GetMoveRange(evo.LevelMax);
            if (hasMoves)
            {
                var moves = learn.Moves;
                for (int i = end; i >= start; i--)
                    result[moves[i]] = true;
            }
        }

        if (types.HasFlagFast(MoveSourceType.SharedEggMove))
        {
            var entry = (PersonalInfo8BDSP)pi;
            var baseSpecies = entry.HatchSpecies;
            var baseForm = entry.HatchFormIndex;
            var egg = GetEggMoves(baseSpecies, baseForm);
            foreach (var move in egg)
                result[move] = true;
        }

        if (types.HasFlagFast(MoveSourceType.Machine))
        {
            var flags = pi.TMHM;
            var moves = TMHM_BDSP;
            for (int i = 0; i < moves.Length; i++)
            {
                if (flags[i])
                    result[moves[i]] = true;
            }
        }

        if (types.HasFlagFast(MoveSourceType.TypeTutor))
        {
            var flags = pi.TypeTutors;
            var moves = TypeTutor8b;
            for (int i = 0; i < moves.Length; i++)
            {
                if (flags[i])
                    result[moves[i]] = true;
            }
        }

        if (types.HasFlagFast(MoveSourceType.EnhancedTutor))
        {
            var species = evo.Species;
            if (species is (int)Species.Rotom && evo.Form is not 0)
                result[MoveTutor.GetRotomFormMove(evo.Form)] = true;
        }
    }

    public static readonly ushort[] TMHM_BDSP =
    {
        264, 337, 352, 347, 046, 092, 258, 339, 331, 526,
        241, 269, 058, 059, 063, 113, 182, 240, 202, 219,
        605, 076, 231, 085, 087, 089, 490, 091, 094, 247,
        280, 104, 115, 351, 053, 188, 201, 126, 317, 332,
        259, 263, 521, 156, 213, 168, 211, 285, 503, 315,
        355, 411, 412, 206, 362, 374, 451, 203, 406, 409,
        261, 405, 417, 153, 421, 371, 278, 416, 397, 148,
        444, 419, 086, 360, 014, 446, 244, 555, 399, 157,
        404, 214, 523, 398, 138, 447, 207, 365, 369, 164,
        430, 433,
        015, 019, 057, 070, 432, 249, 127, 431,
    };

    internal static readonly ushort[] TypeTutor8b =
    {
        (int)Move.FrenzyPlant,
        (int)Move.BlastBurn,
        (int)Move.HydroCannon,
        (int)Move.DracoMeteor,
    };
}
