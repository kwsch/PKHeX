// Bring the inner-layer namespaces into scope so existing tests resolve types that migrated out of
// PKHeX.Avalonia.Services during the Clean Architecture refactor (ports, app services, infrastructure).
global using PKHeX.Application.Abstractions;
global using PKHeX.Application.Services;
global using PKHeX.Infrastructure;
