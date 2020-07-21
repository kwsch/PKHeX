using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_Pokebean : Form
    {
        public SAV_Pokebean(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV7)(Origin = sav).Clone();

            InitializeGrid();
            Pouch = new BeanPouch(SAV);
            LoadValues();
        }

        private readonly BeanPouch Pouch;

        private readonly SaveFile Origin;
        private readonly SAV7 SAV;

        private void LoadValues()
        {
            dgv.Rows.Clear();

            var beans = Pouch.Beans;
            dgv.Rows.Add(beans.Length);
            for (int i = 0; i < beans.Length; i++)
            {
                dgv.Rows[i].Cells[0].Value = BeanPouch.BeanIndexNames[i];
                dgv.Rows[i].Cells[1].Value = beans[i];
            }
        }

        private void InitializeGrid()
        {
            var dgvBean = new DataGridViewTextBoxColumn
            {
                HeaderText = "Slot",
                DisplayIndex = 0,
                Width = 135,
                ReadOnly = true,
                DefaultCellStyle = {Alignment = DataGridViewContentAlignment.MiddleCenter}
            };
            var dgvCount = new DataGridViewComboBoxColumn
            {
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                FlatStyle = FlatStyle.Flat,
                ValueType = typeof(int),
                DisplayIndex = 1,
                Width = 45
            };
            for (var i = 0; i < 256; i++)
                dgvCount.Items.Add(i);

            dgv.Columns.Add(dgvBean);
            dgv.Columns.Add(dgvCount);
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
            Pouch.SetCountAll(255);
            LoadValues();
            System.Media.SystemSounds.Asterisk.Play();
        }

        private void B_None_Click(object sender, EventArgs e)
        {
            Pouch.SetCountAll(0);
            LoadValues();
            System.Media.SystemSounds.Asterisk.Play();
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            var beans = Pouch.Beans;
            for (int i = 0; i < beans.Length; i++)
                beans[i] = (int)dgv.Rows[i].Cells[1].Value;
            Pouch.Beans = beans;
            Origin.CopyChangesFrom(SAV);
            Close();
        }
    }
}
