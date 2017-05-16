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
            var path3DS = Path.GetPathRoot(PathUtilWindows.get3DSLocation() ?? "");
            var locs = new List<CustomFolderPath>
            {
                new CustomFolderPath {Path = Main.BackupPath, DisplayText = "PKHeX Backups"}
            };
            locs.AddRange(getUserPaths());
            foreach (var z in PathUtilWindows.get3DSBackupPaths(path3DS))
            {
                var di = new DirectoryInfo(z);
                var root = di.Root.Name;
                var folder = di.Parent.Name;
                if (root == folder)
                    folder = di.Name;
                locs.Add(new CustomFolderPath {Path = z, DisplayText = folder});
            }
            locs.Add(new CustomFolderPath {Path = CyberGadgetUtil.GetCacheFolder(), DisplayText = "CGSE Cache"});
            locs.Add(new CustomFolderPath {Path = CyberGadgetUtil.GetTempFolder(), DisplayText = "CGSE Temp"});
            
            foreach (var loc in locs.GroupBy(z => z.Path))
                addButton(loc.First().DisplayText, loc.First().Path);

            WinFormsUtil.TranslateInterface(this, Main.curlanguage);
        }

        private void addButton(string name, string path)
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
                    Process.Start("explorer.exe", path);
                else
                    WinFormsUtil.Alert("Can't find folder:", path);
            };
            FLP_Buttons.Controls.Add(button);
        }

        private static List<CustomFolderPath> getUserPaths()
        {
            const string loc = "savpaths.txt";
            var list = new List<CustomFolderPath>();

            if (!File.Exists(loc))
                return list;

            try
            {
                string[] lines = File.ReadAllLines(loc);
                return (from line in lines
                    select line.Split('\t') into split
                    where split.Length == 2
                    select new CustomFolderPath { DisplayText = split[0], Path = split[1] })
                    .ToList();
            }
            catch
            {
                return list;
            }
        }

        public struct CustomFolderPath
        {
            public string Path;
            public string DisplayText;
        }
    }
}
