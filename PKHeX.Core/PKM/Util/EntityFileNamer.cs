using System;
using System.Collections.Generic;
using System.IO;

namespace PKHeX.Core;

public static class EntityFileNamer
{
    /// <summary>
    /// Object that converts the <see cref="PKM"/> data into a <see cref="string"/> file name.
    /// </summary>
    public static IFileNamer<PKM> Namer { get; set; } = new DefaultEntityNamer();

    /// <summary>
    /// Gets the file name (without extension) for the input <see cref="pk"/> data.
    /// </summary>
    /// <param name="pk">Input entity to create a file name for.</param>
    /// <returns>File name for the <see cref="pk"/> data</returns>
    public static string GetName(PKM pk) => Namer.GetName(pk);

    /// <summary>
    /// A list of all available <see cref="IFileNamer{PKM}"/> objects.
    /// </summary>
    /// <remarks>Used for UI display.</remarks>
    public static readonly List<IFileNamer<PKM>> AvailableNamers = [Namer];
}

/// <summary>
/// PKHeX's default <see cref="PKM"/> file naming logic.
/// </summary>
public sealed class DefaultEntityNamer : IFileNamer<PKM>
{
    public string Name => "Default";

    public string GetName(PKM obj)
    {
        if (obj is GBPKM gb)
            return GetGBPKM(gb);
        return GetRegular(obj);
    }

    private static string GetRegular(PKM pk)
    {
        var chk = pk is ISanityChecksum s ? s.Checksum : Checksums.Add16(pk.Data.AsSpan()[8..pk.SIZE_STORED]);
        var form = pk.Form != 0 ? $"-{pk.Form:00}" : string.Empty;
        var star = pk.IsShiny ? " ★" : string.Empty;
        return $"{pk.Species:0000}{form}{star} - {pk.Nickname} - {chk:X4}{pk.EncryptionConstant:X8}";
    }

    private static string GetGBPKM(GBPKM gb)
    {
        var checksum = gb switch
        {
            PK1 pk1 => pk1.GetSingleListChecksum(),
            PK2 pk2 => pk2.GetSingleListChecksum(),
            _ => Checksums.CRC16_CCITT(gb.Data),
        };

        var form = gb.Form != 0 ? $"-{gb.Form:00}" : string.Empty;
        var star = gb.IsShiny ? " ★" : string.Empty;
        return $"{gb.Species:0000}{form}{star} - {gb.Nickname} - {checksum:X4}";
    }
}

/// <summary>
/// Exposes a method to get a file name (no extension) for the type.
/// </summary>
/// <typeparam name="T">Type that the implementer can create a file name for.</typeparam>
public interface IFileNamer<in T>
{
    /// <summary>
    /// Human-readable name of the <see cref="IFileNamer{T}"/> implementation.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the file name (without extension) for the input <see cref="obj"/> data.
    /// </summary>
    string GetName(T obj);
}

/// <summary>
/// Logic for renaming multiple files.
/// </summary>
public static class BulkFileRenamer
{
    /// <summary>
    /// Bulk renames files.
    /// </summary>
    /// <param name="namer">Rename implementation</param>
    /// <param name="dir">Folder to rename files</param>
    /// <returns>Count of files renamed</returns>
    public static int RenameFiles<T>(this IFileNamer<T> namer, string dir) where T : PKM
    {
        var files = Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories);
        return RenameFiles(namer, files);
    }

    /// <inheritdoc cref="RenameFiles{T}(IFileNamer{T},string)"/>
    public static int RenameFiles<T>(this IFileNamer<T> namer, IEnumerable<string> files) where T : PKM
    {
        var count = 0;
        foreach (var file in files)
        {
            if (namer.RenameFile(file))
                count++;
        }
        return count;
    }

    /// <summary>
    /// Renames a file using the input <see cref="namer"/>.
    /// </summary>
    /// <returns>True if renamed.</returns>
    public static bool RenameFile<T>(this IFileNamer<T> namer, string file) where T : PKM
    {
        var dirName = Path.GetDirectoryName(file);
        if (dirName is null)
            return false;

        var fi = new FileInfo(file);
        if (fi.Attributes.HasFlag(FileAttributes.ReadOnly))
            return false;

        if (!EntityDetection.IsSizePlausible(fi.Length))
            return false;

        var data = File.ReadAllBytes(file);
        var pk = EntityFormat.GetFromBytes(data);
        if (pk is not T x)
            return false;

        var name = namer.GetName(x);
        name += pk.Extension;
        var newPath = Path.Combine(dirName, name);
        if (file == newPath)
            return false;

        try
        {
            File.Move(file, newPath, true);
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            return false;
        }
    }
}
