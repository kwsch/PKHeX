using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace PKHeX
{
    public partial class RibbMedal : Form
    {
        Form1 m_parent;
        CheckBox[] distro;
        public RibbMedal(Form1 frm1)
        {
            InitializeComponent();
            Util.TranslateInterface(this, Form1.curlanguage);
            m_parent = frm1;

            // Set up Training Bag Data
            comboBox1.Items.Clear();
            comboBox1.Items.Add("---");
            for (int i = 1; i < Form1.trainingbags.Length - 1; i++)
                comboBox1.Items.Add(Form1.trainingbags[i]);
            comboBox1.SelectedIndex = m_parent.buff[0x17];
            numericUpDown1.Value = m_parent.buff[0x16];
            distro = new CheckBox[] { CHK_D0, CHK_D1, CHK_D2, CHK_D3, CHK_D4, CHK_D5 };
            getRibbons();
        }
        private void getRibbons()
        {
            Bitmap[] bma = {
                                   Properties.Resources.kaloschamp, Properties.Resources.hoennchamp,        Properties.Resources.sinnohchamp,   Properties.Resources.bestfriends,
                                   Properties.Resources.training,   Properties.Resources.skillfullbattler,  Properties.Resources.expertbattler, Properties.Resources.effort,

                                   Properties.Resources.alert,      Properties.Resources.shock,             Properties.Resources.downcast,      Properties.Resources.careless,
                                   Properties.Resources.relax,      Properties.Resources.snooze,            Properties.Resources.smile,         Properties.Resources.gorgeous,

                                   Properties.Resources.royal,      Properties.Resources.gorgeousroyal,     Properties.Resources.artist,        Properties.Resources.footprint,
                                   Properties.Resources.record,     Properties.Resources.legend,            Properties.Resources.country,       Properties.Resources.national,

                                   Properties.Resources.earth,      Properties.Resources.world,             Properties.Resources.classic,       Properties.Resources.premier,
                                   Properties.Resources._event,     Properties.Resources.birthday,          Properties.Resources.special,       Properties.Resources.souvenir,

                                   Properties.Resources.wishing,    Properties.Resources.battlechamp,       Properties.Resources.regionalchamp, Properties.Resources.nationalchamp,
                                   Properties.Resources.worldchamp,
                                   
                                   Properties.Resources.ribbon_40,Properties.Resources.ribbon_41,Properties.Resources.ribbon_42,Properties.Resources.ribbon_43,
                                   Properties.Resources.ribbon_44,Properties.Resources.ribbon_45,Properties.Resources.ribbon_46,
                           };
            PictureBox[] pba = { 
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

            for (int i = 0; i < bma.Length; i++)
                pba[i].Image = Util.ChangeOpacity(bma[i], 0.1);
            
            int rv = 0; // Read from value (later redefined)

            // Populate Medals (what a mess)
            rv = m_parent.buff[0x2C];
            updateRibbon(TMedal1_0, rv, 0);
            updateRibbon(TMedal1_1, rv, 1);
            updateRibbon(TMedal1_2, rv, 2);
            updateRibbon(TMedal1_3, rv, 3);
            updateRibbon(TMedal1_4, rv, 4);
            updateRibbon(TMedal1_5, rv, 5);
            updateRibbon(TMedal1_6, rv, 6);
            updateRibbon(TMedal1_7, rv, 7);
            rv = m_parent.buff[0x2D];
            updateRibbon(TMedal2_0, rv, 0);
            updateRibbon(TMedal2_1, rv, 1);
            updateRibbon(TMedal2_2, rv, 2);
            updateRibbon(TMedal2_3, rv, 3);
            updateRibbon(TMedal2_4, rv, 4);
            updateRibbon(TMedal2_5, rv, 5);
            updateRibbon(TMedal2_6, rv, 6);
            updateRibbon(TMedal2_7, rv, 7);
            rv = m_parent.buff[0x2E];
            updateRibbon(TMedal3_0, rv, 0);
            updateRibbon(TMedal3_1, rv, 1);
            updateRibbon(TMedal3_2, rv, 2);
            updateRibbon(TMedal3_3, rv, 3);
            updateRibbon(TMedal3_4, rv, 4);
            updateRibbon(TMedal3_5, rv, 5);
            updateRibbon(TMedal3_6, rv, 6);
            updateRibbon(TMedal3_7, rv, 7);
            rv = m_parent.buff[0x2F];
            updateRibbon(TMedal4_0, rv, 0);
            updateRibbon(TMedal4_1, rv, 1);
            updateRibbon(TMedal4_2, rv, 2);
            updateRibbon(TMedal4_3, rv, 3);
            updateRibbon(TMedal4_4, rv, 4);
            updateRibbon(TMedal4_5, rv, 5);
            updateRibbon(TMedal4_6, rv, 6);
            updateRibbon(TMedal4_7, rv, 7);

            // Populate Kalos Ribbons
            rv = m_parent.buff[0x30];
            updateRibbon(Kalos1a_0, rv, 0);
            updateRibbon(Kalos1a_1, rv, 1);
            updateRibbon(Kalos1a_2, rv, 2);
            updateRibbon(Kalos1a_3, rv, 3);
            updateRibbon(Kalos1a_4, rv, 4);
            updateRibbon(Kalos1a_5, rv, 5);
            updateRibbon(Kalos1a_6, rv, 6);
            updateRibbon(Kalos1a_7, rv, 7);
            rv = m_parent.buff[0x31];
            updateRibbon(Kalos1b_0, rv, 0);
            updateRibbon(Kalos1b_1, rv, 1);
            updateRibbon(Kalos1b_2, rv, 2);
            updateRibbon(Kalos1b_3, rv, 3);
            updateRibbon(Kalos1b_4, rv, 4);
            updateRibbon(Kalos1b_5, rv, 5);
            updateRibbon(Kalos1b_6, rv, 6);
            updateRibbon(Kalos1b_7, rv, 7);
            rv = m_parent.buff[0x32];
            updateRibbon(Kalos2a_0, rv, 0);
            updateRibbon(Kalos2a_1, rv, 1);
            updateRibbon(Kalos2a_2, rv, 2);
            updateRibbon(Kalos2a_3, rv, 3);
            updateRibbon(Kalos2a_4, rv, 4);
            updateRibbon(Kalos2a_5, rv, 5);
            updateRibbon(Kalos2a_6, rv, 6);
            updateRibbon(Kalos2a_7, rv, 7);
            rv = m_parent.buff[0x33];
            updateRibbon(Kalos2b_0, rv, 0);
            updateRibbon(Kalos2b_1, rv, 1);
            updateRibbon(Kalos2b_2, rv, 2);
            updateRibbon(Kalos2b_3, rv, 3);
            updateRibbon(Kalos2b_4, rv, 4);
            updateRibbon(Kalos2b_5, rv, 5);
            updateRibbon(Kalos2b_6, rv, 6);
            updateRibbon(Kalos2b_7, rv, 7);

            // Populate Extra Ribbons
            rv = m_parent.buff[0x34];
            updateRibbon(Extra1_0, rv, 0);
            updateRibbon(Extra1_1, rv, 1);
            updateRibbon(Extra1_2, rv, 2);
            updateRibbon(Extra1_3, rv, 3);
            updateRibbon(Extra1_4, rv, 4);

            // oras
            updateRibbon(Extra1_7, rv, 7);
            rv = m_parent.buff[0x35];
            updateRibbon(ORAS_0, rv, 0);
            updateRibbon(ORAS_1, rv, 1);
            updateRibbon(ORAS_2, rv, 2);
            updateRibbon(ORAS_3, rv, 3);
            updateRibbon(ORAS_4, rv, 4);
            updateRibbon(ORAS_5, rv, 5);

            TB_PastContest.Text = m_parent.buff[0x38].ToString();
            TB_PastBattle.Text = m_parent.buff[0x39].ToString();

            rv = m_parent.buff[0x3A];
            updateRibbon(CHK_D0, rv, 0);
            updateRibbon(CHK_D1, rv, 1);
            updateRibbon(CHK_D2, rv, 2);
            updateRibbon(CHK_D3, rv, 3);
            updateRibbon(CHK_D4, rv, 4);
            updateRibbon(CHK_D5, rv, 5);

            CHK_Secret.Checked = Convert.ToBoolean(m_parent.buff[0x72]);
        }                                       // Populate Ribbons prompted
        private void setRibbons()
        {
            // pass kalos data
            int kalos1a = 0, kalos1b = 0, kalos2a = 0, kalos2b = 0;
            kalos1a |= addRibbon(Kalos1a_0);
            kalos1a |= addRibbon(Kalos1a_1);
            kalos1a |= addRibbon(Kalos1a_2);
            kalos1a |= addRibbon(Kalos1a_3);
            kalos1a |= addRibbon(Kalos1a_4);
            kalos1a |= addRibbon(Kalos1a_5);
            kalos1a |= addRibbon(Kalos1a_6);
            kalos1a |= addRibbon(Kalos1a_7);////
            kalos1b |= addRibbon(Kalos1b_0);
            kalos1b |= addRibbon(Kalos1b_1);
            kalos1b |= addRibbon(Kalos1b_2);
            kalos1b |= addRibbon(Kalos1b_3);
            kalos1b |= addRibbon(Kalos1b_4);
            kalos1b |= addRibbon(Kalos1b_5);
            kalos1b |= addRibbon(Kalos1b_6);
            kalos1b |= addRibbon(Kalos1b_7);////
            kalos2a |= addRibbon(Kalos2a_0);
            kalos2a |= addRibbon(Kalos2a_1);
            kalos2a |= addRibbon(Kalos2a_2);
            kalos2a |= addRibbon(Kalos2a_3);
            kalos2a |= addRibbon(Kalos2a_4);
            kalos2a |= addRibbon(Kalos2a_5);
            kalos2a |= addRibbon(Kalos2a_6);
            kalos2a |= addRibbon(Kalos2a_7);////
            kalos2b |= addRibbon(Kalos2b_0);
            kalos2b |= addRibbon(Kalos2b_1);
            kalos2b |= addRibbon(Kalos2b_2);
            kalos2b |= addRibbon(Kalos2b_3);
            kalos2b |= addRibbon(Kalos2b_4);
            kalos2b |= addRibbon(Kalos2b_5);
            kalos2b |= addRibbon(Kalos2b_6);
            kalos2b |= addRibbon(Kalos2b_7);////
            m_parent.buff[0x30] = (byte)kalos1a;
            m_parent.buff[0x31] = (byte)kalos1b;
            m_parent.buff[0x32] = (byte)kalos2a;
            m_parent.buff[0x33] = (byte)kalos2b;

            // Pass Extra Ribbon
            int extra1 = 0;
            extra1 |= addRibbon(Extra1_0);
            extra1 |= addRibbon(Extra1_1);
            extra1 |= addRibbon(Extra1_2);
            extra1 |= addRibbon(Extra1_3);
            extra1 |= addRibbon(Extra1_4);

            // ORAS
            extra1 |= addRibbon(Extra1_7); // Hoenn Champ
            m_parent.buff[0x34] = (byte)extra1;

            int oras = 0;
            oras |= addRibbon(ORAS_0);
            oras |= addRibbon(ORAS_1);
            oras |= addRibbon(ORAS_2);
            oras |= addRibbon(ORAS_3);
            oras |= addRibbon(ORAS_4);
            oras |= addRibbon(ORAS_5);
            m_parent.buff[0x35] = (byte)oras;

            // Gather Super Training Medals
            int medals1 = 0, medals2 = 0, medals3 = 0, medals4 = 0;
            medals1 |= addRibbon(TMedal1_0);
            medals1 |= addRibbon(TMedal1_1);
            medals1 |= addRibbon(TMedal1_2);
            medals1 |= addRibbon(TMedal1_3);
            medals1 |= addRibbon(TMedal1_4);
            medals1 |= addRibbon(TMedal1_5);
            medals1 |= addRibbon(TMedal1_6);
            medals1 |= addRibbon(TMedal1_7);////
            medals2 |= addRibbon(TMedal2_0);
            medals2 |= addRibbon(TMedal2_1);
            medals2 |= addRibbon(TMedal2_2);
            medals2 |= addRibbon(TMedal2_3);
            medals2 |= addRibbon(TMedal2_4);
            medals2 |= addRibbon(TMedal2_5);
            medals2 |= addRibbon(TMedal2_6);
            medals2 |= addRibbon(TMedal2_7);////
            medals3 |= addRibbon(TMedal3_0);
            medals3 |= addRibbon(TMedal3_1);
            medals3 |= addRibbon(TMedal3_2);
            medals3 |= addRibbon(TMedal3_3);
            medals3 |= addRibbon(TMedal3_4);
            medals3 |= addRibbon(TMedal3_5);
            medals3 |= addRibbon(TMedal3_6);
            medals3 |= addRibbon(TMedal3_7);////
            medals4 |= addRibbon(TMedal4_0);
            medals4 |= addRibbon(TMedal4_1);
            medals4 |= addRibbon(TMedal4_2);
            medals4 |= addRibbon(TMedal4_3);
            medals4 |= addRibbon(TMedal4_4);
            medals4 |= addRibbon(TMedal4_5);
            medals4 |= addRibbon(TMedal4_6);
            medals4 |= addRibbon(TMedal4_7);////
            m_parent.buff[0x2C] = (byte)medals1;
            m_parent.buff[0x2D] = (byte)medals2;
            m_parent.buff[0x2E] = (byte)medals3;
            m_parent.buff[0x2F] = (byte)medals4;

            m_parent.buff[0x38] = (byte)Util.ToUInt32(TB_PastContest.Text);
            m_parent.buff[0x39] = (byte)Util.ToUInt32(TB_PastBattle.Text);

            int dis = 0;
            dis |= addRibbon(CHK_D0);
            dis |= addRibbon(CHK_D1);
            dis |= addRibbon(CHK_D2);
            dis |= addRibbon(CHK_D3);
            dis |= addRibbon(CHK_D4);
            dis |= addRibbon(CHK_D5);
            m_parent.buff[0x3A] = (byte)dis;

            m_parent.buff[0x72] = (byte)Convert.ToByte(CHK_Secret.Checked);
        }                                       // Saving Ribbons prompted
        private void updateRibbon(CheckBox chk, int rv, int sh)
        {
            chk.Checked = Convert.ToBoolean((rv >> sh) & 1);
        }         // Checkbox back to bitflag
        private int addRibbon(CheckBox cb)
        {
            string a = cb.Name;
            int sv = Convert.ToInt32(a.Substring(a.Length - 1, 1));
            int cv = Convert.ToInt32(cb.Checked);
            return (cv << sv);
        }                              // Add Ribbon if Checked
        private void checkboxFlag(CheckBox[] ck, bool b)
        {
            for (int i = 0; i < ck.Count(); i++)
                ck[i].Checked = b;
        }                // Checkbox flipping dependent on T/F
        private void buttonFlag(bool b)
        {
            if (tabControl1.SelectedTab == Tab_Kalos)
            {
                // Kalos
                CheckBox[] ck = { 
                                  Kalos1a_0, Kalos1a_1, Kalos1a_2, Kalos1a_3, Kalos1a_4, Kalos1a_5, Kalos1a_6, Kalos1a_7, 
                                  Kalos1b_0, Kalos1b_1, Kalos1b_2, Kalos1b_3, Kalos1b_4, Kalos1b_5, Kalos1b_6, Kalos1b_7,
                                  Kalos2a_0, Kalos2a_1, Kalos2a_2, Kalos2a_3, Kalos2a_4, Kalos2a_5, Kalos2a_6, Kalos2a_7,
                                  Kalos2b_0, Kalos2b_1, Kalos2b_2, Kalos2b_3, Kalos2b_4, Kalos2b_5, Kalos2b_6, Kalos2b_7
                                };
                checkboxFlag(ck, b);
            }
            else if (tabControl1.SelectedTab == Tab_Extra)
            {
                // Extra
                CheckBox[] ck = { 
                                  Extra1_0, Extra1_1, Extra1_2, Extra1_3, Extra1_4, 
                                  
                                  Extra1_7, ORAS_0, ORAS_1, ORAS_2, ORAS_3, ORAS_4, ORAS_5, 
                                };
                checkboxFlag(ck, b);

                TB_PastContest.Text = (Convert.ToInt32(b) * 40).ToString();
                TB_PastBattle.Text = (Convert.ToInt32(b) * 8).ToString();
                if (m_parent.buff[0xDF] > 0x10)
                {
                    TB_PastContest.Text = 0.ToString();
                    TB_PastBattle.Text = 0.ToString();
                }
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
                    checkboxFlag(ck2, b);
                }
                CheckBox[] ck = { 
                                  //TMedal1_0, TMedal1_1, 
                                  TMedal1_2, TMedal1_3, TMedal1_4, TMedal1_5, TMedal1_6, TMedal1_7, 
                                  TMedal2_0, TMedal2_1, TMedal2_2, TMedal2_3, TMedal2_4, TMedal2_5, 
                                  TMedal2_6, TMedal2_7, TMedal3_0, TMedal3_1, TMedal3_2, TMedal3_3, 
                                  CHK_Secret
                                };
                checkboxFlag(ck, b);
                foreach (CheckBox chk in distro) chk.Checked = b;
            }
        }                                 // Checkbox Flipping Logic (dependent on Tab)

        private void BTN_Save_Click(object sender, EventArgs e)
        {
            m_parent.buff[0x17] = (byte)comboBox1.SelectedIndex;
            m_parent.buff[0x16] = (byte)numericUpDown1.Value;
            setRibbons();
            Close();
        }         // Save Button
        private void BTN_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }       // Cancel Button
        private void BTN_AllRibbons_Click(object sender, EventArgs e)
        {
            buttonFlag(true);
        }   // All checkboxes Button
        private void BTN_NoRibbons_Click(object sender, EventArgs e)
        {
            buttonFlag(false);
        }    // No checkboxes Button

        private void updateSecretSuper(object sender, EventArgs e)
        {
            GB_Medals2.Enabled = CHK_Secret.Checked;
            if (!CHK_Secret.Checked)
            {
                CheckBox[] ck = {TMedal3_4, 
                                  TMedal3_5, TMedal3_6, TMedal3_7, TMedal4_0, 
                                  TMedal4_1, TMedal4_2, TMedal4_3, 
                                  TMedal4_4, TMedal4_5, TMedal4_6, 
                                  TMedal4_7};
                checkboxFlag(ck, false);
            }
        }      // Only allow secret if checkbox is toggled.
        private void unusedbits(object sender, EventArgs e)
        {
            // Futureproofing: If either of the unused bitflags are 1 (true), 
            // we'll display them to alert the user.
            TMedal1_0.Visible = TMedal1_0.Checked;
            TMedal1_1.Visible = TMedal1_1.Checked;
        }

        private void toggleribbon(object sender, EventArgs e)
        {
            PictureBox[] pba = { 
                                   PB_10, PB_11, PB_12, PB_13, PB_14, PB_15, PB_16, PB_17,
                                   PB_20, PB_21, PB_22, PB_23, PB_24, PB_25, PB_26, PB_27,
                                   PB_30, PB_31, PB_32, PB_33, PB_34, PB_35, PB_36, PB_37,  
                                   PB_40, PB_41, PB_42, PB_43, PB_44, PB_45, PB_46, PB_47,
                                   PB_50, PB_51, PB_52, PB_53, PB_54,               PB_57,

                                   PB_O0, PB_O1, PB_O2, PB_O3, PB_O4, PB_O5,    
                               };
            CheckBox[] cba = {
                                   Kalos1a_0, Kalos1a_1, Kalos1a_2, Kalos1a_3, Kalos1a_4, Kalos1a_5, Kalos1a_6, Kalos1a_7,
                                   Kalos1b_0, Kalos1b_1, Kalos1b_2, Kalos1b_3, Kalos1b_4, Kalos1b_5, Kalos1b_6, Kalos1b_7,
                                   Kalos2a_0, Kalos2a_1, Kalos2a_2, Kalos2a_3, Kalos2a_4, Kalos2a_5, Kalos2a_6, Kalos2a_7,
                                   Kalos2b_0, Kalos2b_1, Kalos2b_2, Kalos2b_3, Kalos2b_4, Kalos2b_5, Kalos2b_6, Kalos2b_7,
                                   Extra1_0,  Extra1_1,  Extra1_2,  Extra1_3,  Extra1_4,

                                   Extra1_7, ORAS_0, ORAS_1, ORAS_2, ORAS_3, ORAS_4, ORAS_5, 
                             };

            Bitmap[] bma = {
                                   Properties.Resources.kaloschamp, Properties.Resources.hoennchamp,        Properties.Resources.sinnohchamp,   Properties.Resources.bestfriends,
                                   Properties.Resources.training,   Properties.Resources.skillfullbattler,  Properties.Resources.expertbattler, Properties.Resources.effort,

                                   Properties.Resources.alert,      Properties.Resources.shock,             Properties.Resources.downcast,      Properties.Resources.careless,
                                   Properties.Resources.relax,      Properties.Resources.snooze,            Properties.Resources.smile,         Properties.Resources.gorgeous,

                                   Properties.Resources.royal,      Properties.Resources.gorgeousroyal,     Properties.Resources.artist,        Properties.Resources.footprint,
                                   Properties.Resources.record,     Properties.Resources.legend,            Properties.Resources.country,       Properties.Resources.national,

                                   Properties.Resources.earth,      Properties.Resources.world,             Properties.Resources.classic,       Properties.Resources.premier,
                                   Properties.Resources._event,     Properties.Resources.birthday,          Properties.Resources.special,       Properties.Resources.souvenir,

                                   Properties.Resources.wishing,    Properties.Resources.battlechamp,       Properties.Resources.regionalchamp, Properties.Resources.nationalchamp,
                                   Properties.Resources.worldchamp,

                                   // ORAS
                                   Properties.Resources.ribbon_40,Properties.Resources.ribbon_41,Properties.Resources.ribbon_42,Properties.Resources.ribbon_43,
                                   Properties.Resources.ribbon_44,Properties.Resources.ribbon_45,Properties.Resources.ribbon_46,
                           };
            int index = Array.IndexOf(cba, sender as CheckBox);


            pba[index].Image = Util.ChangeOpacity(bma[index], (float)(Convert.ToInt32(cba[index].Checked)) * 0.9 + 0.1);
        }
        private void pastribbontoggle(object sender, EventArgs e)
        {
            PictureBox[] pba2 = {
                                    PastContest, PastBattle,
                                };

            if (Util.ToUInt32(TB_PastContest.Text) < 40)
                pba2[0].Image = Properties.Resources.contestmemory;
            else
                pba2[0].Image = Properties.Resources.contestmemory2;

            if (Util.ToUInt32(TB_PastBattle.Text) < 8)
                pba2[1].Image = Properties.Resources.battlememory;
            else
                pba2[1].Image = Properties.Resources.battlememory2;

            pba2[0].Image = Util.ChangeOpacity(pba2[0].Image, (float)(Util.ToUInt32(TB_PastContest.Text)) * 0.9 + 0.1);
            pba2[1].Image = Util.ChangeOpacity(pba2[1].Image, (float)(Util.ToUInt32(TB_PastBattle.Text)) * 0.9 + 0.1);
        }

        private void clickRibbon(object sender, EventArgs e)
        {
            PictureBox[] pba = { 
                                   PB_10, PB_11, PB_12, PB_13, 
                                   PB_14, PB_15, PB_16, PB_17,

                                   PB_20, PB_21, PB_22, PB_23, 
                                   PB_24, PB_25, PB_26, PB_27,

                                   PB_30, PB_31, PB_32, PB_33, 
                                   PB_34, PB_35, PB_36, PB_37,  

                                   PB_40, PB_41, PB_42, PB_43, 
                                   PB_44, PB_45, PB_46, PB_47,

                                   PB_50, PB_51, PB_52, PB_53, 
                                   PB_54,               PB_57,

                                   PB_O0, PB_O1, PB_O2, PB_O3, PB_O4, PB_O5,                                    
                               };
            CheckBox[] cba = {
                                 Kalos1a_0,Kalos1a_1,Kalos1a_2,Kalos1a_3,
                                 Kalos1a_4,Kalos1a_5,Kalos1a_6,Kalos1a_7,

                                 Kalos1b_0,Kalos1b_1,Kalos1b_2,Kalos1b_3,
                                 Kalos1b_4,Kalos1b_5,Kalos1b_6,Kalos1b_7,

                                 Kalos2a_0,Kalos2a_1,Kalos2a_2,Kalos2a_3,
                                 Kalos2a_4,Kalos2a_5,Kalos2a_6,Kalos2a_7,

                                 Kalos2b_0,Kalos2b_1,Kalos2b_2,Kalos2b_3,
                                 Kalos2b_4,Kalos2b_5,Kalos2b_6,Kalos2b_7,

                                 Extra1_0,Extra1_1,Extra1_2,Extra1_3,Extra1_4,

                                 Extra1_7, ORAS_0, ORAS_1, ORAS_2, ORAS_3, ORAS_4, ORAS_5, 
                             };

            CheckBox cb = cba[Array.IndexOf(pba, sender as PictureBox)];
            cb.Checked = !cb.Checked;
        }             
    }
}
