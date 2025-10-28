using System;
using System.IO;
using static PKHeX.Core.MessageStrings;
using static PKHeX.Core.EventWorkDiffCompatibility;

namespace PKHeX.Core;

/// <summary>
/// Indicates if the compared data is incompatible in any way.
/// </summary>
public enum EventWorkDiffCompatibility
{
    Valid,
    DifferentVersion,
    DifferentGameGroup,
    FileTooBig1,
    FileTooBig2,
    FileMissing1,
    FileMissing2,
}

/// <summary>
/// Extension methods and utility logic for <see cref="EventWorkDiffCompatibility"></see>.
/// </summary>
public static class EventWorkDiffCompatibilityExtensions
{
    public static string GetMessage(this EventWorkDiffCompatibility value) => value switch
    {
        DifferentVersion => MsgSaveDifferentVersions,
        DifferentGameGroup => MsgSaveDifferentTypes,
        FileTooBig1 => string.Format(MsgSaveNumberInvalid, 1),
        FileTooBig2 => string.Format(MsgSaveNumberInvalid, 2),
        FileMissing1 => string.Format(MsgSaveNumberInvalid, 1),
        FileMissing2 => string.Format(MsgSaveNumberInvalid, 2),
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
    };

    public static EventWorkDiffCompatibility SanityCheckFiles(string f1, string f2, int MAX_SAVEFILE_SIZE)
    {
        if (!File.Exists(f1))
            return FileMissing1;

        if (!File.Exists(f2))
            return FileMissing2;

        if (new FileInfo(f1).Length > MAX_SAVEFILE_SIZE)
            return FileTooBig1;

        if (new FileInfo(f2).Length > MAX_SAVEFILE_SIZE)
            return FileTooBig2;

        return Valid;
    }
}
