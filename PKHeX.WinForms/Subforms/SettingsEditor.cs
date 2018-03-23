using System;
using System.Linq;
using System.Windows.Forms;

using PKHeX.Core;
using PKHeX.WinForms.Properties;

namespace PKHeX.WinForms
{
    public partial class SettingsEditor : Form
    {
        public SettingsEditor()
        {
            InitializeComponent();
            LoadSettings();

            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            this.CenterToForm(Parent);
        }
        private void SettingsEditor_FormClosing(object sender, FormClosingEventArgs e) => SaveSettings();

        private void LoadSettings()
        {
            var props = ReflectUtil.GetPropertiesCanWritePublicDeclared(typeof(Settings));
            foreach (var p in props)
            {
                var state = ReflectUtil.GetValue(Settings.Default, p);
                switch (state)
                {
                    case bool b:
                        var chk = GetCheckBox(p, b);
                        FLP_Settings.Controls.Add(chk);
                        FLP_Settings.SetFlowBreak(chk, true);
                        continue;
                }
            }
        }
        private void SaveSettings()
        {
            foreach (var s in FLP_Settings.Controls.OfType<Control>())
                ReflectUtil.SetValue(Settings.Default, s.Name, GetValue(s));
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
