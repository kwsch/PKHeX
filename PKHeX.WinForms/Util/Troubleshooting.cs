using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public static class Troubleshooting
{
    public static void AddTroubleshootingControls(ToolStripDropDownItem item, List<IPlugin> plugins)
    {
        item.DropDownItems.Add(GetSaveHandlerTroubleshooter(Keys.L));
        item.DropDownItems.Add(GetHexImporter(Keys.I));
        item.DropDownItems.Add(GetPluginInfo(Keys.U, plugins));
    }

    private static ToolStripMenuItem GetSaveHandlerTroubleshooter(Keys key)
    {
        var item = GetHiddenMenu(key);
        item.Click += (_, _) => OpenSaveHandlerTroubleshooter();
        return item;
    }

    private static ToolStripMenuItem GetHexImporter(Keys key)
    {
        var item = GetHiddenMenu(key);
        item.Click += (_, _) => OpenFileFromClipboardHex();
        return item;
    }

    private static ToolStripMenuItem GetPluginInfo(Keys key, List<IPlugin> plugins)
    {
        var item = GetHiddenMenu(key);
        item.Click += (_, _) => DisplayPluginList(plugins);
        return item;
    }

    private static ToolStripMenuItem GetHiddenMenu(Keys key) => new()
    {
        ShortcutKeys = Keys.Control | Keys.Alt | key,
        Visible = false,
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
            WinFormsUtil.Alert("Clipboard is empty.");
            return;
        }

        try
        {
            var data = Convert.FromHexString(hex.Replace(" ", ""));
            Application.OpenForms.OfType<Main>().First().OpenFile(data, string.Empty, string.Empty);
        }
        catch (FormatException)
        {
            WinFormsUtil.Alert("Clipboard does not contain valid hex data.");
        }
    }

    private static void DisplayPluginList(List<IPlugin> plugins)
    {
        var text = new StringBuilder();

        text.AppendLine($"Loaded {plugins.Count} plugins:");
        if (plugins.Count == 0)
        {
            text.AppendLine("None.");
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
