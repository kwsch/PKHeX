using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using static PKHeX.Core.MessageStrings;
using Exception = System.Exception;

namespace PKHeX.WinForms
{
    public static class WinFormsUtil
    {
        internal static void TranslateInterface(Control form, string lang) => form.TranslateInterface(lang);

        /// <summary>
        /// Centers the <see cref="child"/> horizontally and vertically so that its center is the same as the <see cref="parent"/>'s center.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        internal static void CenterToForm(this Control child, Control? parent)
        {
            if (parent == null)
                return;
            int x = parent.Location.X + ((parent.Width - child.Width) / 2);
            int y = parent.Location.Y + ((parent.Height - child.Height) / 2);
            child.Location = new Point(Math.Max(x, 0), Math.Max(y, 0));
        }

        /// <summary>
        /// Horizontally centers the <see cref="child"/> to the <see cref="parent"/>'s horizontal center.
        /// </summary>
        internal static void HorizontallyCenter(this Control child, Control parent)
        {
            int x = ((parent.Width - child.Width) / 2);
            child.Location = new Point(x, child.Location.Y);
        }

        public static T? FirstFormOfType<T>() where T : Form => (T?)Application.OpenForms.Cast<Form>().FirstOrDefault(form => form is T);

        public static T? FindFirstControlOfType<T>(Control aParent) where T : class
        {
            while (true)
            {
                if (aParent is T t)
                    return t;

                if (aParent.Parent != null)
                    aParent = aParent.Parent;
                else
                    return null;
            }
        }

        public static T? GetUnderlyingControl<T>(object sender) where T : class
        {
            while (true)
            {
                switch (sender)
                {
                    case ToolStripItem t:
                        sender = t.Owner;
                        continue;
                    case ContextMenuStrip c:
                        sender = c.SourceControl;
                        continue;
                    case T p:
                        return p;
                    default:
                        return default;
                }
            }
        }

        public static bool OpenWindowExists<T>(this Form parent) where T : Form
        {
            var form = FirstFormOfType<T>();
            if (form == null)
                return false;

            form.CenterToForm(parent);
            form.BringToFront();
            return true;
        }

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
            System.Media.SystemSounds.Hand.Play();
            string msg = string.Join(Environment.NewLine + Environment.NewLine, lines);
            return MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        internal static DialogResult Alert(params string[] lines) => Alert(true, lines);

        internal static DialogResult Alert(bool sound, params string[] lines)
        {
            if (sound)
                System.Media.SystemSounds.Asterisk.Play();
            string msg = string.Join(Environment.NewLine + Environment.NewLine, lines);
            return MessageBox.Show(msg, "Alert", MessageBoxButtons.OK, sound ? MessageBoxIcon.Information : MessageBoxIcon.None);
        }

        internal static DialogResult Prompt(MessageBoxButtons btn, params string[] lines)
        {
            System.Media.SystemSounds.Asterisk.Play();
            string msg = string.Join(Environment.NewLine + Environment.NewLine, lines);
            return MessageBox.Show(msg, "Prompt", btn, MessageBoxIcon.Question);
        }
        #endregion

        internal static bool SetClipboardText(string text)
        {
            try
            {
                Clipboard.SetText(text);
                return true;
            }
            catch (ExternalException x)
            {
                Error(MsgClipboardFailWrite, x);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            // Clipboard might be locked sometimes
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                Error(MsgClipboardFailWrite);
            }

            return false;
        }

        /// <summary>
        /// Gets the selected value of the input <see cref="cb"/>. If no value is selected, will return 0.
        /// </summary>
        /// <param name="cb">ComboBox to retrieve value for.</param>
        internal static int GetIndex(ListControl cb)
        {
            return (int)(cb.SelectedValue ?? 0);
        }

        public static void PanelScroll(object? sender, ScrollEventArgs e)
        {
            if (sender is not ScrollableControl p || e.NewValue < 0)
                return;
            switch (e.ScrollOrientation)
            {
                case ScrollOrientation.HorizontalScroll:
                    p.HorizontalScroll.Value = Clamp(e.NewValue, p.HorizontalScroll);
                    break;
                case ScrollOrientation.VerticalScroll:
                    p.VerticalScroll.Value = Clamp(e.NewValue, p.VerticalScroll);
                    break;
                default:
                    throw new IndexOutOfRangeException(nameof(e.ScrollOrientation));
            }
            static int Clamp(int value, ScrollProperties prop) => Math.Max(prop.Minimum, Math.Min(prop.Maximum, value));
        }

        public static void DoubleBuffered(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            var pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            if (pi == null)
                throw new Exception(nameof(dgv));
            pi.SetValue(dgv, setting, null);
        }

        /// <summary>
        /// Initializes the <see cref="control"/> to be bound to a provided <see cref="ComboItem"/> list.
        /// </summary>
        /// <param name="control">Control to initialize binding</param>
        public static void InitializeBinding(this ListControl control)
        {
            control.DisplayMember = nameof(ComboItem.Text);
            control.ValueMember = nameof(ComboItem.Value);
        }

        public static void RemoveDropCB(object? sender, KeyEventArgs e)
        {
            if (sender == null)
                return;
            ((ComboBox)sender).DroppedDown = false;
        }

        /// <summary>
        /// Iterates the Control's child controls recursively to obtain all controls of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of control</typeparam>
        /// <param name="control"></param>
        /// <returns>All children and subchildren contained by <see cref="control"/>.</returns>
        public static IEnumerable<Control> GetAllControlsOfType<T>(Control control) where T : Control
        {
            foreach (var c in control.Controls.Cast<Control>())
            {
                if (c is T match)
                    yield return match;
                foreach (var sub in GetAllControlsOfType<T>(c))
                    yield return sub;
            }
        }

        /// <summary>
        /// Reads in custom extension types that allow the program to open more extensions.
        /// </summary>
        /// <param name="exts">Extensions to add</param>
        public static void AddSaveFileExtensions(IEnumerable<string> exts)
        {
            // Only add new (unique) extensions
            var dest = CustomSaveExtensions;
            foreach (var ext in exts)
            {
                if (!dest.Contains(ext))
                    dest.Add(ext);
            }
        }

        private static readonly List<string> CustomSaveExtensions = new()
        {
            // THESE ARE SAVE FILE EXTENSION TYPES. SAVE STATE (RAM SNAPSHOT) EXTENSIONS DO NOT GO HERE.
            "sav", // standard
            "dat", // VC data
            "gci", // Dolphin GameCubeImage
            "dsv", // DeSmuME
            "srm", // RetroArch save files
            "fla", // flashcard
            "SaveRAM", // BizHawk
        };

        public static bool IsFileExtensionSAV(string file) => CustomSaveExtensions.Contains(Path.GetExtension(file));

        private static string ExtraSaveExtensions => ";" + string.Join(";", CustomSaveExtensions.Select(z => $"*.{z}"));

        public static bool DetectSaveFileOnFileOpen { private get; set; } = true;

        /// <summary>
        /// Opens a dialog to open a <see cref="SaveFile"/>, <see cref="PKM"/> file, or any other supported file.
        /// </summary>
        /// <param name="extensions">Misc extensions of <see cref="PKM"/> files supported by the Save File.</param>
        /// <param name="path">Output result path</param>
        /// <returns>Result of whether or not a file is to be loaded from the output path.</returns>
        public static bool OpenSAVPKMDialog(IEnumerable<string> extensions, out string? path)
        {
            string supported = string.Join(";", extensions.Select(s => $"*.{s}").Concat(new[] { "*.pkm" }));
            using var ofd = new OpenFileDialog
            {
                Filter = "All Files|*.*" +
                         $"|Supported Files (*.*)|main;*.bin;{supported};*.bak" + ExtraSaveExtensions +
                         "|Save Files (*.sav)|main" + ExtraSaveExtensions +
                         "|Decrypted PKM File (*.pkm)|" + supported +
                         "|Binary File|*.bin" +
                         "|Backup File|*.bak"
            };

            // Detect main
            var msg = string.Empty;
            SaveFile? sav = null;
            if (DetectSaveFileOnFileOpen)
                sav = SaveFinder.FindMostRecentSaveFile(Environment.GetLogicalDrives(), ref msg);
            if (sav == null && !string.IsNullOrWhiteSpace(msg))
                Error(msg);

            if (sav != null)
                ofd.FileName = sav.Metadata.FileName;

            if (ofd.ShowDialog() != DialogResult.OK)
            {
                path = null;
                return false;
            }

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
            bool allowEncrypted = pk.Format >= 3 && pkx[0] == 'p';
            var genericFilter = $"Decrypted PKM File|*.{pkx}" +
                         (allowEncrypted ? $"|Encrypted PKM File|*.e{pkx[1..]}" : string.Empty) +
                         "|Binary File|*.bin" +
                         "|All Files|*.*";
            using var sfd = new SaveFileDialog
            {
                Filter = genericFilter,
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
            var data = $".{pkx}" == ext ? pk.DecryptedPartyData : pk.EncryptedPartyData;
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
        /// <param name="sav"><see cref="SaveFile"/> to be saved.</param>
        /// <param name="currentBox">Box the player will be greeted with when accessing the PC ingame.</param>
        /// <returns>Result of whether or not the file was saved.</returns>
        public static bool ExportSAVDialog(SaveFile sav, int currentBox = 0)
        {
            using var sfd = new SaveFileDialog
            {
                Filter = sav.Metadata.Filter,
                FileName = sav.Metadata.FileName,
                FilterIndex = 1000, // default to last, All Files
                RestoreDirectory = true
            };
            if (Directory.Exists(sav.Metadata.FileFolder))
                sfd.InitialDirectory = sav.Metadata.FileFolder;

            if (sfd.ShowDialog() != DialogResult.OK)
                return false;

            // Set box now that we're saving
            if (sav.HasBox)
                sav.CurrentBox = currentBox;

            var path = sfd.FileName;
            if (path == null)
                throw new NullReferenceException(nameof(sfd.FileName));

            ExportSAV(sav, path);
            return true;
        }

        private static void ExportSAV(SaveFile sav, string path)
        {
            var ext = Path.GetExtension(path).ToLower();
            var flags = sav.Metadata.GetSuggestedFlags(ext);

            try
            {
                File.WriteAllBytes(path, sav.Write(flags));
                sav.State.Edited = false;
                sav.Metadata.SetExtraInfo(path);
                Alert(MsgSaveExportSuccessPath, path);
            }
            catch (Exception x)
            {
                switch (x)
                {
                    case UnauthorizedAccessException:
                    case FileNotFoundException:
                    case IOException:
                        Error(MsgFileWriteFail + Environment.NewLine + x.Message, MsgFileWriteProtectedAdvice);
                        break;
                    default:
                        throw;
                }
            }
        }

        /// <summary>
        /// Opens a dialog to save a <see cref="MysteryGift"/> file.
        /// </summary>
        /// <param name="gift"><see cref="MysteryGift"/> to be saved.</param>
        /// <param name="origin">Game the gift originates from</param>
        /// <returns>Result of whether or not the file was saved.</returns>
        public static bool ExportMGDialog(DataMysteryGift gift, GameVersion origin)
        {
            using var sfd = new SaveFileDialog
            {
                Filter = GetMysterGiftFilter(gift.Generation, origin),
                FileName = Util.CleanFileName(gift.FileName)
            };
            if (sfd.ShowDialog() != DialogResult.OK)
                return false;

            string path = sfd.FileName;
            SaveBackup(path);

            File.WriteAllBytes(path, gift.Write());
            return true;
        }

        /// <summary>
        /// Gets the File Dialog filter for a Mystery Gift I/O operation.
        /// </summary>
        /// <param name="format">Format specifier for the </param>
        /// <param name="origin">Game the format originated from/to</param>
        public static string GetMysterGiftFilter(int format, GameVersion origin) => format switch
        {
            4 => "Gen4 Mystery Gift|*.pgt;*.pcd;*.wc4" + all,
            5 => "Gen5 Mystery Gift|*.pgf" + all,
            6 => "Gen6 Mystery Gift|*.wc6;*.wc6full" + all,
            7 => GameVersion.GG.Contains(origin)
                ? "Beluga Gift Record|*.wr7" + all
                : "Gen7 Mystery Gift|*.wc7;*.wc7full" + all,
            _ => string.Empty,
        };

        private const string all = "|All Files|*.*";
    }
}
