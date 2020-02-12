using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="SaveFile"/> format for <see cref="GameVersion.HGSS"/>
    /// </summary>
    public sealed class SAV4HGSS : SAV4
    {
        public SAV4HGSS() => Initialize();
        public SAV4HGSS(byte[] data) : base(data) => Initialize();
        protected override SAV4 CloneInternal() => Exportable ? new SAV4HGSS(Data) : new SAV4HGSS();

        public override PersonalTable Personal => PersonalTable.HGSS;
        public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_HGSS;
        protected override int GeneralSize => 0xF628;
        protected override int StorageSize => 0x12310; // Start 0xF700, +0 starts box data
        protected override int StorageStart => 0xF700; // unused section right after GeneralSize, alignment?
        protected override int FooterSize => 0x10;

        private void Initialize()
        {
            Version = GameVersion.HGSS;
            GetSAVOffsets();
        }

        private void GetSAVOffsets()
        {
            AdventureInfo = 0;
            Trainer1 = 0x64;
            Party = 0x98;
            PokeDex = 0x12B8;
            WondercardFlags = 0x9D3C;
            WondercardData = 0x9E3C;

            EventConst = 0xDE4;
            EventFlag = 0x10C4;
            DaycareOffset = 0x15FC;
            Seal = 0x4E20;

            Box = 0;
        }

        #region Storage
        // box{pk4[30}[18]
        // u32 currentBox
        // u32 counter
        // g4str[18] boxNames
        // byte[18] boxWallpapers
        // -- each box is chunked, padded to nearest 0x100 (resulting in 0x10 trailing zeroes)
        // -- The final 0x16 bytes in the Storage block are unused (padding to nearest 0x100).
        private const int BOX_COUNT = 18;
        private const int BOX_SLOTS = 30;
        private const int BOX_NAME_LEN = 40; // 20 characters

        private const int BOX_DATA_LEN = (BOX_SLOTS * PokeCrypto.SIZE_4STORED) + 0x10; // 0xFF0, each box chunk is padded to nearest 0x100
        private const int BOX_END = BOX_COUNT * BOX_DATA_LEN; // 18 * 0x1000
        private const int BOX_NAME = 0x12008; // after current & counter
        private const int BOX_WP = BOX_NAME + (BOX_COUNT * BOX_NAME_LEN); // 0x122D8;
        private const int BOX_FLAGS = 18 + BOX_WP; // 0x122EA;

        public override int GetBoxOffset(int box) => box * 0x1000;
        private static int GetBoxNameOffset(int box) => BOX_NAME + (box * BOX_NAME_LEN);
        protected override int GetBoxWallpaperOffset(int box) => BOX_WP + box;

        // 8 bytes current box (align 32) & (stored count?)
        public override int CurrentBox
        {
            get => Storage[BOX_END];
            set => Storage[BOX_END] = (byte)value;
        }

        public override byte[] BoxFlags
        {
            get => new[] { Storage[BOX_FLAGS] };
            set => Storage[BOX_FLAGS] = value[0];
        }

        public int Counter
        {
            get => BitConverter.ToInt32(Storage, BOX_END + 4);
            set => SetData(Storage, BitConverter.GetBytes(value), BOX_END + 4);
        }

        public override string GetBoxName(int box) => GetString(Storage, GetBoxNameOffset(box), BOX_NAME_LEN);

        public override void SetBoxName(int box, string value)
        {
            const int maxlen = 8;
            if (value.Length > maxlen)
                value = value.Substring(0, maxlen); // Hard cap
            int offset = GetBoxNameOffset(box);
            var str = SetString(value, maxlen);
            SetData(Storage, str, offset);
        }

        private static int AdjustWallpaper(int value, int shift)
        {
            // Pt's  Special Wallpapers 1-8 are shifted by +0x8
            // HG/SS Special Wallpapers 1-8 (Primo Phrases) are shifted by +0x10
            if (value >= 0x10) // special
                return value + shift;
            return value;
        }

        public override int GetBoxWallpaper(int box)
        {
            int offset = GetBoxWallpaperOffset(box);
            int value = Storage[offset];
            return AdjustWallpaper(value, -0x10);
        }

        public override void SetBoxWallpaper(int box, int value)
        {
            value = AdjustWallpaper(value, 0x10);
            Storage[GetBoxWallpaperOffset(box)] = (byte)value;
        }
        #endregion

        public override InventoryPouch[] Inventory
        {
            get
            {
                InventoryPouch[] pouch =
                {
                    new InventoryPouch4(InventoryType.Items, Legal.Pouch_Items_HGSS, 999, 0x644), // 0x644-0x8D7 (0x8CB)
                    new InventoryPouch4(InventoryType.KeyItems, Legal.Pouch_Key_HGSS, 1, 0x8D8), // 0x8D8-0x99F (0x979)
                    new InventoryPouch4(InventoryType.TMHMs, Legal.Pouch_TMHM_HGSS, 99, 0x9A0), // 0x9A0-0xB33 (0xB2F)
                    new InventoryPouch4(InventoryType.MailItems, Legal.Pouch_Mail_HGSS, 999, 0xB34), // 0xB34-0xB63 (0xB63)
                    new InventoryPouch4(InventoryType.Medicine, Legal.Pouch_Medicine_HGSS, 999, 0xB64), // 0xB64-0xC03 (0xBFB)
                    new InventoryPouch4(InventoryType.Berries, Legal.Pouch_Berries_HGSS, 999, 0xC04), // 0xC04-0xD03
                    new InventoryPouch4(InventoryType.Balls, Legal.Pouch_Ball_HGSS, 999, 0xD04), // 0xD04-0xD63
                    new InventoryPouch4(InventoryType.BattleItems, Legal.Pouch_Battle_HGSS, 999, 0xD64), // 0xD64-0xD97
                };
                return pouch.LoadAll(General);
            }
            set => value.SaveAll(General);
        }

        public int Badges16
        {
            get => General[Trainer1 + 0x1F];
            set => General[Trainer1 + 0x1F] = (byte)value;
        }

        private const int OFS_GearRolodex = 0xC0EC;
        private const byte GearMaxCallers = (byte)(PokegearNumber.Ernest + 1);

        public PokegearNumber GetCallerAtIndex(int index) => (PokegearNumber)General[OFS_GearRolodex + index];
        public void SetCallerAtIndex(int index, PokegearNumber caller) => General[OFS_GearRolodex + index] = (byte)caller;

        public PokegearNumber[] PokeGearRoloDex
        {
            get
            {
                var arr = new PokegearNumber[GearMaxCallers];
                for (int i = 0; i < arr.Length; i++)
                    arr[i] = GetCallerAtIndex(i);
                return arr;
            }
            set
            {
                for (int i = 0; i < value.Length; i++)
                    SetCallerAtIndex(i, value[i]);
            }
        }

        public void PokeGearUnlockAllCallers()
        {
            for (int i = 0; i < GearMaxCallers; i++)
                SetCallerAtIndex(i, (PokegearNumber)i);
        }

        public void PokeGearClearAllCallers(int start = 0)
        {
            for (int i = start; i < GearMaxCallers; i++)
                SetCallerAtIndex(i, PokegearNumber.None);
        }

        public void PokeGearUnlockAllCallersNoTrainers()
        {
            var nonTrainers = new[]
            {
                PokegearNumber.Mother,
                PokegearNumber.Professor_Elm,
                PokegearNumber.Professor_Oak,
                PokegearNumber.Ethan,
                PokegearNumber.Lyra,
                PokegearNumber.Kurt,
                PokegearNumber.Daycare_Man,
                PokegearNumber.Daycare_Lady,
                PokegearNumber.Bill,
                PokegearNumber.Bike_Shop,
                PokegearNumber.Baoba,
            };
            for (int i = 0; i < nonTrainers.Length; i++)
                SetCallerAtIndex(i, nonTrainers[i]);

            // clear remaining callers
            PokeGearClearAllCallers(nonTrainers.Length);
        }

        // Apricorn Pouch
        public int GetApricornCount(int i) => General[0xE558 + i];
        public void SetApricornCount(int i, int count) => General[0xE558 + i] = (byte)count;

        // Pokewalker
        private const int OFS_WALKER = 0xE70C;

        public bool[] PokewalkerCoursesUnlocked
        {
            get
            {
                var val = BitConverter.ToUInt32(General, OFS_WALKER);
                bool[] courses = new bool[32];
                for (int i = 0; i < courses.Length; i++)
                    courses[i] = ((val >> i) & 1) == 1;
                return courses;
            }
            set
            {
                uint val = 0;
                bool[] courses = new bool[32];
                for (int i = 0; i < courses.Length; i++)
                    val |= value[i] ? 1u << i : 0;
                SetData(General, BitConverter.GetBytes(val), OFS_WALKER);
            }
        }

        public void PokewalkerCoursesUnlockAll() => SetData(General, BitConverter.GetBytes(0x07FF_FFFFu), OFS_WALKER);
    }
}