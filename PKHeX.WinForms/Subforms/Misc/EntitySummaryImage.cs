using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.WinForms;

/// <summary>
/// Bind-able summary object that can fetch sprite and strings that summarize a <see cref="PKM"/>.
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public sealed class EntitySummaryImage(PKM pk, GameStrings strings, string Position) : EntitySummary(pk, strings)
{
    public Image Sprite => Entity.Sprite();
    public override string Position { get; } = Position;
}
