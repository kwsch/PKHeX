using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PKHeX.Core;

#if !DEBUG
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
#endif

namespace PKHeX.WinForms;

internal static class Program
{
    // Pipelines build can sometimes tack on text to the version code. Strip it out.
    public static readonly Version CurrentVersion = Version.Parse(GetSaneVersionTag(Application.ProductVersion));

    public static readonly string WorkingDirectory = Path.GetDirectoryName(Environment.ProcessPath) ?? "";
    public const string ConfigFileName = "cfg.json";
    public static string PathConfig => Path.Combine(WorkingDirectory, ConfigFileName);

    /// <summary>
    /// Global settings instance, loaded before any forms are created.
    /// </summary>
    public static PKHeXSettings Settings { get; }

    public static bool HaX { get; private set; }
    static Program()
    {
#if !DEBUG
        Application.ThreadException += UIThreadException;
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Settings = PKHeXSettings.GetSettings(PathConfig);

        if (Settings.Startup.DarkMode)
            Application.SetColorMode(SystemColorMode.Dark);
    }

    [STAThread]
    private static void Main()
    {
        // Load settings first
        var settings = Settings;
        settings.LocalResources.SetLocalPath(WorkingDirectory);
        StartupUtil.ReloadSettings(settings);

        SplashScreen? splash = null;
        if (!settings.Startup.SkipSplashScreen)
        {
            // Show splash screen on a dedicated STA thread so it can pump its own message loop.
            var splashThread = new Thread(() =>
            {
                splash = new SplashScreen();
                Application.Run(splash);
            })
            { IsBackground = true };
            splashThread.SetApartmentState(ApartmentState.STA);
            splashThread.Start();
        }

        var args = Environment.GetCommandLineArgs();
        // Prepare initial values for the main form.
        var startup = StartupUtil.GetStartup(args, settings);
        var init = StartupUtil.FormLoadInitialActions(args, settings, CurrentVersion);
        HaX = init.HaX;
        var main = new Main();

        // Close splash when Main is ready to display, then perform startup animation.
        main.Shown += (_, _) =>
        {
            Task.Run(() => splash?.BeginInvoke(splash.ForceClose));
            main.Activate();

            // Follow-up: display popups if needed.
            if (init.HaX)
                main.WarnBehavior();
            else if (init.ShowChangelog)
                main.ShowAboutDialog(AboutPage.Changelog);
            else if (init.BackupPrompt)
                main.PromptBackup(settings.LocalResources.GetBackupPath());

            Task.Run(main.CheckForUpdates);
        };

        // Setup complete.
        if (Settings.Startup.PluginLoadEnable)
            main.AttachPlugins();
        main.LoadInitialFiles(startup);
        Application.Run(main);
    }

    private static ReadOnlySpan<char> GetSaneVersionTag(ReadOnlySpan<char> productVersion)
    {
        for (int i = 0; i < productVersion.Length; i++)
        {
            char c = productVersion[i];
            if (c == '.')
                continue;
            if (char.IsNumber(c))
                continue;
            return productVersion[..i];
        }
        return productVersion;
    }

#if !DEBUG
    private static void Error(string msg) => MessageBox.Show(msg, "PKHeX Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);

    private static void UIThreadException(object sender, ThreadExceptionEventArgs t)
    {
        DialogResult result = DialogResult.Cancel;
        try
        {
            var e = t.Exception;
            string errorMessage = GetErrorMessage(e);
            result = ErrorWindow.ShowErrorDialog(errorMessage, e, true);
        }
        catch (Exception reportingException)
        {
            HandleReportingException(t.Exception, reportingException);
        }

        if (result == DialogResult.Abort)
            Application.Exit();
    }

    private static string GetErrorMessage(Exception e)
    {
        try
        {
            if (IsPluginError<IPlugin>(e, out var pluginName))
                return $"An error occurred in a PKHeX plugin. Please report this error to the plugin author/maintainer.\n{pluginName}";
        }
        catch { }
        return "An error occurred in PKHeX. Please report this error to the PKHeX author.";
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var ex = e.ExceptionObject as Exception;
        try
        {
            if (IsOldPkhexCorePresent(ex))
            {
                Error("You have upgraded PKHeX incorrectly. Please delete PKHeX.Core.dll.");
            }
            else if (IsPkhexCoreMissing(ex))
            {
                Error("You have installed PKHeX incorrectly. Please ensure you have unzipped all files before running.");
            }
            else if (ex is not null)
            {
                var msg = GetErrorMessage(ex);
                ErrorWindow.ShowErrorDialog($"{msg}\nPKHeX must now close.", ex, false);
            }
            else
            {
                Error("A fatal non-UI error has occurred in PKHeX, and the details could not be displayed.  Please report this to the author.");
            }
        }
        catch (Exception reportingException)
        {
            HandleReportingException(ex, reportingException);
        }
    }

    private static bool IsPluginError<T>(Exception exception, out string pluginName)
    {
        pluginName = string.Empty;
        var stackTrace = new System.Diagnostics.StackTrace(exception);
        foreach (var frame in stackTrace.GetFrames())
        {
            var method = frame.GetMethod();
            var type = method?.DeclaringType;
            if (!typeof(T).IsAssignableFrom(type))
                continue;
            pluginName = type.Namespace ?? string.Empty;
            return true;
        }
        return false;
    }

    private static void HandleReportingException(Exception? ex, Exception reportingException)
    {
        try
        {
            EmergencyErrorLog(ex, reportingException);
        }
        catch
        {
        }
        if (reportingException is FileNotFoundException x && x.FileName?.StartsWith("PKHeX.Core") == true)
        {
            Error("Could not locate PKHeX.Core.dll. Make sure you're running PKHeX together with its code library. Usually caused when all files are not extracted.");
            return;
        }
        try
        {
            Error("A fatal non-UI error has occurred in PKHeX, and there was a problem displaying the details.  Please report this to the author.");
        }
        finally
        {
            Application.Exit();
        }
    }

    private static bool EmergencyErrorLog(Exception? originalException, Exception errorHandlingException)
    {
        try
        {
            var message = (originalException?.ToString() ?? "null first exception") + Environment.NewLine + errorHandlingException;
            File.WriteAllText($"PKHeX_Error_Report {DateTime.Now:yyyyMMddHHmmss}.txt", message);
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }

    private static bool IsOldPkhexCorePresent([NotNullWhen(true)] Exception? ex)
    {
        return ex is MissingMethodException or TypeLoadException or TypeInitializationException
            && File.Exists("PKHeX.Core.dll")
            && AssemblyName.GetAssemblyName("PKHeX.Core.dll").Version < CurrentVersion;
    }

    private static bool IsPkhexCoreMissing([NotNullWhen(true)] Exception? ex)
    {
        return ex is FileNotFoundException { FileName: {} n } && n.Contains("PKHeX.Core");
    }
#endif
}
