using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.EncounterUtil;

namespace PKHeX.Core
{
    internal static class Encounters7b
    {
        internal static readonly EncounterArea[] SlotsGP = GetEncounterTables(GameVersion.GP);
        internal static readonly EncounterArea[] SlotsGE = GetEncounterTables(GameVersion.GE);
        internal static readonly EncounterStatic[] StaticGP, StaticGE;
        internal static readonly EncounterArea[] SlotsGO_GG = GetGoParkArea();

        static Encounters7b()
        {
            StaticGP = GetStaticEncounters(Encounter_GG, GameVersion.GP);
            StaticGE = GetStaticEncounters(Encounter_GG, GameVersion.GE);

            ManuallyAddRareSpawns(SlotsGP);
            ManuallyAddRareSpawns(SlotsGE);
            SlotsGP.SetVersion(GameVersion.GP);
            SlotsGE.SetVersion(GameVersion.GE);
            Encounter_GG.SetVersion(GameVersion.GG);
            TradeGift_GG.SetVersion(GameVersion.GG);
            MarkEncountersGeneration(7, SlotsGP, SlotsGE);
            MarkEncountersGeneration(7, StaticGP, StaticGE, TradeGift_GG);
        }

        private static readonly EncounterStatic[] Encounter_GG =
        {
            // encounters
            new EncounterStatic { Species = 144, Level = 50, Location = 44, FlawlessIVCount = 3, }, // Articuno @ Seafoam Islands
            new EncounterStatic { Species = 145, Level = 50, Location = 42, FlawlessIVCount = 3, }, // Zapdos @ Power Plant
            new EncounterStatic { Species = 146, Level = 50, Location = 45, FlawlessIVCount = 3, }, // Moltres @ Victory Road
            new EncounterStatic { Species = 150, Level = 70, Location = 46, FlawlessIVCount = 3, }, // Mewtwo @ Cerulean Cave
            new EncounterStatic { Species = 143, Level = 34, Location = 14, FlawlessIVCount = 3, }, // Snorlax @ Route 12
            new EncounterStatic { Species = 143, Level = 34, Location = 18, FlawlessIVCount = 3, }, // Snorlax @ Route 16
            new EncounterStatic { Species = 100, Level = 42, Location = 42, FlawlessIVCount = 3, }, // Voltorb @ Power Plant
            new EncounterStatic { Species = 101, Level = 42, Location = 42, FlawlessIVCount = 3, }, // Electrode @ Power Plant

            // gifts
            new EncounterStatic { Species = 025, Level = 05, Location = 28, Gift = true, IVs = new[] {31,31,31,31,31,31}, Shiny = Shiny.Never, Form = 8, Version = GameVersion.GP }, // Pikachu @ Pallet Town
            new EncounterStatic { Species = 133, Level = 05, Location = 28, Gift = true, IVs = new[] {31,31,31,31,31,31}, Shiny = Shiny.Never, Form = 1, Version = GameVersion.GE }, // Eevee @ Pallet Town

            new EncounterStatic { Species = 129, Level = 05, Location = 06, Gift = true, IVs = new[] {30,31,25,30,25,25} }, // Magikarp @ Route 4

            // unused new EncounterStatic { Species = 133, Level = 30, Location = 34, Gift = true }, // Eevee @ Celadon City
            new EncounterStatic { Species = 131, Level = 34, Location = 52, Gift = true, IVs = new[] {31,25,25,25,30,30} }, // Lapras @ Saffron City (Silph Co. Employee, inside)
            new EncounterStatic { Species = 106, Level = 30, Location = 38, Gift = true, IVs = new[] {25,30,25,31,25,30} }, // Hitmonlee @ Saffron City (Karate Master)
            new EncounterStatic { Species = 107, Level = 30, Location = 38, Gift = true, IVs = new[] {25,31,30,25,25,30} }, // Hitmonchan @ Saffron City (Karate Master)
            new EncounterStatic { Species = 140, Level = 44, Location = 36, Gift = true, FlawlessIVCount = 3 }, // Kabuto @ Cinnabar Island (Cinnabar Pokémon Lab)
            new EncounterStatic { Species = 138, Level = 44, Location = 36, Gift = true, FlawlessIVCount = 3 }, // Omanyte @ Cinnabar Island (Cinnabar Pokémon Lab)
            new EncounterStatic { Species = 142, Level = 44, Location = 36, Gift = true, FlawlessIVCount = 3 }, // Aerodactyl @ Cinnabar Island (Cinnabar Pokémon Lab)
            new EncounterStatic { Species = 001, Level = 12, Location = 31, Gift = true, IVs = new[] {31,25,30,25,25,30} }, // Bulbasaur @ Cerulean City
            new EncounterStatic { Species = 004, Level = 14, Location = 26, Gift = true, IVs = new[] {25,30,25,31,30,25} }, // Charmander @ Route 24
            new EncounterStatic { Species = 007, Level = 16, Location = 33, Gift = true, IVs = new[] {25,25,30,25,31,30} }, // Squirtle @ Vermillion City
            new EncounterStatic { Species = 137, Level = 34, Location = 38, Gift = true, IVs = new[] {25,25,30,25,31,30} }, // Porygon @ Saffron City (Silph Co. Employee, outside)
            new EncounterStatic { Species = 053, Level = 16, Location = 33, Gift = true, IVs = new[] {30,30,25,31,25,25}, Version = GameVersion.GP }, // Persian @ Vermillion City (Outside Fan Club)
            new EncounterStatic { Species = 059, Level = 16, Location = 33, Gift = true, IVs = new[] {25,30,25,31,30,25}, Version = GameVersion.GE }, // Arcanine @ Vermillion City (Outside Fan Club)
        };

        private static readonly string[] T1 = { null, "ミニコ", "Tatianna", "BarbaRatatta", "Addoloratta", "Barbaratt", null, "Tatiana", "미니꼬", "小幂妮", "小幂妮", };
        private static readonly string[] T2 = { null, "ボーアイス", "Nicholice", "Iceman-4L0L4", "Goffreddo", "Eisper", null, "Gelasio", "보아이스", "露冰冰", "露冰冰", };
        private static readonly string[] T3 = { null, "レディダグ", "Diggette", "Taupilady", "Lady Glett", "Digga", null, "Glenda", "레이디그다", "蒂淑", "蒂淑", };
        private static readonly string[] T4 = { null, "ワルモン", "Darko", "AlolaZeDark", "Mattetro", "Bösbert", null, "Sinesio", "나뻐기", "达怀丹", "达怀丹", };
        private static readonly string[] T5 = { null, "エリッチ", "Psytrice", "TopDeTonCœur", "Chulia", "Assana", null, "Menchu", "엘리츄", "晶莹丘", "晶莹丘", };
        private static readonly string[] T6 = { null, "ジェンガラ", "Genmar", "OSS-Dandy7", "Mr. Owak", "Knoggelius", null, "Mario", "젠구리", "申史加拉", "申史加拉", };
        private static readonly string[] T7 = { null, "マニシ", "Exemann", "Koko-fan", "Exechiele", "Einrich", null, "Gunter", "마니시", "艾浩舒", "艾浩舒", };
        private static readonly string[] T8 = { null, "コツブ", "Higeo", "Montagnou", "George", "Karstein", null, "Georgie", "산돌", "科布", "科布", };

        internal static readonly EncounterTrade[] TradeGift_GG =
        {
            // Random candy values! They can be zero so no impact on legality even though statistically rare.
            new EncounterTrade { Species = 019, Level = 12, Form = 1, TrainerNames = T1, TID7 = 121106, OTGender = 1, Shiny = Shiny.Random, IVs = new[] {31,31,-1,-1,-1,-1}, IsNicknamed = false }, // Rattata @ Cerulean City, AV rand [0-5)
            new EncounterTrade { Species = 027, Level = 27, Form = 1, TrainerNames = T2, TID7 = 703019, OTGender = 0, Shiny = Shiny.Random, IVs = new[] {-1,31,31,-1,-1,-1}, IsNicknamed = false, Version = GameVersion.GP }, // Sandshrew @ Celadon City, AV rand [0-5)
            new EncounterTrade { Species = 037, Level = 27, Form = 1, TrainerNames = T2, TID7 = 703019, OTGender = 0, Shiny = Shiny.Random, IVs = new[] {-1,-1,-1,31,31,-1}, IsNicknamed = false, Version = GameVersion.GE }, // Vulpix @ Celadon City, AV rand [0-5)
            new EncounterTrade { Species = 050, Level = 25, Form = 1, TrainerNames = T3, TID7 = 520159, OTGender = 1, Shiny = Shiny.Random, IVs = new[] {-1,31,-1,31,-1,-1}, IsNicknamed = false }, // Diglett @ Lavender Town, AV rand [0-5)
            new EncounterTrade { Species = 052, Level = 44, Form = 1, TrainerNames = T4, TID7 = 000219, OTGender = 0, Shiny = Shiny.Random, IVs = new[] {31,-1,-1,31,-1,-1}, IsNicknamed = false, Version = GameVersion.GE }, // Meowth @ Cinnabar Island, AV rand [0-10)
            new EncounterTrade { Species = 088, Level = 44, Form = 1, TrainerNames = T4, TID7 = 000219, OTGender = 0, Shiny = Shiny.Random, IVs = new[] {31,31,-1,-1,-1,-1}, IsNicknamed = false, Version = GameVersion.GP }, // Grimer @ Cinnabar Island, AV rand [0-10)
            new EncounterTrade { Species = 026, Level = 30, Form = 1, TrainerNames = T5, TID7 = 940711, OTGender = 1, Shiny = Shiny.Random, IVs = new[] {-1,-1,-1,31,31,-1}, IsNicknamed = false }, // Raichu @ Saffron City, AV rand [0-10)
            new EncounterTrade { Species = 105, Level = 38, Form = 1, TrainerNames = T6, TID7 = 102595, OTGender = 0, Shiny = Shiny.Random, IVs = new[] {-1,31,31,-1,-1,-1}, IsNicknamed = false }, // Marowak @ Fuchsia City, AV rand [0-10)
            new EncounterTrade { Species = 103, Level = 46, Form = 1, TrainerNames = T7, TID7 = 060310, OTGender = 0, Shiny = Shiny.Random, IVs = new[] {-1,31,-1,-1,31,-1}, IsNicknamed = false }, // Exeggutor @ Indigo Plateau, AV rand [0-15)
            new EncounterTrade { Species = 074, Level = 16, Form = 1, TrainerNames = T8, TID7 = 551873, OTGender = 0, Shiny = Shiny.Random, IVs = new[] {31,31,-1,-1,-1,-1}, IsNicknamed = false }, // Geodude @ Vermilion City, AV rand [0-5)
        };

        private static EncounterArea[] GetGoParkArea()
        {
            var area = new EncounterArea { Location = 50 };
            IEnumerable<EncounterSlot> GetAllSlot(int species)
            {
                yield return GetSlot(species, 0);
                if (Legal.AlolanOriginForms.Contains(species) || Legal.EvolveToAlolanForms.Contains(species))
                    yield return GetSlot(species, 1);
            }

            EncounterSlot GetSlot(int species, int form)
            {
                return new EncounterSlot
                {
                    Area = area,
                    Generation = 7,
                    Species = species,
                    LevelMin = 1,
                    LevelMax = 40,
                    Form = form,
                    Type = SlotType.GoPark,
                    Version = GameVersion.GO,
                };
            }
            area.Slots = Enumerable.Range(1, 150).Concat(Enumerable.Range(808, 2)).SelectMany(GetAllSlot).ToArray();
            return new[] {area};
        }

        // todo: manual addition of slots
        private class RareSpawn
        {
            public int Species;
            public int[] Locations;
        }

        private static readonly int[] Sky = {003, 004, 005, 006, 009, 010, 011, 012, 013, 014, 015, 016, 017, 018, 019, 020, 021, 022, 023, 024, 025, 026, 027};

        private static readonly RareSpawn[] Rare =
        {
            // Normal
            new RareSpawn {Species = 001, Locations = new[] {039}},
            new RareSpawn {Species = 004, Locations = new[] {005, 006, 041}},
            new RareSpawn {Species = 007, Locations = new[] {026, 027, 044}},
            new RareSpawn {Species = 106, Locations = new[] {045}},
            new RareSpawn {Species = 107, Locations = new[] {045}},
            new RareSpawn {Species = 113, Locations = new[] {007, 008, 010, 011, 012, 013, 014, 015, 016, 017, 018, 019, 020, 023, 025, 040, 042, 043, 045, 047, 051}},
            new RareSpawn {Species = 137, Locations = new[] {009}},
            new RareSpawn {Species = 143, Locations = new[] {046}},

            // Water
            new RareSpawn {Species = 131, Locations = new[] {021, 022}},

            // Fly
            new RareSpawn {Species = 006, Locations = Sky,},
            new RareSpawn {Species = 144, Locations = Sky,},
            new RareSpawn {Species = 145, Locations = Sky,},
            new RareSpawn {Species = 146, Locations = Sky,},
            new RareSpawn {Species = 149, Locations = Sky,},
        };

        private static void ManuallyAddRareSpawns(IEnumerable<EncounterArea> areas)
        {
            foreach (var table in areas)
            {
                var loc = table.Location;
                var species = Rare.Where(z => z.Locations.Contains(loc)).Select(z => z.Species).ToArray();
                if (species.Length == 0)
                    continue;
                var slots = table.Slots;
                var first = slots[0];
                var extra = species
                    .Select(z => new EncounterSlot
                    {
                        Area = table,
                        Species = z,
                        LevelMin = (z == 006 || z >= 144) ? 03 : first.LevelMin,
                        LevelMax = (z == 006 || z >= 144) ? 56 : first.LevelMax,
                    }).ToArray();

                int count = slots.Length;
                Array.Resize(ref slots, count + extra.Length);
                extra.CopyTo(slots, count);
                table.Slots = slots;
            }
        }
    }
}
