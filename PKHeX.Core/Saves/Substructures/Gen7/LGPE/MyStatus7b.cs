using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class MyStatus7b(SAV7b sav, Memory<byte> raw) : SaveBlock<SAV7b>(sav, raw), IGameSync
{
    // Player Information

    // idb uint8 offset: 0x58

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
        set => Data[5] = OverworldGender = value;
    }

    public const int GameSyncIDSize = 16; // 8 bytes
    int IGameSync.GameSyncIDSize => GameSyncIDSize;

    public string GameSyncID
    {
        get => Util.GetHexStringFromBytes(Data.Slice(0x10, GameSyncIDSize / 2));
        set
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value.Length, 16);
            Span<byte> dest = Data.Slice(0x10, GameSyncIDSize / 2);
            dest.Clear();
            Util.GetBytesFromHexString(value, dest);
        }
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

    // The value here corresponds to a Trainer Class string (ranging from 000 to 383, use pkNX to get a full list).
    public byte TrainerClassIndex
    {
        get => Data[0x076];
        set => Data[0x076] = value;
    }

    public StarterChoice7b StarterChoice
    {
        get => (StarterChoice7b)Data[0x0B8];
        set => Data[0x0B8] = (byte)value;
    }

    public byte StarterGender
    {
        get => Data[0x0B9];
        set => Data[0x0B9] = value;
    }

    public byte OverworldGender // Model
    {
        get => Data[0x108];
        set => Data[0x108] = value;
    }

    public enum StarterChoice7b : byte
    {
        None = 0,
        Pikachu = 1,
        Eevee = 2,
    }

    // Fashion and Appearance
    public ulong StarterHairstyle      { get => ReadUInt64LittleEndian(Data[0x0C0..]); set => WriteUInt64LittleEndian(Data[0x0C0..], value); }
    public ulong StarterHat            { get => ReadUInt64LittleEndian(Data[0x0C8..]); set => WriteUInt64LittleEndian(Data[0x0C8..], value); }
    public ulong StarterGlasses        { get => ReadUInt64LittleEndian(Data[0x0D0..]); set => WriteUInt64LittleEndian(Data[0x0D0..], value); }
    public ulong StarterAccessoryEar   { get => ReadUInt64LittleEndian(Data[0x0D8..]); set => WriteUInt64LittleEndian(Data[0x0D8..], value); }
    public ulong StarterAccessoryChest { get => ReadUInt64LittleEndian(Data[0x0E0..]); set => WriteUInt64LittleEndian(Data[0x0E0..], value); }
    public ulong StarterAccessoryBack  { get => ReadUInt64LittleEndian(Data[0x0E8..]); set => WriteUInt64LittleEndian(Data[0x0E8..], value); }
    public ulong StarterAccessoryTail  { get => ReadUInt64LittleEndian(Data[0x0F0..]); set => WriteUInt64LittleEndian(Data[0x0F0..], value); }
    public ulong StarterClothes        { get => ReadUInt64LittleEndian(Data[0x0F8..]); set => WriteUInt64LittleEndian(Data[0x0F8..], value); }
    public ulong PlayerSkinColor       { get => ReadUInt64LittleEndian(Data[0x110..]); set => WriteUInt64LittleEndian(Data[0x110..], value); }
    public ulong PlayerEyeColor        { get => ReadUInt64LittleEndian(Data[0x118..]); set => WriteUInt64LittleEndian(Data[0x118..], value); }
    public ulong PlayerHairColor       { get => ReadUInt64LittleEndian(Data[0x120..]); set => WriteUInt64LittleEndian(Data[0x120..], value); }
    public ulong PlayerOuterwear       { get => ReadUInt64LittleEndian(Data[0x128..]); set => WriteUInt64LittleEndian(Data[0x128..], value); }
    public ulong PlayerPants           { get => ReadUInt64LittleEndian(Data[0x130..]); set => WriteUInt64LittleEndian(Data[0x130..], value); }
    public ulong PlayerShoes           { get => ReadUInt64LittleEndian(Data[0x138..]); set => WriteUInt64LittleEndian(Data[0x138..], value); }
    public ulong PlayerHat             { get => ReadUInt64LittleEndian(Data[0x140..]); set => WriteUInt64LittleEndian(Data[0x140..], value); }
    public ulong PlayerBag             { get => ReadUInt64LittleEndian(Data[0x148..]); set => WriteUInt64LittleEndian(Data[0x148..], value); }
}
