using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class BoxExporter : Form
{
    private readonly SaveFile SAV;
    private readonly IFileNamer<PKM>[] Namers = [.. EntityFileNamer.AvailableNamers];

    private BoxExportSettings Settings
    {
        get => (BoxExportSettings)PG_Settings.SelectedObject;
        init => PG_Settings.SelectedObject = value;
    }

    public BoxExporter(SaveFile sav, ExportOverride eo = ExportOverride.None)
    {
        InitializeComponent();
        Icon = Properties.Resources.Icon;
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = sav;
        var obj = Main.Settings.SlotExport;
        var settings = obj.BoxExport;
        if (eo != 0)
            settings = settings with { Scope = eo == ExportOverride.All ? BoxExportScope.All : BoxExportScope.Current };
        Settings = settings;

        int index = 0;
        for (var i = 0; i < Namers.Length; i++)
        {
            var name = Namers[i].Name;
            CB_Namer.Items.Add(name);
            if (name == obj.DefaultBoxExportNamer)
                index = i;
        }
        CB_Namer.SelectedIndex = index;
    }

    private void B_Export_Click(object sender, EventArgs e)
    {
        if (!TryGetFolder(out var folder))
            return;

        var namer = GetSelectedNamer();
        var settings = Settings;
        int ctr = BoxExport.Export(SAV, folder, namer, settings);
        if (settings.Notify == BoxExportNofify.Silent)
        {
            System.Media.SystemSounds.Asterisk.Play();
            return;
        }

        if (ctr < 0)
        {
            WinFormsUtil.Error(MessageStrings.MsgSaveBoxExportInvalid);
            return;
        }
        var result = string.Format(MessageStrings.MsgSaveBoxExportPathCount, ctr) + Environment.NewLine + folder;
        WinFormsUtil.Alert(result);
    }

    private IFileNamer<PKM> GetSelectedNamer() => Namers[CB_Namer.SelectedIndex];

    private bool TryGetFolder([NotNullWhen(true)] out string? folder)
    {
        using var fbd = new FolderBrowserDialog();
        fbd.Description = "Select a folder to export the boxes to.";
        fbd.ShowNewFolderButton = true;
        var result = fbd.ShowDialog(this);
        folder = fbd.SelectedPath;
        return result == DialogResult.OK;
    }

    private void BoxExporter_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (e.CloseReason != CloseReason.UserClosing)
            return;

        var obj = Main.Settings.SlotExport;
        obj.DefaultBoxExportNamer = GetSelectedNamer().Name;
        obj.BoxExport = Settings;
    }

    public enum ExportOverride
    {
        None = 0,
        All = 1,
        Current = 2,
    }
}
