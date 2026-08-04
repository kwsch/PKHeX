using Microsoft.Extensions.DependencyInjection;
using PKHeX.Application.Abstractions;
using PKHeX.Application.Services;

namespace PKHeX.Application;

/// <summary>Registers the Application layer (framework-free app services + ports with app-side impls).</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<LanguageService>();
        services.AddSingleton<UndoRedoService>();
        services.AddSingleton<ISlotService, SlotService>();
        // Workflow use cases (PKHeX.Application.UseCases) are stateless and constructed at call sites.
        return services;
    }
}
