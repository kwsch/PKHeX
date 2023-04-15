using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

public sealed class SummaryPreviewer
{
    private readonly ToolTip ShowSet = new() { InitialDelay = 200, IsBalloon = false, AutoPopDelay = 32_767 };
    private readonly CryPlayer Cry = new();

    public void Show(Control pb, PKM pk)
    {
        if (pk.Species == 0)
        {
            Clear();
            return;
        }

        if (Main.Settings.Hover.HoverSlotShowText)
        {
            var text = ShowdownParsing.GetLocalizedPreviewText(pk, Main.Settings.Startup.Language);
            var la = new LegalityAnalysis(pk);
            var result = new List<string> { text, string.Empty };
            LegalityFormatting.AddEncounterInfo(la, result);
            ShowSet.SetToolTip(pb, string.Join(Environment.NewLine, result));
        }

        if (Main.Settings.Hover.HoverSlotPlayCry)
            Cry.PlayCry(pk, pk.Context);
    }

    public void Show(Control pb, IEncounterInfo enc)
    {
        if (enc.Species == 0)
        {
            Clear();
            return;
        }

        if (Main.Settings.Hover.HoverSlotShowText)
        {
            var lines = enc.GetTextLines(GameInfo.Strings);
            var text = string.Join(Environment.NewLine, lines);
            ShowSet.SetToolTip(pb, text);
        }

        if (Main.Settings.Hover.HoverSlotPlayCry)
            Cry.PlayCry(enc, enc.Context);
    }

    public void Clear()
    {
        ShowSet.RemoveAll();
        Cry.Stop();
    }
}
