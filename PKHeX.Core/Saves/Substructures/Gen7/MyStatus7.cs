using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class MyStatus7(SAV7 sav, Memory<byte> raw) : SaveBlock<SAV7>(sav, raw), IRegionOrigin, IGameSync
{
    public const int GameSyncIDSize = 16; // 64 bits
    public const int NexUniqueIDSize = 32; // 128 bits
    int IGameSync.GameSyncIDSize => GameSyncIDSize;

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

    public string GameSyncID
    {
        get => Util.GetHexStringFromBytes(Data.Slice(0x10, GameSyncIDSize / 2));
        set
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(value.Length, GameSyncIDSize);
            Span<byte> dest = Data.Slice(0x10, GameSyncIDSize / 2);
            dest.Clear();
            Util.GetBytesFromHexString(value, dest);
        }
    }

    public string NexUniqueID
    {
        get => Util.GetHexStringFromBytes(Data.Slice(0x18, NexUniqueIDSize / 2));
        set
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(value.Length, NexUniqueIDSize);
            Span<byte> dest = Data.Slice(0x18, GameSyncIDSize / 2);
            dest.Clear();
            Util.GetBytesFromHexString(value, dest);
        }
    }

    public uint FestaID
    {
        get => ReadUInt32LittleEndian(Data[0x28..]);
        set => WriteUInt32LittleEndian(Data[0x28..], value);
    }

    public byte Region
    {
        get => Data[0x2E];
        set => Data[0x2E] = value;
    }

    public byte Country
    {
        get => Data[0x2F];
        set => Data[0x2F] = value;
    }

    public byte ConsoleRegion
    {
        get => Data[0x34];
        set => Data[0x34] = value;
    }

    public int Language
    {
        get => Data[0x35];
        set => Data[0x35] = (byte)value;
    }

    private Span<byte> OriginalTrainerTrash => Data.Slice(0x38, 0x1A);

    public string OT
    {
        get => SAV.GetString(OriginalTrainerTrash);
        set => SAV.SetString(OriginalTrainerTrash, value, SAV.MaxStringLengthTrainer, StringConverterOption.ClearZero);
    }

    public int DressUpSkinColor
    {
        get => (Data[0x54] >> 2) & 7;
        set => Data[0x54] = (byte)((Data[0x54] & ~(7 << 2)) | (value << 2));
    }

    public int MultiplayerSpriteID // holdover from Gen6
    {
        get => Data[0x58];
        set => Data[0x58] = (byte)value;
    }

    public bool MegaUnlocked
    {
        get => (Data[0x78] & 0x01) != 0;
        set => Data[0x78] = (byte)((Data[0x78] & 0xFE) | (value ? 1 : 0)); // in battle
    }

    public bool ZMoveUnlocked
    {
        get => (Data[0x78] & 2) != 0;
        set => Data[0x78] = (byte)((Data[0x78] & ~2) | (value ? 2 : 0)); // in battle
    }

    public byte BallThrowType
    {
        get => Data[0x7A];
        set => Data[0x7A] = (byte)(value > 8 ? 0 : value);
    }
}
