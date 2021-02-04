using System;
using System.Collections.Generic;
using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    public static class EncountersHOME
    {
        public static bool IsValidDateWC8(int species, DateTime obtained) => WC8Gifts.TryGetValue(species, out var time) && obtained >= time && obtained <= DateTime.Today.AddDays(1);

        /// <summary>
        /// Minimum date the gift can be received.
        /// </summary>
        public static readonly Dictionary<int, DateTime> WC8Gifts = new()
        {
            {(int) Bulbasaur,  new DateTime(2020, 12, 02)},
            {(int) Charmander, new DateTime(2020, 12, 02)},
            {(int) Squirtle,   new DateTime(2020, 12, 02)},
            {(int) Pikachu,    new DateTime(2020, 12, 02)},
            {(int) Eevee,      new DateTime(2020, 12, 02)},
            {(int) Pichu,      new DateTime(2020, 12, 02)},
            {(int) Rotom,      new DateTime(2020, 12, 02)},
            {(int) Magearna,   new DateTime(2020, 15, 02)},
            {(int) Zeraora,    new DateTime(2020, 30, 06)},
            {(int) Melmetal,   new DateTime(2020, 10, 11)},
            {(int) Grookey,    new DateTime(2020, 02, 06)},
            {(int) Scorbunny,  new DateTime(2020, 02, 06)},
            {(int) Sobble,     new DateTime(2020, 02, 06)},
        };
    }
}
