using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class TechRecordEditor : Form
    {
        private readonly PK8 pkm;

        public TechRecordEditor(PKM pk)
        {
            pkm = (PK8)pk;
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

            PopulateRecords();
            LoadRecords();
        }

        private void PopulateRecords()
        {
            var trs = Legal.TMHM_SWSH;
            var names = GameInfo.Strings.Move;
            var lines = trs.Skip(100).Select((z, i) => $"{i:00} - {names[z]}").ToArray();
            CLB_Flags.Items.AddRange(lines);
        }

        private void B_Cancel_Click(object sender, EventArgs e) => Close();

        private void B_Save_Click(object sender, EventArgs e)
        {
            Save();
            Close();
        }

        private void LoadRecords()
        {
            for (int i = 0; i < 100; i++)
                CLB_Flags.SetItemChecked(i, pkm.GetMoveRecordFlag(i));
        }

        private void Save()
        {
            for (int i = 0; i < 100; i++)
                pkm.SetMoveRecordFlag(i, CLB_Flags.GetItemChecked(i));
        }

        private void B_All_Click(object sender, EventArgs e)
        {
            Save();
            if (ModifierKeys == Keys.Shift)
                pkm.SetRecordFlags(true);
            else if (ModifierKeys == Keys.Control)
                pkm.SetRecordFlags(pkm.Moves);
            else
                pkm.SetRecordFlags();
            LoadRecords();
            Close();
        }

        private void B_None_Click(object sender, EventArgs e)
        {
            Save();
            pkm.ClearRecordFlags();
            LoadRecords();
            Close();
        }
    }
}
