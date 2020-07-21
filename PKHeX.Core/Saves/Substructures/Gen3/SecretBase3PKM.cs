using System;
using System.Linq;

namespace PKHeX.Core
{
    public sealed class SecretBase3PKM
    {
        public uint PID { get; set; }
        public int Move1 { get; set; }
        public int Move2 { get; set; }
        public int Move3 { get; set; }
        public int Move4 { get; set; }
        public int Species { get; set; }
        public int HeldItem { get; set; }
        public int Level { get; set; }
        public int EVAll { get; set; }
        public int[] Moves => new[] { Move1, Move2, Move3, Move4 };

        public string Summary
        {
            get
            {
                var first = $"{Species:000} - {GameInfo.Strings.Species[Species]}";
                if (HeldItem != 0)
                    first += $"@ {GameInfo.Strings.Item[HeldItem]}";
                var second = $"Moves: {string.Join(" / ", Moves.Select(z => GameInfo.Strings.Move[z]))}";
                var third = $"Level: {Level}, EVs: {EVAll}, PID: {PID}";
                return first + Environment.NewLine + second + Environment.NewLine + third;
            }
        }
    }
}