using System;
using static PKHeX.Core.Legal;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>
/// Logic for obtaining a list of moves.
/// </summary>
internal static class MoveList
{
    internal static int[] GetBaseEggMoves(PKM pk, int species, int form, GameVersion gameSource, int lvl)
    {
        if (gameSource == Any)
            gameSource = (GameVersion)pk.Version;

        switch (gameSource)
        {
            case GSC or GS:
                // If checking back-transfer specimen (GSC->RBY), remove moves that must be deleted prior to transfer
                static int[] getRBYCompatibleMoves(int format, int[] moves) => format == 1 ? Array.FindAll(moves, m => m <= MaxMoveID_1) : moves;
                if (pk.InhabitedGeneration(2))
                    return getRBYCompatibleMoves(pk.Format, LevelUpGS[species].GetMoves(lvl));
                break;
            case C:
                if (pk.InhabitedGeneration(2))
                    return getRBYCompatibleMoves(pk.Format, LevelUpC[species].GetMoves(lvl));
                break;

            case R or S or RS:
                if (pk.InhabitedGeneration(3))
                    return LevelUpRS[species].GetMoves(lvl);
                break;
            case E:
                if (pk.InhabitedGeneration(3))
                    return LevelUpE[species].GetMoves(lvl);
                break;
            case FR or LG or FRLG:
                // The only difference in FR/LG is Deoxys, which doesn't breed.
                if (pk.InhabitedGeneration(3))
                    return LevelUpFR[species].GetMoves(lvl);
                break;

            case D or P or DP:
                if (pk.InhabitedGeneration(4))
                    return LevelUpDP[species].GetMoves(lvl);
                break;
            case Pt:
                if (pk.InhabitedGeneration(4))
                    return LevelUpPt[species].GetMoves(lvl);
                break;
            case HG or SS or HGSS:
                if (pk.InhabitedGeneration(4))
                    return LevelUpHGSS[species].GetMoves(lvl);
                break;

            case B or W or BW:
                if (pk.InhabitedGeneration(5))
                    return LevelUpBW[species].GetMoves(lvl);
                break;

            case B2 or W2 or B2W2:
                if (pk.InhabitedGeneration(5))
                    return LevelUpB2W2[species].GetMoves(lvl);
                break;

            case X or Y or XY:
                if (pk.InhabitedGeneration(6))
                    return LevelUpXY[species].GetMoves(lvl);
                break;

            case AS or OR or ORAS:
                if (pk.InhabitedGeneration(6))
                    return LevelUpAO[species].GetMoves(lvl);
                break;

            case SN or MN or SM:
                if (species > MaxSpeciesID_7)
                    break;
                if (pk.InhabitedGeneration(7))
                {
                    int index = PersonalTable.SM.GetFormIndex(species, form);
                    return LevelUpSM[index].GetMoves(lvl);
                }
                break;

            case US or UM or USUM:
                if (pk.InhabitedGeneration(7))
                {
                    int index = PersonalTable.USUM.GetFormIndex(species, form);
                    return LevelUpUSUM[index].GetMoves(lvl);
                }
                break;

            case SW or SH or SWSH:
                if (pk.InhabitedGeneration(8))
                {
                    int index = PersonalTable.SWSH.GetFormIndex(species, form);
                    return LevelUpSWSH[index].GetMoves(lvl);
                }
                break;

            case PLA:
                if (pk.InhabitedGeneration(8))
                {
                    int index = PersonalTable.LA.GetFormIndex(species, form);
                    return LevelUpLA[index].GetMoves(lvl);
                }
                break;

            case BD or SP or BDSP:
                if (pk.InhabitedGeneration(8))
                {
                    int index = PersonalTable.BDSP.GetFormIndex(species, form);
                    return LevelUpBDSP[index].GetMoves(lvl);
                }
                break;
        }
        return Array.Empty<int>();
    }
}
