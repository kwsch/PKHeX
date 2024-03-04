using System;

namespace PKHeX.Core;

public sealed class BoxLayout8(SAV8SWSH sav, SCBlock block) : SaveBlock<SAV8SWSH>(sav, block.Data), IBoxDetailName
{
    public const int BoxCount = 32;

    private const int StringMaxLength = SAV6.LongStringLength / 2;

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
        get => SAV.GetValue<byte>(SaveBlockAccessor8SWSH.KCurrentBox);
        set => SAV.SetValue(SaveBlockAccessor8SWSH.KCurrentBox, (byte)value);
    }
}
