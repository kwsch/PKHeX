using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_Trainer : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV6 SAV;

        public SAV_Trainer(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV6)(Origin = sav).Clone();
            if (Main.Unicode)
            {
                try
                {
                    TB_OTName.Font = FontUtil.GetPKXFont(11);
                    if (SAV.XY)
                        TB_TRNick.Font = TB_OTName.Font;
                }
                catch (Exception e) { WinFormsUtil.Alert("Font loading failed...", e.ToString()); }
            }

            B_MaxCash.Click += (sender, e) => MT_Money.Text = "9,999,999";

            CB_Gender.Items.Clear();
            CB_Gender.Items.AddRange(Main.GenderSymbols.Take(2).ToArray()); // m/f depending on unicode selection

            TrainerStats.LoadRecords(SAV, Records.RecordList_6);
            TrainerStats.GetToolTipText = UpdateTip;

            MaisonRecords = new[]
            {
                TB_MCSN,TB_MCSS,TB_MBSN,TB_MBSS,
                TB_MCDN,TB_MCDS,TB_MBDN,TB_MBDS,
                TB_MCTN,TB_MCTS,TB_MBTN,TB_MBTS,
                TB_MCRN,TB_MCRS,TB_MBRN,TB_MBRS,
                TB_MCMN,TB_MCMS,TB_MBMN,TB_MBMS,
            };
            cba = new[] { CHK_Badge1, CHK_Badge2, CHK_Badge3, CHK_Badge4, CHK_Badge5, CHK_Badge6, CHK_Badge7, CHK_Badge8, };
            pba = new [] { PB_Badge1, PB_Badge2, PB_Badge3, PB_Badge4, PB_Badge5, PB_Badge6, PB_Badge7, PB_Badge8, };

            L_MultiplayerSprite.Enabled = CB_MultiplayerSprite.Enabled = SAV.ORAS;
            L_MultiplayerSprite.Visible = CB_MultiplayerSprite.Visible = SAV.ORAS;
            PB_Sprite.Visible = CHK_MegaRayquazaUnlocked.Visible = SAV.ORAS;

            L_Style.Visible = TB_Style.Visible = SAV.XY;
            if (!SAV.XY)
                TC_Editor.TabPages.Remove(Tab_Appearance);
            if (SAV.ORASDEMO)
                TC_Editor.TabPages.Remove(Tab_Multiplayer);
            if (SAV.MaisonStats < 0)
                TC_Editor.TabPages.Remove(Tab_Maison);

            GetComboBoxes();
            GetTextBoxes();
            GetBadges();
            editing = false;

            CHK_MegaUnlocked.Checked = SAV.IsMegaEvolutionUnlocked;
            CHK_MegaRayquazaUnlocked.Checked = SAV.IsMegaRayquazaUnlocked;
        }

        private readonly bool editing = true;
        private readonly ToolTip Tip1 = new ToolTip(), Tip2 = new ToolTip();
        private readonly MaskedTextBox[] MaisonRecords;
        private readonly CheckBox[] cba;
        private readonly PictureBox[] pba;
        private bool MapUpdated;

        private void GetComboBoxes()
        {
            CB_3DSReg.InitializeBinding();
            CB_3DSReg.DataSource = GameInfo.Regions;
            CB_Language.InitializeBinding();
            CB_Language.DataSource = GameInfo.LanguageDataSource(SAV.Generation);

            CB_Country.InitializeBinding();
            CB_Region.InitializeBinding();
            Main.SetCountrySubRegion(CB_Country, "countries");

            var names = Enum.GetNames(typeof(TrainerSprite6));
            var values = (int[])Enum.GetValues(typeof(TrainerSprite6));
            var data = names.Zip(values, (a, b) => new ComboItem {Text = a, Value = b})
                .Where(z => z.Value >= 2) // ignore Calem & Serena (no sprite)
                .ToList();

            CB_MultiplayerSprite.InitializeBinding();
            CB_MultiplayerSprite.DataSource = data;

            L_Vivillon.Text = GameInfo.Strings.specieslist[(int)Species.Vivillon] + ":";
            CB_Vivillon.InitializeBinding();
            CB_Vivillon.DataSource = PKX.GetFormList((int)Species.Vivillon, GameInfo.Strings.types, GameInfo.Strings.forms, Main.GenderSymbols, 6).ToList();
        }

        private void GetBadges()
        {
            var bma = GetGen6BadgeSprites(SAV.ORAS);
            for (int i = 0; i < 8; i++)
                pba[i].Image = ImageUtil.ChangeOpacity(bma[i], cba[i].Checked ? 1 : 0.1);
        }

        private static Bitmap[] GetGen6BadgeSprites(bool ORAS)
        {
            if (ORAS)
            {
                return new[]
                {
                    Properties.Resources.badge_01, Properties.Resources.badge_02,
                    Properties.Resources.badge_03, Properties.Resources.badge_04,
                    Properties.Resources.badge_05, Properties.Resources.badge_06,
                    Properties.Resources.badge_07, Properties.Resources.badge_08
                };
            }
            return new[] // XY
            {
                Properties.Resources.badge_1, Properties.Resources.badge_2,
                Properties.Resources.badge_3, Properties.Resources.badge_4,
                Properties.Resources.badge_5, Properties.Resources.badge_6,
                Properties.Resources.badge_7, Properties.Resources.badge_8,
            };
        }

        private void GetTextBoxes()
        {
            int badgeval = SAV.Badges;
            for (int i = 0; i < 8; i++)
                cba[i].Checked = (badgeval & 1 << i) != 0;

            // Get Data
            string OT_NAME = SAV.OT;

            CB_Game.SelectedIndex = SAV.Game - 0x18;
            CB_Gender.SelectedIndex = SAV.Gender;

            // Display Data
            TB_OTName.Text = OT_NAME;

            MT_TID.Text = SAV.TID.ToString("00000");
            MT_SID.Text = SAV.SID.ToString("00000");
            MT_Money.Text = SAV.Money.ToString();

            TB_Saying1.Text = SAV.Saying1;
            TB_Saying2.Text = SAV.Saying2;
            TB_Saying3.Text = SAV.Saying3;
            TB_Saying4.Text = SAV.Saying4;
            TB_Saying5.Text = SAV.Saying5;

            CB_Country.SelectedValue = SAV.Country;
            CB_Region.SelectedValue = SAV.SubRegion;
            CB_3DSReg.SelectedValue = SAV.ConsoleRegion;
            CB_Language.SelectedValue = SAV.Language;

            // Maison Data
            if (SAV.MaisonStats > -1)
            {
                for (int i = 0; i < MaisonRecords.Length; i++)
                    MaisonRecords[i].Text = SAV.GetMaisonStat(i).ToString();
            }

            NUD_M.Value = SAV.M;
            // Sanity Check Map Coordinates
            if (!GB_Map.Enabled || SAV.X%0.5 != 0 || SAV.Z%0.5 != 0 || SAV.Y%0.5 != 0)
            {
                GB_Map.Enabled = false;
            }
            else
            {
                try
                {
                    NUD_X.Value = (decimal)SAV.X;
                    NUD_Z.Value = (decimal)SAV.Z;
                    NUD_Y.Value = (decimal)SAV.Y;
                }
                catch { GB_Map.Enabled = false; }
            }

            // Load BP and PokeMiles
            TB_BP.Text = SAV.BP.ToString();
            TB_PM.Text = SAV.GetRecord(63).ToString();

            TB_Style.Text = SAV.Style.ToString();

            // Load Play Time
            MT_Hours.Text = SAV.PlayedHours.ToString();
            MT_Minutes.Text = SAV.PlayedMinutes.ToString();
            MT_Seconds.Text = SAV.PlayedSeconds.ToString();

            // Load PSS Sprite
            CB_MultiplayerSprite.SelectedValue = SAV.MultiplayerSpriteID;
            PB_Sprite.Image = SAV.Sprite();

            if (SAV.XY)
            {
                // Load Clothing Data
                propertyGrid1.SelectedObject = TrainerFashion6.GetFashion(SAV.Data, SAV.TrainerCard + 0x30, SAV.Gender);

                TB_TRNick.Text = SAV.OT_Nick;
            }

            CB_Vivillon.SelectedIndex = SAV.Vivillon;
            if (SAV.LastSavedDate.HasValue)
            {
                CAL_LastSavedDate.Value = SAV.LastSavedDate.Value;
                CAL_LastSavedTime.Value = SAV.LastSavedDate.Value;
            }
            else
            {
                L_LastSaved.Visible = CAL_LastSavedDate.Visible = CAL_LastSavedTime.Visible = false;
            }

            var epoch = new DateTime(2000, 1, 1);
            CAL_AdventureStartDate.Value = epoch.AddSeconds(SAV.SecondsToStart);
            CAL_AdventureStartTime.Value = epoch.AddSeconds(SAV.SecondsToStart % 86400);
            CAL_HoFDate.Value = epoch.AddSeconds(SAV.SecondsToFame);
            CAL_HoFTime.Value = epoch.AddSeconds(SAV.SecondsToFame % 86400);
        }

        private void Save()
        {
            SAV.Game = (byte)(CB_Game.SelectedIndex + 0x18);
            SAV.Gender = (byte)CB_Gender.SelectedIndex;

            SAV.TID = (ushort)Util.ToUInt32(MT_TID.Text);
            SAV.SID = (ushort)Util.ToUInt32(MT_SID.Text);
            SAV.Money = Util.ToUInt32(MT_Money.Text);
            SAV.SubRegion = WinFormsUtil.GetIndex(CB_Region);
            SAV.Country = WinFormsUtil.GetIndex(CB_Country);
            SAV.ConsoleRegion = WinFormsUtil.GetIndex(CB_3DSReg);
            SAV.Language = WinFormsUtil.GetIndex(CB_Language);

            SAV.OT = TB_OTName.Text;

            SAV.Saying1 = TB_Saying1.Text;
            SAV.Saying2 = TB_Saying2.Text;
            SAV.Saying3 = TB_Saying3.Text;
            SAV.Saying4 = TB_Saying4.Text;
            SAV.Saying5 = TB_Saying5.Text;

            // Copy Maison Data in
            if (SAV.MaisonStats > -1)
            {
                for (int i = 0; i < MaisonRecords.Length; i++)
                    SAV.SetMaisonStat(i, ushort.Parse(MaisonRecords[i].Text));
            }

            // Copy Position
            if (GB_Map.Enabled && MapUpdated)
            {
                SAV.M = (int)NUD_M.Value;
                SAV.X = (float)NUD_X.Value;
                SAV.Z = (float)NUD_Z.Value;
                SAV.Y = (float)NUD_Y.Value;
            }

            SAV.BP = ushort.Parse(TB_BP.Text);
            // Set Current PokéMiles
            SAV.SetRecord(63, Util.ToInt32(TB_PM.Text));
            // Set Max Obtained Pokémiles
            SAV.SetRecord(64, Util.ToInt32(TB_PM.Text));
            SAV.Style = byte.Parse(TB_Style.Text);

            // Copy Badges
            int badgeval = 0;
            for (int i = 0; i < 8; i++)
                badgeval |= (cba[i].Checked ? 1 : 0) << i;
            SAV.Badges = badgeval;

            // Save PlayTime
            SAV.PlayedHours = ushort.Parse(MT_Hours.Text);
            SAV.PlayedMinutes = ushort.Parse(MT_Minutes.Text)%60;
            SAV.PlayedSeconds = ushort.Parse(MT_Seconds.Text)%60;

            // Sprite
            SAV.MultiplayerSpriteID = Convert.ToByte(CB_MultiplayerSprite.SelectedValue);

            // Appearance
            if (SAV.XY)
            {
                // Save Clothing Data
                var obj = (TrainerFashion6)propertyGrid1.SelectedObject;
                obj.Write(SAV.Data, SAV.TrainerCard + 0x30);

                SAV.OT_Nick = TB_TRNick.Text;
            }

            // Vivillon
            SAV.Vivillon = CB_Vivillon.SelectedIndex;

            uint seconds = (uint)(CAL_AdventureStartDate.Value - new DateTime(2000, 1, 1)).TotalSeconds;
            seconds -= seconds%86400;
            seconds += (uint)(CAL_AdventureStartTime.Value - new DateTime(2000, 1, 1)).TotalSeconds;
            SAV.SecondsToStart = seconds;

            uint fame = (uint)(CAL_HoFDate.Value - new DateTime(2000, 1, 1)).TotalSeconds;
            fame -= fame % 86400;
            fame += (uint)(CAL_HoFTime.Value - new DateTime(2000, 1, 1)).TotalSeconds;
            SAV.SecondsToFame = fame;

            if (SAV.LastSavedDate.HasValue)
                SAV.LastSavedDate = new DateTime(CAL_LastSavedDate.Value.Year, CAL_LastSavedDate.Value.Month, CAL_LastSavedDate.Value.Day, CAL_LastSavedTime.Value.Hour, CAL_LastSavedTime.Value.Minute, 0);

            SAV.IsMegaEvolutionUnlocked = CHK_MegaUnlocked.Checked;
            SAV.IsMegaRayquazaUnlocked = CHK_MegaRayquazaUnlocked.Checked;
        }

        private void ClickOT(object sender, MouseEventArgs e)
        {
            TextBox tb = sender as TextBox ?? TB_OTName;
            // Special Character Form
            if (ModifierKeys != Keys.Control)
                return;

            var d = new TrashEditor(tb, null, SAV);
            d.ShowDialog();
            tb.Text = d.FinalString;
        }

        private void ShowTSV(object sender, EventArgs e)
        {
            uint TID = Util.ToUInt32(MT_TID.Text);
            uint SID = Util.ToUInt32(MT_SID.Text);
            uint tsv = (TID ^ SID) >> 4;
            Tip1.SetToolTip(MT_TID, $"TSV: {tsv:0000}");
            Tip2.SetToolTip(MT_SID, $"TSV: {tsv:0000}");
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            Save();
            Origin.SetData(SAV.Data, 0);
            Close();
        }

        private void ChangeBadge(object sender, EventArgs e)
        {
            GetBadges();
        }

        private void Change255(object sender, EventArgs e)
        {
            MaskedTextBox box = (MaskedTextBox)sender;
            if (box.Text.Length == 0) box.Text = "0";
            if (Util.ToInt32(box.Text) > 255) box.Text = "255";
        }

        private void ChangeFFFF(object sender, EventArgs e)
        {
            MaskedTextBox box = (MaskedTextBox)sender;
            if (box.Text.Length == 0) box.Text = "0";
            if (Util.ToInt32(box.Text) > 65535) box.Text = "65535";
        }

        private void GiveAllAccessories(object sender, EventArgs e)
        {
            SAV.UnlockAllAccessories();
        }

        private void UpdateCountry(object sender, EventArgs e)
        {
            int index;
            if (sender is ComboBox c && (index = WinFormsUtil.GetIndex(c)) > 0)
                Main.SetCountrySubRegion(CB_Region, $"sr_{index:000}");
        }

        private void ToggleBadge(object sender, EventArgs e)
        {
            cba[Array.IndexOf(pba, sender)].Checked ^= true;
        }

        private void ChangeMapValue(object sender, EventArgs e)
        {
            if (!editing)
                MapUpdated = true;
        }

        private void CB_Multi_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (editing)
                return;
            SAV.MultiplayerSpriteID = WinFormsUtil.GetIndex(CB_MultiplayerSprite);
            PB_Sprite.Image = SAV.Sprite();
        }

        private string UpdateTip(int index)
        {
            switch (index)
            {
                case 2: // Storyline Completed Time
                    var epoch = new DateTime(2000, 1, 1);
                    int seconds = (int)(CAL_AdventureStartDate.Value - epoch).TotalSeconds;
                    seconds -= seconds % 86400;
                    seconds += (int)(CAL_AdventureStartTime.Value - epoch).TotalSeconds;
                    return ConvertDateValueToString(SAV.GetRecord(index), seconds);
                default:
                    return null;
            }
        }

        private static string ConvertDateValueToString(int value, int secondsBias = -1)
        {
            const int spd = 86400; // seconds per day
            string tip = string.Empty;
            if (value >= spd)
                tip += (value / spd) + "d ";
            tip += new DateTime(0).AddSeconds(value).ToString("HH:mm:ss");
            if (secondsBias >= 0)
                tip += Environment.NewLine + $"Date: {new DateTime(2000, 1, 1).AddSeconds(value + secondsBias)}";
            return tip;
        }
    }
}