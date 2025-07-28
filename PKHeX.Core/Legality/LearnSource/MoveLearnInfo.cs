using System;
using System.Text;
using static PKHeX.Core.LearnMethod;

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
    public void Summarize(StringBuilder sb, MoveSourceLocalization strings)
    {
        var localized = GetLocalizedMethod(strings);
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

    private string GetLocalizedMethod(MoveSourceLocalization strings) => Method switch
    {
        Empty => strings.SourceEmpty,
        Relearn => strings.SourceRelearn,
        Initial => strings.SourceDefault,
        LevelUp => strings.SourceLevelUp,
        TMHM => strings.SourceTMHM,
        Tutor => strings.SourceTutor,
        Sketch => strings.SourceShared,
        EggMove => strings.RelearnEgg,
        InheritLevelUp => strings.EggInherited,

        HOME => strings.SourceSpecial,
        Evolution => strings.SourceSpecial,
        Encounter => strings.SourceSpecial,
        SpecialEgg => strings.SourceSpecial,
        ShedinjaEvo => strings.SourceSpecial,

        Shared => strings.SourceShared,

        // Invalid
        None => strings.SourceInvalid,
        Unobtainable or UnobtainableExpect => strings.SourceInvalid,
        Duplicate => strings.SourceDuplicate,
        EmptyInvalid => strings.SourceEmpty,

        _ => throw new ArgumentOutOfRangeException(nameof(Method), Method, null),
    };
}
