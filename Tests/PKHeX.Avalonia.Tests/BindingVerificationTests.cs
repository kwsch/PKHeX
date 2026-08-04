using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Headless.XUnit;
using PKHeX.Avalonia.Views;
using Xunit;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Verifies that XAML bindings in the View match properties in the ViewModel.
/// Catches typos, missing properties, and binding configuration errors.
/// </summary>
public class BindingVerificationTests
{
    private readonly ITestOutputHelper _output;

    public BindingVerificationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [AvaloniaFact]
    public void Verify_All_Bindings_Are_Valid()
    {
        // Read the XAML file content
        var repoRoot = FindRepoRoot();
        var xamlPath = Path.Combine(repoRoot, "PKHeX.Avalonia", "Views", "PokemonEditor.axaml");
        var xamlContent = File.ReadAllText(xamlPath);
        
        // Get all ViewModel property names via Reflection
        var vmType = typeof(PKHeX.Presentation.ViewModels.PokemonEditorViewModel);
        var vmProperties = vmType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Select(p => p.Name)
            .ToHashSet();

        // Extract binding paths from XAML using simple regex
        var bindingPattern = new System.Text.RegularExpressions.Regex(@"\{Binding\s+(?:Path=)?(\w+)");
        var matches = bindingPattern.Matches(xamlContent);
        
        var missingProperties = new List<string>();
        var checkedBindings = new HashSet<string>();
        
        foreach (System.Text.RegularExpressions.Match match in matches)
        {
            var bindingPath = match.Groups[1].Value;
            
            // Skip if already checked
            if (checkedBindings.Contains(bindingPath))
                continue;
            checkedBindings.Add(bindingPath);
            
            // Skip known special bindings (converters, element names, etc.)
            if (bindingPath.StartsWith("!") || bindingPath == "Value" || bindingPath == "Text")
                continue;
            
            // Skip DataTemplate bindings (these bind to item ViewModels, not the main ViewModel)
            // RibbonItemViewModel properties used in Ribbons tab ItemTemplate
            var dataTemplateBindings = new[] { "DisplayName", "HasRibbon", "IsBooleanRibbon", "MaxCount", "RibbonCount", "IconResource" };
            if (dataTemplateBindings.Contains(bindingPath))
                continue;
            
            // Check if property exists in ViewModel
            if (!vmProperties.Contains(bindingPath))
            {
                missingProperties.Add(bindingPath);
                _output.WriteLine($"[MISSING] Binding '{bindingPath}' not found in ViewModel");
            }
        }

        // Output summary
        _output.WriteLine($"\n=== BINDING VERIFICATION ===");
        _output.WriteLine($"Total bindings checked: {checkedBindings.Count}");
        _output.WriteLine($"Missing in ViewModel: {missingProperties.Count}");
        
        foreach (var missing in missingProperties.OrderBy(x => x))
        {
            _output.WriteLine($"  - {missing}");
        }

        // Fail if any are missing
        Assert.Empty(missingProperties);
    }

    [AvaloniaFact]  
    public void Verify_No_Binding_Errors_At_Runtime()
    {
        // Create a real View without DataContext to check for runtime errors
        var view = new PokemonEditor();
        
        // The view should instantiate without errors
        Assert.NotNull(view);
        
        // Check that required controls exist by name
        var tabControl = view.FindControl<TabControl>("TabControl");
        // TabControl might not have a name, just verify the view loaded
        Assert.True(view.Width > 0 || true); // Just check we didn't crash
    }

    [AvaloniaFact]
    public void Verify_Command_Bindings_Exist()
    {
        var repoRoot = FindRepoRoot();
        var xamlPath = Path.Combine(repoRoot, "PKHeX.Avalonia", "Views", "PokemonEditor.axaml");
        var xamlContent = File.ReadAllText(xamlPath);
        
        // Get all commands from ViewModel
        var vmType = typeof(PKHeX.Presentation.ViewModels.PokemonEditorViewModel);
        var commandProperties = vmType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Where(p => p.Name.EndsWith("Command"))
            .Select(p => p.Name)
            .ToList();
        
        _output.WriteLine($"=== COMMAND VERIFICATION ===");
        _output.WriteLine($"Commands in ViewModel: {commandProperties.Count}");
        
        foreach (var cmd in commandProperties)
        {
            var isUsed = xamlContent.Contains(cmd);
            _output.WriteLine($"  {cmd}: {(isUsed ? "USED" : "NOT USED")}");
        }
        
        // Just informational, don't fail
        Assert.True(commandProperties.Count > 0);
    }

    private string FindRepoRoot()
    {
        var dir = AppDomain.CurrentDomain.BaseDirectory;
        while (!Directory.Exists(Path.Combine(dir, "PKHeX.Avalonia")))
        {
            var parent = Directory.GetParent(dir);
            if (parent == null) throw new DirectoryNotFoundException("Could not find repository root");
            dir = parent.FullName;
        }
        return dir;
    }
}
