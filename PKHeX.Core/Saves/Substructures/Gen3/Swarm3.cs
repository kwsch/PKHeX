using System.Runtime.InteropServices;
using static PKHeX.Core.Move;
using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = SIZE)]
    public sealed class Swarm3
    {
        public const int SIZE = 0x14;

        public ushort Gen3Species { get; set; }
        public byte MapNum { get; set; }
        public byte MapGroup { get; set; }
        public byte Level { get; set; }
        public byte Unused1 { get; set; }
        public ushort Unused2 { get; set; }
        public ushort Move1 { get; set; }
        public ushort Move2 { get; set; }
        public ushort Move3 { get; set; }
        public ushort Move4 { get; set; }
        public byte Unused3 { get; set; }
        public byte EncounterProbability { get; set; }
        public ushort DaysLeft { get; set; }

        public Swarm3() {}

        public Swarm3(Species species, byte level, byte map, Move m1, Move m2 = 0, Move m3 = 0, Move m4 = 0)
        {
            Gen3Species = (ushort)SpeciesConverter.GetG3Species((int)species);
            Level = level;
            MapNum = map;
            Move1 = (ushort)m1;
            Move2 = (ushort)m2;
            Move3 = (ushort)m3;
            Move4 = (ushort)m4;
            EncounterProbability = 50;
            DaysLeft = 1337;
        }
    }

    public static class Swarm3Details
    {
        public static readonly Swarm3[] Swarms_RS =
        {
            new(Surskit, 03, 0x11, Bubble, QuickAttack), // Route 102
            new(Surskit, 15, 0x1D, Bubble, QuickAttack), // Route 114
            new(Surskit, 15, 0x20, Bubble, QuickAttack), // Route 117
            new(Surskit, 28, 0x23, Bubble, QuickAttack), // Route 120
            new(Skitty,  15, 0x1F, Growl,  Tackle),      // Route 116
        };

        public static readonly Swarm3[] Swarms_E =
        {
            new(Seedot,  03, 0x11, Bide,      Harden,      LeechSeed),              // Route 102
            new(Nuzleaf, 15, 0x1D, Harden,    Growth,      NaturePower, LeechSeed), // Route 114
            new(Seedot,  13, 0x20, Harden,    Growth,      NaturePower, LeechSeed), // Route 117
            new(Seedot,  25, 0x23, GigaDrain, Frustration, SolarBeam,   LeechSeed), // Route 120
            new(Skitty,  08, 0x1F, Growl,     Tackle,      TailWhip,    Attract),   // Route 116
        };
    }
}
