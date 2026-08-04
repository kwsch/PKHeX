using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PKHeX.Application.Abstractions;

namespace PKHeX.Avalonia.Tests.Harness;

/// <summary>
/// Deterministic <see cref="IDialogService"/> stand-in for the headless harness. Never opens a real
/// picker or message box (there is no interactive user in a headless run); instead it records every
/// call and returns canned results, so a flow that would otherwise block on a dialog stays fast and
/// assertable. Configure <see cref="OpenFileResult"/>/<see cref="ConfirmResult"/> per test as needed.
/// </summary>
public sealed class RecordingDialogService : IDialogService
{
    public List<(string Title, string Message)> Errors { get; } = [];
    public List<(string Title, string Message)> Infos { get; } = [];
    public List<(string Title, string Message)> Confirmations { get; } = [];

    /// <summary>Path returned by <see cref="OpenFileAsync"/> (simulating the native picker result).</summary>
    public string? OpenFileResult { get; set; }

    /// <summary>Result returned by <see cref="ShowConfirmationAsync"/>.</summary>
    public bool ConfirmResult { get; set; }

    public Task<string?> OpenFileAsync(string title, string[]? filters = null) => Task.FromResult(OpenFileResult);
    public Task<string?> OpenFolderAsync(string title) => Task.FromResult<string?>(null);
    public Task<string?> SaveFileAsync(string title, string? defaultFileName = null, string[]? filters = null) => Task.FromResult<string?>(null);

    public Task ShowErrorAsync(string title, string message)
    {
        Errors.Add((title, message));
        return Task.CompletedTask;
    }

    public Task ShowInformationAsync(string title, string message)
    {
        Infos.Add((title, message));
        return Task.CompletedTask;
    }

    public Task<bool> ShowConfirmationAsync(string title, string message, string confirmText = "Yes", string cancelText = "Cancel")
    {
        Confirmations.Add((title, message));
        return Task.FromResult(ConfirmResult);
    }

    public void RevealInFileManager(string path) { }
    public Task<string?> GetClipboardTextAsync() => Task.FromResult<string?>(null);
    public Task SetClipboardTextAsync(string text) => Task.CompletedTask;
}

/// <summary>
/// No-op <see cref="IWindowService"/> for the headless harness: opening child/tool windows requires
/// a live desktop windowing surface that headless does not provide, so these calls are recorded and
/// ignored rather than actually shown. Tests that need to assert a dialog/tool was requested can read
/// <see cref="ShownDialogs"/>/<see cref="ShownTools"/>.
/// </summary>
public sealed class NoopWindowService : IWindowService
{
    public List<(object ViewModel, string Title)> ShownDialogs { get; } = [];
    public List<(object ViewModel, string Title)> ShownTools { get; } = [];
    public int CloseAllToolsCount { get; private set; }

    public Task ShowDialogAsync(object viewModel, string title)
    {
        ShownDialogs.Add((viewModel, title));
        return Task.CompletedTask;
    }

    public void ShowTool(object viewModel, string title) => ShownTools.Add((viewModel, title));
    public void CloseAllTools() => CloseAllToolsCount++;
}
