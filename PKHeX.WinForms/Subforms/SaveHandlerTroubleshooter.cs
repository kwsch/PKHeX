using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms;

internal sealed partial class SaveHandlerTroubleshooter : Form
{
    private readonly Main _main;

    internal SaveHandlerTroubleshooter(Main main)
    {
        ArgumentNullException.ThrowIfNull(main);
        _main = main;

        InitializeComponent();
        InitializeBindings();
        CB_Type.SelectedIndexChanged += CB_Type_SelectedIndexChanged;
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        CenterToParent();
    }

    private void InitializeBindings()
    {
        CB_Type.DataSource = Enum.GetValues<SaveFileType>()
            .Where(z => z is not SaveFileType.None)
            .Select(z => new SelectionItem<SaveFileType>(z.ToString(), z))
            .ToList();

        CB_Language.DataSource = Enum.GetValues<LanguageID>()
            .Select(z => new SelectionItem<LanguageID>(z.ToString(), z))
            .ToList();

        var handlers = SaveUtil.Handlers.ToList();
        handlers.Insert(0, new SaveHandlerDefault());
        CB_Handler.DataSource = handlers
            .Select(z => new HandlerItem(GetHandlerDisplayName(z), z))
            .ToList();

        UpdateSubVersionChoices();
    }

    private void CB_Type_SelectedIndexChanged(object? sender, EventArgs e) => UpdateSubVersionChoices();

    private void UpdateSubVersionChoices()
    {
        var type = GetSelectedValue(CB_Type, SaveFileType.None);
        List<SelectionItem<GameVersion>> versions =
        [
            ..GameUtil.GameVersions
                .Where(z => z.SaveFileType == type)
                .Distinct()
                .Select(z => new SelectionItem<GameVersion>(GetGameVersionDisplayName(z), z))
        ];

        CB_SubVersion.DataSource = versions;
        CB_SubVersion.SelectedIndex = 0;
    }

    private void B_Browse_Click(object? sender, EventArgs e)
    {
        using OpenFileDialog ofd = new();
        ofd.Filter = "All Files|*.*";
        ofd.Title = MsgFileLoadSelectFileSave;

        if (!string.IsNullOrWhiteSpace(TB_Path.Text))
            ofd.FileName = TB_Path.Text;

        if (ofd.ShowDialog(this) == DialogResult.OK)
            TB_Path.Text = ofd.FileName;
    }

    private void B_Continue_Click(object? sender, EventArgs e)
    {
        var path = TB_Path.Text.Trim();
        if (string.IsNullOrWhiteSpace(path))
        {
            WinFormsUtil.Error(MsgFileLoadSelectFileSave);
            return;
        }

        if (!File.Exists(path))
        {
            WinFormsUtil.Error(MsgFileLoadFail, path);
            return;
        }

        byte[] data;
        try
        {
            data = File.ReadAllBytes(path);
        }
        catch (Exception ex)
        {
            WinFormsUtil.Error($"{MsgFileInUse}{Environment.NewLine}{path}", ex);
            return;
        }

        if (CB_Handler.SelectedItem is not HandlerItem handler)
        {
            WinFormsUtil.Error(MsgFileLoadFail);
            return;
        }

        var typeInfo = new SaveTypeInfo(
            GetSelectedValue(CB_Type, SaveFileType.None),
            GetSelectedValue(CB_SubVersion, GameVersion.Any),
            GetSelectedValue(CB_Language, LanguageID.None));

        SaveFile? sav;
        try
        {
            if (!SaveUtil.TryGetSaveFileHandler(data, out sav, path, handler.Handler, typeInfo))
            {
                WinFormsUtil.Error(MsgFileLoadSaveFail, path);
                return;
            }
        }
        catch (Exception ex)
        {
            WinFormsUtil.Error(MsgFileLoadSaveLoadFail, ex);
            return;
        }

        try
        {
            _main.OpenSAV(sav, path, forceOpen: true);
            Close();
        }
        catch (Exception ex)
        {
            WinFormsUtil.Error(MsgFileLoadSaveLoadFail, ex);
        }
    }

    private static void SaveHandlerTroubleshooter_DragEnter(object? sender, DragEventArgs e)
    {
        e.Effect = e.Data?.GetDataPresent(DataFormats.FileDrop) == true
            ? DragDropEffects.Copy
            : DragDropEffects.None;
    }

    private void SaveHandlerTroubleshooter_DragDrop(object? sender, DragEventArgs e)
    {
        if (e.Data?.GetData(DataFormats.FileDrop) is not string[] files || files.Length == 0)
            return;

        TB_Path.Text = files[0];
    }

    private static T GetSelectedValue<T>(ComboBox comboBox, T fallback) where T : struct
        => comboBox.SelectedItem is SelectionItem<T> item ? item.Value : fallback;

    private static string GetGameVersionDisplayName(GameVersion version)
    {
        var text = GameInfo.GetVersionName(version);
        return string.IsNullOrWhiteSpace(text) ? version.ToString() : text;
    }

    private static string GetHandlerDisplayName(ISaveHandler handler)
    {
        const string Prefix = "SaveHandler";
        var name = handler.GetType().Name;
        return name.StartsWith(Prefix, StringComparison.Ordinal) ? name[Prefix.Length..] : name;
    }

    private sealed record SelectionItem<T>(string Text, T Value)
    {
        public override string ToString() => Text;
    }

    private sealed record HandlerItem(string Text, ISaveHandler Handler)
    {
        public override string ToString() => Text;
    }

    private sealed class SaveHandlerDefault : ISaveHandler
    {
        public bool IsRecognized(long size) => true;

        public SaveHandlerSplitResult TrySplit(Memory<byte> input)
            => new(input, default, default, this);

        public void Finalize(Span<byte> input) { }
    }
}
