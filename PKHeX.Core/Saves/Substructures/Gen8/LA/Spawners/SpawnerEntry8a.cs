using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Data structure representing a predetermined Pokémon-to-be-encountered, with fixed randomness and details like position if already determined.
/// </summary>
/// <remarks>
/// The game takes the <see cref="GenerateSeed"/> to roll for which encounter slot, then uses the next rand() to generate the entity's data when interacted with.
/// </remarks>
public readonly ref struct SpawnerEntry8a
{
    public const int SIZE = 0x80;
    private readonly Span<byte> Data;

    // ReSharper disable once ConvertToPrimaryConstructor
    public SpawnerEntry8a(Span<byte> data) => Data = data;

    public float Coordinate_00 { get => ReadSingleLittleEndian(Data); set => WriteSingleLittleEndian(Data, value); }
    public float Coordinate_04 { get => ReadSingleLittleEndian(Data[0x04..]); set => WriteSingleLittleEndian(Data[0x04..], value); }
    public float Coordinate_08 { get => ReadSingleLittleEndian(Data[0x08..]); set => WriteSingleLittleEndian(Data[0x08..], value); }
    // 10 - float?
    public float Field_14 { get => ReadSingleLittleEndian(Data[0x14..]); set => WriteSingleLittleEndian(Data[0x14..], value); }
    // 18 - float?
    public float Field_1C { get => ReadSingleLittleEndian(Data[0x1C..]); set => WriteSingleLittleEndian(Data[0x1C..], value); }
    public ulong GenerateSeed { get => ReadUInt64LittleEndian(Data[0x20..]); set => WriteUInt64LittleEndian(Data[0x20..], value); }

    public float Field_34 { get => ReadSingleLittleEndian(Data[0x34..]); set => WriteSingleLittleEndian(Data[0x34..], value); }
    public float Field_38 { get => ReadSingleLittleEndian(Data[0x38..]); set => WriteSingleLittleEndian(Data[0x38..], value); }
    public float Field_3C { get => ReadSingleLittleEndian(Data[0x3C..]); set => WriteSingleLittleEndian(Data[0x3C..], value); }

    public byte  HasBeenStunned { get => Data[0x40]; set => Data[0x40] = value; } // 1 if previously stunned (less resistant to future stuns)
    public byte  IsDead   { get => Data[0x42]; set => Data[0x42] = value; } // 1 if battled or captured
    public byte  IsEmpty  { get => Data[0x43]; set => Data[0x43] = value; } // 0 for slots with data, 1 for empty
    public byte  Field_45 { get => Data[0x45]; set => Data[0x45] = value; }
    public byte  Field_46 { get => Data[0x46]; set => Data[0x46] = value; }
    public short Field_48 { get => ReadInt16LittleEndian (Data[0x48..]); set => WriteInt16LittleEndian (Data[0x48..], value); }
    public byte  Field_4A { get => Data[0x4A]; set => Data[0x4A] = value; }

    public byte IsSeen { get => Data[0x4C]; set => Data[0x4C] = value; } // 1 = Fixed values present (???), use them when regenerating
    public byte FixedTime { get => Data[0x4D]; set => Data[0x4D] = value; } // Time of Day the player first saw the entity
    public byte FixedWeather { get => Data[0x4E]; set => Data[0x4E] = value; } // Weather the player first saw the entity
    public byte FixedShinyRolls { get => Data[0x4F]; set => Data[0x4F] = value; } // Shiny rolls originally generated with

    /// <summary>
    /// Determines the Alpha move index (from move shop)
    /// </summary>
    public ulong AlphaSeed { get => ReadUInt64LittleEndian(Data[0x58..]); set => WriteUInt64LittleEndian(Data[0x58..], value); }

    /// <summary>
    /// When spawning an entity, the game assigns a random alpha move index from the allowed move shop list.
    /// </summary>
    /// <param name="countMoveShopCanLearn">Count of move shop moves that the species can learn</param>
    /// <returns>Learn-able move shop index to set as the <see cref="PA8.AlphaMove"/></returns>
    public int GetAlphaMoveIndex(int countMoveShopCanLearn)
    {
        var rand = new Xoroshiro128Plus(AlphaSeed);
        return (int)rand.NextInt((ulong)countMoveShopCanLearn);
    }
}
