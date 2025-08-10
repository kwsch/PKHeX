using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_Trainer4BR : Form
{
    private readonly SAV4BR Origin;
    private readonly SAV4BR SAV;

    public SAV_Trainer4BR(SAV4BR sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV4BR)(Origin = sav).Clone();

        TB_OTName.MaxLength = SAV.MaxStringLengthTrainer;
        B_MaxCash.Click += (_, _) => MT_Money.Text = SAV.MaxMoney.ToString();
        CB_Country.InitializeBinding();
        CB_Region.InitializeBinding();
        Main.SetCountrySubRegion(CB_Country, "gen4_countries");
        CB_Language.InitializeBinding();
        CB_Language.DataSource = GameInfo.LanguageDataSource(3);

        TB_OTName.Text = SAV.CurrentOT;
        TB_BirthMonth.Text = SAV.BirthMonth;
        TB_BirthDay.Text = SAV.BirthDay;
        CB_Country.SelectedValue = SAV.Country;
        CB_Region.SelectedValue = SAV.Region;
        TB_SelfIntroduction.Lines = SAV.SelfIntroduction.TrimStart(StringConverter4GC.Proportional).Split(StringConverter4GC.LineBreak);
        MT_PlayerID.Text = SAV.PlayerID.ToString("X16");
        CB_Language.SelectedValue = SAV.Language;

        MT_Hours.Text = SAV.PlayedHours.ToString();
        MT_Minutes.Text = SAV.PlayedMinutes.ToString();
        MT_Seconds.Text = SAV.PlayedSeconds.ToString();
        MT_Money.Text = SAV.Money.ToString();
        MT_TID.Text = SAV.TID16.ToString("00000");
        MT_SID.Text = SAV.SID16.ToString("00000");

        NUD_RecordTotalBattles.Value = SAV.RecordTotalBattles;
        NUD_RecordColosseumBattles.Value = SAV.RecordColosseumBattles;
        NUD_RecordFreeBattles.Value = SAV.RecordFreeBattles;
        NUD_RecordWiFiBattles.Value = SAV.RecordWiFiBattles;
        NUD_RecordGatewayColosseumClears.Value = SAV.RecordGatewayColosseumClears;
        NUD_RecordMainStreetColosseumClears.Value = SAV.RecordMainStreetColosseumClears;
        NUD_RecordWaterfallColosseumClears.Value = SAV.RecordWaterfallColosseumClears;
        NUD_RecordNeonColosseumClears.Value = SAV.RecordNeonColosseumClears;
        NUD_RecordCrystalColosseumClears.Value = SAV.RecordCrystalColosseumClears;
        NUD_RecordSunnyParkColosseumClears.Value = SAV.RecordSunnyParkColosseumClears;
        NUD_RecordMagmaColosseumClears.Value = SAV.RecordMagmaColosseumClears;
        NUD_RecordCourtyardColosseumClears.Value = SAV.RecordCourtyardColosseumClears;
        NUD_RecordSunsetColosseumClears.Value = SAV.RecordSunsetColosseumClears;
        NUD_RecordStargazerColosseumClears.Value = SAV.RecordStargazerColosseumClears;

        CHK_1.Checked = SAV.UnlockedGatewayColosseum;
        CHK_2.Checked = SAV.UnlockedMainStreetColosseum;
        CHK_3.Checked = SAV.UnlockedWaterfallColosseum;
        CHK_4.Checked = SAV.UnlockedNeonColosseum;
        CHK_5.Checked = SAV.UnlockedCrystalColosseum;
        CHK_6.Checked = SAV.UnlockedSunnyParkColosseum;
        CHK_7.Checked = SAV.UnlockedMagmaColosseum;
        CHK_8.Checked = SAV.UnlockedSunsetColosseum;
        CHK_9.Checked = SAV.UnlockedCourtyardColosseum;
        CHK_10.Checked = SAV.UnlockedStargazerColosseum;
        CHK_PostGame.Checked = SAV.UnlockedPostGame;
    }

    private void ChangeFFFF(object sender, EventArgs e)
    {
        MaskedTextBox box = (MaskedTextBox)sender;
        if (box.Text.Length == 0) box.Text = "0";
        if (Util.ToInt32(box.Text) > 65535) box.Text = "65535";
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        if (SAV.CurrentOT != TB_OTName.Text) // only modify if changed (preserve trash bytes?)
            SAV.CurrentOT = TB_OTName.Text;

        SAV.BirthMonth = TB_BirthMonth.Text;
        SAV.BirthDay = TB_BirthDay.Text;
        SAV.Country = WinFormsUtil.GetIndex(CB_Country);
        SAV.Region = WinFormsUtil.GetIndex(CB_Region);
        SAV.SelfIntroduction = (SAV.Japanese ? string.Empty : StringConverter4GC.Proportional.ToString()) + string.Join(StringConverter4GC.LineBreak, TB_SelfIntroduction.Lines);
        SAV.PlayerID = Util.GetHexValue64(MT_PlayerID.Text);
        SAV.Language = WinFormsUtil.GetIndex(CB_Language);

        SAV.PlayedHours = ushort.Parse(MT_Hours.Text);
        SAV.PlayedMinutes = ushort.Parse(MT_Minutes.Text) % 60;
        SAV.PlayedSeconds = ushort.Parse(MT_Seconds.Text) % 60;
        SAV.Money = Util.ToUInt32(MT_Money.Text);
        SAV.TID16 = (ushort)Util.ToUInt32(MT_TID.Text);
        SAV.SID16 = (ushort)Util.ToUInt32(MT_SID.Text);

        SAV.RecordTotalBattles = (uint)NUD_RecordTotalBattles.Value;
        SAV.RecordColosseumBattles = (uint)NUD_RecordColosseumBattles.Value;
        SAV.RecordFreeBattles = (uint)NUD_RecordFreeBattles.Value;
        SAV.RecordWiFiBattles = (uint)NUD_RecordWiFiBattles.Value;
        SAV.RecordGatewayColosseumClears = (byte)NUD_RecordGatewayColosseumClears.Value;
        SAV.RecordMainStreetColosseumClears = (byte)NUD_RecordMainStreetColosseumClears.Value;
        SAV.RecordWaterfallColosseumClears = (byte)NUD_RecordWaterfallColosseumClears.Value;
        SAV.RecordNeonColosseumClears = (byte)NUD_RecordNeonColosseumClears.Value;
        SAV.RecordCrystalColosseumClears = (byte)NUD_RecordCrystalColosseumClears.Value;
        SAV.RecordSunnyParkColosseumClears = (byte)NUD_RecordSunnyParkColosseumClears.Value;
        SAV.RecordMagmaColosseumClears = (byte)NUD_RecordMagmaColosseumClears.Value;
        SAV.RecordCourtyardColosseumClears = (byte)NUD_RecordCourtyardColosseumClears.Value;
        SAV.RecordSunsetColosseumClears = (byte)NUD_RecordSunsetColosseumClears.Value;
        SAV.RecordStargazerColosseumClears = (byte)NUD_RecordStargazerColosseumClears.Value;

        SAV.UnlockedGatewayColosseum = CHK_1.Checked;
        SAV.UnlockedMainStreetColosseum = CHK_2.Checked;
        SAV.UnlockedWaterfallColosseum = CHK_3.Checked;
        SAV.UnlockedNeonColosseum = CHK_4.Checked;
        SAV.UnlockedCrystalColosseum = CHK_5.Checked;
        SAV.UnlockedSunnyParkColosseum = CHK_6.Checked;
        SAV.UnlockedMagmaColosseum = CHK_7.Checked;
        SAV.UnlockedSunsetColosseum = CHK_8.Checked;
        SAV.UnlockedCourtyardColosseum = CHK_9.Checked;
        SAV.UnlockedStargazerColosseum = CHK_10.Checked;
        SAV.UnlockedPostGame = CHK_PostGame.Checked;

        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void ValidateCatchphrase(object sender, EventArgs e)
    {
        if (sender is not TextBox tb)
            return;

        int i = 0;
        int length = 0;
        foreach (string line in tb.Lines)
        {
            foreach (char c in line)
            {
                length += c switch
                {
                    StringConverter4GC.LineBreak or
                        StringConverter4GC.Proportional or
                        StringConverter4GC.PokemonName => 2,
                    _ => 1,
                };
                if (length > tb.MaxLength)
                {
                    tb.Text = tb.Text[..i];
                    return;
                }
                i++;
            }
            length += 2;
            i += Environment.NewLine.Length;
        }
    }

    private void ValidatePlayerID(object sender, EventArgs e)
    {
        if (sender is MaskedTextBox mt)
            mt.Text = Util.GetHexValue64(mt.Text).ToString("X16");
    }

    private void UpdateCountry(object sender, EventArgs e)
    {
        if (sender is ComboBox c)
        {
            int index = WinFormsUtil.GetIndex(c);
            Main.SetCountrySubRegion(CB_Region, $"gen4_sr_{index:000}");
            if (CB_Region.Items.Count == 0)
                Main.SetCountrySubRegion(CB_Region, "gen4_sr_default");
        }
    }

    private void CB_Language_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (sender is not ComboBox c)
            return;

        int index = WinFormsUtil.GetIndex(c);
        if (index != (int)LanguageID.Japanese)
            TB_SelfIntroduction.MaxLength = 51;
        else
            TB_SelfIntroduction.MaxLength = 53;
        ValidateCatchphrase(TB_SelfIntroduction, e);
    }
}
