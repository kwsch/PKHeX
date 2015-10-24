using System;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_SuperTrain : Form
    {
        public SAV_SuperTrain(Main frm1)
        {
            m_parent = frm1;
            sav = (byte[])Main.SAV.Data.Clone();
            trba = Main.trainingbags;
            trba[0] = "---";
            offsetTime = Main.SAV.SuperTrain + 0x08;
            offsetSpec = Main.SAV.SuperTrain + 0x188;
            offsetVal = Main.SAV.SuperTrain + 0x18A;
            InitializeComponent();
            Util.TranslateInterface(this, Main.curlanguage);
            string[] stages = Main.trainingstage;
            listBox1.Items.Clear();
            for (int i = 0; i < 30; i++)
                listBox1.Items.Add((i + 1).ToString("00") + " - " + stages[i + 2]);

            setup();
        }
        Main m_parent;
        public byte[] sav;
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
        private int offsetVal;
        private int offsetTime;
        private int offsetSpec;
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
            DataGridViewComboBoxColumn dgvBag = new DataGridViewComboBoxColumn
            {
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            {
                foreach (string t in trba)
                    if (t.Length > 0)
                        dgvBag.Items.Add(t);

                dgvBag.DisplayIndex = 1;
                dgvBag.Width = 135;
                dgvBag.FlatStyle = FlatStyle.Flat;
            }
            dataGridView1.Columns.Add(dgvIndex);
            dataGridView1.Columns.Add(dgvBag);

            dataGridView1.Rows.Add(12);
            int offset = Main.SAV.SuperTrain + 0x308;
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
                if (e.ColumnIndex != 1) return;
                ComboBox comboBox = (ComboBox)dataGridView1.EditingControl;
                comboBox.DroppedDown = true;
            }
            catch { }
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
            int offsetTime = Main.SAV.SuperTrain + 0x10;
            try
            {
                byte[] data = BitConverter.GetBytes(Single.Parse(TB_Time1.Text));
                Array.Resize(ref data, 4);
                Array.Copy(data, 0, sav, offsetTime + 4 * 30, 4);
            }
            catch { }
            try 
            {
                byte[] data = BitConverter.GetBytes(Single.Parse(TB_Time2.Text));
                Array.Resize(ref data, 4);
                Array.Copy(data, 0, sav, offsetTime + 4 * 31, 4);
            }
            catch { }
            {
                int offsetSpec = Main.SAV.SuperTrain + 0x188;
                byte[] data = BitConverter.GetBytes(Convert.ToUInt16(CB_S2.SelectedValue.ToString()));
                Array.Resize(ref data, 2);
                Array.Copy(data, 0, sav, offsetSpec + 4 * 30, 2);
            }
            Array.Copy(bagarray, 0, sav, Main.SAV.SuperTrain + 0x308, 12);
            Array.Copy(sav, Main.SAV.Data, Main.SAV.Data.Length);
            Main.savedited = true;
            Close();
        }
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
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
            catch { }
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
            catch { }
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
            catch { }
        }
    }
}

