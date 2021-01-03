using System;
using System.Collections.Generic;
using static PKHeX.Core.Legal;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    public static class MoveEgg
    {
        public static int[] GetEggMoves(PersonalInfo pi, int species, int form, GameVersion version, int generation)
        {
            if (species > GetMaxSpeciesOrigin(generation))
                return Array.Empty<int>();

            if (pi.Genderless && !FixedGenderFromBiGender.Contains(species))
                return Array.Empty<int>();

            if (!Breeding.CanGameGenerateEggs(version))
                return Array.Empty<int>();

            return GetEggMoves(generation, species, form, version);
        }

        public static int[] GetEggMoves(int generation, int species, int form, GameVersion version) => generation switch
        {
            1 or 2 => (version == C ? EggMovesC : EggMovesGS)[species].Moves,
            3 => EggMovesRS[species].Moves,
            4 when version is D or P or Pt => EggMovesDPPt[species].Moves,
            4 when version is HG or SS => EggMovesHGSS[species].Moves,
            5 => EggMovesBW[species].Moves,

            6 when version is X or Y => EggMovesXY[species].Moves,
            6 when version is OR or AS => EggMovesAO[species].Moves,

            7 when version is SN or MN => GetFormEggMoves(species, form, EggMovesSM),
            7 when version is US or UM => GetFormEggMoves(species, form, EggMovesUSUM),
            8 => GetFormEggMoves(species, form, EggMovesSWSH),
            _ => Array.Empty<int>(),
        };

        private static int[] GetFormEggMoves(int species, int form, IReadOnlyList<EggMoves7> table)
        {
            var entry = table[species];
            if (form > 0 && entry.FormTableIndex > species)
                entry = table[entry.FormTableIndex + form - 1];
            return entry.Moves;
        }

        internal static int[] GetRelearnLVLMoves(PKM pkm, int species, int form, int lvl, GameVersion version = Any)
        {
            if (version == Any)
                version = (GameVersion)pkm.Version;
            // A pkm can only have levelup relearn moves from the game it originated on
            // eg Plusle/Minun have Charm/Fake Tears (respectively) only in OR/AS, not X/Y
            return version switch
            {
                X or Y => getMoves(LevelUpXY, PersonalTable.XY),
                OR or AS => getMoves(LevelUpAO, PersonalTable.AO),
                SN or MN when species <= MaxSpeciesID_7 => getMoves(LevelUpSM, PersonalTable.SM),
                US or UM => getMoves(LevelUpUSUM, PersonalTable.USUM),
                SW or SH => getMoves(LevelUpSWSH, PersonalTable.SWSH),
                _ => Array.Empty<int>(),
            };

            int[] getMoves(IReadOnlyList<Learnset> moves, PersonalTable table) => moves[table.GetFormIndex(species, form)].GetMoves(lvl);
        }

        public static bool GetIsSharedEggMove(PKM pkm, int gen, int move)
        {
            if (gen < 8 || pkm.IsEgg)
                return false;
            var table = PersonalTable.SWSH;
            var entry = (PersonalInfoSWSH)table.GetFormEntry(pkm.Species, pkm.Form);
            var baseSpecies = entry.HatchSpecies;
            var baseForm = entry.HatchFormIndexEverstone;
            var egg = GetEggMoves(8, baseSpecies, baseForm, SW);
            return Array.Exists(egg, z => z == move);
        }
    }
}
