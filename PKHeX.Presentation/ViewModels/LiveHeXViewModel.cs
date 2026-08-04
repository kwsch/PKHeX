using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Application.Abstractions;
using PKHeX.Application.Abstractions.LiveHex;
using PKHeX.Core;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// LiveHeX tool window: connect to a hacked console over Wi-Fi (sys-botbase TCP) and read/write the
/// currently displayed box. Framework-free; all network IO is delegated to <see cref="ILiveHexService"/>
/// and all writes are gated behind an explicit confirmation dialog.
/// </summary>
public partial class LiveHeXViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly ILiveHexService _liveHex;
    private readonly IDialogService _dialogs;
    private readonly Func<int> _getCurrentBox;
    private readonly Action _onBoxUpdated;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    private string _ipAddress = "192.168.0.1";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    private int _port = 6000;

    [ObservableProperty] private string _status;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    [NotifyCanExecuteChangedFor(nameof(DisconnectCommand))]
    [NotifyCanExecuteChangedFor(nameof(ReadBoxCommand))]
    [NotifyCanExecuteChangedFor(nameof(WriteBoxCommand))]
    private bool _isConnected;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    [NotifyCanExecuteChangedFor(nameof(DisconnectCommand))]
    [NotifyCanExecuteChangedFor(nameof(ReadBoxCommand))]
    [NotifyCanExecuteChangedFor(nameof(WriteBoxCommand))]
    private bool _isBusy;

    [ObservableProperty] private string _sessionInfo = string.Empty;

    public LiveHeXViewModel(
        SaveFile sav,
        ILiveHexService liveHex,
        IDialogService dialogs,
        Func<int> getCurrentBox,
        Action onBoxUpdated)
    {
        _sav = sav;
        _liveHex = liveHex;
        _dialogs = dialogs;
        _getCurrentBox = getCurrentBox;
        _onBoxUpdated = onBoxUpdated;

        var support = _liveHex.GetSupport(sav);
        GameName = support.GameName;
        IsGameSupported = support.Supported;
        SupportDetail = support.Detail;
        _status = support.Supported
            ? LocalizedStrings.Instance["LiveHeX_Ready"]
            : LocalizedStrings.Instance.Format("LiveHeX_NotSupported", support.GameName);
    }

    /// <summary>Loaded game's display name.</summary>
    public string GameName { get; }

    /// <summary>Whether LiveHeX supports the loaded save type.</summary>
    public bool IsGameSupported { get; }

    /// <summary>Support note (supported firmware versions, or why unsupported).</summary>
    public string SupportDetail { get; }

    /// <summary>Static support matrix shown in the tool window.</summary>
    public string SupportMatrix => LocalizedStrings.Instance["LiveHeX_SupportMatrixText"];

    private bool CanConnect => IsGameSupported && !IsConnected && !IsBusy
                               && !string.IsNullOrWhiteSpace(IpAddress) && Port is > 0 and <= 65535;
    private bool CanDisconnect => IsConnected && !IsBusy;
    private bool CanTransfer => IsConnected && !IsBusy;

    [RelayCommand(CanExecute = nameof(CanConnect))]
    private async Task ConnectAsync()
    {
        IsBusy = true;
        Status = LocalizedStrings.Instance.Format("LiveHeX_Connecting", IpAddress, Port);
        try
        {
            await _liveHex.ConnectAsync(IpAddress, Port, _sav);
            IsConnected = _liveHex.IsConnected;
            if (_liveHex.Session is { } s)
            {
                SessionInfo = LocalizedStrings.Instance.Format("LiveHeX_SessionInfo", s.ProfileLabel, s.TitleId, s.BotbaseVersion);
                Status = LocalizedStrings.Instance.Format("LiveHeX_ConnectedTo", s.ProfileLabel);
            }
            else
            {
                Status = LocalizedStrings.Instance["LiveHeX_Connected"];
            }
        }
        catch (LiveHexConnectionException ex)
        {
            IsConnected = false;
            SessionInfo = string.Empty;
            Status = LocalizedStrings.Instance.Format("LiveHeX_ConnectionFailed", ex.Message);
        }
        catch (Exception ex)
        {
            IsConnected = false;
            SessionInfo = string.Empty;
            Status = LocalizedStrings.Instance.Format("LiveHeX_UnexpectedError", ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanDisconnect))]
    private async Task DisconnectAsync()
    {
        IsBusy = true;
        try
        {
            await _liveHex.DisconnectAsync();
        }
        finally
        {
            IsConnected = false;
            SessionInfo = string.Empty;
            Status = LocalizedStrings.Instance["LiveHeX_Disconnected"];
            IsBusy = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanTransfer))]
    private async Task ReadBoxAsync()
    {
        var box = _getCurrentBox();
        IsBusy = true;
        Status = LocalizedStrings.Instance.Format("LiveHeX_ReadingBox", box + 1);
        try
        {
            await _liveHex.ReadBoxAsync(_sav, box);
            _onBoxUpdated();
            Status = LocalizedStrings.Instance.Format("LiveHeX_ReadBox", box + 1);
        }
        catch (LiveHexConnectionException ex)
        {
            Status = LocalizedStrings.Instance.Format("LiveHeX_ReadFailed", ex.Message);
        }
        catch (Exception ex)
        {
            Status = LocalizedStrings.Instance.Format("LiveHeX_UnexpectedError", ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanTransfer))]
    private async Task WriteBoxAsync()
    {
        var box = _getCurrentBox();

        // HARD REQUIREMENT: never write to the console without explicit user confirmation.
        var confirmed = await _dialogs.ShowConfirmationAsync(
            LocalizedStrings.Instance["LiveHeX_WriteConfirmTitle"],
            LocalizedStrings.Instance.Format("LiveHeX_WriteConfirmMessage", box + 1, GameName),
            confirmText: LocalizedStrings.Instance["LiveHeX_WriteConfirmText"],
            cancelText: LocalizedStrings.Instance["Common_Cancel"]);

        if (!confirmed)
        {
            Status = LocalizedStrings.Instance["LiveHeX_WriteCancelled"];
            return;
        }

        IsBusy = true;
        Status = LocalizedStrings.Instance.Format("LiveHeX_WritingBox", box + 1);
        try
        {
            await _liveHex.WriteBoxAsync(_sav, box);
            Status = LocalizedStrings.Instance.Format("LiveHeX_WroteBox", box + 1);
        }
        catch (LiveHexConnectionException ex)
        {
            Status = LocalizedStrings.Instance.Format("LiveHeX_WriteFailed", ex.Message);
        }
        catch (Exception ex)
        {
            Status = LocalizedStrings.Instance.Format("LiveHeX_UnexpectedError", ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
