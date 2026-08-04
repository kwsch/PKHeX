using CommunityToolkit.Mvvm.Input;
using Moq;
using PKHeX.Avalonia.Services;
using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Behavioral tests for BatchEditorViewModel.
/// The batch editor parses text instructions and applies them to all non-empty
/// PKM in boxes/party. RunBatchAsync is async; tests await IAsyncRelayCommand.
/// </summary>
public class BatchEditorTests(ITestOutputHelper output)
{
    private static Mock<IDialogService> DialogMock() => new();

    // -----------------------------------------------------------------------
    // 1. PropertySuggestions is populated with PKM properties
    // -----------------------------------------------------------------------

    [Fact]
    public void BatchEditor_PropertySuggestions_Populated()
    {
        var sav = new SAV6XY();
        var vm = new BatchEditorViewModel(sav, DialogMock().Object);

        Assert.NotEmpty(vm.PropertySuggestions);
        Assert.Contains("Species",      vm.PropertySuggestions);
        Assert.Contains("Nickname",     vm.PropertySuggestions);
        Assert.Contains("CurrentLevel", vm.PropertySuggestions);
        output.WriteLine($"PropertySuggestions: {vm.PropertySuggestions.Count} entries ✓");
    }

    // -----------------------------------------------------------------------
    // 2. Priority properties appear first in PropertySuggestions
    // -----------------------------------------------------------------------

    [Fact]
    public void BatchEditor_PropertySuggestions_PriorityFirst()
    {
        var sav = new SAV6XY();
        var vm = new BatchEditorViewModel(sav, DialogMock().Object);

        // "Species" is first in the priority list
        Assert.Equal("Species", vm.PropertySuggestions[0]);
        output.WriteLine("PropertySuggestions: Species is first ✓");
    }

    // -----------------------------------------------------------------------
    // 3. AddInstruction builds correct instruction string
    // -----------------------------------------------------------------------

    [Fact]
    public void BatchEditor_AddInstruction_BuildsCorrectString()
    {
        var sav = new SAV6XY();
        var vm = new BatchEditorViewModel(sav, DialogMock().Object);

        vm.SelectedProperty = "CurrentLevel";
        vm.SelectedOperator = "=";
        vm.SelectedValue    = "50";

        vm.AddInstructionCommand.Execute(null);

        Assert.Equal(".CurrentLevel=50", vm.Instructions);
        output.WriteLine($"AddInstruction: '{vm.Instructions}' ✓");
    }

    // -----------------------------------------------------------------------
    // 4. Multiple AddInstruction calls join with newlines
    // -----------------------------------------------------------------------

    [Fact]
    public void BatchEditor_AddInstruction_MultipleLines_JoinedWithNewline()
    {
        var sav = new SAV6XY();
        var vm = new BatchEditorViewModel(sav, DialogMock().Object);

        vm.SelectedProperty = "CurrentLevel";
        vm.SelectedOperator = "=";
        vm.SelectedValue    = "50";
        vm.AddInstructionCommand.Execute(null);

        vm.SelectedProperty = "IsShiny";
        vm.SelectedOperator = "=";
        vm.SelectedValue    = "true";
        vm.AddInstructionCommand.Execute(null);

        var lines = vm.Instructions.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(2, lines.Length);
        Assert.Equal(".CurrentLevel=50", lines[0]);
        Assert.Equal(".IsShiny=true", lines[1]);
        output.WriteLine($"AddInstruction x2: '{vm.Instructions.Replace(Environment.NewLine, "|")}' ✓");
    }

    // -----------------------------------------------------------------------
    // 5. AddFilter builds correct filter string (= prefix)
    // -----------------------------------------------------------------------

    [Fact]
    public void BatchEditor_AddFilter_BuildsCorrectString()
    {
        var sav = new SAV6XY();
        var vm = new BatchEditorViewModel(sav, DialogMock().Object);

        vm.SelectedProperty = "Species";
        vm.SelectedOperator = "=";
        vm.SelectedValue    = "25";

        vm.AddFilterCommand.Execute(null);

        Assert.Equal("=Species=25", vm.Instructions);
        output.WriteLine($"AddFilter: '{vm.Instructions}' ✓");
    }

    // -----------------------------------------------------------------------
    // 6. ClearInstructions empties both Instructions and Results
    // -----------------------------------------------------------------------

    [Fact]
    public void BatchEditor_ClearInstructions_ClearsAll()
    {
        var sav = new SAV6XY();
        var vm = new BatchEditorViewModel(sav, DialogMock().Object);

        vm.Instructions = ".Level=50";
        vm.Results = "some results";

        vm.ClearInstructionsCommand.Execute(null);

        Assert.Empty(vm.Instructions);
        Assert.Empty(vm.Results);
        output.WriteLine("ClearInstructions: Instructions and Results cleared ✓");
    }

    // -----------------------------------------------------------------------
    // 7. RunBatch with empty instructions sets Results message
    // -----------------------------------------------------------------------

    [Fact]
    public async Task BatchEditor_RunBatch_EmptyInstructions_SetsResultsMessage()
    {
        var sav = new SAV6XY();
        var vm = new BatchEditorViewModel(sav, DialogMock().Object);

        vm.Instructions = string.Empty;
        await ((IAsyncRelayCommand)vm.RunBatchCommand).ExecuteAsync(null);

        Assert.False(string.IsNullOrEmpty(vm.Results));
        output.WriteLine($"RunBatch(empty): Results='{vm.Results}' ✓");
    }

    // -----------------------------------------------------------------------
    // 8. RunBatch processes PKM in boxes (Gen6)
    // -----------------------------------------------------------------------

    [Fact]
    public async Task BatchEditor_RunBatch_ModifiesBoxPokemon()
    {
        var sav = new SAV6XY();

        // Inject a PK6 into box 0
        var pk = new PK6 { Species = 1 }; pk.CurrentLevel = 5;
        sav.SetBoxSlotAtIndex(pk, 0);

        var vm = new BatchEditorViewModel(sav, DialogMock().Object);
        vm.EditBoxes = true;
        vm.EditParty = false;
        vm.Instructions = ".CurrentLevel=50";

        await ((IAsyncRelayCommand)vm.RunBatchCommand).ExecuteAsync(null);

        var resultPk = sav.GetBoxSlotAtIndex(0, 0);
        Assert.Equal(50, resultPk.CurrentLevel);
        Assert.False(string.IsNullOrEmpty(vm.Results));
        output.WriteLine($"RunBatch: Bulbasaur Level 5→50 ✓ Results='{vm.Results}'");
    }

    // -----------------------------------------------------------------------
    // 9. BatchEditCompleted event fires after successful batch
    // -----------------------------------------------------------------------

    [Fact]
    public async Task BatchEditor_RunBatch_FiresBatchEditCompleted()
    {
        var sav = new SAV6XY();
        var pk = new PK6 { Species = 25 }; pk.CurrentLevel = 5;
        sav.SetBoxSlotAtIndex(pk, 0);

        var vm = new BatchEditorViewModel(sav, DialogMock().Object);
        vm.Instructions = ".Nickname=Pika";

        bool eventFired = false;
        vm.BatchEditCompleted += () => eventFired = true;

        await ((IAsyncRelayCommand)vm.RunBatchCommand).ExecuteAsync(null);

        Assert.True(eventFired, "BatchEditCompleted should fire after successful batch");
        output.WriteLine("BatchEditCompleted event fired ✓");
    }

    // -----------------------------------------------------------------------
    // 10. Operators list contains expected operators
    // -----------------------------------------------------------------------

    [Fact]
    public void BatchEditor_Operators_ContainsExpected()
    {
        var sav = new SAV6XY();
        var vm = new BatchEditorViewModel(sav, DialogMock().Object);

        Assert.Contains("=", vm.Operators);
        Assert.Contains("!", vm.Operators);
        output.WriteLine($"Operators: [{string.Join(", ", vm.Operators)}] ✓");
    }
}
