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

    public void Show(Control pb, PKM pk)
    {
        if (pk.Species == 0)
        {
            Clear();
            return;
        }

        var programLanguage = Language.GetLanguageValue(Main.Settings.Startup.Language);
        var cfg = Main.Settings.BattleTemplate;
        var settings = cfg.Hover.GetSettings(programLanguage, pk.Context);

        if (Settings.HoverSlotShowPreview && Control.ModifierKeys != Keys.Alt)
        {
            UpdatePreview(pb, pk, settings);
        }
        else if (Settings.HoverSlotShowText)
        {
            var text = GetPreviewText(pk, settings);
            if (Settings.HoverSlotShowEncounter)
                text = AppendEncounterInfo(new LegalityAnalysis(pk), text);
            ShowSet.SetToolTip(pb, text);
        }

        if (Settings.HoverSlotPlayCry)
            Cry.PlayCry(pk, pk.Context);
    }

    private void UpdatePreview(Control pb, PKM pk, BattleTemplateExportSettings settings)
    {
        _source.Cancel();
        _source = new();
        UpdatePreviewPosition(new());
        Previewer.Populate(pk, settings);
        ShowInactiveTopmost(Previewer);
    }

    private const int SW_SHOWNOACTIVATE = 4;
    private const int HWND_TOPMOST = -1;
    private const uint SWP_NOACTIVATE = 0x0010;

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
        var src = _source;
        Task.Run(async () =>
        {
            if (!Previewer.IsHandleCreated)
                return; // not shown ever

            // Give a little bit of fade-out delay
            await Task.Delay(50, CancellationToken.None).ConfigureAwait(false);
            if (!src.IsCancellationRequested)
                Previewer.Invoke(Previewer.Hide);
        }, src.Token);
        ShowSet.RemoveAll();
        Cry.Stop();
    }

    public static string GetPreviewText(PKM pk, BattleTemplateExportSettings settings) => ShowdownParsing.GetLocalizedPreviewText(pk, settings);

    public static string AppendEncounterInfo(LegalityAnalysis la, string text)
    {
        var result = new List<string>(8) { text };
        if (text.Length != 0) // add a blank line between the set and the encounter info if isn't already a blank line
            result.Add("");
        LegalityFormatting.AddEncounterInfo(la, result);
        return string.Join(Environment.NewLine, result);
    }

    private static string GetPreviewText(IEncounterInfo enc, bool verbose = false)
    {
        var lines = enc.GetTextLines(GameInfo.Strings, verbose);
        return string.Join(Environment.NewLine, lines);
    }
}
