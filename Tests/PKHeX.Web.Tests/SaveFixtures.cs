using System.Buffers.Binary;
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

    /// <summary>A blank XY or ORAS save holding one known PK6 in box 1, slot 1.</summary>
    /// <param name="oras">True for ORAS, false for XY.</param>
    /// <param name="legal">Whether the stored PK6 is a known legal or a known illegal entity.</param>
    /// <param name="customize">Applied to the save before it is written, e.g. to set trainer values.</param>
    public static byte[] Synthetic(bool oras, bool legal = true, Action<SaveFile>? customize = null)
    {
        var save = BlankSaveFile.Get(oras ? GameVersion.OR : GameVersion.X);
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
    public static Action<SaveFile> WithPartyMember(string nickname = "PartyMon") => save =>
    {
        var entity = new PK6(ReadEntity(true)) { Nickname = nickname, IsNicknamed = true };
        save.SetPartySlotAtIndex(entity, 0, EntityImportSettings.None);
    };

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
