using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using PKHeX.Core;

namespace PKHeX.Avalonia.Controls;

/// <summary>
/// A type-to-filter combo box built on <see cref="AutoCompleteBox"/>, bound like a value-backed
/// ComboBox via <see cref="SelectedValue"/> against a list of <see cref="ComboItem"/>.
/// </summary>
public class FilterableComboBox : AutoCompleteBox
{
    public static readonly StyledProperty<int> SelectedValueProperty =
        AvaloniaProperty.Register<FilterableComboBox, int>(nameof(SelectedValue), defaultBindingMode: BindingMode.TwoWay);

    public int SelectedValue
    {
        get => GetValue(SelectedValueProperty);
        set => SetValue(SelectedValueProperty, value);
    }

    private bool _syncing;

    // Without this, the control's style key defaults to FilterableComboBox and the
    // Fluent AutoCompleteBox control theme never applies — the control renders empty.
    protected override Type StyleKeyOverride => typeof(AutoCompleteBox);

    public FilterableComboBox()
    {
        FilterMode = AutoCompleteFilterMode.Contains;
        ValueMemberBinding = new Binding("Text");
        MinimumPrefixLength = 0;
        MinimumPopulateDelay = TimeSpan.FromMilliseconds(150);
        MaxDropDownHeight = 300;

        SelectionChanged += OnSelectionChanged;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SelectedValueProperty || change.Property == ItemsSourceProperty)
            SyncSelectedItemFromValue();
    }

    // Deliberately does NOT mirror raw SelectedItem -> SelectedValue: transient nulls while
    // typing must never clobber the bound value. Only a concrete ComboItem selection commits.
    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_syncing)
            return;

        if (SelectedItem is ComboItem ci)
        {
            _syncing = true;
            try
            {
                SelectedValue = ci.Value;
            }
            finally
            {
                _syncing = false;
            }
        }
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);

        var expected = FindComboItem(SelectedValue);
        if (expected is null)
        {
            if (!string.IsNullOrEmpty(Text))
                Text = string.Empty;
            return;
        }

        if (Text != expected.Text)
        {
            _syncing = true;
            try
            {
                SelectedItem = expected;
                Text = expected.Text;
            }
            finally
            {
                _syncing = false;
            }
        }
    }

    private void SyncSelectedItemFromValue()
    {
        if (_syncing)
            return;

        _syncing = true;
        try
        {
            var match = FindComboItem(SelectedValue);
            if (match is null)
            {
                SelectedItem = null;
                Text = string.Empty;
                return;
            }

            SelectedItem = match;
            Text = match.Text;
        }
        finally
        {
            _syncing = false;
        }
    }

    private ComboItem? FindComboItem(int value)
    {
        if (ItemsSource is null)
            return null;

        foreach (var item in ItemsSource)
        {
            if (item is ComboItem ci && ci.Value == value)
                return ci;
        }

        return null;
    }
}
