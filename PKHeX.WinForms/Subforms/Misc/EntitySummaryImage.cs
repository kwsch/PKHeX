using System.Drawing;
using PKHeX.Core;
using PKHeX.Drawing;

namespace PKHeX.WinForms
{
    /// <summary>
    /// Bind-able summary object that can fetch sprite and strings that summarize a <see cref="PKM"/>.
    /// </summary>
    public sealed class EntitySummaryImage : EntitySummary
    {
        public Image Sprite => pkm.Sprite();

        public EntitySummaryImage(PKM p, GameStrings strings) : base(p, strings)
        {
        }
    }
}
