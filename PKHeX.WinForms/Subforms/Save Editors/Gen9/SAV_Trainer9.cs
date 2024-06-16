using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing;
using static PKHeX.Core.SaveBlockAccessor9SV;

namespace PKHeX.WinForms;

public partial class SAV_Trainer9 : Form
{
    private readonly SaveFile Origin;
    private readonly SAV9SV SAV;

    public SAV_Trainer9(SAV9SV sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV9SV)(Origin = sav).Clone();

        Loading = true;
        if (Main.Unicode)
        {
            TB_OTName.Font = FontUtil.GetPKXFont();
        }

        B_MaxCash.Click += (_, _) => MT_Money.Text = SAV.MaxMoney.ToString();
        B_MaxLP.Click += (_, _) => MT_LP.Text = SAV.MaxMoney.ToString();
        B_MaxBP.Click += (_, _) => MT_BP.Text = SAV.MaxMoney.ToString();

        var games = GameInfo.Strings.gamelist;
        CB_Game.Items.Clear();
        CB_Game.Items.Add(games[(int)GameVersion.SL]);
        CB_Game.Items.Add(games[(int)GameVersion.VL]);

        CB_Gender.Items.Clear();
        CB_Gender.Items.AddRange(Main.GenderSymbols.Take(2).ToArray()); // m/f depending on unicode selection

        GetImages();
        GetComboBoxes();
        GetTextBoxes();
        LoadMap();

        if (SAV.SaveRevision >= 2)
            LoadBlueberry();
        else
            TC_Editor.TabPages.Remove(Tab_Blueberry);

        Loading = false;
    }

    private void LoadBlueberry()
    {
        var bbq = SAV.BlueberryQuestRecord;
        MT_BP.Text = SAV.BlueberryPoints.ToString();
        NUD_BBQSolo.Value = bbq.QuestsDoneSolo;
        NUD_BBQGroup.Value = bbq.QuestsDoneGroup;

        CB_ThrowStyle.Items.Clear();
        CB_ThrowStyle.Items.AddRange(Util.GetStringList("throw_styles", Main.CurrentLanguage));
        CB_ThrowStyle.SelectedIndex = (int)SAV.ThrowStyle - 1;
    }

    private void GetImages()
    {
        static Bitmap GetImage(SCBlockAccessor blocks, uint kd, uint kw, uint kh)
        {
            var data = blocks.GetBlock(kd).Data;
            var width = blocks.GetBlockValue<uint>(kw);
            var height = blocks.GetBlockValue<uint>(kh);
            var result = DXT1.Decompress(data, (int)width, (int)height);
            return ImageUtil.GetBitmap(result, (int)width, (int)height, PixelFormat.Format32bppArgb);
        }

        var blocks = SAV.Blocks;
        P_CurrPhoto.Image = GetImage(blocks, KPictureProfileCurrent, KPictureProfileCurrentWidth, KPictureProfileCurrentHeight);
        P_CurrIcon.Image = GetImage(blocks, KPictureIconCurrent, KPictureIconCurrentWidth, KPictureIconCurrentHeight);
        P_InitialIcon.Image = GetImage(blocks, KPictureIconInitial, KPictureIconInitialWidth, KPictureIconInitialHeight);
    }

    private readonly bool Loading;
    private bool MapUpdated;

    private void LoadMap()
    {
        try
        {
            NUD_X.Value = (decimal)(double)SAV.X;
            NUD_Y.Value = (decimal)(double)SAV.Y;
            NUD_Z.Value = (decimal)(double)SAV.Z;
            NUD_R.Value = (decimal)(Math.Atan2(SAV.RZ, SAV.RW) * 360.0 / Math.PI);
        }
        // If we can't accurately represent the coordinates, don't allow them to be changed.
        catch { GB_Map.Enabled = false; }
    }

    private void GetComboBoxes()
    {
        CB_Language.InitializeBinding();
        CB_Language.DataSource = GameInfo.LanguageDataSource(SAV.Generation);
    }

    private void GetTextBoxes()
    {
        // Get Data
        CB_Game.SelectedIndex = SAV.Version - GameVersion.SL;
        CB_Gender.SelectedIndex = SAV.Gender;

        // Display Data
        TB_OTName.Text = SAV.OT;
        trainerID1.LoadIDValues(SAV, SAV.Generation);
        MT_Money.Text = SAV.Money.ToString();
        MT_LP.Text = SAV.LeaguePoints.ToString();
        CB_Language.SelectedValue = SAV.Language;

        // Load Play Time
        MT_Hours.Text = SAV.PlayedHours.ToString();
        MT_Minutes.Text = SAV.PlayedMinutes.ToString();
        MT_Seconds.Text = SAV.PlayedSeconds.ToString();

        CAL_AdventureStartDate.Value = SAV.EnrollmentDate.Timestamp;
        CAL_LastSavedDate.Value = CAL_LastSavedTime.Value = SAV.LastSaved.Timestamp;
    }

    private void Save()
    {
        SaveTrainerInfo();
        SaveMap();
        if (SAV.SaveRevision >= 2)
            SaveBlueberry();
    }

    private void SaveBlueberry()
    {
        var bbq = SAV.BlueberryQuestRecord;
        SAV.BlueberryPoints = Util.ToUInt32(MT_BP.Text);
        bbq.QuestsDoneSolo = (uint)NUD_BBQSolo.Value;
        bbq.QuestsDoneGroup = (uint)NUD_BBQGroup.Value;
        SAV.ThrowStyle = (ThrowStyle9)CB_ThrowStyle.SelectedIndex + 1;
    }

    private void SaveMap()
    {
        if (!MapUpdated)
            return;
        SAV.SetCoordinates((float)NUD_X.Value, (float)NUD_Y.Value, (float)NUD_Z.Value);
        var angle = (double)NUD_R.Value * Math.PI / 360.0;
        SAV.SetPlayerRotation(0, (float)Math.Sin(angle), 0, (float)Math.Cos(angle));
    }

    private void SaveTrainerInfo()
    {
        SAV.Version = (GameVersion)(CB_Game.SelectedIndex + (byte)GameVersion.SL);
        SAV.Gender = (byte)CB_Gender.SelectedIndex;

        SAV.Money = Util.ToUInt32(MT_Money.Text);
        SAV.LeaguePoints = Util.ToUInt32(MT_LP.Text);
        SAV.Language = WinFormsUtil.GetIndex(CB_Language);
        SAV.OT = TB_OTName.Text;

        // Save PlayTime
        SAV.PlayedHours = ushort.Parse(MT_Hours.Text);
        SAV.PlayedMinutes = ushort.Parse(MT_Minutes.Text) % 60;
        SAV.PlayedSeconds = ushort.Parse(MT_Seconds.Text) % 60;

        SAV.EnrollmentDate.Timestamp = CAL_AdventureStartDate.Value;
        SAV.LastSaved.Timestamp = CAL_LastSavedDate.Value.Date.AddSeconds(CAL_LastSavedTime.Value.TimeOfDay.TotalSeconds);

        if (SAV.Blocks.TryGetBlock(KBlueberryPoints, out var block))
            block.SetValue(Util.ToUInt32(MT_BP.Text));
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

    private void ChangeMapValue(object sender, EventArgs e)
    {
        if (!Loading)
            MapUpdated = true;
    }

    private void Change255(object sender, EventArgs e)
    {
        MaskedTextBox box = (MaskedTextBox)sender;
        if (box.Text.Length == 0) box.Text = "0";
        if (Util.ToInt32(box.Text) > 255) box.Text = "255";
    }

    private void B_UnlockFlyLocations_Click(object sender, EventArgs e)
    {
        var accessor = SAV.Accessor;
        foreach (var hash in FlyHashes)
        {
            if (accessor.TryGetBlock(hash, out var block))
                block.ChangeBooleanType(SCTypeCode.Bool2);
        }
        B_UnlockFlyLocations.Enabled = false;
        System.Media.SystemSounds.Asterisk.Play();
    }

    private static ReadOnlySpan<uint> FlyHashes =>
    [
        #region Fly Flags
        FSYS_YMAP_FLY_01,
        FSYS_YMAP_FLY_02,
        FSYS_YMAP_FLY_03,
        FSYS_YMAP_FLY_04,
        FSYS_YMAP_FLY_05,
        FSYS_YMAP_FLY_06,
        FSYS_YMAP_FLY_07,
        FSYS_YMAP_FLY_08,
        FSYS_YMAP_FLY_09,
        FSYS_YMAP_FLY_10,
        FSYS_YMAP_FLY_11,
        FSYS_YMAP_FLY_12,
        FSYS_YMAP_FLY_13,
        FSYS_YMAP_FLY_14,
        FSYS_YMAP_FLY_15,
        FSYS_YMAP_FLY_16,
        FSYS_YMAP_FLY_17,
        FSYS_YMAP_FLY_18,
        FSYS_YMAP_FLY_19,
        FSYS_YMAP_FLY_20,
        FSYS_YMAP_FLY_21,
        FSYS_YMAP_FLY_22,
        FSYS_YMAP_FLY_23,
        FSYS_YMAP_FLY_24,
        FSYS_YMAP_FLY_25,
        FSYS_YMAP_FLY_26,
        FSYS_YMAP_FLY_27,
        FSYS_YMAP_FLY_28,
        FSYS_YMAP_FLY_29,
        FSYS_YMAP_FLY_30,
        FSYS_YMAP_FLY_31,
        FSYS_YMAP_FLY_32,
        FSYS_YMAP_FLY_33,
        FSYS_YMAP_FLY_34,
        FSYS_YMAP_FLY_35,
        FSYS_YMAP_FLY_MAGATAMA,
        FSYS_YMAP_FLY_MOKKAN,
        FSYS_YMAP_FLY_TSURUGI,
        FSYS_YMAP_FLY_UTSUWA,
        FSYS_YMAP_POKECEN_02,
        FSYS_YMAP_POKECEN_03,
        FSYS_YMAP_POKECEN_04,
        FSYS_YMAP_POKECEN_05,
        FSYS_YMAP_POKECEN_06,
        FSYS_YMAP_POKECEN_07,
        FSYS_YMAP_POKECEN_08,
        FSYS_YMAP_POKECEN_09,
        FSYS_YMAP_POKECEN_10,
        FSYS_YMAP_POKECEN_11,
        FSYS_YMAP_POKECEN_12,
        FSYS_YMAP_POKECEN_13,
        FSYS_YMAP_POKECEN_14,
        FSYS_YMAP_POKECEN_15,
        FSYS_YMAP_POKECEN_16,
        FSYS_YMAP_POKECEN_17,
        FSYS_YMAP_POKECEN_18,
        FSYS_YMAP_POKECEN_19,
        FSYS_YMAP_POKECEN_20,
        FSYS_YMAP_POKECEN_21,
        FSYS_YMAP_POKECEN_22,
        FSYS_YMAP_POKECEN_23,
        FSYS_YMAP_POKECEN_24,
        FSYS_YMAP_POKECEN_25,
        FSYS_YMAP_POKECEN_26,
        FSYS_YMAP_POKECEN_27,
        FSYS_YMAP_POKECEN_28,
        FSYS_YMAP_POKECEN_29,
        FSYS_YMAP_POKECEN_30,
        FSYS_YMAP_POKECEN_31,
        FSYS_YMAP_POKECEN_32,
        FSYS_YMAP_POKECEN_33,
        FSYS_YMAP_POKECEN_34,
        FSYS_YMAP_POKECEN_35,

        // Treasures of Ruin shrine toggles
        FSYS_YMAP_MAGATAMA,
        FSYS_YMAP_MOKKAN,
        FSYS_YMAP_TSURUGI,
        FSYS_YMAP_UTSUWA,

        // Sudachi 1
        FSYS_YMAP_SU1MAP_CHANGE, // can change map to Kitakami
        FSYS_YMAP_FLY_SU1_AREA10,
        FSYS_YMAP_FLY_SU1_BUSSTOP,
        FSYS_YMAP_FLY_SU1_CENTER01,
        FSYS_YMAP_FLY_SU1_PLAZA,
        FSYS_YMAP_FLY_SU1_SPOT01,
        FSYS_YMAP_FLY_SU1_SPOT02,
        FSYS_YMAP_FLY_SU1_SPOT03,
        FSYS_YMAP_FLY_SU1_SPOT04,
        FSYS_YMAP_FLY_SU1_SPOT05,
        FSYS_YMAP_FLY_SU1_SPOT06,

        // Sudachi 2
        FSYS_YMAP_S2_MAPCHANGE_ENABLE, // can change map to Blueberry Academy
        FSYS_YMAP_FLY_SU2_DRAGON,
        FSYS_YMAP_FLY_SU2_ENTRANCE,
        FSYS_YMAP_FLY_SU2_FAIRY,
        FSYS_YMAP_FLY_SU2_HAGANE,
        FSYS_YMAP_FLY_SU2_HONOO,
        FSYS_YMAP_FLY_SU2_SPOT01,
        FSYS_YMAP_FLY_SU2_SPOT02,
        FSYS_YMAP_FLY_SU2_SPOT03,
        FSYS_YMAP_FLY_SU2_SPOT04,
        FSYS_YMAP_FLY_SU2_SPOT05,
        FSYS_YMAP_FLY_SU2_SPOT06,
        FSYS_YMAP_FLY_SU2_SPOT07,
        FSYS_YMAP_FLY_SU2_SPOT08,
        FSYS_YMAP_FLY_SU2_SPOT09,
        FSYS_YMAP_FLY_SU2_SPOT10,
        FSYS_YMAP_FLY_SU2_SPOT11,
        FSYS_YMAP_POKECEN_SU02,
        #endregion
    ];

    private void B_CollectAllStakes_Click(object sender, EventArgs e)
    {
        SAV.CollectAllStakes();
        B_CollectAllStakes.Enabled = false;
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_UnlockTMRecipes_Click(object sender, EventArgs e)
    {
        SAV.UnlockAllTMRecipes();
        B_UnlockTMRecipes.Enabled = false;
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_ActivateSnacksworthLegendaries_Click(object sender, EventArgs e)
    {
        SAV.ActivateSnacksworthLegendaries();
        B_ActivateSnacksworthLegendaries.Enabled = false;
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_UnlockCoaches_Click(object sender, EventArgs e)
    {
        SAV.UnlockAllCoaches();
        B_UnlockCoaches.Enabled = false;
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_UnlockBikeUpgrades_Click(object sender, EventArgs e)
    {
        string[] blocks =
        [
            "FSYS_RIDE_DASH_ENABLE",
            "FSYS_RIDE_SWIM_ENABLE",
            "FSYS_RIDE_HIJUMP_ENABLE",
            "FSYS_RIDE_GLIDE_ENABLE",
            "FSYS_RIDE_CLIMB_ENABLE",
        ];

        var accessor = SAV.Accessor;
        foreach (var block in blocks)
            accessor.GetBlock(block).ChangeBooleanType(SCTypeCode.Bool2);
        if (accessor.TryGetBlock("FSYS_RIDE_FLIGHT_ENABLE", out var fly))
            fly.ChangeBooleanType(SCTypeCode.Bool2); // Base & DLC1 saves do not have this block
        B_UnlockBikeUpgrades.Enabled = false;
        System.Media.SystemSounds.Asterisk.Play();
    }

    private static void IMG_Save(Image image, string name)
    {
        var sfd = new SaveFileDialog
        {
            FileName = name,
            Filter = "Images|*.png;*.bmp;*.jpg",
        };
        if (sfd.ShowDialog() != DialogResult.OK)
            return;

        var path = sfd.FileName;
        var format = Path.GetExtension(path) switch
        {
            ".jpg" or ".jpeg" => ImageFormat.Jpeg,
            ".bmp" => ImageFormat.Bmp,
            _ => ImageFormat.Png,
        };
        image.Save(path, format);
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void P_CurrPhoto_Click(object sender, EventArgs e) => IMG_Save(P_CurrPhoto.Image, "current_photo");
    private void P_CurrIcon_Click(object sender, EventArgs e) => IMG_Save(P_CurrIcon.Image, "current_icon");
    private void P_InitialIcon_Click(object sender, EventArgs e) => IMG_Save(P_InitialIcon.Image, "initial_icon");

    private void B_UnlockClothing_Click(object sender, EventArgs e)
    {
        var accessor = SAV.Accessor;
        PlayerFashionUnlock9.UnlockBase(accessor, SAV.Gender);
        B_UnlockClothing.Enabled = false;
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_UnlockThrowStyles_Click(object sender, EventArgs e)
    {
        SAV.UnlockAllThrowStyles();
        B_UnlockThrowStyles.Enabled = false;
        System.Media.SystemSounds.Asterisk.Play();
    }
}
