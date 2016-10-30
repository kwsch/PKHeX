using System;
using System.Linq;

namespace PKHeX
{
    public sealed class SAV1 : SaveFile
    {
        public override string BAKName => $"{FileName} [{OT} ({Version}) - {PlayTimeString}].bak";
        public override string Filter => "SAV File|*.sav|All Files|*.*";
        public override string Extension => ".sav";

        public SAV1(byte[] data = null)
        {
            Data = data == null ? new byte[SaveUtil.SIZE_G1RAW] : (byte[])data.Clone();
            BAK = (byte[])Data.Clone();
            Exportable = !Data.SequenceEqual(new byte[Data.Length]);

            Version = data == null ? GameVersion.RBY : SaveUtil.getIsG1SAV(Data);
            if (Version == GameVersion.Invalid)
                return;

            Box = Data.Length;
            Array.Resize(ref Data, Data.Length + SIZE_RESERVED);
            Party = getPartyOffset(0);

            Japanese = SaveUtil.getIsG1SAVJ(data);
            Personal = PersonalTable.RBY;

            // Stash boxes after the save file's end.
            byte[] TempBox = new byte[SIZE_STOREDBOX];
            for (int i = 0; i < BoxCount; i++)
            {
                if (i < BoxCount / 2)
                    Array.Copy(Data, 0x4000 + i * TempBox.Length, TempBox, 0, TempBox.Length);
                else
                    Array.Copy(Data, 0x6000 + (i - BoxCount / 2) * TempBox.Length, TempBox, 0, TempBox.Length);
                PokemonList1 PL1 = new PokemonList1(TempBox, Japanese ? PokemonList1.CapacityType.StoredJP : PokemonList1.CapacityType.Stored, Japanese);
                for (int j = 0; j < PL1.Pokemon.Length; j++)
                {
                    if (j < PL1.Count)
                    {
                        byte[] pkDat = new PokemonList1(PL1[j]).GetBytes();
                        pkDat.CopyTo(Data, Data.Length - SIZE_RESERVED + i * SIZE_BOX + j * SIZE_STORED);
                    }
                    else
                    {
                        byte[] pkDat = new byte[PokemonList1.GetDataLength(PokemonList1.CapacityType.Single, Japanese)];
                        pkDat.CopyTo(Data, Data.Length - SIZE_RESERVED + i * SIZE_BOX + j * SIZE_STORED);
                    }
                }
            }

            Array.Copy(Data, Japanese ? 0x302D : 0x30C0, TempBox, 0, TempBox.Length);
            PokemonList1 curBoxPL = new PokemonList1(TempBox, Japanese ? PokemonList1.CapacityType.StoredJP : PokemonList1.CapacityType.Stored, Japanese);
            for (int i = 0; i < curBoxPL.Pokemon.Length; i++)
            {
                if (i < curBoxPL.Count)
                {
                    byte[] pkDat = new PokemonList1(curBoxPL[i]).GetBytes();
                    pkDat.CopyTo(Data, Data.Length - SIZE_RESERVED + CurrentBox * SIZE_BOX + i * SIZE_STORED);
                }
                else
                {
                    byte[] pkDat = new byte[PokemonList1.GetDataLength(PokemonList1.CapacityType.Single, Japanese)];
                    pkDat.CopyTo(Data, Data.Length - SIZE_RESERVED + CurrentBox * SIZE_BOX + i * SIZE_STORED);
                }
            }

            byte[] TempParty = new byte[PokemonList1.GetDataLength(PokemonList1.CapacityType.Party, Japanese)];
            Array.Copy(Data, Japanese ? 0x2ED5 : 0x2F2C, TempParty, 0, TempParty.Length);
            PokemonList1 partyList = new PokemonList1(TempParty, PokemonList1.CapacityType.Party, Japanese);
            for (int i = 0; i < partyList.Pokemon.Length; i++)
            {
                if (i < partyList.Count)
                {
                    byte[] pkDat = new PokemonList1(partyList[i]).GetBytes();
                    pkDat.CopyTo(Data, getPartyOffset(i));
                }
                else
                {
                    byte[] pkDat = new byte[PokemonList1.GetDataLength(PokemonList1.CapacityType.Single, Japanese)];
                    pkDat.CopyTo(Data, getPartyOffset(i));
                }
            }

            byte[] rawDC = new byte[0x38];
            Array.Copy(Data, Japanese ? 0x2CA7 : 0x2CF4, rawDC, 0, rawDC.Length);
            byte[] TempDaycare = new byte[PokemonList1.GetDataLength(PokemonList1.CapacityType.Single, Japanese)];
            TempDaycare[0] = rawDC[0];
            Array.Copy(rawDC, 1, TempDaycare, 2 + 1 + PKX.SIZE_1PARTY + StringLength, StringLength);
            Array.Copy(rawDC, 1 + StringLength, TempDaycare, 2 + 1 + PKX.SIZE_1PARTY, StringLength);
            Array.Copy(rawDC, 1 + 2 * StringLength, TempDaycare, 2 + 1, PKX.SIZE_1STORED);
            PokemonList1 daycareList = new PokemonList1(TempDaycare, PokemonList1.CapacityType.Single, Japanese);
            daycareList.GetBytes().CopyTo(Data, getPartyOffset(7));
            Daycare = getPartyOffset(7); 

            // Enable Pokedex editing
            PokeDex = 0;

            if (!Exportable)
                resetBoxes();
        }

        private const int SIZE_RESERVED = 0x8000; // unpacked box data
        public override byte[] Write(bool DSV)
        {
            for (int i = 0; i < BoxCount; i++)
            {
                PokemonList1 boxPL = new PokemonList1(Japanese ? PokemonList1.CapacityType.StoredJP : PokemonList1.CapacityType.Stored, Japanese);
                int slot = 0;
                for (int j = 0; j < boxPL.Pokemon.Length; j++)
                {
                    PK1 boxPK = (PK1) getPKM(getData(getBoxOffset(i) + j*SIZE_STORED, SIZE_STORED));
                    if (boxPK.Species > 0)
                        boxPL[slot++] = boxPK;
                }
                if (i < BoxCount / 2)
                    boxPL.GetBytes().CopyTo(Data, 0x4000 + i * SIZE_STOREDBOX);
                else
                    boxPL.GetBytes().CopyTo(Data, 0x6000 + (i - BoxCount / 2) * SIZE_STOREDBOX);
                if (i == CurrentBox)
                    boxPL.GetBytes().CopyTo(Data, Japanese ? 0x302D : 0x30C0);
            }

            PokemonList1 partyPL = new PokemonList1(PokemonList1.CapacityType.Party, Japanese);
            int pSlot = 0;
            for (int i = 0; i < 6; i++)
            {
                PK1 partyPK = (PK1)getPKM(getData(getPartyOffset(i), SIZE_STORED));
                if (partyPK.Species > 0)
                    partyPL[pSlot++] = partyPK;
            }
            partyPL.GetBytes().CopyTo(Data, Japanese ? 0x2ED5 : 0x2F2C);

            // Daycare is read-only, but in case it ever becomes editable, copy it back in.
            byte[] rawDC = getData(getDaycareSlotOffset(loc: 0, slot: 0), SIZE_STORED);
            byte[] dc = new byte[1 + 2*StringLength + PKX.SIZE_1STORED];
            dc[0] = rawDC[0];
            Array.Copy(rawDC, 2 + 1 + PKX.SIZE_1PARTY + StringLength, dc, 1, StringLength);
            Array.Copy(rawDC, 2 + 1 + PKX.SIZE_1PARTY, dc, 1 + StringLength, StringLength);
            Array.Copy(rawDC, 2 + 1, dc, 1 + 2*StringLength, PKX.SIZE_1STORED);
            dc.CopyTo(Data, Japanese ? 0x2CA7 : 0x2CF4);

            setChecksums();
            byte[] outData = new byte[Data.Length - SIZE_RESERVED];
            Array.Copy(Data, outData, outData.Length);
            return outData;
        }


        // Configuration
        public override SaveFile Clone() { return new SAV1(Write(DSV: false)); }

        public override int SIZE_STORED => Japanese ? PKX.SIZE_1JLIST : PKX.SIZE_1ULIST;
        public override int SIZE_PARTY => Japanese ? PKX.SIZE_1JLIST : PKX.SIZE_1ULIST;

        public int SIZE_BOX => BoxSlotCount*SIZE_STORED;

        public int SIZE_STOREDBOX => PokemonList1.GetDataLength(Japanese ? PokemonList1.CapacityType.StoredJP : PokemonList1.CapacityType.Stored, Japanese);

        public override PKM BlankPKM => new PK1(null, null, Japanese);
        public override Type PKMType => typeof(PK1);

        public override int MaxMoveID => 165;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_1;
        public override int MaxAbilityID => 0;
        public override int MaxItemID => 255;
        public override int MaxBallID => 0;
        public override int MaxGameID => 99; // What do I set this to...?
        public override int MaxMoney => 999999;

        public override int BoxCount => Japanese ? 8 : 12;
        public override int MaxEV => 65535;
        public override int MaxIV => 15;
        public override int Generation => 1;
        protected override int GiftCountMax => 0;
        public override int OTLength => Japanese ? 5 : 7;
        public override int NickLength => Japanese ? 5 : 10;
        public override int BoxSlotCount => Japanese ? 30 : 20;

        public override bool HasParty => true;
        private int StringLength => Japanese ? PK1.STRLEN_J : PK1.STRLEN_U;

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
                byte temp = Data[CHECKSUM_OFS]; // cache current chk
                setChecksums(); // chksum is recalculated (after being set to 0 to perform check)
                byte chk = Data[CHECKSUM_OFS]; // correct checksum
                Data[CHECKSUM_OFS] = temp; // restore old chk
                return temp == chk;
            }
        }
        public override string ChecksumInfo => ChecksumsValid ? "Checksum valid." : "Checksum invalid";

        // Trainer Info
        public override GameVersion Version { get; protected set; }

        public override string OT
        {
            get { return PKX.getG1Str(Data.Skip(0x2598).Take(StringLength).ToArray(), Japanese); }
            set
            {
                byte[] strdata = PKX.setG1Str(value, Japanese);
                if (strdata.Length > StringLength)
                    throw new ArgumentException("OT Name too long for given save file.");
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
            get { return BigEndian.ToUInt16(Data, Japanese ? 0x25FB : 0x2605); }
            set { BigEndian.GetBytes(value).CopyTo(Data, Japanese ? 0x25FB : 0x2605); }
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

        public int Badges
        {
            get { return Data[Japanese ? 0x25F8 : 0x2602]; }
            set { if (value < 0) return; Data[Japanese ? 0x25F8 : 0x2602] = (byte)value; }
        }
        private byte Options
        {
            get { return Data[Japanese ? 0x25F7 : 0x2601]; }
            set { Data[Japanese ? 0x25F7 : 0x2601] = value; }
        }
        public bool BattleEffects
        {
            get { return (Options & 0x80) == 0; }
            set { Options = (byte)((Options & 0x7F) | (value ? 0 : 0x80)); }
        }
        public bool BattleStyleSwitch
        {
            get { return (Options & 0x40) == 0; }
            set { Options = (byte)((Options & 0xBF) | (value ? 0 : 0x40)); }
        }
        public int Sound
        {
            get { return (Options & 0x30) >> 4; }
            set
            {
                var new_sound = value;
                if (new_sound > 3)
                    new_sound = 3;
                if (new_sound < 0)
                    new_sound = 0;
                Options = (byte)((Options & 0xCF) | (new_sound << 4));
            }
        }
        public int TextSpeed
        {
            get { return Options & 0x7; }
            set
            {
                var new_speed = value;
                if (new_speed > 7)
                    new_speed = 7;
                if (new_speed < 0)
                    new_speed = 0;
                Options = (byte)((Options & 0xF8) | new_speed);
            }
        }
        public override uint Money
        {
            get { return uint.Parse((BigEndian.ToUInt32(Data, Japanese ? 0x25EE : 0x25F3) >> 8).ToString("X6")); }
            set
            {
                BigEndian.GetBytes(Convert.ToUInt32(value.ToString("000000"), 16)).Skip(1).ToArray().CopyTo(Data, Japanese ? 0x25EE : 0x25F3);
            }
        }
        public uint Coin
        {
            get
            {
                return uint.Parse(BigEndian.ToUInt16(Data, Japanese ? 0x2846 : 0x2850).ToString("X4"));
            }
            set
            {
                BigEndian.GetBytes(Convert.ToUInt16(value.ToString("0000"), 16)).ToArray().CopyTo(Data, Japanese ? 0x2846 : 0x2850);
            }
        }

        private readonly ushort[] LegalItems = Legal.Pouch_Items_RBY;
        public override InventoryPouch[] Inventory
        {
            get
            {
                ushort[] legalItems = LegalItems;
                InventoryPouch[] pouch =
                {
                    new InventoryPouch(InventoryType.Items, legalItems, 99, Japanese ? 0x25C4 : 0x25C9, 20),
                    new InventoryPouch(InventoryType.PCItems, legalItems, 99, Japanese ? 0x27DC : 0x27E6, 50)
                };
                foreach (var p in pouch)
                {
                    p.getPouchG1(ref Data);
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
                    while (ofs < p.Items.Length)
                        p.Items[ofs++] = new InventoryItem { Count = 0, Index = 0 };
                    p.setPouchG1(ref Data);
                }
            }
        }
        public override int getDaycareSlotOffset(int loc, int slot)
        {
            return Daycare;
        }
        public override uint? getDaycareEXP(int loc, int slot)
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
            return Data.Length - SIZE_RESERVED + box * SIZE_BOX;
        }
        public override int getPartyOffset(int slot)
        {
            return Data.Length - SIZE_RESERVED + BoxCount * SIZE_BOX + slot * SIZE_STORED;
        }
        public override int CurrentBox
        {
            get { return Data[Japanese ? 0x2842 : 0x284C] & 0x7F; }
            set { Data[Japanese ? 0x2842 : 0x284C] = (byte)((Data[Japanese ? 0x2842 : 0x284C] & 0x80) | (value & 0x7F)); }
        }
        public override string getBoxName(int box)
        {
            return $"BOX {box + 1}";
        }
        public override void setBoxName(int box, string value)
        {
            // Don't allow for custom box names
        }

        public override PKM getPKM(byte[] data)
        {
            if (data.Length == SIZE_STORED)
                return new PokemonList1(data, PokemonList1.CapacityType.Single, Japanese)[0];
            return new PK1(data);
        }
        public override byte[] decryptPKM(byte[] data)
        {
            return data;
        }

        // Pokédex
        public override bool getSeen(PKM pkm)
        {
            if (pkm.Species == 0)
                return false;
            if (pkm.Species > MaxSpeciesID)
                return false;
            if (Version == GameVersion.Unknown)
                return false;

            int bit = pkm.Species - 1;
            int ofs = bit >> 3;
            byte bitval = (byte)(1 << (bit & 7));
            // Get the Seen Flag
            return (Data[(Japanese ? 0x25B1 : 0x25B6) + ofs] & bitval) != 0;
        }
        public override bool getCaught(PKM pkm)
        {
            if (pkm.Species == 0)
                return false;
            if (pkm.Species > MaxSpeciesID)
                return false;
            if (Version == GameVersion.Unknown)
                return false;

            int bit = pkm.Species - 1;
            int ofs = bit >> 3;
            byte bitval = (byte)(1 << (bit & 7));
            // Get the Caught Flag
            return (Data[(Japanese ? 0x259E : 0x25A3) + ofs] & bitval) != 0;
        }
        protected internal override void setSeen(PKM pkm, bool seen = true)
        {
            if (pkm.Species == 0)
                return;
            if (pkm.Species > MaxSpeciesID)
                return;
            if (Version == GameVersion.Unknown)
                return;

            int bit = pkm.Species - 1;
            int ofs = bit >> 3;
            byte bitval = (byte)(1 << (bit & 7));
            // Set the Seen Flag
            Data[(Japanese ? 0x25B1 : 0x25B6) + ofs] &= (byte)(~bitval);
            if (seen)
                Data[(Japanese ? 0x25B1 : 0x25B6) + ofs] |= bitval;
        }
        protected internal override void setCaught(PKM pkm, bool caught = true)
        {
            if (pkm.Species == 0)
                return;
            if (pkm.Species > MaxSpeciesID)
                return;
            if (Version == GameVersion.Unknown)
                return;

            int bit = pkm.Species - 1;
            int ofs = bit >> 3;
            byte bitval = (byte)(1 << (bit & 7));
            // Set the Captured Flag
            Data[(Japanese ? 0x259E : 0x25A3) + ofs] &= (byte)(~bitval);
            if (caught)
                Data[(Japanese ? 0x259E : 0x25A3) + ofs] |= bitval;
        }
    }
}
