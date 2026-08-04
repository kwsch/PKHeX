using System.Linq;
using PKHeX.Presentation;

namespace PKHeX.Architecture.Tests;

/// <summary>
/// Locks in the Interface-Adapters boundary: all ViewModels live in PKHeX.Presentation, and no
/// ViewModel leaked back into the host (Frameworks &amp; Drivers) assembly.
/// </summary>
public class ViewModelPlacementTests
{
    private static readonly System.Reflection.Assembly Presentation = typeof(PresentationAssembly).Assembly;
    private static readonly System.Reflection.Assembly Host = typeof(PKHeX.Avalonia.App).Assembly;

    [Fact]
    public void All_ViewModels_live_in_the_Presentation_assembly()
    {
        var presentationViewModels = Presentation.GetTypes()
            .Where(t => t is { IsClass: true } && t.Name.EndsWith("ViewModel"))
            .ToList();

        Assert.NotEmpty(presentationViewModels); // sanity: they really are here
        Assert.All(presentationViewModels, t =>
            Assert.StartsWith("PKHeX.Presentation.ViewModels", t.Namespace ?? ""));
    }

    [Fact]
    public void Host_assembly_contains_no_ViewModels()
    {
        var hostViewModels = Host.GetTypes()
            .Where(t => t is { IsClass: true } && t.Name.EndsWith("ViewModel"))
            .Select(t => t.FullName)
            .ToList();

        Assert.True(hostViewModels.Count == 0,
            "ViewModels must not live in the host: " + string.Join(", ", hostViewModels));
    }
}
