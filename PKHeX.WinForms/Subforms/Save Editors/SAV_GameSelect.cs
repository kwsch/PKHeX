using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_GameSelect : Form
    {
        public GameVersion Result = GameVersion.Invalid;

        public SAV_GameSelect(IEnumerable<ComboItem> items, params string[] lines)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            L_Prompt.Text = string.Join(Environment.NewLine + Environment.NewLine, lines);
            CB_Game.InitializeBinding();
            CB_Game.DataSource = new BindingSource(items.ToList(), null);
            CB_Game.SelectedIndex = 0;
            CB_Game.Focus();
        }

        private void B_Cancel_Click(object sender, EventArgs e) => Close();

        private void B_OK_Click(object sender, EventArgs e)
        {
            Result = (GameVersion)WinFormsUtil.GetIndex(CB_Game);
            Close();
        }

        private void SAV_GameSelect_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                B_OK_Click(sender, EventArgs.Empty);
            if (e.KeyCode == Keys.Escape)
                B_Cancel_Click(sender, EventArgs.Empty);
        }
    }
}
