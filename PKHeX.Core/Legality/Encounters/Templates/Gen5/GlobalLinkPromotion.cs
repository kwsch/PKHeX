using System;
using static PKHeX.Core.GlobalLinkPromotion;
using static PKHeX.Core.GlobalLinkPromotionLanguage;

namespace PKHeX.Core;

public enum GlobalLinkPromotion : byte
{
    // B/W
    NotPromotion,
    Eeveelutions,
    StartersKanto,
    StartersHoenn,
    StartersSinnoh, // No bonus moves
    Croagunk1, // No bonus moves
    Croagunk2, // w/ Bonus Move Poison Jab
    Arceus,
    Mamoswine,
    Porygon,
    Rayquaza,
    Blissey,
    Jumpluff,
    Altaria,
    Banette,
    Lucario,
    Togekiss,

    // B2/W2
    Gothorita1, // Mirror Coat (strategy guide)
    Gothorita2, // Imprison (guidebook app)
    Pikachu,
    MonkeyUnova, // (B2/W2)
    StartersSinnoh2, // (B2/W2) w/ Bonus Moves

    // Both Games
    Dragonite1, // ThunderPunch (not-international; password)
    Dragonite2, // ExtremeSpeed (league events during Nimbasa Gym season)
    Scizor, // Aspertia Gym
    Garchomp, // Virbank Gym
    Tyranitar, // Castelia Gym
    Metagross, // Driftveil Gym
}

[Flags]
public enum GlobalLinkPromotionLanguage : byte
{
    None,
    Japanese      = 1 << 0,
    Korean        = 1 << 1,
    International = 1 << 2,

    NotInt = Japanese | Korean,
    NotKor = Japanese | International,
    All = Japanese | Korean | International,
}

public static class GlobalLinkPromotionExtensions
{
    extension(GlobalLinkPromotion promotion)
    {
        public GlobalLinkPromotionLanguage GetLanguages() => promotion switch
        {
            StartersKanto => NotInt,
            Croagunk1 => NotInt,
            Croagunk2 => International,
            Arceus => NotKor,
            StartersHoenn => Japanese,
            Rayquaza => Korean,
            Blissey => NotInt,
            Pikachu => NotKor,
            Jumpluff => Korean,
            MonkeyUnova => Japanese,
            StartersSinnoh2 => International,
            Gothorita2 => Japanese,

            Dragonite1 => NotInt,
            Dragonite2 => International,
            Scizor => International,
            Garchomp => International,
            Tyranitar => International,
            Metagross => International,
            _ => All,
        };

        public LanguageID GetSafeLanguage(LanguageID prefer)
        {
            if (promotion is NotPromotion)
                return Language.GetSafeLanguage456(prefer);

            // Promotions can be language-locked.
            var restrict = promotion.GetLanguages();
            if (restrict == All)
                return Language.GetSafeLanguage456(prefer);

            // Need to restrict to the allowed languages.
            if (restrict == Japanese)
                return LanguageID.Japanese;
            if (restrict == Korean)
                return LanguageID.Korean;
            if (restrict == NotKor && prefer == LanguageID.Korean)
                return LanguageID.Japanese;
            if (restrict == NotInt)
                return (prefer == LanguageID.Korean ? prefer : LanguageID.Japanese);

            // International language only.
            if (prefer is LanguageID.Japanese or LanguageID.Korean)
                return LanguageID.English;
            return Language.GetSafeLanguage456(prefer);
        }

        public bool CanBeReceivedBy(LanguageID language)
        {
            if (promotion is NotPromotion)
                return true;

            var restrict = promotion.GetLanguages();
            if (restrict is All)
                return true;

            if (language is LanguageID.Japanese)
                return restrict.HasFlag(Japanese);
            if (language is LanguageID.Korean)
                return restrict.HasFlag(Korean);

            // Only International languages remain.
            return restrict.HasFlag(International);
        }
    }
}
