using System;
using System.Linq;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_Trainer7 : Form
    {
        private readonly SAV7 SAV = new SAV7(Main.SAV.Data);
        public SAV_Trainer7()
        {
            Loading = true;
            InitializeComponent();
            if (!Main.unicode)
            try { TB_OTName.Font = PKX.getPKXFont(11); }
            catch (Exception e) { Util.Alert("Font loading failed...", e.ToString()); }

            Util.TranslateInterface(this, Main.curlanguage);
            B_MaxCash.Click += (sender, e) => MT_Money.Text = "9,999,999";

            CB_Gender.Items.Clear();
            CB_Gender.Items.AddRange(Main.gendersymbols.Take(2).ToArray()); // m/f depending on unicode selection
            
            getComboBoxes();
            getTextBoxes();
            Loading = false;
        }
        private readonly ToolTip Tip1 = new ToolTip(), Tip2 = new ToolTip();
        private readonly bool Loading;
        private bool MapUpdated;

        private void getComboBoxes()
        {
            var dsregion_list = new[] {
                    new { Text = "NA/SA", Value = 1 },
                    new { Text = "EUR", Value = 2 },
                    new { Text = "JPN", Value = 0 },
                    new { Text = "CN", Value = 4 },
                    new { Text = "KOR", Value = 5 },
                    new { Text = "TW", Value = 6 }
                };

            var language_list = new[] {
                    new { Text = "ENG", Value = 2 },
                    new { Text = "JPN", Value = 1 },
                    new { Text = "FRE", Value = 3 },
                    new { Text = "ITA", Value = 4 },
                    new { Text = "GER", Value = 5 },
                    new { Text = "SPA", Value = 7 },
                    new { Text = "KOR", Value = 8 },
                    new { Text = "CHS", Value = 9 },
                    new { Text = "CHT", Value = 10},
                };

            var alolatime_list = new[] { new { Text = "Sun Time", Value = 24*60*60 } };
            Array.Resize(ref alolatime_list, 24);
            for (int i = 1; i < 24; i++)
                alolatime_list[i] = new {Text = $"+{i.ToString("00")} Hours", Value = i*60*60};
            alolatime_list[12] = new {Text = "Moon Time", Value = 12 * 60 * 60};

            CB_3DSReg.DisplayMember = "Text";
            CB_3DSReg.ValueMember = "Value";
            CB_3DSReg.DataSource = dsregion_list;
            CB_Language.DisplayMember = "Text";
            CB_Language.ValueMember = "Value";
            CB_Language.DataSource = language_list;
            CB_AlolaTime.DisplayMember = "Text";
            CB_AlolaTime.ValueMember = "Value";
            CB_AlolaTime.DataSource = alolatime_list;

            CB_Country.DisplayMember = "Text";
            CB_Country.ValueMember = "Value";
            CB_Region.DisplayMember = "Text";
            CB_Region.ValueMember = "Value";
            Main.setCountrySubRegion(CB_Country, "countries");
        }
        private void getTextBoxes()
        {
            // Get Data
            string OT_NAME = SAV.OT;

            CB_Game.SelectedIndex = SAV.Game - 30;
            CB_Gender.SelectedIndex = SAV.Gender;
            
            // Display Data
            TB_OTName.Text = OT_NAME;

            MT_TID.Text = SAV.TID.ToString("00000");
            MT_SID.Text = SAV.SID.ToString("00000");
            MT_Money.Text = SAV.Money.ToString();
            
            CB_Country.SelectedValue = SAV.Country;
            CB_Region.SelectedValue = SAV.SubRegion;
            CB_3DSReg.SelectedValue = SAV.ConsoleRegion;
            CB_Language.SelectedValue = SAV.Language;
            if (SAV.AlolaTime == 0)
            {
                SAV.AlolaTime = 24 * 60 * 60; // Patch up any bad times from previous program versions.
            }
            CB_AlolaTime.SelectedValue = (int)SAV.AlolaTime;
            
            NUD_M.Value = SAV.M;
            // Sanity Check Map Coordinates
            try
            {
                NUD_X.Value = (decimal)SAV.X;
                NUD_Z.Value = (decimal)SAV.Z;
                NUD_Y.Value = (decimal)SAV.Y;
                NUD_R.Value = (decimal)SAV.R;
            }
            catch { GB_Map.Enabled = false; }

            // Load Play Time
            MT_Hours.Text = SAV.PlayedHours.ToString();
            MT_Minutes.Text = SAV.PlayedMinutes.ToString();
            MT_Seconds.Text = SAV.PlayedSeconds.ToString();
            
            CAL_LastSavedDate.Value = new DateTime(SAV.LastSavedYear, SAV.LastSavedMonth, SAV.LastSavedDay);
            CAL_LastSavedTime.Value = new DateTime(2000, 1, 1, SAV.LastSavedHour, SAV.LastSavedMinute, 0);
            CAL_AdventureStartDate.Value = new DateTime(2000, 1, 1).AddSeconds(SAV.SecondsToStart);
            CAL_AdventureStartTime.Value = new DateTime(2000, 1, 1).AddSeconds(SAV.SecondsToStart % 86400);
            CAL_HoFDate.Value = new DateTime(2000, 1, 1).AddSeconds(SAV.SecondsToFame);
            CAL_HoFTime.Value = new DateTime(2000, 1, 1).AddSeconds(SAV.SecondsToFame % 86400);

            NUD_BP.Value = Math.Min(NUD_BP.Maximum, SAV.BP);
            NUD_FC.Value = Math.Min(NUD_BP.Maximum, SAV.FestaCoins);
        }
        private void save()
        {
            SAV.Game = (byte)(CB_Game.SelectedIndex + 30);
            SAV.Gender = (byte)CB_Gender.SelectedIndex;
            
            SAV.TID = (ushort)Util.ToUInt32(MT_TID.Text);
            SAV.SID = (ushort)Util.ToUInt32(MT_SID.Text);
            SAV.Money = Util.ToUInt32(MT_Money.Text);
            SAV.SubRegion = Util.getIndex(CB_Region);
            SAV.Country = Util.getIndex(CB_Country);
            SAV.ConsoleRegion = Util.getIndex(CB_3DSReg);
            SAV.Language = Util.getIndex(CB_Language);
            SAV.AlolaTime = (ulong)Util.getIndex(CB_AlolaTime);

            SAV.OT = TB_OTName.Text;

            // Copy Position
            if (GB_Map.Enabled && MapUpdated)
            {
                SAV.M = (int)NUD_M.Value;
                SAV.X = (float)NUD_X.Value;
                SAV.Z = (float)NUD_Z.Value;
                SAV.Y = (float)NUD_Y.Value;
                SAV.R = (float)NUD_R.Value;
            }
            
            // Save PlayTime
            SAV.PlayedHours = ushort.Parse(MT_Hours.Text);
            SAV.PlayedMinutes = ushort.Parse(MT_Minutes.Text)%60;
            SAV.PlayedSeconds = ushort.Parse(MT_Seconds.Text)%60;
            
            int seconds = (int)(CAL_AdventureStartDate.Value - new DateTime(2000, 1, 1)).TotalSeconds;
            seconds -= seconds%86400;
            seconds += (int)(CAL_AdventureStartTime.Value - new DateTime(2000, 1, 1)).TotalSeconds;
            SAV.SecondsToStart = seconds;

            int fame = (int)(CAL_HoFDate.Value - new DateTime(2000, 1, 1)).TotalSeconds;
            fame -= fame % 86400;
            fame += (int)(CAL_HoFTime.Value - new DateTime(2000, 1, 1)).TotalSeconds;
            SAV.SecondsToFame = fame;

            SAV.LastSavedYear = CAL_LastSavedDate.Value.Year;
            SAV.LastSavedMonth = CAL_LastSavedDate.Value.Month;
            SAV.LastSavedDay = CAL_LastSavedDate.Value.Day;
            SAV.LastSavedHour = CAL_LastSavedTime.Value.Hour;
            SAV.LastSavedMinute = CAL_LastSavedTime.Value.Minute;

            SAV.BP = (uint)NUD_BP.Value;
            SAV.FestaCoins = (uint)NUD_FC.Value;
        }

        private void clickOT(object sender, MouseEventArgs e)
        {
            TextBox tb = sender as TextBox ?? TB_OTName;
            // Special Character Form
            if (ModifierKeys != Keys.Control)
                return;

            var z = Application.OpenForms.Cast<Form>().FirstOrDefault(form => form.Name == typeof(f2_Text).Name) as f2_Text;
            if (z != null)
            { z.Location = Location; z.BringToFront(); return; }
            new f2_Text(tb).Show();
        }
        private void showTSV(object sender, EventArgs e)
        {
            SAV.TID = (ushort)Util.ToUInt32(MT_TID.Text);
            SAV.SID = (ushort)Util.ToUInt32(MT_SID.Text);
            int tsv = (SAV.TID ^ SAV.SID) >> 4;
            string IDstr = "TSV: " + tsv.ToString("0000");
            if (SAV.Generation > 6) // always true for G7
                IDstr += Environment.NewLine + "G7TID: " + SAV.TrainerID7.ToString("000000");
            Tip1.SetToolTip(MT_TID, "TSV: " + IDstr);
            Tip2.SetToolTip(MT_SID, "TSV: " + IDstr);
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            save();
            Main.SAV.Data = SAV.Data;
            Main.SAV.Edited = true;
            Close();
        }
        private void change255(object sender, EventArgs e)
        {
            MaskedTextBox box = sender as MaskedTextBox;
            if (box?.Text == "") box.Text = "0";
            if (Util.ToInt32(box.Text) > 255) box.Text = "255";
        }
        private void changeFFFF(object sender, EventArgs e)
        {
            MaskedTextBox box = sender as MaskedTextBox;
            if (box?.Text == "") box.Text = "0";
            if (Util.ToInt32(box.Text) > 65535) box.Text = "65535";
        }
        private void changeMapValue(object sender, EventArgs e)
        {
            if (!Loading)
                MapUpdated = true;
        }
        private void updateCountry(object sender, EventArgs e)
        {
            if (Util.getIndex(sender as ComboBox) > 0)
                Main.setCountrySubRegion(CB_Region, "sr_" + Util.getIndex(sender as ComboBox).ToString("000"));
        }
    }
}