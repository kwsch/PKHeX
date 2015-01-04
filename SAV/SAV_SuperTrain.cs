using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_SuperTrain : Form
    {
        public SAV_SuperTrain(Form1 frm1)
        {
            m_parent = frm1;
            savindex = m_parent.savindex;
            specieslist = Form1.specieslist;
            Array.Copy(m_parent.savefile, sav, 0x100000);
            if (m_parent.savegame_oras) data_offset = 0x25600;
            trba = Form1.trainingbags;
            trba[0] = "---";
            offsetTime = data_offset + 0x08 + 0x7F000 * savindex;
            offsetSpec = data_offset + 0x188 + 0x7F000 * savindex;
            offsetVal = data_offset + 0x18A + 0x7F000 * savindex;
            InitializeComponent();
            Util.TranslateInterface(this, Form1.curlanguage);
            string[] stages = Form1.trainingstage;
            listBox1.Items.Clear();
            for (int i = 0; i < 30; i++)
                listBox1.Items.Add((i + 1).ToString("00") + " - " + stages[i + 2]);

            setup();
        }
        Form1 m_parent;
        public byte[] sav = new byte[0x100000];
        public string[] specieslist;
        public int savindex;
        private int data_offset = 0x24600;
        private string[] trba = {
                                "Empty",
                                "HP Bag S","HP Bag M","HP Bag L",
                                "ATK Bag S","ATK Bag M","ATK Bag L",
                                "DEF Bag S","DEF Bag M","DEF Bag L",
                                "SpA Bag S","SpA Bag M","SpA Bag L",
                                "SpD Bag S","SpD Bag M","SpD Bag L",
                                "Speed Bag S","Speed Bag M","Speed Bag L",
                                "Strength Bag","Toughen Up Bag","Swiftness Bag",
                                "Big-Shot Bag","Double-Up Bag","Team Flare Bag",
                                "Reset Bag","Soothing Bag",                              
                               };
        private int offsetVal = 0;
        private int offsetTime = 0;
        private int offsetSpec = 0;
        private void setup()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            {
                CB_Species.DisplayMember = "Text";
                CB_Species.ValueMember = "Value";
                CB_Species.DataSource = new BindingSource(m_parent.CB_Species.DataSource, null);

                CB_S2.DisplayMember = "Text";
                CB_S2.ValueMember = "Value";
                CB_S2.DataSource = new BindingSource(m_parent.CB_Species.DataSource, null);
            }
            listBox1.SelectedIndex = 0;
            fillTrainingBags();

            CB_S2.SelectedValue = (int)BitConverter.ToUInt16(sav, offsetSpec + 4 * 30);
            TB_Time1.Text = BitConverter.ToSingle(sav, offsetTime + 4 * 30).ToString();
            TB_Time2.Text = BitConverter.ToSingle(sav, offsetTime + 4 * 31).ToString();
        }
        private void fillTrainingBags()
        {
            DataGridViewColumn dgvIndex = new DataGridViewTextBoxColumn();
            {
                dgvIndex.HeaderText = "Slot";
                dgvIndex.DisplayIndex = 0;
                dgvIndex.Width = 25;
                dgvIndex.ReadOnly = true;
                dgvIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            DataGridViewComboBoxColumn dgvBag = new DataGridViewComboBoxColumn();
            dgvBag.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            {
                for (int i = 0; i < trba.Length; i++)
                    if (trba[i].Length > 0)
                        dgvBag.Items.Add(trba[i]);

                dgvBag.DisplayIndex = 1;
                dgvBag.Width = 135;
                dgvBag.FlatStyle = FlatStyle.Flat;
            }
            dataGridView1.Columns.Add(dgvIndex);
            dataGridView1.Columns.Add(dgvBag);

            dataGridView1.Rows.Add(12);
            int offset = data_offset + 0x308 + 0x7F000 * savindex;
            for (int i = 0; i < 12; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = (i + 1).ToString();
                dataGridView1.Rows[i].Cells[1].Value = trba[sav[offset + i]];
            }
        }        
        private void dropclick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1)
                {
                    ComboBox comboBox = (ComboBox)dataGridView1.EditingControl;
                    comboBox.DroppedDown = true;
                }
            }
            catch { return; }
        }
        private void changeListRecordSelection(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;

            TB_Time.Text = BitConverter.ToSingle(sav, offsetTime + 4 * index).ToString();
            TB_Unk.Text = BitConverter.ToUInt16(sav, offsetVal + 4 * index).ToString();
            CB_Species.SelectedValue = (int)BitConverter.ToUInt16(sav, offsetSpec + 4 * index);
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            // Copy Bags
            byte[] bagarray = new byte[12];
            int emptyslots = 0;
            for (int i = 0; i < 12; i++)
            {
                string bag = dataGridView1.Rows[i].Cells[1].Value.ToString();
                if (Array.IndexOf(trba, bag) == 0)
                {
                    emptyslots++;
                    continue;
                }
                bagarray[i - emptyslots] = (byte)Array.IndexOf(trba, bag);
            }
            int offsetTime = data_offset + 0x10 + 0x7F000 * savindex;
            try
            {
                byte[] data = BitConverter.GetBytes(Single.Parse(TB_Time1.Text));
                Array.Resize(ref data, 4);
                Array.Copy(data, 0, sav, offsetTime + 4 * 30, 4);
            }
            catch { };
            try 
            {
                byte[] data = BitConverter.GetBytes(Single.Parse(TB_Time2.Text));
                Array.Resize(ref data, 4);
                Array.Copy(data, 0, sav, offsetTime + 4 * 31, 4);
            }
            catch { };
            {
                int offsetSpec = data_offset + 0x188 + 0x7F000 * savindex;
                byte[] data = BitConverter.GetBytes(Convert.ToUInt16(CB_S2.SelectedValue.ToString()));
                Array.Resize(ref data, 2);
                Array.Copy(data, 0, sav, offsetSpec + 4 * 30, 2);
            }
            Array.Copy(bagarray, 0, sav, data_offset + 0x308 + savindex * 0x7F000, 12);
            Array.Copy(sav, m_parent.savefile, 0x100000);
            m_parent.savedited = true;
            this.Close();
        }
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void changeRecordSpecies(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index < 0) return;
            try
            {
                byte[] data = BitConverter.GetBytes(Convert.ToUInt16(CB_Species.SelectedValue.ToString())); 
                Array.Resize(ref data, 2);
                Array.Copy(data, 0, sav, offsetSpec + 4 * index, 2);
            }
            catch { return; }
        }
        private void changeRecordVal(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index < 0) return;
            try
            {
                byte[] data = BitConverter.GetBytes(UInt16.Parse(TB_Unk.Text)); 
                Array.Resize(ref data, 2);
                Array.Copy(data, 0, sav, offsetVal + 4 * index, 2);
            }
            catch { return; }
        }
        private void changeRecordTime(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index < 0) return;
            try
            {
                byte[] data = BitConverter.GetBytes(Single.Parse(TB_Time.Text)); 
                Array.Resize(ref data, 4);
                Array.Copy(data, 0, sav, offsetTime + 4 * index, 4);
            }
            catch { return; }
        }
    }
}

