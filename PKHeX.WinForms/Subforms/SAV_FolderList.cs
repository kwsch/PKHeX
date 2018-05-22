using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms
{
    public partial class SAV_FolderList : Form
    {
        public SAV_FolderList()
        {
            InitializeComponent();

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

            var paths = locs.GroupBy(z => z.Path).Select(z => z.First())
                .OrderByDescending(z => Directory.Exists(z.Path));
            foreach (var loc in paths)
                AddButton(loc.DisplayText, loc.Path);

            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

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

        private struct CustomFolderPath
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
    }
}
