using System;
using System.Collections.Generic;
using static PKHeX.Core.Legal;

namespace PKHeX.Core
{
    internal static class MoveEgg
    {
        internal static int[] GetEggMoves(PKM pkm, int species, int formnum, GameVersion version)
        {
            int gen = pkm.Format <= 2 || pkm.VC ? 2 : pkm.GenNumber;
            if (!pkm.InhabitedGeneration(gen, species) || (pkm.PersonalInfo.Gender == 255 && !FixedGenderFromBiGender.Contains(species)))
                return Array.Empty<int>();

            if (pkm.Version == 15 || pkm.GG)
                return Array.Empty<int>();

            if (version == GameVersion.Any)
                version = (GameVersion)pkm.Version;
            return GetEggMoves(gen, species, formnum, version);
        }

        internal static int[] GetEggMoves(int gen, int species, int formnum, GameVersion version)
        {
            switch (gen)
            {
                case 1:
                case 2:
                    return (version == GameVersion.C ? EggMovesC : EggMovesGS)[species].Moves;
                case 3:
                    return EggMovesRS[species].Moves;
                case 4:
                    return version switch
                    {
                        GameVersion.HG => EggMovesHGSS[species].Moves,
                        GameVersion.SS => EggMovesHGSS[species].Moves,
                        _ => EggMovesDPPt[species].Moves
                    };
                case 5:
                    return EggMovesBW[species].Moves;
                case 6: // entries per species
                    return version switch
                    {
                        GameVersion.OR => EggMovesAO[species].Moves,
                        GameVersion.AS => EggMovesAO[species].Moves,
                        _ => EggMovesXY[species].Moves
                    };

                case 7: // entries per form if required
                    return version switch
                    {
                        GameVersion.US => GetFormEggMoves(species, formnum, EggMovesUSUM),
                        GameVersion.UM => GetFormEggMoves(species, formnum, EggMovesUSUM),
                        _ => GetFormEggMoves(species, formnum, EggMovesSM)
                    };

                case 8:
                    return version switch
                    {
                        _ => GetFormEggMoves(species, formnum, EggMovesSWSH)
                    };

                default:
                    return Array.Empty<int>();
            }
        }

        private static int[] GetFormEggMoves(int species, int formnum, IReadOnlyList<EggMoves7> table)
        {
            var entry = table[species];
            if (formnum > 0 && entry.FormTableIndex > species)
                entry = table[entry.FormTableIndex + formnum - 1];
            return entry.Moves;
        }

        internal static int[] GetRelearnLVLMoves(PKM pkm, int species, int lvl, int formnum, GameVersion version = GameVersion.Any)
        {
            if (version == GameVersion.Any)
                version = (GameVersion)pkm.Version;
            // A pkm can only have levelup relearn moves from the game it originated on
            // eg Plusle/Minun have Charm/Fake Tears (respectively) only in OR/AS, not X/Y
            switch (version)
            {
                case GameVersion.X:
                case GameVersion.Y:
                    return getMoves(LevelUpXY, PersonalTable.XY);
                case GameVersion.AS:
                case GameVersion.OR:
                    return getMoves(LevelUpAO, PersonalTable.AO);

                case GameVersion.SN:
                case GameVersion.MN:
                    if (species > MaxSpeciesID_7)
                        break;
                    return getMoves(LevelUpSM, PersonalTable.SM);
                case GameVersion.US:
                case GameVersion.UM:
                    return getMoves(LevelUpUSUM, PersonalTable.USUM);

                case GameVersion.SW:
                case GameVersion.SH:
                    return getMoves(LevelUpSWSH, PersonalTable.SWSH);
            }
            return Array.Empty<int>();

            int[] getMoves(IReadOnlyList<Learnset> moves, PersonalTable table) => moves[table.GetFormeIndex(species, formnum)].GetMoves(lvl);
        }

        public static bool GetIsSharedEggMove(PKM pkm, int gen, int move)
        {
            if (gen < 8 || pkm.IsEgg)
                return false;
            var table = PersonalTable.SWSH;
            var entry = (PersonalInfoSWSH)table.GetFormeEntry(pkm.Species, pkm.AltForm);
            var baseSpecies = entry.BaseSpecies;
            var baseForm = entry.FormIndex;

            // since we aren't storing entry->seed_poke_index, there's oddballs we can't handle with just personal data (?)
            if (pkm.Species == (int)Species.Indeedee)
                baseForm = pkm.AltForm;

            var egg = GetEggMoves(8, baseSpecies, baseForm, GameVersion.SW);
            return Array.Exists(egg, z => z == move);
        }
    }
}
