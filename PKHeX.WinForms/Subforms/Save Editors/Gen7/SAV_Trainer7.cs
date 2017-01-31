using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_Trainer7 : Form
    {
        private readonly SAV7 SAV = new SAV7(Main.SAV.Data);
        public SAV_Trainer7()
        {
            Loading = true;
            InitializeComponent();
            if (Main.unicode)
            try { TB_OTName.Font = FontUtil.getPKXFont(11); }
            catch (Exception e) { WinFormsUtil.Alert("Font loading failed...", e.ToString()); }

            WinFormsUtil.TranslateInterface(this, Main.curlanguage);
            B_MaxCash.Click += (sender, e) => MT_Money.Text = "9,999,999";

            CB_Gender.Items.Clear();
            CB_Gender.Items.AddRange(Main.gendersymbols.Take(2).ToArray()); // m/f depending on unicode selection
            
            getComboBoxes();
            getTextBoxes();

            CB_Stats.Items.Clear();
            for (int i = 0; i < 200; i++)
            {
                string name;
                if (!RecordList.TryGetValue(i, out name))
                    name =  i.ToString("D3");

                CB_Stats.Items.Add(name);
            }
            CB_Stats.SelectedIndex = RecordList.First().Key;
            CB_Fashion.SelectedIndex = 1;

            Loading = false;
        }
        private readonly ToolTip Tip1 = new ToolTip(), Tip2 = new ToolTip();
        private readonly bool Loading;
        private bool MapUpdated;
        private bool editing;

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
                alolatime_list[i] = new {Text = $"+{i:00} Hours", Value = i*60*60};
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

            CB_SkinColor.Items.Clear();
            string[] skinColors = { "Pale", "Default", "Tan", "Dark" };
            foreach (string c in skinColors)
            {
                CB_SkinColor.Items.Add($"{Main.gendersymbols[0]} - {c}"); // M
                CB_SkinColor.Items.Add($"{Main.gendersymbols[1]} - {c}"); // F
            }
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
            
            if (SAV.LastSavedDate.HasValue)
            {
                CAL_LastSavedDate.Value = SAV.LastSavedDate.Value;
                CAL_LastSavedTime.Value = SAV.LastSavedDate.Value;
            }
            else
            {
                L_LastSaved.Visible = CAL_LastSavedDate.Visible = CAL_LastSavedTime.Visible = false;
            }
                
            CAL_AdventureStartDate.Value = new DateTime(2000, 1, 1).AddSeconds(SAV.SecondsToStart);
            CAL_AdventureStartTime.Value = new DateTime(2000, 1, 1).AddSeconds(SAV.SecondsToStart % 86400);
            CAL_HoFDate.Value = new DateTime(2000, 1, 1).AddSeconds(SAV.SecondsToFame);
            CAL_HoFTime.Value = new DateTime(2000, 1, 1).AddSeconds(SAV.SecondsToFame % 86400);

            NUD_BP.Value = Math.Min(NUD_BP.Maximum, SAV.BP);
            NUD_FC.Value = Math.Min(NUD_FC.Maximum, SAV.FestaCoins);

            // Poké Finder
            NUD_SnapCount.Value = Math.Min(NUD_SnapCount.Maximum, SAV.PokeFinderSnapCount);
            NUD_ThumbsTotal.Value = Math.Min(NUD_SnapCount.Maximum, SAV.PokeFinderThumbsTotalValue);
            NUD_ThumbsRecord.Value = Math.Min(NUD_SnapCount.Maximum, SAV.PokeFinderThumbsHighValue);

            CB_CameraVersion.SelectedIndex = Math.Min(CB_CameraVersion.Items.Count - 1, SAV.PokeFinderCameraVersion);
            CHK_Gyro.Checked = SAV.PokeFinderGyroFlag;

            // Battle Tree
            NUD_RCStreak0.Value = Math.Min(NUD_RCStreak0.Maximum, SAV.getTreeStreak(0, super: false, max: false));
            NUD_RCStreak1.Value = Math.Min(NUD_RCStreak1.Maximum, SAV.getTreeStreak(1, super: false, max: false));
            NUD_RCStreak2.Value = Math.Min(NUD_RCStreak2.Maximum, SAV.getTreeStreak(2, super: false, max: false));
            NUD_RMStreak0.Value = Math.Min(NUD_RMStreak0.Maximum, SAV.getTreeStreak(0, super: false, max: true));
            NUD_RMStreak1.Value = Math.Min(NUD_RMStreak1.Maximum, SAV.getTreeStreak(1, super: false, max: true));
            NUD_RMStreak2.Value = Math.Min(NUD_RMStreak2.Maximum, SAV.getTreeStreak(2, super: false, max: true));

            NUD_SCStreak0.Value = Math.Min(NUD_SCStreak0.Maximum, SAV.getTreeStreak(0, super: true, max: false));
            NUD_SCStreak1.Value = Math.Min(NUD_SCStreak1.Maximum, SAV.getTreeStreak(1, super: true, max: false));
            NUD_SCStreak2.Value = Math.Min(NUD_SCStreak2.Maximum, SAV.getTreeStreak(2, super: true, max: false));
            NUD_SMStreak0.Value = Math.Min(NUD_SMStreak0.Maximum, SAV.getTreeStreak(0, super: true, max: true));
            NUD_SMStreak1.Value = Math.Min(NUD_SMStreak1.Maximum, SAV.getTreeStreak(1, super: true, max: true));
            NUD_SMStreak2.Value = Math.Min(NUD_SMStreak2.Maximum, SAV.getTreeStreak(2, super: true, max: true));

            CB_SkinColor.SelectedIndex = SAV.DressUpSkinColor;
        }
        private void save()
        {
            SAV.Game = (byte)(CB_Game.SelectedIndex + 30);
            SAV.Gender = (byte)CB_Gender.SelectedIndex;
            
            SAV.TID = (ushort)Util.ToUInt32(MT_TID.Text);
            SAV.SID = (ushort)Util.ToUInt32(MT_SID.Text);
            SAV.Money = Util.ToUInt32(MT_Money.Text);
            SAV.SubRegion = WinFormsUtil.getIndex(CB_Region);
            SAV.Country = WinFormsUtil.getIndex(CB_Country);
            SAV.ConsoleRegion = WinFormsUtil.getIndex(CB_3DSReg);
            SAV.Language = WinFormsUtil.getIndex(CB_Language);
            SAV.AlolaTime = (ulong)WinFormsUtil.getIndex(CB_AlolaTime);

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

            if (SAV.LastSavedDate.HasValue)
                SAV.LastSavedDate = new DateTime(CAL_LastSavedDate.Value.Year, CAL_LastSavedDate.Value.Month, CAL_LastSavedDate.Value.Day, CAL_LastSavedTime.Value.Hour, CAL_LastSavedTime.Value.Minute, 0);

            SAV.BP = (uint)NUD_BP.Value;
            SAV.FestaCoins = (uint)NUD_FC.Value;

            // Poké Finder
            SAV.PokeFinderSnapCount = (uint)NUD_SnapCount.Value;
            SAV.PokeFinderThumbsTotalValue = (uint)NUD_ThumbsTotal.Value;
            SAV.PokeFinderThumbsHighValue = (uint)NUD_ThumbsRecord.Value;

            SAV.PokeFinderCameraVersion = (ushort)CB_CameraVersion.SelectedIndex;
            SAV.PokeFinderGyroFlag = CHK_Gyro.Checked;

            // Battle Tree
            SAV.setTreeStreak((int)NUD_RCStreak0.Value, 0, super:false, max:false);
            SAV.setTreeStreak((int)NUD_RCStreak1.Value, 1, super:false, max:false);
            SAV.setTreeStreak((int)NUD_RCStreak2.Value, 2, super:false, max:false);
            SAV.setTreeStreak((int)NUD_RMStreak0.Value, 0, super:false, max:true);
            SAV.setTreeStreak((int)NUD_RMStreak1.Value, 1, super:false, max:true);
            SAV.setTreeStreak((int)NUD_RMStreak2.Value, 2, super:false, max:true);

            SAV.setTreeStreak((int)NUD_SCStreak0.Value, 0, super:true, max:false);
            SAV.setTreeStreak((int)NUD_SCStreak1.Value, 1, super:true, max:false);
            SAV.setTreeStreak((int)NUD_SCStreak2.Value, 2, super:true, max:false);
            SAV.setTreeStreak((int)NUD_SMStreak0.Value, 0, super:true, max:true);
            SAV.setTreeStreak((int)NUD_SMStreak1.Value, 1, super:true, max:true);
            SAV.setTreeStreak((int)NUD_SMStreak2.Value, 2, super:true, max:true);

            // Skin changed && (gender matches || override)
            int gender = CB_Gender.SelectedIndex & 1;
            int skin = CB_SkinColor.SelectedIndex & 1;
            string gStr = CB_Gender.Items[gender].ToString();
            string sStr = CB_Gender.Items[skin].ToString();

            if (SAV.DressUpSkinColor != CB_SkinColor.SelectedIndex && 
                (SAV.Gender == skin || DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, $"Gender-Skin mismatch:\nGender: {gStr}, Skin: {sStr}", "Save selected Skin Color?")))
                    SAV.DressUpSkinColor = CB_SkinColor.SelectedIndex;
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
            Tip1.SetToolTip(MT_TID, IDstr);
            Tip2.SetToolTip(MT_SID, IDstr);
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
            if (WinFormsUtil.getIndex(sender as ComboBox) > 0)
                Main.setCountrySubRegion(CB_Region, "sr_" + WinFormsUtil.getIndex(sender as ComboBox).ToString("000"));
        }
        private void B_Fashion_Click(object sender, EventArgs e)
        {
            var prompt = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Modifying Fashion Items will clear existing data", "Continue?");
            if (DialogResult.Yes != prompt)
                return;

            // Clear Block
            new byte[SAV.FashionLength].CopyTo(SAV.Data, SAV.Fashion);
            
            // Write Payload

            switch (CB_Fashion.SelectedIndex)
            {
                case 0: // Base Fashion
                    if (SAV.Gender == 0) // Male
                    {
                        SAV.Data[0x42000] = 3;
                        SAV.Data[0x420FB] = 3;
                        SAV.Data[0x42124] = 3;
                        SAV.Data[0x4228F] = 3;
                        SAV.Data[0x423B4] = 3;
                        SAV.Data[0x42452] = 3;
                        SAV.Data[0x42517] = 3;
                    }
                    else // Female
                    {
                        SAV.Data[0x42000] = 3;
                        SAV.Data[0x42100] = 3;
                        SAV.Data[0x42223] = 3;
                        SAV.Data[0x42288] = 3;
                        SAV.Data[0x423B4] = 3;
                        SAV.Data[0x42452] = 3;
                        SAV.Data[0x42517] = 3;
                    }
                    break;
                case 1: // Full Legal
                    byte[] data1 = SAV.Gender == 0 ? Core.Properties.Resources.fashion_m_sm : Core.Properties.Resources.fashion_f_sm;
                    data1.CopyTo(SAV.Data, SAV.Fashion);
                    break;
                case 2: // Everything
                    byte[] data2 = SAV.Gender == 0 ? Core.Properties.Resources.fashion_m_sm_illegal : Core.Properties.Resources.fashion_f_sm_illegal;
                    data2.CopyTo(SAV.Data, SAV.Fashion);
                    break;
                default:
                    return;
            }
            System.Media.SystemSounds.Asterisk.Play();
        }
        private void changeStat(object sender, EventArgs e)
        {
            editing = true;
            int index = CB_Stats.SelectedIndex;
            NUD_Stat.Maximum = SAV.getRecordMax(index);
            NUD_Stat.Value = SAV.getRecord(index);

            int offset = SAV.getRecordOffset(index);
            L_Offset.Text = "Offset: 0x" + offset.ToString("X3");
            editing = false;
        }
        private void changeStatVal(object sender, EventArgs e)
        {
            if (editing) return;
            int index = CB_Stats.SelectedIndex;
            SAV.setRecord(index, (int)NUD_Stat.Value);
        }

        private readonly Dictionary<int, string> RecordList = new Dictionary<int, string>
        {
            {000, "Steps Taken"},
            {001, "Times Saved"},
            {004, "Wild Pokémon Battles"},
            {006, "Pokemon Caught"},
            {007, "Pokemon Caught Fishing"},
            {008, "Eggs Hatched"},
            {009, "Pokemon Evolved"},
            {011, "Link Trades"},
            {012, "Link Battles"},
            {013, "Link Battle Wins"},
            {015, "Battle Spot Battles"},
            {019, "Money Spent"},
            {022, "Exp. Points Collected"},
            {024, "Deposited in the GTS"},
            {025, "Nicknames Given"},
            {028, "Battle Points Spent"},
            {029, "Super Effective Moves Used"},
            {032, "Berry Harvests"},
            {033, "Trades at the GTS"},
            {034, "Wonder Trades"},
            {036, "Pokemon Rides"},
            {037, "Beans Given"},
            {038, "Festival Coins Spent"},
            {039, "Poke Beans Collected"},
            {040, "Battles at the Battle Tree"},
            {041, "Z-Moves Used"},
            {053, "Pokemon Petted"},
            {066, "Guests Interacted With"},

            {100, "Champion Title Defense"},
            {110, "Pokemon Defeated"},
            {112, "Pokemon Caught"},
            {114, "Trainers Battled"},
            {116, "Pokemon Evolved"},
            {118, "Fossils Restored"},
            {119, "Photos Taken"},
            {123, "Loto-ID Wins"},
            {124, "PP Raised"},
            {127, "Shiny Pokemon Encountered"},
            {128, "Missions Participated In"},
            {129, "Facilities Hosted"},
            {130, "QR Code Scans"},
            {158, "Outfit Changes"},
            {161, "Pelago Training Sessions"},
            {162, "Pelago Hot Spring Sessions"},
            {166, "Island Scans"},
        };
    }
}
