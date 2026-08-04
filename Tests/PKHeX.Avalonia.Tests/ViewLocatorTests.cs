using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using PKHeX.Avalonia.Views;
using PKHeX.Presentation.ViewModels;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Verifies the VM-first navigation seam: <see cref="PKHeX.Avalonia.ViewLocator"/> resolves a dialog
/// ViewModel to its View and wires the DataContext, the behavior <c>WindowService</c> relies on.
/// </summary>
public class ViewLocatorTests
{
    [AvaloniaFact]
    public void Build_resolves_View_and_sets_DataContext_for_About()
    {
        var vm = new AboutViewModel();
        var view = PKHeX.Avalonia.ViewLocator.Build(vm);
        Assert.IsType<AboutView>(view);
        Assert.Same(vm, view.DataContext);
    }

    [AvaloniaFact]
    public void Build_resolves_LegalityView_with_report()
    {
        var vm = new LegalityViewModel("legality report text");
        var view = PKHeX.Avalonia.ViewLocator.Build(vm);
        Assert.IsType<LegalityView>(view);
        Assert.Same(vm, view.DataContext);
    }

    [AvaloniaFact]
    public void Build_returns_fallback_for_unmapped_object()
    {
        var view = PKHeX.Avalonia.ViewLocator.Build("not a view model");
        Assert.IsType<TextBlock>(view);
    }
}
