using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SettingsEditor : Form
{
    public bool BlankChanged { get; private set; }

    // Remember the last settings tab for the remainder of the session.
    private static string? _last;

    private readonly List<SettingItem> _settingsPages = [];

    public SettingsEditor(object obj)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        LoadSettings(obj);

        if (obj is PKHeXSettings s)
        {
            static bool IsInvalidSaveFileVersion(GameVersion value) => value is 0 or GameVersion.GO or GameVersion.CP;
            CB_Blank.InitializeBinding();
            CB_Blank.DataSource = GameInfo.Sources.VersionDataSource.Where(z => !IsInvalidSaveFileVersion((GameVersion)z.Value)).ToList();
            CB_Blank.SelectedValue = (int)s.Startup.DefaultSaveVersion;
            CB_Blank.SelectedValueChanged += (_, _) =>
            {
                var index = WinFormsUtil.GetIndex(CB_Blank);
                var version = (GameVersion)index;
                if (IsInvalidSaveFileVersion(version))
                    return;
                s.Startup.DefaultSaveVersion = version;
            };
            CB_Blank.SelectedIndexChanged += (_, _) => BlankChanged = !IsInvalidSaveFileVersion((GameVersion)WinFormsUtil.GetIndex(CB_Blank));
            B_Reset.Click += (_, _) => DeleteSettings();
        }
        else
        {
            FLP_Blank.Visible = false;
            B_Reset.Visible = false;
        }

        // Set the split container width based on the longest tab name, with a minimum width of 120 pixels.
        var longestTabName = _settingsPages.Max(z => TextRenderer.MeasureText(z.Name, LB_Tabs.Font).Width);
        splitContainer1.SplitterDistance = Math.Max(longestTabName + SystemInformation.VerticalScrollBarWidth + 2, 120);

        LB_Tabs.DisplayMember = nameof(SettingItem.Name);
        LB_Tabs.ValueMember = nameof(SettingItem.Item);
        LB_Tabs.DataSource = _settingsPages;

        if (_last is not null && _settingsPages.Find(z => z.Name == _last) is { } find)
            LB_Tabs.SelectedItem = find;

        this.CenterToForm(FindForm());
    }

    private void LoadSettings(object obj)
    {
        var type = obj.GetType();
        var props = ReflectUtil.GetPropertiesCanWritePublicDeclared(type);
        foreach (var p in props)
        {
            var state = ReflectUtil.GetValue(obj, p);
            if (state is null)
                continue;

            var key = WinFormsTranslator.GetKey(nameof(SettingsEditor), p);
            var text = WinFormsTranslator.TranslateText(key, p, Main.CurrentLanguage);
            _settingsPages.Add(new SettingItem { Name = text, Item = state });
        }

        _settingsPages.Sort(static (a, b) => string.Compare(a.Name, b.Name, StringComparison.CurrentCulture));
        foreach (var page in _settingsPages)
            LB_Tabs.Items.Add(page.Name);
    }

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
            var path = Program.PathConfig;
            if (File.Exists(path))
                File.Delete(path);
            System.Diagnostics.Process.Start(Application.ExecutablePath);
            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            WinFormsUtil.Error("Failed to delete settings.", ex.Message);
        }
    }

    private void LB_Tabs_SelectedIndexChanged(object sender, EventArgs e)
    {
        var state = LB_Tabs.SelectedItem;
        if (state is not SettingItem { Item: { } obj } item)
        {
            _last = null;
            PG_Editor.Visible = false;
            return;
        }

        PropertyGridLocalization.Apply(PG_Editor, obj, Main.CurrentLanguage);
        PG_Editor.ExpandAllGridItems();
        _last = item.Name;
        PG_Editor.Visible = true;
    }

    private class SettingItem
    {
        public required string Name { get; init; }
        public required object Item { get; init; }
    }
}
