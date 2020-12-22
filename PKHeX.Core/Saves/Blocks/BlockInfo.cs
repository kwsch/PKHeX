using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public abstract class BlockInfo
    {
        // General
        public uint ID { get; init; }
        public int Offset { get; protected init; }
        public int Length { get; init; }

        public string Summary => $"{ID:00}: {Offset:X5}-{Offset + Length - 1:X5}, {Length:X5}";

        protected abstract bool ChecksumValid(byte[] data);
        protected abstract void SetChecksum(byte[] data);

        /// <summary>
        /// Checks if the currently written checksum values are valid.
        /// </summary>
        /// <param name="blocks">Block info objects used for offset/length</param>
        /// <param name="data">Complete data array</param>
        /// <returns>True if checksums are valid, false if anything is invalid.</returns>
        public static bool GetChecksumsValid(IEnumerable<BlockInfo> blocks, byte[] data)
        {
            foreach (var b in blocks)
            {
                if (b.Length + b.Offset > data.Length)
                    return false;

                if (!b.ChecksumValid(data))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Applies checksums to the <see cref="data"/> object based on the input blocks.
        /// </summary>
        /// <param name="blocks">Block info objects used for offset/length</param>
        /// <param name="data">Complete data array</param>
        public static void SetChecksums(IEnumerable<BlockInfo> blocks, byte[] data)
        {
            foreach (var b in blocks)
                b.SetChecksum(data);
        }

        /// <summary>
        /// Gets information pertaining to the checksum data for diagnostic purposes.
        /// </summary>
        /// <param name="blocks">Block info objects used for offset/length</param>
        /// <param name="data">Complete data array</param>
        /// <returns>Multi-line string with <see cref="Summary"/> data.</returns>
        public static string GetChecksumInfo(IReadOnlyList<BlockInfo> blocks, byte[] data)
        {
            var invalid = GetInvalidBlockCount(blocks, data, out var list);
            list.Add($"SAV: {blocks.Count - invalid}/{blocks.Count}");
            return string.Join(Environment.NewLine, list);
        }

        private static int GetInvalidBlockCount(IReadOnlyList<BlockInfo> blocks, byte[] data, out List<string> list)
        {
            int invalid = 0;
            list = new List<string>();
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i].Length + blocks[i].Offset > data.Length)
                {
                    list.Add($"Block {i} Invalid Offset/Length.");
                    return invalid;
                }

                if (blocks[i].ChecksumValid(data))
                    continue;

                invalid++;
                list.Add($"Invalid: {i:X2} @ Region {blocks[i].Offset:X5}");
            }
            return invalid;
        }
    }

    public static partial class Extensions
    {
        public static bool GetChecksumsValid(this IEnumerable<BlockInfo> blocks, byte[] data) => BlockInfo.GetChecksumsValid(blocks, data);
        public static void SetChecksums(this IEnumerable<BlockInfo> blocks, byte[] data) => BlockInfo.SetChecksums(blocks, data);
        public static string GetChecksumInfo(this IReadOnlyList<BlockInfo> blocks, byte[] data) => BlockInfo.GetChecksumInfo(blocks, data);
    }
}
