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

    public static byte[] Synthetic(bool oras, bool legal = true)
    {
        var save = BlankSaveFile.Get(oras ? GameVersion.OR : GameVersion.X);
        // Test-only synthetic container marker, matching SaveUtil.HasSaveFooterBEEF.
        // This is not a gameplay-ready save and is never used instead of a real fixture.
        BinaryPrimitives.WriteUInt32LittleEndian(save.Data[^0x1F0..], 0x42454546);
        var entity = new PK6(ReadEntity(legal));
        save.SetBoxSlotAtIndex(entity, 0, EntityImportSettings.None);
        return save.Write().ToArray();
    }

    /// <summary>Opens <paramref name="bytes"/> through <see cref="Services.SaveLoader"/>, failing the test with the outcome if no session was created.</summary>
    public static SaveSession Open(byte[] bytes, string? fileName = null)
    {
        var outcome = SaveLoader.Load(bytes, fileName);
        return outcome.Session ?? throw new InvalidOperationException($"Fixture did not open: {outcome.Failure} {outcome.Integrity}.");
    }

    public static SaveFile Parse(byte[] bytes) => SaveUtil.GetSaveFile(bytes.ToArray())
        ?? throw new InvalidOperationException("Fixture recognition failed.");

    public static SlotInfoBox Slot(SaveFile save, int index) => new(index / save.BoxSlotCount, index % save.BoxSlotCount, save);

    /// <summary>First occupied, checksum-valid, writable boxed PK6 in box/slot order.</summary>
    public static int WritableSlot(SaveFile save)
    {
        var result = Enumerable.Range(0, save.SlotCount).FirstOrDefault(i =>
        {
            var slot = Slot(save, i);
            var pk = slot.Read(save);
            return pk is PK6 && pk.Species != 0 && pk.ChecksumValid && slot.CanWriteTo(save) && slot.CanWriteTo(save, pk) == WriteBlockedMessage.None;
        }, -1);
        return result >= 0 ? result : throw new InvalidOperationException("Real fixture contains no occupied, checksum-valid, writable boxed PK6.");
    }
}
