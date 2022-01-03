using System;

namespace PKHeX.Core
{
    public sealed class BoxLayout8 : SaveBlock, IBoxDetailName
    {
        public const int BoxCount = 32;

        private const int StringMaxLength = SAV6.LongStringLength / 2;

        public BoxLayout8(SAV8SWSH sav, SCBlock block) : base(sav, block.Data) { }

        private static int GetBoxNameOffset(int box) => SAV6.LongStringLength * box;
        private Span<byte> GetBoxNameSpan(int box) => Data.AsSpan(GetBoxNameOffset(box), SAV6.LongStringLength);
        public string GetBoxName(int box) => SAV.GetString(GetBoxNameSpan(box));
        public void SetBoxName(int box, string value) => SAV.SetString(GetBoxNameSpan(box), value.AsSpan(), StringMaxLength, StringConverterOption.ClearZero);

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