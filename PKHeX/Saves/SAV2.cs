using System;
using System.Linq;

namespace PKHeX
{
    public sealed class SAV2 : SaveFile
    {
        public override string BAKName => $"{FileName} [{OT} ({Version})" +/* - {LastSavedTime}*/ "].bak";
        public override string Filter => "SAV File|*.sav";
        public override string Extension => ".sav";

        public SAV2(byte[] data = null)
        {
            Data = data == null ? new byte[SaveUtil.SIZE_G2RAW] : (byte[])data.Clone();
            BAK = (byte[])Data.Clone();
            Exportable = !Data.SequenceEqual(new byte[Data.Length]);

            Version = data == null ? GameVersion.GSC : SaveUtil.getIsG2SAV(Data);
            if (Version == GameVersion.Invalid)
                return;

            Box = Data.Length;
            Array.Resize(ref Data, Data.Length + SIZE_RESERVED);
            Party = getPartyOffset(0);

            Japanese = false;
            Personal = Version == GameVersion.GS ? PersonalTable.GS : PersonalTable.C;

            OptionsOffset = 0x2000;
            Trainer1 = 0x2009;
            switch (Version)
            {
                case GameVersion.GS:
                    DaylightSavingsOffset = Japanese ? -1 : 0x2037;
                    TimePlayedOffset = Japanese ? -1 : 0x2053;
                    PaletteOffset = Japanese ? -1 : 0x206B;
                    MoneyOffset = Japanese ? -1 : 0x23DB;
                    JohtoBadgesOffset = Japanese ? -1 : 0xD57C;
                    CurrentBoxIndexOffset = Japanese ? -1 : 0x2724;
                    BoxNamesOffset = Japanese ? -1 : 0x2727;
                    PartyOffset = Japanese ? 0x283E : 0x288A;
                    PokedexCaughtOffset = Japanese ? -1 : 0x2A4C;
                    PokedexSeenOffset = Japanese ? -1 : 0x2A6C;
                    CurrentBoxOffset = Japanese ? -1 : 0x2D6C;
                    GenderOffset = -1; // No gender in GSC
                    break;
                case GameVersion.C:
                    DaylightSavingsOffset = Japanese ? -1 : 0x2037;
                    TimePlayedOffset = Japanese ? -1 : 0x2054;
                    PaletteOffset = Japanese ? -1 : 0x206A;
                    MoneyOffset = Japanese ? -1 : 0x23DC;
                    JohtoBadgesOffset = Japanese ? -1 : 0x23E5;
                    CurrentBoxIndexOffset = Japanese ? -1 : 0x2700;
                    BoxNamesOffset = Japanese ? 0x2708 : 0x2703;
                    PartyOffset = Japanese ? 0x281A : 0x2865;
                    PokedexCaughtOffset = Japanese ? -1 : 0x2A27;
                    PokedexSeenOffset = Japanese ? -1 : 0x2A47;
                    CurrentBoxOffset = 0x2D10;
                    GenderOffset = Japanese ? -1 : 0x3E3D;
                    break;
            }
            LegalItems = new ushort[] { 3, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 46, 47, 48, 49, 51, 52, 53, 57, 60, 62, 63, 64, 65, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 91, 92, 93, 94, 95, 96, 97, 98, 99, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 117, 118, 119, 121, 122, 123, 124, 125, 126, 131, 132, 138, 139, 140, 143, 144, 146, 150, 151, 152, 156, 158, 163, 168, 169, 170, 172, 173, 174, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189 };
            LegalBalls = new ushort[] { 1, 2, 4, 5, 157, 159, 160, 161, 164, 165, 166, 167};
            HeldItems = new ushort[] {0}.Concat(LegalItems).Concat(LegalBalls).ToArray();
            Array.Sort(HeldItems);
            
            // Stash boxes after the save file's end.
            byte[] TempBox = new byte[SIZE_STOREDBOX];
            for (int i = 0; i < BoxCount; i++)
            {
                if (i < (Japanese ? 6 : 7))
                    Array.Copy(Data, 0x4000 + i * (TempBox.Length + 2), TempBox, 0, TempBox.Length);
                else
                    Array.Copy(Data, 0x6000 + (i - (Japanese ? 6 : 7)) * (TempBox.Length + 2), TempBox, 0, TempBox.Length);
                PokemonList2 PL2 = new PokemonList2(TempBox, Japanese ? PokemonList2.CapacityType.StoredJP : PokemonList2.CapacityType.Stored, Japanese);
                for (int j = 0; j < PL2.Pokemon.Length; j++)
                {
                    if (j < PL2.Count)
                    {
                        byte[] pkDat = new PokemonList2(PL2[j]).GetBytes();
                        pkDat.CopyTo(Data, Data.Length - SIZE_RESERVED + i * SIZE_BOX + j * SIZE_STORED);
                    }
                    else
                    {
                        byte[] pkDat = new byte[PokemonList2.GetDataLength(PokemonList2.CapacityType.Single, Japanese)];
                        pkDat.CopyTo(Data, Data.Length - SIZE_RESERVED + i * SIZE_BOX + j * SIZE_STORED);
                    }
                }
            }

            Array.Copy(Data, CurrentBoxOffset, TempBox, 0, TempBox.Length);
            PokemonList2 curBoxPL = new PokemonList2(TempBox, Japanese ? PokemonList2.CapacityType.StoredJP : PokemonList2.CapacityType.Stored, Japanese);
            for (int i = 0; i < curBoxPL.Pokemon.Length; i++)
            {
                if (i < curBoxPL.Count)
                {
                    byte[] pkDat = new PokemonList2(curBoxPL[i]).GetBytes();
                    pkDat.CopyTo(Data, Data.Length - SIZE_RESERVED + CurrentBox * SIZE_BOX + i * SIZE_STORED);
                }
                else
                {
                    byte[] pkDat = new byte[PokemonList2.GetDataLength(PokemonList2.CapacityType.Single, Japanese)];
                    pkDat.CopyTo(Data, Data.Length - SIZE_RESERVED + CurrentBox * SIZE_BOX + i * SIZE_STORED);
                }
            }

            byte[] TempParty = new byte[PokemonList2.GetDataLength(PokemonList2.CapacityType.Party, Japanese)];
            Array.Copy(Data, PartyOffset, TempParty, 0, TempParty.Length);
            PokemonList2 partyList = new PokemonList2(TempParty, PokemonList2.CapacityType.Party, Japanese);
            for (int i = 0; i < partyList.Pokemon.Length; i++)
            {
                if (i < partyList.Count)
                {
                    byte[] pkDat = new PokemonList2(partyList[i]).GetBytes();
                    pkDat.CopyTo(Data, getPartyOffset(i));
                }
                else
                {
                    byte[] pkDat = new byte[PokemonList2.GetDataLength(PokemonList2.CapacityType.Single, Japanese)];
                    pkDat.CopyTo(Data, getPartyOffset(i));
                }
            }
            /*
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
            */
            // Enable Pokedex editing
            Console.WriteLine(SIZE_STOREDBOX.ToString("X"));
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
        public override SaveFile Clone() { return new SAV2(Data.Take(Data.Length - SIZE_RESERVED).ToArray()); }

        public override int SIZE_STORED => Japanese ? PKX.SIZE_2JLIST : PKX.SIZE_2ULIST;
        public override int SIZE_PARTY => Japanese ? PKX.SIZE_2JLIST : PKX.SIZE_2ULIST;

        public int SIZE_BOX => BoxSlotCount*SIZE_STORED;

        public int SIZE_STOREDBOX => PokemonList2.GetDataLength(Japanese ? PokemonList2.CapacityType.StoredJP : PokemonList2.CapacityType.Stored, Japanese);

        public override PKM BlankPKM => new PK2(null, null, Japanese);
        protected override Type PKMType => typeof(PK2);

        public override int MaxMoveID => 251;
        public override int MaxSpeciesID => 251;
        public override int MaxAbilityID => 0;
        public override int MaxItemID => 255;
        public override int MaxBallID => 0;
        public override int MaxGameID => 99; // What do I set this to...?

        public override int BoxCount => Japanese ? 9 : 14;
        public override int MaxEV => 65535;
        public override int MaxIV => 15;
        public override int Generation => 2;
        protected override int GiftCountMax => 0;
        public override int OTLength => Japanese ? 5 : 7;
        public override int NickLength => Japanese ? 5 : 10;
        public override int BoxSlotCount => Japanese ? 30 : 20;

        public override bool HasParty => true;
        private int StringLength => Japanese ? PK2.STRLEN_J : PK2.STRLEN_U;


        // Offsets
        protected int OptionsOffset { get; set; } = int.MinValue;
        protected int DaylightSavingsOffset { get; set; } = int.MinValue;
        protected int TimePlayedOffset { get; set; } = int.MinValue;
        protected int PaletteOffset { get; set; } = int.MinValue;
        protected int MoneyOffset { get; set; } = int.MinValue;
        protected int JohtoBadgesOffset { get; set; } = int.MinValue;
        protected int CurrentBoxIndexOffset { get; set; } = int.MinValue;
        protected int BoxNamesOffset { get; set; } = int.MinValue;
        protected int PartyOffset { get; set; } = int.MinValue;
        protected int PokedexSeenOffset { get; set; } = int.MinValue;
        protected int PokedexCaughtOffset { get; set; } = int.MinValue;
        protected int CurrentBoxOffset { get; set; } = int.MinValue;
        protected int GenderOffset { get; set; } = int.MinValue;

        // Checksums
        protected override void setChecksums()
        {
            ushort accum = 0;
            for (int i = 0x2009; i <= 0x2B3A; i++)
                accum += Data[i];
            if (Version == GameVersion.C && Japanese)
                BitConverter.GetBytes(Util.SwapEndianness(accum)).CopyTo(Data, 0x2D0D);

            for (int i = 0x2B3B; i <= 0x2B82; i++)
                accum += Data[i];
            if (Version == GameVersion.C && !Japanese)
                BitConverter.GetBytes(Util.SwapEndianness(accum)).CopyTo(Data, 0x2D0D);

            /* TODO: Find Japanese GS Checksum region */

            for (int i = 0x2B83; i <= 0x2D68; i++)
                accum += Data[i];
            if (Version == GameVersion.GS && !Japanese)
                BitConverter.GetBytes(Util.SwapEndianness(accum)).CopyTo(Data, 0x2D69);
            
        }
        public override bool ChecksumsValid
        {
            get
            {
                ushort accum = 0;
                for (int i = 0x2009; i <= 0x2B3A; i++)
                    accum += Data[i];
                if (Version == GameVersion.C && Japanese)
                    return accum == Util.SwapEndianness(BitConverter.ToUInt16(Data, 0x2D0D)); // Japanese Crystal

                for (int i = 0x2B3B; i <= 0x2B82; i++)
                    accum += Data[i];
                if (Version == GameVersion.C && !Japanese)
                    return accum == Util.SwapEndianness(BitConverter.ToUInt16(Data, 0x2D0D)); // Japanese Crystal

                /* TODO: Find Japanese GS Checksum region */

                for (int i = 0x2B83; i <= 0x2D68; i++)
                    accum += Data[i];
                if (Version == GameVersion.GS && !Japanese)
                    return accum == Util.SwapEndianness(BitConverter.ToUInt16(Data, 0x2D69)); // US Gold/Silver

                return false;
            }
        }
        public override string ChecksumInfo => ChecksumsValid ? "Checksum valid." : "Checksum invalid";

        // Trainer Info
        public override GameVersion Version { get; protected set; }

        public override string OT
        {
            get { return PKX.getG1Str(Data.Skip(0x200B).Take(StringLength).ToArray(), Japanese); }
            set
            {
                byte[] strdata = PKX.setG1Str(value, Japanese);
                if (strdata.Length > StringLength)
                    throw new ArgumentException("OT Name too long for given save file.");
                strdata.CopyTo(Data, 0x200B);
            }
        }
        public override int Gender
        {
            get { return 0; }
            set { }
        }
        public override ushort TID
        {
            get { return Util.SwapEndianness(BitConverter.ToUInt16(Data, 0x2009)); }
            set { BitConverter.GetBytes(Util.SwapEndianness(value)).CopyTo(Data, 0x2009); }
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
            get { return uint.Parse((Util.SwapEndianness(BitConverter.ToUInt32(Data, Japanese ? 0x25EE : 0x25F3)) >> 8).ToString("X6")); }
            set
            {
                BitConverter.GetBytes(Util.SwapEndianness(Convert.ToUInt32(value.ToString("000000"), 16))).Skip(1).ToArray().CopyTo(Data, Japanese ? 0x25EE : 0x25F3);
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
                BitConverter.GetBytes(Util.SwapEndianness(Convert.ToUInt16(value.ToString("0000"), 16))).ToArray().CopyTo(Data, Japanese ? 0x2846 : 0x2850);
            }
        }

        private readonly ushort[] LegalItems, LegalKeyItems, LegalBalls, LegalTMHMs, LegalBerries;
        public override InventoryPouch[] Inventory
        {
            get
            {
                ushort[] legalItems = LegalItems;
                InventoryPouch[] pouch =
                {
                    new InventoryPouch(InventoryType.Items, legalItems, 99, Japanese ? 0x25C4 : 0x25C9, 20),
                    new InventoryPouch(InventoryType.MailItems, legalItems, 99, Japanese ? 0x27DC : 0x27E6, 50)
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
        public override ulong? getDaycareRNGSeed(int loc)
        {
            return null;
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
            get { return Data[PartyOffset]; }
            protected set
            {
                Data[PartyOffset] = (byte)value; 
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
            get { return Data[CurrentBoxIndexOffset] & 0x7F; }
            set { Data[CurrentBoxIndexOffset] = (byte)((Data[Japanese ? 0x2842 : 0x284C] & 0x80) | (value & 0x7F)); }
        }
        public override int getBoxWallpaper(int box)
        {
            return 0;
        }
        public override string getBoxName(int box)
        {
            return PKX.getG1Str(Data.Skip(BoxNamesOffset + box*9).Take(9).ToArray(), Japanese);
        }
        public override void setBoxName(int box, string value)
        {
            // Don't allow for custom box names
        }

        public override PKM getPKM(byte[] data)
        {
            if (data.Length == SIZE_STORED)
                return new PokemonList2(data, PokemonList2.CapacityType.Single, Japanese)[0];
            return new PK2(data);
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
            return (Data[PokedexSeenOffset + ofs] & bitval) != 0;
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
            return (Data[PokedexCaughtOffset + ofs] & bitval) != 0;
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
            Data[PokedexSeenOffset + ofs] &= (byte)(~bitval);
            if (seen)
                Data[PokedexSeenOffset + ofs] |= bitval;
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
            Data[PokedexCaughtOffset + ofs] &= (byte)(~bitval);
            if (caught)
                Data[PokedexCaughtOffset + ofs] |= bitval;
        }
    }
}
