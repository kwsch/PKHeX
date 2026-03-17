using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PKHeX.Avalonia.ViewModels.Subforms;
using PKHeX.Core;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class DLC5View : SubformWindow
{
    public DLC5View()
    {
        InitializeComponent();
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is DLC5ViewModel vm)
            vm.SaveCommand.Execute(null);
        CloseWithResult(true);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }

    // ========== C-Gear ==========

    private async void OnImportCGearClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DLC5ViewModel vm)
            return;
        var storage = GetTopLevel(this)?.StorageProvider;
        if (storage is null)
            return;

        var ext = vm.CGearExtension;
        var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Import C-Gear Skin",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("C-Gear Skin") { Patterns = [$"*.{CGearBackgroundBW.Extension}", $"*.{CGearBackgroundB2W2.Extension}"] }],
        });
        var file = files.FirstOrDefault();
        if (file is not null)
            vm.ImportCGearCommand.Execute(file.Path.LocalPath);
    }

    private async void OnExportCGearClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DLC5ViewModel vm)
            return;
        var storage = GetTopLevel(this)?.StorageProvider;
        if (storage is null)
            return;

        var ext = vm.CGearExtension;
        var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export C-Gear Skin",
            SuggestedFileName = $"CGear.{ext}",
            FileTypeChoices = [new FilePickerFileType("C-Gear Skin") { Patterns = [$"*.{ext}"] }],
        });
        if (file is not null)
            vm.ExportCGearCommand.Execute(file.Path.LocalPath);
    }

    // ========== Battle Videos ==========

    private async void OnImportBattleVideoClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DLC5ViewModel vm)
            return;
        var storage = GetTopLevel(this)?.StorageProvider;
        if (storage is null)
            return;

        var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Import Battle Video",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("Battle Video") { Patterns = [$"*.{BattleVideo5.Extension}"] }],
        });
        var file = files.FirstOrDefault();
        if (file is not null)
            vm.ImportBattleVideoCommand.Execute(file.Path.LocalPath);
    }

    private async void OnExportBattleVideoClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DLC5ViewModel vm)
            return;
        var storage = GetTopLevel(this)?.StorageProvider;
        if (storage is null)
            return;

        var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export Battle Video",
            SuggestedFileName = $"BattleVideo.{BattleVideo5.Extension}",
            FileTypeChoices = [new FilePickerFileType("Battle Video") { Patterns = [$"*.{BattleVideo5.Extension}"] }],
        });
        if (file is not null)
            vm.ExportBattleVideoCommand.Execute(file.Path.LocalPath);
    }

    // ========== PWT ==========

    private async void OnImportPWTClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DLC5ViewModel vm)
            return;
        var storage = GetTopLevel(this)?.StorageProvider;
        if (storage is null)
            return;

        var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Import PWT",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("PWT") { Patterns = [$"*.{WorldTournament5.Extension}"] }],
        });
        var file = files.FirstOrDefault();
        if (file is not null)
            vm.ImportPWTCommand.Execute(file.Path.LocalPath);
    }

    private async void OnExportPWTClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DLC5ViewModel vm)
            return;
        var storage = GetTopLevel(this)?.StorageProvider;
        if (storage is null)
            return;

        var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export PWT",
            SuggestedFileName = $"PWT.{WorldTournament5.Extension}",
            FileTypeChoices = [new FilePickerFileType("PWT") { Patterns = [$"*.{WorldTournament5.Extension}"] }],
        });
        if (file is not null)
            vm.ExportPWTCommand.Execute(file.Path.LocalPath);
    }

    // ========== Pokestar ==========

    private async void OnImportPokestarClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DLC5ViewModel vm)
            return;
        var storage = GetTopLevel(this)?.StorageProvider;
        if (storage is null)
            return;

        var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Import Pokestar Movie",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("Pokestar Movie") { Patterns = [$"*.{PokestarMovie5.Extension}"] }],
        });
        var file = files.FirstOrDefault();
        if (file is not null)
            vm.ImportPokestarCommand.Execute(file.Path.LocalPath);
    }

    private async void OnExportPokestarClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DLC5ViewModel vm)
            return;
        var storage = GetTopLevel(this)?.StorageProvider;
        if (storage is null)
            return;

        var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export Pokestar Movie",
            SuggestedFileName = $"PokestarMovie.{PokestarMovie5.Extension}",
            FileTypeChoices = [new FilePickerFileType("Pokestar Movie") { Patterns = [$"*.{PokestarMovie5.Extension}"] }],
        });
        if (file is not null)
            vm.ExportPokestarCommand.Execute(file.Path.LocalPath);
    }

    // ========== Musical ==========

    private async void OnImportMusicalClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DLC5ViewModel vm)
            return;
        var storage = GetTopLevel(this)?.StorageProvider;
        if (storage is null)
            return;

        var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Import Musical Show",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("Musical Show") { Patterns = [$"*.{MusicalShow5.Extension}"] }],
        });
        var file = files.FirstOrDefault();
        if (file is not null)
            vm.ImportMusicalCommand.Execute(file.Path.LocalPath);
    }

    private async void OnExportMusicalClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DLC5ViewModel vm)
            return;
        var storage = GetTopLevel(this)?.StorageProvider;
        if (storage is null)
            return;

        var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export Musical Show",
            SuggestedFileName = $"MusicalShow.{MusicalShow5.Extension}",
            FileTypeChoices = [new FilePickerFileType("Musical Show") { Patterns = [$"*.{MusicalShow5.Extension}"] }],
        });
        if (file is not null)
            vm.ExportMusicalCommand.Execute(file.Path.LocalPath);
    }

    // ========== Memory Link ==========

    private async void OnImportLink1Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DLC5ViewModel vm)
            return;
        var storage = GetTopLevel(this)?.StorageProvider;
        if (storage is null)
            return;

        var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Import Memory Link 1",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("Memory Link") { Patterns = ["*.ml5"] }],
        });
        var file = files.FirstOrDefault();
        if (file is not null)
            vm.ImportLink1Command.Execute(file.Path.LocalPath);
    }

    private async void OnExportLink1Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DLC5ViewModel vm)
            return;
        var storage = GetTopLevel(this)?.StorageProvider;
        if (storage is null)
            return;

        var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export Memory Link 1",
            SuggestedFileName = "MemoryLink1.ml5",
            FileTypeChoices = [new FilePickerFileType("Memory Link") { Patterns = ["*.ml5"] }],
        });
        if (file is not null)
            vm.ExportLink1Command.Execute(file.Path.LocalPath);
    }

    private async void OnImportLink2Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DLC5ViewModel vm)
            return;
        var storage = GetTopLevel(this)?.StorageProvider;
        if (storage is null)
            return;

        var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Import Memory Link 2",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("Memory Link") { Patterns = ["*.ml5"] }],
        });
        var file = files.FirstOrDefault();
        if (file is not null)
            vm.ImportLink2Command.Execute(file.Path.LocalPath);
    }

    private async void OnExportLink2Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DLC5ViewModel vm)
            return;
        var storage = GetTopLevel(this)?.StorageProvider;
        if (storage is null)
            return;

        var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export Memory Link 2",
            SuggestedFileName = "MemoryLink2.ml5",
            FileTypeChoices = [new FilePickerFileType("Memory Link") { Patterns = ["*.ml5"] }],
        });
        if (file is not null)
            vm.ExportLink2Command.Execute(file.Path.LocalPath);
    }

    // ========== Pokedex Skin ==========

    private async void OnImportPokedexSkinClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DLC5ViewModel vm)
            return;
        var storage = GetTopLevel(this)?.StorageProvider;
        if (storage is null)
            return;

        var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Import Pokedex Skin",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("Pokedex Skin") { Patterns = [$"*.{PokeDexSkin5.Extension}"] }],
        });
        var file = files.FirstOrDefault();
        if (file is not null)
            vm.ImportPokedexSkinCommand.Execute(file.Path.LocalPath);
    }

    private async void OnExportPokedexSkinClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DLC5ViewModel vm)
            return;
        var storage = GetTopLevel(this)?.StorageProvider;
        if (storage is null)
            return;

        var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export Pokedex Skin",
            SuggestedFileName = $"PokedexSkin.{PokeDexSkin5.Extension}",
            FileTypeChoices = [new FilePickerFileType("Pokedex Skin") { Patterns = [$"*.{PokeDexSkin5.Extension}"] }],
        });
        if (file is not null)
            vm.ExportPokedexSkinCommand.Execute(file.Path.LocalPath);
    }

    // ========== Battle Test ==========

    private async void OnImportBattleTestClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DLC5ViewModel vm)
            return;
        var storage = GetTopLevel(this)?.StorageProvider;
        if (storage is null)
            return;

        var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Import Battle Test",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("Battle Test") { Patterns = [$"*.{BattleTest5.Extension}"] }],
        });
        var file = files.FirstOrDefault();
        if (file is not null)
            vm.ImportBattleTestCommand.Execute(file.Path.LocalPath);
    }

    private async void OnExportBattleTestClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not DLC5ViewModel vm)
            return;
        var storage = GetTopLevel(this)?.StorageProvider;
        if (storage is null)
            return;

        var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export Battle Test",
            SuggestedFileName = $"BattleTest.{BattleTest5.Extension}",
            FileTypeChoices = [new FilePickerFileType("Battle Test") { Patterns = [$"*.{BattleTest5.Extension}"] }],
        });
        if (file is not null)
            vm.ExportBattleTestCommand.Execute(file.Path.LocalPath);
    }
}
