using System.Diagnostics;

namespace PKHeX.WinForms.Properties
{
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    internal sealed partial class Settings
    {
        private Settings()
        {
            SettingChanging += SettingChangingEventHandler;
            SettingsSaving += SettingsSavingEventHandler;
        }

        private static void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            Debug.WriteLine($"Changed setting: {e.SettingName}");
            // Add code to handle the SettingChangingEvent event here.
        }

        private static void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
            Debug.WriteLine("Saving settings...");
        }
    }
}
