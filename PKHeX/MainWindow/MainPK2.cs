using System;
using System.Drawing;

namespace PKHeX
{
    public partial class Main
    {
        private void populateFieldsPK2()
        {
            PK2 pk2 = pkm as PK2;
            if (pk2 == null)
                return;

            // Do first
            pk2.Stat_Level = HaX ? pk2.Stat_Level : PKX.getLevel(pk2.Species, pk2.EXP);
            if (!HaX && pk2.Stat_Level == 100)
                pk2.EXP = PKX.getEXP(pk2.Stat_Level, pk2.Species);

            CB_Species.SelectedValue = pk2.Species;
            if (HaX)
                MT_Level.Text = pk2.Stat_Level.ToString();
            TB_Level.Text = pk2.Stat_Level.ToString();
            TB_EXP.Text = pk2.EXP.ToString();
            CB_HeldItem.SelectedValue = pk2.HeldItem;
            CB_Form.SelectedIndex = pk2.AltForm;
            CHK_IsEgg.Checked = pk2.IsEgg;
            TB_Friendship.Text = pk2.CurrentFriendship.ToString();

            TB_MetLevel.Text = pk2.Met_Level.ToString();
            CB_MetLocation.SelectedValue = pk2.Met_Location;
            CB_MetTimeOfDay.SelectedIndex = pk2.Met_TimeOfDay;

            // Load rest
            TB_TID.Text = pk2.TID.ToString("00000");
            CHK_Nicknamed.Checked = pk2.IsNicknamed;
            TB_Nickname.Text = pk2.Nickname;
            TB_OT.Text = pk2.OT_Name;
            GB_OT.BackgroundImage = null;
            Label_OTGender.Text = gendersymbols[pk2.OT_Gender];
            Label_OTGender.ForeColor = pk2.OT_Gender == 1 ? Color.Red : Color.Blue;
            // Reset Label and ComboBox visibility, as well as non-data checked status.
            Label_PKRS.Visible = false;
            Label_PKRSdays.Visible = false;

            TB_HPIV.Text = pk2.IV_HP.ToString();
            TB_ATKIV.Text = pk2.IV_ATK.ToString();
            TB_DEFIV.Text = pk2.IV_DEF.ToString();
            TB_SPEIV.Text = pk2.IV_SPE.ToString();
            TB_SPAIV.Text = pk2.IV_SPA.ToString();

            TB_HPEV.Text = pk2.EV_HP.ToString();
            TB_ATKEV.Text = pk2.EV_ATK.ToString();
            TB_DEFEV.Text = pk2.EV_DEF.ToString();
            TB_SPEEV.Text = pk2.EV_SPE.ToString();
            TB_SPAEV.Text = pk2.EV_SPA.ToString();

            CB_Move1.SelectedValue = pk2.Move1;
            CB_Move2.SelectedValue = pk2.Move2;
            CB_Move3.SelectedValue = pk2.Move3;
            CB_Move4.SelectedValue = pk2.Move4;
            CB_PPu1.SelectedIndex = pk2.Move1_PPUps;
            CB_PPu2.SelectedIndex = pk2.Move2_PPUps;
            CB_PPu3.SelectedIndex = pk2.Move3_PPUps;
            CB_PPu4.SelectedIndex = pk2.Move4_PPUps;
            TB_PP1.Text = pk2.Move1_PP.ToString();
            TB_PP2.Text = pk2.Move2_PP.ToString();
            TB_PP3.Text = pk2.Move3_PP.ToString();
            TB_PP4.Text = pk2.Move4_PP.ToString();

            CB_Language.SelectedIndex = pk2.Japanese ? 0 : 1;

            updateStats();
            setIsShiny(null);

            Label_Gender.Text = gendersymbols[pk2.Gender];
            Label_Gender.ForeColor = pk2.Gender == 2 ? Label_Species.ForeColor : (pk2.Gender == 1 ? Color.Red : Color.Blue);
            TB_EXP.Text = pk2.EXP.ToString();
        }
        private PKM preparePK2()
        {
            PK2 pk2 = pkm as PK2;
            if (pk2 == null)
                return null;

            pk2.Species = Util.getIndex(CB_Species);
            pk2.TID = Util.ToInt32(TB_TID.Text);
            pk2.EXP = Util.ToUInt32(TB_EXP.Text);
            pk2.HeldItem = Util.getIndex(CB_HeldItem);
            pk2.IsEgg = CHK_IsEgg.Checked;
            pk2.CurrentFriendship = Util.ToInt32(TB_Friendship.Text);
            pk2.Met_Level = Util.ToInt32(TB_MetLevel.Text);
            pk2.Met_Location = Util.getIndex(CB_MetLocation);
            pk2.Met_TimeOfDay = CB_MetTimeOfDay.SelectedIndex;

            pk2.EV_HP = Util.ToInt32(TB_HPEV.Text);
            pk2.EV_ATK = Util.ToInt32(TB_ATKEV.Text);
            pk2.EV_DEF = Util.ToInt32(TB_DEFEV.Text);
            pk2.EV_SPE = Util.ToInt32(TB_SPEEV.Text);
            pk2.EV_SPC = Util.ToInt32(TB_SPAEV.Text);

            if (CHK_Nicknamed.Checked)
                pk2.Nickname = TB_Nickname.Text;
            else 
                pk2.setNotNicknamed();
            pk2.Move1 = Util.getIndex(CB_Move1);
            pk2.Move2 = Util.getIndex(CB_Move2);
            pk2.Move3 = Util.getIndex(CB_Move3);
            pk2.Move4 = Util.getIndex(CB_Move4);
            pk2.Move1_PP = Util.getIndex(CB_Move1) > 0 ? Util.ToInt32(TB_PP1.Text) : 0;
            pk2.Move2_PP = Util.getIndex(CB_Move2) > 0 ? Util.ToInt32(TB_PP2.Text) : 0;
            pk2.Move3_PP = Util.getIndex(CB_Move3) > 0 ? Util.ToInt32(TB_PP3.Text) : 0;
            pk2.Move4_PP = Util.getIndex(CB_Move4) > 0 ? Util.ToInt32(TB_PP4.Text) : 0;
            pk2.Move1_PPUps = Util.getIndex(CB_Move1) > 0 ? CB_PPu1.SelectedIndex : 0;
            pk2.Move2_PPUps = Util.getIndex(CB_Move2) > 0 ? CB_PPu2.SelectedIndex : 0;
            pk2.Move3_PPUps = Util.getIndex(CB_Move3) > 0 ? CB_PPu3.SelectedIndex : 0;
            pk2.Move4_PPUps = Util.getIndex(CB_Move4) > 0 ? CB_PPu4.SelectedIndex : 0;

            pk2.IV_HP = Util.ToInt32(TB_HPIV.Text);
            pk2.IV_ATK = Util.ToInt32(TB_ATKIV.Text);
            pk2.IV_DEF = Util.ToInt32(TB_DEFIV.Text);
            pk2.IV_SPE = Util.ToInt32(TB_SPEIV.Text);
            pk2.IV_SPA = Util.ToInt32(TB_SPAIV.Text);

            pk2.OT_Name = TB_OT.Text;

            // Toss in Party Stats
            Array.Resize(ref pk2.Data, pk2.SIZE_PARTY);
            pk2.Stat_Level = Util.ToInt32(TB_Level.Text);
            pk2.Stat_HPCurrent = Util.ToInt32(Stat_HP.Text);
            pk2.Stat_HPMax = Util.ToInt32(Stat_HP.Text);
            pk2.Stat_ATK = Util.ToInt32(Stat_ATK.Text);
            pk2.Stat_DEF = Util.ToInt32(Stat_DEF.Text);
            pk2.Stat_SPE = Util.ToInt32(Stat_SPE.Text);
            pk2.Stat_SPA = Util.ToInt32(Stat_SPA.Text);
            pk2.Stat_SPD = Util.ToInt32(Stat_SPD.Text);

            if (HaX)
            {
                pk2.Stat_Level = (byte)Math.Min(Convert.ToInt32(MT_Level.Text), byte.MaxValue);
            }

            // Fix Moves if a slot is empty 
            pk2.FixMoves();

            return pk2;
        }
    }
}
