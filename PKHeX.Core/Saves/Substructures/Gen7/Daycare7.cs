using System;

namespace PKHeX.Core;

public sealed class Daycare7(SAV7 sav, Memory<byte> raw) : SaveBlock<SAV7>(sav, raw)
{
    public const int DaycareSeedSize = 32; // 128 bits

    public bool GetIsOccupied(int slot)
    {
        return Data[((PokeCrypto.SIZE_6STORED + 1) * slot)] != 0;
    }

    public void SetOccupied(int slot, bool occupied)
    {
        Data[((PokeCrypto.SIZE_6STORED + 1) * slot)] = occupied ? (byte)1 : (byte)0;
    }

    public int GetDaycareSlotOffset(int slot)
    {
        return 1 + (slot * (PokeCrypto.SIZE_6STORED + 1));
    }

    public bool HasEgg
    {
        get => Data[0x1D8] == 1;
        set => Data[0x1D8] = value ? (byte)1 : (byte)0;
    }

    public string RNGSeed
    {
        get => Util.GetHexStringFromBytes(Data.Slice(0x1DC, DaycareSeedSize / 2));
        set
        {
            if (value.Length != DaycareSeedSize)
                return;

            var data = Util.GetBytesFromHexString(value);
            SAV.SetData(Data[0x1DC..], data);
        }
    }
}
