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
        internal static void TranslateInterface(Control form, string lang)
        {
            // Check to see if a the translation file exists in the same folder as the executable
            string externalLangPath = "lang_" + lang + ".txt";
            string[] rawlist;
            if (File.Exists(externalLangPath))
                rawlist = File.ReadAllLines(externalLangPath);
            else
            {
                var file = "lang_" + lang;
                rawlist = Util.getStringList(file);
                if (rawlist.Length == 0)
                {
                    // Translation file does not exist as a resource; abort this function and don't translate UI.
                    return;
                }
            }

            List<string> stringdata = new List<string>();
            int start = -1;
            for (int i = 0; i < rawlist.Length; i++)
            {
                // Find our starting point
                if (!rawlist[i].Contains("! " + form.Name)) continue;
                start = i;
                break;
            }
            if (start < 0)
                return;

            // Rename Window Title
            string[] WindowName = rawlist[start].Split(new[] { " = " }, StringSplitOptions.None);
            if (WindowName.Length > 1) form.Text = WindowName[1];

            // Fetch controls to rename
            for (int i = start + 1; i < rawlist.Length; i++)
            {
                if (rawlist[i].Length == 0) continue; // Skip Over Empty Lines, errhandled
                if (rawlist[i][0] == '-') continue; // Keep translating if line is a comment line
                if (rawlist[i][0] == '!') // Stop if we have reached the end of translation
                    break;
                stringdata.Add(rawlist[i]); // Add the entry to process later.
            }

            if (stringdata.Count == 0)
                return;

            // Find control then change display Text.
            form.SuspendLayout();
            foreach (string str in stringdata)
            {
                string[] SplitString = str.Split(new[] { " = " }, StringSplitOptions.None);
                if (SplitString.Length < 2)
                    continue;

                object c = FindControl(SplitString[0], form.Controls); // Find control within Form's controls
                if (c == null) // Not found
                    continue;

                string text = SplitString[1]; // Text to set Control.Text to...

                if (c is Control)
                    (c as Control).Text = text;
                else if (c is ToolStripItem)
                    (c as ToolStripItem).Text = text;
            }
            form.ResumeLayout();
        }
        private static object FindControl(string name, Control.ControlCollection c)
        {
            Control control = c.Find(name, true).FirstOrDefault();
            if (control != null)
                return control;
            foreach (MenuStrip menu in c.OfType<MenuStrip>())
            {
                var item = menu.Items.Find(name, true).FirstOrDefault();
                if (item != null)
                    return item;
            }
            foreach (ContextMenuStrip strip in FindContextMenuStrips(c.OfType<Control>()))
            {
                var item = strip.Items.Find(name, true).FirstOrDefault();
                if (item != null)
                    return item;
            }
            return null;
        }
        private static List<ContextMenuStrip> FindContextMenuStrips(IEnumerable<Control> c)
        {
            List<ContextMenuStrip> cs = new List<ContextMenuStrip>();
            foreach (Control control in c)
            {
                if (control.ContextMenuStrip != null)
                    cs.Add(control.ContextMenuStrip);

                else if (control.Controls.Count > 0)
                    cs.AddRange(FindContextMenuStrips(control.Controls.OfType<Control>()));
            }
            return cs;
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

        internal static int getIndex(ComboBox cb)
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
            string supported = string.Join(";", Extensions.Select(s => "*." + s).Concat(new[] { "*.pkm" }));
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
            if (!PathUtilWindows.detectSaveFile(out path, cgse))
                Error(path);

            if (path != null)
            { ofd.FileName = path; }

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
                string bakpath = path + ".bak";
                if (!File.Exists(bakpath))
                {
                    byte[] backupfile = File.ReadAllBytes(path);
                    File.WriteAllBytes(bakpath, backupfile);
                }
            }

            if (new[] { ".ekx", "." + ekx, ".bin" }.Contains(ext))
                File.WriteAllBytes(path, pk.EncryptedPartyData);
            else if (new[] { "." + pkx }.Contains(ext))
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
                Filter = getMysterGiftFilter(gift.Format),
                FileName = Util.CleanFileName(gift.FileName)
            };
            if (output.ShowDialog() != DialogResult.OK)
                return false;

            string path = output.FileName;

            if (File.Exists(path))
            {
                // File already exists, save a .bak
                string bakpath = path + ".bak";
                if (!File.Exists(bakpath))
                {
                    byte[] backupfile = File.ReadAllBytes(path);
                    File.WriteAllBytes(bakpath, backupfile);
                }
            }

            File.WriteAllBytes(path, gift.Data);
            return true;
        }

        public static string getMysterGiftFilter(int Format)
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
