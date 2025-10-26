using System;
using System.Text;
using PKHeX.Core;
using static PKHeX.Core.Species;

namespace PKHeX.Drawing.PokeSprite;

/// <summary>
/// Logic for retrieving the resource names of Pokémon and related sprites.
/// </summary>
public static class SpriteName
{
    /// <summary>
    /// Allows returning a Shiny sprite when available.
    /// </summary>
    public static bool AllowShinySprite { get; set; }

    private const char Separator = '_';
    private const char Cosplay = 'c';
    private const char Shiny = 's';
    private const char GGStarter = 'p';

    /// <summary>
    /// Gets the resource name of the <see cref="Ball"/> sprite.
    /// </summary>
    public static string GetResourceStringBall(byte ball) => $"_ball{ball}";

    /// <summary>
    /// Gets the resource name of the Pokémon sprite.
    /// </summary>
    public static string GetResourceStringSprite(ushort species, byte form, byte gender, uint formarg, EntityContext context = Latest.Context, bool shiny = false)
    {
        if (SpeciesDefaultFormSprite.Contains(species)) // Species who show their default sprite regardless of Form
            form = 0;

        // Starting in Legends: Z-A, Xerneas no longer has two forms, and it transitions to what was then-considered Active Mode when it attacks
        // The in-game icon always displays it as active, so match that behavior here
        if (species == (ushort)Xerneas && context == EntityContext.Gen9a)
            form = 1;

        var sb = new StringBuilder(12); // longest expected string result
        sb.Append(Separator).Append(species);

        if (form != 0)
        {
            sb.Append(Separator).Append(form);

            if (species == (ushort)Pikachu)
            {
                if (context == EntityContext.Gen6)
                {
                    sb.Append(Cosplay);
                }
                else if (form == 8)
                {
                    sb.Append(GGStarter);
                }
            }
            else if (species == (ushort)Eevee)
            {
                if (form == 1)
                    sb.Append(GGStarter);
            }
        }
        if (gender == 1 && SpeciesGenderedSprite.Contains(species))
        {
            sb.Append('f');
        }

        if (species == (ushort)Alcremie)
        {
            if (form == 0)
                sb.Append(Separator).Append(form);
            sb.Append(Separator).Append(formarg);
        }

        if (shiny && AllowShinySprite)
            sb.Append(Shiny);
        return sb.ToString();
    }

    /// <summary>
    /// Species that show their default Species sprite regardless of current <see cref="PKM.Form"/>
    /// </summary>
    private static ReadOnlySpan<ushort> SpeciesDefaultFormSprite =>
    [
        (ushort)Mothim,
        (ushort)Scatterbug,
        (ushort)Spewpa,
        (ushort)Rockruff,
        (ushort)Mimikyu,
        (ushort)Sinistea,
        (ushort)Polteageist,
        (ushort)Urshifu,
        (ushort)Dudunsparce,
        (ushort)Poltchageist,
        (ushort)Sinistcha,
    ];

    /// <summary>
    /// Species that show a <see cref="PKM.Gender"/> specific Sprite
    /// </summary>
    private static ReadOnlySpan<ushort> SpeciesGenderedSprite =>
    [
        (ushort)Hippopotas,
        (ushort)Hippowdon,
        (ushort)Unfezant,
        (ushort)Frillish,
        (ushort)Jellicent,
        (ushort)Pyroar,
    ];
}
