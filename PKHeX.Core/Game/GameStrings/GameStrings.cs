using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public class GameStrings : IBasicStrings
    {
        // PKM Info
        public readonly string[] specieslist, movelist, itemlist, abilitylist, types, natures, forms,
            memories, genloc, trainingbags, trainingstage, characteristics,
            encountertypelist, gamelanguages, balllist, gamelist, pokeblocks, ribbons;

        private readonly string[] g4items, g3coloitems, g3xditems, g3items, g2items, g1items;

        // Met Locations
        public readonly string[] metGSC_00000, metRSEFRLG_00000, metCXD_00000;
        public readonly string[] metHGSS_00000, metHGSS_02000, metHGSS_03000;
        public readonly string[] metBW2_00000, metBW2_30000, metBW2_40000, metBW2_60000;
        public readonly string[] metXY_00000, metXY_30000, metXY_40000, metXY_60000;
        public readonly string[] metSM_00000, metSM_30000, metSM_40000, metSM_60000;
        public readonly string[] metGG_00000, metGG_30000, metGG_40000, metGG_60000;

        // Misc
        public readonly string[] wallpapernames, puffs;
        private readonly string lang;

        public string EggName { get; }
        public IReadOnlyList<string> Species => specieslist;
        public IReadOnlyList<string> Item => itemlist;
        public IReadOnlyList<string> Move => movelist;
        public IReadOnlyList<string> Ability => abilitylist;
        public IReadOnlyList<string> Types => types;
        public IReadOnlyList<string> Natures => natures;

        private string[] Get(string ident) => GameInfo.GetStrings(ident, lang);
        private const string NPC = "NPC";
        private static readonly string[] abilIdentifier = { " (1)", " (2)", " (H)" };
        public static readonly IReadOnlyList<ComboItem> Regions = Util.GetCSVUnsortedCBList("regions3ds");
        private static readonly IReadOnlyList<ComboItem> LanguageList = Util.GetCSVUnsortedCBList("languages");
        private static readonly string[] LanguageNames = LanguageList.GetArray();

        public GameStrings(string l)
        {
            lang = l;
            ribbons = Get("ribbons");
            // Past Generation strings
            g3items = Get("ItemsG3");
            // XD and Colosseum
            {
                g3coloitems = (string[])g3items.Clone();
                string[] tmp = Get("ItemsG3Colosseum");
                Array.Resize(ref g3coloitems, 500 + tmp.Length);
                for (int i = g3items.Length; i < g3coloitems.Length; i++)
                    g3coloitems[i] = $"UNUSED {i}";
                tmp.CopyTo(g3coloitems, g3coloitems.Length - tmp.Length);

                g3xditems = (string[])g3items.Clone();
                string[] tmp2 = Get("ItemsG3XD");
                Array.Resize(ref g3xditems, 500 + tmp2.Length);
                for (int i = g3items.Length; i < g3xditems.Length; i++)
                    g3xditems[i] = $"UNUSED {i}";
                tmp2.CopyTo(g3xditems, g3xditems.Length - tmp2.Length);
            }
            g2items = Get("ItemsG2");
            g1items = Get("ItemsG1");
            metRSEFRLG_00000 = Get("rsefrlg_00000");
            metGSC_00000 = Get("gsc_00000");

            metCXD_00000 = Get("cxd_00000");
            metCXD_00000 = SanitizeMetStringsCXD(metCXD_00000);

            // Current Generation strings
            natures = Util.GetNaturesList(l);
            types = Get("types");
            abilitylist = Get("abilities");

            movelist = Get("moves");
            string[] ps = { "P", "S" }; // Distinguish Physical/Special
            for (int i = 622; i < 658; i++)
                movelist[i] += $" ({ps[i % 2]})";

            itemlist = Get("items");
            characteristics = Get("character");
            specieslist = Get("species");
            wallpapernames = Get("wallpaper");
            encountertypelist = Get("encountertype");
            gamelist = Get("games");
            gamelanguages = LanguageNames;

            balllist = new string[Legal.Items_Ball.Length];
            for (int i = 0; i < balllist.Length; i++)
                balllist[i] = itemlist[Legal.Items_Ball[i]];

            pokeblocks = Get("pokeblock");
            forms = Get("forms");
            memories = Get("memories");
            genloc = Get("genloc");
            trainingbags = Get("trainingbag");
            trainingstage = Get("supertraining");
            puffs = Get("puff");
            Array.Resize(ref puffs, puffs.Length + 1); // shift all down, 0th will be 'none' -- applied later
            Array.Copy(puffs, 0, puffs, 1, puffs.Length - 1);

            EggName = specieslist[0];
            metHGSS_00000 = Get("hgss_00000");
            metHGSS_02000 = Get("hgss_02000");
            metHGSS_03000 = Get("hgss_03000");
            metBW2_00000 = Get("bw2_00000");
            metBW2_30000 = Get("bw2_30000");
            metBW2_40000 = Get("bw2_40000");
            metBW2_60000 = Get("bw2_60000");
            metXY_00000 = Get("xy_00000");
            metXY_30000 = Get("xy_30000");
            metXY_40000 = Get("xy_40000");
            metXY_60000 = Get("xy_60000");
            metSM_00000 = Get("sm_00000");
            metSM_30000 = Get("sm_30000");
            metSM_40000 = Get("sm_40000");
            metSM_60000 = Get("sm_60000");

            metGG_00000 = Get("gg_00000");
            metGG_30000 = metSM_30000;
            metGG_40000 = Get("gg_40000");
            metGG_60000 = metSM_60000;

            Sanitize();

            g4items = (string[])itemlist.Clone();
            Get("mail4").CopyTo(g4items, 137);

            InitializeDataSources();
        }

        private static string[] SanitizeMetStringsCXD(string[] cxd)
        {
            // Mark duplicate locations with their index
            var metSanitize = (string[])cxd.Clone();
            for (int i = 0; i < metSanitize.Length; i++)
            {
                if (cxd.Count(z => z == metSanitize[i]) > 1)
                    metSanitize[i] += $" [{i:000}]";
            }

            return metSanitize;
        }

        private void Sanitize()
        {
            SanitizeItemNames();
            SanitizeMetLocations();

            // Replace the Egg Name with ---; egg name already stored to eggname
            specieslist[0] = "---";
            // Fix (None) tags
            var none = $"({itemlist[0]})";
            abilitylist[0] = itemlist[0] = movelist[0] = metXY_00000[0] = metBW2_00000[0] = metHGSS_00000[0] = metCXD_00000[0] = puffs[0] = none;
        }

        private void SanitizeItemNames()
        {
            // Fix Item Names (Duplicate entries)
            var HM06 = itemlist[425];
            var HM0 = HM06.Substring(0, HM06.Length - 1); // language ambiguous!
            itemlist[426] = $"{HM0}7 (G4)";
            itemlist[427] = $"{HM0}8 (G4)";
            itemlist[456] += " (HG/SS)"; // S.S. Ticket
            itemlist[736] += " (OR/AS)"; // S.S. Ticket
            itemlist[463] += " (DPPt)"; // Storage Key
            itemlist[734] += " (OR/AS)"; // Storage Key
            itemlist[476] += " (HG/SS)"; // Basement Key
            itemlist[723] += " (OR/AS)"; // Basement Key
            itemlist[621] += " (M)"; // Xtransceiver
            itemlist[626] += " (F)"; // Xtransceiver
            itemlist[629] += " (2)"; // DNA Splicers
            itemlist[637] += " (2)"; // Dropped Item
            itemlist[707] += " (2)"; // Travel Trunk
            itemlist[713] += " (2)"; // Alt Bike
            itemlist[714] += " (2)"; // Holo Caster
            itemlist[729] += " (1)"; // Meteorite
            itemlist[740] += " (2)"; // Contest Costume
            itemlist[751] += " (2)"; // Meteorite
            itemlist[771] += " (3)"; // Meteorite
            itemlist[772] += " (4)"; // Meteorite
            itemlist[842] += " (SM)"; // Fishing Rod
            itemlist[945] += " (2)"; // Used Solarizer
            itemlist[946] += " (2)"; // Used Lunarizer

            itemlist[873] += " (GP/GE)"; // S.S. Ticket
            itemlist[459] += " (HG/SS)"; // Parcel
            itemlist[467] += " (Pt)"; // Secret Key
            itemlist[475] += " (HG/SS)"; // Card Key
            itemlist[894] += " (GP)"; // Leaf Letter
            itemlist[895] += " (GE)"; // Leaf Letter

            // some languages have same names for other items!
            itemlist[878] += " (GP/GE)"; // Lift Key (Elevator Key=700)
            itemlist[479] += " (HG/SS)"; // Lost Item (Dropped Item=636)

            // Append Z-Crystal flagging
            foreach (var i in Legal.Pouch_ZCrystal_USUM)
                itemlist[i] += " [Z]";

            for (int i = 12; i <= 29; i++) // Differentiate DNA Samples
                g3coloitems[500 + i] += $" ({i - 11:00})";
            // differentiate G3 Card Key from Colo
            g3coloitems[500 + 10] += " (COLO)";
        }

        private void SanitizeMetLocations()
        {
            // Fix up some of the Location strings to make them more descriptive
            SanitizeMetG5BW();
            SanitizeMetG6XY();
            SanitizeMetG7SM();
        }

        private void SanitizeMetG5BW()
        {
            metHGSS_02000[1] += $" ({NPC})";     // Anything from an NPC
            metHGSS_02000[2] += $" ({EggName})"; // Egg From Link Trade
            metBW2_00000[36] = $"{metBW2_00000[84]}/{metBW2_00000[36]}"; // Cold Storage in BW = PWT in BW2
            metBW2_00000[40] += "(B/W)"; // Victory Road in BW
            metBW2_00000[134] += "(B2/W2)"; // Victory Road in B2W2
            // BW2 Entries from 76 to 105 are for Entralink in BW
            for (int i = 76; i < 106; i++)
                metBW2_00000[i] += "●";

            // Collision between 40002 (legal) and 00002 (illegal) "Faraway place"
            if (metBW2_00000[2] == metBW2_40000[2 - 1])
                metBW2_00000[2] += " (2)";

            // Localize the Poketransfer to the language (30001)
            metBW2_30000[1 - 1] = GameInfo.GetTransporterName(lang); // Default to English
            metBW2_30000[2 - 1] += $" ({NPC})";             // Anything from an NPC
            metBW2_30000[3 - 1] += $" ({EggName})";         // Link Trade (Egg)

            // Zorua/Zoroark events
            metBW2_30000[10 - 1] = $"{specieslist[251]} ({specieslist[570]} 1)"; // Celebi's Zorua Event
            metBW2_30000[11 - 1] = $"{specieslist[251]} ({specieslist[570]} 2)"; // Celebi's Zorua Event
            metBW2_30000[12 - 1] = $"{specieslist[571]} (1)"; // Zoroark
            metBW2_30000[13 - 1] = $"{specieslist[571]} (2)"; // Zoroark

            metBW2_60000[3 - 1] += $" ({EggName})";  // Egg Treasure Hunter/Breeder, whatever...
        }

        private void SanitizeMetG6XY()
        {
            metXY_00000[104] += " (X/Y)";      // Victory Road
            metXY_00000[106] += " (X/Y)";      // Pokémon League
            metXY_00000[202] += " (OR/AS)";    // Pokémon League
            metXY_00000[298] += " (OR/AS)";    // Victory Road
            metXY_30000[0] += $" ({NPC})";     // Anything from an NPC
            metXY_30000[1] += $" ({EggName})"; // Egg From Link Trade
        }

        private void SanitizeMetG7SM()
        {
            // Sun/Moon duplicates -- elaborate!
            var metSM_00000_good = (string[])metSM_00000.Clone();
            for (int i = 0; i < metSM_00000.Length; i += 2)
            {
                var nextLoc = metSM_00000[i + 1];
                if (!string.IsNullOrWhiteSpace(nextLoc) && nextLoc[0] != '[')
                    metSM_00000_good[i] += $" ({nextLoc})";
                if (i > 0 && !string.IsNullOrWhiteSpace(metSM_00000_good[i]) && metSM_00000_good.Take(i - 1).Contains(metSM_00000_good[i]))
                    metSM_00000_good[i] += $" ({metSM_00000_good.Take(i - 1).Count(s => s == metSM_00000_good[i]) + 1})";
            }
            Array.Copy(metSM_00000, 194, metSM_00000_good, 194, 4); // Restore Island Names (unused)
            metSM_00000_good.CopyTo(metSM_00000, 0);

            metSM_30000[0] += $" ({NPC})";      // Anything from an NPC
            metSM_30000[1] += $" ({EggName})";  // Egg From Link Trade
            for (int i = 2; i <= 5; i++) // distinguish first set of regions (unused) from second (used)
                metSM_30000[i] += " (-)";
        }

        public IReadOnlyList<string> GetItemStrings(int generation, GameVersion game = GameVersion.Any)
        {
            switch (generation)
            {
                case 0: return Array.Empty<string>();
                case 1: return g1items;
                case 2: return g2items;
                case 3: return GetItemStrings3(game);
                case 4: return g4items; // mail names changed 4->5
                default: return itemlist;
            }
        }

        private string[] GetItemStrings3(GameVersion game)
        {
            switch (game)
            {
                case GameVersion.COLO:
                    return g3coloitems;
                case GameVersion.XD:
                    return g3xditems;
                default:
                    if (Legal.EReaderBerryIsEnigma)
                        return g3items;

                    var g3itemsEBerry = (string[])g3items.Clone();
                    g3itemsEBerry[175] = Legal.EReaderBerryDisplayName;
                    return g3itemsEBerry;
            }
        }

        // DataSource providing
        public IReadOnlyList<ComboItem> ItemDataSource { get; private set; }
        public IReadOnlyList<ComboItem> SpeciesDataSource { get; private set; }
        public IReadOnlyList<ComboItem> BallDataSource { get; private set; }
        public IReadOnlyList<ComboItem> NatureDataSource { get; private set; }
        public IReadOnlyList<ComboItem> AbilityDataSource { get; private set; }
        public IReadOnlyList<ComboItem> VersionDataSource { get; private set; }
        public IReadOnlyList<ComboItem> LegalMoveDataSource { get; private set; }
        public IReadOnlyList<ComboItem> HaXMoveDataSource { get; private set; }
        public IReadOnlyList<ComboItem> MoveDataSource { get; set; }
        public IReadOnlyList<ComboItem> EncounterTypeDataSource { get; private set; }

        private IReadOnlyList<ComboItem> MetGen2 { get; set; }
        private IReadOnlyList<ComboItem> MetGen3 { get; set; }
        private IReadOnlyList<ComboItem> MetGen3CXD { get; set; }
        private IReadOnlyList<ComboItem> MetGen4 { get; set; }
        private IReadOnlyList<ComboItem> MetGen5 { get; set; }
        private IReadOnlyList<ComboItem> MetGen6 { get; set; }
        private IReadOnlyList<ComboItem> MetGen7 { get; set; }
        private IReadOnlyList<ComboItem> MetGen7GG { get; set; }

        public MemoryStrings Memories { get; private set; }

        // ignores Poke/Great/Ultra
        private static readonly int[] ball_nums = { 007, 576, 013, 492, 497, 014, 495, 493, 496, 494, 011, 498, 008, 006, 012, 015, 009, 005, 499, 010, 001, 016, 851 };
        private static readonly int[] ball_vals = { 007, 025, 013, 017, 022, 014, 020, 018, 021, 019, 011, 023, 008, 006, 012, 015, 009, 005, 024, 010, 001, 016, 026 };

        private void InitializeDataSources()
        {
            BallDataSource = Util.GetVariedCBListBall(itemlist, ball_nums, ball_vals);
            SpeciesDataSource = Util.GetCBList(specieslist);
            NatureDataSource = Util.GetCBList(natures);
            AbilityDataSource = Util.GetCBList(abilitylist);
            VersionDataSource = GetVersionList();
            EncounterTypeDataSource = Util.GetCBList(encountertypelist, new[] {0}, Legal.Gen4EncounterTypes);

            HaXMoveDataSource = Util.GetCBList(movelist);
            MoveDataSource = LegalMoveDataSource = HaXMoveDataSource.Where(m => !Legal.Z_Moves.Contains(m.Value)).ToList();
            InitializeMetSources();
            Memories = new MemoryStrings(this);
        }

        private IReadOnlyList<ComboItem> GetVersionList()
        {
            var ver = Util.GetCBList(gamelist,
                Legal.Games_7gg,
                Legal.Games_7usum, Legal.Games_7sm,
                Legal.Games_6oras, Legal.Games_6xy,
                Legal.Games_5, Legal.Games_4, Legal.Games_4e, Legal.Games_4r,
                Legal.Games_3, Legal.Games_3e, Legal.Games_3r, Legal.Games_3s);
            ver.AddRange(Util.GetCBList(gamelist, Legal.Games_7vc1).OrderBy(g => g.Value)); // stuff to end unsorted
            ver.AddRange(Util.GetCBList(gamelist, Legal.Games_7vc2).OrderBy(g => g.Value)); // stuff to end unsorted
            ver.AddRange(Util.GetCBList(gamelist, Legal.Games_7go).OrderBy(g => g.Value)); // stuff to end unsorted
            return ver;
        }

        private void InitializeMetSources()
        {
            // Gen 2
            {
                var met_list = Util.GetCBList(metGSC_00000, Enumerable.Range(0, 0x5F).ToArray());
                Util.AddCBWithOffset(met_list, metGSC_00000, 00000, 0x7E, 0x7F);
                MetGen2 = met_list;
            }
            // Gen 3
            {
                var met_list = Util.GetCBList(metRSEFRLG_00000, Enumerable.Range(0, 213).ToArray());
                Util.AddCBWithOffset(met_list, metRSEFRLG_00000, 00000, 253, 254, 255);
                MetGen3 = met_list;

                MetGen3CXD = Util.GetCBList(metCXD_00000, Enumerable.Range(0, metCXD_00000.Length).ToArray()).Where(c => c.Text.Length > 0).ToList();
            }
            // Gen 4
            {
                var met_list = Util.GetCBList(metHGSS_00000, 0);
                Util.AddCBWithOffset(met_list, metHGSS_02000, 2000, Locations.Daycare4);
                Util.AddCBWithOffset(met_list, metHGSS_02000, 2000, Locations.LinkTrade4);
                Util.AddCBWithOffset(met_list, metHGSS_03000, 3000, Locations.Ranger4);
                Util.AddCBWithOffset(met_list, metHGSS_00000, 0000, Legal.Met_HGSS_0);
                Util.AddCBWithOffset(met_list, metHGSS_02000, 2000, Legal.Met_HGSS_2);
                Util.AddCBWithOffset(met_list, metHGSS_03000, 3000, Legal.Met_HGSS_3);
                MetGen4 = met_list;
            }
            // Gen 5
            {
                var met_list = Util.GetCBList(metBW2_00000, 0);
                Util.AddCBWithOffset(met_list, metBW2_60000, 60001, Locations.Daycare5);
                Util.AddCBWithOffset(met_list, metBW2_30000, 30001, Locations.LinkTrade5);
                Util.AddCBWithOffset(met_list, metBW2_00000, 00000, Legal.Met_BW2_0);
                Util.AddCBWithOffset(met_list, metBW2_30000, 30001, Legal.Met_BW2_3);
                Util.AddCBWithOffset(met_list, metBW2_40000, 40001, Legal.Met_BW2_4);
                Util.AddCBWithOffset(met_list, metBW2_60000, 60001, Legal.Met_BW2_6);
                MetGen5 = met_list;
            }
            // Gen 6
            {
                var met_list = Util.GetCBList(metXY_00000, 0);
                Util.AddCBWithOffset(met_list, metXY_60000, 60001, Locations.Daycare5);
                Util.AddCBWithOffset(met_list, metXY_30000, 30001, Locations.LinkTrade6);
                Util.AddCBWithOffset(met_list, metXY_00000, 00000, Legal.Met_XY_0);
                Util.AddCBWithOffset(met_list, metXY_30000, 30001, Legal.Met_XY_3);
                Util.AddCBWithOffset(met_list, metXY_40000, 40001, Legal.Met_XY_4);
                Util.AddCBWithOffset(met_list, metXY_60000, 60001, Legal.Met_XY_6);
                MetGen6 = met_list;
            }
            // Gen 7
            {
                var met_list = Util.GetCBList(metSM_00000, 0);
                Util.AddCBWithOffset(met_list, metSM_60000, 60001, Locations.Daycare5);
                Util.AddCBWithOffset(met_list, metSM_30000, 30001, Locations.LinkTrade6);
                Util.AddCBWithOffset(met_list, metSM_00000, 00000, Legal.Met_SM_0);
                Util.AddCBWithOffset(met_list, metSM_30000, 30001, Legal.Met_SM_3);
                Util.AddCBWithOffset(met_list, metSM_40000, 40001, Legal.Met_SM_4);
                Util.AddCBWithOffset(met_list, metSM_60000, 60001, Legal.Met_SM_6);
                MetGen7 = met_list;
            }
            // Gen 7 GG
            {
                var met_list = Util.GetCBList(metGG_00000, 0);
                Util.AddCBWithOffset(met_list, metGG_60000, 60001, 60002);
                Util.AddCBWithOffset(met_list, metGG_30000, 30001, Locations.LinkTrade6);
                Util.AddCBWithOffset(met_list, metGG_00000, 00000, Legal.Met_GG_0);
                Util.AddCBWithOffset(met_list, metGG_30000, 30001, Legal.Met_GG_3);
                Util.AddCBWithOffset(met_list, metGG_40000, 40001, Legal.Met_GG_4);
                Util.AddCBWithOffset(met_list, metGG_60000, 60001, Legal.Met_GG_6);
                MetGen7GG = met_list;
            }
        }

        public void SetItemDataSource(GameVersion game, int generation, int MaxItemID, IEnumerable<ushort> allowed = null, bool HaX = false)
        {
            var items = GetItemStrings(generation, game);
            ItemDataSource = Util.GetCBList(items, (allowed == null || HaX ? Enumerable.Range(0, MaxItemID) : allowed.Select(i => (int)i)).ToArray());
        }

        /// <summary>
        /// Fetches a Met Location list for a <see cref="version"/> that has been transferred away from and overwritten.
        /// </summary>
        /// <param name="version">Origin version</param>
        /// <param name="currentGen">Current savefile generation</param>
        /// <param name="egg">True if an egg location list, false if a regular met location list</param>
        /// <returns>Met location list</returns>
        public IReadOnlyList<ComboItem> GetLocationList(GameVersion version, int currentGen, bool egg = false)
        {
            if (currentGen == 2)
                return MetGen2;

            if (egg && version < GameVersion.W && currentGen >= 5)
                return MetGen4;

            switch (version)
            {
                case GameVersion.CXD:
                    if (currentGen == 3)
                        return MetGen3CXD;
                    break;

                case GameVersion.R:
                case GameVersion.S:
                    if (currentGen == 3)
                        return MetGen3.OrderByDescending(loc => loc.Value <= 87).ToList(); // Ferry
                    break;
                case GameVersion.E:
                    if (currentGen == 3)
                        return MetGen3.OrderByDescending(loc => loc.Value <= 87 || (loc.Value >= 196 && loc.Value <= 212)).ToList(); // Trainer Hill
                    break;
                case GameVersion.FR:
                case GameVersion.LG:
                    if (currentGen == 3)
                        return MetGen3.OrderByDescending(loc => loc.Value > 87 && loc.Value < 197).ToList(); // Celadon Dept.
                    break;

                case GameVersion.D:
                case GameVersion.P:
                    if (currentGen == 4 || (currentGen >= 5 && egg))
                        return MetGen4.Take(4).Concat(MetGen4.Skip(4).OrderByDescending(loc => loc.Value <= 111)).ToList(); // Battle Park
                    break;

                case GameVersion.Pt:
                    if (currentGen == 4 || (currentGen >= 5 && egg))
                        return MetGen4.Take(4).Concat(MetGen4.Skip(4).OrderByDescending(loc => loc.Value <= 125)).ToList(); // Rock Peak Ruins
                    break;

                case GameVersion.HG:
                case GameVersion.SS:
                    if (currentGen == 4 || (currentGen >= 5 && egg))
                        return MetGen4.Take(4).Concat(MetGen4.Skip(4).OrderByDescending(loc => loc.Value > 125 && loc.Value < 234)).ToList(); // Celadon Dept.
                    break;

                case GameVersion.B:
                case GameVersion.W:
                    return MetGen5;

                case GameVersion.B2:
                case GameVersion.W2:
                    return MetGen5.Take(3).Concat(MetGen5.Skip(3).OrderByDescending(loc => loc.Value <= 116)).ToList(); // Abyssal Ruins

                case GameVersion.X:
                case GameVersion.Y:
                    return MetGen6.Take(3).Concat(MetGen6.Skip(3).OrderByDescending(loc => loc.Value <= 168)).ToList(); // Unknown Dungeon

                case GameVersion.OR:
                case GameVersion.AS:
                    return MetGen6.Take(3).Concat(MetGen6.Skip(3).OrderByDescending(loc => loc.Value > 168 && loc.Value <= 354)).ToList(); // Secret Base

                case GameVersion.SN:
                case GameVersion.MN:
                    return MetGen7.Take(3).Concat(MetGen7.Skip(3).OrderByDescending(loc => loc.Value < 200)).ToList(); // Outer Cape

                case GameVersion.US:
                case GameVersion.UM:

                case GameVersion.RD:
                case GameVersion.BU:
                case GameVersion.GN:
                case GameVersion.YW:

                case GameVersion.GD:
                case GameVersion.SV:
                case GameVersion.C:
                    return MetGen7.Take(3).Concat(MetGen7.Skip(3).OrderByDescending(loc => loc.Value < 234)).ToList(); // Dividing Peak Tunnel

                case GameVersion.GP:
                case GameVersion.GE:
                case GameVersion.GO:
                    return MetGen7GG.Take(3).Concat(MetGen7GG.Skip(3).OrderByDescending(loc => loc.Value <= 54)).ToList(); // Pokémon League
            }

            return GetLocationListModified(version, currentGen);
        }

        /// <summary>
        /// Fetches a Met Location list for a <see cref="version"/> that has been transferred away from and overwritten.
        /// </summary>
        /// <param name="version">Origin version</param>
        /// <param name="currentGen">Current savefile generation</param>
        /// <returns>Met location list</returns>
        private IReadOnlyList<ComboItem> GetLocationListModified(GameVersion version, int currentGen)
        {
            if (version <= GameVersion.CXD && currentGen == 4)
            {
                return MetGen4.Where(loc => loc.Value == Locations.Transfer3) // Pal Park to front
                    .Concat(MetGen4.Take(4))
                    .Concat(MetGen4.Skip(4).Where(loc => loc.Value != Locations.Transfer3)).ToList();
            }

            if (version < GameVersion.X && currentGen >= 5) // PokéTransfer to front
            {
                return MetGen5.Where(loc => loc.Value == Locations.Transfer4)
                    .Concat(MetGen5.Take(3))
                    .Concat(MetGen5.Skip(3).Where(loc => loc.Value != Locations.Transfer4)).ToList();
            }

            return Array.Empty<ComboItem>();
        }

        public static IReadOnlyList<ComboItem> LanguageDataSource(int gen)
        {
            var languages = LanguageList.ToList();
            if (gen == 3)
                languages.RemoveAll(l => l.Value >= (int)LanguageID.Korean);
            else if (gen < 7)
                languages.RemoveAll(l => l.Value > (int)LanguageID.Korean);
            return languages;
        }

        public IReadOnlyList<ComboItem> GetAbilityDataSource(IEnumerable<int> abils)
        {
            return abils.Select(GetItem).ToList();
            ComboItem GetItem(int ability, int index) => new ComboItem
            {
                Value = ability,
                Text = abilitylist[ability] + abilIdentifier[index]
            };
        }
    }
}