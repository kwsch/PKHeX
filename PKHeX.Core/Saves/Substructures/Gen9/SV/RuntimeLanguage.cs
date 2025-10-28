namespace PKHeX.Core;

public enum RuntimeLanguage
{
    Japanese = 0,
    English = 1,
    Spanish = 2,
    German = 3,
    French = 4,
    Italian = 5,
    Korean = 6,
    ChineseS = 7,
    ChineseT = 8,
    LATAM = 9,
}

public static class RuntimeLanguageExtensions
{
    public static RuntimeLanguage GetRuntimeLanguage(LanguageID value) => value switch
    {
        LanguageID.Japanese => RuntimeLanguage.Japanese,
        LanguageID.English => RuntimeLanguage.English,
        LanguageID.Spanish => RuntimeLanguage.Spanish,
        LanguageID.German => RuntimeLanguage.German,
        LanguageID.French => RuntimeLanguage.French,
        LanguageID.Italian => RuntimeLanguage.Italian,
        LanguageID.Korean => RuntimeLanguage.Korean,
        LanguageID.ChineseS => RuntimeLanguage.ChineseS,
        LanguageID.ChineseT => RuntimeLanguage.ChineseT,
        LanguageID.SpanishL => RuntimeLanguage.LATAM,
        _ => RuntimeLanguage.English, // Default to English
    };
}
