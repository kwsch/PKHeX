using System;
using System.Linq;

namespace PKHeX
{
    public sealed class SAV2 : SaveFile
    {
        public override string BAKName => $"{FileName} [{OT} ({Version}) - {PlayTimeString}].bak";
        public override string Filter => "SAV File|*.sav|All Files|*.*";
        public override string Extension => ".sav";

        public SAV2(byte[] data = null)
        {
            Data = data == null ? new byte[SaveUtil.SIZE_G2RAW_U] : (byte[])data.Clone();
            BAK = (byte[])Data.Clone();
            Exportable = !Data.SequenceEqual(new byte[Data.Length]);

            Version = data == null ? GameVersion.GSC : SaveUtil.getIsG2SAV(Data);
            if (Version == GameVersion.Invalid)
                return;

            Japanese = SaveUtil.getIsG2SAVJ(Data) != GameVersion.Invalid;
            if (Japanese && Data.Length < SaveUtil.SIZE_G2RAW_J)
                Array.Resize(ref Data, SaveUtil.SIZE_G2RAW_J);

            Box = Data.Length;
            Array.Resize(ref Data, Data.Length + SIZE_RESERVED);
            Party = getPartyOffset(0);
            
            Personal = Version == GameVersion.GS ? PersonalTable.GS : PersonalTable.C;

            getSAVOffsets();

            LegalItems = Legal.Pouch_Items_GSC;
            LegalBalls = Legal.Pouch_Ball_GSC;
            LegalKeyItems = Version == GameVersion.C ? Legal.Pouch_Key_C : Legal.Pouch_Key_GS;
            LegalTMHMs = Legal.Pouch_TMHM_GSC;
            HeldItems = Legal.HeldItems_GSC;
            
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

            // Daycare currently undocumented for all Gen II games.

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
                PokemonList2 boxPL = new PokemonList2(Japanese ? PokemonList2.CapacityType.StoredJP : PokemonList2.CapacityType.Stored, Japanese);
                int slot = 0;
                for (int j = 0; j < boxPL.Pokemon.Length; j++)
                {
                    PK2 boxPK = (PK2) getPKM(getData(getBoxOffset(i) + j*SIZE_STORED, SIZE_STORED));
                    if (boxPK.Species > 0)
                        boxPL[slot++] = boxPK;
                }
                if (i < (Japanese ? 6 : 7))
                    boxPL.GetBytes().CopyTo(Data, 0x4000 + i * (SIZE_STOREDBOX + 2));
                else
                    boxPL.GetBytes().CopyTo(Data, 0x6000 + (i - (Japanese ? 6 : 7)) * (SIZE_STOREDBOX + 2));
                if (i == CurrentBox)
                    boxPL.GetBytes().CopyTo(Data, CurrentBoxOffset);
            }

            PokemonList2 partyPL = new PokemonList2(PokemonList2.CapacityType.Party, Japanese);
            int pSlot = 0;
            for (int i = 0; i < 6; i++)
            {
                PK2 partyPK = (PK2)getPKM(getData(getPartyOffset(i), SIZE_STORED));
                if (partyPK.Species > 0)
                    partyPL[pSlot++] = partyPK;
            }
            partyPL.GetBytes().CopyTo(Data, PartyOffset);

            setChecksums();
            if (Version == GameVersion.C && !Japanese)
            {
                Array.Copy(Data, 0x2009, Data, 0x1209, 0xB7A);
            }
            if (Version == GameVersion.C && Japanese)
            {
                Array.Copy(Data, 0x2009, Data, 0x7209, 0xB32);
            }
            if (Version == GameVersion.GS && !Japanese)
            {
                Array.Copy(Data, 0x2009, Data, 0x15C7, 0x222F - 0x2009);
                Array.Copy(Data, 0x222F, Data, 0x3D69, 0x23D9 - 0x222F);
                Array.Copy(Data, 0x23D9, Data, 0x0C6B, 0x2856 - 0x23D9);
                Array.Copy(Data, 0x2856, Data, 0x7E39, 0x288A - 0x2856);
                Array.Copy(Data, 0x288A, Data, 0x10E8, 0x2D69 - 0x288A);
            }
            if (Version == GameVersion.GS && Japanese)
            {
                Array.Copy(Data, 0x2009, Data, 0x7209, 0xC83);
            }
            byte[] outData = new byte[Data.Length - SIZE_RESERVED];
            Array.Copy(Data, outData, outData.Length);
            return outData;
        }

        private void getSAVOffsets()
        {
            OptionsOffset = 0x2000;
            Trainer1 = 0x2009;
            switch (Version)
            {
                case GameVersion.GS:
                    DaylightSavingsOffset = Japanese ? 0x2029 : 0x2042;
                    TimePlayedOffset = Japanese ? 0x2034 : 0x2053;
                    PaletteOffset = Japanese ? 0x204C : 0x206B;
                    MoneyOffset = Japanese ? 0x23BC : 0x23DB;
                    JohtoBadgesOffset = Japanese ? 0x23C5 : 0x23E4;
                    CurrentBoxIndexOffset = Japanese ? 0x2705 : 0x2724;
                    BoxNamesOffset = Japanese ? 0x2708 : 0x2727;
                    PartyOffset = Japanese ? 0x283E : 0x288A;
                    PokedexCaughtOffset = Japanese ? 0x29CE : 0x2A4C;
                    PokedexSeenOffset = Japanese ? 0x29EE : 0x2A6C;
                    CurrentBoxOffset = Japanese ? 0x2D10 : 0x2D6C;
                    GenderOffset = -1; // No gender in GSC
                    break;
                case GameVersion.C:
                    DaylightSavingsOffset = Japanese ? 0x2029 : 0x2042;
                    TimePlayedOffset = Japanese ? 0x2034 : 0x2052;
                    PaletteOffset = Japanese ? 0x204C : 0x206A;
                    MoneyOffset = Japanese ? 0x23BE : 0x23DC;
                    JohtoBadgesOffset = Japanese ? 0x23C7 : 0x23E5;
                    CurrentBoxIndexOffset = Japanese ? 0x26E2 : 0x2700;
                    BoxNamesOffset = Japanese ? 0x26E5 : 0x2703;
                    PartyOffset = Japanese ? 0x281A : 0x2865;
                    PokedexCaughtOffset = Japanese ? 0x29AA : 0x2A27;
                    PokedexSeenOffset = Japanese ? 0x29CA : 0x2A47;
                    CurrentBoxOffset = 0x2D10;
                    GenderOffset = Japanese ? 0x8000 : 0x3E3D;
                    break;
            }
        }

        // Configuration
        public override SaveFile Clone() { return new SAV2(Write(DSV: false)); }

        public override int SIZE_STORED => Japanese ? PKX.SIZE_2JLIST : PKX.SIZE_2ULIST;
        public override int SIZE_PARTY => Japanese ? PKX.SIZE_2JLIST : PKX.SIZE_2ULIST;

        public int SIZE_BOX => BoxSlotCount*SIZE_STORED;

        public int SIZE_STOREDBOX => PokemonList2.GetDataLength(Japanese ? PokemonList2.CapacityType.StoredJP : PokemonList2.CapacityType.Stored, Japanese);

        public override PKM BlankPKM => new PK2(null, null, Japanese);
        public override Type PKMType => typeof(PK2);

        public override int MaxMoveID => 251;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_2;
        public override int MaxAbilityID => 0;
        public override int MaxItemID => 255;
        public override int MaxBallID => 0;
        public override int MaxGameID => 99; // What do I set this to...?
        public override int MaxMoney => 999999;

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
        private int OptionsOffset { get; set; } = int.MinValue;
        private int DaylightSavingsOffset { get; set; } = int.MinValue;
        private int TimePlayedOffset { get; set; } = int.MinValue;
        private int PaletteOffset { get; set; } = int.MinValue;
        private int MoneyOffset { get; set; } = int.MinValue;
        private int JohtoBadgesOffset { get; set; } = int.MinValue;
        private int CurrentBoxIndexOffset { get; set; } = int.MinValue;
        private int BoxNamesOffset { get; set; } = int.MinValue;
        private int PartyOffset { get; set; } = int.MinValue;
        private int PokedexSeenOffset { get; set; } = int.MinValue;
        private int PokedexCaughtOffset { get; set; } = int.MinValue;
        private int CurrentBoxOffset { get; set; } = int.MinValue;
        private int GenderOffset { get; set; } = int.MinValue;

        // Checksums
        protected override void setChecksums()
        {
            ushort accum = 0;
            for (int i = 0x2009; i <= 0x2B3A; i++)
                accum += Data[i];
            if (Version == GameVersion.C && Japanese)
            {
                BitConverter.GetBytes(accum).CopyTo(Data, 0x2D0D);
                BitConverter.GetBytes(accum).CopyTo(Data, 0x7F0D);
            }

            for (int i = 0x2B3B; i <= 0x2B82; i++)
                accum += Data[i];
            if (Version == GameVersion.C && !Japanese)
            { 
                BitConverter.GetBytes(accum).CopyTo(Data, 0x2D0D);
                BitConverter.GetBytes(accum).CopyTo(Data, 0x1F0D);
            }

            for (int i = 0x2B83; i <= 0x2C8B; i++)
                accum += Data[i];
            if (Version == GameVersion.GS && Japanese)
            {
                BitConverter.GetBytes(accum).CopyTo(Data, 0x2D0D);
                BitConverter.GetBytes(accum).CopyTo(Data, 0x7F0D);
            }

            for (int i = 0x2C8C; i <= 0x2D68; i++)
                accum += Data[i];
            if (Version == GameVersion.GS && !Japanese)
            {
                BitConverter.GetBytes(accum).CopyTo(Data, 0x2D69);
                BitConverter.GetBytes(accum).CopyTo(Data, 0x7E6D);
            }

        }
        public override bool ChecksumsValid
        {
            get
            {
                ushort accum = 0;
                for (int i = 0x2009; i <= 0x2B3A; i++)
                    accum += Data[i];
                if (Version == GameVersion.C && Japanese)
                    return accum == BitConverter.ToUInt16(Data, 0x2D0D); // Japanese Crystal

                for (int i = 0x2B3B; i <= 0x2B82; i++)
                    accum += Data[i];
                if (Version == GameVersion.C && !Japanese)
                    return accum == BitConverter.ToUInt16(Data, 0x2D0D); // US Crystal

                for (int i = 0x2B83; i <= 0x2C8B; i++)
                    accum += Data[i];
                if (Version == GameVersion.GS && Japanese)
                    return accum == BitConverter.ToUInt16(Data, 0x2D69); // Japanese Gold/Silver

                for (int i = 0x2C8C; i <= 0x2D68; i++)
                    accum += Data[i];
                if (Version == GameVersion.GS && !Japanese)
                    return accum == BitConverter.ToUInt16(Data, 0x2D69); // US Gold/Silver

                return false;
            }
        }

        private int getChecksum()
        {
            ushort accum = 0;
            for (int i = 0x2009; i <= 0x2B3A; i++)
                accum += Data[i];
            if (Version == GameVersion.C && Japanese)
                return accum; // Japanese Crystal

            for (int i = 0x2B3B; i <= 0x2B82; i++)
                accum += Data[i];
            if (Version == GameVersion.C && !Japanese)
                return accum; // US Crystal

            for (int i = 0x2B83; i <= 0x2C8B; i++)
                accum += Data[i];
            if (Version == GameVersion.GS && Japanese)
                return accum; // Japanese Gold/Silver

            for (int i = 0x2C8C; i <= 0x2D68; i++)
                accum += Data[i];
            if (Version == GameVersion.GS && !Japanese)
                return accum; // US Gold/Silver

            return 0;
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
            get { return Version == GameVersion.C ? Data[GenderOffset] : 0; }
            set
            {
                if (Version != GameVersion.C)
                    return;
                Data[GenderOffset] = (byte) value;
                Data[PaletteOffset] = (byte) value;
            }
        }
        public override ushort TID
        {
            get { return BigEndian.ToUInt16(Data, 0x2009); }
            set { BigEndian.GetBytes(value).CopyTo(Data, 0x2009); }
        }
        public override ushort SID
        {
            get { return 0; }
            set { }
        }
        public override int PlayedHours
        {
            get { return BigEndian.ToUInt16(Data, TimePlayedOffset); }
            set { BigEndian.GetBytes((ushort)value).CopyTo(Data,TimePlayedOffset); }
        }
        public override int PlayedMinutes
        {
            get { return Data[TimePlayedOffset+2]; }
            set { Data[TimePlayedOffset+2] = (byte)value; }
        }
        public override int PlayedSeconds
        {
            get { return Data[TimePlayedOffset + 3]; }
            set { Data[TimePlayedOffset + 3] = (byte)value; }
        }

        public int Badges
        {
            get { return BitConverter.ToUInt16(Data, JohtoBadgesOffset); }
            set { if (value < 0) return; BitConverter.GetBytes(value).CopyTo(Data, JohtoBadgesOffset); }
        }
        private byte Options
        {
            get { return Data[0x2000]; }
            set { Data[0x2000] = value; }
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
                if (new_sound > 0)
                    new_sound = 2; // Stereo
                if (new_sound < 0)
                    new_sound = 0; // Mono
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
            get { return BigEndian.ToUInt32(Data, MoneyOffset) >> 8; }
            set
            {
                BigEndian.GetBytes(value > 999999 ? 999999 : value).Skip(1).ToArray().CopyTo(Data, MoneyOffset);
            }
        }
        public uint Coin
        {
            get
            {
                return BigEndian.ToUInt16(Data, MoneyOffset+7);
            }
            set
            {
               BigEndian.GetBytes(value > 9999 ? 9999 : value).ToArray().CopyTo(Data, MoneyOffset + 7);
            }
        }

        private readonly ushort[] LegalItems, LegalKeyItems, LegalBalls, LegalTMHMs;
        public override InventoryPouch[] Inventory
        {
            get
            {
                InventoryPouch[] pouch;
                if (Version == GameVersion.C)
                {
                    pouch = new[]
                    {
                        new InventoryPouch(InventoryType.TMHMs, LegalTMHMs, 1, Japanese ? 0x23C9 : 0x23E7, 57),
                        new InventoryPouch(InventoryType.Items, LegalItems, 99, Japanese ? 0x2402 : 0x2420, 20),
                        new InventoryPouch(InventoryType.KeyItems, LegalKeyItems, 1, Japanese ? 0x242C : 0x244A, 26),
                        new InventoryPouch(InventoryType.Balls, LegalBalls, 99, Japanese ? 0x2447 : 0x2465, 12),
                        new InventoryPouch(InventoryType.PCItems, LegalItems.Concat(LegalKeyItems).Concat(LegalBalls).Concat(LegalTMHMs).ToArray(), 99, Japanese ? 0x2461 : 0x247F, 50)
                    };
                }
                else
                {
                    pouch = new[]
                    {
                        new InventoryPouch(InventoryType.TMHMs, LegalTMHMs, 1, Japanese ? 0x23C7 : 0x23E6, 57),
                        new InventoryPouch(InventoryType.Items, LegalItems, 99, Japanese ? 0x2400 : 0x241F, 20),
                        new InventoryPouch(InventoryType.KeyItems, LegalKeyItems, 99, Japanese ? 0x242A : 0x2449, 26),
                        new InventoryPouch(InventoryType.Balls, LegalBalls, 99, Japanese ? 0x2445 : 0x2464, 12),
                        new InventoryPouch(InventoryType.PCItems, LegalItems.Concat(LegalKeyItems).Concat(LegalBalls).Concat(LegalTMHMs).ToArray(), 99, Japanese ? 0x245F : 0x247E, 50)
                    };
                }
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
            {
                Data[PokedexCaughtOffset + ofs] |= bitval;
                if (pkm.Species == 201) // Unown
                {
                    // Give all Unown caught to prevent a crash on pokedex view
                    for (int i = 1; i <= 26; i++)
                    {
                        Data[PokedexSeenOffset + 0x1F + i] = (byte)(i);
                    }
                }
            }
        }
    }
}
