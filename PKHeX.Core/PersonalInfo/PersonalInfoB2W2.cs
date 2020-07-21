namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="PersonalInfo"/> class with values from the Black 2 &amp; White 2 games.
    /// </summary>
    public sealed class PersonalInfoB2W2 : PersonalInfoBW
    {
        public new const int SIZE = 0x4C;

        public PersonalInfoB2W2(byte[] data) : base(data)
        {
            // Unpack TMHM & Tutors
            TMHM = GetBits(Data, 0x28, 0x10);
            TypeTutors = GetBits(Data, 0x38, 0x4);
            SpecialTutors = new[]
            {
                GetBits(Data, 0x3C, 0x04),
                GetBits(Data, 0x40, 0x04),
                GetBits(Data, 0x44, 0x04),
                GetBits(Data, 0x48, 0x04),
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
