using System;

namespace PKHeX.Core;

public sealed class BoxLayout9 : SaveBlock<SAV9SV>, IBoxDetailName
{
    public const int BoxCount = 32;

    private const int StringMaxLength = SAV6.LongStringLength / 2;

    public BoxLayout9(SAV9SV sav, SCBlock block) : base(sav, block.Data) { }

    private static int GetBoxNameOffset(int box) => SAV6.LongStringLength * box;
    private Span<byte> GetBoxNameSpan(int box) => Data.AsSpan(GetBoxNameOffset(box), SAV6.LongStringLength);

    public string GetBoxName(int box)
    {
        var span = GetBoxNameSpan(box);
        if (System.Buffers.Binary.BinaryPrimitives.ReadUInt16LittleEndian(span) == 0)
            return $"Box {box + 1}";
        return SAV.GetString(span);
    }

    public void SetBoxName(int box, ReadOnlySpan<char> value)
    {
        var span = GetBoxNameSpan(box);
        SAV.SetString(span, value, StringMaxLength, StringConverterOption.ClearZero);
    }

    public string this[int i]
    {
        get => GetBoxName(i);
        set => SetBoxName(i, value);
    }

    public int CurrentBox
    {
        get => SAV.GetValue<byte>(SaveBlockAccessor9SV.KCurrentBox);
        set => SAV.SetValue(SaveBlockAccessor9SV.KCurrentBox, (byte)value);
    }
}
