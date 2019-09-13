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
        public static T[] GetArray<T>(byte[][] entries) where T : EncounterArea32, new()
        {
            T[] data = new T[entries.Length];
            for (int i = 0; i < data.Length; i++)
            {
                var loc = data[i] = new T();
                loc.LoadSlots(entries[i]);
            }
            return data;
        }

        private void LoadSlots(byte[] areaData)
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