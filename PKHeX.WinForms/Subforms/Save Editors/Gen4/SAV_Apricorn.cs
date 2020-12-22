using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_Apricorn : Form
    {
        public SAV_Apricorn(SAV4HGSS sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV4HGSS)(Origin = sav).Clone();

            Setup();
        }

        private readonly SAV4HGSS Origin;
        private readonly SAV4HGSS SAV;
        private const int Count = 7;
        private static readonly string[] itemlist = {"Red", "Yellow", "Blue", "Green", "Pink", "White", "Black"};

        private void Setup()
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();

            DataGridViewColumn dgvApricorn = new DataGridViewTextBoxColumn();
            {
                dgvApricorn.HeaderText = "Slot";
                dgvApricorn.DisplayIndex = 0;
                dgvApricorn.Width = 135;
                dgvApricorn.ReadOnly = true;
                dgvApricorn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            DataGridViewComboBoxColumn dgvCount = new()
            {
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                DisplayIndex = 0,
                Width = 135,
                FlatStyle = FlatStyle.Flat,
                ValueType = typeof(int)
            };
            {
                for (var i = 0; i <= 99; i++)
                    dgvCount.Items.Add(i);

                dgvCount.DisplayIndex = 1;
                dgvCount.Width = 45;
                dgvCount.FlatStyle = FlatStyle.Flat;
            }
            dgv.Columns.Add(dgvApricorn);
            dgv.Columns.Add(dgvCount);

            dgv.Rows.Add(Count);
            for (int i = 0; i < Count; i++)
            {
                dgv.Rows[i].Cells[0].Value = itemlist[i];
                dgv.Rows[i].Cells[1].Value = SAV.GetApricornCount(i);
            }
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void B_All_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Count; i++)
                SAV.SetApricornCount(i, 99);
            Setup();
        }

        private void B_None_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Count; i++)
                SAV.SetApricornCount(i, 0);
            Setup();
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Count; i++)
                SAV.SetApricornCount(i, (int)dgv.Rows[i].Cells[1].Value);
            Origin.CopyChangesFrom(SAV);
            Close();
        }
    }
}
