using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="FR"/>.
/// </summary>
public sealed class LearnSource3FR : LearnSource3, ILearnSource<PersonalInfo3>, IEggSource
{
    public static readonly LearnSource3FR Instance = new();
    private static readonly PersonalTable3 Personal = PersonalTable.FR;
    private static readonly Learnset[] Learnsets = LearnsetReader.GetArray(BinLinkerAccessor.Get(Util.GetBinaryResource("lvlmove_fr.pkl"), "fr"u8));
    private const int MaxSpecies = Legal.MaxSpeciesID_3;
    private const LearnEnvironment Game = FR;
    private const byte Generation = 3;
    private const int CountTM = 50;

    public LearnEnvironment Environment => Game;

    public Learnset GetLearnset(ushort species, byte form) => Learnsets[species < Learnsets.Length ? species : 0];
    internal PersonalInfo3 this[ushort species] => Personal[species];

    public bool TryGetPersonal(ushort species, byte form, [NotNullWhen(true)] out PersonalInfo3? pi)
    {
        pi = null;
        if (species > MaxSpecies)
            return false;
        pi = Personal[species];
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

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo3 pi, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
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
            if (pk.Format == Generation && GetIsHM(pi, move))
                return new(TMHM, Game);
        }

        if (types.HasFlag(MoveSourceType.SpecialTutor) && GetIsTutor(evo.Species, move))
            return new(Tutor, Game);

        return default;
    }

    private static bool GetIsTutor(ushort species, ushort move) => move switch
    {
        (int)Move.BlastBurn   => species == (int)Species.Charizard,
        (int)Move.HydroCannon => species == (int)Species.Blastoise,
        (int)Move.FrenzyPlant => species == (int)Species.Venusaur,
        _ => false,
    };

    private static bool GetIsTM(PersonalInfo3 info, ushort move)
    {
        var index = TM_3.IndexOf(move);
        if (index == -1)
            return false;
        return info.TMHM[index];
    }

    private static bool GetIsHM(PersonalInfo3 info, ushort move)
    {
        var index = HM_3.IndexOf(move);
        if (index == -1)
            return false;
        return info.TMHM[CountTM + index];
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
            var flags = pi.TMHM;
            var moves = TM_3;
            for (int i = 0; i < moves.Length; i++)
            {
                if (flags[i])
                    result[moves[i]] = true;
            }

            if (pk.Format == 3)
            {
                moves = HM_3;
                for (int i = 0; i < moves.Length; i++)
                {
                    if (flags[CountTM + i])
                        result[moves[i]] = true;
                }
            }
        }

        if (types.HasFlag(MoveSourceType.SpecialTutor))
        {
            if (evo.Species == (int)Species.Charizard)
                result[(int)Move.BlastBurn] = true;
            else if (evo.Species == (int)Species.Blastoise)
                result[(int)Move.HydroCannon] = true;
            else if (evo.Species == (int)Species.Venusaur)
                result[(int)Move.FrenzyPlant] = true;
        }
    }
}
