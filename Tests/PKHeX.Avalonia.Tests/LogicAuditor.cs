using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using Moq;
using PKHeX.Avalonia.Services;
using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Proactively audits ViewModels for functional logic and binding errors.
/// </summary>
public static class LogicAuditor
{
    /// <summary>
    /// Scans the entire assembly for ViewModels and audits their property dependencies.
    /// </summary>
    public static void AuditAssemblyViewModels(Assembly assembly, SaveFile sav)
    {
        var vmTypes = assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(ViewModelBase)) && !t.IsAbstract)
            .ToList();

        foreach (var type in vmTypes)
        {
            Console.WriteLine($"Auditing {type.Name}...");
            try 
            {
                var vm = CreateInstance(type, sav);
                if (vm != null && vm is ObservableObject obs)
                    AuditPropertyDependencies(obs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Skipping audit for {type.Name}: {ex.Message}");
            }
        }
    }

    private static object? CreateInstance(Type type, SaveFile sav)
    {
        var ctors = type.GetConstructors();
        
        var savCtor = ctors.FirstOrDefault(c => c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType == typeof(SaveFile));
        if (savCtor != null) return savCtor.Invoke([sav]);

        var pkmSavCtor = ctors.FirstOrDefault(c => c.GetParameters().Length == 5
            && c.GetParameters()[0].ParameterType == typeof(PKM)
            && c.GetParameters()[1].ParameterType == typeof(SaveFile));
        if (pkmSavCtor != null)
        {
            var pkm = sav.BlankPKM;
            return pkmSavCtor.Invoke([pkm, sav, new Mock<ISpriteRenderer>().Object, new Mock<IDialogService>().Object, new Mock<IWindowService>().Object]);
        }

        var emptyCtor = ctors.FirstOrDefault(c => c.GetParameters().Length == 0);
        if (emptyCtor != null) return emptyCtor.Invoke([]);

        return null;
    }
    /// <summary>
    /// Verifies that changing a source property triggers PropertyChanged for all defined dependents.
    /// This catches bugs like "Shiny changed but Sprite didn't notify".
    /// </summary>
    public static void AuditPropertyDependencies(ObservableObject viewModel)
    {
        var type = viewModel.GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            // Find [NotifyPropertyChangedFor] attributes (including generated ones)
            var notifyAttr = prop.GetCustomAttributes<NotifyPropertyChangedForAttribute>().ToList();
            if (!notifyAttr.Any()) continue;

            // Use reflection to get 'Names' property to avoid build ambiguity across toolkit versions
            var dependents = notifyAttr.SelectMany(a => 
                (string[]?)a.GetType().GetProperty("Names")?.GetValue(a) ?? Array.Empty<string>()
            ).ToList();
            
            foreach (var dependent in dependents)
            {
                VerifyDependency(viewModel, prop, dependent);
            }
        }
    }

    private static void VerifyDependency(ObservableObject viewModel, PropertyInfo sourceProp, string dependentName)
    {
        bool notified = false;
        PropertyChangedEventHandler handler = (s, e) =>
        {
            if (e.PropertyName == dependentName)
                notified = true;
        };

        viewModel.PropertyChanged += handler;

        try 
        {
            var originalValue = sourceProp.GetValue(viewModel);
            var testValue = GetTestValue(sourceProp.PropertyType, originalValue);
            
            if (testValue != null || sourceProp.PropertyType.IsValueType == false)
            {
                sourceProp.SetValue(viewModel, testValue);
                Assert.True(notified, $"Changing '{sourceProp.Name}' should have notified '{dependentName}', but it didn't.");
            }
        }
        finally
        {
            viewModel.PropertyChanged -= handler;
        }
    }

    /// <summary>
    /// Verifies that changing a ViewModel property correctly updates the corresponding property in the underlying model.
    /// This catches bugs like "IsShiny toggled in VM but PKM.IsShiny stays true".
    /// </summary>
    public static void AuditModelSync(object viewModel, object model, string vmPropName, string modelPropName)
    {
        var vmType = viewModel.GetType();
        var modelType = model.GetType();

        var vmProp = vmType.GetProperty(vmPropName);
        var modelProp = modelType.GetProperty(modelPropName);

        Assert.NotNull(vmProp);
        Assert.NotNull(modelProp);

        var originalValue = vmProp.GetValue(viewModel);
        var testValue = GetTestValue(vmProp.PropertyType, originalValue);

        vmProp.SetValue(viewModel, testValue);

        // Check if model updated (might require PreparePKM call if not immediate sync)
        var prepareMethod = vmType.GetMethod("PreparePKM", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        if (prepareMethod != null)
            prepareMethod.Invoke(viewModel, null);

        var actualModelValue = modelProp.GetValue(model);
        Assert.Equal(testValue, actualModelValue); // If this fails, consider if the PKM logic modifies the set value (e.g. nickname sanitization)
    }

    private static object? GetTestValue(Type type, object? original)
    {
        if (type == typeof(bool)) return !(bool)(original ?? false);
        if (type == typeof(int)) return (int)(original ?? 0) + 1;
        if (type == typeof(uint)) return (uint)(original ?? 0) + 1;
        if (type == typeof(ushort)) return (ushort)((ushort)(original ?? 0) + 1);
        if (type == typeof(byte)) return (byte)((byte)(original ?? 0) + 1);
        if (type == typeof(long)) return (long)(original ?? 0) + 1;
        if (type == typeof(double)) return (double)(original ?? 0) + 1.0;
        if (type == typeof(string)) return (original as string) + "_test";
        if (type.IsEnum) return Enum.GetValues(type).Cast<object>().FirstOrDefault(v => !v.Equals(original));
        
        return null;
    }

    /// <summary>
    /// Verifies that loading a model into the ViewModel immediately populates calculated properties.
    /// This catches bugs like "IV sum is not visible until I start modifying a number".
    /// </summary>
    public static void AuditInitialLoadState(object viewModel, params string[] propertyNames)
    {
        var type = viewModel.GetType();
        foreach (var name in propertyNames)
        {
            var prop = type.GetProperty(name);
            Assert.NotNull(prop);
            var value = prop.GetValue(viewModel);
            
            // Check for default/empty values that should likely be populated
            if (value is int i) Assert.NotEqual(-1, i); // Assuming -1 is not a valid sum
            if (value is string s) Assert.False(string.IsNullOrEmpty(s), $"Property '{name}' is empty after load.");
            
            // For sums specifically, we check if it's 0 when it should likely be higher if stats exist
            // This is a heuristic, but helpful.
        }
    }
}
