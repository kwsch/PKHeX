using System;
using System.Drawing;

namespace PKHeX
{
    public partial class Main
    {
        private void populateFieldsXK3()
        {
            XK3 xk3 = pkm as XK3;
            if (xk3 == null)
                return;

            // Do first
            xk3.Stat_Level = PKX.getLevel(xk3.Species, xk3.EXP);
            if (xk3.Stat_Level == 100)
                xk3.EXP = PKX.getEXP(xk3.Stat_Level, xk3.Species);

            CB_Species.SelectedValue = xk3.Species;
            TB_Level.Text = xk3.Stat_Level.ToString();
            TB_EXP.Text = xk3.EXP.ToString();

            // Load rest
            CHK_Fateful.Checked = xk3.FatefulEncounter;
            CHK_IsEgg.Checked = xk3.IsEgg;
            CHK_Nicknamed.Checked = xk3.IsNicknamed;
            Label_OTGender.Text = gendersymbols[xk3.OT_Gender];
            Label_OTGender.ForeColor = xk3.OT_Gender == 1 ? Color.Red : Color.Blue;
            TB_PID.Text = xk3.PID.ToString("X8");
            CB_HeldItem.SelectedValue = xk3.HeldItem;
            CB_Ability.SelectedIndex = xk3.AbilityNumber > CB_Ability.Items.Count ? 0 : xk3.AbilityNumber;
            CB_Nature.SelectedValue = xk3.Nature;
            TB_TID.Text = xk3.TID.ToString("00000");
            TB_SID.Text = xk3.SID.ToString("00000");
            TB_Nickname.Text = xk3.Nickname;
            TB_OT.Text = xk3.OT_Name;
            TB_Friendship.Text = xk3.CurrentFriendship.ToString();
            GB_OT.BackgroundImage = null;
            CB_Language.SelectedValue = xk3.Language;
            CB_GameOrigin.SelectedValue = xk3.Version;
            CB_EncounterType.SelectedValue = xk3.Gen4 ? xk3.EncounterType : 0;
            CB_Ball.SelectedValue = xk3.Ball;

            CB_MetLocation.SelectedValue = xk3.Met_Location;

            TB_MetLevel.Text = xk3.Met_Level.ToString();

            // Reset Label and ComboBox visibility, as well as non-data checked status.
            Label_PKRS.Visible = CB_PKRSStrain.Visible = CHK_Infected.Checked = xk3.PKRS_Strain != 0;
            Label_PKRSdays.Visible = CB_PKRSDays.Visible = xk3.PKRS_Days != 0;

            // Set SelectedIndexes for PKRS
            CB_PKRSStrain.SelectedIndex = xk3.PKRS_Strain;
            CHK_Cured.Checked = xk3.PKRS_Strain > 0 && xk3.PKRS_Days == 0;
            CB_PKRSDays.SelectedIndex = Math.Min(CB_PKRSDays.Items.Count - 1, xk3.PKRS_Days); // to strip out bad hacked 'rus

            TB_Cool.Text = xk3.CNT_Cool.ToString();
            TB_Beauty.Text = xk3.CNT_Beauty.ToString();
            TB_Cute.Text = xk3.CNT_Cute.ToString();
            TB_Smart.Text = xk3.CNT_Smart.ToString();
            TB_Tough.Text = xk3.CNT_Tough.ToString();
            TB_Sheen.Text = xk3.CNT_Sheen.ToString();

            TB_HPIV.Text = xk3.IV_HP.ToString();
            TB_ATKIV.Text = xk3.IV_ATK.ToString();
            TB_DEFIV.Text = xk3.IV_DEF.ToString();
            TB_SPEIV.Text = xk3.IV_SPE.ToString();
            TB_SPAIV.Text = xk3.IV_SPA.ToString();
            TB_SPDIV.Text = xk3.IV_SPD.ToString();
            CB_HPType.SelectedValue = xk3.HPType;

            TB_HPEV.Text = xk3.EV_HP.ToString();
            TB_ATKEV.Text = xk3.EV_ATK.ToString();
            TB_DEFEV.Text = xk3.EV_DEF.ToString();
            TB_SPEEV.Text = xk3.EV_SPE.ToString();
            TB_SPAEV.Text = xk3.EV_SPA.ToString();
            TB_SPDEV.Text = xk3.EV_SPD.ToString();

            CB_Move1.SelectedValue = xk3.Move1;
            CB_Move2.SelectedValue = xk3.Move2;
            CB_Move3.SelectedValue = xk3.Move3;
            CB_Move4.SelectedValue = xk3.Move4;
            CB_PPu1.SelectedIndex = xk3.Move1_PPUps;
            CB_PPu2.SelectedIndex = xk3.Move2_PPUps;
            CB_PPu3.SelectedIndex = xk3.Move3_PPUps;
            CB_PPu4.SelectedIndex = xk3.Move4_PPUps;
            TB_PP1.Text = xk3.Move1_PP.ToString();
            TB_PP2.Text = xk3.Move2_PP.ToString();
            TB_PP3.Text = xk3.Move3_PP.ToString();
            TB_PP4.Text = xk3.Move4_PP.ToString();

            // Set Form if count is enough, else cap.
            CB_Form.SelectedIndex = CB_Form.Items.Count > xk3.AltForm ? xk3.AltForm : CB_Form.Items.Count - 1;

            // Load Extrabyte Value
            TB_ExtraByte.Text = xk3.Data[Convert.ToInt32(CB_ExtraBytes.Text, 16)].ToString();

            NUD_ShadowID.Value = xk3.ShadowID;
            FLP_Purification.Visible = xk3.ShadowID > 0;
            if (xk3.ShadowID > 0)
            {
                int puri = xk3.Purification;
                if (puri > NUD_Purification.Maximum)
                    puri = (int)NUD_Purification.Maximum;
                NUD_Purification.Value = puri;
                CHK_Shadow.Checked = puri > 0;

                NUD_ShadowID.Value = Math.Max(xk3.ShadowID, 0);
            }
            else
            {
                NUD_Purification.Value = 0;
                CHK_Shadow.Checked = false;
                NUD_ShadowID.Value = 0;
            }

            updateStats();

            TB_EXP.Text = xk3.EXP.ToString();
            Label_Gender.Text = gendersymbols[xk3.Gender];
            Label_Gender.ForeColor = xk3.Gender == 2 ? Label_Species.ForeColor : (xk3.Gender == 1 ? Color.Red : Color.Blue);
        }
        private PKM prepareXK3()
        {
            XK3 xk3 = pkm as XK3;
            if (xk3 == null)
                return null;

            xk3.Species = Util.getIndex(CB_Species);
            xk3.HeldItem = Util.getIndex(CB_HeldItem);
            xk3.TID = Util.ToInt32(TB_TID.Text);
            xk3.SID = Util.ToInt32(TB_SID.Text);
            xk3.EXP = Util.ToUInt32(TB_EXP.Text);
            xk3.PID = Util.getHEXval(TB_PID.Text);
            xk3.AbilityNumber = CB_Ability.SelectedIndex; // 0/1 (stored in IVbits)

            xk3.FatefulEncounter = CHK_Fateful.Checked;
            xk3.Gender = PKX.getGender(Label_Gender.Text);
            xk3.EV_HP = Util.ToInt32(TB_HPEV.Text);
            xk3.EV_ATK = Util.ToInt32(TB_ATKEV.Text);
            xk3.EV_DEF = Util.ToInt32(TB_DEFEV.Text);
            xk3.EV_SPE = Util.ToInt32(TB_SPEEV.Text);
            xk3.EV_SPA = Util.ToInt32(TB_SPAEV.Text);
            xk3.EV_SPD = Util.ToInt32(TB_SPDEV.Text);

            xk3.CNT_Cool = Util.ToInt32(TB_Cool.Text);
            xk3.CNT_Beauty = Util.ToInt32(TB_Beauty.Text);
            xk3.CNT_Cute = Util.ToInt32(TB_Cute.Text);
            xk3.CNT_Smart = Util.ToInt32(TB_Smart.Text);
            xk3.CNT_Tough = Util.ToInt32(TB_Tough.Text);
            xk3.CNT_Sheen = Util.ToInt32(TB_Sheen.Text);

            xk3.PKRS_Days = CB_PKRSDays.SelectedIndex;
            xk3.PKRS_Strain = CB_PKRSStrain.SelectedIndex;
            xk3.Nickname = TB_Nickname.Text;
            xk3.Move1 = Util.getIndex(CB_Move1);
            xk3.Move2 = Util.getIndex(CB_Move2);
            xk3.Move3 = Util.getIndex(CB_Move3);
            xk3.Move4 = Util.getIndex(CB_Move4);
            xk3.Move1_PP = Util.getIndex(CB_Move1) > 0 ? Util.ToInt32(TB_PP1.Text) : 0;
            xk3.Move2_PP = Util.getIndex(CB_Move2) > 0 ? Util.ToInt32(TB_PP2.Text) : 0;
            xk3.Move3_PP = Util.getIndex(CB_Move3) > 0 ? Util.ToInt32(TB_PP3.Text) : 0;
            xk3.Move4_PP = Util.getIndex(CB_Move4) > 0 ? Util.ToInt32(TB_PP4.Text) : 0;
            xk3.Move1_PPUps = Util.getIndex(CB_Move1) > 0 ? CB_PPu1.SelectedIndex : 0;
            xk3.Move2_PPUps = Util.getIndex(CB_Move2) > 0 ? CB_PPu2.SelectedIndex : 0;
            xk3.Move3_PPUps = Util.getIndex(CB_Move3) > 0 ? CB_PPu3.SelectedIndex : 0;
            xk3.Move4_PPUps = Util.getIndex(CB_Move4) > 0 ? CB_PPu4.SelectedIndex : 0;

            xk3.IV_HP = Util.ToInt32(TB_HPIV.Text);
            xk3.IV_ATK = Util.ToInt32(TB_ATKIV.Text);
            xk3.IV_DEF = Util.ToInt32(TB_DEFIV.Text);
            xk3.IV_SPE = Util.ToInt32(TB_SPEIV.Text);
            xk3.IV_SPA = Util.ToInt32(TB_SPAIV.Text);
            xk3.IV_SPD = Util.ToInt32(TB_SPDIV.Text);
            xk3.IsEgg = CHK_IsEgg.Checked;
            xk3.IsNicknamed = CHK_Nicknamed.Checked;

            xk3.OT_Name = TB_OT.Text;
            xk3.CurrentFriendship = Util.ToInt32(TB_Friendship.Text);

            xk3.Ball = Util.getIndex(CB_Ball);
            xk3.Met_Level = Util.ToInt32(TB_MetLevel.Text);
            xk3.OT_Gender = PKX.getGender(Label_OTGender.Text);
            xk3.Version = Util.getIndex(CB_GameOrigin);
            xk3.Language = Util.getIndex(CB_Language);

            xk3.Met_Location = Util.getIndex(CB_MetLocation);

            // Toss in Party Stats
            Array.Resize(ref xk3.Data, xk3.SIZE_PARTY);
            xk3.Stat_Level = Util.ToInt32(TB_Level.Text);
            xk3.Stat_HPCurrent = Util.ToInt32(Stat_HP.Text);
            xk3.Stat_HPMax = Util.ToInt32(Stat_HP.Text);
            xk3.Stat_ATK = Util.ToInt32(Stat_ATK.Text);
            xk3.Stat_DEF = Util.ToInt32(Stat_DEF.Text);
            xk3.Stat_SPE = Util.ToInt32(Stat_SPE.Text);
            xk3.Stat_SPA = Util.ToInt32(Stat_SPA.Text);
            xk3.Stat_SPD = Util.ToInt32(Stat_SPD.Text);

            if (HaX)
            {
                xk3.Stat_Level = (byte)Math.Min(Convert.ToInt32(MT_Level.Text), byte.MaxValue);
            }

            // Shadow Info
            xk3.ShadowID = (int)NUD_ShadowID.Value;
            if (xk3.ShadowID > 0)
                xk3.Purification = (int)NUD_Purification.Value;

            // Fix Moves if a slot is empty 
            xk3.FixMoves();

            xk3.RefreshChecksum();
            return xk3;
        }
    }
}
