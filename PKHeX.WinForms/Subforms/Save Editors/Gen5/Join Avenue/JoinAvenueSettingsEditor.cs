using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class JoinAvenueSettingsEditor : UserControl
{
    private const int VisitingPlayerColumnIndex = 0;
    private const int VisitingPlayerColumnTID = 1;
    private const int VisitingPlayerColumnSID = 2;

    private static readonly List<ComboItem> CeilingColorList = WinFormsTranslator.GetEnumTranslation<JoinAvenueCeilingColor5>(Main.CurrentLanguage)
        .Select((z, i) => new ComboItem(z, i)).ToList();

    public JoinAvenueSettingsEditor()
    {
        InitializeComponent();
        CB_CeilingColor.InitializeBinding();
        CB_CeilingColor.DataSource = new BindingSource(CeilingColorList, string.Empty);
        DGV_VisitingPlayerDatabase.Rows.Add(JoinAvenueSettings5.CountVisitingPlayersRemembered);
        for (int i = 0; i < JoinAvenueSettings5.CountVisitingPlayersRemembered; i++)
            DGV_VisitingPlayerDatabase.Rows[i].Cells[VisitingPlayerColumnIndex].Value = i + 1;
    }

    public void LoadObject(JoinAvenueSettings5 settings)
    {
        TB_Name.Text = settings.Name;
        TB_Title.Text = settings.PlayerTitle;
        NUD_Experience.Value = Math.Clamp(settings.Experience, 0, (uint)NUD_Experience.Maximum);
        NUD_Rank.Value = Math.Clamp(settings.Rank, (ushort)0, (ushort)NUD_Rank.Maximum);
        CB_CeilingColor.SelectedValue = (int)settings.CeilingColor;
        NUD_Flags.Value = Math.Clamp(settings.Flags, 0, (uint)NUD_Flags.Maximum);
        NUD_PlayerCount.Value = Math.Clamp(settings.VisitingPlayerDatabaseCount, (ushort)0, (ushort)NUD_PlayerCount.Maximum);
        NUD_PlayerInsert.Value = Math.Clamp(settings.VistiingPlayerDatabaseInsertIndex, (ushort)0, (ushort)NUD_PlayerInsert.Maximum);
        NUD_Seed.Value = Math.Clamp(settings.Seed, 0, (uint)NUD_Seed.Maximum);
        NUD_PromotionDaysElapsed.Value = Math.Clamp(settings.PromotionDaysElapsed, (ushort)0, (ushort)NUD_PromotionDaysElapsed.Maximum);
        CHK_IsPromotionActive.Checked = settings.IsPromotionActive;

        for (int i = 0; i < JoinAvenueSettings5.CountVisitingPlayersRemembered; i++)
        {
            var value = settings.GetVisitingPlayerTrainerID(i);
            var row = DGV_VisitingPlayerDatabase.Rows[i];
            row.Cells[VisitingPlayerColumnTID].Value = ((ushort)value).ToString("00000");
            row.Cells[VisitingPlayerColumnSID].Value = ((ushort)(value >> 16)).ToString("00000");
        }
    }

    public void SaveObject(JoinAvenueSettings5 settings)
    {
        settings.Name = TB_Name.Text;
        settings.PlayerTitle = TB_Title.Text;
        settings.Experience = (uint)NUD_Experience.Value;
        settings.Rank = (ushort)NUD_Rank.Value;
        settings.CeilingColor = (JoinAvenueCeilingColor5)WinFormsUtil.GetIndex(CB_CeilingColor);
        settings.Flags = (uint)NUD_Flags.Value;
        settings.VisitingPlayerDatabaseCount = (ushort)NUD_PlayerCount.Value;
        settings.VistiingPlayerDatabaseInsertIndex = (ushort)NUD_PlayerInsert.Value;
        settings.Seed = (uint)NUD_Seed.Value;
        settings.PromotionDaysElapsed = (ushort)NUD_PromotionDaysElapsed.Value;
        settings.IsPromotionActive = CHK_IsPromotionActive.Checked;

        for (int i = 0; i < JoinAvenueSettings5.CountVisitingPlayersRemembered; i++)
        {
            var row = DGV_VisitingPlayerDatabase.Rows[i];
            var tid = ParseUInt16(row.Cells[VisitingPlayerColumnTID].Value);
            var sid = ParseUInt16(row.Cells[VisitingPlayerColumnSID].Value);
            var value = tid | ((uint)sid << 16);
            settings.SetVisitingPlayerTrainerID(i, value);
        }
    }

    private static ushort ParseUInt16(object? value)
    {
        if (value is null)
            return 0;
        return ushort.TryParse(value.ToString(), out var result) ? result : (ushort)0;
    }
}
