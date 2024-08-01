using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PKHeX.Core;

/// <summary>
/// Utility logic for dumping <see cref="SCBlock"/> lists for external analysis
/// </summary>
public static class SCBlockUtil
{
    /// <summary>
    /// Create a blank block array using the provided <see cref="arr"/> definition.
    /// </summary>
    /// <param name="arr">Block specification tuples (key, size)</param>
    /// <returns>List of blocks</returns>
    internal static SCBlock[] GetBlankBlockArray(ReadOnlySpan<uint> arr)
    {
        var blocks = new SCBlock[arr.Length / 2];
        for (int i = 0; i < blocks.Length; i++)
        {
            int index = i * 2;
            var key = arr[index];
            var length = (int)arr[index + 1];
            var dummy = new byte[length];
            blocks[i] = new SCBlock(key, SCTypeCode.None, dummy);
        }
        return blocks;
    }

#if DEBUG
    /// <summary>
    /// Exports the block keys &amp; sizes in a human-readable format.
    /// </summary>
    public static IEnumerable<string> RipSizes(IReadOnlyCollection<SCBlock> blocks)
    {
        int ctr = 0;
        foreach (var block in blocks)
        {
            if (block.Data.Length == 0)
                continue;
            if (ctr == 4)
            {
                yield return Environment.NewLine;
                ctr = 0;
            }
            yield return $"0x{block.Key:X8}, 0x{block.Data.Length:X5}, ";
            ctr++;
        }
    }
#endif

    /// <summary>
    /// Concatenates all blocks into a single file with some metadata sprinkled in.
    /// </summary>
    /// <param name="blocks">Ascending-key ordered blocks</param>
    /// <param name="path">File path to write to</param>
    /// <param name="option">Export options</param>
    public static void ExportAllBlocksAsSingleFile(IReadOnlyList<SCBlock> blocks, string path, SCBlockExportOption option = SCBlockExportOption.All)
    {
        var data = ExportAllBlocks(blocks, option);
        File.WriteAllBytes(path, data);
    }

    /// <inheritdoc cref="ExportAllBlocksAsSingleFile(IReadOnlyList{SCBlock}, string, SCBlockExportOption)"/>
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

    /// <summary>
    /// Writes the block to the <see cref="bw"/> stream with the specified <see cref="option"/>.
    /// </summary>
    private static void ExportBlock(SCBlock block, BinaryWriter bw, int blockIndex, SCBlockExportOption option)
    {
        if (option.HasFlag(SCBlockExportOption.DataOnly) && block.Data.Length == 0)
            return;

        if (option.HasFlag(SCBlockExportOption.FakeHeader))
            bw.Write($"BLOCK{blockIndex:0000} {block.Key:X8}");

        if (option.HasFlag(SCBlockExportOption.Key))
            bw.Write(block.Key);

        if (option.HasFlag(SCBlockExportOption.TypeInfo))
        {
            bw.Write((byte) block.Type);
            bw.Write((byte) block.SubType);
        }

        bw.Write(block.Data);
    }

    /// <summary>
    /// Get the suggested file name for a <see cref="block"/>, without the file extension.
    /// </summary>
    /// <param name="block"></param>
    /// <returns></returns>
    public static string GetBlockFileNameWithoutExtension(SCBlock block)
    {
        var key = block.Key;
        var name = $"{key:X8}";
        if (block.HasValue())
            name += $" {block.GetValue()}";
        return name;
    }

    /// <summary>
    /// Gets a summary of the block similar to it being a record type.
    /// </summary>
    public static string GetBlockSummary(SCBlock b)
    {
        var sb = new StringBuilder(64);
        sb.AppendLine($"Key: {b.Key:X8}");
        sb.AppendLine($"Type: {b.Type}");
        if (b.Data.Length != 0)
            sb.AppendLine($"Length: {b.Data.Length:X8}");

        if (b.SubType != 0)
            sb.AppendLine($"SubType: {b.SubType}");
        else if (b.HasValue())
            sb.AppendLine($"Value: {b.GetValue()}");

        return sb.ToString();
    }

    /// <summary>
    /// Import blocks from a folder, using the block key as the file name.
    /// </summary>
    /// <param name="path">Folder to import from</param>
    /// <param name="sav">Save file to import into</param>
    /// <returns>List of block keys that failed to import</returns>
    public static List<string> ImportBlocksFromFolder(string path, ISCBlockArray sav)
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
                var block = sav.Accessor.GetBlock(hex);
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
            catch
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
