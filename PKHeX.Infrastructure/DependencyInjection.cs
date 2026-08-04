using Microsoft.Extensions.DependencyInjection;
using PKHeX.Application.Abstractions;
using PKHeX.Application.Abstractions.LiveHex;
using PKHeX.Application.Abstractions.GiftRecords;
using PKHeX.Infrastructure.AutoLegality;
using PKHeX.Infrastructure.Configuration;
using PKHeX.Infrastructure.GiftRecords;
using PKHeX.Infrastructure.LiveHex;

namespace PKHeX.Infrastructure;

/// <summary>Registers the Infrastructure layer (non-UI drivers: file IO over Core).</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ISaveFileGateway, SaveFileService>();
        services.AddSingleton<IAppPaths, AppPaths>();
        services.AddSingleton<ISettingsStore, SettingsStore>();
        services.AddSingleton<IUpdateCheckService, GitHubUpdateCheckService>();
        services.AddSingleton<IUpdateInstaller, GitHubUpdateInstaller>();
        services.AddSingleton<IAutoLegalityService, AutoLegalityService>();
        services.AddSingleton<IConsoleConnectionFactory, SysBotConnectionFactory>();
        services.AddSingleton<ILiveHexService, LiveHexService>();
        services.AddSingleton<ILivingDexService, LivingDexService>();
        services.AddSingleton<ISaveBackupService, SaveBackupService>();
        services.AddSingleton<IGiftRecordProvider, GiftRecordProvider>();
        return services;
    }
}
