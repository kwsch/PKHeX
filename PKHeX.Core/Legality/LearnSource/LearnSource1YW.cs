using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

public class LearnSource1YW : ILearnSource
{
    public static readonly LearnSource1RB Instance = new();
    private static readonly PersonalTable Personal = PersonalTable.Y;
    private static readonly Learnset[] LevelUp = LearnsetReader.GetArray(Util.GetBinaryResource("lvlmove_y.pkl"), Legal.MaxSpeciesID_1);

    public Learnset GetLearnset(int species, int form) => LevelUp[species];

    public bool TryGetPersonal(int species, int form, [NotNullWhen(true)] out PersonalInfo? pi)
    {
        pi = null;
        if (form is not 0 || species > Legal.MaxSpeciesID_1)
            return false;
        pi = Personal[species];
        return true;
    }

    public LearnMethod GetCanLearn(PKM pk, PersonalInfo pi, EvoCriteria evo, int move, MoveSourceType types = MoveSourceType.All)
    {
        if (types.HasFlagFast(MoveSourceType.LevelUp))
        {
            var info = MoveLevelUp.GetIsLevelUp1(evo.Species, evo.Form, move, evo.LevelMax, evo.LevelMin, GameVersion.YW);
            if (info != default)
                return LearnMethod.LevelUp;
        }

        if (types.HasFlagFast(MoveSourceType.Machine) && GetIsTM(pi, move))
            return LearnMethod.TMHM;

        if (types.HasFlagFast(MoveSourceType.SpecialTutor) && GetIsTutor(pk, evo.Species, move))
            return LearnMethod.Tutor;

        return LearnMethod.None;
    }

    private static bool GetIsTutor(PKM pk, int species, int move)
    {
        // No special tutors besides Stadium, which is GB-era only.
        if (!ParseSettings.AllowGBCartEra)
            return false;
        if (pk.Format >= 3)
            return false;

        // Surf Pikachu via Stadium
        if (move != (int)Move.Surf)
            return false;
        return species is (int)Species.Pikachu or (int)Species.Raichu;
    }

    private static bool GetIsTM(PersonalInfo info, int move)
    {
        var index = Array.IndexOf(Legal.TMHM_RBY, move);
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
            var moves = MoveLevelUp.GetMovesLevelUp1(evo.Species, evo.Form, evo.LevelMax, evo.LevelMin, GameVersion.YW);
            foreach (var move in moves)
                yield return move;
        }

        if (types.HasFlagFast(MoveSourceType.Machine))
        {
            var permit = pi.TMHM;
            var moveIDs = Legal.TMHM_RBY;
            for (int i = 0; i < moveIDs.Length; i++)
            {
                if (permit[i])
                    yield return moveIDs[i];
            }
        }

        if (types.HasFlagFast(MoveSourceType.SpecialTutor))
        {
            if (GetIsTutor(pk, evo.Species, (int)Move.Surf))
                yield return (int)Move.Surf;
        }
    }
}
