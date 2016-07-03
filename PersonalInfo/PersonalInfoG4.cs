using System.Linq;

namespace PKHeX
{
    public class PersonalInfoG4 : PersonalInfoG3
    {
        public PersonalInfoG4(byte[] data)
        {
            if (data.Length != SIZE_G4)
                return;
            Data = data;

            // Unpack TMHM & Tutors
            TMHM = getBits(Data.Skip(0x1C).Take(0x0D).ToArray());
            TypeTutors = new bool[0]; // not stored in personal
        }
        public override byte[] Write()
        {
            setBits(TMHM).CopyTo(Data, 0x28);
            // setBits(TypeTutors).CopyTo(Data, 0x38);
            return Data;
        }
    }
}
