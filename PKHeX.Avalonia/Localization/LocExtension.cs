using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using PKHeX.Presentation.Localization;

namespace PKHeX.Avalonia.Localization;

/// <summary>
/// XAML markup extension that binds a control property to a localized UI string by key, e.g.
/// <c>Header="{loc:Loc Menu_File}"</c>.
/// </summary>
/// <remarks>
/// <para>
/// It binds to <see cref="LocalizedStrings.CurrentLanguage"/> — a scalar property that raises a
/// change notification on every language switch — and resolves the key through
/// <see cref="LocKeyConverter"/>. Binding to a normal property (rather than the string indexer) is
/// what makes live switching reliable: Avalonia re-runs the binding when
/// <c>CurrentLanguage</c> changes, and the converter returns the freshly-looked-up translation. This
/// is what re-renders menus, tabs, buttons, and dialogs in the new language with no restart.
/// </para>
/// <para>
/// Lives in the Avalonia host layer (not Presentation) because it depends on Avalonia's binding
/// types; the underlying string table stays framework-free in Presentation.
/// </para>
/// </remarks>
public sealed class LocExtension : MarkupExtension
{
    public LocExtension() { }

    public LocExtension(string key) => Key = key;

    /// <summary>The resource key to look up in the active language table.</summary>
    public string Key { get; set; } = string.Empty;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return new Binding
        {
            Path = nameof(LocalizedStrings.CurrentLanguage),
            Source = LocalizedStrings.Instance,
            Mode = BindingMode.OneWay,
            Converter = LocKeyConverter.Instance,
            ConverterParameter = Key,
        };
    }
}

/// <summary>
/// Maps the active language (the bound value, ignored) plus a resource key (the converter parameter)
/// to the localized string via <see cref="LocalizedStrings.Instance"/>. One shared instance; the
/// lookup is what actually produces the translated text each time the language changes.
/// </summary>
public sealed class LocKeyConverter : IValueConverter
{
    public static readonly LocKeyConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => LocalizedStrings.Instance[parameter as string ?? string.Empty];

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
