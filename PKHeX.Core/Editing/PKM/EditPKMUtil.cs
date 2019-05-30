using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.Core
{
    public static class EditPKMUtil
    {
        public static List<string> GetSuggestionMessage(PKM pkm, int level, int location, int minlvl)
        {
            var suggestion = new List<string> { MsgPKMSuggestionStart };
            if (pkm.Format >= 3)
            {
                var met_list = GameInfo.GetLocationList((GameVersion)pkm.Version, pkm.Format, egg: false);
                var locstr = met_list.First(loc => loc.Value == location).Text;
                suggestion.Add($"{MsgPKMSuggestionMetLocation} {locstr}");
                suggestion.Add($"{MsgPKMSuggestionMetLevel} {level}");
            }
            if (pkm.CurrentLevel < minlvl)
                suggestion.Add($"{MsgPKMSuggestionLevel} {minlvl}");
            return suggestion;
        }

        public static IReadOnlyList<ComboItem> GetAbilityList(PKM pkm)
        {
            var abils = pkm.PersonalInfo.Abilities;
            return GameInfo.GetAbilityList(abils, pkm.Format);
        }
    }
}
