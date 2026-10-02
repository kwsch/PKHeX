using PKHeX.Core;

namespace PKHeX.Web.State;

/// <summary>
/// The PK6 party-stat policy: what an edit does to a party member's battle stats, current HP and status condition.
/// </summary>
/// <remarks>
/// <para>
/// Core keeps a party member's stored stats when it writes it (<c>SaveFile.SetPartyValues</c> skips members whose stats are present),
/// and <see cref="PKM.ResetPartyStats"/> recalculates them but also restores full HP and clears the status. Neither is what a player
/// expects from an edit, so this release follows the policy in <c>PKHeX.Web.md</c> §State model:
/// </para>
/// <list type="bullet">
/// <item>An edit that does not affect stats keeps the stored stats, HP and status byte for byte.</item>
/// <item>An edit that does (species/form, level/EXP, nature, IVs/EVs) recalculates the stats through Core, keeps the status, and
/// sets current HP to the smaller of the previous HP and the new maximum, so a fainted member stays fainted and nothing is healed.</item>
/// </list>
/// <para>It is written for PK6 only. Later families need their own explicit policy rather than inheriting this one.</para>
/// </remarks>
internal static class PartyStatPolicy
{
    /// <summary>
    /// Recalculates the party stats of <paramref name="pk"/> after a stat-affecting edit, keeping its status condition and never raising its current HP.
    /// </summary>
    /// <remarks>
    /// Move PP is left alone: an edit never refills it. The member must already have party stats; one without them has no current HP to
    /// keep, and would be left fainted.
    /// </remarks>
    /// <exception cref="ArgumentException"><paramref name="pk"/> has no party stats (<see cref="PKM.PartyStatsPresent"/>).</exception>
    public static void Recalculate(PK6 pk)
    {
        if (!pk.PartyStatsPresent)
        {
            throw new ArgumentException("The member has no party stats to recalculate from.", nameof(pk));
        }
        var status = pk.Status_Condition;
        var hp = pk.Stat_HPCurrent;
        // Sets the six stats and the party level from the entity, but also fills HP and clears the status, which are restored below.
        pk.ResetPartyStats();
        pk.Status_Condition = status;
        pk.Stat_HPCurrent = Math.Min(hp, pk.Stat_HPMax);
    }
}

/// <summary>
/// How a draft changes a party member's current and maximum HP, shown before the draft is applied.
/// </summary>
/// <param name="PreviousHp">Current HP as stored.</param>
/// <param name="NewHp">Current HP as drafted.</param>
/// <param name="PreviousMax">Maximum HP as stored.</param>
/// <param name="NewMax">Maximum HP as drafted.</param>
public readonly record struct PartyHpChange(int PreviousHp, int NewHp, int PreviousMax, int NewMax)
{
    /// <summary>True when applying the draft lowers the member's current HP.</summary>
    public bool IsReduction => NewHp < PreviousHp;

    /// <summary>True when the member is fainted (zero HP) in the draft.</summary>
    public bool IsFainted => NewHp == 0;

    /// <summary>The change from <paramref name="stored"/> to <paramref name="drafted"/>, or null when neither current nor maximum HP changes.</summary>
    public static PartyHpChange? Between(PKM stored, PKM drafted)
    {
        var change = new PartyHpChange(stored.Stat_HPCurrent, drafted.Stat_HPCurrent, stored.Stat_HPMax, drafted.Stat_HPMax);
        return change.PreviousHp == change.NewHp && change.PreviousMax == change.NewMax ? null : change;
    }
}
