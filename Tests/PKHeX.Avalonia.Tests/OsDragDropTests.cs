using System.IO;
using PKHeX.Application.UseCases;
using PKHeX.Core;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Unit tests for the testable parts of OS drag-and-drop (issue #136): entity file-name
/// generation for drag-out, single-file format validation/conversion for drag-in, and
/// batch-placement logic for dropping multiple files onto a box. File-system/DataTransfer
/// plumbing lives in the Avalonia view layer and is not covered here.
/// </summary>
public class OsDragDropTests(ITestOutputHelper output)
{
    // -----------------------------------------------------------------------
    // ExportEntityToFileUseCase (drag OUT — file naming + byte-identical payload)
    // -----------------------------------------------------------------------

    [Fact]
    public void Export_EmptySlot_ReturnsNull()
    {
        var pk = new PK9();
        var result = new ExportEntityToFileUseCase().Execute(pk);

        Assert.Null(result);
        output.WriteLine("Export: blank PKM (Species=0) correctly yields no file ✓");
    }

    [Fact]
    public void Export_OccupiedSlot_UsesUpstreamNamingConvention()
    {
        var sav = new SAV9SV();
        var pk = (PK9)sav.BlankPKM;
        pk.Species = 6; // Charizard
        pk.Nickname = "Charizard";

        var result = new ExportEntityToFileUseCase().Execute(pk);

        Assert.NotNull(result);
        Assert.EndsWith(".pk9", result!.Value.FileName);
        Assert.StartsWith("0006", result.Value.FileName);
        Assert.Contains("Charizard", result.Value.FileName);
        output.WriteLine($"Export: Charizard -> '{result.Value.FileName}' ✓");
    }

    [Fact]
    public void Export_FileContent_RoundTripsToIdenticalEntity()
    {
        var sav = new SAV9SV();
        var pk = (PK9)sav.BlankPKM;
        pk.Species = 25; // Pikachu
        pk.Nickname = "Sparky";
        pk.RefreshChecksum();

        var exported = new ExportEntityToFileUseCase().Execute(pk);
        Assert.NotNull(exported);

        // Re-importing the exported bytes must reconstruct a byte-identical (decrypted) entity —
        // this is the "byte-identical" acceptance criterion for drag-out.
        var reimported = EntityFormat.GetFromBytes(exported!.Value.Data);
        Assert.NotNull(reimported);
        Assert.Equal(pk.Species, reimported!.Species);
        Assert.Equal(pk.Nickname, reimported.Nickname);
        Assert.True(pk.Data[..pk.SIZE_STORED].SequenceEqual(reimported.Data[..reimported.SIZE_STORED]));
        output.WriteLine("Export: round-tripped bytes reconstruct a byte-identical entity ✓");
    }

    // -----------------------------------------------------------------------
    // ImportEntityFileUseCase (drag IN to a single slot — validation/conversion decision)
    // -----------------------------------------------------------------------

    [Fact]
    public void Import_CompatibleEntityFile_ResolvesToEntity()
    {
        var sav = new SAV9SV();
        var pk = (PK9)sav.BlankPKM;
        pk.Species = 1;
        pk.RefreshChecksum();

        var path = WriteTempEntityFile(pk);
        try
        {
            var result = new ImportEntityFileUseCase().Execute(sav, path);

            Assert.Equal(EntityFileDropKind.Entity, result.Kind);
            Assert.NotNull(result.Entity);
            Assert.Equal(1, result.Entity!.Species);
            output.WriteLine("Import: compatible .pk9 resolves to Entity ✓");
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Import_IncompatibleGenerationFile_IsRejectedNotCrashed()
    {
        // A Gen 1 entity dropped onto a Gen 9 save cannot be converted forward across that many
        // generations without an evolutionary/transfer chain, so it should be rejected cleanly.
        var sav = new SAV9SV();
        var pk1 = new PK1 { Species = 1, TID16 = 1 };

        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.pk1");
        File.WriteAllBytes(path, pk1.Data.ToArray());
        try
        {
            var result = new ImportEntityFileUseCase().Execute(sav, path);

            Assert.NotEqual(EntityFileDropKind.Entity, result.Kind);
            Assert.NotNull(result.Message);
            output.WriteLine($"Import: incompatible Gen1 file rejected cleanly ({result.Kind}): {result.Message} ✓");
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Import_UnsupportedFile_ReportsUnsupportedWithoutThrowing()
    {
        var sav = new SAV9SV();
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.txt");
        File.WriteAllText(path, "not a Pokémon file");

        try
        {
            var result = new ImportEntityFileUseCase().Execute(sav, path);

            Assert.Equal(EntityFileDropKind.Unsupported, result.Kind);
            Assert.NotNull(result.Message);
            output.WriteLine("Import: unsupported file type reported cleanly, no exception ✓");
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Import_SaveFile_IsClassifiedAsSaveFileNotEntity()
    {
        var sav = new SAV9SV();
        // Gen 5 (B2/W2) is the blank save type in this Core version whose Write() output is
        // detectable from an in-memory round trip without having been loaded from a real file
        // on disk (some other blank save types omit platform-container bytes that only real
        // dumps carry, which is a Core detection detail unrelated to this feature).
        var openMe = BlankSaveFile.Get(GameVersion.B2);

        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.sav");
        File.WriteAllBytes(path, openMe.Write().ToArray());
        try
        {
            var result = new ImportEntityFileUseCase().Execute(sav, path);

            Assert.Equal(EntityFileDropKind.SaveFile, result.Kind);
            Assert.Null(result.Entity);
            output.WriteLine("Import: dropped save file classified as SaveFile (routes to File>Open path) ✓");
        }
        finally { File.Delete(path); }
    }

    // -----------------------------------------------------------------------
    // BatchImportEntityFilesUseCase (drag IN multiple files onto the grid — placement logic)
    // -----------------------------------------------------------------------

    [Fact]
    public void Batch_PlaceInBox_FillsEmptySlotsInOrder()
    {
        var sav = new SAV9SV();
        // Slot 1 (index 1) is already occupied; everything else in box 0 is empty.
        var occupant = (PK9)sav.BlankPKM;
        occupant.Species = 7;
        sav.SetBoxSlotAtIndex(occupant, 0, 1);

        var candidates = new List<PKM>
        {
            Make(sav, 4),
            Make(sav, 1),
            Make(sav, 25),
        };

        var placed = new BatchImportEntityFilesUseCase().PlaceInBox(sav, 0, candidates);

        Assert.Equal(3, placed);
        Assert.Equal(4, sav.GetBoxSlotAtIndex(0, 0).Species);   // first empty slot
        Assert.Equal(7, sav.GetBoxSlotAtIndex(0, 1).Species);   // untouched occupant
        Assert.Equal(1, sav.GetBoxSlotAtIndex(0, 2).Species);   // next empty slot after skipping slot 1
        Assert.Equal(25, sav.GetBoxSlotAtIndex(0, 3).Species);
        output.WriteLine("Batch: 3 candidates placed into next empty slots in order, occupied slot skipped ✓");
    }

    [Fact]
    public void Batch_PlaceInBox_StopsWhenBoxIsFull()
    {
        var sav = new SAV9SV();
        var boxSlotCount = sav.BoxSlotCount;

        // Fill every slot but the last one.
        for (int i = 0; i < boxSlotCount - 1; i++)
            sav.SetBoxSlotAtIndex(Make(sav, 1), 0, i);

        var candidates = new List<PKM> { Make(sav, 4), Make(sav, 7) }; // only 1 empty slot available

        var placed = new BatchImportEntityFilesUseCase().PlaceInBox(sav, 0, candidates);

        Assert.Equal(1, placed);
        output.WriteLine($"Batch: box with 1 empty slot placed {placed} of 2 candidates, stopped cleanly when full ✓");
    }

    [Fact]
    public void Batch_PlaceInBox_NoCandidates_PlacesNothing()
    {
        var sav = new SAV9SV();
        var placed = new BatchImportEntityFilesUseCase().PlaceInBox(sav, 0, []);

        Assert.Equal(0, placed);
        output.WriteLine("Batch: empty candidate list places 0, no crash ✓");
    }

    [Fact]
    public void Batch_Execute_FromFiles_ReportsPlacedAndSkipped()
    {
        var sav = new SAV9SV();

        var goodPath1 = WriteTempEntityFile(Make(sav, 1));
        var goodPath2 = WriteTempEntityFile(Make(sav, 4));
        var badPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.txt");
        File.WriteAllText(badPath, "junk");

        try
        {
            var result = new BatchImportEntityFilesUseCase().Execute(sav, 0, [goodPath1, goodPath2, badPath]);

            Assert.Equal(2, result.Placed);
            Assert.Equal(1, result.Skipped);
            output.WriteLine($"Batch Execute: placed={result.Placed}, skipped={result.Skipped} (1 unsupported file) ✓");
        }
        finally
        {
            File.Delete(goodPath1);
            File.Delete(goodPath2);
            File.Delete(badPath);
        }
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private static PK9 Make(SaveFile sav, ushort species)
    {
        var pk = (PK9)sav.BlankPKM;
        pk.Species = species;
        pk.RefreshChecksum();
        return pk;
    }

    private static string WriteTempEntityFile(PKM pk)
    {
        var data = new byte[pk.SIZE_STORED];
        pk.WriteDecryptedDataStored(data);
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.{pk.Extension}");
        File.WriteAllBytes(path, data);
        return path;
    }
}
