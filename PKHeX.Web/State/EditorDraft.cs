using PKHeX.Core;

namespace PKHeX.Web.State;

/// <summary>
/// An unapplied edit of one party member or boxed entity. The working save is untouched until <see cref="SaveSession.Apply"/>.
/// </summary>
/// <remarks>
/// Only the edited fields are held; the entity to store is rebuilt from the original slot contents,
/// so an apply cannot carry changes to any other field.
/// </remarks>
public sealed class EditorDraft
{
    private readonly PK6 baseline;

    /// <summary><see cref="SaveSession.SessionId"/> of the session the draft was taken from.</summary>
    public Guid SessionId { get; }

    /// <summary>The party position or box slot the draft was taken from.</summary>
    public SlotRef Slot { get; }

    /// <summary>
    /// True when <see cref="SaveSession.Apply"/> can write the draft back. Party members are inspected only: writing them needs the
    /// party-stat policy (stored stats, HP and status), which this release does not have yet.
    /// </summary>
    public bool CanApply => !Slot.IsParty;

    /// <summary><see cref="SaveSession.Revision"/> the draft was taken from.</summary>
    public int SourceRevision { get; }

    /// <summary>Longest nickname the save format can store.</summary>
    public int MaxNicknameLength { get; }

    /// <summary>Drafted nickname text.</summary>
    public string Nickname { get; private set; }

    /// <summary>Drafted nickname flag.</summary>
    public bool IsNicknamed { get; private set; }

    /// <summary>True when the draft differs from the slot it was taken from.</summary>
    public bool IsDirty => Nickname != baseline.Nickname || IsNicknamed != baseline.IsNicknamed;

    internal EditorDraft(Guid sessionId, SlotRef slot, int sourceRevision, PK6 source, int maxNicknameLength)
    {
        SessionId = sessionId;
        Slot = slot;
        SourceRevision = sourceRevision;
        MaxNicknameLength = maxNicknameLength;
        baseline = (PK6)source.Clone();
        Nickname = baseline.Nickname;
        IsNicknamed = baseline.IsNicknamed;
    }

    /// <summary>
    /// Replaces the draft's nickname fields. On failure the previous values are kept.
    /// </summary>
    /// <exception cref="SessionException">The text is too long, contains control characters, or cannot be stored unchanged.</exception>
    public void EditNickname(string nickname, bool isNicknamed)
    {
        if (nickname.Length > MaxNicknameLength)
        {
            throw new SessionException(SessionError.NicknameTooLong);
        }
        if (nickname.Any(char.IsControl))
        {
            throw new SessionException(SessionError.NicknameInvalidCharacters);
        }
        var candidate = (PK6)baseline.Clone();
        candidate.Nickname = nickname;
        if (candidate.Nickname != nickname)
        {
            throw new SessionException(SessionError.NicknameNotRepresentable);
        }
        Nickname = nickname;
        IsNicknamed = isNicknamed;
    }

    /// <summary>
    /// Runs Core legality analysis on the drafted entity, in the context of <paramref name="session"/>'s save and the draft's slot type
    /// (party or box).
    /// </summary>
    /// <exception cref="SessionException">The draft is foreign or stale.</exception>
    public LegalityResult Analyze(SaveSession session)
    {
        session.EnsureOwns(this);
        session.EnsureCurrent(this);
        var save = session.Working;
        var analysis = new LegalityAnalysis(ToStoredEntity(), save.Personal, Slot.ToSlotInfo(save).Type);
        var verdict = !analysis.Parsed ? "Unavailable" : analysis.Valid ? "Valid" : "Invalid";
        return new(verdict, analysis.Report());
    }

    /// <summary>
    /// Builds the entity to store: the original slot contents with only the edited fields copied across.
    /// </summary>
    internal PK6 ToStoredEntity()
    {
        var result = (PK6)baseline.Clone();
        result.Nickname = Nickname;
        result.IsNicknamed = IsNicknamed;
        return result;
    }
}
