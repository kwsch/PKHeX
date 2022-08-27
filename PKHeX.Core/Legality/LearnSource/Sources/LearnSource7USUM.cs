using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;
using static PKHeX.Core.LearnSource7;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="USUM"/>.
/// </summary>
public sealed class LearnSource7USUM : ILearnSource, IEggSource
{
    public static readonly LearnSource7USUM Instance = new();
    private static readonly PersonalTable7 Personal = PersonalTable.USUM;
    private static readonly Learnset[] Learnsets = Legal.LevelUpUSUM;
    private static readonly EggMoves7[] EggMoves = Legal.EggMovesUSUM;
    private const int MaxSpecies = Legal.MaxSpeciesID_7_USUM;
    private const LearnEnvironment Game = USUM;
    private const int ReminderBonus = 100; // Move reminder allows re-learning ALL level up moves regardless of level.

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
        if ((uint)species > MaxSpecies)
            return false;
        var moves = MoveEgg.GetFormEggMoves(species, form, EggMoves).AsSpan();
        return moves.IndexOf(move) != -1;
    }

    public ReadOnlySpan<ushort> GetEggMoves(ushort species, byte form)
    {
        if ((uint)species > MaxSpecies)
            return ReadOnlySpan<ushort>.Empty;
        return MoveEgg.GetFormEggMoves(species, form, EggMoves).AsSpan();
    }

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo pi, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlagFast(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1) // Can relearn at any level!
                return new(LevelUp, Game, 1);
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

    private static bool GetIsEnhancedTutor(EvoCriteria evo, ISpeciesForm current, ushort move, LearnOption option) => evo.Species switch
    {
        (int)Species.Pikachu or (int)Species.Raichu => move is (int)Move.VoltTackle,
        (int)Species.Necrozma => move switch
        {
            (int)Move.SunsteelStrike => (option == LearnOption.AtAnyTime || current.Form == 1), // Sun w/ Solgaleo
            (int)Move.MoongeistBeam  => (option == LearnOption.AtAnyTime || current.Form == 2), // Moon w/ Lunala
            _ => false,
        },
        (int)Species.Keldeo   => move is (int)Move.SecretSword,
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
        (int)Species.Zygarde => move is (int)Move.ExtremeSpeed or (int)Move.DragonDance or (int)Move.ThousandArrows or (int)Move.ThousandWaves or (int)Move.CoreEnforcer,
        _ => false,
    };

    private static bool GetIsTypeTutor(PersonalInfo pi, ushort move)
    {
        var index = Array.IndexOf(LearnSource5.TypeTutor567, move);
        if (index == -1)
            return false;
        return pi.TypeTutors[index];
    }

    private static bool GetIsSpecialTutor(PersonalInfo pi, ushort move)
    {
        // US/UM Tutors
        var tutors = Tutors_USUM;
        var tutor = Array.IndexOf(tutors, move);
        if (tutor == -1)
            return false;
        return pi.SpecialTutors[0][tutor];
    }

    private static bool GetIsTM(PersonalInfo info, ushort move)
    {
        var index = Array.IndexOf(TMHM_SM, move);
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
            (bool hasMoves, int start, int end) = learn.GetMoveRange(ReminderBonus);
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
            var moves = TMHM_SM;
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
            // US/UM Tutors
            var flags = pi.SpecialTutors[0];
            var moves = Tutors_USUM;
            for (int i = 0; i < flags.Length; i++)
            {
                if (flags[i])
                    result[moves[i]] = true;
            }
        }

        if (types.HasFlagFast(MoveSourceType.EnhancedTutor))
        {
            var species = evo.Species;
            if (species is (int)Species.Zygarde)
            {
                result[(int)Move.CoreEnforcer] = true;
                result[(int)Move.ExtremeSpeed] = true;
                result[(int)Move.DragonDance] = true;
                result[(int)Move.ThousandArrows] = true;
                result[(int)Move.ThousandWaves] = true;
                return;
            }

            if (species is (int)Species.Rotom && evo.Form is not 0)
                result[MoveTutor.GetRotomFormMove(evo.Form)] = true;
            else if (species is (int)Species.Pikachu or (int)Species.Raichu) // Gen7 only Volt Tackle tutor
                result[(int)Move.VoltTackle] = true;
            else if (species is (int)Species.Keldeo)
                result[(int)Move.SecretSword] = true;
            else if (species is (int)Species.Meloetta)
                result[(int)Move.RelicSong] = true;
            else if (species is (int)Species.Necrozma && pk.Form is 1) // Sun
                result[(int)Move.SunsteelStrike] = true;
            else if (species is (int)Species.Necrozma && pk.Form is 2) // Moon
                result[(int)Move.MoongeistBeam] = true;
        }
    }

    private static readonly ushort[] Tutors_USUM =
    {
        450, 343, 162, 530, 324, 442, 402, 529, 340, 067, 441, 253, 009, 007, 008,
        277, 335, 414, 492, 356, 393, 334, 387, 276, 527, 196, 401,      428, 406, 304, 231,
        020, 173, 282, 235, 257, 272, 215, 366, 143, 220, 202, 409,      264, 351, 352,
        380, 388, 180, 495, 270, 271, 478, 472, 283, 200, 278, 289, 446,      285,

        477, 502, 432, 710, 707, 675, 673,
    };
}
