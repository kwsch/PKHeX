namespace PKHeX.Core
{
    public sealed class BoxLayout8 : SaveBlock
    {
        public const int BoxCount = 32;

        private const int strlen = SAV6.LongStringLength / 2;

        public BoxLayout8(SAV8SWSH sav, SCBlock block) : base(sav, block.Data) { }

        private static int GetBoxNameOffset(int box) => SAV6.LongStringLength * box;

        public string GetBoxName(int box)
        {
            return SAV.GetString(Data, GetBoxNameOffset(box), SAV6.LongStringLength);
        }

        public void SetBoxName(int box, string value)
        {
            var data = SAV.SetString(value, strlen, strlen, 0);
            var offset = GetBoxNameOffset(box);
            SAV.SetData(Data, data, offset);
        }

        public string this[int i]
        {
            get => GetBoxName(i);
            set => SetBoxName(i, value);
        }

        public int CurrentBox
        {
            get => ((SAV8SWSH)SAV).GetValue<byte>(SaveBlockAccessor8SWSH.KCurrentBox);
            set => ((SAV8SWSH)SAV).SetValue(SaveBlockAccessor8SWSH.KCurrentBox, (byte)value);
        }
    }
}