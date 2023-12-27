using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.WinForms;

/// <summary>
/// Bind-able summary object that can fetch sprite and strings that summarize a <see cref="PKM"/>.
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public sealed class EntitySummaryImage(PKM p, GameStrings strings, string Position) : EntitySummary(p, strings)
{
    public Image Sprite => pk.Sprite();
    public override string Position { get; } = Position;
}
