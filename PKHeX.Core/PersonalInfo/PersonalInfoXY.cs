using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="PersonalInfo"/> class with values from the X & Y games.
    /// </summary>
    public class PersonalInfoXY : PersonalInfoBW
    {
        protected PersonalInfoXY() { } // For ORAS
        public new const int SIZE = 0x40;
        public PersonalInfoXY(byte[] data)
        {
            if (data.Length != SIZE)
                return;
            Data = data;

            // Unpack TMHM & Tutors
            TMHM = GetBits(Data.Skip(0x28).Take(0x10).ToArray());
            TypeTutors = GetBits(Data.Skip(0x38).Take(0x4).ToArray());
            // 0x3C-0x40 unknown
        }
        public override byte[] Write()
        {
            SetBits(TMHM).CopyTo(Data, 0x28);
            SetBits(TypeTutors).CopyTo(Data, 0x38);
            return Data;
        }
    }
}
