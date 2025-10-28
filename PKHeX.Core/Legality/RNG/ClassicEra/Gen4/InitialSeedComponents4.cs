using System.ComponentModel.DataAnnotations;

namespace PKHeX.Core
{
    /// <summary>
    /// Stores the components of an initial seed from Generation 4.
    /// </summary>
    public readonly record struct InitialSeedComponents4
    {
        [Range(0, 99)] public required byte Year { get; init; }
        [Range(1, 12)] public required byte Month { get; init; }
        [Range(1, 31)] public required byte Day { get; init; }
        [Range(0, 23)] public required byte Hour { get; init; }
        [Range(0, 59)] public required byte Minute { get; init; }
        [Range(0, 59)] public required byte Second { get; init; }

        public required ushort Delay { get; init; } // essentially XXX-65535, but can overflow. Not that anyone waits the 30+ minutes to do that since other initial seeds are more efficient.

        public uint ToSeed() => ClassicEraRNG.GetInitialSeed(Year, Month, Day, Hour, Minute, Second, Delay);
        public bool IsInvalid() => Month == 0;
    }
}
