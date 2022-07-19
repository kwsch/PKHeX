using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;
using static PKHeX.Core.LearnSource3;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="RS"/>.
/// </summary>
public sealed class LearnSource3RS : ILearnSource, IEggSource
{
    public static readonly LearnSource3RS Instance = new();
    private static readonly PersonalTable3 Personal = PersonalTable.RS;
    private static readonly Learnset[] Learnsets = Legal.LevelUpRS;
    private static readonly EggMoves6[] EggMoves = Legal.EggMovesRS; // same for all Gen3 games
    private const int MaxSpecies = Legal.MaxSpeciesID_3;
    private const LearnEnvironment Game = RS;
    private const int Generation = 3;
    private const int CountTM = 50;

    public Learnset GetLearnset(int species, int form) => Learnsets[species];

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

        if (types.HasFlagFast(MoveSourceType.Machine))
        {
            if (GetIsTM(pi, move))
                return new(TMHM, Game);
            if (pk.Format == Generation && GetIsHM(pi, move))
                return new(TMHM, Game);
        }

        if (types.HasFlagFast(MoveSourceType.SpecialTutor) && GetIsTutor(evo.Species, move))
            return new(Tutor, Game);

        return default;
    }

    private static bool GetIsTutor(int species, int move)
    {
        // XD (Mew)
        if (species == (int)Species.Mew && Tutor_3Mew.AsSpan().IndexOf(move) != -1)
            return true;

        return move switch
        {
            (int)Move.SelfDestruct => Array.BinarySearch(SpecialTutors_XD_SelfDestruct, (ushort)species) != -1,
            (int)Move.SkyAttack => Array.BinarySearch(SpecialTutors_XD_SkyAttack, (ushort)species) != -1,
            (int)Move.Nightmare => Array.BinarySearch(SpecialTutors_XD_Nightmare, (ushort)species) != -1,
            _ => false,
        };
    }

    private static bool GetIsTM(PersonalInfo info, int move)
    {
        var index = Array.IndexOf(TM_3, move);
        if (index == -1)
            return false;
        return info.TMHM[index];
    }

    private static bool GetIsHM(PersonalInfo info, int move)
    {
        var index = Array.IndexOf(HM_3, move);
        if (index == -1)
            return false;
        return info.TMHM[CountTM + index];
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
            var moves = TM_3;
            for (int i = 0; i < moves.Length; i++)
            {
                if (flags[i])
                    result[moves[i]] = true;
            }

            if (pk.Format == Generation)
            {
                moves = HM_3;
                for (int i = 0; i < moves.Length; i++)
                {
                    if (flags[CountTM + i])
                        result[moves[i]] = true;
                }
            }
        }

        if (types.HasFlagFast(MoveSourceType.SpecialTutor))
        {
            if (evo.Species == (int)Species.Mew)
            {
                foreach (var move in Tutor_3Mew)
                    result[move] = true;
            }

            if (Array.BinarySearch(SpecialTutors_XD_SelfDestruct, evo.Species) != -1)
                result[(int)Move.SelfDestruct] = true;
            if (Array.BinarySearch(SpecialTutors_XD_SkyAttack, evo.Species) != -1)
                result[(int)Move.SkyAttack] = true;
            if (Array.BinarySearch(SpecialTutors_XD_Nightmare, evo.Species) != -1)
                result[(int)Move.Nightmare] = true;
        }
    }

    private static readonly int[] Tutor_3Mew =
    {
        (int)Move.FeintAttack,
        (int)Move.FakeOut,
        (int)Move.Hypnosis,
        (int)Move.NightShade,
        (int)Move.RolePlay,
        (int)Move.ZapCannon,
    };

    private static readonly ushort[] SpecialTutors_XD_SelfDestruct =
    {
        074, 075, 076, 088, 089, 090, 091, 092, 093, 094, 095,
        100, 101, 102, 103, 109, 110, 143, 150, 151, 185, 204,
        205, 208, 211, 218, 219, 222, 273, 274, 275, 299, 316,
        317, 320, 321, 323, 324, 337, 338, 343, 344, 362, 375,
        376, 377, 378, 379,
    };

    private static readonly ushort[] SpecialTutors_XD_SkyAttack =
    {
        016, 017, 018, 021, 022, 084, 085, 142, 144, 145, 146,
        151, 163, 164, 176, 177, 178, 198, 225, 227, 250, 276,
        277, 278, 279, 333, 334,
    };

    private static readonly ushort[] SpecialTutors_XD_Nightmare =
    {
        012, 035, 036, 039, 040, 052, 053, 063, 064, 065, 079,
        080, 092, 093, 094, 096, 097, 102, 103, 108, 121, 122,
        124, 131, 137, 150, 151, 163, 164, 173, 174, 177, 178,
        190, 196, 197, 198, 199, 200, 203, 206, 215, 228, 229,
        233, 234, 238, 248, 249, 250, 251, 280, 281, 282, 284,
        292, 302, 315, 316, 317, 327, 353, 354, 355, 356, 358,
        359, 385, 386,
    };
}
