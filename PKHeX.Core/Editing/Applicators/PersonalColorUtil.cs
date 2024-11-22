using System;

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

    public static ReadOnlySpan<Ball> GetPreferredByColor<T>(T enc, PersonalColor color) where T : IVersion
    {
        if (enc.Version is GameVersion.PLA)
            return GetPreferredByColorLA(color);
        return GetPreferredByColor(color);
    }

    /// <summary>
    /// Priority Match ball IDs that match the color ID
    /// </summary>
    public static ReadOnlySpan<Ball> GetPreferredByColor(PersonalColor color) => color switch
    {
        PersonalColor.Red => [Ball.Repeat, Ball.Fast, Ball.Heal, Ball.Great, Ball.Dream, Ball.Lure],
        PersonalColor.Blue => [Ball.Dive, Ball.Net, Ball.Great, Ball.Lure, Ball.Beast],
        PersonalColor.Yellow => [Ball.Level, Ball.Ultra, Ball.Repeat, Ball.Quick, Ball.Moon],
        PersonalColor.Green => [Ball.Safari, Ball.Friend, Ball.Nest, Ball.Dusk],
        PersonalColor.Black => [Ball.Luxury, Ball.Heavy, Ball.Ultra, Ball.Moon, Ball.Net, Ball.Beast],
        PersonalColor.Brown => [Ball.Level, Ball.Heavy],
        PersonalColor.Purple => [Ball.Master, Ball.Love, Ball.Heal, Ball.Dream],
        PersonalColor.Gray => [Ball.Heavy, Ball.Premier, Ball.Luxury],
        PersonalColor.White => [Ball.Premier, Ball.Timer, Ball.Luxury, Ball.Ultra],
        _ => [Ball.Love, Ball.Heal, Ball.Dream],
    };

    public static ReadOnlySpan<Ball> GetPreferredByColorLA(PersonalColor color) => color switch
    {
        PersonalColor.Red => [Ball.LAPoke],
        PersonalColor.Blue => [Ball.LAFeather, Ball.LAGreat, Ball.LAJet],
        PersonalColor.Yellow => [Ball.LAUltra],
        PersonalColor.Green => [Ball.LAPoke],
        PersonalColor.Black => [Ball.LAGigaton, Ball.LALeaden, Ball.LAHeavy, Ball.LAUltra],
        PersonalColor.Brown => [Ball.LAPoke],
        PersonalColor.Purple => [Ball.LAPoke],
        PersonalColor.Gray => [Ball.LAGigaton, Ball.LALeaden, Ball.LAHeavy],
        PersonalColor.White => [Ball.LAWing, Ball.LAJet],
        _ => [Ball.LAPoke],
    };
}
