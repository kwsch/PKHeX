using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PKHeX.Core
{
    /// <summary>
    /// Stores customized ball seal configurations.
    /// </summary>
    /// <remarks>size 0x4288</remarks>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class SealBallDecoData8b : SaveBlock
    {
        public const int COUNT_CAPSULE = 99; // CapsuleData[99]

        private const int SIZE = 4 + (COUNT_CAPSULE * SealCapsule8b.SIZE); // 0x4288

        public SealBallDecoData8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        public void Clear() => Data.AsSpan(Offset, SIZE).Clear();

        public byte CapsuleCount { get => Data[Offset]; set => Data[Offset] = value; }

        public SealCapsule8b[] Capsules
        {
            get => GetCapsules();
            set => SetCapsules(value);
        }

        private SealCapsule8b[] GetCapsules()
        {
            var result = new SealCapsule8b[COUNT_CAPSULE];
            for (int i = 0; i < result.Length; i++)
                result[i] = new SealCapsule8b(Data, Offset + 4 + (i * SealCapsule8b.SIZE));
            return result;
        }

        private static void SetCapsules(IReadOnlyList<SealCapsule8b> value)
        {
            if (value.Count != COUNT_CAPSULE)
                throw new ArgumentOutOfRangeException(nameof(value.Count));
            // data is already hard-referencing the original byte array. This is mostly a hack for Property Grid displays.
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class SealCapsule8b
    {
        public const int COUNT_SEAL = 20; // AffixSealData[20]
        public const int SIZE = 12 + (COUNT_SEAL * AffixSealData8b.SIZE); // 0xAC

        private readonly int Offset;
        private readonly byte[] Data;

        public override string ToString() => $"{(Species)Species}-{EncryptionConstant:X8}-{Unknown}";

        public SealCapsule8b(byte[] data, int offset)
        {
            Data = data;
            Offset = offset;
        }
        public uint Species            { get => BitConverter.ToUInt32(Data, Offset + 0); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0); }
        public uint EncryptionConstant { get => BitConverter.ToUInt32(Data, Offset + 4); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 4); }
        public uint Unknown            { get => BitConverter.ToUInt32(Data, Offset + 8); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 8); }

        public AffixSealData8b[] Seals
        {
            get => GetSeals();
            set => SetSeals(value);
        }

        private AffixSealData8b[] GetSeals()
        {
            var result = new AffixSealData8b[COUNT_SEAL];
            for (int i = 0; i < result.Length; i++)
                result[i] = new AffixSealData8b(Data, Offset + 0xC + (i * AffixSealData8b.SIZE));
            return result;
        }

        private static void SetSeals(IReadOnlyList<AffixSealData8b> value)
        {
            if (value.Count != COUNT_SEAL)
                throw new ArgumentOutOfRangeException(nameof(value.Count));
            // data is already hard-referencing the original byte array. This is mostly a hack for Property Grid displays.
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class AffixSealData8b
    {
        public const int SIZE = 8; // u16 id, s16 x,y,z

        private readonly int Offset;
        private readonly byte[] Data;

        public override string ToString() => $"{(Seal8b)SealID}-({X},{Y},{Z})";

        public AffixSealData8b(byte[] data, int offset)
        {
            Data = data;
            Offset = offset;
        }

        public ushort SealID { get => BitConverter.ToUInt16(Data, Offset + 0); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0); }
        public short X { get => BitConverter.ToInt16(Data, Offset + 2); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 2); }
        public short Y { get => BitConverter.ToInt16(Data, Offset + 4); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 4); }
        public short Z { get => BitConverter.ToInt16(Data, Offset + 6); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 6); }
    }
}
