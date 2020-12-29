using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Properties;

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

        public void Clear() => ShowSet.RemoveAll();
    }
}