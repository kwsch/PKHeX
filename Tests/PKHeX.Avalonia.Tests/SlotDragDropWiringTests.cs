using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Structural guardrail for the box/party/editor drag-and-drop fix: a pragmatic, regex-free
/// source scan of the .axaml views (mirroring <see cref="AccessibilityAuditTests"/>'s approach)
/// confirming the drop-target wiring actually landed where the fix requires it:
///  - DragDrop.AllowDrop lives on the slot Button (BoxViewer/PartyViewer) so it, not a child
///    visual, is the drop target once its content is made hit-test-transparent.
///  - The slot content (sprite image + overlays) is IsHitTestVisible="False" so it doesn't
///    swallow the hit before it reaches the AllowDrop Button.
///  - PokemonEditor's AllowDrop lives on the inner Border (not the root UserControl).
///  - DragDrop.DragOver handlers are wired for drop-cursor feedback.
/// This is a static-source check; the behavioral counterpart (real layout + hit-testing that a slot
/// click routes to the Button) lives in <see cref="SlotClickHitTestTests"/>.
/// </summary>
public class SlotDragDropWiringTests(ITestOutputHelper output)
{
    [Fact]
    public void BoxViewer_SlotButton_HasAllowDropAndDragOverAndTransparentContent()
    {
        var xml = ReadView("BoxViewer.axaml");

        Assert.Contains("DragDrop.AllowDrop=\"True\"", xml);
        Assert.Contains("DragDrop.Drop=\"OnSlotDrop\"", xml);
        Assert.Contains("DragDrop.DragOver=\"OnSlotDragOver\"", xml);

        // The sprite/overlay content Panel inside the slot Button must not intercept hit-testing.
        var contentPanelIndex = xml.IndexOf("<Panel HorizontalAlignment=\"Stretch\" VerticalAlignment=\"Stretch\" IsHitTestVisible=\"False\">", StringComparison.Ordinal);
        Assert.True(contentPanelIndex >= 0, "Expected the slot content Panel to be IsHitTestVisible=\"False\".");
        output.WriteLine("BoxViewer.axaml: slot Button carries AllowDrop/Drop/DragOver, content Panel is hit-test-transparent ✓");
    }

    [Fact]
    public void PartyViewer_SlotButton_HasAllowDropAndDragOverAndTransparentContent()
    {
        var xml = ReadView("PartyViewer.axaml");

        Assert.Contains("DragDrop.AllowDrop=\"True\"", xml);
        Assert.Contains("DragDrop.Drop=\"OnSlotDrop\"", xml);
        Assert.Contains("DragDrop.DragOver=\"OnSlotDragOver\"", xml);

        // The local Button template's PART_Border root must stay hit-test-VISIBLE (it carries the
        // Button's Background), otherwise the Button's whole visual subtree is excluded from
        // hit-testing and clicks/drops die. Regression guard for PR #169, which had set it
        // IsHitTestVisible="False". Drop targeting still resolves because Avalonia walks up from the
        // hit element to the Button's DragDrop.AllowDrop.
        Assert.Contains("<Border Name=\"PART_Border\"", xml);
        var partBorderIndex = xml.IndexOf("<Border Name=\"PART_Border\"", StringComparison.Ordinal);
        var partBorderTag = xml[partBorderIndex..xml.IndexOf('>', partBorderIndex)];
        Assert.DoesNotContain("IsHitTestVisible=\"False\"", partBorderTag);

        // The card content Panel (sprite + overlays) stays hit-test-transparent so hits resolve to
        // the Button, not the Image.
        Assert.Contains("<Panel IsHitTestVisible=\"False\">", xml);
        output.WriteLine("PartyViewer.axaml: slot Button carries AllowDrop/Drop/DragOver, PART_Border root is hit-testable, content Panel is hit-test-transparent ✓");
    }

    [Fact]
    public void BoxViewer_SlotTemplate_InThemeAxaml_HasHitTestableRoot()
    {
        var repoRoot = FindRepoRoot();
        var themePath = Path.Combine(repoRoot, "PKHeX.Avalonia", "Styles", "Theme.axaml");
        var xml = File.ReadAllText(themePath);

        var styleIndex = xml.IndexOf("Selector=\"Button.slot\"", StringComparison.Ordinal);
        Assert.True(styleIndex >= 0, "Expected a Button.slot style in Theme.axaml.");

        var templateSlice = xml[styleIndex..Math.Min(xml.Length, styleIndex + 2000)];
        // The template root Panel must stay hit-test-VISIBLE with a non-null fill (Transparent
        // counts). A templated Button has no hittable surface of its own, so a non-hittable root
        // excludes the entire subtree from hit-testing and kills every slot click (and drop).
        // Regression guard for PR #169, which had set the root <Panel IsHitTestVisible="False">.
        Assert.Contains("<Panel Background=\"Transparent\">", templateSlice);
        Assert.DoesNotContain("<Panel IsHitTestVisible=\"False\">", templateSlice);
        output.WriteLine("Theme.axaml: Button.slot template root Panel is hit-testable (Transparent fill) ✓");
    }

    [Fact]
    public void PokemonEditor_AllowDrop_IsOnInnerBorder_NotRootUserControl()
    {
        var xml = ReadView("PokemonEditor.axaml");

        var userControlOpenTag = xml[..xml.IndexOf(">", StringComparison.Ordinal)];
        Assert.DoesNotContain("DragDrop.AllowDrop", userControlOpenTag);

        var borderIndex = xml.IndexOf("<Border Padding=\"6\" Classes=\"view-container\"", StringComparison.Ordinal);
        Assert.True(borderIndex >= 0, "Expected the editor's outer Border.");
        var borderSlice = xml[borderIndex..Math.Min(xml.Length, borderIndex + 400)];
        Assert.Contains("DragDrop.AllowDrop=\"True\"", borderSlice);
        Assert.Contains("DragDrop.Drop=\"OnEditorDrop\"", borderSlice);
        Assert.Contains("DragDrop.DragOver=\"OnEditorDragOver\"", borderSlice);
        output.WriteLine("PokemonEditor.axaml: AllowDrop lives on the inner Border, not the root UserControl ✓");
    }

    private static string ReadView(string fileName)
    {
        var repoRoot = FindRepoRoot();
        var path = Path.Combine(repoRoot, "PKHeX.Avalonia", "Views", fileName);
        Assert.True(File.Exists(path), $"View not found: {path}");
        return File.ReadAllText(path);
    }

    private static string FindRepoRoot()
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
