using System;

namespace PKHeX.Core
{
    public sealed class ResortSave7 : SaveBlock
    {
        public ResortSave7(SAV7SM sav, int offset) : base(sav) => Offset = offset;
        public ResortSave7(SAV7USUM sav, int offset) : base(sav) => Offset = offset;

        public const int ResortCount = 93;
        public int GetResortSlotOffset(int slot) => Offset + 0x16 + (slot * PokeCrypto.SIZE_6STORED);

        public PKM[] ResortPKM
        {
            get
            {
                PKM[] data = new PKM[ResortCount];
                for (int i = 0; i < data.Length; i++)
                {
                    var bytes = SAV.GetData(GetResortSlotOffset(i), PokeCrypto.SIZE_6STORED);
                    data[i] = new PK7(bytes) { Identifier = $"Resort Slot {i}" };
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

        public int GetPokebeanCount(int bean_id)
        {
            if ((uint)bean_id > 14)
                throw new ArgumentException("Invalid bean id!");
            return Data[Offset + 0x564C + bean_id];
        }

        public void SetPokebeanCount(int bean_id, int count)
        {
            if ((uint)bean_id > 14)
                throw new ArgumentException("Invalid bean id!");
            if (count < 0)
                count = 0;
            if (count > 255)
                count = 255;
            Data[Offset + 0x564C + bean_id] = (byte)count;
        }
    }
}