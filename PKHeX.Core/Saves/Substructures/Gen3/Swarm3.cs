using System.Runtime.InteropServices;

namespace PKHeX.Core
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = SIZE)]
    public sealed class Swarm3
    {
        public const int SIZE = 0x14;

        public ushort Species { get; set; }
        public byte MapNum { get; set; }
        public byte MapGroup { get; set; }
        public byte Level { get; set; }
        // 3byte align
        public ushort Move1 { get; set; }
        public ushort Move2 { get; set; }
        public ushort Move3 { get; set; }
        public ushort Move4 { get; set; }
        public byte Zero { get; set; }
        public byte EncounterProbability { get; set; }
        public ushort DaysLeft { get; set; }
    }
}
