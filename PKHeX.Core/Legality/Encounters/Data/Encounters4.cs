using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static PKHeX.Core.EncounterUtil;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 4 Encounters
    /// </summary>
    internal static class Encounters4
    {
        internal static readonly EncounterArea4DPPt[] SlotsD, SlotsP, SlotsPt;
        internal static readonly EncounterArea4HGSS[] SlotsHG, SlotsSS;
        internal static readonly EncounterStatic[] StaticD, StaticP, StaticPt, StaticHG, StaticSS;

        static Encounters4()
        {
            MarkG4PokeWalker(Encounter_PokeWalker);
            StaticD = GetStaticEncounters(Encounter_DPPt, GameVersion.D);
            StaticP = GetStaticEncounters(Encounter_DPPt, GameVersion.P);
            StaticPt = GetStaticEncounters(Encounter_DPPt, GameVersion.Pt);
            var staticHGSS = Encounter_HGSS.Concat(Encounter_PokeWalker).ToArray();
            StaticHG = GetStaticEncounters(staticHGSS, GameVersion.HG);
            StaticSS = GetStaticEncounters(staticHGSS, GameVersion.SS);

            static byte[][] get(string resource, string ident)
                => BinLinker.Unpack(Util.GetBinaryResource($"encounter_{resource}.pkl"), ident);

            var D_Slots = EncounterArea4DPPt.GetArray4DPPt(get("d", "da"));
            var P_Slots = EncounterArea4DPPt.GetArray4DPPt(get("p", "pe"));
            var Pt_Slots = EncounterArea4DPPt.GetArray4DPPt(get("pt", "pt"), true);
            var HG_Slots = EncounterArea4HGSS.GetArray4HGSS(get("hg", "hg"));
            var SS_Slots = EncounterArea4HGSS.GetArray4HGSS(get("ss", "ss"));

            var DP_Feebas = GetFeebasArea(D_Slots[10]);
            var Pt_Feebas = GetFeebasArea(Pt_Slots[10]);
            var HG_Headbutt_Slots = EncounterArea4HGSS.GetArray4HGSS_Headbutt(get("hb_hg", "hg"));
            var SS_Headbutt_Slots = EncounterArea4HGSS.GetArray4HGSS_Headbutt(get("hb_ss", "ss"));

            var D_HoneyTrees_Slots = SlotsD_HoneyTree.Clone(HoneyTreesLocation);
            var P_HoneyTrees_Slots = SlotsP_HoneyTree.Clone(HoneyTreesLocation);
            var Pt_HoneyTrees_Slots = SlotsPt_HoneyTree.Clone(HoneyTreesLocation);

            MarkG4SwarmSlots(HG_Slots, SlotsHG_Swarm);
            MarkG4SwarmSlots(SS_Slots, SlotsSS_Swarm);

            MarkEncounterTypeData(D_Slots, P_Slots, Pt_Slots, HG_Slots, SS_Slots);

            ReduceAreasSize(ref D_Slots);
            ReduceAreasSize(ref P_Slots);
            ReduceAreasSize(ref Pt_Slots);
            ReduceAreasSize(ref HG_Slots);
            ReduceAreasSize(ref SS_Slots);
            ReduceAreasSize(ref HG_Headbutt_Slots);
            ReduceAreasSize(ref SS_Headbutt_Slots);

            MarkG4SlotsGreatMarsh(D_Slots, 52);
            MarkG4SlotsGreatMarsh(P_Slots, 52);
            MarkG4SlotsGreatMarsh(Pt_Slots, 52);

            MarkEncounterAreaArray(D_HoneyTrees_Slots, P_HoneyTrees_Slots, Pt_HoneyTrees_Slots, DP_GreatMarshAlt, Pt_GreatMarshAlt, DPPt_Unown, DP_Feebas, Pt_Feebas);
            MarkEncounterAreaArray(HG_Headbutt_Slots, SS_Headbutt_Slots, SlotsHGSSAlt);

            SlotsD = AddExtraTableSlots(D_Slots, D_HoneyTrees_Slots, DP_GreatMarshAlt, DPPt_Unown, DP_Feebas);
            SlotsP = AddExtraTableSlots(P_Slots, P_HoneyTrees_Slots, DP_GreatMarshAlt, DPPt_Unown, DP_Feebas);
            SlotsPt = AddExtraTableSlots(Pt_Slots, Pt_HoneyTrees_Slots, Pt_GreatMarshAlt, DPPt_Unown, Pt_Feebas);
            SlotsHG = AddExtraTableSlots(HG_Slots, HG_Headbutt_Slots, SlotsHGSSAlt);
            SlotsSS = AddExtraTableSlots(SS_Slots, SS_Headbutt_Slots, SlotsHGSSAlt);

            MarkDPPtEncounterTypeSlots(SlotsD);
            MarkDPPtEncounterTypeSlots(SlotsP);
            MarkDPPtEncounterTypeSlots(SlotsPt);
            MarkHGSSEncounterTypeSlots(SlotsHG);
            MarkHGSSEncounterTypeSlots(SlotsSS);

            MarkEncountersGeneration(4, SlotsD, SlotsP, SlotsPt, SlotsHG, SlotsSS);
            MarkEncountersGeneration(4, StaticD, StaticP, StaticPt, StaticHG, StaticSS, TradeGift_DPPt, TradeGift_HGSS);

            MarkEncounterTradeStrings(TradeGift_DPPt, TradeDPPt);
            MarkEncounterTradeStrings(TradeGift_HGSS, TradeHGSS);

            foreach (var t in RanchGifts)
                t.TrainerNames = RanchOTNames;

            DP_GreatMarshAlt.SetVersion(GameVersion.DP);
            DPPt_Unown.SetVersion(GameVersion.DPPt);
            DP_Feebas.SetVersion(GameVersion.DP);
            SlotsHGSSAlt.SetVersion(GameVersion.HGSS);
            SlotsD.SetVersion(GameVersion.D);
            SlotsP.SetVersion(GameVersion.P);
            SlotsPt.SetVersion(GameVersion.Pt);
            SlotsHG.SetVersion(GameVersion.HG);
            SlotsSS.SetVersion(GameVersion.SS);
            Encounter_DPPt.SetVersion(GameVersion.DPPt);
            Encounter_HGSS.SetVersion(GameVersion.HGSS);
            TradeGift_DPPt.SetVersion(GameVersion.DPPt);
            TradeGift_HGSS.SetVersion(GameVersion.HGSS);
        }

        private static EncounterArea4DPPt[] GetFeebasArea(EncounterArea4DPPt template)
        {
            Debug.Assert(template.Location == 50); // Mt Coronet
            Debug.Assert(template.Slots.Last().Species == (int)Species.Whiscash);
            var slots = template.Slots.Where(z => z.Type.IsFishingRodType()).Select(z => z.Clone()).ToArray();
            Debug.Assert(slots[0].Species == (int)Species.Magikarp);
            foreach (var s in slots)
            {
                s.Species = (int)Species.Feebas;
                s.TypeEncounter = EncounterType.Surfing_Fishing;
            }

            var area = new EncounterArea4DPPt
            {
                Location = template.Location,
                Slots = slots,
            };
            return new[] {area};
        }

        private static void MarkEncounterTypeData(EncounterArea4[] D_Slots, EncounterArea4[] P_Slots, EncounterArea4[] Pt_Slots, EncounterArea4[] HG_Slots, EncounterArea4[] SS_Slots)
        {
            // Shellos & Gastrodon
            MarkG4AltFormSlots(D_Slots, 422, 1, Shellos_EastSeaLocation_DP);
            MarkG4AltFormSlots(D_Slots, 423, 1, Gastrodon_EastSeaLocation_DP);
            MarkG4AltFormSlots(P_Slots, 422, 1, Shellos_EastSeaLocation_DP);
            MarkG4AltFormSlots(P_Slots, 423, 1, Gastrodon_EastSeaLocation_DP);
            MarkG4AltFormSlots(Pt_Slots, 422, 1, Shellos_EastSeaLocation_Pt);
            MarkG4AltFormSlots(Pt_Slots, 423, 1, Gastrodon_EastSeaLocation_Pt);

            const int Route209 = 24;
            MarkDPPtEncounterTypeSlots_MultipleTypes(D_Slots, Route209, EncounterType.Building_EnigmaStone, 1);
            MarkDPPtEncounterTypeSlots_MultipleTypes(P_Slots, Route209, EncounterType.Building_EnigmaStone, 1);
            MarkDPPtEncounterTypeSlots_MultipleTypes(Pt_Slots, Route209, EncounterType.Building_EnigmaStone, 1);
            const int StarkMountain = 84;
            MarkDPPtEncounterTypeSlots_MultipleTypes(D_Slots, StarkMountain, EncounterType.Cave_HallOfOrigin, 1);
            MarkDPPtEncounterTypeSlots_MultipleTypes(P_Slots, StarkMountain, EncounterType.Cave_HallOfOrigin, 1);
            MarkDPPtEncounterTypeSlots_MultipleTypes(Pt_Slots, StarkMountain, EncounterType.Cave_HallOfOrigin, 1);
            const int MtCoronet = 50;
            MarkDPPtEncounterTypeSlots_MultipleTypes(D_Slots, MtCoronet, EncounterType.Cave_HallOfOrigin, DPPt_MtCoronetExteriorEncounters);
            MarkDPPtEncounterTypeSlots_MultipleTypes(P_Slots, MtCoronet, EncounterType.Cave_HallOfOrigin, DPPt_MtCoronetExteriorEncounters);
            MarkDPPtEncounterTypeSlots_MultipleTypes(Pt_Slots, MtCoronet, EncounterType.Cave_HallOfOrigin, DPPt_MtCoronetExteriorEncounters);
            const int RuinsOfAlph = 209;
            MarkHGSSEncounterTypeSlots_MultipleTypes(HG_Slots, RuinsOfAlph, EncounterType.Cave_HallOfOrigin, 1);
            MarkHGSSEncounterTypeSlots_MultipleTypes(SS_Slots, RuinsOfAlph, EncounterType.Cave_HallOfOrigin, 1);
            MarkSpecific(HG_Slots, RuinsOfAlph, SlotType.Rock_Smash, EncounterType.DialgaPalkia);
            MarkSpecific(SS_Slots, RuinsOfAlph, SlotType.Rock_Smash, EncounterType.DialgaPalkia);
            const int MtSilver = 219;
            MarkHGSSEncounterTypeSlots_MultipleTypes(HG_Slots, MtSilver, EncounterType.Cave_HallOfOrigin, HGSS_MtSilverCaveExteriorEncounters);
            MarkHGSSEncounterTypeSlots_MultipleTypes(SS_Slots, MtSilver, EncounterType.Cave_HallOfOrigin, HGSS_MtSilverCaveExteriorEncounters);
            const int Cianwood = 130;
            MarkHGSSEncounterTypeSlots_MultipleTypes(HG_Slots, Cianwood, EncounterType.RockSmash);
            MarkHGSSEncounterTypeSlots_MultipleTypes(SS_Slots, Cianwood, EncounterType.RockSmash);
        }

        private static void MarkG4PokeWalker(IEnumerable<EncounterStatic> t)
        {
            foreach (EncounterStatic s in t)
            {
                s.Location = 233;  //Pokéwalker
                s.Gift = true;    //Pokeball only
                s.Version = GameVersion.HGSS;
            }
        }

        private static void MarkG4SlotsGreatMarsh(IEnumerable<EncounterArea> Areas, int location)
        {
            foreach (EncounterArea Area in Areas.Where(a => a.Location == location))
            {
                foreach (EncounterSlot Slot in Area.Slots)
                    Slot.Type |= SlotType.Safari;
            }
        }

        private static void MarkG4SwarmSlots<T>(IEnumerable<T> Areas, T[] SwarmAreas) where T : EncounterArea
        {
            // Grass Swarm slots replace slots 0 and 1 from encounters data
            // for surfing only replace slots 0 from encounters data
            // for fishing replace one or several random slots from encounters data, but all slots have the same level, it's ok to only replace the first
            // Species id are not included in encounter tables but levels can be copied from the encounter raw data
            foreach (var Area in Areas)
            {
                var SwarmSlots = SwarmAreas.Where(a => a.Location == Area.Location).SelectMany(s => s.Slots);
                var OutputSlots = new List<EncounterSlot>();
                foreach (EncounterSlot SwarmSlot in SwarmSlots)
                {
                    int slotsnum = SwarmSlot.Type == SlotType.Grass ? 2 : 1;
                    foreach (var swarmSlot in Area.Slots.Where(s => s.Type == SwarmSlot.Type).Take(slotsnum).Select(slot => slot.Clone()))
                    {
                        swarmSlot.Species = SwarmSlot.Species;
                        if (swarmSlot.Species == (int)Species.Mawile) // edge case, Mawile is only swarm subject to magnet pull (no other steel types in area)
                        {
                            swarmSlot.Permissions.MagnetPullIndex = swarmSlot.SlotNumber;
                            swarmSlot.Permissions.MagnetPullCount = 2;
                        }
                        OutputSlots.Add(swarmSlot);
                    }
                }
                Area.Slots = Area.Slots.Concat(OutputSlots).Where(a => a.Species > 0).ToArray();
            }
        }

        // Gen 4 raw encounter data does not contains info for alt slots
        // Shellos and Gastrodon East Sea form should be modified
        private static void MarkG4AltFormSlots(IEnumerable<EncounterArea4> Areas, int Species, int form, int[] Locations)
        {
            foreach (var Area in Areas.Where(a => Locations.Contains(a.Location)))
            {
                foreach (EncounterSlot Slot in Area.Slots.Where(s => s.Species == Species))
                    Slot.Form = form;
            }
        }

        private static EncounterType GetEncounterTypeBySlotDPPt(SlotType Type, EncounterType GrassType)
        {
            switch (Type)
            {
                case SlotType.Pokeradar:
                case SlotType.Pokeradar_Safari:
                case SlotType.Swarm:
                case SlotType.Grass: return GrassType;
                case SlotType.Surf:
                case SlotType.Old_Rod:
                case SlotType.Good_Rod:
                case SlotType.Super_Rod:
                case SlotType.Surf_Safari:
                case SlotType.Old_Rod_Safari:
                case SlotType.Good_Rod_Safari:
                case SlotType.Super_Rod_Safari: return EncounterType.Surfing_Fishing;
                case SlotType.Grass_Safari: return EncounterType.MarshSafari;
                case SlotType.HoneyTree: return EncounterType.None;
            }
            return EncounterType.None;
        }

        private static EncounterType GetEncounterTypeBySlotHGSS(SlotType Type, EncounterType GrassType, EncounterType HeadbuttType)
        {
            switch (Type)
            {
                // HGSS Safari encounters have normal water/grass encounter type, not safari encounter type
                case SlotType.Grass:
                case SlotType.Grass_Safari:
                case SlotType.BugContest: return GrassType;

                case SlotType.Surf:
                case SlotType.Old_Rod:
                case SlotType.Good_Rod:
                case SlotType.Super_Rod:
                case SlotType.Surf_Safari:
                case SlotType.Old_Rod_Safari:
                case SlotType.Good_Rod_Safari:
                case SlotType.Super_Rod_Safari: return EncounterType.Surfing_Fishing;

                case SlotType.Rock_Smash:
                    if (GrassType == EncounterType.RockSmash)
                        return EncounterType.RockSmash | EncounterType.Building_EnigmaStone;
                    if (HeadbuttType == EncounterType.Building_EnigmaStone)
                        return HeadbuttType;
                    if (GrassType == EncounterType.Cave_HallOfOrigin)
                        return GrassType;
                    return EncounterType.None;

                case SlotType.Headbutt_Special:
                case SlotType.Headbutt: return HeadbuttType | EncounterType.None;
                    // not sure on if "None" should always be allowed, but this is so uncommon it shouldn't matter (gen7 doesn't keep this value anyway).
            }
            return EncounterType.None;
        }

        private static void MarkDPPtEncounterTypeSlots_MultipleTypes(EncounterArea4[] Areas, int Location, EncounterType NormalEncounterType, params int[] SpecialEncounterFiles)
        {
            var numfile = 0;
            foreach (var Area in Areas.Where(x => x.Location == Location))
            {
                numfile++;
                var GrassType = SpecialEncounterFiles.Contains(numfile) ? EncounterType.TallGrass : NormalEncounterType;
                foreach (EncounterSlot Slot in Area.Slots)
                {
                    Slot.TypeEncounter = GetEncounterTypeBySlotDPPt(Slot.Type, GrassType);
                }
            }
        }

        private static void MarkHGSSEncounterTypeSlots_MultipleTypes(EncounterArea4[] Areas, int Location, EncounterType NormalEncounterType, params int[] SpecialEncounterFile)
        {
            // Area with two different encounter type for grass encounters
            // SpecialEncounterFile is taall grass encounter type, the other files have the normal encounter type for this location
            var HeadbuttType = GetHeadbuttEncounterType(Location);
            var numfile = 0;
            foreach (var Area in Areas.Where(x => x.Location == Location))
            {
                numfile++;
                var GrassType = SpecialEncounterFile.Contains(numfile) ? EncounterType.TallGrass : NormalEncounterType;
                foreach (EncounterSlot Slot in Area.Slots)
                {
                    Slot.TypeEncounter = GetEncounterTypeBySlotHGSS(Slot.Type, GrassType, HeadbuttType);
                }
            }
        }

        private static void MarkSpecific(EncounterArea4[] Areas, int Location, SlotType t, EncounterType val)
        {
            foreach (var Area in Areas.Where(x => x.Location == Location))
            {
                foreach (var s in Area.Slots.Where(s => s.Type == t))
                    s.TypeEncounter = val;
            }
        }

        private static void MarkDPPtEncounterTypeSlots(EncounterArea4[] Areas)
        {
            foreach (var Area in Areas)
            {
                if (DPPt_MixInteriorExteriorLocations.Contains(Area.Location))
                    continue;
                var GrassType = GetGrassType(Area.Location);

                EncounterType GetGrassType(int location)
                {
                    if (location == 70) // Old Chateau
                        return EncounterType.Building_EnigmaStone;
                    if (DPPt_CaveLocations.Contains(Area.Location))
                        return EncounterType.Cave_HallOfOrigin;
                    return EncounterType.TallGrass;
                }
                foreach (EncounterSlot Slot in Area.Slots)
                {
                    if (Slot.TypeEncounter == EncounterType.None) // not defined yet
                        Slot.TypeEncounter = GetEncounterTypeBySlotDPPt(Slot.Type, GrassType);
                }
            }
        }

        private static EncounterType GetHeadbuttEncounterType(int Location)
        {
            if (Location == 195 || Location == 196) // Route 47/48
                return EncounterType.DialgaPalkia | EncounterType.TallGrass;

            // Routes with trees adjacent to water tiles
            var allowsurf = HGSS_SurfingHeadbutt_Locations.Contains(Location);

            // Cities
            if (HGSS_CityLocations.Contains(Location))
            {
                return allowsurf
                    ? EncounterType.Building_EnigmaStone | EncounterType.Surfing_Fishing
                    : EncounterType.Building_EnigmaStone;
            }

            // Caves with no exterior zones
            if (!HGSS_MixInteriorExteriorLocations.Contains(Location) && HGSS_CaveLocations.Contains(Location))
            {
                return allowsurf
                    ? EncounterType.Cave_HallOfOrigin | EncounterType.Surfing_Fishing
                    : EncounterType.Cave_HallOfOrigin;
            }

            // Routes and exterior areas
            // Routes with trees adjacent to grass tiles
            var allowgrass = HGSS_GrassHeadbutt_Locations.Contains(Location);
            if (allowgrass)
            {
                return allowsurf
                    ? EncounterType.TallGrass | EncounterType.Surfing_Fishing
                    : EncounterType.TallGrass;
            }

            return allowsurf
                ? EncounterType.Surfing_Fishing
                : EncounterType.None;
        }

        private static void MarkHGSSEncounterTypeSlots(EncounterArea4[] Areas)
        {
            foreach (var Area in Areas)
            {
                if (HGSS_MixInteriorExteriorLocations.Contains(Area.Location))
                    continue;
                var GrassType = HGSS_CaveLocations.Contains(Area.Location) ? EncounterType.Cave_HallOfOrigin : EncounterType.TallGrass;
                var HeadbuttType = GetHeadbuttEncounterType(Area.Location);
                foreach (EncounterSlot Slot in Area.Slots)
                {
                    if (Slot.TypeEncounter == EncounterType.None) // not defined yet
                        Slot.TypeEncounter = GetEncounterTypeBySlotHGSS(Slot.Type, GrassType, HeadbuttType);
                }
            }
        }

        #region Encounter Types
        private static readonly HashSet<int> DPPt_CaveLocations = new HashSet<int>
        {
            46, // Oreburgh Mine
            50, // Mt. Coronet
            53, // Solaceon Ruins
            54, // Sinnoh Victory Road
            57, // Ravaged Path
            59, // Oreburgh Gate
            62, // Turnback Cave
            64, // Snowpoint Temple
            65, // Wayward Cave
            66, // Ruin Maniac Cave
            67, // Maniac Tunnel
            66, // Ruin Maniac Cave
            69, // Iron Island
            84, // Stark Mountain
        };

        private static readonly HashSet<int> DPPt_MixInteriorExteriorLocations = new HashSet<int>
        {
            24, // Route 209 (Lost Tower)
            50, // Mt Coronet
            84, // Stark Mountain
        };

        private static readonly int[] DPPt_MtCoronetExteriorEncounters =
        {
            4, 5, 70
        };

        /// <summary>
        /// Locations with headbutt trees accessible from Cave tiles
        /// </summary>
        private static readonly HashSet<int> HGSS_CaveLocations = new HashSet<int>
        {
            197, // DIGLETT's Cave
            198, // Mt. Moon
            199, // Cerulean Cave
            200, // Rock Tunnel
            201, // Power Plant
            203, // Seafoam Islands
            204, // Sprout Tower
            205, // Bell Tower
            206, // Burned Tower
            209, // Ruins of Alph
            210, // Union Cave
            211, // SLOWPOKE Well
            214, // Ilex Forest
            216, // Mt. Mortar
            217, // Ice Path
            218, // Whirl Islands
            219, // Mt. Silver Cave
            220, // Dark Cave
            221, // Kanto Victory Road
            223, // Tohjo Falls
            228, // Cliff Cave
            234, // Cliff Edge Gate
        };

        /// <summary>
        /// Locations with headbutt trees accessible from city tiles
        /// </summary>
        private static readonly HashSet<int> HGSS_CityLocations = new HashSet<int>
        {
            126, // New Bark Town
            127, // Cherrygrove City
            128, // Violet City
            129, // Azalea Town
            130, // Cianwood City
            131, // Goldenrod City
            132, // Olivine City
            133, // Ecruteak City
            134, // Mahogany Town
            136, // Blackthorn City
            138, // Pallet Town
            139, // Viridian City
            140, // Pewter City
            141, // Cerulean City
            142, // Lavender Town
            143, // Vermilion City
            144, // Celadon City
            145, // Fuchsia City
            146, // Cinnabar Island
            147, // Indigo Plateau
            148, // Saffron City
            227, // Safari Zone Gate
        };

        /// <summary>
        /// Locations with headbutt trees accessible from water tiles
        /// </summary>
        private static readonly HashSet<int> HGSS_SurfingHeadbutt_Locations = new HashSet<int>
        {
            126, // New Bark Town
            127, // Cherrygrove City
            128, // Violet City
            133, // Ecruteak City
            135, // Lake of Rage
            138, // Pallet Town
            139, // Viridian City
            160, // Route 12
            169, // Route 21
            170, // Route 22
            174, // Route 26
            175, // Route 27
            176, // Route 28
            178, // Route 30
            179, // Route 31
            180, // Route 32
            182, // Route 34
            183, // Route 35
            190, // Route 42
            191, // Route 43
            192, // Route 44
            214, // Ilex Forest
        };

        /// <summary>
        /// Locations with headbutt trees accessible from tall grass tiles
        /// </summary>
        private static readonly HashSet<int> HGSS_GrassHeadbutt_Locations = new HashSet<int>
        {
            137, // Mt. Silver
            149, // Route 1
            150, // Route 2
            151, // Route 3
            152, // Route 4
            153, // Route 5
            154, // Route 6
            155, // Route 7
            159, // Route 11
            161, // Route 13
            163, // Route 15
            164, // Route 16
            169, // Route 21
            170, // Route 22
            174, // Route 26
            175, // Route 27
            176, // Route 28
            177, // Route 29
            178, // Route 30
            179, // Route 31
            180, // Route 32
            181, // Route 33
            182, // Route 34
            183, // Route 35
            184, // Route 36
            185, // Route 37
            186, // Route 38
            187, // Route 39
            191, // Route 43
            192, // Route 44
            194, // Route 46
            195, // Route 47
            196, // Route 48
            219, // Mt. Silver Cave
            224, // Viridian Forest
        };

        private static readonly int[] HGSS_MtSilverCaveExteriorEncounters =
        {
            2, 3
        };

        private static readonly int[] HGSS_MixInteriorExteriorLocations =
        {
            209, // Ruins of Alph
            219, // Mt. Silver Cave
        };

        #endregion
        #region Pokéwalker Encounter
        // all pkm are in Poke Ball and have a met location of "PokeWalker"
        private static readonly EncounterStatic[] Encounter_PokeWalker =
        {
            // Some pkm has a pre-level move, an egg move or even a special move, it might be also available via HM/TM/Tutor
            // Johto/Kanto Courses
            new EncounterStatic { Species = 084, Gender = 1, Level = 08, }, // Doduo
            new EncounterStatic { Species = 115, Gender = 1, Level = 08, }, // Kangaskhan
            new EncounterStatic { Species = 029, Gender = 1, Level = 05, }, // Nidoran♀
            new EncounterStatic { Species = 032, Gender = 0, Level = 05, }, // Nidoran♂
            new EncounterStatic { Species = 016, Gender = 0, Level = 05, }, // Pidgey
            new EncounterStatic { Species = 161, Gender = 1, Level = 05, }, // Sentret
            new EncounterStatic { Species = 202, Gender = 1, Level = 15, }, // Wobbuffet
            new EncounterStatic { Species = 069, Gender = 1, Level = 08, }, // Bellsprout
            new EncounterStatic { Species = 046, Gender = 1, Level = 06, }, // Paras
            new EncounterStatic { Species = 048, Gender = 0, Level = 06, }, // Venonat
            new EncounterStatic { Species = 021, Gender = 0, Level = 05, }, // Spearow
            new EncounterStatic { Species = 043, Gender = 1, Level = 05, }, // Oddish
            new EncounterStatic { Species = 095, Gender = 0, Level = 09, }, // Onix
            new EncounterStatic { Species = 240, Gender = 0, Level = 09, Moves = new[]{241},}, // Magby: Sunny Day
            new EncounterStatic { Species = 066, Gender = 1, Level = 07, }, // Machop
            new EncounterStatic { Species = 077, Gender = 1, Level = 07, }, // Ponyta
            new EncounterStatic { Species = 074, Gender = 1, Level = 08, Moves = new[]{189},}, // Geodude: Mud-Slap
            new EncounterStatic { Species = 163, Gender = 1, Level = 06, }, // Hoothoot
            new EncounterStatic { Species = 054, Gender = 1, Level = 10, }, // Psyduck
            new EncounterStatic { Species = 120, Gender = 2, Level = 10, }, // Staryu
            new EncounterStatic { Species = 060, Gender = 0, Level = 08, }, // Poliwag
            new EncounterStatic { Species = 079, Gender = 0, Level = 08, }, // Slowpoke
            new EncounterStatic { Species = 191, Gender = 1, Level = 06, }, // Sunkern
            new EncounterStatic { Species = 194, Gender = 0, Level = 06, }, // Wooper
            new EncounterStatic { Species = 081, Gender = 2, Level = 11, }, // Magnemite
            new EncounterStatic { Species = 239, Gender = 0, Level = 11, Moves = new[]{009},}, // Elekid: Thunder Punch
            new EncounterStatic { Species = 081, Gender = 2, Level = 08, }, // Magnemite
            new EncounterStatic { Species = 198, Gender = 1, Level = 11, }, // Murkrow
            new EncounterStatic { Species = 019, Gender = 1, Level = 07, }, // Rattata
            new EncounterStatic { Species = 163, Gender = 1, Level = 07, }, // Hoothoot
            new EncounterStatic { Species = 092, Gender = 1, Level = 15, Moves = new[]{194},}, // Gastly: Destiny Bond
            new EncounterStatic { Species = 238, Gender = 1, Level = 12, Moves = new[]{419},}, // Smoochum: Avalanche
            new EncounterStatic { Species = 092, Gender = 1, Level = 10, }, // Gastly
            new EncounterStatic { Species = 095, Gender = 0, Level = 10, }, // Onix
            new EncounterStatic { Species = 041, Gender = 0, Level = 08, }, // Zubat
            new EncounterStatic { Species = 066, Gender = 0, Level = 08, }, // Machop
            new EncounterStatic { Species = 060, Gender = 1, Level = 15, Moves = new[]{187}, }, // Poliwag: Belly Drum
            new EncounterStatic { Species = 147, Gender = 1, Level = 10, }, // Dratini
            new EncounterStatic { Species = 090, Gender = 1, Level = 12, }, // Shellder
            new EncounterStatic { Species = 098, Gender = 0, Level = 12, Moves = new[]{152}, }, // Krabby: Crabhammer
            new EncounterStatic { Species = 072, Gender = 1, Level = 09, }, // Tentacool
            new EncounterStatic { Species = 118, Gender = 1, Level = 09, }, // Goldeen
            new EncounterStatic { Species = 063, Gender = 1, Level = 15, }, // Abra
            new EncounterStatic { Species = 100, Gender = 2, Level = 15, }, // Voltorb
            new EncounterStatic { Species = 088, Gender = 0, Level = 13, }, // Grimer
            new EncounterStatic { Species = 109, Gender = 1, Level = 13, Moves = new[]{120}, }, // Koffing: Self-Destruct
            new EncounterStatic { Species = 019, Gender = 1, Level = 16, }, // Rattata
            new EncounterStatic { Species = 162, Gender = 0, Level = 15, }, // Furret
            // Hoenn Courses
            new EncounterStatic { Species = 264, Gender = 1, Level = 30, }, // Linoone
            new EncounterStatic { Species = 300, Gender = 1, Level = 30, }, // Skitty
            new EncounterStatic { Species = 313, Gender = 0, Level = 25, }, // Volbeat
            new EncounterStatic { Species = 314, Gender = 1, Level = 25, }, // Illumise
            new EncounterStatic { Species = 263, Gender = 1, Level = 17, }, // Zigzagoon
            new EncounterStatic { Species = 265, Gender = 1, Level = 15, }, // Wurmple
            new EncounterStatic { Species = 298, Gender = 1, Level = 20, }, // Azurill
            new EncounterStatic { Species = 320, Gender = 1, Level = 31, }, // Wailmer
            new EncounterStatic { Species = 116, Gender = 1, Level = 20, }, // Horsea
            new EncounterStatic { Species = 318, Gender = 1, Level = 26, }, // Carvanha
            new EncounterStatic { Species = 118, Gender = 1, Level = 22, Moves = new[]{401}, }, // Goldeen: Aqua Tail
            new EncounterStatic { Species = 129, Gender = 1, Level = 15, }, // Magikarp
            new EncounterStatic { Species = 218, Gender = 1, Level = 31, }, // Slugma
            new EncounterStatic { Species = 307, Gender = 0, Level = 32, }, // Meditite
            new EncounterStatic { Species = 111, Gender = 0, Level = 25, }, // Rhyhorn
            new EncounterStatic { Species = 228, Gender = 0, Level = 27, }, // Houndour
            new EncounterStatic { Species = 074, Gender = 0, Level = 29, }, // Geodude
            new EncounterStatic { Species = 077, Gender = 1, Level = 19, }, // Ponyta
            new EncounterStatic { Species = 351, Gender = 1, Level = 30, }, // Castform
            new EncounterStatic { Species = 352, Gender = 0, Level = 30, }, // Kecleon
            new EncounterStatic { Species = 203, Gender = 1, Level = 28, }, // Girafarig
            new EncounterStatic { Species = 234, Gender = 1, Level = 28, }, // Stantler
            new EncounterStatic { Species = 044, Gender = 1, Level = 14, }, // Gloom
            new EncounterStatic { Species = 070, Gender = 0, Level = 13, }, // Weepinbell
            new EncounterStatic { Species = 105, Gender = 1, Level = 30, Moves = new[]{037}, }, // Marowak: Thrash
            new EncounterStatic { Species = 128, Gender = 0, Level = 30, }, // Tauros
            new EncounterStatic { Species = 042, Gender = 0, Level = 33, }, // Golbat
            new EncounterStatic { Species = 177, Gender = 1, Level = 24, }, // Natu
            new EncounterStatic { Species = 066, Gender = 0, Level = 13, Moves = new[]{418}, }, // Machop: Bullet Punch
            new EncounterStatic { Species = 092, Gender = 1, Level = 15, }, // Gastly
            // Sinnoh Courses
            new EncounterStatic { Species = 415, Gender = 0, Level = 30, }, // Combee
            new EncounterStatic { Species = 439, Gender = 0, Level = 29, }, // Mime Jr.
            new EncounterStatic { Species = 403, Gender = 1, Level = 33, }, // Shinx
            new EncounterStatic { Species = 406, Gender = 0, Level = 30, }, // Budew
            new EncounterStatic { Species = 399, Gender = 1, Level = 13, }, // Bidoof
            new EncounterStatic { Species = 401, Gender = 0, Level = 15, }, // Kricketot
            new EncounterStatic { Species = 361, Gender = 1, Level = 28, }, // Snorunt
            new EncounterStatic { Species = 459, Gender = 0, Level = 31, Moves = new[]{452}, }, // Snover: Wood Hammer
            new EncounterStatic { Species = 215, Gender = 0, Level = 28, Moves = new[]{306}, }, // Sneasel: Crash Claw
            new EncounterStatic { Species = 436, Gender = 2, Level = 20, }, // Bronzor
            new EncounterStatic { Species = 179, Gender = 1, Level = 15, }, // Mareep
            new EncounterStatic { Species = 220, Gender = 1, Level = 16, }, // Swinub
            new EncounterStatic { Species = 357, Gender = 1, Level = 35, }, // Tropius
            new EncounterStatic { Species = 438, Gender = 0, Level = 30, }, // Bonsly
            new EncounterStatic { Species = 114, Gender = 1, Level = 30, }, // Tangela
            new EncounterStatic { Species = 400, Gender = 1, Level = 30, }, // Bibarel
            new EncounterStatic { Species = 102, Gender = 1, Level = 17, }, // Exeggcute
            new EncounterStatic { Species = 179, Gender = 0, Level = 19, }, // Mareep
            new EncounterStatic { Species = 200, Gender = 1, Level = 32, Moves = new[]{194},}, // Misdreavus: Destiny Bond
            new EncounterStatic { Species = 433, Gender = 0, Level = 22, Moves = new[]{105},}, // Chingling: Recover
            new EncounterStatic { Species = 093, Gender = 0, Level = 25, }, // Haunter
            new EncounterStatic { Species = 418, Gender = 0, Level = 28, Moves = new[]{226},}, // Buizel: Baton Pass
            new EncounterStatic { Species = 170, Gender = 1, Level = 17, }, // Chinchou
            new EncounterStatic { Species = 223, Gender = 1, Level = 19, }, // Remoraid
            new EncounterStatic { Species = 422, Gender = 1, Level = 30, Moves = new[]{243},}, // Shellos: Mirror Coat
            new EncounterStatic { Species = 456, Gender = 1, Level = 26, }, // Finneon
            new EncounterStatic { Species = 086, Gender = 1, Level = 27, }, // Seel
            new EncounterStatic { Species = 129, Gender = 1, Level = 30, }, // Magikarp
            new EncounterStatic { Species = 054, Gender = 1, Level = 22, Moves = new[]{281},}, // Psyduck: Yawn
            new EncounterStatic { Species = 090, Gender = 0, Level = 20, }, // Shellder
            new EncounterStatic { Species = 025, Gender = 1, Level = 30, }, // Pikachu
            new EncounterStatic { Species = 417, Gender = 1, Level = 33, Moves = new[]{175},}, // Pachirisu: Flail
            new EncounterStatic { Species = 035, Gender = 1, Level = 31, }, // Clefairy
            new EncounterStatic { Species = 039, Gender = 1, Level = 30, }, // Jigglypuff
            new EncounterStatic { Species = 183, Gender = 1, Level = 25, }, // Marill
            new EncounterStatic { Species = 187, Gender = 1, Level = 25, }, // Hoppip
            new EncounterStatic { Species = 442, Gender = 0, Level = 31, }, // Spiritomb
            new EncounterStatic { Species = 446, Gender = 0, Level = 33, }, // Munchlax
            new EncounterStatic { Species = 349, Gender = 0, Level = 30, }, // Feebas
            new EncounterStatic { Species = 433, Gender = 1, Level = 26, }, // Chingling
            new EncounterStatic { Species = 042, Gender = 0, Level = 33, }, // Golbat
            new EncounterStatic { Species = 164, Gender = 1, Level = 30, }, // Noctowl
            // Special Courses
            new EncounterStatic { Species = 120, Gender = 2, Level = 18, Moves = new[]{113}, }, // Staryu: Light Screen
            new EncounterStatic { Species = 224, Gender = 1, Level = 19, Moves = new[]{324}, }, // Octillery: Signal Beam
            new EncounterStatic { Species = 116, Gender = 0, Level = 15, }, // Horsea
            new EncounterStatic { Species = 222, Gender = 1, Level = 16, }, // Corsola
            new EncounterStatic { Species = 170, Gender = 1, Level = 12, }, // Chinchou
            new EncounterStatic { Species = 223, Gender = 0, Level = 14, }, // Remoraid
            new EncounterStatic { Species = 035, Gender = 0, Level = 08, Moves = new[]{236}, }, // Clefairy: Moonlight
            new EncounterStatic { Species = 039, Gender = 0, Level = 10, }, // Jigglypuff
            new EncounterStatic { Species = 041, Gender = 0, Level = 09, }, // Zubat
            new EncounterStatic { Species = 163, Gender = 1, Level = 06, }, // Hoothoot
            new EncounterStatic { Species = 074, Gender = 0, Level = 05, }, // Geodude
            new EncounterStatic { Species = 095, Gender = 1, Level = 05, Moves = new[]{088}, }, // Onix: Rock Throw
            new EncounterStatic { Species = 025, Gender = 0, Level = 15, Moves = new[]{019}, }, // Pikachu: Fly
            new EncounterStatic { Species = 025, Gender = 1, Level = 14, Moves = new[]{057}, }, // Pikachu: Surf
            new EncounterStatic { Species = 025, Gender = 1, Level = 12, Moves = new[]{344, 252}, }, // Pikachu: Volt Tackle, Fake Out
            new EncounterStatic { Species = 025, Gender = 0, Level = 13, Moves = new[]{175}, }, // Pikachu: Flail
            new EncounterStatic { Species = 025, Gender = 0, Level = 10, }, // Pikachu
            new EncounterStatic { Species = 025, Gender = 1, Level = 10, }, // Pikachu
            new EncounterStatic { Species = 302, Gender = 1, Level = 15, }, // Sableye
            new EncounterStatic { Species = 441, Gender = 0, Level = 15, }, // Chatot
            new EncounterStatic { Species = 025, Gender = 1, Level = 10, }, // Pikachu
            new EncounterStatic { Species = 453, Gender = 0, Level = 10, }, // Croagunk
            new EncounterStatic { Species = 417, Gender = 0, Level = 05, }, // Pachirisu
            new EncounterStatic { Species = 427, Gender = 1, Level = 05, }, // Buneary
            new EncounterStatic { Species = 133, Gender = 0, Level = 10, }, // Eevee
            new EncounterStatic { Species = 255, Gender = 0, Level = 10, }, // Torchic
            new EncounterStatic { Species = 061, Gender = 1, Level = 15, Moves = new[]{003}, }, // Poliwhirl: Double Slap
            new EncounterStatic { Species = 279, Gender = 0, Level = 15, }, // Pelipper
            new EncounterStatic { Species = 025, Gender = 1, Level = 08, }, // Pikachu
            new EncounterStatic { Species = 052, Gender = 0, Level = 10, }, // Meowth
            new EncounterStatic { Species = 374, Gender = 2, Level = 05, Moves = new[]{428,334,442}, }, // Beldum: Zen Headbutt, Iron Defense & Iron Head.
            new EncounterStatic { Species = 446, Gender = 0, Level = 05, Moves = new[]{120}, }, // Munchlax: Self-Destruct
            new EncounterStatic { Species = 116, Gender = 0, Level = 05, Moves = new[]{330}, }, // Horsea: Muddy Water
            new EncounterStatic { Species = 355, Gender = 0, Level = 05, Moves = new[]{286}, }, // Duskull: Imprison
            new EncounterStatic { Species = 129, Gender = 0, Level = 05, Moves = new[]{340}, }, // Magikarp: Bounce
            new EncounterStatic { Species = 436, Gender = 2, Level = 05, Moves = new[]{433}, }, // Bronzor: Trick Room
            new EncounterStatic { Species = 239, Gender = 0, Level = 05, Moves = new[]{9}}, // Elekid: Thunder Punch (can be tutored)
            new EncounterStatic { Species = 240, Gender = 0, Level = 05, Moves = new[]{7}}, // Magby: Fire Punch (can be tutored)
            new EncounterStatic { Species = 238, Gender = 1, Level = 05, Moves = new[]{8}}, // Smoochum: Ice Punch (can be tutored)
            new EncounterStatic { Species = 440, Gender = 1, Level = 05, Moves = new[]{215}}, // Happiny: Heal Bell
            new EncounterStatic { Species = 173, Gender = 1, Level = 05, Moves = new[]{118}}, // Cleffa: Metronome
            new EncounterStatic { Species = 174, Gender = 0, Level = 05, Moves = new[]{273}}, // Igglybuff: Wish
        };
        #endregion
        #region Static Encounter/Gift Tables
        private static readonly int[] Roaming_MetLocation_DPPt_Grass =
        {
            // Routes 201-218, 221-222 can be encountered in grass
            16, 17, 18, 19, 20, 21, 22, 23, 24, 25,
            26, 27, 28, 29, 30, 31, 32, 33, 36, 37,
            47,     // Valley Windworks
            49,     // Fuego Ironworks
        };

        private static readonly int[] Roaming_MetLocation_DPPt_Surf =
        {
            // Routes 203-205, 208-210, 212-214, 218-222 can be encountered in water
            18, 19, 20, 23, 24, 25, 27, 28, 29, 33,
            34, 35, 36, 37,
            47,     // Valley Windworks
            49,     // Fuego Ironworks
        };

        private static readonly EncounterStaticTyped[] Encounter_DPPt_Roam_Grass =
        {
            new EncounterStaticTyped { Species = 481, Level = 50, Roaming = true, TypeEncounter = EncounterType.TallGrass }, // Mesprit
            new EncounterStaticTyped { Species = 488, Level = 50, Roaming = true, TypeEncounter = EncounterType.TallGrass }, // Cresselia
            new EncounterStaticTyped { Species = 144, Level = 60, Roaming = true, TypeEncounter = EncounterType.TallGrass, Version = GameVersion.Pt }, // Articuno
            new EncounterStaticTyped { Species = 145, Level = 60, Roaming = true, TypeEncounter = EncounterType.TallGrass, Version = GameVersion.Pt }, // Zapdos
            new EncounterStaticTyped { Species = 146, Level = 60, Roaming = true, TypeEncounter = EncounterType.TallGrass, Version = GameVersion.Pt }, // Moltres
        };

        private static readonly EncounterStaticTyped[] Encounter_DPPt_Roam_Surf =
        {
            new EncounterStaticTyped { Species = 481, Level = 50, Roaming = true, TypeEncounter = EncounterType.Surfing_Fishing }, // Mesprit
            new EncounterStaticTyped { Species = 488, Level = 50, Roaming = true, TypeEncounter = EncounterType.Surfing_Fishing }, // Cresselia
            new EncounterStaticTyped { Species = 144, Level = 60, Roaming = true, TypeEncounter = EncounterType.Surfing_Fishing, Version = GameVersion.Pt }, // Articuno
            new EncounterStaticTyped { Species = 145, Level = 60, Roaming = true, TypeEncounter = EncounterType.Surfing_Fishing, Version = GameVersion.Pt }, // Zapdos
            new EncounterStaticTyped { Species = 146, Level = 60, Roaming = true, TypeEncounter = EncounterType.Surfing_Fishing, Version = GameVersion.Pt }, // Moltres
        };

        private static readonly EncounterStatic[] Encounter_DPPt_Regular =
        {
            // Starters
            new EncounterStaticTyped { Gift = true, Species = 387, Level = 5, Location = 076, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP, Version = GameVersion.DP }, // Turtwig @ Lake Verity
            new EncounterStaticTyped { Gift = true, Species = 390, Level = 5, Location = 076, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP, Version = GameVersion.DP }, // Chimchar
            new EncounterStaticTyped { Gift = true, Species = 393, Level = 5, Location = 076, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP, Version = GameVersion.DP }, // Piplup
            new EncounterStaticTyped { Gift = true, Species = 387, Level = 5, Location = 016, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Version = GameVersion.Pt }, // Turtwig @ Route 201
            new EncounterStaticTyped { Gift = true, Species = 390, Level = 5, Location = 016, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Version = GameVersion.Pt }, // Chimchar
            new EncounterStaticTyped { Gift = true, Species = 393, Level = 5, Location = 016, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Version = GameVersion.Pt }, // Piplup

            // Fossil @ Mining Museum
            new EncounterStaticTyped { Gift = true, Species = 138, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP, Version = GameVersion.DP }, // Omanyte
            new EncounterStaticTyped { Gift = true, Species = 140, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP, Version = GameVersion.DP }, // Kabuto
            new EncounterStaticTyped { Gift = true, Species = 142, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP, Version = GameVersion.DP }, // Aerodactyl
            new EncounterStaticTyped { Gift = true, Species = 345, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP, Version = GameVersion.DP }, // Lileep
            new EncounterStaticTyped { Gift = true, Species = 347, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP, Version = GameVersion.DP}, // Anorith
            new EncounterStaticTyped { Gift = true, Species = 408, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP, Version = GameVersion.DP }, // Cranidos
            new EncounterStaticTyped { Gift = true, Species = 410, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP, Version = GameVersion.DP }, // Shieldon
            new EncounterStaticTyped { Gift = true, Species = 138, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Version = GameVersion.Pt }, // Omanyte
            new EncounterStaticTyped { Gift = true, Species = 140, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Version = GameVersion.Pt }, // Kabuto
            new EncounterStaticTyped { Gift = true, Species = 142, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Version = GameVersion.Pt }, // Aerodactyl
            new EncounterStaticTyped { Gift = true, Species = 345, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Version = GameVersion.Pt }, // Lileep
            new EncounterStaticTyped { Gift = true, Species = 347, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Version = GameVersion.Pt}, // Anorith
            new EncounterStaticTyped { Gift = true, Species = 408, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Version = GameVersion.Pt }, // Cranidos
            new EncounterStaticTyped { Gift = true, Species = 410, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Version = GameVersion.Pt }, // Shieldon

            // Gift
            new EncounterStaticTyped { Gift = true, Species = 133, Level = 05, Location = 010, Version = GameVersion.DP, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP, }, // Eevee @ Hearthome City
            new EncounterStaticTyped { Gift = true, Species = 133, Level = 20, Location = 010, Version = GameVersion.Pt, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Eevee @ Hearthome City
            new EncounterStaticTyped { Gift = true, Species = 137, Level = 25, Location = 012, Version = GameVersion.Pt, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Porygon @ Veilstone City
            new EncounterStatic { Gift = true, Species = 175, Level = 01, EggLocation = 2011, Version = GameVersion.Pt,}, // Togepi Egg from Cynthia
            new EncounterStatic { Gift = true, Species = 440, Level = 01, EggLocation = 2009, Version = GameVersion.DP,}, // Happiny Egg from Traveling Man
            new EncounterStatic { Gift = true, Species = 447, Level = 01, EggLocation = 2010, }, // Riolu Egg from Riley

            // Stationary
            new EncounterStatic { Species = 425, Level = 22, Location = 47, Version = GameVersion.DP }, // Drifloon @ Valley Windworks
            new EncounterStatic { Species = 425, Level = 15, Location = 47, Version = GameVersion.Pt }, // Drifloon @ Valley Windworks
            new EncounterStaticTyped { Species = 479, Level = 15, Location = 70, Version = GameVersion.DP, TypeEncounter = EncounterType.Building_EnigmaStone, }, // Rotom @ Old Chateau
            new EncounterStaticTyped { Species = 479, Level = 20, Location = 70, Version = GameVersion.Pt, TypeEncounter = EncounterType.Building_EnigmaStone, }, // Rotom @ Old Chateau
            new EncounterStatic { Species = 442, Level = 25, Location = 24 }, // Spiritomb @ Route 209

            // Stationary Legendary
            new EncounterStaticTyped { Species = 377, Level = 30, Location = 125, Version = GameVersion.Pt, TypeEncounter = EncounterType.Cave_HallOfOrigin, }, // Regirock @ Rock Peak Ruins
            new EncounterStaticTyped { Species = 378, Level = 30, Location = 124, Version = GameVersion.Pt, TypeEncounter = EncounterType.Cave_HallOfOrigin, }, // Regice @ Iceberg Ruins
            new EncounterStaticTyped { Species = 379, Level = 30, Location = 123, Version = GameVersion.Pt, TypeEncounter = EncounterType.Cave_HallOfOrigin, }, // Registeel @ Iron Ruins
            new EncounterStaticTyped { Species = 480, Level = 50, Location = 089, TypeEncounter = EncounterType.Cave_HallOfOrigin, }, // Uxie @ Acuity Cavern
            new EncounterStaticTyped { Species = 482, Level = 50, Location = 088, TypeEncounter = EncounterType.Cave_HallOfOrigin, }, // Azelf @ Valor Cavern
            new EncounterStaticTyped { Species = 483, Level = 47, Location = 051, Version = GameVersion.D, TypeEncounter = EncounterType.DialgaPalkia }, // Dialga @ Spear Pillar
            new EncounterStaticTyped { Species = 484, Level = 47, Location = 051, Version = GameVersion.P, TypeEncounter = EncounterType.DialgaPalkia }, // Palkia @ Spear Pillar
            new EncounterStaticTyped { Species = 483, Level = 70, Location = 051, Version = GameVersion.Pt, TypeEncounter = EncounterType.DialgaPalkia }, // Dialga @ Spear Pillar
            new EncounterStaticTyped { Species = 484, Level = 70, Location = 051, Version = GameVersion.Pt, TypeEncounter = EncounterType.DialgaPalkia }, // Palkia @ Spear Pillar
            new EncounterStaticTyped { Species = 485, Level = 70, Location = 084, Version = GameVersion.DP, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Heatran @ Stark Mountain
            new EncounterStaticTyped { Species = 485, Level = 50, Location = 084, Version = GameVersion.Pt, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Heatran @ Stark Mountain
            new EncounterStaticTyped { Species = 486, Level = 70, Location = 064, Version = GameVersion.DP, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Regigigas @ Snowpoint Temple
            new EncounterStaticTyped { Species = 486, Level = 01, Location = 064, Version = GameVersion.Pt, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Regigigas @ Snowpoint Temple
            new EncounterStaticTyped { Species = 487, Level = 70, Location = 062, Version = GameVersion.DP, Form = 0, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Giratina @ Turnback Cave
            new EncounterStaticTyped { Species = 487, Level = 47, Location = 117, Version = GameVersion.Pt, Form = 1, TypeEncounter = EncounterType.DistortionWorld_Pt, HeldItem = 112 }, // Giratina @ Distortion World
            new EncounterStaticTyped { Species = 487, Level = 47, Location = 062, Version = GameVersion.Pt, Form = 0, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Giratina @ Turnback Cave

            // Event
            new EncounterStaticTyped { Species = 491, Level = 40, Location = 079, Version = GameVersion.DP, TypeEncounter = EncounterType.TallGrass }, // Darkrai @ Newmoon Island (Unreleased in Diamond and Pearl)
            new EncounterStaticTyped { Species = 491, Level = 50, Location = 079, Version = GameVersion.Pt, TypeEncounter = EncounterType.TallGrass }, // Darkrai @ Newmoon Island
            new EncounterStatic { Species = 492, Form = 0, Level = 30, Location = 063, Version = GameVersion.Pt, Fateful = true }, // Shaymin @ Flower Paradise
            new EncounterStatic { Species = 492, Form = 0, Level = 30, Location = 063, Version = GameVersion.DP, Fateful = false }, // Shaymin @ Flower Paradise (Unreleased in Diamond and Pearl)
            new EncounterStaticTyped { Species = 493, Form = 0, Level = 80, Location = 086, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Arceus @ Hall of Origin (Unreleased)
        };

        private static readonly EncounterStatic[] Encounter_DPPt = Encounter_DPPt_Roam_Grass.SelectMany(e => e.Clone(Roaming_MetLocation_DPPt_Grass)).Concat(
            Encounter_DPPt_Roam_Surf.SelectMany(e => e.Clone(Roaming_MetLocation_DPPt_Surf))).Concat(
            Encounter_DPPt_Regular).ToArray();

        // Grass 29-39, 42-46, 47, 48
        // Surf 30-32 34-35, 40-45, 47
        // Route 45 innacesible surf
        private static readonly int[] Roaming_MetLocation_HGSS_Johto_Grass =
        {
            // Routes 29-48 can be encountered in grass
            // Won't go to routes 40,41,47,48
            177, 178, 179, 180, 181, 182, 183, 184, 185, 186,
            187,                     190, 191, 192, 193, 194,
        };

        private static readonly int[] Roaming_MetLocation_HGSS_Johto_Surf =
        {
            // Routes 30-32,34-35,40-45 and 47 can be encountered in water
            // Won't go to routes 40,41,47,48
            178, 179, 180, 182, 183, 190, 191, 192, 193
        };

        private static readonly EncounterStaticTyped[] Encounter_HGSS_JohtoRoam_Grass =
        {
            new EncounterStaticTyped { Species = 243, Level = 40, Roaming = true, TypeEncounter = EncounterType.TallGrass, }, // Raikou
            new EncounterStaticTyped { Species = 244, Level = 40, Roaming = true, TypeEncounter = EncounterType.TallGrass, }, // Entei
        };

        private static readonly EncounterStaticTyped[] Encounter_HGSS_JohtoRoam_Surf =
        {
            new EncounterStaticTyped { Species = 243, Level = 40, Roaming = true, TypeEncounter = EncounterType.Surfing_Fishing, }, // Raikou
            new EncounterStaticTyped { Species = 244, Level = 40, Roaming = true, TypeEncounter = EncounterType.Surfing_Fishing, }, // Entei
        };

        private static readonly int[] Roaming_MetLocation_HGSS_Kanto_Grass =
        {
            // Route 01-18,21,22,24,26 and 28 can be encountered in grass
            // Won't go to route 23 25 27
            149, 150, 151, 152, 153, 154, 155, 156, 157, 158,
            159, 160, 161, 162, 163, 164, 165, 166,
            169, 170,      172,      174,      176,
        };

        private static readonly int[] Roaming_MetLocation_HGSS_Kanto_Surf =
        {
            // Route 4,6,9,10,12,13,19-22,24,26 and 28 can be encountered in water
            // Won't go to route 23 25 27
            152, 154, 157, 158, 160, 161, 167, 168, 169, 170,
            172,      174,      176,
        };

        private static readonly EncounterStaticTyped[] Encounter_HGSS_KantoRoam_Grass =
        {
            new EncounterStaticTyped { Species = 380, Level = 35, Version = GameVersion.HG, Roaming = true, TypeEncounter = EncounterType.TallGrass, }, // Latias
            new EncounterStaticTyped { Species = 381, Level = 35, Version = GameVersion.SS, Roaming = true, TypeEncounter = EncounterType.TallGrass, }, // Latios
        };

        private static readonly EncounterStaticTyped[] Encounter_HGSS_KantoRoam_Surf =
        {
            new EncounterStaticTyped { Species = 380, Level = 35, Version = GameVersion.HG, Roaming = true, TypeEncounter = EncounterType.Surfing_Fishing, }, // Latias
            new EncounterStaticTyped { Species = 381, Level = 35, Version = GameVersion.SS, Roaming = true, TypeEncounter = EncounterType.Surfing_Fishing, }, // Latios
        };

        internal static readonly EncounterStatic SpikyEaredPichu = new EncounterStaticTyped // Spiky-Eared Pichu @ Ilex Forest
        {
            Species = 172,
            Level = 30,
            Gender = 1,
            Form = 1,
            Nature = Nature.Naughty,
            Location = 214,
            Moves = new[] { 344, 270, 207, 220 },
            TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio,
            Shiny = Shiny.Never
        };

        private static readonly EncounterStatic[] Encounter_HGSS_Regular =
        {
            // Starters
            new EncounterStaticTyped { Gift = true, Species = 001, Level = 05, Location = 138, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Bulbasaur @ Pallet Town
            new EncounterStaticTyped { Gift = true, Species = 004, Level = 05, Location = 138, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Charmander
            new EncounterStaticTyped { Gift = true, Species = 007, Level = 05, Location = 138, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Squirtle
            new EncounterStaticTyped { Gift = true, Species = 152, Level = 05, Location = 126, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP }, // Chikorita @ New Bark Town
            new EncounterStaticTyped { Gift = true, Species = 155, Level = 05, Location = 126, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP }, // Cyndaquil
            new EncounterStaticTyped { Gift = true, Species = 158, Level = 05, Location = 126, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP }, // Totodile
            new EncounterStaticTyped { Gift = true, Species = 252, Level = 05, Location = 148, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Treecko @ Saffron City
            new EncounterStaticTyped { Gift = true, Species = 255, Level = 05, Location = 148, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Torchic
            new EncounterStaticTyped { Gift = true, Species = 258, Level = 05, Location = 148, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Mudkip

            // Fossils @ Pewter City
            new EncounterStaticTyped { Gift = true, Species = 138, Level = 20, Location = 140, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Omanyte
            new EncounterStaticTyped { Gift = true, Species = 140, Level = 20, Location = 140, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Kabuto
            new EncounterStaticTyped { Gift = true, Species = 142, Level = 20, Location = 140, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Aerodactyl
            new EncounterStaticTyped { Gift = true, Species = 345, Level = 20, Location = 140, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Lileep
            new EncounterStaticTyped { Gift = true, Species = 347, Level = 20, Location = 140, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Anorith
            new EncounterStaticTyped { Gift = true, Species = 408, Level = 20, Location = 140, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Cranidos
            new EncounterStaticTyped { Gift = true, Species = 410, Level = 20, Location = 140, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Shieldon

            // Gift
            new EncounterStaticTyped { Gift = true, Species = 133, Level = 05, Location = 131, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Eevee @ Goldenrod City
            new EncounterStaticTyped { Gift = true, Species = 147, Level = 15, Location = 222, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Moves = new[] {245} }, // Dratini @ Dragon's Den (ExtremeSpeed)
            new EncounterStaticTyped { Gift = true, Species = 236, Level = 10, Location = 216, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Tyrogue @ Mt. Mortar
            new EncounterStatic { Gift = true, Species = 175, Level = 01, EggLocation = 2013, Moves = new[] {326} }, // Togepi Egg from Mr. Pokemon (Extrasensory as Egg move)
            new EncounterStatic { Gift = true, Species = 179, Level = 01, EggLocation = 2014, }, // Mareep Egg from Primo
            new EncounterStatic { Gift = true, Species = 194, Level = 01, EggLocation = 2014, }, // Wooper Egg from Primo
            new EncounterStatic { Gift = true, Species = 218, Level = 01, EggLocation = 2014, }, // Slugma Egg from Primo

            // Celadon City Game Corner
            new EncounterStaticTyped { Gift = true, Species = 122, Level = 15, Location = 144, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Mr. Mime
            new EncounterStaticTyped { Gift = true, Species = 133, Level = 15, Location = 144, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Eevee
            new EncounterStaticTyped { Gift = true, Species = 137, Level = 15, Location = 144, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Porygon

            // Goldenrod City Game Corner
            new EncounterStaticTyped { Gift = true, Species = 063, Level = 15, Location = 131, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Abra
            new EncounterStaticTyped { Gift = true, Species = 023, Level = 15, Location = 131, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Version = GameVersion.HG }, // Ekans
            new EncounterStaticTyped { Gift = true, Species = 027, Level = 15, Location = 131, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Version = GameVersion.SS }, // Sandshrew
            new EncounterStaticTyped { Gift = true, Species = 147, Level = 15, Location = 131, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Dratini

            // Team Rocket HQ Trap Floor
            new EncounterStaticTyped { Species = 100, Level = 23, Location = 213, TypeEncounter = EncounterType.Building_EnigmaStone, }, // Voltorb
            new EncounterStaticTyped { Species = 074, Level = 21, Location = 213, TypeEncounter = EncounterType.Building_EnigmaStone, }, // Geodude
            new EncounterStaticTyped { Species = 109, Level = 21, Location = 213, TypeEncounter = EncounterType.Building_EnigmaStone, }, // Koffing

            // Stationary
            new EncounterStaticTyped { Species = 130, Level = 30, Location = 135, TypeEncounter = EncounterType.Surfing_Fishing, Shiny = Shiny.Always }, // Gyarados @ Lake of Rage
            new EncounterStaticTyped { Species = 131, Level = 20, Location = 210, TypeEncounter = EncounterType.Surfing_Fishing, }, // Lapras @ Union Cave Friday Only
            new EncounterStaticTyped { Species = 101, Level = 23, Location = 213, TypeEncounter = EncounterType.Building_EnigmaStone, }, // Electrode @ Team Rocket HQ
            new EncounterStatic { Species = 143, Level = 50, Location = 159, }, // Snorlax @ Route 11
            new EncounterStatic { Species = 143, Level = 50, Location = 160, }, // Snorlax @ Route 12
            new EncounterStatic { Species = 185, Level = 20, Location = 184, }, // Sudowoodo @ Route 36, Encounter does not have type
            SpikyEaredPichu,

            // Stationary Legendary
            new EncounterStaticTyped { Species = 144, Level = 50, Location = 203, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Articuno @ Seafoam Islands
            new EncounterStatic { Species = 145, Level = 50, Location = 158, }, // Zapdos @ Route 10
            new EncounterStaticTyped { Species = 146, Level = 50, Location = 219, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Moltres @ Mt. Silver Cave
            new EncounterStaticTyped { Species = 150, Level = 70, Location = 199, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Mewtwo @ Cerulean Cave
            new EncounterStatic { Species = 245, Level = 40, Location = 173, }, // Suicune @ Route 25
            new EncounterStaticTyped { Species = 245, Level = 40, Location = 206, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Suicune @ Burned Tower
            new EncounterStaticTyped { Species = 249, Level = 45, Location = 218, Version = GameVersion.SS, TypeEncounter = EncounterType.Surfing_Fishing }, // Lugia @ Whirl Islands
            new EncounterStaticTyped { Species = 249, Level = 70, Location = 218, Version = GameVersion.HG, TypeEncounter = EncounterType.Surfing_Fishing }, // Lugia @ Whirl Islands
            new EncounterStaticTyped { Species = 250, Level = 45, Location = 205, Version = GameVersion.HG, TypeEncounter = EncounterType.Building_EnigmaStone }, // Ho-Oh @ Bell Tower
            new EncounterStaticTyped { Species = 250, Level = 70, Location = 205, Version = GameVersion.SS, TypeEncounter = EncounterType.Building_EnigmaStone }, // Ho-Oh @ Bell Tower
            new EncounterStaticTyped { Species = 380, Level = 40, Location = 140, Version = GameVersion.SS, TypeEncounter = EncounterType.Building_EnigmaStone }, // Latias @ Pewter City
            new EncounterStaticTyped { Species = 381, Level = 40, Location = 140, Version = GameVersion.HG, TypeEncounter = EncounterType.Building_EnigmaStone }, // Latios @ Pewter City
            new EncounterStaticTyped { Species = 382, Level = 50, Location = 232, Version = GameVersion.HG, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Kyogre @ Embedded Tower
            new EncounterStaticTyped { Species = 383, Level = 50, Location = 232, Version = GameVersion.SS, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Groudon @ Embedded Tower
            new EncounterStaticTyped { Species = 384, Level = 50, Location = 232, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Rayquaza @ Embedded Tower
            new EncounterStaticTyped { Species = 483, Level = 01, Location = 231, Gift = true, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Dialga @ Sinjoh Ruins
            new EncounterStaticTyped { Species = 484, Level = 01, Location = 231, Gift = true, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Palkia @ Sinjoh Ruins
            new EncounterStaticTyped { Species = 487, Level = 01, Location = 231, Gift = true, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Form = 1, HeldItem = 112 }, // Giratina @ Sinjoh Ruins
        };

        private static readonly EncounterStatic[] Encounter_HGSS = ConcatAll(
            Encounter_HGSS_KantoRoam_Grass.SelectMany(e => e.Clone(Roaming_MetLocation_HGSS_Kanto_Grass)),
            Encounter_HGSS_KantoRoam_Surf.SelectMany(e => e.Clone(Roaming_MetLocation_HGSS_Kanto_Surf)),
            Encounter_HGSS_JohtoRoam_Grass.SelectMany(e => e.Clone(Roaming_MetLocation_HGSS_Johto_Grass)),
            Encounter_HGSS_JohtoRoam_Surf.SelectMany(e => e.Clone(Roaming_MetLocation_HGSS_Johto_Surf)),
            Encounter_HGSS_Regular);
        #endregion
        #region Trade Tables
        private static readonly string[] RanchOTNames = { string.Empty, "ユカリ", "Hayley", "EULALIE", "GIULIA", "EUKALIA", string.Empty, "Eulalia" };

        private static readonly EncounterTrade[] RanchGifts =
        {
            new EncounterTradePID(323975838) { Species = 025, Level = 18, Moves = new[] {447,085,148,104}, TID = 1000, SID = 19840, OTGender = 1, Version = GameVersion.D, Location = 0068, Gender = 0, Ability = 1, CurrentLevel = 20, }, // Pikachu
            new EncounterTradePID(323977664) { Species = 037, Level = 16, Moves = new[] {412,109,053,219}, TID = 1000, SID = 21150, OTGender = 1, Version = GameVersion.D, Location = 3000, Gender = 0, Ability = 1, CurrentLevel = 30, }, // Vulpix
            new EncounterTradePID(323975579) { Species = 077, Level = 13, Moves = new[] {036,033,039,052}, TID = 1000, SID = 01123, OTGender = 1, Version = GameVersion.D, Location = 3000, Gender = 0, Ability = 2, CurrentLevel = 16, }, // Ponyta
            new EncounterTradePID(323975564) { Species = 108, Level = 34, Moves = new[] {076,111,014,205}, TID = 1000, SID = 03050, OTGender = 1, Version = GameVersion.D, Location = 0077, Gender = 0, Ability = 1, CurrentLevel = 40, }, // Lickitung
            new EncounterTradePID(323977579) { Species = 114, Level = 01, Moves = new[] {437,438,079,246}, TID = 1000, SID = 49497, OTGender = 1, Version = GameVersion.D, Location = 3000, Gender = 1, Ability = 2, }, // Tangela
            new EncounterTradePID(323977675) { Species = 133, Level = 16, Moves = new[] {363,270,098,247}, TID = 1000, SID = 47710, OTGender = 1, Version = GameVersion.D, Location = 0068, Gender = 0, Ability = 2, CurrentLevel = 30, }, // Eevee
            new EncounterTradePID(323977588) { Species = 142, Level = 20, Moves = new[] {363,089,444,332}, TID = 1000, SID = 43066, OTGender = 1, Version = GameVersion.D, Location = 0094, Gender = 0, Ability = 1, CurrentLevel = 50, }, // Aerodactyl
            new EncounterTrade    { Species = 151, Level = 50, Moves = new[] {235,216,095,100}, TID = 1000, SID = 59228, OTGender = 1, Version = GameVersion.D, Location = 3000, Gender = 2, Fateful = true, Ball = 0x10, }, // Mew
            new EncounterTradePID(232975554) { Species = 193, Level = 22, Moves = new[] {318,095,246,138}, TID = 1000, SID = 42301, OTGender = 1, Version = GameVersion.D, Location = 0052, Gender = 0, Ability = 1, CurrentLevel = 45, Ball = 0x05, }, // Yanma
            new EncounterTradePID(323975570) { Species = 241, Level = 16, Moves = new[] {208,215,360,359}, TID = 1000, SID = 02707, OTGender = 1, Version = GameVersion.D, Location = 3000, Gender = 1, Ability = 1, CurrentLevel = 48, }, // Miltank
            new EncounterTradePID(323975563) { Species = 285, Level = 22, Moves = new[] {402,147,206,078}, TID = 1000, SID = 02788, OTGender = 1, Version = GameVersion.D, Location = 3000, Gender = 0, Ability = 2, CurrentLevel = 45, Ball = 0x05, }, // Shroomish
            new EncounterTradePID(323975559) { Species = 320, Level = 30, Moves = new[] {156,323,133,058}, TID = 1000, SID = 27046, OTGender = 1, Version = GameVersion.D, Location = 0038, Gender = 0, Ability = 2, CurrentLevel = 45, }, // Wailmer
            new EncounterTradePID(323977657) { Species = 360, Level = 01, Moves = new[] {204,150,227,000}, TID = 1000, SID = 01788, OTGender = 1, Version = GameVersion.D, Location = 0004, Gender = 0, Ability = 2, EggLocation = 2000, }, // Wynaut
            new EncounterTradePID(323975563) { Species = 397, Level = 02, Moves = new[] {355,017,283,018}, TID = 1000, SID = 59298, OTGender = 1, Version = GameVersion.D, Location = 0016, Gender = 0, Ability = 2, CurrentLevel = 23, }, // Staravia
            new EncounterTradePID(323970584) { Species = 415, Level = 05, Moves = new[] {230,016,000,000}, TID = 1000, SID = 54140, OTGender = 1, Version = GameVersion.D, Location = 0020, Gender = 1, Ability = 1, CurrentLevel = 20, }, // Combee
            new EncounterTradePID(323977539) { Species = 417, Level = 09, Moves = new[] {447,045,351,098}, TID = 1000, SID = 18830, OTGender = 1, Version = GameVersion.D, Location = 0020, Gender = 1, Ability = 2, CurrentLevel = 10, }, // Pachirisu
            new EncounterTradePID(323974107) { Species = 422, Level = 20, Moves = new[] {363,352,426,104}, TID = 1000, SID = 39272, OTGender = 1, Version = GameVersion.D, Location = 0028, Gender = 0, Ability = 2, CurrentLevel = 25, Form = 1 }, // Shellos
            new EncounterTradePID(323977566) { Species = 427, Level = 10, Moves = new[] {204,193,409,098}, TID = 1000, SID = 31045, OTGender = 1, Version = GameVersion.D, Location = 3000, Gender = 1, Ability = 1, CurrentLevel = 16, }, // Buneary
            new EncounterTradePID(323975579) { Species = 453, Level = 22, Moves = new[] {310,207,426,389}, TID = 1000, SID = 41342, OTGender = 1, Version = GameVersion.D, Location = 0052, Gender = 0, Ability = 2, CurrentLevel = 31, Ball = 0x05, }, // Croagunk
            new EncounterTradePID(323977566) { Species = 456, Level = 15, Moves = new[] {213,352,219,392}, TID = 1000, SID = 48348, OTGender = 1, Version = GameVersion.D, Location = 0020, Gender = 1, Ability = 1, CurrentLevel = 35, }, // Finneon
            new EncounterTradePID(323975582) { Species = 459, Level = 32, Moves = new[] {452,420,275,059}, TID = 1000, SID = 23360, OTGender = 1, Version = GameVersion.D, Location = 0031, Gender = 0, Ability = 1, CurrentLevel = 41, }, // Snover
            new EncounterTrade    { Species = 489, Level = 01, Moves = new[] {447,240,156,057}, TID = 1000, SID = 09248, OTGender = 1, Version = GameVersion.D, Location = 3000, Gender = 2, Fateful = true, CurrentLevel = 50, Ball = 0x10, EggLocation = 3000, }, // Phione
        };

        internal static readonly EncounterTrade[] TradeGift_DPPt = new[]
        {
            new EncounterTradePID(0x0000008E) { Species = 063, Level = 01, Ability = 1, TID = 25643, SID = 00000, OTGender = 1, Gender = 0, IVs = new[] {15,15,15,20,25,25} }, // Machop -> Abra
            new EncounterTradePID(0x00000867) { Species = 441, Level = 01, Ability = 2, TID = 44142, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {15,20,15,25,25,15}, Contest = new[] {20,20,20,20,20,0} }, // Buizel -> Chatot
            new EncounterTradePID(0x00000088) { Species = 093, Level = 35, Ability = 1, TID = 19248, SID = 00000, OTGender = 1, Gender = 0, IVs = new[] {20,25,15,25,15,15} }, // Medicham (35 from Route 217) -> Haunter
            new EncounterTradePID(0x0000045C) { Species = 129, Level = 01, Ability = 1, TID = 53277, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {15,25,15,20,25,15} }, // Finneon -> Magikarp
        }.Concat(RanchGifts).ToArray();

        internal static readonly EncounterTrade[] TradeGift_HGSS =
        {
            new EncounterTradePID(0x000025EF) { Species = 095, Level = 01, Ability = 2, TID = 48926, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {25,20,25,15,15,15} }, // Bellsprout -> Onix
            new EncounterTradePID(0x00002310) { Species = 066, Level = 01, Ability = 1, TID = 37460, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {15,25,20,20,15,15} }, // Drowzee -> Machop
            new EncounterTradePID(0x000001DB) { Species = 100, Level = 01, Ability = 2, TID = 29189, SID = 00000, OTGender = 0, Gender = 2, IVs = new[] {15,20,15,25,25,15} }, // Krabby -> Voltorb
            new EncounterTradePID(0x0001FC0A) { Species = 085, Level = 15, Ability = 1, TID = 00283, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {20,20,20,15,15,15} }, // Dragonair (15 from DPPt) -> Dodrio
            new EncounterTradePID(0x0000D136) { Species = 082, Level = 19, Ability = 1, TID = 50082, SID = 00000, OTGender = 0, Gender = 2, IVs = new[] {15,20,15,20,20,20} }, // Dugtrio (19 from Diglett's Cave) -> Magneton
            new EncounterTradePID(0x000034E4) { Species = 178, Level = 16, Ability = 1, TID = 15616, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {15,20,15,20,20,20} }, // Haunter (16 from Old Chateau) -> Xatu
            new EncounterTradePID(0x00485876) { Species = 025, Level = 02, Ability = 1, TID = 33038, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {20,25,18,31,25,13} }, // Pikachu
            new EncounterTradePID(0x0012B6D4) { Species = 374, Level = 31, Ability = 1, TID = 23478, SID = 00000, OTGender = 0, Gender = 2, IVs = new[] {28,29,24,23,24,25} }, // Forretress -> Beldum
            new EncounterTradePID(0x0012971C) { Species = 111, Level = 01, Ability = 1, TID = 06845, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {22,31,13,00,22,09}, Moves = new[]{422} }, // Bonsly -> Rhyhorn
            new EncounterTradePID(0x00101596) { Species = 208, Level = 01, Ability = 1, TID = 26491, SID = 00000, OTGender = 1, Gender = 0, IVs = new[] {08,30,28,06,18,20}}, // Any -> Steelix

            //Gift
            new EncounterTradePID(0x00006B5E) { Species = 021, Ability = 1, TID = 01001, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {15,20,15,20,20,20}, Level = 20, Location = 183, Moves = new[]{043,031,228,332} },// Webster's Spearow
            new EncounterTradePID(0x000214D7) { Species = 213, Ability = 2, TID = 04336, SID = 00001, OTGender = 0, Gender = 0, IVs = new[] {15,20,15,20,20,20}, Level = 20, Location = 130, Moves = new[]{132,117,227,219} },// Kirk's Shuckle
        };

        private const string tradeDPPt = "tradedppt";
        private const string tradeHGSS = "tradehgss";
        private static readonly string[][] TradeDPPt = Util.GetLanguageStrings8(tradeDPPt);
        private static readonly string[][] TradeHGSS = Util.GetLanguageStrings8(tradeHGSS);
        #endregion

        #region Alt Slots

        internal static readonly int[] SafariZoneLocation_4 =
        {
            52, 202
        };

        private static readonly EncounterArea4DPPt[] DPPt_Unown =
        {
            new EncounterArea4DPPt {
                Location = 53, // Solaceon Ruins
                Slots = new int[25].Select((_, i) => new EncounterSlot { Species = 201, LevelMin = 14, LevelMax = 30, Type = SlotType.Grass, Form = i+1 }).ToArray() // B->?, Unown A is loaded from encounters raw file
            },
        };

        private static readonly EncounterArea4HGSS SlotsHGSS_BCC =

            new EncounterArea4HGSS
            {
                // Source http://bulbapedia.bulbagarden.net/wiki/Bug-Catching_Contest#Generation_IV
                Location = 207, // National Park Catching Contest
                Slots = new[]
                {
                    // Bug Contest Pre-National Pokédex
                    new EncounterSlot { Species = 010, LevelMin = 07, LevelMax = 18, Type = SlotType.BugContest, SlotNumber = 0 }, // Caterpie
                    new EncounterSlot { Species = 013, LevelMin = 07, LevelMax = 18, Type = SlotType.BugContest, SlotNumber = 1 }, // Weedle
                    new EncounterSlot { Species = 011, LevelMin = 09, LevelMax = 18, Type = SlotType.BugContest, SlotNumber = 2 }, // Metapod
                    new EncounterSlot { Species = 014, LevelMin = 09, LevelMax = 18, Type = SlotType.BugContest, SlotNumber = 3 }, // Kakuna
                    new EncounterSlot { Species = 012, LevelMin = 12, LevelMax = 15, Type = SlotType.BugContest, SlotNumber = 4 }, // Butterfree
                    new EncounterSlot { Species = 015, LevelMin = 12, LevelMax = 15, Type = SlotType.BugContest, SlotNumber = 5 }, // Beedrill
                    new EncounterSlot { Species = 048, LevelMin = 10, LevelMax = 16, Type = SlotType.BugContest, SlotNumber = 6 }, // Venonat
                    new EncounterSlot { Species = 046, LevelMin = 10, LevelMax = 17, Type = SlotType.BugContest, SlotNumber = 7 }, // Paras
                    new EncounterSlot { Species = 123, LevelMin = 13, LevelMax = 14, Type = SlotType.BugContest, SlotNumber = 8 }, // Scyther
                    new EncounterSlot { Species = 127, LevelMin = 13, LevelMax = 14, Type = SlotType.BugContest, SlotNumber = 9 }, // Pinsir
                    // Bug Contest Tuesday Post-National Pokédex
                    new EncounterSlot { Species = 010, LevelMin = 24, LevelMax = 36, Type = SlotType.BugContest, SlotNumber = 0 }, // Caterpie
                    new EncounterSlot { Species = 013, LevelMin = 24, LevelMax = 36, Type = SlotType.BugContest, SlotNumber = 1 }, // Weedle
                    new EncounterSlot { Species = 011, LevelMin = 26, LevelMax = 36, Type = SlotType.BugContest, SlotNumber = 2 }, // Metapod
                    new EncounterSlot { Species = 014, LevelMin = 26, LevelMax = 36, Type = SlotType.BugContest, SlotNumber = 3 }, // Kakuna
                    new EncounterSlot { Species = 012, LevelMin = 27, LevelMax = 30, Type = SlotType.BugContest, SlotNumber = 4 }, // Butterfree
                    new EncounterSlot { Species = 015, LevelMin = 27, LevelMax = 30, Type = SlotType.BugContest, SlotNumber = 5 }, // Beedrill
                    new EncounterSlot { Species = 048, LevelMin = 25, LevelMax = 32, Type = SlotType.BugContest, SlotNumber = 6 }, // Venonat
                    new EncounterSlot { Species = 046, LevelMin = 27, LevelMax = 34, Type = SlotType.BugContest, SlotNumber = 7 }, // Paras
                    new EncounterSlot { Species = 123, LevelMin = 27, LevelMax = 28, Type = SlotType.BugContest, SlotNumber = 8 }, // Scyther
                    new EncounterSlot { Species = 127, LevelMin = 27, LevelMax = 28, Type = SlotType.BugContest, SlotNumber = 9 }, // Pinsir

                    // Bug Contest Thursday and Saturday Post-National Pokédex
                    new EncounterSlot { Species = 265, LevelMin = 24, LevelMax = 36, Type = SlotType.BugContest, SlotNumber = 0 }, // Wurmple
                    new EncounterSlot { Species = 266, LevelMin = 24, LevelMax = 36, Type = SlotType.BugContest, SlotNumber = 1 }, // Silcoon (Thursday)
                    new EncounterSlot { Species = 268, LevelMin = 24, LevelMax = 36, Type = SlotType.BugContest, SlotNumber = 1 }, // Cascoon (Saturday)
                    new EncounterSlot { Species = 290, LevelMin = 26, LevelMax = 36, Type = SlotType.BugContest, SlotNumber = 2 }, // Nincada
                    new EncounterSlot { Species = 313, LevelMin = 26, LevelMax = 36, Type = SlotType.BugContest, SlotNumber = 3 }, // Volbeat (Thursday)
                    new EncounterSlot { Species = 314, LevelMin = 26, LevelMax = 36, Type = SlotType.BugContest, SlotNumber = 3 }, // Illumise (Saturday)
                    new EncounterSlot { Species = 401, LevelMin = 27, LevelMax = 30, Type = SlotType.BugContest, SlotNumber = 4 }, // Kricketot
                    new EncounterSlot { Species = 402, LevelMin = 27, LevelMax = 30, Type = SlotType.BugContest, SlotNumber = 5 }, // Kricketune
                    new EncounterSlot { Species = 269, LevelMin = 25, LevelMax = 32, Type = SlotType.BugContest, SlotNumber = 6 }, // Dustox (Thursday)
                    new EncounterSlot { Species = 267, LevelMin = 25, LevelMax = 32, Type = SlotType.BugContest, SlotNumber = 6 }, // Beautifly (Saturday)
                    new EncounterSlot { Species = 415, LevelMin = 27, LevelMax = 34, Type = SlotType.BugContest, SlotNumber = 7 }, // Combee
                    new EncounterSlot { Species = 123, LevelMin = 27, LevelMax = 28, Type = SlotType.BugContest, SlotNumber = 8 }, // Scyther
                    new EncounterSlot { Species = 127, LevelMin = 27, LevelMax = 28, Type = SlotType.BugContest, SlotNumber = 9 }, // Pinsir
                }
            };

        private static readonly EncounterSlot[] SAFARIZONE_PEAK =
        {
            new EncounterSlot { Species = 022, LevelMin = 44, LevelMax = 44, Type = SlotType.Grass_Safari }, // Fearow
            new EncounterSlot { Species = 046, LevelMin = 42, LevelMax = 42, Type = SlotType.Grass_Safari }, // Paras
            new EncounterSlot { Species = 074, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Geodude
            new EncounterSlot { Species = 075, LevelMin = 16, LevelMax = 17, Type = SlotType.Grass_Safari }, // Graveler
            new EncounterSlot { Species = 080, LevelMin = 45, LevelMax = 45, Type = SlotType.Grass_Safari }, // Slowbro
            new EncounterSlot { Species = 081, LevelMin = 15, LevelMax = 16, Type = SlotType.Grass_Safari }, // Magnemite
            new EncounterSlot { Species = 082, LevelMin = 17, LevelMax = 17, Type = SlotType.Grass_Safari }, // Magneton
            new EncounterSlot { Species = 126, LevelMin = 17, LevelMax = 17, Type = SlotType.Grass_Safari }, // Magmar
            new EncounterSlot { Species = 126, LevelMin = 41, LevelMax = 41, Type = SlotType.Grass_Safari }, // Magmar
            new EncounterSlot { Species = 202, LevelMin = 16, LevelMax = 17, Type = SlotType.Grass_Safari }, // Wobbuffet
            new EncounterSlot { Species = 202, LevelMin = 41, LevelMax = 41, Type = SlotType.Grass_Safari }, // Wobbuffet
            new EncounterSlot { Species = 264, LevelMin = 46, LevelMax = 46, Type = SlotType.Grass_Safari }, // Linoone
            new EncounterSlot { Species = 288, LevelMin = 47, LevelMax = 47, Type = SlotType.Grass_Safari }, // Vigoroth
            new EncounterSlot { Species = 305, LevelMin = 45, LevelMax = 45, Type = SlotType.Grass_Safari }, // Lairon
            new EncounterSlot { Species = 335, LevelMin = 43, LevelMax = 45, Type = SlotType.Grass_Safari }, // Zangoose
            new EncounterSlot { Species = 363, LevelMin = 44, LevelMax = 45, Type = SlotType.Grass_Safari }, // Spheal
            new EncounterSlot { Species = 436, LevelMin = 45, LevelMax = 46, Type = SlotType.Grass_Safari }, // Bronzor
        };

        private static readonly EncounterSlot[] SAFARIZONE_DESERT =
        {
            new EncounterSlot { Species = 022, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Fearow
            new EncounterSlot { Species = 022, LevelMin = 38, LevelMax = 38, Type = SlotType.Grass_Safari }, // Fearow
            new EncounterSlot { Species = 022, LevelMin = 41, LevelMax = 41, Type = SlotType.Grass_Safari }, // Fearow
            new EncounterSlot { Species = 027, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Sandshrew
            new EncounterSlot { Species = 028, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Sandslash
            new EncounterSlot { Species = 104, LevelMin = 16, LevelMax = 17, Type = SlotType.Grass_Safari }, // Cubone
            new EncounterSlot { Species = 105, LevelMin = 17, LevelMax = 17, Type = SlotType.Grass_Safari }, // Marowak
            new EncounterSlot { Species = 105, LevelMin = 41, LevelMax = 41, Type = SlotType.Grass_Safari }, // Marowak
            new EncounterSlot { Species = 270, LevelMin = 38, LevelMax = 38, Type = SlotType.Grass_Safari }, // Lotad
            new EncounterSlot { Species = 327, LevelMin = 45, LevelMax = 45, Type = SlotType.Grass_Safari }, // Spinda
            new EncounterSlot { Species = 328, LevelMin = 46, LevelMax = 47, Type = SlotType.Grass_Safari }, // Trapinch
            new EncounterSlot { Species = 329, LevelMin = 44, LevelMax = 45, Type = SlotType.Grass_Safari }, // Vibrava
            new EncounterSlot { Species = 331, LevelMin = 35, LevelMax = 35, Type = SlotType.Grass_Safari }, // Cacnea
            new EncounterSlot { Species = 332, LevelMin = 48, LevelMax = 48, Type = SlotType.Grass_Safari }, // Cacturne
            new EncounterSlot { Species = 449, LevelMin = 43, LevelMax = 43, Type = SlotType.Grass_Safari }, // Hippopotas
            new EncounterSlot { Species = 455, LevelMin = 48, LevelMax = 48, Type = SlotType.Grass_Safari }, // Carnivine
        };

        private static readonly EncounterSlot[] SAFARIZONE_PLAINS =
        {
            new EncounterSlot { Species = 019, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Rattata
            new EncounterSlot { Species = 020, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Raticate
            new EncounterSlot { Species = 063, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Abra
            new EncounterSlot { Species = 077, LevelMin = 42, LevelMax = 42, Type = SlotType.Grass_Safari }, // Ponyta
            new EncounterSlot { Species = 203, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Girafarig
            new EncounterSlot { Species = 203, LevelMin = 40, LevelMax = 40, Type = SlotType.Grass_Safari }, // Girafarig
            new EncounterSlot { Species = 229, LevelMin = 43, LevelMax = 44, Type = SlotType.Grass_Safari }, // Houndoom
            new EncounterSlot { Species = 234, LevelMin = 17, LevelMax = 17, Type = SlotType.Grass_Safari }, // Stantler
            new EncounterSlot { Species = 234, LevelMin = 40, LevelMax = 41, Type = SlotType.Grass_Safari }, // Stantler
            new EncounterSlot { Species = 235, LevelMin = 17, LevelMax = 17, Type = SlotType.Grass_Safari }, // Smeargle
            new EncounterSlot { Species = 235, LevelMin = 41, LevelMax = 41, Type = SlotType.Grass_Safari }, // Smeargle
            new EncounterSlot { Species = 263, LevelMin = 44, LevelMax = 44, Type = SlotType.Grass_Safari }, // Zigzagoon
            new EncounterSlot { Species = 270, LevelMin = 42, LevelMax = 42, Type = SlotType.Grass_Safari }, // Lotad
            new EncounterSlot { Species = 283, LevelMin = 46, LevelMax = 46, Type = SlotType.Grass_Safari }, // Surskit
            new EncounterSlot { Species = 310, LevelMin = 45, LevelMax = 45, Type = SlotType.Grass_Safari }, // Manectric
            new EncounterSlot { Species = 335, LevelMin = 43, LevelMax = 45, Type = SlotType.Grass_Safari }, // Zangoose
            new EncounterSlot { Species = 403, LevelMin = 43, LevelMax = 44, Type = SlotType.Grass_Safari }, // Shinx
        };

        private static readonly EncounterSlot[] SAFARIZONE_MEADOW =
        {
            new EncounterSlot { Species = 020, LevelMin = 40, LevelMax = 40, Type = SlotType.Grass_Safari }, // Raticate
            new EncounterSlot { Species = 035, LevelMin = 17, LevelMax = 17, Type = SlotType.Grass_Safari }, // Clefairy
            new EncounterSlot { Species = 035, LevelMin = 42, LevelMax = 42, Type = SlotType.Grass_Safari }, // Clefairy
            new EncounterSlot { Species = 039, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Jigglypuff
            new EncounterSlot { Species = 060, LevelMin = 12, LevelMax = 15, Type = SlotType.Old_Rod_Safari }, // Poliwag
            new EncounterSlot { Species = 060, LevelMin = 22, LevelMax = 24, Type = SlotType.Good_Rod_Safari }, // Poliwag
            new EncounterSlot { Species = 060, LevelMin = 35, LevelMax = 36, Type = SlotType.Super_Rod_Safari }, // Poliwag
            new EncounterSlot { Species = 061, LevelMin = 15, LevelMax = 16, Type = SlotType.Old_Rod_Safari }, // Poliwhirl
            new EncounterSlot { Species = 061, LevelMin = 24, LevelMax = 25, Type = SlotType.Good_Rod_Safari }, // Poliwhirl
            new EncounterSlot { Species = 061, LevelMin = 27, LevelMax = 27, Type = SlotType.Good_Rod_Safari }, // Poliwhirl
            new EncounterSlot { Species = 061, LevelMin = 35, LevelMax = 38, Type = SlotType.Super_Rod_Safari }, // Poliwhirl
            new EncounterSlot { Species = 074, LevelMin = 45, LevelMax = 45, Type = SlotType.Grass_Safari }, // Geodude
            new EncounterSlot { Species = 113, LevelMin = 42, LevelMax = 42, Type = SlotType.Grass_Safari }, // Chansey
            new EncounterSlot { Species = 129, LevelMin = 15, LevelMax = 17, Type = SlotType.Surf_Safari }, // Magikarp
            new EncounterSlot { Species = 129, LevelMin = 12, LevelMax = 15, Type = SlotType.Old_Rod_Safari }, // Magikarp
            new EncounterSlot { Species = 129, LevelMin = 22, LevelMax = 24, Type = SlotType.Good_Rod_Safari }, // Magikarp
            new EncounterSlot { Species = 130, LevelMin = 28, LevelMax = 28, Type = SlotType.Good_Rod_Safari }, // Gyarados
            new EncounterSlot { Species = 130, LevelMin = 42, LevelMax = 42, Type = SlotType.Super_Rod_Safari }, // Gyarados
            new EncounterSlot { Species = 130, LevelMin = 45, LevelMax = 45, Type = SlotType.Super_Rod_Safari }, // Gyarados
            new EncounterSlot { Species = 183, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Marill
            new EncounterSlot { Species = 183, LevelMin = 16, LevelMax = 17, Type = SlotType.Surf_Safari }, // Marill
            new EncounterSlot { Species = 187, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Hoppip
            new EncounterSlot { Species = 188, LevelMin = 17, LevelMax = 17, Type = SlotType.Grass_Safari }, // Skiploom
            new EncounterSlot { Species = 188, LevelMin = 40, LevelMax = 40, Type = SlotType.Grass_Safari }, // Skiploom
            new EncounterSlot { Species = 188, LevelMin = 47, LevelMax = 47, Type = SlotType.Surf_Safari }, // Skiploom
            new EncounterSlot { Species = 191, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Sunkern
            new EncounterSlot { Species = 194, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Wooper
            new EncounterSlot { Species = 194, LevelMin = 40, LevelMax = 40, Type = SlotType.Grass_Safari }, // Wooper
            new EncounterSlot { Species = 194, LevelMin = 15, LevelMax = 17, Type = SlotType.Surf_Safari }, // Wooper
            new EncounterSlot { Species = 273, LevelMin = 45, LevelMax = 45, Type = SlotType.Grass_Safari }, // Seedot
            new EncounterSlot { Species = 274, LevelMin = 38, LevelMax = 38, Type = SlotType.Grass_Safari }, // Nuzleaf
            new EncounterSlot { Species = 274, LevelMin = 47, LevelMax = 48, Type = SlotType.Grass_Safari }, // Nuzleaf
            new EncounterSlot { Species = 284, LevelMin = 42, LevelMax = 42, Type = SlotType.Surf_Safari }, // Masquerain
            new EncounterSlot { Species = 284, LevelMin = 46, LevelMax = 46, Type = SlotType.Surf_Safari }, // Masquerain
            new EncounterSlot { Species = 299, LevelMin = 45, LevelMax = 45, Type = SlotType.Grass_Safari }, // Nosepass
            new EncounterSlot { Species = 447, LevelMin = 45, LevelMax = 46, Type = SlotType.Grass_Safari }, // Riolu
        };

        private static readonly EncounterSlot[] SAFARIZONE_FOREST =
        {
            new EncounterSlot { Species = 016, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Pidgey
            new EncounterSlot { Species = 069, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Bellsprout
            new EncounterSlot { Species = 092, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Gastly
            new EncounterSlot { Species = 093, LevelMin = 16, LevelMax = 17, Type = SlotType.Grass_Safari }, // Haunter
            new EncounterSlot { Species = 108, LevelMin = 40, LevelMax = 40, Type = SlotType.Grass_Safari }, // Lickitung
            new EncounterSlot { Species = 122, LevelMin = 16, LevelMax = 17, Type = SlotType.Grass_Safari }, // Mr. Mime
            new EncounterSlot { Species = 122, LevelMin = 45, LevelMax = 45, Type = SlotType.Grass_Safari }, // Mr. Mime
            new EncounterSlot { Species = 125, LevelMin = 41, LevelMax = 41, Type = SlotType.Grass_Safari }, // Electabuzz
            new EncounterSlot { Species = 200, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Misdreavus
            new EncounterSlot { Species = 200, LevelMin = 42, LevelMax = 42, Type = SlotType.Grass_Safari }, // Misdreavus
            new EncounterSlot { Species = 283, LevelMin = 42, LevelMax = 42, Type = SlotType.Grass_Safari }, // Surskit
            new EncounterSlot { Species = 353, LevelMin = 46, LevelMax = 47, Type = SlotType.Grass_Safari }, // Shuppet
            new EncounterSlot { Species = 374, LevelMin = 44, LevelMax = 44, Type = SlotType.Grass_Safari }, // Beldum
            new EncounterSlot { Species = 399, LevelMin = 40, LevelMax = 40, Type = SlotType.Grass_Safari }, // Bidoof
            new EncounterSlot { Species = 406, LevelMin = 47, LevelMax = 47, Type = SlotType.Grass_Safari }, // Budew
            new EncounterSlot { Species = 437, LevelMin = 44, LevelMax = 45, Type = SlotType.Grass_Safari }, // Bronzong
        };

        private static readonly EncounterSlot[] SAFARIZONE_SWAMP =
        {
            new EncounterSlot { Species = 039, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Jigglypuff
            new EncounterSlot { Species = 046, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Paras
            new EncounterSlot { Species = 047, LevelMin = 41, LevelMax = 41, Type = SlotType.Grass_Safari }, // Parasect
            new EncounterSlot { Species = 070, LevelMin = 46, LevelMax = 46, Type = SlotType.Grass_Safari }, // Weepinbell
            new EncounterSlot { Species = 096, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Drowzee
            new EncounterSlot { Species = 097, LevelMin = 16, LevelMax = 17, Type = SlotType.Grass_Safari }, // Hypno
            new EncounterSlot { Species = 097, LevelMin = 37, LevelMax = 37, Type = SlotType.Grass_Safari }, // Hypno
            new EncounterSlot { Species = 100, LevelMin = 42, LevelMax = 42, Type = SlotType.Grass_Safari }, // Voltorb
            new EncounterSlot { Species = 118, LevelMin = 15, LevelMax = 17, Type = SlotType.Surf_Safari }, // Goldeen
            new EncounterSlot { Species = 118, LevelMin = 17, LevelMax = 17, Type = SlotType.Old_Rod_Safari }, // Goldeen
            new EncounterSlot { Species = 118, LevelMin = 22, LevelMax = 24, Type = SlotType.Good_Rod_Safari }, // Goldeen
            new EncounterSlot { Species = 118, LevelMin = 35, LevelMax = 37, Type = SlotType.Super_Rod_Safari }, // Goldeen
            new EncounterSlot { Species = 119, LevelMin = 17, LevelMax = 17, Type = SlotType.Old_Rod_Safari }, // Seaking
            new EncounterSlot { Species = 119, LevelMin = 24, LevelMax = 25, Type = SlotType.Good_Rod_Safari }, // Seaking
            new EncounterSlot { Species = 119, LevelMin = 27, LevelMax = 27, Type = SlotType.Good_Rod_Safari }, // Seaking
            new EncounterSlot { Species = 119, LevelMin = 35, LevelMax = 37, Type = SlotType.Super_Rod_Safari }, // Seaking
            new EncounterSlot { Species = 119, LevelMin = 42, LevelMax = 42, Type = SlotType.Surf_Safari }, // Seaking
            new EncounterSlot { Species = 129, LevelMin = 12, LevelMax = 15, Type = SlotType.Old_Rod_Safari }, // Magikarp
            new EncounterSlot { Species = 129, LevelMin = 22, LevelMax = 24, Type = SlotType.Good_Rod_Safari }, // Magikarp
            new EncounterSlot { Species = 129, LevelMin = 15, LevelMax = 17, Type = SlotType.Surf_Safari }, // Magikarp
            new EncounterSlot { Species = 147, LevelMin = 36, LevelMax = 37, Type = SlotType.Super_Rod_Safari }, // Dratini
            new EncounterSlot { Species = 147, LevelMin = 29, LevelMax = 29, Type = SlotType.Good_Rod_Safari }, // Dratini
            new EncounterSlot { Species = 148, LevelMin = 42, LevelMax = 42, Type = SlotType.Super_Rod_Safari }, // Dragonair
            new EncounterSlot { Species = 148, LevelMin = 45, LevelMax = 45, Type = SlotType.Super_Rod_Safari }, // Dragonair
            new EncounterSlot { Species = 161, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Sentret
            new EncounterSlot { Species = 162, LevelMin = 42, LevelMax = 42, Type = SlotType.Grass_Safari }, // Furret
            new EncounterSlot { Species = 198, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Murkrow
            new EncounterSlot { Species = 198, LevelMin = 37, LevelMax = 37, Type = SlotType.Grass_Safari }, // Murkrow
            new EncounterSlot { Species = 198, LevelMin = 47, LevelMax = 47, Type = SlotType.Surf_Safari }, // Murkrow
            new EncounterSlot { Species = 355, LevelMin = 38, LevelMax = 38, Type = SlotType.Grass_Safari }, // Duskull
            new EncounterSlot { Species = 355, LevelMin = 48, LevelMax = 48, Type = SlotType.Surf_Safari }, // Duskull
            new EncounterSlot { Species = 358, LevelMin = 46, LevelMax = 47, Type = SlotType.Grass_Safari }, // Chimecho
            new EncounterSlot { Species = 371, LevelMin = 44, LevelMax = 45, Type = SlotType.Grass_Safari }, // Bagon
            new EncounterSlot { Species = 417, LevelMin = 47, LevelMax = 47, Type = SlotType.Grass_Safari }, // Pachirisu
            new EncounterSlot { Species = 419, LevelMin = 44, LevelMax = 44, Type = SlotType.Grass_Safari }, // Floatzel
        };

        private static readonly EncounterSlot[] SAFARIZONE_MARSHLAND =
        {
            new EncounterSlot { Species = 023, LevelMin = 15, LevelMax = 16, Type = SlotType.Grass_Safari }, // Ekans
            new EncounterSlot { Species = 024, LevelMin = 17, LevelMax = 17, Type = SlotType.Grass_Safari }, // Arbok
            new EncounterSlot { Species = 043, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Oddish
            new EncounterSlot { Species = 044, LevelMin = 16, LevelMax = 17, Type = SlotType.Grass_Safari }, // Gloom
            new EncounterSlot { Species = 044, LevelMin = 42, LevelMax = 42, Type = SlotType.Grass_Safari }, // Gloom
            new EncounterSlot { Species = 050, LevelMin = 43, LevelMax = 43, Type = SlotType.Grass_Safari }, // Diglett
            new EncounterSlot { Species = 060, LevelMin = 12, LevelMax = 15, Type = SlotType.Old_Rod_Safari }, // Poliwag
            new EncounterSlot { Species = 060, LevelMin = 15, LevelMax = 17, Type = SlotType.Surf_Safari }, // Poliwag
            new EncounterSlot { Species = 060, LevelMin = 16, LevelMax = 16, Type = SlotType.Old_Rod_Safari }, // Poliwag
            new EncounterSlot { Species = 060, LevelMin = 18, LevelMax = 18, Type = SlotType.Old_Rod_Safari }, // Poliwag
            new EncounterSlot { Species = 061, LevelMin = 22, LevelMax = 25, Type = SlotType.Good_Rod_Safari }, // Poliwhirl
            new EncounterSlot { Species = 061, LevelMin = 35, LevelMax = 38, Type = SlotType.Super_Rod_Safari }, // Poliwhirl
            new EncounterSlot { Species = 088, LevelMin = 17, LevelMax = 17, Type = SlotType.Grass_Safari }, // Grimer
            new EncounterSlot { Species = 088, LevelMin = 15, LevelMax = 17, Type = SlotType.Surf_Safari }, // Grimer
            new EncounterSlot { Species = 089, LevelMin = 38, LevelMax = 38, Type = SlotType.Grass_Safari }, // Muk
            new EncounterSlot { Species = 089, LevelMin = 48, LevelMax = 48, Type = SlotType.Surf_Safari }, // Muk
            new EncounterSlot { Species = 109, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Koffing
            new EncounterSlot { Species = 110, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Weezing
            new EncounterSlot { Species = 129, LevelMin = 12, LevelMax = 15, Type = SlotType.Old_Rod_Safari }, // Magikarp
            new EncounterSlot { Species = 129, LevelMin = 22, LevelMax = 24, Type = SlotType.Good_Rod_Safari }, // Magikarp
            new EncounterSlot { Species = 130, LevelMin = 36, LevelMax = 37, Type = SlotType.Super_Rod_Safari }, // Gyarados
            new EncounterSlot { Species = 130, LevelMin = 26, LevelMax = 26, Type = SlotType.Good_Rod_Safari }, // Gyarados
            new EncounterSlot { Species = 130, LevelMin = 29, LevelMax = 29, Type = SlotType.Good_Rod_Safari }, // Gyarados
            new EncounterSlot { Species = 189, LevelMin = 38, LevelMax = 38, Type = SlotType.Grass_Safari }, // Jumpluff
            new EncounterSlot { Species = 189, LevelMin = 47, LevelMax = 47, Type = SlotType.Surf_Safari }, // Jumpluff
            new EncounterSlot { Species = 194, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Wooper
            new EncounterSlot { Species = 194, LevelMin = 15, LevelMax = 17, Type = SlotType.Surf_Safari }, // Wooper
            new EncounterSlot { Species = 195, LevelMin = 43, LevelMax = 43, Type = SlotType.Surf_Safari }, // Quagsire
            new EncounterSlot { Species = 213, LevelMin = 44, LevelMax = 44, Type = SlotType.Grass_Safari }, // Shuckle
            new EncounterSlot { Species = 315, LevelMin = 46, LevelMax = 46, Type = SlotType.Grass_Safari }, // Roselia
            new EncounterSlot { Species = 336, LevelMin = 47, LevelMax = 48, Type = SlotType.Grass_Safari }, // Seviper
            new EncounterSlot { Species = 339, LevelMin = 42, LevelMax = 42, Type = SlotType.Super_Rod_Safari }, // Barboach
            new EncounterSlot { Species = 339, LevelMin = 45, LevelMax = 45, Type = SlotType.Super_Rod_Safari }, // Barboach
            new EncounterSlot { Species = 354, LevelMin = 44, LevelMax = 45, Type = SlotType.Grass_Safari }, // Banette
            new EncounterSlot { Species = 453, LevelMin = 44, LevelMax = 44, Type = SlotType.Grass_Safari }, // Croagunk
            new EncounterSlot { Species = 455, LevelMin = 41, LevelMax = 41, Type = SlotType.Grass_Safari }, // Carnivine
        };

        private static readonly EncounterSlot[] SAFARIZONE_MOUNTAIN =
        {
            new EncounterSlot { Species = 019, LevelMin = 15, LevelMax = 16, Type = SlotType.Grass_Safari }, // Rattata
            new EncounterSlot { Species = 020, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Raticate
            new EncounterSlot { Species = 041, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Zubat
            new EncounterSlot { Species = 042, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Golbat
            new EncounterSlot { Species = 082, LevelMin = 17, LevelMax = 17, Type = SlotType.Grass_Safari }, // Magneton
            new EncounterSlot { Species = 082, LevelMin = 42, LevelMax = 42, Type = SlotType.Grass_Safari }, // Magneton
            new EncounterSlot { Species = 098, LevelMin = 43, LevelMax = 43, Type = SlotType.Grass_Safari }, // Krabby
            new EncounterSlot { Species = 108, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Lickitung
            new EncounterSlot { Species = 246, LevelMin = 17, LevelMax = 17, Type = SlotType.Grass_Safari }, // Larvitar
            new EncounterSlot { Species = 246, LevelMin = 42, LevelMax = 42, Type = SlotType.Grass_Safari }, // Larvitar
            new EncounterSlot { Species = 307, LevelMin = 43, LevelMax = 44, Type = SlotType.Grass_Safari }, // Meditite
            new EncounterSlot { Species = 313, LevelMin = 46, LevelMax = 46, Type = SlotType.Grass_Safari }, // Volbeat
            new EncounterSlot { Species = 337, LevelMin = 46, LevelMax = 46, Type = SlotType.Grass_Safari }, // Lunatone
            new EncounterSlot { Species = 356, LevelMin = 45, LevelMax = 46, Type = SlotType.Grass_Safari }, // Dusclops
            new EncounterSlot { Species = 364, LevelMin = 45, LevelMax = 45, Type = SlotType.Grass_Safari }, // Sealeo
            new EncounterSlot { Species = 375, LevelMin = 44, LevelMax = 44, Type = SlotType.Grass_Safari }, // Metang
            new EncounterSlot { Species = 433, LevelMin = 38, LevelMax = 38, Type = SlotType.Grass_Safari }, // Chingling
        };

        private static readonly EncounterSlot[] SAFARIZONE_ROCKYBEACH =
        {
            new EncounterSlot { Species = 041, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Zubat
            new EncounterSlot { Species = 060, LevelMin = 15, LevelMax = 16, Type = SlotType.Surf_Safari }, // Poliwag
            new EncounterSlot { Species = 061, LevelMin = 16, LevelMax = 17, Type = SlotType.Surf_Safari }, // Poliwhirl
            new EncounterSlot { Species = 079, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Slowpoke
            new EncounterSlot { Species = 080, LevelMin = 17, LevelMax = 17, Type = SlotType.Grass_Safari }, // Slowbro
            new EncounterSlot { Species = 080, LevelMin = 37, LevelMax = 37, Type = SlotType.Grass_Safari }, // Slowbro
            new EncounterSlot { Species = 080, LevelMin = 42, LevelMax = 42, Type = SlotType.Grass_Safari }, // Slowbro
            new EncounterSlot { Species = 084, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Doduo
            new EncounterSlot { Species = 085, LevelMin = 42, LevelMax = 42, Type = SlotType.Grass_Safari }, // Dodrio
            new EncounterSlot { Species = 098, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Krabby
            new EncounterSlot { Species = 098, LevelMin = 13, LevelMax = 15, Type = SlotType.Old_Rod_Safari }, // Krabby
            new EncounterSlot { Species = 098, LevelMin = 22, LevelMax = 25, Type = SlotType.Good_Rod_Safari }, // Krabby
            new EncounterSlot { Species = 098, LevelMin = 17, LevelMax = 17, Type = SlotType.Old_Rod_Safari }, // Krabby
            new EncounterSlot { Species = 098, LevelMin = 18, LevelMax = 18, Type = SlotType.Old_Rod_Safari }, // Krabby
            new EncounterSlot { Species = 099, LevelMin = 26, LevelMax = 27, Type = SlotType.Good_Rod_Safari }, // Kingler
            new EncounterSlot { Species = 099, LevelMin = 40, LevelMax = 40, Type = SlotType.Grass_Safari }, // Kingler
            new EncounterSlot { Species = 099, LevelMin = 38, LevelMax = 39, Type = SlotType.Super_Rod_Safari }, // Kingler
            new EncounterSlot { Species = 118, LevelMin = 13, LevelMax = 15, Type = SlotType.Old_Rod_Safari }, // Goldeen
            new EncounterSlot { Species = 118, LevelMin = 22, LevelMax = 23, Type = SlotType.Good_Rod_Safari }, // Goldeen
            new EncounterSlot { Species = 118, LevelMin = 35, LevelMax = 38, Type = SlotType.Super_Rod_Safari }, // Goldeen
            new EncounterSlot { Species = 119, LevelMin = 35, LevelMax = 38, Type = SlotType.Super_Rod_Safari }, // Seaking
            new EncounterSlot { Species = 129, LevelMin = 12, LevelMax = 14, Type = SlotType.Old_Rod_Safari }, // Magikarp
            new EncounterSlot { Species = 129, LevelMin = 22, LevelMax = 23, Type = SlotType.Good_Rod_Safari }, // Magikarp
            new EncounterSlot { Species = 129, LevelMin = 15, LevelMax = 16, Type = SlotType.Surf_Safari }, // Magikarp
            new EncounterSlot { Species = 131, LevelMin = 15, LevelMax = 16, Type = SlotType.Surf_Safari }, // Lapras
            new EncounterSlot { Species = 131, LevelMin = 36, LevelMax = 37, Type = SlotType.Surf_Safari }, // Lapras
            new EncounterSlot { Species = 131, LevelMin = 41, LevelMax = 42, Type = SlotType.Surf_Safari }, // Lapras
            new EncounterSlot { Species = 131, LevelMin = 46, LevelMax = 47, Type = SlotType.Surf_Safari }, // Lapras
            new EncounterSlot { Species = 179, LevelMin = 43, LevelMax = 43, Type = SlotType.Grass_Safari }, // Mareep
            new EncounterSlot { Species = 304, LevelMin = 44, LevelMax = 45, Type = SlotType.Grass_Safari }, // Aron
            new EncounterSlot { Species = 309, LevelMin = 42, LevelMax = 42, Type = SlotType.Grass_Safari }, // Electrike
            new EncounterSlot { Species = 310, LevelMin = 37, LevelMax = 37, Type = SlotType.Grass_Safari }, // Manectric
            new EncounterSlot { Species = 341, LevelMin = 46, LevelMax = 46, Type = SlotType.Super_Rod_Safari }, // Corphish
            new EncounterSlot { Species = 341, LevelMin = 48, LevelMax = 48, Type = SlotType.Super_Rod_Safari }, // Corphish
            new EncounterSlot { Species = 406, LevelMin = 40, LevelMax = 40, Type = SlotType.Grass_Safari }, // Budew
            new EncounterSlot { Species = 443, LevelMin = 44, LevelMax = 44, Type = SlotType.Grass_Safari }, // Gible
        };

        private static readonly EncounterSlot[] SAFARIZONE_WASTELAND =
        {
            new EncounterSlot { Species = 022, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Fearow
            new EncounterSlot { Species = 055, LevelMin = 45, LevelMax = 45, Type = SlotType.Grass_Safari }, // Golduck
            new EncounterSlot { Species = 066, LevelMin = 16, LevelMax = 17, Type = SlotType.Grass_Safari }, // Machop
            new EncounterSlot { Species = 067, LevelMin = 17, LevelMax = 17, Type = SlotType.Grass_Safari }, // Machoke
            new EncounterSlot { Species = 067, LevelMin = 40, LevelMax = 40, Type = SlotType.Grass_Safari }, // Machoke
            new EncounterSlot { Species = 069, LevelMin = 41, LevelMax = 41, Type = SlotType.Grass_Safari }, // Bellsprout
            new EncounterSlot { Species = 081, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Magnemite
            new EncounterSlot { Species = 095, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Onix
            new EncounterSlot { Species = 099, LevelMin = 48, LevelMax = 48, Type = SlotType.Grass_Safari }, // Kingler
            new EncounterSlot { Species = 115, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Kangaskhan
            new EncounterSlot { Species = 286, LevelMin = 46, LevelMax = 46, Type = SlotType.Grass_Safari }, // Breloom
            new EncounterSlot { Species = 308, LevelMin = 44, LevelMax = 44, Type = SlotType.Grass_Safari }, // Medicham
            new EncounterSlot { Species = 310, LevelMin = 41, LevelMax = 41, Type = SlotType.Grass_Safari }, // Manectric
            new EncounterSlot { Species = 314, LevelMin = 46, LevelMax = 46, Type = SlotType.Grass_Safari }, // Illumise
            new EncounterSlot { Species = 338, LevelMin = 45, LevelMax = 46, Type = SlotType.Grass_Safari }, // Solrock
            new EncounterSlot { Species = 451, LevelMin = 44, LevelMax = 45, Type = SlotType.Grass_Safari }, // Skorupi
        };

        private static readonly EncounterSlot[] SAFARIZONE_SAVANNAH =
        {
            new EncounterSlot { Species = 029, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Nidoran♀
            new EncounterSlot { Species = 030, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Nidorina
            new EncounterSlot { Species = 032, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Nidoran♂
            new EncounterSlot { Species = 033, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Nidorino
            new EncounterSlot { Species = 041, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Zubat
            new EncounterSlot { Species = 042, LevelMin = 17, LevelMax = 17, Type = SlotType.Grass_Safari }, // Golbat
            new EncounterSlot { Species = 111, LevelMin = 17, LevelMax = 17, Type = SlotType.Grass_Safari }, // Rhyhorn
            new EncounterSlot { Species = 111, LevelMin = 41, LevelMax = 41, Type = SlotType.Grass_Safari }, // Rhyhorn
            new EncounterSlot { Species = 112, LevelMin = 44, LevelMax = 44, Type = SlotType.Grass_Safari }, // Rhydon
            new EncounterSlot { Species = 128, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Tauros
            new EncounterSlot { Species = 128, LevelMin = 41, LevelMax = 41, Type = SlotType.Grass_Safari }, // Tauros
            new EncounterSlot { Species = 228, LevelMin = 42, LevelMax = 42, Type = SlotType.Grass_Safari }, // Houndour
            new EncounterSlot { Species = 263, LevelMin = 38, LevelMax = 38, Type = SlotType.Grass_Safari }, // Zigzagoon
            new EncounterSlot { Species = 285, LevelMin = 45, LevelMax = 45, Type = SlotType.Grass_Safari }, // Shroomish
            new EncounterSlot { Species = 298, LevelMin = 42, LevelMax = 42, Type = SlotType.Grass_Safari }, // Azurill
            new EncounterSlot { Species = 324, LevelMin = 46, LevelMax = 47, Type = SlotType.Grass_Safari }, // Torkoal
            new EncounterSlot { Species = 332, LevelMin = 42, LevelMax = 42, Type = SlotType.Grass_Safari }, // Cacturne
            new EncounterSlot { Species = 404, LevelMin = 45, LevelMax = 46, Type = SlotType.Grass_Safari }, // Luxio
        };

        private static readonly EncounterSlot[] SAFARIZONE_WETLAND =
        {
            new EncounterSlot { Species = 021, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Spearow
            new EncounterSlot { Species = 054, LevelMin = 15, LevelMax = 16, Type = SlotType.Grass_Safari }, // Psyduck
            new EncounterSlot { Species = 054, LevelMin = 16, LevelMax = 17, Type = SlotType.Surf_Safari }, // Psyduck
            new EncounterSlot { Species = 055, LevelMin = 17, LevelMax = 17, Type = SlotType.Grass_Safari }, // Golduck
            new EncounterSlot { Species = 055, LevelMin = 40, LevelMax = 40, Type = SlotType.Grass_Safari }, // Golduck
            new EncounterSlot { Species = 055, LevelMin = 37, LevelMax = 37, Type = SlotType.Surf_Safari }, // Golduck
            new EncounterSlot { Species = 055, LevelMin = 45, LevelMax = 45, Type = SlotType.Surf_Safari }, // Golduck
            new EncounterSlot { Species = 060, LevelMin = 12, LevelMax = 15, Type = SlotType.Old_Rod_Safari }, // Poliwag
            new EncounterSlot { Species = 060, LevelMin = 22, LevelMax = 24, Type = SlotType.Good_Rod_Safari }, // Poliwag
            new EncounterSlot { Species = 060, LevelMin = 35, LevelMax = 37, Type = SlotType.Super_Rod_Safari }, // Poliwag
            new EncounterSlot { Species = 060, LevelMin = 15, LevelMax = 16, Type = SlotType.Surf_Safari }, // Poliwag
            new EncounterSlot { Species = 061, LevelMin = 17, LevelMax = 18, Type = SlotType.Old_Rod_Safari }, // Poliwhirl
            new EncounterSlot { Species = 061, LevelMin = 23, LevelMax = 25, Type = SlotType.Good_Rod_Safari }, // Poliwhirl
            new EncounterSlot { Species = 061, LevelMin = 35, LevelMax = 37, Type = SlotType.Super_Rod_Safari }, // Poliwhirl
            new EncounterSlot { Species = 083, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Farfetch'd
            new EncounterSlot { Species = 083, LevelMin = 41, LevelMax = 41, Type = SlotType.Grass_Safari }, // Farfetch'd
            new EncounterSlot { Species = 084, LevelMin = 45, LevelMax = 45, Type = SlotType.Grass_Safari }, // Doduo
            new EncounterSlot { Species = 129, LevelMin = 12, LevelMax = 15, Type = SlotType.Old_Rod_Safari }, // Magikarp
            new EncounterSlot { Species = 130, LevelMin = 44, LevelMax = 45, Type = SlotType.Super_Rod_Safari }, // Gyarados
            new EncounterSlot { Species = 130, LevelMin = 47, LevelMax = 48, Type = SlotType.Super_Rod_Safari }, // Gyarados
            new EncounterSlot { Species = 132, LevelMin = 17, LevelMax = 17, Type = SlotType.Grass_Safari }, // Ditto
            new EncounterSlot { Species = 132, LevelMin = 41, LevelMax = 41, Type = SlotType.Grass_Safari }, // Ditto
            new EncounterSlot { Species = 161, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Sentret
            new EncounterSlot { Species = 162, LevelMin = 37, LevelMax = 37, Type = SlotType.Grass_Safari }, // Furret
            new EncounterSlot { Species = 194, LevelMin = 15, LevelMax = 17, Type = SlotType.Grass_Safari }, // Wooper
            new EncounterSlot { Species = 194, LevelMin = 15, LevelMax = 16, Type = SlotType.Surf_Safari }, // Wooper
            new EncounterSlot { Species = 195, LevelMin = 16, LevelMax = 17, Type = SlotType.Grass_Safari }, // Quagsire
            new EncounterSlot { Species = 195, LevelMin = 16, LevelMax = 17, Type = SlotType.Surf_Safari }, // Quagsire
            new EncounterSlot { Species = 195, LevelMin = 37, LevelMax = 37, Type = SlotType.Surf_Safari }, // Quagsire
            new EncounterSlot { Species = 271, LevelMin = 47, LevelMax = 47, Type = SlotType.Grass_Safari }, // Lombre
            new EncounterSlot { Species = 283, LevelMin = 40, LevelMax = 40, Type = SlotType.Grass_Safari }, // Surskit
            new EncounterSlot { Species = 341, LevelMin = 26, LevelMax = 26, Type = SlotType.Good_Rod_Safari }, // Corphish
            new EncounterSlot { Species = 341, LevelMin = 28, LevelMax = 28, Type = SlotType.Good_Rod_Safari }, // Corphish
            new EncounterSlot { Species = 372, LevelMin = 46, LevelMax = 46, Type = SlotType.Grass_Safari }, // Shelgon
            new EncounterSlot { Species = 417, LevelMin = 43, LevelMax = 43, Type = SlotType.Grass_Safari }, // Pachirisu
            new EncounterSlot { Species = 418, LevelMin = 44, LevelMax = 45, Type = SlotType.Grass_Safari }, // Buizel
        };

        private static readonly EncounterArea4HGSS SlotsHGSS_SafariZone = new EncounterArea4HGSS
        {
            // Source http://bulbapedia.bulbagarden.net/wiki/Johto_Safari_Zone#Pok.C3.A9mon
            // Supplement http://www.psypokes.com/hgss/safari_areas.php
            Location = 202, // Johto Safari Zone
            Slots = ArrayUtil.ConcatAll(
                SAFARIZONE_PEAK,
                SAFARIZONE_DESERT,
                SAFARIZONE_PLAINS,
                SAFARIZONE_MEADOW,
                SAFARIZONE_FOREST,
                SAFARIZONE_SWAMP,
                SAFARIZONE_MARSHLAND,
                SAFARIZONE_MOUNTAIN,
                SAFARIZONE_ROCKYBEACH,
                SAFARIZONE_WASTELAND,
                SAFARIZONE_SAVANNAH,
                SAFARIZONE_WETLAND)
        };

        private static readonly EncounterArea4HGSS[] SlotsHGSSAlt =
        {
            SlotsHGSS_BCC,
            new EncounterArea4HGSS {
                Location = 209, // Ruins of Alph
                Slots = new int[25].Select((_, i) => new EncounterSlot { Species = 201, LevelMin = 5, LevelMax = 5, Type = SlotType.Grass, Form = i+1 }).ToArray() // B->?, Unown A is loaded from encounters raw file
            },
            SlotsHGSS_SafariZone,
            //Some edge cases
            new EncounterArea4HGSS
            {
                Location = 219, // Mt. Silver Cave 1F
                Slots = new[]{new EncounterSlot { Species = 130, LevelMin = 20, LevelMax = 20, Type = SlotType.Good_Rod },}, // Gyarados at night
            },
        };

        private static readonly EncounterArea4DPPt SlotsPt_HoneyTree = new EncounterArea4DPPt
        {
            Slots = new[]
            {
                new EncounterSlot {Species = 190, LevelMin = 5, LevelMax = 15, Type = SlotType.HoneyTree}, // Aipom
                new EncounterSlot {Species = 214, LevelMin = 5, LevelMax = 15, Type = SlotType.HoneyTree}, // Heracross
                new EncounterSlot {Species = 265, LevelMin = 5, LevelMax = 15, Type = SlotType.HoneyTree}, // Wurmple
                new EncounterSlot {Species = 412, LevelMin = 5, LevelMax = 15, Type = SlotType.HoneyTree, Form = 0}, // Burmy Plant Cloak
                new EncounterSlot {Species = 415, LevelMin = 5, LevelMax = 15, Type = SlotType.HoneyTree}, // Combee
                new EncounterSlot {Species = 420, LevelMin = 5, LevelMax = 15, Type = SlotType.HoneyTree}, // Cheruby
                new EncounterSlot {Species = 446, LevelMin = 5, LevelMax = 15, Type = SlotType.HoneyTree}, // Munchlax
            },
        };

        private static readonly EncounterArea4DPPt SlotsD_HoneyTree = new EncounterArea4DPPt
        {
            Slots = SlotsPt_HoneyTree.Slots.Concat(new[]
            {
                new EncounterSlot {Species = 266, LevelMin = 5, LevelMax = 15, Type = SlotType.HoneyTree}, // Silcoon
            }).ToArray()
        };

        private static readonly EncounterArea4DPPt SlotsP_HoneyTree = new EncounterArea4DPPt
        {
            Slots = SlotsPt_HoneyTree.Slots.Concat(new[]
            {
                new EncounterSlot {Species = 268, LevelMin = 5, LevelMax = 15, Type = SlotType.HoneyTree}, // Cascoon
            }).ToArray()
        };

        internal static readonly int[] TrophyDP = { 035, 039, 052, 113, 133, 137, 173, 174, 183, 298, 311, 312, 351, 438, 439, 440 }; // Porygon
        internal static readonly int[] TrophyPt = { 035, 039, 052, 113, 133, 132, 173, 174, 183, 298, 311, 312, 351, 438, 439, 440 }; // Ditto

        private static readonly int[] DP_GreatMarshAlt_Species =
        {
            // Daily changing Pokemon are not in the raw data http://bulbapedia.bulbagarden.net/wiki/Great_Marsh
            055, 315, 397, 451, 453, 455,
            183, 194, 195, 298, 399, 400,          // Pre-National Pokédex
            046, 102, 115, 193, 285, 316, 452, 454 // Post-National Pokédex
        };

        private static readonly EncounterArea4DPPt[] DP_GreatMarshAlt = EncounterArea.GetSimpleEncounterArea<EncounterArea4DPPt>(DP_GreatMarshAlt_Species, new[] { 22, 22, 24, 24, 26, 26 }, 52, SlotType.Grass_Safari);

        private static readonly int[] Pt_GreatMarshAlt_Species =
        {
            114,193,195,357,451,453,455,
            194,                            // Pre-National Pokédex
            046,102,115,285,316,352,452,454 // Post-National Pokédex
        };

        private static readonly EncounterArea4DPPt[] Pt_GreatMarshAlt = EncounterArea.GetSimpleEncounterArea<EncounterArea4DPPt>(Pt_GreatMarshAlt_Species, new[] { 27, 30 }, 52, SlotType.Grass_Safari);

        private static readonly int[] Shellos_EastSeaLocation_DP =
        {
            28, // Route 213
            39, // Route 224
        };

        private static readonly int[] Shellos_EastSeaLocation_Pt =
        {
            11, // Pastoria City
            27, // Route 212
            28, // Route 213
        };

        private static readonly int[] Gastrodon_EastSeaLocation_DP =
        {
            37, // Route 222
            39, // Route 224
            45, // Route 230
        };

        private static readonly int[] Gastrodon_EastSeaLocation_Pt =
        {
            11, // Pastoria City
            27, // Route 212
            28, // Route 213
            39, // Route 224
            45, // Route 230
        };

        private static readonly int[] HoneyTreesLocation =
        {
            20, // Route 205
            21, // Route 206
            22, // Route 207
            23, // Route 208
            24, // Route 209
            25, // Route 210
            26, // Route 211
            27, // Route 212
            28, // Route 213
            29, // Route 214
            30, // Route 215
            33, // Route 218
            36, // Route 221
            37, // Route 222
            47, // Valley Windworks
            48, // Eterna Forest
            49, // Fuego Ironworks
            58, // Floaroma Meadow
        };

        private static readonly EncounterArea4HGSS[] SlotsHGSS_Swarm =
        {
            new EncounterArea4HGSS {Location = 143, Slots = new[]{new EncounterSlot {Species = 278, Type = SlotType.Surf },},}, // Wingull @ Vermillion City
            new EncounterArea4HGSS {Location = 149, Slots = new[]{new EncounterSlot {Species = 261, Type = SlotType.Grass },},}, // Poochyena @ Route 1
            new EncounterArea4HGSS {Location = 161, Slots = new[]{new EncounterSlot {Species = 113, Type = SlotType.Grass },},}, // Chansey @ Route 13
            new EncounterArea4HGSS {Location = 167, Slots = new[]{new EncounterSlot {Species = 366, Type = SlotType.Surf },},}, // Clamperl @ Route 19
            new EncounterArea4HGSS {Location = 173, Slots = new[]{new EncounterSlot {Species = 427, Type = SlotType.Grass },},}, // Buneary @ Route 25
            new EncounterArea4HGSS {Location = 175, Slots = new[]{new EncounterSlot {Species = 370, Type = SlotType.Surf },},}, // Luvdisc @ Route 27
            new EncounterArea4HGSS {Location = 182, Slots = new[]{new EncounterSlot {Species = 280, Type = SlotType.Grass },},}, // Ralts @ Route 34
            new EncounterArea4HGSS {Location = 183, Slots = new[]{new EncounterSlot {Species = 193, Type = SlotType.Grass },},}, // Yanma @ Route 35
            new EncounterArea4HGSS {Location = 186, Slots = new[]{new EncounterSlot {Species = 209, Type = SlotType.Grass },},}, // Snubbull @ Route 38
            new EncounterArea4HGSS {Location = 193, Slots = new[]{new EncounterSlot {Species = 333, Type = SlotType.Grass },},}, // Swablu @ Route 45
            new EncounterArea4HGSS {Location = 195, Slots = new[]{new EncounterSlot {Species = 132, Type = SlotType.Grass },},}, // Ditto @ Route 47
            new EncounterArea4HGSS {Location = 216, Slots = new[]{new EncounterSlot {Species = 183, Type = SlotType.Grass },},}, // Marill @ Mt. Mortar
            new EncounterArea4HGSS {Location = 220, Slots = new[]{new EncounterSlot {Species = 206, Type = SlotType.Grass },},}, // Dunsparce @ Dark Cave
            new EncounterArea4HGSS {Location = 224, Slots = new[]{new EncounterSlot {Species = 401, Type = SlotType.Grass },},}, // Kricketot @ Viridian Forest

            new EncounterArea4HGSS {Location = 128, Slots = new[]{ // Whiscash @ Violet City
                new EncounterSlot {Species = 340, Type = SlotType.Old_Rod },
                new EncounterSlot {Species = 340, Type = SlotType.Good_Rod },
                new EncounterSlot {Species = 340, Type = SlotType.Super_Rod },
            },},
            new EncounterArea4HGSS {Location = 160, Slots = new[]{ // Relicanth @ Route 12
                new EncounterSlot {Species = 369, Type = SlotType.Old_Rod },
                new EncounterSlot {Species = 369, Type = SlotType.Good_Rod },
                new EncounterSlot {Species = 369, Type = SlotType.Super_Rod },
            },},
            new EncounterArea4HGSS {Location = 180, Slots = new[]{ // Qwilfish @ Route 32
                new EncounterSlot {Species = 211, Type = SlotType.Old_Rod },
                new EncounterSlot {Species = 211, Type = SlotType.Good_Rod },
                new EncounterSlot {Species = 211, Type = SlotType.Super_Rod },
            },},
            new EncounterArea4HGSS {Location = 192, Slots = new[]{ // Remoraid @ Route 44
                new EncounterSlot {Species = 223, Type = SlotType.Old_Rod },
                new EncounterSlot {Species = 223, Type = SlotType.Good_Rod },
                new EncounterSlot {Species = 223, Type = SlotType.Super_Rod },
            },},
        };

        private static readonly EncounterArea4HGSS[] SlotsHG_Swarm = SlotsHGSS_Swarm.Concat(new[] {
            new EncounterArea4HGSS {Location = 151, Slots = new[]{new EncounterSlot {Species = 343, Type = SlotType.Grass },},}, // Baltoy @ Route 3
            new EncounterArea4HGSS {Location = 157, Slots = new[]{new EncounterSlot {Species = 302, Type = SlotType.Grass },},}, // Sableye @ Route 9
        }).ToArray();

        private static readonly EncounterArea4HGSS[] SlotsSS_Swarm = SlotsHGSS_Swarm.Concat(new[] {
            new EncounterArea4HGSS {Location = 151, Slots = new[]{new EncounterSlot {Species = 316, Type = SlotType.Grass },},}, // Gulpin @ Route 3
            new EncounterArea4HGSS {Location = 157, Slots = new[]{new EncounterSlot {Species = 303, Type = SlotType.Grass },},}, // Mawile @ Route 9
        }).ToArray();

        #endregion
    }
}
