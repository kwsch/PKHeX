using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Player status and info about the trainer
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class MyStatus8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw)
{
    public const int MAX_MONEY = 999_999;
    public const byte MAX_BADGE = 8;
    // public const byte MAX_RANK = 5; // unused?

    private Span<byte> OriginalTrainerTrash => Data[..0x1A];

    public string OT
    {
        get => SAV.GetString(OriginalTrainerTrash);
        set => SAV.SetString(OriginalTrainerTrash, value, SAV.MaxStringLengthTrainer, StringConverterOption.ClearZero);
    }

    public uint ID32
    {
        get => ReadUInt32LittleEndian(Data[0x1C..]);
        set => WriteUInt32LittleEndian(Data[0x1C..], value);
    }

    public ushort TID16
    {
        get => ReadUInt16LittleEndian(Data[0x1C..]);
        set => WriteUInt16LittleEndian(Data[0x1C..], value);
    }

    public ushort SID16
    {
        get => ReadUInt16LittleEndian(Data[0x1E..]);
        set => WriteUInt16LittleEndian(Data[0x1E..], value);
    }

    public uint Money
    {
        get => ReadUInt32LittleEndian(Data[0x20..]);
        set => WriteUInt32LittleEndian(Data[0x20..], Math.Min(MAX_MONEY, value));
    }

    public bool Male { get => Data[0x24] == 1; set => Data[0x24] = (byte)(value ? 1 : 0); }
    // 3byte align
    public byte RegionCode { get => Data[0x28]; set => Data[0x28] = value; }
    public byte BadgeCount { get => Data[0x29]; set => Data[0x29] = Math.Min(MAX_BADGE, value); }
    public byte TrainerView { get => Data[0x2A]; set => Data[0x2A] = value; }
    public byte ROMCode { get => Data[0x2B]; set => Data[0x2B] = value; }
    public bool GameClear { get => Data[0x2C] == 1; set => Data[0x2C] = (byte)(value ? 1 : 0); }
    // 3byte align
    public byte BodyType { get => Data[0x30]; set => Data[0x30] = value; }
    public Fashion FashionID { get => (Fashion)Data[0x31]; set => Data[0x31] = (byte)value; }
    // 2byte align

    public MoveType StarterType
    {
        get => (MoveType)ReadInt32LittleEndian(Data[0x34..]);
        set => WriteInt32LittleEndian(Data[0x34..], (int)value);
    }

    public bool DSPlayer { get => Data[0x38] == 1; set => Data[0x38] = (byte)(value ? 1 : 0); }
    // 3byte align

    /// <summary> turewalkMemberIndex </summary>
    public int FollowIndex { get => ReadInt32LittleEndian(Data[0x3C..]); set => WriteInt32LittleEndian(Data[0x3C..], value); }
    public int X { get => ReadInt32LittleEndian(Data[0x40..]); set => WriteInt32LittleEndian(Data[0x40..], value); }
    public int Y { get => ReadInt32LittleEndian(Data[0x44..]); set => WriteInt32LittleEndian(Data[0x44..], value); }
    public float Height { get => ReadSingleLittleEndian(Data[0x48..]); set => WriteSingleLittleEndian(Data[0x48..], value); }
    public float Rotation { get => ReadSingleLittleEndian(Data[0x4C..]); set => WriteSingleLittleEndian(Data[0x4C..], value); }

    // end structure!

    public GameVersion Game
    {
        get => ROMCode switch
        {
            0 => GameVersion.BD,
            1 => GameVersion.SP,
            _ => throw new ArgumentOutOfRangeException(nameof(Game)),
        };
        set => ROMCode = value switch
        {
            GameVersion.BD => 0,
            GameVersion.SP => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(Game)),
        };
    }

    public enum Fashion : byte
    {
        Everyday_M = 0,
        Contest_M = 1,
        Pikachu_Hoodie_M = 2,
        Platinum_M = 3,
        Overalls_M = 4,
        Eevee_Jacket_M = 5,
        Gengar_Jacket_M = 6,
        Cyber_M = 7,
        Summer_M = 8,
        Winter_M = 9,
        Spring_M = 10,
        Casual_M = 11,
        Leather_Jacket_M = 12,

        Everyday_F = 100,
        Contest_F = 101,
        Pikachu_Hoodie_F = 102,
        Platinum_F = 103,
        Overalls_F = 104,
        Eevee_Jacket_F = 105,
        Gengar_Jacket_F = 106,
        Cyber_F = 107,
        Summer_F = 108,
        Winter_F = 109,
        Spring_F = 110,
        Casual_F = 111,
        Leather_Jacket_F = 112,
    }
}
