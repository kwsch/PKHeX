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
        /// <summary>
        /// Gets an array of areas from an array of raw area data
        /// </summary>
        /// <param name="entries">Simplified raw format of an Area</param>
        /// <returns>Array of areas</returns>
        public static A[] GetArray<A, S>(byte[][] entries)
            where A : EncounterArea32, new()
            where S : EncounterSlot, new()
        {
            var data = new A[entries.Length];
            for (int i = 0; i < data.Length; i++)
            {
                var loc = data[i] = new A();
                loc.LoadSlots<S>(entries[i]);
            }
            return data;
        }

        private void LoadSlots<S>(byte[] areaData) where S : EncounterSlot, new()
        {
            var count = (areaData.Length - 2) / 4;
            Location = BitConverter.ToUInt16(areaData, 0);
            Slots = new EncounterSlot[count];
            for (int i = 0; i < Slots.Length; i++)
            {
                int ofs = 2 + (i * 4);
                ushort SpecForm = BitConverter.ToUInt16(areaData, ofs);
                Slots[i] = new S
                {
                    Area = this,
                    Species = SpecForm & 0x7FF,
                    Form = SpecForm >> 11,
                    LevelMin = areaData[ofs + 2],
                    LevelMax = areaData[ofs + 3],
                };
            }
        }
    }
}