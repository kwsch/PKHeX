namespace PKHeX.Core
{
    public sealed class BoxLayout5 : SaveBlock
    {
        public BoxLayout5(SAV5BW sav, int offset) : base(sav) => Offset = offset;
        public BoxLayout5(SAV5B2W2 sav, int offset) : base(sav) => Offset = offset;

        public int CurrentBox { get => Data[Offset]; set => Data[Offset] = (byte)value; }
        public int GetBoxNameOffset(int box) => Offset + (0x28 * box) + 4;
        public int GetBoxWallpaperOffset(int box) => Offset + 0x3C4 + box;

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

        public string GetBoxName(int box)
        {
            if (box >= SAV.BoxCount)
                return string.Empty;
            return SAV.GetString(GetBoxNameOffset(box), 0x14);
        }

        public void SetBoxName(int box, string value)
        {
            if (value.Length > 0x26 / 2)
                return;
            var data = SAV.SetString(value + '\uFFFF', 0x14, 0x14, 0);
            SAV.SetData(data, GetBoxNameOffset(box));
        }

        public string this[int i]
        {
            get => GetBoxName(i);
            set => SetBoxName(i, value);
        }
    }
}