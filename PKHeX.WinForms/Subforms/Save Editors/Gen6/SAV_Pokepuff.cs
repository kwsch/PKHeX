using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_Pokepuff : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV6 SAV;
        public SAV_Pokepuff(SaveFile sav)
        {
            SAV = (SAV6)(Origin = sav).Clone();
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

            Setup();

            new ToolTip().SetToolTip(B_Sort, "Hold CTRL to reverse sort.");
            new ToolTip().SetToolTip(B_All, "Hold CTRL to best instead of varied.");
        }

        private readonly string[] pfa = GameInfo.Strings.puffs;
        private int PuffCount { get; set; }
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
            PuffCount = Puffs.Length;
            dgv.Rows.Add(Puffs.Length);
            for (int i = 0; i < Puffs.Length; i++)
            {
                dgv.Rows[i].Cells[0].Value = (i + 1).ToString();
                int puffval = Puffs[i];
                if (puffval >= pfa.Length)
                {
                    WinFormsUtil.Error($"Invalid Puff Index: {i}", $"Expected < ${pfa.Length}");
                    puffval = 0;
                }
                dgv.Rows[i].Cells[1].Value = pfa[puffval];
            }
            MT_CNT.Text = SAV.PuffCount.ToString();
        }

        private static void DropClick(object sender, DataGridViewCellEventArgs e)
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
            byte[] newpuffs = new byte[PuffCount];

            if (ModifierKeys == Keys.Control)
                for (int i = 0; i < PuffCount; i++)
                    newpuffs[i] = (byte)plus10[Util.Rand32() & 1];
            else
            {
                for (int i = 0; i < PuffCount; i++)
                    newpuffs[i] = (byte)(i % (pfa.Length - 1) + 1);
                Util.Shuffle(newpuffs);
            }

            Array.Copy(newpuffs, 0, SAV.Data, SAV.Puff, PuffCount);
            Setup();
        }
        private void B_None_Click(object sender, EventArgs e)
        {
            byte[] newpuffs = new byte[PuffCount];
            newpuffs[0] = 1;
            newpuffs[1] = 2;
            newpuffs[2] = 3;
            newpuffs[3] = 4;
            newpuffs[4] = 5;
            Array.Copy(newpuffs, 0, SAV.Data, SAV.Puff, PuffCount);
            Setup();
        }
        private void B_Sort_Click(object sender, EventArgs e)
        {
            var puffs = GetPuffs(false);
            Array.Sort(puffs);
            if (ModifierKeys == Keys.Control)
                Array.Reverse(puffs);

            Array.Copy(puffs, 0, SAV.Data, SAV.Puff, PuffCount);
            Setup();
        }

        private byte[] GetPuffs(bool resize = true)
        {
            List<byte> puffs = new List<byte>();
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                string puff = dgv.Rows[i].Cells[1].Value.ToString();
                int index = (byte)Array.IndexOf(pfa, puff);
                puffs.Add((byte)index);
            }
            var arr = puffs.ToArray();
            if (resize && arr.Length != PuffCount)
                Array.Resize(ref arr, PuffCount);
            return arr;
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            SAV.Puffs = GetPuffs();
            SAV.PuffCount = Util.ToInt32(MT_CNT.Text);
            Origin.SetData(SAV.Data, 0);
            Close();
        }
    }
}
