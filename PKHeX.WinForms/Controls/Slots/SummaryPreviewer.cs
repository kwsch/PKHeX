using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.WinForms.Controls
{
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
                Cry.PlayCry(pk, pk.Format);
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
                var lines = GetTextLines(enc);
                var text = string.Join(Environment.NewLine, lines);
                ShowSet.SetToolTip(pb, text);
            }

            if (Main.Settings.Hover.HoverSlotPlayCry)
                Cry.PlayCry(enc, enc.Generation);
        }

        public static IEnumerable<string> GetTextLines(IEncounterInfo enc)
        {
            var lines = new List<string>();
            var str = GameInfo.Strings.Species;
            var name = (uint) enc.Species < str.Count ? str[enc.Species] : enc.Species.ToString();
            var EncounterName = $"{(enc is IEncounterable ie ? ie.LongName : "Special")} ({name})";
            lines.Add(string.Format(L_FEncounterType_0, EncounterName));
            if (enc is MysteryGift mg)
            {
                lines.AddRange(mg.GetDescription());
            }
            else if (enc is IMoveset m)
            {
                var nonzero = m.Moves.Where(z => z != 0).ToList();
                if (nonzero.Count != 0)
                    lines.Add(string.Join(" / ", nonzero.Select(z => GameInfo.Strings.Move[z])));
            }

            var el = enc as ILocation;
            var loc = el?.GetEncounterLocation(enc.Generation, (int) enc.Version);
            if (!string.IsNullOrEmpty(loc))
                lines.Add(string.Format(L_F0_1, "Location", loc));
            lines.Add(string.Format(L_F0_1, nameof(GameVersion), enc.Version));
            lines.Add(enc.LevelMin == enc.LevelMax
                ? $"Level: {enc.LevelMin}"
                : $"Level: {enc.LevelMin}-{enc.LevelMax}");

#if DEBUG
            // Record types! Can get a nice summary.
            // Won't work neatly for Mystery Gift types since those aren't record types.
            if (enc is not MysteryGift)
            {
                // ReSharper disable once ConstantNullCoalescingCondition
                var raw = enc.ToString() ?? throw new ArgumentNullException(nameof(enc));
                lines.AddRange(raw.Split(',', '}', '{'));
            }
#endif
            return lines;
        }

        public void Clear()
        {
            ShowSet.RemoveAll();
            Cry.Stop();
        }
    }
}
