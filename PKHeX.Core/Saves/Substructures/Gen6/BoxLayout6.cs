namespace PKHeX.Core
{
    public class BoxLayout6 : SaveBlock
    {
        private const int PCBackgrounds = 0x41E;
        private const int PCFlags = 0x43D;
        private const int LastViewedBoxOffset = 0x43F;

        public BoxLayout6(SAV6 sav, int offset) : base(sav) => Offset = offset;

        public int BoxesUnlocked { get => Data[Offset + PCFlags + 1] - 1; set => Data[Offset + PCFlags + 1] = (byte)(value + 1); }

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

        public  int GetBoxWallpaperOffset(int box) => Offset + PCBackgrounds + box;

        public int GetBoxWallpaper(int box)
        {
            if ((uint) box > SAV.BoxCount)
                return 0;
            return Data[Offset + PCBackgrounds + box];
        }

        public void SetBoxWallpaper(int box, int value)
        {
            if ((uint)box > SAV.BoxCount)
                return;
            Data[Offset + PCBackgrounds + box] = (byte)value;
        }

        private int GetBoxNameOffset(int box) => Offset + (SAV6.LongStringLength * box);

        public string GetBoxName(int box)
        {
            return SAV.GetString(Data, GetBoxNameOffset(box), SAV6.LongStringLength);
        }

        private const int strlen = SAV6.LongStringLength / 2;

        public void SetBoxName(int box, string value)
        {
            var data = SAV.SetString(value, strlen, strlen, 0);
            var offset = GetBoxNameOffset(box) + (SAV6.LongStringLength * box);
            SAV.SetData(data, offset);
        }

        public int CurrentBox { get => Data[LastViewedBoxOffset]; set => Data[LastViewedBoxOffset] = (byte)value; }
    }
}