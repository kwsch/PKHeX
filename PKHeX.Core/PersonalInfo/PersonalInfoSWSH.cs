using System;

namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="PersonalInfo"/> class with values from the <see cref="GameVersion.SWSH"/> games.
    /// </summary>
    public class PersonalInfoSWSH : PersonalInfoSM
    {
        public new const int SIZE = PersonalInfoSM.SIZE;

        // todo: this is a copy of lgpe class
        public PersonalInfoSWSH(byte[] data)
        {
            if (data.Length != SIZE)
                return;
            Data = data;

            TMHM = GetBits(Data, 0x28, 8); // only 60 TMs used
            TypeTutors = GetBits(Data, 0x38, 1); // at most 8 flags used
        }

        public int GoSpecies { get => BitConverter.ToUInt16(Data, 0x48); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x48); }
    }
}