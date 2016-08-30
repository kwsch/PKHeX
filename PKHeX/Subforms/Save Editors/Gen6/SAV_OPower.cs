using System;
using System.Linq;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_OPower : Form
    {
        public SAV_OPower()
        {
            InitializeComponent();
            Util.TranslateInterface(this, Main.curlanguage);
            LoadData();
        }

        private readonly SAV6 SAV = new SAV6(Main.SAV.Data);
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            SaveData();
            Close();
        }
        private void LoadData()
        {
            int o = SAV.OPower;

            // Fill up the 17 o-powers
            // 1 2 3 4 5 10 use 4 bytes, everything else uses 3
            o++; // Skip first 0
            CB_1.SelectedIndex = getIndex(o, 4); o += 4; o++; // @ 1
            CB_2.SelectedIndex = getIndex(o, 4); o += 4; o++; // @ 6
            CB_3.SelectedIndex = getIndex(o, 4); o += 4; o++; // @ B
            CB_4.SelectedIndex = getIndex(o, 4); o += 4; o++; // @ 10
            CB_5.SelectedIndex = getIndex(o, 4); o += 4; o++; // @ 15

            CB_6.SelectedIndex = getIndex(o, 3); o += 3; // 1A
            CB_7.SelectedIndex = getIndex(o, 3); o += 3; // 1D
            CB_8.SelectedIndex = getIndex(o, 3); o += 3; // 20
            CB_9.SelectedIndex = getIndex(o, 3); o += 3; // 23

            o++;
            CB_10.SelectedIndex = getIndex(o, 4); o += 4; o++; // @ 27-

            CB_11.SelectedIndex = getIndex(o, 3); o += 3; // 2C-2E
            CB_12.SelectedIndex = getIndex(o, 3); o += 3; // 2F-31
            CB_13.SelectedIndex = getIndex(o, 3); o += 3; // 32-34
            CB_14.SelectedIndex = getIndex(o, 3); o += 3; // 35-37
            CB_15.SelectedIndex = getIndex(o, 3); o += 3; // 38-3A
            CB_16.SelectedIndex = getIndex(o, 3); o += 3; // 3B-3D
            CB_17.SelectedIndex = getIndex(o, 3);//o += 3; // 3E-40

            // Load Maxes
            o = SAV.OPower;
            CHK_1.Checked = Convert.ToBoolean(SAV.Data[o + 0x00]);
            CHK_2.Checked = Convert.ToBoolean(SAV.Data[o + 0x05]);
            CHK_3.Checked = Convert.ToBoolean(SAV.Data[o + 0x0A]);
            CHK_4.Checked = Convert.ToBoolean(SAV.Data[o + 0x0F]);
            CHK_5.Checked = Convert.ToBoolean(SAV.Data[o + 0x14]);
            CHK_6.Checked = Convert.ToBoolean(SAV.Data[o + 0x19]);
            CHK_7.Checked = Convert.ToBoolean(SAV.Data[o + 0x26]);
            CHK_8.Checked = Convert.ToBoolean(SAV.Data[o + 0x2B]);
        }
        private void SaveData()
        {
            ComboBox[] cba =
            {
                CB_1, CB_2, CB_3, CB_4, CB_5, CB_6, CB_7, CB_8, CB_9,
                CB_10, CB_11, CB_12, CB_13, CB_14, CB_15, CB_16, CB_17,
            };
            int[] offsets =
            {
                1,6,0xB,0x10,0x15,
                0x1A,0x1D,0x20,0x23,
                0x27,
                0x2C,0x2F,0x32,0x35,0x38,0x3B,0x3E,
            };
            int o = SAV.OPower; // offset

            for (int i = 0; i < cba.Length; i++)
            {
                byte[] data = new byte[cba[i].Items.Count - 1];
                for (int c = 0; c < cba[i].SelectedIndex; c++)
                {
                    data[c] = 1;
                }
                Array.Copy(data, 0, SAV.Data, o + offsets[i], data.Length);
            }

            // Save Maxes

            SAV.Data[o + 0x00] = Convert.ToByte(CHK_1.Checked);
            SAV.Data[o + 0x05] = Convert.ToByte(CHK_2.Checked);
            SAV.Data[o + 0x0A] = Convert.ToByte(CHK_3.Checked);
            SAV.Data[o + 0x0F] = Convert.ToByte(CHK_4.Checked);
            SAV.Data[o + 0x14] = Convert.ToByte(CHK_5.Checked);
            SAV.Data[o + 0x19] = Convert.ToByte(CHK_6.Checked);
            SAV.Data[o + 0x26] = Convert.ToByte(CHK_7.Checked);
            SAV.Data[o + 0x2B] = Convert.ToByte(CHK_8.Checked);
            Array.Copy(SAV.Data, Main.SAV.Data, SAV.Data.Length);
            Main.SAV.Edited = true;
        }
        private int getIndex(int o, int l)
        {
            byte[] _0 = { 00, 00, 00, 00, };
            byte[] _1 = { 01, 00, 00, 00, };
            byte[] _2 = { 01, 01, 00, 00, };
            byte[] _3 = { 01, 01, 01, 00, };
            byte[] _4 = { 01, 01, 01, 01, };
            
            byte[] data = new byte[4];
            Array.Copy(Main.SAV.Data, o, data, 0, l);

            if (data.SequenceEqual(_4)) return 4;
            if (data.SequenceEqual(_3)) return 3;
            if (data.SequenceEqual(_2)) return 2;
            if (data.SequenceEqual(_1)) return 1;
            return data.SequenceEqual(_0) ? 0 : 1;
        }
        private void B_AllMax_Click(object sender, EventArgs e)
        {
            max(false);
        }
        private void B_MaxP_Click(object sender, EventArgs e)
        {
            max(true);
        }
        private void max(bool s)
        {
            ComboBox[] cba =
            {
                CB_1, CB_2, CB_3, CB_4, CB_5, CB_6, CB_7, CB_8, CB_9,
                CB_10, CB_11, CB_12, CB_13, CB_14, CB_15, CB_16, CB_17,
            };
            CheckBox[] echk = { CHK_1, CHK_2, CHK_3, CHK_4, CHK_5, CHK_6, CHK_7, CHK_8 };
            
            foreach (ComboBox t in cba)
                t.SelectedIndex = t.Items.Count-1;

            if (!s)
                foreach (CheckBox t in echk)
                    t.Checked = ModifierKeys != Keys.Control;
            else if (ModifierKeys == Keys.Control)
            {
                foreach (ComboBox t in cba)
                    t.SelectedIndex = 0;
                foreach (CheckBox t in echk)
                    t.Checked = false;
            }
        }
    }
}
