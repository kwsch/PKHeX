using static PKHeX.Core.EntityContext;
using static PKHeX.Core.Species;

namespace PKHeX.Core.AutoMod;

public static class EasterEggs
{
    public static Species GetMemeSpecies(EntityContext context) => context switch
    {
        Gen1 => Diglett,
        Gen2 => Shuckle,
        Gen3 => Ludicolo,
        Gen4 => Bidoof,
        Gen5 => Stunfisk,
        Gen6 => Sliggoo,
        Gen7 => Cosmog,
        Gen8 => Chewtle,

        Gen7b => Meltan,
        Gen8a => Porygon,
        Gen8b => Bidoof,
        Gen9 => Wiglett,
        _ => Mew,
    };

    public static string GetMemeNickname(EntityContext context) => context switch
    {
        Gen1 => "HOWDOIHAK",
        Gen2 => "DONT FCKLE",
        Gen3 => "CANTA",
        Gen4 => "U R A DOOF",
        Gen5 => "PANCAKE",
        Gen6 => "SHOOT DAT GOO",
        Gen7 => "GET IN BAG",
        Gen8 => "CUTLE",
        Gen9 => "WIGGLE",

        Gen7b => "MATT'S NUT",
        Gen8a => "BAD DATA BOI",
        Gen8b => "CHIBIDOOF",
        _ => "OwG",
    };
}
