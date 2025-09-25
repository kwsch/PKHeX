using System;
using System.IO;
using System.IO.Compression;

namespace PKHeX.Core;

/// <summary>
/// Handles zipped save files, whether they contain multiple files or just one.
/// </summary>
public sealed class ZipReader : ISaveReader
{
    public bool IsRecognized(long dataLength) => dataLength > 4;

    private static bool IsValidFileName(string name) => name is "main" or "SaveData.bin";

    public SaveFile? ReadSaveFile(Memory<byte> data, string? path = null)
    {
        // check ZIP header in first 4 bytes
        if (data.Length < 4 || data.Span is not [0x50, 0x4B, 0x03, 0x04, ..]) // "PK\x03\x04"
            return null;

        using var ms = new MemoryStream(data.ToArray());
        using var archive = new ZipArchive(ms, ZipArchiveMode.Read, false);
        return Read(archive);
    }

    private static SaveFile? Read(ZipArchive zip)
    {
        var entries = zip.Entries;
        foreach (var entry in entries)
        {
            if (!IsValidFileName(entry.Name) && entries.Count != 1)
                continue;
            if (!SaveUtil.IsSizeValid(entry.Length))
                continue;

            using var entryStream = entry.Open();
            var tmp = new MemoryStream();
            entryStream.CopyTo(tmp);
            if (!SaveUtil.TryGetSaveFile(tmp.ToArray(), out var result, entry.FullName))
                continue;
            return result;
        }
        return null;
    }

    public static void UpdateSaveFile(ReadOnlySpan<byte> data, string path)
    {
        var ext = Path.GetExtension(path);
        if (ext is not ".zip")
            return;

        if (!File.Exists(path))
            return;

        using var zip = ZipFile.Open(path, ZipArchiveMode.Update);
        Update(zip, data);
    }

    private static void Update(ZipArchive zip, ReadOnlySpan<byte> data)
    {
        var entries = zip.Entries;
        foreach (var entry in entries)
        {
            if (!IsValidFileName(entry.Name) && entries.Count != 1)
                continue;
            using var entryStream = entry.Open();
            entryStream.Write(data);
            break;
        }
    }
}
