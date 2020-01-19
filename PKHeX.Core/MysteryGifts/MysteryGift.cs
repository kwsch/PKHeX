using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public abstract class DataMysteryGift : MysteryGift
    {
        public readonly byte[] Data;

        protected DataMysteryGift(byte[] data) => Data = data;

        /// <summary>
        /// Returns an array for exporting outside the program (to disk, etc).
        /// </summary>
        public virtual byte[] Write() => Data;

        public override int GetHashCode()
        {
            int hash = 17;
            foreach (var b in Data)
                hash = (hash * 31) + b;
            return hash;
        }

        /// <summary>
        /// Creates a deep copy of the <see cref="MysteryGift"/> object data.
        /// </summary>
        /// <returns></returns>
        public override MysteryGift Clone()
        {
            byte[] data = (byte[])Data.Clone();
            var result = GetMysteryGift(data);
            if (result == null)
                throw new ArgumentException(nameof(MysteryGift));
            return result;
        }

        public override bool Empty => Data.IsRangeAll((byte)0, 0, Data.Length);
    }

    /// <summary>
    /// Mystery Gift Template File
    /// </summary>
    public abstract class MysteryGift : IEncounterable, IMoveset, IRelearn, IGeneration, ILocation
    {
        /// <summary>
        /// Determines whether or not the given length of bytes is valid for a mystery gift.
        /// </summary>
        /// <param name="len">Length, in bytes, of the data of which to determine validity.</param>
        /// <returns>A boolean indicating whether or not the given length is valid for a mystery gift.</returns>
        public static bool IsMysteryGift(long len) => MGSizes.Contains((int)len);

        private static readonly HashSet<int> MGSizes = new HashSet<int>{ WC8.Size, WC6Full.Size, WC6.Size, PGF.Size, PGT.Size, PCD.Size };

        /// <summary>
        /// Converts the given data to a <see cref="MysteryGift"/>.
        /// </summary>
        /// <param name="data">Raw data of the mystery gift.</param>
        /// <param name="ext">Extension of the file from which the <paramref name="data"/> was retrieved.</param>
        /// <returns>An instance of <see cref="MysteryGift"/> representing the given data, or null if <paramref name="data"/> or <paramref name="ext"/> is invalid.</returns>
        /// <remarks>This overload differs from <see cref="GetMysteryGift(byte[])"/> by checking the <paramref name="data"/>/<paramref name="ext"/> combo for validity.  If either is invalid, a null reference is returned.</remarks>
        public static DataMysteryGift? GetMysteryGift(byte[] data, string ext)
        {
            if (ext == null)
                return GetMysteryGift(data);

            switch (data.Length)
            {
                case WC8.Size when ext == ".wc8":
                    return new WC8(data);
                case WB7.SizeFull when ext == ".wb7full":
                case WB7.Size when ext == ".wb7":
                    return new WB7(data);
                case WC7Full.Size when ext == ".wc7full":
                    return new WC7Full(data).Gift;
                case WC7.Size when ext == ".wc7":
                    return new WC7(data);
                case WC6Full.Size when ext == ".wc6full":
                    return new WC6Full(data).Gift;
                case WC6.Size when ext == ".wc6":
                    return new WC6(data);
                case WR7.Size when ext == ".wr7":
                    return new WR7(data);
                case WC8.Size when ext == ".wc8":
                case WC8.Size when ext == ".wc8full":
                    return new WC8(data);

                case PGF.Size when ext == ".pgf":
                    return new PGF(data);
                case PGT.Size when ext == ".pgt":
                    return new PGT(data);
                case PCD.Size when ext == ".pcd" || ext == ".wc4":
                    return new PCD(data);
            }

            return null;
        }

        /// <summary>
        /// Converts the given data to a <see cref="MysteryGift"/>.
        /// </summary>
        /// <param name="data">Raw data of the mystery gift.</param>
        /// <returns>An instance of <see cref="MysteryGift"/> representing the given data, or null if <paramref name="data"/> is invalid.</returns>
        public static DataMysteryGift? GetMysteryGift(byte[] data)
        {
            switch (data.Length)
            {
                case WC6Full.Size:
                    // Check WC7 size collision
                    if (data[0x205] == 0) // 3 * 0x46 for gen6, now only 2.
                        return new WC7Full(data).Gift;
                    return new WC6Full(data).Gift;
                case WC6.Size:
                    // Check year for WC7 size collision
                    if (BitConverter.ToUInt32(data, 0x4C) / 10000 < 2000)
                        return new WC7(data);
                    return new WC6(data);
                case WR7.Size: return new WR7(data);
                case WC8.Size: return new WC8(data);

                case PGF.Size: return new PGF(data);
                case PGT.Size: return new PGT(data);
                case PCD.Size: return new PCD(data);
                default: return null;
            }
        }

        public string Extension => GetType().Name.ToLower();
        public string FileName => $"{CardHeader}.{Extension}";
        public abstract int Format { get; }

        public PKM ConvertToPKM(ITrainerInfo SAV) => ConvertToPKM(SAV, EncounterCriteria.Unrestricted);
        public abstract PKM ConvertToPKM(ITrainerInfo SAV, EncounterCriteria criteria);

        protected abstract bool IsMatchExact(PKM pkm);
        protected abstract bool IsMatchDeferred(PKM pkm);

        public EncounterMatchRating IsMatch(PKM pkm)
        {
            if (!IsMatchExact(pkm))
                return EncounterMatchRating.None;
            if (IsMatchDeferred(pkm))
                return EncounterMatchRating.Deferred;
            return EncounterMatchRating.Match;
        }

        /// <summary>
        /// Creates a deep copy of the <see cref="MysteryGift"/> object data.
        /// </summary>
        /// <returns></returns>
        public abstract MysteryGift Clone();

        /// <summary>
        /// Gets a friendly name for the underlying <see cref="MysteryGift"/> type.
        /// </summary>
        public string Type => GetType().Name;

        /// <summary>
        /// Gets a friendly name for the underlying <see cref="MysteryGift"/> type for the <see cref="IEncounterable"/> interface.
        /// </summary>
        public string Name => $"Event Gift";

        /// <summary>
        /// Gets a friendly name for the underlying <see cref="MysteryGift"/> type for the <see cref="IEncounterable"/> interface.
        /// </summary>
        public string LongName => $"{Name} ({Type})";

        // Properties
        public virtual int Species { get => -1; set { } }
        public abstract bool GiftUsed { get; set; }
        public abstract string CardTitle { get; set; }
        public abstract int CardID { get; set; }

        public abstract bool IsItem { get; set; }
        public abstract int ItemID { get; set; }

        public abstract bool IsPokémon { get; set; }
        public virtual int Quantity { get => 1; set { } }
        public virtual bool Empty => false;

        public virtual bool IsBP { get => false; set { } }
        public virtual int BP { get => 0; set { } }
        public virtual bool IsBean { get => false; set { } }
        public virtual int Bean { get => 0; set { } }
        public virtual int BeanCount { get => 0; set { } }

        public virtual string CardHeader => (CardID > 0 ? $"Card #: {CardID:0000}" : "N/A") + $" - {CardTitle.Replace('\u3000',' ').Trim()}";

        // Search Properties
        public virtual IReadOnlyList<int> Moves { get => Array.Empty<int>(); set { } }
        public virtual IReadOnlyList<int> Relearn { get => Array.Empty<int>(); set { } }
        public virtual int[] IVs { get => Array.Empty<int>(); set { } }
        public virtual bool IsShiny => false;
        public virtual bool IsEgg { get => false; set { } }
        public virtual int HeldItem { get => -1; set { } }
        public virtual int AbilityType { get => -1; set { } }
        public virtual object Content => this;
        public abstract int Gender { get; set; }
        public abstract int Form { get; set; }
        public abstract int TID { get; set; }
        public abstract int SID { get; set; }
        public abstract string OT_Name { get; set; }
        public abstract int Location { get; set; }

        public abstract int Level { get; set; }
        public int LevelMin => Level;
        public int LevelMax => Level;
        public abstract int Ball { get; set; }
        public virtual bool EggEncounter => IsEgg;
        public int Generation { get => Format; set {} }
        public abstract int EggLocation { get; set; }

        public int TrainerID7 => (int)((uint)(TID | (SID << 16)) % 1000000);
        public int TrainerSID7 => (int)((uint)(TID | (SID << 16)) / 1000000);
    }
}
