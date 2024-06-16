using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_Trainer8b : Form
{
    private readonly SaveFile Origin;
    private readonly SAV8BS SAV;

    private readonly bool Loading;
    private bool MapUpdated;

    public SAV_Trainer8b(SAV8BS sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV8BS)(Origin = sav).Clone();
        Loading = true;
        if (Main.Unicode)
        {
            TB_OTName.Font = TB_Rival.Font = FontUtil.GetPKXFont();
        }

        B_MaxCash.Click += (_, _) => MT_Money.Text = SAV.MaxMoney.ToString();

        CB_Gender.Items.Clear();
        CB_Gender.Items.AddRange(Main.GenderSymbols.Take(2).ToArray()); // m/f depending on unicode selection

        GetComboBoxes();
        GetTextBoxes();

        TrainerStats.LoadRecords(SAV, Record8b.RecordList_8b);

        Loading = false;
    }

    private void GetComboBoxes()
    {
        CB_Language.InitializeBinding();
        CB_Language.DataSource = GameInfo.LanguageDataSource(SAV.Generation);
    }

    private void GetTextBoxes()
    {
        // Get Data
        CB_Game.SelectedIndex = Math.Clamp((byte)SAV.Version - (byte)GameVersion.BD, 0, 1);
        CB_Gender.SelectedIndex = SAV.Gender;

        NUD_BP.Value = SAV.BattleTower.BP;

        // Display Data
        TB_OTName.Text = SAV.OT;
        trainerID1.LoadIDValues(SAV, SAV.Generation);
        MT_Money.Text = SAV.Money.ToString();
        CB_Language.SelectedValue = SAV.Language;
        TB_Rival.Text = SAV.Rival;

        NUD_M.Value = SAV.ZoneID;
        NUD_X.Value = SAV.MyStatus.X;
        NUD_Z.Value = (decimal)SAV.MyStatus.Height;
        NUD_Y.Value = SAV.MyStatus.Y;
        NUD_R.Value = (decimal)SAV.MyStatus.Rotation;

        // Load Play Time
        MT_Hours.Text = SAV.PlayedHours.ToString();
        MT_Minutes.Text = SAV.PlayedMinutes.ToString();
        MT_Seconds.Text = SAV.PlayedSeconds.ToString();

        CAL_LastSavedDate.Value = SAV.System.LocalTimestampLatest;
        CAL_LastSavedTime.Value = SAV.System.LocalTimestampLatest;

        CAL_AdventureStartDate.Value = SAV.System.LocalTimestampStart;
        CAL_AdventureStartTime.Value = SAV.System.LocalTimestampStart;

        L_Fame.Visible = CAL_HoFDate.Visible = CAL_HoFTime.Visible = false;
        // DateUtil.GetDateTime2000(SAV.SecondsToFame, out date, out time);
        // CAL_HoFDate.Value = date;
        // CAL_HoFTime.Value = time;

        CHK_Badge1.Checked = SAV.FlagWork.GetSystemFlag(124);
        CHK_Badge2.Checked = SAV.FlagWork.GetSystemFlag(125);
        CHK_Badge3.Checked = SAV.FlagWork.GetSystemFlag(126);
        CHK_Badge4.Checked = SAV.FlagWork.GetSystemFlag(127);
        CHK_Badge5.Checked = SAV.FlagWork.GetSystemFlag(128);
        CHK_Badge6.Checked = SAV.FlagWork.GetSystemFlag(129);
        CHK_Badge7.Checked = SAV.FlagWork.GetSystemFlag(130);
        CHK_Badge8.Checked = SAV.FlagWork.GetSystemFlag(131);
    }

    private void Save()
    {
        SaveTrainerInfo();
        if (SAV is { TID16: 0, SID16: 0 })
            SAV.SID16 = 1; // Cannot have an all-zero ID.

        // Trickle down the changes to the extra record block.
        if (SAV.HasFirstSaveFileExpansion && (SAV.OT != Origin.OT || SAV.TID16 != Origin.TID16 || SAV.SID16 != Origin.SID16))
            SAV.RecordAdd.ReplaceOT(Origin, SAV);
    }

    private void SaveTrainerInfo()
    {
        SAV.Version = (GameVersion)(CB_Game.SelectedIndex + (int)GameVersion.BD);
        SAV.Gender = (byte)CB_Gender.SelectedIndex;

        SAV.Money = Util.ToUInt32(MT_Money.Text);
        SAV.Language = WinFormsUtil.GetIndex(CB_Language);
        SAV.OT = TB_OTName.Text;
        SAV.Rival = TB_Rival.Text;
        SAV.BattleTower.BP = (uint)NUD_BP.Value;

        // Copy Position
        if (GB_Map.Enabled && MapUpdated)
        {
            SAV.ZoneID = (short)NUD_M.Value;
            SAV.MyStatus.X = (int)NUD_X.Value;
            SAV.MyStatus.Height = (float)NUD_Z.Value;
            SAV.MyStatus.Y = (int)NUD_Y.Value;
            SAV.MyStatus.Rotation = (float)NUD_R.Value;
            //SAV.Situation.UpdateOverworldCoordinates();
        }

        // Save PlayTime
        SAV.PlayedHours = ushort.Parse(MT_Hours.Text);
        SAV.PlayedMinutes = ushort.Parse(MT_Minutes.Text) % 60;
        SAV.PlayedSeconds = ushort.Parse(MT_Seconds.Text) % 60;

        //SAV.SecondsToStart = (uint)DateUtil.GetSecondsFrom2000(CAL_AdventureStartDate.Value, CAL_AdventureStartTime.Value);
        //SAV.SecondsToFame = (uint)DateUtil.GetSecondsFrom2000(CAL_HoFDate.Value, CAL_HoFTime.Value);

        SAV.System.LocalTimestampStart = ReviseTimestamp(SAV.System.LocalTimestampStart, CAL_AdventureStartDate.Value, CAL_AdventureStartTime.Value);
        SAV.System.LocalTimestampLatest = ReviseTimestamp(SAV.System.LocalTimestampLatest, CAL_LastSavedDate.Value, CAL_LastSavedTime.Value);

        SAV.FlagWork.SetSystemFlag(124, CHK_Badge1.Checked);
        SAV.FlagWork.SetSystemFlag(125, CHK_Badge2.Checked);
        SAV.FlagWork.SetSystemFlag(126, CHK_Badge3.Checked);
        SAV.FlagWork.SetSystemFlag(127, CHK_Badge4.Checked);
        SAV.FlagWork.SetSystemFlag(128, CHK_Badge5.Checked);
        SAV.FlagWork.SetSystemFlag(129, CHK_Badge6.Checked);
        SAV.FlagWork.SetSystemFlag(130, CHK_Badge7.Checked);
        SAV.FlagWork.SetSystemFlag(131, CHK_Badge8.Checked);
    }

    private static DateTime ReviseTimestamp(DateTime original, DateTime date, DateTime time)
    {
        var revised = date.Date + time.TimeOfDay;
        return revised.AddTicks(original.Ticks % TimeSpan.TicksPerSecond); // keep the ticks consistent
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
        if (!Loading)
            MapUpdated = true;
    }

    //private string UpdateTip(int index)
    //{
    //    switch (index)
    //    {
    //        case 2: // Storyline Completed Time
    //            var seconds = DateUtil.GetSecondsFrom2000(CAL_AdventureStartDate.Value, CAL_AdventureStartTime.Value);
    //            return DateUtil.ConvertDateValueToString(SAV.GetRecord(index), seconds);
    //        default:
    //            return null;
    //    }
    //}
}
