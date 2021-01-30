using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Mystery Gift Template File
    /// </summary>
    public abstract class MysteryGift : IEncounterable, IMoveset, IRelearn, ILocation
    {
        /// <summary>
        /// Determines whether or not the given length of bytes is valid for a mystery gift.
        /// </summary>
        /// <param name="len">Length, in bytes, of the data of which to determine validity.</param>
        /// <returns>A boolean indicating whether or not the given length is valid for a mystery gift.</returns>
        public static bool IsMysteryGift(long len) => Sizes.Contains((int)len);

        private static readonly HashSet<int> Sizes = new() { WC8.Size, WC6Full.Size, WC6.Size, PGF.Size, PGT.Size, PCD.Size };

        /// <summary>
        /// Converts the given data to a <see cref="MysteryGift"/>.
        /// </summary>
        /// <param name="data">Raw data of the mystery gift.</param>
        /// <param name="ext">Extension of the file from which the <paramref name="data"/> was retrieved.</param>
        /// <returns>An instance of <see cref="MysteryGift"/> representing the given data, or null if <paramref name="data"/> or <paramref name="ext"/> is invalid.</returns>
        /// <remarks>This overload differs from <see cref="GetMysteryGift(byte[])"/> by checking the <paramref name="data"/>/<paramref name="ext"/> combo for validity.  If either is invalid, a null reference is returned.</remarks>
        public static DataMysteryGift? GetMysteryGift(byte[] data, string ext) => data.Length switch
        {
            PGT.Size when ext == ".pgt" => new PGT(data),
            PCD.Size when ext is ".pcd" or ".wc4" => new PCD(data),
            PGF.Size when ext == ".pgf" => new PGF(data),
            WC6.Size when ext == ".wc6" => new WC6(data),
            WC7.Size when ext == ".wc7" => new WC7(data),
            WB7.Size when ext == ".wb7" => new WB7(data),
            WR7.Size when ext == ".wr7" => new WR7(data),
            WC8.Size when ext is ".wc8" or ".wc8full" => new WC8(data),

            WB7.SizeFull when ext == ".wb7full" => new WB7(data),
            WC6Full.Size when ext == ".wc6full" => new WC6Full(data).Gift,
            WC7Full.Size when ext == ".wc7full" => new WC7Full(data).Gift,
            _ => null
        };

        /// <summary>
        /// Converts the given data to a <see cref="MysteryGift"/>.
        /// </summary>
        /// <param name="data">Raw data of the mystery gift.</param>
        /// <returns>An instance of <see cref="MysteryGift"/> representing the given data, or null if <paramref name="data"/> is invalid.</returns>
        public static DataMysteryGift? GetMysteryGift(byte[] data) => data.Length switch
        {
            PGT.Size => new PGT(data),
            PCD.Size => new PCD(data),
            PGF.Size => new PGF(data),
            WR7.Size => new WR7(data),
            WC8.Size => new WC8(data),

            // WC6/WC7: Check year
            WC6.Size => BitConverter.ToUInt32(data, 0x4C) / 10000 < 2000 ? new WC7(data) : new WC6(data),
            // WC6Full/WC7Full: 0x205 has 3 * 0x46 for gen6, now only 2.
            WC6Full.Size => data[0x205] == 0 ? new WC7Full(data).Gift : new WC6Full(data).Gift,
            _ => null
        };

        public string Extension => GetType().Name.ToLower();
        public string FileName => $"{CardHeader}.{Extension}";
        public abstract int Generation { get; }

        public PKM ConvertToPKM(ITrainerInfo sav) => ConvertToPKM(sav, EncounterCriteria.Unrestricted);
        public abstract PKM ConvertToPKM(ITrainerInfo sav, EncounterCriteria criteria);

        public abstract bool IsMatchExact(PKM pkm, DexLevel evo);
        protected abstract bool IsMatchDeferred(PKM pkm);
        protected abstract bool IsMatchPartial(PKM pkm);

        public EncounterMatchRating GetMatchRating(PKM pkm)
        {
            if (IsMatchPartial(pkm))
                return EncounterMatchRating.PartialMatch;
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
        public string Name => "Event Gift";

        /// <summary>
        /// Gets a friendly name for the underlying <see cref="MysteryGift"/> type for the <see cref="IEncounterable"/> interface.
        /// </summary>
        public string LongName => $"{Name} ({Type})";

        public virtual GameVersion Version
        {
            get => GameUtil.GetVersion(Generation);
            set { }
        }

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

        public virtual string CardHeader => (CardID > 0 ? $"Card #: {CardID:0000}" : "N/A") + $" - {CardTitle.Replace('\u3000',' ').Trim()}";

        // Search Properties
        public virtual IReadOnlyList<int> Moves { get => Array.Empty<int>(); set { } }
        public virtual IReadOnlyList<int> Relearn { get => Array.Empty<int>(); set { } }
        public virtual int[] IVs { get => Array.Empty<int>(); set { } }
        public virtual bool IsShiny => false;
        public virtual bool IsEgg { get => false; set { } }
        public virtual int HeldItem { get => -1; set { } }
        public virtual int AbilityType { get => -1; set { } }
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
        public abstract int EggLocation { get; set; }

        public int TrainerID7 => (int)((uint)(TID | (SID << 16)) % 1000000);
        public int TrainerSID7 => (int)((uint)(TID | (SID << 16)) / 1000000);

        /// <summary>
        /// Checks if the <see cref="PKM"/> has the <see cref="move"/> in its current move list.
        /// </summary>
        public bool HasMove(int move) => Moves.Contains(move);
    }
}
