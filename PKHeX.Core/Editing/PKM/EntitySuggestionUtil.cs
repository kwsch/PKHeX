using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.Core;

/// <summary>
/// Utility for editing a <see cref="PKM"/>
/// </summary>
public static class EntitySuggestionUtil
{
    public static List<string> GetMetLocationSuggestionMessage(PKM pk, int level, ushort location, int minimumLevel, IEncounterable? enc)
    {
        var suggestion = new List<string> { MsgPKMSuggestionStart };
        if (pk.Format >= 3)
        {
            var metList = GameInfo.GetLocationList(pk.Version, pk.Context, egg: false);
            var locationName = metList.First(loc => loc.Value == location).Text;
            suggestion.Add($"{MsgPKMSuggestionMetLocation} {locationName}");
            suggestion.Add($"{MsgPKMSuggestionMetLevel} {level}");
        }
        else if (pk is ICaughtData2)
        {
            var metList = GameInfo.GetLocationList(GameVersion.C, pk.Context);
            string locationName;
            if (enc?.Version.Contains(GameVersion.C) == true)
            {
                locationName = metList.First(loc => loc.Value == location).Text;
            }
            else
            {
                locationName = metList[0].Text;
                level = 0;
            }
            suggestion.Add($"{MsgPKMSuggestionMetLocation} {locationName}");
            suggestion.Add($"{MsgPKMSuggestionMetLevel} {level}");
        }
        if (pk.CurrentLevel < minimumLevel)
            suggestion.Add($"{MsgPKMSuggestionLevel} {minimumLevel}");
        return suggestion;
    }
}
