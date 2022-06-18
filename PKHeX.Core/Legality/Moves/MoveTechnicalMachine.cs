using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public static class MoveTechnicalMachine
{
    public static GameVersion GetIsMachineMove(PKM pk, int species, int form, int generation, int move, GameVersion ver = GameVersion.Any, bool RemoveTransfer = false)
    {
        var (isRestricted, game) = pk.IsMovesetRestricted();
        if (isRestricted)
            ver = game;

        switch (generation)
        {
            case 1: return GetIsMachine1(species, move);
            case 2:
                if (pk.VC1 && move > Legal.MaxMoveID_1)
                    return Legal.NONE;
                return GetIsMachine2(species, move);
            case 3: return GetIsMachine3(species, move, pk.Format, RemoveTransfer);
            case 4: return GetIsMachine4(species, move, pk.Format, RemoveTransfer, form);
            case 5: return GetIsMachine5(species, move, form);
            case 6: return GetIsMachine6(species, move, form, ver);
            case 7: return GetIsMachine7(species, move, form, pk.LGPE || pk.GO ? (GameVersion)pk.Version : ver);
            case 8: return GetIsMachine8(species, move, form, ver);
            default:
                return Legal.NONE;
        }
    }

    public static GameVersion GetIsRecordMove(PKM pk, int species, int form, int generation, int move, GameVersion ver = GameVersion.Any, bool allowBit = false)
    {
        var (isRestricted, game) = pk.IsMovesetRestricted();
        if (isRestricted)
            ver = game;

        return generation switch
        {
            8 => GetIsRecord8(pk, species, move, form, ver, allowBit),
            _ => Legal.NONE,
        };
    }

    private static GameVersion GetIsMachine1(int species, int move)
    {
        var index = Array.IndexOf(Legal.TMHM_RBY, move);
        if (index == -1)
            return Legal.NONE;
        if (PersonalTable.RB.GetFormEntry(species, 0).TMHM[index])
            return GameVersion.RB;
        if (PersonalTable.Y.GetFormEntry(species, 0).TMHM[index])
            return GameVersion.YW;

        return Legal.NONE;
    }

    private static GameVersion GetIsMachine2(int species, int move)
    {
        var index = Array.IndexOf(Legal.TMHM_GSC, move);
        if (index != -1 && PersonalTable.C.GetFormEntry(species, 0).TMHM[index])
            return GameVersion.GS;

        return Legal.NONE;
    }

    private static GameVersion GetIsMachine3(int species, int move, int format, bool RemoveTransfer)
    {
        var index = Array.IndexOf(Legal.TM_3, move);
        if (index != -1 && PersonalTable.E.GetFormEntry(species, 0).TMHM[index])
            return GameVersion.Gen3;

        if (!RemoveTransfer && format <= 3)
            return GetIsMachine3HM(species, move);

        return Legal.NONE;
    }

    private static GameVersion GetIsMachine3HM(int species, int move)
    {
        var index = Array.IndexOf(Legal.HM_3, move);
        if (index == -1)
            return Legal.NONE;
        if (PersonalTable.E.GetFormEntry(species, 0).TMHM[index + 50])
            return GameVersion.Gen3;
        return Legal.NONE;
    }

    private static GameVersion GetIsMachine4(int species, int move, int format, bool RemoveTransfer, int form)
    {
        // TM
        var index = Array.IndexOf(Legal.TM_4, move);
        if (index != -1 && PersonalTable.HGSS.GetFormEntry(species, form).TMHM[index])
            return GameVersion.Gen4;

        // HM
        if (RemoveTransfer && format > 4)
            return GetIsMachine4HMTransfer(species, move, form);

        return GetIsMachine4HM(species, move, form);
    }

    private static GameVersion GetIsMachine4HMTransfer(int species, int move, int form)
    {
        // The combination of both these moves is illegal, it should be checked that the pokemon only learn one
        // except if it can learn any of these moves in gen 5 or later
        switch (move)
        {
            case 250: // Whirlpool
                if (PersonalTable.HGSS.GetFormEntry(species, form).TMHM[96])
                    return GameVersion.HGSS;
                break;
            case 432: // Defog
                if (PersonalTable.Pt.GetFormEntry(species, form).TMHM[96])
                    return GameVersion.DPPt;
                break;
        }
        return Legal.NONE;
    }

    private static GameVersion GetIsMachine4HM(int species, int move, int form)
    {
        var dpi = Array.IndexOf(Legal.HM_DPPt, move);
        if (dpi != 0 && PersonalTable.Pt.GetFormEntry(species, form).TMHM[92 + dpi])
            return GameVersion.DPPt;

        var gsi = Array.IndexOf(Legal.HM_HGSS, move);
        if (gsi != 0 && PersonalTable.HGSS.GetFormEntry(species, form).TMHM[92 + gsi])
            return GameVersion.DPPt;

        return Legal.NONE;
    }

    private static GameVersion GetIsMachine5(int species, int move, int form)
    {
        var index = Array.IndexOf(Legal.TMHM_BW, move);
        if (index != -1 && PersonalTable.B2W2.GetFormEntry(species, form).TMHM[index])
            return GameVersion.Gen5;

        return Legal.NONE;
    }

    private static GameVersion GetIsMachine6(int species, int move, int form, GameVersion ver)
    {
        if (GameVersion.XY.Contains(ver))
        {
            var index = Array.IndexOf(Legal.TMHM_XY, move);
            if (index != -1 && PersonalTable.XY.GetFormEntry(species, form).TMHM[index])
                return GameVersion.XY;
        }

        if (GameVersion.ORAS.Contains(ver))
        {
            var index = Array.IndexOf(Legal.TMHM_AO, move);
            if (index != -1 && PersonalTable.AO.GetFormEntry(species, form).TMHM[index])
                return GameVersion.ORAS;
        }

        return Legal.NONE;
    }

    private static GameVersion GetIsMachine7(int species, int move, int form, GameVersion ver)
    {
        if (GameVersion.Gen7b.Contains(ver))
        {
            var index = Array.IndexOf(Legal.TMHM_GG, move);
            if (index != -1 && PersonalTable.GG.GetFormEntry(species, form).TMHM[index])
                return GameVersion.GG;
        }

        if (GameVersion.SM.Contains(ver) && species <= Legal.MaxSpeciesID_7)
        {
            var index = Array.IndexOf(Legal.TMHM_SM, move);
            if (index != -1 && PersonalTable.SM.GetFormEntry(species, form).TMHM[index])
                return GameVersion.SM;
        }

        if (GameVersion.USUM.Contains(ver) && species <= Legal.MaxSpeciesID_7_USUM)
        {
            var index = Array.IndexOf(Legal.TMHM_SM, move);
            if (index != -1 && PersonalTable.USUM.GetFormEntry(species, form).TMHM[index])
                return GameVersion.USUM;
        }

        return Legal.NONE;
    }

    private static GameVersion GetIsMachine8(int species, int move, int form, GameVersion ver)
    {
        if (GameVersion.SWSH.Contains(ver))
        {
            var index = Legal.TMHM_SWSH.AsSpan(0, PersonalInfoSWSH.CountTM).IndexOf(move);
            if (index != -1 && PersonalTable.SWSH.GetFormEntry(species, form).TMHM[index])
                return GameVersion.SWSH;
        }

        if (GameVersion.BDSP.Contains(ver))
        {
            var index = Legal.TMHM_BDSP.AsSpan(0, PersonalInfoBDSP.CountTM).IndexOf(move);
            if (index != -1 && PersonalTable.BDSP.GetFormEntry(species, form).TMHM[index])
                return GameVersion.BDSP;
        }

        return Legal.NONE;
    }

    private static GameVersion GetIsRecord8(PKM pk, int species, int move, int form, GameVersion ver, bool allowBit)
    {
        if (GameVersion.SWSH.Contains(ver))
        {
            var index = Legal.TMHM_SWSH.AsSpan(PersonalInfoSWSH.CountTM, PersonalInfoSWSH.CountTR).IndexOf(move);
            if (index != -1)
            {
                if (allowBit)
                    return GameVersion.SWSH;
                if (((ITechRecord8)pk).GetMoveRecordFlag(index))
                    return GameVersion.SWSH;
                if (index == 12 && species == (int)Species.Calyrex && form == 0) // TR12
                    return GameVersion.SWSH; // Agility Calyrex without TR glitch.
            }
        }

        return Legal.NONE;
    }

    public static IEnumerable<int> GetTMHM(PKM pk, int species, int form, int generation, GameVersion ver = GameVersion.Any, bool RemoveTransfer = true)
    {
        var r = new List<int>();
        var (isRestricted, game) = pk.IsMovesetRestricted();
        if (isRestricted)
            ver = game;

        switch (generation)
        {
            case 1: AddMachine1(r, species); break;
            case 2: AddMachine2(r, species);
                if (pk.Format >= 7 && pk.VC1)
                    r.RemoveAll(z => z > Legal.MaxMoveID_1);
                break;
            case 3: AddMachine3(r, species, pk.Format, RemoveTransfer); break;
            case 4: AddMachine4(r, species, pk.Format, RemoveTransfer, form); break;
            case 5: AddMachine5(r, species, form); break;
            case 6: AddMachine6(r, species, form, ver); break;
            case 7: AddMachine7(r, species, form, (pk.LGPE || pk.GO) ? (GameVersion)pk.Version : ver); break;
            case 8: AddMachine8(r, species, form, ver); break;
        }
        return r;
    }

    public static IEnumerable<int> GetRecords(PKM pk, int species, int form, int generation)
    {
        var r = new List<int>();
        switch (generation)
        {
            case 8: AddRecordSWSH(r, species, form, pk); break;
        }
        return r;
    }

    private static void AddPermittedIndexes(List<int> moves, ReadOnlySpan<int> moveIDs, ReadOnlySpan<bool> permit)
    {
        for (int i = 0; i < moveIDs.Length; i++)
        {
            if (permit[i])
                moves.Add(moveIDs[i]);
        }
    }

    private static void AddMachine1(List<int> r, int species)
    {
        int index = PersonalTable.RB.GetFormIndex(species, 0);
        if (index == 0)
            return;
        var pi_rb = (PersonalInfoG1)PersonalTable.RB[index];
        var pi_y = (PersonalInfoG1)PersonalTable.Y[index];
        AddPermittedIndexes(r, Legal.TMHM_RBY, pi_rb.TMHM);
        AddPermittedIndexes(r, Legal.TMHM_RBY, pi_y.TMHM);
    }

    private static void AddMachine2(List<int> r, int species)
    {
        int index = PersonalTable.C.GetFormIndex(species, 0);
        if (index == 0)
            return;
        var pi_c = PersonalTable.C[index];
        AddPermittedIndexes(r, Legal.TMHM_GSC, pi_c.TMHM);
    }

    private static void AddMachine3(List<int> r, int species, int format, bool RemoveTransfer)
    {
        int index = PersonalTable.E.GetFormIndex(species, 0);
        if (index == 0)
            return;
        var pi_c = PersonalTable.E[index];
        AddPermittedIndexes(r, Legal.TM_3, pi_c.TMHM);

        if (!RemoveTransfer || format == 3) // HM moves must be removed for 3->4, only give if current format.
            AddPermittedIndexes(r, Legal.HM_3, pi_c.TMHM.AsSpan(50));
    }

    private static void AddMachine4(List<int> r, int species, int format, bool RemoveTransfer, int form)
    {
        var pi_hgss = PersonalTable.HGSS.GetFormEntry(species, form);
        var pi_dppt = PersonalTable.Pt.GetFormEntry(species, form);
        AddPermittedIndexes(r, Legal.TM_4, pi_hgss.TMHM);

        if (RemoveTransfer && format > 4)
        {
            // The combination of both these moves is illegal, it should be checked that the pokemon only learn one
            // except if it can learn any of these moves in gen 5 or later
            if (pi_hgss.TMHM[96])
                r.Add(250); // Whirlpool
            if (pi_dppt.TMHM[96])
                r.Add(432); // Defog
        }
        else
        {
            AddPermittedIndexes(r, Legal.HM_DPPt, pi_dppt.TMHM.AsSpan(92));
            AddPermittedIndexes(r, Legal.HM_HGSS, pi_hgss.TMHM.AsSpan(92));
        }
    }

    private static void AddMachine5(List<int> r, int species, int form)
    {
        var pi = PersonalTable.B2W2.GetFormEntry(species, form);
        AddPermittedIndexes(r, Legal.TMHM_BW, pi.TMHM);
    }

    private static void AddMachine6(List<int> r, int species, int form, GameVersion ver = GameVersion.Any)
    {
        switch (ver)
        {
            case GameVersion.Any: // Start at the top, hit every table
            case GameVersion.X or GameVersion.Y or GameVersion.XY:
                AddMachine6XY(r, species, form);
                if (ver == GameVersion.Any) // Fall Through
                    AddMachine6AO(r, species, form);
                break;
            case GameVersion.AS or GameVersion.OR or GameVersion.ORAS:
                AddMachine6AO(r, species, form);
                break;
        }
    }

    private static void AddMachine7(List<int> r, int species, int form, GameVersion ver = GameVersion.Any)
    {
        switch (ver)
        {
            case GameVersion.GP or GameVersion.GE or GameVersion.GG or GameVersion.GO:
                AddMachineGG(r, species, form);
                return;
            case GameVersion.SN or GameVersion.MN or GameVersion.SM:
                if (species <= Legal.MaxSpeciesID_7)
                    AddMachineSM(r, species, form);
                return;
            case GameVersion.Any:
            case GameVersion.US or GameVersion.UM or GameVersion.USUM:
                AddMachineUSUM(r, species, form);
                if (ver == GameVersion.Any) // Fall Through
                    AddMachineSM(r, species, form);
                return;
        }
    }

    private static void AddMachine8(List<int> r, int species, int form, GameVersion ver = GameVersion.Any)
    {
        switch (ver)
        {
            case GameVersion.Any:
            case GameVersion.SW or GameVersion.SH or GameVersion.SWSH:
                AddMachineSWSH(r, species, form);
                return;
            case GameVersion.BD or GameVersion.SP or GameVersion.BDSP:
                AddMachineBDSP(r, species, form);
                return;
        }
    }

    private static void AddMachine6XY(List<int> r, int species, int form)
    {
        var pi = PersonalTable.XY.GetFormEntry(species, form);
        AddPermittedIndexes(r, Legal.TMHM_XY, pi.TMHM);
    }

    private static void AddMachine6AO(List<int> r, int species, int form)
    {
        var pi = PersonalTable.AO.GetFormEntry(species, form);
        AddPermittedIndexes(r, Legal.TMHM_AO, pi.TMHM);
    }

    private static void AddMachineSM(List<int> r, int species, int form)
    {
        if (species > Legal.MaxSpeciesID_7)
            return;
        var pi = PersonalTable.SM.GetFormEntry(species, form);
        AddPermittedIndexes(r, Legal.TMHM_SM, pi.TMHM);
    }

    private static void AddMachineUSUM(List<int> r, int species, int form)
    {
        var pi = PersonalTable.USUM.GetFormEntry(species, form);
        AddPermittedIndexes(r, Legal.TMHM_SM, pi.TMHM);
    }

    private static void AddMachineGG(List<int> r, int species, int form)
    {
        if (species > Legal.MaxSpeciesID_7b)
            return;
        var pi = PersonalTable.GG.GetFormEntry(species, form);
        AddPermittedIndexes(r, Legal.TMHM_GG, pi.TMHM);
    }

    private static void AddMachineSWSH(List<int> r, int species, int form)
    {
        if (species > Legal.MaxSpeciesID_8)
            return;
        var pi = PersonalTable.SWSH.GetFormEntry(species, form);
        var tmhm = pi.TMHM;
        var arr = Legal.TMHM_SWSH.AsSpan(0, PersonalInfoSWSH.CountTM);
        AddPermittedIndexes(r, arr, tmhm);
    }

    private static void AddMachineBDSP(List<int> r, int species, int form)
    {
        if (species > Legal.MaxSpeciesID_8b)
            return;
        var pi = PersonalTable.BDSP.GetFormEntry(species, form);
        AddPermittedIndexes(r, Legal.TMHM_BDSP, pi.TMHM);
    }

    public static void AddRecordSWSH(List<int> r, int species, int form, PKM pk)
    {
        if (pk is not PK8 pk8)
            return;
        var pi = PersonalTable.SWSH.GetFormEntry(species, form);
        var tmhm = pi.TMHM;
        for (int i = 0; i < PersonalInfoSWSH.CountTR; i++)
        {
            var index = i + PersonalInfoSWSH.CountTM;
            if (!tmhm[index])
                continue;
            if (!pk8.GetMoveRecordFlag(i))
            {
                if (i == 12 && species == (int) Species.Calyrex && form == 0) // TR12
                {
                    // Unfuse logic does not check if the currently known TR move has the flag or not.
                    // Agility can be learned via Level Up while fused, but only via TR when Unfused.
                    // We'll let Agility be a legal TR move without the flag, only for Calyrex!
                }
                else
                {
                    continue;
                }
            }
            r.Add(Legal.TMHM_SWSH[index]);
        }
    }

    public static IEnumerable<int> GetAllPossibleRecords(int species, int form)
    {
        var pi = PersonalTable.SWSH.GetFormEntry(species, form);
        var tmhm = pi.TMHM;
        for (int i = 0; i < PersonalInfoSWSH.CountTM; i++)
        {
            var index = i + PersonalInfoSWSH.CountTM;
            if (!tmhm[index])
                continue;
            yield return Legal.TMHM_SWSH[index];
        }
    }
}
