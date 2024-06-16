using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_Trainer8 : Form
{
    private readonly SaveFile Origin;
    private readonly SAV8SWSH SAV;

    public SAV_Trainer8(SAV8SWSH sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV8SWSH)(Origin = sav).Clone();
        //Loading = true;
        if (Main.Unicode)
        {
            TB_OTName.Font = TB_TrainerCardName.Font = FontUtil.GetPKXFont();
        }

        B_MaxCash.Click += (_, _) => MT_Money.Text = SAV.MaxMoney.ToString();
        B_MaxWatt.Click += (_, _) => MT_Watt.Text = MyStatus8.MaxWatt.ToString();

        CB_Gender.Items.Clear();
        CB_Gender.Items.AddRange(Main.GenderSymbols.Take(2).ToArray()); // m/f depending on unicode selection

        TrainerStats.LoadRecords(SAV, RecordLists.RecordList_8);

        NUD_BP.Value = Math.Min(SAV.Misc.BP, 9999);
        GetComboBoxes();
        GetTextBoxes();
        GetMiscValues();

        ChangeTitleScreenIndex(this, EventArgs.Empty);
        ChangeTrainerCardIndex(this, EventArgs.Empty);

        if (SAV.SaveRevision == 0)
            B_CollectDiglett.Visible = false;

        //Loading = false;
    }

    // private readonly bool Loading;
    private bool MapUpdated;

    private void GetComboBoxes()
    {
        CB_Language.InitializeBinding();
        CB_Language.DataSource = GameInfo.LanguageDataSource(SAV.Generation);
    }

    private void GetTextBoxes()
    {
        // Get Data
        CB_Game.SelectedIndex = SAV.Version - GameVersion.SW;
        CB_Gender.SelectedIndex = SAV.Gender;

        // Display Data
        TB_OTName.Text = SAV.OT;
        TB_TrainerCardName.Text = SAV.Blocks.TrainerCard.OT;
        TB_TrainerCardNumber.Text = SAV.Blocks.TrainerCard.Number;
        MT_TrainerCardID.Text = SAV.Blocks.TrainerCard.TrainerID.ToString("000000");
        MT_RotoRally.Text = SAV.Blocks.TrainerCard.RotoRallyScore.ToString();
        trainerID1.LoadIDValues(SAV, SAV.Generation);
        MT_Money.Text = SAV.Money.ToString();
        MT_Watt.Text = SAV.MyStatus.Watt.ToString();
        CB_Language.SelectedValue = SAV.Language;

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

        if (SAV.Played.LastSavedDate.HasValue)
            CAL_LastSavedDate.Value = CAL_LastSavedTime.Value = SAV.Played.LastSavedDate.Value;
        else
            L_LastSaved.Visible = CAL_LastSavedDate.Visible = CAL_LastSavedTime.Visible = false;

        CAL_AdventureStartTime.Visible = false;
        CAL_AdventureStartDate.Value = new DateTime(SAV.TrainerCard.StartedYear, SAV.TrainerCard.StartedMonth, SAV.TrainerCard.StartedDay);

        L_Fame.Visible = CAL_HoFDate.Visible = CAL_HoFTime.Visible = false;
        // DateUtil.GetDateTime2000(SAV.SecondsToFame, out date, out time);
        // CAL_HoFDate.Value = date;
        // CAL_HoFTime.Value = time;
    }

    private void GetMiscValues()
    {
        MT_BattleTowerSinglesWin.Text = SAV.GetValue<uint>(SaveBlockAccessor8SWSH.KBattleTowerSinglesVictory).ToString();
        MT_BattleTowerDoublesWin.Text = SAV.GetValue<uint>(SaveBlockAccessor8SWSH.KBattleTowerDoublesVictory).ToString();
        MT_BattleTowerSinglesStreak.Text = SAV.GetValue<ushort>(SaveBlockAccessor8SWSH.KBattleTowerSinglesStreak).ToString();
        MT_BattleTowerDoublesStreak.Text = SAV.GetValue<ushort>(SaveBlockAccessor8SWSH.KBattleTowerDoublesStreak).ToString();
    }

    private void SaveMiscValues()
    {
        var singles = Math.Min(9_999_999u, Util.ToUInt32(MT_BattleTowerSinglesWin.Text));
        var doubles = Math.Min(9_999_999u, Util.ToUInt32(MT_BattleTowerDoublesWin.Text));
        SAV.SetValue(SaveBlockAccessor8SWSH.KBattleTowerSinglesVictory, singles);
        SAV.SetValue(SaveBlockAccessor8SWSH.KBattleTowerDoublesVictory, doubles);
        SAV.SetValue(SaveBlockAccessor8SWSH.KBattleTowerSinglesStreak, (ushort)Math.Min(300, Util.ToUInt32(MT_BattleTowerSinglesStreak.Text)));
        SAV.SetValue(SaveBlockAccessor8SWSH.KBattleTowerDoublesStreak, (ushort)Math.Min(300, Util.ToUInt32(MT_BattleTowerDoublesStreak.Text)));

        SAV.SetRecord(RecordLists.G8BattleTowerSingleWin, (int)singles);
        SAV.SetRecord(RecordLists.G8BattleTowerDoubleWin, (int)doubles);
    }

    private void Save()
    {
        SaveTrainerInfo();
        SaveMiscValues();
    }

    private void SaveTrainerInfo()
    {
        SAV.Version = (GameVersion)(CB_Game.SelectedIndex + (int)GameVersion.SW);
        SAV.Gender = (byte)CB_Gender.SelectedIndex;

        SAV.Money = Util.ToUInt32(MT_Money.Text);
        SAV.Language = WinFormsUtil.GetIndex(CB_Language);
        SAV.OT = TB_OTName.Text;
        SAV.Blocks.TrainerCard.OT = TB_TrainerCardName.Text;
        SAV.Blocks.MyStatus.Number = SAV.Blocks.TrainerCard.Number = TB_TrainerCardNumber.Text;
        SAV.Blocks.TrainerCard.TrainerID = Util.ToInt32(MT_TrainerCardID.Text);
        SAV.Blocks.TrainerCard.RotoRallyScore = Util.ToInt32(MT_RotoRally.Text);

        var watt = Util.ToUInt32(MT_Watt.Text);
        SAV.MyStatus.Watt = watt;
        if (SAV.GetRecord(Record8.WattTotal) < watt)
            SAV.SetRecord(Record8.WattTotal, (int)watt);

        SAV.Misc.BP = (int)NUD_BP.Value;

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

        SAV.TrainerCard.StartedYear = (ushort)CAL_AdventureStartDate.Value.Year;
        SAV.TrainerCard.StartedMonth = (byte)CAL_AdventureStartDate.Value.Month;
        SAV.TrainerCard.StartedDay = (byte)CAL_AdventureStartDate.Value.Day;

        //SAV.SecondsToFame = (uint)DateUtil.GetSecondsFrom2000(CAL_HoFDate.Value, CAL_HoFTime.Value);

        if (SAV.Played.LastSavedDate.HasValue)
            SAV.Played.LastSavedDate = CAL_LastSavedDate.Value.Date.AddMinutes(CAL_LastSavedTime.Value.TimeOfDay.TotalMinutes);
    }

    private void ClickOT(object sender, MouseEventArgs e)
    {
        TextBox tb = sender as TextBox ?? TB_OTName;
        // Special Character Form
        if (ModifierKeys != Keys.Control)
            return;

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
        //if (!Loading)
        MapUpdated = true;
    }

    private void ChangeTrainerCardIndex(object sender, EventArgs e)
    {
        var index = (int)NUD_ShowTrainerCard.Value - 1;
        PG_ShowTrainerCard.SelectedObject = SAV.Blocks.TrainerCard.ViewPoke(index);
    }

    private void ChangeTitleScreenIndex(object sender, EventArgs e)
    {
        var index = (int)NUD_ShowTitleScreen.Value - 1;
        PG_ShowTitleScreen.SelectedObject = SAV.Blocks.TitleScreen.ViewPoke(index);
    }

    private void B_CopyFromPartyToTrainerCard_Click(object sender, EventArgs e)
    {
        SAV.Blocks.TrainerCard.SetPartyData();
        System.Media.SystemSounds.Asterisk.Play();
        ChangeTrainerCardIndex(this, EventArgs.Empty);
    }

    private void B_CopyFromPartyToTitleScreen_Click(object sender, EventArgs e)
    {
        SAV.Blocks.TitleScreen.SetPartyData();
        System.Media.SystemSounds.Asterisk.Play();
        ChangeTitleScreenIndex(this, EventArgs.Empty);
    }

    private void B_GetAllDiglett_Click(object sender, EventArgs e)
    {
        SAV.UnlockAllDiglett();
        System.Media.SystemSounds.Asterisk.Play();
    }
}
