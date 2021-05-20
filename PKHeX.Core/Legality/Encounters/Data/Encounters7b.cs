using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    internal static class Encounters7b
    {
        internal static readonly EncounterArea7b[] SlotsGP = EncounterArea7b.GetAreas(Get("gp", "gg"), GP);
        internal static readonly EncounterArea7b[] SlotsGE = EncounterArea7b.GetAreas(Get("ge", "gg"), GE);
        private static byte[][] Get(string resource, string ident) => BinLinker.Unpack(Util.GetBinaryResource($"encounter_{resource}.pkl"), ident);

        private static readonly EncounterStatic7b[] Encounter_GG =
        {
            // encounters
            new(GG) { Species = 144, Level = 50, Location = 44, FlawlessIVCount = 3, }, // Articuno @ Seafoam Islands
            new(GG) { Species = 145, Level = 50, Location = 42, FlawlessIVCount = 3, }, // Zapdos @ Power Plant
            new(GG) { Species = 146, Level = 50, Location = 45, FlawlessIVCount = 3, }, // Moltres @ Victory Road
            new(GG) { Species = 150, Level = 70, Location = 46, FlawlessIVCount = 3, }, // Mewtwo @ Cerulean Cave
            new(GG) { Species = 143, Level = 34, Location = 14, FlawlessIVCount = 3, }, // Snorlax @ Route 12
            new(GG) { Species = 143, Level = 34, Location = 18, FlawlessIVCount = 3, }, // Snorlax @ Route 16
            // unused new EncounterStatic7b { Species = 100, Level = 42, Location = 42, FlawlessIVCount = 3, }, // Voltorb @ Power Plant
            // collision new EncounterStatic7b { Species = 101, Level = 42, Location = 42, FlawlessIVCount = 3, }, // Electrode @ Power Plant

            // gifts
            new(GP) { Species = 025, Level = 05, Location = 28, Gift = true, IVs = new[] {31,31,31,31,31,31}, Shiny = Shiny.Never, Form = 8 }, // Pikachu @ Pallet Town
            new(GE) { Species = 133, Level = 05, Location = 28, Gift = true, IVs = new[] {31,31,31,31,31,31}, Shiny = Shiny.Never, Form = 1 }, // Eevee @ Pallet Town

            new(GG) { Species = 129, Level = 05, Location = 06, Gift = true, IVs = new[] {30,31,25,30,25,25} }, // Magikarp @ Route 4

            // unused new EncounterStatic7b { Species = 133, Level = 30, Location = 34, Gift = true }, // Eevee @ Celadon City
            new(GG) { Species = 131, Level = 34, Location = 52, Gift = true, IVs = new[] {31,25,25,25,30,30} }, // Lapras @ Saffron City (Silph Co. Employee, inside)
            new(GG) { Species = 106, Level = 30, Location = 38, Gift = true, IVs = new[] {25,30,25,31,25,30} }, // Hitmonlee @ Saffron City (Karate Master)
            new(GG) { Species = 107, Level = 30, Location = 38, Gift = true, IVs = new[] {25,31,30,25,25,30} }, // Hitmonchan @ Saffron City (Karate Master)
            new(GG) { Species = 140, Level = 44, Location = 36, Gift = true, FlawlessIVCount = 3 }, // Kabuto @ Cinnabar Island (Cinnabar Pokémon Lab)
            new(GG) { Species = 138, Level = 44, Location = 36, Gift = true, FlawlessIVCount = 3 }, // Omanyte @ Cinnabar Island (Cinnabar Pokémon Lab)
            new(GG) { Species = 142, Level = 44, Location = 36, Gift = true, FlawlessIVCount = 3 }, // Aerodactyl @ Cinnabar Island (Cinnabar Pokémon Lab)
            new(GG) { Species = 001, Level = 12, Location = 31, Gift = true, IVs = new[] {31,25,30,25,25,30} }, // Bulbasaur @ Cerulean City
            new(GG) { Species = 004, Level = 14, Location = 26, Gift = true, IVs = new[] {25,30,25,31,30,25} }, // Charmander @ Route 24
            new(GG) { Species = 007, Level = 16, Location = 33, Gift = true, IVs = new[] {25,25,30,25,31,30} }, // Squirtle @ Vermillion City
            new(GG) { Species = 137, Level = 34, Location = 38, Gift = true, IVs = new[] {25,25,30,25,31,30} }, // Porygon @ Saffron City (Silph Co. Employee, outside)
            new(GP) { Species = 053, Level = 16, Location = 33, Gift = true, IVs = new[] {30,30,25,31,25,25} }, // Persian @ Vermillion City (Outside Fan Club)
            new(GE) { Species = 059, Level = 16, Location = 33, Gift = true, IVs = new[] {25,30,25,31,30,25} }, // Arcanine @ Vermillion City (Outside Fan Club)
        };

        private static readonly string[] T1 = { string.Empty, "ミニコ", "Tatianna", "BarbaRatatta", "Addoloratta", "Barbaratt", string.Empty, "Tatiana", "미니꼬", "小幂妮", "小幂妮", };
        private static readonly string[] T2 = { string.Empty, "ボーアイス", "Nicholice", "Iceman-4L0L4", "Goffreddo", "Eisper", string.Empty, "Gelasio", "보아이스", "露冰冰", "露冰冰", };
        private static readonly string[] T3 = { string.Empty, "レディダグ", "Diggette", "Taupilady", "Lady Glett", "Digga", string.Empty, "Glenda", "레이디그다", "蒂淑", "蒂淑", };
        private static readonly string[] T4 = { string.Empty, "ワルモン", "Darko", "AlolaZeDark", "Mattetro", "Bösbert", string.Empty, "Sinesio", "나뻐기", "达怀丹", "达怀丹", };
        private static readonly string[] T5 = { string.Empty, "エリッチ", "Psytrice", "TopDeTonCœur", "Chulia", "Assana", string.Empty, "Menchu", "엘리츄", "晶莹丘", "晶莹丘", };
        private static readonly string[] T6 = { string.Empty, "ジェンガラ", "Genmar", "OSS-Dandy7", "Mr. Owak", "Knoggelius", string.Empty, "Mario", "젠구리", "申史加拉", "申史加拉", };
        private static readonly string[] T7 = { string.Empty, "マニシ", "Exemann", "Koko-fan", "Exechiele", "Einrich", string.Empty, "Gunter", "마니시", "艾浩舒", "艾浩舒", };
        private static readonly string[] T8 = { string.Empty, "コツブ", "Higeo", "Montagnou", "George", "Karstein", string.Empty, "Georgie", "산돌", "科布", "科布", };

        internal static readonly EncounterTrade7b[] TradeGift_GG =
        {
            // Random candy values! They can be zero so no impact on legality even though statistically rare.
            new(GG) { Species = 019, Form = 1, Level = 12, TrainerNames = T1, TID7 = 121106, OTGender = 1, IVs = new[] {31,31,-1,-1,-1,-1} }, // Rattata @ Cerulean City, AV rand [0-5)
            new(GP) { Species = 027, Form = 1, Level = 27, TrainerNames = T2, TID7 = 703019, OTGender = 0, IVs = new[] {-1,31,31,-1,-1,-1} }, // Sandshrew @ Celadon City, AV rand [0-5)
            new(GE) { Species = 037, Form = 1, Level = 27, TrainerNames = T2, TID7 = 703019, OTGender = 0, IVs = new[] {-1,-1,-1,31,31,-1} }, // Vulpix @ Celadon City, AV rand [0-5)
            new(GG) { Species = 050, Form = 1, Level = 25, TrainerNames = T3, TID7 = 520159, OTGender = 1, IVs = new[] {-1,31,-1,31,-1,-1} }, // Diglett @ Lavender Town, AV rand [0-5)
            new(GE) { Species = 052, Form = 1, Level = 44, TrainerNames = T4, TID7 = 000219, OTGender = 0, IVs = new[] {31,-1,-1,31,-1,-1} }, // Meowth @ Cinnabar Island, AV rand [0-10)
            new(GP) { Species = 088, Form = 1, Level = 44, TrainerNames = T4, TID7 = 000219, OTGender = 0, IVs = new[] {31,31,-1,-1,-1,-1} }, // Grimer @ Cinnabar Island, AV rand [0-10)
            new(GG) { Species = 026, Form = 1, Level = 30, TrainerNames = T5, TID7 = 940711, OTGender = 1, IVs = new[] {-1,-1,-1,31,31,-1} }, // Raichu @ Saffron City, AV rand [0-10)
            new(GG) { Species = 105, Form = 1, Level = 38, TrainerNames = T6, TID7 = 102595, OTGender = 0, IVs = new[] {-1,31,31,-1,-1,-1} }, // Marowak @ Fuchsia City, AV rand [0-10)
            new(GG) { Species = 103, Form = 1, Level = 46, TrainerNames = T7, TID7 = 060310, OTGender = 0, IVs = new[] {-1,31,-1,-1,31,-1} }, // Exeggutor @ Indigo Plateau, AV rand [0-15)
            new(GG) { Species = 074, Form = 1, Level = 16, TrainerNames = T8, TID7 = 551873, OTGender = 0, IVs = new[] {31,31,-1,-1,-1,-1} }, // Geodude @ Vermilion City, AV rand [0-5)
        };

        internal static readonly EncounterStatic7b[] StaticGP = GetEncounters(Encounter_GG, GP);
        internal static readonly EncounterStatic7b[] StaticGE = GetEncounters(Encounter_GG, GE);
    }
}
