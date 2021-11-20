using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
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
            CB_Game.SelectedIndex = Math.Max(0, Math.Min(1, SAV.Game - (int)GameVersion.BD));
            CB_Gender.SelectedIndex = SAV.Gender;

            GB_Stats.Visible = false;

            // Display Data
            TB_OTName.Text = SAV.OT;
            trainerID1.LoadIDValues(SAV);
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

            //if (SAV.Played.LastSavedDate.HasValue)
            //{
            //    CAL_LastSavedDate.Value = SAV.Played.LastSavedDate.Value;
            //    CAL_LastSavedTime.Value = SAV.Played.LastSavedDate.Value;
            //}
            //else
            //{
                L_LastSaved.Visible = CAL_LastSavedDate.Visible = CAL_LastSavedTime.Visible = false;
            //}

            L_Started.Visible = CAL_AdventureStartDate.Visible = CAL_AdventureStartTime.Visible = false;
            L_Fame.Visible = CAL_HoFDate.Visible = CAL_HoFTime.Visible = false;
            // DateUtil.GetDateTime2000(SAV.SecondsToStart, out var date, out var time);
            // CAL_AdventureStartDate.Value = date;
            // CAL_AdventureStartTime.Value = time;
            // 
            // DateUtil.GetDateTime2000(SAV.SecondsToFame, out date, out time);
            // CAL_HoFDate.Value = date;
            // CAL_HoFTime.Value = time;

            CHK_Badge1.Checked = SAV.Work.GetSystemFlag(124);
            CHK_Badge2.Checked = SAV.Work.GetSystemFlag(125);
            CHK_Badge3.Checked = SAV.Work.GetSystemFlag(126);
            CHK_Badge4.Checked = SAV.Work.GetSystemFlag(127);
            CHK_Badge5.Checked = SAV.Work.GetSystemFlag(128);
            CHK_Badge6.Checked = SAV.Work.GetSystemFlag(129);
            CHK_Badge7.Checked = SAV.Work.GetSystemFlag(130);
            CHK_Badge8.Checked = SAV.Work.GetSystemFlag(131);
        }

        private void Save()
        {
            SaveTrainerInfo();
        }

        private void SaveTrainerInfo()
        {
            SAV.Game = (byte)(CB_Game.SelectedIndex + (int)GameVersion.BD);
            SAV.Gender = (byte)CB_Gender.SelectedIndex;

            SAV.Money = Util.ToUInt32(MT_Money.Text);
            SAV.Language = WinFormsUtil.GetIndex(CB_Language);
            SAV.OT = TB_OTName.Text;
            SAV.Rival = TB_Rival.Text;

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
            SAV.PlayedMinutes = ushort.Parse(MT_Minutes.Text)%60;
            SAV.PlayedSeconds = ushort.Parse(MT_Seconds.Text)%60;

            //SAV.SecondsToStart = (uint)DateUtil.GetSecondsFrom2000(CAL_AdventureStartDate.Value, CAL_AdventureStartTime.Value);
            //SAV.SecondsToFame = (uint)DateUtil.GetSecondsFrom2000(CAL_HoFDate.Value, CAL_HoFTime.Value);
            //
            //if (SAV.Played.LastSavedDate.HasValue)
            //    SAV.Played.LastSavedDate = new DateTime(CAL_LastSavedDate.Value.Year, CAL_LastSavedDate.Value.Month, CAL_LastSavedDate.Value.Day, CAL_LastSavedTime.Value.Hour, CAL_LastSavedTime.Value.Minute, 0);

            SAV.Work.SetSystemFlag(124, CHK_Badge1.Checked);
            SAV.Work.SetSystemFlag(125, CHK_Badge2.Checked);
            SAV.Work.SetSystemFlag(126, CHK_Badge3.Checked);
            SAV.Work.SetSystemFlag(127, CHK_Badge4.Checked);
            SAV.Work.SetSystemFlag(128, CHK_Badge5.Checked);
            SAV.Work.SetSystemFlag(129, CHK_Badge6.Checked);
            SAV.Work.SetSystemFlag(130, CHK_Badge7.Checked);
            SAV.Work.SetSystemFlag(131, CHK_Badge8.Checked);
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
}
