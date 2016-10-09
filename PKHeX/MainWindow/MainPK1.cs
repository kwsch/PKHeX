using System;

namespace PKHeX
{
    public partial class Main
    {
        private void populateFieldsPK1()
        {
            PK1 pk1 = pkm as PK1;
            if (pk1 == null)
                return;

            // Do first
            pk1.Stat_Level = HaX ? pk1.Stat_Level : PKX.getLevel(pk1.Species, pk1.EXP);
            if (!HaX && pk1.Stat_Level == 100)
                pk1.EXP = PKX.getEXP(pk1.Stat_Level, pk1.Species);

            CB_Species.SelectedValue = pk1.Species;
            if (HaX)
                MT_Level.Text = pk1.Stat_Level.ToString();
            TB_Level.Text = pk1.Stat_Level.ToString();
            TB_EXP.Text = pk1.EXP.ToString();

            // Load rest
            TB_TID.Text = pk1.TID.ToString("00000");
            CHK_Nicknamed.Checked = pk1.IsNicknamed;
            TB_Nickname.Text = pk1.Nickname;
            TB_OT.Text = pk1.OT_Name;
            GB_OT.BackgroundImage = null;

            // Reset Label and ComboBox visibility, as well as non-data checked status.
            Label_PKRS.Visible = false;
            Label_PKRSdays.Visible = false;

            TB_HPIV.Text = pk1.IV_HP.ToString();
            TB_ATKIV.Text = pk1.IV_ATK.ToString();
            TB_DEFIV.Text = pk1.IV_DEF.ToString();
            TB_SPEIV.Text = pk1.IV_SPE.ToString();
            TB_SPAIV.Text = pk1.IV_SPA.ToString();

            TB_HPEV.Text = pk1.EV_HP.ToString();
            TB_ATKEV.Text = pk1.EV_ATK.ToString();
            TB_DEFEV.Text = pk1.EV_DEF.ToString();
            TB_SPEEV.Text = pk1.EV_SPE.ToString();
            TB_SPAEV.Text = pk1.EV_SPA.ToString();

            CB_Move1.SelectedValue = pk1.Move1;
            CB_Move2.SelectedValue = pk1.Move2;
            CB_Move3.SelectedValue = pk1.Move3;
            CB_Move4.SelectedValue = pk1.Move4;
            CB_PPu1.SelectedIndex = pk1.Move1_PPUps;
            CB_PPu2.SelectedIndex = pk1.Move2_PPUps;
            CB_PPu3.SelectedIndex = pk1.Move3_PPUps;
            CB_PPu4.SelectedIndex = pk1.Move4_PPUps;
            TB_PP1.Text = pk1.Move1_PP.ToString();
            TB_PP2.Text = pk1.Move2_PP.ToString();
            TB_PP3.Text = pk1.Move3_PP.ToString();
            TB_PP4.Text = pk1.Move4_PP.ToString();

            CB_Language.SelectedIndex = pk1.Japanese ? 0 : 1;

            updateStats();

            TB_EXP.Text = pk1.EXP.ToString();
        }
        private PKM preparePK1()
        {
            PK1 pk1 = pkm as PK1;
            if (pk1 == null)
                return null;

            pk1.Species = Util.getIndex(CB_Species);
            pk1.TID = Util.ToInt32(TB_TID.Text);
            pk1.EXP = Util.ToUInt32(TB_EXP.Text);

            pk1.EV_HP = Util.ToInt32(TB_HPEV.Text);
            pk1.EV_ATK = Util.ToInt32(TB_ATKEV.Text);
            pk1.EV_DEF = Util.ToInt32(TB_DEFEV.Text);
            pk1.EV_SPE = Util.ToInt32(TB_SPEEV.Text);
            pk1.EV_SPC = Util.ToInt32(TB_SPAEV.Text);

            if (CHK_Nicknamed.Checked)
                pk1.Nickname = TB_Nickname.Text;
            else 
                pk1.setNotNicknamed();
            pk1.Move1 = Util.getIndex(CB_Move1);
            pk1.Move2 = Util.getIndex(CB_Move2);
            pk1.Move3 = Util.getIndex(CB_Move3);
            pk1.Move4 = Util.getIndex(CB_Move4);
            pk1.Move1_PP = Util.getIndex(CB_Move1) > 0 ? Util.ToInt32(TB_PP1.Text) : 0;
            pk1.Move2_PP = Util.getIndex(CB_Move2) > 0 ? Util.ToInt32(TB_PP2.Text) : 0;
            pk1.Move3_PP = Util.getIndex(CB_Move3) > 0 ? Util.ToInt32(TB_PP3.Text) : 0;
            pk1.Move4_PP = Util.getIndex(CB_Move4) > 0 ? Util.ToInt32(TB_PP4.Text) : 0;
            pk1.Move1_PPUps = Util.getIndex(CB_Move1) > 0 ? CB_PPu1.SelectedIndex : 0;
            pk1.Move2_PPUps = Util.getIndex(CB_Move2) > 0 ? CB_PPu2.SelectedIndex : 0;
            pk1.Move3_PPUps = Util.getIndex(CB_Move3) > 0 ? CB_PPu3.SelectedIndex : 0;
            pk1.Move4_PPUps = Util.getIndex(CB_Move4) > 0 ? CB_PPu4.SelectedIndex : 0;

            pk1.IV_HP = Util.ToInt32(TB_HPIV.Text);
            pk1.IV_ATK = Util.ToInt32(TB_ATKIV.Text);
            pk1.IV_DEF = Util.ToInt32(TB_DEFIV.Text);
            pk1.IV_SPE = Util.ToInt32(TB_SPEIV.Text);
            pk1.IV_SPA = Util.ToInt32(TB_SPAIV.Text);

            pk1.OT_Name = TB_OT.Text;

            // Toss in Party Stats
            Array.Resize(ref pk1.Data, pk1.SIZE_PARTY);
            pk1.Stat_Level = Util.ToInt32(TB_Level.Text);
            pk1.Stat_HPCurrent = Util.ToInt32(Stat_HP.Text);
            pk1.Stat_HPMax = Util.ToInt32(Stat_HP.Text);
            pk1.Stat_ATK = Util.ToInt32(Stat_ATK.Text);
            pk1.Stat_DEF = Util.ToInt32(Stat_DEF.Text);
            pk1.Stat_SPE = Util.ToInt32(Stat_SPE.Text);
            pk1.Stat_SPA = Util.ToInt32(Stat_SPA.Text);
            pk1.Stat_SPD = Util.ToInt32(Stat_SPD.Text);

            if (HaX)
            {
                pk1.Stat_Level = (byte)Math.Min(Convert.ToInt32(MT_Level.Text), byte.MaxValue);
            }

            // Fix Moves if a slot is empty 
            pk1.FixMoves();

            pk1.RefreshChecksum();
            return pk1;
        }
    }
}
