using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class TechRecordEditor : Form
    {
        private readonly ITechRecord8 Entity;
        private readonly PKM pkm;

        public TechRecordEditor(ITechRecord8 techRecord8, PKM pk)
        {
            Entity = techRecord8;
            pkm = pk;
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

            PopulateRecords();
            LoadRecords();
        }

        private void PopulateRecords()
        {
            var names = GameInfo.Strings.Move;
            var indexes = Entity.TechRecordPermitIndexes;
            var lines = new string[indexes.Length];
            for (int i = 0; i < lines.Length; i++)
                lines[i] = $"{i:00} - {names[i]}";
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
            for (int i = 0; i < PersonalInfoSWSH.CountTR; i++)
                CLB_Flags.SetItemChecked(i, Entity.GetMoveRecordFlag(i));
        }

        private void Save()
        {
            for (int i = 0; i < PersonalInfoSWSH.CountTR; i++)
                Entity.SetMoveRecordFlag(i, CLB_Flags.GetItemChecked(i));
        }

        private void B_All_Click(object sender, EventArgs e)
        {
            Save();
            if (ModifierKeys == Keys.Shift)
                Entity.SetRecordFlags(true);
            else if (ModifierKeys == Keys.Control)
                Entity.SetRecordFlags(pkm.Moves);
            else
                Entity.SetRecordFlags();
            LoadRecords();
            Close();
        }

        private void B_None_Click(object sender, EventArgs e)
        {
            Save();
            Entity.ClearRecordFlags();
            LoadRecords();
            Close();
        }
    }
}
