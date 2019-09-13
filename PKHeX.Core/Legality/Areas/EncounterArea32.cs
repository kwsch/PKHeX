using System;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// Base encounter class for manually repacked areas
    /// </summary>
    /// <remarks>
    /// Encounter Data is stored in the following format: (u16 Location, n*[u16 Species/Form, u8 Min, u8 Max]), hence the 32bit name
    /// </remarks>
    public abstract class EncounterArea32 : EncounterArea
    {
        protected internal void LoadSlots(byte[] areaData)
        {
            var count = (areaData.Length - 2) / 4;
            Location = BitConverter.ToUInt16(areaData, 0);
            Slots = new EncounterSlot[count];
            for (int i = 0; i < Slots.Length; i++)
            {
                int ofs = 2 + (i * 4);
                ushort SpecForm = BitConverter.ToUInt16(areaData, ofs);
                Slots[i] = new EncounterSlot
                {
                    Species = SpecForm & 0x7FF,
                    Form = SpecForm >> 11,
                    LevelMin = areaData[ofs + 2],
                    LevelMax = areaData[ofs + 3],
                };
            }
            foreach (var slot in Slots)
                slot.Area = this;
        }

        protected static EncounterSlot GetPressureSlot(EncounterSlot s, PKM pkm)
        {
            var max = s.Clone();
            max.Permissions.Pressure = true;
            max.Form = pkm.AltForm;
            return max;
        }
    }
}