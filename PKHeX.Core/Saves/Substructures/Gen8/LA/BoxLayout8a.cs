using System;
using System.ComponentModel;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about Box Names and which box is the first box to show when the UI is opened.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class BoxLayout8a(SAV8LA sav, SCBlock block) : SaveBlock<SAV8LA>(sav, block.Data), IBoxDetailName
{
    public const int BoxCount = 32;

    private const int StringMaxLength = SAV6.LongStringLength / 2; // 0x22 bytes

    private static int GetBoxNameOffset(int box) => SAV6.LongStringLength * box;
    private Span<byte> GetBoxNameSpan(int box) => Data.Slice(GetBoxNameOffset(box), SAV6.LongStringLength);
    public string GetBoxName(int box) => SAV.GetString(GetBoxNameSpan(box));
    public void SetBoxName(int box, ReadOnlySpan<char> value) => SAV.SetString(GetBoxNameSpan(box), value, StringMaxLength, StringConverterOption.ClearZero);

    public string this[int i]
    {
        get => GetBoxName(i);
        set => SetBoxName(i, value);
    }

    public int CurrentBox
    {
        get => SAV.GetValue<byte>(SaveBlockAccessor8LA.KCurrentBox);
        set => SAV.SetValue(SaveBlockAccessor8LA.KCurrentBox, (byte)value);
    }
}
