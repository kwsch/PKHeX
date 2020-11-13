using System.Collections.Generic;
using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    /// <summary>
    /// Encounter data from <see cref="GameVersion.GO"/>, which has multiple generations of origin.
    /// </summary>
    internal static class EncountersGO
    {
        /// <summary> Clamp for generating encounters; no species allowed above this value except for those in <see cref="ExtraSpecies"/>. </summary>
        private const int MaxSpeciesID_GO_HOME = Legal.MaxSpeciesID_5;

        /// <summary> Species beyond <see cref="MaxSpeciesID_GO_HOME"/> that are available for capture and transferring. </summary>
        private static readonly int[] ExtraSpecies =
        {
            (int)Meltan,
            (int)Melmetal,

            (int)Obstagoon,
            (int)Perrserker,
            (int)Sirfetchd,
            (int)Runerigus,
        };

        /// <summary> When generating encounters, these species will be skipped. </summary>
        private static readonly HashSet<int> DisallowedSpecies = new HashSet<int>
        {
            // Unreleased Galarian Forms and Galarian Evolutions
            (int)Slowpoke + (1 << 11), (int)Slowbro + (2 << 11), (int)Slowking + (1 << 11),
            (int)MrMime + (1 << 11), (int)MrRime,
            (int)Articuno + (1 << 11),
            (int)Zapdos + (1 << 11),
            (int)Moltres + (1 << 11),
            (int)Corsola + (1 << 11), (int)Cursola,

            // Unreleased Pokémon (Generations 6, 7, and 8 are all unavailable, with some exceptions)
            (int)Kecleon,
            (int)Phione,
            (int)Manaphy,
            (int)Shaymin,
            (int)Arceus,
            (int)Munna, (int)Musharna,
            (int)Zorua, (int)Zoroark,
            (int)Vanillite, (int)Vanillish, (int)Vanilluxe,
            (int)Deerling, (int)Sawsbuck, // Spring
            (int)Deerling + (1 << 11), (int)Sawsbuck + (1 << 11), // Summer
            (int)Deerling + (3 << 11), (int)Sawsbuck + (3 << 11), // Winter
            (int)Frillish, (int)Jellicent,
            (int)Tynamo, (int)Eelektrik, (int)Eelektross,
            (int)Mienfoo, (int)Mienshao,
            (int)Druddigon,
            (int)Larvesta, (int)Volcarona,
            (int)Keldeo,
            (int)Meloetta,

            // Cannot be transferred to HOME
            (int)Spinda,
        };

        private static readonly HashSet<int> UnavailableShiny = new HashSet<int>
        {
            // Unreleased Shiny Galarian Forms and Galarian Evolutions
            (int)Meowth + (2 << 11), (int)Perrserker,
            (int)Ponyta + (1 << 11), (int)Rapidash + (1 << 11),
            (int)Slowpoke + (1 << 11), (int)Slowbro + (2 << 11), (int)Slowking + (1 << 11),
            (int)Farfetchd + (1 << 11), (int)Sirfetchd,
            (int)Weezing + (1 << 11),
            (int)MrMime + (1 << 11), (int)MrRime,
            (int)Articuno + (1 << 11),
            (int)Zapdos + (1 << 11),
            (int)Moltres + (1 << 11),
            (int)Corsola + (1 << 11), (int)Cursola,
            (int)Zigzagoon + (1 << 11), (int)Linoone + (1 << 11), (int)Obstagoon,
            (int)Darumaka + (1 << 11), (int)Darmanitan + (2 << 11),
            (int)Yamask + (1 << 11), (int)Runerigus,
            (int)Stunfisk + (1 << 11),

            // Unreleased Shiny Pokémon (Generations 6, 7, and 8 are all unavailable, with some exceptions)
            (int)Spearow, (int)Fearow,
            (int)Paras, (int)Parasect,
            (int)Slowpoke, (int)Slowbro, (int)Slowking,
            (int)Goldeen, (int)Seaking,
            (int)Ditto,
            (int)Mew,
            (int)Hoothoot, (int)Noctowl,
            (int)Spinarak, (int)Ariados,
            (int)Hoppip, (int)Skiploom, (int)Jumpluff,
            (int)Wooper, (int)Quagsire,
            (int)Unown + (2 << 11), // B
            (int)Unown + (3 << 11), // C
            (int)Unown + (4 << 11), // D
            (int)Unown + (5 << 11), // E
            (int)Unown + (6 << 11), // F
            (int)Unown + (8 << 11), // H
            (int)Unown + (9 << 11), // I
            (int)Unown + (10 << 11), // J
            (int)Unown + (11 << 11), // K
            (int)Unown + (13 << 11), // M
            (int)Unown + (14 << 11), // N
            (int)Unown + (16 << 11), // P
            (int)Unown + (17 << 11), // Q
            (int)Unown + (19 << 11), // S
            (int)Unown + (22 << 11), // V
            (int)Unown + (23 << 11), // W
            (int)Unown + (24 << 11), // X
            (int)Unown + (25 << 11), // Y
            (int)Unown + (26 << 11), // Z
            (int)Unown + (27 << 11), // !
            (int)Unown + (28 << 11), // ?
            (int)Girafarig,
            (int)Heracross,
            (int)Slugma, (int)Magcargo,
            (int)Corsola,
            (int)Remoraid, (int)Octillery,
            (int)Phanpy, (int)Donphan,
            (int)Smeargle,
            (int)Tyrogue, (int)Hitmonlee, (int)Hitmonchan, (int)Hitmontop,
            (int)Miltank,
            (int)Celebi,
            (int)Surskit, (int)Masquerain,
            (int)Shroomish, (int)Breloom,
            (int)Whismur, (int)Loudred, (int)Exploud,
            (int)Nosepass, (int)Probopass,
            (int)Gulpin, (int)Swalot,
            (int)Numel, (int)Camerupt,
            (int)Torkoal,
            (int)Cacnea, (int)Cacturne,
            (int)Corphish, (int)Crawdaunt,
            (int)Kecleon,
            (int)Tropius,
            (int)Spheal, (int)Sealeo, (int)Walrein,
            (int)Relicanth,
            (int)Jirachi,
            (int)Starly, (int)Staravia, (int)Staraptor,
            (int)Bidoof, (int)Bibarel,
            (int)Cranidos, (int)Rampardos,
            (int)Shieldon, (int)Bastiodon,
            (int)Combee, (int)Vespiquen,
            (int)Pachirisu,
            (int)Buizel, (int)Floatzel,
            (int)Cherubi, (int)Cherrim,
            (int)Shellos, (int)Gastrodon, // West Sea
            (int)Shellos + (1 << 11), (int)Gastrodon + (1 << 11), // East Sea
            (int)Chingling, (int)Chimecho,
            (int)Stunky, (int)Skuntank,
            (int)Chatot,
            (int)Munchlax, (int)Snorlax,
            (int)Carnivine,
            (int)Finneon, (int)Lumineon,
            (int)Mantyke, (int)Mantine,
            (int)Rotom,
            (int)Uxie,
            (int)Mesprit,
            (int)Azelf,
            (int)Dialga,
            (int)Palkia,
            (int)Regigigas,
            (int)Phione,
            (int)Manaphy,
            (int)Shaymin,
            (int)Arceus,
            (int)Victini,
            (int)Snivy, (int)Servine, (int)Serperior,
            (int)Tepig, (int)Pignite, (int)Emboar,
            (int)Oshawott, (int)Dewott, (int)Samurott,
            (int)Purrloin, (int)Liepard,
            (int)Pansage, (int)Simisage,
            (int)Pansear, (int)Simisear,
            (int)Panpour, (int)Simipour,
            (int)Munna, (int)Musharna,
            (int)Blitzle, (int)Zebstrika,
            (int)Drilbur, (int)Excadrill,
            (int)Audino,
            (int)Tympole, (int)Palpitoad, (int)Seismitoad,
            (int)Throh,
            (int)Sawk,
            (int)Sewaddle, (int)Swadloon, (int)Leavanny,
            (int)Venipede, (int)Whirlipede, (int)Scolipede,
            (int)Cottonee, (int)Whimsicott,
            (int)Petilil, (int)Lilligant,
            (int)Basculin, // Red-Striped
            (int)Basculin + (1 << 11), // Blue-Striped
            (int)Sandile, (int)Krokorok, (int)Krookodile,
            (int)Darumaka, (int)Darmanitan,
            (int)Maractus,
            (int)Scraggy, (int)Scrafty,
            (int)Sigilyph,
            (int)Tirtouga, (int)Carracosta,
            (int)Archen, (int)Archeops,
            (int)Trubbish, (int)Garbodor,
            (int)Zorua, (int)Zoroark,
            (int)Gothita, (int)Gothorita, (int)Gothitelle,
            (int)Solosis, (int)Duosion, (int)Reuniclus,
            (int)Ducklett, (int)Swanna,
            (int)Vanillite, (int)Vanillish, (int)Vanilluxe,
            (int)Deerling, (int)Sawsbuck, // Spring
            (int)Deerling + (1 << 11), (int)Sawsbuck + (1 << 11), // Summer
            (int)Deerling + (2 << 11), (int)Sawsbuck + (2 << 11), // Autumn
            (int)Deerling + (3 << 11), (int)Sawsbuck + (3 << 11), // Winter
            (int)Emolga,
            (int)Karrablast, (int)Escavalier,
            (int)Foongus, (int)Amoonguss,
            (int)Frillish, (int)Jellicent,
            (int)Alomomola,
            (int)Joltik, (int)Galvantula,
            (int)Tynamo, (int)Eelektrik, (int)Eelektross,
            (int)Elgyem, (int)Beheeyem,
            (int)Litwick, (int)Lampent, (int)Chandelure,
            (int)Axew, (int)Fraxure, (int)Haxorus,
            (int)Cubchoo, (int)Beartic,
            (int)Cryogonal,
            (int)Shelmet, (int)Accelgor,
            (int)Stunfisk,
            (int)Mienfoo, (int)Mienshao,
            (int)Druddigon,
            (int)Golett, (int)Golurk,
            (int)Pawniard, (int)Bisharp,
            (int)Bouffalant,
            (int)Rufflet, (int)Braviary,
            (int)Vullaby, (int)Mandibuzz,
            (int)Larvesta, (int)Volcarona,
            (int)Tornadus,
            (int)Thundurus,
            (int)Reshiram,
            (int)Zekrom,
            (int)Landorus,
            (int)Kyurem,
            (int)Keldeo,
            (int)Meloetta,
        };

        private static readonly HashSet<int> Purified = new HashSet<int>
        {
            // Purified Pokémon
            (int)Bulbasaur, (int)Ivysaur, (int)Venusaur,
            (int)Charmander, (int)Charmeleon, (int)Charizard,
            (int)Squirtle, (int)Wartortle, (int)Blastoise,
            (int)Weedle, (int)Kakuna, (int)Beedrill,
            (int)Rattata, (int)Raticate,
            (int)Ekans, (int)Arbok,
            (int)Sandshrew, (int)Sandslash,
            (int)NidoranF, (int)Nidorina, (int)Nidoqueen,
            (int)NidoranM, (int)Nidorino, (int)Nidoking,
            (int)Vulpix, (int)Ninetales,
            (int)Zubat, (int)Golbat, (int)Crobat,
            (int)Oddish, (int)Gloom, (int)Vileplume, (int)Bellossom,
            (int)Venonat, (int)Venomoth,
            (int)Diglett, (int)Dugtrio,
            (int)Meowth, (int)Persian,
            (int)Psyduck, (int)Golduck,
            (int)Growlithe, (int)Arcanine,
            (int)Poliwag, (int)Poliwhirl, (int)Poliwrath, (int)Politoed,
            (int)Abra, (int)Kadabra, (int)Alakazam,
            (int)Machop, (int)Machoke, (int)Machamp,
            (int)Bellsprout, (int)Weepinbell, (int)Victreebel,
            (int)Slowpoke, (int)Slowbro, (int)Slowking,
            (int)Magnemite, (int)Magneton, (int)Magnezone,
            (int)Grimer, (int)Muk,
            (int)Shellder, (int)Cloyster,
            (int)Drowzee, (int)Hypno,
            (int)Exeggcute, (int)Exeggutor,
            (int)Cubone, (int)Marowak,
            (int)Hitmonlee, (int)Hitmonchan,
            (int)Koffing, (int)Weezing,
            (int)Scyther, (int)Scizor,
            (int)Electabuzz, (int)Electivire,
            (int)Magmar, (int)Magmortar,
            (int)Pinsir,
            (int)Magikarp, (int)Gyarados,
            (int)Lapras,
            (int)Porygon, (int)Porygon2, (int)PorygonZ,
            (int)Omanyte, (int)Omastar,
            (int)Aerodactyl,
            (int)Snorlax,
            (int)Articuno,
            (int)Zapdos,
            (int)Moltres,
            (int)Dratini, (int)Dragonair, (int)Dragonite,
            (int)Mewtwo,
            (int)Mareep, (int)Flaaffy, (int)Ampharos,
            (int)Hoppip, (int)Skiploom, (int)Jumpluff,
            (int)Misdreavus, (int)Mismagius,
            (int)Wobbuffet,
            (int)Pineco, (int)Forretress,
            (int)Gligar, (int)Gliscor,
            (int)Shuckle,
            (int)Sneasel, (int)Weavile,
            (int)Teddiursa, (int)Ursaring,
            (int)Delibird,
            (int)Skarmory,
            (int)Houndour, (int)Houndoom,
            (int)Stantler,
            (int)Raikou,
            (int)Entei,
            (int)Suicune,
            (int)Larvitar, (int)Pupitar, (int)Tyranitar,
            (int)Mudkip, (int)Marshtomp, (int)Swampert,
            (int)Seedot, (int)Nuzleaf, (int)Shiftry,
            (int)Ralts, (int)Kirlia, (int)Gardevoir, (int)Gallade,
            (int)Sableye,
            (int)Mawile,
            (int)Carvanha, (int)Sharpedo,
            (int)Trapinch, (int)Vibrava, (int)Flygon,
            (int)Cacnea, (int)Cacturne,
            (int)Shuppet, (int)Banette,
            (int)Duskull, (int)Dusclops, (int)Dusknoir,
            (int)Absol,
            (int)Bagon, (int)Shelgon, (int)Salamence,
            (int)Beldum, (int)Metang, (int)Metagross,
            (int)Turtwig, (int)Grotle, (int)Torterra,
            (int)Stunky, (int)Skuntank,
            (int)Snover, (int)Abomasnow,
        };

        /// <summary> Premier Ball Legality </summary>
        private static readonly HashSet<int> AvailableAsRaids = new HashSet<int>(Purified)
        {
            // Base Species (Raids)
            (int)Bulbasaur, (int)Ivysaur, (int)Venusaur,
            (int)Charmander, (int)Charmeleon, (int)Charizard,
            (int)Squirtle, (int)Wartortle, (int)Blastoise,
            (int)Caterpie, (int)Metapod, (int)Butterfree,
            (int)Beedrill,
            (int)Pidgeot,
            (int)Fearow,
            (int)Ekans, (int)Arbok,
            (int)Pikachu, (int)Raichu,
            (int)Sandshrew, (int)Sandslash,
            (int)Nidoqueen,
            (int)NidoranM, (int)Nidorino, (int)Nidoking,
            (int)Clefairy, (int)Clefable,
            (int)Vulpix, (int)Ninetales,
            (int)Jigglypuff, (int)Wigglytuff,
            (int)Zubat, (int)Golbat, (int)Crobat,
            (int)Gloom, (int)Vileplume, (int)Bellossom,
            (int)Venonat, (int)Venomoth,
            (int)Meowth, (int)Persian,
            (int)Psyduck, (int)Golduck,
            (int)Mankey, (int)Primeape,
            (int)Growlithe, (int)Arcanine,
            (int)Poliwag, (int)Poliwhirl, (int)Poliwrath, (int)Politoed,
            (int)Abra, (int)Kadabra, (int)Alakazam,
            (int)Machop, (int)Machoke, (int)Machamp,
            (int)Victreebel,
            (int)Tentacruel,
            (int)Geodude, (int)Graveler, (int)Golem,
            (int)Ponyta, (int)Rapidash,
            (int)Slowpoke, (int)Slowbro, (int)Slowking,
            (int)Magnemite, (int)Magneton, (int)Magnezone,
            (int)Seel, (int)Dewgong,
            (int)Grimer, (int)Muk,
            (int)Shellder, (int)Cloyster,
            (int)Haunter, (int)Gengar,
            (int)Onix, (int)Steelix,
            (int)Drowzee, (int)Hypno,
            (int)Krabby, (int)Kingler,
            (int)Voltorb, (int)Electrode,
            (int)Exeggutor,
            (int)Cubone, (int)Marowak,
            (int)Hitmonlee, (int)Hitmonchan, (int)Hitmontop,
            (int)Lickitung, (int)Lickilicky,
            (int)Koffing, (int)Weezing,
            (int)Rhydon, (int)Rhyperior,
            (int)Chansey, (int)Blissey,
            (int)Tangela, (int)Tangrowth,
            (int)Horsea, (int)Seadra, (int)Kingdra,
            (int)Staryu, (int)Starmie,
            (int)Scyther, (int)Scizor,
            (int)Jynx,
            (int)Electabuzz, (int)Electivire,
            (int)Magmar, (int)Magmortar,
            (int)Pinsir,
            (int)Magikarp, (int)Gyarados,
            (int)Lapras,
            (int)Eevee, (int)Vaporeon, (int)Jolteon, (int)Flareon, (int)Espeon, (int)Umbreon, (int)Leafeon, (int)Glaceon,
            (int)Porygon, (int)Porygon2, (int)PorygonZ,
            (int)Omanyte, (int)Omastar,
            (int)Kabuto, (int)Kabutops,
            (int)Aerodactyl,
            (int)Snorlax,
            (int)Articuno,
            (int)Zapdos,
            (int)Moltres,
            (int)Dratini, (int)Dragonair, (int)Dragonite,
            (int)Mewtwo,
            (int)Chikorita, (int)Bayleef, (int)Meganium,
            (int)Cyndaquil, (int)Quilava, (int)Typhlosion,
            (int)Totodile, (int)Croconaw, (int)Feraligatr,
            (int)Sentret, (int)Furret,
            (int)Noctowl,
            (int)Lanturn,
            (int)Togetic, (int)Togekiss,
            (int)Mareep, (int)Flaaffy, (int)Ampharos,
            (int)Azumarill,
            (int)Sudowoodo,
            (int)Aipom, (int)Ambipom,
            (int)Sunkern, (int)Sunflora,
            (int)Yanma, (int)Yanmega,
            (int)Murkrow, (int)Honchkrow,
            (int)Misdreavus, (int)Mismagius,
            (int)Unown + (1 << 11), // A
            (int)Unown + (12 << 11), // L
            (int)Unown + (18 << 11), // R
            (int)Unown + (20 << 11), // T
            (int)Unown + (21 << 11), // U
            (int)Wobbuffet,
            (int)Girafarig,
            (int)Pineco, (int)Forretress,
            (int)Gligar, (int)Gliscor,
            (int)Snubbull, (int)Granbull,
            (int)Qwilfish,
            (int)Shuckle,
            (int)Sneasel, (int)Weavile,
            (int)Ursaring,
            (int)Magcargo,
            (int)Swinub, (int)Piloswine, (int)Mamoswine,
            (int)Octillery,
            (int)Delibird,
            (int)Mantine,
            (int)Skarmory,
            (int)Houndour, (int)Houndoom,
            (int)Donphan,
            (int)Miltank,
            (int)Raikou,
            (int)Entei,
            (int)Suicune,
            (int)Larvitar, (int)Pupitar, (int)Tyranitar,
            (int)Lugia,
            (int)HoOh,
            (int)Treecko, (int)Grovyle, (int)Sceptile,
            (int)Torchic, (int)Combusken, (int)Blaziken,
            (int)Mudkip, (int)Marshtomp, (int)Swampert,
            (int)Mightyena,
            (int)Wurmple, (int)Silcoon, (int)Beautifly, (int)Cascoon, (int)Dustox,
            (int)Lotad, (int)Lombre, (int)Ludicolo,
            (int)Nuzleaf, (int)Shiftry,
            (int)Swellow,
            (int)Wingull, (int)Pelipper,
            (int)Ralts, (int)Kirlia, (int)Gardevoir, (int)Gallade,
            (int)Masquerain,
            (int)Breloom,
            (int)Slakoth, (int)Vigoroth, (int)Slaking,
            (int)Ninjask,
            (int)Makuhita, (int)Hariyama,
            (int)Nosepass, (int)Probopass,
            (int)Sableye,
            (int)Mawile,
            (int)Aron, (int)Lairon, (int)Aggron,
            (int)Meditite, (int)Medicham,
            (int)Manectric,
            (int)Plusle,
            (int)Minun,
            (int)Roselia, (int)Roserade,
            (int)Swalot,
            (int)Carvanha, (int)Sharpedo,
            (int)Wailmer, (int)Wailord,
            (int)Spoink, (int)Grumpig,
            (int)Trapinch, (int)Vibrava, (int)Flygon,
            (int)Cacnea, (int)Cacturne,
            (int)Swablu, (int)Altaria,
            (int)Lunatone,
            (int)Solrock,
            (int)Whiscash,
            (int)Crawdaunt,
            (int)Baltoy, (int)Claydol,
            (int)Lileep, (int)Cradily,
            (int)Anorith, (int)Armaldo,
            (int)Feebas, (int)Milotic,
            (int)Shuppet, (int)Banette,
            (int)Duskull, (int)Dusclops, (int)Dusknoir,
            (int)Absol,
            (int)Snorunt, (int)Glalie, (int)Froslass,
            (int)Walrein,
            (int)Clamperl, (int)Huntail, (int)Gorebyss,
            (int)Luvdisc,
            (int)Bagon, (int)Shelgon, (int)Salamence,
            (int)Beldum, (int)Metang, (int)Metagross,
            (int)Regirock,
            (int)Regice,
            (int)Registeel,
            (int)Latias,
            (int)Latios,
            (int)Kyogre,
            (int)Groudon,
            (int)Rayquaza,
            (int)Deoxys,
            (int)Turtwig, (int)Grotle, (int)Torterra,
            (int)Chimchar, (int)Monferno, (int)Infernape,
            (int)Piplup, (int)Prinplup, (int)Empoleon,
            (int)Staraptor,
            (int)Bidoof, (int)Bibarel,
            (int)Kricketot, (int)Kricketune,
            (int)Shinx, (int)Luxio, (int)Luxray,
            (int)Cranidos, (int)Rampardos,
            (int)Shieldon, (int)Bastiodon,
            (int)Burmy, (int)Wormadam, (int)Mothim, // Plant Cloak
            (int)Burmy + (1 << 11), (int)Wormadam + (1 << 11), (int)Mothim + (1 << 11), // Sandy Cloak
            (int)Burmy + (2 << 11), (int)Wormadam + (2 << 11), (int)Mothim + (2 << 11), // Trash Cloak
            (int)Combee, (int)Vespiquen,
            (int)Buizel, (int)Floatzel,
            (int)Drifloon, (int)Drifblim,
            (int)Buneary, (int)Lopunny,
            (int)Skuntank,
            (int)Bronzor, (int)Bronzong,
            (int)Hippopotas, (int)Hippowdon,
            (int)Gible, (int)Gabite, (int)Garchomp,
            (int)Skorupi, (int)Drapion,
            (int)Croagunk, (int)Toxicroak,
            (int)Lumineon,
            (int)Abomasnow,
            (int)Uxie,
            (int)Mesprit,
            (int)Azelf,
            (int)Dialga,
            (int)Palkia,
            (int)Heatran,
            (int)Regigigas,
            (int)Giratina,
            (int)Cresselia,
            (int)Darkrai,
            (int)Snivy, (int)Servine, (int)Serperior,
            (int)Tepig, (int)Pignite, (int)Emboar,
            (int)Oshawott, (int)Dewott, (int)Samurott,
            (int)Patrat, (int)Watchog,
            (int)Lillipup, (int)Herdier, (int)Stoutland,
            (int)Liepard,
            (int)Pidove, (int)Tranquill, (int)Unfezant,
            (int)Blitzle, (int)Zebstrika,
            (int)Roggenrola, (int)Boldore, (int)Gigalith,
            (int)Woobat, (int)Swoobat,
            (int)Excadrill,
            (int)Timburr, (int)Gurdurr, (int)Conkeldurr,
            (int)Tympole, (int)Palpitoad, (int)Seismitoad,
            (int)Venipede, (int)Whirlipede, (int)Scolipede,
            (int)Cottonee, (int)Whimsicott,
            (int)Petilil, (int)Lilligant,
            (int)Darumaka, (int)Darmanitan,
            (int)Dwebble, (int)Crustle,
            (int)Yamask, (int)Cofagrigus,
            (int)Trubbish, (int)Garbodor,
            (int)Minccino, (int)Cinccino,
            (int)Gothita, (int)Gothorita, (int)Gothitelle,
            (int)Solosis, (int)Duosion, (int)Reuniclus,
            (int)Amoonguss,
            (int)Joltik, (int)Galvantula,
            (int)Ferroseed, (int)Ferrothorn,
            (int)Klink, (int)Klang, (int)Klinklang,
            (int)Elgyem, (int)Beheeyem,
            (int)Litwick, (int)Lampent, (int)Chandelure,
            (int)Cubchoo, (int)Beartic,
            (int)Cryogonal,
            (int)Stunfisk,
            (int)Golurk,
            (int)Cobalion,
            (int)Terrakion,
            (int)Virizion,
            (int)Tornadus,
            (int)Thundurus,
            (int)Landorus,
            (int)Reshiram,
            (int)Zekrom,
            (int)Kyurem,

            // Alolan Forms (Raids)
            (int)Raticate + (1 << 11),
            (int)Raichu + (1 << 11),
            (int)Vulpix + (1 << 11), (int)Ninetales + (1 << 11),
            (int)Dugtrio + (1 << 11),
            (int)Meowth + (1 << 11), (int)Persian + (1 << 11),
            (int)Geodude + (1 << 11), (int)Graveler + (1 << 11), (int)Golem + (1 << 11),
            (int)Grimer + (1 << 11), (int)Muk + (1 << 11),
            (int)Exeggutor + (1 << 11),
            (int)Marowak + (1 << 11),

            // Galarian Forms (Raids)
            (int)Ponyta + (1 << 11), (int)Rapidash + (1 << 11),
            (int)Weezing + (1 << 11),
            (int)Stunfisk + (1 << 11),
        };

        private static readonly HashSet<int> RequireLevelIV_Egg1_1 = new HashSet<int>
        {
            // Egg
            // Minimum Lvl: 1
            // Minimum GO IVs: 1/1/1
            // Minimum Core Series IVs: 3/3/3/3/3/0
            (int)Meowth + (2 << 11), (int)Perrserker,
            (int)Ponyta + (1 << 11), (int)Rapidash + (1 << 11),
            (int)Pichu,
            (int)Cleffa,
            (int)Igglybuff,
            (int)Togepi,
            (int)Tyrogue,
            (int)Smoochum,
            (int)Elekid,
            (int)Magby,
            (int)Zigzagoon + (1 << 11), (int)Linoone + (1 << 11), (int)Obstagoon,
            (int)Azurill,
            (int)Wynaut,
            (int)Shinx, (int)Luxio, (int)Luxray,
            (int)Budew,
            (int)Chingling,
            (int)Bonsly,
            (int)Happiny,
            (int)Munchlax,
            (int)Riolu, (int)Lucario,
            (int)Mantyke,
            (int)Sandile, (int)Krokorok, (int)Krookodile,
            (int)Darumaka, (int)Darmanitan,
            (int)Darumaka + (1 << 11), (int)Darmanitan + (2 << 11),
            (int)Klink, (int)Klang, (int)Klinklang,
            (int)Pawniard, (int)Bisharp,
            (int)Vullaby, (int)Mandibuzz,
        };

        private static readonly HashSet<int> RequireLevelIV_EggShiny1_1 = new HashSet<int>
        {
            // Shiny Baby Evolutions
            // Minimum Lvl: 1
            // Minimum GO IVs: 1/1/1
            // Minimum Core Series IVs: 3/3/3/3/3/0
            // The following Species can only be Shiny if hatched from an Egg.
            // They can be encountered in the wild, but if they are Shiny, they must abide by Egg requirements.
            (int)Togetic, (int)Togekiss,
            (int)Jynx,
            (int)Electabuzz, (int)Electivire,
            (int)Magmar, (int)Magmortar,
        };

        private static readonly HashSet<int> RequireLevelIV_Raid15_1_LGPE = new HashSet<int>
        {
            // Raid Battle / Field Research
            // Minimum Lvl: 15
            // Minimum GO IVs: 1/1/1
            // Minimum Core Series IVs: 3/3/3/3/3/0
            // Pokémon from regular Raid Battles (Lv. 20) get moved down to Lv. 15 when Traded to low-level accounts.
            (int)Raichu + (1 << 11),
            (int)Articuno,
            (int)Zapdos,
            (int)Moltres,
            (int)Mewtwo,
        };

        private static readonly HashSet<int> RequireLevelIV_Raid15_1 = new HashSet<int>(RequireLevelIV_Raid15_1_LGPE)
        {
            (int)Weezing + (1 << 11),

            (int)Raikou,
            (int)Entei,
            (int)Suicune,
            (int)Lugia,
            (int)HoOh,
            (int)Shedinja,
            (int)Spinda,
            (int)Regirock,
            (int)Regice,
            (int)Registeel,
            (int)Latias,
            (int)Latios,
            (int)Kyogre,
            (int)Groudon,
            (int)Rayquaza,
            (int)Spiritomb,
            (int)Dialga,
            (int)Palkia,
            (int)Heatran,
            (int)Regigigas,
            (int)Giratina,
            (int)Cresselia,
            (int)Victini,
            (int)Yamask + (1 << 11), (int)Runerigus,
            (int)Cobalion,
            (int)Terrakion,
            (int)Virizion,
            (int)Tornadus,
            (int)Thundurus,
            (int)Reshiram,
            (int)Zekrom,
            (int)Landorus,
            (int)Kyurem,
        };

        private static readonly HashSet<int> RequireLevelIV_Raid15_10 = new HashSet<int>
        {
            // Field Research (Mythical)
            // Minimum Lvl: 15
            // Minimum GO IVs: 10/10/10
            // Minimum Core Series IVs: 21/21/21/21/21/0
            (int)Mew,
            (int)Celebi,
            (int)Jirachi,
            (int)Victini,
            (int)Genesect,
        };

        private static readonly HashSet<int> RequireLevelIV_Raid20_10 = new HashSet<int>
        {
            // Raid Battle (Mythical)
            // Minimum Lvl: 20
            // Minimum GO IVs: 10/10/10
            // Minimum Core Series IVs: 21/21/21/21/21/0
            (int)Deoxys,
            (int)Darkrai,
        };

        private static readonly HashSet<int> ShinyBan_LGPE = new HashSet<int>
        {
            (int)Spearow, (int)Fearow,
            (int)Paras, (int)Parasect,
            (int)Slowpoke, (int)Slowbro,
            (int)Hitmonlee, (int)Hitmonchan,
            (int)Goldeen, (int)Seaking,
            (int)Ditto,
            (int)Snorlax,
        };

        private static readonly HashSet<int> ShinyBan = new HashSet<int>(ShinyBan_LGPE)
        {
            (int)Mew,

            // ??
        };

        internal static readonly EncounterArea7g[] SlotsGO_GG = EncounterArea7g.GetArea(RequireLevelIV_Raid15_1_LGPE);
        internal static readonly EncounterArea8g[] SlotsGO = EncounterArea8g.GetArea(MaxSpeciesID_GO_HOME, DisallowedSpecies, ExtraSpecies);

        internal static bool IsBallValid(int species, int form, Ball ball)
        {
            if (ball == 0)
                return false;

            var sf = species | (form << 11);
            if (RequireLevelIV_Raid15_1.Contains(sf))
                return ball == Ball.Premier;
            if (RequireLevelIV_Raid15_10.Contains(sf))
                return ball == Ball.Premier;
            if (RequireLevelIV_Raid20_10.Contains(sf))
                return ball == Ball.Premier;

            if (RequireLevelIV_Egg1_1.Contains(sf) || RequireLevelIV_EggShiny1_1.Contains(sf))
                return ball == Ball.Poke;

            return ball <= Ball.Ultra;
        }

        internal static int GetMinLevel(int species, int form)
        {
            var sf = species | (form << 11);
            if (RequireLevelIV_Raid15_1.Contains(sf))
                return 15;
            if (RequireLevelIV_Raid15_10.Contains(sf))
                return 15;
            if (RequireLevelIV_Raid20_10.Contains(sf))
                return 20;
            return 1;
        }

        internal static int GetMinIVs(int species, int form, Ball ball)
        {
            var sf = species | (form << 11);
            if (ball == Ball.Premier)
            {
                if (RequireLevelIV_Raid15_1.Contains(sf))
                    return 1;
                if (RequireLevelIV_Raid15_10.Contains(sf))
                    return 10;
                if (RequireLevelIV_Raid20_10.Contains(sf))
                    return 10;
                return 0;
            }

            if (RequireLevelIV_Egg1_1.Contains(sf))
                return 1;
            if (RequireLevelIV_EggShiny1_1.Contains(sf))
                return 1;

            return 0;
        }

        public static bool IsShinyValid(int species, int form, Ball pkmBall)
        {
            var sf = species | (form << 11);
            if (pkmBall == Ball.Premier)
                return false;

            return ShinyBan.Contains(sf);
        }
    }
}
