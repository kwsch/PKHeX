using System;
using System.Collections.Generic;

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
        protected internal override string ShortSummary => $"{OT} ({Version}) - {PlayTimeString}";
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

        protected abstract int StorageStart { get; }
        public abstract Zukan4 Dex { get; }

        /// <inheritdoc />
        public override bool GetFlag(int offset, int bitIndex) => FlagUtil.GetFlag(General, offset, bitIndex);

        /// <inheritdoc />
        public override void SetFlag(int offset, int bitIndex, bool value) => FlagUtil.SetFlag(General, offset, bitIndex, value);

        protected SAV4(int gSize, int sSize)
        {
            General = new byte[gSize];
            Storage = new byte[sSize];
            ClearBoxes();
        }

        protected SAV4(byte[] data, int gSize, int sSize, int sStart) : base(data)
        {
            GeneralBlockPosition = GetActiveBlock(data, 0, gSize);
            StorageBlockPosition = GetActiveBlock(data, sStart, sSize);

            var gbo = (GeneralBlockPosition == 0 ? 0 : PartitionSize);
            var sbo = (StorageBlockPosition == 0 ? 0 : PartitionSize) + sStart;
            General = GetData(gbo, gSize);
            Storage = GetData(sbo, sSize);
        }

        // Configuration
        protected override SaveFile CloneInternal()
        {
            var sav = CloneInternal4();
            SetData(sav.General, General, 0);
            SetData(sav.Storage, Storage, 0);
            return sav;
        }

        protected abstract SAV4 CloneInternal4();

        public override void CopyChangesFrom(SaveFile sav)
        {
            SetData(sav.Data, 0);
            var s4 = (SAV4)sav;
            SetData(General, s4.General, 0);
            SetData(Storage, s4.Storage, 0);
        }

        protected override int SIZE_STORED => PokeCrypto.SIZE_4STORED;
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
        // MaxItemID
        public override int MaxAbilityID => Legal.MaxAbilityID_4;
        public override int MaxBallID => Legal.MaxBallID_4;
        public override int MaxGameID => Legal.MaxGameID_4; // Colo/XD

        public bool HGSS => Version == GameVersion.HGSS;
        public bool DP => Version == GameVersion.DP;

        // Checksums
        protected abstract int FooterSize { get; }
        private ushort CalcBlockChecksum(byte[] data) => Checksums.CRC16_CCITT(new ReadOnlySpan<byte>(data, 0, data.Length - FooterSize));
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
            int offset = begin + length - 0x14;
            return SAV4BlockDetection.CompareFooters(data, offset, offset + PartitionSize);
        }

        protected int WondercardFlags = int.MinValue;
        protected int AdventureInfo = int.MinValue;
        protected int Seal = int.MinValue;
        protected int Trainer1;
        public int GTS { get; } = int.MinValue;

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

        public abstract int M { get; set; }
        public abstract int X { get; set; }
        public abstract int Y { get; set; }

        public abstract int X2 { get; set; }
        public abstract int Y2 { get; set; }
        public abstract int Z { get; set; }

        public override uint SecondsToStart { get => BitConverter.ToUInt32(General, AdventureInfo + 0x34); set => BitConverter.GetBytes(value).CopyTo(General, AdventureInfo + 0x34); }
        public override uint SecondsToFame { get => BitConverter.ToUInt32(General, AdventureInfo + 0x3C); set => BitConverter.GetBytes(value).CopyTo(General, AdventureInfo + 0x3C); }

        protected override PKM GetPKM(byte[] data) => new PK4(data);
        protected override byte[] DecryptPKM(byte[] data) => PokeCrypto.DecryptArray45(data);

        protected override void SetPKM(PKM pkm, bool isParty = false)
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
                if (value[i] is PGT {CardType: not 0})
                    return true;
            }
            for (int i = 8; i < 11; i++) // 3 PCD
            {
                if (value[i] is PCD {Gift: {CardType: not 0}})
                    return true;
            }
            return false;
        }

        private byte[] MatchMysteryGifts(DataMysteryGift[] value)
        {
            byte[] cardMatch = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                if (value[i] is not PGT pgt)
                    continue;

                if (pgt.CardType == 0) // empty
                {
                    cardMatch[i] = pgt.Slot = 0;
                    continue;
                }

                cardMatch[i] = pgt.Slot = 3;
                for (byte j = 0; j < 3; j++)
                {
                    if (value[8 + j] is not PCD pcd)
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

        protected override void SetDex(PKM pkm) => Dex.SetDex(pkm);
        public override bool GetCaught(int species) => Dex.GetCaught(species);
        public override bool GetSeen(int species) => Dex.GetSeen(species);

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
                        General[0x1413] = value == 4 ? (byte)1 : (byte)0;
                        General[0x1415] = value >= 3 ? (byte)1 : (byte)0;
                        General[0x1404] = value >= 2 ? (byte)1 : (byte)0;
                        General[0x1414] = value >= 1 ? (byte)1 : (byte)0;
                        break;
                    case GameVersion.HGSS:
                        General[0x15ED] = value == 3 ? (byte)1 : (byte)0;
                        General[0x15EF] = value >= 2 ? (byte)1 : (byte)0;
                        General[0x15EE] = value >= 1 ? (byte)1 : (byte)0;
                        General[0x10D1] = (byte)((General[0x10D1] & ~8) | (value >= 1 ? 8 : 0));
                        break;
                    case GameVersion.Pt:
                        General[0x1641] = value == 4 ? (byte)1 : (byte)0;
                        General[0x1643] = value >= 3 ? (byte)1 : (byte)0;
                        General[0x1640] = value >= 2 ? (byte)1 : (byte)0;
                        General[0x1642] = value >= 1 ? (byte)1 : (byte)0;
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
        public override ushort[] GetEventConsts()
        {
            if (EventConstMax <= 0)
                return Array.Empty<ushort>();

            ushort[] Constants = new ushort[EventConstMax];
            for (int i = 0; i < Constants.Length; i++)
                Constants[i] = BitConverter.ToUInt16(General, EventConst + (i * 2));
            return Constants;
        }

        /// <summary> All Event Constant values for the savegame </summary>
        public override void SetEventConsts(ushort[] value)
        {
            if (EventConstMax <= 0)
                return;
            if (value.Length != EventConstMax)
                return;

            for (int i = 0; i < value.Length; i++)
                BitConverter.GetBytes(value[i]).CopyTo(General, EventConst + (i * 2));
        }

        // Seals
        private const byte SealMaxCount = 99;

        public byte[] GetSealCase() => General.Slice(Seal, (int)Seal4.MAX);
        public void SetSealCase(byte[] value) => SetData(General, value, Seal);

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

        public Mail4 GetMail(int i)
        {
            int ofs = GetMailOffset(i);
            return new Mail4(GetMailData(ofs), ofs);
        }
    }
}
