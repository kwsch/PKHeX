using System.Buffers.Binary;
using Microsoft.Extensions.Time.Testing;
using PKHeX.Core;
using PKHeX.Web.Services;
using PKHeX.Web.State;

namespace PKHeX.Web.Tests;

/// <summary>
/// Synthetic saves for the <see cref="TestCategory.Unit"/> and <see cref="TestCategory.E2E"/> tiers, plus shared Core helpers.
/// </summary>
internal static class SaveFixtures
{
    public static string RepositoryRoot
    {
        get
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory is not null && !Directory.Exists(Path.Combine(directory.FullName, "PKHeX.Core")))
            {
                directory = directory.Parent;
            }
            return directory?.FullName ?? throw new InvalidOperationException("Cannot find the repository root.");
        }
    }

    public static byte[] ReadEntity(bool legal)
    {
        var path = legal
            ? "Legality/Legal/Generation 6/263 - Zigzagoon - 2746E8288E7E.pk6"
            : "Legality/Illegal/Wild/132 - Ditto - 5CD1D273663D.pk6";
        return File.ReadAllBytes(Path.Combine(RepositoryRoot, "Tests/PKHeX.Core.Tests", path));
    }

    /// <summary>
    /// Every PK6 in Core's legality test fixtures (legal and illegal), ordered by path: the corpus the legality tests and timings run over.
    /// </summary>
    public static IReadOnlyList<(string Name, byte[] Data)> LegalityCorpus()
    {
        var root = Path.Combine(RepositoryRoot, "Tests/PKHeX.Core.Tests/Legality");
        return [.. Directory.EnumerateFiles(root, "*.pk6", SearchOption.AllDirectories)
            .Order(StringComparer.Ordinal)
            .Select(path => (Path.GetRelativePath(root, path), File.ReadAllBytes(path)))];
    }

    /// <summary>
    /// Customisation for <see cref="Synthetic"/>: stores <paramref name="entities"/> in box 1 from slot 2 on (zero-based slot 1), leaving slot 1
    /// to the known entity <see cref="Synthetic"/> writes.
    /// </summary>
    public static Action<SaveFile> WithBoxEntities(IEnumerable<byte[]> entities) => save =>
    {
        var slot = 1;
        foreach (var data in entities)
        {
            if (slot >= save.BoxSlotCount)
            {
                throw new InvalidOperationException($"Only {save.BoxSlotCount - 1} entities fit after box 1, slot 1; split them across saves.");
            }
            save.SetBoxSlotAtIndex(new PK6(data.ToArray()), 0, slot++, EntityImportSettings.None);
        }
    };

    /// <summary>A blank XY or ORAS save holding one known PK6 in box 1, slot 1.</summary>
    /// <remarks>
    /// Legality is analysed with the save as Core's active trainer, as the desktop editor does, and Core then checks whether the save's trainer
    /// is the entity's original trainer. So the save belongs to the known legal entity's original trainer (ID, name and gender), and the ORAS
    /// save is Alpha Sapphire, that entity's game: there it is its trainer's own Pokémon and is legal. No X/Y save can be its original trainer's
    /// (Core matches the exact game), so in an XY save it is held by its original trainer without having been traded, and Core finds it invalid,
    /// as the desktop does.
    /// </remarks>
    /// <param name="oras">True for ORAS (Alpha Sapphire), false for XY (X).</param>
    /// <param name="legal">Whether the stored PK6 is the known legal or the known illegal entity.</param>
    /// <param name="customize">Applied to the save before it is written, e.g. to set trainer values.</param>
    public static byte[] Synthetic(bool oras, bool legal = true, Action<SaveFile>? customize = null)
    {
        var save = BlankSaveFile.Get(oras ? GameVersion.AS : GameVersion.X);
        var owner = new PK6(ReadEntity(true));
        save.ID32 = owner.ID32;
        save.OT = owner.OriginalTrainerName;
        save.Gender = owner.OriginalTrainerGender;
        customize?.Invoke(save);
        // Test-only synthetic container marker, matching SaveUtil.HasSaveFooterBEEF.
        // This is not a gameplay-ready save and is never used instead of a real fixture.
        BinaryPrimitives.WriteUInt32LittleEndian(save.Data[^0x1F0..], 0x42454546);
        var entity = new PK6(ReadEntity(legal));
        save.SetBoxSlotAtIndex(entity, 0, EntityImportSettings.None);
        return save.Write().ToArray();
    }

    /// <summary>
    /// Customisation for <see cref="Synthetic"/>: puts the known legal PK6 in party position 1, nicknamed <paramref name="nickname"/>,
    /// so the party and box 1, slot 1 hold told-apart copies of the same entity.
    /// </summary>
    /// <param name="nickname">The party member's nickname.</param>
    /// <param name="battle">
    /// Changes the member after its party stats are calculated, e.g. to injure it, faint it or give it a status. Core keeps stats that are
    /// present when it writes a party member, so these survive into the save.
    /// </param>
    /// <param name="position">The party position (zero-based); earlier positions must already be filled.</param>
    public static Action<SaveFile> WithPartyMember(string nickname = "PartyMon", Action<PK6>? battle = null, int position = 0) => save =>
    {
        var entity = new PK6(ReadEntity(true)) { Nickname = nickname, IsNicknamed = true };
        entity.ResetPartyStats();
        battle?.Invoke(entity);
        save.SetPartySlotAtIndex(entity, position, EntityImportSettings.None);
    };

    /// <summary>Applies each customisation in turn, e.g. to fill several party positions.</summary>
    public static Action<SaveFile> All(params Action<SaveFile>[] customizations) => save =>
    {
        foreach (var customize in customizations)
        {
            customize(save);
        }
    };

    /// <summary>
    /// Opens <paramref name="bytes"/> as a session whose family does not write party members, as a family without a party-stat policy would be.
    /// </summary>
    public static SaveSession OpenWithoutPartyWrites(byte[] bytes)
    {
        var save = Parse(bytes);
        var family = SupportMatrix.Find(save)! with { WritesParty = false };
        return new SaveSession(bytes.ToArray(), save, "fixture.sav", SaveCapabilities.For(save, family));
    }

    /// <summary>
    /// Customisation for <see cref="Synthetic"/>: stores a copy of the known legal PK6, changed by <paramref name="change"/>, in box
    /// <paramref name="box"/>, slot <paramref name="slot"/> (both zero-based), e.g. to vary species, form, shininess or egg state.
    /// </summary>
    public static Action<SaveFile> WithBoxEntity(int box, int slot, Action<PK6> change) => save =>
    {
        var entity = new PK6(ReadEntity(true));
        change(entity);
        entity.RefreshChecksum();
        save.SetBoxSlotAtIndex(entity, box, slot, EntityImportSettings.None);
    };

    /// <summary>Opens <paramref name="bytes"/> through <see cref="Services.SaveLoader"/>, failing the test with the outcome if no session was created.</summary>
    public static SaveSession Open(byte[] bytes, string? fileName = null)
    {
        var outcome = SaveLoader.Load(bytes, fileName);
        return outcome.Session ?? throw new InvalidOperationException($"Fixture did not open: {outcome.Failure} {outcome.Integrity}.");
    }

    public static SaveFile Parse(byte[] bytes) => SaveUtil.GetSaveFile(bytes.ToArray())
        ?? throw new InvalidOperationException("Fixture recognition failed.");

    /// <summary>
    /// A workspace state on a clock that never moves on its own, so no legality analysis runs unless a test advances the clock or asks for one.
    /// </summary>
    public static WorkspaceState NewState() => new(new FakeTimeProvider());

    /// <summary>Box 1, slot 1, where <see cref="Synthetic"/> stores its entity.</summary>
    public static readonly SlotRef FirstBoxSlot = SlotRef.InBox(0, 0);

    public static SlotInfoBox Slot(SaveFile save, SlotRef slot) => new(slot.Box, slot.Slot, save);

    /// <summary>First occupied, checksum-valid, writable boxed PK6 in box/slot order.</summary>
    public static SlotRef WritableSlot(SaveFile save)
    {
        var result = Enumerable.Range(0, save.SlotCount).FirstOrDefault(i =>
        {
            var slot = Slot(save, SlotRef.InBox(i / save.BoxSlotCount, i % save.BoxSlotCount));
            var pk = slot.Read(save);
            return pk is PK6 && pk.Species != 0 && pk.ChecksumValid && slot.CanWriteTo(save) && slot.CanWriteTo(save, pk) == WriteBlockedMessage.None;
        }, -1);
        return result >= 0
            ? SlotRef.InBox(result / save.BoxSlotCount, result % save.BoxSlotCount)
            : throw new InvalidOperationException("Real fixture contains no occupied, checksum-valid, writable boxed PK6.");
    }
}
