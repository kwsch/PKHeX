using NetArchTest.Rules;
using PKHeX.Application;
using PKHeX.Infrastructure;
using PKHeX.Presentation;

namespace PKHeX.Architecture.Tests;

public class LayerDependencyTests
{
    private static readonly System.Reflection.Assembly App = typeof(ApplicationAssembly).Assembly;
    private static readonly System.Reflection.Assembly Infra = typeof(InfrastructureAssembly).Assembly;
    private static readonly System.Reflection.Assembly Pres = typeof(PresentationAssembly).Assembly;

    [Fact]
    public void Application_does_not_depend_on_outer_layers_or_frameworks()
    {
        var result = Types.InAssembly(App)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Avalonia", "SkiaSharp", "CommunityToolkit.Mvvm",
                "PKHeX.Infrastructure", "PKHeX.Presentation", "PKHeX.Avalonia")
            .GetResult();
        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Infrastructure_does_not_depend_on_ui_or_presentation_or_host()
    {
        var result = Types.InAssembly(Infra)
            .ShouldNot()
            .HaveDependencyOnAny("Avalonia", "SkiaSharp", "PKHeX.Presentation", "PKHeX.Avalonia")
            .GetResult();
        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Presentation_does_not_depend_on_ui_or_infrastructure_or_host()
    {
        var result = Types.InAssembly(Pres)
            .ShouldNot()
            .HaveDependencyOnAny("Avalonia", "SkiaSharp", "PKHeX.Infrastructure", "PKHeX.Avalonia")
            .GetResult();
        Assert.True(result.IsSuccessful, Describe(result));
    }

    private static string Describe(TestResult r) =>
        r.IsSuccessful ? "" : "Offending types: " + string.Join(", ", r.FailingTypeNames ?? []);
}
