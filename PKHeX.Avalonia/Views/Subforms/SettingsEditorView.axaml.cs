using System;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using PKHeX.Avalonia.ViewModels.Subforms;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class SettingsEditorView : SubformWindow
{
    public SettingsEditorView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is not SettingsEditorViewModel vm)
            return;

        try
        {
            BuildSettingsTabs(vm);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Settings UI build failed: {ex.Message}");
        }
    }

    private void BuildSettingsTabs(SettingsEditorViewModel vm)
    {
        SettingsTabs.Items.Clear();
        foreach (var category in vm.Categories)
        {
            var tab = new TabItem { Header = category.Name };
            var scrollViewer = new ScrollViewer();
            var panel = new StackPanel { Spacing = 4, Margin = new global::Avalonia.Thickness(4) };

            foreach (var prop in category.Properties)
            {
                var row = new Grid
                {
                    ColumnDefinitions = ColumnDefinitions.Parse("200,*"),
                    Margin = new global::Avalonia.Thickness(2),
                };

                var label = new TextBlock
                {
                    Text = prop.Name,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                Grid.SetColumn(label, 0);
                row.Children.Add(label);

                Control editor;
                switch (prop.TypeTag)
                {
                    case "bool":
                        var cb = new CheckBox();
                        cb.Bind(CheckBox.IsCheckedProperty, new Binding(nameof(SettingPropertyModel.Value))
                        {
                            Source = prop,
                            Mode = BindingMode.TwoWay,
                        });
                        editor = cb;
                        break;
                    case "number":
                        var nud = new NumericUpDown
                        {
                            Width = 120,
                            HorizontalAlignment = HorizontalAlignment.Left,
                        };
                        // Set min/max based on type
                        if (prop.PropertyType == typeof(byte))
                        {
                            nud.Minimum = byte.MinValue;
                            nud.Maximum = byte.MaxValue;
                        }
                        else if (prop.PropertyType == typeof(uint))
                        {
                            nud.Minimum = uint.MinValue;
                            nud.Maximum = uint.MaxValue;
                        }
                        else if (prop.PropertyType == typeof(float))
                        {
                            nud.Minimum = 0;
                            nud.Maximum = 1;
                            nud.Increment = 0.05m;
                            nud.FormatString = "F2";
                        }
                        nud.Bind(NumericUpDown.ValueProperty, new Binding(nameof(SettingPropertyModel.Value))
                        {
                            Source = prop,
                            Mode = BindingMode.TwoWay,
                            Converter = new DecimalObjectConverter(),
                        });
                        editor = nud;
                        break;
                    case "enum":
                        var combo = new ComboBox
                        {
                            Width = 200,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            ItemsSource = prop.EnumValues,
                        };
                        combo.Bind(ComboBox.SelectedItemProperty, new Binding(nameof(SettingPropertyModel.Value))
                        {
                            Source = prop,
                            Mode = BindingMode.TwoWay,
                        });
                        editor = combo;
                        break;
                    default: // string
                        var tb = new TextBox
                        {
                            Width = 200,
                            HorizontalAlignment = HorizontalAlignment.Left,
                        };
                        tb.Bind(TextBox.TextProperty, new Binding(nameof(SettingPropertyModel.Value))
                        {
                            Source = prop,
                            Mode = BindingMode.TwoWay,
                        });
                        editor = tb;
                        break;
                }

                Grid.SetColumn(editor, 1);
                row.Children.Add(editor);
                panel.Children.Add(row);
            }

            scrollViewer.Content = panel;
            tab.Content = scrollViewer;
            SettingsTabs.Items.Add(tab);
        }
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SettingsEditorViewModel vm)
            vm.SaveCommand.Execute(null);
        CloseWithResult(true);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }
}

/// <summary>
/// Converts between decimal (used by NumericUpDown) and object (used by SettingPropertyModel.Value).
/// </summary>
internal sealed class DecimalObjectConverter : global::Avalonia.Data.Converters.IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is null)
            return null;
        try
        {
            return System.Convert.ToDecimal(value);
        }
        catch
        {
            return null;
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is not decimal d)
            return value;
        try
        {
            if (targetType == typeof(object))
                return d;
            if (targetType == typeof(int))
                return (int)d;
            if (targetType == typeof(uint))
                return (uint)d;
            if (targetType == typeof(byte))
                return (byte)d;
            if (targetType == typeof(float))
                return (float)d;
            return d;
        }
        catch
        {
            return value;
        }
    }
}
