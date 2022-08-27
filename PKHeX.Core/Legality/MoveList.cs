using System;
using static PKHeX.Core.Legal;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>
/// Logic for obtaining a list of moves.
/// </summary>
internal static class MoveList
{
    internal static void GetCurrentMoves(PKM pk, ushort species, byte form, GameVersion gameSource, int lvl, Span<ushort> moves)
    {
        if (gameSource == Any)
            gameSource = (GameVersion)pk.Version;

        _ = gameSource switch
        {
            GSC or GS => Get(moves, LevelUpGS, species, lvl, pk.Format),
            C => Get(moves, LevelUpC, species, lvl, pk.Format),

            R or S or RS => Get(moves, LevelUpRS, species, lvl),
            E => Get(moves, LevelUpE, species, lvl),
            FR or LG or FRLG => Get(moves, LevelUpFR, species, lvl),

            D or P or DP => Get(moves, LevelUpDP, species, lvl),
            Pt => Get(moves, LevelUpPt, species, lvl),
            HG or SS or HGSS => Get(moves, LevelUpHGSS, species, lvl),

            B or W or BW => Get(moves, LevelUpBW, species, lvl),
            B2 or W2 or B2W2 => Get(moves, LevelUpB2W2, species, lvl),

            X or Y or XY => Get(moves, LevelUpXY, species, lvl),
            AS or OR or ORAS => Get(moves, LevelUpAO, species, lvl),

            SN or MN or SM => Get(moves, LevelUpSM, PersonalTable.SM, species, form, lvl),
            US or UM or USUM => Get(moves, LevelUpUSUM, PersonalTable.USUM, species, form, lvl),
            SW or SH or SWSH => Get(moves, LevelUpSWSH, PersonalTable.SWSH, species, form, lvl),
            BD or SP or BDSP => Get(moves, LevelUpBDSP, PersonalTable.BDSP, species, form, lvl),
            PLA => Get(moves, LevelUpLA, PersonalTable.LA, species, form, lvl),
            _ => moves,
        };
    }

    private static Span<ushort> Get(Span<ushort> moves, Learnset[] source, ushort species, int lvl)
    {
        if (species >= source.Length)
            return moves;
        source[species].SetLevelUpMoves(1, lvl, moves);
        return moves;
    }

    private static Span<ushort> Get(Span<ushort> moves, Learnset[] source, IPersonalTable pt, ushort species, byte form, int lvl)
    {
        if (!pt.IsPresentInGame(species, form))
            return moves;

        int index = pt.GetFormIndex(species, form);
        source[index].SetLevelUpMoves(1, lvl, moves);
        return moves;
    }

    private static Span<ushort> Get(Span<ushort> moves, Learnset[] source, ushort species, int lvl, int format)
    {
        if ((uint)species > MaxSpeciesID_2)
            return moves;

        source[species].SetLevelUpMoves(1, lvl, moves);
        if (format != 1)
            return moves;

        // If checking back-transfer specimen (GSC->RBY), remove moves that must be deleted prior to transfer
        // Remove all values greater than MaxMoveID_1, and shift the remaining indexes down.
        for (int i = 0; i < moves.Length; i++)
        {
            if (moves[i] <= MaxMoveID_1)
                continue;
            // Shift remaining indexes down, set last index to 0
            for (int j = i; j < moves.Length - 1; j++)
                moves[j] = moves[j + 1];

            moves[^1] = 0;
        }

        return moves;
    }
}
