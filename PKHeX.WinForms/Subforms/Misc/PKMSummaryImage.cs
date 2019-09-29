using System.Drawing;
using PKHeX.Core;
using PKHeX.Drawing;

namespace PKHeX.WinForms
{
    /// <summary>
    /// Bindable summary object that can fetch sprite and strings that summarize a <see cref="PKM"/>.
    /// </summary>
    public class PKMSummaryImage : PKMSummary
    {
        public Image Sprite => pkm.Sprite();

        public PKMSummaryImage(PKM p, GameStrings strings) : base(p, strings)
        {
        }
    }
}