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
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV7)(Origin = sav).Clone();
            Loading = true;
            if (Main.Unicode)
            {
                TB_OTName.Font = FontUtil.GetPKXFont();
            }

            B_MaxCash.Click += (sender, e) => MT_Money.Text = "9,999,999";

            CB_Gender.Items.Clear();
            CB_Gender.Items.AddRange(Main.GenderSymbols.Take(2).ToArray()); // m/f depending on unicode selection

            GetComboBoxes();
            GetTextBoxes();

            TrainerStats.LoadRecords(SAV, Records.RecordList_7);
            TrainerStats.GetToolTipText = UpdateTip;
            CB_Fashion.SelectedIndex = 1;

            if (SAV is SAV7USUM)
                LoadUltraData();
            else
                TC_Editor.TabPages.Remove(Tab_Ultra);

            Loading = false;
        }

        private readonly bool Loading;
        private bool MapUpdated;

        private static readonly string[] AllStyles = Enum.GetNames(typeof(PlayerBattleStyle7));
        private readonly List<string> BattleStyles = new(AllStyles);

        private int[] FlyDestFlagOfs = null!, MapUnmaskFlagOfs = null!;
        private int SkipFlag => SAV is SAV7USUM ? 4160 : 3200; // FlagMax - 768

        private void GetComboBoxes()
        {
            CB_3DSReg.InitializeBinding();
            CB_3DSReg.DataSource = GameInfo.Regions;
            CB_Language.InitializeBinding();
            CB_Language.DataSource = GameInfo.LanguageDataSource(SAV.Generation);
            CB_AlolaTime.InitializeBinding();
            CB_AlolaTime.DataSource = GetAlolaTimeList();

            CB_Country.InitializeBinding();
            CB_Region.InitializeBinding();
            Main.SetCountrySubRegion(CB_Country, "countries");

            CB_SkinColor.Items.Clear();
            CB_SkinColor.Items.AddRange(Enum.GetNames(typeof(PlayerSkinColor7)));

            L_Vivillon.Text = GameInfo.Strings.Species[(int)Species.Vivillon] + ":";
            CB_Vivillon.InitializeBinding();
            CB_Vivillon.DataSource = FormConverter.GetFormList((int)Species.Vivillon, GameInfo.Strings.types, GameInfo.Strings.forms, Main.GenderSymbols, SAV.Generation);

            if (SAV is not SAV7USUM)
                BattleStyles.RemoveAt(BattleStyles.Count - 1); // remove Nihilist
            foreach (string t in BattleStyles)
            {
                CB_BallThrowType.Items.Add(t);
                LB_BallThrowTypeUnlocked.Items.Add(t);
                LB_BallThrowTypeLearned.Items.Add(t);
            }

            var stamps = Enum.GetNames(typeof(Stamp7)).Select(z => z.Replace("_", " "));
            foreach (string t in stamps)
                LB_Stamps.Items.Add(t);
        }

        private static ComboItem[] GetAlolaTimeList()
        {
            var alolatime_list = new ComboItem[24];
            for (int i = 1; i < alolatime_list.Length; i++)
                alolatime_list[i] = new ComboItem($"+{i:00} Hours", i * 60 * 60);
            alolatime_list[0] = new ComboItem("Sun Time", 24 * 60 * 60);
            alolatime_list[12] = new ComboItem("Moon Time", 12 * 60 * 60);
            return alolatime_list;
        }

        private void GetTextBoxes()
        {
            // Get Data
            CB_Game.SelectedIndex = SAV.Game - (int)GameVersion.SN;
            CB_Gender.SelectedIndex = SAV.Gender;

            // Display Data
            TB_OTName.Text = SAV.OT;
            trainerID1.LoadIDValues(SAV);
            MT_Money.Text = SAV.Money.ToString();

            CB_Country.SelectedValue = SAV.Country;
            CB_Region.SelectedValue = SAV.Region;
            CB_3DSReg.SelectedValue = SAV.ConsoleRegion;
            CB_Language.SelectedValue = SAV.Language;
            var timeA = SAV.GameTime.AlolaTime;
            if (timeA == 0)
                timeA = 24 * 60 * 60; // Patch up any bad times from previous program versions.
            if (timeA == 9_999_999)
                CB_AlolaTime.Enabled = false; // alola time doesn't exist yet
            else
                CB_AlolaTime.SelectedValue = (int)timeA;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (CB_AlolaTime.SelectedValue == null)
                CB_AlolaTime.Enabled = false;

            NUD_M.Value = SAV.Situation.M;
            // Sanity Check Map Coordinates
            try
            {
                NUD_X.Value = (decimal)SAV.Situation.X;
                NUD_Z.Value = (decimal)SAV.Situation.Z;
                NUD_Y.Value = (decimal)SAV.Situation.Y;
                NUD_R.Value = (decimal)SAV.Situation.R;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            // Sometimes the coordinates aren't really decimal/float coordinates?
            catch { GB_Map.Enabled = false; }
#pragma warning restore CA1031 // Do not catch general exception types

            // Load Play Time
            MT_Hours.Text = SAV.PlayedHours.ToString();
            MT_Minutes.Text = SAV.PlayedMinutes.ToString();
            MT_Seconds.Text = SAV.PlayedSeconds.ToString();

            if (SAV.Played.LastSavedDate.HasValue)
            {
                CAL_LastSavedDate.Value = SAV.Played.LastSavedDate.Value;
                CAL_LastSavedTime.Value = SAV.Played.LastSavedDate.Value;
            }
            else
            {
                L_LastSaved.Visible = CAL_LastSavedDate.Visible = CAL_LastSavedTime.Visible = false;
            }

            DateUtil.GetDateTime2000(SAV.SecondsToStart, out var date, out var time);
            CAL_AdventureStartDate.Value = date;
            CAL_AdventureStartTime.Value = time;

            DateUtil.GetDateTime2000(SAV.SecondsToFame, out date, out time);
            CAL_HoFDate.Value = date;
            CAL_HoFTime.Value = time;

            NUD_BP.Value = Math.Min(NUD_BP.Maximum, SAV.Misc.BP);
            NUD_FC.Value = Math.Min(NUD_FC.Maximum, SAV.Festa.FestaCoins);

            // Poké Finder
            NUD_SnapCount.Value = Math.Min(NUD_SnapCount.Maximum, SAV.PokeFinder.SnapCount);
            NUD_ThumbsTotal.Value = Math.Min(NUD_ThumbsTotal.Maximum, SAV.PokeFinder.ThumbsTotalValue);
            NUD_ThumbsRecord.Value = Math.Min(NUD_ThumbsRecord.Maximum, SAV.PokeFinder.ThumbsHighValue);

            CB_CameraVersion.SelectedIndex = Math.Min(CB_CameraVersion.Items.Count - 1, SAV.PokeFinder.CameraVersion);
            CHK_Gyro.Checked = SAV.PokeFinder.GyroFlag;

            // Battle Tree
            var bt = SAV.BattleTree;
            NUD_RCStreak0.Value = Math.Min(NUD_RCStreak0.Maximum, bt.GetTreeStreak(0, super: false, max: false));
            NUD_RCStreak1.Value = Math.Min(NUD_RCStreak1.Maximum, bt.GetTreeStreak(1, super: false, max: false));
            NUD_RCStreak2.Value = Math.Min(NUD_RCStreak2.Maximum, bt.GetTreeStreak(2, super: false, max: false));
            NUD_RMStreak0.Value = Math.Min(NUD_RMStreak0.Maximum, bt.GetTreeStreak(0, super: false, max: true));
            NUD_RMStreak1.Value = Math.Min(NUD_RMStreak1.Maximum, bt.GetTreeStreak(1, super: false, max: true));
            NUD_RMStreak2.Value = Math.Min(NUD_RMStreak2.Maximum, bt.GetTreeStreak(2, super: false, max: true));

            NUD_SCStreak0.Value = Math.Min(NUD_SCStreak0.Maximum, bt.GetTreeStreak(0, super: true, max: false));
            NUD_SCStreak1.Value = Math.Min(NUD_SCStreak1.Maximum, bt.GetTreeStreak(1, super: true, max: false));
            NUD_SCStreak2.Value = Math.Min(NUD_SCStreak2.Maximum, bt.GetTreeStreak(2, super: true, max: false));
            NUD_SMStreak0.Value = Math.Min(NUD_SMStreak0.Maximum, bt.GetTreeStreak(0, super: true, max: true));
            NUD_SMStreak1.Value = Math.Min(NUD_SMStreak1.Maximum, bt.GetTreeStreak(1, super: true, max: true));
            NUD_SMStreak2.Value = Math.Min(NUD_SMStreak2.Maximum, bt.GetTreeStreak(2, super: true, max: true));

            CB_SkinColor.SelectedIndex = SAV.MyStatus.DressUpSkinColor;
            TB_PlazaName.Text = SAV.Festa.FestivalPlazaName;

            CB_Vivillon.SelectedIndex = (SAV.Misc.Vivillon < CB_Vivillon.Items.Count) ? SAV.Misc.Vivillon : -1;
            NUD_DaysFromRefreshed.Value = Math.Min(NUD_DaysFromRefreshed.Maximum, SAV.Misc.DaysFromRefreshed);

            if (SAV.MyStatus.BallThrowType >= 0 && SAV.MyStatus.BallThrowType < CB_BallThrowType.Items.Count)
                CB_BallThrowType.SelectedIndex = SAV.MyStatus.BallThrowType;

            if (SAV is SAV7SM)
                LoadThrowTypeLists();
            else
                CB_BallThrowTypeListMode.Visible = LB_BallThrowTypeLearned.Visible = LB_BallThrowTypeUnlocked.Visible = false;

            uint stampBits = SAV.Misc.Stamps;
            for (int i = 0; i < LB_Stamps.Items.Count; i++)
                LB_Stamps.SetSelected(i, (stampBits & (1 << i)) != 0);

            CHK_UnlockSuperSingles.Checked = SAV.GetEventFlag(333);
            CHK_UnlockSuperDoubles.Checked = SAV.GetEventFlag(334);
            CHK_UnlockSuperMulti.Checked = SAV.GetEventFlag(335);

            CHK_UnlockMega.Checked = SAV.MyStatus.MegaUnlocked;
            CHK_UnlockZMove.Checked = SAV.MyStatus.ZMoveUnlocked;

            LoadMapFlyToData();
        }

        private void LoadThrowTypeLists()
        {
            const int unlockStart = 292;
            const int learnedStart = 3479;
            LB_BallThrowTypeUnlocked.SetSelected(0, true);
            LB_BallThrowTypeUnlocked.SetSelected(1, true);
            for (int i = 2; i < BattleStyles.Count; i++)
                LB_BallThrowTypeUnlocked.SetSelected(i, SAV.GetEventFlag(unlockStart + i));

            LB_BallThrowTypeLearned.SetSelected(0, true);
            for (int i = 1; i < BattleStyles.Count; i++)
                LB_BallThrowTypeLearned.SetSelected(i, SAV.GetEventFlag(learnedStart + i));

            CB_BallThrowTypeListMode.SelectedIndex = 0;
        }

        private void LoadMapFlyToData()
        {
            var metLocationList = GameInfo.GetLocationList(GameVersion.US, 7, false);
            int[] FlyDestNameIndex = {
                -1,24,34,8,20,38,12,46,40,30,//Melemele
                70,68,78,86,74,104,82,58,90,72,76,92,62,//Akala
                132,136,138,114,118,144,130,154,140,//Ula'ula
                172,184,180,174,176,156,186,//Poni
                188,-1,-1,
                198,202,110,204,//Beach
            };
            if (SAV.Version is GameVersion.UM or GameVersion.MN)
            {
                FlyDestNameIndex[28] = 142;
                FlyDestNameIndex[36] = 178;
            }
            FlyDestFlagOfs = new[] {
                44,43,45,40,41,49,42,47,46,48,
                50,54,39,57,51,55,59,52,58,53,61,60,56,
                62,66,67,64,65,273,270,37,38,
                69,74,72,71,276,73,70,
                75,332,334,
                331,333,335,336,
            };
            string[] FlyDestAltName = { "My House", "Photo Club (Hau'oli)", "Photo Club (Konikoni)", };
            CLB_FlyDest.Items.Clear();
            for (int i = 0, u = 0, m = FlyDestNameIndex.Length - (SAV is SAV7USUM ? 0 : 6); i < m; i++)
            {
                var dest = FlyDestNameIndex[i];
                var name = dest < 0 ? FlyDestAltName[u++] : metLocationList.First(v => v.Value == dest).Text;
                var state = SAV.GetEventFlag(SkipFlag + FlyDestFlagOfs[i]);
                CLB_FlyDest.Items.Add(name, state);
            }
            int[] MapUnmaskNameIndex = {
                6,8,24,-1,18,-1,20,22,12,10,14,
                70,50,68,52,74,54,56,58,60,72,62,64,
                132,192,106,108,122,112,114,126,116,118,120,154,
                172,158,160,162,164,166,168,170,
                188,
                198,202,110,204,
            };
            MapUnmaskFlagOfs = new[] {
                5,76,82,91,79,84,80,81,77,78,83,
                19,10,18,11,21,12,13,14,15,20,16,17,
                33,34,30,31,98,92,93,94,95,96,97,141,
                173,144,145,146,147,148,149,172,
                181,
                409,297,32,296,
            };
            string[] MapUnmaskAltName = { "Melemele Sea (East)", "Melemele Sea (West)", };
            CLB_MapUnmask.Items.Clear();
            for (int i = 0, u = 0, m = MapUnmaskNameIndex.Length - (SAV is SAV7USUM ? 0 : 4); i < m; i++)
            {
                var dest = MapUnmaskNameIndex[i];
                var name = dest < 0 ? MapUnmaskAltName[u++] : metLocationList.First(v => v.Value == dest).Text;
                var state = SAV.GetEventFlag(SkipFlag + MapUnmaskFlagOfs[i]);
                CLB_MapUnmask.Items.Add(name, state);
            }
        }

        private void LoadUltraData()
        {
            NUD_Surf0.Value = SAV.Misc.GetSurfScore(0);
            NUD_Surf1.Value = SAV.Misc.GetSurfScore(1);
            NUD_Surf2.Value = SAV.Misc.GetSurfScore(2);
            NUD_Surf3.Value = SAV.Misc.GetSurfScore(3);
            TB_RotomOT.Font = TB_OTName.Font;
            TB_RotomOT.Text = SAV.FieldMenu.RotomOT;
            NUD_RotomAffection.Value = Math.Min(NUD_RotomAffection.Maximum, SAV.FieldMenu.RotomAffection);
            CHK_RotoLoto1.Checked = SAV.FieldMenu.RotomLoto1;
            CHK_RotoLoto2.Checked = SAV.FieldMenu.RotomLoto2;
        }

        private void Save()
        {
            SaveTrainerInfo();
            SavePokeFinder();
            SaveBattleTree();
            SaveTrainerAppearance();
            SAV.Misc.DaysFromRefreshed = (byte)NUD_DaysFromRefreshed.Value;
            SaveThrowType();

            SAV.Festa.FestivalPlazaName = TB_PlazaName.Text;

            // Vivillon
            if (CB_Vivillon.SelectedIndex >= 0) SAV.Misc.Vivillon = CB_Vivillon.SelectedIndex;

            SaveFlags();

            if (SAV is SAV7USUM)
                SaveUltraData();
        }

        private void SaveTrainerInfo()
        {
            SAV.Game = (byte)(CB_Game.SelectedIndex + 30);
            SAV.Gender = (byte)CB_Gender.SelectedIndex;

            SAV.Money = Util.ToUInt32(MT_Money.Text);
            SAV.Region = WinFormsUtil.GetIndex(CB_Region);
            SAV.Country = WinFormsUtil.GetIndex(CB_Country);
            SAV.ConsoleRegion = WinFormsUtil.GetIndex(CB_3DSReg);
            SAV.Language = WinFormsUtil.GetIndex(CB_Language);
            if (CB_AlolaTime.Enabled)
                SAV.GameTime.AlolaTime = (ulong)WinFormsUtil.GetIndex(CB_AlolaTime);

            SAV.OT = TB_OTName.Text;

            // Copy Position
            if (GB_Map.Enabled && MapUpdated)
            {
                SAV.Situation.M = (int)NUD_M.Value;
                SAV.Situation.X = (float)NUD_X.Value;
                SAV.Situation.Z = (float)NUD_Z.Value;
                SAV.Situation.Y = (float)NUD_Y.Value;
                SAV.Situation.R = (float)NUD_R.Value;
                SAV.Situation.UpdateOverworldCoordinates();
            }

            // Save PlayTime
            SAV.PlayedHours = ushort.Parse(MT_Hours.Text);
            SAV.PlayedMinutes = ushort.Parse(MT_Minutes.Text)%60;
            SAV.PlayedSeconds = ushort.Parse(MT_Seconds.Text)%60;

            SAV.SecondsToStart = (uint)DateUtil.GetSecondsFrom2000(CAL_AdventureStartDate.Value, CAL_AdventureStartTime.Value);
            SAV.SecondsToFame = (uint)DateUtil.GetSecondsFrom2000(CAL_HoFDate.Value, CAL_HoFTime.Value);

            if (SAV.Played.LastSavedDate.HasValue)
                SAV.Played.LastSavedDate = new DateTime(CAL_LastSavedDate.Value.Year, CAL_LastSavedDate.Value.Month, CAL_LastSavedDate.Value.Day, CAL_LastSavedTime.Value.Hour, CAL_LastSavedTime.Value.Minute, 0);

            SAV.Misc.BP = (uint)NUD_BP.Value;
            SAV.Festa.FestaCoins = (int)NUD_FC.Value;
        }

        private void SavePokeFinder()
        {
            SAV.PokeFinder.SnapCount = (uint)NUD_SnapCount.Value;
            SAV.PokeFinder.ThumbsTotalValue = (uint)NUD_ThumbsTotal.Value;
            SAV.PokeFinder.ThumbsHighValue = (uint)NUD_ThumbsRecord.Value;

            SAV.PokeFinder.CameraVersion = (ushort)CB_CameraVersion.SelectedIndex;
            SAV.PokeFinder.GyroFlag = CHK_Gyro.Checked;
        }

        private void SaveBattleTree()
        {
            var bt = SAV.BattleTree;
            bt.SetTreeStreak((int)NUD_RCStreak0.Value, 0, super:false, max:false);
            bt.SetTreeStreak((int)NUD_RCStreak1.Value, 1, super:false, max:false);
            bt.SetTreeStreak((int)NUD_RCStreak2.Value, 2, super:false, max:false);
            bt.SetTreeStreak((int)NUD_RMStreak0.Value, 0, super:false, max:true);
            bt.SetTreeStreak((int)NUD_RMStreak1.Value, 1, super:false, max:true);
            bt.SetTreeStreak((int)NUD_RMStreak2.Value, 2, super:false, max:true);

            bt.SetTreeStreak((int)NUD_SCStreak0.Value, 0, super:true, max:false);
            bt.SetTreeStreak((int)NUD_SCStreak1.Value, 1, super:true, max:false);
            bt.SetTreeStreak((int)NUD_SCStreak2.Value, 2, super:true, max:false);
            bt.SetTreeStreak((int)NUD_SMStreak0.Value, 0, super:true, max:true);
            bt.SetTreeStreak((int)NUD_SMStreak1.Value, 1, super:true, max:true);
            bt.SetTreeStreak((int)NUD_SMStreak2.Value, 2, super:true, max:true);
        }

        private void SaveTrainerAppearance()
        {
            // Skin changed && (gender matches || override)
            int gender = CB_Gender.SelectedIndex & 1;
            int skin = CB_SkinColor.SelectedIndex & 1;
            var gStr = CB_Gender.Items[gender].ToString();
            var sStr = CB_Gender.Items[skin].ToString();

            if (SAV.MyStatus.DressUpSkinColor == CB_SkinColor.SelectedIndex)
                return;

            if (SAV.Gender == skin || DialogResult.Yes == WinFormsUtil.Prompt(MessageBoxButtons.YesNo, $"Gender-Skin mismatch:{Environment.NewLine}Gender: {gStr}, Skin: {sStr}", "Save selected Skin Color?"))
                SAV.MyStatus.DressUpSkinColor = CB_SkinColor.SelectedIndex;
        }

        private void SaveThrowType()
        {
            if (CB_BallThrowType.SelectedIndex >= 0)
                SAV.MyStatus.BallThrowType = CB_BallThrowType.SelectedIndex;

            if (SAV is not SAV7SM) // unlock flags are in flag editor instead
                return;

            const int unlockStart = 292;
            const int learnedStart = 3479;
            for (int i = 2; i < BattleStyles.Count; i++)
                SAV.SetEventFlag(unlockStart + i, LB_BallThrowTypeUnlocked.GetSelected(i));
            for (int i = 1; i < BattleStyles.Count; i++)
                SAV.SetEventFlag(learnedStart + i, LB_BallThrowTypeLearned.GetSelected(i));
        }

        private void SaveFlags()
        {
            SAV.Misc.Stamps = GetBits(LB_Stamps);

            SAV.SetEventFlag(333, CHK_UnlockSuperSingles.Checked);
            SAV.SetEventFlag(334, CHK_UnlockSuperDoubles.Checked);
            SAV.SetEventFlag(335, CHK_UnlockSuperMulti.Checked);

            SAV.MyStatus.MegaUnlocked = CHK_UnlockMega.Checked;
            SAV.MyStatus.ZMoveUnlocked = CHK_UnlockZMove.Checked;

            for (int i = 0; i < CLB_FlyDest.Items.Count; i++)
                SAV.SetEventFlag(SkipFlag + FlyDestFlagOfs[i], CLB_FlyDest.GetItemChecked(i));
            for (int i = 0; i < CLB_MapUnmask.Items.Count; i++)
                SAV.SetEventFlag(SkipFlag + MapUnmaskFlagOfs[i], CLB_MapUnmask.GetItemChecked(i));
        }

        private void SaveUltraData()
        {
            SAV.Misc.SetSurfScore(0, (int)NUD_Surf0.Value);
            SAV.Misc.SetSurfScore(1, (int)NUD_Surf1.Value);
            SAV.Misc.SetSurfScore(2, (int)NUD_Surf2.Value);
            SAV.Misc.SetSurfScore(3, (int)NUD_Surf3.Value);

            SAV.FieldMenu.RotomAffection = (ushort)NUD_RotomAffection.Value;
            SAV.FieldMenu.RotomLoto1 = CHK_RotoLoto1.Checked;
            SAV.FieldMenu.RotomLoto2 = CHK_RotoLoto2.Checked;

            if (TB_RotomOT.Text != TB_OTName.Text // different Rotom name from OT
                && TB_OTName.Text != SAV.OT // manually changed
                && DialogResult.Yes == // wants to update
                WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Rotom OT does not match OT name. Update Rotom OT name with OT name?"))
            {
                SAV.FieldMenu.RotomOT = TB_OTName.Text;
            }
            else
            {
                SAV.FieldMenu.RotomOT = TB_RotomOT.Text;
            }
        }

        private static uint GetBits(ListBox listbox)
        {
            uint bits = 0;
            for (int i = 0; i < listbox.Items.Count; i++)
            {
                if (listbox.GetSelected(i))
                    bits |= (uint)(1 << i);
            }
            return bits;
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

        private void UpdateCountry(object sender, EventArgs e)
        {
            int index;
            if (sender is ComboBox c && (index = WinFormsUtil.GetIndex(c)) > 0)
                Main.SetCountrySubRegion(CB_Region, $"sr_{index:000}");
        }

        private void B_Fashion_Click(object sender, EventArgs e)
        {
            var prompt = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Modifying Fashion Items will clear existing fashion unlock data.", "Continue?");
            if (DialogResult.Yes != prompt)
                return;

            // Clear Block
            SAV.Fashion.Clear();

            // Write Payload

            switch (CB_Fashion.SelectedIndex)
            {
                case 0: // Base Fashion
                {
                    SAV.Fashion.Reset();
                    break;
                }
                case 1: // Full Legal
                    byte[] data1 = SAV is SAV7USUM
                        ? SAV.Gender == 0 ? Properties.Resources.fashion_m_uu : Properties.Resources.fashion_f_uu
                        : SAV.Gender == 0 ? Properties.Resources.fashion_m_sm : Properties.Resources.fashion_f_sm;
                    SAV.SetData(data1, SAV.Fashion.Offset);
                    break;
                case 2: // Everything
                    byte[] data2 = SAV is SAV7USUM
                        ? SAV.Gender == 0 ? Properties.Resources.fashion_m_uu_illegal : Properties.Resources.fashion_f_uu_illegal
                        : SAV.Gender == 0 ? Properties.Resources.fashion_m_sm_illegal : Properties.Resources.fashion_f_sm_illegal;
                    SAV.SetData(data2, SAV.Fashion.Offset);
                    break;
                default:
                    return;
            }
            System.Media.SystemSounds.Asterisk.Play();
        }

        private string? UpdateTip(int index)
        {
            switch (index)
            {
                case 2: // Storyline Completed Time
                    var seconds = DateUtil.GetSecondsFrom2000(CAL_AdventureStartDate.Value, CAL_AdventureStartTime.Value);
                    return DateUtil.ConvertDateValueToString(SAV.GetRecord(index), seconds);
                default:
                    return null;
            }
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

        private void B_AllFlyDest_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < CLB_FlyDest.Items.Count; i++)
                CLB_FlyDest.SetItemChecked(i, true);
        }

        private void B_AllMapUnmask_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < CLB_MapUnmask.Items.Count; i++)
                CLB_MapUnmask.SetItemChecked(i, true);
        }
    }
}
