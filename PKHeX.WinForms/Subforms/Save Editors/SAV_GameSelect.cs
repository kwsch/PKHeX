using System;
using System.Collections;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_GameSelect : Form
    {
        public GameVersion Result = GameVersion.Invalid;
        public SAV_GameSelect(IEnumerable items)
        {
            InitializeComponent();
            CB_Game.DisplayMember = nameof(ComboItem.Text);
            CB_Game.ValueMember = nameof(ComboItem.Value);
            CB_Game.DataSource = new BindingSource(items, null);
            CB_Game.SelectedIndex = 0;
            CB_Game.Focus();
        }
        private void B_Cancel_Click(object sender, EventArgs e) => Close();
        private void B_OK_Click(object sender, EventArgs e)
        {
            Result = (GameVersion)WinFormsUtil.getIndex(CB_Game);
            Close();
        }
        private void SAV_GameSelect_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                B_OK_Click(null, null);
            if (e.KeyCode == Keys.Escape)
                B_Cancel_Click(null, null);
        }
    }
}
