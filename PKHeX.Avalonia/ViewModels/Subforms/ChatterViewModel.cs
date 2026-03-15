using System;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Chatter (Chatot phrase) editor (Gen 4/5).
/// Edits the recorded Chatot phrase data.
/// </summary>
public partial class ChatterViewModel : SaveEditorViewModelBase
{
    private readonly IChatter _chatter;

    [ObservableProperty]
    private bool _initialized;

    [ObservableProperty]
    private int _confusionChance;

    [ObservableProperty]
    private string _statusText = string.Empty;

    public string WindowTitle { get; }

    public ChatterViewModel(SaveFile sav, IChatter chatter) : base(sav)
    {
        _chatter = chatter;
        WindowTitle = $"Chatot Chatter ({sav.Version})";
        _initialized = chatter.Initialized;
        _confusionChance = chatter.ConfusionChance;
    }

    partial void OnInitializedChanged(bool value)
    {
        _chatter.Initialized = value;
        ConfusionChance = _chatter.ConfusionChance;
    }

    /// <summary>
    /// Imports PCM data from the given file path.
    /// </summary>
    [RelayCommand]
    private void ImportPcm(string? path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            StatusText = "File not found.";
            return;
        }

        var len = new FileInfo(path).Length;
        if (len != IChatter.SIZE_PCM)
        {
            StatusText = $"Incorrect size: got {len} bytes, expected {IChatter.SIZE_PCM} bytes.";
            return;
        }

        byte[] data = File.ReadAllBytes(path);
        data.CopyTo(_chatter.Recording);
        Initialized = _chatter.Initialized = true;
        ConfusionChance = _chatter.ConfusionChance;
        StatusText = "PCM imported successfully.";
    }

    /// <summary>
    /// Exports PCM data to the given file path.
    /// </summary>
    [RelayCommand]
    private void ExportPcm(string? path)
    {
        if (string.IsNullOrEmpty(path))
            return;

        File.WriteAllBytes(path, _chatter.Recording.ToArray());
        StatusText = "PCM exported successfully.";
    }

    [RelayCommand]
    private void Save()
    {
        Modified = true;
    }
}
