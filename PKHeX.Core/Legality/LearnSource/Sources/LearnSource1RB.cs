using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;
using static PKHeX.Core.LearnSource1;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="RB"/>.
/// </summary>
public sealed class LearnSource1RB : ILearnSource
{
    public static readonly LearnSource1RB Instance = new();
    private static readonly PersonalTable1 Personal = PersonalTable.RB;
    private static readonly Learnset[] Learnsets = Legal.LevelUpRB;
    private const LearnEnvironment Game = RB;

    public Learnset GetLearnset(int species, int form) => Learnsets[species];

    public bool TryGetPersonal(int species, int form, [NotNullWhen(true)] out PersonalInfo? pi)
    {
        pi = null;
        if (form is not 0 || species > Legal.MaxSpeciesID_1)
            return false;
        pi = Personal[species];
        return true;
    }

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo pi, EvoCriteria evo, int move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlagFast(MoveSourceType.Machine) && GetIsTM(pi, move))
            return new(TMHM, Game);

        if (types.HasFlagFast(MoveSourceType.SpecialTutor) && GetIsTutor(evo.Species, move))
            return new (Tutor, Game);

        if (types.HasFlagFast(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1 && evo.LevelMin <= level && level <= evo.LevelMax)
                return new(LevelUp, Game, (byte)level);
        }

        return default;
    }

    private static bool GetIsTutor(int species, int move)
    {
        // No special tutors besides Stadium, which is GB-era only.
        if (!ParseSettings.AllowGBCartEra)
            return false;

        // Surf Pikachu via Stadium
        if (move != (int)Move.Surf)
            return false;
        return species is (int)Species.Pikachu or (int)Species.Raichu;
    }

    private static bool GetIsTM(PersonalInfo info, int move)
    {
        var index = Array.IndexOf(TMHM_RBY, move);
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
            var min = ParseSettings.AllowGen1Tradeback && ParseSettings.AllowGen2MoveReminder(pk) ? 1 : evo.LevelMin;
            (bool hasMoves, int start, int end) = learn.GetMoveRange(evo.LevelMax, min);
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
            var moves = TMHM_RBY;
            for (int i = 0; i < moves.Length; i++)
            {
                if (flags[i])
                    result[moves[i]] = true;
            }
        }

        if (types.HasFlagFast(MoveSourceType.SpecialTutor))
        {
            if (GetIsTutor(evo.Species, (int)Move.Surf))
                result[(int)Move.Surf] = true;
        }
    }

    public void GetEncounterMoves(IEncounterTemplate enc, Span<int> init)
    {
        var species = enc.Species;
        if (!TryGetPersonal(species, 0, out var personal))
            return;

        var pi = (PersonalInfo1)personal;
        var learn = Learnsets[species];
        pi.GetMoves(init);
        var start = (4 - init.Count(0)) & 3;
        learn.SetEncounterMoves(enc.LevelMin, init, start);
    }
}
