using PKHeX.Core;
using PKHeX.Web.Services;

namespace PKHeX.Web.State;

/// <summary>
/// An unapplied edit of one party member or boxed entity. The working save is untouched until <see cref="SaveSession.Apply(EditorDraft, LegalityVerdict)"/>.
/// </summary>
/// <remarks>
/// The draft owns a private clone of the slot's entity, and its typed edit methods (such as <see cref="TypeNickname"/>) are the only
/// code that changes it. Nothing outside the draft is ever handed that clone: readers get a copy through <see cref="Preview"/>, so an
/// apply carries exactly the changes the edit methods made, and every other stored byte stays as it was read.
/// </remarks>
public sealed class EditorDraft
{
    private readonly PK6 baseline;
    private readonly PK6 working;

    /// <summary>
    /// The PP Ups a move change gives each slot's new move: the count the slot last had on a move that can take them (at first, the stored
    /// count), so passing through a move without PP Ups or an empty slot does not lose them (see <see cref="EditMove"/>).
    /// </summary>
    private readonly int[] carriedPpUps = new int[MoveCount];

    /// <summary><see cref="SaveSession.SessionId"/> of the session the draft was taken from.</summary>
    public Guid SessionId { get; }

    /// <summary>The party position or box slot the draft was taken from.</summary>
    public SlotRef Slot { get; }

    /// <summary>What the session's save allows, which decides the fields that can be drafted and whether the draft can be applied.</summary>
    public SaveCapabilities Capabilities { get; }

    /// <summary>
    /// True when <see cref="SaveSession.Apply(EditorDraft, LegalityVerdict)"/> can write the draft back (see <see cref="SaveCapabilities.CanApply"/>).
    /// Party members can be applied only in families with a party-stat policy (<see cref="SupportedFamily.WritesParty"/>).
    /// </summary>
    public bool CanApply => Capabilities.CanApply(Slot);

    /// <summary>
    /// Fields the user can change in this draft: none when it cannot be applied, so nothing is drafted that could never be kept, and none
    /// for an egg (<see cref="SessionError.EggNotEditable"/>).
    /// </summary>
    public EditableFields Editable => CanApply && !IsEgg ? Capabilities.Editable : EditableFields.None;

    /// <summary>True when the drafted Pokémon is an egg. No edit changes the egg state.</summary>
    public bool IsEgg => baseline.IsEgg;

    /// <summary><see cref="SaveSession.Revision"/> the draft was taken from.</summary>
    public int SourceRevision { get; }

    /// <summary>Longest nickname the save format can store.</summary>
    public int MaxNicknameLength => Capabilities.MaxNicknameLength;

    /// <summary>
    /// Number of edits accepted since the draft was taken. It tags legality results (<see cref="LegalityTag"/>), so a result for an
    /// earlier edit is stale at once. A refused edit leaves it unchanged, because it leaves the draft unchanged.
    /// </summary>
    public int EditRevision { get; private set; }

    /// <summary>Drafted nickname text.</summary>
    public string Nickname => working.Nickname;

    /// <summary>Drafted nickname flag.</summary>
    public bool IsNicknamed => working.IsNicknamed;

    /// <summary>Drafted language.</summary>
    public int Language => working.Language;

    /// <summary>What the drafted name means: its default for the language, a name kept from another language, and how the game shows it.</summary>
    public NameStatus Name => NameRules.Describe(working, Capabilities.SaveLanguage);

    /// <summary>The original trainer's name, as stored.</summary>
    public string TrainerName => working.OriginalTrainerName;

    /// <summary>Drafted friendship towards the original trainer (for an egg, the hatch counter).</summary>
    public byte TrainerFriendship => working.OriginalTrainerFriendship;

    /// <summary>The handling trainer's name, as stored; empty when the Pokémon has never left its original trainer.</summary>
    public string HandlerName => working.HandlingTrainerName;

    /// <summary>True when a handling trainer is stored, so friendship towards it can be edited.</summary>
    public bool HasHandlingTrainer => HandlerName.Length != 0;

    /// <summary>Drafted friendship towards the handling trainer.</summary>
    public byte HandlerFriendship => working.HandlingTrainerFriendship;

    /// <summary>
    /// True when the stored current handler is the handling trainer, so the game uses <see cref="HandlerFriendship"/>; otherwise it uses
    /// <see cref="TrainerFriendship"/>. No edit changes the current handler.
    /// </summary>
    public bool IsWithHandler => working.CurrentHandler == 1;

    /// <summary>Drafted level, as Core derives it from the experience points.</summary>
    public byte Level => working.CurrentLevel;

    /// <summary>Drafted experience points.</summary>
    public uint Experience => working.EXP;

    /// <summary>Where the drafted experience points sit on the species' growth curve.</summary>
    public LevelProgress Progress => LevelProgress.Of(working);

    /// <summary>Drafted nature. In Generation 6 it is stored apart from the PID, and it is the nature the stats are calculated with.</summary>
    public Nature Nature => working.Nature;

    /// <summary>Number of stats with an IV and an EV.</summary>
    public const int StatCount = 6;

    /// <summary>Drafted individual values in the summary's stat order: HP, Attack, Defense, Sp. Atk, Sp. Def, Speed.</summary>
    public IReadOnlyList<int> Ivs => [.. CoreIndex.ToArray().Select(i => working.GetIV(i))];

    /// <summary>Drafted effort values in the summary's stat order: HP, Attack, Defense, Sp. Atk, Sp. Def, Speed.</summary>
    public IReadOnlyList<int> Evs => [.. CoreIndex.ToArray().Select(i => working.GetEV(i))];

    /// <summary>The sum of the drafted IVs.</summary>
    public int IvTotal => working.IVTotal;

    /// <summary>The sum of the drafted EVs.</summary>
    public int EvTotal => working.EVTotal;

    /// <summary>The highest IV the format stores.</summary>
    public int MaxIv => working.MaxIV;

    /// <summary>The highest EV the format allows in one stat.</summary>
    public int MaxEv => working.MaxEV;

    /// <summary>The highest EV total a Pokémon can hold.</summary>
    public int MaxEvTotal => EffortValues.Max510;

    /// <summary>The type Hidden Power has with the drafted IVs, as an index into Core's Hidden Power type names.</summary>
    public int HiddenPowerType => working.HPType;

    /// <summary>Drafted held item; 0 is none.</summary>
    public int HeldItem => working.HeldItem;

    /// <summary>Number of move slots.</summary>
    public const int MoveCount = 4;

    /// <summary>The most PP Ups a move can take. Core checks it per move (<see cref="Legal.IsPPUpAvailable(ushort)"/>) but names no limit; the desktop editor offers 0–3.</summary>
    public const int MaxPpUps = 3;

    /// <summary>The drafted move slots, in order. An empty slot stays where it is: nothing reorders them.</summary>
    public IReadOnlyList<MoveSlot> Moves => [.. Enumerable.Range(0, MoveCount).Select(i => MoveSlot.Of(working, i))];

    /// <summary>The move slots as stored in the slot the draft was taken from, so an edit's effect can be told from a return to the stored move.</summary>
    public IReadOnlyList<MoveSlot> StoredMoves => [.. Enumerable.Range(0, MoveCount).Select(i => MoveSlot.Of(baseline, i))];

    /// <summary>Drafted ability, as stored.</summary>
    public int Ability => working.Ability;

    /// <summary>Drafted ability slot number, as stored: 1 first, 2 second, 4 hidden; any other value is not a valid slot.</summary>
    public int AbilityNumber => working.AbilityNumber;

    /// <summary>
    /// The drafted ability slot (0 first, 1 second, 2 hidden), or null when the stored ability and slot number do not name one of the
    /// species' slots together: the slot number is not 1, 2 or 4, or the ability is not the one in that slot. Such a pair is kept until
    /// a slot is chosen.
    /// </summary>
    public int? AbilitySlot
    {
        get
        {
            var number = working.AbilityNumber;
            if (!AbilityVerifier.IsValidAbilityBits(number))
            {
                return null;
            }
            var slot = number >> 1;
            var personal = working.PersonalInfo;
            return slot < personal.AbilityCount && personal.GetAbilityAtIndex(slot) == working.Ability ? slot : null;
        }
    }

    /// <summary>
    /// The ability slots of the drafted species and form, from Core's personal data and named as the desktop names them ("Levitate (1)",
    /// "Levitate (2)", "Levitate (H)"). Each value is the slot, not the ability, so two slots with the same ability are told apart.
    /// </summary>
    /// <remarks>The same list instance is returned until the species or form changes, so a select over it is not rebuilt on every edit.</remarks>
    public IReadOnlyList<ComboItem> AbilityChoices
    {
        get
        {
            var key = (working.Species, working.Form);
            if (abilityChoices is null || abilityChoicesFor != key)
            {
                var names = Capabilities.Lists.GetAbilityList(working.PersonalInfo);
                abilityChoices = [.. names.Select((item, slot) => item with { Value = slot })];
                abilityChoicesFor = key;
            }
            return abilityChoices;
        }
    }

    /// <summary>True when the drafted species and form have the same ability in the first and second slots, so only the slot number tells them apart.</summary>
    public bool RegularAbilitiesSame => working.PersonalInfo is IPersonalAbility12 { IsAbility12Same: true };

    private IReadOnlyList<ComboItem>? abilityChoices;
    private (ushort Species, byte Form) abilityChoicesFor;

    /// <summary>Drafted gender, as stored: 0 male, 1 female, 2 genderless; any other value is not a gender.</summary>
    public byte Gender => working.Gender;

    /// <summary>Which genders the drafted species and form can have.</summary>
    public GenderRule GenderRule => GenderRules.Of(working.PersonalInfo);

    /// <summary>The genders <see cref="EditGender"/> accepts, in the order the editor offers them.</summary>
    public IReadOnlyList<byte> GenderChoices => GenderRules.Allowed(GenderRule);

    /// <summary>
    /// True when the drafted form is a gender (Meowstic's male and female forms), so a gender edit changes the form with it, as the
    /// desktop editor does.
    /// </summary>
    public bool FormFollowsGender => FormGender(working) is not null;

    /// <summary>The drafted values that follow the form, which a gender edit of a species whose form is its gender can change.</summary>
    public FormDependents Dependents => new(working.Form, working.Ability, working.AbilityNumber, working.EXP);

    /// <summary>Drafted species, by national dex number.</summary>
    public ushort Species => working.Species;

    /// <summary>Drafted form, as an index of the species' form list.</summary>
    public byte Form => working.Form;

    /// <summary>
    /// The forms the drafted species can be changed to, from Core's form list for the Pokémon's generation (as the desktop editor lists them),
    /// or empty when the species has no alternate forms in the save's game, so it takes only form 0.
    /// </summary>
    /// <remarks>The same list instance is returned until the species changes, so a select over it is not rebuilt on every edit.</remarks>
    public IReadOnlyList<FormChoice> FormChoices
    {
        get
        {
            if (formChoices is null || formChoicesFor != working.Species)
            {
                formChoices = FormsOf(working.Species);
                formChoicesFor = working.Species;
            }
            return formChoices;
        }
    }

    private IReadOnlyList<FormChoice>? formChoices;
    private ushort formChoicesFor;

    /// <summary>True when any stored byte of the draft differs from the slot it was taken from.</summary>
    /// <remarks>Neither copy has its checksum refreshed in memory, so the checksum bytes cannot make an unchanged draft look dirty.</remarks>
    public bool IsDirty => !working.Data.SequenceEqual(baseline.Data);

    /// <summary>
    /// How applying the draft changes a party member's current and maximum HP (see <see cref="PartyStatPolicy"/>), or null for a box slot
    /// or when neither changes. The UI previews any reduction before the draft is applied.
    /// </summary>
    public PartyHpChange? HpChange => Slot.IsParty ? PartyHpChange.Between(baseline, working) : null;

    internal EditorDraft(Guid sessionId, SlotRef slot, int sourceRevision, PK6 source, SaveCapabilities capabilities)
    {
        SessionId = sessionId;
        Slot = slot;
        SourceRevision = sourceRevision;
        Capabilities = capabilities;
        baseline = (PK6)source.Clone();
        working = (PK6)source.Clone();
        for (var moveSlot = 0; moveSlot < MoveCount; moveSlot++)
        {
            CarryPpUps(moveSlot);
        }
    }

    /// <summary>
    /// Replaces the draft's nickname text and flag exactly as given. On failure the previous values are kept.
    /// </summary>
    /// <remarks>
    /// Text equal to the stored nickname keeps the stored encoding, including any bytes after the terminator, so returning to the
    /// original text (or changing only the flag) never rewrites the name. New text is encoded by Core. The editor uses
    /// <see cref="TypeNickname"/> and <see cref="SetNicknamed"/>, which also apply the desktop's name rules.
    /// </remarks>
    /// <exception cref="SessionException">
    /// The family does not allow nickname edits, the Pokémon is an egg, or the text is too long, contains control characters, or cannot be
    /// stored unchanged.
    /// </exception>
    public void EditNickname(string nickname, bool isNicknamed)
    {
        Require(EditableFields.Nickname);
        var candidate = (PK6)working.Clone();
        SetName(candidate, nickname);
        candidate.IsNicknamed = isNicknamed;
        Commit(candidate, affectsStats: false);
    }

    /// <summary>
    /// Replaces the nickname text as typed. The flag is set when the text is not the species' name in any language, and is never cleared
    /// by typing (<see cref="NameRules.FlagAfterTyping"/>). On failure the previous values are kept.
    /// </summary>
    /// <exception cref="SessionException">As for <see cref="EditNickname"/>.</exception>
    public void TypeNickname(string nickname)
    {
        Require(EditableFields.Nickname);
        var candidate = (PK6)working.Clone();
        SetName(candidate, nickname);
        candidate.IsNicknamed = NameRules.FlagAfterTyping(candidate, nickname, working.IsNicknamed);
        Commit(candidate, affectsStats: false);
    }

    /// <summary>
    /// Sets or clears the nickname flag. Clearing it gives the Pokémon its default name in its language, unless its name is already the
    /// species' name in some language (<see cref="NameRules.NameAfterReset"/>). On failure the previous values are kept.
    /// </summary>
    /// <exception cref="SessionException">As for <see cref="EditNickname"/>.</exception>
    public void SetNicknamed(bool isNicknamed)
    {
        Require(EditableFields.Nickname);
        var candidate = (PK6)working.Clone();
        candidate.IsNicknamed = isNicknamed;
        if (!isNicknamed)
        {
            SetName(candidate, NameRules.NameAfterReset(candidate, working.Nickname, working.Language));
        }
        Commit(candidate, affectsStats: false);
    }

    /// <summary>
    /// Changes the language. A Pokémon that is not nicknamed is given its default name in the new language, unless its name is already
    /// the species' name in some language, which is kept (<see cref="NameRules.NameAfterReset"/>); <see cref="Name"/> then reports it.
    /// On failure the previous values are kept.
    /// </summary>
    /// <param name="language">A language from the session's list (<see cref="SaveCapabilities.Lists"/>).</param>
    /// <exception cref="SessionException">
    /// The family does not allow language edits, the Pokémon is an egg, the language is not in the game's list, or the new name cannot be
    /// stored (as for <see cref="EditNickname"/>).
    /// </exception>
    public void EditLanguage(int language)
    {
        Require(EditableFields.Language);
        if (!Capabilities.Lists.Languages.Any(l => l.Value == language))
        {
            throw new SessionException(SessionError.LanguageNotAvailable);
        }
        var candidate = (PK6)working.Clone();
        candidate.Language = language;
        if (!candidate.IsNicknamed)
        {
            var name = NameRules.NameAfterReset(candidate, working.Nickname, language);
            if (name != working.Nickname)
            {
                Require(EditableFields.Nickname);
                SetName(candidate, name);
            }
        }
        Commit(candidate, affectsStats: false);
    }

    /// <summary>
    /// Sets the friendship towards the original trainer. The value is refused outside 0–255, never clamped, and the current handler is not
    /// changed. Friendship does not affect Generation 6 stats. On failure the previous value is kept.
    /// </summary>
    /// <exception cref="SessionException">
    /// The family does not allow friendship edits, the Pokémon is an egg, or the value is outside 0–255.
    /// </exception>
    public void EditTrainerFriendship(int value)
    {
        Require(EditableFields.Friendship);
        var candidate = (PK6)working.Clone();
        candidate.OriginalTrainerFriendship = Friendship(value);
        Commit(candidate, affectsStats: false);
    }

    /// <summary>
    /// Sets the friendship towards the handling trainer, as <see cref="EditTrainerFriendship"/> does for the original trainer.
    /// </summary>
    /// <exception cref="SessionException">
    /// As for <see cref="EditTrainerFriendship"/>, or the Pokémon has no handling trainer (<see cref="SessionError.NoHandlingTrainer"/>).
    /// </exception>
    public void EditHandlerFriendship(int value)
    {
        Require(EditableFields.Friendship);
        if (!HasHandlingTrainer)
        {
            throw new SessionException(SessionError.NoHandlingTrainer);
        }
        var candidate = (PK6)working.Clone();
        candidate.HandlingTrainerFriendship = Friendship(value);
        Commit(candidate, affectsStats: false);
    }

    /// <summary>
    /// Sets the level. The experience points become the fewest for that level on the species' growth curve, as the desktop editor sets them;
    /// re-entering the current level keeps them, and returning to the stored level restores the stored ones. The value is refused outside 1–100, never clamped. On failure the previous values are kept.
    /// </summary>
    /// <remarks>
    /// Only the experience points are written. Core's <see cref="PKM.CurrentLevel"/> setter would also write the party level byte, which a
    /// boxed Pokémon does not store; a party member's is set by <see cref="PartyStatPolicy"/> with its stats.
    /// </remarks>
    /// <exception cref="SessionException">
    /// The family does not allow level edits, the Pokémon is an egg, the level is outside 1–100, or the member is stored without party stats
    /// (<see cref="SessionError.PartyStatsMissing"/>).
    /// </exception>
    public void EditLevel(int level)
    {
        Require(EditableFields.Level);
        if (level is < Core.Experience.MinLevel or > Core.Experience.MaxLevel)
        {
            throw new SessionException(SessionError.LevelOutOfRange);
        }
        var candidate = (PK6)working.Clone();
        if (level != candidate.CurrentLevel)
        {
            // Returning to the stored level, such as through "9" while retyping 90, gives back the stored experience points rather than the
            // start of the level, so the draft is clean again.
            var growth = candidate.PersonalInfo.EXPGrowth;
            candidate.EXP = Core.Experience.GetLevel(baseline.EXP, growth) == level ? baseline.EXP : Core.Experience.GetEXP((byte)level, growth);
        }
        Commit(candidate, affectsStats: true);
    }

    /// <summary>
    /// Sets the experience points; the level follows from the species' growth curve. The value is refused below 0 or above the maximum level's
    /// threshold (<see cref="LevelProgress.Maximum"/>), never clamped. On failure the previous values are kept.
    /// </summary>
    /// <exception cref="SessionException">
    /// The family does not allow level edits, the Pokémon is an egg, the value is out of range, or the member is stored without party stats.
    /// </exception>
    public void EditExperience(long experience)
    {
        Require(EditableFields.Level);
        if (experience < 0 || experience > Progress.Maximum)
        {
            throw new SessionException(SessionError.ExperienceOutOfRange);
        }
        var candidate = (PK6)working.Clone();
        candidate.EXP = (uint)experience;
        Commit(candidate, affectsStats: true);
    }

    /// <summary>
    /// Sets the nature. In Generation 6 the nature is stored apart from the PID, so the PID, and with it shininess, gender and the ability
    /// slot, is not changed. On failure the previous value is kept.
    /// </summary>
    /// <remarks>
    /// Formats that derive the nature from the PID (Generations 3 and 4) are not opened by this release, and would need a PID change instead.
    /// </remarks>
    /// <param name="nature">A nature from the session's list (<see cref="SaveCapabilities.Lists"/>).</param>
    /// <exception cref="SessionException">
    /// The family does not allow nature edits, the Pokémon is an egg, the nature is not in the game's list, or the member is stored without
    /// party stats.
    /// </exception>
    public void EditNature(int nature)
    {
        Require(EditableFields.Nature);
        if (!Capabilities.Lists.Natures.Any(n => n.Value == nature))
        {
            throw new SessionException(SessionError.NatureNotAvailable);
        }
        var candidate = (PK6)working.Clone();
        candidate.Nature = (Nature)nature;
        Commit(candidate, affectsStats: true);
    }

    /// <summary>
    /// Sets one individual value. The value is refused outside 0 to <see cref="MaxIv"/>, never clamped. Only that stat's bits are written,
    /// so the IVs of other stats and the egg and nickname flags stored beside them are kept. On failure the previous value is kept.
    /// </summary>
    /// <remarks>
    /// The range is checked here, not left to Core: the PK6 setter clamps a value above 31, and would write a negative one over the
    /// neighbouring bits.
    /// </remarks>
    /// <param name="stat">The stat, by its index in the summary order (see <see cref="Ivs"/>).</param>
    /// <param name="value">The new individual value.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="stat"/> is not 0–5.</exception>
    /// <exception cref="SessionException">
    /// The family does not allow IV edits, the Pokémon is an egg, the value is out of range, or the member is stored without party stats.
    /// </exception>
    public void EditIv(int stat, int value)
    {
        var index = ToCoreIndex(stat);
        Require(EditableFields.Ivs);
        if (value < 0 || value > MaxIv)
        {
            throw new SessionException(SessionError.IvOutOfRange);
        }
        var candidate = (PK6)working.Clone();
        candidate.SetIV(index, value);
        Commit(candidate, affectsStats: true);
    }

    /// <summary>
    /// Sets one effort value. The value is refused outside 0 to <see cref="MaxEv"/>, and refused when it raises the total above
    /// <see cref="MaxEvTotal"/>; it is never clamped. An edit that lowers the total is accepted even while the total stays above the limit,
    /// so a stored total over it can be brought down one stat at a time. On failure the previous value is kept.
    /// </summary>
    /// <param name="stat">The stat, by its index in the summary order (see <see cref="Evs"/>).</param>
    /// <param name="value">The new effort value.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="stat"/> is not 0–5.</exception>
    /// <exception cref="SessionException">
    /// The family does not allow EV edits, the Pokémon is an egg, the value is out of range, the total would rise above the limit, or the
    /// member is stored without party stats.
    /// </exception>
    public void EditEv(int stat, int value)
    {
        var index = ToCoreIndex(stat);
        Require(EditableFields.Evs);
        if (value < 0 || value > MaxEv)
        {
            throw new SessionException(SessionError.EvOutOfRange);
        }
        var total = working.EVTotal - working.GetEV(index) + value;
        if (total > MaxEvTotal && total > working.EVTotal)
        {
            throw new SessionException(SessionError.EvTotalAboveLimit);
        }
        var candidate = (PK6)working.Clone();
        candidate.SetEV(index, value);
        Commit(candidate, affectsStats: true);
    }

    /// <summary>
    /// Sets the held item. Only the item is written; it does not affect Generation 6 stats. A stored item outside the game's list is kept
    /// until it is changed. On failure the previous item is kept.
    /// </summary>
    /// <param name="item">An item from the session's list (<see cref="SaveCapabilities.Lists"/>), or 0 for none.</param>
    /// <exception cref="SessionException">
    /// The family does not allow held item edits, the Pokémon is an egg, or the item is not in the game's list.
    /// </exception>
    public void EditHeldItem(int item)
    {
        Require(EditableFields.HeldItem);
        if (!Capabilities.Lists.Items.Any(i => i.Value == item))
        {
            throw new SessionException(SessionError.ItemNotAvailable);
        }
        var candidate = (PK6)working.Clone();
        candidate.HeldItem = item;
        Commit(candidate, affectsStats: false);
    }

    /// <summary>
    /// Sets the move in one slot, and that slot's PP as the desktop editor sets them (<c>MoveChoice.HealPP</c>): the new move gets full PP
    /// for the PP Ups it keeps, an empty slot has no PP or PP Ups, and a move PP Ups cannot be used on has its PP Ups cleared. Returning to
    /// the stored move restores its stored PP and PP Ups, and choosing the drafted move again keeps them. On failure the previous values are kept.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The PP Ups a new move keeps are the slot's last count on a move that can take them, at first the stored count, not the count of the
    /// move it replaces. A move box picks a move a keystroke at a time as a name is typed into it, so "Sky Attack" passes through "Sketch";
    /// keeping only the replaced move's count would lose the PP Ups at that step. A stored count above <see cref="MaxPpUps"/> is never
    /// carried, so it cannot inflate a new move's PP.
    /// </para>
    /// <para>
    /// Only this slot's move, PP and PP Ups are written. The other slots keep their place, and an empty slot is not filled from the next
    /// one, as Core's <see cref="PKM.FixMoves"/> would do. The list holds every move the game has; it says nothing about whether this
    /// Pokémon can learn it, which legality analysis reports.
    /// </para>
    /// </remarks>
    /// <param name="slot">The move slot, 0–3.</param>
    /// <param name="move">A move from the session's list (<see cref="SaveCapabilities.Lists"/>), or 0 to empty the slot.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="slot"/> is not 0–3.</exception>
    /// <exception cref="SessionException">
    /// The family does not allow move edits, the Pokémon is an egg, or the move is not in the game's list.
    /// </exception>
    public void EditMove(int slot, int move)
    {
        CheckSlot(slot);
        Require(EditableFields.Moves);
        if (!Capabilities.Lists.Moves.Any(m => m.Value == move))
        {
            throw new SessionException(SessionError.MoveNotAvailable);
        }
        var candidate = (PK6)working.Clone();
        var id = (ushort)move;
        if (id != working.GetMove(slot))
        {
            candidate.SetMove(slot, id);
            if (id == baseline.GetMove(slot))
            {
                MoveSlots.SetPp(candidate, slot, MoveSlots.GetPp(baseline, slot), MoveSlots.GetPpUps(baseline, slot));
            }
            else
            {
                var ppUps = Legal.IsPPUpAvailable(id) ? carriedPpUps[slot] : 0;
                MoveSlots.SetPp(candidate, slot, id == 0 ? 0 : candidate.GetMovePP(id, ppUps), ppUps);
            }
        }
        Commit(candidate, affectsStats: false);
        CarryPpUps(slot);
    }

    /// <summary>
    /// Sets the number of PP Ups on one slot's move. The PP becomes the move's PP with the new count, as the desktop editor sets it; returning
    /// to the stored move's stored count restores the stored PP. The count is refused outside 0 to <see cref="MaxPpUps"/>, never clamped. On
    /// failure the previous values are kept.
    /// </summary>
    /// <param name="slot">The move slot, 0–3.</param>
    /// <param name="ppUps">The number of PP Ups.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="slot"/> is not 0–3.</exception>
    /// <exception cref="SessionException">
    /// The family does not allow PP edits, the Pokémon is an egg, the slot is empty, the count is out of range, or it is not 0 for a move
    /// PP Ups cannot be used on.
    /// </exception>
    public void EditPpUps(int slot, int ppUps)
    {
        CheckSlot(slot);
        Require(EditableFields.Pp);
        var move = working.GetMove(slot);
        if (move == 0)
        {
            throw new SessionException(SessionError.MoveSlotEmpty);
        }
        if (ppUps is < 0 or > MaxPpUps)
        {
            throw new SessionException(SessionError.PpUpsOutOfRange);
        }
        if (ppUps != 0 && !Legal.IsPPUpAvailable(move))
        {
            throw new SessionException(SessionError.PpUpsNotAllowed);
        }
        var candidate = (PK6)working.Clone();
        if (ppUps != MoveSlots.GetPpUps(working, slot))
        {
            var stored = move == baseline.GetMove(slot) && ppUps == MoveSlots.GetPpUps(baseline, slot);
            MoveSlots.SetPp(candidate, slot, stored ? MoveSlots.GetPp(baseline, slot) : candidate.GetMovePP(move, ppUps), ppUps);
        }
        Commit(candidate, affectsStats: false);
        CarryPpUps(slot);
    }

    /// <summary>
    /// Sets the current PP of one slot's move. The value is refused outside 0 to the move's PP with its PP Ups (<see cref="MoveSlot.MaxPp"/>),
    /// never clamped. On failure the previous value is kept.
    /// </summary>
    /// <remarks>
    /// PK6 stores PP in one byte, so a value above 255 is refused even when a stored PP Ups count above <see cref="MaxPpUps"/> makes Core's
    /// figure larger; Core's setter would keep only the low byte.
    /// </remarks>
    /// <param name="slot">The move slot, 0–3.</param>
    /// <param name="pp">The current PP.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="slot"/> is not 0–3.</exception>
    /// <exception cref="SessionException">
    /// The family does not allow PP edits, the Pokémon is an egg, the slot is empty, or the value is out of range.
    /// </exception>
    public void EditPp(int slot, int pp)
    {
        CheckSlot(slot);
        Require(EditableFields.Pp);
        var current = MoveSlot.Of(working, slot);
        if (current.IsEmpty)
        {
            throw new SessionException(SessionError.MoveSlotEmpty);
        }
        if (pp < 0 || pp > Math.Min(current.MaxPp, byte.MaxValue))
        {
            throw new SessionException(SessionError.PpOutOfRange);
        }
        var candidate = (PK6)working.Clone();
        MoveSlots.SetPp(candidate, slot, pp, current.PpUps);
        Commit(candidate, affectsStats: false);
    }

    /// <summary>
    /// Sets the ability slot. The ability becomes the one the species and form have in that slot, and the slot number is written with it,
    /// as the desktop editor writes them; nothing else changes. On failure the previous values are kept.
    /// </summary>
    /// <remarks>
    /// In Generation 6 the ability and its slot number are stored apart from the PID, so no PID change is needed (Core's
    /// <see cref="CommonEdits.SetAbilityIndex"/> changes the PID only for Generations 3–5). Choosing the stored slot of a stored pair that
    /// names it gives back the stored bytes. Whether the Pokémon could have the hidden ability depends on how it was met, which legality
    /// analysis reports.
    /// </remarks>
    /// <param name="slot">The slot: 0 first, 1 second, 2 hidden (see <see cref="AbilityChoices"/>).</param>
    /// <exception cref="SessionException">
    /// The family does not allow ability edits, the Pokémon is an egg, or the species and form have no such slot.
    /// </exception>
    public void EditAbilitySlot(int slot)
    {
        Require(EditableFields.Ability);
        if ((uint)slot >= (uint)working.PersonalInfo.AbilityCount)
        {
            throw new SessionException(SessionError.AbilitySlotNotAvailable);
        }
        var candidate = (PK6)working.Clone();
        candidate.SetAbilityIndex(slot);
        Commit(candidate, affectsStats: false);
    }

    /// <summary>
    /// Sets the gender, which must be one the species can have (<see cref="GenderChoices"/>). A species with a single gender can only be
    /// given that gender, which corrects a wrong stored value, as the desktop editor does. On failure the previous values are kept.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In Generation 6 the gender is stored apart from the PID, so no PID change is made. A Pokémon from Generations 3–5 has its gender
    /// checked against its PID by legality analysis, which reports a mismatch; this release does not change PIDs.
    /// </para>
    /// <para>
    /// When the form is the gender (<see cref="FormFollowsGender"/>), the form changes with it through Core's
    /// <see cref="SpeciesFormChange.ChangeSpeciesForm(PKM,ushort,byte,IPersonalTable,int)"/>, as the desktop editor changes it: the ability
    /// slot is kept and its ability taken from the new form, and the experience points become the fewest for the level. It is a form change
    /// like <see cref="EditSpeciesForm"/>, and changing back undoes it the same way: when none of the values it changed has been edited
    /// since, the experience points, ability and slot number from before are given back, so changing the gender and back leaves the draft as
    /// it was; once one has been edited, changing back sets them as Core does and keeps the edit.
    /// </para>
    /// </remarks>
    /// <param name="gender">The gender: 0 male, 1 female, 2 genderless.</param>
    /// <exception cref="SessionException">
    /// The family does not allow gender edits, the Pokémon is an egg, the species cannot have that gender, or the form follows the gender and
    /// the member is stored without party stats.
    /// </exception>
    public void EditGender(int gender)
    {
        Require(EditableFields.Gender);
        if (!GenderChoices.Any(g => g == gender))
        {
            throw new SessionException(SessionError.GenderNotAvailable);
        }
        var candidate = (PK6)working.Clone();
        var value = (byte)gender;
        if (FormGender(working) is not { } formCount)
        {
            candidate.Gender = value;
            Commit(candidate, affectsStats: false);
            return;
        }

        // The desktop picks the form at the gender's place in the form list (PKMEditor.ClickGender).
        var form = (byte)Math.Min(value, formCount - 1);
        var before = (PK6)working.Clone();
        var (_, restored) = ChangeSpeciesForm(candidate, working.Species, form);
        candidate.Gender = value; // Core sets it from the form; set here too in case the form already matched and nothing was changed.
        Commit(candidate, affectsStats: true);
        RecordFormChange(before, restored);
    }

    /// <summary>
    /// Shows what changing the species and form would do, without changing the draft (WEB-PKM-002): the dependent fields Core changes with
    /// them, their values before and after, a party member's recalculated stats and HP, and whether the form is battle-only or missing from
    /// the save's game. It is refused exactly as <see cref="EditSpeciesForm"/> would be.
    /// </summary>
    /// <param name="species">A species from <see cref="SaveCapabilities.SpeciesChoices"/>.</param>
    /// <param name="form">A form from the species' <see cref="FormChoices"/>, or 0 for a species without alternate forms.</param>
    /// <exception cref="SessionException">As for <see cref="EditSpeciesForm"/>.</exception>
    public SpeciesFormPreview PreviewSpeciesForm(int species, int form)
    {
        var (candidate, changes, restored) = SpeciesFormCandidate(species, form);
        var party = Slot.IsParty;
        var forms = candidate.Species == working.Species ? FormChoices : FormsOf(candidate.Species);
        return new SpeciesFormPreview(
            SpeciesFormValues.Of(working, party),
            SpeciesFormValues.Of(candidate, party),
            changes,
            FormInfo.IsBattleOnlyForm(candidate.Species, candidate.Form, candidate.Format),
            Capabilities.Personal.IsPresentInGame(candidate.Species, candidate.Form),
            restored,
            party ? PartyHpChange.Between(working, candidate) : null,
            forms);
    }

    /// <summary>
    /// Changes the species and form through Core's <see cref="SpeciesFormChange.ChangeSpeciesForm(PKM,ushort,byte,IPersonalTable)"/>, which
    /// updates the fields that follow them as the desktop editor does: the experience points become the fewest for the level they give on the
    /// new growth curve, the ability keeps its slot and takes the new species' ability in it, the gender is made one the species can have
    /// (and follows a gendered form such as Meowstic's), and a Pokémon that is not nicknamed is given the new species' name. The PID is never
    /// changed. On failure the previous values are kept.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Changing back to the species and form a run of species and form changes started from, with none of the values they changed edited
    /// since, gives back the values the run started with (experience points, ability and slot number, gender and name), so changing and
    /// changing back leaves the draft as it was. Once one of those values is edited, a later change sets it as Core does.
    /// </para>
    /// <para>
    /// Moves, IVs, EVs, nature, held item, the PID and form timers (Furfrou's trim, Hoopa's unbound days) are kept as stored. Whether the
    /// Pokémon can be this species and form, know its moves or be in a battle-only form outside battle is reported by legality analysis.
    /// </para>
    /// </remarks>
    /// <param name="species">A species from <see cref="SaveCapabilities.SpeciesChoices"/>.</param>
    /// <param name="form">A form from the species' <see cref="FormChoices"/>, or 0 for a species without alternate forms.</param>
    /// <exception cref="SessionException">
    /// The family does not allow species edits, the Pokémon is an egg, the species is not in the game's list, the form is not in the species'
    /// form list, or the member is stored without party stats.
    /// </exception>
    public void EditSpeciesForm(int species, int form)
    {
        var before = (PK6)working.Clone();
        var (candidate, _, restored) = SpeciesFormCandidate(species, form);
        Store(candidate);
        RecordFormChange(before, restored);
    }

    /// <summary>
    /// A validated copy of the draft changed to <paramref name="species"/> and <paramref name="form"/>, with a party member's battle state
    /// settled as an apply will store it, and the dependent fields the change altered.
    /// </summary>
    private (PK6 Candidate, SpeciesFormChangeResult Changes, bool Restored) SpeciesFormCandidate(int species, int form)
    {
        Require(EditableFields.Species);
        if (!Capabilities.SpeciesChoices.Any(s => s.Value == species))
        {
            throw new SessionException(SessionError.SpeciesNotAvailable);
        }
        var forms = species == working.Species ? FormChoices : FormsOf((ushort)species);
        if (forms.Count == 0 ? form != 0 : (uint)form >= (uint)forms.Count)
        {
            throw new SessionException(SessionError.FormNotAvailable);
        }
        var candidate = (PK6)working.Clone();
        var (changes, restored) = ChangeSpeciesForm(candidate, (ushort)species, (byte)form);
        Settle(candidate, affectsStats: true);
        return (candidate, changes, restored);
    }

    /// <summary>
    /// The forms <paramref name="species"/> can be changed to: Core's form list for the generation when the save's personal data gives the
    /// species a form choice (<see cref="FormInfo.HasFormSelection"/>) and the list has more than one form, as the desktop editor shows its
    /// form box (<c>PKMEditor.SetForms</c>); otherwise none.
    /// </summary>
    private IReadOnlyList<FormChoice> FormsOf(ushort species)
    {
        var personal = Capabilities.Personal;
        if (!FormInfo.HasFormSelection(personal[species], species, working.Format))
        {
            return [];
        }
        var strings = GameInfo.Strings;
        var names = FormConverter.GetFormList(species, strings.types, strings.forms, GameInfo.GenderSymbolUnicode, working.Context);
        if (names.Length <= 1)
        {
            return [];
        }
        return [.. names.Select((name, i) => new FormChoice((byte)i, name, FormInfo.IsBattleOnlyForm(species, (byte)i, working.Format), personal.IsPresentInGame(species, (byte)i)))];
    }

    /// <summary>
    /// Changes <paramref name="candidate"/>'s species and form through Core, keeping its ability slot. When the change returns to where the
    /// current run of changes started (<see cref="formChanges"/>) and nothing it changed has been edited since, the values the run started
    /// with are given back instead.
    /// </summary>
    /// <returns>
    /// The dependent fields changed (Core's flags, or for a return the fields that differ from the draft), and whether the run was undone.
    /// </returns>
    private (SpeciesFormChangeResult Changes, bool Restored) ChangeSpeciesForm(PK6 candidate, ushort species, byte form)
    {
        var changes = candidate.ChangeSpeciesForm(species, form, Capabilities.Personal);
        if (formChanges is not { } run || DependentValues.Of(working) != run.After
            || (species, form) != (run.Origin.Species, run.Origin.Form) || (species, form) == (working.Species, working.Form))
        {
            return (changes, false);
        }
        var origin = run.Origin;
        candidate.EXP = origin.EXP;
        candidate.Ability = origin.Ability;
        candidate.AbilityNumber = origin.AbilityNumber;
        candidate.Gender = origin.Gender;
        origin.NicknameTrash.CopyTo(candidate.NicknameTrash);
        candidate.IsNicknamed = origin.IsNicknamed;
        return (Changed(working, candidate), true);
    }

    /// <summary>
    /// Keeps <see cref="formChanges"/> in step after an edit that may have changed the species or form: a return to the run's start ends it,
    /// a change continues the run it follows or starts a new one from <paramref name="before"/>, and an edit of a value a change alters ends it.
    /// </summary>
    /// <param name="before">The draft before the edit.</param>
    /// <param name="restored">True when the edit returned to the run's start.</param>
    private void RecordFormChange(PK6 before, bool restored)
    {
        var after = DependentValues.Of(working);
        if (restored)
        {
            formChanges = null;
        }
        else if ((before.Species, before.Form) != (working.Species, working.Form))
        {
            formChanges = formChanges is { } run && DependentValues.Of(before) == run.After ? run with { After = after } : new FormChangeRun(before, after);
        }
        else if (formChanges is { } run && after != run.After)
        {
            formChanges = null;
        }
    }

    /// <summary>
    /// The run of species and form changes the draft is in, so changing back can give back what the run changed: the draft before its first
    /// change, and the values that follow the species and form after its last. Null when there is none, or once it has been undone. Another
    /// edit leaves it in place, but a change finds it broken once a value it records has been edited since (see <see cref="ChangeSpeciesForm"/>).
    /// </summary>
    private FormChangeRun? formChanges;

    /// <summary>A run of species and form changes: the draft before it, and the dependent values after its last change.</summary>
    private sealed record FormChangeRun(PK6 Origin, DependentValues After);

    /// <summary>
    /// The values a species or form change sets, and the language it names a Pokémon that is not nicknamed in, compared to tell whether any has
    /// been edited since. After a language edit, changing back must give the name Core gives in the new language, not the old language's.
    /// </summary>
    private readonly record struct DependentValues(ushort Species, byte Form, uint Experience, int Ability, int AbilityNumber, byte Gender, string Nickname, bool IsNicknamed, int Language)
    {
        public static DependentValues Of(PK6 pk) => new(pk.Species, pk.Form, pk.EXP, pk.Ability, pk.AbilityNumber, pk.Gender, pk.Nickname, pk.IsNicknamed, pk.Language);
    }

    /// <summary>The dependent fields that differ between <paramref name="before"/> and <paramref name="after"/>, as Core's change flags name them.</summary>
    private static SpeciesFormChangeResult Changed(PK6 before, PK6 after)
    {
        var result = SpeciesFormChangeResult.None;
        if (before.Form != after.Form)
        {
            result |= SpeciesFormChangeResult.Form;
        }
        if (before.EXP != after.EXP)
        {
            result |= SpeciesFormChangeResult.EXP;
        }
        if (before.Ability != after.Ability || before.AbilityNumber != after.AbilityNumber)
        {
            result |= SpeciesFormChangeResult.Ability;
        }
        if (before.Gender != after.Gender)
        {
            result |= SpeciesFormChangeResult.Gender;
        }
        if (before.PID != after.PID || before.EncryptionConstant != after.EncryptionConstant)
        {
            result |= SpeciesFormChangeResult.PID;
        }
        if (before.Nickname != after.Nickname || before.IsNicknamed != after.IsNicknamed)
        {
            result |= SpeciesFormChangeResult.Nickname;
        }
        return result;
    }

    /// <summary>
    /// The number of forms in <paramref name="pk"/>'s form list when its current form is a gender (Meowstic's "♂" and "♀"), or null when it
    /// is not. The desktop editor tells such forms by their names in the form list (<c>PKMEditor.ClickGender</c>); so does this.
    /// </summary>
    /// <remarks>
    /// Only a species with two genders has gendered forms, as the desktop checks before it looks at the form (<c>ClickGender</c> returns for
    /// any other): Unown's forms F and M also read as gender symbols.
    /// </remarks>
    private static int? FormGender(PK6 pk)
    {
        if (!pk.PersonalInfo.IsDualGender)
        {
            return null;
        }
        var strings = GameInfo.Strings;
        var forms = FormConverter.GetFormList(pk.Species, strings.types, strings.forms, GameInfo.GenderSymbolUnicode, pk.Context);
        return pk.Form < forms.Length && EntityGender.GetFromString(forms[pk.Form]) < EntityGender.Genderless ? forms.Length : null;
    }

    /// <summary>Records the drafted PP Ups of <paramref name="slot"/> for its next move change, when its move can take them and the count is one a move can have.</summary>
    private void CarryPpUps(int slot)
    {
        var current = MoveSlot.Of(working, slot);
        if (current.CanTakePpUps && current.PpUps <= MaxPpUps)
        {
            carriedPpUps[slot] = current.PpUps;
        }
    }

    /// <summary>Refuses a move slot outside 0–3, which is a programming error rather than user input.</summary>
    private static void CheckSlot(int slot)
    {
        if ((uint)slot >= MoveCount)
        {
            throw new ArgumentOutOfRangeException(nameof(slot), slot, "The move slot must be 0–3.");
        }
    }

    /// <summary>Core's stat index (HP, Attack, Defense, Speed, Sp. Atk, Sp. Def) of each stat in the summary order.</summary>
    private static ReadOnlySpan<byte> CoreIndex => [0, 1, 2, 4, 5, 3];

    /// <summary>Core's stat index for a summary-order <paramref name="stat"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="stat"/> is not 0–5.</exception>
    private static int ToCoreIndex(int stat) => (uint)stat < StatCount
        ? CoreIndex[stat]
        : throw new ArgumentOutOfRangeException(nameof(stat), stat, "The stat index must be 0–5.");

    /// <summary>Refuses an edit of <paramref name="field"/> that the family does not allow, and any edit of an egg.</summary>
    /// <remarks>
    /// Gated on the family, not the position: a party draft of a family without party writes can still be edited in memory, and Apply
    /// refuses it.
    /// </remarks>
    private void Require(EditableFields field)
    {
        if (!Capabilities.Editable.HasFlag(field))
        {
            throw new SessionException(SessionError.FieldNotEditable);
        }
        if (IsEgg)
        {
            throw new SessionException(SessionError.EggNotEditable);
        }
    }

    /// <summary>
    /// Stores <paramref name="nickname"/> in <paramref name="candidate"/>, refusing text the format cannot store unchanged.
    /// </summary>
    /// <remarks>
    /// The stored nickname keeps its stored bytes, and the drafted one is left as it is, so only new text is encoded by Core.
    /// </remarks>
    private void SetName(PK6 candidate, string nickname)
    {
        if (nickname.Length > MaxNicknameLength)
        {
            throw new SessionException(SessionError.NicknameTooLong);
        }
        if (nickname.Any(char.IsControl))
        {
            throw new SessionException(SessionError.NicknameInvalidCharacters);
        }
        if (nickname == baseline.Nickname)
        {
            baseline.NicknameTrash.CopyTo(candidate.NicknameTrash);
            return;
        }
        if (nickname == candidate.Nickname)
        {
            return;
        }
        candidate.Nickname = nickname;
        if (candidate.Nickname != nickname)
        {
            throw new SessionException(SessionError.NicknameNotRepresentable);
        }
    }

    /// <summary>A friendship value as stored, refusing one outside the byte's range rather than clamping it.</summary>
    private static byte Friendship(int value) => value is >= byte.MinValue and <= byte.MaxValue
        ? (byte)value
        : throw new SessionException(SessionError.FriendshipOutOfRange);

    /// <summary>
    /// Applies an arbitrary change to the draft through the same commit path as the typed edit methods.
    /// </summary>
    /// <remarks>
    /// Test-only: it reaches states no typed edit method can, such as an edited egg, so the later checks (Apply, export) can be tested on
    /// them. Stat edits go through the typed methods (<see cref="EditLevel"/>, <see cref="EditNature"/>,
    /// <see cref="EditIv"/>, <see cref="EditEv"/>).
    /// </remarks>
    /// <param name="change">The change to make on a copy of the drafted entity.</param>
    /// <param name="affectsStats">Whether the change affects calculated stats, as a typed edit method would declare.</param>
    internal void EditForTest(Action<PK6> change, bool affectsStats)
    {
        var candidate = (PK6)working.Clone();
        change(candidate);
        Commit(candidate, affectsStats);
    }

    /// <summary>
    /// Makes a validated <paramref name="candidate"/> the drafted entity and counts the edit.
    /// </summary>
    /// <remarks>
    /// For a party member, an edit that affects stats has its stats recalculated by <see cref="PartyStatPolicy"/>, so status is kept and
    /// HP is never raised, unless the edit leaves the calculation as it was for the stored member, which keeps its stored battle state; any
    /// other edit keeps the stored stats, HP and status as they are.
    /// </remarks>
    /// <param name="candidate">A changed copy of the drafted entity.</param>
    /// <param name="affectsStats">True for an edit of species/form (including a gender edit that changes the form), level/EXP, nature, IVs or EVs.</param>
    /// <exception cref="SessionException">
    /// <see cref="SessionError.PartyStatsMissing"/>: a stat edit of a party member stored without stats, which has no current HP to keep.
    /// </exception>
    private void Commit(PK6 candidate, bool affectsStats)
    {
        Settle(candidate, affectsStats);
        Store(candidate);
    }

    /// <summary>Settles a party member's battle state in <paramref name="candidate"/> for the edit, as <see cref="Commit"/> describes.</summary>
    /// <exception cref="SessionException"><see cref="SessionError.PartyStatsMissing"/>, as for <see cref="Commit"/>.</exception>
    private void Settle(PK6 candidate, bool affectsStats)
    {
        if (affectsStats && Slot.IsParty)
        {
            if (!baseline.PartyStatsPresent)
            {
                throw new SessionException(SessionError.PartyStatsMissing);
            }
            PartyStatPolicy.AfterStatEdit(candidate, baseline);
        }
    }

    /// <summary>Makes a settled <paramref name="candidate"/> the drafted entity and counts the edit.</summary>
    private void Store(PK6 candidate)
    {
        candidate.Data.CopyTo(working.Data);
        EditRevision++;
    }

    /// <summary>A copy of the drafted entity, for read-only use. Changing it does not change the draft.</summary>
    public PK6 Preview() => (PK6)working.Clone();

    /// <summary>The inspector's view of the drafted entity, including unapplied edits.</summary>
    /// <remarks>
    /// Edits do not refresh the draft's checksum; Core refreshes it when the entity is written. The inspected copy is refreshed the
    /// same way, so the checksum shown is the one an apply would store, not a stale one that would read as corruption.
    /// </remarks>
    public EntityInspection Inspect()
    {
        var copy = Preview();
        copy.RefreshChecksum();
        return EntityInspection.From(copy, Slot, Capabilities, statsRecalculated: Slot.IsParty && StatsRecalculated);
    }

    /// <summary>True when the drafted battle state (stats, level, HP, status) differs from the stored one, which only a stat edit does.</summary>
    private bool StatsRecalculated => !working.Data[working.SIZE_STORED..].SequenceEqual(baseline.Data[baseline.SIZE_STORED..]);

    /// <summary>The entity to store or analyse: a copy of the drafted entity.</summary>
    internal PK6 ToStoredEntity() => Preview();
}
