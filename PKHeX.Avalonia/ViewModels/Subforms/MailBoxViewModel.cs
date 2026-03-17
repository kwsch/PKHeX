using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single mail entry.
/// </summary>
public partial class MailEntryModel : ObservableObject
{
    public int Index { get; }
    public string Label { get; }
    public bool IsParty { get; }

    [ObservableProperty]
    private string _authorName = string.Empty;

    [ObservableProperty]
    private ushort _authorTid;

    [ObservableProperty]
    private int _mailType;

    [ObservableProperty]
    private bool _isEmpty;

    public MailEntryModel(int index, MailDetail mail, bool isParty)
    {
        Index = index;
        IsParty = isParty;
        Label = isParty ? $"Party Mail {index + 1}" : $"PC Mail {index + 1}";

        _authorName = mail.AuthorName;
        _authorTid = mail.AuthorTID;
        _mailType = mail.MailType;
        _isEmpty = mail.IsEmpty == true;
    }
}

/// <summary>
/// ViewModel for the Mail Box editor (Gen 2-5).
/// Displays and edits mail stored in the save file.
/// </summary>
public partial class MailBoxViewModel : SaveEditorViewModelBase
{
    private readonly MailDetail[] _mails;
    private readonly int _partyCount;

    [ObservableProperty]
    private int _selectedIndex = -1;

    [ObservableProperty]
    private string _selectedAuthorName = string.Empty;

    [ObservableProperty]
    private ushort _selectedAuthorTid;

    [ObservableProperty]
    private int _selectedMailType;

    public ObservableCollection<MailEntryModel> MailEntries { get; } = [];
    public string WindowTitle { get; }

    public MailBoxViewModel(SaveFile sav, MailDetail[] mails, int partyCount) : base(sav)
    {
        _mails = mails;
        _partyCount = partyCount;
        WindowTitle = $"Mail Box ({sav.Version})";

        for (int i = 0; i < mails.Length; i++)
        {
            bool isParty = i < partyCount;
            MailEntries.Add(new MailEntryModel(isParty ? i : i - partyCount, mails[i], isParty));
        }

        if (MailEntries.Count > 0)
            SelectedIndex = 0;
    }

    partial void OnSelectedIndexChanged(int oldValue, int newValue)
    {
        // Flush previous selection back to the mail array
        if (oldValue >= 0 && oldValue < _mails.Length)
        {
            _mails[oldValue].AuthorName = SelectedAuthorName;
            _mails[oldValue].AuthorTID = SelectedAuthorTid;
            _mails[oldValue].MailType = SelectedMailType;
        }

        // Load new selection
        if (newValue < 0 || newValue >= _mails.Length)
            return;

        var mail = _mails[newValue];
        SelectedAuthorName = mail.AuthorName;
        SelectedAuthorTid = mail.AuthorTID;
        SelectedMailType = mail.MailType;
    }

    [RelayCommand]
    private void ClearMail()
    {
        if (SelectedIndex < 0 || SelectedIndex >= _mails.Length)
            return;

        _mails[SelectedIndex].SetBlank();
        var entry = MailEntries[SelectedIndex];
        entry.AuthorName = string.Empty;
        entry.AuthorTid = 0;
        entry.MailType = 0;
        entry.IsEmpty = true;

        SelectedAuthorName = string.Empty;
        SelectedAuthorTid = 0;
        SelectedMailType = 0;
    }

    [RelayCommand]
    private void Save()
    {
        // Apply current edits to the selected mail
        if (SelectedIndex >= 0 && SelectedIndex < _mails.Length)
        {
            _mails[SelectedIndex].AuthorName = SelectedAuthorName;
            _mails[SelectedIndex].AuthorTID = SelectedAuthorTid;
            _mails[SelectedIndex].MailType = SelectedMailType;
        }

        // Copy all mails back to save
        foreach (var mail in _mails)
            mail.CopyTo(SAV);

        Modified = true;
    }
}
