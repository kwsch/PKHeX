using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;

namespace PKHeX.Core;

/// <summary>
/// Handles zipped save files, whether they contain multiple files or just one.
/// </summary>
public sealed class ZipReader : ISaveReader
{
    /// <summary>
    /// Indicates if the provided data length is large enough to attempt ZIP recognition.
    /// </summary>
    /// <param name="dataLength">Length of the data buffer.</param>
    /// <returns><see langword="true"/> if the data length is large enough; otherwise, <see langword="false"/>.</returns>
    public bool IsRecognized(long dataLength) => dataLength > 4;

    private static bool IsValidFileName(ReadOnlySpan<char> name) => Is(name, "main") || Is(name, "SaveData.bin");
    private static bool Is(ReadOnlySpan<char> value, ReadOnlySpan<char> other) => value.Equals(other, StringComparison.OrdinalIgnoreCase);

    // check ZIP header in first 4 bytes
    private static bool IsPossiblyZip(ReadOnlySpan<byte> data) => data.Length >= 16 && data is [0x50, 0x4B, 0x03, 0x04, ..]; // "PK\x03\x04"

    /// <summary>
    /// Attempts to read a <see cref="SaveFile"/> from the provided ZIP data.
    /// </summary>
    /// <param name="data">Raw file data that may represent a ZIP archive.</param>
    /// <param name="result">When this method returns <see langword="true"/>, contains the parsed <see cref="SaveFile"/> instance; otherwise <see langword="null"/>.</param>
    /// <param name="path">Optional original file path (ignored for ZIP contents).</param>
    /// <returns><see langword="true"/> if a save file was successfully parsed; otherwise, <see langword="false"/>.</returns>
    // ReSharper disable once MethodOverloadWithOptionalParameter
    public bool TryRead(Memory<byte> data, [NotNullWhen(true)] out SaveFile? result, string? path = null) => TryRead(data, out result);

    /// <summary>
    /// Attempts to read a <see cref="SaveFile"/> from the provided ZIP data.
    /// </summary>
    /// <param name="data">Raw file data that may represent a ZIP archive.</param>
    /// <param name="result">When this method returns <see langword="true"/>, contains the parsed <see cref="SaveFile"/> instance; otherwise <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if a save file was successfully parsed; otherwise, <see langword="false"/>.</returns>
    public static bool TryRead(Memory<byte> data, [NotNullWhen(true)] out SaveFile? result)
    {
        result = null;
        if (!IsPossiblyZip(data.Span))
            return false;

        using var ms = new MemoryStream(data.ToArray());
        return TryRead(ms, out result);
    }

    /// <inheritdoc cref="TryRead(Memory{byte}, out SaveFile?)"/>
    public static bool TryRead(byte[] data, [NotNullWhen(true)] out SaveFile? result)
    {
        result = null;
        if (!IsPossiblyZip(data))
            return false;

        using var ms = new MemoryStream(data);
        return TryRead(ms, out result);
    }

    /// <summary>
    /// Attempts to read a <see cref="SaveFile"/> from the provided stream assumed to contain a ZIP archive.
    /// </summary>
    /// <param name="ms">Stream positioned at the beginning of a ZIP archive.</param>
    /// <param name="result">When this method returns <see langword="true"/>, contains the parsed <see cref="SaveFile"/> instance; otherwise <see langword="null"/>.</param>
    /// <param name="leaveOpen">Whether to leave the stream open after reading.</param>
    /// <returns><see langword="true"/> if a save file was successfully parsed; otherwise, <see langword="false"/>.</returns>
    public static bool TryRead(Stream ms, [NotNullWhen(true)] out SaveFile? result, bool leaveOpen = false)
    {
        try
        {
            using var archive = new ZipArchive(ms, ZipArchiveMode.Read, leaveOpen);
            return TryRead(archive, out result);
        }
        catch
        {
            result = null;
            return false;
        }
    }

    private static bool TryRead(ZipArchive zip, [NotNullWhen(true)] out SaveFile? result)
    {
        var entries = zip.Entries;
        if (entries.Count == 1)
            return TryRead(entries[0], out result);

        foreach (var entry in entries)
        {
            if (!IsValidFileName(entry.Name))
                continue;
            if (TryRead(entry, out result))
                return true;
        }
        result = null;
        return false;
    }

    private static bool TryRead(ZipArchiveEntry entry, [NotNullWhen(true)] out SaveFile? result)
    {
        if (!SaveUtil.IsSizeValid(entry.Length))
        {
            result = null;
            return false;
        }

        using var entryStream = entry.Open();
        var tmp = new MemoryStream();
        entryStream.CopyTo(tmp);
        return SaveUtil.TryGetSaveFile(tmp.ToArray(), out result, entry.FullName);
    }

    /// <summary>
    /// Updates the first valid save file entry in a ZIP file on disk with the provided save data.
    /// </summary>
    /// <param name="path">Path to an existing ZIP file.</param>
    /// <param name="data">New save data to write.</param>
    /// <returns><see langword="true"/> if an entry was updated; otherwise, <see langword="false"/>.</returns>
    public static bool Update(string path, ReadOnlySpan<byte> data)
    {
        var ext = Path.GetExtension(path);
        if (ext is not ".zip")
            return false;

        if (!File.Exists(path))
            return false;

        using var zip = ZipFile.Open(path, ZipArchiveMode.Update);
        return Update(zip, data);
    }

    /// <summary>
    /// Updates the first save entry in the provided <see cref="ZipArchive"/> with the given data.
    /// </summary>
    /// <param name="zip">Open ZIP archive in update mode.</param>
    /// <param name="data">New save data to write.</param>
    /// <returns><see langword="true"/> if an entry was updated; otherwise, <see langword="false"/>.</returns>
    public static bool Update(ZipArchive zip, ReadOnlySpan<byte> data)
    {
        var entries = zip.Entries;
        if (entries.Count == 1)
        {
            UpdateEntry(entries[0], data);
            return true;
        }
        foreach (var entry in entries)
        {
            if (!IsValidFileName(entry.Name))
                continue;
            UpdateEntry(entry, data);
            return true;
        }
        return false;
    }

    private static void UpdateEntry(ZipArchiveEntry entry, ReadOnlySpan<byte> data)
    {
        using var entryStream = entry.Open();
        entryStream.SetLength(0);
        entryStream.Write(data);
    }
}
