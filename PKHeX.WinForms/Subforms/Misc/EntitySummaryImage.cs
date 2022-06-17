using System.Drawing;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.WinForms;

/// <summary>
/// Bind-able summary object that can fetch sprite and strings that summarize a <see cref="PKM"/>.
/// </summary>
public sealed class EntitySummaryImage : EntitySummary
{
    public Image Sprite => pk.Sprite();
    public override string Position { get; }

    public EntitySummaryImage(PKM p, GameStrings strings, string position) : base(p, strings)
    {
        Position = position;
    }
}
