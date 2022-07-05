using System;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

public readonly record struct MoveLearnInfo(LearnMethod Method, GameVersion Environment, byte Argument = 0)
{
    public readonly LearnMethod Method = Method;
    public readonly byte Argument = Argument;
    public readonly GameVersion Environment = Environment;

    public string Summarize()
    {
        var localized = GetLocalizedMethod();
        return Summarize(localized);
    }

    private string Summarize(string localizedMethod) => Method switch
    {
        LevelUp => $"{Environment}-{localizedMethod} @ lv{Argument}",
        _       => $"{Environment}-{localizedMethod}",
    };

    private string GetLocalizedMethod() => Method switch
    {
        Empty => LMoveSourceEmpty,
        Relearn => LMoveSourceRelearn,
        Initial => LMoveSourceDefault,
        LevelUp => LMoveSourceLevelUp,
        TMHM => LMoveSourceTMHM,
        Tutor => LMoveSourceTutor,
        Sketch => LMoveSourceShared,
        EggMove => LMoveRelearnEgg,
        InheritLevelUp => LMoveEggInherited,

        Special => LMoveSourceSpecial,
        SpecialEgg => LMoveSourceSpecial,
        ShedinjaEvo => LMoveSourceSpecial,

        Shared => LMoveSourceShared,

        // Invalid
        None => LMoveSourceInvalid,
        Unobtainable => LMoveSourceInvalid,
        Duplicate => LMoveSourceDuplicate,
        EmptyInvalid => LMoveSourceEmpty,
        _ => throw new ArgumentOutOfRangeException(nameof(Method), Method, null),
    };
}
