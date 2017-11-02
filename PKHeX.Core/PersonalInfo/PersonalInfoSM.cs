using System;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="PersonalInfo"/> class with values from the Sun/Moon games.
    /// </summary>
    public class PersonalInfoSM : PersonalInfoXY
    {
        public new const int SIZE = 0x54;
        public PersonalInfoSM(byte[] data)
        {
            if (data.Length != SIZE)
                return;
            Data = data;

            TMHM = GetBits(Data.Skip(0x28).Take(0x10).ToArray()); // 36-39
            TypeTutors = GetBits(Data.Skip(0x38).Take(0x4).ToArray()); // 40

            SpecialTutors = new[]
            {
                GetBits(Data.Skip(0x3C).Take(0x04).ToArray()),
                GetBits(Data.Skip(0x40).Take(0x04).ToArray()),
                GetBits(Data.Skip(0x44).Take(0x04).ToArray()),
                GetBits(Data.Skip(0x48).Take(0x04).ToArray()),
            };
        }
        public override byte[] Write()
        {
            SetBits(TMHM).CopyTo(Data, 0x28);
            SetBits(TypeTutors).CopyTo(Data, 0x38);
            SetBits(SpecialTutors[0]).CopyTo(Data, 0x3C);
            SetBits(SpecialTutors[1]).CopyTo(Data, 0x40);
            SetBits(SpecialTutors[2]).CopyTo(Data, 0x44);
            SetBits(SpecialTutors[3]).CopyTo(Data, 0x48);
            return Data;
        }
        
        // No accessing for 3C-4B

        public int SpecialZ_Item { get => BitConverter.ToUInt16(Data, 0x4C); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x4C); }
        public int SpecialZ_BaseMove { get => BitConverter.ToUInt16(Data, 0x4E); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x4E); }
        public int SpecialZ_ZMove { get => BitConverter.ToUInt16(Data, 0x50); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x50); }
        public bool LocalVariant { get => Data[0x52] == 1; set => Data[0x52] = (byte)(value ? 1 : 0); }
    }
}
