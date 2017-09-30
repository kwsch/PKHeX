using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_Trainer7 : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV7 SAV;
        public SAV_Trainer7(SaveFile sav)
        {
            SAV = (SAV7)(Origin = sav).Clone();
            Loading = true;
            InitializeComponent();
            if (Main.Unicode)
            try { TB_OTName.Font = FontUtil.GetPKXFont(11); }
            catch (Exception e) { WinFormsUtil.Alert("Font loading failed...", e.ToString()); }

            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            B_MaxCash.Click += (sender, e) => MT_Money.Text = "9,999,999";

            CB_Gender.Items.Clear();
            CB_Gender.Items.AddRange(Main.GenderSymbols.Take(2).ToArray()); // m/f depending on unicode selection
            
            GetComboBoxes();
            GetTextBoxes();

            CB_Stats.Items.Clear();
            for (int i = 0; i < 200; i++)
            {
                if (!RecordList.TryGetValue(i, out string name))
                    name = i.ToString("D3");

                CB_Stats.Items.Add(name);
            }
            CB_Stats.SelectedIndex = RecordList.First().Key;
            CB_Fashion.SelectedIndex = 1;

            Loading = false;
        }
        private readonly ToolTip Tip1 = new ToolTip(), Tip2 = new ToolTip(), Tip3 = new ToolTip();
        private readonly bool Loading;
        private bool MapUpdated;
        private bool editing;

        private static readonly string[] TrainerStampTitle = { "01:Official Pokemon Trainer", "02:Melemele Trial Completion", "03:Akala Trial Completion", "04:Ula'ula Trial Completion", "05:Poni Trial Completion", "06:Island Challenge Completion", "07:Melemele Pokedex Completion", "08:Akala Pokedex Completion", "09:Ula'ula Pokedex Completion", "10:Poni Pokedex Completion", "11:Alola Pokedex Completion", "12:50 Consecutive Single Battle Wins", "13:50 Consecutive Double Battle Wins", "14:50 Consecutive Multi Battle Wins", "15:Poke Finder Pro" };
        private static readonly string[] BattleStyles = { "Normal", "Elegant", "Girlish", "Reverent", "Smug", "Left-handed", "Passionate", "Idol" };

        private void GetComboBoxes()
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
            Main.SetCountrySubRegion(CB_Country, "countries");

            CB_SkinColor.Items.Clear();
            string[] skinColors = { "Pale", "Default", "Tan", "Dark" };
            foreach (string c in skinColors)
            {
                CB_SkinColor.Items.Add($"{Main.GenderSymbols[0]} - {c}"); // M
                CB_SkinColor.Items.Add($"{Main.GenderSymbols[1]} - {c}"); // F
            }

            L_Vivillon.Text = GameInfo.Strings.specieslist[666] + ":";
            CB_Vivillon.DisplayMember = "Text";
            CB_Vivillon.ValueMember = "Value";
            CB_Vivillon.DataSource = PKX.GetFormList(666, GameInfo.Strings.types, GameInfo.Strings.forms, Main.GenderSymbols).ToList();

            foreach (string t in BattleStyles)
            {
                CB_BallThrowType.Items.Add(t);
                LB_BallThrowTypeUnlocked.Items.Add(t);
                LB_BallThrowTypeLearned.Items.Add(t);
            }

            foreach (string t in TrainerStampTitle)
                LB_Stamps.Items.Add(t);
        }
        private void GetTextBoxes()
        {
            // Get Data
            string OT_NAME = SAV.OT;

            CB_Game.SelectedIndex = SAV.Game - 30;
            CB_Gender.SelectedIndex = SAV.Gender;
            
            // Display Data
            TB_OTName.Text = OT_NAME;

            MT_TID.Text = SAV.TID.ToString("00000");
            MT_SID.Text = SAV.SID.ToString("00000");
            MT_G7TID.Text = SAV.TrainerID7.ToString("000000");
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
            NUD_ThumbsTotal.Value = Math.Min(NUD_ThumbsTotal.Maximum, SAV.PokeFinderThumbsTotalValue);
            NUD_ThumbsRecord.Value = Math.Min(NUD_ThumbsRecord.Maximum, SAV.PokeFinderThumbsHighValue);

            CB_CameraVersion.SelectedIndex = Math.Min(CB_CameraVersion.Items.Count - 1, SAV.PokeFinderCameraVersion);
            CHK_Gyro.Checked = SAV.PokeFinderGyroFlag;

            // Battle Tree
            NUD_RCStreak0.Value = Math.Min(NUD_RCStreak0.Maximum, SAV.GetTreeStreak(0, super: false, max: false));
            NUD_RCStreak1.Value = Math.Min(NUD_RCStreak1.Maximum, SAV.GetTreeStreak(1, super: false, max: false));
            NUD_RCStreak2.Value = Math.Min(NUD_RCStreak2.Maximum, SAV.GetTreeStreak(2, super: false, max: false));
            NUD_RMStreak0.Value = Math.Min(NUD_RMStreak0.Maximum, SAV.GetTreeStreak(0, super: false, max: true));
            NUD_RMStreak1.Value = Math.Min(NUD_RMStreak1.Maximum, SAV.GetTreeStreak(1, super: false, max: true));
            NUD_RMStreak2.Value = Math.Min(NUD_RMStreak2.Maximum, SAV.GetTreeStreak(2, super: false, max: true));

            NUD_SCStreak0.Value = Math.Min(NUD_SCStreak0.Maximum, SAV.GetTreeStreak(0, super: true, max: false));
            NUD_SCStreak1.Value = Math.Min(NUD_SCStreak1.Maximum, SAV.GetTreeStreak(1, super: true, max: false));
            NUD_SCStreak2.Value = Math.Min(NUD_SCStreak2.Maximum, SAV.GetTreeStreak(2, super: true, max: false));
            NUD_SMStreak0.Value = Math.Min(NUD_SMStreak0.Maximum, SAV.GetTreeStreak(0, super: true, max: true));
            NUD_SMStreak1.Value = Math.Min(NUD_SMStreak1.Maximum, SAV.GetTreeStreak(1, super: true, max: true));
            NUD_SMStreak2.Value = Math.Min(NUD_SMStreak2.Maximum, SAV.GetTreeStreak(2, super: true, max: true));

            CB_SkinColor.SelectedIndex = SAV.DressUpSkinColor;
            TB_PlazaName.Text = SAV.FestivalPlazaName;

            CB_Vivillon.SelectedIndex = (SAV.Vivillon < CB_Vivillon.Items.Count) ? SAV.Vivillon : -1;
            NUD_DaysFromRefreshed.Value = Math.Min(NUD_DaysFromRefreshed.Maximum, SAV.DaysFromRefreshed);

            if (SAV.BallThrowType >= 0 && SAV.BallThrowType < CB_BallThrowType.Items.Count)
                CB_BallThrowType.SelectedIndex = SAV.BallThrowType;

            byte bttu = SAV.BallThrowTypeUnlocked;
            LB_BallThrowTypeUnlocked.SetSelected(0, true);
            LB_BallThrowTypeUnlocked.SetSelected(1, true);
            for (int i = 2; i < LB_BallThrowTypeUnlocked.Items.Count; i++)
                LB_BallThrowTypeUnlocked.SetSelected(i, (bttu & (1 << i)) != 0);

            byte bttl = SAV.BallThrowTypeLearned;
            LB_BallThrowTypeLearned.SetSelected(0, true);
            for (int i = 1; i < LB_BallThrowTypeLearned.Items.Count; i++)
                LB_BallThrowTypeLearned.SetSelected(i, (bttl & (1 << i)) != 0);

            CB_BallThrowTypeListMode.SelectedIndex = 0;

            uint stampBits = SAV.Stamps;
            for (int i = 0; i < LB_Stamps.Items.Count; i++)
                LB_Stamps.SetSelected(i, (stampBits & (1 << i)) != 0);

            byte btsu = SAV.BattleTreeSuperUnlocked;
            CHK_UnlockSuperSingles.Checked = (btsu & 1) != 0;
            CHK_UnlockSuperDoubles.Checked = (btsu & (1 << 1)) != 0;
            CHK_UnlockSuperMulti.Checked = (btsu & (1 << 2)) != 0;

            CHK_UnlockMega.Checked = SAV.MegaUnlocked;
        }
        private void Save()
        {
            SAV.Game = (byte)(CB_Game.SelectedIndex + 30);
            SAV.Gender = (byte)CB_Gender.SelectedIndex;
            
            SAV.TID = (ushort)Util.ToUInt32(MT_TID.Text);
            SAV.SID = (ushort)Util.ToUInt32(MT_SID.Text);
            SAV.Money = Util.ToUInt32(MT_Money.Text);
            SAV.SubRegion = WinFormsUtil.GetIndex(CB_Region);
            SAV.Country = WinFormsUtil.GetIndex(CB_Country);
            SAV.ConsoleRegion = WinFormsUtil.GetIndex(CB_3DSReg);
            SAV.Language = WinFormsUtil.GetIndex(CB_Language);
            SAV.AlolaTime = (ulong)WinFormsUtil.GetIndex(CB_AlolaTime);

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
            SAV.SetTreeStreak((int)NUD_RCStreak0.Value, 0, super:false, max:false);
            SAV.SetTreeStreak((int)NUD_RCStreak1.Value, 1, super:false, max:false);
            SAV.SetTreeStreak((int)NUD_RCStreak2.Value, 2, super:false, max:false);
            SAV.SetTreeStreak((int)NUD_RMStreak0.Value, 0, super:false, max:true);
            SAV.SetTreeStreak((int)NUD_RMStreak1.Value, 1, super:false, max:true);
            SAV.SetTreeStreak((int)NUD_RMStreak2.Value, 2, super:false, max:true);

            SAV.SetTreeStreak((int)NUD_SCStreak0.Value, 0, super:true, max:false);
            SAV.SetTreeStreak((int)NUD_SCStreak1.Value, 1, super:true, max:false);
            SAV.SetTreeStreak((int)NUD_SCStreak2.Value, 2, super:true, max:false);
            SAV.SetTreeStreak((int)NUD_SMStreak0.Value, 0, super:true, max:true);
            SAV.SetTreeStreak((int)NUD_SMStreak1.Value, 1, super:true, max:true);
            SAV.SetTreeStreak((int)NUD_SMStreak2.Value, 2, super:true, max:true);

            // Skin changed && (gender matches || override)
            int gender = CB_Gender.SelectedIndex & 1;
            int skin = CB_SkinColor.SelectedIndex & 1;
            string gStr = CB_Gender.Items[gender].ToString();
            string sStr = CB_Gender.Items[skin].ToString();

            if (SAV.DressUpSkinColor != CB_SkinColor.SelectedIndex && 
                (SAV.Gender == skin || DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, $"Gender-Skin mismatch:\nGender: {gStr}, Skin: {sStr}", "Save selected Skin Color?")))
                    SAV.DressUpSkinColor = CB_SkinColor.SelectedIndex;

            SAV.FestivalPlazaName = TB_PlazaName.Text;

            // Vivillon
            if (CB_Vivillon.SelectedIndex >= 0) SAV.Vivillon = CB_Vivillon.SelectedIndex;
            
            SAV.DaysFromRefreshed = (byte)NUD_DaysFromRefreshed.Value;
            SAV.BallThrowType = CB_BallThrowType.SelectedIndex;
            SAV.BallThrowTypeUnlocked = (byte)GetBits(LB_BallThrowTypeUnlocked);
            SAV.BallThrowTypeLearned = (byte)GetBits(LB_BallThrowTypeLearned);
            SAV.Stamps = GetBits(LB_Stamps);

            byte btsu = 0;
            if (CHK_UnlockSuperSingles.Checked) btsu |= 1;
            if (CHK_UnlockSuperDoubles.Checked) btsu |= 1 << 1;
            if (CHK_UnlockSuperMulti.Checked) btsu |= 1 << 2;
            SAV.BattleTreeSuperUnlocked = btsu;

            SAV.MegaUnlocked = CHK_UnlockMega.Checked;
        }
        private static uint GetBits(ListBox listbox)
        {
            uint bits = 0;
            for (int i = 0; i < listbox.Items.Count; i++)
                if (listbox.GetSelected(i))
                    bits |= (uint)(1 << i);
            return bits;
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
            SAV.TID = (ushort)Util.ToUInt32(MT_TID.Text);
            SAV.SID = (ushort)Util.ToUInt32(MT_SID.Text);
            int tsv = (SAV.TID ^ SAV.SID) >> 4;
            string IDstr = $"TSV: {tsv:0000}";
            if (SAV.Generation > 6) // always true for G7
                IDstr += Environment.NewLine + $"G7TID: {SAV.TrainerID7:000000}";
            Tip1.SetToolTip(MT_TID, IDstr);
            Tip2.SetToolTip(MT_SID, IDstr);
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
        private void Change255(object sender, EventArgs e)
        {
            MaskedTextBox box = sender as MaskedTextBox;
            if (box?.Text == "") box.Text = "0";
            if (Util.ToInt32(box.Text) > 255) box.Text = "255";
        }
        private void ChangeFFFF(object sender, EventArgs e)
        {
            MaskedTextBox box = sender as MaskedTextBox;
            if (box?.Text == "") box.Text = "0";
            if (Util.ToInt32(box.Text) > 65535) box.Text = "65535";
        }
        private void ChangeMapValue(object sender, EventArgs e)
        {
            if (!Loading)
                MapUpdated = true;
        }
        private void UpdateCountry(object sender, EventArgs e)
        {
            int index;
            if (sender is ComboBox c && (index = WinFormsUtil.GetIndex(c)) > 0)
                Main.SetCountrySubRegion(CB_Region, $"sr_{index:000}");
        }
        private void B_Fashion_Click(object sender, EventArgs e)
        {
            var prompt = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Modifying Fashion Items will clear existing data", "Continue?");
            if (DialogResult.Yes != prompt)
                return;

            // Clear Block
            new byte[SAV.FashionLength].CopyTo(SAV.Data, SAV.Fashion);
            
            // Write Payload
            // Every fashion item is 2 bits, New Flag (high) & Owned Flag (low)

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
                    byte[] data1 = SAV.Gender == 0 ? Properties.Resources.fashion_m_sm : Properties.Resources.fashion_f_sm;
                    data1.CopyTo(SAV.Data, SAV.Fashion);
                    break;
                case 2: // Everything
                    byte[] data2 = SAV.Gender == 0 ? Properties.Resources.fashion_m_sm_illegal : Properties.Resources.fashion_f_sm_illegal;
                    data2.CopyTo(SAV.Data, SAV.Fashion);
                    break;
                default:
                    return;
            }
            System.Media.SystemSounds.Asterisk.Play();
        }
        private void ChangeStat(object sender, EventArgs e)
        {
            editing = true;
            int index = CB_Stats.SelectedIndex;
            NUD_Stat.Maximum = SAV7.GetRecordMax(index);
            NUD_Stat.Value = SAV.GetRecord(index);

            int offset = SAV.GetRecordOffset(index);
            L_Offset.Text = $"Offset: 0x{offset:X3}";
            UpdateTip(index, true);
            editing = false;
        }
        private void ChangeStatVal(object sender, EventArgs e)
        {
            if (editing) return;
            int index = CB_Stats.SelectedIndex;
            SAV.SetRecord(index, (int)NUD_Stat.Value);
            UpdateTip(index, false);
        }
        private void UpdateTip(int index, bool updateStats)
        {
            switch (index)
            {
                case 2: // Storyline Completed Time
                    int seconds = (int)(CAL_AdventureStartDate.Value - new DateTime(2000, 1, 1)).TotalSeconds;
                    seconds -= seconds % 86400;
                    seconds += (int)(CAL_AdventureStartTime.Value - new DateTime(2000, 1, 1)).TotalSeconds;
                    Tip3.SetToolTip(NUD_Stat, ConvertDateValueToString(SAV.GetRecord(index), seconds));
                    break;
                default:
                    Tip3.RemoveAll();
                    break;
            }
            if (!updateStats)
                return;

            if (RecordList.TryGetValue(index, out string tip))
                Tip3.SetToolTip(CB_Stats, tip);
        }
        private static string ConvertDateValueToString(int value, int refval = -1)
        {
            string tip = "";
            if (value >= 86400)
                tip += value / 86400 + "d ";
            tip += new DateTime(0).AddSeconds(value).ToString("HH:mm:ss");
            if (refval >= 0)
                tip += Environment.NewLine + $"Date: {new DateTime(2000, 1, 1).AddSeconds(refval + value)}";
            return tip;
        }

        private void UpdateBattleStyle(object sender, EventArgs e)
        {
            if (CB_BallThrowTypeListMode.SelectedIndex == 0)
            {
                LB_BallThrowTypeUnlocked.Visible = true;
                LB_BallThrowTypeLearned.Visible = false;
            }
            else
            {
                LB_BallThrowTypeUnlocked.Visible = false;
                LB_BallThrowTypeLearned.Visible = true;
            }
        }
        private void UpdateBallThrowTypeLearned(object sender, EventArgs e)
        {
            if (Loading) return;
            if (!LB_BallThrowTypeLearned.GetSelected(0))
                LB_BallThrowTypeLearned.SetSelected(0, true);
        }
        private void UpdateBallThrowTypeUnlocked(object sender, EventArgs e)
        {
            if (Loading) return;
            for (int i = 0; i < 2; i++)
            {
                if (!LB_BallThrowTypeUnlocked.GetSelected(i))
                    LB_BallThrowTypeUnlocked.SetSelected(i, true);
            }
        }

        private void B_GenTID_Click(object sender, EventArgs e)
        {
            var tuple = SaveUtil.GetTIDSID(Util.ToUInt32(MT_G7TID.Text), ModifierKeys == Keys.Control);
            MT_TID.Text = tuple.Item1.ToString("D5");
            MT_SID.Text = tuple.Item2.ToString("D5");
        }

        private readonly Dictionary<int, string> RecordList = new Dictionary<int, string>
        {
            {000, "Steps Taken"},
            {001, "Times Saved"},
            {002, "Storyline Completed Time"},
            {003, "Total Battles"},
            {004, "Wild Pokémon Battles"},
            {005, "Trainer Battles"},
            {006, "Pokemon Caught"},
            {007, "Pokemon Caught Fishing"},
            {008, "Eggs Hatched"},
            {009, "Pokémon Evolved"},
            {010, "Pokémon Healed at Pokémon Centers"},
            {011, "Link Trades"},
            {012, "Link Battles"},
            {013, "Link Battle Wins"},
            {014, "Link Battle Losses"},
            {015, "Battle Spot Battles"},
            {016, "Battle Spot Wins"},
            {017, "Battle Spot Losses"},
            {018, "Mart Stack Purchases"},
            {019, "Money Spent"},
            {020, "Pokémon deposited at Nursery"},
            {021, "Pokémon Defeated"},
            {022, "Exp. Points Collected (Highest)"},
            {023, "Exp. Points Collected (Today)"},
            {024, "Deposited in the GTS"},
            {025, "Nicknames Given"},
            {026, "Bonus Premier Balls Received"},
            {027, "Battle Points Earned"},
            {028, "Battle Points Spent"},
            {029, "Super Effective Moves Used"},
            {031, "Salon Uses"},
            {032, "Berry Harvests"},
            {033, "Trades at the GTS"},
            {034, "Wonder Trades"},
            {035, "Quick Links"},
            {036, "Pokemon Rides"},
            {037, "Beans Given"},
            {038, "Festival Coins Spent"},
            {039, "Poke Beans Collected"},
            {040, "Battle Tree Challenges"},
            {041, "Z-Moves Used"},
            {042, "Balls Used"},
            {044, "Moves Used"},
            {046, "Ran From Battles"},
            {047, "Rock Smash Items"},
            {048, "Medicine Used"},
            {050, "Total Thumbs-Ups"},
            {051, "Times Twirled (Pirouette)"},
            {052, "Record Thumbs-ups"},
            {053, "Pokemon Petted"},
            {054, "Poké Pelago Visits"},
            {055, "Poké Bean Trades"},
            {056, "Poké Pelago Tapped Pokémon"},
            {057, "Poké Pelago Bean Stacks put in Crate"},
            {063, "Battle Videos Watched"},
            {064, "Battle Videos Rebattled"},
            {065, "RotomDex Interactions"},
            {066, "Guests Interacted With"},
            {067, "Berry Piles (not full) Collected"},
            {068, "Berry Piles (full) Collected"},
            {069, "Items Reeled In"},

            {100, "Champion Title Defense"},
            {104, "Moves used with No Effect"},
            {105, "Own Fainted Pokémon"},
            {107, "Failed Run Attempts"},
            {110, "Pokemon Defeated (Highest)"},
            {111, "Pokemon Defeated (Today)"},
            {112, "Pokemon Caught (Highest)"},
            {113, "Pokemon Caught (Today)"},
            {114, "Trainers Battled (Highest)"},
            {115, "Trainers Battled (Today)"},
            {116, "Pokemon Evolved (Highest)"},
            {117, "Pokemon Evolved (Today)"},
            {118, "Fossils Restored"},
            {119, "Photos Rated"},
            {120, "Best (Super) Singles Streak"},
            {121, "Best (Super) Doubles Streak"},
            {122, "Best (Super) Multi Streak"},
            {123, "Loto-ID Wins"},
            {124, "PP Raised"},
            {127, "Shiny Pokemon Encountered"},
            {128, "Missions Participated In"},
            {129, "Facilities Hosted"},
            {130, "QR Code Scans"},
            {131, "Moves learned with TMs"},
            {132, "Café Drinks Bought"},
            {133, "Trainer Card Photos Taken"},
            {134, "Evolutions Cancelled"},
            {135, "SOS Battle Allies Called"},
            {137, "Battle Royal Dome Battles"},
            {138, "Items Picked Up after Battle"},
            {139, "Ate in Malasadas Shop"},
            {141, "Dishes eaten in Battle Buffet"},
            {142, "Pokémon Refresh Accessed"},
            {143, "Pokémon Storage System Log-outs"},
            {144, "Lomi Lomi Massages"},
            {145, "Times laid down in Ilima's Bed"},
            {146, "Times laid down in Guzma's Bed"},
            {147, "Times laid down in Kiawe's Bed"},
            {148, "Times laid down in Lana's Bed"},
            {149, "Times laid down in Mallow's Bed"},
            {150, "Times laid down in Olivia's Bed"},
            {151, "Times laid down in Hapu's Bed"},
            {152, "Times laid down in Lusamine's Bed"},
            {153, "Ambush/Smash post-battle items received"},
            {154, "Rustling Tree Encounters"},
            {155, "Ledges Jumped Down"},
            {156, "Water Splash Encounters"},
            {157, "Sand Cloud Encounters"},
            {158, "Outfit Changes"},
            {159, "Battle Royal Dome Wins"},
            {161, "Pelago Training Sessions"},
            {162, "Pelago Hot Spring Sessions"},
            {166, "Island Scans"},
            {167, "Rustling Bush Encounters"},
            {168, "Fly Shadow Encounters"},
            {169, "Rustling Grass Encounters"},
            {170, "Dirt Cloud Encounters"},
            {171, "Wimpod Chases"},
            {172, "Berry Tree Battles won"},
            {173, "Bubbling Spot Encounters/Items"},
            {174, "Times laid down in Own Bed"},

            {175, "Trade Pokémon at the GTS!"},
            {176, "176 - Global Mission"},
            {177, "Hatch a lot of Eggs!"},
            {178, "Harvest Poké Beans!"},
            {179, "179 - Global Mission"},
            {180, "Find Pokémon using Island Scan!"},
            {181, "181 - Global Mission"},
            {182, "Defend your Champion title!"},
            {183, "Fish Pokémon at rare spots!"},
            {185, "Try your luck!"},
            {186, "186 - Global Mission"},
            {187, "Catch a lot of Pokémon!"},
        };
    }
}
