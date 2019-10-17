namespace PKHeX.Core
{
    public sealed class BoxLayout8 : SaveBlock
    {
        public const int BoxCount = 32; // todo

        private const int BattleBoxFlags = 0x4C4;
        private const int PCBackgrounds = 0x5C0;
        private const int PCFlags = 0x5E0;
        private const int Unlocked = 0x5E1;
        private const int LastViewedBoxOffset = 0x5E3;

        private const int strlen = SAV6XY.LongStringLength / 2;

        public BoxLayout8(SAV8SWSH sav, int offset) : base(sav) => Offset = offset;

        public int GetBoxWallpaperOffset(int box) => -1; // Offset + PCBackgrounds + box;

        public int GetBoxWallpaper(int box)
        {
            if ((uint)box > SAV.BoxCount)
                return 0;
            return Data[GetBoxWallpaperOffset(box)];
        }

        public void SetBoxWallpaper(int box, int value)
        {
            if ((uint)box > SAV.BoxCount)
                return;
            Data[GetBoxWallpaperOffset(box)] = (byte)value;
        }

        private int GetBoxNameOffset(int box) => Offset + (SAV6XY.LongStringLength * box);

        public string GetBoxName(int box)
        {
            return SAV.GetString(Data, GetBoxNameOffset(box), SAV6XY.LongStringLength);
        }

        public void SetBoxName(int box, string value)
        {
            var data = SAV.SetString(value, strlen, strlen, 0);
            var offset = GetBoxNameOffset(box) + (SAV6XY.LongStringLength * box);
            SAV.SetData(data, offset);
        }

        public byte[] BoxFlags
        {
            get => new[] { Data[Offset + PCFlags] }; // bits for wallpaper unlocks
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
                Data[Offset + Unlocked] = (byte)value;
            }
        }

        public int CurrentBox { get => Data[Offset + LastViewedBoxOffset]; set => Data[Offset + LastViewedBoxOffset] = (byte)value; }

        public string this[int i]
        {
            get => GetBoxName(i);
            set => SetBoxName(i, value);
        }
    }
}