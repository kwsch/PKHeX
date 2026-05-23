using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public sealed partial class JoinAvenueVisitorSpecificEditor : UserControl, IJoinAvenueSpecificEditor<JoinAvenueVisitor5>
{
    private static readonly List<ComboItem> ShopTypeList = Enum.GetValues<JoinAvenueShopType5>().Select(z => new ComboItem(z.ToString(), (int)z)).ToList();
    private static readonly List<ComboItem> OriginList = [new("NPC", 0), new("Human Player", 1)];

    public JoinAvenueVisitorSpecificEditor()
    {
        InitializeComponent();
        InitializeCombo(CB_DesiredShopType, ShopTypeList);
        InitializeCombo(CB_FavoriteSpecies, GameInfo.FilteredSources.Species.ToList());
        InitializeCombo(CB_Origin, OriginList);
        InitializeCombo(CB_ShopType, ShopTypeList);
    }

    public void LoadObject(JoinAvenueVisitor5 entity)
    {
        CHK_IsFlag2C.Checked = entity.IsFlag2C;
        NUD_AvenueLevel.Value = Math.Clamp(entity.JoinAvenueLevel, (byte)0, (byte)NUD_AvenueLevel.Maximum);
        NUD_Unused2D.Value = Math.Clamp(entity.Unused2D, (byte)0, (byte)NUD_Unused2D.Maximum);
        SetComboValue(CB_DesiredShopType, entity.DesiredShopType);

        byte[] counts =
        [
            entity.ShopCountRaffle, entity.ShopCountSalon, entity.ShopCountMarket, entity.ShopCountFlorist,
            entity.ShopCountDojo, entity.ShopCountNurse, entity.ShopCountAntique, entity.ShopCountCafe,
        ];
        TB_ShopCounts.Text = string.Join(", ", counts);
        NUD_DexSeen.Value = Math.Clamp(entity.DexSeen, (ushort)0, (ushort)NUD_DexSeen.Maximum);
        SetComboValue(CB_FavoriteSpecies, entity.FavoriteSpecies);
        NUD_MedalRank.Value = Math.Clamp(entity.MedalRank, (byte)0, (byte)NUD_MedalRank.Maximum);
        NUD_MedalHint.Value = Math.Clamp(entity.MedalHint, (byte)0, (byte)NUD_MedalHint.Maximum);
        NUD_MedalCount.Value = Math.Clamp(entity.MedalCount, (byte)0, (byte)NUD_MedalCount.Maximum);
        TB_Date1.Text = FormatDate(entity.Date1.RawValue);
        TB_DateStart.Text = FormatDate(entity.DateAdventureStart.RawValue);
        TB_DateHall.Text = FormatDate(entity.DateHallOfFame.RawValue);

        var records = new uint[(int)JoinAvenueRecordIndex5.COUNT_MAX];
        for (int i = 0; i < records.Length; i++)
            records[i] = entity.GetRecord((JoinAvenueRecordIndex5)i);
        TB_Records.Text = string.Join(", ", records);

        var trivia = new byte[JoinAvenueVisitor5.TriviaCount];
        for (int i = 0; i < trivia.Length; i++)
            trivia[i] = entity.GetTrivia(i);
        TB_Trivia.Text = string.Join(", ", trivia);

        var activities = new byte[JoinAvenueVisitor5.ActivityCount];
        var dates = new string[JoinAvenueVisitor5.ActivityCount];
        for (int i = 0; i < activities.Length; i++)
        {
            activities[i] = entity.GetActivity(i);
            dates[i] = FormatDate(entity.GetActivityDate(i).RawValue);
        }

        TB_Activities.Text = string.Join(", ", activities);
        TB_ActivityDates.Text = string.Join(", ", dates);
        SetComboValue(CB_Origin, entity.Origin);
        NUD_MetHour.Value = Math.Clamp(entity.MetHour, (byte)0, (byte)NUD_MetHour.Maximum);
        NUD_MetMinute.Value = Math.Clamp(entity.MetMinute, (byte)0, (byte)NUD_MetMinute.Maximum);
        NUD_UnknownA8.Value = Math.Clamp(entity.UnknownA8, (byte)0, (byte)NUD_UnknownA8.Maximum);
        CHK_IsShopChangeAllowed.Checked = entity.IsShopChangeAllowed;
        CHK_IsFlagA9_1.Checked = entity.IsFlagA9_1;
        CHK_IsFlagA9_2.Checked = entity.IsFlagA9_2;
        CHK_InteractedToday.Checked = entity.IsInteractedToday;
        CHK_IsFlagAA.Checked = entity.IsFlagAA;
        NUD_JoinAvenueRank.Value = Math.Clamp(entity.JoinAvenueRank, (byte)0, (byte)NUD_JoinAvenueRank.Maximum);
        NUD_UnknownAC.Value = Math.Clamp(entity.UnknownAC, (byte)0, (byte)NUD_UnknownAC.Maximum);
        NUD_ShopLevel.Value = Math.Clamp(entity.ShopLevel, (byte)0, (byte)NUD_ShopLevel.Maximum);
        NUD_ShopExperience.Value = Math.Clamp(entity.ShopExperience, (ushort)0, (ushort)NUD_ShopExperience.Maximum);
        NUD_IsInventory.Value = Math.Clamp(entity.IsInventory, 0, (uint)NUD_IsInventory.Maximum);
        SetComboValue(CB_ShopType, (int)entity.ShopType);
        NUD_ShopWork.Value = Math.Clamp(entity.ShopWork, (ushort)0, (ushort)NUD_ShopWork.Maximum);
        NUD_UnusedB8.Value = Math.Clamp(entity.UnusedB8, 0, (uint)NUD_UnusedB8.Maximum);
        NUD_UnknownBits0_8.Value = Math.Clamp(entity.UnknownBits0_8, (ushort)0, (ushort)NUD_UnknownBits0_8.Maximum);
        CHK_UnknownBit9.Checked = entity.IsUnknownBits9;
        NUD_UnknownBits10.Value = Math.Clamp(entity.UnknownBits10, (byte)0, (byte)NUD_UnknownBits10.Maximum);
        NUD_UnknownBits13_20.Value = Math.Clamp(entity.UnknownBits13_20, (byte)0, (byte)NUD_UnknownBits13_20.Maximum);
        NUD_UnknownBits21_27.Value = Math.Clamp(entity.UnknownBits21_27, (byte)0, (byte)NUD_UnknownBits21_27.Maximum);
        NUD_UnknownBits28_31.Value = Math.Clamp(entity.UnknownBits28_31, (byte)0, (byte)NUD_UnknownBits28_31.Maximum);
    }

    public void SaveObject(JoinAvenueVisitor5 entity)
    {
        entity.IsFlag2C = CHK_IsFlag2C.Checked;
        entity.JoinAvenueLevel = (byte)NUD_AvenueLevel.Value;
        entity.Unused2D = (byte)NUD_Unused2D.Value;
        entity.DesiredShopType = (ushort)WinFormsUtil.GetIndex(CB_DesiredShopType);

        var shopCounts = ParseByteList(TB_ShopCounts.Text, 8, 0x0F);
        entity.ShopCountRaffle = shopCounts[0];
        entity.ShopCountSalon = shopCounts[1];
        entity.ShopCountMarket = shopCounts[2];
        entity.ShopCountFlorist = shopCounts[3];
        entity.ShopCountDojo = shopCounts[4];
        entity.ShopCountNurse = shopCounts[5];
        entity.ShopCountAntique = shopCounts[6];
        entity.ShopCountCafe = shopCounts[7];

        entity.DexSeen = (ushort)NUD_DexSeen.Value;
        entity.FavoriteSpecies = (ushort)WinFormsUtil.GetIndex(CB_FavoriteSpecies);
        entity.MedalRank = (byte)NUD_MedalRank.Value;
        entity.MedalHint = (byte)NUD_MedalHint.Value;
        entity.MedalCount = (byte)NUD_MedalCount.Value;
        entity.Date1 = new JoinAvenueDate5(ParseDate(TB_Date1.Text));
        entity.DateAdventureStart = new JoinAvenueDate5(ParseDate(TB_DateStart.Text));
        entity.DateHallOfFame = new JoinAvenueDate5(ParseDate(TB_DateHall.Text));

        var records = ParseUIntList(TB_Records.Text, (int)JoinAvenueRecordIndex5.COUNT_MAX);
        for (int i = 0; i < records.Length; i++)
            entity.SetRecord((JoinAvenueRecordIndex5)i, records[i]);

        var trivia = ParseByteList(TB_Trivia.Text, JoinAvenueVisitor5.TriviaCount, byte.MaxValue);
        for (int i = 0; i < trivia.Length; i++)
            entity.SetTrivia(i, trivia[i]);

        var activities = ParseByteList(TB_Activities.Text, JoinAvenueVisitor5.ActivityCount, byte.MaxValue);
        var activityDates = ParseDateList(TB_ActivityDates.Text, JoinAvenueVisitor5.ActivityCount);
        for (int i = 0; i < activities.Length; i++)
        {
            entity.SetActivity(i, activities[i]);
            entity.SetActivityDate(i, new JoinAvenueDate5(activityDates[i]));
        }

        entity.Origin = (ushort)WinFormsUtil.GetIndex(CB_Origin);
        entity.MetHour = (byte)NUD_MetHour.Value;
        entity.MetMinute = (byte)NUD_MetMinute.Value;
        entity.UnknownA8 = (byte)NUD_UnknownA8.Value;
        entity.IsShopChangeAllowed = CHK_IsShopChangeAllowed.Checked;
        entity.IsFlagA9_1 = CHK_IsFlagA9_1.Checked;
        entity.IsFlagA9_2 = CHK_IsFlagA9_2.Checked;
        entity.IsInteractedToday = CHK_InteractedToday.Checked;
        entity.IsFlagAA = CHK_IsFlagAA.Checked;
        entity.JoinAvenueRank = (byte)NUD_JoinAvenueRank.Value;
        entity.UnknownAC = (byte)NUD_UnknownAC.Value;
        entity.ShopLevel = (byte)NUD_ShopLevel.Value;
        entity.ShopExperience = (ushort)NUD_ShopExperience.Value;
        entity.IsInventory = (uint)NUD_IsInventory.Value;
        entity.ShopType = (JoinAvenueShopType5)WinFormsUtil.GetIndex(CB_ShopType);
        entity.ShopWork = (ushort)NUD_ShopWork.Value;
        entity.UnusedB8 = (uint)NUD_UnusedB8.Value;
        entity.UnknownBits0_8 = (ushort)NUD_UnknownBits0_8.Value;
        entity.IsUnknownBits9 = CHK_UnknownBit9.Checked;
        entity.UnknownBits10 = (byte)NUD_UnknownBits10.Value;
        entity.UnknownBits13_20 = (byte)NUD_UnknownBits13_20.Value;
        entity.UnknownBits21_27 = (byte)NUD_UnknownBits21_27.Value;
        entity.UnknownBits28_31 = (byte)NUD_UnknownBits28_31.Value;
    }

    private static void InitializeCombo(ComboBox cb, IReadOnlyList<ComboItem> source)
    {
        cb.InitializeBinding();
        cb.DataSource = new BindingSource(source, string.Empty);
    }

    private static void SetComboValue(ComboBox cb, int value) => cb.SelectedValue = value;

    private static string FormatDate(ushort raw)
    {
        if (raw == 0)
            return string.Empty;

        var date = new JoinAvenueDate5(raw);
        return date.Date is { } value ? value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : $"0x{raw:X4}";
    }

    private static ushort ParseDate(string text)
    {
        text = text.Trim();
        if (string.IsNullOrWhiteSpace(text))
            return 0;
        if (TryParseUInt(text, out var raw))
            return (ushort)Math.Min(raw, ushort.MaxValue);
        if (DateOnly.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            JoinAvenueDate5 value = default;
            value.Date = date;
            return value.RawValue;
        }
        return 0;
    }

    private static byte[] ParseByteList(string text, int count, byte max)
    {
        var result = new byte[count];
        var split = Split(text);
        for (int i = 0; i < count && i < split.Length; i++)
        {
            if (TryParseUInt(split[i], out var value))
                result[i] = (byte)Math.Min(value, max);
        }
        return result;
    }

    private static uint[] ParseUIntList(string text, int count)
    {
        var result = new uint[count];
        var split = Split(text);
        for (int i = 0; i < count && i < split.Length; i++)
        {
            if (TryParseUInt(split[i], out var value))
                result[i] = value;
        }
        return result;
    }

    private static ushort[] ParseDateList(string text, int count)
    {
        var result = new ushort[count];
        var split = Split(text);
        for (int i = 0; i < count && i < split.Length; i++)
            result[i] = ParseDate(split[i]);
        return result;
    }

    private static string[] Split(string text) => text.Split([',', ';', '|', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private static bool TryParseUInt(string text, out uint value)
    {
        text = text.Trim();
        if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            return uint.TryParse(text[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
        return uint.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
    }
}
