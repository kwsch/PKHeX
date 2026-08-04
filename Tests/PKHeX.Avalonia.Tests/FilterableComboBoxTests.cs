using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using PKHeX.Avalonia.Controls;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Covers <see cref="FilterableComboBox"/>'s value/item synchronization: the reentrancy-guarded
/// bridge between the ComboItem-based ItemsSource, the displayed AutoCompleteBox Text/SelectedItem,
/// and the bound <see cref="FilterableComboBox.SelectedValue"/>.
/// </summary>
public class FilterableComboBoxTests
{
    private static ObservableCollection<ComboItem> Items() =>
    [
        new ComboItem("Bulbasaur", 1),
        new ComboItem("Ivysaur", 2),
        new ComboItem("Venusaur", 3),
    ];

    /// <summary>
    /// Regression guard for the invisible-control bug: <see cref="FilterableComboBox"/> subclasses
    /// <see cref="AutoCompleteBox"/>, so without a <c>StyleKeyOverride</c> its style key would default
    /// to <c>FilterableComboBox</c> and the Fluent <see cref="AutoCompleteBox"/> ControlTheme would never
    /// apply — the control would render with no chrome. Assert the override points at the base type.
    /// </summary>
    // Must be [AvaloniaFact], not [Fact]: it constructs a FilterableComboBox (an AvaloniaObject),
    // whose ctor calls Dispatcher.VerifyAccess(). Once any headless test has established the UI-thread
    // dispatcher, constructing a control on a plain xUnit worker thread throws "Call from invalid
    // thread" — a latent, test-order-dependent failure surfaced by adding more [AvaloniaFact] tests.
    [AvaloniaFact]
    public void StyleKeyOverride_IsAutoCompleteBox()
    {
        var control = new FilterableComboBox();

        var prop = typeof(FilterableComboBox).GetProperty(
            "StyleKeyOverride", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(prop);

        var styleKey = prop!.GetValue(control);
        Assert.Equal(typeof(AutoCompleteBox), styleKey);
    }

    /// <summary>
    /// Stronger end-to-end guard: once attached to a themed headless window and laid out, the control
    /// must resolve the Fluent <see cref="AutoCompleteBox"/> ControlTheme and receive a non-null
    /// <see cref="TemplatedControl.Template"/>. Before the <c>StyleKeyOverride</c> fix this was null,
    /// which is what made the combo render empty/invisible at runtime.
    /// </summary>
    [AvaloniaFact]
    public void AttachedToThemedWindow_ResolvesAutoCompleteBoxTemplate()
    {
        var control = new FilterableComboBox { ItemsSource = Items() };
        var window = new Window { Content = control, Width = 200, Height = 60 };
        window.Show();

        Dispatcher.UIThread.RunJobs();
        control.Measure(new Size(200, 60));
        control.Arrange(new Rect(0, 0, 200, 60));
        Dispatcher.UIThread.RunJobs();
        control.ApplyTemplate();

        Assert.NotNull(control.Template);
    }

    [AvaloniaFact]
    public void SettingSelectedValue_SyncsSelectedItemAndText()
    {
        var control = new FilterableComboBox { ItemsSource = Items() };

        control.SelectedValue = 2;

        Assert.Equal(new ComboItem("Ivysaur", 2), control.SelectedItem);
        Assert.Equal("Ivysaur", control.Text);
    }

    [AvaloniaFact]
    public void CommittingSelection_PushesValueBackToSelectedValue()
    {
        var control = new FilterableComboBox { ItemsSource = Items() };

        control.SelectedItem = new ComboItem("Venusaur", 3);

        Assert.Equal(3, control.SelectedValue);
    }

    [AvaloniaFact]
    public void ReassigningItemsSource_ResyncsDisplayedText()
    {
        var control = new FilterableComboBox { ItemsSource = Items() };
        control.SelectedValue = 3;
        Assert.Equal("Venusaur", control.Text);

        var replacement = new ObservableCollection<ComboItem>
        {
            new("Charmander", 4),
            new("Charmeleon", 5),
            new("Charizard", 3),
        };
        control.ItemsSource = replacement;

        Assert.Equal("Charizard", control.Text);
        Assert.Equal(new ComboItem("Charizard", 3), control.SelectedItem);
    }

    [AvaloniaFact]
    public void TransientSelectedItemNull_DoesNotClobberSelectedValue()
    {
        var control = new FilterableComboBox { ItemsSource = Items() };
        control.SelectedValue = 2;

        // AutoCompleteBox clears SelectedItem transiently while the user types; that must not
        // reset the bound value.
        control.SelectedItem = null;

        Assert.Equal(2, control.SelectedValue);
    }

    [AvaloniaFact]
    public void AbsentValue_ClearsText()
    {
        var control = new FilterableComboBox { ItemsSource = Items() };

        control.SelectedValue = 999;

        Assert.Null(control.SelectedItem);
        Assert.Equal(string.Empty, control.Text);
    }

    /// <summary>
    /// <see cref="FilterableComboBox.SelectedValue"/> is declared as <see cref="int"/>, but
    /// EncounterDatabaseViewModel.SelectedSpecies (a common binding target for species pickers)
    /// is <see cref="ushort"/>. Avalonia's binding engine coerces numeric types on both legs of a
    /// two-way binding, so this should round-trip cleanly without needing a converter.
    /// </summary>
    private sealed class UshortValueViewModel : INotifyPropertyChanged
    {
        private ushort _selectedSpecies;

        public ushort SelectedSpecies
        {
            get => _selectedSpecies;
            set
            {
                if (_selectedSpecies == value)
                    return;
                _selectedSpecies = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSpecies)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    [AvaloniaFact]
    public void TwoWayBinding_RoundTripsThroughUshortViewModelProperty()
    {
        var vm = new UshortValueViewModel { SelectedSpecies = 1 };
        var control = new FilterableComboBox { ItemsSource = Items() };
        control.Bind(FilterableComboBox.SelectedValueProperty,
            new Binding(nameof(UshortValueViewModel.SelectedSpecies)) { Source = vm, Mode = BindingMode.TwoWay });

        // VM -> control: initial value flows in.
        Assert.Equal(1, control.SelectedValue);
        Assert.Equal(new ComboItem("Bulbasaur", 1), control.SelectedItem);

        // VM -> control: subsequent VM changes flow in.
        vm.SelectedSpecies = 3;
        Assert.Equal(3, control.SelectedValue);
        Assert.Equal("Venusaur", control.Text);

        // control -> VM: selecting a card pushes the value back and it lands as a ushort.
        control.SelectedItem = new ComboItem("Ivysaur", 2);
        Assert.Equal((ushort)2, vm.SelectedSpecies);
    }
}
