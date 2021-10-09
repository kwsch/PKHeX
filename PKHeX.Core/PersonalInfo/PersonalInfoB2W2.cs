using System;

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
            TMHM = GetBits(Data.AsSpan(0x28, 0x10));
            TypeTutors = GetBits(Data.AsSpan(0x38, 0x4));
            SpecialTutors = new[]
            {
                GetBits(Data.AsSpan(0x3C, 0x04)),
                GetBits(Data.AsSpan(0x40, 0x04)),
                GetBits(Data.AsSpan(0x44, 0x04)),
                GetBits(Data.AsSpan(0x48, 0x04)),
            };
        }

        public override byte[] Write()
        {
            SetBits(TMHM, Data.AsSpan(0x28));
            SetBits(TypeTutors, Data.AsSpan(0x38));
            SetBits(SpecialTutors[0], Data.AsSpan(0x3C));
            SetBits(SpecialTutors[1], Data.AsSpan(0x40));
            SetBits(SpecialTutors[2], Data.AsSpan(0x44));
            SetBits(SpecialTutors[3], Data.AsSpan(0x48));
            return Data;
        }
    }
}
