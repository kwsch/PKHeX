using System;

namespace PKHeX.Core;

/// <summary>
/// Group that checks the source of a move in <see cref="EntityContext.Gen3"/>.
/// </summary>
public sealed class LearnGroup3 : ILearnGroup
{
    public static readonly LearnGroup3 Instance = new();
    private const EntityContext Context = EntityContext.Gen3;
    public ushort MaxMoveID => Legal.MaxMoveID_3;

    public ILearnGroup? GetPrevious(PKM pk, EvolutionHistory history, IEncounterTemplate enc, LearnOption option) => null; // Gen3 is the end of the line!
    public bool HasVisited(PKM pk, EvolutionHistory history) => history.HasVisitedGen3;

    public bool Check(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvolutionHistory history, IEncounterTemplate enc,
        MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        var evos = history.Gen3;
        for (var i = 0; i < evos.Length; i++)
            Check(result, current, pk, evos[i], i, types);

        if (types.HasFlag(MoveSourceType.Encounter) && enc is EncounterEgg3 { Context: Context } egg)
            CheckEncounterMoves(result, current, egg);

        if (types.HasFlag(MoveSourceType.LevelUp) && enc.Species is (int)Species.Nincada && evos is [{ Species: (int)Species.Shedinja }, _])
            CheckNincadaMoves(result, current, evos[^1]);

        return MoveResult.AllParsed(result);
    }

    private static void CheckNincadaMoves(Span<MoveResult> result, ReadOnlySpan<ushort> current, EvoCriteria nincada)
    {
        if (MoveResult.AllParsed(result))
            return;

        // If a Ninjask move is already marked as learned, it's not a valid move for Nincada. Reset that index and try again.
        for (int i = 0; i < current.Length; i++)
        {
            if (result[i].Info.Method is not LearnMethod.ShedinjaEvo)
                continue;
            result[i] = default;
            break;
        }

        // If a result is not valid, check to see if it is a Shedinja move.
        var shedinja = LearnSource3E.Instance;
        var moves = shedinja.GetLearnset((int)Species.Ninjask, 0);
        for (var i = 0; i < result.Length; i++)
        {
            if (result[i].Valid)
                continue;
            var move = current[i];
            if (move == 0)
                break;

            if (!moves.TryGetLevelLearnMove(move, out var level))
                continue;
            if (!nincada.InsideLevelRange(level))
                continue;

            var info = new MoveLearnInfo(LearnMethod.ShedinjaEvo, LearnEnvironment.E, level);
            result[i] = new MoveResult(info, 0, Context);
            break; // Can only have one Ninjask move.
        }
    }

    private static void CheckEncounterMoves(Span<MoveResult> result, ReadOnlySpan<ushort> current, EncounterEgg3 egg)
    {
        ILearnSource inst = egg.Version switch
        {
            GameVersion.E => LearnSource3E.Instance,
            GameVersion.FR => LearnSource3FR.Instance,
            GameVersion.LG => LearnSource3LG.Instance,
            _ => LearnSource3RS.Instance,
        };
        var eggMoves = inst.GetEggMoves(egg.Species, egg.Form);
        var levelMoves = inst.GetInheritMoves(egg.Species, egg.Form);

        for (var i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;
            var move = current[i];
            if (eggMoves.Contains(move))
                result[i] = new(LearnMethod.EggMove, inst.Environment);
            else if (levelMoves.Contains(move))
                result[i] = new(LearnMethod.InheritLevelUp, inst.Environment);
            else if (move is (int)Move.VoltTackle && egg.CanHaveVoltTackle)
                result[i] = new(LearnMethod.SpecialEgg, inst.Environment);
        }
    }

    private static void Check(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvoCriteria evo, int stage, MoveSourceType types)
    {
        if (!ParseSettings.AllowGBACrossTransferRSE(pk))
        {
            CheckNX(result, current, pk, evo, stage, types);
            return;
        }

        var rs = LearnSource3RS.Instance;
        var species = evo.Species;
        if (!rs.TryGetPersonal(species, evo.Form, out var rp))
            return; // should never happen.

        var e = LearnSource3E.Instance;
        var fr = LearnSource3FR.Instance;
        var lg = LearnSource3LG.Instance;
        var ep = e[species];
        var fp = fr[species];
        var lp = lg[species];

        for (int i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;

            // Level Up moves are different for each game, but TM/HM is shared (use Emerald).
            var move = current[i];
            var chk = e.GetCanLearn(pk, ep, evo, move, types);
            if (chk != default)
            {
                result[i] = new(chk, (byte)stage, Context);
                continue;
            }
            chk = rs.GetCanLearn(pk, rp, evo, move, types & (MoveSourceType.LevelUp | MoveSourceType.AllTutors));
            if (chk != default)
            {
                result[i] = new(chk, (byte)stage, Context);
                continue;
            }
            chk = fr.GetCanLearn(pk, fp, evo, move, types & (MoveSourceType.LevelUp | MoveSourceType.AllTutors));
            if (chk != default)
            {
                result[i] = new(chk, (byte)stage, Context);
                continue;
            }
            chk = lg.GetCanLearn(pk, lp, evo, move, types & MoveSourceType.LevelUp); // Tutors same as FR
            if (chk != default)
                result[i] = new(chk, (byte)stage, Context);
        }
    }

    private static void CheckNX(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvoCriteria evo, int stage, MoveSourceType types)
    {
        var species = evo.Species;
        var fr = LearnSource3FR.Instance;
        if (!fr.TryGetPersonal(species, evo.Form, out var fp))
            return; // should never happen.
        var lg = LearnSource3LG.Instance;
        var lp = lg[species];

        var isFireRed = pk.Version == GameVersion.FR;
        ILearnSource<PersonalInfo3> primaryLS = isFireRed ? fr : lg;
        ILearnSource<PersonalInfo3> secondaryLS = !isFireRed ? fr : lg;
        var primaryPI = isFireRed ? fp : lp;
        var secondaryPI = !isFireRed ? fp : lp;
        var primaryEnv = isFireRed ? LearnEnvironment.FR : LearnEnvironment.LG;

        for (int i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;

            // Level Up moves are different for each game, but TM/HM is shared.
            var move = current[i];
            var chk = primaryLS.GetCanLearn(pk, primaryPI, evo, move, types);
            if (chk != default)
            {
                result[i] = new(chk, (byte)stage, Context);
                continue;
            }
            chk = secondaryLS.GetCanLearn(pk, secondaryPI, evo, move, types & MoveSourceType.LevelUp); // Tutors same as FR
            if (chk != default)
            {
                result[i] = new(chk, (byte)stage, Context);
                continue;
            }

            // Tutors are indexes 0-14, same as Emerald.
            if (LearnSource3E.GetIsTutorFRLG(evo.Species, move))
                result[i] = new(new(LearnMethod.Tutor, primaryEnv));
        }
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvolutionHistory history, IEncounterTemplate enc, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlag(MoveSourceType.Encounter) && enc.Context == Context)
            FlagEncounterMoves(enc, result);

        var evos = history.Gen3;
        foreach (var evo in evos)
            GetAllMoves(result, pk, evo, types);

        if (evos is [{ Species: (int)Species.Shedinja }, _])
        {
            var shedinja = LearnSource3E.Instance;
            var moves = shedinja.GetLearnset((int)Species.Ninjask, 0);
            var span = moves.GetMoveRange(evos[0].LevelMax, 20);
            foreach (var move in span)
                result[move] = true;
        }
    }

    private static void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types)
    {
        if (!ParseSettings.AllowGBACrossTransferRSE(pk)) // NX
        {
            LearnSource3FR.Instance.GetAllMoves(result, pk, evo, types);
            LearnSource3LG.Instance.GetAllMoves(result, pk, evo, types & (MoveSourceType.LevelUp));

            // Tutors are indexes 0-14, same as Emerald.
            if (types.HasFlag(MoveSourceType.EnhancedTutor))
                LearnSource3E.GetAllTutorMovesFRLG(result, evo.Species);
            return;
        }
        LearnSource3E.Instance.GetAllMoves(result, pk, evo, types);
        LearnSource3RS.Instance.GetAllMoves(result, pk, evo, types & (MoveSourceType.LevelUp | MoveSourceType.AllTutors));
        LearnSource3FR.Instance.GetAllMoves(result, pk, evo, types & (MoveSourceType.LevelUp | MoveSourceType.AllTutors));
        LearnSource3LG.Instance.GetAllMoves(result, pk, evo, types & (MoveSourceType.LevelUp));
    }

    private static void FlagEncounterMoves(IEncounterTemplate enc, Span<bool> result)
    {
        if (enc is IMoveset { Moves: { HasMoves: true } x })
            x.FlagMoves(result);
    }
}
