using System;
using PKHeX.Core;
using PKHeX.Drawing;

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
            pk.Stat_Level = Experience.GetLevel(pk.EXP, pk.PersonalInfo.EXPGrowth);
            if (pk.Stat_Level == 100 && !HaX)
                pk.EXP = Experience.GetEXP(pk.Stat_Level, pk.PersonalInfo.EXPGrowth);

            CB_Species.SelectedValue = pk.Species;
            TB_Level.Text = pk.Stat_Level.ToString();
            TB_EXP.Text = pk.EXP.ToString();
        }

        private void SaveSpeciesLevelEXP(PKM pk)
        {
            pk.Species = WinFormsUtil.GetIndex(CB_Species);
            pk.EXP = Util.ToUInt32(TB_EXP.Text);
        }

        private void LoadOT(PKM pk)
        {
            GB_OT.BackgroundImage = null;
            TB_OT.Text = pk.OT_Name;
            Label_OTGender.Text = gendersymbols[pk.OT_Gender];
            Label_OTGender.ForeColor = Draw.GetGenderColor(pk.OT_Gender);
        }

        private void SaveOT(PKM pk)
        {
            pk.OT_Name = TB_OT.Text;
            pk.OT_Gender = PKX.GetGenderFromString(Label_OTGender.Text);
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

        private void LoadIVs(PKM pk) => Stats.LoadIVs(pk.IVs);
        private void LoadEVs(PKM pk) => Stats.LoadEVs(pk.EVs);
        private void LoadAVs(IAwakened a) => Stats.LoadAVs(a);

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
                CHK_Shadow.Checked = ck3.IsShadow;

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
            LoadOT(pk);
            LoadIVs(pk);
            LoadEVs(pk);
            LoadMoves(pk);
        }

        private void SaveMisc1(PKM pk)
        {
            SaveSpeciesLevelEXP(pk);
            SaveNickname(pk);
            SaveOT(pk);
            SaveMoves(pk);
        }

        private void LoadMisc2(PKM pk)
        {
            LoadPKRS(pk);
            CHK_IsEgg.Checked = pk.IsEgg;
            CB_HeldItem.SelectedValue = pk.HeldItem;
            CB_Form.SelectedIndex = CB_Form.Items.Count > pk.Form ? pk.Form : CB_Form.Items.Count - 1;
            if (pk is IFormArgument f)
                FA_Form.LoadArgument(f, pk.Species, pk.Form, pk.Format);

            TB_Friendship.Text = pk.CurrentFriendship.ToString();

            Label_HatchCounter.Visible = CHK_IsEgg.Checked && Entity.Format > 1;
            Label_Friendship.Visible = !CHK_IsEgg.Checked && Entity.Format > 1;
        }

        private void SaveMisc2(PKM pk)
        {
            SavePKRS(pk);
            pk.IsEgg = CHK_IsEgg.Checked;
            pk.HeldItem = WinFormsUtil.GetIndex(CB_HeldItem);
            pk.Form = (MT_Form.Enabled ? Convert.ToInt32(MT_Form.Text) : CB_Form.Enabled ? CB_Form.SelectedIndex : 0) & 0x1F;
            if (Entity is IFormArgument f)
                f.FormArgument = FA_Form.CurrentValue;
            pk.CurrentFriendship = Util.ToInt32(TB_Friendship.Text);
        }

        private void LoadMisc3(PKM pk)
        {
            TB_PID.Text = $"{pk.PID:X8}";
            Label_Gender.Text = gendersymbols[Math.Min(2, pk.Gender)];
            Label_Gender.ForeColor = Draw.GetGenderColor(pk.Gender);
            CB_Nature.SelectedValue = pk.Nature;
            CB_Language.SelectedValue = pk.Language;
            CB_GameOrigin.SelectedValue = pk.Version;
            CB_Ball.SelectedValue = pk.Ball;
            CB_MetLocation.SelectedValue = pk.Met_Location;
            TB_MetLevel.Text = pk.Met_Level.ToString();
            CHK_Fateful.Checked = pk.FatefulEncounter;

            if (pk is IContestStats s)
                s.CopyContestStatsTo(Contest);

            TID_Trainer.LoadIDValues(pk);

            // Load Extrabyte Value
            TB_ExtraByte.Text = pk.Data[Convert.ToInt32(CB_ExtraBytes.Text, 16)].ToString();
        }

        private void SaveMisc3(PKM pk)
        {
            pk.PID = Util.GetHexValue(TB_PID.Text);
            pk.Nature = WinFormsUtil.GetIndex(CB_Nature);
            pk.Gender = PKX.GetGenderFromString(Label_Gender.Text);

            if (pk is IContestStatsMutable s)
                Contest.CopyContestStatsTo(s);

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

            pk.Ability = WinFormsUtil.GetIndex(HaX ? DEV_Ability : CB_Ability);
        }

        private void LoadMisc6(PKM pk)
        {
            TB_EC.Text = $"{pk.EncryptionConstant:X8}";
            int abil = pk.AbilityNumber < 6 ? pk.AbilityNumber >> 1 : 0;
            if (CB_Ability.Items.Count <= abil)
                abil = CB_Ability.Items.Count - 1;
            CB_Ability.SelectedIndex = abil; // with some simple error handling
            DEV_Ability.SelectedValue = pk.Ability;
            TB_AbilityNumber.Text = pk.AbilityNumber.ToString();

            LoadRelearnMoves(pk);
            LoadHandlingTrainer(pk);

            if (pk is IRegionOrigin tr)
                LoadGeolocation(tr);
        }

        private void SaveMisc6(PKM pk)
        {
            pk.EncryptionConstant = Util.GetHexValue(TB_EC.Text);
            pk.AbilityNumber = Util.ToInt32(TB_AbilityNumber.Text);

            SaveRelearnMoves(pk);
            SaveHandlingTrainer(pk);

            if (pk is IRegionOrigin tr)
                SaveGeolocation(tr);
        }

        private void LoadGeolocation(IRegionOrigin pk)
        {
            CB_Country.SelectedValue = pk.Country;
            CB_SubRegion.SelectedValue = pk.Region;
            CB_3DSReg.SelectedValue = pk.ConsoleRegion;
        }

        private void SaveGeolocation(IRegionOrigin pk)
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
            Label_CTGender.ForeColor = Draw.GetGenderColor(gender);

            // Indicate who is currently in possession of the PKM
            UpadteHandlingTrainerBackground(pk);
        }

        private void UpadteHandlingTrainerBackground(PKM pk)
        {
            if (pk.CurrentHandler == 0) // OT
            {
                GB_OT.BackgroundImage = ImageUtil.ChangeOpacity(SpriteUtil.Spriter.Set, 0.5);
                GB_nOT.BackgroundImage = null;
            }
            else // Handling Trainer
            {
                GB_nOT.BackgroundImage = ImageUtil.ChangeOpacity(SpriteUtil.Spriter.Set, 0.5);
                GB_OT.BackgroundImage = null;
            }
        }

        private void SaveHandlingTrainer(PKM pk)
        {
            pk.HT_Name = TB_OTt2.Text;
            pk.HT_Gender = PKX.GetGenderFromString(Label_CTGender.Text) & 1;
        }

        // Misc
        private static void CheckTransferPIDValid(PKM pk)
        {
            var ver = pk.Version;
            if (ver is 0 or >= (int)GameVersion.X) // Gen6+ ignored
                return;

            uint EC = pk.EncryptionConstant;
            uint PID = pk.PID;
            uint LID = PID & 0xFFFF;
            uint HID = PID >> 16;
            uint XOR = (uint)(pk.TID ^ LID ^ pk.SID ^ HID);

            // Ensure we don't have a shiny.
            if (XOR >> 3 == 1) // Illegal, fix. (not 16<XOR>=8)
            {
                // Keep as shiny, so we have to mod the EC
                pk.EncryptionConstant = PID ^ 0x80000000;
            }
            else if ((XOR ^ 0x8000) >> 3 == 1 && PID != EC)
            {
                // Already anti-shiny, ensure the anti-shiny relationship is present.
                pk.EncryptionConstant = PID ^ 0x80000000;
            }
            else
            {
                // Ensure the copy correlation is present.
                pk.EncryptionConstant = PID;
            }
        }

        private void LoadAbility4(PKM pk)
        {
            var index = GetAbilityIndex4(pk);
            CB_Ability.SelectedIndex = Math.Min(CB_Ability.Items.Count - 1, index);
        }

        private static int GetAbilityIndex4(PKM pk)
        {
            var pi = pk.PersonalInfo;
            int abilityIndex = pi.GetAbilityIndex(pk.Ability);
            if (abilityIndex < 0)
                return 0;
            if (abilityIndex >= 2)
                return 2;

            var abils = pi.Abilities;
            if (abils[0] == abils[1])
                return pk.PIDAbility;
            return abilityIndex;
        }

        private void LoadMisc8(PK8 pk8)
        {
            CB_StatNature.SelectedValue = pk8.StatNature;
            Stats.CB_DynamaxLevel.SelectedIndex = pk8.DynamaxLevel;
            Stats.CHK_Gigantamax.Checked = pk8.CanGigantamax;
            CB_HTLanguage.SelectedValue = pk8.HT_Language;
            TB_HomeTracker.Text = pk8.Tracker.ToString("X16");
            CB_BattleVersion.SelectedValue = pk8.BattleVersion;
        }

        private void SaveMisc8(PK8 pk8)
        {
            pk8.StatNature = WinFormsUtil.GetIndex(CB_StatNature);
            pk8.DynamaxLevel = (byte)Math.Max(0, Stats.CB_DynamaxLevel.SelectedIndex);
            pk8.CanGigantamax = Stats.CHK_Gigantamax.Checked;
            pk8.HT_Language = WinFormsUtil.GetIndex(CB_HTLanguage);
            pk8.BattleVersion = WinFormsUtil.GetIndex(CB_BattleVersion);
        }
    }
}
