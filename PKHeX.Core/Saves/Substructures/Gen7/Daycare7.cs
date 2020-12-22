namespace PKHeX.Core
{
    public sealed class Daycare7 : SaveBlock
    {
        public const int DaycareSeedSize = 32; // 128 bits

        public Daycare7(SAV7SM sav, int offset) : base(sav) => Offset = offset;
        public Daycare7(SAV7USUM sav, int offset) : base(sav) => Offset = offset;

        public bool GetIsOccupied(int slot)
        {
            return Data[Offset + ((PokeCrypto.SIZE_6STORED + 1) * slot)] != 0;
        }

        public void SetOccupied(int slot, bool occupied)
        {
            Data[Offset + ((PokeCrypto.SIZE_6STORED + 1) * slot)] = occupied ? 1 : 0;
        }

        public int GetDaycareSlotOffset(int slot)
        {
            return Offset + 1 + (slot * (PokeCrypto.SIZE_6STORED + 1));
        }

        public bool HasEgg
        {
            get => Data[Offset + 0x1D8] == 1;
            set => Data[Offset + 0x1D8] = value ? 1 : 0;
        }

        public string RNGSeed
        {
            get => Util.GetHexStringFromBytes(Data, Offset + 0x1DC, DaycareSeedSize / 2);
            set
            {
                if (value.Length != DaycareSeedSize)
                    return;

                var data = Util.GetBytesFromHexString(value);
                SAV.SetData(data, Offset + 0x1DC);
            }
        }
    }
}