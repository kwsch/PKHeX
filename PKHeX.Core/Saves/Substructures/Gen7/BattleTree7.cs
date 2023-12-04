using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class BattleTree7 : SaveBlock<SAV7>
{
    public BattleTree7(SAV7SM sav, int offset) : base(sav) => Offset = offset;
    public BattleTree7(SAV7USUM sav, int offset) : base(sav) => Offset = offset;

    public int GetTreeStreak(int battletype, bool super, bool max)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(battletype, 3);

        var offset = GetStreakOffset(battletype, super, max);
        return ReadUInt16LittleEndian(Data.AsSpan(Offset + offset));
    }

    public void SetTreeStreak(int value, int battletype, bool super, bool max)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(battletype, 3);

        if (value > ushort.MaxValue)
            value = ushort.MaxValue;

        var offset = GetStreakOffset(battletype, super, max);
        WriteUInt16LittleEndian(Data.AsSpan(Offset + offset), (ushort)value);
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
        if ((uint)index >= ScoutCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        var id = ReadInt16LittleEndian(Data.AsSpan(Offset + 0x24 + (index * 2)));
        var p1 = ReadInt16LittleEndian(Data.AsSpan(Offset + 0x88 + (index * 2)));
        var p2 = ReadInt16LittleEndian(Data.AsSpan(Offset + 0xEC + (index * 2)));

        var a1 = (sbyte)Data[Offset + 0x154 + index];
        var a2 = (sbyte)Data[Offset + 0x186 + index];

        var poke1 = new BattleTreePokemon(p1, a1);
        var poke2 = new BattleTreePokemon(p2, a2);
        return new BattleTreeTrainer(id, poke1, poke2);
    }

    public void SetTrainer(BattleTreeTrainer tr, in int index)
    {
        if ((uint)index >= ScoutCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        WriteInt16LittleEndian(Data.AsSpan(Offset + 0x24 + (index * 2)), tr.ID      );
        WriteInt16LittleEndian(Data.AsSpan(Offset + 0x88 + (index * 2)), tr.Poke1.ID);
        WriteInt16LittleEndian(Data.AsSpan(Offset + 0xEC + (index * 2)), tr.Poke2.ID);

        Data[Offset + 0x154 + index] = (byte)tr.Poke1.AbilityIndex;
        Data[Offset + 0x186 + index] = (byte)tr.Poke2.AbilityIndex;
    }

    public int Music
    {
        get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x18));
        set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x18), value);
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
public sealed class BattleTreeTrainer(short id, BattleTreePokemon poke1, BattleTreePokemon poke2)
{
    public short ID { get; set; } = id;
    public BattleTreePokemon Poke1 { get; set; } = poke1;
    public BattleTreePokemon Poke2 { get; set; } = poke2;

    public override string ToString() => $"{ID}: [{Poke1}] & [{Poke2}]";
}

[TypeConverter(typeof(ValueTypeTypeConverter))]
public sealed class BattleTreePokemon(short p1, sbyte a1)
{
    public short ID { get; set; } = p1;
    public sbyte AbilityIndex { get; set; } = a1;

    public override string ToString() => $"{ID},{AbilityIndex}";
}
