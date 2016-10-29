using System;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_Pokepuff : Form
    {
        public SAV_Pokepuff()
        {
            InitializeComponent();
            Util.TranslateInterface(this, Main.curlanguage);

            pfa = Main.GameStrings.puffs;
            pfa[0] = "---";
            Setup();

            new ToolTip().SetToolTip(B_Sort, "Hold CTRL to reverse sort.");
            new ToolTip().SetToolTip(B_All, "Hold CTRL to best instead of varied.");
        }

        private readonly SAV6 SAV = new SAV6(Main.SAV.Data);
        private readonly string[] pfa;
        private void Setup()
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();

            DataGridViewColumn dgvIndex = new DataGridViewTextBoxColumn();
            {
                dgvIndex.HeaderText = "Slot";
                dgvIndex.DisplayIndex = 0;
                dgvIndex.Width = 45;
                dgvIndex.ReadOnly = true;
                dgvIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            DataGridViewComboBoxColumn dgvPuff = new DataGridViewComboBoxColumn
            {
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            {
                foreach (string t in pfa)
                    dgvPuff.Items.Add(t);

                dgvPuff.DisplayIndex = 1;
                dgvPuff.Width = 135;
                dgvPuff.FlatStyle = FlatStyle.Flat;
            }
            dgv.Columns.Add(dgvIndex);
            dgv.Columns.Add(dgvPuff);

            var Puffs = SAV.Puffs;
            dgv.Rows.Add(Puffs.Length);
            for (int i = 0; i < Puffs.Length; i++)
            {
                dgv.Rows[i].Cells[0].Value = (i + 1).ToString();
                int puffval = Puffs[i];
                if (puffval >= pfa.Length)
                {
                    Util.Error($"Invalid Puff Index: {i}", $"Expected < ${pfa.Length}");
                    puffval = 0;
                }
                dgv.Rows[i].Cells[1].Value = pfa[puffval];
            }
            MT_CNT.Text = SAV.PuffCount.ToString();
        }
        private void dropclick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 1) return;
            ((ComboBox)((DataGridView) sender).EditingControl).DroppedDown = true;
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_All_Click(object sender, EventArgs e)
        {
            int[] plus10 = {21, 22};
            byte[] newpuffs = new byte[100];

            if (ModifierKeys == Keys.Control)
                for (int i = 0; i < 100; i++)
                    newpuffs[i] = (byte)plus10[Util.rnd32() & 1];
            else
            {
                for (int i = 0; i < 100; i++)
                    newpuffs[i] = (byte)(i % (pfa.Length - 1) + 1);
                Util.Shuffle(newpuffs);
            }

            Array.Copy(newpuffs, 0, SAV.Data, SAV.Puff, 100);
            Setup();
        }
        private void B_None_Click(object sender, EventArgs e)
        {
            byte[] newpuffs = new byte[100];
            newpuffs[0] = 1;
            newpuffs[1] = 2;
            newpuffs[2] = 3;
            newpuffs[3] = 4;
            newpuffs[4] = 5;
            Array.Copy(newpuffs, 0, SAV.Data, SAV.Puff, 100);
            Setup();
        }
        private void B_Sort_Click(object sender, EventArgs e)
        {
            byte[] puffarray = new byte[100];
            if (ModifierKeys == Keys.Control)
            {
                for (int i = 0; i < 100; i++)
                {
                    string puff = dgv.Rows[i].Cells[1].Value.ToString();
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
                    string puff = dgv.Rows[i].Cells[1].Value.ToString();
                    byte puffval = (byte)Array.IndexOf(pfa, puff);
                    if (puffval == 0) continue;
                    puffarray[count] = puffval;
                    count++;
                }
                Array.Resize(ref puffarray, count);
                Array.Sort(puffarray);
                Array.Resize(ref puffarray, 100);
            }
            Array.Copy(puffarray, 0, SAV.Data, SAV.Puff, 100);
            Setup();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            byte[] puffarray = new byte[100];
            int emptyslots = 0;
            for (int i = 0; i < 100; i++)
            {
                string puff = dgv.Rows[i].Cells[1].Value.ToString();
                if (Array.IndexOf(pfa, puff) == 0)
                {
                    emptyslots++;
                    continue;
                }
                puffarray[i - emptyslots] = (byte)Array.IndexOf(pfa, puff);
            }
            SAV.Puffs = puffarray;
            SAV.PuffCount = Util.ToInt32(MT_CNT.Text);
            Main.SAV.Data = SAV.Data;
            Main.SAV.Edited = true;
            Close();
        }
    }
}
