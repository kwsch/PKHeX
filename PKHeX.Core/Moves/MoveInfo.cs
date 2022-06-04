using System;
using System.Collections.Generic;
using static PKHeX.Core.Move;
using static PKHeX.Core.EntityContext;

namespace PKHeX.Core;

public static class MoveInfo
{
    /// <summary>
    /// Gets the base PP of a move ID depending on the <see cref="context"/>.
    /// </summary>
    /// <param name="context">Game context</param>
    /// <param name="move">Move ID</param>
    /// <returns>Amount of PP the move has by default (no PP Ups).</returns>
    public static byte GetPP(EntityContext context, int move)
    {
        var table = GetPPTable(context);
        if ((uint)move >= table.Length)
            return 0;
        return table[move];
    }

    public static ReadOnlySpan<byte> GetPPTable(PKM pkm) => GetPPTable(pkm.Context);

    public static ReadOnlySpan<byte> GetPPTable(EntityContext context) => context switch
    {
        Gen1 => MoveInfo1.MovePP_RBY,
        Gen2 => MoveInfo2.MovePP_GSC,
        Gen3 => MoveInfo3.MovePP_RS,
        Gen4 => MoveInfo4.MovePP_DP,
        Gen5 => MoveInfo5.MovePP_BW,
        Gen6 => MoveInfo6.MovePP,
        Gen7 => MoveInfo7.MovePP_SM,
        Gen8 => MoveInfo8.MovePP_SWSH,

        Gen7b => MoveInfo7b.MovePP_GG,
        Gen8a => MoveInfo8a.MovePP_LA,
        Gen8b => MoveInfo8.MovePP_SWSH,
        _ => throw new ArgumentOutOfRangeException(nameof(context)),
    };

    public static ICollection<int> GetDummiedMovesHashSet(EntityContext context) => context switch
    {
        Gen8 => MoveInfo8.DummiedMoves_SWSH,
        Gen8a => MoveInfo8a.DummiedMoves_LA,
        Gen8b => MoveInfo8b.DummiedMoves_BDSP,
        _ => Array.Empty<int>(),
    };

    /// <summary>
    /// Generation 7 Z Moves
    /// </summary>
    public static readonly HashSet<int> Z_Moves = new()
    {
        (int)BreakneckBlitzP,
        (int)BreakneckBlitzS,
        (int)AllOutPummelingP,
        (int)AllOutPummelingS,
        (int)SupersonicSkystrikeP,
        (int)SupersonicSkystrikeS,
        (int)AcidDownpourP,
        (int)AcidDownpourS,
        (int)TectonicRageP,
        (int)TectonicRageS,
        (int)ContinentalCrushP,
        (int)ContinentalCrushS,
        (int)SavageSpinOutP,
        (int)SavageSpinOutS,
        (int)NeverEndingNightmareP,
        (int)NeverEndingNightmareS,
        (int)CorkscrewCrashP,
        (int)CorkscrewCrashS,
        (int)InfernoOverdriveP,
        (int)InfernoOverdriveS,
        (int)HydroVortexP,
        (int)HydroVortexS,
        (int)BloomDoomP,
        (int)BloomDoomS,
        (int)GigavoltHavocP,
        (int)GigavoltHavocS,
        (int)ShatteredPsycheP,
        (int)ShatteredPsycheS,
        (int)SubzeroSlammerP,
        (int)SubzeroSlammerS,
        (int)DevastatingDrakeP,
        (int)DevastatingDrakeS,
        (int)BlackHoleEclipseP,
        (int)BlackHoleEclipseS,
        (int)TwinkleTackleP,
        (int)TwinkleTackleS,

        (int)Catastropika,
        (int)SinisterArrowRaid,
        (int)MaliciousMoonsault,
        (int)OceanicOperetta,
        (int)GuardianofAlola,
        (int)SoulStealing7StarStrike,
        (int)StokedSparksurfer,
        (int)PulverizingPancake,
        (int)ExtremeEvoboost,
        (int)GenesisSupernova,
        (int)TenMVoltThunderbolt,
        (int)LightThatBurnstheSky,
        (int)SearingSunrazeSmash,
        (int)MenacingMoonrazeMaelstrom,
        (int)LetsSnuggleForever,
        (int)SplinteredStormshards,
        (int)ClangorousSoulblaze,
    };

    public static bool IsDynamaxMove(int move) => move is (>= (int)MaxFlare and <= (int)MaxSteelspike);

    /// <summary>
    /// Moves that can not be obtained by using Sketch with Smeargle in any game.
    /// </summary>
    internal static readonly HashSet<int> InvalidSketch = new(Z_Moves)
    {
        // Can't Sketch
        (int)Struggle,
        (int)Chatter,

        // Unreleased
        (int)LightofRuin,
    };

    public static bool IsDummiedMove(PKM pk, ushort move)
    {
        var hashSet = GetDummiedMovesHashSet(pk.Context);
        return hashSet.Contains(move);
    }

    public static bool IsDummiedMoveAny(PKM pk)
    {
        var hs = GetDummiedMovesHashSet(pk.Context);
        if (hs.Count == 0)
            return false;

        for (int i = 0; i < 4; i++)
        {
            var move = (ushort)pk.GetMove(i);
            if (hs.Contains(move))
                return true;
        }
        return false;
    }
}
