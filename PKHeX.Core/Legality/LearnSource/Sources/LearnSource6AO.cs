using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="ORAS"/>.
/// </summary>
public sealed class LearnSource6AO : ILearnSource, IEggSource
{
    public static readonly LearnSource6AO Instance = new();
    private static readonly PersonalTable6AO Personal = PersonalTable.AO;
    private static readonly Learnset[] Learnsets = Legal.LevelUpAO;
    private static readonly EggMoves6[] EggMoves = Legal.EggMovesAO;
    private const int MaxSpecies = Legal.MaxSpeciesID_6;
    private const LearnEnvironment Game = ORAS;

    public Learnset GetLearnset(int species, int form) => Learnsets[Personal.GetFormIndex(species, form)];

    public bool TryGetPersonal(int species, int form, [NotNullWhen(true)] out PersonalInfo? pi)
    {
        pi = null;
        if ((uint)species > MaxSpecies)
            return false;
        pi = Personal[species];
        return true;
    }

    public bool GetIsEggMove(int species, int form, int move)
    {
        if ((uint)species > MaxSpecies)
            return false;
        var moves = EggMoves[species];
        return moves.GetHasEggMove(move);
    }

    public ReadOnlySpan<int> GetEggMoves(int species, int form)
    {
        if ((uint)species > MaxSpecies)
            return ReadOnlySpan<int>.Empty;
        return EggMoves[species].Moves;
    }

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo pi, EvoCriteria evo, int move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlagFast(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1 && level <= evo.LevelMax)
                return new(LevelUp, Game, (byte)level);
        }

        if (types.HasFlagFast(MoveSourceType.Machine) && GetIsTM(pi, move))
            return new(TMHM, Game);

        if (types.HasFlagFast(MoveSourceType.TypeTutor) && GetIsTypeTutor(pi, move))
            return new(Tutor, Game);

        if (types.HasFlagFast(MoveSourceType.SpecialTutor) && GetIsSpecialTutor(pi, move))
            return new(Tutor, Game);

        if (types.HasFlagFast(MoveSourceType.EnhancedTutor) && GetIsEnhancedTutor(evo, pk, move, option))
            return new(Tutor, Game);

        return default;
    }

    private static bool GetIsEnhancedTutor(EvoCriteria evo, ISpeciesForm current, int move, LearnOption option) => evo.Species switch
    {
        (int)Species.Keldeo => move is (int)Move.SecretSword,
        (int)Species.Meloetta => move is (int)Move.RelicSong,
        (int)Species.Rotom => move switch
        {
            (int)Move.Overheat  => option == LearnOption.AtAnyTime || current.Form == 1,
            (int)Move.HydroPump => option == LearnOption.AtAnyTime || current.Form == 2,
            (int)Move.Blizzard  => option == LearnOption.AtAnyTime || current.Form == 3,
            (int)Move.AirSlash  => option == LearnOption.AtAnyTime || current.Form == 4,
            (int)Move.LeafStorm => option == LearnOption.AtAnyTime || current.Form == 5,
            _ => false,
        },
        _ => false,
    };

    private static bool GetIsTypeTutor(PersonalInfo pi, int move)
    {
        var tutors = Tutors_AO;
        for (int i = 0; i < tutors.Length; i++)
        {
            var tutor = Array.IndexOf(tutors[i], move);
            if (tutor == -1)
                continue;
            if (pi.SpecialTutors[i][tutor])
                return true;
            break;
        }

        var index = Array.IndexOf(LearnSource5.TypeTutor567, move);
        if (index == -1)
            return false;
        return pi.TypeTutors[index];
    }

    private static bool GetIsSpecialTutor(PersonalInfo pi, int move)
    {
        var tutors = Tutors_AO;
        for (int i = 0; i < tutors.Length; i++)
        {
            var index = Array.IndexOf(tutors[i], move);
            if (index == -1)
                continue;
            return pi.SpecialTutors[i][index];
        }
        return false;
    }

    private static bool GetIsTM(PersonalInfo info, int move)
    {
        var index = Array.IndexOf(TMHM_AO, move);
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

        if (types.HasFlagFast(MoveSourceType.Machine))
        {
            var flags = pi.TMHM;
            var moves = TMHM_AO;
            for (int i = 0; i < moves.Length; i++)
            {
                if (flags[i])
                    result[moves[i]] = true;
            }
        }

        if (types.HasFlagFast(MoveSourceType.TypeTutor))
        {
            // Beams
            var flags = pi.TypeTutors;
            var moves = LearnSource5.TypeTutor567;
            for (int i = 0; i < moves.Length; i++)
            {
                if (flags[i])
                    result[moves[i]] = true;
            }
        }

        if (types.HasFlagFast(MoveSourceType.SpecialTutor))
        {
            // OR/AS Tutors
            var tutors = Tutors_AO;
            for (int i = 0; i < tutors.Length; i++)
            {
                var flags = pi.SpecialTutors[i];
                var moves = tutors[i];
                for (int m = 0; m < moves.Length; m++)
                {
                    if (flags[m])
                        result[moves[m]] = true;
                }
            }
        }

        if (types.HasFlagFast(MoveSourceType.EnhancedTutor))
        {
            var species = evo.Species;
            if (species is (int)Species.Rotom && evo.Form is not 0)
                result[MoveTutor.GetRotomFormMove(evo.Form)] = true;
            else if (species is (int)Species.Keldeo)
                result[(int)Move.SecretSword] = true;
            else if (species is (int)Species.Meloetta)
                result[(int)Move.RelicSong] = true;
        }
    }

    internal static readonly int[] TMHM_AO =
    {
        468, 337, 473, 347, 046, 092, 258, 339, 474, 237,
        241, 269, 058, 059, 063, 113, 182, 240, 355, 219,
        218, 076, 479, 085, 087, 089, 216, 091, 094, 247,
        280, 104, 115, 482, 053, 188, 201, 126, 317, 332,
        259, 263, 488, 156, 213, 168, 490, 496, 497, 315,
        211, 411, 412, 206, 503, 374, 451, 507, 510, 511,
        261, 512, 373, 153, 421, 371, 514, 416, 397, 148,
        444, 521, 086, 360, 014, 522, 244, 523, 524, 157,
        404, 525, 611, 398, 138, 447, 207, 214, 369, 164,
        430, 433, 528, 290, 555, 267, 399, 612, 605, 590,

        15, 19, 57, 70, 127, 249, 291,
    };

    private static readonly int[][] Tutors_AO =
    {
        new[] {450, 343, 162, 530, 324, 442, 402, 529, 340, 067, 441, 253, 009, 007, 008},
        new[] {277, 335, 414, 492, 356, 393, 334, 387, 276, 527, 196, 401, 399, 428, 406, 304, 231},
        new[] {020, 173, 282, 235, 257, 272, 215, 366, 143, 220, 202, 409, 355, 264, 351, 352},
        new[] {380, 388, 180, 495, 270, 271, 478, 472, 283, 200, 278, 289, 446, 214, 285},
    };
}
