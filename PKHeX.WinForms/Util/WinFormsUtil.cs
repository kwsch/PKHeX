using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PKHeX.WinForms
{
    public static class WinFormsUtil
    {
        #region Form Translation
        private static readonly string[] Splitter = {" = "};
        private const char Comment = '-';
        private const char FormStart = '!';
        internal static void TranslateInterface(Control form, string lang)
        {
            if (!TryGetTranslationFile(lang, out string[] rawlist))
                return; // no translation data retrieved

            // Find Starting Point
            int start = GetTranslationStart(rawlist, form.Name);
            if (start < 0) // no form info found
                return;

            // Rename Window Title
            string[] WindowName = rawlist[start].Split(Splitter, StringSplitOptions.None);
            if (WindowName.Length > 1) // window title is specified
                form.Text = WindowName[1];

            // Fetch controls to rename
            var stringdata = GetTranslationList(rawlist, start);
            if (stringdata.Count == 0) // no translation data available
                return;

            // Execute Translation
            form.SuspendLayout();
            TranslateForm(form, stringdata);
            form.ResumeLayout();
        }
        private static bool TryGetTranslationFile(string lang, out string[] rawlist)
        {
            var file = $"lang_{lang}";
            // Check to see if a the translation file exists in the same folder as the executable
            string externalLangPath = $"{file}.txt";
            if (File.Exists(externalLangPath))
            {
                try 
                {
                    rawlist = File.ReadAllLines(externalLangPath);
                    return true;
                }
                catch { /* In use? Just return the internal resource. */ }
            }

            rawlist = Util.GetStringList(file);
            // If there's no strings (or null), the translation file does not exist.
            // No file => abort this function and don't translate UI.
            return rawlist?.Length > 0; 
        }
        private static int GetTranslationStart(IReadOnlyList<string> rawlist, string name)
        {
            for (int i = 0; i < rawlist.Count; i++)
                if (rawlist[i].StartsWith($"{FormStart} {name}"))
                    return i;
            return -1;
        }
        private static List<string> GetTranslationList(IReadOnlyList<string> rawlist, int start)
        {
            List<string> stringdata = new List<string>();
            for (int i = start + 1; i < rawlist.Count; i++)
            {
                var line = rawlist[i];
                if (line.Length == 0) continue; // Skip Over Empty Lines
                if (line[0] == Comment) continue; // Keep translating if line is a comment line
                if (line[0] == FormStart) // Stop if we have reached the end of translation
                    break;
                stringdata.Add(rawlist[i]); // Add the entry to process later.
            }
            return stringdata;
        }

        private static void TranslateForm(Control form, IEnumerable<string> stringdata)
        {
            // Only fetch the list of controls once; store in dictionary for faster translation
            var controls = GetControlDictionary(form);
            foreach (string str in stringdata)
            {
                string[] SplitString = str.Split(Splitter, StringSplitOptions.None);
                if (SplitString.Length != 2)
                    continue;

                var controlName = SplitString[0];
                if (!controls.TryGetValue(controlName, out object c))
                    continue; // control not found

                string text = SplitString[1];
                if (c is Control r)
                    r.Text = text;
                else if (c is ToolStripItem t)
                    t.Text = text;
            }
        }
        private static Dictionary<string, object> GetControlDictionary(Control form)
        {
            return GetTranslatableControls(form)
                            .GroupBy(p => p.Key, StringComparer.OrdinalIgnoreCase)
                            .ToDictionary(g => g.Key, g => g.First().Value, StringComparer.OrdinalIgnoreCase);
        }
        private static IEnumerable<KeyValuePair<string, object>> GetTranslatableControls(Control f)
        {
            foreach (var z in f.GetChildrenOfType<Control>())
            {
                switch (z)
                {
                    case ToolStrip menu:
                        foreach (var pair in GetToolStripMenuItems(menu))
                            yield return pair;

                        break;
                    default:
                        if (string.IsNullOrWhiteSpace(z.Name))
                            break;
                        
                        if (z.ContextMenuStrip != null) // control has attached menustrip
                            foreach (var pair in GetToolStripMenuItems(z.ContextMenuStrip))
                                yield return pair;

                        if (z is ComboBox || z is TextBox || z is MaskedTextBox || z is LinkLabel)
                            break; // undesirable to modify, ignore

                        if (!string.IsNullOrWhiteSpace(z.Text))
                            yield return new KeyValuePair<string, object>(z.Name, z);
                        break;
                }
            }
        }
        private static IEnumerable<T> GetChildrenOfType<T>(this Control control) where T : class
        {
            foreach (Control child in control.Controls)
            {
                T childOfT = child as T;
                if (childOfT != null)
                    yield return childOfT;

                if (!child.HasChildren) continue;
                foreach (T descendant in GetChildrenOfType<T>(child))
                    yield return descendant;
            }
        }
        private static IEnumerable<KeyValuePair<string, object>> GetToolStripMenuItems(ToolStrip menu)
        {
            foreach (var i in menu.Items.OfType<ToolStripMenuItem>())
            {
                if (!string.IsNullOrWhiteSpace(i.Text))
                    yield return new KeyValuePair<string, object>(i.Name, i);
                foreach (var sub in GetToolsStripDropDownItems(i).Where(z => !string.IsNullOrWhiteSpace(z.Text)))
                    yield return new KeyValuePair<string, object>(sub.Name, sub);
            }
        }
        private static IEnumerable<ToolStripMenuItem> GetToolsStripDropDownItems(ToolStripDropDownItem item)
        {
            foreach (var dropDownItem in item.DropDownItems.OfType<ToolStripMenuItem>())
            {
                if (dropDownItem.HasDropDownItems)
                    foreach (ToolStripMenuItem subItem in GetToolsStripDropDownItems(dropDownItem))
                        yield return subItem;

                yield return dropDownItem;
            }
        }

        internal static void CenterToForm(this Control child, Control parent)
        {
            int x = parent.Location.X + (parent.Width - child.Width) / 2;
            int y = parent.Location.Y + (parent.Height - child.Height) / 2;
            child.Location = new Point(Math.Max(x, 0), Math.Max(y, 0));
        }
        #endregion

        public static Form FirstFormOfType<T>(this Form f) => f.OwnedForms.FirstOrDefault(form => form is T);
        public static Control GetUnderlyingControl(object sender) => ((sender as ToolStripItem)?.Owner as ContextMenuStrip)?.SourceControl ?? sender as PictureBox;

        #region Message Displays
        /// <summary>
        /// Displays a dialog showing the details of an error.
        /// </summary>
        /// <param name="friendlyMessage">User-friendly message about the error.</param>
        /// <param name="exception">Instance of the error's <see cref="Exception"/>.</param>
        /// <returns>The <see cref="DialogResult"/> associated with the dialog.</returns>
        internal static DialogResult Error(string friendlyMessage, Exception exception)
        {
            System.Media.SystemSounds.Exclamation.Play();
            return ErrorWindow.ShowErrorDialog(friendlyMessage, exception, true);
        }

        /// <summary>
        /// Displays a dialog showing the details of an error.
        /// </summary>
        /// <param name="lines">User-friendly message about the error.</param>
        /// <returns>The <see cref="DialogResult"/> associated with the dialog.</returns>
        internal static DialogResult Error(params string[] lines)
        {
            System.Media.SystemSounds.Exclamation.Play();
            string msg = string.Join(Environment.NewLine + Environment.NewLine, lines);
            return MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        internal static DialogResult Alert(params string[] lines)
        {
            System.Media.SystemSounds.Asterisk.Play();
            string msg = string.Join(Environment.NewLine + Environment.NewLine, lines);
            return MessageBox.Show(msg, "Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        internal static DialogResult Prompt(MessageBoxButtons btn, params string[] lines)
        {
            System.Media.SystemSounds.Question.Play();
            string msg = string.Join(Environment.NewLine + Environment.NewLine, lines);
            return MessageBox.Show(msg, "Prompt", btn, MessageBoxIcon.Asterisk);
        }

        internal static int GetIndex(ComboBox cb)
        {
            return (int)(cb?.SelectedValue ?? 0);
        }

        public static void PanelScroll(object sender, ScrollEventArgs e)
        {
            var p = sender as Panel;
            if (e.NewValue < 0)
                return;
            switch (e.ScrollOrientation)
            {
                case ScrollOrientation.HorizontalScroll:
                    p.HorizontalScroll.Value = e.NewValue;
                    break;
                case ScrollOrientation.VerticalScroll:
                    p.VerticalScroll.Value = e.NewValue;
                    break;
            }
        }
        public static void RemoveDropCB(object sender, KeyEventArgs e) => ((ComboBox)sender).DroppedDown = false;
        public static IEnumerable<Control> GetAllControlsOfType(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>().ToList();
            return controls.SelectMany(ctrl => GetAllControlsOfType(ctrl, type))
                .Concat(controls)
                .Where(c => c.GetType() == type);
        }
        #endregion

        public static bool IsClickonceDeployed
        {
            get
            {
#if CLICKONCE
                return System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed;
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// Opens a dialog to open a <see cref="SaveFile"/>, <see cref="PKM"/> file, or any other supported file.
        /// </summary>
        /// <param name="Extensions">Misc extensions of <see cref="PKM"/> files supported by the SAV.</param>
        /// <param name="path">Output result path</param>
        /// <returns>Result of whether or not a file is to be loaded from the output path.</returns>
        public static bool OpenSAVPKMDialog(string[] Extensions, out string path)
        {
            string supported = string.Join(";", Extensions.Select(s => $"*.{s}").Concat(new[] { "*.pkm" }));
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "All Files|*.*" +
                         $"|Supported Files|main;*.sav;*.dat;*.gci;*.bin;{supported};*.bak" +
                         "|3DS Main Files|main" +
                         "|Save Files|*.sav;*.dat;*.gci" +
                         "|Decrypted PKM File|" + supported +
                         "|Binary File|*.bin" +
                         "|Backup File|*.bak"
            };

            // Detect main
            string cgse = "";
            string pathCache = CyberGadgetUtil.GetCacheFolder();
            if (Directory.Exists(pathCache))
                cgse = Path.Combine(pathCache);
            if (!PathUtilWindows.DetectSaveFile(out path, cgse) && !string.IsNullOrEmpty(path))
            {
                Error(path); // `path` contains the error message
                path = null;
            }

            if (path != null)
                ofd.FileName = path;

            if (ofd.ShowDialog() != DialogResult.OK)
                return false;

            path = ofd.FileName;
            return true;
        }
        /// <summary>
        /// Opens a dialog to save a <see cref="PKM"/> file.
        /// </summary>
        /// <param name="pk"><see cref="PKM"/> file to be saved.</param>
        /// <returns>Result of whether or not the file was saved.</returns>
        public static bool SavePKMDialog(PKM pk)
        {
            string pkx = pk.Extension;
            string ekx = 'e' + pkx.Substring(1, pkx.Length - 1);
            bool allowEncrypted = pk.Format > 2 && pkx[0] == 'p' || pkx[0] == 'b';
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = $"Decrypted PKM File|*.{pkx}" +
                         (allowEncrypted ? $"|Encrypted PKM File|*.{ekx}" : "") +
                         "|Binary File|*.bin" +
                         "|All Files|*.*",
                DefaultExt = pkx,
                FileName = Util.CleanFileName(pk.FileName)
            };
            if (sfd.ShowDialog() != DialogResult.OK)
                return false;
            string path = sfd.FileName;
            string ext = Path.GetExtension(path);

            if (File.Exists(path))
            {
                // File already exists, save a .bak
                string bakpath = $"{path}.bak";
                if (!File.Exists(bakpath))
                {
                    byte[] backupfile = File.ReadAllBytes(path);
                    File.WriteAllBytes(bakpath, backupfile);
                }
            }

            if (new[] { ".ekx", $".{ekx}", ".bin" }.Contains(ext))
                File.WriteAllBytes(path, pk.EncryptedPartyData);
            else if (new[] {$".{pkx}"}.Contains(ext))
                File.WriteAllBytes(path, pk.DecryptedBoxData);
            else
            {
                Error($"Foreign File Extension: {ext}", "Exporting as encrypted.");
                File.WriteAllBytes(path, pk.EncryptedPartyData);
            }
            return true;
        }
        /// <summary>
        /// Opens a dialog to save a <see cref="SaveFile"/> file.
        /// </summary>
        /// <param name="SAV"><see cref="SaveFile"/> to be saved.</param>
        /// <param name="CurrentBox">Box the player will be greeted with when accessing the PC ingame.</param>
        /// <returns>Result of whether or not the file was saved.</returns>
        public static bool SaveSAVDialog(SaveFile SAV, int CurrentBox = 0)
        {
            // Chunk Error Checking
            string err = SAV.MiscSaveChecks();
            if (err.Length > 0 && Prompt(MessageBoxButtons.YesNo, err, "Continue saving?") != DialogResult.Yes)
                return false;

            SaveFileDialog main = new SaveFileDialog
            {
                Filter = SAV.Filter,
                FileName = SAV.FileName,
                RestoreDirectory = true
            };
            if (Directory.Exists(SAV.FilePath))
                main.InitialDirectory = SAV.FilePath;

            // Export
            if (main.ShowDialog() != DialogResult.OK)
                return false;

            if (SAV.HasBox)
                SAV.CurrentBox = CurrentBox;

            bool dsv = Path.GetExtension(main.FileName)?.ToLower() == ".dsv";
            bool gci = Path.GetExtension(main.FileName)?.ToLower() == ".gci";
            try
            {
                File.WriteAllBytes(main.FileName, SAV.Write(dsv, gci));
                SAV.Edited = false;
                Alert("SAV exported to:", main.FileName);
            }
            catch (Exception x)
            {
                if (x is UnauthorizedAccessException || x is FileNotFoundException || x is IOException)
                    Error("Unable to save." + Environment.NewLine + x.Message,
                        "If destination is a removable disk (SD card), please ensure the write protection switch is not set.");
                else throw;
            }
            return true;
        }
        /// <summary>
        /// Opens a dialog to save a <see cref="MysteryGift"/> file.
        /// </summary>
        /// <param name="gift"><see cref="MysteryGift"/> to be saved.</param>
        /// <returns>Result of whether or not the file was saved.</returns>
        public static bool SaveMGDialog(MysteryGift gift)
        {
            SaveFileDialog output = new SaveFileDialog
            {
                Filter = GetMysterGiftFilter(gift.Format),
                FileName = Util.CleanFileName(gift.FileName)
            };
            if (output.ShowDialog() != DialogResult.OK)
                return false;

            string path = output.FileName;

            if (File.Exists(path))
            {
                // File already exists, save a .bak
                string bakpath = $"{path}.bak";
                if (!File.Exists(bakpath))
                    File.Move(path, bakpath);
            }

            File.WriteAllBytes(path, gift.Data);
            return true;
        }

        public static string GetMysterGiftFilter(int Format)
        {
            switch (Format)
            {
                case 4:
                    return "Gen4 Mystery Gift|*.pgt;*.pcd|All Files|*.*";
                case 5:
                    return "Gen5 Mystery Gift|*.pgf|All Files|*.*";
                case 6:
                    return "Gen6 Mystery Gift|*.wc6;*.wc6full|All Files|*.*";
                case 7:
                    return "Gen7 Mystery Gift|*.wc7;*.wc7full|All Files|*.*";
                default:
                    return "";
            }
        }
    }
}
