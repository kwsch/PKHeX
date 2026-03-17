using System;
using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the trash byte editor, which allows editing the raw bytes
/// underlying a PKM name field (Nickname, OT Name, or Handling Trainer Name).
/// </summary>
public partial class TrashEditorViewModel : ObservableObject
{
    private readonly IStringConverter _converter;
    private readonly byte[] _raw;
    private bool _editing;

    /// <summary>Whether the user accepted the changes (OK was pressed).</summary>
    public bool Accepted { get; set; }

    /// <summary>The final edited string value.</summary>
    public string FinalString { get; private set; }

    /// <summary>The final edited raw bytes.</summary>
    public byte[] FinalBytes { get; private set; }

    /// <summary>Title shown in the editor window.</summary>
    public string Title { get; }

    /// <summary>Max character length for the text field.</summary>
    public int MaxLength { get; }

    /// <summary>The decoded text value, synced bidirectionally with hex bytes.</summary>
    [ObservableProperty]
    private string _text = string.Empty;

    /// <summary>The hex byte entries for the trash region.</summary>
    public ObservableCollection<TrashByteEntry> Bytes { get; } = [];

    /// <summary>Species combo items for the trash layer applier.</summary>
    public ComboItem[] SpeciesSource { get; }

    /// <summary>Language combo items for the trash layer applier.</summary>
    public ComboItem[] LanguageSource { get; }

    [ObservableProperty]
    private int _selectedSpeciesIndex;

    [ObservableProperty]
    private int _selectedLanguageIndex;

    [ObservableProperty]
    private byte _generation;

    public TrashEditorViewModel(string title, string currentText, Span<byte> trashBytes,
        IStringConverter converter, byte generation, int maxLength)
    {
        _converter = converter;
        Title = title;
        MaxLength = maxLength;
        Generation = generation;

        FinalString = currentText;

        if (trashBytes.Length > 0)
        {
            _raw = trashBytes.ToArray();
            FinalBytes = _raw;
            for (int i = 0; i < _raw.Length; i++)
            {
                var entry = new TrashByteEntry(i, _raw[i]);
                entry.ValueChanged += OnByteValueChanged;
                Bytes.Add(entry);
            }
        }
        else
        {
            _raw = [];
            FinalBytes = [];
        }

        // Species/language sources for trash layer applier
        SpeciesSource = [.. GameInfo.FilteredSources.Species];
        LanguageSource = [.. GameInfo.FilteredSources.Languages];

        _text = currentText;
    }

    partial void OnTextChanged(string value)
    {
        if (_editing)
            return;
        _editing = true;
        try
        {
            if (_raw.Length == 0)
                return;
            ReadOnlySpan<byte> data = SetString(value);
            if (data.Length > _raw.Length)
                data = data[.._raw.Length];
            data.CopyTo(_raw);
            for (int i = 0; i < _raw.Length; i++)
                Bytes[i].SetValueSilent(_raw[i]);
        }
        finally
        {
            _editing = false;
        }
    }

    private void OnByteValueChanged(int index, byte newValue)
    {
        if (_editing)
            return;
        _editing = true;
        try
        {
            _raw[index] = newValue;
            Text = _converter.GetString(_raw);
        }
        finally
        {
            _editing = false;
        }
    }

    [RelayCommand]
    private void ApplyTrash()
    {
        if (_raw.Length == 0)
            return;

        var speciesItem = SelectedSpeciesIndex >= 0 && SelectedSpeciesIndex < SpeciesSource.Length
            ? SpeciesSource[SelectedSpeciesIndex]
            : null;
        var langItem = SelectedLanguageIndex >= 0 && SelectedLanguageIndex < LanguageSource.Length
            ? LanguageSource[SelectedLanguageIndex]
            : null;

        if (speciesItem is null || langItem is null)
            return;

        var species = (ushort)speciesItem.Value;
        var language = langItem.Value;
        var text = SpeciesName.GetSpeciesNameGeneration(species, language, Generation);
        if (string.IsNullOrEmpty(text))
            text = speciesItem.Text;

        ReadOnlySpan<byte> data = SetString(text);
        ReadOnlySpan<byte> current = SetString(Text);

        if (data.Length <= current.Length || data.Length > Bytes.Count)
            return;

        _editing = true;
        try
        {
            for (int i = current.Length; i < data.Length; i++)
            {
                _raw[i] = data[i];
                Bytes[i].SetValueSilent(data[i]);
            }
        }
        finally
        {
            _editing = false;
        }
    }

    [RelayCommand]
    private void ClearTrash()
    {
        if (_raw.Length == 0)
            return;

        ReadOnlySpan<byte> current = SetString(Text);
        _editing = true;
        try
        {
            for (int i = current.Length; i < Bytes.Count; i++)
            {
                _raw[i] = 0;
                Bytes[i].SetValueSilent(0);
            }
        }
        finally
        {
            _editing = false;
        }
    }

    /// <summary>Commits the current state as the final result.</summary>
    public void Accept()
    {
        FinalString = Text;
        if (_raw.Length > 0)
            FinalBytes = (byte[])_raw.Clone();
        Accepted = true;
    }

    private byte[] SetString(ReadOnlySpan<char> text)
    {
        Span<byte> temp = stackalloc byte[_raw.Length];
        var written = _converter.SetString(temp, text, text.Length, StringConverterOption.None);
        return temp[..written].ToArray();
    }
}

/// <summary>
/// Represents a single byte in the trash byte editor grid.
/// </summary>
public partial class TrashByteEntry : ObservableObject
{
    public int Index { get; }

    /// <summary>Event raised when the value changes due to user editing.</summary>
    public event Action<int, byte>? ValueChanged;

    [ObservableProperty]
    private string _hexValue;

    /// <summary>Label like "$00", "$01", etc.</summary>
    public string Label { get; }

    public TrashByteEntry(int index, byte value)
    {
        Index = index;
        Label = $"${index:X2}";
        _hexValue = value.ToString("X2");
    }

    partial void OnHexValueChanged(string value)
    {
        if (byte.TryParse(value, NumberStyles.HexNumber, null, out var b))
            ValueChanged?.Invoke(Index, b);
    }

    /// <summary>Sets the display value without triggering the ValueChanged event.</summary>
    public void SetValueSilent(byte value)
    {
        _hexValue = value.ToString("X2");
        OnPropertyChanged(nameof(HexValue));
    }
}
