using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using PKHeX.Core;
using PKHeX.WinForms.Properties;

namespace PKHeX.WinForms
{
    public partial class SettingsEditor : Form
    {
        public SettingsEditor(object? obj, params string[] blacklist)
        {
            InitializeComponent();
            SettingsObject = obj ?? Settings.Default;
            LoadSettings(blacklist);

            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            // reorder checkboxes
            var checkboxes = FLP_Settings.Controls.OfType<CheckBox>().OrderBy(z => z.Text).ToList();
            var ctr = 0;
            foreach (var c in checkboxes)
                FLP_Settings.Controls.SetChildIndex(c, ctr++);

            if (obj is Settings s)
            {
                var noSelectVersions = new[] {GameVersion.GO};
                CB_Blank.InitializeBinding();
                CB_Blank.DataSource = GameInfo.VersionDataSource.Where(z => !noSelectVersions.Contains((GameVersion)z.Value)).ToList();
                CB_Blank.SelectedValue = (int) s.DefaultSaveVersion;
                CB_Blank.SelectedValueChanged += (_, __) => s.DefaultSaveVersion = (GameVersion)WinFormsUtil.GetIndex(CB_Blank);
                B_Reset.Click += (x, e) => DeleteSettings();
            }
            else
            {
                FLP_Blank.Visible = false;
                B_Reset.Visible = false;
            }

            PG_Color.SelectedObject = Main.Draw;
            this.CenterToForm(FindForm());
        }

        private void SettingsEditor_FormClosing(object sender, FormClosingEventArgs e) => SaveSettings();

        private readonly object SettingsObject;

        private void LoadSettings(IReadOnlyList<string> blacklist)
        {
            var type = SettingsObject.GetType();
            var props = ReflectUtil.GetPropertiesCanWritePublicDeclared(type);
            if (ModifierKeys != Keys.Control)
                props = props.Except(blacklist);
            foreach (var p in props)
            {
                var state = ReflectUtil.GetValue(Settings.Default, p);
                switch (state)
                {
                    case bool b:
                        var chk = GetCheckBox(p, b);
                        FLP_Settings.Controls.Add(chk);
                        FLP_Settings.SetFlowBreak(chk, true);
                        if (blacklist.Contains(p))
                            chk.ForeColor = Color.Red;
                        continue;
                }
            }
        }

        private void SaveSettings()
        {
            foreach (var s in FLP_Settings.Controls.OfType<Control>())
                ReflectUtil.SetValue(SettingsObject, s.Name, GetValue(s));
        }

        private static CheckBox GetCheckBox(string name, bool state) => new()
        {
            Name = name, Checked = state, Text = name,
            AutoSize = true,
        };

        private static object GetValue(IDisposable control) => control switch
        {
            CheckBox cb => cb.Checked,
            _ => throw new Exception(nameof(control)),
        };

        private void SettingsEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W && ModifierKeys == Keys.Control)
                Close();
        }

        private static void DeleteSettings()
        {
            try
            {
                var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Resetting settings requires the program to exit.", MessageStrings.MsgContinue);
                if (dr != DialogResult.Yes)
                    return;
                var path = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
                if (File.Exists(path))
                    File.Delete(path);
                System.Diagnostics.Process.Start(Application.ExecutablePath);
                Environment.Exit(0);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                WinFormsUtil.Error("Failed to delete settings.", ex.Message);
            }
        }
    }
}
