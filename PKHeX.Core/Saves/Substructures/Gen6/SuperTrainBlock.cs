using System;

namespace PKHeX.Core
{
    public class SuperTrainBlock : SaveBlock
    {
        public SuperTrainBlock(SAV6XY sav, int offset) : base(sav) => Offset = offset;
        public SuperTrainBlock(SAV6AO sav, int offset) : base(sav) => Offset = offset;

        // Structure:
        // 8 bytes ??? (flags?)
        // float[48] recordScore1; // 0x08, 4byte/entry
        // float[48] recordScore2; // 0xC8, 4byte/entry
        // SS-F-G[48] recordHolder1; // 0x188, 4byte/entry
        // SS-F-G[48] recordHolder2; // 0x248, 4byte/entry
        // byte[12] bags? // 0x308
        // u32 ??? // 0x314

        // 0x318 total size

        // SS-F-G = u16 species, u8 form, u8 gender

        private const int MAX = 48;

        public byte[] Bytes8
        {
            get => Data.Slice(Offset + 0, 8);
            set
            {
                if (value.Length != 8)
                    throw new ArgumentException(nameof(value));
                value.CopyTo(Data, Offset + 0);
            }
        }

        /// <summary>
        /// Gets the record time.
        /// </summary>
        /// <param name="index">Index of the record.</param>
        public float GetTime1(int index)
        {
            if ((uint) index >= MAX)
                throw new ArgumentException(nameof(index));

            return BitConverter.ToSingle(Data, Offset + 0x08 + (4 * index));
        }

        /// <summary>
        /// Sets the record time.
        /// </summary>
        /// <param name="index">Index of the record.</param>
        /// <param name="value">Value to set.</param>
        public void SetTime1(int index, float value = 0)
        {
            if ((uint)index >= MAX)
                throw new ArgumentException(nameof(index));

            BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x08 + (4 * index));
        }

        /// <summary>
        /// Gets the record time.
        /// </summary>
        /// <param name="index">Index of the record.</param>
        public float GetTime2(int index)
        {
            if ((uint)index >= MAX)
                throw new ArgumentException(nameof(index));

            return BitConverter.ToSingle(Data, Offset + 0xC8 + (4 * index));
        }

        /// <summary>
        /// Sets the record time.
        /// </summary>
        /// <param name="index">Index of the record.</param>
        /// <param name="value">Value to set.</param>
        public void SetTime2(int index, float value = 0)
        {
            if ((uint)index >= MAX)
                throw new ArgumentException(nameof(index));

            BitConverter.GetBytes(value).CopyTo(Data, Offset + 0xC8 + (4 * index));
        }

        /// <summary>
        /// Returns an object which will edit the record directly from data.
        /// </summary>
        /// <param name="index">Index of the record.</param>
        /// <returns>Object that will edit the record data if modified.</returns>
        public SuperTrainingSpeciesRecord GetHolder1(int index)
        {
            if ((uint)index >= MAX)
                throw new ArgumentException(nameof(index));

            return new SuperTrainingSpeciesRecord(Data, Offset + 0x188 + (4 * index));
        }

        /// <summary>
        /// Returns an object which will edit the record directly from data.
        /// </summary>
        /// <param name="index">Index of the record.</param>
        /// <returns>Object that will edit the record data if modified.</returns>
        public SuperTrainingSpeciesRecord GetHolder2(int index)
        {
            if ((uint)index >= MAX)
                throw new ArgumentException(nameof(index));

            return new SuperTrainingSpeciesRecord(Data, Offset + 0x248 + (4 * index));
        }

        public byte[] Bags
        {
            get => Data.Slice(Offset + 0x308, 12);
            set
            {
                if (value.Length != 12)
                    throw new ArgumentException(nameof(value));
                value.CopyTo(Data, Offset + 0x308);
            }
        }

        public byte GetBag(int index)
        {
            if ((uint)index >= 12)
                throw new ArgumentException(nameof(index));
            return Data[Offset + 0x308 + index];
        }

        public void SetBag(int index, byte value)
        {
            if ((uint)index >= 12)
                throw new ArgumentException(nameof(index));
            Data[Offset + 0x308 + index] = value;
        }

        public void ClearBag(int index) => SetBag(index, 0);

        public uint EndValue
        {
            get => BitConverter.ToUInt32(Data, Offset + 0x314);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x314);
        }

        /// <summary>
        /// Clears all data for the record.
        /// </summary>
        /// <param name="index">Index of the record.</param>
        public void ClearRecord1(int index)
        {
            if ((uint)index >= MAX)
                throw new ArgumentException(nameof(index));

            SetTime1(index, 0f);
            GetHolder1(index).Clear();
        }

        /// <summary>
        /// Clears all data for the record.
        /// </summary>
        /// <param name="index">Index of the record.</param>
        public void ClearRecord2(int index)
        {
            if ((uint)index >= MAX)
                throw new ArgumentException(nameof(index));

            SetTime2(index, 0f);
            GetHolder2(index).Clear();
        }
    }

    public class SuperTrainingSpeciesRecord
    {
        private readonly byte[] Data;
        private readonly int Offset;

        public SuperTrainingSpeciesRecord(byte[] data, int offset)
        {
            Data = data;
            Offset = offset;
        }

        /// <summary>
        /// <see cref="PKM.Species"/> of the Record Holder.
        /// </summary>
        public ushort Species
        {
            get => BitConverter.ToUInt16(Data, Offset + 0);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0);
        }

        /// <summary>
        /// <see cref="PKM.AltForm"/> of the Record Holder.
        /// </summary>
        public byte AltForm
        {
            get => Data[Offset + 2];
            set => Data[Offset + 2] = value;
        }

        /// <summary>
        /// <see cref="PKM.Gender"/> of the Record Holder.
        /// </summary>
        /// <seealso cref="PKHeX.Core.Gender"/>
        public byte Gender
        {
            get => Data[Offset + 3];
            set => Data[Offset + 3] = value;
        }

        /// <summary>
        /// Wipes the record holder's pkm-related data.
        /// </summary>
        public void Clear() => Species = AltForm = Gender = 0;
    }
}