using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using PKHeX.Core;

#if !DEBUG
using System.Reflection;
using System.IO;
using System.Threading;
#endif

namespace PKHeX.WinForms;

internal static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
#if !DEBUG
        // Add the event handler for handling UI thread exceptions to the event.
        Application.ThreadException += UIThreadException;

        // Set the unhandled exception mode to force all Windows Forms errors to go through our handler.
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

        // Add the event handler for handling non-UI thread exceptions to the event.
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif
        // Run the application
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        var splash = new SplashScreen();
        new Task(() => splash.ShowDialog()).Start();
        new Task(() => EncounterEvent.RefreshMGDB(WinForms.Main.MGDatabasePath)).Start();
        var main = new Main();
        splash.BeginInvoke(splash.ForceClose);
        Application.Run(main);
    }

    // Pipelines build can sometimes tack on text to the version code. Strip it out.
    public static readonly Version CurrentVersion = Version.Parse(GetSaneVersionTag(Application.ProductVersion));

    private static ReadOnlySpan<char> GetSaneVersionTag(ReadOnlySpan<char> productVersion)
    {
        // Take only 0-9 and '.', stop on first char not in that set.
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

    // Handle the UI exceptions by showing a dialog box, and asking the user if they wish to abort execution.
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

        // Exits the program when the user clicks Abort.
        if (result == DialogResult.Abort)
            Application.Exit();
    }

    private static string GetErrorMessage(Exception e)
    {
        return IsPluginError<IPlugin>(e, out var pluginName)
            ? $"An error occurred in a PKHeX plugin. Please report this error to the plugin author/maintainer.\n{pluginName}"
            : "An error occurred in PKHeX. Please report this error to the PKHeX author.";
    }

    // Handle the UI exceptions by showing a dialog box, and asking the user if they wish to abort execution.
    // NOTE: This exception cannot be kept from terminating the application - it can only
    // log the event, and inform the user about it.
    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var ex = e.ExceptionObject as Exception;
        try
        {
            if (IsOldPkhexCorePresent(ex))
            {
                Error("You have upgraded PKHeX incorrectly. Please delete PKHeX.Core.dll.");
            }
            else if (ex != null)
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
        // Check the stacktrace to see if the namespace is a type that derives from IPlugin
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
            // We've failed to even save the error details to a file. There's nothing else we can do.
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

    /// <summary>
    /// Attempt to log exceptions to a file when there's an error displaying exception details.
    /// </summary>
    /// <param name="originalException"></param>
    /// <param name="errorHandlingException"></param>
    private static bool EmergencyErrorLog(Exception? originalException, Exception errorHandlingException)
    {
        try
        {
            // Not using a string builder because something's very wrong, and we don't want to make things worse
            var message = (originalException?.ToString() ?? "null first exception") + Environment.NewLine + errorHandlingException;
            File.WriteAllText($"PKHeX_Error_Report {DateTime.Now:yyyyMMddHHmmss}.txt", message);
        }
        catch (Exception)
        {
            // We've failed to save the error details twice now. There's nothing else we can do.
            return false;
        }
        return true;
    }

    private static bool IsOldPkhexCorePresent(Exception? ex)
    {
        return ex is MissingMethodException or TypeLoadException or TypeInitializationException
            && File.Exists("PKHeX.Core.dll")
            && AssemblyName.GetAssemblyName("PKHeX.Core.dll").Version < CurrentVersion;
    }
#endif
}
