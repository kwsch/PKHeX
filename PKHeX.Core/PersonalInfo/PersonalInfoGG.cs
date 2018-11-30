using System;

namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="PersonalInfo"/> class with values from the LGPE games.
    /// </summary>
    public class PersonalInfoGG : PersonalInfoSM
    {
        public PersonalInfoGG(byte[] data) : base(data) { }
        public int GoSpecies { get => BitConverter.ToUInt16(Data, 0x48); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x48); }
    }
}
