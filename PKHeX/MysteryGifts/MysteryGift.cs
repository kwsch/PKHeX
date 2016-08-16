using System;
using System.Linq;

namespace PKHeX
{
    public abstract class MysteryGift
    {
        internal static bool getIsMysteryGift(long len)
        {
            return new[] { WC6.SizeFull, WC6.Size, PGF.Size, PGT.Size, PCD.Size }.Contains((int)len);
        }
        internal static MysteryGift getMysteryGift(byte[] data, string ext)
        {
            if (data.Length == WC6.SizeFull && ext == ".wc6full")
                return new WC6(data);
            if (data.Length == WC6.Size && ext == ".wc6")
                return new WC6(data);
            if (data.Length == PGF.Size && ext == ".pgf")
                return new PGF(data);
            if (data.Length == PGT.Size && ext == ".pgt")
                return new PGT(data);
            if (data.Length == PCD.Size && ext == ".pcd")
                return new PCD(data);
            return null;
        }
        internal static MysteryGift getMysteryGift(byte[] data)
        {
            switch (data.Length)
            {
                case WC6.SizeFull:
                case WC6.Size:
                    return new WC6(data);
                case PGF.Size:
                    return new PGF(data);
                case PGT.Size:
                    return new PGT(data);
                case PCD.Size:
                    return new PCD(data);
                default:
                    return null;
            }
        }

        public abstract string Extension { get; }
        public virtual byte[] Data { get; set; }
        public abstract PKM convertToPKM(SaveFile SAV);

        public MysteryGift Clone()
        {
            byte[] data = (byte[])Data.Clone();
            return getMysteryGift(data);
        }
        public string Type => GetType().Name;

        // Properties
        public abstract bool GiftUsed { get; set; }
        public abstract string CardTitle { get; set; }
        public abstract int CardID { get; set; }

        public abstract bool IsItem { get; set; }
        public abstract int Item { get; set; }

        public abstract bool IsPokémon { get; set; }
        public virtual int Quantity { get { return 1; } set { } }
        public bool Empty => Data.SequenceEqual(new byte[Data.Length]);

        public string getCardHeader() => (CardID > 0 ? $"Card #: {CardID.ToString("0000")}" : "N/A") + $" - {CardTitle.Replace('\u3000',' ').Trim()}" + Environment.NewLine;
    }
}
