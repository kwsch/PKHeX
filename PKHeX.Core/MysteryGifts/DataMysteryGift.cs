using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Mystery Gift backed by serialized fields from ROM/SAV data, rather than observed specifications.
    /// </summary>
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
}
