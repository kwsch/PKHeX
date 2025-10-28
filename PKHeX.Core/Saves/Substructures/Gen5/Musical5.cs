using System;

namespace PKHeX.Core;

public sealed class Musical5(SAV5 SAV, Memory<byte> raw) : SaveBlock<SAV5>(SAV, raw)
{
    public const int MusicalNameMaxLength = 20;

    private Span<byte> MusicalTrash => Data.Slice(0x208, MusicalNameMaxLength * 2);

    public string MusicalName
    {
        get => StringConverter5.GetString(MusicalTrash);
        set => StringConverter5.SetString(MusicalTrash, value, MusicalNameMaxLength, 0, StringConverterOption.ClearZero);
    }

    private const int PropOffset = 0x258;

    public bool IsAvailableMusicalDLC
    {
        get => Data[0x29E] == 1;
        set => Data[0x29E] = value ? (byte)1 : (byte)0;
    }

    public void UnlockAllMusicalProps()
    {
        // 100 props, which is 12.5 bytes of bitflags.
        for (int i = 0; i < 0xC; i++)
            Data[PropOffset + i] = 0xFF;
        Data[PropOffset + 0xC] = 0x0F; // top 4 bits unset, to complete multiple of 8 (100=>104 bits).
    }

    public bool GetHasProp(int prop)
    {
        var bitOffset = prop >> 3;
        return FlagUtil.GetFlag(Data, PropOffset + bitOffset, prop & 7);
    }

    public void SetHasProp(int prop, bool value = true)
    {
        var bitOffset = prop >> 3;
        FlagUtil.SetFlag(Data, PropOffset + bitOffset, prop & 7, value);
    }
}
