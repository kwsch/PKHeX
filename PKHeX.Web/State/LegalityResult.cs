namespace PKHeX.Web.State;

/// <summary>Outcome of a legality analysis run on a draft.</summary>
/// <param name="Verdict">"Valid", "Invalid" or "Unavailable".</param>
/// <param name="Report">Core's text report.</param>
public sealed record LegalityResult(string Verdict, string Report);
