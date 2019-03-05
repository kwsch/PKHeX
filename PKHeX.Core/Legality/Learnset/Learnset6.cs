using System;
using System.IO;

namespace PKHeX.Core
{
    /// <summary>
    /// Level Up Learn Movepool Information
    /// </summary>
    public sealed class Learnset6 : Learnset
    {
        private Learnset6(byte[] data)
        {
            if (data.Length < 4 || data.Length % 4 != 0)
            { Count = 0; Levels = Moves = Array.Empty<int>(); return; }
            Count = (data.Length / 4) - 1;
            Moves = new int[Count];
            Levels = new int[Count];
            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms))
            {
                for (int i = 0; i < Count; i++)
                {
                    Moves[i] = br.ReadInt16();
                    Levels[i] = br.ReadInt16();
                }
            }
        }

        public static Learnset[] GetArray(byte[][] entries)
        {
            Learnset[] data = new Learnset[entries.Length];
            for (int i = 0; i < data.Length; i++)
                data[i] = new Learnset6(entries[i]);
            return data;
        }
    }
}