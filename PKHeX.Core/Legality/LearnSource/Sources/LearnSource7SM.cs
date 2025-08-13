using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;
using static PKHeX.Core.PersonalInfo7;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="SM"/>.
/// </summary>
public sealed class LearnSource7SM : ILearnSource<PersonalInfo7>, IEggSource
{
    public static readonly LearnSource7SM Instance = new();
    private static readonly PersonalTable7 Personal = PersonalTable.SM;
    private static readonly Learnset[] Learnsets = LearnsetReader.GetArray(BinLinkerAccessor16.Get(Util.GetBinaryResource("lvlmove_sm.pkl"), "sm"u8));
    private static readonly MoveSource[] EggMoves = MoveSource.GetArray(BinLinkerAccessor16.Get(Util.GetBinaryResource("eggmove_sm.pkl"), "sm"u8));
    private const int MaxSpecies = Legal.MaxSpeciesID_7;
    private const LearnEnvironment Game = SM;
    private const int ReminderBonus = Experience.MaxLevel; // Move reminder allows re-learning ALL level up moves regardless of level.

    public LearnEnvironment Environment => Game;

    public Learnset GetLearnset(ushort species, byte form) => Learnsets[Personal.GetFormIndex(species, form)];

    public bool TryGetPersonal(ushort species, byte form, [NotNullWhen(true)] out PersonalInfo7? pi)
    {
        pi = null;
        if (species > MaxSpecies)
            return false;
        pi = Personal[species, form];
        return true;
    }

    public bool GetIsEggMove(ushort species, byte form, ushort move)
    {
        var index = Personal.GetFormIndex(species, form);
        if (index >= EggMoves.Length)
            return false;
        var moves = EggMoves[index];
        return moves.GetHasMove(move);
    }

    public ReadOnlySpan<ushort> GetEggMoves(ushort species, byte form)
    {
        var index = Personal.GetFormIndex(species, form);
        if (index >= EggMoves.Length)
            return [];
        return EggMoves[index].Moves;
    }

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo7 pi, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            if (learn.TryGetLevelLearnMove(move, out var level)) // Can relearn at any level!
                return new(LevelUp, Game, level);
        }

        if (types.HasFlag(MoveSourceType.Machine) && pi.GetIsLearnTM(MachineMoves.IndexOf(move)))
            return new(TMHM, Game);

        if (types.HasFlag(MoveSourceType.TypeTutor) && pi.GetIsLearnTutorType(PersonalInfo5BW.TypeTutorMoves.IndexOf(move)))
            return new(Tutor, Game);

        if (types.HasFlag(MoveSourceType.EnhancedTutor) && GetIsEnhancedTutor(evo, pk, move, option))
            return new(Tutor, Game);

        return default;
    }

    private static bool GetIsEnhancedTutor<T1, T2>(T1 evo, T2 current, ushort move, LearnOption option)
        where T1 : ISpeciesForm
        where T2 : ISpeciesForm
        => evo.Species switch
    {
        (int)Species.Pikachu or (int)Species.Raichu => move is (int)Move.VoltTackle,
        (int)Species.Necrozma => move switch
        {
            (int)Move.SunsteelStrike => option.IsPast() || current.Form == 1, // Sun w/ Solgaleo
            (int)Move.MoongeistBeam  => option.IsPast() || current.Form == 2, // Moon w/ Lunala
            _ => false,
        },
        (int)Species.Keldeo   => move is (int)Move.SecretSword,
        (int)Species.Meloetta => move is (int)Move.RelicSong,
        (int)Species.Rotom => move switch
        {
            (int)Move.Overheat  => option.IsPast() || current.Form == 1,
            (int)Move.HydroPump => option.IsPast() || current.Form == 2,
            (int)Move.Blizzard  => option.IsPast() || current.Form == 3,
            (int)Move.AirSlash  => option.IsPast() || current.Form == 4,
            (int)Move.LeafStorm => option.IsPast() || current.Form == 5,
            _ => false,
        },
        (int)Species.Zygarde => move is (int)Move.ExtremeSpeed or (int)Move.DragonDance or (int)Move.ThousandArrows or (int)Move.ThousandWaves or (int)Move.CoreEnforcer,
        _ => false,
    };

    public void GetAllMoves(Span<bool> result, PKM _, EvoCriteria evo, MoveSourceType types = MoveSourceType.All)
    {
        if (!TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var span = learn.GetMoveRange(ReminderBonus);
            foreach (var move in span)
                result[move] = true;
        }

        if (types.HasFlag(MoveSourceType.Machine))
            pi.SetAllLearnTM(result, MachineMoves);

        if (types.HasFlag(MoveSourceType.TypeTutor))
            pi.SetAllLearnTutorType(result, PersonalInfo5BW.TypeTutorMoves);

        if (types.HasFlag(MoveSourceType.EnhancedTutor))
        {
            var species = evo.Species;
            if (species is (int)Species.Zygarde)
            {
                result[(int)Move.CoreEnforcer] = true;
                result[(int)Move.ExtremeSpeed] = true;
                result[(int)Move.DragonDance] = true;
                result[(int)Move.ThousandArrows] = true;
                result[(int)Move.ThousandWaves] = true;
            }

            if (species is (int)Species.Rotom && evo.Form is not 0)
                result[MoveTutor.GetRotomFormMove(evo.Form)] = true;
            else if (species is (int)Species.Pikachu or (int)Species.Raichu) // Gen7 only Volt Tackle tutor
                result[(int)Move.VoltTackle] = true;
            else if (species is (int)Species.Keldeo)
                result[(int)Move.SecretSword] = true;
            else if (species is (int)Species.Meloetta)
                result[(int)Move.RelicSong] = true;
            else if (species is (int)Species.Necrozma && evo.Form is 1) // Sun
                result[(int)Move.SunsteelStrike] = true;
            else if (species is (int)Species.Necrozma && evo.Form is 2) // Moon
                result[(int)Move.MoongeistBeam] = true;
        }
    }
}
