using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

public sealed class SummaryPreviewer
{
    private readonly ToolTip ShowSet = new() { InitialDelay = 200, IsBalloon = false, AutoPopDelay = 32_767 };
    private readonly CryPlayer Cry = new();
    private readonly PokePreview Previewer = new();
    private CancellationTokenSource _source = new();
    private static HoverSettings Settings => Main.Settings.Hover;

    public void Show(Control pb, PKM pk, StorageSlotType type = StorageSlotType.None)
    {
        if (pk.Species == 0)
        {
            Clear();
            return;
        }

        var programLanguage = Language.GetLanguageValue(Main.Settings.Startup.Language);
        var cfg = Main.Settings.BattleTemplate;
        var settings = cfg.Hover.GetSettings(programLanguage, pk.Context);
        var localize = LegalityLocalizationSet.GetLocalization(programLanguage);
        var la = new LegalityAnalysis(pk, type);
        var ctx = LegalityLocalizationContext.Create(la, localize);

        if (Settings.HoverSlotShowPreview && Control.ModifierKeys != Keys.Alt)
        {
            UpdatePreview(pb, pk, settings, ctx);
        }
        else if (Settings.HoverSlotShowText)
        {
            var text = GetPreviewText(pk, settings);
            if (Settings.HoverSlotShowEncounter)
                text = AppendEncounterInfo(ctx, text);
            ShowSet.SetToolTip(pb, text);
        }

        if (Settings.HoverSlotPlayCry)
            Cry.PlayCry(pk, pk.Context);
    }

    private void UpdatePreview(Control pb, PKM pk, in BattleTemplateExportSettings settings, in LegalityLocalizationContext ctx)
    {
        _source.Cancel();
        _source.Dispose(); // Properly dispose the previous CancellationTokenSource
        _source = new();
        UpdatePreviewPosition(new());
        Previewer.Populate(pk, settings, ctx);
        ShowInactiveTopmost(Previewer);
    }

    private const int SW_SHOWNOACTIVATE = 4;
    private const int HWND_TOPMOST = -1;
    private const uint SWP_NOACTIVATE = 0x0010;

    #pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    private static extern bool SetWindowPos(
        int hWnd,             // Window handle
        int hWndInsertAfter,  // Placement-order handle
        int X,                // Horizontal position
        int Y,                // Vertical position
        int cx,               // Width
        int cy,               // Height
        uint uFlags);         // Window positioning flags

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool ShowWindow(nint hWnd, int nCmdShow);
    #pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    public static void ShowInactiveTopmost(Form frm)
    {
        try
        {
            ShowWindow(frm.Handle, SW_SHOWNOACTIVATE);
            SetWindowPos(frm.Handle.ToInt32(), HWND_TOPMOST,
                frm.Left, frm.Top, frm.Width, frm.Height,
                SWP_NOACTIVATE);
        }
        catch
        {
            // error handling
        }
    }

    public void UpdatePreviewPosition(Point location)
    {
        var cLoc = Cursor.Position;
        var shift = Settings.PreviewCursorShift;
        cLoc.Offset(shift);
        Previewer.Location = cLoc;
    }

    public void Show(Control pb, IEncounterInfo enc)
    {
        if (enc.Species == 0)
        {
            Clear();
            return;
        }

        if (Settings.HoverSlotShowText)
            ShowSet.SetToolTip(pb, GetPreviewText(enc, Settings.HoverSlotShowEncounterVerbose));
        if (Settings.HoverSlotPlayCry)
            Cry.PlayCry(enc, enc.Context);
    }

    public void Clear()
    {
        try
        {
            var token = _source.Token;
            Task.Run(async () =>
            {
                if (!Previewer.IsHandleCreated)
                    return; // not shown ever

                // Give a little bit of fade-out delay
                await Task.Delay(50, CancellationToken.None).ConfigureAwait(false);
                if (!token.IsCancellationRequested)
                    await Previewer.InvokeAsync(Previewer.Hide, token).ConfigureAwait(false);
            }, token).ConfigureAwait(false);
        }
        catch
        {
            // Ignore.
        }
        ShowSet.RemoveAll();
        Cry.Stop();
    }

    public static string GetPreviewText(PKM pk, BattleTemplateExportSettings settings) => ShowdownParsing.GetLocalizedPreviewText(pk, settings);

    public static string AppendEncounterInfo(LegalityLocalizationContext la, string text)
    {
        var result = new List<string>(8) { text };
        if (text.Length != 0) // add a blank line between the set and the encounter info if isn't already a blank line
            result.Add(string.Empty);
        LegalityFormatting.AddEncounterInfo(la, result);
        return string.Join(Environment.NewLine, result);
    }

    private static string GetPreviewText(IEncounterInfo enc, bool verbose = false)
    {
        var lines = enc.GetTextLines(verbose, Main.CurrentLanguage);
        return string.Join(Environment.NewLine, lines);
    }
}
