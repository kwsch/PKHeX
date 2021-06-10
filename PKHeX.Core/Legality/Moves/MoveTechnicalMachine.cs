using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public static class MoveTechnicalMachine
    {
        public static GameVersion GetIsMachineMove(PKM pkm, int species, int form, int generation, int move, GameVersion ver = GameVersion.Any, bool RemoveTransfer = false)
        {
            if (pkm.IsMovesetRestricted(generation))
                ver = (GameVersion) pkm.Version;
            switch (generation)
            {
                case 1: return GetIsMachine1(species, move);
                case 2:
                    if (pkm.VC1 && move > Legal.MaxMoveID_1)
                        return Legal.NONE;
                    return GetIsMachine2(species, move);
                case 3: return GetIsMachine3(species, move, pkm.Format, RemoveTransfer);
                case 4: return GetIsMachine4(species, move, pkm.Format, RemoveTransfer, form);
                case 5: return GetIsMachine5(species, move, form);
                case 6: return GetIsMachine6(species, move, form, ver);
                case 7: return GetIsMachine7(species, move, form, ver);
                case 8: return GetIsMachine8(species, move, form, ver);
                default:
                    return Legal.NONE;
            }
        }

        public static GameVersion GetIsRecordMove(PKM pkm, int species, int form, int generation, int move, GameVersion ver = GameVersion.Any, bool allowBit = false)
        {
            if (pkm.IsMovesetRestricted(generation))
                ver = (GameVersion)pkm.Version;
            return generation switch
            {
                8 => GetIsRecord8(pkm, species, move, form, ver, allowBit),
                _ => Legal.NONE
            };
        }

        private static GameVersion GetIsMachine1(int species, int move)
        {
            for (int i = 0; i < Legal.TMHM_RBY.Length; i++)
            {
                if (Legal.TMHM_RBY[i] != move)
                    continue;
                if (PersonalTable.RB[species].TMHM[i])
                    return GameVersion.RB;
                if (PersonalTable.Y[species].TMHM[i])
                    return GameVersion.YW;
            }

            return Legal.NONE;
        }

        private static GameVersion GetIsMachine2(int species, int move)
        {
            for (int i = 0; i < Legal.TMHM_GSC.Length; i++)
            {
                if (Legal.TMHM_GSC[i] == move && PersonalTable.C[species].TMHM[i])
                    return GameVersion.GS;
            }

            return Legal.NONE;
        }

        private static GameVersion GetIsMachine3(int species, int move, int format, bool RemoveTransfer)
        {
            for (int i = 0; i < Legal.TM_3.Length; i++)
            {
                if (Legal.TM_3[i] == move && PersonalTable.E[species].TMHM[i])
                    return GameVersion.Gen3;
            }

            if (!RemoveTransfer && format <= 3)
                return GetIsMachine3HM(species, move);

            return Legal.NONE;
        }

        private static GameVersion GetIsMachine3HM(int species, int move)
        {
            int x = 0;
            foreach (var m in Legal.HM_3)
            {
                if (m == move && PersonalTable.E[species].TMHM[x + 50])
                    return GameVersion.Gen3;
                x++;
            }
            return Legal.NONE;
        }

        private static GameVersion GetIsMachine4(int species, int move, int format, bool RemoveTransfer, int form)
        {
            for (int i = 0; i < Legal.TM_3.Length; i++)
            {
                if (Legal.TM_4[i] == move && PersonalTable.HGSS.GetFormEntry(species, form).TMHM[i])
                    return GameVersion.Gen4;
            }

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
            var dp = Legal.HM_DPPt;
            for (var i = 0; i < dp.Length; i++)
            {
                var m = dp[i];
                if (m != move)
                    continue;
                if (PersonalTable.Pt.GetFormEntry(species, form).TMHM[i + 92])
                    return GameVersion.DPPt;
                break;
            }

            var hgss = Legal.HM_HGSS;
            for (var i = 0; i < hgss.Length; i++)
            {
                var m = hgss[i];
                if (m != move)
                    continue;
                if (PersonalTable.HGSS.GetFormEntry(species, form).TMHM[i + 92])
                    return GameVersion.HGSS;
                break;
            }

            return Legal.NONE;
        }

        private static GameVersion GetIsMachine5(int species, int move, int form)
        {
            for (int i = 0; i < Legal.TMHM_BW.Length; i++)
            {
                if (Legal.TMHM_BW[i] == move)
                    return PersonalTable.B2W2.GetFormEntry(species, form).TMHM[i] ? GameVersion.Gen5 : Legal.NONE;
            }
            return Legal.NONE;
        }

        private static GameVersion GetIsMachine6(int species, int move, int form, GameVersion ver)
        {
            if (GameVersion.XY.Contains(ver))
            {
                for (int i = 0; i < Legal.TMHM_XY.Length; i++)
                {
                    if (Legal.TMHM_XY[i] != move)
                        continue;
                    if (PersonalTable.XY.GetFormEntry(species, form).TMHM[i])
                        return GameVersion.XY;
                    break;
                }
            }

            if (GameVersion.ORAS.Contains(ver))
            {
                for (int i = 0; i < Legal.TMHM_AO.Length; i++)
                {
                    if (Legal.TMHM_AO[i] != move)
                        continue;
                    if (PersonalTable.AO.GetFormEntry(species, form).TMHM[i])
                        return GameVersion.ORAS;
                    break;
                }
            }

            return Legal.NONE;
        }

        private static GameVersion GetIsMachine7(int species, int move, int form, GameVersion ver)
        {
            if (GameVersion.Gen7b.Contains(ver))
            {
                for (int i = 0; i < Legal.TMHM_GG.Length; i++)
                {
                    if (Legal.TMHM_GG[i] != move)
                        continue;
                    if (PersonalTable.GG.GetFormEntry(species, form).TMHM[i])
                        return GameVersion.GG;
                    break;
                }
            }

            if (GameVersion.SM.Contains(ver) && species <= Legal.MaxSpeciesID_7)
            {
                for (int i = 0; i < Legal.TMHM_SM.Length; i++)
                {
                    if (Legal.TMHM_SM[i] != move)
                        continue;
                    if (PersonalTable.SM.GetFormEntry(species, form).TMHM[i])
                        return GameVersion.SM;
                    break;
                }
            }

            if (GameVersion.USUM.Contains(ver) && species <= Legal.MaxSpeciesID_7_USUM)
            {
                for (int i = 0; i < Legal.TMHM_SM.Length; i++)
                {
                    if (Legal.TMHM_SM[i] != move)
                        continue;
                    if (PersonalTable.USUM.GetFormEntry(species, form).TMHM[i])
                        return GameVersion.USUM;
                    break;
                }
            }

            return Legal.NONE;
        }

        private static GameVersion GetIsMachine8(int species, int move, int form, GameVersion ver)
        {
            if (GameVersion.SWSH.Contains(ver))
            {
                for (int i = 0; i < 100; i++)
                {
                    if (Legal.TMHM_SWSH[i] != move)
                        continue;
                    if (PersonalTable.SWSH.GetFormEntry(species, form).TMHM[i])
                        return GameVersion.SWSH;
                    break;
                }
            }

            return Legal.NONE;
        }

        private static GameVersion GetIsRecord8(PKM pkm, int species, int move, int form, GameVersion ver, bool allowBit)
        {
            if (GameVersion.SWSH.Contains(ver))
            {
                for (int i = 0; i < 100; i++)
                {
                    if (Legal.TMHM_SWSH[i + 100] != move)
                        continue;
                    if (!PersonalTable.SWSH.GetFormEntry(species, form).TMHM[i + 100])
                        break;
                    if (allowBit)
                        return GameVersion.SWSH;
                    if (((PK8)pkm).GetMoveRecordFlag(i))
                        return GameVersion.SWSH;
                    if (i == 12 && species == (int)Species.Calyrex && form == 0) // TR12
                        return GameVersion.SWSH; // Agility Calyrex without TR glitch.
                    break;
                }
            }

            return Legal.NONE;
        }

        public static IEnumerable<int> GetTMHM(PKM pkm, int species, int form, int generation, GameVersion ver = GameVersion.Any, bool RemoveTransfer = true)
        {
            var r = new List<int>();
            if (pkm.IsMovesetRestricted(generation))
                ver = (GameVersion)pkm.Version;

            switch (generation)
            {
                case 1: AddMachine1(r, species); break;
                case 2: AddMachine2(r, species);
                    if (pkm.Format >= 7 && pkm.VC1)
                        r.RemoveAll(z => z > Legal.MaxMoveID_1);
                    break;
                case 3: AddMachine3(r, species, pkm.Format, RemoveTransfer); break;
                case 4: AddMachine4(r, species, pkm.Format, RemoveTransfer, form); break;
                case 5: AddMachine5(r, species, form); break;
                case 6: AddMachine6(r, species, form, ver); break;
                case 7: AddMachine7(r, species, form, ver); break;
                case 8: AddMachine8(r, species, form, ver); break;
            }
            return r.Distinct();
        }

        public static IEnumerable<int> GetRecords(PKM pkm, int species, int form, int generation)
        {
            var r = new List<int>();
            switch (generation)
            {
                case 8: AddRecordSWSH(r, species, form, pkm); break;
            }
            return r.Distinct();
        }

        private static void AddMachine1(List<int> r, int species)
        {
            int index = PersonalTable.RB.GetFormIndex(species, 0);
            if (index == 0)
                return;
            var pi_rb = (PersonalInfoG1)PersonalTable.RB[index];
            var pi_y = (PersonalInfoG1)PersonalTable.Y[index];
            r.AddRange(Legal.TMHM_RBY.Where((_, m) => pi_rb.TMHM[m]));
            r.AddRange(Legal.TMHM_RBY.Where((_, m) => pi_y.TMHM[m]));
        }

        private static void AddMachine2(List<int> r, int species)
        {
            int index = PersonalTable.C.GetFormIndex(species, 0);
            if (index == 0)
                return;
            var pi_c = PersonalTable.C[index];
            r.AddRange(Legal.TMHM_GSC.Where((_, m) => pi_c.TMHM[m]));
        }

        private static void AddMachine3(List<int> r, int species, int format, bool RemoveTransfer)
        {
            int index = PersonalTable.E.GetFormIndex(species, 0);
            if (index == 0)
                return;
            var pi_c = PersonalTable.E[index];
            r.AddRange(Legal.TM_3.Where((_, m) => pi_c.TMHM[m]));

            if (!RemoveTransfer || format == 3) // HM moves must be removed for 3->4, only give if current format.
                r.AddRange(Legal.HM_3.Where((_, m) => pi_c.TMHM[m + 50]));
            else if (format > 3) //Remove HM
                r.AddRange(Legal.HM_3.Where((_, m) => pi_c.TMHM[m + 50]).Except(Legal.HM_3));
        }

        private static void AddMachine4(List<int> r, int species, int format, bool RemoveTransfer, int form)
        {
            var pi_hgss = PersonalTable.HGSS.GetFormEntry(species, form);
            var pi_dppt = PersonalTable.Pt.GetFormEntry(species, form);
            r.AddRange(Legal.TM_4.Where((_, m) => pi_hgss.TMHM[m]));

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
                r.AddRange(Legal.HM_DPPt.Where((_, m) => pi_dppt.TMHM[m + 92]));
                r.AddRange(Legal.HM_HGSS.Where((_, m) => pi_hgss.TMHM[m + 92]));
            }
        }

        private static void AddMachine5(List<int> r, int species, int form)
        {
            var pi = PersonalTable.B2W2.GetFormEntry(species, form);
            r.AddRange(Legal.TMHM_BW.Where((_, m) => pi.TMHM[m]));
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
            }
        }

        private static void AddMachine6XY(List<int> r, int species, int form)
        {
            var pi = PersonalTable.XY.GetFormEntry(species, form);
            r.AddRange(Legal.TMHM_XY.Where((_, m) => pi.TMHM[m]));
        }

        private static void AddMachine6AO(List<int> r, int species, int form)
        {
            var pi = PersonalTable.AO.GetFormEntry(species, form);
            r.AddRange(Legal.TMHM_AO.Where((_, m) => pi.TMHM[m]));
        }

        private static void AddMachineSM(List<int> r, int species, int form)
        {
            if (species > Legal.MaxSpeciesID_7)
                return;
            var pi = PersonalTable.SM.GetFormEntry(species, form);
            r.AddRange(Legal.TMHM_SM.Where((_, m) => pi.TMHM[m]));
        }

        private static void AddMachineUSUM(List<int> r, int species, int form)
        {
            var pi = PersonalTable.USUM.GetFormEntry(species, form);
            r.AddRange(Legal.TMHM_SM.Where((_, m) => pi.TMHM[m]));
        }

        private static void AddMachineGG(List<int> r, int species, int form)
        {
            if (species > Legal.MaxSpeciesID_7b)
                return;
            var pi = PersonalTable.GG.GetFormEntry(species, form);
            r.AddRange(Legal.TMHM_GG.Where((_, m) => pi.TMHM[m]));
        }

        private static void AddMachineSWSH(List<int> r, int species, int form)
        {
            if (species > Legal.MaxSpeciesID_8)
                return;
            var pi = PersonalTable.SWSH.GetFormEntry(species, form);
            var tmhm = pi.TMHM;
            for (int i = 0; i < 100; i++)
            {
                if (!tmhm[i])
                    continue;
                r.Add(Legal.TMHM_SWSH[i]);
            }
        }

        public static void AddRecordSWSH(List<int> r, int species, int form, PKM pkm)
        {
            var pi = PersonalTable.SWSH.GetFormEntry(species, form);
            var tmhm = pi.TMHM;
            var pk8 = (PK8)pkm;
            for (int i = 0; i < 100; i++)
            {
                if (!tmhm[i + 100])
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
                r.Add(Legal.TMHM_SWSH[i + 100]);
            }
        }

        public static IEnumerable<int> GetAllPossibleRecords(int species, int form)
        {
            var pi = PersonalTable.SWSH.GetFormEntry(species, form);
            var tmhm = pi.TMHM;
            for (int i = 0; i < 100; i++)
            {
                if (!tmhm[i + 100])
                    continue;
                yield return Legal.TMHM_SWSH[i + 100];
            }
        }
    }
}
