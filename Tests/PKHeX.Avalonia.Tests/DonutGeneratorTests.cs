using System.Linq;
using PKHeX.Core;
using PKHeX.Presentation.ViewModels;
using Xunit;

namespace PKHeX.Avalonia.Tests;

public class DonutGeneratorTests
{
    [Fact]
    public void Generator_PopulatesFlavorOptions_AllSelectedByDefault()
    {
        var sav = new SAV9ZA();
        var vm = new DonutEditorViewModel(sav);

        Assert.True(vm.IsSupported);
        Assert.NotEmpty(vm.FlavorOptions);
        Assert.All(vm.FlavorOptions, o => Assert.True(o.IsSelected));

        // Mirrors upstream filter: only flavors whose two-digit type index (chars 6-7) is in [3, 21].
        Assert.All(vm.FlavorOptions, o =>
        {
            Assert.True(o.Name.Length >= 8);
            var value = int.Parse(o.Name.Substring(6, 2));
            Assert.InRange(value, 3, 21);
        });

        // Default range spans the whole pocket.
        Assert.Equal(0, vm.GenerateStart);
        Assert.Equal(DonutPocket9a.MaxCount, vm.GenerateEnd);
    }

    [Fact]
    public void Generate_OverRange_WritesShinyTemplateDonuts()
    {
        var sav = new SAV9ZA();
        var vm = new DonutEditorViewModel(sav);

        // Choose a single flavor set so every generated donut's flavors come from it.
        var chosen = vm.FlavorOptions.First();
        foreach (var option in vm.FlavorOptions)
            option.IsSelected = option == chosen;

        const int start = 10;
        const int end = 25; // end-exclusive in Core
        vm.GenerateStart = start;
        vm.GenerateEnd = end;

        vm.GenerateCommand.Execute(null);

        // Save is marked dirty.
        Assert.True(sav.State.Edited);

        var pocket = sav.Donuts;
        var chosenHash = chosen.Hash;

        // Indices in [start, end) are populated with the chosen flavor (only one option => Flavor0 set, rest 0).
        for (int i = start; i < end; i++)
        {
            var donut = pocket.GetDonut(i);
            Assert.NotEqual(0ul, donut.MillisecondsSince1970); // timestamp applied
            Assert.Equal(chosenHash, donut.Flavor0);
            Assert.Equal(0ul, donut.Flavor1);
            Assert.Equal(0ul, donut.Flavor2);
        }

        // The slot just past the range is untouched (no flavors written).
        var after = pocket.GetDonut(end);
        Assert.Equal(0ul, after.Flavor0);
    }

    [Fact]
    public void Generate_WithMultipleFlavors_DrawsOnlyFromSelectedSet()
    {
        var sav = new SAV9ZA();
        var vm = new DonutEditorViewModel(sav);

        // Pick the first three flavor options as the allowed pool.
        var pool = vm.FlavorOptions.Take(3).ToList();
        var allowed = pool.Select(z => z.Hash).ToHashSet();
        foreach (var option in vm.FlavorOptions)
            option.IsSelected = pool.Contains(option);

        vm.GenerateStart = 0;
        vm.GenerateEnd = 50;
        vm.GenerateCommand.Execute(null);

        var pocket = sav.Donuts;
        for (int i = 0; i < 50; i++)
        {
            var donut = pocket.GetDonut(i);
            foreach (var flavor in donut.GetFlavors())
            {
                if (flavor == 0ul)
                    continue;
                Assert.Contains(flavor, allowed);
            }
        }

        // VM list reloaded after generate.
        Assert.Equal(DonutPocket9a.MaxCount, vm.Donuts.Count);
    }

    [Fact]
    public void Generate_NoFlavorSelected_DoesNothing()
    {
        var sav = new SAV9ZA();
        var vm = new DonutEditorViewModel(sav);
        foreach (var option in vm.FlavorOptions)
            option.IsSelected = false;

        vm.GenerateStart = 0;
        vm.GenerateEnd = 10;
        vm.GenerateCommand.Execute(null);

        Assert.False(sav.State.Edited);
        Assert.Equal(0ul, sav.Donuts.GetDonut(0).Flavor0);
    }

    [Fact]
    public void Generate_StartAfterEnd_DoesNothing()
    {
        var sav = new SAV9ZA();
        var vm = new DonutEditorViewModel(sav);

        vm.GenerateStart = 40;
        vm.GenerateEnd = 10;
        vm.GenerateCommand.Execute(null);

        Assert.False(sav.State.Edited);
        Assert.Equal(0ul, sav.Donuts.GetDonut(20).Flavor0);
    }
}
