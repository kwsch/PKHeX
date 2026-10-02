using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Failure injection into the apply transaction (WEB-SESSION-002, WEB-TEST-003): whatever goes wrong in or after the staged write, the
/// session is left exactly as it was, and the draft can still be applied once the fault is gone.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class ApplyTransactionTests
{
    /// <summary>Core's own write, as the session does it.</summary>
    private static bool CoreWrite(SaveFile save, ISlotInfo slot, PKM entity) => slot.WriteTo(save, entity, EntityImportSettings.None);

    /// <summary>
    /// A save with two injured, burned party members and two boxed entities, so every kind of untargeted position exists and a heal would show.
    /// </summary>
    private static byte[] Populated() => SaveFixtures.Synthetic(true, customize: SaveFixtures.All(
        SaveFixtures.WithPartyMember("First", Injure),
        SaveFixtures.WithPartyMember("Second", Injure, position: 1),
        SaveFixtures.WithBoxEntity(0, 1, p => p.Nickname = "Neighbour")));

    private static void Injure(PK6 pk)
    {
        pk.Stat_HPCurrent = 7;
        pk.Status_Condition = 0x10;
    }

    private static readonly SlotRef[] Targets = [SlotRef.InParty(0), SlotRef.InParty(1), SaveFixtures.FirstBoxSlot];

    /// <summary>Faults injected into the staged write, each with the refusal it must cause.</summary>
    private static readonly (string Name, SessionError Error)[] Faults =
    [
        (nameof(WriteReportsFailure), SessionError.StagedWriteFailed),
        (nameof(WriteChangesTheEntity), SessionError.StagedEditMismatch),
        (nameof(WriteSkipsTheSlot), SessionError.StagedEditMismatch),
        (nameof(WriteHealsAPartyMember), SessionError.StagedEditMismatch),
        (nameof(WriteAlsoChangesABoxSlot), SessionError.UntargetedSlotChanged),
        (nameof(WriteAlsoChangesAPartyMember), SessionError.UntargetedSlotChanged),
        (nameof(WriteAlsoChangesAnEmptyPartyPosition), SessionError.UntargetedSlotChanged),
        (nameof(WriteCorruptsANeighbour), SessionError.UntargetedSlotChanged),
        (nameof(WriteAddsAPartyMember), SessionError.PartyCountChanged),
    ];

    private static Func<SaveFile, ISlotInfo, PKM, bool> Fault(string name, SlotRef target) => name switch
    {
        nameof(WriteReportsFailure) => WriteReportsFailure,
        nameof(WriteChangesTheEntity) => WriteChangesTheEntity,
        nameof(WriteSkipsTheSlot) => WriteSkipsTheSlot,
        nameof(WriteHealsAPartyMember) => target.IsParty ? WriteHealsAPartyMember : WriteChangesTheEntity,
        nameof(WriteAlsoChangesABoxSlot) => WriteAlsoChangesABoxSlot,
        nameof(WriteAlsoChangesAPartyMember) => target == SlotRef.InParty(1) ? WriteAlsoChangesFirstPartyMember : WriteAlsoChangesAPartyMember,
        nameof(WriteAlsoChangesAnEmptyPartyPosition) => WriteAlsoChangesAnEmptyPartyPosition,
        nameof(WriteCorruptsANeighbour) => WriteCorruptsANeighbour,
        nameof(WriteAddsAPartyMember) => WriteAddsAPartyMember,
        _ => throw new ArgumentOutOfRangeException(nameof(name)),
    };

    private static bool WriteReportsFailure(SaveFile save, ISlotInfo slot, PKM entity) => false;

    private static bool WriteChangesTheEntity(SaveFile save, ISlotInfo slot, PKM entity)
    {
        entity.HeldItem = 1;
        return CoreWrite(save, slot, entity);
    }

    private static bool WriteSkipsTheSlot(SaveFile save, ISlotInfo slot, PKM entity) => true;

    /// <summary>A write that heals, as Core's <see cref="PKM.Heal"/> would: the stored format matches, the party bytes do not.</summary>
    private static bool WriteHealsAPartyMember(SaveFile save, ISlotInfo slot, PKM entity)
    {
        entity.Heal();
        return CoreWrite(save, slot, entity);
    }

    private static bool WriteAlsoChangesABoxSlot(SaveFile save, ISlotInfo slot, PKM entity)
    {
        var neighbour = save.GetBoxSlotAtIndex(0, 1);
        neighbour.Nickname = "Overwritten";
        save.SetBoxSlotAtIndex(neighbour, 0, 1, EntityImportSettings.None);
        return CoreWrite(save, slot, entity);
    }

    private static bool WriteAlsoChangesAPartyMember(SaveFile save, ISlotInfo slot, PKM entity) => ChangePartyMember(save, 1) && CoreWrite(save, slot, entity);

    private static bool WriteAlsoChangesFirstPartyMember(SaveFile save, ISlotInfo slot, PKM entity) => ChangePartyMember(save, 0) && CoreWrite(save, slot, entity);

    private static bool ChangePartyMember(SaveFile save, int position)
    {
        var member = save.GetPartySlotAtIndex(position);
        member.Stat_HPCurrent = 1;
        save.SetPartySlotAtIndex(member, position, EntityImportSettings.None);
        return true;
    }

    /// <summary>Writes into the sixth party position, past the party count, without changing the count: empty in game, but part of the file.</summary>
    private static bool WriteAlsoChangesAnEmptyPartyPosition(SaveFile save, ISlotInfo slot, PKM entity)
    {
        var stray = save.GetPartySlotAtIndex(0);
        stray.WriteEncryptedDataParty(save.Data[save.GetPartyOffset(5)..]);
        return CoreWrite(save, slot, entity);
    }

    private static bool WriteCorruptsANeighbour(SaveFile save, ISlotInfo slot, PKM entity)
    {
        save.Data[save.GetBoxSlotOffset(0, 1) + 0x10] ^= 0xFF;
        return CoreWrite(save, slot, entity);
    }

    private static bool WriteAddsAPartyMember(SaveFile save, ISlotInfo slot, PKM entity)
    {
        save.SetPartySlotAtIndex(save.GetPartySlotAtIndex(0), 2, EntityImportSettings.None);
        return CoreWrite(save, slot, entity);
    }

    [Theory]
    [MemberData(nameof(FaultsByTarget))]
    public void AFaultyStagedWriteLeavesTheSessionUnchanged(string fault, SessionError expected, SlotRef target)
    {
        var source = Populated();
        var session = SaveFixtures.Open(source);
        var working = session.Working;
        var workingBytes = working.Data.ToArray();
        var draft = session.Select(target);
        draft.EditNickname("Changed", true);
        session.StagedWriter = Fault(fault, target);

        var apply = () => session.Apply(draft);

        apply.Should().Throw<SessionException>().Which.Error.Should().Be(expected);
        session.Working.Should().BeSameAs(working);
        session.Working.Data.ToArray().Should().Equal(workingBytes);
        session.Revision.Should().Be(0);
        session.HasChangesSinceOpen.Should().BeFalse();
        session.GetOriginalBytes().Should().Equal(source);
        draft.IsDirty.Should().BeTrue("a refused apply keeps the draft");

        // With the fault gone, the same draft applies.
        session.StagedWriter = CoreWrite;
        session.Apply(draft);
        session.Revision.Should().Be(1);
        session.Select(target).Nickname.Should().Be("Changed");
    }

    public static TheoryData<string, SessionError, SlotRef> FaultsByTarget()
    {
        var data = new TheoryData<string, SessionError, SlotRef>();
        foreach (var (name, error) in Faults)
        {
            foreach (var target in Targets)
            {
                data.Add(name, error, target);
            }
        }
        return data;
    }

    [Fact]
    public void AThrowingStagedWriteLeavesTheSessionUnchanged()
    {
        var session = SaveFixtures.Open(Populated());
        var working = session.Working;
        var workingBytes = working.Data.ToArray();
        var draft = session.Select(SlotRef.InParty(0));
        draft.EditNickname("Changed", true);
        session.StagedWriter = (save, slot, entity) =>
        {
            CoreWrite(save, slot, entity);
            throw new InvalidOperationException("Injected after the write.");
        };

        var apply = () => session.Apply(draft);

        apply.Should().Throw<InvalidOperationException>().WithMessage("Injected after the write.");
        session.Working.Should().BeSameAs(working);
        session.Working.Data.ToArray().Should().Equal(workingBytes);
        session.Revision.Should().Be(0);
    }

    [Fact]
    public void TheWriterCannotChangeWhatTheReadBackIsComparedWith()
    {
        var session = SaveFixtures.Open(Populated());
        var draft = session.Select(SaveFixtures.FirstBoxSlot);
        draft.EditNickname("Changed", true);
        // Changes the entity it is handed and writes it: if that were the session's own copy, the read-back would agree with it.
        session.StagedWriter = WriteChangesTheEntity;

        var apply = () => session.Apply(draft);

        apply.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.StagedEditMismatch);
        draft.Preview().HeldItem.Should().NotBe(1);
    }

    [Fact]
    public void TheDefaultWriterUsesNoImportSettings()
    {
        var source = Populated();
        var session = SaveFixtures.Open(source);
        var draft = session.Select(SlotRef.InParty(0));
        draft.EditNickname("Changed", true);

        session.Apply(draft);

        // Core's default import settings would mark the Pokédex, bump records and rewrite handler data; the whole file must match a
        // native write with EntityImportSettings.None.
        var native = SaveFixtures.Parse(source);
        var pk = native.GetPartySlotAtIndex(0);
        pk.Nickname = "Changed";
        pk.IsNicknamed = true;
        native.SetPartySlotAtIndex(pk, 0, EntityImportSettings.None);
        SaveExporter.Export(session, null).Should().Equal(native.Write().ToArray());
    }
}
