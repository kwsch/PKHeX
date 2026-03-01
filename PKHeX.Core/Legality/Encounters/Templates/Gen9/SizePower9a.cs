using System;
using static PKHeX.Core.SizePower9a;

namespace PKHeX.Core;

public enum SizePower9a : byte
{
    Normal,
    Teensy1,
    Teensy2,
    Teensy3,
    Humungo1,
    Humungo2,
    Humungo3,
}

public static class SizePower9aExtensions
{
    extension(SizePower9a type)
    {
        /// <summary>
        /// Gets a random size value for the specified <see cref="SizePower9a"/>.
        /// </summary>
        /// <param name="rand">RNG to generate value with.</param>
        /// <returns>Size Value</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public byte GetSizeValue(ref Xoroshiro128Plus rand) => type switch
        {
            Normal => (byte)(rand.NextInt(0x81) + rand.NextInt(0x80)), // triangular distribution
            Teensy1 => (byte)rand.NextInt(128),
            Teensy2 => (byte)rand.NextInt(96),
            Teensy3 => (byte)rand.NextInt(32),
            Humungo1 => (byte)(rand.NextInt(128) + 128),
            Humungo2 => (byte)(rand.NextInt(96) + 160),
            Humungo3 => (byte)(rand.NextInt(32) + 224),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };

        /// <summary>
        /// Checks if the specified value is within the range of the <see cref="SizePower9a"/>.
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if the value is within the range of the <see cref="SizePower9a"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public bool IsWithinRange(byte value) => type switch
        {
            Normal => true,
            Teensy1 => value < 128,
            Teensy2 => value < 96,
            Teensy3 => value < 32,
            Humungo1 => value >= 128,
            Humungo2 => value >= 160,
            Humungo3 => value >= 224,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
    }
}
