using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class MailBoxEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly SaveFile _clone;
    private readonly MailDetail[] _mail;
    private readonly int _partyMailCount;
    private readonly int[] _mailItemIds;
    private readonly byte _generation;
    private readonly System.Collections.Generic.IList<PKM> _party;

    [ObservableProperty]
    private ObservableCollection<MailEntryViewModel> _partyMail = [];

    [ObservableProperty]
    private ObservableCollection<MailEntryViewModel> _pcMail = [];

    [ObservableProperty]
    private MailEntryViewModel? _selectedMail;

    [ObservableProperty]
    private string _authorName = string.Empty;

    [ObservableProperty]
    private ushort _authorTid;

    [ObservableProperty]
    private ushort _authorSid;

    [ObservableProperty]
    private int _selectedMailTypeIndex;

    [ObservableProperty]
    private int _selectedLanguageIndex;

    [ObservableProperty]
    private int _selectedSpeciesIndex;

    [ObservableProperty]
    private ushort _message00;
    [ObservableProperty]
    private ushort _message01;
    [ObservableProperty]
    private ushort _message02;
    [ObservableProperty]
    private ushort _message10;
    [ObservableProperty]
    private ushort _message11;
    [ObservableProperty]
    private ushort _message12;
    [ObservableProperty]
    private ushort _message20;
    [ObservableProperty]
    private ushort _message21;
    [ObservableProperty]
    private ushort _message22;

    [ObservableProperty]
    private string _messageText1 = string.Empty;

    [ObservableProperty]
    private string _messageText2 = string.Empty;

    [ObservableProperty]
    private bool _userEntered;

    public ObservableCollection<ComboItem> MailTypes { get; } = [];
    public ObservableCollection<ComboItem> Languages { get; } = [];
    public ObservableCollection<ComboItem> Species { get; } = [];

    public bool IsGen2 => _generation == 2;
    public bool IsGen3 => _generation == 3;
    public bool IsGen4Or5 => _generation is 4 or 5;
    public bool ShowSid => _generation != 2;
    public bool ShowMessageNud => _generation != 2;
    public bool ShowMessageText => _generation == 2;

    public bool IsSupported { get; }
    public string UnsupportedMessage { get; } = string.Empty;

    public MailBoxEditorViewModel(SaveFile sav)
    {
        _sav = sav;
        _clone = sav.Clone();
        _generation = sav.Generation;
        _party = _clone.PartyData;

        // Initialize mail items based on generation
        switch (_clone)
        {
            case SAV2 sav2:
                _mail = new MailDetail[6 + 10];
                for (int i = 0; i < _mail.Length; i++)
                    _mail[i] = new Mail2(sav2, i);
                _mailItemIds = [0x9E, 0xB5, 0xB6, 0xB7, 0xB8, 0xB9, 0xBA, 0xBB, 0xBC, 0xBD];
                _partyMailCount = 6;
                IsSupported = true;
                break;

            case SAV3 sav3:
                _mail = new MailDetail[6 + 10];
                for (int i = 0; i < _mail.Length; i++)
                    _mail[i] = sav3.LargeBlock.GetMail(i);
                _mailItemIds = [121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132];
                _partyMailCount = 6;
                IsSupported = true;
                break;

            case SAV4 sav4:
                _mail = new MailDetail[_party.Count + 20];
                for (int i = 0; i < _party.Count; i++)
                    _mail[i] = new Mail4(((PK4)_party[i]).HeldMail.ToArray());
                for (int i = _party.Count, j = 0; i < _mail.Length; i++, j++)
                    _mail[i] = sav4.GetMail(j);
                _mailItemIds = [137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148];
                _partyMailCount = _party.Count;
                IsSupported = true;
                break;

            case SAV5 sav5:
                _mail = new MailDetail[_party.Count + 20];
                for (int i = 0; i < _party.Count; i++)
                    _mail[i] = new Mail5(((PK5)_party[i]).HeldMail.ToArray());
                for (int i = _party.Count, j = 0; i < _mail.Length; i++, j++)
                    _mail[i] = sav5.GetMail(j);
                _mailItemIds = [137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148];
                _partyMailCount = _party.Count;
                IsSupported = true;
                break;

            default:
                _mail = [];
                _mailItemIds = [];
                _partyMailCount = 0;
                IsSupported = false;
                UnsupportedMessage = "Mail is not supported for this save type.";
                return;
        }

        LoadComboBoxData();
        LoadMailLists();
    }

    private void LoadComboBoxData()
    {
        var filtered = GameInfo.FilteredSources;
        var source = filtered.Source;

        // Mail types
        var itemStrings = source.Strings.GetItemStrings(_clone.Context, _clone.Version);
        MailTypes.Add(new ComboItem(itemStrings[0], 0)); // None
        foreach (int item in _mailItemIds)
            MailTypes.Add(new ComboItem(itemStrings[item], item));

        // Languages
        foreach (var lang in GameInfo.LanguageDataSource(_clone.Generation, _clone.Context))
            Languages.Add(lang);

        // Species
        foreach (var spec in filtered.Species)
            Species.Add(spec);
    }

    private void LoadMailLists()
    {
        PartyMail.Clear();
        PcMail.Clear();

        for (int i = 0; i < _partyMailCount; i++)
        {
            var entry = new MailEntryViewModel(i, _mail[i], true);
            PartyMail.Add(entry);
        }

        for (int i = _partyMailCount; i < _mail.Length; i++)
        {
            var entry = new MailEntryViewModel(i, _mail[i], false);
            PcMail.Add(entry);
        }
    }

    partial void OnSelectedMailChanged(MailEntryViewModel? value)
    {
        if (value is null)
            return;

        LoadMailDetails(value.Mail);
    }

    private void LoadMailDetails(MailDetail mail)
    {
        AuthorName = mail.AuthorName;
        AuthorTid = mail.AuthorTID;
        AuthorSid = mail.AuthorSID;

        // Mail type
        var typeIndex = MailTypeToCbIndex(mail);
        SelectedMailTypeIndex = typeIndex >= 0 && typeIndex < MailTypes.Count ? typeIndex : 0;

        // Language
        var langIndex = Languages.ToList().FindIndex(l => l.Value == mail.AuthorLanguage);
        SelectedLanguageIndex = langIndex >= 0 ? langIndex : 0;

        // Species
        var species = mail.AppearPKM;
        if (_generation == 3)
            species = SpeciesConverter.GetNational3(species);
        var specIndex = Species.ToList().FindIndex(s => s.Value == species);
        SelectedSpeciesIndex = specIndex >= 0 ? specIndex : 0;

        // Messages
        if (_generation == 2)
        {
            MessageText1 = mail.GetMessage(false);
            MessageText2 = mail.GetMessage(true);
            UserEntered = mail.UserEntered;
        }
        else
        {
            Message00 = mail.GetMessage(0, 0);
            Message01 = mail.GetMessage(0, 1);
            Message02 = mail.GetMessage(0, 2);
            Message10 = mail.GetMessage(1, 0);
            Message11 = mail.GetMessage(1, 1);
            Message12 = mail.GetMessage(1, 2);
            Message20 = mail.GetMessage(2, 0);
            Message21 = mail.GetMessage(2, 1);
            Message22 = mail.GetMessage(2, 2);
        }
    }

    private void SaveMailDetails()
    {
        if (SelectedMail is null)
            return;

        var mail = SelectedMail.Mail;
        mail.AuthorName = AuthorName;
        mail.AuthorTID = AuthorTid;
        mail.AuthorSID = AuthorSid;
        mail.MailType = CbIndexToMailType(SelectedMailTypeIndex);

        if (SelectedLanguageIndex >= 0 && SelectedLanguageIndex < Languages.Count)
            mail.AuthorLanguage = (byte)Languages[SelectedLanguageIndex].Value;

        var species = SelectedSpeciesIndex >= 0 && SelectedSpeciesIndex < Species.Count
            ? (ushort)Species[SelectedSpeciesIndex].Value
            : (ushort)0;

        if (_generation == 3)
            mail.AppearPKM = SpeciesConverter.GetInternal3(species);
        else
            mail.AppearPKM = species;

        if (_generation == 2)
        {
            mail.SetMessage(MessageText1, MessageText2, UserEntered);
        }
        else
        {
            mail.SetMessage(0, 0, Message00);
            mail.SetMessage(0, 1, Message01);
            mail.SetMessage(0, 2, Message02);
            mail.SetMessage(1, 0, Message10);
            mail.SetMessage(1, 1, Message11);
            mail.SetMessage(1, 2, Message12);
            mail.SetMessage(2, 0, Message20);
            mail.SetMessage(2, 1, Message21);
            mail.SetMessage(2, 2, Message22);
        }

        SelectedMail.Refresh();
    }

    private int MailTypeToCbIndex(MailDetail mail)
    {
        if (_generation <= 3)
        {
            var idx = System.Array.IndexOf(_mailItemIds, mail.MailType);
            return idx >= 0 ? idx + 1 : 0;
        }
        return mail.IsEmpty == false ? 1 + mail.MailType : 0;
    }

    private int CbIndexToMailType(int cbIndex)
    {
        if (_generation <= 3)
            return cbIndex > 0 && cbIndex <= _mailItemIds.Length ? _mailItemIds[cbIndex - 1] : 0;
        return cbIndex > 0 ? cbIndex - 1 : 0xFF;
    }

    [RelayCommand]
    private void DeleteMail()
    {
        if (SelectedMail is null)
            return;

        SelectedMail.Mail.SetBlank();
        SelectedMail.Refresh();
        LoadMailDetails(SelectedMail.Mail);
    }

    [RelayCommand]
    private void Save()
    {
        SaveMailDetails();

        // Copy mail back to save
        switch (_generation)
        {
            case 2:
                foreach (var m in _mail)
                    m.CopyTo(_clone);
                break;
            case 3:
                foreach (var m in _mail)
                    m.CopyTo(_clone);
                break;
            case 4:
                for (int i = 0; i < _party.Count; i++)
                    _mail[i].CopyTo((PK4)_party[i]);
                for (int i = _party.Count; i < _mail.Length; i++)
                    _mail[i].CopyTo(_clone);
                break;
            case 5:
                for (int i = 0; i < _party.Count; i++)
                    _mail[i].CopyTo((PK5)_party[i]);
                for (int i = _party.Count; i < _mail.Length; i++)
                    _mail[i].CopyTo(_clone);
                break;
        }

        if (_party.Count > 0)
            _clone.PartyData = _party;

        _sav.CopyChangesFrom(_clone);
    }
}

public partial class MailEntryViewModel : ViewModelBase
{
    public int Index { get; }
    public MailDetail Mail { get; }
    public bool IsPartyMail { get; }

    [ObservableProperty]
    private string _displayText = string.Empty;

    public MailEntryViewModel(int index, MailDetail mail, bool isParty)
    {
        Index = index;
        Mail = mail;
        IsPartyMail = isParty;
        Refresh();
    }

    public void Refresh()
    {
        DisplayText = Mail.IsEmpty != true
            ? $"{Index}: From {Mail.AuthorName}"
            : $"{Index}: (empty)";
    }
}
