using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Logic for detecting supported binary object formats.
/// </summary>
public static class FileUtil
{
    /// <summary>
    /// Attempts to get a binary object from the provided path.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="reference">Reference SaveFile used for PC Binary compatibility checks.</param>
    /// <returns>Supported file object reference, null if none found.</returns>
    public static object? GetSupportedFile(string path, SaveFile? reference = null)
    {
        try
        {
            var fi = new FileInfo(path);
            if (!fi.Exists || IsFileTooBig(fi.Length) || IsFileTooSmall(fi.Length))
                return null;

            var data = File.ReadAllBytes(path);
            var ext = Path.GetExtension(path.AsSpan());
            return GetSupportedFile(data, ext, reference);
        }
        // User input data can be fuzzed; if anything blows up, just fail safely.
        catch (Exception e)
        {
            Debug.WriteLine(MessageStrings.MsgFileInUse);
            Debug.WriteLine(e.Message);
            return null;
        }
    }

    /// <summary>
    /// Attempts to get a binary object from the provided inputs.
    /// </summary>
    /// <param name="data">Binary data for the file.</param>
    /// <param name="ext">File extension used as a hint.</param>
    /// <param name="reference">Reference SaveFile used for PC Binary compatibility checks.</param>
    /// <returns>Supported file object reference, null if none found.</returns>
    public static object? GetSupportedFile(byte[] data, ReadOnlySpan<char> ext, SaveFile? reference = null)
    {
        if (TryGetSAV(data, out var sav))
            return sav;
        if (TryGetMemoryCard(data, out var mc))
            return mc;
        if (TryGetPKM(data, out var pk, ext))
            return pk;
        if (TryGetPCBoxBin(data, out var concat, reference))
            return concat;
        if (TryGetBattleVideo(data, out var bv))
            return bv;
        if (TryGetMysteryGift(data, out var g, ext))
            return g;
        if (TryGetGP1(data, out var gp))
            return gp;
        if (TryGetBundle(data, out var bundle))
            return bundle;
        return null;
    }

    public static bool IsFileLocked(string path)
    {
        try { return (File.GetAttributes(path) & FileAttributes.ReadOnly) != 0; }
        catch { return true; }
    }

    public static long GetFileSize(string path)
    {
        try
        {
            var fi = new FileInfo(path);
            var size = fi.Length;
            if (size > int.MaxValue)
                return -1;
            return size;
        }
        catch { return -1; } // Bad File / Locked
    }

    public static IEnumerable<T> IterateSafe<T>(this IEnumerable<T> source, int failOut = 10, Action<Exception>? log = null)
    {
        using var enumerator = source.GetEnumerator();
        int ctr = 0;
        while (true)
        {
            try
            {
                var next = enumerator.MoveNext();
                if (!next)
                    yield break;
            }
            catch (Exception ex)
            {
                log?.Invoke(ex);
                if (++ctr >= failOut)
                    yield break;
                continue;
            }
            ctr = 0;
            yield return enumerator.Current;
        }
    }

    private static bool TryGetGP1(byte[] data, [NotNullWhen(true)] out GP1? gp1)
    {
        gp1 = null;
        if (data.Length != GP1.SIZE || ReadUInt32LittleEndian(data.AsSpan(0x28)) == 0)
            return false;
        gp1 = new GP1(data);
        return true;
    }

    private static bool TryGetBundle(byte[] data, [NotNullWhen(true)] out IPokeGroup? result)
    {
        result = null;
        if (RentalTeam8.IsRentalTeam(data))
        {
            result = new RentalTeam8(data);
            return true;
        }
        if (RentalTeam9.IsRentalTeam(data))
        {
            result = new RentalTeam9(data);
            return true;
        }
        if (RentalTeamSet9.IsRentalTeamSet(data))
        {
            result = new RentalTeamSet9(data);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the length is too big to be a detectable file.
    /// </summary>
    /// <param name="length">File size</param>
    public static bool IsFileTooBig(long length)
    {
        if (length <= 0x10_0000) // 1 MB
            return false;
        if (length > int.MaxValue)
            return true;
        if (SaveUtil.IsSizeValid((int)length))
            return false;
        if (SAV3GCMemoryCard.IsMemoryCardSize(length))
            return false; // pbr/GC have size > 1MB
        return true;
    }

    /// <summary>
    /// Checks if the length is too small to be a detectable file.
    /// </summary>
    /// <param name="length">File size</param>
    public static bool IsFileTooSmall(long length) => length < 0x20; // bigger than PK1

    /// <summary>
    /// Tries to get a <see cref="SaveFile"/> object from the input parameters.
    /// </summary>
    /// <param name="data">Binary data</param>
    /// <param name="sav">Output result</param>
    /// <returns>True if file object reference is valid, false if none found.</returns>
    public static bool TryGetSAV(byte[] data, [NotNullWhen(true)] out SaveFile? sav)
    {
        sav = SaveUtil.GetVariantSAV(data);
        return sav != null;
    }

    /// <summary>
    /// Tries to get a <see cref="SAV3GCMemoryCard"/> object from the input parameters.
    /// </summary>
    /// <param name="data">Binary data</param>
    /// <param name="memcard">Output result</param>
    /// <returns>True if file object reference is valid, false if none found.</returns>
    public static bool TryGetMemoryCard(byte[] data, [NotNullWhen(true)] out SAV3GCMemoryCard? memcard)
    {
        if (!SAV3GCMemoryCard.IsMemoryCardSize(data) || IsNoDataPresent(data))
        {
            memcard = null;
            return false;
        }
        memcard = new SAV3GCMemoryCard(data);
        return true;
    }

    /// <inheritdoc cref="TryGetMemoryCard(byte[], out SAV3GCMemoryCard?)"/>
    public static bool TryGetMemoryCard(string file, [NotNullWhen(true)] out SAV3GCMemoryCard? memcard)
    {
        if (!File.Exists(file))
        {
            memcard = null;
            return false;
        }
        var data = File.ReadAllBytes(file);
        return TryGetMemoryCard(data, out memcard);
    }

    /// <summary>
    /// Tries to get a <see cref="PKM"/> object from the input parameters.
    /// </summary>
    /// <param name="data">Binary data</param>
    /// <param name="pk">Output result</param>
    /// <param name="ext">Format hint</param>
    /// <param name="sav">Reference save file used for PC Binary compatibility checks.</param>
    /// <returns>True if file object reference is valid, false if none found.</returns>
    public static bool TryGetPKM(byte[] data, [NotNullWhen(true)] out PKM? pk, ReadOnlySpan<char> ext, ITrainerInfo? sav = null)
    {
        if (ext.EndsWith("pgt")) // size collision with pk6
        {
            pk = null;
            return false;
        }
        var format = EntityFileExtension.GetContextFromExtension(ext, sav?.Context ?? EntityContext.Gen6);
        pk = EntityFormat.GetFromBytes(data, prefer: format);
        return pk != null;
    }

    /// <summary>
    /// Tries to get a <see cref="IEnumerable{T}"/> object from the input parameters.
    /// </summary>
    /// <param name="data">Binary data</param>
    /// <param name="result">Output result</param>
    /// <param name="sav">Reference SaveFile used for PC Binary compatibility checks.</param>
    /// <returns>True if file object reference is valid, false if none found.</returns>
    public static bool TryGetPCBoxBin(byte[] data, [NotNullWhen(true)] out ConcatenatedEntitySet? result, SaveFile? sav)
    {
        result = null;
        if (sav is null || IsNoDataPresent(data))
            return false;

        // Only return if the size is one of the save file's data chunk formats.
        var expect = sav.SIZE_BOXSLOT;

        // Check if it's the entire PC data.
        var countPC = sav.SlotCount;
        if (expect * countPC == data.Length)
        {
            result = new(data, countPC);
            return true;
        }

        // Check if it's a single box data.
        var countBox = sav.BoxSlotCount;
        if (expect * countBox == data.Length)
        {
            result = new(data, countBox);
            return true;
        }

        return false;
    }

    private static bool IsNoDataPresent(ReadOnlySpan<byte> data)
    {
        if (!data.ContainsAnyExcept<byte>(0xFF))
            return true;
        if (!data.ContainsAnyExcept<byte>(0x00))
            return true;
        return false;
    }

    /// <summary>
    /// Tries to get a <see cref="BattleVideo"/> object from the input parameters.
    /// </summary>
    /// <param name="data">Binary data</param>
    /// <param name="bv">Output result</param>
    /// <returns>True if file object reference is valid, false if none found.</returns>
    public static bool TryGetBattleVideo(byte[] data, [NotNullWhen(true)] out IBattleVideo? bv)
    {
        bv = BattleVideo.GetVariantBattleVideo(data);
        return bv != null;
    }

    /// <summary>
    /// Tries to get a <see cref="MysteryGift"/> object from the input parameters.
    /// </summary>
    /// <param name="data">Binary data</param>
    /// <param name="mg">Output result</param>
    /// <param name="ext">Format hint</param>
    /// <returns>True if file object reference is valid, false if none found.</returns>
    public static bool TryGetMysteryGift(byte[] data, [NotNullWhen(true)] out MysteryGift? mg, ReadOnlySpan<char> ext)
    {
        mg = ext.Length == 0
            ? MysteryGift.GetMysteryGift(data)
            : MysteryGift.GetMysteryGift(data, ext);
        return mg != null;
    }

    /// <summary>
    /// Gets a Temp location File Name for the <see cref="PKM"/>.
    /// </summary>
    /// <param name="pk">Data to be exported</param>
    /// <param name="encrypt">Data is to be encrypted</param>
    /// <returns>Path to temporary file location to write to.</returns>
    public static string GetPKMTempFileName(PKM pk, bool encrypt)
    {
        string fn = pk.FileNameWithoutExtension;
        string filename = fn + (encrypt ? $".ek{pk.Format}" : $".{pk.Extension}");

        return Path.Combine(Path.GetTempPath(), Util.CleanFileName(filename));
    }

    /// <summary>
    /// Gets a <see cref="PKM"/> from the provided <see cref="file"/> path, which is to be loaded to the <see cref="SaveFile"/>.
    /// </summary>
    /// <param name="file"><see cref="PKM"/> or <see cref="MysteryGift"/> file path.</param>
    /// <param name="sav">Generation Info</param>
    /// <returns>New <see cref="PKM"/> reference from the file.</returns>
    public static PKM? GetSingleFromPath(string file, ITrainerInfo sav)
    {
        var fi = new FileInfo(file);
        if (!fi.Exists)
            return null;
        if (fi.Length == GP1.SIZE && TryGetGP1(File.ReadAllBytes(file), out var gp1))
            return gp1.ConvertToPKM(sav);
        if (!EntityDetection.IsSizePlausible(fi.Length) && !MysteryGift.IsMysteryGift(fi.Length))
            return null;
        var data = File.ReadAllBytes(file);
        var ext = fi.Extension;
        var mg = MysteryGift.GetMysteryGift(data, ext);
        var gift = mg?.ConvertToPKM(sav);
        if (gift != null)
            return gift;
        _ = TryGetPKM(data, out var pk, ext, sav);
        return pk;
    }
}

/// <summary>
/// Represents a set of concatenated <see cref="PKM"/> data.
/// </summary>
/// <param name="Data">Object data</param>
/// <param name="Count">Count of objects</param>
public sealed record ConcatenatedEntitySet(Memory<byte> Data, int Count)
{
    public int SlotSize => Data.Length / Count;

    public Span<byte> GetSlot(int index)
    {
        var size = SlotSize;
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)size);

        var offset = index * size;
        return Data.Span.Slice(offset, size);
    }
}
