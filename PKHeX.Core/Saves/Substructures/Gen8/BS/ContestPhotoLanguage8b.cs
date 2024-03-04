using System;

namespace PKHeX.Core;

/// <summary>
/// Contest photo language data
/// </summary>
/// <remarks>CON_PHOTO_LANG_DATA size: 0x18</remarks>
public sealed class ContestPhotoLanguage8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw)
{
    // structure:
    // 5 (Style, Beautiful, Cute, Clever, Strong) bytes of language IDs
    // (3+4 bytes alignment)
    // 2 s64 reserved
    private const int COUNT_CONTEST = 5;

    public byte GetLanguage(int contest) => Data[GetContestOffset(contest)];

    private static int GetContestOffset(int contest)
    {
        if ((uint)contest >= COUNT_CONTEST)
            throw new ArgumentOutOfRangeException(nameof(contest));
        return contest;
    }

    public void SetLanguage(int contest, int language) => Data[GetContestOffset(contest)] = (byte)language;
}
