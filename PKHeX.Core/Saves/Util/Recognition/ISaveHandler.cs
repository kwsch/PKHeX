using System;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;
#if !(EXCLUDE_EMULATOR_FORMATS && EXCLUDE_HACKS)
/// <summary>
/// Provides handling for recognizing atypical save file formats.
/// </summary>
public interface ISaveHandler
{
    /// <summary>
    /// Checks if the requested file size is one that can be recognized by this handler.
    /// </summary>
    /// <param name="size">File size</param>
    /// <returns>True if recognized, false if not recognized.</returns>
    bool IsRecognized(long size);

    /// <summary>
    /// Tries splitting up the <see cref="input"/> into header/footer/data components. Returns null if not a valid save file for this handler.
    /// </summary>
    /// <param name="input">Combined data</param>
    /// <returns>Null if not a valid save file for this handler's format. Returns an object containing header, footer, and inner data references.</returns>
    SaveHandlerSplitResult? TrySplit(Memory<byte> input);

    /// <summary>
    /// When exporting a save file, the handler might want to update the header/footer.
    /// </summary>
    /// <param name="input">Combined data</param>
    void Finalize(Span<byte> input);
}
#endif

#if !EXCLUDE_HACKS
/// <summary>
/// Provides handling for recognizing atypical save file formats.
/// </summary>
public interface ISaveReader
{
    /// <summary>
    /// Reads a save file from the <see cref="data"/>
    /// </summary>
    /// <param name="data">Raw input data</param>
    /// <param name="result">The resulting <see cref="SaveFile"/> if successful, otherwise null.</param>
    /// <param name="path">Optional file path.</param>
    /// <returns>Save File object, or null if invalid. Check <see cref="ISaveHandler"/> if it is compatible first.</returns>
    bool TryRead(Memory<byte> data, [NotNullWhen(true)] out SaveFile? result, string? path = null);

    /// <inheritdoc cref="ISaveHandler.IsRecognized"/>
    bool IsRecognized(long dataLength);
}
#endif
