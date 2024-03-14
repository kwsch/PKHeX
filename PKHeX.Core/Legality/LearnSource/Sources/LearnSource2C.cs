using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;
using static PKHeX.Core.LearnSource2;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="C"/>.
/// </summary>
public sealed class LearnSource2C : ILearnSource<PersonalInfo2>, IEggSource
{
    public static readonly LearnSource2C Instance = new();
    private static readonly PersonalTable2 Personal = PersonalTable.C;
    private static readonly EggMoves2[] EggMoves = EggMoves2.GetArray(Util.GetBinaryResource("eggmove_c.pkl"), Legal.MaxSpeciesID_2);
    private static readonly Learnset[] Learnsets = LearnsetReader.GetArray(Util.GetBinaryResource("lvlmove_c.pkl"), Legal.MaxSpeciesID_2);
    private const int MaxSpecies = Legal.MaxSpeciesID_2;
    private const LearnEnvironment Game = C;

    public LearnEnvironment Environment => Game;

    public Learnset GetLearnset(ushort species, byte form) => Learnsets[species < Learnsets.Length ? species : 0];

    public bool TryGetPersonal(ushort species, byte form, [NotNullWhen(true)] out PersonalInfo2? pi)
    {
        if (form is not 0 || species > MaxSpecies)
        {
            pi = null;
            return false;
        }
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

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo2 pi, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (move > Legal.MaxMoveID_2) // byte
            return default;

        if (types.HasFlag(MoveSourceType.Machine) && GetIsTM(pi, (byte)move))
            return new(TMHM, Game);

        if (types.HasFlag(MoveSourceType.SpecialTutor) && GetIsSpecialTutor(pk, evo.Species, (byte)move))
            return new(Tutor, Game);

        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1 && evo.InsideLevelRange(level))
                return new(LevelUp, Game, (byte)level);
        }

        return default;
    }

    private static bool GetIsSpecialTutor(PKM pk, ushort species, byte move)
    {
        if (!ParseSettings.AllowGen2Crystal(pk))
            return false;
        var tutor = Tutors_GSC.IndexOf(move);
        if (tutor == -1)
            return false;
        var info = PersonalTable.C[species];
        return info.GetIsLearnTutorType(tutor);
    }

    private static bool GetIsTM(PersonalInfo2 info, byte move)
    {
        var index = TMHM_GSC.IndexOf(move);
        if (index == -1)
            return false;
        return info.GetIsLearnTM(index);
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types = MoveSourceType.All)
    {
        if (!TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        bool removeVC = pk.Format == 1 || pk.VC1;
        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var min = ParseSettings.AllowGen2MoveReminder(pk) ? 1 : evo.LevelMin;
            var span = learn.GetMoveRange(evo.LevelMax, min);
            foreach (var move in span)
            {
                if (!removeVC || move <= Legal.MaxMoveID_1)
                    result[move] = true;
            }
        }

        if (types.HasFlag(MoveSourceType.Machine))
            pi.SetAllLearnTM(result, TMHM_GSC);

        if (types.HasFlag(MoveSourceType.SpecialTutor))
            pi.SetAllLearnTutorType(result, Tutors_GSC);
    }

    public static void GetEncounterMoves(PKM pk, IEncounterTemplate enc, Span<ushort> init)
    {
        var species = enc.Species;
        var learn = Learnsets[species];
        var level = enc.LevelMin;
        if (pk is ICaughtData2 { CaughtData: not 0 })
            level = Math.Max(level, pk.MetLevel); // ensure the met level is somewhat accurate
        learn.SetEncounterMoves(level, init);
    }
}
