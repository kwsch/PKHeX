using System;
using System.Drawing;

namespace PKHeX
{
    public partial class Main
    {
        // Main Series
        private void populateFieldsPK4()
        {
            PK4 pk4 = pkm as PK4;
            if (pk4 == null)
                return;

            // Do first
            pk4.Stat_Level = PKX.getLevel(pk4.Species, pk4.EXP);
            if (pk4.Stat_Level == 100)
                pk4.EXP = PKX.getEXP(pk4.Stat_Level, pk4.Species);

            CB_Species.SelectedValue = pk4.Species;
            TB_Level.Text = pk4.Stat_Level.ToString();
            TB_EXP.Text = pk4.EXP.ToString();

            // Load rest
            CHK_Fateful.Checked = pk4.FatefulEncounter;
            CHK_IsEgg.Checked = pk4.IsEgg;
            CHK_Nicknamed.Checked = pk4.IsNicknamed;
            Label_OTGender.Text = gendersymbols[pk4.OT_Gender];
            Label_OTGender.ForeColor = pk4.OT_Gender == 1 ? Color.Red : Color.Blue;
            TB_PID.Text = pk4.PID.ToString("X8");
            CB_HeldItem.SelectedValue = pk4.HeldItem;
            CB_Nature.SelectedValue = pk4.Nature;
            TB_TID.Text = pk4.TID.ToString("00000");
            TB_SID.Text = pk4.SID.ToString("00000");
            TB_Nickname.Text = pk4.Nickname;
            TB_OT.Text = pk4.OT_Name;
            TB_Friendship.Text = pk4.CurrentFriendship.ToString();
            GB_OT.BackgroundImage = null;
            CB_Language.SelectedValue = pk4.Language;
            CB_GameOrigin.SelectedValue = pk4.Version;
            CB_EncounterType.SelectedValue = pk4.Gen4 ? pk4.EncounterType : 0;
            CB_Ball.SelectedValue = pk4.Ball;

            if (pk4.MetDate.HasValue)
            {
                CAL_MetDate.Value = pk4.MetDate.Value;
            }
            else
            {
                CAL_MetDate.Value = new DateTime(2000, 1, 1);
            }

            if (pk4.Egg_Location != 0)
            {
                // Was obtained initially as an egg.
                CHK_AsEgg.Checked = true;
                GB_EggConditions.Enabled = true;

                CB_EggLocation.SelectedValue = pk4.Egg_Location;
                if (pk4.EggMetDate.HasValue)
                {
                    CAL_EggDate.Value = pk4.EggMetDate.Value;
                }
                else
                {
                    CAL_EggDate.Value = new DateTime(2000, 1, 1);
                }
            }
            else { CAL_EggDate.Value = new DateTime(2000, 01, 01); CHK_AsEgg.Checked = GB_EggConditions.Enabled = false; CB_EggLocation.SelectedValue = 0; }

            CB_MetLocation.SelectedValue = pk4.Met_Location;

            TB_MetLevel.Text = pk4.Met_Level.ToString();

            // Reset Label and ComboBox visibility, as well as non-data checked status.
            Label_PKRS.Visible = CB_PKRSStrain.Visible = CHK_Infected.Checked = pk4.PKRS_Strain != 0;
            Label_PKRSdays.Visible = CB_PKRSDays.Visible = pk4.PKRS_Days != 0;

            // Set SelectedIndexes for PKRS
            CB_PKRSStrain.SelectedIndex = pk4.PKRS_Strain;
            CHK_Cured.Checked = pk4.PKRS_Strain > 0 && pk4.PKRS_Days == 0;
            CB_PKRSDays.SelectedIndex = Math.Min(CB_PKRSDays.Items.Count - 1, pk4.PKRS_Days); // to strip out bad hacked 'rus

            TB_Cool.Text = pk4.CNT_Cool.ToString();
            TB_Beauty.Text = pk4.CNT_Beauty.ToString();
            TB_Cute.Text = pk4.CNT_Cute.ToString();
            TB_Smart.Text = pk4.CNT_Smart.ToString();
            TB_Tough.Text = pk4.CNT_Tough.ToString();
            TB_Sheen.Text = pk4.CNT_Sheen.ToString();

            TB_HPIV.Text = pk4.IV_HP.ToString();
            TB_ATKIV.Text = pk4.IV_ATK.ToString();
            TB_DEFIV.Text = pk4.IV_DEF.ToString();
            TB_SPEIV.Text = pk4.IV_SPE.ToString();
            TB_SPAIV.Text = pk4.IV_SPA.ToString();
            TB_SPDIV.Text = pk4.IV_SPD.ToString();
            CB_HPType.SelectedValue = pk4.HPType;

            TB_HPEV.Text = pk4.EV_HP.ToString();
            TB_ATKEV.Text = pk4.EV_ATK.ToString();
            TB_DEFEV.Text = pk4.EV_DEF.ToString();
            TB_SPEEV.Text = pk4.EV_SPE.ToString();
            TB_SPAEV.Text = pk4.EV_SPA.ToString();
            TB_SPDEV.Text = pk4.EV_SPD.ToString();

            CB_Move1.SelectedValue = pk4.Move1;
            CB_Move2.SelectedValue = pk4.Move2;
            CB_Move3.SelectedValue = pk4.Move3;
            CB_Move4.SelectedValue = pk4.Move4;
            CB_PPu1.SelectedIndex = pk4.Move1_PPUps;
            CB_PPu2.SelectedIndex = pk4.Move2_PPUps;
            CB_PPu3.SelectedIndex = pk4.Move3_PPUps;
            CB_PPu4.SelectedIndex = pk4.Move4_PPUps;
            TB_PP1.Text = pk4.Move1_PP.ToString();
            TB_PP2.Text = pk4.Move2_PP.ToString();
            TB_PP3.Text = pk4.Move3_PP.ToString();
            TB_PP4.Text = pk4.Move4_PP.ToString();

            // Set Form if count is enough, else cap.
            CB_Form.SelectedIndex = CB_Form.Items.Count > pk4.AltForm ? pk4.AltForm : CB_Form.Items.Count - 1;

            // Load Extrabyte Value
            TB_ExtraByte.Text = pk4.Data[Convert.ToInt32(CB_ExtraBytes.Text, 16)].ToString();

            updateStats();

            TB_EXP.Text = pk4.EXP.ToString();
            Label_Gender.Text = gendersymbols[pk4.Gender];
            Label_Gender.ForeColor = pk4.Gender == 2 ? Label_Species.ForeColor : (pk4.Gender == 1 ? Color.Red : Color.Blue);

            if (HaX)
                DEV_Ability.SelectedValue = pk4.Ability;
            else
            {
                int[] abils = SAV.Personal.getAbilities(pk4.Species, pk4.AltForm);
                int abil = Array.IndexOf(abils, pk4.Ability);

                if (abil < 0)
                    CB_Ability.SelectedIndex = 0;
                else if (abils[0] == abils[1] || abils[1] == 0)
                    CB_Ability.SelectedIndex = pk4.PIDAbility;
                else
                    CB_Ability.SelectedIndex = abil < 0 || abil >= CB_Ability.Items.Count ? 0 : abil;
            }
        }
        private PKM preparePK4()
        {
            PK4 pk4 = pkm as PK4;
            if (pk4 == null)
                return null;

            pk4.Species = Util.getIndex(CB_Species);
            pk4.HeldItem = Util.getIndex(CB_HeldItem);
            pk4.TID = Util.ToInt32(TB_TID.Text);
            pk4.SID = Util.ToInt32(TB_SID.Text);
            pk4.EXP = Util.ToUInt32(TB_EXP.Text);
            pk4.PID = Util.getHEXval(TB_PID.Text);
            pk4.Ability = (byte)Array.IndexOf(GameStrings.abilitylist, CB_Ability.Text.Remove(CB_Ability.Text.Length - 4));

            pk4.FatefulEncounter = CHK_Fateful.Checked;
            pk4.Gender = PKX.getGender(Label_Gender.Text);
            pk4.AltForm = (MT_Form.Enabled ? Convert.ToInt32(MT_Form.Text) : CB_Form.Enabled ? CB_Form.SelectedIndex : 0) & 0x1F;
            pk4.EV_HP = Util.ToInt32(TB_HPEV.Text);
            pk4.EV_ATK = Util.ToInt32(TB_ATKEV.Text);
            pk4.EV_DEF = Util.ToInt32(TB_DEFEV.Text);
            pk4.EV_SPE = Util.ToInt32(TB_SPEEV.Text);
            pk4.EV_SPA = Util.ToInt32(TB_SPAEV.Text);
            pk4.EV_SPD = Util.ToInt32(TB_SPDEV.Text);

            pk4.CNT_Cool = Util.ToInt32(TB_Cool.Text);
            pk4.CNT_Beauty = Util.ToInt32(TB_Beauty.Text);
            pk4.CNT_Cute = Util.ToInt32(TB_Cute.Text);
            pk4.CNT_Smart = Util.ToInt32(TB_Smart.Text);
            pk4.CNT_Tough = Util.ToInt32(TB_Tough.Text);
            pk4.CNT_Sheen = Util.ToInt32(TB_Sheen.Text);

            pk4.PKRS_Days = CB_PKRSDays.SelectedIndex;
            pk4.PKRS_Strain = CB_PKRSStrain.SelectedIndex;
            pk4.Nickname = TB_Nickname.Text;
            pk4.Move1 = Util.getIndex(CB_Move1);
            pk4.Move2 = Util.getIndex(CB_Move2);
            pk4.Move3 = Util.getIndex(CB_Move3);
            pk4.Move4 = Util.getIndex(CB_Move4);
            pk4.Move1_PP = Util.getIndex(CB_Move1) > 0 ? Util.ToInt32(TB_PP1.Text) : 0;
            pk4.Move2_PP = Util.getIndex(CB_Move2) > 0 ? Util.ToInt32(TB_PP2.Text) : 0;
            pk4.Move3_PP = Util.getIndex(CB_Move3) > 0 ? Util.ToInt32(TB_PP3.Text) : 0;
            pk4.Move4_PP = Util.getIndex(CB_Move4) > 0 ? Util.ToInt32(TB_PP4.Text) : 0;
            pk4.Move1_PPUps = Util.getIndex(CB_Move1) > 0 ? CB_PPu1.SelectedIndex : 0;
            pk4.Move2_PPUps = Util.getIndex(CB_Move2) > 0 ? CB_PPu2.SelectedIndex : 0;
            pk4.Move3_PPUps = Util.getIndex(CB_Move3) > 0 ? CB_PPu3.SelectedIndex : 0;
            pk4.Move4_PPUps = Util.getIndex(CB_Move4) > 0 ? CB_PPu4.SelectedIndex : 0;

            pk4.IV_HP = Util.ToInt32(TB_HPIV.Text);
            pk4.IV_ATK = Util.ToInt32(TB_ATKIV.Text);
            pk4.IV_DEF = Util.ToInt32(TB_DEFIV.Text);
            pk4.IV_SPE = Util.ToInt32(TB_SPEIV.Text);
            pk4.IV_SPA = Util.ToInt32(TB_SPAIV.Text);
            pk4.IV_SPD = Util.ToInt32(TB_SPDIV.Text);
            pk4.IsEgg = CHK_IsEgg.Checked;
            pk4.IsNicknamed = CHK_Nicknamed.Checked;

            pk4.OT_Name = TB_OT.Text;
            pk4.CurrentFriendship = Util.ToInt32(TB_Friendship.Text);

            pk4.Ball = Util.getIndex(CB_Ball);
            pk4.Met_Level = Util.ToInt32(TB_MetLevel.Text);
            pk4.OT_Gender = PKX.getGender(Label_OTGender.Text);
            pk4.EncounterType = Util.getIndex(CB_EncounterType);
            pk4.Version = Util.getIndex(CB_GameOrigin);
            pk4.Language = Util.getIndex(CB_Language);

            // Default Dates
            DateTime? egg_date = null;
            int egg_location = 0;
            if (CHK_AsEgg.Checked) // If encountered as an egg, load the Egg Met data from fields.
            {
                egg_date = CAL_EggDate.Value;
                egg_location = Util.getIndex(CB_EggLocation);
            }
            // Egg Met Data
            pk4.EggMetDate = egg_date;
            pk4.Egg_Location = egg_location;
            // Met Data
            pk4.MetDate = CAL_MetDate.Value;
            pk4.Met_Location = Util.getIndex(CB_MetLocation);

            if (pk4.IsEgg && pk4.Met_Location == 0)    // If still an egg, it has no hatch location/date. Zero it!
                pk4.MetDate = null;

            // Toss in Party Stats
            Array.Resize(ref pk4.Data, pk4.SIZE_PARTY);
            pk4.Stat_Level = Util.ToInt32(TB_Level.Text);
            pk4.Stat_HPCurrent = Util.ToInt32(Stat_HP.Text);
            pk4.Stat_HPMax = Util.ToInt32(Stat_HP.Text);
            pk4.Stat_ATK = Util.ToInt32(Stat_ATK.Text);
            pk4.Stat_DEF = Util.ToInt32(Stat_DEF.Text);
            pk4.Stat_SPE = Util.ToInt32(Stat_SPE.Text);
            pk4.Stat_SPA = Util.ToInt32(Stat_SPA.Text);
            pk4.Stat_SPD = Util.ToInt32(Stat_SPD.Text);

            if (HaX)
            {
                pk4.Ability = (byte)Util.getIndex(DEV_Ability);
                pk4.Stat_Level = (byte)Math.Min(Convert.ToInt32(MT_Level.Text), byte.MaxValue);
            }

            // Fix Moves if a slot is empty 
            pk4.FixMoves();

            pk4.RefreshChecksum();
            return pk4;
        }

        // Battle Revolution
        private void populateFieldsBK4()
        {
            BK4 pk4 = pkm as BK4;
            if (pk4 == null)
                return;

            // Do first
            pk4.Stat_Level = PKX.getLevel(pk4.Species, pk4.EXP);
            if (pk4.Stat_Level == 100)
                pk4.EXP = PKX.getEXP(pk4.Stat_Level, pk4.Species);

            CB_Species.SelectedValue = pk4.Species;
            TB_Level.Text = pk4.Stat_Level.ToString();
            TB_EXP.Text = pk4.EXP.ToString();

            // Load rest
            CHK_Fateful.Checked = pk4.FatefulEncounter;
            CHK_IsEgg.Checked = pk4.IsEgg;
            CHK_Nicknamed.Checked = pk4.IsNicknamed;
            Label_OTGender.Text = gendersymbols[pk4.OT_Gender];
            Label_OTGender.ForeColor = pk4.OT_Gender == 1 ? Color.Red : Color.Blue;
            TB_PID.Text = pk4.PID.ToString("X8");
            CB_HeldItem.SelectedValue = pk4.HeldItem;
            setAbilityList();
            CB_Nature.SelectedValue = pk4.Nature;
            TB_TID.Text = pk4.TID.ToString("00000");
            TB_SID.Text = pk4.SID.ToString("00000");
            TB_Nickname.Text = pk4.Nickname;
            TB_OT.Text = pk4.OT_Name;
            TB_Friendship.Text = pk4.CurrentFriendship.ToString();
            GB_OT.BackgroundImage = null;
            CB_Language.SelectedValue = pk4.Language;
            CB_GameOrigin.SelectedValue = pk4.Version;
            CB_EncounterType.SelectedValue = pk4.Gen4 ? pk4.EncounterType : 0;
            CB_Ball.SelectedValue = pk4.Ball;

            if (pk4.MetDate.HasValue)
            {
                CAL_MetDate.Value = pk4.MetDate.Value;
            }
            else
            {
                CAL_MetDate.Value = new DateTime(2000, 1, 1);
            }

            if (pk4.Egg_Location != 0)
            {
                // Was obtained initially as an egg.
                CHK_AsEgg.Checked = true;
                GB_EggConditions.Enabled = true;

                CB_EggLocation.SelectedValue = pk4.Egg_Location;
                if (pk4.EggMetDate.HasValue)
                {
                    CAL_EggDate.Value = pk4.EggMetDate.Value;
                }
                else
                {
                    CAL_EggDate.Value = new DateTime(2000, 1, 1);
                }
            }
            else { CAL_EggDate.Value = new DateTime(2000, 01, 01); CHK_AsEgg.Checked = GB_EggConditions.Enabled = false; CB_EggLocation.SelectedValue = 0; }

            CB_MetLocation.SelectedValue = pk4.Met_Location;

            TB_MetLevel.Text = pk4.Met_Level.ToString();

            // Reset Label and ComboBox visibility, as well as non-data checked status.
            Label_PKRS.Visible = CB_PKRSStrain.Visible = CHK_Infected.Checked = pk4.PKRS_Strain != 0;
            Label_PKRSdays.Visible = CB_PKRSDays.Visible = pk4.PKRS_Days != 0;

            // Set SelectedIndexes for PKRS
            CB_PKRSStrain.SelectedIndex = pk4.PKRS_Strain;
            CHK_Cured.Checked = pk4.PKRS_Strain > 0 && pk4.PKRS_Days == 0;
            CB_PKRSDays.SelectedIndex = Math.Min(CB_PKRSDays.Items.Count - 1, pk4.PKRS_Days); // to strip out bad hacked 'rus

            TB_Cool.Text = pk4.CNT_Cool.ToString();
            TB_Beauty.Text = pk4.CNT_Beauty.ToString();
            TB_Cute.Text = pk4.CNT_Cute.ToString();
            TB_Smart.Text = pk4.CNT_Smart.ToString();
            TB_Tough.Text = pk4.CNT_Tough.ToString();
            TB_Sheen.Text = pk4.CNT_Sheen.ToString();

            TB_HPIV.Text = pk4.IV_HP.ToString();
            TB_ATKIV.Text = pk4.IV_ATK.ToString();
            TB_DEFIV.Text = pk4.IV_DEF.ToString();
            TB_SPEIV.Text = pk4.IV_SPE.ToString();
            TB_SPAIV.Text = pk4.IV_SPA.ToString();
            TB_SPDIV.Text = pk4.IV_SPD.ToString();
            CB_HPType.SelectedValue = pk4.HPType;

            TB_HPEV.Text = pk4.EV_HP.ToString();
            TB_ATKEV.Text = pk4.EV_ATK.ToString();
            TB_DEFEV.Text = pk4.EV_DEF.ToString();
            TB_SPEEV.Text = pk4.EV_SPE.ToString();
            TB_SPAEV.Text = pk4.EV_SPA.ToString();
            TB_SPDEV.Text = pk4.EV_SPD.ToString();

            CB_Move1.SelectedValue = pk4.Move1;
            CB_Move2.SelectedValue = pk4.Move2;
            CB_Move3.SelectedValue = pk4.Move3;
            CB_Move4.SelectedValue = pk4.Move4;
            CB_PPu1.SelectedIndex = pk4.Move1_PPUps;
            CB_PPu2.SelectedIndex = pk4.Move2_PPUps;
            CB_PPu3.SelectedIndex = pk4.Move3_PPUps;
            CB_PPu4.SelectedIndex = pk4.Move4_PPUps;
            TB_PP1.Text = pk4.Move1_PP.ToString();
            TB_PP2.Text = pk4.Move2_PP.ToString();
            TB_PP3.Text = pk4.Move3_PP.ToString();
            TB_PP4.Text = pk4.Move4_PP.ToString();

            // Set Form if count is enough, else cap.
            CB_Form.SelectedIndex = CB_Form.Items.Count > pk4.AltForm ? pk4.AltForm : CB_Form.Items.Count - 1;

            // Load Extrabyte Value
            TB_ExtraByte.Text = pk4.Data[Convert.ToInt32(CB_ExtraBytes.Text, 16)].ToString();

            updateStats();

            TB_EXP.Text = pk4.EXP.ToString();
            Label_Gender.Text = gendersymbols[pk4.Gender];
            Label_Gender.ForeColor = pk4.Gender == 2 ? Label_Species.ForeColor : (pk4.Gender == 1 ? Color.Red : Color.Blue);

            if (HaX)
                DEV_Ability.SelectedValue = pk4.Ability;
            else
            {
                int[] abils = SAV.Personal.getAbilities(pk4.Species, pk4.AltForm);
                int abil = Array.IndexOf(abils, pk4.Ability);

                if (abil < 0)
                    CB_Ability.SelectedIndex = 0;
                else if (abils[0] == abils[1] || abils[1] == 0)
                    CB_Ability.SelectedIndex = pk4.PIDAbility;
                else
                    CB_Ability.SelectedIndex = abil < 0 || abil >= CB_Ability.Items.Count ? 0 : abil;
            }
        }
        private PKM prepareBK4()
        {
            BK4 pk4 = pkm as BK4;
            if (pk4 == null)
                return null;

            pk4.Species = Util.getIndex(CB_Species);
            pk4.HeldItem = Util.getIndex(CB_HeldItem);
            pk4.TID = Util.ToInt32(TB_TID.Text);
            pk4.SID = Util.ToInt32(TB_SID.Text);
            pk4.EXP = Util.ToUInt32(TB_EXP.Text);
            pk4.PID = Util.getHEXval(TB_PID.Text);
            pk4.Ability = (byte)Array.IndexOf(GameStrings.abilitylist, CB_Ability.Text.Remove(CB_Ability.Text.Length - 4));

            pk4.FatefulEncounter = CHK_Fateful.Checked;
            pk4.Gender = PKX.getGender(Label_Gender.Text);
            pk4.AltForm = (MT_Form.Enabled ? Convert.ToInt32(MT_Form.Text) : CB_Form.Enabled ? CB_Form.SelectedIndex : 0) & 0x1F;
            pk4.EV_HP = Util.ToInt32(TB_HPEV.Text);
            pk4.EV_ATK = Util.ToInt32(TB_ATKEV.Text);
            pk4.EV_DEF = Util.ToInt32(TB_DEFEV.Text);
            pk4.EV_SPE = Util.ToInt32(TB_SPEEV.Text);
            pk4.EV_SPA = Util.ToInt32(TB_SPAEV.Text);
            pk4.EV_SPD = Util.ToInt32(TB_SPDEV.Text);

            pk4.CNT_Cool = Util.ToInt32(TB_Cool.Text);
            pk4.CNT_Beauty = Util.ToInt32(TB_Beauty.Text);
            pk4.CNT_Cute = Util.ToInt32(TB_Cute.Text);
            pk4.CNT_Smart = Util.ToInt32(TB_Smart.Text);
            pk4.CNT_Tough = Util.ToInt32(TB_Tough.Text);
            pk4.CNT_Sheen = Util.ToInt32(TB_Sheen.Text);

            pk4.PKRS_Days = CB_PKRSDays.SelectedIndex;
            pk4.PKRS_Strain = CB_PKRSStrain.SelectedIndex;
            pk4.Nickname = TB_Nickname.Text;
            pk4.Move1 = Util.getIndex(CB_Move1);
            pk4.Move2 = Util.getIndex(CB_Move2);
            pk4.Move3 = Util.getIndex(CB_Move3);
            pk4.Move4 = Util.getIndex(CB_Move4);
            pk4.Move1_PP = Util.getIndex(CB_Move1) > 0 ? Util.ToInt32(TB_PP1.Text) : 0;
            pk4.Move2_PP = Util.getIndex(CB_Move2) > 0 ? Util.ToInt32(TB_PP2.Text) : 0;
            pk4.Move3_PP = Util.getIndex(CB_Move3) > 0 ? Util.ToInt32(TB_PP3.Text) : 0;
            pk4.Move4_PP = Util.getIndex(CB_Move4) > 0 ? Util.ToInt32(TB_PP4.Text) : 0;
            pk4.Move1_PPUps = Util.getIndex(CB_Move1) > 0 ? CB_PPu1.SelectedIndex : 0;
            pk4.Move2_PPUps = Util.getIndex(CB_Move2) > 0 ? CB_PPu2.SelectedIndex : 0;
            pk4.Move3_PPUps = Util.getIndex(CB_Move3) > 0 ? CB_PPu3.SelectedIndex : 0;
            pk4.Move4_PPUps = Util.getIndex(CB_Move4) > 0 ? CB_PPu4.SelectedIndex : 0;

            pk4.IV_HP = Util.ToInt32(TB_HPIV.Text);
            pk4.IV_ATK = Util.ToInt32(TB_ATKIV.Text);
            pk4.IV_DEF = Util.ToInt32(TB_DEFIV.Text);
            pk4.IV_SPE = Util.ToInt32(TB_SPEIV.Text);
            pk4.IV_SPA = Util.ToInt32(TB_SPAIV.Text);
            pk4.IV_SPD = Util.ToInt32(TB_SPDIV.Text);
            pk4.IsEgg = CHK_IsEgg.Checked;
            pk4.IsNicknamed = CHK_Nicknamed.Checked;

            pk4.OT_Name = TB_OT.Text;
            pk4.CurrentFriendship = Util.ToInt32(TB_Friendship.Text);

            pk4.Ball = Util.getIndex(CB_Ball);
            pk4.Met_Level = Util.ToInt32(TB_MetLevel.Text);
            pk4.OT_Gender = PKX.getGender(Label_OTGender.Text);
            pk4.EncounterType = Util.getIndex(CB_EncounterType);
            pk4.Version = Util.getIndex(CB_GameOrigin);
            pk4.Language = Util.getIndex(CB_Language);

            // Default Dates
            DateTime? egg_date = null;
            int egg_location = 0;
            if (CHK_AsEgg.Checked) // If encountered as an egg, load the Egg Met data from fields.
            {
                egg_date = CAL_EggDate.Value;
                egg_location = Util.getIndex(CB_EggLocation);
            }
            // Egg Met Data
            pk4.EggMetDate = egg_date;
            pk4.Egg_Location = egg_location;
            // Met Data
            pk4.MetDate = CAL_MetDate.Value;
            pk4.Met_Location = Util.getIndex(CB_MetLocation);

            if (pk4.IsEgg && pk4.Met_Location == 0)    // If still an egg, it has no hatch location/date. Zero it!
                pk4.MetDate = null;

            // Toss in Party Stats
            Array.Resize(ref pk4.Data, pk4.SIZE_PARTY);
            pk4.Stat_Level = Util.ToInt32(TB_Level.Text);
            pk4.Stat_HPCurrent = Util.ToInt32(Stat_HP.Text);
            pk4.Stat_HPMax = Util.ToInt32(Stat_HP.Text);
            pk4.Stat_ATK = Util.ToInt32(Stat_ATK.Text);
            pk4.Stat_DEF = Util.ToInt32(Stat_DEF.Text);
            pk4.Stat_SPE = Util.ToInt32(Stat_SPE.Text);
            pk4.Stat_SPA = Util.ToInt32(Stat_SPA.Text);
            pk4.Stat_SPD = Util.ToInt32(Stat_SPD.Text);

            if (HaX)
            {
                pk4.Ability = (byte)Util.getIndex(DEV_Ability);
                pk4.Stat_Level = (byte)Math.Min(Convert.ToInt32(MT_Level.Text), byte.MaxValue);
            }

            // Fix Moves if a slot is empty 
            pk4.FixMoves();

            pk4.RefreshChecksum();
            return pk4;
        }
    }
}
