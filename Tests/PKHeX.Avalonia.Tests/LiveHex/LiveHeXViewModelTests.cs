using System.Threading;
using System.Threading.Tasks;
using Moq;
using PKHeX.Application.Abstractions;
using PKHeX.Application.Abstractions.LiveHex;
using PKHeX.Core;
using PKHeX.Presentation.ViewModels;

namespace PKHeX.Avalonia.Tests.LiveHex;

/// <summary>
/// Verifies the LiveHeX ViewModel's write-gating: a write to the console must never happen without
/// explicit user confirmation.
/// </summary>
public class LiveHeXViewModelTests
{
    private static (LiveHeXViewModel Vm, Mock<ILiveHexService> Live, Mock<IDialogService> Dialog) NewVm()
    {
        var live = new Mock<ILiveHexService>();
        live.Setup(s => s.GetSupport(It.IsAny<SaveFile>()))
            .Returns(new LiveHexGameSupport(true, "Sword/Shield", "Supported firmware: 1.3.2"));
        var dialog = new Mock<IDialogService>();

        var vm = new LiveHeXViewModel(new SAV8SWSH(), live.Object, dialog.Object,
            getCurrentBox: () => 0, onBoxUpdated: () => { })
        {
            IsConnected = true,
        };
        return (vm, live, dialog);
    }

    [Fact]
    public async Task WriteBox_does_not_write_when_confirmation_is_declined()
    {
        var (vm, live, dialog) = NewVm();
        dialog.Setup(d => d.ShowConfirmationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
              .ReturnsAsync(false);

        await vm.WriteBoxCommand.ExecuteAsync(null);

        live.Verify(s => s.WriteBoxAsync(It.IsAny<SaveFile>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.Equal("Write cancelled.", vm.Status);
    }

    [Fact]
    public async Task WriteBox_writes_after_confirmation()
    {
        var (vm, live, dialog) = NewVm();
        dialog.Setup(d => d.ShowConfirmationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
              .ReturnsAsync(true);
        live.Setup(s => s.WriteBoxAsync(It.IsAny<SaveFile>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await vm.WriteBoxCommand.ExecuteAsync(null);

        live.Verify(s => s.WriteBoxAsync(It.IsAny<SaveFile>(), 0, It.IsAny<CancellationToken>()), Times.Once);
        dialog.Verify(d => d.ShowConfirmationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ReadBox_does_not_require_confirmation()
    {
        var (vm, live, dialog) = NewVm();
        live.Setup(s => s.ReadBoxAsync(It.IsAny<SaveFile>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await vm.ReadBoxCommand.ExecuteAsync(null);

        live.Verify(s => s.ReadBoxAsync(It.IsAny<SaveFile>(), 0, It.IsAny<CancellationToken>()), Times.Once);
        dialog.Verify(d => d.ShowConfirmationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Unsupported_save_disables_connect()
    {
        var live = new Mock<ILiveHexService>();
        live.Setup(s => s.GetSupport(It.IsAny<SaveFile>()))
            .Returns(new LiveHexGameSupport(false, "Gen 7", "Not supported."));
        var vm = new LiveHeXViewModel(new SAV7SM(), live.Object, new Mock<IDialogService>().Object,
            () => 0, () => { });

        Assert.False(vm.IsGameSupported);
        Assert.False(vm.ConnectCommand.CanExecute(null));
    }
}
