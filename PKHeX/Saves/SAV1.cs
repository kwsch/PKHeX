using System;
using System.Globalization;
using System.Linq;

namespace PKHeX
{
    public sealed class SAV1 : SaveFile
    {
        public override string BAKName => $"{FileName} [{OT} ({Version})" +/* - {LastSavedTime}*/ "].bak";
        public override string Filter => "SAV File|*.sav";
        public override string Extension => ".sav";

        public SAV1(byte[] data = null)
        {
            Data = data == null ? new byte[SaveUtil.SIZE_G1RAW] : (byte[])data.Clone();
            BAK = (byte[])Data.Clone();
            Exportable = !Data.SequenceEqual(new byte[Data.Length]);

            if (data == null)
                Version = GameVersion.RBY;
            else Version = SaveUtil.getIsG1SAV(Data);
            if (Version == GameVersion.Invalid)
                return;

            Japanese = SaveUtil.getIsG1SAVJ(data);

            OFS_PouchHeldItem = (Japanese ? 0x25C4 : 0x25C9);
            OFS_PCItem = (Japanese ? 0x27DC : 0x27E6);
            Personal = PersonalTable.RBY;

            if (!Exportable)
                resetBoxes();
        }

        private const int SIZE_RESERVED = 0x10000; // unpacked box data
        public override byte[] Write(bool DSV)
        {

            setChecksums();
            return Data;
        }


        // Configuration
        public override SaveFile Clone() { return new SAV1(Data); }

        public override int SIZE_STORED => PKX.SIZE_1STORED;
        public override int SIZE_PARTY => PKX.SIZE_1PARTY;
        public override PKM BlankPKM => new PK1();
        protected override Type PKMType => typeof(PK1);

        public override int MaxMoveID => 165;
        public override int MaxSpeciesID => 151;
        public override int MaxAbilityID => 0;
        public override int MaxItemID => 255;
        public override int MaxBallID => 0;
        public override int MaxGameID => 99; // What do I set this to...?

        public override int BoxCount => Japanese ? 8 : 12;
        public override int MaxEV => 65535;
        public override int Generation => 1;
        protected override int GiftCountMax => 0;
        public override int OTLength => Japanese ? 5 : 10;
        public override int NickLength => Japanese ? 5 : 10;

        public override bool HasParty => true;

        // Checksums
        protected override void setChecksums()
        {
            int CHECKSUM_OFS = Japanese ? 0x3594 : 0x3523;
            Data[CHECKSUM_OFS] = 0;
            uint chksum = 0;
            for (int i = 0x2598; i < CHECKSUM_OFS; i++)
            {
                chksum += Data[i];
            }

            chksum = ~chksum;
            chksum &= 0xFF;

            Data[CHECKSUM_OFS] = (byte)chksum;
        }
        public override bool ChecksumsValid
        {
            get
            {
                int CHECKSUM_OFS = Japanese ? 0x3594 : 0x3523;
                Data[CHECKSUM_OFS] = 0;
                uint chksum = 0;
                for (int i = 0x2598; i < CHECKSUM_OFS; i++)
                {
                    chksum += Data[i];
                }

                chksum = ~chksum;
                chksum &= 0xFF;

                return Data[CHECKSUM_OFS] == (byte)chksum;
            }
        }
        public override string ChecksumInfo
        {
            get
            {
                return ChecksumsValid ? "Checksum valid." : "Checksum invalid";
            }
        }

        // Trainer Info
        public override GameVersion Version { get; protected set; }

        private int StringLength => Japanese ? PK1.STRLEN_J : PK1.STRLEN_U;

        public override string OT
        {
            get { return PKX.getG3Str(Data.Skip(0x2598).Take(StringLength).ToArray(), Japanese); }
            set
            {
                byte[] strdata = PKX.setG1Str(value, Japanese);
                if (strdata.Length > StringLength)
                    throw new ArgumentOutOfRangeException("OT Name too long for given save file.");
                strdata.CopyTo(Data, 0x2598);
            }
        }
        public override int Gender
        {
            get { return 0; }
            set { }
        }
        public override ushort TID
        {
            get { return Util.SwapEndianness(BitConverter.ToUInt16(Data, Japanese ? 0x25FB : 0x2605)); }
            set { BitConverter.GetBytes(Util.SwapEndianness(value)).CopyTo(Data, Japanese ? 0x25FB : 0x2605); }
        }
        public override ushort SID
        {
            get { return 0; }
            set { }
        }
        public override int PlayedHours
        {
            get { return BitConverter.ToUInt16(Data, Japanese ? 0x2CA0 : 0x2CED); }
            set { BitConverter.GetBytes((ushort)value).CopyTo(Data, Japanese ? 0x2CA0 : 0x2CED); }
        }
        public override int PlayedMinutes
        {
            get { return Data[Japanese ? 0x2CA2 : 0x2CEF]; }
            set { Data[Japanese ? 0x2CA2 : 0x2CEF] = (byte)value; }
        }
        public override int PlayedSeconds
        {
            get { return Data[Japanese ? 0x2CA3 : 0x2CF0]; }
            set { Data[Japanese ? 0x2CA3 : 0x2CF0] = (byte)value; }
        }

        public override uint Money
        {
            get { return uint.Parse((Util.SwapEndianness(BitConverter.ToUInt32(Data, Japanese ? 0x25EE : 0x25F3)) >> 8).ToString("X6")); }
            set
            {
                BitConverter.GetBytes(Util.SwapEndianness(uint.Parse(value.ToString("000000"), NumberStyles.HexNumber))).Skip(1).ToArray().CopyTo(Data, Japanese ? 0x25EE : 0x25F3);
            }
        }
        public uint Coin
        {
            get
            {
                return uint.Parse(Util.SwapEndianness(BitConverter.ToUInt16(Data, Japanese ? 0x2846 : 0x2850)).ToString("X4"));
            }
            set
            {
                BitConverter.GetBytes(Util.SwapEndianness(ushort.Parse(value.ToString("0000"), NumberStyles.HexNumber))).ToArray().CopyTo(Data, Japanese ? 0x2846 : 0x2850);
            }
        }

        private readonly ushort[] LegalItems;
        public override InventoryPouch[] Inventory
        {
            get
            {
                ushort[] legalItems = LegalItems;
                InventoryPouch[] pouch =
                {
                    new InventoryPouch(InventoryType.Items, legalItems, 99, OFS_PouchHeldItem + 2, 20),
                    new InventoryPouch(InventoryType.Items, legalItems, 99, OFS_PCItem + 2, 50),
                };
                foreach (var p in pouch)
                {
                    p.getPouch(ref Data);
                    int ofs = 0;
                    for (int i = 0; i < p.Count; i++)
                    {
                        while (p.Items[ofs].Count == 0)
                            ofs++;
                        p.Items[i] = p.Items[ofs++];
                    }
                    while (ofs < p.MaxCount)
                        p.Items[ofs++] = new InventoryItem { Count = 0, Index = 0 };
                }
                return pouch;
            }
            set
            {
                foreach (var p in value)
                {
                    int ofs = 0;
                    for (int i = 0; i < p.Count; i++)
                    {
                        while (p.Items[ofs].Count == 0)
                            ofs++;
                        p.Items[i] = p.Items[ofs++];
                    }
                    while (ofs < p.MaxCount)
                        p.Items[ofs++] = new InventoryItem { Count = 0, Index = 0 };
                    p.setPouch(ref Data);
                    Data[p.Offset - 2] = (byte)p.Count;
                }
            }
        }
        public override int getDaycareSlotOffset(int loc = 0, int slot = 0)
        {
            return Daycare;
        }
        public override ulong? getDaycareRNGSeed(int loc)
        {
            return null;
        }
        public override uint? getDaycareEXP(int loc = 0, int slot = 0)
        {
            return null;
        }
        public override bool? getDaycareOccupied(int loc, int slot)
        {
            return null;
        }
        public override void setDaycareEXP(int loc, int slot, uint EXP)
        {

        }
        public override void setDaycareOccupied(int loc, int slot, bool occupied)
        {

        }

        // Storage
        public override int PartyCount
        {
            get { return Data[Japanese ? 0x2ED5 : 0x2F2C]; }
            protected set
            {
                Data[Japanese ? 0x2ED5 : 0x2F2C] = (byte)value; 
            }
        }
        public override int getBoxOffset(int box)
        {
            throw new NotImplementedException();
        }
        public override int getPartyOffset(int slot)
        {
            throw new NotImplementedException();
        }
        public override int CurrentBox
        {
            get { return Data[Japanese ? 0x2842 : 0x284C]; }
            set { Data[Japanese ? 0x2842 : 0x284C] = (byte)value; }
        }
        public override int getBoxWallpaper(int box)
        {
            return 0;
        }
        public override string getBoxName(int box)
        {
            int boxNum = box + 1;
            return $"Box {boxNum}";
        }
        public override void setBoxName(int box, string value)
        {

        }
        public override PKM getPKM(byte[] data)
        {
            if (data.Length == PKX.SIZE_1JLIST || data.Length == PKX.SIZE_1ULIST)
                return new PokemonList1(data, PokemonList1.CapacityType.Single, Japanese)[0];
            return new PK1(data);
        }
        public override byte[] decryptPKM(byte[] data)
        {
            return data;
        }

        protected override void setDex(PKM pkm)
        {
            if (pkm.Species == 0)
                return;
            if (pkm.Species > MaxSpeciesID)
                return;
            if (Version == GameVersion.Unknown)
                return;
            
            int bit = pkm.Species - 1;
            int ofs = bit >> 3;
            byte bitval = (byte)(1 << (bit % 7));

            // Set the Captured Flag
            Data[(Japanese ? 0x259E : 0x25A3) + ofs] |= bitval;

            // Set the Seen Flag
            Data[(Japanese ? 0x25B1 : 0x25B6) + ofs] |= bitval;
        }
    }
}
