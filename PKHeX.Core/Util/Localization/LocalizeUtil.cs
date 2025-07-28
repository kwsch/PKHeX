using System.Threading.Tasks;

namespace PKHeX.Core;

public static class LocalizeUtil
{
    /// <summary>
    /// Initializes PKHeX's runtime strings to the specified language.
    /// </summary>
    /// <param name="lang">2-char language ID</param>
    /// <param name="sav">Save data (optional)</param>
    /// <param name="hax">Permit illegal things (items, only)</param>
    public static void InitializeStrings(string lang, SaveFile? sav = null, bool hax = false)
    {
        var str = GameInfo.Strings = GameInfo.GetStrings(lang);
        if (sav is not null)
            GameInfo.FilteredSources = new FilteredGameDataSource(sav, GameInfo.Sources, hax);

        Task.Run(() => LocalizationUtil.SetLocalization(typeof(MessageStrings), lang));
    }
}
