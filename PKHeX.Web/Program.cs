using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PKHeX.Web;
using PKHeX.Web.Interop;
using PKHeX.Web.State;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped<BrowserFileService>();
builder.Services.AddScoped<WorkspaceState>();
await builder.Build().RunAsync();
