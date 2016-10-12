using System;
using System.Drawing;

namespace PKHeX
{
    public partial class Main
    {
        private void populateFieldsPK3()
        {
            PK3 pk3 = pkm as PK3;
            if (pk3 == null)
                return;

            // Do first
            pk3.Stat_Level = PKX.getLevel(pk3.Species, pk3.EXP);
            if (pk3.Stat_Level == 100)
                pk3.EXP = PKX.getEXP(pk3.Stat_Level, pk3.Species);

            CB_Species.SelectedValue = pk3.Species;
            TB_Level.Text = pk3.Stat_Level.ToString();
            TB_EXP.Text = pk3.EXP.ToString();

            // Load rest
            CHK_Fateful.Checked = pk3.FatefulEncounter;
            CHK_IsEgg.Checked = pk3.IsEgg;
            CHK_Nicknamed.Checked = pk3.IsNicknamed;
            Label_OTGender.Text = gendersymbols[pk3.OT_Gender];
            Label_OTGender.ForeColor = pk3.OT_Gender == 1 ? Color.Red : Color.Blue;
            TB_PID.Text = pk3.PID.ToString("X8");
            CB_HeldItem.SelectedValue = pk3.HeldItem;
            CB_Ability.SelectedIndex = pk3.AbilityNumber > CB_Ability.Items.Count ? 0 : pk3.AbilityNumber;
            CB_Nature.SelectedValue = pk3.Nature;
            TB_TID.Text = pk3.TID.ToString("00000");
            TB_SID.Text = pk3.SID.ToString("00000");
            TB_Nickname.Text = pk3.Nickname;
            TB_OT.Text = pk3.OT_Name;
            TB_Friendship.Text = pk3.CurrentFriendship.ToString();
            GB_OT.BackgroundImage = null;
            CB_Language.SelectedValue = pk3.Language;
            CB_GameOrigin.SelectedValue = pk3.Version;
            CB_EncounterType.SelectedValue = pk3.Gen4 ? pk3.EncounterType : 0;
            CB_Ball.SelectedValue = pk3.Ball;

            CB_MetLocation.SelectedValue = pk3.Met_Location;

            TB_MetLevel.Text = pk3.Met_Level.ToString();

            // Reset Label and ComboBox visibility, as well as non-data checked status.
            Label_PKRS.Visible = CB_PKRSStrain.Visible = CHK_Infected.Checked = pk3.PKRS_Strain != 0;
            Label_PKRSdays.Visible = CB_PKRSDays.Visible = pk3.PKRS_Days != 0;

            // Set SelectedIndexes for PKRS
            CB_PKRSStrain.SelectedIndex = pk3.PKRS_Strain;
            CHK_Cured.Checked = pk3.PKRS_Strain > 0 && pk3.PKRS_Days == 0;
            CB_PKRSDays.SelectedIndex = Math.Min(CB_PKRSDays.Items.Count - 1, pk3.PKRS_Days); // to strip out bad hacked 'rus

            TB_Cool.Text = pk3.CNT_Cool.ToString();
            TB_Beauty.Text = pk3.CNT_Beauty.ToString();
            TB_Cute.Text = pk3.CNT_Cute.ToString();
            TB_Smart.Text = pk3.CNT_Smart.ToString();
            TB_Tough.Text = pk3.CNT_Tough.ToString();
            TB_Sheen.Text = pk3.CNT_Sheen.ToString();

            TB_HPIV.Text = pk3.IV_HP.ToString();
            TB_ATKIV.Text = pk3.IV_ATK.ToString();
            TB_DEFIV.Text = pk3.IV_DEF.ToString();
            TB_SPEIV.Text = pk3.IV_SPE.ToString();
            TB_SPAIV.Text = pk3.IV_SPA.ToString();
            TB_SPDIV.Text = pk3.IV_SPD.ToString();
            CB_HPType.SelectedValue = pk3.HPType;

            TB_HPEV.Text = pk3.EV_HP.ToString();
            TB_ATKEV.Text = pk3.EV_ATK.ToString();
            TB_DEFEV.Text = pk3.EV_DEF.ToString();
            TB_SPEEV.Text = pk3.EV_SPE.ToString();
            TB_SPAEV.Text = pk3.EV_SPA.ToString();
            TB_SPDEV.Text = pk3.EV_SPD.ToString();

            CB_Move1.SelectedValue = pk3.Move1;
            CB_Move2.SelectedValue = pk3.Move2;
            CB_Move3.SelectedValue = pk3.Move3;
            CB_Move4.SelectedValue = pk3.Move4;
            CB_PPu1.SelectedIndex = pk3.Move1_PPUps;
            CB_PPu2.SelectedIndex = pk3.Move2_PPUps;
            CB_PPu3.SelectedIndex = pk3.Move3_PPUps;
            CB_PPu4.SelectedIndex = pk3.Move4_PPUps;
            TB_PP1.Text = pk3.Move1_PP.ToString();
            TB_PP2.Text = pk3.Move2_PP.ToString();
            TB_PP3.Text = pk3.Move3_PP.ToString();
            TB_PP4.Text = pk3.Move4_PP.ToString();

            // Set Form if count is enough, else cap.
            CB_Form.SelectedIndex = CB_Form.Items.Count > pk3.AltForm ? pk3.AltForm : CB_Form.Items.Count - 1;

            // Load Extrabyte Value
            TB_ExtraByte.Text = pk3.Data[Convert.ToInt32(CB_ExtraBytes.Text, 16)].ToString();

            updateStats();

            TB_EXP.Text = pk3.EXP.ToString();
            Label_Gender.Text = gendersymbols[pk3.Gender];
            Label_Gender.ForeColor = pk3.Gender == 2 ? Label_Species.ForeColor : (pk3.Gender == 1 ? Color.Red : Color.Blue);
        }
        private PKM preparePK3()
        {
            PK3 pk3 = pkm as PK3;
            if (pk3 == null)
                return null;

            pk3.Species = Util.getIndex(CB_Species);
            pk3.HeldItem = Util.getIndex(CB_HeldItem);
            pk3.TID = Util.ToInt32(TB_TID.Text);
            pk3.SID = Util.ToInt32(TB_SID.Text);
            pk3.EXP = Util.ToUInt32(TB_EXP.Text);
            pk3.PID = Util.getHEXval(TB_PID.Text);
            pk3.AbilityNumber = CB_Ability.SelectedIndex; // 0/1 (stored in IVbits)

            pk3.FatefulEncounter = CHK_Fateful.Checked;
            pk3.Gender = PKX.getGender(Label_Gender.Text);
            pk3.EV_HP = Util.ToInt32(TB_HPEV.Text);
            pk3.EV_ATK = Util.ToInt32(TB_ATKEV.Text);
            pk3.EV_DEF = Util.ToInt32(TB_DEFEV.Text);
            pk3.EV_SPE = Util.ToInt32(TB_SPEEV.Text);
            pk3.EV_SPA = Util.ToInt32(TB_SPAEV.Text);
            pk3.EV_SPD = Util.ToInt32(TB_SPDEV.Text);

            pk3.CNT_Cool = Util.ToInt32(TB_Cool.Text);
            pk3.CNT_Beauty = Util.ToInt32(TB_Beauty.Text);
            pk3.CNT_Cute = Util.ToInt32(TB_Cute.Text);
            pk3.CNT_Smart = Util.ToInt32(TB_Smart.Text);
            pk3.CNT_Tough = Util.ToInt32(TB_Tough.Text);
            pk3.CNT_Sheen = Util.ToInt32(TB_Sheen.Text);

            pk3.PKRS_Days = CB_PKRSDays.SelectedIndex;
            pk3.PKRS_Strain = CB_PKRSStrain.SelectedIndex;
            pk3.Nickname = TB_Nickname.Text;
            pk3.Move1 = Util.getIndex(CB_Move1);
            pk3.Move2 = Util.getIndex(CB_Move2);
            pk3.Move3 = Util.getIndex(CB_Move3);
            pk3.Move4 = Util.getIndex(CB_Move4);
            pk3.Move1_PP = Util.getIndex(CB_Move1) > 0 ? Util.ToInt32(TB_PP1.Text) : 0;
            pk3.Move2_PP = Util.getIndex(CB_Move2) > 0 ? Util.ToInt32(TB_PP2.Text) : 0;
            pk3.Move3_PP = Util.getIndex(CB_Move3) > 0 ? Util.ToInt32(TB_PP3.Text) : 0;
            pk3.Move4_PP = Util.getIndex(CB_Move4) > 0 ? Util.ToInt32(TB_PP4.Text) : 0;
            pk3.Move1_PPUps = Util.getIndex(CB_Move1) > 0 ? CB_PPu1.SelectedIndex : 0;
            pk3.Move2_PPUps = Util.getIndex(CB_Move2) > 0 ? CB_PPu2.SelectedIndex : 0;
            pk3.Move3_PPUps = Util.getIndex(CB_Move3) > 0 ? CB_PPu3.SelectedIndex : 0;
            pk3.Move4_PPUps = Util.getIndex(CB_Move4) > 0 ? CB_PPu4.SelectedIndex : 0;

            pk3.IV_HP = Util.ToInt32(TB_HPIV.Text);
            pk3.IV_ATK = Util.ToInt32(TB_ATKIV.Text);
            pk3.IV_DEF = Util.ToInt32(TB_DEFIV.Text);
            pk3.IV_SPE = Util.ToInt32(TB_SPEIV.Text);
            pk3.IV_SPA = Util.ToInt32(TB_SPAIV.Text);
            pk3.IV_SPD = Util.ToInt32(TB_SPDIV.Text);
            pk3.IsEgg = CHK_IsEgg.Checked;
            pk3.IsNicknamed = CHK_Nicknamed.Checked;

            pk3.OT_Name = TB_OT.Text;
            pk3.CurrentFriendship = Util.ToInt32(TB_Friendship.Text);

            pk3.Ball = Util.getIndex(CB_Ball);
            pk3.Met_Level = Util.ToInt32(TB_MetLevel.Text);
            pk3.OT_Gender = PKX.getGender(Label_OTGender.Text);
            pk3.Version = Util.getIndex(CB_GameOrigin);
            pk3.Language = Util.getIndex(CB_Language);

            pk3.Met_Location = Util.getIndex(CB_MetLocation);

            // Toss in Party Stats
            Array.Resize(ref pk3.Data, pk3.SIZE_PARTY);
            pk3.Stat_Level = Util.ToInt32(TB_Level.Text);
            pk3.Stat_HPCurrent = Util.ToInt32(Stat_HP.Text);
            pk3.Stat_HPMax = Util.ToInt32(Stat_HP.Text);
            pk3.Stat_ATK = Util.ToInt32(Stat_ATK.Text);
            pk3.Stat_DEF = Util.ToInt32(Stat_DEF.Text);
            pk3.Stat_SPE = Util.ToInt32(Stat_SPE.Text);
            pk3.Stat_SPA = Util.ToInt32(Stat_SPA.Text);
            pk3.Stat_SPD = Util.ToInt32(Stat_SPD.Text);

            if (HaX)
            {
                pk3.Stat_Level = (byte)Math.Min(Convert.ToInt32(MT_Level.Text), byte.MaxValue);
            }

            // Fix Moves if a slot is empty 
            pk3.FixMoves();

            pk3.RefreshChecksum();
            return pk3;
        }
    }
}
