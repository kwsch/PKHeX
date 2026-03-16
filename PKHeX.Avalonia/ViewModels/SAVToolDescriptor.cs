using System;
using Avalonia.Controls;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels;

/// <summary>
/// Describes a SAV tool that can be dynamically shown based on save file type.
/// </summary>
public sealed class SAVToolDescriptor
{
    public string Name { get; }
    public Func<SaveFile, bool> IsAvailable { get; }
    public Func<SaveFile, (object ViewModel, Window View)> CreateEditor { get; }
    public bool ReloadsSlots { get; init; }

    public SAVToolDescriptor(string name, Func<SaveFile, bool> isAvailable,
        Func<SaveFile, (object, Window)> createEditor)
    {
        Name = name;
        IsAvailable = isAvailable;
        CreateEditor = createEditor;
    }
}
