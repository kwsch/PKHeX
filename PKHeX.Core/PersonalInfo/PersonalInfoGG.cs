using System;

namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="PersonalInfo"/> class with values from the LGPE games.
    /// </summary>
    public class PersonalInfoGG : PersonalInfoSM
    {
        public PersonalInfoGG(byte[] data)
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
