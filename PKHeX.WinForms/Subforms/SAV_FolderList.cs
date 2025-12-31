using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Properties;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms;

public partial class SAV_FolderList : Form
{
    private readonly Action<SaveFile> OpenSaveFile;
    private readonly List<INamedFolderPath> Paths;
    private readonly SortableBindingList<SavePreview> Recent;
    private readonly SortableBindingList<SavePreview> Backup;
    private readonly CancellationTokenSource cts = new(TimeSpan.FromSeconds(20));

    public SAV_FolderList(Action<SaveFile> openSaveFile)
    {
        InitializeComponent();
        FormClosing += (_, _) => cts.Cancel();
        OpenSaveFile = openSaveFile;

        var backups = Main.BackupPath;
        var drives = Environment.GetLogicalDrives();
        Paths = GetPathList(drives, backups);

        dgDataRecent.ContextMenuStrip = GetContextMenu(dgDataRecent);
        dgDataBackup.ContextMenuStrip = GetContextMenu(dgDataBackup);
        dgDataRecent.Sorted += (_, _) => GetFilterText(dgDataRecent);
        dgDataBackup.Sorted += (_, _) => GetFilterText(dgDataBackup);

        var token = cts.Token;
        var extra = Paths.Select(z => z.Path).Where(z => z != backups).Distinct();
        var backup = SaveFinder.GetSaveFiles(drives, false, [backups], false, token);
        var recent = SaveFinder.GetSaveFiles(drives, false, extra, true, token).ToList();
        var loaded = Main.Settings.Startup.RecentlyLoaded
            .Where(z => recent.All(x => x.Metadata.FilePath != z))
            .Where(File.Exists).Select(SaveUtil.GetSaveFile).OfType<SaveFile>();

        Recent = PopulateData(dgDataRecent, loaded.Concat(recent));
        Backup = PopulateData(dgDataBackup, backup);

        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

        CB_FilterColumn.Items.Add(MsgAny);
        var dgv = Recent.Count >= 1 ? dgDataRecent : dgDataBackup;
        int count = dgv.ColumnCount;
        for (int i = 0; i < count; i++)
        {
            var text = dgv.Columns[i].HeaderText;
            CB_FilterColumn.Items.Add(text);
        }
        CB_FilterColumn.SelectedIndex = 0;

        // Pre-programmed folders
        foreach (var loc in Paths)
            AddButton(loc.DisplayText, loc.Path);

        CenterToParent();

        if (Application.IsDarkModeEnabled)
        {
            WinFormsUtil.InvertToolStripIcons(dgDataBackup.ContextMenuStrip.Items);
            WinFormsUtil.InvertToolStripIcons(dgDataRecent.ContextMenuStrip.Items);
        }
    }

    private static List<INamedFolderPath> GetPathList(IReadOnlyList<string> drives, string backupPath)
    {
        List<INamedFolderPath> locs =
        [
            new CustomFolderPath(backupPath, DisplayText: "PKHeX Backups"),
            ..GetUserPaths(), ..GetPaths3DS(drives), ..GetPathsSwitch(drives),
        ];
        var filtered = locs
            .DistinctBy(z => z.Path)
            .OrderByDescending(z => Directory.Exists(z.Path));
        return [..filtered];
    }

    private const int ButtonHeight = 40;
    private const int ButtonWidth = 130;

    private void AddButton(string name, string path)
    {
        Button button = GetCustomButton(name);
        button.Enabled = Directory.Exists(path);
        button.Click += (_, _) =>
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

        var hover = new ToolTip {AutoPopDelay = 30_000};
        button.MouseHover += (_, _) => hover.Show(path, button);
    }

    private static Button GetCustomButton(string name) => new()
    {
        Size = new Size { Height = ButtonHeight, Width = ButtonWidth },
        Text = name,
        Name = $"B_{name}",
    };

    private static IEnumerable<CustomFolderPath> GetUserPaths()
    {
        var paths = Main.Settings.Backup.OtherBackupPaths;
        return paths.Select(x => new CustomFolderPath(x, FolderPathGroup.Custom));
    }

    private static IEnumerable<CustomFolderPath> GetPaths3DS(IEnumerable<string> drives)
    {
        var path3DS = SaveFinder.Get3DSLocation(drives);
        if (path3DS is null)
            return [];

        var root = Path.GetPathRoot(path3DS);
        if (root is null)
            return [];

        var paths = SaveFinder.Get3DSBackupPaths(root);
        return paths.Select(z => new CustomFolderPath(z, FolderPathGroup.Nintendo3DS));
    }

    private static IEnumerable<CustomFolderPath> GetPathsSwitch(IEnumerable<string> drives)
    {
        var pathNX = SaveFinder.GetSwitchLocation(drives);
        if (pathNX is null)
            return [];

        var root = Path.GetPathRoot(pathNX);
        if (root is null)
            return [];

        var paths = SaveFinder.GetSwitchBackupPaths(root);
        return paths.Select(z => new CustomFolderPath(z, FolderPathGroup.NintendoSwitch));
    }

    private sealed record CustomFolderPath(string Path, string DisplayText, FolderPathGroup Group = 0) : INamedFolderPath
    {
        public CustomFolderPath(string path, FolderPathGroup group = 0)
            : this(path, ResolveFolderName(path), group) { }

        private static string ResolveFolderName(string path)
        {
            var di = new DirectoryInfo(path);
            var root = di.Root.Name;
            var display = di.Parent?.Name ?? di.Name;
            return root == display ? di.Name : display;
        }
    }

    private sealed class SaveList<T> : SortableBindingList<T> where T : class;

    private ContextMenuStrip GetContextMenu(DataGridView dgv)
    {
        var mnuOpen = new ToolStripMenuItem
        {
            Name = "mnuOpen",
            Text = "Open",
            Image = Resources.open,
        };
        mnuOpen.Click += (_, _) => ClickOpenFile(dgv);

        var mnuBrowseAt = new ToolStripMenuItem
        {
            Name = "mnuBrowseAt",
            Text = "Browse...",
            Image = Resources.folder,
        };
        mnuBrowseAt.Click += (_, _) => ClickOpenFolder(dgv);

        ContextMenuStrip mnu = new();
        mnu.Items.Add(mnuOpen);
        mnu.Items.Add(mnuBrowseAt);
        return mnu;
    }

    private void ClickOpenFile(DataGridView dgv)
    {
        var sav = GetSaveFile(dgv);
        if (sav is null || !File.Exists(sav.FilePath))
        {
            WinFormsUtil.Alert(MsgFileLoadFail);
            return;
        }

        OpenSaveFile(sav.Save);
    }

    private void ClickOpenFolder(DataGridView dgv)
    {
        var sav = GetSaveFile(dgv);
        if (sav is null || !File.Exists(sav.FilePath))
        {
            WinFormsUtil.Alert(MsgFileLoadFail);
            return;
        }

        var path = sav.Save.Metadata.FilePath;
        Process.Start("explorer.exe", $"/select, \"{path}\"");
    }

    private SavePreview? GetSaveFile(DataGridView dgData)
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
        var dgv = (DataGridView)sender;
        var c = dgv[e.ColumnIndex, e.RowIndex];
        dgv.ClearSelection();
        dgv.CurrentCell = c;
        c.Selected = true;
    }

    private SaveList<SavePreview> PopulateData(DataGridView dgData, IEnumerable<SaveFile> saves)
    {
        var list = new SaveList<SavePreview>();

        var enumerator = saves.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            enumerator.Dispose();
            return list;
        }

        var first = enumerator.Current;
        var sav1 = new SavePreview(first, Paths);
        LoadEntryInitial(dgData, list, sav1);

        int ctr = 1; // refresh every 7 until 15+ are loaded

        void RefreshResize() => Refresh(dgData);
        Task.Run(async () => // load the rest async
        {
            while (!dgData.IsHandleCreated)
                await Task.Delay(15).ConfigureAwait(false);
            while (enumerator.MoveNext())
            {
                var next = enumerator.Current;
                var sav = new SavePreview(next, Paths);
                void Load() => LoadEntry(dgData, list, sav);

                dgData.BeginInvoke(Load);
                ctr++;
                if (ctr < 15 && ctr % 7 == 0)
                    dgData.BeginInvoke(RefreshResize);
            }
            dgData.BeginInvoke(RefreshResize);
            enumerator.Dispose();
        });

        return list;
    }

    public static void CleanBackups(string path, bool deleteNotSaves)
    {
        var files = Directory.GetFiles(path);
        foreach (var file in files)
        {
            var fi = new FileInfo(file);
            if (!SaveUtil.IsSizeValid(fi.Length) || !SaveUtil.TryGetSaveFile(file, out var sav))
            {
                if (deleteNotSaves)
                    File.Delete(file);
                continue;
            }

            var self = sav.Metadata.FilePath;
            if (self is null)
                continue; // shouldn't hit
            var index = self.IndexOf(" [", StringComparison.OrdinalIgnoreCase);
            if (index < 0)
                continue;
            var original = self[..index];
            sav.Metadata.SetExtraInfo(original);

            string backupName = sav.Metadata.GetBackupFileName(Main.BackupPath);
            if (self == backupName)
                continue;

            if (File.Exists(backupName))
                File.Delete(self);
            else
                File.Move(self, backupName);
        }
    }

    private static void Refresh(DataGridView dgData)
    {
        dgData.SuspendLayout();
        dgData.AutoResizeColumns();
        dgData.ResumeLayout(true);
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
        var cm = (CurrencyManager?)BindingContext?[dg.DataSource!];
        cm?.SuspendBinding();
        int column = CB_FilterColumn.SelectedIndex - 1;
        var text = TB_FilterTextContains.Text.AsSpan();

        for (int i = 0; i < dg.RowCount; i++)
            ToggleRowVisibility(dg, column, text, i);
        cm?.ResumeBinding();
    }

    private static void ToggleRowVisibility(DataGridView dg, int column, ReadOnlySpan<char> text, int rowIndex)
    {
        var row = dg.Rows[rowIndex];
        if (text.Length == 0 || column < 0)
        {
            row.Visible = true;
            return;
        }
        var cell = row.Cells[column];
        var value = cell.Value?.ToString();
        if (value is null)
        {
            row.Visible = false;
            return;
        }
        row.Visible = value.Contains(text, StringComparison.CurrentCultureIgnoreCase); // case insensitive contains
    }
}
