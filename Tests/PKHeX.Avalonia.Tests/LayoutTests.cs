using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using PKHeX.Avalonia.Services;
using PKHeX.Presentation.ViewModels;
using PKHeX.Avalonia.Views;
using PKHeX.Core;
using PKHeX.Infrastructure.GiftRecords;
using Moq;
using Xunit;
using Xunit.Sdk;

namespace PKHeX.Avalonia.Tests;

public class LayoutTests
{
    private readonly SaveFile _saveFile;

    public LayoutTests()
    {
        _saveFile = new SAV3E(); // Using E explicitly as established
    }

    [AvaloniaFact]
    public void Verify_No_Horizontal_Overflow()
    {
        // 1. Setup the View and ViewModel
        var pkm = new PK3();
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, _saveFile);
        var view = new PokemonEditor { DataContext = vm };
        
        // Stress Test Data
        vm.Nickname = "ChristopherMaximumLongName";
        vm.OriginalTrainerName = "ChristopherMaximumLongName";


        // Create a window to host the view (use 700px to match MainWindow left column)
        var window = new Window
        {
            Content = view,
            Width = 700,
            Height = 600
        };
        window.Show();

        // 3. Force Layout for initial state
        Dispatcher.UIThread.RunJobs();
        
        // Find TabControl to cycle through tabs
        // We look for any TabControl since we know there is only one in the view
        var tabControl = view.GetVisualDescendants().OfType<TabControl>().FirstOrDefault();
        
        // Verify we found it
        Assert.NotNull(tabControl);

        // Iterate through each tab to check for overflow in all views
        foreach (var item in tabControl.Items)
        {
            tabControl.SelectedItem = item;
            
            // Force layout update for the new tab
            Dispatcher.UIThread.RunJobs();
            view.Measure(new Size(680, 600)); // Use 680 to match window/column width
            view.Arrange(new Rect(0, 0, 680, 600));
            Dispatcher.UIThread.RunJobs();

            // Verify bounds for this tab's content
            LayoutValidator.Validate(view);
        }
    }


    [AvaloniaFact]
    public void Verify_RTC3Editor_Layout()
    {
        // Must use a Gen 3 save for RTC3Editor
        var pkm = new PK3();
        var sav = new SAV3E(new byte[0x20000]); // Initialize with full buffer
        var vm = new RTC3EditorViewModel(sav);
        var view = new RTC3Editor { DataContext = vm };
        ForceLayout(view);
        LayoutValidator.Validate(view);
    }

    [AvaloniaFact]
    public void Verify_TrainerEditor_Layout()
    {
        var vm = new TrainerEditorViewModel(_saveFile);
        // Stress Test Data
        vm.TrainerName = "ChristopherMaximumLongName";
        vm.Money = 9999999;
        vm.Tid16 = 65535;
        vm.Sid16 = 65535;
        vm.PlayedHours = 999;
        vm.PlayedMinutes = 59;
        vm.PlayedSeconds = 59;
        
        var view = new TrainerEditor { DataContext = vm };
        ForceLayout(view);
        LayoutValidator.Validate(view);
    }

    [AvaloniaFact]
    public void Verify_InventoryEditor_Layout()
    {
        var vm = new InventoryEditorViewModel(_saveFile, new Moq.Mock<ISpriteRenderer>().Object);
        // TODO: Populate inventory with max items if possible, but requires complex mocking
        var view = new InventoryEditor { DataContext = vm };
        ForceLayout(view);
        LayoutValidator.Validate(view);
    }

    [AvaloniaFact]
    public void Verify_EventFlagsEditor_Layout()
    {
        var vm = new EventFlagsEditorViewModel(_saveFile);
        var view = new EventFlagsEditor { DataContext = vm };
        ForceLayout(view);
        LayoutValidator.Validate(view);
    }

    [AvaloniaFact]
    public void Verify_MysteryGiftEditor_Layout()
    {
        var dialogMock = new Mock<IDialogService>();
        var vm = new MysteryGiftEditorViewModel(_saveFile, dialogMock.Object);
        var view = new MysteryGiftEditor { DataContext = vm };
        ForceLayout(view);
        LayoutValidator.Validate(view);
    }

    [AvaloniaFact]
    public void Verify_SwitchGiftRecordEditor_Layout()
    {
        var dialogMock = new Mock<IDialogService>();
        var save = BlankSaveFile.Get(GameVersion.SL);
        var vm = new MysteryGiftEditorViewModel(save, dialogMock.Object, new GiftRecordProvider());
        var view = new MysteryGiftEditor { DataContext = vm };
        ForceLayout(view);
        LayoutValidator.Validate(view);
    }

    [AvaloniaFact]
    public void Verify_BatchEditor_Layout()
    {
        var dialogMock = new Mock<IDialogService>();
        var vm = new BatchEditorViewModel(_saveFile, dialogMock.Object);
        var view = new PKHeX.Avalonia.Views.BatchEditor { DataContext = vm };
        ForceLayout(view);
        LayoutValidator.Validate(view);
    }

    private void ForceLayout(Control view)
    {
         var window = new Window
        {
            Content = view,
            Width = 800,
            Height = 600
        };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        view.Measure(new Size(800, 600)); 
        view.Arrange(new Rect(0, 0, 800, 600));
        Dispatcher.UIThread.RunJobs();
    }

    [AvaloniaFact]
    public void Verify_RibbonEditor_ConstructsWithHeadless()
    {
        // RibbonEditorViewModel loads .png assets via Avalonia AssetLoader —
        // must run inside the headless app context.
        var pkm = new PK6 { Species = 25 };
        var ex = Record.Exception(() =>
        {
            var vm = new RibbonEditorViewModel(pkm);
            Assert.NotNull(vm);
            Assert.NotEmpty(vm.Ribbons);
        });
        Assert.Null(ex);
    }

    [AvaloniaFact]
    public void Verify_Critical_Controls_Are_Visible()
    {
        var pkm = new PK3();
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, _saveFile);
        var view = new PokemonEditor { DataContext = vm };

        var window = new Window { Content = view, Width = 400, Height = 600 };
        window.Show();
        Dispatcher.UIThread.RunJobs();

        // Helper to find by Type
        var textBoxes = view.GetVisualDescendants().OfType<TextBox>();
        
        foreach (var tb in textBoxes)
        {
            if (tb.Name == "NicknameBox") // Example if we named it, or just generally check all
            {
                 // ensure it has size
                 Assert.True(tb.Bounds.Width > 0);
                 Assert.True(tb.Bounds.Height > 0);
            }
        }
    }
}
