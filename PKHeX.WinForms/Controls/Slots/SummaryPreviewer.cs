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

        if (Settings.HoverSlotShowPreview && Control.ModifierKeys != Keys.Alt)
            UpdatePreview(pb, pk);
        else if (Settings.HoverSlotShowText)
            ShowSet.SetToolTip(pb, GetPreviewText(pk, new LegalityAnalysis(pk)));
        if (Settings.HoverSlotPlayCry)
            Cry.PlayCry(pk, pk.Context);
    }

    private void UpdatePreview(Control pb, PKM pk)
    {
        _source.Cancel();
        _source = new();
        UpdatePreviewPosition(new());
        Previewer.Populate(pk);
        Previewer.Show();
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
            ShowSet.SetToolTip(pb, GetPreviewText(enc));
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

    public static string GetPreviewText(PKM pk, LegalityAnalysis la)
    {
        var text = ShowdownParsing.GetLocalizedPreviewText(pk, Main.Settings.Startup.Language);
        if (!Main.Settings.Hover.HoverSlotShowEncounter)
            return text;
        var result = new List<string> { text, string.Empty };
        LegalityFormatting.AddEncounterInfo(la, result);
        return string.Join(Environment.NewLine, result);
    }

    private static string GetPreviewText(IEncounterInfo enc)
    {
        var lines = enc.GetTextLines(GameInfo.Strings);
        return string.Join(Environment.NewLine, lines);
    }
}
