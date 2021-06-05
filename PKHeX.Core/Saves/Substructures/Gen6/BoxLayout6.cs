namespace PKHeX.Core
{
    public sealed class BoxLayout6 : SaveBlock, IBoxDetailName, IBoxDetailWallpaper
    {
        // gfstr5[31] boxNames;
        // byte[31] wallpapers;
        // byte Flags:7;
        // byte FinalBoxUnlocked:1;
        // byte UnlockedCount;
        // byte CurrentBox;

        private const int StringMaxByteCount = SAV6.LongStringLength; // same for both games
        private const int StringMaxLength = StringMaxByteCount / 2;
        private const int BoxCount = 31;
        private const int PCBackgrounds = BoxCount * StringMaxByteCount; // 0x41E;
        private const int PCFlags = PCBackgrounds + BoxCount;      // 0x43D;
        private const int Unlocked = PCFlags + 1;                  // 0x43E;
        private const int LastViewedBoxOffset = Unlocked + 1;      // 0x43F;

        public BoxLayout6(SAV6XY sav, int offset) : base(sav) => Offset = offset;
        public BoxLayout6(SAV6AO sav, int offset) : base(sav) => Offset = offset;

        public int GetBoxWallpaperOffset(int box) => Offset + PCBackgrounds + box;

        public int GetBoxWallpaper(int box)
        {
            if ((uint)box >= SAV.BoxCount)
                return 0;
            return Data[GetBoxWallpaperOffset(box)];
        }

        public void SetBoxWallpaper(int box, int value)
        {
            if ((uint)box >= SAV.BoxCount)
                return;
            Data[GetBoxWallpaperOffset(box)] = (byte)value;
        }

        private int GetBoxNameOffset(int box) => Offset + (StringMaxByteCount * box);

        public string GetBoxName(int box) => SAV.GetString(Data, GetBoxNameOffset(box), StringMaxByteCount);

        public void SetBoxName(int box, string value)
        {
            var data = SAV.SetString(value, StringMaxLength, StringMaxLength, 0);
            var offset = GetBoxNameOffset(box) + (StringMaxByteCount * box);
            SAV.SetData(data, offset);
        }

        public byte[] BoxFlags
        {
            get => new[] { Data[Offset + PCFlags] }; // 7 bits for wallpaper unlocks, top bit to unlock final box (delta episode)
            set
            {
                if (value.Length != 1)
                    return;
                Data[Offset + PCFlags] = value[0];
            }
        }

        public int BoxesUnlocked
        {
            get => Data[Offset + Unlocked];
            set
            {
                if (value > BoxCount)
                    value = BoxCount;
                if (value == BoxCount)
                    Data[Offset + PCFlags] |= 0x80; // set final box unlocked flag
                else
                    Data[Offset + PCFlags] &= 0x7F; // clear final box unlocked flag
                Data[Offset + Unlocked] = (byte)value;
            }
        }

        public int CurrentBox { get => Data[Offset + LastViewedBoxOffset]; set => Data[Offset + LastViewedBoxOffset] = (byte)value; }
    }
}