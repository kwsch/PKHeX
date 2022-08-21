using System;
using System.Text;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Small struct that stores the where &amp; how details a move can be learned.
/// </summary>
/// <param name="Method">How the move was learned</param>
/// <param name="Environment">Where the move was learned</param>
/// <param name="Argument">Conditions in which the move was learned</param>
public readonly record struct MoveLearnInfo(LearnMethod Method, LearnEnvironment Environment, byte Argument = 0)
{
    public void Summarize(StringBuilder sb)
    {
        var localized = GetLocalizedMethod();
        Summarize(sb, localized);
    }

    private void Summarize(StringBuilder sb, string localizedMethod)
    {
        if (Environment.IsSpecified())
            sb.Append(Environment).Append('-');
        sb.Append(localizedMethod);
        if (Method is LevelUp)
            sb.Append(" @ lv").Append(Argument);
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
        Unobtainable or UnobtainableExpect => LMoveSourceInvalid,
        Duplicate => LMoveSourceDuplicate,
        EmptyInvalid => LMoveSourceEmpty,

        _ => throw new ArgumentOutOfRangeException(nameof(Method), Method, null),
    };
}
