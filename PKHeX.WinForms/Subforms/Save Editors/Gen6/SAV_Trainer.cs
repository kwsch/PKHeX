using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.Misc;

namespace PKHeX.WinForms;

public partial class SAV_Trainer : Form
{
    private readonly SaveFile Origin;
    private readonly SAV6 SAV;

    public SAV_Trainer(SAV6 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV6)(Origin = sav).Clone();
        if (Main.Unicode)
        {
            TB_OTName.Font = FontUtil.GetPKXFont();
            if (SAV is SAV6XY)
                TB_TRNick.Font = TB_OTName.Font;
        }

        B_MaxCash.Click += (_, _) => MT_Money.Text = "9,999,999";

        CB_Gender.Items.Clear();
        CB_Gender.Items.AddRange(Main.GenderSymbols.Take(2).ToArray()); // m/f depending on unicode selection

        TrainerStats.LoadRecords(SAV, RecordLists.RecordList_6);
        TrainerStats.GetToolTipText = UpdateTip;

        MaisonRecords =
        [
            TB_MCSN,TB_MCSS,TB_MBSN,TB_MBSS,
            TB_MCDN,TB_MCDS,TB_MBDN,TB_MBDS,
            TB_MCTN,TB_MCTS,TB_MBTN,TB_MBTS,
            TB_MCRN,TB_MCRS,TB_MBRN,TB_MBRS,
            TB_MCMN,TB_MCMS,TB_MBMN,TB_MBMS,
        ];
        cba = [CHK_Badge1, CHK_Badge2, CHK_Badge3, CHK_Badge4, CHK_Badge5, CHK_Badge6, CHK_Badge7, CHK_Badge8];

        L_MultiplayerSprite.Enabled = CB_MultiplayerSprite.Enabled =
            L_MultiplayerSprite.Visible = CB_MultiplayerSprite.Visible = PB_Sprite.Visible = SAV is not SAV6AODemo;
        CHK_MegaRayquazaUnlocked.Visible = SAV is SAV6AO;

        L_Style.Visible = TB_Style.Visible = SAV is SAV6XY;
        if (SAV is not SAV6XY)
            TC_Editor.TabPages.Remove(Tab_Appearance);

        if (SAV is SAV6AODemo)
        {
            TC_Editor.TabPages.Remove(Tab_Multiplayer);
            TC_Editor.TabPages.Remove(Tab_Maison);
        }

        editing = true;
        GetComboBoxes();
        GetTextBoxes();
        editing = false;

        var status = SAV.Status;
        CHK_MegaUnlocked.Checked = status.IsMegaEvolutionUnlocked;
        CHK_MegaRayquazaUnlocked.Checked = status.IsMegaRayquazaUnlocked;
    }

    private readonly bool editing;
    private readonly MaskedTextBox[] MaisonRecords;
    private readonly CheckBox[] cba;
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

        var names = WinFormsTranslator.GetEnumTranslation<TrainerSprite6>(Main.CurrentLanguage);
        var values = Enum.GetValues<TrainerSprite6>();
        var max = SAV is not SAV6AO ? (int)TrainerSprite6.Trevor : names.Length;
        var data = new ComboItem[max];
        for (int i = 0; i < max; i++)
            data[i] = new ComboItem(names[i], (int)values[i]);
        CB_MultiplayerSprite.InitializeBinding();
        CB_MultiplayerSprite.DataSource = data;

        L_Vivillon.Text = GameInfo.Strings.specieslist[(int)Species.Vivillon] + ":";
        CB_Vivillon.InitializeBinding();
        CB_Vivillon.DataSource = FormConverter.GetFormList((int)Species.Vivillon, GameInfo.Strings.types, GameInfo.Strings.forms, Main.GenderSymbols, SAV.Context);
    }

    private void GetTextBoxes()
    {
        int badgeval = SAV.Badges;
        for (int i = 0; i < 8; i++)
            cba[i].Checked = (badgeval & (1 << i)) != 0;

        // Get Data
        string OT_NAME = SAV.OT;

        CB_Game.SelectedIndex = (int)(SAV.Version - 0x18);
        CB_Gender.SelectedIndex = SAV.Gender;

        // Display Data
        TB_OTName.Text = OT_NAME;

        MT_TID.Text = SAV.TID16.ToString("00000");
        MT_SID.Text = SAV.SID16.ToString("00000");
        MT_Money.Text = SAV.Money.ToString();

        var status = SAV.Status;
        TB_Saying1.Text = status.Saying1;
        TB_Saying2.Text = status.Saying2;
        TB_Saying3.Text = status.Saying3;
        TB_Saying4.Text = status.Saying4;
        TB_Saying5.Text = status.Saying5;

        CB_Country.SelectedValue = (int)SAV.Country;
        CB_Region.SelectedValue = (int)SAV.Region;
        CB_3DSReg.SelectedValue = (int)SAV.ConsoleRegion;
        CB_Language.SelectedValue = SAV.Language;

        // Maison Data
        if (SAV is ISaveBlock6Main xyao)
        {
            for (int i = 0; i < MaisonBlock.MaisonStatCount; i++)
                MaisonRecords[i].Text = xyao.Maison.GetMaisonStat(i).ToString();
        }

        var sit = SAV.Situation;
        NUD_M.Value = sit.M;
        NUD_R.Value = sit.R;
        // Sanity Check Map Coordinates
        try
        {
            NUD_X.Value = (decimal)(sit.X / 18.0);
            NUD_Z.Value = (decimal)(sit.Z / 18.0);
            NUD_Y.Value = (decimal)(sit.Y / 18.0);
        }
        catch { GB_Map.Enabled = false; }

        // Load BP and PokeMiles
        TB_BP.Text = SAV.BP.ToString();
        TB_PM.Text = SAV.GetRecord(63).ToString();

        TB_Style.Text = sit.Style.ToString();

        // Load Play Time
        MT_Hours.Text = SAV.PlayedHours.ToString();
        MT_Minutes.Text = SAV.PlayedMinutes.ToString();
        MT_Seconds.Text = SAV.PlayedSeconds.ToString();

        // Load PSS Sprite
        if (SAV is IMultiplayerSprite ms)
            CB_MultiplayerSprite.SelectedValue = ms.MultiplayerSpriteID;
        PB_Sprite.Image = SAV.Sprite();

        if (SAV is SAV6XY xy)
        {
            var xystat = (MyStatus6XY)xy.Status;
            PG_CurrentAppearance.SelectedObject = xystat.Fashion;
            TB_TRNick.Text = xystat.Nickname;
        }

        CB_Vivillon.SelectedIndex = SAV.Vivillon;
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
    }

    private void Save()
    {
        SAV.Version = (GameVersion)(CB_Game.SelectedIndex + 0x18);
        SAV.Gender = (byte)CB_Gender.SelectedIndex;

        SAV.TID16 = (ushort)Util.ToUInt32(MT_TID.Text);
        SAV.SID16 = (ushort)Util.ToUInt32(MT_SID.Text);
        SAV.Money = Util.ToUInt32(MT_Money.Text);
        SAV.Region = (byte)WinFormsUtil.GetIndex(CB_Region);
        SAV.Country = (byte)WinFormsUtil.GetIndex(CB_Country);
        SAV.ConsoleRegion = (byte)WinFormsUtil.GetIndex(CB_3DSReg);
        SAV.Language = WinFormsUtil.GetIndex(CB_Language);

        SAV.OT = TB_OTName.Text;

        var status = SAV.Status;
        status.Saying1 = TB_Saying1.Text;
        status.Saying2 = TB_Saying2.Text;
        status.Saying3 = TB_Saying3.Text;
        status.Saying4 = TB_Saying4.Text;
        status.Saying5 = TB_Saying5.Text;

        // Copy Maison Data in
        if (SAV is ISaveBlock6Main xyao)
        {
            for (int i = 0; i < MaisonBlock.MaisonStatCount; i++)
                xyao.Maison.SetMaisonStat(i, ushort.Parse(MaisonRecords[i].Text));
        }

        // Copy Position
        var sit = SAV.Situation;
        if (GB_Map.Enabled && MapUpdated)
        {
            sit.M = (int)NUD_M.Value;
            sit.X = (float)(NUD_X.Value * 18);
            sit.Z = (float)(NUD_Z.Value * 18);
            sit.Y = (float)(NUD_Y.Value * 18);
            sit.R = (int)NUD_R.Value;
        }

        SAV.BP = ushort.Parse(TB_BP.Text);
        // Set Current PokéMiles
        SAV.SetRecord(63, Util.ToInt32(TB_PM.Text));
        // Set Max Obtained Pokémiles
        SAV.SetRecord(64, Util.ToInt32(TB_PM.Text));
        sit.Style = byte.Parse(TB_Style.Text);

        // Copy Badges
        int badgeval = 0;
        for (int i = 0; i < 8; i++)
            badgeval |= (cba[i].Checked ? 1 : 0) << i;
        SAV.Badges = badgeval;

        // Save PlayTime
        SAV.PlayedHours = ushort.Parse(MT_Hours.Text);
        SAV.PlayedMinutes = ushort.Parse(MT_Minutes.Text) % 60;
        SAV.PlayedSeconds = ushort.Parse(MT_Seconds.Text) % 60;

        // Sprite
        if (SAV is IMultiplayerSprite ms)
            ms.MultiplayerSpriteID = (byte)WinFormsUtil.GetIndex(CB_MultiplayerSprite);

        // Appearance
        if (SAV is SAV6XY xy)
        {
            var xystat = (MyStatus6XY)xy.Status;
            xystat.Fashion = (TrainerFashion6)PG_CurrentAppearance.SelectedObject;
            xystat.Nickname = TB_TRNick.Text;
        }

        // Vivillon
        SAV.Vivillon = CB_Vivillon.SelectedIndex;

        SAV.SecondsToStart = (uint)DateUtil.GetSecondsFrom2000(CAL_AdventureStartDate.Value, CAL_AdventureStartTime.Value);
        SAV.SecondsToFame = (uint)DateUtil.GetSecondsFrom2000(CAL_HoFDate.Value, CAL_HoFTime.Value);

        if (SAV.Played.LastSavedDate.HasValue)
            SAV.Played.LastSavedDate = new DateTime(CAL_LastSavedDate.Value.Year, CAL_LastSavedDate.Value.Month, CAL_LastSavedDate.Value.Day, CAL_LastSavedTime.Value.Hour, CAL_LastSavedTime.Value.Minute, 0);

        status.IsMegaEvolutionUnlocked = CHK_MegaUnlocked.Checked;
        status.IsMegaRayquazaUnlocked = CHK_MegaRayquazaUnlocked.Checked;
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

    private void ShowTSV(object sender, EventArgs e)
    {
        uint TID16 = Util.ToUInt32(MT_TID.Text);
        uint SID16 = Util.ToUInt32(MT_SID.Text);
        uint tsv = (TID16 ^ SID16) >> 4;
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
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void Change255(object sender, EventArgs e)
    {
        MaskedTextBox box = (MaskedTextBox)sender;
        if (box.Text.Length == 0)
            box.Text = "0";
        else if (Util.ToInt32(box.Text) > 255)
            box.Text = "255";
    }

    private void ChangeFFFF(object sender, EventArgs e)
    {
        MaskedTextBox box = (MaskedTextBox)sender;
        if (box.Text.Length == 0)
            box.Text = "0";
        else if (Util.ToInt32(box.Text) > 65535)
            box.Text = "65535";
    }

    private void GiveAllAccessories(object sender, EventArgs e)
    {
        if (SAV is SAV6XY xy)
        {
            xy.Blocks.Fashion.UnlockAllAccessories();
            System.Media.SystemSounds.Asterisk.Play();
        }
    }

    private void UpdateCountry(object sender, EventArgs e)
    {
        int index;
        if (sender is ComboBox c && (index = WinFormsUtil.GetIndex(c)) > 0)
            Main.SetCountrySubRegion(CB_Region, $"sr_{index:000}");
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

        if (SAV is IMultiplayerSprite ms)
            ms.MultiplayerSpriteID = WinFormsUtil.GetIndex(CB_MultiplayerSprite);
        PB_Sprite.Image = SAV.Sprite();
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
}
