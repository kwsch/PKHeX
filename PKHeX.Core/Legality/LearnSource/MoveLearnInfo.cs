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
    /// <inheritdoc cref="Summarize(StringBuilder, ReadOnlySpan{char})"/>
    public void Summarize(StringBuilder sb)
    {
        var localized = GetLocalizedMethod();
        Summarize(sb, localized);
    }

    /// <summary>
    /// Summarizes the move learn info into a human-readable format and appends it to the provided <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to append the summary to.</param>
    /// <param name="localizedMethod">The localized string representing the learning method.</param>
    private void Summarize(StringBuilder sb, ReadOnlySpan<char> localizedMethod)
    {
        if (Environment.IsSpecified())
            sb.Append(Environment).Append('-');
        sb.Append(localizedMethod);
        if (Method is LevelUp)
            sb.Append($" @ lv{Argument}");
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

        HOME => LMoveSourceSpecial,
        Evolution => LMoveSourceSpecial,
        Encounter => LMoveSourceSpecial,
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
