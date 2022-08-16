using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="SWSH"/>.
/// </summary>
public sealed class LearnSource8SWSH : ILearnSource, IEggSource
{
    public static readonly LearnSource8SWSH Instance = new();
    private static readonly PersonalTable8SWSH Personal = PersonalTable.SWSH;
    private static readonly Learnset[] Learnsets = Legal.LevelUpSWSH;
    private static readonly EggMoves7[] EggMoves = Legal.EggMovesSWSH;
    private const int MaxSpecies = Legal.MaxSpeciesID_8_R2;
    private const LearnEnvironment Game = SWSH;

    public Learnset GetLearnset(int species, int form) => Learnsets[Personal.GetFormIndex(species, form)];

    public bool TryGetPersonal(int species, int form, [NotNullWhen(true)] out PersonalInfo? pi)
    {
        pi = null;
        if ((uint)species > MaxSpecies)
            return false;
        pi = Personal[species, form];
        return true;
    }

    public bool GetIsEggMove(int species, int form, int move)
    {
        if ((uint)species > MaxSpecies)
            return false;
        var moves = MoveEgg.GetFormEggMoves(species, form, EggMoves).AsSpan();
        return moves.IndexOf(move) != -1;
    }

    public ReadOnlySpan<int> GetEggMoves(int species, int form)
    {
        if ((uint)species > MaxSpecies)
            return ReadOnlySpan<int>.Empty;
        return MoveEgg.GetFormEggMoves(species, form, EggMoves).AsSpan();
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

        if (types.HasFlagFast(MoveSourceType.SharedEggMove) && GetIsSharedEggMove(pi, move))
            return new(Shared, Game);

        if (types.HasFlagFast(MoveSourceType.Machine) && GetIsTM(pi, move))
            return new(TMHM, Game);

        if (types.HasFlagFast(MoveSourceType.TechnicalRecord) && GetIsTR(pi, pk, evo, move, option))
            return new(TMHM, Game);

        if (types.HasFlagFast(MoveSourceType.TypeTutor) && GetIsTypeTutor(pi, move))
            return new(Tutor, Game);

        if (types.HasFlagFast(MoveSourceType.SpecialTutor) && GetIsSpecialTutor(pi, move))
            return new(Tutor, Game);

        if (types.HasFlagFast(MoveSourceType.EnhancedTutor) && GetIsEnhancedTutor(evo, pk, move, option))
            return new(Tutor, Game);

        return default;
    }

    private static bool GetIsSpecialTutor(PersonalInfo pi, int move)
    {
        var tutor = Array.IndexOf(Tutors_SWSH, move);
        if (tutor == -1)
            return false;
        return pi.SpecialTutors[0][tutor];
    }

    private static bool GetIsEnhancedTutor(EvoCriteria evo, ISpeciesForm current, int move, LearnOption option) => evo.Species switch
    {
        (int)Species.Necrozma => move switch
        {
            (int)Move.SunsteelStrike => (option == LearnOption.AtAnyTime || current.Form == 1), // Sun w/ Solgaleo
            (int)Move.MoongeistBeam => (option == LearnOption.AtAnyTime || current.Form == 2), // Moon w/ Lunala
            _ => false,
        },
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

    private bool GetIsSharedEggMove(PersonalInfo pi, int move)
    {
        var entry = (PersonalInfo8SWSH)pi;
        var baseSpecies = entry.HatchSpecies;
        var baseForm = entry.HatchFormIndexEverstone;
        return GetEggMoves(baseSpecies, baseForm).IndexOf(move) != -1;
    }

    private static bool GetIsTypeTutor(PersonalInfo pi, int move)
    {
        var index = Array.IndexOf(TypeTutor8, move);
        if (index == -1)
            return false;
        return pi.TypeTutors[index];
    }

    private static bool GetIsTM(PersonalInfo info, int move)
    {
        var index = TM_SWSH.AsSpan().IndexOf(move);
        if (index == -1)
            return false;
        return info.TMHM[index];
    }

    private static bool GetIsTR(PersonalInfo info, PKM pk, EvoCriteria evo, int move, LearnOption option)
    {
        if (pk is not ITechRecord8 tr)
            return false;

        var index = TR_SWSH.AsSpan().IndexOf(move);
        if (index == -1)
            return false;
        if (!info.TMHM[PersonalInfo8SWSH.CountTM + index])
            return false;

        if (tr.GetMoveRecordFlag(index))
            return true;

        if (option != LearnOption.Current && !pk.SWSH && pk.IsOriginalMovesetDeleted())
            return true;
        if (index == 12 && evo.Species == (int)Species.Calyrex && evo.Form == 0) // TR12
            return true; // Agility Calyrex without TR glitch.

        return false;
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
            var entry = (PersonalInfo8SWSH)pi;
            var baseSpecies = entry.HatchSpecies;
            var baseForm = entry.HatchFormIndexEverstone;
            var egg = GetEggMoves(baseSpecies, baseForm);
            foreach (var move in egg)
                result[move] = true;
        }

        if (types.HasFlagFast(MoveSourceType.Machine))
        {
            var flags = pi.TMHM;
            var moves = TM_SWSH;
            for (int i = 0; i < PersonalInfo8SWSH.CountTM; i++)
            {
                if (flags[i])
                    result[moves[i]] = true;
            }

            if (pk is ITechRecord8)
            {
                var trFlags = flags.AsSpan(PersonalInfo8SWSH.CountTM);
                var trMoves = TR_SWSH.AsSpan();
                for (int index = 0; index < trFlags.Length; index++)
                {
                    var move = trMoves[index];
                    if (trFlags[index])
                        result[move] = true;
                    else if (index == 12 && evo.Species == (int)Species.Calyrex && evo.Form == 0) // TR12
                        result[move] = true; // Agility Calyrex without TR glitch.
                }
            }
        }

        if (types.HasFlagFast(MoveSourceType.TypeTutor))
        {
            // Beams
            var flags = pi.TypeTutors;
            var moves = TypeTutor8;
            for (int i = 0; i < moves.Length; i++)
            {
                if (flags[i])
                    result[moves[i]] = true;
            }
        }

        if (types.HasFlagFast(MoveSourceType.SpecialTutor))
        {
            // SW/SH Tutors
            var flags = pi.SpecialTutors[0];
            var moves = Tutors_SWSH;
            for (int i = 0; i < flags.Length; i++)
            {
                if (flags[i])
                    result[moves[i]] = true;
            }
        }

        if (types.HasFlagFast(MoveSourceType.EnhancedTutor))
        {
            var species = evo.Species;
            if (species is (int)Species.Rotom && pk.Form is not 0)
                result[MoveTutor.GetRotomFormMove(evo.Form)] = true;
            else if (species is (int)Species.Necrozma && pk.Form is 1) // Sun
                result[(int)Move.SunsteelStrike] = true;
            else if (species is (int)Species.Necrozma && pk.Form is 2) // Moon
                result[(int)Move.MoongeistBeam] = true;
        }
    }

    public static readonly int[] TM_SWSH =
    {
        005, 025, 006, 007, 008, 009, 019, 042, 063, 416,
        345, 076, 669, 083, 086, 091, 103, 113, 115, 219,
        120, 156, 157, 168, 173, 182, 184, 196, 202, 204,
        211, 213, 201, 240, 241, 258, 250, 251, 261, 263,
        129, 270, 279, 280, 286, 291, 311, 313, 317, 328,
        331, 333, 340, 341, 350, 362, 369, 371, 372, 374,
        384, 385, 683, 409, 419, 421, 422, 423, 424, 427,
        433, 472, 478, 440, 474, 490, 496, 506, 512, 514,
        521, 523, 527, 534, 541, 555, 566, 577, 580, 581,
        604, 678, 595, 598, 206, 403, 684, 693, 707, 784,
    };

    internal static readonly int[] TR_SWSH =
    {
        014, 034, 053, 056, 057, 058, 059, 067, 085, 087,
        089, 094, 097, 116, 118, 126, 127, 133, 141, 161,
        164, 179, 188, 191, 200, 473, 203, 214, 224, 226,
        227, 231, 242, 247, 248, 253, 257, 269, 271, 276,
        285, 299, 304, 315, 322, 330, 334, 337, 339, 347,
        348, 349, 360, 370, 390, 394, 396, 398, 399, 402,
        404, 405, 406, 408, 411, 412, 413, 414, 417, 428,
        430, 437, 438, 441, 442, 444, 446, 447, 482, 484,
        486, 492, 500, 502, 503, 526, 528, 529, 535, 542,
        583, 599, 605, 663, 667, 675, 676, 706, 710, 776,
    };

    internal static readonly int[] TypeTutor8 =
    {
        (int)Move.GrassPledge,
        (int)Move.FirePledge,
        (int)Move.WaterPledge,
        (int)Move.FrenzyPlant,
        (int)Move.BlastBurn,
        (int)Move.HydroCannon,
        (int)Move.DracoMeteor,
        (int)Move.SteelBeam,
    };

    internal static readonly int[] Tutors_SWSH =
    {
        805, 807, 812, 804,
        803, 813, 811, 810,
        815, 814, 797, 806,
        800, 809, 799, 808,
        798, 802,
    };
}
