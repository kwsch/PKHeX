using System;
using System.Text;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

public readonly record struct MoveLearnInfo(LearnMethod Method, LearnEnvironment Environment, byte Argument = 0)
{
    public string Summarize()
    {
        var localized = GetLocalizedMethod();
        return Summarize(localized);
    }

    private string Summarize(string localizedMethod)
    {
        var sb = new StringBuilder(48);
        if (Environment.IsSpecified())
            sb.Append(Environment).Append('-');
        sb.Append(localizedMethod);
        if (Method is LevelUp)
            sb.Append(" @ lv").Append(Argument);
        return sb.ToString();
    }

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

        // Fishy
        EmptyFishy => LMoveSourceEmpty,
        _ => throw new ArgumentOutOfRangeException(nameof(Method), Method, null),
    };
}
