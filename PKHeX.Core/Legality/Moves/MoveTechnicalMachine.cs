using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public static class MoveTechnicalMachine
{
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
