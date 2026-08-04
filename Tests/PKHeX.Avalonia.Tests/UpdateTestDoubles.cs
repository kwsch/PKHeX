using Moq;
using PKHeX.Application.Abstractions;
using PKHeX.Application.Services;
using PKHeX.Presentation.ViewModels;

namespace PKHeX.Avalonia.Tests;

/// <summary>Shared factory for a no-op <see cref="UpdateCheckCoordinator"/> in tests that don't exercise updates.</summary>
internal static class UpdateTestDoubles
{
    public static UpdateCheckCoordinator Coordinator() =>
        new(new Mock<IUpdateCheckService>().Object, new Mock<IWindowService>().Object,
            new Mock<IUpdateInstaller>().Object, new Mock<IAppLifetime>().Object,
            new AppSettings(), new FakeSettingsStore());
}
