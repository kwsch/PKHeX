using System;

namespace PKHeX.Core;

public sealed class FashionBlock7(SAV7 sav, Memory<byte> raw) : SaveBlock<SAV7>(sav, raw)
{
    private const int FashionLength = 0x1A08;

    public FashionItem7[] Wardrobe
    {
        get
        {
            var data = Data[..0x5A8].ToArray();
            return Array.ConvertAll(data, b => new FashionItem7(b));
        }
        set
        {
            if (value.Length != 0x5A8)
                throw new ArgumentOutOfRangeException($"Unexpected size: 0x{value.Length:X}");
            var arr = Array.ConvertAll(value, z => z.Value);
            SAV.SetData(Data[..0x5A8], arr);
        }
    }

    public void Clear() => Data[..FashionLength].Clear();

    /// <summary>
    /// Resets the fashion unlocks to default values.
    /// </summary>
    public void Reset()
    {
        var offsetList = GetDefaultFashionOffsets(SAV);
        foreach (var ofs in offsetList)
            Data[ofs] = 3; // owned | new
    }

    private static ReadOnlySpan<ushort> GetDefaultFashionOffsets(SAV7 sav) => sav switch
    {
        SAV7SM   { Gender: 0 } => DefaultFashionOffsetSM_M,
        SAV7SM   { Gender: 1 } => DefaultFashionOffsetSM_F,
        SAV7USUM { Gender: 0 } => DefaultFashionOffsetUU_M,
        SAV7USUM { Gender: 1 } => DefaultFashionOffsetUU_F,
        _ => throw new ArgumentOutOfRangeException(nameof(sav)),
    };

    // Offsets that are set to '3' when the game starts for a specific gender.
    private static ReadOnlySpan<ushort> DefaultFashionOffsetSM_M => [ 0x000, 0x0FB, 0x124, 0x28F, 0x3B4, 0x452, 0x517 ];
    private static ReadOnlySpan<ushort> DefaultFashionOffsetSM_F => [ 0x000, 0x100, 0x223, 0x288, 0x3B4, 0x452, 0x517 ];
    private static ReadOnlySpan<ushort> DefaultFashionOffsetUU_M => [ 0x03A, 0x109, 0x1DA, 0x305, 0x3D9, 0x4B1, 0x584 ];
    private static ReadOnlySpan<ushort> DefaultFashionOffsetUU_F => [ 0x05E, 0x208, 0x264, 0x395, 0x3B4, 0x4F9, 0x5A8 ];

    public void ImportPayload(ReadOnlySpan<byte> data) => SAV.SetData(Data[..FashionLength], data);

    public void GiveAgentSunglasses() => Data[0xD0] = 3;
}

// Every fashion item is 2 bits, New Flag (high) & Owned Flag (low)
public sealed class FashionItem7(byte b)
{
    public bool IsOwned { get; set; } = (b & 1) != 0;
    public bool IsNew { get; set; } = (b & 2) != 0;

    public byte Value => (byte)((IsOwned ? 1 : 0) | (IsNew ? 2 : 0));
}
