using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Avalonia.Tests;

public class PokedexTests
{
    public PokedexTests()
    {
        GameInfo.Strings = GameInfo.GetStrings("en");
    }

    [Fact]
    public void Pokedex8_LoadAndSave_VerifyFlags()
    {
        // Arrange
        var sav = new SAV8SWSH();
        var zukan = sav.Blocks.Zukan;
        
        // Setup initial state for a species (e.g. Bulbasaur = 1)
        var entryInfo = Zukan8.GetRawIndexes(PersonalTable.SWSH, zukan.GetRevision(), Zukan8Index.TotalCount)
                              .FirstOrDefault(z => z.Species == 1);
        
        Assert.NotEqual(0, entryInfo.Species); // Sanity check
        var entry = entryInfo.Entry;

        zukan.SetCaught(entry, false);
        zukan.SetSeenRegion(entry, 0, 0, false); // Form 0, Seen Male

        // Act
        var vm = new Pokedex8EditorViewModel(sav);
        
        // Select Bulbasaur
        var item = vm.FilteredSpecies.FirstOrDefault(x => x.Text.Contains("Bulbasaur"));
        Assert.NotNull(item);
        vm.SelectedSpecies = item;

        // Verify loaded state
        Assert.False(vm.Caught);
        Assert.False(vm.Forms[0].SeenMale);

        // Modify
        vm.Caught = true;
        vm.Forms[0].SeenMale = true;
        
        // Save
        vm.SaveCurrent();

        // Assert
        Assert.True(zukan.GetCaught(entry));
        Assert.True(zukan.GetSeenRegion(entry, 0, 0));
    }

    [Fact]
    public void Pokedex4_LoadAndSave_VerifyListOrdering()
    {
        // Arrange
        var sav = new SAV4DP();
        var zukan = (Zukan4)sav.Dex;
        ushort shellos = 422; // Shellos has 2 forms in Gen 4

        // Act
        var vm = new Pokedex4EditorViewModel(sav);
        var item = vm.Species.FirstOrDefault(x => x.Value == shellos);
        Assert.NotNull(item);
        vm.SelectedSpecies = item;

        var totalForms = vm.SeenForms.Count + vm.UnseenForms.Count;
        // If totalForms > 2, then we have a mismatch between Names and Data Slots in Core.
        // We should only add up to 2.
        
        // Modify forms list: Add East Sea then West Sea
        vm.SelectedUnseenForm = vm.UnseenForms.FirstOrDefault(x => x.Contains("West"));
        if (vm.SelectedUnseenForm != null) vm.AddFormCommand.Execute(null); 
        vm.SelectedUnseenForm = vm.UnseenForms.FirstOrDefault(x => x.Contains("East"));
        if (vm.SelectedUnseenForm != null) vm.AddFormCommand.Execute(null); 
        
        // Ensure we don't exceed 2 even if more are available in Unseen (to avoid ArgumentOutOfRangeException)
        while (vm.SeenForms.Count > 2)
        {
            vm.SelectedSeenForm = vm.SeenForms.Last();
            vm.RemoveFormCommand.Execute(null);
        }
        
        // SeenForms should now have [West, East] or [East, West] depending on previous state.
        // Let's force it to [West, East] then move East up.
        var names = Zukan4.GetFormNames4Dex(shellos);
        vm.SeenForms.Clear();
        vm.SeenForms.Add(names[0]);
        vm.SeenForms.Add(names[1]);
        
        vm.SelectedSeenForm = names[1];
        vm.MoveFormUpCommand.Execute(null);

        vm.SaveCommand.Execute(null);

        // Assert
        var forms = zukan.GetForms(shellos);
        // Shellos form index 1 (East) should be first
        Assert.Equal(1, forms[0]);
        Assert.Equal(0, forms[1]);
    }

    [Fact]
    public void Pokedex5_LoadAndSave_VerifyFlags()
    {
        // Arrange
        var sav = new SAV5BW();
        var zukan = sav.Zukan;
        ushort victini = 494;

        // Act
        var vm = new Pokedex5EditorViewModel(sav);
        var item = vm.Species.FirstOrDefault(x => x.Value == victini);
        Assert.NotNull(item);
        vm.SelectedSpecies = item;

        vm.Caught = true;
        vm.SaveCommand.Execute(null);

        // Assert
        Assert.True(zukan.GetCaught(victini));
    }

    [Fact]
    public void Pokedex6_LoadAndSave_VerifyFlags()
    {
        // Arrange
        var sav = new SAV6XY();
        var zukan = sav.Zukan;
        ushort froakie = 656;

        // Act
        var vm = new Pokedex6EditorViewModel(sav);
        var item = vm.FilteredSpecies.FirstOrDefault(x => x.Value == froakie);
        Assert.NotNull(item);
        vm.SelectedSpecies = item;

        vm.Caught = true;
        vm.SeenMale = true;
        vm.SaveCurrent();

        // Assert
        Assert.True(zukan.GetCaught(froakie));
        Assert.True(zukan.GetSeen(froakie, 0));
    }

    [Fact]
    public void Pokedex7_LoadAndSave_VerifyFlags()
    {
        // Arrange
        var sav = new SAV7SM();
        var zukan = sav.Zukan;
        // Gen 7 Pokedex7EditorViewModel uses combined index. 
        // First entries are species.
        ushort rowlet = 722;

        // Act
        var vm = new Pokedex7EditorViewModel(sav);
        var item = vm.Entries.FirstOrDefault(x => x.Text.Contains("Rowlet"));
        Assert.NotNull(item);
        vm.SelectedEntry = item;

        vm.Caught = true;
        vm.SeenMale = true;
        vm.SaveCommand.Execute(null);

        // Assert
        Assert.True(zukan.GetCaught(rowlet));
        Assert.True(zukan.GetSeen(rowlet, 0));
    }

    [Fact]
    public void PokedexLA_LoadAndSave_VerifyFlags()
    {
        // Arrange
        var sav = new SAV8LA();
        GameInfo.FilteredSources = new FilteredGameDataSource(sav, GameInfo.Sources);
        var dex = sav.Blocks.PokedexSave;
        ushort pikachu = 25; // Pikachu has tasks

        // Act
        var vm = new PokedexLAEditorViewModel(sav);
        var entry = vm.SpeciesList.FirstOrDefault(x => x.DisplayName.Contains("Pikachu"));
        Assert.NotNull(entry);
        vm.SelectedSpecies = entry;

        entry.IsSolitudeComplete = true;
        entry.Forms[0].Obtained0 = true;
        
        vm.SaveCommand.Execute(null);

        // Assert
        Assert.True(dex.GetSolitudeComplete(pikachu));
        Assert.NotEqual(0, dex.GetPokeObtainFlags(pikachu, 0));
    }

    [Fact]
    public void Pokedex9_LoadAndSave_VerifyFlags()
    {
        // Arrange
        var sav = new SAV9SV();
        GameInfo.FilteredSources = new FilteredGameDataSource(sav, GameInfo.Sources);
        var zukan = sav.Blocks.Zukan;
        ushort sprigatito = 906;

        // Act
        var vm = new PokedexGen9EditorViewModel(sav);
        var item = vm.SpeciesList.FirstOrDefault(x => x.Value == sprigatito);
        Assert.NotNull(item);
        vm.SelectedSpecies = item;

        vm.IsSeenMale = true;
        vm.LangENG = true;
        vm.SaveCurrentCommand.Execute(null);

        // Assert
        var entry = zukan.DexPaldea.Get(sprigatito);
        Assert.True(entry.GetIsGenderSeen(0));
        Assert.True(entry.GetLanguageFlag((int)LanguageID.English));
    }

    [Fact]
    public void Pokedex8b_LoadAndSave_VerifyFlags()
    {
        // Arrange
        var sav = new SAV8BS();
        var zukan = sav.Zukan;
        ushort turtwig = 387;

        // Act
        var vm = new Pokedex8bEditorViewModel(sav);
        var item = vm.FilteredSpecies.FirstOrDefault(x => x.Value == turtwig);
        Assert.NotNull(item);
        vm.SelectedSpecies = item;

        vm.State = 3; // Caught
        vm.SeenMale = true;
        vm.LangEnglish = true;
        vm.SaveCurrent();

        // Assert
        Assert.Equal(ZukanState8b.Caught, zukan.GetState(turtwig));
        zukan.GetGenderFlags(turtwig, out bool m, out _, out _, out _);
        Assert.True(m);
        Assert.True(zukan.GetLanguageFlag(turtwig, (int)LanguageID.English));
    }

    [Fact]
    public void Pokedex7b_LoadAndSave_VerifyFlags()
    {
        // Arrange
        var sav = new SAV7b();
        var zukan = sav.Zukan;
        ushort pikachu = 25;

        // Act
        var vm = new Pokedex7bEditorViewModel(sav);
        var item = vm.Entries.FirstOrDefault(x => x.Text.Contains("Pikachu"));
        Assert.NotNull(item);
        vm.SelectedEntry = item;

        vm.Caught = true;
        vm.MinHeight = 100;
        vm.MinHeightFlag = true;
        vm.SaveCommand.Execute(null);

        // Assert
        Assert.True(zukan.GetCaught(pikachu));
        zukan.GetSizeData(DexSizeType.MinHeight, 24, out byte h, out _, out bool f);
        Assert.Equal(100, h);
        Assert.True(f);
    }
}
