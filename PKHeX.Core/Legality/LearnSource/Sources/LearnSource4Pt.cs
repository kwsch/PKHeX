using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="Pt"/>.
/// </summary>
public sealed class LearnSource4Pt : LearnSource4, ILearnSource<PersonalInfo4>, IEggSource
{
    public static readonly LearnSource4Pt Instance = new();
    private static readonly PersonalTable4 Personal = PersonalTable.Pt;
    private static readonly Learnset[] Learnsets = LearnsetReader.GetArray(BinLinkerAccessor.Get(Util.GetBinaryResource("lvlmove_pt.pkl"), "pt"u8));
    private const int MaxSpecies = Legal.MaxSpeciesID_4;
    private const LearnEnvironment Game = Pt;
    private const byte Generation = 4;

    public LearnEnvironment Environment => Game;

    public Learnset GetLearnset(ushort species, byte form) => Learnsets[Personal.GetFormIndex(species, form)];

    public bool TryGetPersonal(ushort species, byte form, [NotNullWhen(true)] out PersonalInfo4? pi)
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
        var moves = EggMoves[species];
        return moves.GetHasEggMove(move);
    }

    public ReadOnlySpan<ushort> GetEggMoves(ushort species, byte form)
    {
        if (species > MaxSpecies)
            return [];
        return EggMoves[species].Moves;
    }

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo4 pi, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1 && level <= evo.LevelMax)
                return new(LevelUp, Game, (byte)level);
        }

        if (types.HasFlag(MoveSourceType.Machine))
        {
            if (GetIsTM(pi, move))
                return new(TMHM, Game);
            if ((move is (int)Move.Defog || pk.Format == Generation) && GetIsHM(pi, move))
                return new(TMHM, Game);
        }

        if (types.HasFlag(MoveSourceType.TypeTutor) && GetIsTypeTutor(evo.Species, move))
            return new(Tutor, Game);

        if (types.HasFlag(MoveSourceType.SpecialTutor) && GetIsSpecialTutor(pi, move))
            return new(Tutor, Game);

        if (types.HasFlag(MoveSourceType.EnhancedTutor) && GetIsEnhancedTutor(evo, pk, move, option))
            return new(Tutor, Game);

        return default;
    }

    private static bool GetIsEnhancedTutor(EvoCriteria evo, ISpeciesForm current, ushort move, LearnOption option) => evo.Species is (int)Species.Rotom && move switch
    {
        (int)Move.Overheat  => option.IsPast() || current.Form == 1,
        (int)Move.HydroPump => option.IsPast() || current.Form == 2,
        (int)Move.Blizzard  => option.IsPast() || current.Form == 3,
        (int)Move.AirSlash  => option.IsPast() || current.Form == 4,
        (int)Move.LeafStorm => option.IsPast() || current.Form == 5,
        _ => false,
    };

    private static bool GetIsTypeTutor(ushort species, ushort move) => move switch
    {
        (ushort)Move.BlastBurn => SpecialTutors_Compatibility_4_BlastBurn.Contains(species),
        (ushort)Move.HydroCannon => SpecialTutors_Compatibility_4_HydroCannon.Contains(species),
        (ushort)Move.FrenzyPlant => SpecialTutors_Compatibility_4_FrenzyPlant.Contains(species),
        (ushort)Move.DracoMeteor => SpecialTutors_Compatibility_4_DracoMeteor.Contains(species),
        _ => false,
    };

    private static bool GetIsSpecialTutor(PersonalInfo4 pi, ushort move)
    {
        var index = Tutors_4.IndexOf(move);
        if (index == -1)
            return false;
        return pi.TypeTutors[index];
    }

    private static bool GetIsTM(PersonalInfo4 info, ushort move)
    {
        var index = TM_4.IndexOf(move);
        return info.GetIsLearnTM(index);
    }

    private static bool GetIsHM(PersonalInfo4 info, ushort move)
    {
        var index = HM_DPPt.IndexOf(move);
        return info.GetIsLearnHM(index);
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types = MoveSourceType.All)
    {
        if (!TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var span = learn.GetMoveRange(evo.LevelMax);
            foreach (var move in span)
                result[move] = true;
        }

        if (types.HasFlag(MoveSourceType.Machine))
        {
            pi.SetAllLearnTM(result, TM_4);

            if (pk.Format == Generation)
                pi.SetAllLearnHM(result, HM_DPPt);
            else if (pi.GetIsLearnHM(4)) // Permit Defog to leak through if transferred to Gen5+ (via HG/SS)
                result[(int)Move.Defog] = true;
        }

        if (types.HasFlag(MoveSourceType.SpecialTutor))
        {
            var flags = pi.TypeTutors;
            var moves = Tutors_4;
            for (int i = 0; i < moves.Length; i++)
            {
                if (flags[i])
                    result[moves[i]] = true;
            }
        }

        if (types.HasFlag(MoveSourceType.EnhancedTutor))
        {
            if (evo.Species is (int)Species.Rotom && evo.Form is not 0)
                result[MoveTutor.GetRotomFormMove(evo.Form)] = true;
        }
    }
}
