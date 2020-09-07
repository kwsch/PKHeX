using System;
using System.ComponentModel;

namespace PKHeX.Core
{
#pragma warning disable CA1819 // Properties should not return arrays
    public sealed class BattleTree7 : SaveBlock
    {
        public BattleTree7(SAV7SM sav, int offset) : base(sav) => Offset = offset;
        public BattleTree7(SAV7USUM sav, int offset) : base(sav) => Offset = offset;

        public int GetTreeStreak(int battletype, bool super, bool max)
        {
            if (battletype > 3)
                throw new ArgumentException(nameof(battletype));

            var offset = GetStreakOffset(battletype, super, max);
            return BitConverter.ToUInt16(Data, Offset + offset);
        }

        public void SetTreeStreak(int value, int battletype, bool super, bool max)
        {
            if (battletype > 3)
                throw new ArgumentException(nameof(battletype));

            if (value > ushort.MaxValue)
                value = ushort.MaxValue;

            var offset = GetStreakOffset(battletype, super, max);
            BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + offset);
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
                throw new ArgumentException(nameof(index));

            var id = BitConverter.ToInt16(Data, Offset + 0x24 + (index * 2));
            var p1 = BitConverter.ToInt16(Data, Offset + 0x88 + (index * 2));
            var p2 = BitConverter.ToInt16(Data, Offset + 0xEC + (index * 2));

            var a1 = (sbyte)Data[Offset + 0x154 + index];
            var a2 = (sbyte)Data[Offset + 0x186 + index];

            var poke1 = new BattleTreePokemon(p1, a1);
            var poke2 = new BattleTreePokemon(p2, a2);
            return new BattleTreeTrainer(id, poke1, poke2);
        }

        public void SetTrainer(BattleTreeTrainer tr, in int index)
        {
            if ((uint)index >= ScoutCount)
                throw new ArgumentException(nameof(index));

            BitConverter.GetBytes(tr.ID).CopyTo(Data, Offset + 0x24 + (index * 2));
            BitConverter.GetBytes(tr.Poke1.ID).CopyTo(Data, Offset + 0x88 + (index * 2));
            BitConverter.GetBytes(tr.Poke2.ID).CopyTo(Data, Offset + 0xEC + (index * 2));

            Data[Offset + 0x154 + index] = (byte)tr.Poke1.AbilityIndex;
            Data[Offset + 0x186 + index] = (byte)tr.Poke2.AbilityIndex;
        }

        public int Music
        {
            get => BitConverter.ToInt32(Data, Offset + 0x18);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x18);
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
    public sealed class BattleTreeTrainer
    {
        public short ID { get; set; }
        public BattleTreePokemon Poke1 { get; set; }
        public BattleTreePokemon Poke2 { get; set; }

        public BattleTreeTrainer(short id, BattleTreePokemon poke1, BattleTreePokemon poke2)
        {
            ID = id;
            Poke1 = poke1;
            Poke2 = poke2;
        }

        public override string ToString() => $"{ID}: [{Poke1}] & [{Poke2}]";
    }

    [TypeConverter(typeof(ValueTypeTypeConverter))]
    public sealed class BattleTreePokemon
    {
        public short ID { get; set; }
        public sbyte AbilityIndex { get; set; }

        public BattleTreePokemon(short p1, sbyte a1)
        {
            ID = p1;
            AbilityIndex = a1;
        }

        public override string ToString() => $"{ID},{AbilityIndex}";
    }
}
