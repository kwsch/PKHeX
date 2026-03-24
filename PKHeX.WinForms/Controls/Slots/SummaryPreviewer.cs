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
            if (!settings.Order.Contains(BattleTemplateToken.FirstLine))
            {
                var insert = GetPreviewText(pk, settings with { Order = [BattleTemplateToken.FirstLine] });
                text = insert + Environment.NewLine + text;
            }
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

        SetWindowState(Previewer, true);
        bool showFirst = !_isFirstShown;
        if (showFirst)
            _isFirstShown = true;
    }

    private static void SetWindowState(Form frm, bool visible)
    {
        try
        {
            const int SW_SHOWNOACTIVATE = 4;
            var state = visible ? SW_SHOWNOACTIVATE : 0;
            ShowWindowAsync(frm.Handle, state);

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            static extern bool ShowWindowAsync(nint hWnd, int nCmdShow);
        }
        catch
        {
            // error handling
        }
    }

    private bool _isFirstShown;

    public void UpdatePreviewPosition(Point location)
    {
        var cLoc = Cursor.Position;
        var shift = Settings.PreviewCursorShift;
        cLoc.Offset(shift);
        Previewer.MoveForm(cLoc.X, cLoc.Y);
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
            var token = _source.Token; // did the user move to another slot in time?
            var noToken = CancellationToken.None; // don't throw task canceled exceptions
            Task.Run(async () =>
            {
                if (!Previewer.IsHandleCreated || !_isFirstShown)
                    return; // not shown ever

                // Give a little bit of delay before hiding, assuming user is moving between slots. If they enter another, we'll cancel.

                await Task.Delay(50, noToken).ConfigureAwait(false);
                if (!token.IsCancellationRequested)
                    await Previewer.InvokeAsync(() => SetWindowState(Previewer, false), noToken); // hide
            }, noToken).ConfigureAwait(false);
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
        var result = new List<string>(8);
        if (text.Length != 0) // add a blank line between the set and the encounter info if isn't already a blank line
        {
            result.Add(text);
            result.Add(string.Empty);
        }
        LegalityFormatting.AddEncounterInfo(la, result);
        return string.Join(Environment.NewLine, result);
    }

    private static string GetPreviewText(IEncounterInfo enc, bool verbose = false)
    {
        var lines = enc.GetTextLines(verbose, Main.CurrentLanguage);
        return string.Join(Environment.NewLine, lines);
    }

    public static string AppendLegalityHint(in LegalityLocalizationContext la, string line)
    {
        // Get the first illegal check result, and append the localization of it as the hint.
        // If all legal, return the input string unchanged.
        var analysis = la.Analysis;
        if (analysis.Valid)
            return line;

        foreach (var chk in analysis.Results)
        {
            if (chk.Valid)
                continue;
            var hint = la.Humanize(chk, verbose: true);
            return Join(line, hint);
        }

        for (var i = 0; i < analysis.Info.Moves.Length; i++)
        {
            var chk = analysis.Info.Moves[i];
            if (chk.Valid)
                continue;
            var hint = la.FormatMove(chk, i + 1, la.Analysis.Info.Entity.Context);
            return Join(line, hint);
        }

        for (var i = 0; i < analysis.Info.Relearn.Length; i++)
        {
            var chk = analysis.Info.Relearn[i];
            if (chk.Valid)
                continue;
            var hint = la.FormatMove(chk, i + 1, la.Analysis.Info.Entity.Context);
            return Join(line, hint);
        }

        return line;

        static string Join(string line, string hint)
        {
            if (hint.Length > 67)
                hint = hint[..67] + "...";
            return string.IsNullOrEmpty(line) ? hint : $"{line}{Environment.NewLine}{hint}";
        }
    }
}
