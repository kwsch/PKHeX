using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 3 <see cref="SaveFile"/> object.
    /// </summary>
    public abstract class SAV3 : SaveFile, ILangDeviantSave
    {
        protected internal sealed override string ShortSummary => $"{OT} ({Version}) - {PlayTimeString}";
        public sealed override string Extension => ".sav";

        public int SaveRevision => Japanese ? 0 : 1;
        public string SaveRevisionString => Japanese ? "J" : "U";
        public bool Japanese { get; }
        public bool Korean => false;

        // Similar to future games, the Generation 3 Mainline save files are comprised of separate objects:
        // Object 1 - Small, containing misc configuration data & the Pokédex.
        // Object 2 - Large, containing everything else that isn't PC Storage system data.
        // Object 3 - Storage, containing all the data for the PC storage system.

        // When the objects are serialized to the savedata, the game fragments each object and saves it to a sector.
        // The main save data for a save file occupies 14 sectors; there are a total of two serialized main saves.
        // After the serialized main save data, there is "extra data", for stuff like Hall of Fame and battle videos.
        // Extra data is always at the same sector, while the main sectors rotate sectors within their region (on each successive save?).

        private const int SIZE_SECTOR = 0x1000;
        private const int SIZE_SECTOR_USED = 0xF80;
        private const int COUNT_MAIN = 14; // sectors worth of data
        private const int SIZE_MAIN = COUNT_MAIN * SIZE_SECTOR;

        // There's no harm having buffers larger than their actual size (per format).
        // A checksum consuming extra zeroes does not change the prior checksum result.
        public readonly byte[] Small   = new byte[1 * SIZE_SECTOR_USED]; //  [0x890 RS, 0xf24 FR/LG, 0xf2c E]
        public readonly byte[] Large   = new byte[4 * SIZE_SECTOR_USED]; //3+[0xc40 RS, 0xee8 FR/LG, 0xf08 E]
        public readonly byte[] Storage = new byte[9 * SIZE_SECTOR_USED]; //  [0x83D0]

        private readonly int ActiveSlot;

        protected SAV3(bool japanese) => Japanese = japanese;

        protected SAV3(byte[] data) : base(data)
        {
            // Copy sector data to the allocated location
            ReadSectors(data, ActiveSlot = GetActiveSlot(data));

            // OT name is the first 8 bytes of Small. The game fills any unused characters with 0xFF.
            // Japanese games are limited to 5 character OT names; INT 7 characters. +1 0xFF terminator.
            // Since JPN games don't touch the last 2 bytes (alignment), they end up as zeroes!
            Japanese = BitConverter.ToInt16(Small, 0x6) == 0;
        }

        private void ReadSectors(byte[] data, int group)
        {
            int start = group * SIZE_MAIN;
            int end = start + SIZE_MAIN;
            for (int ofs = start; ofs < end; ofs += SIZE_SECTOR)
            {
                var id = BitConverter.ToInt16(data, ofs + 0xFF4);
                switch (id)
                {
                    case >=5: Buffer.BlockCopy(data, ofs, Storage, (id - 5) * SIZE_SECTOR_USED, SIZE_SECTOR_USED); break;
                    case >=1: Buffer.BlockCopy(data, ofs, Large  , (id - 1) * SIZE_SECTOR_USED, SIZE_SECTOR_USED); break;
                    default:  Buffer.BlockCopy(data, ofs, Small  , 0                          , SIZE_SECTOR_USED); break;
                }
            }
        }

        private void WriteSectors(byte[] data, int group)
        {
            int start = group * SIZE_MAIN;
            int end = start + SIZE_MAIN;
            for (int ofs = start; ofs < end; ofs += SIZE_SECTOR)
            {
                var id = BitConverter.ToInt16(data, ofs + 0xFF4);
                switch (id)
                {
                    case >=5: Buffer.BlockCopy(Storage, (id - 5) * SIZE_SECTOR_USED, data, ofs, SIZE_SECTOR_USED); break;
                    case >=1: Buffer.BlockCopy(Large  , (id - 1) * SIZE_SECTOR_USED, data, ofs, SIZE_SECTOR_USED); break;
                    default:  Buffer.BlockCopy(Small  , 0                          , data, ofs, SIZE_SECTOR_USED); break;
                }
            }
        }

        /// <summary>
        /// Checks the input data to see if all required sectors for the main save data are present for the <see cref="slot"/>.
        /// </summary>
        /// <param name="data">Data to check</param>
        /// <param name="slot">Which main to check (primary or secondary)</param>
        /// <param name="sector0">Offset of the sector that has the small object data</param>
        public static bool IsAllMainSectorsPresent(byte[] data, int slot, out int sector0)
        {
            System.Diagnostics.Debug.Assert(slot is 0 or 1);
            int start = SIZE_MAIN * slot;
            int end = start + SIZE_MAIN;
            int bitTrack = 0;
            sector0 = 0;
            for (int ofs = 0; ofs < end; ofs += SIZE_SECTOR)
            {
                var id = BitConverter.ToInt16(data, ofs + 0xFF4);
                bitTrack |= (1 << id);
                if (id == 0)
                    sector0 = ofs;
            }
            // all 14 fragments present
            return bitTrack == 0b_0011_1111_1111_1111;
        }

        private static int GetActiveSlot(byte[] data)
        {
            if (data.Length == SaveUtil.SIZE_G3RAWHALF)
                return 0;

            var v0 = IsAllMainSectorsPresent(data, 0, out var sectorZero0);
            var v1 = IsAllMainSectorsPresent(data, 1, out var sectorZero1);
            if (!v0)
                return v1 ? 1 : 0;
            if (!v1)
                return 0;

            var count0 = BitConverter.ToUInt32(data, sectorZero0 + 0x0FFC);
            var count1 = BitConverter.ToUInt32(data, sectorZero1 + 0x0FFC);
            // don't care about 32bit overflow. a 10 second save would take 1,000 years to overflow!
            return count1 > count0 ? 1 : 0;
        }

        protected sealed override byte[] GetFinalData()
        {
            // Copy Box data back
            WriteSectors(Data, ActiveSlot);
            return base.GetFinalData();
        }

        protected sealed override int SIZE_STORED => PokeCrypto.SIZE_3STORED;
        protected sealed override int SIZE_PARTY => PokeCrypto.SIZE_3PARTY;
        public sealed override PKM BlankPKM => new PK3();
        public sealed override Type PKMType => typeof(PK3);

        public sealed override int MaxMoveID => Legal.MaxMoveID_3;
        public sealed override int MaxSpeciesID => Legal.MaxSpeciesID_3;
        public sealed override int MaxAbilityID => Legal.MaxAbilityID_3;
        public sealed override int MaxItemID => Legal.MaxItemID_3;
        public sealed override int MaxBallID => Legal.MaxBallID_3;
        public sealed override int MaxGameID => Legal.MaxGameID_3;

        public sealed override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_RS;

        public sealed override int BoxCount => 14;
        public sealed override int MaxEV => 255;
        public sealed override int Generation => 3;
        protected sealed override int GiftCountMax => 1;
        public sealed override int OTLength => 7;
        public sealed override int NickLength => 10;
        public sealed override int MaxMoney => 999999;

        public sealed override bool HasParty => true;

        public sealed override bool IsPKMPresent(byte[] data, int offset) => PKX.IsPKMPresentGBA(data, offset);
        protected sealed override PKM GetPKM(byte[] data) => new PK3(data);
        protected sealed override byte[] DecryptPKM(byte[] data) => PokeCrypto.DecryptArray3(data);

        protected sealed override byte[] BoxBuffer => Storage;
        protected sealed override byte[] PartyBuffer => Large;

        private const int COUNT_BOX = 14;
        private const int COUNT_SLOTSPERBOX = 30;

        // Checksums
        protected sealed override void SetChecksums()
        {
            int start = ActiveSlot * SIZE_MAIN;
            int end = start + SIZE_MAIN;
            for (int ofs = start; ofs < end; ofs += SIZE_SECTOR)
            {
                ushort chk = Checksums.CheckSum32(Data, ofs, SIZE_SECTOR_USED);
                BitConverter.GetBytes(chk).CopyTo(Data, ofs + 0xFF6);
            }

            if (State.BAK.Length < SaveUtil.SIZE_G3RAW) // don't update HoF for half-sizes
                return;

            // Hall of Fame Checksums
            {
                ushort chk = Checksums.CheckSum32(Data, 0x1C000, SIZE_SECTOR_USED);
                BitConverter.GetBytes(chk).CopyTo(Data, 0x1CFF4);
            }
            {
                ushort chk = Checksums.CheckSum32(Data, 0x1D000, SIZE_SECTOR_USED);
                BitConverter.GetBytes(chk).CopyTo(Data, 0x1DFF4);
            }
        }

        public sealed override bool ChecksumsValid
        {
            get
            {
                for (int i = 0; i < COUNT_MAIN; i++)
                {
                    if (!IsSectorValid(i))
                        return false;
                }

                if (State.BAK.Length < SaveUtil.SIZE_G3RAW) // don't check HoF for half-sizes
                    return true;

                if (!IsSectorValidExtra(0x1C000))
                    return false;
                if (!IsSectorValidExtra(0x1D000))
                    return false;
                return true;
            }
        }

        private bool IsSectorValidExtra(int ofs)
        {
            ushort chk = Checksums.CheckSum32(Data, ofs, SIZE_SECTOR_USED);
            return chk == BitConverter.ToUInt16(Data, ofs + 0xFF4);
        }

        private bool IsSectorValid(int sector)
        {
            int start = ActiveSlot * SIZE_MAIN;
            int ofs = start + (sector * SIZE_SECTOR);
            ushort chk = Checksums.CheckSum32(Data, ofs, SIZE_SECTOR_USED);
            return chk == BitConverter.ToUInt16(Data, ofs + 0xFF6);
        }

        public sealed override string ChecksumInfo
        {
            get
            {
                var list = new List<string>();
                for (int i = 0; i < COUNT_MAIN; i++)
                {
                    if (!IsSectorValid(i))
                        list.Add($"Sector {i} @ {i*SIZE_SECTOR:X5} invalid.");
                }

                if (State.BAK.Length > SaveUtil.SIZE_G3RAW) // don't check HoF for half-sizes
                {
                    if (!IsSectorValidExtra(0x1C000))
                        list.Add("HoF first sector invalid.");
                    if (!IsSectorValidExtra(0x1D000))
                        list.Add("HoF second sector invalid.");
                }
                return list.Count != 0 ? string.Join(Environment.NewLine, list) : "Checksums are valid.";
            }
        }

        public abstract uint SecurityKey { get; set; }

        public sealed override string OT
        {
            get => GetString(Small, 0, 0x10);
            set
            {
                int len = Japanese ? 5 : OTLength;
                SetString(value, len, PadToSize: len, PadWith: 0xFF).CopyTo(Small, 0);
            }
        }

        public sealed override int Gender
        {
            get => Small[8];
            set => Small[8] = (byte)value;
        }

        public sealed override int TID
        {
            get => BitConverter.ToUInt16(Small, 0xA);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Small, 0xA);
        }

        public sealed override int SID
        {
            get => BitConverter.ToUInt16(Small, 0xC);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Small, 0xC);
        }

        public sealed override int PlayedHours
        {
            get => BitConverter.ToUInt16(Small, 0xE);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Small, 0xE);
        }

        public sealed override int PlayedMinutes
        {
            get => Small[0x10];
            set => Small[0x10] = (byte)value;
        }

        public sealed override int PlayedSeconds
        {
            get => Small[0x11];
            set => Small[0x11] = (byte)value;
        }

        public int PlayedFrames
        {
            get => Small[0x12];
            set => Small[0x12] = (byte)value;
        }

        public sealed override bool GetEventFlag(int flagNumber)
        {
            if (flagNumber >= EventFlagMax)
                throw new ArgumentException($"Event Flag to get ({flagNumber}) is greater than max ({EventFlagMax}).");

            var start = EventFlag;
            return GetFlag(start + (flagNumber >> 3), flagNumber & 7);
        }

        public sealed override void SetEventFlag(int flagNumber, bool value)
        {
            if (flagNumber >= EventFlagMax)
                throw new ArgumentException($"Event Flag to set ({flagNumber}) is greater than max ({EventFlagMax}).");

            var start = EventFlag;
            SetFlag(start + (flagNumber >> 3), flagNumber & 7, value);
        }

        public sealed override bool GetFlag(int offset, int bitIndex) => FlagUtil.GetFlag(Large, offset, bitIndex);
        public sealed override void SetFlag(int offset, int bitIndex, bool value) => FlagUtil.SetFlag(Large, offset, bitIndex, value);

        public ushort GetEventConst(int index) => BitConverter.ToUInt16(Large, EventConst + (index * 2));
        public void SetEventConst(int index, ushort value) => BitConverter.GetBytes(value).CopyTo(Large, EventConst + (index * 2));

        public sealed override ushort[] GetEventConsts()
        {
            ushort[] Constants = new ushort[EventConstMax];
            for (int i = 0; i < Constants.Length; i++)
                Constants[i] = GetEventConst(i);
            return Constants;
        }

        public sealed override void SetEventConsts(ushort[] value)
        {
            if (value.Length != EventConstMax)
                return;

            for (int i = 0; i < value.Length; i++)
                SetEventConst(i, value[i]);
        }

        protected abstract int BadgeFlagStart { get; }
        public abstract uint Coin { get; set; }

        public int Badges
        {
            get
            {
                int startFlag = BadgeFlagStart;
                int val = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (GetEventFlag(startFlag + i))
                        val |= 1 << i;
                }

                return val;
            }
            set
            {
                int startFlag = BadgeFlagStart;
                for (int i = 0; i < 8; i++)
                    SetEventFlag(startFlag + i, (value & (1 << i)) != 0);
            }
        }

        public sealed override IReadOnlyList<InventoryPouch> Inventory
        {
            get
            {
                var pouch = GetItems();
                foreach (var p in pouch)
                {
                    if (p.Type != InventoryType.PCItems)
                        p.SecurityKey = SecurityKey;
                }
                return pouch.LoadAll(Large);
            }
            set => value.SaveAll(Large);
        }

        protected abstract InventoryPouch3[] GetItems();

        protected abstract int DaycareSlotSize { get; }

        public sealed override uint? GetDaycareEXP(int loc, int slot) => BitConverter.ToUInt32(Large, GetDaycareEXPOffset(slot));
        public sealed override void SetDaycareEXP(int loc, int slot, uint EXP) => BitConverter.GetBytes(EXP).CopyTo(Large, GetDaycareEXPOffset(slot));
        public sealed override bool? IsDaycareOccupied(int loc, int slot) => IsPKMPresent(Large, GetDaycareSlotOffset(loc, slot));
        public sealed override void SetDaycareOccupied(int loc, int slot, bool occupied) { /* todo */ }
        public sealed override int GetDaycareSlotOffset(int loc, int slot) => DaycareOffset + (slot * DaycareSlotSize);

        protected abstract int EggEventFlag { get; }
        public sealed override bool? IsDaycareHasEgg(int loc) => GetEventFlag(EggEventFlag);
        public sealed override void SetDaycareHasEgg(int loc, bool hasEgg) => SetEventFlag(EggEventFlag, hasEgg);

        protected abstract int GetDaycareEXPOffset(int slot);

        #region Storage
        public sealed override int GetBoxOffset(int box) => Box + 4 + (SIZE_STORED * box * COUNT_SLOTSPERBOX);

        public sealed override int CurrentBox
        {
            get => Storage[0];
            set => Storage[0] = (byte)value;
        }

        public sealed override int GetBoxWallpaper(int box)
        {
            if (box > COUNT_BOX)
                return box;
            int offset = GetBoxWallpaperOffset(box);
            return Storage[offset];
        }

        private const int COUNT_BOXNAME = 8 + 1;

        public sealed override void SetBoxWallpaper(int box, int value)
        {
            if (box > COUNT_BOX)
                return;
            int offset = GetBoxWallpaperOffset(box);
            Storage[offset] = (byte)value;
        }

        protected sealed override int GetBoxWallpaperOffset(int box)
        {
            int offset = GetBoxOffset(COUNT_BOX);
            offset += (COUNT_BOX * COUNT_BOXNAME) + box;
            return offset;
        }

        public sealed override string GetBoxName(int box)
        {
            int offset = GetBoxOffset(COUNT_BOX);
            return StringConverter3.GetString3(Storage, offset + (box * COUNT_BOXNAME), COUNT_BOXNAME, Japanese);
        }

        public sealed override void SetBoxName(int box, string value)
        {
            int offset = GetBoxOffset(COUNT_BOX);
            SetString(value, COUNT_BOXNAME - 1).CopyTo(Storage, offset + (box * COUNT_BOXNAME));
        }
        #endregion

        #region Pokédex
        protected sealed override void SetDex(PKM pkm)
        {
            int species = pkm.Species;
            if (species is 0 or > Legal.MaxSpeciesID_3)
                return;
            if (pkm.IsEgg)
                return;

            switch (species)
            {
                case (int)Species.Unown when !GetSeen(species): // Unown
                    DexPIDUnown = pkm.PID;
                    break;
                case (int)Species.Spinda when !GetSeen(species): // Spinda
                    DexPIDSpinda = pkm.PID;
                    break;
            }
            SetCaught(species, true);
            SetSeen(species, true);
        }

        public uint DexPIDUnown { get => BitConverter.ToUInt32(Small, PokeDex + 0x4); set => BitConverter.GetBytes(value).CopyTo(Small, PokeDex + 0x4); }
        public uint DexPIDSpinda { get => BitConverter.ToUInt32(Small, PokeDex + 0x8); set => BitConverter.GetBytes(value).CopyTo(Small, PokeDex + 0x8); }
        public int DexUnownForm => PKX.GetUnownForm(DexPIDUnown);

        public sealed override bool GetCaught(int species)
        {
            int bit = species - 1;
            int ofs = bit >> 3;
            int caughtOffset = PokeDex + 0x10;
            return FlagUtil.GetFlag(Small, caughtOffset + ofs, bit & 7);
        }

        public sealed override void SetCaught(int species, bool caught)
        {
            int bit = species - 1;
            int ofs = bit >> 3;
            int caughtOffset = PokeDex + 0x10;
            FlagUtil.SetFlag(Small, caughtOffset + ofs, bit & 7, caught);
        }

        public sealed override bool GetSeen(int species)
        {
            int bit = species - 1;
            int ofs = bit >> 3;
            int seenOffset = PokeDex + 0x44;
            return FlagUtil.GetFlag(Small, seenOffset + ofs, bit & 7);
        }

        protected abstract int SeenOffset2 { get; }
        protected abstract int SeenOffset3 { get; }

        public sealed override void SetSeen(int species, bool seen)
        {
            int bit = species - 1;
            int ofs = bit >> 3;

            int seenOffset = PokeDex + 0x44;
            FlagUtil.SetFlag(Small, seenOffset + ofs, bit & 7, seen);
            FlagUtil.SetFlag(Large, SeenOffset2 + ofs, bit & 7, seen);
            FlagUtil.SetFlag(Large, SeenOffset3 + ofs, bit & 7, seen);
        }

        public byte PokedexSort
        {
            get => Small[PokeDex + 0x01];
            set => Small[PokeDex + 0x01] = value;
        }

        public byte PokedexMode
        {
            get => Small[PokeDex + 0x01];
            set => Small[PokeDex + 0x01] = value;
        }

        public byte PokedexNationalMagicRSE
        {
            get => Small[PokeDex + 0x02];
            set => Small[PokeDex + 0x02] = value;
        }

        public byte PokedexNationalMagicFRLG
        {
            get => Small[PokeDex + 0x03];
            set => Small[PokeDex + 0x03] = value;
        }

        protected const byte PokedexNationalUnlockRSE = 0xDA;
        protected const byte PokedexNationalUnlockFRLG = 0xDA;
        protected const ushort PokedexNationalUnlockWorkRSE = 0x0302;
        protected const ushort PokedexNationalUnlockWorkFRLG = 0x6258;

        public abstract bool NationalDex { get; set; }
        #endregion

        public sealed override string GetString(byte[] data, int offset, int length) => StringConverter3.GetString3(data, offset, length, Japanese);

        public sealed override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter3.SetString3(value, maxLength, Japanese, PadToSize, PadWith);
        }

        protected abstract int MailOffset { get; }
        public int GetMailOffset(int index) => (index * Mail3.SIZE) + MailOffset;

        public Mail GetMail(int i)
        {
            var ofs = GetMailOffset(i);
            var data = Large.Slice(ofs, Mail3.SIZE);
            return new Mail3(data, ofs, Japanese);
        }

        public abstract string EBerryName { get; }
        public abstract bool IsEBerryEngima { get; }
        public abstract MysteryEvent3 MysteryEvent { get; set; }

        public byte[] GetHallOfFameData()
        {
            // HoF Data is split across two sectors
            byte[] data = new byte[SIZE_SECTOR_USED * 2];
            Buffer.BlockCopy(Data, 0x1C000, data, 0               , SIZE_SECTOR_USED);
            Buffer.BlockCopy(Data, 0x1D000, data, SIZE_SECTOR_USED, SIZE_SECTOR_USED);
            return data;
        }

        public void SetHallOfFameData(byte[] value)
        {
            if (value.Length != SIZE_SECTOR_USED * 2)
                throw new ArgumentException("Invalid size", nameof(value));
            // HoF Data is split across two sav sectors
            Buffer.BlockCopy(value, 0               , Data, 0x1C000, SIZE_SECTOR_USED);
            Buffer.BlockCopy(value, SIZE_SECTOR_USED, Data, 0x1D000, SIZE_SECTOR_USED);
        }

        public bool IsCorruptPokedexFF() => BitConverter.ToUInt64(Small, 0xAC) == ulong.MaxValue;

        public override void CopyChangesFrom(SaveFile sav)
        {
            SetData(sav.Data, 0);
            var s3 = (SAV3)sav;
            SetData(Small, s3.Small, 0);
            SetData(Large, s3.Large, 0);
            SetData(Storage, s3.Storage, 0);
        }
    }
}
