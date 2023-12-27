using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class MyStatus7 : SaveBlock<SAV7>, IRegionOrigin
{
    public const int GameSyncIDSize = 16; // 64 bits
    public const int NexUniqueIDSize = 32; // 128 bits

    public MyStatus7(SAV7 sav, int offset) : base(sav) => Offset = offset;

    public uint ID32
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0));
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0), value);
    }

    public ushort TID16
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0));
        set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0), value);
    }

    public ushort SID16
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 2));
        set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 2), value);
    }

    public int Game
    {
        get => Data[Offset + 4];
        set => Data[Offset + 4] = (byte)value;
    }

    public int Gender
    {
        get => Data[Offset + 5];
        set => Data[Offset + 5] = (byte)value;
    }

    public string GameSyncID
    {
        get => Util.GetHexStringFromBytes(Data.AsSpan(Offset + 0x10, GameSyncIDSize / 2));
        set
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(value.Length, GameSyncIDSize);

            var data = Util.GetBytesFromHexString(value);
            SAV.SetData(data, Offset + 0x10);
        }
    }

    public string NexUniqueID
    {
        get => Util.GetHexStringFromBytes(Data.AsSpan(Offset + 0x18, NexUniqueIDSize / 2));
        set
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(value.Length, NexUniqueIDSize);

            var data = Util.GetBytesFromHexString(value);
            SAV.SetData(data, Offset + 0x18);
        }
    }

    public uint FestaID
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x28));
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x28), value);
    }

    public byte Region
    {
        get => Data[Offset + 0x2E];
        set => Data[Offset + 0x2E] = value;
    }

    public byte Country
    {
        get => Data[Offset + 0x2F];
        set => Data[Offset + 0x2F] = value;
    }

    public byte ConsoleRegion
    {
        get => Data[Offset + 0x34];
        set => Data[Offset + 0x34] = value;
    }

    public int Language
    {
        get => Data[Offset + 0x35];
        set => Data[Offset + 0x35] = (byte)value;
    }

    private Span<byte> OT_Trash => Data.AsSpan(Offset + 0x38, 0x1A);

    public string OT
    {
        get => SAV.GetString(OT_Trash);
        set => SAV.SetString(OT_Trash, value, SAV.MaxStringLengthOT, StringConverterOption.ClearZero);
    }

    public int DressUpSkinColor
    {
        get => (Data[Offset + 0x54] >> 2) & 7;
        set => Data[Offset + 0x54] = (byte)((Data[Offset + 0x54] & ~(7 << 2)) | (value << 2));
    }

    public int MultiplayerSpriteID // holdover from Gen6
    {
        get => Data[Offset + 0x58];
        set => Data[Offset + 0x58] = (byte)value;
    }

    public bool MegaUnlocked
    {
        get => (Data[Offset + 0x78] & 0x01) != 0;
        set => Data[Offset + 0x78] = (byte)((Data[Offset + 0x78] & 0xFE) | (value ? 1 : 0)); // in battle
    }

    public bool ZMoveUnlocked
    {
        get => (Data[Offset + 0x78] & 2) != 0;
        set => Data[Offset + 0x78] = (byte)((Data[Offset + 0x78] & ~2) | (value ? 2 : 0)); // in battle
    }

    public int BallThrowType
    {
        get => Data[Offset + 0x7A];
        set => Data[Offset + 0x7A] = (byte)(value > 8 ? 0 : value);
    }
}
