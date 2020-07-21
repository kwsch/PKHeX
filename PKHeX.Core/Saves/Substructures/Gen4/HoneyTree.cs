using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Honey Tree in Sinnoh (Gen4)
    /// </summary>
    public sealed class HoneyTree
    {
        public const int Size = 8;

        public readonly byte[] Data;

        public uint Time { get => BitConverter.ToUInt32(Data, 0); set => BitConverter.GetBytes(value).CopyTo(Data, 0); }
        public int Slot { get => Data[4]; set => Data[4] = (byte)value; }
        public int SubTable { get => Data[5]; set => Data[5] = (byte)value; } // offset by 1 with respect to Group
        public int Group { get => Data[6]; set { Data[6] = (byte)value; SubTable = Math.Max(0, Group - 1); } }
        public int Shake { get => Data[7]; set => Data[7] = (byte)value; }

        public HoneyTree(byte[] data)
        {
            Data = data;
        }

        public static readonly int[][] TableDP =
        {
            new[] {000, 000, 000, 000, 000, 000},
            new[] {265, 266, 415, 412, 420, 190},
            new[] {415, 412, 420, 190, 214, 265},
            new[] {446, 446, 446, 446, 446, 446},
        };

        public static readonly int[][] TablePt =
        {
            TableDP[0],
            new[] {415, 265, 412, 420, 190, 190},
            new[] {412, 420, 415, 190, 190, 214},
            TableDP[3],
        };
    }
}
