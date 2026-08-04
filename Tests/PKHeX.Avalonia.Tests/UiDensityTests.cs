using System.Text.RegularExpressions;

namespace PKHeX.Avalonia.Tests;

public class UiDensityTests
{
    [Fact]
    public void Theme_UsesCompactSharedStyleValues()
    {
        var theme = ReadSourceFile("Styles", "Theme.axaml");

        AssertStyleSetter(theme, "Border.card", "Padding", "4");
        AssertStyleSetter(theme, "Border.card-elevated", "Padding", "4");
        AssertStyleSetter(theme, "Button.accent-gradient", "Padding", "16,8");
        AssertStyleSetter(theme, "Border.badge-success", "Padding", "6,3");
        AssertStyleSetter(theme, "Border.badge-error", "Padding", "6,3");
        AssertStyleSetter(theme, "Border.header-accent", "Padding", "12,8");
        AssertStyleSetter(theme, "Border.section-card", "Padding", "6");
        AssertStyleSetter(theme, "Border.view-container", "Padding", "8");
        AssertStyleSetter(theme, "NumericUpDown.form-field", "MinHeight", "32");
        AssertStyleSetter(theme, "NumericUpDown.form-field", "Padding", "6,3");
        AssertStyleSetter(theme, "DataGridRow", "MinHeight", "36");
        AssertStyleSetter(theme, "DataGridCell", "Padding", "6,3");
    }

    [Fact]
    public void MainWindow_UsesCompactEditorColumnAndStatusBar()
    {
        var mainWindow = ReadSourceFile("Views", "MainWindow.axaml");
        var statusBar = Regex.Match(
            mainWindow,
            "<Border\\s+DockPanel.Dock=\"Bottom\"[^>]*>",
            RegexOptions.Singleline);

        Assert.Contains("ColumnDefinitions=\"520,*\"", mainWindow);
        Assert.True(statusBar.Success, "Status bar Border was not found.");
        Assert.Contains("Padding=\"8,4\"", statusBar.Value);
    }

    [Fact]
    public void PokemonEditor_UsesCompactContentSpacingAndMargin()
    {
        var pokemonEditor = ReadSourceFile("Views", "PokemonEditor.axaml");
        var topLevelContentStacks = Regex.Matches(
            pokemonEditor,
            "<ScrollViewer HorizontalScrollBarVisibility=\"Disabled\" VerticalScrollBarVisibility=\"Auto\">\\s*<StackPanel\\b[^>]*>");

        Assert.Equal(7, topLevelContentStacks.Count);
        Assert.All(topLevelContentStacks, stack =>
        {
            Assert.Contains("Spacing=\"12\"", stack.Value);
            Assert.Contains("Margin=\"6,6,6,64\"", stack.Value);
        });
    }

    private static void AssertStyleSetter(string xaml, string selector, string property, string value)
    {
        var style = Regex.Match(
            xaml,
            $"<Style\\s+Selector=\"{Regex.Escape(selector)}\">(?<body>.*?)</Style>",
            RegexOptions.Singleline);

        Assert.True(style.Success, $"Style '{selector}' was not found.");
        Assert.Contains($"<Setter Property=\"{property}\" Value=\"{value}\" />", style.Groups["body"].Value);
    }

    private static string ReadSourceFile(params string[] relativePath)
    {
        var path = Path.Combine([FindRepoRoot(), "PKHeX.Avalonia", .. relativePath]);
        Assert.True(File.Exists(path), $"Source file not found: {path}");
        return File.ReadAllText(path);
    }

    private static string FindRepoRoot()
    {
        var dir = AppDomain.CurrentDomain.BaseDirectory;
        while (!Directory.Exists(Path.Combine(dir, "PKHeX.Avalonia")))
        {
            var parent = Directory.GetParent(dir);
            if (parent == null)
                throw new DirectoryNotFoundException("Could not find repository root");
            dir = parent.FullName;
        }

        return dir;
    }
}
