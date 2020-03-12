using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 4 abstract <see cref="SaveFile"/> object.
    /// </summary>
    /// <remarks>
    /// Storage data is stored in one contiguous block, and the remaining data is stored in another block.
    /// </remarks>
    public abstract class SAV4 : SaveFile
    {
        protected override string BAKText => $"{OT} ({Version}) - {PlayTimeString}";
        public override string Filter => (Footer.Length > 0 ? "DeSmuME DSV|*.dsv|" : string.Empty) + "SAV File|*.sav|All Files|*.*";
        public override string Extension => ".sav";

        // Blocks & Offsets
        private readonly int GeneralBlockPosition; // Small Block
        private readonly int StorageBlockPosition; // Big Block
        private const int PartitionSize = 0x40000;

        // SaveData is chunked into two pieces.
        protected readonly byte[] Storage;
        public readonly byte[] General;
        protected override byte[] BoxBuffer => Storage;
        protected override byte[] PartyBuffer => General;

        protected abstract int StorageSize { get; }
        protected abstract int GeneralSize { get; }
        protected abstract int StorageStart { get; }

        /// <inheritdoc />
        public override bool GetFlag(int offset, int bitIndex) => FlagUtil.GetFlag(General, offset, bitIndex);

        /// <inheritdoc />
        public override void SetFlag(int offset, int bitIndex, bool value) => FlagUtil.SetFlag(General, offset, bitIndex, value);

        protected SAV4()
        {
            Storage = new byte[StorageSize];
            General = new byte[GeneralSize];
            ClearBoxes();
        }

        protected SAV4(byte[] data) : base(data)
        {
            var gSize = GeneralSize;
            var sSize = StorageSize;
            var sStart = StorageStart;
            GeneralBlockPosition = GetActiveBlock(Data, 0, gSize);
            StorageBlockPosition = GetActiveBlock(Data, sStart, sSize);

            var gbo = (GeneralBlockPosition == 0 ? 0 : PartitionSize);
            var sbo = (StorageBlockPosition == 0 ? 0 : PartitionSize) + sStart;
            General = GetData(gbo, gSize);
            Storage = GetData(sbo, sSize);
        }

        // Configuration
        public override SaveFile Clone()
        {
            var sav = CloneInternal();
            SetData(sav.General, General, 0);
            SetData(sav.Storage, Storage, 0);
            sav.Footer = (byte[])Footer.Clone();
            return sav;
        }

        protected abstract SAV4 CloneInternal();

        public override void CopyChangesFrom(SaveFile sav)
        {
            SetData(sav.Data, 0);
            var s4 = (SAV4)sav;
            SetData(General, s4.General, 0);
            SetData(Storage, s4.Storage, 0);
        }

        public override int SIZE_STORED => PokeCrypto.SIZE_4STORED;
        protected override int SIZE_PARTY => PokeCrypto.SIZE_4PARTY;
        public override PKM BlankPKM => new PK4();
        public override Type PKMType => typeof(PK4);

        public override int BoxCount => 18;
        public override int MaxEV => 255;
        public override int Generation => 4;
        protected override int EventFlagMax => 0xB60; // 2912
        protected override int EventConstMax => (EventFlag - EventConst) >> 1;
        protected override int GiftCountMax => 11;
        public override int OTLength => 7;
        public override int NickLength => 10;
        public override int MaxMoney => 999999;
        public override int MaxCoins => 50_000;

        public override int MaxMoveID => Legal.MaxMoveID_4;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_4;
        public override int MaxItemID => Version == GameVersion.HGSS ? Legal.MaxItemID_4_HGSS : Version == GameVersion.Pt ? Legal.MaxItemID_4_Pt : Legal.MaxItemID_4_DP;
        public override int MaxAbilityID => Legal.MaxAbilityID_4;
        public override int MaxBallID => Legal.MaxBallID_4;
        public override int MaxGameID => Legal.MaxGameID_4; // Colo/XD

        public bool HGSS => Version == GameVersion.HGSS;
        public bool DP => Version == GameVersion.DP;

        // Checksums
        protected abstract int FooterSize { get; }
        private ushort CalcBlockChecksum(byte[] data) => Checksums.CRC16_CCITT(data, 0, data.Length - FooterSize);
        private static ushort GetBlockChecksumSaved(byte[] data) => BitConverter.ToUInt16(data, data.Length - 2);
        private bool GetBlockChecksumValid(byte[] data) => CalcBlockChecksum(data) == GetBlockChecksumSaved(data);

        protected override void SetChecksums()
        {
            BitConverter.GetBytes(CalcBlockChecksum(General)).CopyTo(General, General.Length - 2);
            BitConverter.GetBytes(CalcBlockChecksum(Storage)).CopyTo(Storage, Storage.Length - 2);

            // Write blocks back
            General.CopyTo(Data, GeneralBlockPosition * PartitionSize);
            Storage.CopyTo(Data, (StorageBlockPosition * PartitionSize) + StorageStart);
        }

        public override bool ChecksumsValid
        {
            get
            {
                if (!GetBlockChecksumValid(General))
                    return false;
                if (!GetBlockChecksumValid(Storage))
                    return false;

                return true;
            }
        }

        public override string ChecksumInfo
        {
            get
            {
                var list = new List<string>();
                if (!GetBlockChecksumValid(General))
                    list.Add("Small block checksum is invalid");
                if (!GetBlockChecksumValid(Storage))
                    list.Add("Large block checksum is invalid");

                return list.Count != 0 ? string.Join(Environment.NewLine, list) : "Checksums are valid.";
            }
        }

        private static int GetActiveBlock(byte[] data, int begin, int length)
        {
            // Check to see if the save is initialized completely
            // if the block is not initialized, fall back to the other save.
            var start = begin;
            if (data.IsRangeAll((byte)0, start, 10) || data.IsRangeAll((byte)0xFF, start, 10))
                return 1;
            start += PartitionSize; // check other save
            if (data.IsRangeAll((byte)0, start, 10) || data.IsRangeAll((byte)0xFF, start, 10))
                return 0;

            // Fall back to highest value save counter
            return GetActiveBlockViaCounter(data, begin, length);
        }

        private static int GetActiveBlockViaCounter(byte[] data, int begin, int length)
        {
            int ofs = GetBlockSaveCounterOffset(begin, length);
            var block0 = BitConverter.ToUInt16(data, ofs);
            var block1 = BitConverter.ToUInt16(data, ofs + PartitionSize);
            bool first = block0 >= block1;
            return first ? 0 : 1;
        }

        private static int GetBlockSaveCounterOffset(int start, int length) => start + length - 0x10;

        protected int WondercardFlags = int.MinValue;
        protected int AdventureInfo = int.MinValue;
        protected int Seal = int.MinValue;
        protected int Trainer1;
        public int GTS { get; protected set; } = int.MinValue;

        // Storage
        public override int PartyCount
        {
            get => General[Party - 4];
            protected set => General[Party - 4] = (byte)value;
        }

        public override int GetPartyOffset(int slot) => Party + (SIZE_PARTY * slot);

        // Trainer Info
        public override string OT
        {
            get => GetString(General, Trainer1, 16);
            set => SetString(value, OTLength).CopyTo(General, Trainer1);
        }

        public override int TID
        {
            get => BitConverter.ToUInt16(General, Trainer1 + 0x10);
            set => BitConverter.GetBytes((ushort)value).CopyTo(General, Trainer1 + 0x10);
        }

        public override int SID
        {
            get => BitConverter.ToUInt16(General, Trainer1 + 0x12);
            set => BitConverter.GetBytes((ushort)value).CopyTo(General, Trainer1 + 0x12);
        }

        public override uint Money
        {
            get => BitConverter.ToUInt32(General, Trainer1 + 0x14);
            set => BitConverter.GetBytes(value).CopyTo(General, Trainer1 + 0x14);
        }

        public override int Gender
        {
            get => General[Trainer1 + 0x18];
            set => General[Trainer1 + 0x18] = (byte)value;
        }

        public override int Language
        {
            get => General[Trainer1 + 0x19];
            set => General[Trainer1 + 0x19] = (byte)value;
        }

        public int Badges
        {
            get => General[Trainer1 + 0x1A];
            set { if (value < 0) return; General[Trainer1 + 0x1A] = (byte)value; }
        }

        public int Sprite
        {
            get => General[Trainer1 + 0x1B];
            set { if (value < 0) return; General[Trainer1 + 0x1B] = (byte)value; }
        }

        public uint Coin
        {
            get => BitConverter.ToUInt16(General, Trainer1 + 0x20);
            set => BitConverter.GetBytes((ushort)value).CopyTo(General, Trainer1 + 0x20);
        }

        public override int PlayedHours
        {
            get => BitConverter.ToUInt16(General, Trainer1 + 0x22);
            set => BitConverter.GetBytes((ushort)value).CopyTo(General, Trainer1 + 0x22);
        }

        public override int PlayedMinutes
        {
            get => General[Trainer1 + 0x24];
            set => General[Trainer1 + 0x24] = (byte)value;
        }

        public override int PlayedSeconds
        {
            get => General[Trainer1 + 0x25];
            set => General[Trainer1 + 0x25] = (byte)value;
        }

        public int M
        {
            get
            {
                int ofs = 0;
                switch (Version)
                {
                    case GameVersion.DP:   ofs = 0x1238; break;
                    case GameVersion.Pt:   ofs = 0x1280; break;
                    case GameVersion.HGSS: ofs = 0x1234; break;
                }
                return BitConverter.ToUInt16(General, ofs);
            }
            set
            {
                int ofs = 0;
                switch (Version)
                {
                    case GameVersion.DP:   ofs = 0x1238; break;
                    case GameVersion.Pt:   ofs = 0x1280; break;
                    case GameVersion.HGSS: ofs = 0x1234; break;
                }
                BitConverter.GetBytes((ushort)value).CopyTo(General, ofs);
            }
        }

        public int X
        {
            get
            {
                int ofs = 0;
                switch (Version)
                {
                    case GameVersion.DP:   ofs = 0x1240; break;
                    case GameVersion.Pt:   ofs = 0x1288; break;
                    case GameVersion.HGSS: ofs = 0x123C; break;
                }
                return BitConverter.ToUInt16(General, ofs);
            }
            set
            {
                int ofs = 0;
                switch (Version)
                {
                    case GameVersion.DP:   ofs = 0x1240; break;
                    case GameVersion.Pt:   ofs = 0x1288; break;
                    case GameVersion.HGSS: ofs = 0x123C; break;
                }
                BitConverter.GetBytes((ushort)(X2 = value)).CopyTo(General, ofs);
            }
        }

        public int Y
        {
            get
            {
                int ofs = 0;
                switch (Version)
                {
                    case GameVersion.DP:   ofs = 0x1244; break;
                    case GameVersion.Pt:   ofs = 0x128C; break;
                    case GameVersion.HGSS: ofs = 0x1240; break;
                }
                return BitConverter.ToUInt16(General, ofs);
            }
            set
            {
                int ofs = 0;
                switch (Version)
                {
                    case GameVersion.DP:   ofs = 0x1244; break;
                    case GameVersion.Pt:   ofs = 0x128C; break;
                    case GameVersion.HGSS: ofs = 0x1240; break;
                }
                BitConverter.GetBytes((ushort)(Y2 = value)).CopyTo(General, ofs);
            }
        }

        public int X2
        {
            get
            {
                int ofs = 0;
                switch (Version)
                {
                    case GameVersion.DP:   ofs = 0x25FA; break;
                    case GameVersion.Pt:   ofs = 0x287E; break;
                    case GameVersion.HGSS: ofs = 0x236E; break;
                }
                return BitConverter.ToUInt16(General, ofs);
            }
            set
            {
                int ofs = 0;
                switch (Version)
                {
                    case GameVersion.DP:   ofs = 0x25FA; break;
                    case GameVersion.Pt:   ofs = 0x287E; break;
                    case GameVersion.HGSS: ofs = 0x236E; break;
                }
                BitConverter.GetBytes((ushort)value).CopyTo(General, ofs);
            }
        }

        public int Y2
        {
            get
            {
                int ofs = 0;
                switch (Version)
                {
                    case GameVersion.DP:   ofs = 0x25FE; break;
                    case GameVersion.Pt:   ofs = 0x2882; break;
                    case GameVersion.HGSS: ofs = 0x2372; break;
                }
                return BitConverter.ToUInt16(General, ofs);
            }
            set
            {
                int ofs = 0;
                switch (Version)
                {
                    case GameVersion.DP:   ofs = 0x25FE; break;
                    case GameVersion.Pt:   ofs = 0x2882; break;
                    case GameVersion.HGSS: ofs = 0x2372; break;
                }
                BitConverter.GetBytes((ushort)value).CopyTo(General, ofs);
            }
        }

        public int Z
        {
            get
            {
                int ofs = 0;
                switch (Version)
                {
                    case GameVersion.DP:   ofs = 0x2602; break;
                    case GameVersion.Pt:   ofs = 0x2886; break;
                    case GameVersion.HGSS: ofs = 0x2376; break;
                }
                return BitConverter.ToUInt16(General, ofs);
            }
            set
            {
                int ofs = 0;
                switch (Version)
                {
                    case GameVersion.DP:   ofs = 0x2602; break;
                    case GameVersion.Pt:   ofs = 0x2886; break;
                    case GameVersion.HGSS: ofs = 0x2376; break;
                }
                BitConverter.GetBytes((ushort)value).CopyTo(General, ofs);
            }
        }

        public override uint SecondsToStart { get => BitConverter.ToUInt32(General, AdventureInfo + 0x34); set => BitConverter.GetBytes(value).CopyTo(General, AdventureInfo + 0x34); }
        public override uint SecondsToFame { get => BitConverter.ToUInt32(General, AdventureInfo + 0x3C); set => BitConverter.GetBytes(value).CopyTo(General, AdventureInfo + 0x3C); }

        protected override PKM GetPKM(byte[] data) => new PK4(data);
        protected override byte[] DecryptPKM(byte[] data) => PokeCrypto.DecryptArray45(data);

        protected override void SetPKM(PKM pkm)
        {
            var pk4 = (PK4)pkm;
            // Apply to this Save File
            DateTime Date = DateTime.Now;
            if (pk4.Trade(OT, TID, SID, Gender, Date.Day, Date.Month, Date.Year))
                pkm.RefreshChecksum();
        }

        // Daycare
        public override int GetDaycareSlotOffset(int loc, int slot) => DaycareOffset + (slot * SIZE_PARTY);

        public override uint? GetDaycareEXP(int loc, int slot)
        {
            int ofs = DaycareOffset + ((slot+1)*SIZE_PARTY) - 4;
            return BitConverter.ToUInt32(General, ofs);
        }

        public override bool? IsDaycareOccupied(int loc, int slot) => null; // todo

        public override void SetDaycareEXP(int loc, int slot, uint EXP)
        {
            int ofs = DaycareOffset + ((slot+1)*SIZE_PARTY) - 4;
            BitConverter.GetBytes(EXP).CopyTo(General, ofs);
        }

        public override void SetDaycareOccupied(int loc, int slot, bool occupied)
        {
            // todo
        }

        // Mystery Gift
        private bool MysteryGiftActive { get => (General[72] & 1) == 1; set => General[72] = (byte)((General[72] & 0xFE) | (value ? 1 : 0)); }

        private static bool IsMysteryGiftAvailable(DataMysteryGift[] value)
        {
            for (int i = 0; i < 8; i++) // 8 PGT
            {
                if (value[i] is PGT g && g.CardType != 0)
                    return true;
            }
            for (int i = 8; i < 11; i++) // 3 PCD
            {
                if (value[i] is PCD d && d.Gift.CardType != 0)
                    return true;
            }
            return false;
        }

        private int[] MatchMysteryGifts(DataMysteryGift[] value)
        {
            int[] cardMatch = new int[8];
            for (int i = 0; i < 8; i++)
            {
                if (!(value[i] is PGT pgt))
                    continue;

                if (pgt.CardType == 0) // empty
                {
                    cardMatch[i] = pgt.Slot = 0;
                    continue;
                }

                cardMatch[i] = pgt.Slot = 3;
                for (byte j = 0; j < 3; j++)
                {
                    if (!(value[8 + j] is PCD pcd))
                        continue;

                    // Check if data matches (except Slot @ 0x02)
                    if (!pcd.GiftEquals(pgt))
                        continue;

                    if (this is SAV4HGSS)
                        j++; // hgss 0,1,2; dppt 1,2,3
                    cardMatch[i] = pgt.Slot = j;
                    break;
                }
            }
            return cardMatch;
        }

        public override MysteryGiftAlbum GiftAlbum
        {
            get
            {
                var album = new MysteryGiftAlbum(MysteryGiftCards, MysteryGiftReceivedFlags);
                album.Flags[2047] = false;
                return album;
            }
            set
            {
                bool available = IsMysteryGiftAvailable(value.Gifts);
                if (available && !MysteryGiftActive)
                    MysteryGiftActive = true;
                value.Flags[2047] = available;

                // Check encryption for each gift (decrypted wc4 sneaking in)
                foreach (var g in value.Gifts)
                {
                    if (g is PGT pgt)
                    {
                        pgt.VerifyPKEncryption();
                    }
                    else if (g is PCD pcd)
                    {
                        var dg = pcd.Gift;
                        if (dg.VerifyPKEncryption())
                            pcd.Gift = dg; // set encrypted gift back to PCD.
                    }
                }

                MysteryGiftReceivedFlags = value.Flags;
                MysteryGiftCards = value.Gifts;
            }
        }

        protected override bool[] MysteryGiftReceivedFlags
        {
            get
            {
                bool[] result = new bool[GiftFlagMax];
                for (int i = 0; i < result.Length; i++)
                    result[i] = (General[WondercardFlags + (i >> 3)] >> (i & 7) & 0x1) == 1;
                return result;
            }
            set
            {
                if (GiftFlagMax != value.Length)
                    return;

                byte[] data = new byte[value.Length / 8];
                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i])
                        data[i >> 3] |= (byte)(1 << (i & 7));
                }

                SetData(General, data, WondercardFlags);
            }
        }

        protected override DataMysteryGift[] MysteryGiftCards
        {
            get
            {
                DataMysteryGift[] cards = new DataMysteryGift[8 + 3];
                for (int i = 0; i < 8; i++) // 8 PGT
                    cards[i] = new PGT(General.Slice(WondercardData + (i * PGT.Size), PGT.Size));
                for (int i = 8; i < 11; i++) // 3 PCD
                    cards[i] = new PCD(General.Slice(WondercardData + (8 * PGT.Size) + ((i-8) * PCD.Size), PCD.Size));
                return cards;
            }
            set
            {
                var Matches = MatchMysteryGifts(value); // automatically applied
                if (Matches.Length == 0)
                    return;

                for (int i = 0; i < 8; i++) // 8 PGT
                {
                    if (value[i] is PGT)
                        SetData(General, value[i].Data, WondercardData + (i *PGT.Size));
                }
                for (int i = 8; i < 11; i++) // 3 PCD
                {
                    if (value[i] is PCD)
                        SetData(General, value[i].Data, WondercardData + (8 *PGT.Size) + ((i - 8)*PCD.Size));
                }
            }
        }

        protected override void SetDex(PKM pkm)
        {
            if (pkm.Species == 0)
                return;
            if (pkm.Species > MaxSpeciesID)
                return;

            const int brSize = 0x40;
            int bit = pkm.Species - 1;
            byte mask = (byte)(1 << (bit & 7));
            int ofs = PokeDex + (bit >> 3) + 0x4;

            /* 4 BitRegions with 0x40*8 bits
            * Region 0: Caught (Captured/Owned) flags
            * Region 1: Seen flags
            * Region 2/3: Toggle for gender display
            * 4 possible states: 00, 01, 10, 11
            * 00 - 1Seen: Male Only
            * 01 - 2Seen: Male First, Female Second
            * 10 - 2Seen: Female First, Male Second
            * 11 - 1Seen: Female Only
            * (bit1 ^ bit2) + 1 = forms in dex
            * bit2 = male/female shown first toggle
            */

            // Set the Species Owned Flag
            General[ofs + (brSize * 0)] |= mask;

            // Check if already Seen
            if ((General[ofs + (brSize * 1)] & mask) == 0) // Not seen
            {
                General[ofs + (brSize * 1)] |= mask; // Set seen
                int gr = pkm.PersonalInfo.Gender;
                switch (gr)
                {
                    case 255: // Genderless
                    case 0: // Male Only
                        General[ofs + (brSize * 2)] &= (byte)~mask; // unset
                        General[ofs + (brSize * 3)] &= (byte)~mask; // unset
                        break;
                    case 254: // Female Only
                        General[ofs + (brSize * 2)] |= mask;
                        General[ofs + (brSize * 3)] |= mask;
                        break;
                    default: // Male or Female
                        bool m = (General[ofs + (brSize * 2)] & mask) != 0;
                        bool f = (General[ofs + (brSize * 3)] & mask) != 0;
                        if (m || f) // bit already set?
                            break;
                        int gender = pkm.Gender & 1;
                        General[ofs + (brSize * 2)] &= (byte)~mask; // unset
                        General[ofs + (brSize * 3)] &= (byte)~mask; // unset
                        gender ^= 1; // Set OTHER gender seen bit so it appears second
                        General[ofs + (brSize * (2 + gender))] |= mask;
                        break;
                }
            }

            int FormOffset1 = PokeDex + 4 + (brSize * 4) + 4;
            var forms = GetForms(pkm.Species);
            if (forms.Length > 0)
            {
                if (pkm.Species == (int)Species.Unown) // Unown
                {
                    for (int i = 0; i < 0x1C; i++)
                    {
                        byte val = General[FormOffset1 + 4 + i];
                        if (val == pkm.AltForm)
                            break; // already set
                        if (val != 0xFF)
                            continue; // keep searching

                        General[FormOffset1 + 4 + i] = (byte)pkm.AltForm;
                        break; // form now set
                    }
                }
                else if (pkm.Species == (int)Species.Pichu && HGSS) // Pichu (HGSS Only)
                {
                    int form = pkm.AltForm == 1 ? 2 : pkm.Gender;
                    CheckInsertForm(ref forms, form);
                    SetForms(pkm.Species, forms);
                }
                else
                {
                    CheckInsertForm(ref forms, pkm.AltForm);
                    SetForms(pkm.Species, forms);
                }
            }

            int dpl = 1 + Array.IndexOf(DPLangSpecies, pkm.Species);
            if (DP && dpl <= 0)
                return;

            // Set the Language
            int PokeDexLanguageFlags = FormOffset1 + (HGSS ? 0x3C : 0x20);
            int lang = GetGen4LanguageBitIndex(pkm.Language);
            General[PokeDexLanguageFlags + (DP ? dpl : pkm.Species)] |= (byte)(1 << lang);
        }

        private static readonly int[] DPLangSpecies = { 23, 25, 54, 77, 120, 129, 202, 214, 215, 216, 228, 278, 287, 315 };

        private static int GetGen4LanguageBitIndex(int lang)
        {
            lang--;
            switch (lang) // invert ITA/GER
            {
                case 3: return 4;
                case 4: return 3;
            }
            if (lang > 5)
                return 0; // no KOR+
            return lang < 0 ? 1 : lang; // default English
        }

        public override bool GetCaught(int species)
        {
            int bit = species - 1;
            int bd = bit >> 3; // div8
            int bm = bit & 7; // mod8
            int ofs = PokeDex // Raw Offset
                      + 0x4; // Magic
            return (1 << bm & General[ofs + bd]) != 0;
        }

        public override bool GetSeen(int species)
        {
            const int brSize = 0x40;

            int bit = species - 1;
            int bd = bit >> 3; // div8
            int bm = bit & 7; // mod8
            int ofs = PokeDex // Raw Offset
                      + 0x4; // Magic

            return (1 << bm & General[ofs + bd + (brSize * 1)]) != 0;
        }

        public int[] GetForms(int species)
        {
            const int brSize = 0x40;
            if (species == (int)Species.Deoxys)
            {
                uint val = (uint) (General[PokeDex + 0x4 + (1 * brSize) - 1] | General[PokeDex + 0x4 + (2 * brSize) - 1] << 8);
                return GetDexFormValues(val, 4, 4);
            }

            int FormOffset1 = PokeDex + 4 + (4 * brSize) + 4;
            switch (species)
            {
                case (int)Species.Shellos: // Shellos
                    return GetDexFormValues(General[FormOffset1 + 0], 1, 2);
                case (int)Species.Gastrodon: // Gastrodon
                    return GetDexFormValues(General[FormOffset1 + 1], 1, 2);
                case (int)Species.Burmy: // Burmy
                    return GetDexFormValues(General[FormOffset1 + 2], 2, 3);
                case (int)Species.Wormadam: // Wormadam
                    return GetDexFormValues(General[FormOffset1 + 3], 2, 3);
                case (int)Species.Unown: // Unown
                    return General.Slice(FormOffset1 + 4, 0x1C).Select(i => (int)i).ToArray();
            }
            if (DP)
                return Array.Empty<int>();

            int PokeDexLanguageFlags = FormOffset1 + (HGSS ? 0x3C : 0x20);
            int FormOffset2 = PokeDexLanguageFlags + 0x1F4;
            return species switch
            {
                (int)Species.Rotom => GetDexFormValues(BitConverter.ToUInt32(General, FormOffset2), 3, 6),
                (int)Species.Shaymin => GetDexFormValues(General[FormOffset2 + 4], 1, 2),
                (int)Species.Giratina => GetDexFormValues(General[FormOffset2 + 5], 1, 2),
                (int)Species.Pichu when HGSS => GetDexFormValues(General[FormOffset2 + 6], 2, 3),
                _ => Array.Empty<int>()
            };
        }

        public void SetForms(int spec, int[] forms)
        {
            const int brSize = 0x40;
            switch (spec)
            {
                case (int)Species.Deoxys: // Deoxys
                    uint newval = SetDexFormValues(forms, 4, 4);
                    General[PokeDex + 0x4 + (1 * brSize) - 1] = (byte) (newval & 0xFF);
                    General[PokeDex + 0x4 + (2 * brSize) - 1] = (byte) ((newval >> 8) & 0xFF);
                    break;
            }

            int FormOffset1 = PokeDex + 4 + (4 * brSize) + 4;
            switch (spec)
            {
                case (int)Species.Shellos: // Shellos
                    General[FormOffset1 + 0] = (byte)SetDexFormValues(forms, 1, 2);
                    return;
                case (int)Species.Gastrodon: // Gastrodon
                    General[FormOffset1 + 1] = (byte)SetDexFormValues(forms, 1, 2);
                    return;
                case (int)Species.Burmy: // Burmy
                    General[FormOffset1 + 2] = (byte)SetDexFormValues(forms, 2, 3);
                    return;
                case (int)Species.Wormadam: // Wormadam
                    General[FormOffset1 + 3] = (byte)SetDexFormValues(forms, 2, 3);
                    return;
                case (int)Species.Unown: // Unown
                    int ofs = FormOffset1 + 4;
                    int len = forms.Length;
                    Array.Resize(ref forms, 0x1C);
                    for (int i = len; i < forms.Length; i++)
                        forms[i] = 0xFF;
                    Array.Copy(forms.Select(b => (byte)b).ToArray(), 0, General, ofs, forms.Length);
                    return;
            }

            if (DP)
                return;

            int PokeDexLanguageFlags = FormOffset1 + (HGSS ? 0x3C : 0x20);
            int FormOffset2 = PokeDexLanguageFlags + 0x1F4;
            switch (spec)
            {
                case (int)Species.Rotom: // Rotom
                    BitConverter.GetBytes(SetDexFormValues(forms, 3, 6)).CopyTo(General, FormOffset2);
                    return;
                case (int)Species.Shaymin: // Shaymin
                    General[FormOffset2 + 4] = (byte)SetDexFormValues(forms, 1, 2);
                    return;
                case (int)Species.Giratina: // Giratina
                    General[FormOffset2 + 5] = (byte)SetDexFormValues(forms, 1, 2);
                    return;
                case (int)Species.Pichu when HGSS: // Pichu
                    General[FormOffset2 + 6] = (byte)SetDexFormValues(forms, 2, 3);
                    return;
            }
        }

        private static int[] GetDexFormValues(uint Value, int BitsPerForm, int readCt)
        {
            int[] Forms = new int[readCt];
            int n1 = 0xFF >> (8 - BitsPerForm);
            for (int i = 0; i < Forms.Length; i++)
            {
                int val = (int)(Value >> (i * BitsPerForm)) & n1;
                if (n1 == val && BitsPerForm > 1)
                    Forms[i] = -1;
                else
                    Forms[i] = val;
            }

            // (BitsPerForm > 1) was already handled, handle (BitsPerForm == 1)
            if (BitsPerForm == 1 && Forms[0] == Forms[1] && Forms[0] == 1)
                Forms[0] = Forms[1] = -1;

            return Forms;
        }

        private static uint SetDexFormValues(int[] Forms, int BitsPerForm, int readCt)
        {
            int n1 = 0xFF >> (8 - BitsPerForm);
            uint Value = 0xFFFFFFFF << (readCt * BitsPerForm);
            for (int i = 0; i < Forms.Length; i++)
            {
                int val = Forms[i];
                if (val == -1)
                    val = n1;

                Value |= (uint)(val << (BitsPerForm*i));
                if (i >= readCt)
                    throw new ArgumentException("Array count should be less than bitfield count", nameof(Forms));
            }
            return Value;
        }

        private static bool CheckInsertForm(ref int[] Forms, int FormNum)
        {
            if (Forms.Any(num => num == FormNum))
            {
                return false; // already in list
            }
            if (Forms.All(num => num == -1))
            {
                Forms[0] = FormNum;
                return true; // none in list, insert at top
            }

            // insert at first empty
            int n1 = Array.IndexOf(Forms, -1);
            if (n1 < 0)
                return false;

            Forms[n1] = FormNum;
            return true;
        }

        public int DexUpgraded
        {
            get
            {
                switch (Version)
                {
                    case GameVersion.DP:
                        if (General[0x1413] != 0) return 4;
                        else if (General[0x1415] != 0) return 3;
                        else if (General[0x1404] != 0) return 2;
                        else if (General[0x1414] != 0) return 1;
                        else return 0;
                    case GameVersion.HGSS:
                        if (General[0x15ED] != 0) return 3;
                        else if (General[0x15EF] != 0) return 2;
                        else if (General[0x15EE] != 0 && (General[0x10D1] & 8) != 0) return 1;
                        else return 0;
                    case GameVersion.Pt:
                        if (General[0x1641] != 0) return 4;
                        else if (General[0x1643] != 0) return 3;
                        else if (General[0x1640] != 0) return 2;
                        else if (General[0x1642] != 0) return 1;
                        else return 0;
                    default: return 0;
                }
            }
            set
            {
                switch (Version)
                {
                    case GameVersion.DP:
                        General[0x1413] = (byte)(value == 4 ? 1 : 0);
                        General[0x1415] = (byte)(value >= 3 ? 1 : 0);
                        General[0x1404] = (byte)(value >= 2 ? 1 : 0);
                        General[0x1414] = (byte)(value >= 1 ? 1 : 0);
                        break;
                    case GameVersion.HGSS:
                        General[0x15ED] = (byte)(value == 3 ? 1 : 0);
                        General[0x15EF] = (byte)(value >= 2 ? 1 : 0);
                        General[0x15EE] = (byte)(value >= 1 ? 1 : 0);
                        General[0x10D1] = (byte)((General[0x10D1] & ~8) | (value >= 1 ? 8 : 0));
                        break;
                    case GameVersion.Pt:
                        General[0x1641] = (byte)(value == 4 ? 1 : 0);
                        General[0x1643] = (byte)(value >= 3 ? 1 : 0);
                        General[0x1640] = (byte)(value >= 2 ? 1 : 0);
                        General[0x1642] = (byte)(value >= 1 ? 1 : 0);
                        break;
                    default: return;
                }
            }
        }

        public override string GetString(byte[] data, int offset, int length) => StringConverter4.GetString4(data, offset, length);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter4.SetString4(value, maxLength, PadToSize, PadWith);
        }

        /// <summary> All Event Constant values for the savegame </summary>
        public override ushort[] EventConsts
        {
            get
            {
                if (EventConstMax <= 0)
                    return Array.Empty<ushort>();

                ushort[] Constants = new ushort[EventConstMax];
                for (int i = 0; i < Constants.Length; i++)
                    Constants[i] = BitConverter.ToUInt16(General, EventConst + (i * 2));
                return Constants;
            }
            set
            {
                if (EventConstMax <= 0)
                    return;
                if (value.Length != EventConstMax)
                    return;

                for (int i = 0; i < value.Length; i++)
                    BitConverter.GetBytes(value[i]).CopyTo(General, EventConst + (i * 2));
            }
        }

        // Seals
        private const byte SealMaxCount = 99;
        public byte[] SealCase { get => General.Slice(Seal, (int) Seal4.MAX); set => SetData(General, value, Seal); }
        public byte GetSealCount(Seal4 id) => General[Seal + (int)id];
        public byte SetSealCount(Seal4 id, byte count) => General[Seal + (int)id] = Math.Min(SealMaxCount, count);

        public void SetAllSeals(byte count, bool unreleased = false)
        {
            var sealIndexCount = (int)(unreleased ? Seal4.MAX : Seal4.MAXLEGAL);
            var val = Math.Min(count, SealMaxCount);
            for (int i = 0; i < sealIndexCount; i++)
                General[Seal + i] = val;
        }

        public int GetMailOffset(int index)
        {
            int ofs = (index * Mail4.SIZE);
            return Version switch
            {
                GameVersion.DP => (ofs + 0x4BEC),
                GameVersion.Pt => (ofs + 0x4E80),
                _ => (ofs + 0x3FA8)
            };
        }

        public byte[] GetMailData(int ofs) => General.Slice(ofs, Mail4.SIZE);
    }
}
