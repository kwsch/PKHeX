using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace PKHeX.WinForms;

internal static class PropertyGridLocalization
{
    // Track which instances already have a provider registered to avoid stacking providers
    // (TypeDescriptor.AddProviderTransparent registrations live for the process lifetime).
    private static readonly ConditionalWeakTable<object, LocalizationState> Registered = [];

    public static void Apply(PropertyGrid grid, object selectedObject, string lang)
    {
        var localizer = WinFormsTranslator.GetDictionary(lang);
        if (!Registered.TryGetValue(selectedObject, out var state))
        {
            state = new LocalizationState(localizer);
            TypeDescriptor.AddProviderTransparent(new LocalizedTypeDescriptionProvider(selectedObject.GetType(), state), selectedObject);
            Registered.Add(selectedObject, state);
        }
        else
        {
            state.Localizer = localizer;
        }
        grid.SelectedObject = selectedObject;
    }
}

internal sealed class LocalizationState(IReadOnlyDictionary<string, string> localizer)
{
    public IReadOnlyDictionary<string, string> Localizer { get; set; } = localizer;
}

internal sealed class LocalizedTypeDescriptionProvider(System.Type type, LocalizationState state)
    : TypeDescriptionProvider(TypeDescriptor.GetProvider(type))
{
    public override ICustomTypeDescriptor GetTypeDescriptor(System.Type objectType, object? instance)
    {
        var descriptor = base.GetTypeDescriptor(objectType, instance)!;
        return new LocalizedTypeDescriptor(descriptor, state);
    }
}

internal sealed class LocalizedTypeDescriptor(ICustomTypeDescriptor parent, LocalizationState state) : CustomTypeDescriptor(parent)
{
    public override PropertyDescriptorCollection GetProperties() => Localize(base.GetProperties());

    public override PropertyDescriptorCollection GetProperties(Attribute[]? attributes) => Localize(base.GetProperties(attributes));

    private PropertyDescriptorCollection Localize(PropertyDescriptorCollection properties)
    {
        var localized = properties.Cast<PropertyDescriptor>()
            .Select(z => new LocalizedPropertyDescriptor(z, state))
            .ToArray<PropertyDescriptor>();
        return new PropertyDescriptorCollection(localized, true);
    }
}

internal sealed class LocalizedPropertyDescriptor(PropertyDescriptor parent, LocalizationState state)
    : PropertyDescriptor(parent)
{
    private TypeConverter? converter;

    public override string DisplayName => TranslatePropertyName(Name, parent.DisplayName);
    public override string Category => Translate($"PropertyGrid.Category.{parent.Category}", parent.Category);
    public override TypeConverter Converter => converter ??= new LocalizedTypeConverter(parent.Converter, PropertyType, state);

    public override bool CanResetValue(object component) => parent.CanResetValue(component);
    public override System.Type ComponentType => parent.ComponentType;
    public override object? GetValue(object? component) => parent.GetValue(component);
    public override bool IsReadOnly => parent.IsReadOnly;
    public override System.Type PropertyType => parent.PropertyType;
    public override void ResetValue(object component) => parent.ResetValue(component);
    public override void SetValue(object? component, object? value) => parent.SetValue(component, value);
    public override bool ShouldSerializeValue(object component) => parent.ShouldSerializeValue(component);

    private string TranslatePropertyName(string name, string fallback)
        => state.Localizer.GetValueOrDefault($"PropertyGrid.{name}", fallback);

    private string Translate(string key, string fallback) => state.Localizer.GetValueOrDefault(key, fallback);
}

internal sealed class LocalizedTypeConverter(TypeConverter parent, System.Type valueType, LocalizationState state) : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, System.Type sourceType) => parent.CanConvertFrom(context, sourceType);

    public override bool CanConvertTo(ITypeDescriptorContext? context, System.Type? destinationType) => parent.CanConvertTo(context, destinationType);

    public override bool GetCreateInstanceSupported(ITypeDescriptorContext? context) => parent.GetCreateInstanceSupported(context);

    public override object? CreateInstance(ITypeDescriptorContext? context, System.Collections.IDictionary propertyValues) => parent.CreateInstance(context, propertyValues);

    public override bool GetPropertiesSupported(ITypeDescriptorContext? context) => parent.GetPropertiesSupported(context);

    public override PropertyDescriptorCollection? GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes)
    {
        var properties = parent.GetProperties(context, value, attributes);
        if (properties is null)
            return null;

        var localized = properties.Cast<PropertyDescriptor>()
            .Select(z => new LocalizedPropertyDescriptor(z, state))
            .ToArray<PropertyDescriptor>();
        return new PropertyDescriptorCollection(localized, true);
    }

    public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context) => parent.GetStandardValuesExclusive(context);

    public override bool GetStandardValuesSupported(ITypeDescriptorContext? context) => parent.GetStandardValuesSupported(context);

    public override StandardValuesCollection? GetStandardValues(ITypeDescriptorContext? context) => parent.GetStandardValues(context);

    public override bool IsValid(ITypeDescriptorContext? context, object? value)
    {
        if (value is string text && TryGetOriginalValue(text, out _))
            return true;
        return parent.IsValid(context, value);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string text && TryGetOriginalValue(text, out var original))
            return original;
        return parent.ConvertFrom(context, culture, value);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, System.Type destinationType)
    {
        if (destinationType == typeof(string) && value is not null && ShouldLocalizeString(value.GetType()))
            return GetLocalizedText(value) ?? parent.ConvertTo(context, culture, value, destinationType);
        return parent.ConvertTo(context, culture, value, destinationType);
    }

    private bool ShouldLocalizeString(System.Type type)
    {
        var effective = Nullable.GetUnderlyingType(type) ?? type;
        if (effective == typeof(string) || effective == typeof(bool) || effective.IsEnum || typeof(ICollection).IsAssignableFrom(effective))
            return true;
        if (effective.IsPrimitive || effective.Namespace?.StartsWith("System.", StringComparison.Ordinal) == true)
            return false;
        if (valueType.Namespace?.StartsWith("PKHeX.", StringComparison.Ordinal) == true)
            return true;
        return false;
    }

    private string? GetLocalizedText(object value) => GetLocalizedText(value, 0);

    private string? GetLocalizedText(object value, int depth) => value switch
    {
        string s => Translate($"PropertyGrid.Value.{s}", s),
        bool b => Translate($"PropertyGrid.Value.{b}", b.ToString()),
        Enum e => Translate($"{e.GetType().Name}.{e}", e.ToString()),
        ICollection => Translate("PropertyGrid.Value.Collection", "(Collection)"),
        _ => GetLocalizedObjectText(value, depth),
    };

    private bool TryGetOriginalValue(string text, out object? value)
    {
        var type = Nullable.GetUnderlyingType(valueType) ?? valueType;

        if (type == typeof(bool))
        {
            if (TryGetSingleTranslatedMatch(text, [true, false], out value))
                return true;
            if (MatchesRaw(text, true)) { value = true; return true; }
            if (MatchesRaw(text, false)) { value = false; return true; }
        }
        else if (type.IsEnum)
        {
            var values = Enum.GetValues(type).Cast<object>().ToArray();
            if (TryGetSingleTranslatedMatch(text, values, out value))
                return true;
            foreach (var candidate in values)
            {
                if (MatchesRaw(text, candidate)) { value = candidate; return true; }
            }
        }

        value = null;
        return false;
    }

    private bool TryGetSingleTranslatedMatch(string text, ReadOnlySpan<object> values, out object? value)
    {
        value = null;
        foreach (var candidate in values)
        {
            if (!MatchesTranslated(text, candidate))
                continue;
            if (value is not null)
            {
                value = null;
                return false; // ambiguous translation; let the parent converter decide.
            }
            value = candidate;
        }
        return value is not null;
    }

    private bool MatchesTranslated(string text, object value)
    {
        var translated = GetLocalizedText(value);
        return !string.IsNullOrEmpty(translated) && string.Equals(text, translated, StringComparison.CurrentCultureIgnoreCase);
    }

    private static bool MatchesRaw(string text, object value)
        => string.Equals(text, value.ToString(), StringComparison.OrdinalIgnoreCase);

    private string? GetLocalizedObjectText(object value, int depth)
    {
        var type = value.GetType();
        if (!ShouldLocalizeString(type) || type.IsPrimitive)
            return null;

        var typeName = type.Name;
        var prefix = Translate($"PropertyGrid.Type.{typeName}", typeName);
        if (depth >= 2)
            return prefix;

        PropertyDescriptorCollection? descriptors;
        if (parent.GetPropertiesSupported())
        {
            try { descriptors = parent.GetProperties(value); }
            catch { return prefix; }
        }
        else if (type.GetMethod("<Clone>$") is not null)
        {
            // record type: synthesize a localized summary instead of the compiler-generated ToString.
            try { descriptors = TypeDescriptor.GetProperties(value); }
            catch { return prefix; }
        }
        else if (type.GetMethod(nameof(ToString), System.Type.EmptyTypes)?.DeclaringType is { } dt && dt != typeof(object) && dt != typeof(ValueType))
        {
            return null; // has a meaningful ToString override; respect it.
        }
        else
        {
            return prefix; // plain object: show the (translated) type name instead of the namespace-qualified default.
        }
        var properties = descriptors?.Cast<PropertyDescriptor>() ?? [];
        var parts = new List<string>();
        foreach (var property in properties)
        {
            if (!property.IsBrowsable)
                continue;
            object? propertyValue;
            try { propertyValue = property.GetValue(value); }
            catch { continue; }
            if (propertyValue is null)
                continue;
            var name = TranslatePropertyName(property.Name, property.DisplayName);
            string text;
            try { text = (GetLocalizedText(propertyValue, depth + 1) ?? propertyValue).ToString() ?? string.Empty; }
            catch { continue; }
            parts.Add($"{name} = {text}");
            if (parts.Count >= 4)
                break;
        }

        return parts.Count == 0 ? prefix : $"{prefix} {{ {string.Join(", ", parts)} }}";
    }

    private string TranslatePropertyName(string name, string fallback)
        => state.Localizer.GetValueOrDefault($"PropertyGrid.{name}", fallback);

    private string Translate(string key, string fallback) => state.Localizer.GetValueOrDefault(key, fallback);
}
