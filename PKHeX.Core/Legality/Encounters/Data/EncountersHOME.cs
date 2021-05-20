using System;
using System.Collections.Generic;
using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    public static class EncountersHOME
    {
        public static bool IsValidDateWC8(int species, DateTime obtained) => WC8Gifts.TryGetValue(species, out var time) && obtained >= time && obtained <= DateTime.UtcNow;

        /// <summary>
        /// Minimum date the gift can be received.
        /// </summary>
        public static readonly Dictionary<int, DateTime> WC8Gifts = new()
        {
            {(int) Bulbasaur,  new DateTime(2020, 02, 12)},
            {(int) Charmander, new DateTime(2020, 02, 12)},
            {(int) Squirtle,   new DateTime(2020, 02, 12)},
            {(int) Pikachu,    new DateTime(2020, 02, 12)},
            {(int) Eevee,      new DateTime(2020, 02, 12)},
            {(int) Pichu,      new DateTime(2020, 02, 12)},
            {(int) Rotom,      new DateTime(2020, 02, 12)},
            {(int) Magearna,   new DateTime(2020, 02, 15)},
            {(int) Zeraora,    new DateTime(2020, 06, 30)},
            {(int) Melmetal,   new DateTime(2020, 11, 10)},
            {(int) Grookey,    new DateTime(2020, 06, 02)},
            {(int) Scorbunny,  new DateTime(2020, 06, 02)},
            {(int) Sobble,     new DateTime(2020, 06, 02)},
        };
    }
}
