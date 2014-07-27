using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace PKHeX
{
    public partial class SAV_Trainer : Form
    {
        public SAV_Trainer(Form1 frm1, string gamename)
        {
            Game = gamename;
            InitializeComponent();
            m_parent = frm1;
            Array.Copy(m_parent.savefile, sav, 0x100000);
            savindex = m_parent.savindex;
            getComboBoxes();
            getTextBoxes();
            getBadges();
        }
        string Game;
        Form1 m_parent;
        public byte[] sav = new Byte[0x100000];
        public int savindex;
        public bool editing = false;
        public byte badgeval = 0;
        public ToolTip Tip1 = new ToolTip();
        public ToolTip Tip2 = new ToolTip(); 

        private static uint ToUInt32(String value)
        {
            if (String.IsNullOrEmpty(value))
                return 0;
            try
            {
                return UInt32.Parse(value);
            }
            catch { return 0; }
        }
        private void getComboBoxes()
        {
            var dsregion_list = new[] {
                    new { Text = "NA/SA", Value = 1 },
                    new { Text = "EUR", Value = 2 },
                    new { Text = "JPN", Value = 0 },
                    new { Text = "CN", Value = 4 },
                    new { Text = "KOR", Value = 5 },
                    new { Text = "TW", Value = 6 }
                };


            var language_list = new[] {
                    new { Text = "ENG", Value = 2 },
                    new { Text = "JPN", Value = 1 },
                    new { Text = "FRE", Value = 3 },
                    new { Text = "ITA", Value = 4 },
                    new { Text = "GER", Value = 5 },
                    new { Text = "SPA", Value = 7 },
                    new { Text = "KOR", Value = 8 }
                };


            CB_3DSReg.DataSource = dsregion_list;
            CB_3DSReg.DisplayMember = "Text";
            CB_3DSReg.ValueMember = "Value";
            CB_Language.DataSource = language_list;
            CB_Language.DisplayMember = "Text";
            CB_Language.ValueMember = "Value";

            var subreg_list = new[] {
                    new { Text = "sr_0", Value = 0 },
                    new { Text = "sr_1", Value = 1 },
                    new { Text = "sr_2", Value = 2 },
                    new { Text = "sr_3", Value = 3 },
                    new { Text = "sr_4", Value = 4 },
                    new { Text = "sr_5", Value = 5 },
                    new { Text = "sr_6", Value = 6 },
                    new { Text = "sr_7", Value = 7 },
                    new { Text = "sr_8", Value = 8 },
                    new { Text = "sr_9", Value = 9 },
                    new { Text = "sr_10", Value = 10 },
                    new { Text = "sr_11", Value = 11 },
                    new { Text = "sr_12", Value = 12 },
                    new { Text = "sr_13", Value = 13 },
                    new { Text = "sr_14", Value = 14 },
                    new { Text = "sr_15", Value = 15 },
                    new { Text = "sr_16", Value = 16 },
                    new { Text = "sr_17", Value = 17 },
                    new { Text = "sr_18", Value = 18 },
                    new { Text = "sr_19", Value = 19 },
                    new { Text = "sr_20", Value = 20 },
                    new { Text = "sr_21", Value = 21 },
                    new { Text = "sr_22", Value = 22 },
                    new { Text = "sr_23", Value = 23 },
                    new { Text = "sr_24", Value = 24 },
                    new { Text = "sr_25", Value = 25 },
                    new { Text = "sr_26", Value = 26 },
                    new { Text = "sr_27", Value = 27 },
                    new { Text = "sr_28", Value = 28 },
                    new { Text = "sr_29", Value = 29 },
                    new { Text = "sr_30", Value = 30 },
                    new { Text = "sr_31", Value = 31 },
                    new { Text = "sr_32", Value = 32 },
                    new { Text = "sr_33", Value = 33 },
                    new { Text = "sr_34", Value = 34 },
                    new { Text = "sr_35", Value = 35 },
                    new { Text = "sr_36", Value = 36 },
                    new { Text = "sr_37", Value = 37 },
                    new { Text = "sr_38", Value = 38 },
                    new { Text = "sr_39", Value = 39 },
                    new { Text = "sr_40", Value = 40 },
                    new { Text = "sr_41", Value = 41 },
                    new { Text = "sr_42", Value = 42 },
                    new { Text = "sr_43", Value = 43 },
                    new { Text = "sr_44", Value = 44 },
                    new { Text = "sr_45", Value = 45 },
                    new { Text = "sr_46", Value = 46 },
                    new { Text = "sr_47", Value = 47 },
                    new { Text = "sr_48", Value = 48 },
                    new { Text = "sr_49", Value = 49 },
                    new { Text = "sr_50", Value = 50 },
                    new { Text = "sr_51", Value = 51 },
                    new { Text = "sr_52", Value = 52 },
                    new { Text = "sr_53", Value = 53 },
                    new { Text = "sr_54", Value = 54 },
                    new { Text = "sr_55", Value = 55 },
                    new { Text = "sr_56", Value = 56 },
                    new { Text = "sr_57", Value = 57 },
                    new { Text = "sr_58", Value = 58 },
                    new { Text = "sr_59", Value = 59 },
                    new { Text = "sr_60", Value = 60 },
                    new { Text = "sr_61", Value = 61 },
                    new { Text = "sr_62", Value = 62 },
                    new { Text = "sr_63", Value = 63 },
                    new { Text = "sr_64", Value = 64 },
                    new { Text = "sr_65", Value = 65 },
                    new { Text = "sr_66", Value = 66 },
                    new { Text = "sr_67", Value = 67 },
                    new { Text = "sr_68", Value = 68 },
                    new { Text = "sr_69", Value = 69 },
                    new { Text = "sr_70", Value = 70 },
                    new { Text = "sr_71", Value = 71 },
                    new { Text = "sr_72", Value = 72 },
                    new { Text = "sr_73", Value = 73 },
                    new { Text = "sr_74", Value = 74 },
                    new { Text = "sr_75", Value = 75 },
                    new { Text = "sr_76", Value = 76 },
                    new { Text = "sr_77", Value = 77 },
                    new { Text = "sr_78", Value = 78 },
                    new { Text = "sr_79", Value = 79 },
                    new { Text = "sr_80", Value = 80 },
               };
            CB_Region.DataSource = subreg_list;
            CB_Region.DisplayMember = "Text";
            CB_Region.ValueMember = "Value";

            m_parent.setcountry(CB_Country);

        }
        private void getBadges()
        {
            // Fetch Badges
            Bitmap[] bma = {
                                   Properties.Resources.badge_1, 
                                   Properties.Resources.badge_2,  
                                   Properties.Resources.badge_3,   
                                   Properties.Resources.badge_4,
                                   Properties.Resources.badge_5, 
                                   Properties.Resources.badge_6,  
                                   Properties.Resources.badge_7, 
                                   Properties.Resources.badge_8,
                           };
            if (Game == "ORAS")
            {
                bma = new Bitmap[] {
                                   Properties.Resources.rsBadge_1, // Add in ORAS badges.
                                   Properties.Resources.rsBadge_2,  
                                   Properties.Resources.rsBadge_3,   
                                   Properties.Resources.rsBadge_4,
                                   Properties.Resources.rsBadge_5, 
                                   Properties.Resources.rsBadge_6,  
                                   Properties.Resources.rsBadge_7, 
                                   Properties.Resources.rsBadge_8,
                       };
            }
            CheckBox[] cba = {cb1, cb2, cb3, cb4, cb5, cb6, cb7, cb8,};
            PictureBox[] pba = {pb1, pb2, pb3, pb4, pb5, pb6, pb7, pb8,};

            for (int i = 0; i < 8; i++)
            {
                if (!cba[i].Checked)
                {
                    pba[i].Image = ImageTransparency.ChangeOpacity(bma[i], 0.1);
                }
                else
                {
                    pba[i].Image = ImageTransparency.ChangeOpacity(bma[i], 1);
                }
            }
        }
        private void getTextBoxes()
        {
            int savshift = savindex * 0x7F000;
            byte badgeval = sav[0x960C + savindex * 0x7F000];
            CheckBox[] cba = { cb1, cb2, cb3, cb4, cb5, cb6, cb7, cb8, };
            for (int i = 0; i < 8; i++)
                cba[i].Checked = !((badgeval & (1 << i)) == 0);

            // Get Data
            string OT_NAME = Encoding.Unicode.GetString(sav, 0x19448 + savshift, 0x1A);
            string RIV_NAME = Encoding.Unicode.GetString(sav, 0x9610 + savshift, 0x1A);
            
            CB_Game.SelectedIndex = sav[0x19404 + savshift]-0x18;
            CB_Gender.SelectedIndex = sav[0x19405 + savshift];

            int TID = BitConverter.ToUInt16(sav, 0x19400 + savshift);
            int SID = BitConverter.ToUInt16(sav, 0x19402 + savshift);
            uint money = BitConverter.ToUInt32(sav, 0x9608 + savshift);

            string saying1 = Encoding.Unicode.GetString(sav, 0x1947C + savshift, 0x20);
            string saying2 = Encoding.Unicode.GetString(sav, 0x1949E + savshift, 0x20);
            string saying3 = Encoding.Unicode.GetString(sav, 0x194C0 + savshift, 0x20);
            string saying4 = Encoding.Unicode.GetString(sav, 0x194E2 + savshift, 0x20);
            string saying5 = Encoding.Unicode.GetString(sav, 0x19504 + savshift, 0x20);

            int _region   = sav[0x19426 + savshift];
            int _country  = sav[0x19427+savshift];
            int _3dsreg   = sav[0x1942C+savshift];
            int _language = sav[0x1942D+savshift];

            // Display Data
            TB_OTName.Text = OT_NAME;
            //TB_Rival.Text = RIV_NAME;

            MT_TID.Text = TID.ToString();
            MT_SID.Text = SID.ToString();
            MT_Money.Text = money.ToString();

            TB_Saying1.Text = saying1;
            TB_Saying2.Text = saying2;
            TB_Saying3.Text = saying3;
            TB_Saying4.Text = saying4;
            TB_Saying5.Text = saying5;

            CB_Country.SelectedValue = _country;
            CB_Region.SelectedValue = _region;

            CB_3DSReg.SelectedValue = _3dsreg;
            CB_Language.SelectedValue = _language;

            // Maison Data (Display Only)
            TB_MCSN.Text = BitConverter.ToUInt16(sav, savshift + 0x205C0 + 2 * 0).ToString();
            TB_MCSS.Text = BitConverter.ToUInt16(sav, savshift + 0x205C0 + 2 * 1).ToString();

            TB_MBSN.Text = BitConverter.ToUInt16(sav, savshift + 0x205C0 + 2 * 2).ToString();
            TB_MBSS.Text = BitConverter.ToUInt16(sav, savshift + 0x205C0 + 2 * 3).ToString();

            TB_MCDN.Text = BitConverter.ToUInt16(sav, savshift + 0x205C0 + 2 * 4).ToString();
            TB_MCDS.Text = BitConverter.ToUInt16(sav, savshift + 0x205C0 + 2 * 5).ToString();
            
            TB_MBDN.Text = BitConverter.ToUInt16(sav, savshift + 0x205C0 + 2 * 6).ToString();
            TB_MBDS.Text = BitConverter.ToUInt16(sav, savshift + 0x205C0 + 2 * 7).ToString();

            TB_MCTN.Text = BitConverter.ToUInt16(sav, savshift + 0x205C0 + 2 * 8).ToString();
            TB_MCTS.Text = BitConverter.ToUInt16(sav, savshift + 0x205C0 + 2 * 9).ToString();

            TB_MBTN.Text = BitConverter.ToUInt16(sav, savshift + 0x205C0 + 2 * 10).ToString();
            TB_MBTS.Text = BitConverter.ToUInt16(sav, savshift + 0x205C0 + 2 * 11).ToString();

            TB_MCRN.Text = BitConverter.ToUInt16(sav, savshift + 0x205C0 + 2 * 12).ToString();
            TB_MCRS.Text = BitConverter.ToUInt16(sav, savshift + 0x205C0 + 2 * 13).ToString();

            TB_MBRN.Text = BitConverter.ToUInt16(sav, savshift + 0x205C0 + 2 * 14).ToString();
            TB_MBRS.Text = BitConverter.ToUInt16(sav, savshift + 0x205C0 + 2 * 15).ToString();

            TB_MCMN.Text = BitConverter.ToUInt16(sav, savshift + 0x205C0 + 2 * 16).ToString();
            TB_MCMS.Text = BitConverter.ToUInt16(sav, savshift + 0x205C0 + 2 * 17).ToString();

            TB_MBMN.Text = BitConverter.ToUInt16(sav, savshift + 0x205C0 + 2 * 18).ToString();
            TB_MBMS.Text = BitConverter.ToUInt16(sav, savshift + 0x205C0 + 2 * 19).ToString();
                        
            TB_CoordM.Text = BitConverter.ToUInt16(sav, savshift + 0x6802).ToString("X4");
            TB_CoordX.Text = BitConverter.ToSingle(sav, savshift + 0x6810).ToString("0.0");
            TB_CoordZ.Text = BitConverter.ToSingle(sav, savshift + 0x6814).ToString("0.0");
            TB_CoordY.Text = BitConverter.ToSingle(sav, savshift + 0x6818).ToString("0.0");

            // Load BP and PokeMiles
            TB_BP.Text = BitConverter.ToUInt32(sav, savshift + 0x963C).ToString();
            TB_PM.Text = BitConverter.ToUInt32(sav, savshift + 0x238FC).ToString();

            TB_Style.Text = sav[0x694D + savshift].ToString();
        }
        private void save()
        {
            int savshift = savindex * 0x7F000;

            string OT_Name = TB_OTName.Text;
            //string RIV_Name = TB_Rival.Text;

            sav[0x19404 + savshift] = (byte)(CB_Game.SelectedIndex + 0x18);
            sav[0x19405 + savshift] = (byte)CB_Gender.SelectedIndex;

            uint TID = ToUInt32(MT_TID.Text);
            uint SID = ToUInt32(MT_SID.Text);
            uint money = ToUInt32(MT_Money.Text);

            string saying1 = TB_Saying1.Text;
            string saying2 = TB_Saying2.Text;
            string saying3 = TB_Saying3.Text;
            string saying4 = TB_Saying4.Text;
            string saying5 = TB_Saying5.Text;

            uint _region = ToUInt32(CB_Region.SelectedValue.ToString());
            uint _country   = ToUInt32(CB_Country.SelectedValue.ToString());
            uint _3dsreg    = ToUInt32(CB_3DSReg.SelectedValue.ToString());
            uint _language  = ToUInt32(CB_Language.SelectedValue.ToString());

            Array.Copy(BitConverter.GetBytes(TID), 0, sav, 0x19400 + savshift, 2);
            Array.Copy(BitConverter.GetBytes(SID), 0, sav, 0x19402 + savshift, 2);
            Array.Copy(BitConverter.GetBytes(money), 0, sav, 0x9608 + savshift, 4);
            Array.Copy(BitConverter.GetBytes(_region), 0, sav, 0x19426 + savshift, 1);
            Array.Copy(BitConverter.GetBytes(_country), 0, sav, 0x19427 + savshift, 1);
            Array.Copy(BitConverter.GetBytes(_3dsreg), 0, sav, 0x1942C + savshift, 1);
            Array.Copy(BitConverter.GetBytes(_language), 0, sav, 0x1942D + savshift, 1);

            byte[] OT = Encoding.Unicode.GetBytes(OT_Name);
            Array.Resize(ref OT, 0x1A);
            Array.Copy(OT, 0, sav, 0x19448 + savshift, 0x1A);
            //byte[] Rival = Encoding.Unicode.GetBytes(RIV_Name);
            //Array.Resize(ref Rival, 0x1A);
            //Array.Copy(Rival, 0, sav, 0x9610 + savshift, 0x1A);

            byte[] s1 = Encoding.Unicode.GetBytes(saying1);
            Array.Resize(ref s1, 0x22);
            Array.Copy(s1, 0, sav, 0x1947C + savshift, 0x22);
            byte[] s2 = Encoding.Unicode.GetBytes(saying2);
            Array.Resize(ref s2, 0x22);
            Array.Copy(s2, 0, sav, 0x1949E + savshift, 0x22);
            byte[] s3 = Encoding.Unicode.GetBytes(saying3);
            Array.Resize(ref s3, 0x22);
            Array.Copy(s3, 0, sav, 0x194C0 + savshift, 0x22);
            byte[] s4 = Encoding.Unicode.GetBytes(saying4);
            Array.Resize(ref s4, 0x22);
            Array.Copy(s4, 0, sav, 0x194E2 + savshift, 0x22);
            byte[] s5 = Encoding.Unicode.GetBytes(saying5);
            Array.Resize(ref s5, 0x22);
            Array.Copy(s5, 0, sav, 0x19504 + savshift, 0x22);

            // New stuff.
            // Copy Maison Data in
            TextBox[] tba = new TextBox[] {
                TB_MCSN,TB_MCSS,TB_MBSN,TB_MBSS,
                TB_MCDN,TB_MCDS,TB_MBDN,TB_MBDS,
                TB_MCTN,TB_MCTS,TB_MBTN,TB_MBTS,
                TB_MCRN,TB_MCRS,TB_MBRN,TB_MBRS,
                TB_MCMN,TB_MCMS,TB_MBMN,TB_MBMS,
            };
            for (int i = 0; i < tba.Length; i++)
            {
                byte[] streak = BitConverter.GetBytes(ToUInt32(tba[i].Text));
                Array.Resize(ref streak, 2);
                Array.Copy(streak, 0, sav, 0x205C0 + 2 * i + savshift, 2);
            }

            // Copy Position
            byte[] m = BitConverter.GetBytes( Int16.Parse(TB_CoordM.Text)); Array.Resize(ref m, 2); Array.Copy(m,0,sav, savshift + 0x6802,2);
            byte[] x = BitConverter.GetBytes(Single.Parse(TB_CoordX.Text)); Array.Resize(ref x, 4); Array.Copy(x,0,sav, savshift + 0x6810,4);
            byte[] z = BitConverter.GetBytes(Single.Parse(TB_CoordZ.Text)); Array.Resize(ref z, 4); Array.Copy(z,0,sav, savshift + 0x6814,4);
            byte[] y = BitConverter.GetBytes(Single.Parse(TB_CoordY.Text)); Array.Resize(ref y, 4); Array.Copy(y,0,sav, savshift + 0x6818,4);
            TB_BP.Text = BitConverter.ToUInt32(sav, savshift + 0x963C).ToString();
            TB_PM.Text = BitConverter.ToUInt32(sav, savshift + 0x238FC).ToString();
            
            byte[] bp = BitConverter.GetBytes(ToUInt32(TB_BP.Text)); Array.Resize(ref bp, 2); Array.Copy(bp, 0, sav, savshift + 0x963C, 2);
            byte[] pm = BitConverter.GetBytes(ToUInt32(TB_PM.Text)); Array.Resize(ref pm, 4); Array.Copy(pm, 0, sav, savshift + 0x238FC, 4); Array.Copy(pm, 0, sav, savshift + 0x23900, 4);
            sav[0x694D+savshift] = Byte.Parse(TB_Style.Text);

            // Copy Badges
            badgeval = 0;
            CheckBox[] cba = { cb1, cb2, cb3, cb4, cb5, cb6, cb7, cb8, };
            for (int i = 0; i < 8; i++)
            {
                badgeval |= (byte)(Convert.ToByte(cba[i].Checked) << i);
            }
            sav[0x960C + savindex * 0x7F000] = badgeval;
        }

        private void showTSV(object sender, EventArgs e)
        {
            uint TID = ToUInt32(MT_TID.Text);
            uint SID = ToUInt32(MT_SID.Text);
            uint tsv = (TID ^ SID) >> 4;
            Tip1.SetToolTip(this.MT_TID, "TSV: " + tsv.ToString("0000"));
            Tip2.SetToolTip(this.MT_SID, "TSV: " + tsv.ToString("0000"));
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            save();
            Array.Copy(sav, m_parent.savefile, 0x100000);
            m_parent.savedited = true;
            Close();
        }
        private void B_MaxCash_Click(object sender, EventArgs e)
        {
            MT_Money.Text = "9,999,999";
        }

        private void changeBadge(object sender, EventArgs e)
        {
            getBadges();
        }

        private void changeStyle(object sender, EventArgs e)
        {
            if (int.Parse(TB_Style.Text) > 255) TB_Style.Text = "255";
        }

    }

    
}
