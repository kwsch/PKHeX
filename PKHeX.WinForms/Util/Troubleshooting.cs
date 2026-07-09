using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public static class Troubleshooting
{
    public static void AddTroubleshootingControls(ToolStripDropDownItem item, List<IPlugin> plugins, bool visible)
    {
        if (visible)
        {
            // Sub-group all controls rather than dumping them in directly.
            const string name = "Menu_Troubleshooting";
            var parent = new ToolStripMenuItem
            {
                Name = name,
                Text = name,
                Visible = true,
                Image = Properties.Resources.settings,
            };
            item.DropDownItems.Add(parent);
            item = parent;
        }

        var saveHandlerItem = GetSaveHandlerTroubleshooter("Menu_ForceLoadSAV", Keys.L);
        saveHandlerItem.Image = Properties.Resources.main;
        item.DropDownItems.Add(saveHandlerItem);

        var hexImporterItem = GetHexImporter("Menu_HexImporter", Keys.I);
        hexImporterItem.Image = Properties.Resources.database;
        item.DropDownItems.Add(hexImporterItem);

        var pluginInfoItem = GetPluginInfo("Menu_PluginInfo", Keys.U, plugins);
        pluginInfoItem.Image = Properties.Resources.about;
        item.DropDownItems.Add(pluginInfoItem);
    }

    private static ToolStripMenuItem GetSaveHandlerTroubleshooter(string name, Keys key)
    {
        var item = GetMenu(name, key);
        item.Click += (_, _) => OpenSaveHandlerTroubleshooter();
        return item;
    }

    private static ToolStripMenuItem GetHexImporter(string name, Keys key)
    {
        var item = GetMenu(name, key);
        item.Click += (_, _) => OpenFileFromClipboardHex();
        return item;
    }

    private static ToolStripMenuItem GetPluginInfo(string name, Keys key, List<IPlugin> plugins)
    {
        var item = GetMenu(name, key);
        item.Click += (_, _) => DisplayPluginList(plugins);
        return item;
    }

    private static ToolStripMenuItem GetMenu(string name, Keys key) => new()
    {
        Name = name,
        Text = name, // will be replaced by localization, but set to something for design time
        ShortcutKeys = Keys.Control | Keys.Alt | key,
    };

    private static void OpenSaveHandlerTroubleshooter()
    {
        var main = Application.OpenForms.OfType<Main>().FirstOrDefault();
        if (main is null)
            return;

        using var form = new SaveHandlerTroubleshooter(main);
        form.ShowDialog(main);
    }

    private static void OpenFileFromClipboardHex()
    {
        var hex = Clipboard.GetText().Trim();
        if (string.IsNullOrEmpty(hex))
        {
            WinFormsUtil.Alert(MessageStrings.MsgTroubleshootingClipboardEmpty);
            return;
        }

        try
        {
            var data = Convert.FromHexString(hex.Replace(" ", ""));
            Application.OpenForms.OfType<Main>().First().OpenFile(data, string.Empty, string.Empty);
        }
        catch (FormatException)
        {
            WinFormsUtil.Alert(MessageStrings.MsgTroubleshootingClipboardInvalidHex);
        }
    }

    private static void DisplayPluginList(List<IPlugin> plugins)
    {
        var text = new StringBuilder();

        text.AppendLine(string.Format(MessageStrings.MsgTroubleshootingPluginListHeader, plugins.Count));
        if (plugins.Count == 0)
        {
            text.AppendLine(MessageStrings.MsgTroubleshootingPluginListEmpty);
            WinFormsUtil.Alert(text.ToString());
            return;
        }

        List<(IPlugin Plugin, string Group)> loaded = [];
        foreach (var plugin in plugins)
        {
            var assembly = plugin.GetType().Assembly;
            var fullName = assembly.FullName;
            if (fullName != null)
            {
                var culture = fullName.IndexOf("Culture", StringComparison.Ordinal);
                if (culture != -1)
                    fullName = fullName[..(culture - 2)];
                if (fullName.EndsWith(".0", StringComparison.Ordinal))
                    fullName = fullName[..^2];
            }

            loaded.Add(new(plugin, fullName ?? "Unknown"));
        }

        foreach (var group in loaded.GroupBy(z => z.Group).OrderBy(z => z.Key))
        {
            text.AppendLine(group.Key);
            foreach (var plugin in group.OrderBy(z => z.Plugin.Name))
                text.AppendLine($"- {plugin.Plugin.Name}");
        }

        WinFormsUtil.Alert(text.ToString());
    }
}
