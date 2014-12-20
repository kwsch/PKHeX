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
    public partial class SAV_Pokepuff : Form
    {
        public SAV_Pokepuff(Form1 frm1)
        {
            InitializeComponent();
            Util.TranslateInterface(this, Form1.curlanguage);
            m_parent = frm1;
            Array.Copy(m_parent.savefile, sav, 0x100000);
            pfa = Form1.puffs;
            savindex = m_parent.savindex;
            pfa[0] = "---";
            setup();

            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.SetToolTip(this.B_Sort, "Hold CTRL to reverse sort.");

            ToolTip ToolTip2 = new ToolTip();
            ToolTip2.SetToolTip(this.B_All, "Hold CTRL to give Deluxe instead of Supreme.");
        }
        Form1 m_parent;
        public byte[] sav = new byte[0x100000];
        public int savindex;
        public bool editing = false;
        private string[] pfa = {
                                "Empty",
                                "Basic Sweet","Basic Mint","Basic Citrus","Basic Mocha","Basic Spice",
                                "Frosted Sweet","Frosted Mint","Frosted Citrus","Frosted Mocha","Frosted Spice",
                                "Fancy Sweet","Fancy Mint","Fancy Citrus","Fancy Mocha","Fancy Spice",
                                "Deluxe Sweet","Deluxe Mint","Deluxe Citrus","Deluxe Mocha","Deluxe Spice",
                                "Supreme Wish","Supreme Honor","Supreme Spring","Supreme Summer","Supreme Fall","Supreme Winter",
                            };
        private void setup()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            

            DataGridViewColumn dgvIndex = new DataGridViewTextBoxColumn();
            {
                dgvIndex.HeaderText = "Slot";
                dgvIndex.DisplayIndex = 0;
                dgvIndex.Width = 45;
                dgvIndex.ReadOnly = true;
                dgvIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            DataGridViewComboBoxColumn dgvPuff = new DataGridViewComboBoxColumn();
            dgvPuff.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            {
                for (int i = 0; i < pfa.Length; i++)
                {
                    dgvPuff.Items.Add(pfa[i]);
                }
                dgvPuff.DisplayIndex = 1;
                dgvPuff.Width = 135;
                dgvPuff.FlatStyle = FlatStyle.Flat;
            }
            dataGridView1.Columns.Add(dgvIndex);
            dataGridView1.Columns.Add(dgvPuff);

            dataGridView1.Rows.Add(100);
            int offset = 0x5400 + 0x7F000 * savindex;
            for (int i = 0; i < 100; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = (i + 1).ToString();
                dataGridView1.Rows[i].Cells[1].Value = pfa[sav[offset+i]];
            }
            MT_CNT.Text = BitConverter.ToUInt32(sav, offset + 100).ToString("0");
        }
        private void dropclick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                ComboBox comboBox = (ComboBox)dataGridView1.EditingControl;
                comboBox.DroppedDown = true;
            }
        }
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_All_Click(object sender, EventArgs e)
        {
            int basepuff = 20;
            int basemod = 6;
            if (ModifierKeys == Keys.Control)
            {
                basepuff = 1;
                basemod = 0x19;
            }
            byte[] newpuffs = new byte[100];
            for (int i = 0; i < 100; i++)
            {
                newpuffs[i] = (byte)(Util.rnd32() % basemod + basepuff);
            }
            Array.Copy(newpuffs, 0, sav, 0x5400 + savindex * 0x7F000, 100);
            setup();
        }
        private void B_None_Click(object sender, EventArgs e)
        {
            byte[] newpuffs = new byte[100];
            newpuffs[0] = 1;
            newpuffs[1] = 2;
            newpuffs[2] = 3;
            newpuffs[3] = 4;
            newpuffs[4] = 5;
            Array.Copy(newpuffs, 0, sav, 0x5400 + savindex * 0x7F000, 100);
            setup();
        }
        private void B_Sort_Click(object sender, EventArgs e)
        {
            byte[] puffarray = new byte[100];
            if (ModifierKeys == Keys.Control)
            {
                for (int i = 0; i < 100; i++)
                {
                    string puff = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    puffarray[i] = (byte)Array.IndexOf(pfa, puff);
                }
                Array.Sort(puffarray);
                Array.Reverse(puffarray);
            }
            else
            {
                int count = 0;
                for (int i = 0; i < 100; i++)
                {
                    string puff = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    byte puffval = (byte)Array.IndexOf(pfa, puff);
                    if (puffval != 0)
                    {
                        puffarray[count] = puffval;
                        count++;
                    }
                }
                Array.Resize(ref puffarray, count);
                Array.Sort(puffarray);
                Array.Resize(ref puffarray, 100);
            }
            Array.Copy(puffarray, 0, sav, 0x5400 + savindex * 0x7F000, 100);
            setup();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            byte[] puffarray = new byte[100];
            int emptyslots = 0;
            for (int i = 0; i < 100; i++)
            {
                string puff = dataGridView1.Rows[i].Cells[1].Value.ToString();
                if (Array.IndexOf(pfa, puff) == 0)
                {
                    emptyslots++;
                    continue;
                }
                puffarray[i-emptyslots] = (byte)Array.IndexOf(pfa, puff);
            }
            Array.Copy(puffarray, 0, sav, 0x5400 + savindex * 0x7F000, 100);
            Array.Copy(sav, m_parent.savefile, 0x100000);
            m_parent.savedited = true;
            Close();
        }
    }
}
