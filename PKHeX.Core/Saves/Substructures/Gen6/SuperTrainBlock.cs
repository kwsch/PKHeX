using System;

namespace PKHeX.Core
{
    public class MaisonBlock : SaveBlock
    {
        public MaisonBlock(SAV6XY sav, int offset) : base(sav) => Offset = offset;
        public MaisonBlock(SAV6AO sav, int offset) : base(sav) => Offset = offset;

        public ushort GetMaisonStat(int index) { return BitConverter.ToUInt16(Data, Offset + 0x1C0 + (2 * index)); }
        public void SetMaisonStat(int index, ushort value) { BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x1C0 + (2 * index)); }
    }

    public class SuperTrainBlock : SaveBlock
    {
        public SuperTrainBlock(SAV6XY sav, int offset) : base(sav) => Offset = offset;
        public SuperTrainBlock(SAV6AO sav, int offset) : base(sav) => Offset = offset;

        // Structure:
        // 6 bytes stage unlock flags
        // 1 byte distribution stage unlock flags
        // 1 byte counter (max value = 10)
        // float[48] recordScore1; // 0x08, 4byte/entry
        // float[48] recordScore2; // 0xC8, 4byte/entry
        // SS-F-G[48] recordHolder1; // 0x188, 4byte/entry
        // SS-F-G[48] recordHolder2; // 0x248, 4byte/entry
        // byte[12] bags // 0x308
        // u32 tutorial tracker (max value = 6) // 0x314

        // 0x318 total size

        // SS-F-G = u16 species, u8 form, u8 gender

        private const int MAX = 48;
        private const int MAX_RELEASED = 32;
        private const int MAX_DIST = 6;
        private const int MAX_BAG = 12;

        /// <summary>
        /// Checks if a Regimen is unlocked.
        /// </summary>
        /// <param name="index">Index of regimen.</param>
        /// <returns>Is Unlocked</returns>
        public bool GetIsRegimenUnlocked(int index)
        {
            if ((uint)index >= MAX)
                throw new ArgumentException(nameof(index));
            return SAV.GetFlag(Offset, index);
        }

        /// <summary>
        /// Sets a Regimen to the desired unlocked state.
        /// </summary>
        /// <param name="index">Index of regimen.</param>
        /// <param name="value">Is Unlocked</param>
        public void SetIsRegimenUnlocked(int index, bool value)
        {
            if ((uint)index >= MAX)
                throw new ArgumentException(nameof(index));
            SAV.SetFlag(Offset, index, value);
        }

        /// <summary>
        /// Checks if a Distribution Regimen is unlocked.
        /// </summary>
        /// <param name="index">Index of regimen.</param>
        /// <returns>Is Unlocked</returns>
        public bool GetIsDistributionUnlocked(int index)
        {
            if ((uint)index >= MAX_DIST)
                throw new ArgumentException(nameof(index));
            return SAV.GetFlag(Offset + 6, index);
        }

        /// <summary>
        /// Sets a Distribution Regimen to the desired unlocked state.
        /// </summary>
        /// <param name="index">Index of regimen.</param>
        /// <param name="value">Is Unlocked</param>
        public void SetIsDistributionUnlocked(int index, bool value)
        {
            if ((uint)index >= MAX_DIST)
                throw new ArgumentException(nameof(index));
            SAV.SetFlag(Offset + 6, index, value);
        }

        /// <summary>
        /// Counts something up to 10 (overall stages unlocked?)
        /// </summary>
        public byte Counter
        {
            get => Data[Offset + 7];
            set => Data[Offset + 7] = Math.Min((byte)10, value);
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

        /// <summary>
        /// Gets the bag from the desired <see cref="index"/>.
        /// </summary>
        /// <param name="index">Bag index</param>
        public byte GetBag(int index)
        {
            if ((uint)index >= MAX_BAG)
                throw new ArgumentException(nameof(index));
            return Data[Offset + 0x308 + index];
        }

        /// <summary>
        /// Sets a bag to the desired <see cref="index"/>.
        /// </summary>
        /// <param name="index">Bag index</param>
        /// <param name="value">Bag ID</param>
        public void SetBag(int index, byte value)
        {
            if ((uint)index >= MAX_BAG)
                throw new ArgumentException(nameof(index));
            Data[Offset + 0x308 + index] = value;
        }

        /// <summary>
        /// Gets the next open bag index.
        /// </summary>
        /// <returns>Bag index that is empty</returns>
        public int GetOpenBagIndex()
        {
            for (int i = 0; i < MAX_BAG; i++)
            {
                if (GetBag(i) != 0)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Adds a bag to the list of bags.
        /// </summary>
        /// <param name="bag">Bag type</param>
        /// <returns>Bag was added or not</returns>
        public bool AddBag(byte bag)
        {
            int index = GetOpenBagIndex();
            if (index < 0)
                return false;
            SetBag(index, bag);
            return true;
        }

        /// <summary>
        /// Removes a bag from the list of bags.
        /// </summary>
        /// <param name="index">Bag index</param>
        public void RemoveBag(int index)
        {
            if ((uint)index >= MAX_BAG)
                throw new ArgumentException(nameof(index));
            for (int i = index; i < MAX_BAG - 1; i++)
            {
                var next = GetBag(i + 1);
                SetBag(i, next);
            }
            SetBag(MAX_BAG - 1, 0);
        }

        public uint TutorialIndex
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

        /// <summary>
        /// Unlocks all stages.
        /// </summary>
        /// <param name="dist">Unlock all Distribution stages too.</param>
        public void UnlockAllStages(bool dist)
        {
            for (int i = 0; i < MAX_RELEASED; i++)
                SetIsRegimenUnlocked(i, true);

            if (!dist)
                return;

            for (int i = 0; i < MAX_DIST; i++)
                SetIsDistributionUnlocked(i, true);
        }

        /// <summary>
        /// Clears all data in the block.
        /// </summary>
        public void ClearBlock() => Array.Clear(Data, Offset, 0x318);
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

        /// <summary>
        /// Sets the data to match what is in the provided reference.
        /// </summary>
        /// <param name="pkm">Reference to load from.</param>
        public void LoadFrom(PKM pkm)
        {
            Species = (ushort)pkm.Species;
            AltForm = (byte)pkm.AltForm;
            Gender = (byte)pkm.Gender;
        }
    }
}