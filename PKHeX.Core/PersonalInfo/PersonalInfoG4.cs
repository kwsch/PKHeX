using System;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="PersonalInfo"/> class with values from Generation 4 games.
    /// </summary>
    public class PersonalInfoG4 : PersonalInfoG3
    {
        public new const int SIZE = 0x2C;
        public PersonalInfoG4(byte[] data)
        {
            if (data.Length != SIZE)
                return;
            Data = data;

            // Unpack TMHM & Tutors
            TMHM = GetBits(Data.Skip(0x1C).Take(0x0D).ToArray());
            TypeTutors = new bool[0]; // not stored in personal
        }
        public override byte[] Write()
        {
            SetBits(TMHM).CopyTo(Data, 0x1C);
            return Data;
        }

        // Manually added attributes
        public override int FormeCount { get => Data[0x29]; set {} }
        protected internal override int FormStatsIndex { get => BitConverter.ToUInt16(Data, 0x2A); set {} }
    }
}
