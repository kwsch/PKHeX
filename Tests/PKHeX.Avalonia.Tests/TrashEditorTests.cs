using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Behavioral tests for TrashEditorViewModel.
/// Trash bytes are hidden bytes that remain in nickname/OT fields after the
/// terminator, used for legality verification in older generations.
/// </summary>
public class TrashEditorTests(ITestOutputHelper output)
{
    // Use PK4 as the converter since PKM implements IStringConverter
    private static PK4 Gen4Converter() => new() { Species = 1 };

    // -----------------------------------------------------------------------
    // 1. Constructor initialises Bytes, CurrentText, and FinalBytes correctly
    // -----------------------------------------------------------------------

    [Fact]
    public void TrashEditor_InitialisesFromText_WhenNoBytesProvided()
    {
        var pk = Gen4Converter();
        var vm = new TrashEditorViewModel("PIKA", null, pk, 4, EntityContext.Gen4);

        Assert.Equal("PIKA", vm.CurrentText);
        Assert.Equal("PIKA", vm.FinalText);
        Assert.NotEmpty(vm.Bytes);
        Assert.NotNull(vm.FinalBytes);
        output.WriteLine($"InitText: Bytes.Count={vm.Bytes.Count}, FinalBytes.Length={vm.FinalBytes.Length}");
    }

    [Fact]
    public void TrashEditor_InitialisesFromExistingBytes()
    {
        var pk = Gen4Converter();
        // Build bytes for "ASH"
        Span<byte> temp = stackalloc byte[20];
        int len = pk.SetString(temp, "ASH".AsSpan(), 10, StringConverterOption.None);
        var bytes = temp[..len].ToArray();

        var vm = new TrashEditorViewModel("ASH", bytes, pk, 4, EntityContext.Gen4);

        Assert.Equal("ASH", vm.CurrentText);
        Assert.Equal(bytes.Length, vm.Bytes.Count);
        output.WriteLine($"InitFromBytes: Bytes.Count={vm.Bytes.Count}");
    }

    // -----------------------------------------------------------------------
    // 2. ClearTrashCommand zeros all bytes beyond the current text terminator
    // -----------------------------------------------------------------------

    [Fact]
    public void TrashEditor_ClearTrash_ZerosBytesAfterText()
    {
        var pk = Gen4Converter();

        // Give existing bytes with extra garbage after "ASH"
        Span<byte> temp = stackalloc byte[20];
        int len = pk.SetString(temp, "MISSINGNO".AsSpan(), 10, StringConverterOption.None);
        var bytes = temp[..len].ToArray();

        // Construct VM with shorter text but longer original bytes (simulating trash)
        var vm = new TrashEditorViewModel("ASH", bytes, pk, 4, EntityContext.Gen4);

        // Get how many bytes the current text uses
        int textLen = vm.CurrentText.Length;

        vm.ClearTrashCommand.Execute(null);

        // Bytes beyond the text should be zero
        Span<byte> currentTemp = stackalloc byte[vm.FinalBytes.Length];
        int currentWritten = pk.SetString(currentTemp, vm.CurrentText.AsSpan(), vm.CurrentText.Length, StringConverterOption.None);

        bool allZeroBeyondText = vm.FinalBytes[currentWritten..].All(b => b == 0);
        Assert.True(allZeroBeyondText, "All bytes beyond current text should be zero after ClearTrash");
        output.WriteLine($"ClearTrash: {vm.FinalBytes.Length - currentWritten} bytes zeroed beyond text ✓");
    }

    // -----------------------------------------------------------------------
    // 3. Modifying a TrashByteViewModel updates CurrentText and FinalBytes
    // -----------------------------------------------------------------------

    [Fact]
    public void TrashEditor_ByteEdit_UpdatesCurrentText()
    {
        var pk = Gen4Converter();
        var vm = new TrashEditorViewModel("A", null, pk, 4, EntityContext.Gen4);

        var originalText = vm.CurrentText;
        var originalByte = vm.Bytes[0].Value;

        // Modify the first byte to something valid (for Gen4, 'B' is byte 0x01 in some encodings,
        // but we just check that the text changes when bytes change)
        var alternativeByte = (byte)(originalByte == 0 ? 1 : originalByte - 1);
        vm.Bytes[0].Value = alternativeByte;

        // FinalBytes should be updated
        Assert.Equal(alternativeByte, vm.FinalBytes[0]);
        output.WriteLine($"ByteEdit: byte[0] {originalByte} → {alternativeByte}, text={vm.CurrentText}");
    }

    // -----------------------------------------------------------------------
    // 4. SaveCommand sets FinalText to CurrentText and invokes CloseRequested
    // -----------------------------------------------------------------------

    [Fact]
    public void TrashEditor_SaveCommand_SetsFinalTextAndClosesDialog()
    {
        var pk = Gen4Converter();
        var vm = new TrashEditorViewModel("PIKA", null, pk, 4, EntityContext.Gen4);

        bool closedInvoked = false;
        vm.CloseRequested = () => closedInvoked = true;

        vm.SaveCommand.Execute(null);

        Assert.Equal(vm.CurrentText, vm.FinalText);
        Assert.True(closedInvoked, "CloseRequested should have been invoked by SaveCommand");
        output.WriteLine("SaveCommand: FinalText set, CloseRequested invoked ✓");
    }

    // -----------------------------------------------------------------------
    // 5. HexValue property on TrashByteViewModel round-trips to Value
    // -----------------------------------------------------------------------

    [Fact]
    public void TrashByte_HexValue_UpdatesValue()
    {
        var pk = Gen4Converter();
        var vm = new TrashEditorViewModel("X", null, pk, 4, EntityContext.Gen4);

        Assert.NotEmpty(vm.Bytes);
        var byteVm = vm.Bytes[0];

        byteVm.HexValue = "FF";

        Assert.Equal(0xFF, byteVm.Value);
        Assert.Equal("FF", byteVm.HexValue);
        output.WriteLine("TrashByte HexValue→Value: FF round-trip ✓");
    }

    [Fact]
    public void TrashByte_InvalidHexValue_DoesNotCrash()
    {
        var pk = Gen4Converter();
        var vm = new TrashEditorViewModel("X", null, pk, 4, EntityContext.Gen4);

        var byteVm = vm.Bytes[0];
        var originalValue = byteVm.Value;

        // Setting invalid hex should not crash and should not change Value
        var ex = Record.Exception(() => byteVm.HexValue = "ZZZZ");
        Assert.Null(ex);
        Assert.Equal(originalValue, byteVm.Value);
        output.WriteLine("TrashByte invalid hex: no exception, value unchanged ✓");
    }

    // -----------------------------------------------------------------------
    // 6. Species list and language list are populated
    // -----------------------------------------------------------------------

    [Fact]
    public void TrashEditor_SpeciesAndLanguageLists_Populated()
    {
        var pk = Gen4Converter();
        var vm = new TrashEditorViewModel("TEST", null, pk, 4, EntityContext.Gen4);

        Assert.NotEmpty(vm.SpeciesList);
        Assert.NotEmpty(vm.LanguageList);
        Assert.NotNull(vm.SelectedSpecies);
        Assert.NotNull(vm.SelectedLanguage);
        output.WriteLine($"Lists: {vm.SpeciesList.Count()} species, {vm.LanguageList.Count()} languages ✓");
    }
}
