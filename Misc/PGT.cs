using System;

namespace PKHeX
{
    public class PCD
    {
        internal static int Size = 0x358;

        public byte[] Data;
        public PCD(byte[] data = null)
        {
            Data = data ?? new byte[Size];

            byte[] giftData = new byte[PGT.Size];
            Array.Copy(Data, 0, giftData, 0, PGT.Size);
            Gift = new PGT(giftData);

            Information = new byte[Data.Length - PGT.Size];
            Array.Copy(Data, PGT.Size, Information, 0, Information.Length);
        }
        public PGT Gift;
        public byte[] Information;
    }
    public class PGT
    {
        internal static int Size = 0x104;

        public byte[] Data;
        public PGT(byte[] data = null)
        {
            Data = data ?? new byte[Size];
            byte[] pkdata = new byte[PK4.SIZE_PARTY];
            Array.Copy(data, 8, pkdata, 0, pkdata.Length);
            // Decrypt PK4
            PKM = new PK4(pkdata);
        }

        public byte CardType { get { return Data[0]; } set { Data[0] = value; } }
        // Unused 0x01
        public byte Slot { get { return Data[2]; } set { Data[2] = value; } }
        public byte Detail { get { return Data[3]; } set { Data[3] = value; } }
        public PK4 PKM;
        public byte[] Unknown;

        public bool IsPokémon { get { return CardType == 1; } set { if (value) CardType = 1; } }
        public bool IsItem { get { return CardType == 2; } set { if (value) CardType = 2; } }
        public bool IsPower { get { return CardType == 3; } set { if (value) CardType = 3; } }

        public PK4 convertToPK4(SAV6 SAV)
        {
            if (!IsPokémon)
                return null;

            return null;
        }
    }
}
