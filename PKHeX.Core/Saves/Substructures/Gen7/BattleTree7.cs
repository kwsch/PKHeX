using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class BattleTree7(SAV7 sav, Memory<byte> raw) : SaveBlock<SAV7>(sav, raw)
{
    public const int BattleTypeMax = 4;

    public int GetTreeStreak(int battletype, bool super, bool max)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(battletype, BattleTypeMax);

        var offset = GetStreakOffset(battletype, super, max);
        return ReadUInt16LittleEndian(Data[offset..]);
    }

    public void SetTreeStreak(int value, int battletype, bool super, bool max)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(battletype, BattleTypeMax);

        if (value > ushort.MaxValue)
            value = ushort.MaxValue;

        var offset = GetStreakOffset(battletype, super, max);
        WriteUInt16LittleEndian(Data[offset..], (ushort)value);
    }

    private static int GetStreakOffset(int battletype, bool super, bool max)
    {
        int offset = 8 * battletype;
        if (super)
            offset += 2;
        if (max)
            offset += 4;
        return offset;
    }

    private const int ScoutCount = 50;

    public BattleTreeTrainer GetTrainer(in int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, ScoutCount);

        var id = ReadInt16LittleEndian(Data[(0x24 + (index * 2))..]);
        var p1 = ReadInt16LittleEndian(Data[(0x88 + (index * 2))..]);
        var p2 = ReadInt16LittleEndian(Data[(0xEC + (index * 2))..]);

        var a1 = (sbyte)Data[0x154 + index];
        var a2 = (sbyte)Data[0x186 + index];

        var poke1 = new BattleTreePokemon(p1, a1);
        var poke2 = new BattleTreePokemon(p2, a2);
        return new BattleTreeTrainer(id, poke1, poke2);
    }

    public void SetTrainer(BattleTreeTrainer tr, in int index)
    {
        if ((uint)index >= ScoutCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        WriteInt16LittleEndian(Data[(0x24 + (index * 2))..], tr.ID      );
        WriteInt16LittleEndian(Data[(0x88 + (index * 2))..], tr.Poke1.ID);
        WriteInt16LittleEndian(Data[(0xEC + (index * 2))..], tr.Poke2.ID);

        Data[0x154 + index] = (byte)tr.Poke1.AbilityIndex;
        Data[0x186 + index] = (byte)tr.Poke2.AbilityIndex;
    }

    public int Music
    {
        get => ReadInt32LittleEndian(Data[0x18..]);
        set => WriteInt32LittleEndian(Data[0x18..], value);
    }

    public BattleTreeTrainer[] ScoutedTrainers
    {
        get
        {
            var result = new BattleTreeTrainer[ScoutCount];
            for (int i = 0; i < result.Length; i++)
                result[i] = GetTrainer(i);
            return result;
        }
        set
        {
            for (int i = 0; i < value.Length; i++)
                SetTrainer(value[i], i);
        }
    }
}

[TypeConverter(typeof(ValueTypeTypeConverter))]
public sealed class BattleTreeTrainer(short ID, BattleTreePokemon Poke1, BattleTreePokemon Poke2)
{
    public short ID { get; set; } = ID;
    public BattleTreePokemon Poke1 { get; set; } = Poke1;
    public BattleTreePokemon Poke2 { get; set; } = Poke2;

    public override string ToString() => $"{ID}: [{Poke1}] & [{Poke2}]";
}

[TypeConverter(typeof(ValueTypeTypeConverter))]
public sealed class BattleTreePokemon(short ID, sbyte AbilityIndex)
{
    public short ID { get; set; } = ID;
    public sbyte AbilityIndex { get; set; } = AbilityIndex;

    public override string ToString() => $"{ID},{AbilityIndex}";
}
