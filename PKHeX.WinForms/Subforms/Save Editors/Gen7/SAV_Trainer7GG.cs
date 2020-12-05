using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_Trainer7GG : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV7b SAV;
        private readonly GoParkStorage Park;

        public SAV_Trainer7GG(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV7b)(Origin = sav).Clone();
            Park = new GoParkStorage(SAV);
            UpdateGoSummary(0);

            if (Main.Unicode)
            {
                TB_OTName.Font = TB_RivalName.Font = FontUtil.GetPKXFont();
            }

            B_MaxCash.Click += (sender, e) => MT_Money.Text = "9,999,999";

            GetComboBoxes();
            LoadTrainerInfo();
        }

        // Drag & Drop Events
        private void Main_DragEnter(object sender, DragEventArgs e)
        {
            if (e.AllowedEffect == (DragDropEffects.Copy | DragDropEffects.Link)) // external file
                e.Effect = DragDropEffects.Copy;
            else if (e.Data != null) // within
                e.Effect = DragDropEffects.Move;
        }

        private void Main_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[]?)e.Data.GetData(DataFormats.FileDrop);
            if (files == null || files.Length == 0)
                return;
            ImportGP1From(files[0]);
            e.Effect = DragDropEffects.Copy;
        }

        private void GetComboBoxes()
        {
            CB_Gender.Items.Clear();
            CB_Gender.Items.AddRange(Main.GenderSymbols.Take(2).ToArray()); // m/f depending on unicode selection
            CB_Language.InitializeBinding();
            CB_Language.DataSource = GameInfo.LanguageDataSource(SAV.Generation);
            CB_Game.InitializeBinding();
            CB_Game.DataSource = new BindingSource(GameInfo.VersionDataSource.Where(z => GameVersion.Gen7b.Contains(z.Value)).ToList(), null);
        }

        private void LoadTrainerInfo()
        {
            // Get Data
            TB_OTName.Text = SAV.OT;
            TB_RivalName.Text = SAV.Blocks.Misc.Rival;
            CB_Language.SelectedValue = SAV.Language;
            MT_Money.Text = SAV.Blocks.Misc.Money.ToString();

            CB_Game.SelectedValue = SAV.Game;
            CB_Gender.SelectedIndex = SAV.Gender;
            trainerID1.LoadIDValues(SAV);

            // Load Play Time
            MT_Hours.Text = SAV.PlayedHours.ToString();
            MT_Minutes.Text = SAV.PlayedMinutes.ToString();
            MT_Seconds.Text = SAV.PlayedSeconds.ToString();
        }

        private void Save()
        {
            SaveTrainerInfo();
        }

        private void SaveTrainerInfo()
        {
            SAV.Game = WinFormsUtil.GetIndex(CB_Game);
            SAV.Gender = (byte)CB_Gender.SelectedIndex;

            SAV.Money = Util.ToUInt32(MT_Money.Text);
            SAV.Language = WinFormsUtil.GetIndex(CB_Language);

            SAV.OT = TB_OTName.Text;
            SAV.Blocks.Misc.Rival = TB_RivalName.Text;

            // Save PlayTime
            SAV.PlayedHours = ushort.Parse(MT_Hours.Text);
            SAV.PlayedMinutes = ushort.Parse(MT_Minutes.Text)%60;
            SAV.PlayedSeconds = ushort.Parse(MT_Seconds.Text)%60;
        }

        private void ClickString(object sender, MouseEventArgs e)
        {
            if (ModifierKeys != Keys.Control)
                return;

            TextBox tb = sender as TextBox ?? TB_OTName;

            // Special Character Form
            var d = new TrashEditor(tb, SAV);
            d.ShowDialog();
            tb.Text = d.FinalString;
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            Save();
            Origin.CopyChangesFrom(SAV);
            Close();
        }

        private void Change255(object sender, EventArgs e)
        {
            MaskedTextBox box = (MaskedTextBox)sender;
            if (box.Text.Length == 0) box.Text = "0";
            if (Util.ToInt32(box.Text) > 255) box.Text = "255";
        }

        private void B_ExportGoSummary_Click(object sender, EventArgs e)
        {
            var summary = Park.DumpAll(GameInfo.Strings.Species).ToArray();
            if (summary.Length == 0)
            {
                WinFormsUtil.Alert("No entities present in Go Park to dump.");
                return;
            }
            WinFormsUtil.SetClipboardText(string.Join(Environment.NewLine, summary));
            System.Media.SystemSounds.Asterisk.Play();
        }

        private void B_ExportGoFiles_Click(object sender, EventArgs e)
        {
            var gofiles = Park.GetAllEntities().Where(z => z.Species != 0).ToArray();
            if (gofiles.Length == 0)
            {
                WinFormsUtil.Alert("No entities present in Go Park to dump.");
                return;
            }
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK)
                return;

            var folder = fbd.SelectedPath;
            foreach (var gpk in gofiles)
                File.WriteAllBytes(Path.Combine(folder, Util.CleanFileName(gpk.FileName)), gpk.Data);
            WinFormsUtil.Alert($"Dumped {gofiles.Length} files to {folder}");
        }

        private void B_Import_Click(object sender, EventArgs e)
        {
            using var sfd = new OpenFileDialog
            {
                Filter = GoFilter,
                FilterIndex = 0,
                RestoreDirectory = true
            };

            // Export
            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            string path = sfd.FileName;
            ImportGP1From(path);
        }

        private void ImportGP1From(string path)
        {
            int index = (int)NUD_GoIndex.Value;
            index = Math.Min(GoParkStorage.Count - 1, Math.Max(0, index));
            ImportGP1From(path, index);
        }

        private void ImportGP1From(string path, int index)
        {
            var data = File.ReadAllBytes(path);
            if (data.Length != GP1.SIZE)
            {
                WinFormsUtil.Error(MessageStrings.MsgFileLoadIncompatible);
                return;
            }
            var gp1 = new GP1();
            data.CopyTo(gp1.Data, 0);
            Park[index] = gp1;
            UpdateGoSummary((int)NUD_GoIndex.Value);
        }

        private const string GoFilter = "Go Park Entity |*.gp1|All Files|*.*";

        private void B_Export_Click(object sender, EventArgs e)
        {
            int index = (int)NUD_GoIndex.Value;
            index = Math.Min(GoParkStorage.Count - 1, Math.Max(0, index));
            var data = Park[index];

            using var sfd = new SaveFileDialog
            {
                FileName = data.FileName,
                Filter = GoFilter,
                FilterIndex = 0,
                RestoreDirectory = true
            };

            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            File.WriteAllBytes(sfd.FileName, data.Data);
        }

        private void B_ImportGoFiles_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK)
                return;

            IEnumerable<string> files = Directory.GetFiles(fbd.SelectedPath);
            files = files.Where(z => Path.GetExtension(z) == ".gp1" && new FileInfo(z).Length == GP1.SIZE);

            int ctr = (int)NUD_GoIndex.Value;
            foreach (var f in files)
            {
                while (true)
                {
                    if (ctr >= GoParkStorage.Count)
                        return;
                    if (Park[ctr].Species != 0)
                        ctr++;
                    else
                        break;
                }
                var data = File.ReadAllBytes(f);
                Park[ctr] = new GP1(data);
                ctr++;
            }
            UpdateGoSummary((int) NUD_GoIndex.Value);
            System.Media.SystemSounds.Asterisk.Play();
        }

        private void NUD_GoIndex_ValueChanged(object sender, EventArgs e) => UpdateGoSummary((int)NUD_GoIndex.Value);

        private void UpdateGoSummary(int index)
        {
            index = Math.Min(GoParkStorage.Count - 1, Math.Max(0, index));
            int area = index / GoParkStorage.SlotsPerArea;
            int slot = index % GoParkStorage.SlotsPerArea;

            var data = Park[index];
            var prefix = $"Area: {area + 1:00}, Slot: {slot + 1:00}{Environment.NewLine}";
            var dump = data.Species == 0 ? "Empty" : data.Dump(GameInfo.Strings.Species, index);
            L_GoSlotSummary.Text = prefix + dump;
        }

        private void B_DeleteAll_Click(object sender, EventArgs e)
        {
            if (WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Delete all slots?") != DialogResult.Yes)
                return;

            Park.DeleteAll();
            UpdateGoSummary((int) NUD_GoIndex.Value);
            System.Media.SystemSounds.Asterisk.Play();
        }

        private void B_DeleteGo_Click(object sender, EventArgs e)
        {
            int index = (int)NUD_GoIndex.Value;
            index = Math.Min(GoParkStorage.Count - 1, Math.Max(0, index));
            Park[index] = new GP1();
            UpdateGoSummary((int)NUD_GoIndex.Value);
            System.Media.SystemSounds.Asterisk.Play();
        }
    }
}
