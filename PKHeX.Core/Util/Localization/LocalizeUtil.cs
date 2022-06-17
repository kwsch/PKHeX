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
        if (sav != null)
            GameInfo.FilteredSources = new FilteredGameDataSource(sav, GameInfo.Sources, hax);

        // Update Legality Analysis strings
        ParseSettings.ChangeLocalizationStrings(str.movelist, str.specieslist);

        // Update Legality Strings
        Task.Run(() =>
        {
            RibbonStrings.ResetDictionary(str.ribbons);
            LocalizationUtil.SetLocalization(typeof(LegalityCheckStrings), lang);
            LocalizationUtil.SetLocalization(typeof(MessageStrings), lang);
        });
    }
}
