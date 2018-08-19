using System;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 1 <see cref="SaveFile"/> object.
    /// </summary>
    public sealed class SAV1 : SaveFile
    {
        protected override string BAKText => $"{OT} ({Version}) - {PlayTimeString}";
        public override string Filter => "SAV File|*.sav|All Files|*.*";
        public override string Extension => ".sav";

        public override string[] PKMExtensions => PKM.Extensions.Where(f =>
        {
            int gen = f.Last() - 0x30;
            return 1 <= gen && gen <= 2;
        }).ToArray();

        public SAV1(byte[] data = null, GameVersion versionOverride = GameVersion.Any)
        {
            Data = data ?? new byte[SaveUtil.SIZE_G1RAW];
            BAK = (byte[])Data.Clone();
            Exportable = !IsRangeEmpty(0, Data.Length);

            if (data == null)
                Version = GameVersion.RBY;
            else if (versionOverride != GameVersion.Any)
                Version = versionOverride;
            else Version = SaveUtil.GetIsG1SAV(Data);
            if (Version == GameVersion.Invalid)
                return;

            Japanese = SaveUtil.GetIsG1SAVJ(Data);
            Offsets = Japanese ? SAV1Offsets.JPN : SAV1Offsets.INT;
            if (Starter != 0)
                Version = Yellow ? GameVersion.YW : GameVersion.RB;

            Box = Data.Length;
            Array.Resize(ref Data, Data.Length + SIZE_RESERVED);
            Party = GetPartyOffset(0);

            Personal = PersonalTable.Y;

            // Stash boxes after the save file's end.
            int stored = SIZE_STOREDBOX;
            int baseDest = Data.Length - SIZE_RESERVED;
            var capacity = Japanese ? PokeListType.StoredJP : PokeListType.Stored;
            for (int i = 0; i < BoxCount; i++)
            {
                int ofs = GetBoxRawDataOffset(i);
                var box = GetData(ofs, stored);
                var boxDest = baseDest + (i * SIZE_BOX);
                var boxPL = new PokeList1(box, capacity, Japanese);
                for (int j = 0; j < boxPL.Pokemon.Length; j++)
                {
                    var dest = boxDest + (j * SIZE_STORED);
                    var pkDat = (j < boxPL.Count)
                        ? new PokeList1(boxPL[j]).Write()
                        : new byte[PokeList1.GetDataLength(PokeListType.Single, Japanese)];
                    pkDat.CopyTo(Data, dest);
                }
            }

            var current = GetData(Offsets.CurrentBox, SIZE_STOREDBOX);
            var curBoxPL = new PokeList1(current, capacity, Japanese);
            for (int i = 0; i < curBoxPL.Pokemon.Length; i++)
            {
                var dest = Data.Length - SIZE_RESERVED + (CurrentBox * SIZE_BOX) + (i * SIZE_STORED);
                var pkDat = i < curBoxPL.Count
                    ? new PokeList1(curBoxPL[i]).Write()
                    : new byte[PokeList1.GetDataLength(PokeListType.Single, Japanese)];
                pkDat.CopyTo(Data, dest);
            }

            var party = GetData(Offsets.Party, SIZE_STOREDPARTY);
            var partyPL = new PokeList1(party, PokeListType.Party, Japanese);
            for (int i = 0; i < partyPL.Pokemon.Length; i++)
            {
                var dest = GetPartyOffset(i);
                var pkDat = i < partyPL.Count
                    ? new PokeList1(partyPL[i]).Write()
                    : new byte[PokeList1.GetDataLength(PokeListType.Single, Japanese)];
                pkDat.CopyTo(Data, dest);
            }

            byte[] rawDC = new byte[0x38];
            Array.Copy(Data, Offsets.Daycare, rawDC, 0, rawDC.Length);
            byte[] TempDaycare = new byte[PokeList1.GetDataLength(PokeListType.Single, Japanese)];
            TempDaycare[0] = rawDC[0];
            Array.Copy(rawDC, 1, TempDaycare, 2 + 1 + PKX.SIZE_1PARTY + StringLength, StringLength);
            Array.Copy(rawDC, 1 + StringLength, TempDaycare, 2 + 1 + PKX.SIZE_1PARTY, StringLength);
            Array.Copy(rawDC, 1 + (2 * StringLength), TempDaycare, 2 + 1, PKX.SIZE_1STORED);
            PokeList1 daycareList = new PokeList1(TempDaycare, PokeListType.Single, Japanese);
            daycareList.Write().CopyTo(Data, GetPartyOffset(7));
            Daycare = GetPartyOffset(7);

            EventFlag = Offsets.EventFlag;

            // Enable Pokedex editing
            PokeDex = 0;

            if (!Exportable)
                ClearBoxes();
        }

        private readonly SAV1Offsets Offsets;

        // Event Flags
        protected override int EventFlagMax => EventFlag > 0 ? 0xA00 : int.MinValue; // 320 * 8
        protected override int EventConstMax => 0;

        private const int SIZE_RESERVED = 0x8000; // unpacked box data

        protected override byte[] Write(bool DSV)
        {
            var capacity = Japanese ? PokeListType.StoredJP : PokeListType.Stored;
            for (int i = 0; i < BoxCount; i++)
            {
                var boxPL = new PokeList1(capacity, Japanese);
                int slot = 0;
                for (int j = 0; j < boxPL.Pokemon.Length; j++)
                {
                    PK1 boxPK = (PK1)GetPKM(GetData(GetBoxOffset(i) + (j * SIZE_STORED), SIZE_STORED));
                    if (boxPK.Species > 0)
                        boxPL[slot++] = boxPK;
                }

                // copy to box location
                var boxdata = boxPL.Write();
                int ofs = GetBoxRawDataOffset(i);
                SetData(boxdata, ofs);

                // copy to active loc if current box
                if (i == CurrentBox)
                    SetData(boxdata, Offsets.CurrentBox);
            }

            var partyPL = new PokeList1(PokeListType.Party, Japanese);
            int pSlot = 0;
            for (int i = 0; i < 6; i++)
            {
                PK1 partyPK = (PK1)GetPKM(GetData(GetPartyOffset(i), SIZE_STORED));
                if (partyPK.Species > 0)
                    partyPL[pSlot++] = partyPK;
            }
            partyPL.Write().CopyTo(Data, Offsets.Party);

            // Daycare is read-only, but in case it ever becomes editable, copy it back in.
            byte[] rawDC = GetData(GetDaycareSlotOffset(loc: 0, slot: 0), SIZE_STORED);
            byte[] dc = new byte[1 + (2 * StringLength) + PKX.SIZE_1STORED];
            dc[0] = rawDC[0];
            Array.Copy(rawDC, 2 + 1 + PKX.SIZE_1PARTY + StringLength, dc, 1, StringLength);
            Array.Copy(rawDC, 2 + 1 + PKX.SIZE_1PARTY, dc, 1 + StringLength, StringLength);
            Array.Copy(rawDC, 2 + 1, dc, 1 + (2 * StringLength), PKX.SIZE_1STORED);
            dc.CopyTo(Data, Offsets.Daycare);

            SetChecksums();
            byte[] outData = new byte[Data.Length - SIZE_RESERVED];
            Array.Copy(Data, outData, outData.Length);
            return outData;
        }

        private int GetBoxRawDataOffset(int i)
        {
            if (i < BoxCount / 2)
                return 0x4000 + (i * SIZE_STOREDBOX);
            return 0x6000 + ((i - (BoxCount / 2)) * SIZE_STOREDBOX);
        }

        // Configuration
        public override SaveFile Clone() { return new SAV1(Write(DSV: false)); }

        public override int SIZE_STORED => Japanese ? PKX.SIZE_1JLIST : PKX.SIZE_1ULIST;
        protected override int SIZE_PARTY => Japanese ? PKX.SIZE_1JLIST : PKX.SIZE_1ULIST;
        private int SIZE_BOX => BoxSlotCount*SIZE_STORED;
        private int SIZE_STOREDBOX => PokeList1.GetDataLength(Japanese ? PokeListType.StoredJP : PokeListType.Stored, Japanese);
        private int SIZE_STOREDPARTY => PokeList1.GetDataLength(PokeListType.Party, Japanese);

        public override PKM BlankPKM => new PK1(null, null, Japanese);
        public override Type PKMType => typeof(PK1);

        public override int MaxMoveID => Legal.MaxMoveID_1;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_1;
        public override int MaxAbilityID => Legal.MaxAbilityID_1;
        public override int MaxItemID => Legal.MaxItemID_1;
        public override int MaxBallID => 0; // unused
        public override int MaxGameID => 99; // unused
        public override int MaxMoney => 999999;
        public override int MaxCoins => 9999;

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

        public override bool IsPKMPresent(int Offset) => PKX.IsPKMPresentGB(Data, Offset);

        // Checksums
        protected override void SetChecksums() => Data[Offsets.ChecksumOfs] = GetRBYChecksum(Offsets.ChecksumOfs);
        public override bool ChecksumsValid => Data[Offsets.ChecksumOfs] == GetRBYChecksum(Offsets.ChecksumOfs);
        public override string ChecksumInfo => ChecksumsValid ? "Checksum valid." : "Checksum invalid";

        private const int CHECKSUM_START = 0x2598;

        private byte GetRBYChecksum(int end)
        {
            byte chksum = 0;
            for (int i = CHECKSUM_START; i < end; i++)
                chksum += Data[i];
            chksum ^= 0xFF;
            return chksum;
        }

        // Trainer Info
        public override GameVersion Version { get; protected set; }

        public override string OT
        {
            get => GetString(0x2598, OTLength);
            set => SetString(value, OTLength).CopyTo(Data, 0x2598);
        }

        public byte[] OT_Trash { get => GetData(0x2598, StringLength); set { if (value?.Length == StringLength) SetData(value, 0x2598); } }

        public override int Gender
        {
            get => 0;
            set { }
        }

        public override int TID
        {
            get => BigEndian.ToUInt16(Data, Offsets.TID);
            set => BigEndian.GetBytes((ushort)value).CopyTo(Data, Offsets.TID);
        }

        public override int SID { get => 0; set { } }

        public bool Yellow => Starter == 0x54; // Pikachu
        public int Starter => Data[Offsets.Starter];

        public byte PikaFriendship
        {
            get => Data[Offsets.PikaFriendship];
            set => Data[Offsets.PikaFriendship] = value;
        }

        public override string PlayTimeString => !PlayedMaximum ? base.PlayTimeString : $"{base.PlayTimeString} {SaveUtil.CRC16_CCITT(Data):X4}";

        public override int PlayedHours
        {
            get => Data[Offsets.PlayTime + 0];
            set
            {
                if (value >= byte.MaxValue) // Set 255:00:00.00 and flag
                {
                    PlayedMaximum = true;
                    value = byte.MaxValue;
                    PlayedMinutes = PlayedSeconds = PlayedFrames = 0;
                }
                Data[Offsets.PlayTime + 0] = (byte) value;
            }
        }

        public bool PlayedMaximum
        {
            get => Data[Offsets.PlayTime + 1] != 0;
            set => Data[Offsets.PlayTime + 1] = (byte)(value ? 1 : 0);
        }

        public override int PlayedMinutes
        {
            get => Data[Offsets.PlayTime + 2];
            set => Data[Offsets.PlayTime + 2] = (byte)value;
        }

        public override int PlayedSeconds
        {
            get => Data[Offsets.PlayTime + 3];
            set => Data[Offsets.PlayTime + 3] = (byte)value;
        }

        public int PlayedFrames
        {
            get => Data[Offsets.PlayTime + 4];
            set => Data[Offsets.PlayTime + 4] = (byte)value;
        }

        public int Badges
        {
            get => Data[Offsets.Badges];
            set { if (value < 0) return; Data[Offsets.Badges] = (byte)value; }
        }

        private byte Options
        {
            get => Data[Offsets.Options];
            set => Data[Offsets.Options] = value;
        }

        public bool BattleEffects
        {
            get => (Options & 0x80) == 0;
            set => Options = (byte)((Options & 0x7F) | (value ? 0 : 0x80));
        }

        public bool BattleStyleSwitch
        {
            get => (Options & 0x40) == 0;
            set => Options = (byte)((Options & 0xBF) | (value ? 0 : 0x40));
        }

        public int Sound
        {
            get => (Options & 0x30) >> 4;
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
            get => Options & 0x7;
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
            get => (uint)BigEndian.BCDToInt32(Data, Offsets.Money, 3);
            set
            {
                value = (uint)Math.Min(value, MaxMoney);
                BigEndian.Int32ToBCD((int)value, 3).CopyTo(Data, Offsets.Money);
            }
        }

        public uint Coin
        {
            get => (uint)BigEndian.BCDToInt32(Data, Offsets.Coin, 2);
            set
            {
                value = (ushort)Math.Min(value, MaxCoins);
                BigEndian.Int32ToBCD((int)value, 2).CopyTo(Data, Offsets.Coin);
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
                    new InventoryPouchGB(InventoryType.Items, legalItems, 99, Offsets.Items, 20),
                    new InventoryPouchGB(InventoryType.PCItems, legalItems, 99, Offsets.PCItems, 50)
                };
                foreach (var p in pouch)
                {
                    p.GetPouch(Data);
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
                        p.Items[ofs++] = new InventoryItem();
                    p.SetPouch(Data);
                }
            }
        }

        public override int GetDaycareSlotOffset(int loc, int slot)
        {
            return Daycare;
        }

        public override uint? GetDaycareEXP(int loc, int slot)
        {
            return null;
        }

        public override bool? IsDaycareOccupied(int loc, int slot)
        {
            if (loc == 0 && slot == 0)
                return Data[Offsets.Daycare] == 0x01;
            else
                return null;
        }

        public override void SetDaycareEXP(int loc, int slot, uint EXP)
        {

        }

        public override void SetDaycareOccupied(int loc, int slot, bool occupied)
        {

        }

        // Storage
        public override int PartyCount
        {
            get => Data[Offsets.Party];
            protected set => Data[Offsets.Party] = (byte)value;
        }

        public override int GetBoxOffset(int box)
        {
            return Data.Length - SIZE_RESERVED + (box * SIZE_BOX);
        }

        public override int GetPartyOffset(int slot)
        {
            return Data.Length - SIZE_RESERVED + (BoxCount * SIZE_BOX) + (slot * SIZE_STORED);
        }

        public override int CurrentBox
        {
            get => Data[Offsets.CurrentBoxIndex] & 0x7F;
            set => Data[Offsets.CurrentBoxIndex] = (byte)((Data[Offsets.CurrentBoxIndex] & 0x80) | (value & 0x7F));
        }

        public override string GetBoxName(int box)
        {
            return $"BOX {box + 1}";
        }

        public override void SetBoxName(int box, string value)
        {
            // Don't allow for custom box names
        }

        public override PKM GetPKM(byte[] data)
        {
            if (data.Length == SIZE_STORED)
                return new PokeList1(data, PokeListType.Single, Japanese)[0];
            return new PK1(data);
        }

        public override byte[] DecryptPKM(byte[] data)
        {
            return data;
        }

        // Pokédex
        protected override void SetDex(PKM pkm)
        {
            int species = pkm.Species;
            if (!CanSetDex(species))
                return;

            SetCaught(pkm.Species, true);
            SetSeen(pkm.Species, true);
        }

        private bool CanSetDex(int species)
        {
            if (species <= 0)
                return false;
            if (species > MaxSpeciesID)
                return false;
            if (Version == GameVersion.Invalid)
                return false;
            return true;
        }

        public override void SetSeen(int species, bool seen)
        {
            int bit = species - 1;
            int ofs = bit >> 3;
            SetFlag(Offsets.DexSeen + ofs, bit & 7, seen);
        }

        public override void SetCaught(int species, bool caught)
        {
            int bit = species - 1;
            int ofs = bit >> 3;
            SetFlag(Offsets.DexCaught + ofs, bit & 7, caught);
        }

        public override bool GetSeen(int species)
        {
            int bit = species - 1;
            int ofs = bit >> 3;
            return GetFlag(Offsets.DexSeen + ofs, bit & 7);
        }

        public override bool GetCaught(int species)
        {
            int bit = species - 1;
            int ofs = bit >> 3;
            return GetFlag(Offsets.DexCaught + ofs, bit & 7);
        }

        public override void SetStoredSlot(PKM pkm, int offset, bool? trade = null, bool? dex = null)
        {
            // pkm that have never been boxed have yet to save the 'current level' for box indication
            // set this value at this time
            ((PK1)pkm).Stat_LevelBox = pkm.CurrentLevel;
            base.SetStoredSlot(pkm, offset, trade, dex);
        }

        private const int SpawnFlagCount = 0xF0;

        public bool[] EventSpawnFlags
        {
            get
            {
                // RB uses 0xE4 (0xE8) flags, Yellow uses 0xF0 flags. Just grab 0xF0
                bool[] data = new bool[SpawnFlagCount];
                for (int i = 0; i < data.Length; i++)
                    data[i] = GetFlag(Offsets.ObjectSpawnFlags + i >> 3, i & 7);
                return data;
            }
            set
            {
                if (value?.Length != SpawnFlagCount)
                    return;
                for (int i = 0; i < value.Length; i++)
                    SetFlag(Offsets.ObjectSpawnFlags + i >> 3, i & 7, value[i]);
            }
        }

        public override string GetString(int Offset, int Length) => StringConverter.GetString1(Data, Offset, Length, Japanese);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter.SetString1(value, maxLength, Japanese, PadToSize, PadWith);
        }
    }
}
