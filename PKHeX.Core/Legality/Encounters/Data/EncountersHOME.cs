using System;
using System.Collections.Generic;
using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    public static class EncountersHOME
    {
        public static bool IsValidDateWC8(int cardID, DateTime obtained) => WC8Gifts.TryGetValue(cardID, out var time) && obtained >= time && obtained <= DateTime.UtcNow;

        /// <summary>
        /// Minimum date the gift can be received.
        /// </summary>
        public static readonly Dictionary<int, DateTime> WC8Gifts = new()
        {
            {9000, new DateTime(2020, 02, 12)}, // Bulbasaur
            {9001, new DateTime(2020, 02, 12)}, // Charmander
            {9002, new DateTime(2020, 02, 12)}, // Squirtle
            {9003, new DateTime(2020, 02, 12)}, // Pikachu
            {9004, new DateTime(2020, 02, 15)}, // Original Color Magearna
            {9005, new DateTime(2020, 02, 12)}, // Eevee
            {9006, new DateTime(2020, 02, 12)}, // Rotom
            {9007, new DateTime(2020, 02, 12)}, // Pichu
            {9008, new DateTime(2020, 06, 02)}, // Hidden Ability Grookey
            {9009, new DateTime(2020, 06, 02)}, // Hidden Ability Scorbunny
            {9010, new DateTime(2020, 06, 02)}, // Hidden Ability Sobble
            {9011, new DateTime(2020, 06, 30)}, // Shiny Zeraora
            {9012, new DateTime(2020, 11, 10)}, // Gigantamax Melmetal
            {9013, new DateTime(2021, 06, 17)}, // Gigantamax Bulbasaur
            {9014, new DateTime(2021, 06, 17)}, // Gigantamax Squirtle
        };
    }
}
