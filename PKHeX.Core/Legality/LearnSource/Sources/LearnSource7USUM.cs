using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;
using static PKHeX.Core.LearnSource7;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="USUM"/>.
/// </summary>
public sealed class LearnSource7USUM : ILearnSource<PersonalInfo7>, IEggSource
{
    public static readonly LearnSource7USUM Instance = new();
    private static readonly PersonalTable7 Personal = PersonalTable.USUM;
    private static readonly Learnset[] Learnsets = LearnsetReader.GetArray(BinLinkerAccessor.Get(Util.GetBinaryResource("lvlmove_uu.pkl"), "uu"u8));
    private static readonly EggMoves7[] EggMoves = EggMoves7.GetArray(BinLinkerAccessor.Get(Util.GetBinaryResource("eggmove_uu.pkl"), "uu"u8));
    private const int MaxSpecies = Legal.MaxSpeciesID_7_USUM;
    private const LearnEnvironment Game = USUM;
    private const int ReminderBonus = 100; // Move reminder allows re-learning ALL level up moves regardless of level.

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
        if (species > MaxSpecies)
            return false;
        var moves = EggMoves.GetFormEggMoves(species, form);
        return moves.Contains(move);
    }

    public ReadOnlySpan<ushort> GetEggMoves(ushort species, byte form)
    {
        if (species > MaxSpecies)
            return [];
        return EggMoves.GetFormEggMoves(species, form);
    }

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo7 pi, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1) // Can relearn at any level!
                return new(LevelUp, Game, 1);
        }

        if (types.HasFlag(MoveSourceType.Machine) && pi.GetIsLearnTM(TMHM_SM.IndexOf(move)))
            return new(TMHM, Game);

        if (types.HasFlag(MoveSourceType.TypeTutor) && pi.GetIsLearnTutorType(LearnSource5.TypeTutor567.IndexOf(move)))
            return new(Tutor, Game);

        if (types.HasFlag(MoveSourceType.SpecialTutor) && pi.GetIsLearnTutorSpecial(move))
            return new(Tutor, Game);

        if (types.HasFlag(MoveSourceType.EnhancedTutor) && GetIsEnhancedTutor(evo, pk, move, option))
            return new(Tutor, Game);

        return default;
    }

    private static bool GetIsEnhancedTutor(EvoCriteria evo, ISpeciesForm current, ushort move, LearnOption option) => evo.Species switch
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

    public void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types = MoveSourceType.All)
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
            pi.SetAllLearnTM(result, TMHM_SM);

        if (types.HasFlag(MoveSourceType.TypeTutor))
            pi.SetAllLearnTutorType(result, LearnSource5.TypeTutor567);

        if (types.HasFlag(MoveSourceType.SpecialTutor))
            pi.SetAllLearnTutorSpecial(result);

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
}
