using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_Trainer8a : Form
{
    private readonly SaveFile Origin;
    private readonly SAV8LA SAV;

    public SAV_Trainer8a(SAV8LA sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV8LA)(Origin = sav).Clone();
        if (Main.Unicode)
            TB_OTName.Font = FontUtil.GetPKXFont();

        B_MaxCash.Click += (_, _) => MT_Money.Text = SAV.MaxMoney.ToString();

        CB_Gender.Items.Clear();
        CB_Gender.Items.AddRange(Main.GenderSymbols.Take(2).ToArray()); // m/f depending on unicode selection

        GetComboBoxes();
        GetTextBoxes();
    }

    private void GetComboBoxes()
    {
        CB_Language.InitializeBinding();
        CB_Language.DataSource = GameInfo.LanguageDataSource(SAV.Generation);
    }

    private void GetTextBoxes()
    {
        // Get Data
        CB_Gender.SelectedIndex = SAV.Gender;

        // Display Data
        TB_OTName.Text = SAV.OT;
        trainerID1.LoadIDValues(SAV, SAV.Generation);
        MT_Money.Text = SAV.Money.ToString();
        CB_Language.SelectedValue = SAV.Language;

        // Load Play Time
        MT_Hours.Text = SAV.PlayedHours.ToString();
        MT_Minutes.Text = SAV.PlayedMinutes.ToString();
        MT_Seconds.Text = SAV.PlayedSeconds.ToString();

        if (SAV.LastSaved.LastSavedDate is { } d)
            CAL_LastSavedDate.Value = CAL_LastSavedTime.Value = d;
        else
            CAL_LastSavedDate.Enabled = CAL_LastSavedTime.Enabled = false;

        CAL_AdventureStartDate.Value = CAL_AdventureStartTime.Value = SAV.AdventureStart.Timestamp;

        LoadClamp(NUD_MeritCurrent, SaveBlockAccessor8LA.KMeritCurrent);
        LoadClamp(NUD_MeritEarned, SaveBlockAccessor8LA.KMeritEarnedTotal);
        LoadClamp(NUD_Rank, SaveBlockAccessor8LA.KExpeditionTeamRank);
        LoadClamp(NUD_Satchel, SaveBlockAccessor8LA.KSatchelUpgrades);
    }

    private void LoadClamp(NumericUpDown nud, uint key)
    {
        var actual = (uint)SAV.Blocks.GetBlockValue(key);
        nud.Value = Math.Min(actual, nud.Maximum);
    }

    private void Save()
    {
        SaveTrainerInfo();
    }

    private void SaveTrainerInfo()
    {
        SAV.Gender = (byte)CB_Gender.SelectedIndex;

        SAV.Money = Util.ToUInt32(MT_Money.Text);
        SAV.Language = WinFormsUtil.GetIndex(CB_Language);
        SAV.OT = TB_OTName.Text;

        // Save PlayTime
        SAV.PlayedHours = ushort.Parse(MT_Hours.Text);
        SAV.PlayedMinutes = ushort.Parse(MT_Minutes.Text)%60;
        SAV.PlayedSeconds = ushort.Parse(MT_Seconds.Text)%60;

        var advDay = CAL_AdventureStartDate.Value.Date;
        SAV.AdventureStart.Timestamp = advDay.AddSeconds(CAL_AdventureStartTime.Value.TimeOfDay.TotalSeconds);
        if (CAL_LastSavedDate.Enabled)
            SAV.LastSaved.LastSavedDate = CAL_LastSavedDate.Value.Date.AddSeconds(CAL_LastSavedTime.Value.TimeOfDay.TotalSeconds);

        SAV.Blocks.SetBlockValue(SaveBlockAccessor8LA.KMeritCurrent, (uint)NUD_MeritCurrent.Value);
        SAV.Blocks.SetBlockValue(SaveBlockAccessor8LA.KMeritEarnedTotal, (uint)NUD_MeritEarned.Value);
        SAV.Blocks.SetBlockValue(SaveBlockAccessor8LA.KExpeditionTeamRank, (uint)NUD_Rank.Value);
        SAV.Blocks.SetBlockValue(SaveBlockAccessor8LA.KSatchelUpgrades, (uint)NUD_Satchel.Value);
    }

    private void ClickOT(object sender, MouseEventArgs e)
    {
        TextBox tb = sender as TextBox ?? TB_OTName;
        // Special Character Form
        if (ModifierKeys != Keys.Control)
            return;

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
}
