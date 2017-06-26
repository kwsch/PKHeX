using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

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
                new CustomFolderPath {Path = Main.BackupPath, DisplayText = "PKHeX Backups"}
            };
            locs.AddRange(GetUserPaths());
            locs.AddRange(Get3DSPaths());
            locs.Add(new CustomFolderPath {Path = CyberGadgetUtil.GetCacheFolder(), DisplayText = "CGSE Cache"});
            locs.Add(new CustomFolderPath {Path = CyberGadgetUtil.GetTempFolder(), DisplayText = "CGSE Temp"});
            
            foreach (var loc in locs.GroupBy(z => z.Path))
                AddButton(loc.First().DisplayText, loc.First().Path);

            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        }

        private void AddButton(string name, string path)
        {
            var button = new Button
            {
                Size = new Size { Height = 30, Width = 130 },
                Text = name,
                Name = "B_" + name,
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
                    WinFormsUtil.Alert("Can't find folder:", path);
            };
            FLP_Buttons.Controls.Add(button);
        }

        private static IEnumerable<CustomFolderPath> GetUserPaths()
        {
            string loc = Path.Combine(Main.WorkingDirectory, "savpaths.txt");

            if (!File.Exists(loc))
                yield break;

            string[] lines = File.ReadAllLines(loc);
            foreach (var line in lines)
            {
                var split = line.Split('\t');
                if (split.Length == 2)
                    yield return new CustomFolderPath {DisplayText = split[0], Path = split[1]};
            }
        }
        private static IEnumerable<CustomFolderPath> Get3DSPaths()
        {
            var path3DS = PathUtilWindows.Get3DSLocation();
            var path = path3DS == null || !Directory.Exists(path3DS) ? @"C:\" : Path.GetPathRoot(path3DS);
            foreach (var z in PathUtilWindows.Get3DSBackupPaths(path))
            {
                var di = new DirectoryInfo(z);
                var root = di.Root.Name;
                var folder = di.Parent.Name;
                if (root == folder)
                    folder = di.Name;
                yield return new CustomFolderPath {Path = z, DisplayText = folder};
            }
        }

        private struct CustomFolderPath
        {
            public string Path;
            public string DisplayText;
        }
    }
}
