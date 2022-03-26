using System;

namespace PKHeX.Core
{
    public sealed class ResortSave7 : SaveBlock
    {
        public ResortSave7(SAV7SM sav, int offset) : base(sav) => Offset = offset;
        public ResortSave7(SAV7USUM sav, int offset) : base(sav) => Offset = offset;

        public const int ResortCount = 93;
        public int GetResortSlotOffset(int slot) => Offset + 0x16 + (slot * PokeCrypto.SIZE_6STORED);

        public PK7[] ResortPKM
        {
            get
            {
                PK7[] data = new PK7[ResortCount];
                for (int i = 0; i < data.Length; i++)
                {
                    var bytes = SAV.GetData(GetResortSlotOffset(i), PokeCrypto.SIZE_6STORED);
                    data[i] = new PK7(bytes);
                }
                return data;
            }
            set
            {
                if (value.Length != ResortCount)
                    throw new ArgumentException(nameof(ResortCount));

                for (int i = 0; i < value.Length; i++)
                    SAV.SetSlotFormatStored(value[i], Data, GetResortSlotOffset(i));
            }
        }

        public const int BEANS_MAX = 15;
        public Span<byte> GetBeans() => Data.AsSpan(Offset + 0x564C, BEANS_MAX);
        public void ClearBeans() => GetBeans().Clear();
        public void FillBeans(byte value = 255) => GetBeans().Fill(value);

        public int GetPokebeanCount(int bean_id) => GetBeans()[bean_id];

        public void SetPokebeanCount(int bean_id, int count)
        {
            if (count < 0)
                count = 0;
            if (count > 255)
                count = 255;
            GetBeans()[bean_id] = (byte)count;
        }

        /// <summary>
        /// Utility to indicate the bean pouch indexes.
        /// </summary>
        public static string[] GetBeanIndexNames()
        {
            var colors = Enum.GetNames(typeof(BeanColor7));
            return GetBeanIndexNames(colors);
        }

        private static string[] GetBeanIndexNames(string[] colors)
        {
            // 7 regular, 7 patterned, one rainbow
            var beans = new string[(colors.Length * 2) + 1];
            for (int i = 0; i < colors.Length; i++)
            {
                var z = colors[i];
                beans[i] = $"{z} Bean";
                beans[i + colors.Length] = $"{z} Patterned Bean";
            }
            beans[^1] = "Rainbow Bean";
            return beans;
        }
    }

    public enum BeanColor7 : byte
    {
        Red,
        Blue,
        LightBlue,
        Green,
        Yellow,
        Purple,
        Orange,
    }
}
