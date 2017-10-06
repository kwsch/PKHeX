using System;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class PKMEditor
    {
        private void LoadNickname(PKM pk)
        {
            CHK_Nicknamed.Checked = pk.IsNicknamed;
            TB_Nickname.Text = pk.Nickname;
        }
        private void SaveNickname(PKM pk)
        {
            pk.IsNicknamed = CHK_Nicknamed.Checked;
            pk.Nickname = TB_Nickname.Text;
        }

        private void LoadSpeciesLevelEXP(PKM pk)
        {
            // Do first
            pk.Stat_Level = PKX.GetLevel(pk.Species, pk.EXP);
            if (pk.Stat_Level == 100 && !HaX)
                pk.EXP = PKX.GetEXP(pk.Stat_Level, pk.Species);

            CB_Species.SelectedValue = pk.Species;
            TB_Level.Text = pk.Stat_Level.ToString();
            TB_EXP.Text = pk.EXP.ToString();
        }
        private void SaveSpeciesLevelEXP(PKM pk)
        {
            pk.Species = WinFormsUtil.GetIndex(CB_Species);
            pk.EXP = Util.ToUInt32(TB_EXP.Text);
        }

        private void LoadOTID(PKM pk)
        {
            GB_OT.BackgroundImage = null;
            TB_OT.Text = pk.OT_Name;
            Label_OTGender.Text = gendersymbols[pk.OT_Gender];
            Label_OTGender.ForeColor = GetGenderColor(pk.OT_Gender);
            TB_TID.Text = $"{pk.TID:00000}";
            TB_SID.Text = $"{pk.SID:00000}";
        }
        private void SaveOTID(PKM pk)
        {
            pk.OT_Name = TB_OT.Text;
            pk.OT_Gender = PKX.GetGenderFromString(Label_OTGender.Text);
            pk.TID = Util.ToInt32(TB_TID.Text);
            pk.SID = Util.ToInt32(TB_SID.Text);
        }

        private void LoadPKRS(PKM pk)
        {
            Label_PKRS.Visible = CB_PKRSStrain.Visible = CHK_Infected.Checked = pk.PKRS_Strain != 0;
            Label_PKRSdays.Visible = CB_PKRSDays.Visible = pk.PKRS_Days != 0;
            CB_PKRSStrain.SelectedIndex = pk.PKRS_Strain;
            CHK_Cured.Checked = pk.PKRS_Strain > 0 && pk.PKRS_Days == 0;
            CB_PKRSDays.SelectedIndex = Math.Min(CB_PKRSDays.Items.Count - 1, pk.PKRS_Days); // to strip out bad hacked 'rus
        }
        private void SavePKRS(PKM pk)
        {
            pk.PKRS_Days = CB_PKRSDays.SelectedIndex;
            pk.PKRS_Strain = CB_PKRSStrain.SelectedIndex;
        }

        private void LoadPartyStats(PKM pk)
        {
            Stat_HP.Text = pk.Stat_HPCurrent.ToString();
            Stat_ATK.Text = pk.Stat_ATK.ToString();
            Stat_DEF.Text = pk.Stat_DEF.ToString();
            Stat_SPA.Text = pk.Stat_SPA.ToString();
            Stat_SPD.Text = pk.Stat_SPD.ToString();
            Stat_SPE.Text = pk.Stat_SPE.ToString();
        }
        private void SavePartyStats(PKM pk)
        {
            int size = pk.SIZE_PARTY;
            if (pk.Data.Length != size)
                Array.Resize(ref pk.Data, size);

            pk.Stat_HPCurrent = Util.ToInt32(Stat_HP.Text);
            pk.Stat_HPMax = Util.ToInt32(Stat_HP.Text);
            pk.Stat_ATK = Util.ToInt32(Stat_ATK.Text);
            pk.Stat_DEF = Util.ToInt32(Stat_DEF.Text);
            pk.Stat_SPE = Util.ToInt32(Stat_SPE.Text);
            pk.Stat_SPA = Util.ToInt32(Stat_SPA.Text);
            pk.Stat_SPD = Util.ToInt32(Stat_SPD.Text);

            pk.Stat_Level = HaX
                ? (byte)Math.Min(Convert.ToInt32(MT_Level.Text), byte.MaxValue)
                : Util.ToInt32(TB_Level.Text);
        }

        private void LoadContestStats(PKM pk)
        {
            Contest.Cool = pk.CNT_Cool;
            Contest.Beauty = pk.CNT_Beauty;
            Contest.Cute = pk.CNT_Cute;
            Contest.Smart = pk.CNT_Smart;
            Contest.Tough = pk.CNT_Tough;
            Contest.Sheen = pk.CNT_Sheen;
        }
        private void SaveContestStats(PKM pk)
        {
            pk.CNT_Cool = Contest.Cool;
            pk.CNT_Beauty = Contest.Beauty;
            pk.CNT_Cute = Contest.Cute;
            pk.CNT_Smart = Contest.Smart;
            pk.CNT_Tough = Contest.Tough;
            pk.CNT_Sheen = Contest.Sheen;
        }

        private void LoadIVs(PKM pk)
        {
            TB_HPIV.Text = pk.IV_HP.ToString();
            TB_ATKIV.Text = pk.IV_ATK.ToString();
            TB_DEFIV.Text = pk.IV_DEF.ToString();
            TB_SPEIV.Text = pk.IV_SPE.ToString();
            TB_SPAIV.Text = pk.IV_SPA.ToString();
            TB_SPDIV.Text = pk.IV_SPD.ToString();
            CB_HPType.SelectedValue = pk.HPType;
        }
        private void SaveIVs(PKM pk)
        {
            pk.IV_HP = Util.ToInt32(TB_HPIV.Text);
            pk.IV_ATK = Util.ToInt32(TB_ATKIV.Text);
            pk.IV_DEF = Util.ToInt32(TB_DEFIV.Text);
            pk.IV_SPE = Util.ToInt32(TB_SPEIV.Text);
            pk.IV_SPA = Util.ToInt32(TB_SPAIV.Text);
            pk.IV_SPD = Util.ToInt32(TB_SPDIV.Text);
        }

        private void LoadEVs(PKM pk)
        {
            TB_HPEV.Text = pk.EV_HP.ToString();
            TB_ATKEV.Text = pk.EV_ATK.ToString();
            TB_DEFEV.Text = pk.EV_DEF.ToString();
            TB_SPEEV.Text = pk.EV_SPE.ToString();
            TB_SPAEV.Text = pk.EV_SPA.ToString();
            TB_SPDEV.Text = pk.EV_SPD.ToString();
        }
        private void SaveEVs(PKM pk)
        {
            pk.EV_HP = Util.ToInt32(TB_HPEV.Text);
            pk.EV_ATK = Util.ToInt32(TB_ATKEV.Text);
            pk.EV_DEF = Util.ToInt32(TB_DEFEV.Text);
            pk.EV_SPE = Util.ToInt32(TB_SPEEV.Text);
            pk.EV_SPA = Util.ToInt32(TB_SPAEV.Text);
            pk.EV_SPD = Util.ToInt32(TB_SPDEV.Text);
        }

        private void LoadMoves(PKM pk)
        {
            CB_Move1.SelectedValue = pk.Move1;
            CB_Move2.SelectedValue = pk.Move2;
            CB_Move3.SelectedValue = pk.Move3;
            CB_Move4.SelectedValue = pk.Move4;
            CB_PPu1.SelectedIndex = pk.Move1_PPUps;
            CB_PPu2.SelectedIndex = pk.Move2_PPUps;
            CB_PPu3.SelectedIndex = pk.Move3_PPUps;
            CB_PPu4.SelectedIndex = pk.Move4_PPUps;
            TB_PP1.Text = pk.Move1_PP.ToString();
            TB_PP2.Text = pk.Move2_PP.ToString();
            TB_PP3.Text = pk.Move3_PP.ToString();
            TB_PP4.Text = pk.Move4_PP.ToString();
        }
        private void SaveMoves(PKM pk)
        {
            pk.Move1 = WinFormsUtil.GetIndex(CB_Move1);
            pk.Move2 = WinFormsUtil.GetIndex(CB_Move2);
            pk.Move3 = WinFormsUtil.GetIndex(CB_Move3);
            pk.Move4 = WinFormsUtil.GetIndex(CB_Move4);
            pk.Move1_PP = WinFormsUtil.GetIndex(CB_Move1) > 0 ? Util.ToInt32(TB_PP1.Text) : 0;
            pk.Move2_PP = WinFormsUtil.GetIndex(CB_Move2) > 0 ? Util.ToInt32(TB_PP2.Text) : 0;
            pk.Move3_PP = WinFormsUtil.GetIndex(CB_Move3) > 0 ? Util.ToInt32(TB_PP3.Text) : 0;
            pk.Move4_PP = WinFormsUtil.GetIndex(CB_Move4) > 0 ? Util.ToInt32(TB_PP4.Text) : 0;
            pk.Move1_PPUps = WinFormsUtil.GetIndex(CB_Move1) > 0 ? CB_PPu1.SelectedIndex : 0;
            pk.Move2_PPUps = WinFormsUtil.GetIndex(CB_Move2) > 0 ? CB_PPu2.SelectedIndex : 0;
            pk.Move3_PPUps = WinFormsUtil.GetIndex(CB_Move3) > 0 ? CB_PPu3.SelectedIndex : 0;
            pk.Move4_PPUps = WinFormsUtil.GetIndex(CB_Move4) > 0 ? CB_PPu4.SelectedIndex : 0;
        }

        private void LoadShadow3(IShadowPKM ck3)
        {
            NUD_ShadowID.Value = ck3.ShadowID;
            FLP_Purification.Visible = ck3.ShadowID > 0;
            if (ck3.ShadowID > 0)
            {
                int value = ck3.Purification;
                if (value < NUD_Purification.Minimum)
                    value = (int)NUD_Purification.Minimum;

                NUD_Purification.Value = value;
                CHK_Shadow.Checked = value > 0;

                NUD_ShadowID.Value = Math.Max(ck3.ShadowID, 0);
            }
            else
            {
                NUD_Purification.Value = 0;
                CHK_Shadow.Checked = false;
                NUD_ShadowID.Value = 0;
            }
        }
        private void SaveShadow3(IShadowPKM ck3)
        {
            ck3.ShadowID = (int)NUD_ShadowID.Value;
            if (ck3.ShadowID > 0)
                ck3.Purification = (int)NUD_Purification.Value;
        }

        private void LoadRelearnMoves(PKM pk)
        {
            CB_RelearnMove1.SelectedValue = pk.RelearnMove1;
            CB_RelearnMove2.SelectedValue = pk.RelearnMove2;
            CB_RelearnMove3.SelectedValue = pk.RelearnMove3;
            CB_RelearnMove4.SelectedValue = pk.RelearnMove4;
        }
        private void SaveRelearnMoves(PKM pk7)
        {
            pk7.RelearnMove1 = WinFormsUtil.GetIndex(CB_RelearnMove1);
            pk7.RelearnMove2 = WinFormsUtil.GetIndex(CB_RelearnMove2);
            pk7.RelearnMove3 = WinFormsUtil.GetIndex(CB_RelearnMove3);
            pk7.RelearnMove4 = WinFormsUtil.GetIndex(CB_RelearnMove4);
        }

        private void LoadMisc1(PKM pk)
        {
            LoadSpeciesLevelEXP(pk);
            LoadNickname(pk);
            LoadOTID(pk);
            LoadIVs(pk);
            LoadEVs(pk);
            LoadMoves(pk);
        }
        private void SaveMisc1(PKM pk)
        {
            SaveSpeciesLevelEXP(pk);
            SaveNickname(pk);
            SaveOTID(pk);
            SaveIVs(pk);
            SaveEVs(pk);
            SaveMoves(pk);
        }

        private void LoadMisc2(PKM pk)
        {
            LoadPKRS(pk);
            CHK_IsEgg.Checked = pk.IsEgg;
            CB_HeldItem.SelectedValue = pk.HeldItem;
            CB_Form.SelectedIndex = CB_Form.Items.Count > pk.AltForm ? pk.AltForm : CB_Form.Items.Count - 1;
            TB_Friendship.Text = pk.CurrentFriendship.ToString();
        }
        private void SaveMisc2(PKM pk)
        {
            SavePKRS(pk);
            pk.IsEgg = CHK_IsEgg.Checked;
            pk.HeldItem = WinFormsUtil.GetIndex(CB_HeldItem);
            pk.AltForm = (MT_Form.Enabled ? Convert.ToInt32(MT_Form.Text) : CB_Form.Enabled ? CB_Form.SelectedIndex : 0) & 0x1F;
            pk.CurrentFriendship = Util.ToInt32(TB_Friendship.Text);
        }

        private void LoadMisc3(PKM pk)
        {
            TB_PID.Text = $"{pk.PID:X8}";
            Label_Gender.Text = gendersymbols[pk.Gender];
            Label_Gender.ForeColor = GetGenderColor(pk.Gender);
            CB_Nature.SelectedValue = pk.Nature;
            CB_Language.SelectedValue = pk.Language;
            CB_GameOrigin.SelectedValue = pk.Version;
            CB_Ball.SelectedValue = pk.Ball;
            CB_MetLocation.SelectedValue = pk.Met_Location;
            TB_MetLevel.Text = pk.Met_Level.ToString();
            CHK_Fateful.Checked = pk.FatefulEncounter;

            LoadContestStats(pk);

            // Load Extrabyte Value
            TB_ExtraByte.Text = pk.Data[Convert.ToInt32(CB_ExtraBytes.Text, 16)].ToString();
        }
        private void SaveMisc3(PKM pk)
        {
            pk.PID = Util.GetHexValue(TB_PID.Text);
            pk.Nature = WinFormsUtil.GetIndex(CB_Nature);
            pk.Gender = PKX.GetGenderFromString(Label_Gender.Text);

            SaveContestStats(pk);

            pk.FatefulEncounter = CHK_Fateful.Checked;
            pk.Ball = WinFormsUtil.GetIndex(CB_Ball);
            pk.Version = WinFormsUtil.GetIndex(CB_GameOrigin);
            pk.Language = WinFormsUtil.GetIndex(CB_Language);
            pk.Met_Level = Util.ToInt32(TB_MetLevel.Text);
            pk.Met_Location = WinFormsUtil.GetIndex(CB_MetLocation);
        }

        private void LoadMisc4(PKM pk)
        {
            CAL_MetDate.Value = pk.MetDate ?? new DateTime(2000, 1, 1);
            if (pk.Egg_Location == 0)
            {
                CHK_AsEgg.Checked = GB_EggConditions.Enabled = false;

                CB_EggLocation.SelectedValue = 0;
                CAL_EggDate.Value = new DateTime(2000, 01, 01);
            }
            else
            {
                // Was obtained initially as an egg.
                CHK_AsEgg.Checked = GB_EggConditions.Enabled = true;

                CB_EggLocation.SelectedValue = pk.Egg_Location;
                CAL_EggDate.Value = pk.EggMetDate ?? new DateTime(2000, 1, 1);
            }
        }
        private void SaveMisc4(PKM pk)
        {
            pk.MetDate = CAL_MetDate.Value;

            // Default Dates
            DateTime? egg_date = null;
            int egg_location = 0;
            if (CHK_AsEgg.Checked) // If encountered as an egg, load the Egg Met data from fields.
            {
                egg_date = CAL_EggDate.Value;
                egg_location = WinFormsUtil.GetIndex(CB_EggLocation);
            }
            // Egg Met Data
            pk.EggMetDate = egg_date;
            pk.Egg_Location = egg_location;
            if (pk.IsEgg && pk.Met_Location == 0) // If still an egg, it has no hatch location/date. Zero it!
                pk.MetDate = null;

            pk.Ability = (byte)WinFormsUtil.GetIndex(HaX ? DEV_Ability : CB_Ability);
        }

        private void LoadMisc6(PKM pk)
        {
            TB_EC.Text = $"{pk.EncryptionConstant:X8}";
            CB_Ability.SelectedIndex = pk.AbilityNumber < 6 ? pk.AbilityNumber >> 1 : 0; // with some simple error handling
            DEV_Ability.SelectedValue = pk.Ability;
            TB_AbilityNumber.Text = pk.AbilityNumber.ToString();

            LoadRelearnMoves(pk);
            LoadHandlingTrainer(pk);
            LoadGeolocation(pk);
        }
        private void SaveMisc6(PKM pk)
        {
            pk.EncryptionConstant = Util.GetHexValue(TB_EC.Text);
            pk.AbilityNumber = Util.ToInt32(TB_AbilityNumber.Text);

            SaveRelearnMoves(pk);
            SaveHandlingTrainer(pk);
            SaveGeolocation(pk);
        }

        private void LoadGeolocation(PKM pk)
        {
            CB_Country.SelectedValue = pk.Country;
            CB_SubRegion.SelectedValue = pk.Region;
            CB_3DSReg.SelectedValue = pk.ConsoleRegion;
        }
        private void SaveGeolocation(PKM pk)
        {
            pk.Country = WinFormsUtil.GetIndex(CB_Country);
            pk.Region = WinFormsUtil.GetIndex(CB_SubRegion);
            pk.ConsoleRegion = WinFormsUtil.GetIndex(CB_3DSReg);
        }

        private void LoadHandlingTrainer(PKM pk)
        {
            TB_OTt2.Text = pk.HT_Name;
            int gender = pk.HT_Gender & 1;
            // Set CT Gender to None if no CT, else set to gender symbol.
            Label_CTGender.Text = string.IsNullOrEmpty(pk.HT_Name) ? string.Empty : gendersymbols[gender];
            Label_CTGender.ForeColor = GetGenderColor(gender);

            // Indicate who is currently in posession of the PKM
            if (pk.CurrentHandler == 0) // OT
            {
                GB_OT.BackgroundImage = mixedHighlight;
                GB_nOT.BackgroundImage = null;
            }
            else // Handling Trainer
            {
                GB_nOT.BackgroundImage = mixedHighlight;
                GB_OT.BackgroundImage = null;
            }
        }
        private void SaveHandlingTrainer(PKM pk)
        {
            pk.HT_Name = TB_OTt2.Text;
            pk.HT_Gender = PKX.GetGenderFromString(Label_CTGender.Text) & 1;
        }

        // Misc
        private void CheckTransferPIDValid()
        {
            if (pkm.Version >= 24)
                return;

            uint EC = Util.GetHexValue(TB_EC.Text);
            uint PID = Util.GetHexValue(TB_PID.Text);
            uint SID = Util.ToUInt32(TB_SID.Text);
            uint TID = Util.ToUInt32(TB_TID.Text);
            uint LID = PID & 0xFFFF;
            uint HID = PID >> 16;
            uint XOR = TID ^ LID ^ SID ^ HID;

            // Ensure we don't have a shiny.
            if (XOR >> 3 == 1) // Illegal, fix. (not 16<XOR>=8)
            {
                // Keep as shiny, so we have to mod the PID
                PID ^= XOR;
                TB_PID.Text = PID.ToString("X8");
                TB_EC.Text = PID.ToString("X8");
            }
            else if ((XOR ^ 0x8000) >> 3 == 1 && PID != EC)
                TB_EC.Text = (PID ^ 0x80000000).ToString("X8");
            else // Not illegal, no fix.
                TB_EC.Text = PID.ToString("X8");
        }
        private void LoadAbility4(PKM pk)
        {
            int[] abils = pk.PersonalInfo.Abilities;
            int abil = Array.IndexOf(abils, pk.Ability);

            if (abil < 0)
                CB_Ability.SelectedIndex = 0;
            if (abil == 2)
                CB_Ability.SelectedIndex = 2;
            else if (abils[0] == abils[1] || abils[1] == 0)
                CB_Ability.SelectedIndex = pk.PIDAbility;
            else
                CB_Ability.SelectedIndex = abil >= CB_Ability.Items.Count ? 0 : abil;
        }
    }
}
