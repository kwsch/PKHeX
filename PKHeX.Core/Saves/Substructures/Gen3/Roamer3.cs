using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Roamer3 : IContestStats
{
    public const int SIZE = 0x14;
    public bool IsGlitched { get; }
    private readonly Memory<byte> Raw;
    private Span<byte> Data => Raw.Span;

    public Roamer3(SAV3 sav)
    {
        var buffer = sav.Large;
        var offset = sav switch
        {
            SAV3RS => 0x3144,
            SAV3E => 0x31DC,
            _ => 0x30D0, // FRLG
        };
        Raw = buffer.AsMemory(offset, SIZE);
        IsGlitched = sav is not SAV3E;
    }

    public uint IV32
    {
        get => ReadUInt32LittleEndian(Data);
        set => WriteUInt32LittleEndian(Data, value);
    }

    public uint PID
    {
        get => ReadUInt32LittleEndian(Data[4..]);
        set => WriteUInt32LittleEndian(Data[4..], value);
    }

    public ushort Species
    {
        get => SpeciesConverter.GetNational3(ReadUInt16LittleEndian(Data[8..]));
        set => WriteUInt16LittleEndian(Data[8..], SpeciesConverter.GetInternal3(value));
    }

    public int HP_Current
    {
        get => ReadInt16LittleEndian(Data[10..]);
        set => WriteInt16LittleEndian(Data[10..], (short)value);
    }

    public byte CurrentLevel
    {
        get => Data[12];
        set => Data[12] = value;
    }

    public int Status { get => Data[0x0D]; set => Data[0x0D] = (byte)value; }

    public byte ContestCool   { get => Data[0x0E]; set => Data[0x0E] = value; }
    public byte ContestBeauty { get => Data[0x0F]; set => Data[0x0F] = value; }
    public byte ContestCute   { get => Data[0x10]; set => Data[0x10] = value; }
    public byte ContestSmart  { get => Data[0x11]; set => Data[0x11] = value; }
    public byte ContestTough  { get => Data[0x12]; set => Data[0x12] = value; }
    public byte ContestSheen  { get => 0; set { } }
    public bool Active    { get => Data[0x13] == 1; set => Data[0x13] = value ? (byte)1 : (byte)0; }

    // Derived Properties
    private int IV_HP  { get => (int)(IV32 >> 00) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 00)) | (uint)((value > 31 ? 31 : value) << 00); }
    private int IV_ATK { get => (int)(IV32 >> 05) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 05)) | (uint)((value > 31 ? 31 : value) << 05); }
    private int IV_DEF { get => (int)(IV32 >> 10) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 10)) | (uint)((value > 31 ? 31 : value) << 10); }
    private int IV_SPE { get => (int)(IV32 >> 15) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 15)) | (uint)((value > 31 ? 31 : value) << 15); }
    private int IV_SPA { get => (int)(IV32 >> 20) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 20)) | (uint)((value > 31 ? 31 : value) << 20); }
    private int IV_SPD { get => (int)(IV32 >> 25) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 25)) | (uint)((value > 31 ? 31 : value) << 25); }

    /// <summary>
    /// Roamer's IVs.
    /// </summary>
    public int[] IVs
    {
        get => [IV_HP, IV_ATK, IV_DEF, IV_SPE, IV_SPA, IV_SPD];
        set => SetIVs(value);
    }

    public void SetIVs(ReadOnlySpan<int> value)
    {
        if (value.Length != 6)
            return;
        IV_HP = value[0];
        IV_ATK = value[1];
        IV_DEF = value[2];
        IV_SPE = value[3];
        IV_SPA = value[4];
        IV_SPD = value[5];
    }

    /// <summary>
    /// Indicates if the Roamer is shiny with the <see cref="tr"/>'s Trainer Details.
    /// </summary>
    /// <param name="pid">PID to check for</param>
    /// <param name="tr">Trainer to check for</param>
    /// <returns>Indication if the PID is shiny for the trainer.</returns>
    public static bool IsShiny(uint pid, ITrainerID32 tr)
    {
        var tmp = tr.ID32 ^ pid;
        var xor = (tmp >> 16) ^ (tmp & 0xFFFF);
        return xor < 8;
    }

    /// <summary>
    /// Gets the glitched Roamer IVs, where only 1 byte of IV data is loaded when encountered.
    /// </summary>
    public int[] IVsGlitch
    {
        get
        {
            var ivs = IV32; // store for restoration later
            IV32 &= 0xFF; // only 1 byte is loaded to the encounter
            var glitch = IVs; // get glitched IVs
            IV32 = ivs; // restore unglitched IVs
            return glitch;
        }
    }
}
