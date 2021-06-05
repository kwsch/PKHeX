using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PKHeX.Core
{
    /// <summary>
    /// Utility logic for dumping <see cref="SCBlock"/> lists for external analysis
    /// </summary>
    public static class SCBlockUtil
    {
        public static void ExportAllBlocksAsSingleFile(IReadOnlyList<SCBlock> blocks, string path, SCBlockExportOption option = SCBlockExportOption.All)
        {
            var data = ExportAllBlocks(blocks, option);
            File.WriteAllBytes(path, data);
        }

        public static byte[] ExportAllBlocks(IReadOnlyList<SCBlock> blocks, SCBlockExportOption option = SCBlockExportOption.None)
        {
            if (option == SCBlockExportOption.None)
                return SwishCrypto.GetDecryptedRawData(blocks);

            using var stream = new MemoryStream();
            using var bw = new BinaryWriter(stream);
            for (var i = 0; i < blocks.Count; i++)
                ExportBlock(blocks[i], bw, i, option);
            return stream.ToArray();
        }

        private static void ExportBlock(SCBlock block, BinaryWriter bw, int blockIndex, SCBlockExportOption option)
        {
            if (option.HasFlagFast(SCBlockExportOption.DataOnly) && block.Data.Length == 0)
                return;

            if (option.HasFlagFast(SCBlockExportOption.FakeHeader))
                bw.Write($"BLOCK{blockIndex:0000} {block.Key:X8}");

            if (option.HasFlagFast(SCBlockExportOption.Key))
                bw.Write(block.Key);

            if (option.HasFlagFast(SCBlockExportOption.TypeInfo))
            {
                bw.Write((byte) block.Type);
                bw.Write((byte) block.SubType);
            }

            bw.Write(block.Data);
        }

        public static string GetBlockFileNameWithoutExtension(SCBlock block)
        {
            var key = block.Key;
            var name = $"{key:X8}";
            if (block.HasValue())
                name += $" {block.GetValue()}";
            return name;
        }

        public static string GetBlockSummary(SCBlock b)
        {
            var sb = new StringBuilder(64);
            sb.Append("Key: ").AppendFormat("{0:X8}", b.Key).AppendLine();
            sb.Append("Type: ").Append(b.Type).AppendLine();
            if (b.Data.Length != 0)
                sb.Append("Length: ").AppendFormat("{0:X8}", b.Data.Length).AppendLine();

            if (b.SubType != 0)
                sb.Append("SubType: ").Append(b.SubType).AppendLine();
            else if (b.HasValue())
                sb.Append("Value: ").Append(b.GetValue()).AppendLine();

            return sb.ToString();
        }

        public static List<string> ImportBlocksFromFolder(string path, SAV8SWSH sav)
        {
            var failed = new List<string>();
            var files = Directory.EnumerateFiles(path);
            foreach (var f in files)
            {
                var fn = Path.GetFileNameWithoutExtension(f);

                // Trim off Value summary if present
                var space = fn.IndexOf(' ');
                if (space != -1)
                    fn = fn[..space];

                var hex = Util.GetHexValue(fn);
                try
                {
                    var block = sav.Blocks.GetBlock(hex);
                    var len = block.Data.Length;
                    var fi = new FileInfo(f);
                    if (fi.Length != len)
                    {
                        failed.Add(fn);
                        continue;
                    }

                    var data = File.ReadAllBytes(f);
                    block.ChangeData(data);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    failed.Add(fn);
                }
            }

            return failed;
        }
    }

    [Flags]
    public enum SCBlockExportOption
    {
        None = 0,

        /// <summary>
        /// Will only export blocks with backing data.
        /// </summary>
        /// <remarks>Excludes Bool flags from the dump.</remarks>
        DataOnly = 1,

        /// <summary>
        /// Includes the Block Key ahead of the data.
        /// </summary>
        Key = 2,

        /// <summary>
        /// Includes the Block Info ahead of the data.
        /// </summary>
        TypeInfo = 4,

        /// <summary>
        /// Includes a fake header indicating which block it is in ASCII.
        /// </summary>
        FakeHeader = 8,

        /// <summary>
        /// Standard export options.
        /// </summary>
        All = DataOnly | Key | TypeInfo | FakeHeader,
    }

    internal static class ScBlockExportOptionExtensions
    {
        public static bool HasFlagFast(this SCBlockExportOption value, SCBlockExportOption flag)
        {
            return (value & flag) != 0;
        }
    }
}
