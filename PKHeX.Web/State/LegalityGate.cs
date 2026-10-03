namespace PKHeX.Web.State;

/// <summary>What legality asks before the open draft, as it is now, can be applied; see <see cref="DraftLegality.Gate"/>.</summary>
/// <remarks>
/// Legality findings are warnings, not a prohibition on saving an illegal Pokémon, but they are never passed over silently: an applied change
/// carries the verdict it was acknowledged with, and the download that contains it asks again (<see cref="SaveSession.ExportNeedsAcknowledgement"/>).
/// </remarks>
public enum LegalityGate
{
    /// <summary>The draft as it is now has no result yet (not analysed, pending, stale, or its last edit was refused), so it cannot be applied.</summary>
    Waiting,

    /// <summary>The draft as it is now is <see cref="LegalityVerdict.Valid"/>.</summary>
    Clear,

    /// <summary>The draft as it is now is <see cref="LegalityVerdict.Invalid"/> or <see cref="LegalityVerdict.Unavailable"/>, and the user has not acknowledged it.</summary>
    NeedsAcknowledgement,

    /// <summary>The user acknowledged the Invalid or Unavailable result of the draft as it is now; any accepted edit withdraws it.</summary>
    Acknowledged,
}
