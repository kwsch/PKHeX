using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Properties;

using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.WinForms.Controls
{
    public sealed class SummaryPreviewer
    {
        private readonly ToolTip ShowSet = new() { InitialDelay = 200, IsBalloon = false };

        public void Show(Control pb, PKM pk)
        {
            if (pk.Species == 0)
            {
                Clear();
                return;
            }
            var text = ShowdownParsing.GetLocalizedPreviewText(pk, Settings.Default.Language);
            ShowSet.SetToolTip(pb, text);
        }

        public void Show(Control pb, IEncounterInfo enc)
        {
            if (enc.Species == 0)
            {
                Clear();
                return;
            }

            var lines = new List<string>();
            var str = GameInfo.Strings.Species;
            var name = (uint)enc.Species < str.Count ? str[enc.Species] : enc.Species.ToString();
            var EncounterName = $"{(enc is IEncounterable ie ? ie.LongName : "Special")} ({name})";
            lines.Add(string.Format(L_FEncounterType_0, EncounterName));
            if (enc is MysteryGift mg)
                lines.Add(mg.CardHeader);

            var el = enc as ILocation;
            var loc = el?.GetEncounterLocation(enc.Generation, (int)enc.Version);
            if (!string.IsNullOrEmpty(loc))
                lines.Add(string.Format(L_F0_1, "Location", loc));
            lines.Add(string.Format(L_F0_1, nameof(GameVersion), enc.Version));
            lines.Add(enc.LevelMin == enc.LevelMax
                ? $"Level: {enc.LevelMin}"
                : $"Level: {enc.LevelMin}-{enc.LevelMax}");

            var text = string.Join(Environment.NewLine, lines);
            ShowSet.SetToolTip(pb, text);
        }

        public void Clear() => ShowSet.RemoveAll();
    }
}
