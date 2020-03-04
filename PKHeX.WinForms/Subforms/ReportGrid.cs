using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms
{
    public partial class ReportGrid : Form
    {
        public ReportGrid()
        {
            InitializeComponent();
            dgData.DoubleBuffered(true);
            CenterToParent();
            GetContextMenu();
        }

        private void GetContextMenu()
        {
            var mnuHide = new ToolStripMenuItem { Name = "mnuHide", Text = MsgReportColumnHide, };
            mnuHide.Click += (sender, e) =>
            {
                int c = dgData.SelectedCells.Count;
                if (c == 0)
                { WinFormsUtil.Alert(MsgReportColumnHideFail); return; }

                for (int i = 0; i < c; i++)
                    dgData.Columns[dgData.SelectedCells[i].ColumnIndex].Visible = false;
            };
            var mnuRestore = new ToolStripMenuItem { Name = "mnuRestore", Text = MsgReportColumnRestore, };
            mnuRestore.Click += (sender, e) =>
            {
                int c = dgData.ColumnCount;
                for (int i = 0; i < c; i++)
                    dgData.Columns[i].Visible = true;

                WinFormsUtil.Alert(MsgReportColumnRestoreSuccess);
            };

            ContextMenuStrip mnu = new ContextMenuStrip();
            mnu.Items.Add(mnuHide);
            mnu.Items.Add(mnuRestore);

            dgData.ContextMenuStrip = mnu;
        }

        private sealed class PokemonList<T> : SortableBindingList<T> { }

        public void PopulateData(IList<PKM> Data)
        {
            SuspendLayout();
            BoxBar.Step = 1;
            var PL = new PokemonList<PKMSummaryImage>();
            var strings = GameInfo.Strings;
            foreach (PKM pkm in Data.Where(pkm => pkm.ChecksumValid && pkm.Species != 0))
            {
                pkm.Stat_Level = Experience.GetLevel(pkm.EXP, pkm.PersonalInfo.EXPGrowth); // recalc Level
                PL.Add(new PKMSummaryImage(pkm, strings));
                BoxBar.PerformStep();
            }

            dgData.DataSource = PL;
            dgData.AutoGenerateColumns = true;
            BoxBar.Maximum = Data.Count + dgData.Columns.Count;
            for (int i = 0; i < dgData.Columns.Count; i++)
            {
                BoxBar.PerformStep();
                if (dgData.Columns[i] is DataGridViewImageColumn) continue; // Don't add sorting for Sprites
                dgData.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
            }
            BoxBar.Visible = false;

            // Trigger Resizing
            dgData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            for (int i = 0; i < dgData.Columns.Count; i++)
            {
                int w = dgData.Columns[i].Width;
                dgData.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgData.Columns[i].Width = w;
            }
            dgData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            Data_Sorted(null, EventArgs.Empty); // trigger row resizing

            ResumeLayout();
        }

        private void Data_Sorted(object sender, EventArgs e)
        {
            int height = SpriteUtil.GetSprite(1, 0, 0, 0, 0, false, false).Height + 1; // dummy sprite, max height of a row
            for (int i = 0; i < dgData.Rows.Count; i++)
                dgData.Rows[i].Height = height;
        }

        private void PromptSaveCSV(object sender, FormClosingEventArgs e)
        {
            if (WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgReportExportCSV) != DialogResult.Yes)
                return;
            using var savecsv = new SaveFileDialog
            {
                Filter = "Spreadsheet|*.csv",
                FileName = "Box Data Dump.csv"
            };
            if (savecsv.ShowDialog() == DialogResult.OK)
                Export_CSV(savecsv.FileName);
        }

        private void Export_CSV(string path)
        {
            var sb = new StringBuilder();

            var headers = dgData.Columns.Cast<DataGridViewColumn>();
            sb.AppendLine(string.Join(",", headers.Select(column => $"\"{column.HeaderText}\"")));

            foreach (var cells in from DataGridViewRow row in dgData.Rows select row.Cells.Cast<DataGridViewCell>())
                sb.AppendLine(string.Join(",", cells.Select(cell => $"\"{cell.Value}\"")));

            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool cp = keyData == (Keys.Control | Keys.C) && ActiveControl is DataGridView;
            if (!cp)
                return base.ProcessCmdKey(ref msg, keyData);

            string data = dgData.GetClipboardContent().GetText();
            var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgReportExportTable);
            if (dr != DialogResult.Yes)
            {
                WinFormsUtil.SetClipboardText(data);
                return true;
            }

            // Reformat datagrid clipboard content
            string[] lines = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            string[] newlines = ConvertTabbedToRedditTable(lines);
            WinFormsUtil.SetClipboardText(string.Join(Environment.NewLine, newlines));
            return true;
        }

        private static string[] ConvertTabbedToRedditTable(string[] lines)
        {
            string[] newlines = new string[lines.Length + 1];
            int tabcount = lines[0].Count(c => c == '\t');

            newlines[0] = lines[0].Replace('\t', '|');
            newlines[1] = string.Join(":--:", new int[tabcount + 2].Select(_ => '|')); // 2 pipes for each end
            for (int i = 1; i < lines.Length; i++)
                newlines[i + 1] = lines[i].Replace('\t', '|');
            return newlines;
        }
    }
}
