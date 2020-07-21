namespace PKHeX.Core
{
    internal class SAV1Offsets
    {
        public static readonly SAV1Offsets INT = GetINT();
        public static readonly SAV1Offsets JPN = GetJPN();

        private static SAV1Offsets GetINT() => new SAV1Offsets
        {
            DexCaught = 0x25A3,
            DexSeen = 0x25B6,
            Items = 0x25C9,
            Money = 0x25F3,
            Options = 0x2601,
            Badges = 0x2602,
            TID = 0x2605,
            PikaFriendship = 0x271C,
            PikaBeachScore = 0x2741,
            PrinterBrightness = 0x2744,
            PCItems = 0x27E6,
            CurrentBoxIndex = 0x284C,
            Coin = 0x2850,
            ObjectSpawnFlags = 0x2852, // 2 bytes after Coin
            Starter = 0x29C3,
            EventFlag = 0x29F3,
            PlayTime = 0x2CED,
            Daycare = 0x2CF4,
            Party = 0x2F2C,
            CurrentBox = 0x30C0,
            ChecksumOfs = 0x3523,
        };

        private static SAV1Offsets GetJPN() => new SAV1Offsets
        {
            DexCaught = 0x259E,
            DexSeen = 0x25B1,
            Items = 0x25C4,
            Money = 0x25EE,
            Options = 0x25F7,
            Badges = 0x25F8,
            TID = 0x25FB,
            PikaFriendship = 0x2712,
            PikaBeachScore = 0x2737,
            PrinterBrightness = 0x273A,
            PCItems = 0x27DC,
            CurrentBoxIndex = 0x2842,
            Coin = 0x2846,
            ObjectSpawnFlags = 0x2848, // 2 bytes after Coin
            Starter = 0x29B9,
            EventFlag = 0x29E9,
            PlayTime = 0x2CA0,
            Daycare = 0x2CA7,
            Party = 0x2ED5,
            CurrentBox = 0x302D,
            ChecksumOfs = 0x3594,
        };

        public int OT { get; } = 0x2598;
        public int DexCaught { get; private set; }
        public int DexSeen { get; private set; }
        public int Items { get; private set; }
        public int Money { get; private set; }
        public int Options { get; private set; }
        public int Badges { get; private set; }
        public int TID { get; private set; }
        public int PikaFriendship { get; private set; }
        public int PikaBeachScore { get; private set; }
        public int PrinterBrightness { get; private set; }
        public int PCItems { get; private set; }
        public int CurrentBoxIndex { get; private set; }
        public int Coin { get; private set; }
        public int ObjectSpawnFlags { get; private set; }
        public int Starter { get; private set; }
        public int EventFlag { get; private set; }
        public int PlayTime { get; private set; }
        public int Daycare { get; private set; }
        public int Party { get; private set; }
        public int CurrentBox { get; private set; }
        public int ChecksumOfs { get; private set; }
    }
}
