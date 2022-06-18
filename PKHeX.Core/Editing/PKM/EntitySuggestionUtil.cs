using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.Core;

/// <summary>
/// Utility for editing a <see cref="PKM"/>
/// </summary>
public static class EntitySuggestionUtil
{
    public static List<string> GetMetLocationSuggestionMessage(PKM pk, int level, int location, int minimumLevel)
    {
        var suggestion = new List<string> { MsgPKMSuggestionStart };
        if (pk.Format >= 3)
        {
            var metList = GameInfo.GetLocationList((GameVersion)pk.Version, pk.Context, egg: false);
            var locationName = metList.First(loc => loc.Value == location).Text;
            suggestion.Add($"{MsgPKMSuggestionMetLocation} {locationName}");
            suggestion.Add($"{MsgPKMSuggestionMetLevel} {level}");
        }
        if (pk.CurrentLevel < minimumLevel)
            suggestion.Add($"{MsgPKMSuggestionLevel} {minimumLevel}");
        return suggestion;
    }
}
