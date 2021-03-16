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

        // Similar to future games, the Generation 3 Mainline save files are comprised of two separate objects:
        // Object 1 - Small Block, containing misc configuration data & the Pokédex.
        // Object 2 - Large Block, containing everything else that isn't PC Storage system data.
        // Object 3 - Storage Block, containing all the data for the PC storage system.

        // When the objects are serialized to the savedata, the game breaks up each object into chunks < 0x1000 bytes.
        // Each serialized save occupies 14 chunks; there are a total of two serialized saves.
        // After the serialized save data, there is "extra data", for stuff like Hall of Fame and battle videos.

        private const int SIZE_BLOCK = 0x1000;
        private const int BLOCK_COUNT = 14;
        public const int SIZE_BLOCK_USED = 0xF80;

        private const int COUNT_BOX = 14;
        private const int COUNT_SLOTSPERBOX = 30;
        // Use the largest of structure sizes, as zeroes being fed into checksum function don't change the value.
        private const int SIZE_SMALL = 0xF2C; // maximum size for R/S/E/FR/LG structures
        private const int SIZE_LARGE = (3 * 0xF80) + 0xF08; // maximum size for R/S/E/FR/LG structures

        public readonly byte[] Small = new byte[SIZE_SMALL];
        public readonly byte[] Large = new byte[SIZE_LARGE];
        public readonly byte[] Storage = new byte[SIZE_PC];
        protected sealed override byte[] BoxBuffer => Storage;
        protected sealed override byte[] PartyBuffer => Large;

        // 0x83D0
        private const int SIZE_PC = sizeof(int) // Current Box
                                    + (COUNT_BOX * (COUNT_SLOTSPERBOX * PokeCrypto.SIZE_3STORED)) // Slots
                                    + (COUNT_BOX * (8 + 1)) // Box Names
                                    + (COUNT_BOX * 1); // Box Wallpapers

        private static readonly ushort[] chunkLength =
        {
            0xf2c, // 0 | Small Block (Trainer Info) [0x890 RS, 0xf24 FR/LG]
            0xf80, // 1 | Large Block Part 1
            0xf80, // 2 | Large Block Part 2
            0xf80, // 3 | Large Block Part 3
            0xf08, // 4 | Large Block Part 4 [0xc40 RS, 0xee8 FR/LG]
            0xf80, // 5 | PC Block 0
            0xf80, // 6 | PC Block 1
            0xf80, // 7 | PC Block 2
            0xf80, // 8 | PC Block 3
            0xf80, // 9 | PC Block 4
            0xf80, // A | PC Block 5
            0xf80, // B | PC Block 6
            0xf80, // C | PC Block 7
            0x7d0  // D | PC Block 8
        };

        public sealed override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_RS;

        protected SAV3(bool japanese)
        {
            Japanese = japanese;
            BlockOrder = Array.Empty<short>();
        }

        protected SAV3(byte[] data) : base(data)
        {
            LoadBlocks(out BlockOrder);

            // Copy chunk to the allocated location
            LoadBlocks(Small, 0, 1);
            LoadBlocks(Large, 1, 5);
            LoadBlocks(Storage, 5, BLOCK_COUNT);

            // Japanese games are limited to 5 character OT names; any unused characters are 0xFF.
            // 5 for JP, 7 for INT. There's always 1 terminator, thus we can check 0x6-0x7 being 0xFFFF = INT
            // OT name is stored at the top of the first block.
            Japanese = BitConverter.ToInt16(Small, 0x6) == 0;
        }

        private void LoadBlocks(byte[] dest, short start, short end)
        {
            for (short i = start; i < end; i++)
            {
                int blockIndex = Array.IndexOf(BlockOrder, i);
                if (blockIndex == -1) // block empty
                    continue;

                var sOfs = (blockIndex * SIZE_BLOCK) + ABO;
                var dOfs = (i - start) * SIZE_BLOCK_USED;
                var count = chunkLength[i];
                Buffer.BlockCopy(Data, sOfs, dest, dOfs, count);
            }
        }

        private void SaveBlocks(byte[] dest, short start, short end)
        {
            for (short i = start; i < end; i++)
            {
                int blockIndex = Array.IndexOf(BlockOrder, i);
                if (blockIndex == -1) // block empty
                    continue;

                var sOfs = (blockIndex * SIZE_BLOCK) + ABO;
                var dOfs = (i - start) * SIZE_BLOCK_USED;
                var count = chunkLength[i];
                Buffer.BlockCopy(dest, dOfs, Data, sOfs, count);
            }
        }

        private void LoadBlocks(out short[] blockOrder)
        {
            var o1 = GetBlockOrder(0);
            if (Data.Length > SaveUtil.SIZE_G3RAWHALF)
            {
                var o2 = GetBlockOrder(0xE000);
                ActiveSAV = GetActiveSaveIndex(o1, o2);
                blockOrder = ActiveSAV == 0 ? o1 : o2;
            }
            else
            {
                ActiveSAV = 0;
                blockOrder = o1;
            }
        }

        private short[] GetBlockOrder(int ofs)
        {
            short[] order = new short[BLOCK_COUNT];
            for (int i = 0; i < BLOCK_COUNT; i++)
                order[i] = BitConverter.ToInt16(Data, ofs + (i * SIZE_BLOCK) + 0xFF4);
            return order;
        }

        private int GetActiveSaveIndex(short[] BlockOrder1, short[] BlockOrder2)
        {
            int zeroBlock1 = Array.IndexOf(BlockOrder1, (short)0);
            int zeroBlock2 = Array.IndexOf(BlockOrder2, (short)0);
            if (zeroBlock2 < 0)
                return 0;
            if (zeroBlock1 < 0)
                return 1;
            var count1 = BitConverter.ToUInt32(Data, (zeroBlock1 * SIZE_BLOCK) + 0x0FFC);
            var count2 = BitConverter.ToUInt32(Data, (zeroBlock2 * SIZE_BLOCK) + 0xEFFC);
            return count1 > count2 ? 0 : 1;
        }

        protected sealed override byte[] GetFinalData()
        {
            // Copy Box data back
            SaveBlocks(Small, 0, 1);
            SaveBlocks(Large, 1, 5);
            SaveBlocks(Storage, 5, BLOCK_COUNT);
            return base.GetFinalData();
        }

        private int ActiveSAV;
        private int ABO => ActiveSAV*SIZE_BLOCK*0xE;
        private readonly short[] BlockOrder;

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

        // Checksums
        protected sealed override void SetChecksums()
        {
            for (int i = 0; i < BLOCK_COUNT; i++)
            {
                int ofs = ABO + (i * SIZE_BLOCK);
                var index = BlockOrder[i];
                if (index == -1)
                    continue;
                int len = chunkLength[index];
                ushort chk = Checksums.CheckSum32(Data, ofs, len);
                BitConverter.GetBytes(chk).CopyTo(Data, ofs + 0xFF6);
            }

            if (State.BAK.Length < SaveUtil.SIZE_G3RAW) // don't update HoF for half-sizes
                return;

            // Hall of Fame Checksums
            {
                ushort chk = Checksums.CheckSum32(Data, 0x1C000, SIZE_BLOCK_USED);
                BitConverter.GetBytes(chk).CopyTo(Data, 0x1CFF4);
            }
            {
                ushort chk = Checksums.CheckSum32(Data, 0x1D000, SIZE_BLOCK_USED);
                BitConverter.GetBytes(chk).CopyTo(Data, 0x1DFF4);
            }
        }

        public sealed override bool ChecksumsValid
        {
            get
            {
                for (int i = 0; i < BLOCK_COUNT; i++)
                {
                    if (!IsChunkValid(i))
                        return false;
                }

                if (State.BAK.Length < SaveUtil.SIZE_G3RAW) // don't check HoF for half-sizes
                    return true;

                if (!IsChunkValidHoF(0x1C000))
                    return false;
                if (!IsChunkValidHoF(0x1D000))
                    return false;
                return true;
            }
        }

        private bool IsChunkValidHoF(int ofs)
        {
            ushort chk = Checksums.CheckSum32(Data, ofs, SIZE_BLOCK_USED);
            return chk == BitConverter.ToUInt16(Data, ofs + 0xFF4);
        }

        private bool IsChunkValid(int chunk)
        {
            int ofs = ABO + (chunk * SIZE_BLOCK);
            int len = chunkLength[BlockOrder[chunk]];
            ushort chk = Checksums.CheckSum32(Data, ofs, len);
            return chk == BitConverter.ToUInt16(Data, ofs + 0xFF6);
        }

        public sealed override string ChecksumInfo
        {
            get
            {
                var list = new List<string>();
                for (int i = 0; i < BLOCK_COUNT; i++)
                {
                    if (!IsChunkValid(i))
                        list.Add($"Block {BlockOrder[i]:00} @ {i*SIZE_BLOCK:X5} invalid.");
                }

                if (State.BAK.Length > SaveUtil.SIZE_G3RAW) // don't check HoF for half-sizes
                {
                    if (!IsChunkValidHoF(0x1C000))
                        list.Add("HoF Block 1 invalid.");
                    if (!IsChunkValidHoF(0x1D000))
                        list.Add("HoF Block 2 invalid.");
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
            if (!CanSetDex(species))
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

        private bool CanSetDex(int species)
        {
            if (species <= 0)
                return false;
            if (species > MaxSpeciesID)
                return false;
            return true;
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

        protected const int PokedexNationalUnlockRSE = 0xDA;
        protected const int PokedexNationalUnlockFRLG = 0xDA;
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

        public abstract string EBerryName { get; }
        public abstract bool IsEBerryEngima { get; }
    }
}
