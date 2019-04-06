using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Properties;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms
{
    public partial class SAV_FolderList : Form
    {
        private readonly Action<SaveFile> OpenSaveFile;
        private readonly List<CustomFolderPath> Paths;
        private readonly SortableBindingList<SavePreview> Recent;
        private readonly SortableBindingList<SavePreview> Backup;
        private readonly List<Label> TempTranslationLabels = new List<Label>();

        public SAV_FolderList(Action<SaveFile> openSaveFile)
        {
            InitializeComponent();
            OpenSaveFile = openSaveFile;

            var drives = Environment.GetLogicalDrives();
            Paths = GetPathList(drives);

            dgDataRecent.ContextMenuStrip = GetContextMenu(dgDataRecent);
            dgDataBackup.ContextMenuStrip = GetContextMenu(dgDataBackup);

            var recent = SaveDetection.GetSaveFiles(drives, false, Paths.Select(z => z.Path).Where(z => z != Main.BackupPath));
            Recent = PopulateData(dgDataRecent, recent);
            var backup = SaveDetection.GetSaveFiles(drives, false, Main.BackupPath);
            Backup = PopulateData(dgDataBackup, backup);

            CB_FilterColumn.Items.Add(MsgAny);
            var dgv = Recent.Count >= 1 ? dgDataRecent : dgDataBackup;
            int count = dgv.ColumnCount;
            for (int i = 0; i < count; i++)
            {
                var text = dgv.Columns[i].HeaderText;
                CB_FilterColumn.Items.Add(text);
                var tempLabel = new Label {Name = "DGV_" + text, Text = text, Visible = false};
                Controls.Add(tempLabel);
                TempTranslationLabels.Add(tempLabel);
            }
            CB_FilterColumn.SelectedIndex = 0;

            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

            // Update Translated headers
            for (int i = 0; i < TempTranslationLabels.Count; i++)
            {
                var text = TempTranslationLabels[i].Text;
                if (i < dgDataRecent.ColumnCount)
                    dgDataRecent.Columns[i].HeaderText = text;
                if (i < dgDataBackup.ColumnCount)
                    dgDataBackup.Columns[i].HeaderText = text;
                CB_FilterColumn.Items[i+1] = text;
            }

            // Preprogrammed folders
            foreach (var loc in Paths)
                AddButton(loc.DisplayText, loc.Path);
            AddCustomizeButton();

            dgDataRecent.DoubleBuffered(true);
            dgDataBackup.DoubleBuffered(true);

            CenterToParent();
        }

        private static List<CustomFolderPath> GetPathList(IReadOnlyList<string> drives)
        {
            var locs = new List<CustomFolderPath>
            {
                new CustomFolderPath(Main.BackupPath, "PKHeX Backups")
            };
            locs.AddRange(GetUserPaths());
            locs.AddRange(GetConsolePaths(drives));
            locs.AddRange(GetSwitchPaths(drives));
            return locs.GroupBy(z => z.Path).Select(z => z.First())
                .OrderByDescending(z => Directory.Exists(z.Path)).ToList();
        }

        private const int ButtonHeight = 30;
        private const int ButtonWidth = 130;

        private void AddButton(string name, string path)
        {
            Button button = GetCustomButton(name);
            button.Enabled = new DirectoryInfo(path).Exists;
            button.Click += (s, e) =>
            {
                if (!Directory.Exists(path))
                {
                    WinFormsUtil.Alert(MsgFolderNotFound, path);
                    return;
                }
                Process.Start("explorer.exe", path);
                Close();
            };
            FLP_Buttons.Controls.Add(button);
        }

        private void AddCustomizeButton()
        {
            const string name = "Customize";
            Button button = GetCustomButton(name);
            button.Click += (s, e) =>
            {
                var loc = Main.SAVPaths;
                if (!File.Exists(loc))
                {
                    var custom = Paths.Where(z => z.Custom).ToList();
                    if (custom.Count == 0)
                        Paths.Add(new CustomFolderPath("DISPLAY_TEXT", "FOLDER_PATH", true));
                    var lines = custom.Select(z => z.Write());
                    File.WriteAllLines(loc, lines);
                }
                Process.Start(loc);
                Close();
            };
            FLP_Buttons.Controls.Add(button);
        }

        private static Button GetCustomButton(string name)
        {
            return new Button
            {
                Size = new Size { Height = ButtonHeight, Width = ButtonWidth },
                Text = name,
                Name = $"B_{name}",
            };
        }

        private static IEnumerable<CustomFolderPath> GetUserPaths()
        {
            string loc = Main.SAVPaths;
            if (!File.Exists(loc))
                return Enumerable.Empty<CustomFolderPath>();

            var lines = File.ReadLines(loc);
            return lines.Select(z => z.Split('\t'))
                .Where(a => a.Length == 2)
                .Select(x => new CustomFolderPath(x, true));
        }

        private static IEnumerable<CustomFolderPath> GetConsolePaths(IEnumerable<string> drives)
        {
            var path3DS = SaveDetection.Get3DSLocation(drives);
            if (path3DS == null)
                return Enumerable.Empty<CustomFolderPath>();
            var root = Path.GetPathRoot(path3DS);
            var paths = SaveDetection.Get3DSBackupPaths(root);
            return paths.Select(z => new CustomFolderPath(z));
        }

        private static IEnumerable<CustomFolderPath> GetSwitchPaths(IEnumerable<string> drives)
        {
            var pathNX = SaveDetection.GetSwitchLocation(drives);
            if (pathNX == null)
                return Enumerable.Empty<CustomFolderPath>();
            var root = Path.GetPathRoot(pathNX);
            var paths = SaveDetection.GetSwitchBackupPaths(root);
            return paths.Select(z => new CustomFolderPath(z));
        }

        private class CustomFolderPath
        {
            public readonly string Path;
            public readonly string DisplayText;
            public readonly bool Custom;

            public CustomFolderPath(string z, bool custom = false)
            {
                var di = new DirectoryInfo(z);
                var root = di.Root.Name;
                var folder = di.Parent.Name;
                if (root == folder)
                    folder = di.Name;

                Path = z;
                DisplayText = folder;
                Custom = custom;
            }

            public CustomFolderPath(IReadOnlyList<string> arr, bool custom = false)
            {
                Path = arr[1];
                DisplayText = arr[0];
                Custom = custom;
            }

            public CustomFolderPath(string path, string display, bool custom = false)
            {
                Path = path;
                DisplayText = display;
                Custom = custom;
            }

            public string Write() => $"{DisplayText}\t{Path}";
        }

        private string GetParentFolderName(SaveFile first)
        {
            var parent = Paths.Find(z => first.FileFolder.StartsWith(z.Path));
            return parent?.DisplayText ?? "???";
        }

        private sealed class SaveList<T> : SortableBindingList<T> { }

        private sealed class SavePreview
        {
            public readonly SaveFile Save;

            public SavePreview(SaveFile sav, string parent)
            {
                Save = sav;
                Folder = parent;
            }

            public string OT => Save.OT;
            public int G => Save.Generation;
            public GameVersion Game => Save.Version;

            public string Played => Save.PlayTimeString.PadLeft(9, '0');
            public string FileTime => File.GetLastWriteTimeUtc(Save.FilePath).ToString("yyyy.MM.dd:hh:mm:ss");

            public string TID => Save.Generation >= 7 ? Save.TrainerID7.ToString("000000") : Save.TID.ToString("00000");
            public string SID => Save.Generation >= 7 ? Save.TrainerSID7.ToString("0000") : Save.SID.ToString("00000");

            // ReSharper disable once MemberCanBePrivate.Local
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string Folder { get; }

            public string Name => Path.GetFileName(Save.FilePath);
        }

        private ContextMenuStrip GetContextMenu(DataGridView dgv)
        {
            var mnuOpen = new ToolStripMenuItem
            {
                Name = "mnuOpen",
                Text = "Open",
                Image = Resources.open,
            };
            mnuOpen.Click += (sender, e) => ClickOpenFile(dgv);

            var mnuBrowseAt = new ToolStripMenuItem
            {
                Name = "mnuBrowseAt",
                Text = "Browse...",
                Image = Resources.folder,
            };
            mnuBrowseAt.Click += (sender, e) => ClickOpenFolder(dgv);

            ContextMenuStrip mnu = new ContextMenuStrip();
            mnu.Items.Add(mnuOpen);
            mnu.Items.Add(mnuBrowseAt);
            return mnu;
        }

        private void ClickOpenFile(DataGridView dgv)
        {
            var sav = GetSaveFile(dgv);
            if (sav == null)
            {
                WinFormsUtil.Alert(MsgFileLoadFail);
                return;
            }

            OpenSaveFile(sav.Save);
        }

        private void ClickOpenFolder(DataGridView dgv)
        {
            var sav = GetSaveFile(dgv);
            if (sav == null)
            {
                WinFormsUtil.Alert(MsgFileLoadFail);
                return;
            }

            var path = sav.Save.FilePath;
            Process.Start("explorer.exe", $"/select, \"{path}\"");
        }

        private SavePreview GetSaveFile(DataGridView dgData)
        {
            var c = dgData.SelectedCells;
            if (c.Count != 1)
                return null;

            var item = c[0].RowIndex;
            var parent = dgData == dgDataRecent ? Recent : Backup;
            return parent[item];
        }

        private void DataGridCellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1 || e.Button != MouseButtons.Right)
                return;

            var c = ((DataGridView) sender)[e.ColumnIndex, e.RowIndex];
            c.DataGridView.ClearSelection();
            c.DataGridView.CurrentCell = c;
            c.Selected = true;
        }

        private SaveList<SavePreview> PopulateData(DataGridView dgData, IEnumerable<SaveFile> saves)
        {
            var list = new SaveList<SavePreview>();

            var enumerator = saves.GetEnumerator();
            while (enumerator.Current == null)
            {
                if (!enumerator.MoveNext())
                    return list;
            }

            var first = enumerator.Current;
            var sav1 = new SavePreview(first, GetParentFolderName(first));
            LoadEntryInitial(dgData, list, sav1);

            int ctr = 1; // refresh every 7 until 15+ are loaded
            Task.Run(async () => // load the rest async
            {
                while (!dgData.IsHandleCreated)
                    await Task.Delay(15).ConfigureAwait(false);
                while (enumerator.MoveNext())
                {
                    var next = enumerator.Current;
                    if (next == null)
                        continue;
                    var sav = new SavePreview(next, GetParentFolderName(next));
                    dgData.Invoke(new Action(() => LoadEntry(dgData, list, sav)));
                    ctr++;
                    if (ctr < 15 && ctr % 7 == 0)
                        dgData.Invoke(new Action(() => Refresh(dgData)));
                }
                dgData.Invoke(new Action(() => Refresh(dgData)));
                enumerator.Dispose();
            });

            return list;
        }

        private static void Refresh(DataGridView dgData)
        {
            dgData.SuspendLayout();
            dgData.AutoResizeColumns();
            dgData.Invalidate();
        }

        private static void LoadEntryInitial(DataGridView dgData, ICollection<SavePreview> list, SavePreview sav)
        {
            list.Add(sav);
            dgData.DataSource = list;
            dgData.AutoGenerateColumns = true;
            for (int i = 0; i < dgData.Columns.Count; i++)
                dgData.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
            dgData.AutoResizeColumns(); // Trigger Resizing
        }

        private void LoadEntry(DataGridView dgData, ICollection<SavePreview> list, SavePreview sav)
        {
            list.Add(sav);
            int count = list.Count;
            if (CB_FilterColumn.SelectedIndex != 0)
                ToggleRowVisibility(dgData, CB_FilterColumn.SelectedIndex - 1, TB_FilterTextContains.Text, count - 1);
        }

        private void ChangeFilterIndex(object sender, EventArgs e)
        {
            TB_FilterTextContains.Enabled = CB_FilterColumn.SelectedIndex != 0;
            SetRowFilter();
        }

        private void ChangeFilterText(object sender, EventArgs e)
        {
            if (CB_FilterColumn.SelectedIndex != 0)
                SetRowFilter();
        }

        private void SetRowFilter()
        {
            GetFilterText(dgDataRecent);
            GetFilterText(dgDataBackup);
        }

        private void GetFilterText(DataGridView dg)
        {
            if (dg.RowCount == 0)
                return;
            var cm = (CurrencyManager)BindingContext[dg.DataSource];
            cm.SuspendBinding();
            int column = CB_FilterColumn.SelectedIndex - 1;
            var text = TB_FilterTextContains.Text;

            for (int i = 0; i < dg.RowCount; i++)
                ToggleRowVisibility(dg, column, text, i);
            cm.ResumeBinding();
        }

        private static void ToggleRowVisibility(DataGridView dg, int column, string text, int i)
        {
            var row = dg.Rows[i];
            if (text.Length == 0 || column < 0)
            {
                row.Visible = true;
                return;
            }
            var cell = row.Cells[column];
            var value = cell.Value.ToString();
            row.Visible = value.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0; // case insensitive contains
        }
    }
}
