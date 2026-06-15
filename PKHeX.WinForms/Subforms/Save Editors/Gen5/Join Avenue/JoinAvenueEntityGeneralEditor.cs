using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public sealed partial class JoinAvenueEntityGeneralEditor : UserControl
{
    private static readonly IReadOnlyList<ComboItem> VersionList = GameInfo.FilteredSources.Games.ToList();
    private static readonly IReadOnlyList<ComboItem> LanguageList = GameInfo.LanguageDataSource(5, EntityContext.Gen5);
    private static readonly IReadOnlyList<ComboItem> GenderList = Main.GenderSymbols.Select((z, i) => new ComboItem(z, i)).ToList();

    public JoinAvenueEntityGeneralEditor()
    {
        InitializeComponent();
        InitializeCombo(CB_Version, VersionList);
        InitializeCombo(CB_Language, LanguageList);
    }

    public void LoadObject(IJoinAvenueEntity5 entity)
    {
        TB_Name.Text = entity.Name;
        NUD_Country.Value = Math.Clamp(entity.Country, (byte)0, (byte)NUD_Country.Maximum);
        NUD_Subregion.Value = Math.Clamp(entity.Subregion, (byte)0, (byte)NUD_Subregion.Maximum);
        TB_Shout.Text = entity.Shout;
        SetComboValue(CB_Version, entity.Version);
        SetComboValue(CB_Language, entity.Language);
        NUD_Unknown22.Value = Math.Clamp(entity.Unknown22, (byte)0, (byte)NUD_Unknown22.Maximum);
        UC_Gender.Gender = entity.Gender;
        NUD_Unused23.Value = Math.Clamp(entity.Unused23, (byte)0, (byte)NUD_Unused23.Maximum);
        NUD_TID16.Value = Math.Clamp(entity.TID16, (ushort)0, (ushort)NUD_TID16.Maximum);
        NUD_Unknown26.Value = Math.Clamp(entity.Unknown26, (byte)0, (byte)NUD_Unknown26.Maximum);
        NUD_Unknown27.Value = Math.Clamp(entity.Unknown27, (byte)0, (byte)NUD_Unknown27.Maximum);
        NUD_PlayedHours.Value = Math.Clamp(entity.PlayedHours, (ushort)0, (ushort)NUD_PlayedHours.Maximum);
        NUD_PlayedMinutes.Value = Math.Clamp(entity.PlayedMinutes, (byte)0, (byte)NUD_PlayedMinutes.Maximum);
        NUD_Sprite.Value = Math.Clamp(entity.Sprite, (ushort)0, (ushort)NUD_Sprite.Maximum);
        TB_Greeting.Text = entity.Greeting;
        TB_Farewell.Text = entity.Farewell;
        NUD_MetYear.Value = Math.Clamp(entity.MetYear, (byte)0, (byte)NUD_MetYear.Maximum);
        NUD_MetMonth.Value = Math.Clamp(entity.MetMonth, (byte)0, (byte)NUD_MetMonth.Maximum);
        NUD_MetDay.Value = Math.Clamp(entity.MetDay, (byte)0, (byte)NUD_MetDay.Maximum);
        NUD_Seed.Value = Math.Clamp(entity.Seed, 0, (uint)NUD_Seed.Maximum);
    }

    public void SaveObject(IJoinAvenueEntity5 entity)
    {
        entity.Name = TB_Name.Text;
        entity.Country = (byte)NUD_Country.Value;
        entity.Subregion = (byte)NUD_Subregion.Value;
        entity.Shout = TB_Shout.Text;
        entity.Version = (byte)WinFormsUtil.GetIndex(CB_Version);
        entity.Language = (byte)WinFormsUtil.GetIndex(CB_Language);
        entity.Unknown22 = (byte)NUD_Unknown22.Value;
        entity.Gender = UC_Gender.Gender;
        entity.Unused23 = (byte)NUD_Unused23.Value;
        entity.TID16 = (ushort)NUD_TID16.Value;
        entity.Unknown26 = (byte)NUD_Unknown26.Value;
        entity.Unknown27 = (byte)NUD_Unknown27.Value;
        entity.PlayedHours = (ushort)NUD_PlayedHours.Value;
        entity.PlayedMinutes = (byte)NUD_PlayedMinutes.Value;
        entity.Sprite = (ushort)NUD_Sprite.Value;
        entity.Greeting = TB_Greeting.Text;
        entity.Farewell = TB_Farewell.Text;
        entity.MetYear = (byte)NUD_MetYear.Value;
        entity.MetMonth = (byte)NUD_MetMonth.Value;
        entity.MetDay = (byte)NUD_MetDay.Value;
        entity.Seed = (uint)NUD_Seed.Value;
    }

    private static void InitializeCombo(ComboBox cb, IReadOnlyList<ComboItem> source)
    {
        cb.InitializeBinding();
        cb.DataSource = new BindingSource(source, string.Empty);
    }

    private static void SetComboValue(ComboBox cb, int value) => cb.SelectedValue = value;
}
