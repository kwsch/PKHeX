using CommunityToolkit.Mvvm.ComponentModel;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels;

/// <summary>
/// Base ViewModel for subforms that edit save file data.
/// </summary>
public abstract partial class SaveEditorViewModelBase : ObservableObject
{
    protected SaveFile SAV { get; }

    [ObservableProperty]
    private bool _modified;

    protected SaveEditorViewModelBase(SaveFile sav)
    {
        SAV = sav;
    }
}
