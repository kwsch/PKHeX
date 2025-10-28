using System;
using static PKHeX.Core.Ball;
using static PKHeX.Core.PersonalColor;

namespace PKHeX.Core;

public static class PersonalColorUtil
{
    public static PersonalColor GetColor(PKM pk)
    {
        // Gen1/2 don't store color in personal info
        if (pk.Format < 3)
            return (PersonalColor)PersonalTable.USUM[pk.Species, 0].Color;
        return (PersonalColor)pk.PersonalInfo.Color;
    }

    public static PersonalColor GetColor(IEncounterTemplate enc)
    {
        // Gen1/2 don't store color in personal info
        if (enc.Generation < 3)
            return (PersonalColor)PersonalTable.USUM[enc.Species, 0].Color;

        var pt = GameData.GetPersonal(enc.Version);
        var pi = pt[enc.Species, enc.Form];
        return (PersonalColor)pi.Color;
    }

    public static ReadOnlySpan<Ball> GetPreferredByColor(IEncounterTemplate enc) => GetPreferredByColor(enc, GetColor(enc));

    public static ReadOnlySpan<Ball> GetPreferredByColor<T>(T enc, PersonalColor color) where T : IContext
    {
        if (enc.Context is EntityContext.Gen8a)
            return GetPreferredByColorLA(color);
        return GetPreferredByColor(color);
    }

    /// <summary>
    /// Priority Match ball IDs that match the color ID
    /// </summary>
    public static ReadOnlySpan<Ball> GetPreferredByColor(PersonalColor color) => color switch
    {
        Red => [Repeat, Fast, Heal, Great, Dream, Lure],
        Blue => [Dive, Net, Great, Lure, Beast],
        Yellow => [Level, Ultra, Repeat, Quick, Moon],
        Green => [Safari, Friend, Nest, Dusk],
        Black => [Luxury, Heavy, Ultra, Moon, Net, Beast],
        Brown => [Level, Heavy],
        Purple => [Master, Love, Heal, Dream],
        Gray => [Heavy, Premier, Luxury],
        White => [Premier, Timer, Luxury, Ultra],
        _ => [Love, Heal, Dream],
    };

    public static ReadOnlySpan<Ball> GetPreferredByColorLA(PersonalColor color) => color switch
    {
        Red => [LAPoke],
        Blue => [LAFeather, LAGreat, LAJet],
        Yellow => [LAUltra],
        Green => [LAPoke],
        Black => [LAGigaton, LALeaden, LAHeavy, LAUltra],
        Brown => [LAPoke],
        Purple => [LAPoke],
        Gray => [LAGigaton, LALeaden, LAHeavy],
        White => [LAWing, LAJet],
        _ => [LAPoke],
    };
}
