using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_Trainer7GG : Form
{
    private readonly SaveFile Origin;
    private readonly SAV7b SAV;
    private readonly GoParkStorage Park;

    public SAV_Trainer7GG(SAV7b sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV7b)(Origin = sav).Clone();
        Park = SAV.Park;
        UpdateGoSummary(0);

        if (Main.Unicode)
        {
            TB_OTName.Font = TB_RivalName.Font = FontUtil.GetPKXFont();
        }

        B_MaxCash.Click += (_, _) => MT_Money.Text = "9,999,999";

        GetComboBoxes();
        LoadTrainerInfo();
    }

    private bool MapUpdated;

    // Drag & Drop Events
    private void Main_DragEnter(object? sender, DragEventArgs? e)
    {
        if (e is null)
            return;
        if (e.AllowedEffect == (DragDropEffects.Copy | DragDropEffects.Link)) // external file
            e.Effect = DragDropEffects.Copy;
        else if (e.Data != null) // within
            e.Effect = DragDropEffects.Move;
    }

    private void Main_DragDrop(object? sender, DragEventArgs? e)
    {
        if (e?.Data?.GetData(DataFormats.FileDrop) is not string[] { Length: not 0 } files)
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

        CB_Game.SelectedValue = (int)SAV.Version;
        CB_Gender.SelectedIndex = SAV.Gender;
        trainerID1.LoadIDValues(SAV, SAV.Generation);

        NUD_M.Value = SAV.Coordinates.M;
        // Sanity Check Map Coordinates
        try
        {
            NUD_X.Value = (decimal)(double)SAV.Coordinates.X;
            NUD_Z.Value = (decimal)(double)SAV.Coordinates.Z;
            NUD_Y.Value = (decimal)(double)SAV.Coordinates.Y;
            NUD_SX.Value = (decimal)(double)SAV.Coordinates.SX;
            NUD_SZ.Value = (decimal)(double)SAV.Coordinates.SZ;
            NUD_SY.Value = (decimal)(double)SAV.Coordinates.SY;
            NUD_R.Value = (decimal)(Math.Atan2(SAV.Coordinates.RZ, SAV.Coordinates.RW) * 360.0 / Math.PI);
        }
        catch { GB_Map.Enabled = false; }

        // Load Play Time
        MT_Hours.Text = SAV.PlayedHours.ToString();
        MT_Minutes.Text = SAV.PlayedMinutes.ToString();
        MT_Seconds.Text = SAV.PlayedSeconds.ToString();

        CAL_AdventureBeginDate.Value = CAL_AdventureBeginTime.Value = SAV.PlayerGeoLocation.AdventureBegin.Timestamp;
        if (SAV.Played.LastSavedDate is { } d)
            CAL_LastSavedDate.Value = CAL_LastSavedTime.Value = d;
        else
            CAL_LastSavedDate.Enabled = CAL_LastSavedTime.Enabled = false;
    }

    private void Save()
    {
        SaveTrainerInfo();
    }

    private void SaveTrainerInfo()
    {
        SAV.Version = (GameVersion)WinFormsUtil.GetIndex(CB_Game);
        SAV.Gender = (byte)CB_Gender.SelectedIndex;

        SAV.Money = Util.ToUInt32(MT_Money.Text);
        SAV.Language = WinFormsUtil.GetIndex(CB_Language);

        SAV.OT = TB_OTName.Text;
        SAV.Blocks.Misc.Rival = TB_RivalName.Text;

        // Copy Position
        if (GB_Map.Enabled && MapUpdated)
        {
            SAV.Coordinates.M = (ulong)NUD_M.Value;
            SAV.Coordinates.X = (float)NUD_X.Value;
            SAV.Coordinates.Z = (float)NUD_Z.Value;
            SAV.Coordinates.Y = (float)NUD_Y.Value;
            SAV.Coordinates.SX = (float)NUD_SX.Value;
            SAV.Coordinates.SZ = (float)NUD_SZ.Value;
            SAV.Coordinates.SY = (float)NUD_SY.Value;
            var angle = (double)NUD_R.Value * Math.PI / 360.0;
            SAV.Coordinates.RX = 0;
            SAV.Coordinates.RZ = (float)Math.Sin(angle);
            SAV.Coordinates.RY = 0;
            SAV.Coordinates.RW = (float)Math.Cos(angle);
        }

        // Save PlayTime
        SAV.PlayedHours = ushort.Parse(MT_Hours.Text);
        SAV.PlayedMinutes = ushort.Parse(MT_Minutes.Text) % 60;
        SAV.PlayedSeconds = ushort.Parse(MT_Seconds.Text) % 60;

        SAV.PlayerGeoLocation.AdventureBegin.Timestamp = CAL_AdventureBeginDate.Value.Date.AddSeconds(CAL_AdventureBeginTime.Value.TimeOfDay.TotalSeconds);
        if (CAL_LastSavedDate.Enabled)
            SAV.Played.LastSavedDate = CAL_LastSavedDate.Value.Date.AddSeconds(CAL_LastSavedTime.Value.TimeOfDay.TotalSeconds);
    }

    private void ClickString(object sender, MouseEventArgs e)
    {
        if (ModifierKeys != Keys.Control)
            return;

        TextBox tb = sender as TextBox ?? TB_OTName;

        // Special Character Form
        var d = new TrashEditor(tb, SAV, SAV.Generation);
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

    private void ChangeMapValue(object sender, EventArgs e)
    {
        MapUpdated = true;
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
            File.WriteAllBytes(Path.Combine(folder, Util.CleanFileName(gpk.FileName)), gpk.Data.ToArray());
        WinFormsUtil.Alert($"Dumped {gofiles.Length} files to {folder}");
    }

    private void B_Import_Click(object sender, EventArgs e)
    {
        using var sfd = new OpenFileDialog();
        sfd.Filter = GoFilter;
        sfd.FilterIndex = 0;
        sfd.RestoreDirectory = true;

        // Export
        if (sfd.ShowDialog() != DialogResult.OK)
            return;

        string path = sfd.FileName;
        ImportGP1From(path);
    }

    private void ImportGP1From(string path)
    {
        int index = (int)NUD_GoIndex.Value;
        const int max = GoParkStorage.Count - 1;
        index = Math.Clamp(index, 0, max);
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
        data.CopyTo(gp1.Data);
        Park[index] = gp1;
        UpdateGoSummary((int)NUD_GoIndex.Value);
    }

    private const string GoFilter = "Go Park Entity |*.gp1|All Files|*.*";

    private void B_Export_Click(object sender, EventArgs e)
    {
        int index = (int)NUD_GoIndex.Value;
        const int max = GoParkStorage.Count - 1;
        index = Math.Clamp(index, 0, max);
        var data = Park[index];

        using var sfd = new SaveFileDialog();
        sfd.FileName = data.FileName;
        sfd.Filter = GoFilter;
        sfd.FilterIndex = 0;
        sfd.RestoreDirectory = true;

        if (sfd.ShowDialog() != DialogResult.OK)
            return;

        File.WriteAllBytes(sfd.FileName, data.Data.ToArray());
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
        UpdateGoSummary((int)NUD_GoIndex.Value);
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void NUD_GoIndex_ValueChanged(object sender, EventArgs e) => UpdateGoSummary((int)NUD_GoIndex.Value);

    private void UpdateGoSummary(int index)
    {
        const int max = GoParkStorage.Count - 1;
        index = Math.Clamp(index, 0, max);
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
        UpdateGoSummary((int)NUD_GoIndex.Value);
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_DeleteGo_Click(object sender, EventArgs e)
    {
        int index = (int)NUD_GoIndex.Value;
        const int max = GoParkStorage.Count - 1;
        index = Math.Clamp(index, 0, max);
        Park[index] = new GP1();
        UpdateGoSummary((int)NUD_GoIndex.Value);
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_AllTrainerTitles_Click(object sender, EventArgs e)
    {
        SAV.Blocks.EventWork.UnlockAllTitleFlags();
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_AllFashionItems_Click(object sender, EventArgs e)
    {
        SAV.Blocks.FashionPlayer.UnlockAllAccessoriesPlayer();
        SAV.Blocks.FashionStarter.UnlockAllAccessoriesStarter();
        System.Media.SystemSounds.Asterisk.Play();
    }
}
