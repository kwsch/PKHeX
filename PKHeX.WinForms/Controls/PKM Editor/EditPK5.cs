using System;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class PKMEditor
    {
        private void PopulateFieldsPK5()
        {
            PK5 pk5 = pkm as PK5;
            if (pk5 == null)
                return;

            // Do first
            pk5.Stat_Level = PKX.GetLevel(pk5.Species, pk5.EXP);
            if (pk5.Stat_Level == 100 && !HaX)
                pk5.EXP = PKX.GetEXP(pk5.Stat_Level, pk5.Species);

            CB_Species.SelectedValue = pk5.Species;
            TB_Level.Text = pk5.Stat_Level.ToString();
            TB_EXP.Text = pk5.EXP.ToString();

            // Load rest
            CHK_Fateful.Checked = pk5.FatefulEncounter;
            CHK_IsEgg.Checked = pk5.IsEgg;
            CHK_Nicknamed.Checked = pk5.IsNicknamed;
            Label_OTGender.Text = gendersymbols[pk5.OT_Gender];
            Label_OTGender.ForeColor = GetGenderColor(pk5.OT_Gender);
            TB_PID.Text = pk5.PID.ToString("X8");
            CB_HeldItem.SelectedValue = pk5.HeldItem;
            CB_Nature.SelectedValue = pk5.Nature;
            TB_TID.Text = pk5.TID.ToString("00000");
            TB_SID.Text = pk5.SID.ToString("00000");
            TB_Nickname.Text = pk5.Nickname;
            TB_OT.Text = pk5.OT_Name;
            TB_Friendship.Text = pk5.CurrentFriendship.ToString();
            if (pk5.CurrentHandler == 1)  // HT
            {
                GB_nOT.BackgroundImage = mixedHighlight;
                GB_OT.BackgroundImage = null;
            }
            else                  // = 0
            {
                GB_OT.BackgroundImage = mixedHighlight;
                GB_nOT.BackgroundImage = null;
            }
            CB_Language.SelectedValue = pk5.Language;
            CB_GameOrigin.SelectedValue = pk5.Version;
            CB_EncounterType.SelectedValue = pk5.Gen4 ? pk5.EncounterType : 0;
            CB_Ball.SelectedValue = pk5.Ball;

            CAL_MetDate.Value = pk5.MetDate ?? new DateTime(2000, 1, 1);

            if (pk5.Egg_Location != 0)
            {
                // Was obtained initially as an egg.
                CHK_AsEgg.Checked = true;
                GB_EggConditions.Enabled = true;

                CB_EggLocation.SelectedValue = pk5.Egg_Location;
                CAL_EggDate.Value = pk5.EggMetDate ?? new DateTime(2000, 1, 1);
            }
            else { CAL_EggDate.Value = new DateTime(2000, 01, 01); CHK_AsEgg.Checked = GB_EggConditions.Enabled = false; CB_EggLocation.SelectedValue = 0; }

            CB_MetLocation.SelectedValue = pk5.Met_Location;

            TB_MetLevel.Text = pk5.Met_Level.ToString();

            // Reset Label and ComboBox visibility, as well as non-data checked status.
            Label_PKRS.Visible = CB_PKRSStrain.Visible = CHK_Infected.Checked = pk5.PKRS_Strain != 0;
            Label_PKRSdays.Visible = CB_PKRSDays.Visible = pk5.PKRS_Days != 0;

            // Set SelectedIndexes for PKRS
            CB_PKRSStrain.SelectedIndex = pk5.PKRS_Strain;
            CHK_Cured.Checked = pk5.PKRS_Strain > 0 && pk5.PKRS_Days == 0;
            CB_PKRSDays.SelectedIndex = Math.Min(CB_PKRSDays.Items.Count - 1, pk5.PKRS_Days); // to strip out bad hacked 'rus

            Contest.Cool = pk5.CNT_Cool;
            Contest.Beauty = pk5.CNT_Beauty;
            Contest.Cute = pk5.CNT_Cute;
            Contest.Smart = pk5.CNT_Smart;
            Contest.Tough = pk5.CNT_Tough;
            Contest.Sheen = pk5.CNT_Sheen;

            TB_HPIV.Text = pk5.IV_HP.ToString();
            TB_ATKIV.Text = pk5.IV_ATK.ToString();
            TB_DEFIV.Text = pk5.IV_DEF.ToString();
            TB_SPEIV.Text = pk5.IV_SPE.ToString();
            TB_SPAIV.Text = pk5.IV_SPA.ToString();
            TB_SPDIV.Text = pk5.IV_SPD.ToString();
            CB_HPType.SelectedValue = pk5.HPType;

            TB_HPEV.Text = pk5.EV_HP.ToString();
            TB_ATKEV.Text = pk5.EV_ATK.ToString();
            TB_DEFEV.Text = pk5.EV_DEF.ToString();
            TB_SPEEV.Text = pk5.EV_SPE.ToString();
            TB_SPAEV.Text = pk5.EV_SPA.ToString();
            TB_SPDEV.Text = pk5.EV_SPD.ToString();

            CB_Move1.SelectedValue = pk5.Move1;
            CB_Move2.SelectedValue = pk5.Move2;
            CB_Move3.SelectedValue = pk5.Move3;
            CB_Move4.SelectedValue = pk5.Move4;
            CB_PPu1.SelectedIndex = pk5.Move1_PPUps;
            CB_PPu2.SelectedIndex = pk5.Move2_PPUps;
            CB_PPu3.SelectedIndex = pk5.Move3_PPUps;
            CB_PPu4.SelectedIndex = pk5.Move4_PPUps;
            TB_PP1.Text = pk5.Move1_PP.ToString();
            TB_PP2.Text = pk5.Move2_PP.ToString();
            TB_PP3.Text = pk5.Move3_PP.ToString();
            TB_PP4.Text = pk5.Move4_PP.ToString();

            // Set Form if count is enough, else cap.
            CB_Form.SelectedIndex = CB_Form.Items.Count > pk5.AltForm ? pk5.AltForm : CB_Form.Items.Count - 1;

            // Load Extrabyte Value
            TB_ExtraByte.Text = pk5.Data[Convert.ToInt32(CB_ExtraBytes.Text, 16)].ToString();

            UpdateStats();

            TB_EXP.Text = pk5.EXP.ToString();
            Label_Gender.Text = gendersymbols[pk5.Gender];
            Label_Gender.ForeColor = GetGenderColor(pk5.Gender);
            CHK_NSparkle.Checked = pk5.NPokémon;

            if (HaX)
                DEV_Ability.SelectedValue = pk5.Ability;
            else if (pk5.HiddenAbility)
                CB_Ability.SelectedIndex = CB_Ability.Items.Count - 1;
            else
            {
                int[] abils = pkm.PersonalInfo.Abilities;
                int abil = Array.IndexOf(abils, pk5.Ability);

                if (abil < 0)
                    CB_Ability.SelectedIndex = 0;
                else if (abil == 2)
                    CB_Ability.SelectedIndex = 2;
                else if (abils[0] == abils[1] || abils[1] == 0)
                    CB_Ability.SelectedIndex = pk5.PIDAbility;
                else
                    CB_Ability.SelectedIndex = abil >= CB_Ability.Items.Count ? 0 : abil;
            }
        }
        private PKM PreparePK5()
        {
            PK5 pk5 = pkm as PK5;
            if (pk5 == null)
                return null;

            pk5.Species = WinFormsUtil.GetIndex(CB_Species);
            pk5.HeldItem = WinFormsUtil.GetIndex(CB_HeldItem);
            pk5.TID = Util.ToInt32(TB_TID.Text);
            pk5.SID = Util.ToInt32(TB_SID.Text);
            pk5.EXP = Util.ToUInt32(TB_EXP.Text);
            pk5.PID = Util.GetHexValue(TB_PID.Text);

            pk5.Nature = (byte)WinFormsUtil.GetIndex(CB_Nature);
            pk5.FatefulEncounter = CHK_Fateful.Checked;
            pk5.Gender = PKX.GetGenderFromPID(Label_Gender.Text);
            pk5.AltForm = (MT_Form.Enabled ? Convert.ToInt32(MT_Form.Text) : CB_Form.Enabled ? CB_Form.SelectedIndex : 0) & 0x1F;
            pk5.EV_HP = Util.ToInt32(TB_HPEV.Text);
            pk5.EV_ATK = Util.ToInt32(TB_ATKEV.Text);
            pk5.EV_DEF = Util.ToInt32(TB_DEFEV.Text);
            pk5.EV_SPE = Util.ToInt32(TB_SPEEV.Text);
            pk5.EV_SPA = Util.ToInt32(TB_SPAEV.Text);
            pk5.EV_SPD = Util.ToInt32(TB_SPDEV.Text);

            pk5.CNT_Cool = Contest.Cool;
            pk5.CNT_Beauty = Contest.Beauty;
            pk5.CNT_Cute = Contest.Cute;
            pk5.CNT_Smart = Contest.Smart;
            pk5.CNT_Tough = Contest.Tough;
            pk5.CNT_Sheen = Contest.Sheen;

            pk5.PKRS_Days = CB_PKRSDays.SelectedIndex;
            pk5.PKRS_Strain = CB_PKRSStrain.SelectedIndex;
            pk5.Nickname = TB_Nickname.Text;
            pk5.Move1 = WinFormsUtil.GetIndex(CB_Move1);
            pk5.Move2 = WinFormsUtil.GetIndex(CB_Move2);
            pk5.Move3 = WinFormsUtil.GetIndex(CB_Move3);
            pk5.Move4 = WinFormsUtil.GetIndex(CB_Move4);
            pk5.Move1_PP = WinFormsUtil.GetIndex(CB_Move1) > 0 ? Util.ToInt32(TB_PP1.Text) : 0;
            pk5.Move2_PP = WinFormsUtil.GetIndex(CB_Move2) > 0 ? Util.ToInt32(TB_PP2.Text) : 0;
            pk5.Move3_PP = WinFormsUtil.GetIndex(CB_Move3) > 0 ? Util.ToInt32(TB_PP3.Text) : 0;
            pk5.Move4_PP = WinFormsUtil.GetIndex(CB_Move4) > 0 ? Util.ToInt32(TB_PP4.Text) : 0;
            pk5.Move1_PPUps = WinFormsUtil.GetIndex(CB_Move1) > 0 ? CB_PPu1.SelectedIndex : 0;
            pk5.Move2_PPUps = WinFormsUtil.GetIndex(CB_Move2) > 0 ? CB_PPu2.SelectedIndex : 0;
            pk5.Move3_PPUps = WinFormsUtil.GetIndex(CB_Move3) > 0 ? CB_PPu3.SelectedIndex : 0;
            pk5.Move4_PPUps = WinFormsUtil.GetIndex(CB_Move4) > 0 ? CB_PPu4.SelectedIndex : 0;

            pk5.IV_HP = Util.ToInt32(TB_HPIV.Text);
            pk5.IV_ATK = Util.ToInt32(TB_ATKIV.Text);
            pk5.IV_DEF = Util.ToInt32(TB_DEFIV.Text);
            pk5.IV_SPE = Util.ToInt32(TB_SPEIV.Text);
            pk5.IV_SPA = Util.ToInt32(TB_SPAIV.Text);
            pk5.IV_SPD = Util.ToInt32(TB_SPDIV.Text);
            pk5.IsEgg = CHK_IsEgg.Checked;
            pk5.IsNicknamed = CHK_Nicknamed.Checked;

            pk5.OT_Name = TB_OT.Text;
            pk5.CurrentFriendship = Util.ToInt32(TB_Friendship.Text);

            // Default Dates
            DateTime? egg_date = null;
            int egg_location = 0;
            if (CHK_AsEgg.Checked) // If encountered as an egg, load the Egg Met data from fields.
            {
                egg_date = CAL_EggDate.Value;
                egg_location = WinFormsUtil.GetIndex(CB_EggLocation);
            }
            // Egg Met Data
            pk5.EggMetDate = egg_date;
            pk5.Egg_Location = egg_location;
            // Met Data
            pk5.MetDate = CAL_MetDate.Value;
            pk5.Met_Location = WinFormsUtil.GetIndex(CB_MetLocation);

            if (pk5.IsEgg && pk5.Met_Location == 0)    // If still an egg, it has no hatch location/date. Zero it!
                pk5.MetDate = null;

            pk5.Ball = WinFormsUtil.GetIndex(CB_Ball);
            pk5.Met_Level = Util.ToInt32(TB_MetLevel.Text);
            pk5.OT_Gender = PKX.GetGenderFromPID(Label_OTGender.Text);
            pk5.EncounterType = WinFormsUtil.GetIndex(CB_EncounterType);
            pk5.Version = WinFormsUtil.GetIndex(CB_GameOrigin);
            pk5.Language = WinFormsUtil.GetIndex(CB_Language);

            pk5.NPokémon = CHK_NSparkle.Checked;

            // Toss in Party Stats
            Array.Resize(ref pk5.Data, pk5.SIZE_PARTY);
            pk5.Stat_Level = Util.ToInt32(TB_Level.Text);
            pk5.Stat_HPCurrent = Util.ToInt32(Stat_HP.Text);
            pk5.Stat_HPMax = Util.ToInt32(Stat_HP.Text);
            pk5.Stat_ATK = Util.ToInt32(Stat_ATK.Text);
            pk5.Stat_DEF = Util.ToInt32(Stat_DEF.Text);
            pk5.Stat_SPE = Util.ToInt32(Stat_SPE.Text);
            pk5.Stat_SPA = Util.ToInt32(Stat_SPA.Text);
            pk5.Stat_SPD = Util.ToInt32(Stat_SPD.Text);

            if (HaX)
            {
                pk5.Ability = (byte)WinFormsUtil.GetIndex(DEV_Ability);
                pk5.Stat_Level = (byte)Math.Min(Convert.ToInt32(MT_Level.Text), byte.MaxValue);
            }
            else
            {
                if (CB_Ability.Text.Length >= 4)
                {
                    pk5.Ability = (byte)Array.IndexOf(GameInfo.Strings.abilitylist, CB_Ability.Text.Remove(CB_Ability.Text.Length - 4));
                    pk5.HiddenAbility = CB_Ability.SelectedIndex > 1; // not 0 or 1
                }
            }

            // Fix Moves if a slot is empty 
            pk5.FixMoves();

            pk5.RefreshChecksum();
            return pk5;
        }
    }
}
