using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single editable setting property.
/// </summary>
public partial class SettingPropertyModel : ObservableObject
{
    public string Name { get; }
    public string Category { get; }
    public Type PropertyType { get; }
    private readonly object _owner;
    private readonly PropertyInfo _prop;

    /// <summary>Type tag for UI: "bool", "int", "float", "string", "enum".</summary>
    public string TypeTag { get; }

    [ObservableProperty]
    private object? _value;

    /// <summary>Enum values if the property is an enum type.</summary>
    public ObservableCollection<object>? EnumValues { get; }

    public SettingPropertyModel(object owner, PropertyInfo prop, string category)
    {
        _owner = owner;
        _prop = prop;
        Name = prop.Name;
        Category = category;
        PropertyType = prop.PropertyType;
        _value = prop.GetValue(owner);

        if (PropertyType == typeof(bool))
            TypeTag = "bool";
        else if (PropertyType == typeof(int) || PropertyType == typeof(uint) || PropertyType == typeof(byte) || PropertyType == typeof(float))
            TypeTag = "number";
        else if (PropertyType == typeof(string))
            TypeTag = "string";
        else if (PropertyType.IsEnum)
        {
            TypeTag = "enum";
            EnumValues = new ObservableCollection<object>(Enum.GetValues(PropertyType).Cast<object>());
        }
        else
            TypeTag = "string"; // fallback
    }

    partial void OnValueChanged(object? value)
    {
        if (value is null)
            return;
        try
        {
            object converted;
            if (PropertyType == typeof(bool) && value is bool b)
                converted = b;
            else if (PropertyType == typeof(int))
                converted = Convert.ToInt32(value);
            else if (PropertyType == typeof(uint))
                converted = Convert.ToUInt32(value);
            else if (PropertyType == typeof(byte))
                converted = Convert.ToByte(value);
            else if (PropertyType == typeof(float))
                converted = Convert.ToSingle(value);
            else if (PropertyType == typeof(string))
                converted = value.ToString() ?? string.Empty;
            else if (PropertyType.IsEnum)
                converted = value;
            else
                return;
            _prop.SetValue(_owner, converted);
        }
        catch
        {
            // Ignore conversion failures
        }
    }
}

/// <summary>
/// Model for a settings category containing multiple properties.
/// </summary>
public sealed class SettingsCategoryModel
{
    public string Name { get; }
    public ObservableCollection<SettingPropertyModel> Properties { get; }

    public SettingsCategoryModel(string name, IEnumerable<SettingPropertyModel> properties)
    {
        Name = name;
        Properties = new ObservableCollection<SettingPropertyModel>(properties);
    }
}

/// <summary>
/// ViewModel for the Settings Editor subform.
/// Enumerates settings categories and their properties for editing.
/// </summary>
public partial class SettingsEditorViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _modified;

    [ObservableProperty]
    private int _selectedTabIndex;

    public ObservableCollection<SettingsCategoryModel> Categories { get; } = [];

    public SettingsEditorViewModel([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] object settings)
    {
        LoadSettings(settings);
    }

    private void LoadSettings([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] object obj)
    {
        var type = obj.GetType();
        var props = ReflectUtil.GetPropertiesCanWritePublicDeclared(type).Order();

        foreach (var propName in props)
        {
            var propInfo = type.GetProperty(propName);
            if (propInfo is null)
                continue;
            var state = propInfo.GetValue(obj);
            if (state is null)
                continue;

            // Only expand sub-objects that themselves have writable properties
            var stateType = state.GetType();
            if (stateType.IsPrimitive || stateType == typeof(string) || stateType.IsEnum)
                continue;

            var subProps = GetEditableProperties(state, propName);
            if (subProps.Count > 0)
                Categories.Add(new SettingsCategoryModel(propName, subProps));
        }
    }

    private static List<SettingPropertyModel> GetEditableProperties(object obj, string category)
    {
        var result = new List<SettingPropertyModel>();
        var type = obj.GetType();

        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!prop.CanRead || !prop.CanWrite)
                continue;
            // Only handle simple editable types
            var pt = prop.PropertyType;
            if (pt == typeof(bool) || pt == typeof(int) || pt == typeof(uint) || pt == typeof(byte) ||
                pt == typeof(float) || pt == typeof(string) || pt.IsEnum)
            {
                result.Add(new SettingPropertyModel(obj, prop, category));
            }
        }
        return result;
    }

    [RelayCommand]
    private void Save()
    {
        Modified = true;
    }
}
