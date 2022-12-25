using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class EventLabelCollectionSystem
{
    public readonly IReadOnlyList<NamedEventWork> Work;
    public readonly IReadOnlyList<NamedEventValue> Flag;
    public readonly IReadOnlyList<NamedEventValue> System;

    public EventLabelCollectionSystem(string game, int maxFlag = int.MaxValue, int maxSystem = int.MaxValue, int maxValue = int.MaxValue)
    {
        var f = GameLanguage.GetStrings(game, GameInfo.CurrentLanguage, "flag");
        var s = GameLanguage.GetStrings(game, GameInfo.CurrentLanguage, "system");
        var c = GameLanguage.GetStrings(game, GameInfo.CurrentLanguage, "work");
        Flag = EventLabelParsing.GetFlags(f, maxFlag);
        System = EventLabelParsing.GetFlags(s, maxSystem);
        Work = EventLabelParsing.GetWork(c, maxValue);
    }
}
