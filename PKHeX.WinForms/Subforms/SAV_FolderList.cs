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

        public SAV_FolderList(Action<SaveFile> openSaveFile)
        {
            InitializeComponent();
            OpenSaveFile = openSaveFile;

            // Preprogrammed folders
            var locs = new List<CustomFolderPath>
            {
                new CustomFolderPath(Main.BackupPath, "PKHeX Backups")
            };
            locs.AddRange(GetUserPaths());
            locs.AddRange(Get3DSPaths());
            locs.AddRange(GetSwitchPaths());
            addIfExists(CyberGadgetUtil.GetCacheFolder(), "CGSE Cache");
            addIfExists(CyberGadgetUtil.GetTempFolder(), "CGSE Temp");

            Paths = locs.GroupBy(z => z.Path).Select(z => z.First())
                .OrderByDescending(z => Directory.Exists(z.Path)).ToList();
            foreach (var loc in Paths)
                AddButton(loc.DisplayText, loc.Path);

            var recent = PathUtilWindows.GetSaveFiles(Paths.Select(z => z.Path).Where(z => z != Main.BackupPath));
            Recent = PopulateData(dgDataRecent, recent);
            var backup = PathUtilWindows.GetSaveFiles(Main.BackupPath);
            Backup = PopulateData(dgDataBackup, backup);

            dgDataRecent.ContextMenuStrip = GetContextMenu(dgDataRecent);
            dgDataBackup.ContextMenuStrip = GetContextMenu(dgDataBackup);

            dgDataRecent.DoubleBuffered(true);
            dgDataBackup.DoubleBuffered(true);

            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            CenterToParent();

            void addIfExists(string path, string text)
            {
                if (Directory.Exists(path))
                    locs.Add(new CustomFolderPath(path, text));
            }
        }

        private void AddButton(string name, string path)
        {
            var button = new Button
            {
                Size = new Size { Height = 30, Width = 130 },
                Text = name,
                Name = $"B_{name}",
                Enabled = new DirectoryInfo(path).Exists,
            };
            button.Click += (s, e) =>
            {
                if (Directory.Exists(path))
                {
                    Process.Start("explorer.exe", path);
                    Close();
                }
                else
                    WinFormsUtil.Alert(MsgFolderNotFound, path);
            };
            FLP_Buttons.Controls.Add(button);
        }

        private static IEnumerable<CustomFolderPath> GetUserPaths()
        {
            string loc = Path.Combine(Main.WorkingDirectory, "savpaths.txt");

            if (!File.Exists(loc))
                return Enumerable.Empty<CustomFolderPath>();

            var lines = File.ReadLines(loc);
            return lines.Select(z => z.Split('\t'))
                .Where(a => a.Length == 2)
                .Select(x => new CustomFolderPath(x));
        }
        private static IEnumerable<CustomFolderPath> Get3DSPaths()
        {
            var path3DS = PathUtilWindows.GetSwitchLocation();
            if (path3DS == null || !Directory.Exists(path3DS))
                return Enumerable.Empty<CustomFolderPath>();
            var root = Path.GetPathRoot(path3DS);
            var paths = PathUtilWindows.GetSwitchBackupPaths(root);
            return paths.Select(z => new CustomFolderPath(z));
        }
        private static IEnumerable<CustomFolderPath> GetSwitchPaths()
        {
            var pathNX = PathUtilWindows.GetSwitchLocation();
            if (pathNX == null || !Directory.Exists(pathNX))
                return Enumerable.Empty<CustomFolderPath>();
            var root = Path.GetPathRoot(pathNX);
            var paths = PathUtilWindows.GetSwitchBackupPaths(root);
            return paths.Select(z => new CustomFolderPath(z));
        }

        private class CustomFolderPath
        {
            public readonly string Path;
            public readonly string DisplayText;

            public CustomFolderPath(string z)
            {
                var di = new DirectoryInfo(z);
                var root = di.Root.Name;
                var folder = di.Parent.Name;
                if (root == folder)
                    folder = di.Name;

                Path = z;
                DisplayText = folder;
            }

            public CustomFolderPath(IReadOnlyList<string> arr)
            {
                Path = arr[1];
                DisplayText = arr[0];
            }

            public CustomFolderPath(string path, string display)
            {
                Path = path;
                DisplayText = display;
            }
        }

        private string GetParentFolderName(SaveFile first)
        {
            var parent = Paths.Find(z => first.FilePath.StartsWith(z.Path));
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
            public string FileTime => new FileInfo(Save.FilePath).LastWriteTime.ToString("yyyy.MM.dd:hh:mm:ss");

            public int TID => Save.Generation >= 7 ? Save.TrainerID7 : Save.TID;
            public int SID => Save.Generation >= 7 ? Save.TrainerSID7 : Save.SID;

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
                }
                enumerator.Dispose();
            });

            return list;
        }

        private static void LoadEntryInitial(DataGridView dgData, SaveList<SavePreview> list, SavePreview sav)
        {
            list.Add(sav);
            dgData.DataSource = list;
            dgData.AutoGenerateColumns = true;
            for (int i = 0; i < dgData.Columns.Count; i++)
                dgData.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
            dgData.AutoResizeColumns(); // Trigger Resizing
        }

        private static void LoadEntry(DataGridView dgData, ICollection<SavePreview> list, SavePreview sav)
        {
            dgData.SuspendLayout();
            list.Add(sav);
            dgData.AutoResizeColumns();
            dgData.ResumeLayout();
        }
    }
}
