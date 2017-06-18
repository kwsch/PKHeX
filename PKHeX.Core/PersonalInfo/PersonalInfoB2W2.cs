using System.Linq;

namespace PKHeX.Core
{
    public class PersonalInfoB2W2 : PersonalInfoBW
    {
        public new const int SIZE = 0x4C;
        public PersonalInfoB2W2(byte[] data)
        {
            if (data.Length != SIZE)
                return;
            Data = data;

            // Unpack TMHM & Tutors
            TMHM = GetBits(Data.Skip(0x28).Take(0x10).ToArray());
            TypeTutors = GetBits(Data.Skip(0x38).Take(0x4).ToArray());
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
    }
}
