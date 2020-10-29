using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using PKHeX.Core;

#if !DEBUG
using System.Reflection;
using System.IO;
using System.Threading;
#endif

namespace PKHeX.WinForms
{
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
            splash.Invoke((MethodInvoker)(() => splash.Close()));
            Application.Run(main);
        }

#if !DEBUG
        private static void Error(string msg) => MessageBox.Show(msg, "PKHeX Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);

        // Handle the UI exceptions by showing a dialog box, and asking the user whether or not they wish to abort execution.
        private static void UIThreadException(object sender, ThreadExceptionEventArgs t)
        {
            DialogResult result = DialogResult.Cancel;
            try
            {
                result = ErrorWindow.ShowErrorDialog("An unhandled exception has occurred.\nYou can continue running PKHeX, but please report this error.", t.Exception, true);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception reportingException)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                HandleReportingException(t.Exception, reportingException);
            }

            // Exits the program when the user clicks Abort.
            if (result == DialogResult.Abort)
                Application.Exit();
        }

        // Handle the UI exceptions by showing a dialog box, and asking the user whether
        // or not they wish to abort execution.
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
                    ErrorWindow.ShowErrorDialog("An unhandled exception has occurred.\nPKHeX must now close.", ex, false);
                }
                else
                {
                    Error("A fatal non-UI error has occurred in PKHeX, and the details could not be displayed.  Please report this to the author.");
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception reportingException)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                HandleReportingException(ex, reportingException);
            }
        }

        private static void HandleReportingException(Exception? ex, Exception reportingException)
        {
            if (reportingException is FileNotFoundException x && x.FileName?.StartsWith("PKHeX.Core") == true)
            {
                Error("Could not locate PKHeX.Core.dll. Make sure you're running PKHeX together with its code library. Usually caused when all files are not extracted.");
                return;
            }
            try
            {
                Error("A fatal non-UI error has occurred in PKHeX, and there was a problem displaying the details.  Please report this to the author.");
                EmergencyErrorLog(ex, reportingException);
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
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                // We've failed to save the error details twice now. There's nothing else we can do.
                return false;
            }
            return true;
        }

        private static bool IsOldPkhexCorePresent(Exception? ex)
        {
            return ex is MissingMethodException
                && File.Exists("PKHeX.Core.dll")
                && AssemblyName.GetAssemblyName("PKHeX.Core.dll").Version < Assembly.GetExecutingAssembly().GetName().Version;
        }
#endif
    }
}
