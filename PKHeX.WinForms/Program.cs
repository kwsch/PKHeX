using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

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

            try
            {
                if (IsOnWindows())
                {
                    if (GetFrameworkVersion() >= 393295)
                    {
                        StartPKHeX();
                    }
                    else
                    {
                        // Todo: make this translatable
                        MessageBox.Show(".NET Framework 4.6 needs to be installed for this version of PKHeX to run.", "PKHeX Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        Process.Start(@"https://www.microsoft.com/download/details.aspx?id=48130");
                    }
                }
                else
                {
                    //CLR Version 4.0.30319.42000 is equivalent to .NET Framework version 4.6
                    if ((Environment.Version.CompareTo(Version.Parse("4.0.30319.42000"))) >= 0)
                    {
                        StartPKHeX();
                    }
                    else
                    {
                        MessageBox.Show("Your version of Mono needs to target the .NET Framework 4.6 or higher for this version of PKHeX to run.",
                                        "PKHeX Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                
            }
            catch (FileNotFoundException ex)
            {
                // Check whether or not the exception was from missing PKHeX.Core, rather than something else in the constructor of Main
                if (ex.TargetSite == typeof(Program).GetMethod(nameof(StartPKHeX), BindingFlags.Static | BindingFlags.NonPublic))
                {
                    // Exception came from StartPKHeX and (probably) corresponds to missing PKHeX.Core
                    MessageBox.Show("Could not locate PKHeX.Core.dll. Make sure you're running PKHeX together with its code library. Usually caused when all files are not extracted.", "PKHeX Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                // Exception came from Main
                throw;
            }
        }

        private static void StartPKHeX()
        {
            // Run the application
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }

        private static bool IsOnWindows()
        {
            // 4 -> UNIX, 6 -> Mac OSX, 128 -> UNIX (old)
            int p = (int)Environment.OSVersion.Platform;
            return p != 4 && p != 6 && p != 128;
        }

        private static int GetFrameworkVersion()
        {
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
            {
                if (ndpKey == null)
                    return 0;
                int releaseKey = (int)ndpKey.GetValue("Release");
                return releaseKey;
            }
        }

        // Handle the UI exceptions by showing a dialog box, and asking the user whether or not they wish to abort execution.
        private static void UIThreadException(object sender, ThreadExceptionEventArgs t)
        {
            DialogResult result = DialogResult.Cancel;
            try
            {
                // Todo: make this translatable
                ErrorWindow.ShowErrorDialog("An unhandled exception has occurred.\nYou can continue running PKHeX, but please report this error.", t.Exception, true);
            }
            catch (Exception reportingException)
            {
                try
                {
                    // Todo: make this translatable
                    MessageBox.Show("A fatal error has occurred in PKHeX, and there was a problem displaying the details.  Please report this to the author.", "PKHeX Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    EmergencyErrorLog(t.Exception, reportingException);
                }
                finally
                {
                    Application.Exit();
                }
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
                if (ex != null)
                {
                    // Todo: make this translatable
                    ErrorWindow.ShowErrorDialog("An unhandled exception has occurred.\nPKHeX must now close.", ex, false);
                }
                else
                {
                    MessageBox.Show("A fatal non-UI error has occurred in PKHeX, and the details could not be displayed.  Please report this to the author.", "PKHeX Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            catch (Exception reportingException)
            {
                try
                {
                    // Todo: make this translatable
                    MessageBox.Show("A fatal non-UI error has occurred in PKHeX, and there was a problem displaying the details.  Please report this to the author.", "PKHeX Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    EmergencyErrorLog(ex, reportingException);
                }
                finally
                {
                    Application.Exit();
                }
            }
        }

        /// <summary>
        /// Attempt to log exceptions to a file when there's an error displaying exception details.
        /// </summary>
        /// <param name="originalException"></param>
        /// <param name="errorHandlingException"></param>
        private static bool EmergencyErrorLog(Exception originalException, Exception errorHandlingException)
        {
            try
            {
                // Not using a string builder because something's very wrong, and we don't want to make things worse
                var message = (originalException?.ToString() ?? "null first exception") + Environment.NewLine + errorHandlingException;
                File.WriteAllText($"PKHeX_Error_Report {DateTime.Now:YYYYMMDDhhmmss}.txt", message);
            }
            catch (Exception)
            {
                // We've failed to save the error details twice now. There's nothing else we can do.
                return false;
            }
            return true;
        }
    }
}
