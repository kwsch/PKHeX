using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class RibbMedal : Form
    {
        private readonly PictureBox[] pba;
        private readonly CheckBox[] cba;
        private readonly Image[] bma;
        private readonly PK6 pk6;
        public RibbMedal()
        {
            InitializeComponent();
            pk6 = Main.pk6; // cache copy
            pba = new[] { 
                                   PB_10, PB_11, PB_12, PB_13, 
                                   PB_14, PB_15, PB_16, PB_17,

                                   PB_20, PB_21, PB_22, PB_23, 
                                   PB_24, PB_25, PB_26, PB_27,

                                   PB_30, PB_31, PB_32, PB_33, 
                                   PB_34, PB_35, PB_36, PB_37,  

                                   PB_40, PB_41, PB_42, PB_43, 
                                   PB_44, PB_45, PB_46, PB_47,

                                   PB_50, PB_51, PB_52, PB_53, 
                                   PB_54,

                                   PB_57,
                                   PB_O0, PB_O1,PB_O2,PB_O3,PB_O4,PB_O5,
                               };
            cba = new [] {
                                   Kalos1a_0, Kalos1a_1, Kalos1a_2, Kalos1a_3, Kalos1a_4, Kalos1a_5, Kalos1a_6, Kalos1a_7,
                                   Kalos1b_0, Kalos1b_1, Kalos1b_2, Kalos1b_3, Kalos1b_4, Kalos1b_5, Kalos1b_6, Kalos1b_7,
                                   Kalos2a_0, Kalos2a_1, Kalos2a_2, Kalos2a_3, Kalos2a_4, Kalos2a_5, Kalos2a_6, Kalos2a_7,
                                   Kalos2b_0, Kalos2b_1, Kalos2b_2, Kalos2b_3, Kalos2b_4, Kalos2b_5, Kalos2b_6, Kalos2b_7,
                                   Extra1_0,  Extra1_1,  Extra1_2,  Extra1_3,  Extra1_4,

                                   Extra1_7, ORAS_0, ORAS_1, ORAS_2, ORAS_3, ORAS_4, ORAS_5, 
                             };
            bma = new Image[pba.Length];
            for (int i = 0; i < bma.Length; i++)
            {
                bma[i] = pba[i].Image;
                pba[i].Image = Util.ChangeOpacity(bma[i], 0.1);
            }
            Util.TranslateInterface(this, Main.curlanguage);

            // Set up Training Bag Data
            CB_Bag.Items.Clear();
            CB_Bag.Items.Add("---");
            for (int i = 1; i < Main.trainingbags.Length - 1; i++)
                CB_Bag.Items.Add(Main.trainingbags[i]);

            getData();
        }
        private void getData()
        {
            CHK_Secret.Checked = pk6.SecretSuperTraining;

            TMedal1_0.Checked = pk6.Unused0;
            TMedal1_1.Checked = pk6.Unused1;
            TMedal1_2.Checked = pk6.ST1_SPA;
            TMedal1_3.Checked = pk6.ST1_HP;
            TMedal1_4.Checked = pk6.ST1_ATK;
            TMedal1_5.Checked = pk6.ST1_SPD;
            TMedal1_6.Checked = pk6.ST1_SPE;
            TMedal1_7.Checked = pk6.ST1_DEF;

            TMedal2_0.Checked = pk6.ST2_SPA;
            TMedal2_1.Checked = pk6.ST2_HP;
            TMedal2_2.Checked = pk6.ST2_ATK;
            TMedal2_3.Checked = pk6.ST2_SPD;
            TMedal2_4.Checked = pk6.ST2_SPE;
            TMedal2_5.Checked = pk6.ST2_DEF;
            TMedal2_6.Checked = pk6.ST3_SPA;
            TMedal2_7.Checked = pk6.ST3_HP;

            TMedal3_0.Checked = pk6.ST3_ATK;
            TMedal3_1.Checked = pk6.ST3_SPD;
            TMedal3_2.Checked = pk6.ST3_SPE;
            TMedal3_3.Checked = pk6.ST3_DEF;
            TMedal3_4.Checked = pk6.ST4_1;
            TMedal3_5.Checked = pk6.ST5_1;
            TMedal3_6.Checked = pk6.ST5_2;
            TMedal3_7.Checked = pk6.ST5_3;

            TMedal4_0.Checked = pk6.ST5_4;
            TMedal4_1.Checked = pk6.ST6_1;
            TMedal4_2.Checked = pk6.ST6_2;
            TMedal4_3.Checked = pk6.ST6_3;
            TMedal4_4.Checked = pk6.ST7_1;
            TMedal4_5.Checked = pk6.ST7_2;
            TMedal4_6.Checked = pk6.ST7_3;
            TMedal4_7.Checked = pk6.ST8_1;

            CHK_D0.Checked = pk6.Dist1;
            CHK_D1.Checked = pk6.Dist2;
            CHK_D2.Checked = pk6.Dist3;
            CHK_D3.Checked = pk6.Dist4;
            CHK_D4.Checked = pk6.Dist5;
            CHK_D5.Checked = pk6.Dist6;

            Kalos1a_0.Checked = pk6.RIB0_0;
            Kalos1a_1.Checked = pk6.RIB0_1;
            Kalos1a_2.Checked = pk6.RIB0_2;
            Kalos1a_3.Checked = pk6.RIB0_3;
            Kalos1a_4.Checked = pk6.RIB0_4;
            Kalos1a_5.Checked = pk6.RIB0_5;
            Kalos1a_6.Checked = pk6.RIB0_6;
            Kalos1a_7.Checked = pk6.RIB0_7;

            Kalos1b_0.Checked = pk6.RIB1_0;
            Kalos1b_1.Checked = pk6.RIB1_1;
            Kalos1b_2.Checked = pk6.RIB1_2;
            Kalos1b_3.Checked = pk6.RIB1_3;
            Kalos1b_4.Checked = pk6.RIB1_4;
            Kalos1b_5.Checked = pk6.RIB1_5;
            Kalos1b_6.Checked = pk6.RIB1_6;
            Kalos1b_7.Checked = pk6.RIB1_7;

            Kalos2a_0.Checked = pk6.RIB2_0;
            Kalos2a_1.Checked = pk6.RIB2_1;
            Kalos2a_2.Checked = pk6.RIB2_2;
            Kalos2a_3.Checked = pk6.RIB2_3;
            Kalos2a_4.Checked = pk6.RIB2_4;
            Kalos2a_5.Checked = pk6.RIB2_5;
            Kalos2a_6.Checked = pk6.RIB2_6;
            Kalos2a_7.Checked = pk6.RIB2_7;

            Kalos2b_0.Checked = pk6.RIB3_0;
            Kalos2b_1.Checked = pk6.RIB3_1;
            Kalos2b_2.Checked = pk6.RIB3_2;
            Kalos2b_3.Checked = pk6.RIB3_3;
            Kalos2b_4.Checked = pk6.RIB3_4;
            Kalos2b_5.Checked = pk6.RIB3_5;
            Kalos2b_6.Checked = pk6.RIB3_6;
            Kalos2b_7.Checked = pk6.RIB3_7;

            Extra1_0.Checked = pk6.RIB4_0;
            Extra1_1.Checked = pk6.RIB4_1;
            Extra1_2.Checked = pk6.RIB4_2;
            Extra1_3.Checked = pk6.RIB4_3;
            Extra1_4.Checked = pk6.RIB4_4;

            // Introduced in ORAS
            Extra1_7.Checked = pk6.RIB4_7;

            ORAS_0.Checked = pk6.RIB5_0;
            ORAS_1.Checked = pk6.RIB5_1;
            ORAS_2.Checked = pk6.RIB5_2;
            ORAS_3.Checked = pk6.RIB5_3;
            ORAS_4.Checked = pk6.RIB5_4;
            ORAS_5.Checked = pk6.RIB5_5;

            TB_PastContest.Text = pk6.Memory_ContestCount.ToString();
            TB_PastBattle.Text = pk6.Memory_BattleCount.ToString();

            CB_Bag.SelectedIndex = pk6.TrainingBag;
            NUD_BagHits.Value = pk6.TrainingBagHits;
        }
        private void setData()
        {
            pk6.SecretSuperTraining = CHK_Secret.Checked;

            pk6.Unused0 = TMedal1_0.Checked;
            pk6.Unused1 = TMedal1_1.Checked;
            pk6.ST1_SPA = TMedal1_2.Checked;
            pk6.ST1_HP  = TMedal1_3.Checked;
            pk6.ST1_ATK = TMedal1_4.Checked;
            pk6.ST1_SPD = TMedal1_5.Checked;
            pk6.ST1_SPE = TMedal1_6.Checked;
            pk6.ST1_DEF = TMedal1_7.Checked;

            pk6.ST2_SPA = TMedal2_0.Checked;
            pk6.ST2_HP  = TMedal2_1.Checked;
            pk6.ST2_ATK = TMedal2_2.Checked;
            pk6.ST2_SPD = TMedal2_3.Checked;
            pk6.ST2_SPE = TMedal2_4.Checked;
            pk6.ST2_DEF = TMedal2_5.Checked;
            pk6.ST3_SPA = TMedal2_6.Checked;
            pk6.ST3_HP  = TMedal2_7.Checked;

            pk6.ST3_ATK = TMedal3_0.Checked;
            pk6.ST3_SPD = TMedal3_1.Checked;
            pk6.ST3_SPE = TMedal3_2.Checked;
            pk6.ST3_DEF = TMedal3_3.Checked;
            pk6.ST4_1 = TMedal3_4.Checked;
            pk6.ST5_1 = TMedal3_5.Checked;
            pk6.ST5_2 = TMedal3_6.Checked;
            pk6.ST5_3 = TMedal3_7.Checked;

            pk6.ST5_4 = TMedal4_0.Checked;
            pk6.ST6_1 = TMedal4_1.Checked;
            pk6.ST6_2 = TMedal4_2.Checked;
            pk6.ST6_3 = TMedal4_3.Checked;
            pk6.ST7_1 = TMedal4_4.Checked;
            pk6.ST7_2 = TMedal4_5.Checked;
            pk6.ST7_3 = TMedal4_6.Checked;
            pk6.ST8_1 = TMedal4_7.Checked;

            pk6.Dist1 = CHK_D0.Checked;
            pk6.Dist2 = CHK_D1.Checked;
            pk6.Dist3 = CHK_D2.Checked;
            pk6.Dist4 = CHK_D3.Checked;
            pk6.Dist5 = CHK_D4.Checked;
            pk6.Dist6 = CHK_D5.Checked;

            pk6.RIB0_0 = Kalos1a_0.Checked;
            pk6.RIB0_1 = Kalos1a_1.Checked;
            pk6.RIB0_2 = Kalos1a_2.Checked;
            pk6.RIB0_3 = Kalos1a_3.Checked;
            pk6.RIB0_4 = Kalos1a_4.Checked;
            pk6.RIB0_5 = Kalos1a_5.Checked;
            pk6.RIB0_6 = Kalos1a_6.Checked;
            pk6.RIB0_7 = Kalos1a_7.Checked;

            pk6.RIB1_0 = Kalos1b_0.Checked;
            pk6.RIB1_1 = Kalos1b_1.Checked;
            pk6.RIB1_2 = Kalos1b_2.Checked;
            pk6.RIB1_3 = Kalos1b_3.Checked;
            pk6.RIB1_4 = Kalos1b_4.Checked;
            pk6.RIB1_5 = Kalos1b_5.Checked;
            pk6.RIB1_6 = Kalos1b_6.Checked;
            pk6.RIB1_7 = Kalos1b_7.Checked;

            pk6.RIB2_0 = Kalos2a_0.Checked;
            pk6.RIB2_1 = Kalos2a_1.Checked;
            pk6.RIB2_2 = Kalos2a_2.Checked;
            pk6.RIB2_3 = Kalos2a_3.Checked;
            pk6.RIB2_4 = Kalos2a_4.Checked;
            pk6.RIB2_5 = Kalos2a_5.Checked;
            pk6.RIB2_6 = Kalos2a_6.Checked;
            pk6.RIB2_7 = Kalos2a_7.Checked;

            pk6.RIB3_0 = Kalos2b_0.Checked;
            pk6.RIB3_1 = Kalos2b_1.Checked;
            pk6.RIB3_2 = Kalos2b_2.Checked;
            pk6.RIB3_3 = Kalos2b_3.Checked;
            pk6.RIB3_4 = Kalos2b_4.Checked;
            pk6.RIB3_5 = Kalos2b_5.Checked;
            pk6.RIB3_6 = Kalos2b_6.Checked;
            pk6.RIB3_7 = Kalos2b_7.Checked;

            pk6.RIB4_0 = Extra1_0.Checked;
            pk6.RIB4_1 = Extra1_1.Checked;
            pk6.RIB4_2 = Extra1_2.Checked;
            pk6.RIB4_3 = Extra1_3.Checked;
            pk6.RIB4_4 = Extra1_4.Checked;

            // Introduced in ORAS
            pk6.RIB4_7 = Extra1_7.Checked;

            pk6.RIB5_0 = ORAS_0.Checked;
            pk6.RIB5_1 = ORAS_1.Checked;
            pk6.RIB5_2 = ORAS_2.Checked;
            pk6.RIB5_3 = ORAS_3.Checked;
            pk6.RIB5_4 = ORAS_4.Checked;
            pk6.RIB5_5 = ORAS_5.Checked;

            pk6.Memory_ContestCount = Util.ToInt32(TB_PastContest.Text);
            pk6.Memory_BattleCount = Util.ToInt32(TB_PastBattle.Text);

            pk6.TrainingBag = CB_Bag.SelectedIndex;
            pk6.TrainingBagHits = (int)NUD_BagHits.Value;
        }
        private void buttonFlag(bool b)
        {
            if (tabControl1.SelectedTab == Tab_Kalos)
            {
                // Kalos
                foreach (var chk in cba.Skip(0).Take(32))
                    chk.Checked = b;
            }
            else if (tabControl1.SelectedTab == Tab_Extra)
            {
                // Extra
                foreach (var chk in cba.Skip(32))
                    chk.Checked = b;

                if (Main.pk6.Version >= 0x10) return; // No Memory Ribbons for Pokémon from Generation 4+
                TB_PastContest.Text = (b ? 40 : 0).ToString();
                TB_PastBattle.Text = (b ? 8 : 0).ToString();
            }
            else if (tabControl1.SelectedTab == Tab_Medals)
            {
                // Medals
                if (CHK_Secret.Checked)
                {
                    CheckBox[] ck2 = { 
                                  TMedal3_4, 
                                  TMedal3_5, TMedal3_6, TMedal3_7, TMedal4_0, 
                                  TMedal4_1, TMedal4_2, TMedal4_3, 
                                  TMedal4_4, TMedal4_5, TMedal4_6, 
                                  TMedal4_7
                                };
                    foreach (var chk in ck2) chk.Checked = b;
                }
                CheckBox[] ck = { 
                                  // TMedal1_0, TMedal1_1, 
                                  TMedal1_2, TMedal1_3, TMedal1_4, TMedal1_5, TMedal1_6, TMedal1_7, 
                                  TMedal2_0, TMedal2_1, TMedal2_2, TMedal2_3, TMedal2_4, TMedal2_5, 
                                  TMedal2_6, TMedal2_7, TMedal3_0, TMedal3_1, TMedal3_2, TMedal3_3, 
                                  CHK_Secret
                                };
                foreach (var chk in ck) chk.Checked = b;
                foreach (CheckBox chk in new[] { CHK_D0, CHK_D1, CHK_D2, CHK_D3, CHK_D4, CHK_D5 }) chk.Checked = b;
            }
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            setData();
            Main.pk6 = pk6; // set back
            Close();
        }
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_AllRibbons_Click(object sender, EventArgs e)
        {
            buttonFlag(true);
        }
        private void B_NoRibbons_Click(object sender, EventArgs e)
        {
            buttonFlag(false);
        }

        private void updateSecretSuper(object sender, EventArgs e)
        {
            GB_Medals2.Enabled = CHK_Secret.Checked;
            if (CHK_Secret.Checked) return;

            CheckBox[] ck = {
                TMedal3_4, TMedal3_5, TMedal3_6, TMedal3_7, 
                TMedal4_0, TMedal4_1, TMedal4_2, TMedal4_3, 
                TMedal4_4, TMedal4_5, TMedal4_6, TMedal4_7
                            };
            foreach (var chk in ck) chk.Checked = false;
        }
        private void updateUnused(object sender, EventArgs e)
        {
            // Futureproofing: If either of the unused bitflags are 1 (true), 
            // we'll display them to alert the user.
            TMedal1_0.Visible = TMedal1_0.Checked;
            TMedal1_1.Visible = TMedal1_1.Checked;
        }
        private void updateRibbon(object sender, EventArgs e)
        {
            int index = Array.IndexOf(cba, sender);
            pba[index].Image = Util.ChangeOpacity(bma[index], (cba[index].Checked ? 1 : 0) * 0.9 + 0.1);
        }
        private void updateMemoryRibbon(object sender, EventArgs e)
        {
            if ((sender as MaskedTextBox).Text.Length == 0) { (sender as MaskedTextBox).Text = "0"; return; }
            if (sender as MaskedTextBox == TB_PastContest)
            {
                var val = Util.ToInt32(TB_PastContest.Text);
                if (val > 40) { TB_PastContest.Text = 40.ToString(); return; }
                PastContest.Image = Util.ChangeOpacity(val < 40 ? Properties.Resources.contestmemory : Properties.Resources.contestmemory2,
                    (val != 0 ? 1 : 0) * 0.9 + 0.1);
            }
            else
            {
                var val = Util.ToInt32(TB_PastBattle.Text);
                if (val > 8) { TB_PastBattle.Text = 8.ToString(); return; }
                PastBattle.Image = Util.ChangeOpacity(Util.ToUInt32(TB_PastBattle.Text) < 40 ? Properties.Resources.battlememory : Properties.Resources.battlememory2,
                    (val != 0 ? 1 : 0) * 0.9 + 0.1);
            }
        }
        private void clickRibbon(object sender, EventArgs e)
        {
            cba[Array.IndexOf(pba, sender)].Checked ^= true;
        }
    }
}
