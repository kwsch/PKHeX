using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="USUM"/>.
/// </summary>
public sealed class LearnSource7USUM : ILearnSource, IEggSource
{
    public static readonly LearnSource7USUM Instance = new();
    private static readonly PersonalTable Personal = PersonalTable.USUM;
    private static readonly Learnset[] Learnsets = Legal.LevelUpUSUM;
    private static readonly EggMoves7[] EggMoves = Legal.EggMovesUSUM;
    private const int MaxSpecies = Legal.MaxSpeciesID_7_USUM;
    private const GameVersion Game = USUM;

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
            if (level != -1) // Can relearn at any level!
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

    private static bool GetIsTypeTutor(PersonalInfo pi, int move)
    {
        var index = Array.IndexOf(Legal.TypeTutor6, move);
        if (index == -1)
            return false;
        return pi.TypeTutors[index];
    }

    private static bool GetIsSpecialTutor(PersonalInfo pi, int move)
    {
        // US/UM Tutors
        var tutors = Legal.Tutors_USUM;
        var tutor = Array.IndexOf(tutors, move);
        if (tutor == -1)
            return false;
        return pi.SpecialTutors[0][tutor];
    }

    private static bool GetIsTM(PersonalInfo info, int move)
    {
        var index = Array.IndexOf(Legal.TMHM_SM, move);
        if (index == -1)
            return false;
        return info.TMHM[index];
    }

    public IEnumerable<int> GetAllMoves(PKM pk, EvoCriteria evo, MoveSourceType types = MoveSourceType.All)
    {
        if (!TryGetPersonal(evo.Species, evo.Form, out var pi))
            yield break;

        if (types.HasFlagFast(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            foreach (var move in learn.GetMoves(evo.LevelMin, 100))
                yield return move;
        }

        if (types.HasFlagFast(MoveSourceType.Machine))
        {
            var permit = pi.TMHM;
            var moveIDs = Legal.TMHM_SM;
            for (int i = 0; i < moveIDs.Length; i++)
            {
                if (permit[i])
                    yield return moveIDs[i];
            }
        }

        if (types.HasFlagFast(MoveSourceType.TypeTutor))
        {
            // Beams
            var permit = pi.TypeTutors;
            var moveIDs = Legal.TypeTutor6;
            for (int i = 0; i < moveIDs.Length; i++)
            {
                if (permit[i])
                    yield return moveIDs[i];
            }
        }

        if (types.HasFlagFast(MoveSourceType.SpecialTutor))
        {
            // US/UM Tutors
            var permit = pi.SpecialTutors[0];
            var moveIDs = Legal.Tutors_USUM;
            for (int i = 0; i < permit.Length; i++)
            {
                if (permit[i])
                    yield return moveIDs[i];
            }
        }

        if (types.HasFlagFast(MoveSourceType.EnhancedTutor))
        {
            var species = evo.Species;
            if (species is (int)Species.Zygarde)
            {
                yield return (int)Move.ExtremeSpeed;
                yield return (int)Move.DragonDance;
                yield return (int)Move.ThousandArrows;
                yield return (int)Move.ThousandWaves;
                yield return (int)Move.CoreEnforcer;
                yield break;
            }

            if (species is (int)Species.Rotom && evo.Form is not 0)
                yield return MoveTutor.GetRotomFormMove(evo.Form);
            else if (species is (int)Species.Pikachu or (int)Species.Raichu) // Gen7 only Volt Tackle tutor
                yield return (int)Move.VoltTackle;
            else if (species is (int)Species.Keldeo)
                yield return (int)Move.SecretSword;
            else if (species is (int)Species.Meloetta)
                yield return (int)Move.RelicSong;
            else if (species is (int)Species.Necrozma && pk.Form is 1) // Sun
                yield return (int)Move.SunsteelStrike;
            else if (species is (int)Species.Necrozma && pk.Form is 2) // Moon
                yield return (int)Move.MoongeistBeam;
        }
    }
}
