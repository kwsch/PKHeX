using Avalonia;
using Avalonia.Headless;
using Avalonia.Threading;
using PKHeX.Avalonia;

[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]

public class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp()
    {
        var builder = AppBuilder.Configure<App>();
        return Environment.GetEnvironmentVariable("PKHEX_HEADLESS_CAPTURE") == "1"
            ? builder.UseSkia()
                .WithInterFont()
                .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false })
            : builder.UseHeadless(new AvaloniaHeadlessPlatformOptions());
    }
}
