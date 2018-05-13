using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms
{
    public static class WinFormsUtil
    {
        internal static void TranslateInterface(Control form, string lang) => form.TranslateInterface(lang);

        internal static void CenterToForm(this Control child, Control parent)
        {
            int x = parent.Location.X + (parent.Width - child.Width) / 2;
            int y = parent.Location.Y + (parent.Height - child.Height) / 2;
            child.Location = new Point(Math.Max(x, 0), Math.Max(y, 0));
        }

        public static Form FirstFormOfType<T>(this Form f) => Array.Find(f.OwnedForms, form => form is T);
        public static T FindFirstControlOfType<T>(Control aParent) where T : class
        {
            while (true)
            {
                var t = aParent as T;
                if (t != null)
                    return t;

                if (aParent.Parent != null)
                    aParent = aParent.Parent;
                else
                    return null;
            }
        }
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
            if (!(sender is Panel p) || e.NewValue < 0)
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

#if CLICKONCE
        public static bool IsClickonceDeployed => System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed;
#else
        public static bool IsClickonceDeployed => false;
#endif

        /// <summary>
        /// Opens a dialog to open a <see cref="SaveFile"/>, <see cref="PKM"/> file, or any other supported file.
        /// </summary>
        /// <param name="Extensions">Misc extensions of <see cref="PKM"/> files supported by the SAV.</param>
        /// <param name="path">Output result path</param>
        /// <returns>Result of whether or not a file is to be loaded from the output path.</returns>
        public static bool OpenSAVPKMDialog(IEnumerable<string> Extensions, out string path)
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
            bool allowEncrypted = pk.Format > 3 || pk is PK3;
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = $"Decrypted PKM File|*.{pkx}" +
                         (allowEncrypted ? $"|Encrypted PKM File|*.e{pkx.Substring(1)}" : "") +
                         "|Binary File|*.bin" +
                         "|All Files|*.*",
                DefaultExt = pkx,
                FileName = Util.CleanFileName(pk.FileName)
            };
            if (sfd.ShowDialog() != DialogResult.OK)
                return false;

            SavePKM(pk, sfd.FileName, pkx);
            return true;
        }
        private static void SavePKM(PKM pk, string path, string pkx)
        {
            SaveBackup(path);
            string ext = Path.GetExtension(path);
            var data = $".{pkx}" == ext ? pk.DecryptedBoxData : pk.EncryptedPartyData;
            File.WriteAllBytes(path, data);
        }
        private static void SaveBackup(string path)
        {
            if (!File.Exists(path))
                return;

            // File already exists, save a .bak
            string bakpath = $"{path}.bak";
            if (!File.Exists(bakpath))
                File.Move(path, bakpath);
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
            if (err.Length > 0 && Prompt(MessageBoxButtons.YesNo, err, MsgSaveExportContinue) != DialogResult.Yes)
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

            var ext = Path.GetExtension(main.FileName)?.ToLower();
            bool dsv = ext == ".dsv";
            bool gci = ext == ".gci";
            try
            {
                File.WriteAllBytes(main.FileName, SAV.Write(dsv, gci));
                SAV.Edited = false;
                Alert(MsgSaveExportSuccessPath, main.FileName);
            }
            catch (Exception x)
            {
                if (x is UnauthorizedAccessException || x is FileNotFoundException || x is IOException)
                    Error(MsgFileWriteFail + Environment.NewLine + x.Message, MsgFileWriteProtectedAdvice);
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
                case 4: return "Gen4 Mystery Gift|*.pgt;*.pcd;*.wc4|All Files|*.*";
                case 5: return "Gen5 Mystery Gift|*.pgf|All Files|*.*";
                case 6: return "Gen6 Mystery Gift|*.wc6;*.wc6full|All Files|*.*";
                case 7: return "Gen7 Mystery Gift|*.wc7;*.wc7full|All Files|*.*";
                default: return string.Empty;
            }
        }
    }
}
