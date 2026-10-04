using System.Globalization;
using PKHeX.Core;
using PKHeX.Web.State;

namespace PKHeX.Web.Tests;

/// <summary>How the published-journey drivers make one <see cref="JourneyStep"/> in the editor.</summary>
internal enum JourneyAction
{
    /// <summary>Types the value into a text field.</summary>
    Fill,

    /// <summary>
    /// A checkbox the previous step ticks: typing a name other than the species' ticks "Is nicknamed" (<c>NameRules.FlagAfterTyping</c>), so the
    /// step checks it is ticked rather than ticking it, which would clear it. The value is unused.
    /// </summary>
    Ticked,

    /// <summary>Chooses the option with the value from a select.</summary>
    Choose,

    /// <summary>Chooses the species with the value, then confirms its preview (<c>#species-confirm</c>).</summary>
    ConfirmSpecies,

    /// <summary>Types <see cref="JourneyStep.Query"/> into the select's search field (<c>#{id}-search</c>), then chooses the value from the narrowed list.</summary>
    Search,
}

/// <summary>One edit of the published journey: the editor control it is made in, how, and the value it leaves there.</summary>
/// <param name="ControlId">The control's id, as <see cref="Components.EditorFields.FieldsetOf"/> knows it.</param>
/// <param name="Action">How the edit is made.</param>
/// <param name="Value">
/// The text typed or the option value chosen; after the edit, and after reopening the export, the control holds it. The control never holds it
/// before the edit, so every step changes something.
/// </param>
/// <param name="Query">For <see cref="JourneyAction.Search"/>, the text typed into the search field.</param>
internal sealed record JourneyStep(string ControlId, JourneyAction Action, string Value = "", string? Query = null);

/// <summary>
/// The published journey's edits on one save and the output native Core gives for the same edits (WEB-TEST-005).
/// </summary>
/// <remarks>
/// <para>
/// A boxed Pokémon gets a species, name, friendship, nature, held item and move edit; a party member gets a level, IV, EV, ability, gender and PP
/// edit. Together they reach every fieldset of the editor once (<c>JourneyPlanTests</c> holds that). Each native recipe is the one the
/// RealSave round trips already prove byte for byte against the published app, so both tiers share one recipe for each edit.
/// </para>
/// <para>
/// The party member's battle state follows the PK6 party-stat policy: stats recalculated, the stored status kept, and current HP the smaller of
/// the stored HP and the new maximum. The policy works from the stored member, so the order of the edits does not change the result.
/// </para>
/// </remarks>
internal sealed class JourneyPlan
{
    private JourneyPlan(SaveFile native, SlotRef box, SlotRef party, string nickname)
    {
        Native = native;
        Box = box;
        Party = party;
        Nickname = nickname;
    }

    /// <summary>The save as opened.</summary>
    public SaveFile Native { get; }

    /// <summary>The boxed Pokémon edited.</summary>
    public SlotRef Box { get; }

    /// <summary>The party member edited.</summary>
    public SlotRef Party { get; }

    /// <summary>The nickname typed for the boxed Pokémon; a sentinel the privacy trace looks for.</summary>
    public string Nickname { get; }

    /// <summary>The boxed Pokémon's edits, in the order they are made: the species change first, since it renames a Pokémon that is not nicknamed.</summary>
    public IReadOnlyList<JourneyStep> BoxSteps { get; private set; } = [];

    /// <summary>The party member's edits, in the order they are made.</summary>
    public IReadOnlyList<JourneyStep> PartySteps { get; private set; } = [];

    /// <summary>The boxed Pokémon after its edits.</summary>
    public PK6 ExpectedBox { get; private set; } = null!;

    /// <summary>The party member after its edits and the party-stat policy.</summary>
    public PK6 ExpectedParty { get; private set; } = null!;

    /// <summary>The save with both edits made through Core, for the legality context of each.</summary>
    public SaveFile Changed { get; private set; } = null!;

    /// <summary>The save written without a change, as a no-op download gives it.</summary>
    public byte[] NoOp { get; private set; } = [];

    /// <summary>The save written with both edits, as the edited download must give it.</summary>
    public byte[] ExpectedEdited { get; private set; } = [];

    /// <summary>Every step, box first.</summary>
    public IEnumerable<JourneyStep> Steps => BoxSteps.Concat(PartySteps);

    /// <summary>
    /// Plans the journey on <paramref name="native"/>: the first writable boxed Pokémon that is not an egg, and the first party member that can be
    /// edited in every group (see <see cref="FindPartyMember"/>). The save is not changed.
    /// </summary>
    /// <exception cref="InvalidOperationException">The save has no such boxed Pokémon or party member; the message names no stored value.</exception>
    public static JourneyPlan For(SaveFile native)
    {
        var box = FirstWritableNonEgg(native);
        var party = FindPartyMember(native);
        var stored = SaveFixtures.Slot(native, box).Read(native);
        var plan = new JourneyPlan(native, box, SlotRef.InParty(party), stored.Nickname == "Wayfarer" ? "Waystone" : "Wayfarer");
        plan.Build();
        return plan;
    }

    /// <summary>The first writable boxed PK6 that is not an egg, since eggs are not edited.</summary>
    /// <exception cref="InvalidOperationException">The save holds none.</exception>
    public static SlotRef FirstWritableNonEgg(SaveFile save)
    {
        for (var i = 0; i < save.SlotCount; i++)
        {
            var slot = SlotRef.InBox(i / save.BoxSlotCount, i % save.BoxSlotCount);
            var info = SaveFixtures.Slot(save, slot);
            var pk = info.Read(save);
            if (pk is PK6 { Species: not 0, ChecksumValid: true, IsEgg: false } && info.CanWriteTo(save) && info.CanWriteTo(save, pk) == WriteBlockedMessage.None)
            {
                return slot;
            }
        }
        throw new InvalidOperationException("The save contains no writable boxed PK6 that is not an egg.");
    }

    /// <summary>
    /// The first party member with party stats that is not an egg, can be male or female, is not Meowstic (whose form follows its gender), and has a
    /// first move with no more PP Ups than a move can take.
    /// </summary>
    /// <exception cref="InvalidOperationException">The save's party has none.</exception>
    private static int FindPartyMember(SaveFile save)
    {
        for (var i = 0; i < save.PartyCount; i++)
        {
            if (save.GetPartySlotAtIndex(i) is { IsEgg: false, PartyStatsPresent: true, PersonalInfo.IsDualGender: true, Species: not (ushort)Species.Meowstic, Move1: not 0, Move1_PPUps: <= 3 })
            {
                return i;
            }
        }
        throw new InvalidOperationException("The save has no party member that can be edited in every group (party stats, not an egg, either gender, not Meowstic, a first move).");
    }

    private void Build()
    {
        NoOp = Native.Clone().Write().ToArray();
        var changed = Native.Clone();

        // The boxed Pokémon: species, name, friendship, nature, held item and the first move.
        var boxed = (PK6)SaveFixtures.Slot(changed, Box).Read(changed);
        var species = boxed.Species == (ushort)Species.Linoone ? (ushort)Species.Zigzagoon : (ushort)Species.Linoone;
        boxed.ChangeSpeciesForm(species, 0, changed.Personal);
        boxed.Nickname = Nickname;
        boxed.IsNicknamed = true;
        var trainer = boxed.OriginalTrainerFriendship == 200 ? 201 : 200;
        boxed.OriginalTrainerFriendship = (byte)trainer;
        var hasHandler = boxed.HandlingTrainerName.Length != 0;
        var handler = boxed.HandlingTrainerFriendship == 100 ? 101 : 100;
        if (hasHandler)
        {
            boxed.HandlingTrainerFriendship = (byte)handler;
        }
        var nature = boxed.Nature == Nature.Adamant ? Nature.Modest : Nature.Adamant;
        boxed.Nature = nature;
        var item = boxed.HeldItem == ItemMoveDraftTests.Leftovers ? ItemMoveDraftTests.ChoiceScarf : ItemMoveDraftTests.Leftovers;
        boxed.HeldItem = item;
        var move = boxed.Move1 == (ushort)Move.Thunderbolt ? (ushort)Move.Surf : (ushort)Move.Thunderbolt;
        // A move change keeps the slot's PP Ups (none for an empty slot) and gives full PP.
        var ppUps = boxed.Move1 == 0 ? 0 : boxed.Move1_PPUps;
        if (ppUps > 3)
        {
            throw new InvalidOperationException("The boxed Pokémon's first move has more PP Ups than a move can take.");
        }
        boxed.Move1 = move;
        boxed.Move1_PPUps = ppUps;
        boxed.Move1_PP = boxed.GetMovePP(move, ppUps);
        if (!SaveFixtures.Slot(changed, Box).WriteTo(changed, boxed, EntityImportSettings.None))
        {
            throw new InvalidOperationException("Core refused the boxed Pokémon's edit.");
        }

        List<JourneyStep> boxSteps =
        [
            new("species", JourneyAction.ConfirmSpecies, Id(species)),
            new("nickname", JourneyAction.Fill, Nickname),
            new("nicknamed", JourneyAction.Ticked),
            new("ot-friendship", JourneyAction.Fill, Id(trainer)),
        ];
        if (hasHandler)
        {
            boxSteps.Add(new("ht-friendship", JourneyAction.Fill, Id(handler)));
        }
        boxSteps.Add(new("nature", JourneyAction.Choose, Id((int)nature)));
        boxSteps.Add(new("held-item", JourneyAction.Choose, Id(item)));
        boxSteps.Add(new("move-0", JourneyAction.Search, Id(move), GameInfo.Strings.movelist[move]));
        BoxSteps = boxSteps;

        // The party member: level, an IV, an EV, the ability slot, the gender and the first move's PP, then the party-stat policy.
        var member = (PK6)changed.GetPartySlotAtIndex(Party.Slot);
        var (hp, status) = (member.Stat_HPCurrent, member.Status_Condition);
        var level = LevelStep(member.CurrentLevel);
        member.EXP = Experience.GetEXP(level, member.PersonalInfo.EXPGrowth);
        var speedIv = IvStep(member.IV_SPE);
        member.IV_SPE = speedIv;
        var (evStat, evCore, ev) = EvStep(member);
        member.SetEV(evCore, ev);
        // The next slot after the stored one (the first for a slot number that names none), so the edit always changes the ability's slot.
        var slot = AbilityVerifier.IsValidAbilityBits(member.AbilityNumber) ? ((member.AbilityNumber >> 1) + 1) % member.PersonalInfo.AbilityCount : 0;
        member.SetAbilityIndex(slot);
        var gender = member.Gender == EntityGender.Male ? EntityGender.Female : EntityGender.Male;
        member.Gender = gender;
        var max = member.GetMovePP(member.Move1, member.Move1_PPUps);
        // One down, or up from none, within the move's PP, so the edit always changes the PP and is accepted.
        var pp = member.Move1_PP == 0 ? 1 : Math.Min(member.Move1_PP, max) - 1;
        member.Move1_PP = pp;
        member.ResetPartyStats();
        member.Status_Condition = status;
        member.Stat_HPCurrent = Math.Min(hp, member.Stat_HPMax);
        changed.SetPartySlotAtIndex(member, Party.Slot, EntityImportSettings.None);

        PartySteps =
        [
            new("level", JourneyAction.Fill, Id(level)),
            new("iv-5", JourneyAction.Fill, Id(speedIv)),
            new($"ev-{evStat}", JourneyAction.Fill, Id(ev)),
            new("ability", JourneyAction.Choose, Id(slot)),
            new("gender", JourneyAction.Choose, Id(gender)),
            new("pp-0", JourneyAction.Fill, Id(pp)),
        ];

        Changed = changed;
        ExpectedBox = (PK6)SaveFixtures.Slot(changed, Box).Read(changed);
        ExpectedParty = (PK6)changed.GetPartySlotAtIndex(Party.Slot);
        ExpectedEdited = changed.Clone().Write().ToArray();
    }

    /// <summary>
    /// Fails unless <paramref name="edited"/> differs from <see cref="NoOp"/> only in the edited box slot, the edited party position and the
    /// checksum footer.
    /// </summary>
    public void AssertOnlyTheEditedSlotsDiffer(byte[] edited)
    {
        // Take the party position as edited, then check the rest against the box slot.
        var partyOffset = Native.GetPartyOffset(Party.Slot);
        var withParty = NoOp.ToArray();
        edited.AsSpan(partyOffset, Native.SIZE_PARTY).CopyTo(withParty.AsSpan(partyOffset));
        ProofPage.AssertOnlyRangeDiffers(withParty, edited, Native.GetBoxSlotOffset(Box.Box, Box.Slot), Native.SIZE_BOXSLOT);
    }

    /// <summary>
    /// Fails unless <paramref name="reopened"/> holds both edited Pokémon exactly as planned, with valid checksums and the party count unchanged.
    /// Values are withheld from the messages.
    /// </summary>
    public void AssertReopened(SaveFile reopened)
    {
        if (!reopened.ChecksumsValid || reopened.PartyCount != Native.PartyCount)
        {
            throw new Xunit.Sdk.XunitException("The export reopened with invalid checksums or another party count.");
        }
        var box = SaveFixtures.Slot(reopened, Box).Read(reopened);
        var party = reopened.GetPartySlotAtIndex(Party.Slot);
        if (box.Nickname != Nickname || box.Species != ExpectedBox.Species || !box.Data.SequenceEqual(ExpectedBox.Data))
        {
            throw new Xunit.Sdk.XunitException("The reopened boxed Pokémon differs from the planned edit (values withheld).");
        }
        if (party.CurrentLevel != ExpectedParty.CurrentLevel || !party.Data.SequenceEqual(ExpectedParty.Data))
        {
            throw new Xunit.Sdk.XunitException("The reopened party member differs from the planned edit (values withheld).");
        }
    }

    /// <summary>One level up, or down from the highest level, so the edit always changes the level.</summary>
    public static byte LevelStep(byte level) => level == Experience.MaxLevel ? (byte)(level - 1) : (byte)(level + 1);

    /// <summary>One up, or down from the highest IV, so the edit always changes the IV.</summary>
    public static int IvStep(int iv) => iv == 31 ? 30 : iv + 1;

    /// <summary>
    /// An EV edit that is always accepted: one HP or Attack EV less when there are any, 4 HP EVs when there are none at all, and otherwise a Speed
    /// step within the total. Returns the editor's stat row (HP, Atk, Def, SpA, SpD, Spe), Core's EV index (HP, Atk, Def, Spe, SpA, SpD) and the value.
    /// </summary>
    public static (int Row, int CoreIndex, int Value) EvStep(PKM pk) =>
        pk.EV_HP > 0 ? (0, 0, pk.EV_HP - 1)
        : pk.EV_ATK > 0 ? (1, 1, pk.EV_ATK - 1)
        : pk.EVTotal == 0 ? (0, 0, 4)
        : (5, 3, pk.EV_SPE > 0 ? pk.EV_SPE - 1 : Math.Min(4, EffortValues.Max510 - pk.EVTotal));

    private static string Id(int value) => value.ToString(CultureInfo.InvariantCulture);
}
