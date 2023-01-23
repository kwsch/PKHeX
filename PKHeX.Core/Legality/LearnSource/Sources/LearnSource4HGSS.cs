using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;
using static PKHeX.Core.LearnSource4;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="HGSS"/>.
/// </summary>
public sealed class LearnSource4HGSS : ILearnSource<PersonalInfo4>, IEggSource
{
    public static readonly LearnSource4HGSS Instance = new();
    private static readonly PersonalTable4 Personal = PersonalTable.HGSS;
    private static readonly Learnset[] Learnsets = Legal.LevelUpHGSS;
    private static readonly EggMoves6[] EggMoves = Legal.EggMovesHGSS;
    private const int MaxSpecies = Legal.MaxSpeciesID_4;
    private const LearnEnvironment Game = HGSS;
    private const int Generation = 4;
    private const int CountTM = 92;

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
            return ReadOnlySpan<ushort>.Empty;
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
            if ((move is (int)Move.Whirlpool || pk.Format == Generation) && GetIsHM(pi, move))
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
        (int)Move.Overheat  => option == LearnOption.AtAnyTime || current.Form == 1,
        (int)Move.HydroPump => option == LearnOption.AtAnyTime || current.Form == 2,
        (int)Move.Blizzard  => option == LearnOption.AtAnyTime || current.Form == 3,
        (int)Move.AirSlash  => option == LearnOption.AtAnyTime || current.Form == 4,
        (int)Move.LeafStorm => option == LearnOption.AtAnyTime || current.Form == 5,
        _ => false,
    };

    private static bool GetIsTypeTutor(ushort species, ushort move)
    {
        var index = Array.IndexOf(SpecialTutors_4, move);
        if (index == -1)
            return false;
        var list = SpecialTutors_Compatibility_4[index];
        return Array.IndexOf(list, species) != -1;
    }

    private static bool GetIsSpecialTutor(PersonalInfo4 pi, ushort move)
    {
        var index = Array.IndexOf(Tutors_4, move);
        if (index == -1)
            return false;
        return pi.TypeTutors[index];
    }

    private static bool GetIsTM(PersonalInfo4 info, ushort move)
    {
        var index = Array.IndexOf(TM_4, move);
        return info.GetIsLearnTM(index);
    }

    private static bool GetIsHM(PersonalInfo4 info, ushort move)
    {
        var index = Array.IndexOf(HM_HGSS, move);
        return info.GetIsLearnHM(index);
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types = MoveSourceType.All)
    {
        if (!TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        if (types.HasFlag(MoveSourceType.LevelUp))
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

        if (types.HasFlag(MoveSourceType.Machine))
        {
            pi.SetAllLearnTM(result, TM_4);

            if (pk.Format == Generation)
                pi.SetAllLearnHM(result, HM_HGSS);
            else if (pi.GetIsLearnHM(4)) // Permit Whirlpool to leak through if transferred to Gen5+ (via D/P/Pt)
                result[(int)Move.Whirlpool] = true;
        }

        if (types.HasFlag(MoveSourceType.TypeTutor))
        {
            // Elemental Beams
            var species = SpecialTutors_Compatibility_4;
            var moves = SpecialTutors_4;
            for (int i = 0; i < species.Length; i++)
            {
                var index = Array.IndexOf(species[i], evo.Species);
                if (index != -1)
                    result[moves[i]] = true;
            }
        }

        if (types.HasFlag(MoveSourceType.SpecialTutor))
            pi.SetAllLearnTutorType(result, Tutors_4);

        if (types.HasFlag(MoveSourceType.EnhancedTutor))
        {
            if (evo.Species is (int)Species.Rotom && evo.Form is not 0)
                result[MoveTutor.GetRotomFormMove(evo.Form)] = true;
        }
    }
}
