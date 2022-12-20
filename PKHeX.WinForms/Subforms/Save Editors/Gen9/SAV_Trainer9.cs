using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using static PKHeX.Core.SaveBlockAccessor9SV;

namespace PKHeX.WinForms;

public partial class SAV_Trainer9 : Form
{
    private readonly SaveFile Origin;
    private readonly SAV9SV SAV;

    public SAV_Trainer9(SaveFile sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV9SV)(Origin = sav).Clone();

        Loading = true;
        if (Main.Unicode)
        {
            TB_OTName.Font = FontUtil.GetPKXFont();
        }

        B_MaxCash.Click += (sender, e) => MT_Money.Text = SAV.MaxMoney.ToString();
        B_MaxLP.Click += (sender, e) => MT_LP.Text = SAV.MaxMoney.ToString();

        var games = GameInfo.Strings.gamelist;
        CB_Game.Items.Clear();
        CB_Game.Items.Add(games[(int)GameVersion.SL]);
        CB_Game.Items.Add(games[(int)GameVersion.VL]);

        CB_Gender.Items.Clear();
        CB_Gender.Items.AddRange(Main.GenderSymbols.Take(2).ToArray()); // m/f depending on unicode selection

        GetComboBoxes();
        GetTextBoxes();
        LoadMap();

        Loading = false;
    }

    private readonly bool Loading;
    private bool MapUpdated;

    private void LoadMap()
    {
        try
        {
            NUD_X.Value = (decimal)SAV.X;
            NUD_Y.Value = (decimal)SAV.Y;
            NUD_Z.Value = (decimal)SAV.Z;
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
        CB_Game.SelectedIndex = SAV.Game - (int)GameVersion.SL;
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
    }

    private void Save()
    {
        SaveTrainerInfo();
        SaveMap();
    }

    private void SaveMap()
    {
        if (!MapUpdated)
            return;
        SAV.SetCoordinates((float)NUD_X.Value, (float)NUD_Y.Value, (float)NUD_Z.Value);
    }

    private void SaveTrainerInfo()
    {
        SAV.Game = (byte)(CB_Game.SelectedIndex + (int)GameVersion.SL);
        SAV.Gender = (byte)CB_Gender.SelectedIndex;

        SAV.Money = Util.ToUInt32(MT_Money.Text);
        SAV.LeaguePoints = Util.ToUInt32(MT_LP.Text);
        SAV.Language = WinFormsUtil.GetIndex(CB_Language);
        SAV.OT = TB_OTName.Text;

        // Save PlayTime
        SAV.PlayedHours = ushort.Parse(MT_Hours.Text);
        SAV.PlayedMinutes = ushort.Parse(MT_Minutes.Text)%60;
        SAV.PlayedSeconds = ushort.Parse(MT_Seconds.Text)%60;
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

    private void B_UnlockFlyLocations_Click(object sender, EventArgs e) => UnlockFlyLocations();

    private void UnlockFlyLocations()
    {
        var blocks = new[]
        {
            #region Fly Flags
            FSYS_YMAP_FLY_01,
            FSYS_YMAP_FLY_03,
            FSYS_YMAP_FLY_12,
            FSYS_YMAP_FLY_02,
            FSYS_YMAP_FLY_22,
            FSYS_YMAP_FLY_04,
            FSYS_YMAP_FLY_05,
            FSYS_YMAP_FLY_23,
            FSYS_YMAP_FLY_24,
            FSYS_YMAP_FLY_14,
            FSYS_YMAP_FLY_15,
            FSYS_YMAP_FLY_07,
            FSYS_YMAP_FLY_08,
            FSYS_YMAP_FLY_09,
            FSYS_YMAP_FLY_25,
            FSYS_YMAP_FLY_10,
            FSYS_YMAP_FLY_11,
            FSYS_YMAP_FLY_26,
            FSYS_YMAP_FLY_18,
            FSYS_YMAP_FLY_16,
            FSYS_YMAP_FLY_17,
            FSYS_YMAP_FLY_19,
            FSYS_YMAP_FLY_27,
            FSYS_YMAP_FLY_28,
            FSYS_YMAP_FLY_13,
            FSYS_YMAP_FLY_29,
            FSYS_YMAP_FLY_30,
            FSYS_YMAP_FLY_20,
            FSYS_YMAP_FLY_21,
            FSYS_YMAP_FLY_31,
            FSYS_YMAP_FLY_32,
            FSYS_YMAP_FLY_33,
            FSYS_YMAP_FLY_34,
            FSYS_YMAP_FLY_35,
            FSYS_YMAP_FLY_MAGATAMA,
            FSYS_YMAP_FLY_MOKKAN,
            FSYS_YMAP_FLY_TSURUGI,
            FSYS_YMAP_FLY_UTSUWA,
            FSYS_YMAP_MAGATAMA,
            FSYS_YMAP_MOKKAN,
            FSYS_YMAP_TSURUGI,
            FSYS_YMAP_UTSUWA,
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
            #endregion
        };
        var accessor = SAV.Accessor;
        foreach (var block in blocks)
            accessor.GetBlock(block).ChangeBooleanType(SCTypeCode.Bool2);
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_CollectAllStakes_Click(object sender, EventArgs e)
    {
        SAV.CollectAllStakes();
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_UnlockTMRecipes_Click(object sender, EventArgs e)
    {
        SAV.UnlockAllTMRecipes();
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_UnlockBikeUpgrades_Click(object sender, EventArgs e)
    {
        var blocks = new[]
        {
            "FSYS_RIDE_DASH_ENABLE",
            "FSYS_RIDE_SWIM_ENABLE",
            "FSYS_RIDE_HIJUMP_ENABLE",
            "FSYS_RIDE_GLIDE_ENABLE",
            "FSYS_RIDE_CLIMB_ENABLE",
        };

        var accessor = SAV.Accessor;
        foreach (var block in blocks)
            accessor.GetBlock(block).ChangeBooleanType(SCTypeCode.Bool2);
        System.Media.SystemSounds.Asterisk.Play();
    }
}
