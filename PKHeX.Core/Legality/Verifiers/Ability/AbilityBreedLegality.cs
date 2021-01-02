using System.Collections.Generic;
using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    /// <summary>
    /// Tables used for <see cref="AbilityVerifier"/>
    /// </summary>
    internal static class AbilityBreedLegality
    {
        /// <summary>
        /// Species that cannot be bred with a Hidden Ability originating in <see cref="GameVersion.Gen5"/>
        /// </summary>
        internal static readonly HashSet<int> BanHidden5 = new()
        {
            // Only males distributed; unable to pass to offspring
            (int)Bulbasaur, (int)Charmander, (int)Squirtle,
            (int)Tauros,
            (int)Chikorita, (int)Cyndaquil, (int)Totodile,
            (int)Tyrogue,
            (int)Treecko, (int)Torchic, (int)Mudkip,
            (int)Turtwig, (int)Chimchar, (int)Piplup,
            (int)Pansage, (int)Pansear, (int)Panpour,
            (int)Gothita,

            // Genderless; unable to pass to offspring
            (int)Magnemite,
            (int)Voltorb,
            (int)Staryu,
            (int)Ditto,
            (int)Porygon,
            (int)Beldum,
            (int)Bronzor,
            (int)Golett,

            // Not available at all
            (int)Gastly,
            (int)Koffing,
            (int)Misdreavus,
            (int)Unown,
            (int)Slakoth,
            (int)Plusle,
            (int)Plusle,
            (int)Lunatone,
            (int)Solrock,
            (int)Baltoy,
            (int)Castform,
            (int)Kecleon,
            (int)Duskull,
            (int)Chimecho,
            (int)Cherubi,
            (int)Chingling,
            (int)Rotom,
            (int)Phione,
            (int)Snivy, (int)Tepig, (int)Oshawott,
            (int)Throh, (int)Sawk,
            (int)Sigilyph,
            (int)Yamask,
            (int)Archen,
            (int)Zorua,
            (int)Ferroseed,
            (int)Klink,
            (int)Tynamo,
            (int)Litwick,
            (int)Cryogonal,
            (int)Rufflet,
            (int)Deino,
            (int)Larvesta,
        };

        /// <summary>
        /// Species that cannot be bred with a Hidden Ability originating in <see cref="GameVersion.Gen6"/>
        /// </summary>
        internal static readonly HashSet<int> BanHidden6 = new()
        {
            // Not available at Friend Safari or Horde Encounter
            (int)Flabébé + (2 << 11), // Orange
            (int)Flabébé + (4 << 11), // White

            // Super Size can be obtained as a Pumpkaboo from event distributions
            (int)Pumpkaboo + (1 << 11), // Small
            (int)Pumpkaboo + (2 << 11), // Large

            // Same abilities (1/2/H), not available as H
            (int)Honedge,
            (int)Carnivine,
            (int)Cryogonal,
            (int)Archen,
            (int)Rotom,
            (int)Rotom + (1 << 11),
            (int)Rotom + (2 << 11),
            (int)Rotom + (3 << 11),
            (int)Rotom + (4 << 11),
            (int)Rotom + (5 << 11),

            (int)Castform,
            (int)Furfrou,
            (int)Furfrou + (1 << 11),
            (int)Furfrou + (2 << 11),
            (int)Furfrou + (3 << 11),
            (int)Furfrou + (4 << 11),
            (int)Furfrou + (5 << 11),
            (int)Furfrou + (6 << 11),
            (int)Furfrou + (7 << 11),
            (int)Furfrou + (8 << 11),
            (int)Furfrou + (9 << 11),
        };

        /// <summary>
        /// Species that cannot be bred with a Hidden Ability originating in <see cref="GameVersion.Gen7"/>
        /// </summary>
        internal static readonly HashSet<int> BanHidden7 = new()
        {
            // SOS slots have 0 call rate
            (int)Wimpod,
            (int)Golisopod,
            (int)Komala,

            // No Encounter
            (int)Minior + (07 << 11),
            (int)Minior + (08 << 11),
            (int)Minior + (09 << 11),
            (int)Minior + (10 << 11),
            (int)Minior + (11 << 11),
            (int)Minior + (12 << 11),
            (int)Minior + (13 << 11),

            // Previous-Gen
            (int)Pumpkaboo + (1 << 11), // Small
            (int)Pumpkaboo + (2 << 11), // Large

            // Same abilities (1/2/H), not available as H
            (int)Honedge,
            (int)Doublade,
            (int)Aegislash,
            (int)Carnivine,
            (int)Cryogonal,
            (int)Archen,
            (int)Archeops,
            (int)Rotom,
            (int)Rotom + (1 << 11),
            (int)Rotom + (2 << 11),
            (int)Rotom + (3 << 11),
            (int)Rotom + (4 << 11),
            (int)Rotom + (5 << 11),
        };

        // <summary>
        // Species that cannot be bred with a Hidden Ability originating in <see cref="GameVersion.Gen8"/>
        // </summary>
        // internal static readonly HashSet<int> BanHidden8 = new(); // none as of DLC 1!
    }
}
