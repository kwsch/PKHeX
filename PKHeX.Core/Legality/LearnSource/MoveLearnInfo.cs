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
    /// <summary>
    /// Summarizes the move learn info into a human-readable format and appends it to the provided <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to append the summary to.</param>
    /// <param name="strings">The localized strings to use for displaying the learning method.</param>
    public void Summarize(StringBuilder sb, MoveSourceLocalization strings)
    {
        var localizedMethod = strings.Localize(Method);
        if (Environment.IsSpecified)
            sb.Append(Environment).Append('-');
        sb.Append(localizedMethod);
        if (Method is LevelUp)
            sb.AppendFormat(strings.LevelUpSuffix, Argument);
    }
}
