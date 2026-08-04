using PKHeX.Core;

namespace PKHeX.Application.Abstractions.GiftRecords;

/// <summary>
/// Resolves a <see cref="IGiftRecordStore"/> for saves that keep a received Mystery Gift
/// record log instead of a re-insertable wondercard album (Switch titles: SWSH/BDSP/PLA/SV).
/// </summary>
public interface IGiftRecordProvider
{
    /// <summary>Returns a record store for the save, or <see langword="null"/> when the save has no gift record data.</summary>
    IGiftRecordStore? GetStore(SaveFile sav);
}
