using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Utility for editing a <see cref="PKM"/>
    /// </summary>
    public static class EntitySuggestionUtil
    {
        public static List<string> GetMetLocationSuggestionMessage(PKM pkm, int level, int location, int minimumLevel)
        {
            var suggestion = new List<string> { MsgPKMSuggestionStart };
            if (pkm.Format >= 3)
            {
                var metList = GameInfo.GetLocationList((GameVersion)pkm.Version, pkm.Format, egg: false);
                var locationName = metList.First(loc => loc.Value == location).Text;
                suggestion.Add($"{MsgPKMSuggestionMetLocation} {locationName}");
                suggestion.Add($"{MsgPKMSuggestionMetLevel} {level}");
            }
            if (pkm.CurrentLevel < minimumLevel)
                suggestion.Add($"{MsgPKMSuggestionLevel} {minimumLevel}");
            return suggestion;
        }
    }
}
