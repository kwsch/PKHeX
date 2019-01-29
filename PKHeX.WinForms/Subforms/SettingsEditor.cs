using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using PKHeX.Core;
using PKHeX.WinForms.Properties;

namespace PKHeX.WinForms
{
    public partial class SettingsEditor : Form
    {
        public SettingsEditor(object obj, params string[] blacklist)
        {
            InitializeComponent();
            SettingsObject = obj ?? Settings.Default;
            LoadSettings(blacklist);

            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            // reorder checkboxes
            int ctr = 0;
            foreach (var c in FLP_Settings.Controls.OfType<CheckBox>().OrderBy(z => z.Text).ToList())
                FLP_Settings.Controls.SetChildIndex(c, ctr++);

            if (obj is Settings s)
            {
                var noSelectVersions = new[] {GameVersion.GO};
                CB_Blank.InitializeBinding();
                CB_Blank.DataSource = GameInfo.VersionDataSource.Where(z => !noSelectVersions.Contains((GameVersion)z.Value)).ToList();
                CB_Blank.SelectedValue = (int) s.DefaultSaveVersion;
                CB_Blank.SelectedValueChanged += (_, __) => s.DefaultSaveVersion = (GameVersion)WinFormsUtil.GetIndex(CB_Blank);
            }
            else
            {
                FLP_Blank.Visible = false;
            }

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

        private static CheckBox GetCheckBox(string name, bool state) => new CheckBox
        {
            Name = name, Checked = state, Text = name,
            AutoSize = true,
        };

        private static object GetValue(IDisposable control)
        {
            switch (control)
            {
                case CheckBox cb:
                    return cb.Checked;
                default: return null;
            }
        }
    }
}
