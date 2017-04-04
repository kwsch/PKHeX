using System;
using System.Linq;

namespace PKHeX.Core
{
    public class PersonalInfoSM : PersonalInfoXY
    {
        public new const int SIZE = 0x54;
        public PersonalInfoSM(byte[] data)
        {
            if (data.Length != SIZE)
                return;
            Data = data;

            TMHM = getBits(Data.Skip(0x28).Take(0x10).ToArray()); // 36-39
            TypeTutors = getBits(Data.Skip(0x38).Take(0x4).ToArray()); // 40
        }
        public override byte[] Write()
        {
            return Data;
        }
        
        // No accessing for 3C-4B

        public int SpecialZ_Item { get { return BitConverter.ToUInt16(Data, 0x4C); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x4C); } }
        public int SpecialZ_BaseMove { get { return BitConverter.ToUInt16(Data, 0x4E); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x4E); } }
        public int SpecialZ_ZMove { get { return BitConverter.ToUInt16(Data, 0x50); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x50); } }
        public bool LocalVariant { get { return Data[0x52] == 1; } set { Data[0x52] = (byte)(value ? 1 : 0); } }
    }
}
