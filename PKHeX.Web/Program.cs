using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PKHeX.Web;
using PKHeX.Web.Interop;
using PKHeX.Web.Services.Sprites;
using PKHeX.Web.State;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped<BrowserFileService>();
builder.Services.AddScoped<WorkspaceState>();
// The browser's clock and time zone (the runtime reads the zone from the browser), used to stamp edited download names in local time.
builder.Services.AddSingleton(TimeProvider.System);
// Same-origin only: the app fetches nothing but its own published files.
builder.Services.AddSingleton(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
// A singleton, so the instance loaded below is the one the components are given (they resolve from a scope the host creates later).
builder.Services.AddSingleton<SpriteCatalog>();

var host = builder.Build();
// The sprite atlas is loaded before the app renders, so it is resident before a save can be chosen and no request depends on a save.
await host.Services.GetRequiredService<SpriteCatalog>().LoadAsync();
await host.RunAsync();
