using System;
using System.Drawing;

namespace PKHeX
{
    public partial class Main
    {
        private void populateFieldsCK3()
        {
            CK3 ck3 = pkm as CK3;
            if (ck3 == null)
                return;

            // Do first
            ck3.Stat_Level = PKX.getLevel(ck3.Species, ck3.EXP);
            if (ck3.Stat_Level == 100)
                ck3.EXP = PKX.getEXP(ck3.Stat_Level, ck3.Species);

            CB_Species.SelectedValue = ck3.Species;
            TB_Level.Text = ck3.Stat_Level.ToString();
            TB_EXP.Text = ck3.EXP.ToString();

            // Load rest
            CHK_Fateful.Checked = ck3.FatefulEncounter;
            CHK_IsEgg.Checked = ck3.IsEgg;
            CHK_Nicknamed.Checked = ck3.IsNicknamed;
            Label_OTGender.Text = gendersymbols[ck3.OT_Gender];
            Label_OTGender.ForeColor = ck3.OT_Gender == 1 ? Color.Red : Color.Blue;
            TB_PID.Text = ck3.PID.ToString("X8");
            CB_HeldItem.SelectedValue = ck3.HeldItem;
            CB_Ability.SelectedIndex = ck3.AbilityNumber > CB_Ability.Items.Count ? 0 : ck3.AbilityNumber;
            CB_Nature.SelectedValue = ck3.Nature;
            TB_TID.Text = ck3.TID.ToString("00000");
            TB_SID.Text = ck3.SID.ToString("00000");
            TB_Nickname.Text = ck3.Nickname;
            TB_OT.Text = ck3.OT_Name;
            TB_Friendship.Text = ck3.CurrentFriendship.ToString();
            GB_OT.BackgroundImage = null;
            CB_Language.SelectedValue = ck3.Language;
            CB_GameOrigin.SelectedValue = ck3.Version;
            CB_EncounterType.SelectedValue = ck3.Gen4 ? ck3.EncounterType : 0;
            CB_Ball.SelectedValue = ck3.Ball;

            int cr = ck3.CurrentRegion;
            int or = ck3.OriginalRegion;
            int ver = ck3.Version;
            CB_MetLocation.SelectedValue = ck3.Met_Location;

            TB_MetLevel.Text = ck3.Met_Level.ToString();

            // Reset Label and ComboBox visibility, as well as non-data checked status.
            Label_PKRS.Visible = CB_PKRSStrain.Visible = CHK_Infected.Checked = ck3.PKRS_Strain != 0;
            Label_PKRSdays.Visible = CB_PKRSDays.Visible = ck3.PKRS_Days != 0;

            // Set SelectedIndexes for PKRS
            CB_PKRSStrain.SelectedIndex = ck3.PKRS_Strain;
            CHK_Cured.Checked = ck3.PKRS_Strain > 0 && ck3.PKRS_Days == 0;
            CB_PKRSDays.SelectedIndex = Math.Min(CB_PKRSDays.Items.Count - 1, ck3.PKRS_Days); // to strip out bad hacked 'rus

            TB_Cool.Text = ck3.CNT_Cool.ToString();
            TB_Beauty.Text = ck3.CNT_Beauty.ToString();
            TB_Cute.Text = ck3.CNT_Cute.ToString();
            TB_Smart.Text = ck3.CNT_Smart.ToString();
            TB_Tough.Text = ck3.CNT_Tough.ToString();
            TB_Sheen.Text = ck3.CNT_Sheen.ToString();

            TB_HPIV.Text = ck3.IV_HP.ToString();
            TB_ATKIV.Text = ck3.IV_ATK.ToString();
            TB_DEFIV.Text = ck3.IV_DEF.ToString();
            TB_SPEIV.Text = ck3.IV_SPE.ToString();
            TB_SPAIV.Text = ck3.IV_SPA.ToString();
            TB_SPDIV.Text = ck3.IV_SPD.ToString();
            CB_HPType.SelectedValue = ck3.HPType;

            TB_HPEV.Text = ck3.EV_HP.ToString();
            TB_ATKEV.Text = ck3.EV_ATK.ToString();
            TB_DEFEV.Text = ck3.EV_DEF.ToString();
            TB_SPEEV.Text = ck3.EV_SPE.ToString();
            TB_SPAEV.Text = ck3.EV_SPA.ToString();
            TB_SPDEV.Text = ck3.EV_SPD.ToString();

            CB_Move1.SelectedValue = ck3.Move1;
            CB_Move2.SelectedValue = ck3.Move2;
            CB_Move3.SelectedValue = ck3.Move3;
            CB_Move4.SelectedValue = ck3.Move4;
            CB_PPu1.SelectedIndex = ck3.Move1_PPUps;
            CB_PPu2.SelectedIndex = ck3.Move2_PPUps;
            CB_PPu3.SelectedIndex = ck3.Move3_PPUps;
            CB_PPu4.SelectedIndex = ck3.Move4_PPUps;
            TB_PP1.Text = ck3.Move1_PP.ToString();
            TB_PP2.Text = ck3.Move2_PP.ToString();
            TB_PP3.Text = ck3.Move3_PP.ToString();
            TB_PP4.Text = ck3.Move4_PP.ToString();

            // Set Form if count is enough, else cap.
            CB_Form.SelectedIndex = CB_Form.Items.Count > ck3.AltForm ? ck3.AltForm : CB_Form.Items.Count - 1;

            // Load Extrabyte Value
            TB_ExtraByte.Text = ck3.Data[Convert.ToInt32(CB_ExtraBytes.Text, 16)].ToString();

            NUD_ShadowID.Value = ck3.ShadowID;
            FLP_Purification.Visible = ck3.ShadowID > 0;
            if (ck3.ShadowID > 0)
            {
                int puri = ck3.Purification;
                if (puri > NUD_Purification.Maximum)
                    puri = (int)NUD_Purification.Maximum;
                NUD_Purification.Value = puri;
                CHK_Shadow.Checked = puri > 0;

                NUD_ShadowID.Value = Math.Max(ck3.ShadowID, 0);
            }
            else
            {
                NUD_Purification.Value = 0;
                CHK_Shadow.Checked = false;
                NUD_ShadowID.Value = 0;
            }

            updateStats();

            TB_EXP.Text = ck3.EXP.ToString();
            Label_Gender.Text = gendersymbols[ck3.Gender];
            Label_Gender.ForeColor = ck3.Gender == 2 ? Label_Species.ForeColor : (ck3.Gender == 1 ? Color.Red : Color.Blue);
        }
        private PKM prepareCK3()
        {
            CK3 ck3 = pkm as CK3;
            if (ck3 == null)
                return null;

            ck3.Species = Util.getIndex(CB_Species);
            ck3.HeldItem = Util.getIndex(CB_HeldItem);
            ck3.TID = Util.ToInt32(TB_TID.Text);
            ck3.SID = Util.ToInt32(TB_SID.Text);
            ck3.EXP = Util.ToUInt32(TB_EXP.Text);
            ck3.PID = Util.getHEXval(TB_PID.Text);
            ck3.AbilityNumber = CB_Ability.SelectedIndex;

            ck3.FatefulEncounter = CHK_Fateful.Checked;
            ck3.Gender = PKX.getGender(Label_Gender.Text);
            ck3.EV_HP = Util.ToInt32(TB_HPEV.Text);
            ck3.EV_ATK = Util.ToInt32(TB_ATKEV.Text);
            ck3.EV_DEF = Util.ToInt32(TB_DEFEV.Text);
            ck3.EV_SPE = Util.ToInt32(TB_SPEEV.Text);
            ck3.EV_SPA = Util.ToInt32(TB_SPAEV.Text);
            ck3.EV_SPD = Util.ToInt32(TB_SPDEV.Text);

            ck3.CNT_Cool = Util.ToInt32(TB_Cool.Text);
            ck3.CNT_Beauty = Util.ToInt32(TB_Beauty.Text);
            ck3.CNT_Cute = Util.ToInt32(TB_Cute.Text);
            ck3.CNT_Smart = Util.ToInt32(TB_Smart.Text);
            ck3.CNT_Tough = Util.ToInt32(TB_Tough.Text);
            ck3.CNT_Sheen = Util.ToInt32(TB_Sheen.Text);

            ck3.PKRS_Days = CB_PKRSDays.SelectedIndex;
            ck3.PKRS_Strain = CB_PKRSStrain.SelectedIndex;
            ck3.Nickname = TB_Nickname.Text;
            ck3.Move1 = Util.getIndex(CB_Move1);
            ck3.Move2 = Util.getIndex(CB_Move2);
            ck3.Move3 = Util.getIndex(CB_Move3);
            ck3.Move4 = Util.getIndex(CB_Move4);
            ck3.Move1_PP = Util.getIndex(CB_Move1) > 0 ? Util.ToInt32(TB_PP1.Text) : 0;
            ck3.Move2_PP = Util.getIndex(CB_Move2) > 0 ? Util.ToInt32(TB_PP2.Text) : 0;
            ck3.Move3_PP = Util.getIndex(CB_Move3) > 0 ? Util.ToInt32(TB_PP3.Text) : 0;
            ck3.Move4_PP = Util.getIndex(CB_Move4) > 0 ? Util.ToInt32(TB_PP4.Text) : 0;
            ck3.Move1_PPUps = Util.getIndex(CB_Move1) > 0 ? CB_PPu1.SelectedIndex : 0;
            ck3.Move2_PPUps = Util.getIndex(CB_Move2) > 0 ? CB_PPu2.SelectedIndex : 0;
            ck3.Move3_PPUps = Util.getIndex(CB_Move3) > 0 ? CB_PPu3.SelectedIndex : 0;
            ck3.Move4_PPUps = Util.getIndex(CB_Move4) > 0 ? CB_PPu4.SelectedIndex : 0;

            ck3.IV_HP = Util.ToInt32(TB_HPIV.Text);
            ck3.IV_ATK = Util.ToInt32(TB_ATKIV.Text);
            ck3.IV_DEF = Util.ToInt32(TB_DEFIV.Text);
            ck3.IV_SPE = Util.ToInt32(TB_SPEIV.Text);
            ck3.IV_SPA = Util.ToInt32(TB_SPAIV.Text);
            ck3.IV_SPD = Util.ToInt32(TB_SPDIV.Text);
            ck3.IsEgg = CHK_IsEgg.Checked;
            ck3.IsNicknamed = CHK_Nicknamed.Checked;

            ck3.OT_Name = TB_OT.Text;
            ck3.CurrentFriendship = Util.ToInt32(TB_Friendship.Text);

            ck3.Ball = Util.getIndex(CB_Ball);
            ck3.Met_Level = Util.ToInt32(TB_MetLevel.Text);
            ck3.OT_Gender = PKX.getGender(Label_OTGender.Text);
            ck3.Version = Util.getIndex(CB_GameOrigin);
            ck3.Language = Util.getIndex(CB_Language);

            ck3.Met_Location = Util.getIndex(CB_MetLocation);

            // Toss in Party Stats
            Array.Resize(ref ck3.Data, ck3.SIZE_PARTY);
            ck3.Stat_Level = Util.ToInt32(TB_Level.Text);
            ck3.Stat_HPCurrent = Util.ToInt32(Stat_HP.Text);
            ck3.Stat_HPMax = Util.ToInt32(Stat_HP.Text);
            ck3.Stat_ATK = Util.ToInt32(Stat_ATK.Text);
            ck3.Stat_DEF = Util.ToInt32(Stat_DEF.Text);
            ck3.Stat_SPE = Util.ToInt32(Stat_SPE.Text);
            ck3.Stat_SPA = Util.ToInt32(Stat_SPA.Text);
            ck3.Stat_SPD = Util.ToInt32(Stat_SPD.Text);

            if (HaX)
            {
                ck3.Stat_Level = (byte)Math.Min(Convert.ToInt32(MT_Level.Text), byte.MaxValue);
            }

            // Shadow Info
            ck3.ShadowID = (int)NUD_ShadowID.Value;
            if (ck3.ShadowID > 0)
                ck3.Purification = (int)NUD_Purification.Value;

            // Fix Moves if a slot is empty 
            ck3.FixMoves();

            ck3.RefreshChecksum();
            return ck3;
        }
    }
}
