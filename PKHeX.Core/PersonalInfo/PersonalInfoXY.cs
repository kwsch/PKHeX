namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="PersonalInfo"/> class with values from the X &amp; Y games.
    /// </summary>
    public class PersonalInfoXY : PersonalInfoBW
    {
        public new const int SIZE = 0x40;

        public PersonalInfoXY(byte[] data) : base(data)
        {
            // Unpack TMHM & Tutors
            TMHM = GetBits(Data, 0x28, 0x10);
            TypeTutors = GetBits(Data, 0x38, 0x4);
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
