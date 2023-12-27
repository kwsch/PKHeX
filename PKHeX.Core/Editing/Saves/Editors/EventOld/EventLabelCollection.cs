using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class EventLabelCollection
{
    public readonly IReadOnlyList<NamedEventWork> Work;
    public readonly IReadOnlyList<NamedEventValue> Flag;

    public EventLabelCollection(string game, int maxFlag = int.MaxValue, int maxValue = int.MaxValue)
    {
        var f = GameLanguage.GetStrings(game, GameInfo.CurrentLanguage, "flags");
        var c = GameLanguage.GetStrings(game, GameInfo.CurrentLanguage, "const");
        Flag = EventLabelParsing.GetFlags(f, maxFlag);
        Work = EventLabelParsing.GetWork(c, maxValue);
    }
}
