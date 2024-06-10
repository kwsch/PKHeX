using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 6 savedata object that stores the player's trainer data.
/// </summary>
public class MyStatus6(SAV6 sav, Memory<byte> raw) : SaveBlock<SAV6>(sav, raw), IRegionOrigin, IGameSync
{
    public uint ID32
    {
        get => ReadUInt32LittleEndian(Data);
        set => WriteUInt32LittleEndian(Data, value);
    }

    public ushort TID16
    {
        get => ReadUInt16LittleEndian(Data);
        set => WriteUInt16LittleEndian(Data, value);
    }

    public ushort SID16
    {
        get => ReadUInt16LittleEndian(Data[2..]);
        set => WriteUInt16LittleEndian(Data[2..], value);
    }

    public byte Game
    {
        get => Data[4];
        set => Data[4] = value;
    }

    public byte Gender
    {
        get => Data[5];
        set => Data[5] = value;
    }

    public int MultiplayerSpriteID_1
    {
        get => Data[6];
        set => Data[6] = (byte)value;
    }

    public int MultiplayerSpriteID_2
    {
        get => Data[7];
        set => Data[7] = (byte)value;
    }

    public const int GameSyncIDSize = 16; // 64 bits
    int IGameSync.GameSyncIDSize => GameSyncIDSize;

    public string GameSyncID
    {
        get => Util.GetHexStringFromBytes(Data.Slice(0x08, GameSyncIDSize / 2));
        set
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(value.Length, GameSyncIDSize);
            Span<byte> dest = Data.Slice(8, GameSyncIDSize / 2);
            dest.Clear();
            Util.GetBytesFromHexString(value, dest);
        }
    }

    public byte Region
    {
        get => Data[0x26];
        set => Data[0x26] = value;
    }

    public byte Country
    {
        get => Data[0x27];
        set => Data[0x27] = value;
    }

    public decimal Latitude // don't use the setters
    {
        get => (ReadInt16LittleEndian(Data[0x28..]) * 180m) / 0x8000;
        set => WriteInt16LittleEndian(Data[0x28..], (short)((value * 0x8000) / 180m));
    }

    public decimal Longitude // don't use the setters
    {
        get => (ReadInt16LittleEndian(Data[0x2A..]) * 180m) / 0x8000;
        set => WriteInt16LittleEndian(Data[0x2A..], (short)((value * 0x8000) / 180m));
    }

    public byte ConsoleRegion
    {
        get => Data[0x2C];
        set => Data[0x2C] = value;
    }

    public int Language
    {
        get => Data[0x2D];
        set => Data[0x2D] = (byte)value;
    }

    private Span<byte> OriginalTrainerTrash => Data.Slice(0x48, 0x1A);

    public string OT
    {
        get => SAV.GetString(OriginalTrainerTrash);
        set => SAV.SetString(OriginalTrainerTrash, value, SAV.MaxStringLengthTrainer, StringConverterOption.ClearZero);
    }

    private Span<byte> GetSayingSpan(int say) => Data.Slice(GetSayingOffset(say), SAV6.LongStringLength);
    private static int GetSayingOffset(int say) => 0x7C + (SAV6.LongStringLength * say);
    private string GetSaying(int say) => SAV.GetString(GetSayingSpan(say));
    private void SetSaying(int say, ReadOnlySpan<char> value) => SAV.SetString(GetSayingSpan(say), value, SAV6.LongStringLength / 2, StringConverterOption.ClearZero);

    public string Saying1 { get => GetSaying(0); set => SetSaying(0, value); }
    public string Saying2 { get => GetSaying(1); set => SetSaying(1, value); }
    public string Saying3 { get => GetSaying(2); set => SetSaying(2, value); }
    public string Saying4 { get => GetSaying(3); set => SetSaying(3, value); }
    public string Saying5 { get => GetSaying(4); set => SetSaying(4, value); }

    public bool IsMegaEvolutionUnlocked
    {
        get => (Data[0x14A] & 0x01) != 0;
        set => Data[0x14A] = (byte)((Data[0x14A] & 0xFE) | (value ? 1 : 0)); // in battle
    }

    public bool IsMegaRayquazaUnlocked
    {
        get => (Data[0x14A] & 0x02) != 0;
        set => Data[0x14A] = (byte)((Data[0x14A] & ~2) | (value ? 2 : 0)); // in battle
    }
}
