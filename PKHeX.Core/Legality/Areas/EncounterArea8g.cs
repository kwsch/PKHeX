using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="GameVersion.GO"/> encounter area for direct-to-HOME transfers.
    /// </summary>
    public sealed class EncounterArea8g : EncounterArea
    {
        private EncounterArea8g() : base(GameVersion.GO) { }

        internal static EncounterArea8g[] GetArea(int maxSpecies, HashSet<int> banlist, IEnumerable<int> extras)
        {
            var area = new EncounterArea8g { Location = Locations.GO8, Type = SlotType.GoPark };
            var all = GetSlots(area, maxSpecies, banlist, extras);
            area.Slots = all.ToArray();
            return new[] { area };
        }

        private static EncounterSlot8GO GetSlot(EncounterArea8g area, int species, int form, GameVersion baseOrigin)
        {
            var min = EncountersGO.GetMinLevel(species, form);
            return new EncounterSlot8GO(area, species, form, baseOrigin, min, 40);
        }

        private static IEnumerable<EncounterSlot> GetSlots(EncounterArea8g area, int maxSpecies, HashSet<int> banlist, IEnumerable<int> extras)
        {
            // Gen7: GO transfers to LGPE cannot send Mew.
            // Gen8: GO transfers to HOME can send Mew. Iterate from here.
            // However, Mew transfers with LGPE base moves. Because everything <= 151 uses LGPE level-up table. Handle manually!
            yield return GetSlot(area, (int)Species.Mew, 0, GameVersion.GG);
            const int start = 1;
            var speciesList = Enumerable.Range(start, maxSpecies - start + 1).Concat(extras);

            var pt7 = PersonalTable.USUM;
            var pt8 = PersonalTable.SWSH;
            var ptGG = PersonalTable.GG;
            foreach (var specform in speciesList)
            {
                var species = specform & 0x7FF;
                if (banlist.Contains(species))
                    continue;

                var pi8 = (PersonalInfoSWSH)pt8[species];
                if (pi8.IsPresentInGame)
                {
                    for (int f = 0; f < pi8.FormeCount; f++)
                    {
                        var sf = species | (f << 11);
                        if (banlist.Contains(sf))
                            continue;
                        if (IsDisallowedDuplicateForm(species, f))
                            continue;
                        bool lgpe = (species <= 151 || species == 808 || species == 809) && ptGG[species].HasForme(f);
                        var game = lgpe ? GameVersion.GG : GameVersion.SWSH;
                        yield return GetSlot(area, species, f, game);
                    }
                }
                else if (species <= Legal.MaxSpeciesID_7_USUM)
                {
                    var pi7 = pt7[species];
                    for (int f = 0; f < pi7.FormeCount; f++)
                    {
                        var sf = species | (f << 11);
                        if (banlist.Contains(sf))
                            continue;
                        if (IsDisallowedDuplicateForm(species, f))
                            continue;
                        bool lgpe = species <= 151 && ptGG[species].HasForme(f);
                        var game = lgpe ? GameVersion.GG : GameVersion.USUM;
                        yield return GetSlot(area, species, f, game);
                    }
                }
            }
        }

        private static bool IsDisallowedDuplicateForm(int species, int f)
        {
            if (AltFormInfo.IsBattleOnlyForm(species, f, 8))
                return true;
            if (AltFormInfo.IsFusedForm(species, f, 8))
                return true;
            if (species == (int) Species.Mothim)
                return f != 0; // Reverts to Plant Cloak on transfer.
            return false;
        }

        public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<EvoCriteria> chain)
        {
            if (pkm.TSV == 0) // HOME doesn't assign TSV=0 to accounts.
                yield break;

            foreach (var slot in Slots)
            {
                foreach (var evo in chain)
                {
                    if (slot.Species != evo.Species)
                        continue;

                    if (!slot.IsLevelWithinRange(pkm.Met_Level))
                        break;
                    if (slot.Form != evo.Form)
                        break;

                    yield return slot;
                    break;
                }
            }
        }
    }
}
