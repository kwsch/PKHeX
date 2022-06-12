using System;
using System.Collections.Generic;

using static PKHeX.Core.Legal;

namespace PKHeX.Core
{
    public static class MoveTutor
    {
        public static GameVersion GetIsTutorMove(PKM pkm, int species, int form, int generation, int move, bool specialTutors = true)
        {
            return generation switch
            {
                1 => GetIsTutor1(pkm, species, move),
                2 => GetIsTutor2(pkm, species, move),
                3 => GetIsTutor3(species, move),
                4 => GetIsTutor4(species, form, move),
                5 => GetIsTutor5(pkm, species, form, specialTutors, move),
                6 => GetIsTutor6(pkm, species, form, specialTutors, move),
                7 => GetIsTutor7(pkm, species, form, specialTutors, move),
                8 => GetIsTutor8(pkm, species, form, specialTutors, move),
                _ => NONE,
            };
        }

        private static GameVersion GetIsTutor1(PKM pkm, int species, int move)
        {
            // Surf Pikachu via Stadium
            if (move != (int)Move.Surf || ParseSettings.AllowGBCartEra)
                return NONE;
            if (pkm.Format < 3 && species is (int)Species.Pikachu or (int)Species.Raichu)
                return GameVersion.Stadium;
            return NONE;
        }

        private static GameVersion GetIsTutor2(PKM pkm, int species, int move)
        {
            if (!ParseSettings.AllowGen2Crystal(pkm))
                return NONE;
            var info = PersonalTable.C[species];
            var tutor = Array.IndexOf(Tutors_GSC, move);
            if (tutor != -1 && info.TMHM[57 + tutor])
                return GameVersion.C;
            return NONE;
        }

        private static GameVersion GetIsTutor3(int species, int move)
        {
            // E Tutors (Free)
            // E Tutors (BP)
            var info = PersonalTable.E[species];
            var e = Array.IndexOf(Tutor_E, move);
            if (e != -1 && info.TypeTutors[e])
                return GameVersion.E;

            // FRLG Tutors
            // Only special tutor moves, normal tutor moves are already included in Emerald data
            var frlg = Array.IndexOf(SpecialTutors_FRLG, move);
            if (frlg != -1 && SpecialTutors_Compatibility_FRLG[frlg] == species)
                return GameVersion.FRLG;

            // XD
            var xd = Array.IndexOf(SpecialTutors_XD_Exclusive, move);
            if (xd != -1 && SpecialTutors_Compatibility_XD_Exclusive[xd].AsSpan().IndexOf(species) != -1)
                return GameVersion.XD;

            // XD (Mew)
            if (species == (int)Species.Mew && Tutor_3Mew.AsSpan().IndexOf(move) != -1)
                return GameVersion.XD;

            return NONE;
        }

        private static GameVersion GetIsTutor4(int species, int form, int move)
        {
            var pi = PersonalTable.HGSS[species, form];
            var type = Array.IndexOf(Tutors_4, move);
            if (type != -1 && pi.TypeTutors[type])
                return GameVersion.Gen4;

            var special = Array.IndexOf(SpecialTutors_4, move);
            if (special != -1 && SpecialTutors_Compatibility_4[special].AsSpan().IndexOf(species) != -1)
                return GameVersion.HGSS;

            return NONE;
        }

        private static GameVersion GetIsTutor5(PKM pkm, int species, int form, bool specialTutors, int move)
        {
            var pi = PersonalTable.B2W2[species, form];
            var type = Array.IndexOf(TypeTutor6, move);
            if (type != -1 && pi.TypeTutors[type])
                    return GameVersion.Gen5;

            if (specialTutors && pkm.HasVisitedB2W2(species))
            {
                var tutors = Tutors_B2W2;
                for (int i = 0; i < tutors.Length; i++)
                {
                    var tutor = Array.IndexOf(tutors[i], move);
                    if (tutor == -1)
                        continue;
                    if (pi.SpecialTutors[i][tutor])
                        return GameVersion.B2W2;
                    break;
                }
            }

            return NONE;
        }

        private static GameVersion GetIsTutor6(PKM pkm, int species, int form, bool specialTutors, int move)
        {
            var pi = PersonalTable.AO[species, form];
            var type = Array.IndexOf(TypeTutor6, move);
            if (type != -1 && pi.TypeTutors[type])
                return GameVersion.Gen6;

            if (specialTutors && pkm.HasVisitedORAS(species))
            {
                var tutors = Tutors_AO;
                for (int i = 0; i < tutors.Length; i++)
                {
                    var tutor = Array.IndexOf(tutors[i], move);
                    if (tutor == -1)
                        continue;
                    if (pi.SpecialTutors[i][tutor])
                        return GameVersion.ORAS;
                    break;
                }
            }

            return NONE;
        }

        private static GameVersion GetIsTutor7(PKM pkm, int species, int form, bool specialTutors, int move)
        {
            var pi = PersonalTable.USUM[species, form];
            var type = Array.IndexOf(TypeTutor6, move);
            if (type != -1 && pi.TypeTutors[type])
                return GameVersion.Gen7;

            if (specialTutors && pkm.HasVisitedUSUM(species))
            {
                var tutor = Array.IndexOf(Tutors_USUM, move);
                if (tutor != -1 && pi.SpecialTutors[0][tutor])
                    return GameVersion.USUM;
            }

            return NONE;
        }

        private static GameVersion GetIsTutor8(PKM pkm, int species, int form, bool specialTutors, int move)
        {
            if (pkm is PA8)
            {
                var pi = (PersonalInfoLA)PersonalTable.LA[species, form];
                if (!pi.IsPresentInGame)
                    return NONE;
                var index = Array.IndexOf(MoveShop8_LA, (ushort)move);
                if (index != -1 && pi.SpecialTutors[0][index])
                    return GameVersion.PLA;

                return NONE;
            }
            if (pkm is PB8)
            {
                var pi = (PersonalInfoBDSP)PersonalTable.BDSP[species, form];
                if (!pi.IsPresentInGame)
                    return NONE;
                var type = Array.IndexOf(TypeTutor8b, move);
                if (type != -1 && pi.TypeTutors[type])
                    return GameVersion.BDSP;

                return NONE;
            }
            else
            {
                var pi = (PersonalInfoSWSH)PersonalTable.SWSH[species, form];
                if (!pi.IsPresentInGame)
                    return NONE;
                var type = Array.IndexOf(TypeTutor8, move);
                if (type != -1 && pi.TypeTutors[type])
                    return GameVersion.SWSH;

                if (!specialTutors)
                    return NONE;

                var tutor = Array.IndexOf(Tutors_SWSH_1, move);
                if (tutor != -1 && pi.SpecialTutors[0][tutor])
                    return GameVersion.SWSH;

                return NONE;
            }
        }

        public static IEnumerable<int> GetTutorMoves(PKM pkm, int species, int form, bool specialTutors, int generation)
        {
            List<int> moves = new();
            switch (generation)
            {
                case 1: AddMovesTutor1(moves, species, pkm.Format); break;
                case 2: AddMovesTutor2(moves, species, pkm.Format, pkm.Korean); break;
                case 3: AddMovesTutor3(moves, species); break;
                case 4: AddMovesTutor4(moves, species, form); break;
                case 5: AddMovesTutor5(moves, species, form, pkm, specialTutors); break;
                case 6: AddMovesTutor6(moves, species, form, pkm, specialTutors); break;
                case 7: AddMovesTutor7(moves, species, form, pkm, specialTutors); break;
                case 8: AddMovesTutor8(moves, species, form, pkm, specialTutors); break;
            }
            return moves;
        }

        private static void AddMovesTutor1(List<int> moves, int species, int format)
        {
            if (ParseSettings.AllowGBCartEra && format < 3 && species is (int)Species.Pikachu or (int)Species.Raichu) // Surf Pikachu via Stadium
                moves.Add(57);
        }

        private static void AddMovesTutor2(List<int> moves, int species, int format, bool korean = false)
        {
            if (korean)
                return;
            var pi = PersonalTable.C[species];
            var hmBits = pi.TMHM.AsSpan(57, 3);
            for (int i = 0; i < Tutors_GSC.Length; i++)
            {
                if (hmBits[i])
                    moves.Add(Tutors_GSC[i]);
            }
            AddMovesTutor1(moves, species, format);
        }

        private static void AddMovesTutor3(List<int> moves, int species)
        {
            // E Tutors (Free)
            // E Tutors (BP)
            var pi = PersonalTable.E[species];
            AddPermittedIndexes(moves, Tutor_E, pi.TypeTutors);
            // FRLG Tutors
            // Only special tutor moves, normal tutor moves are already included in Emerald data
            AddIfPermitted(moves, SpecialTutors_FRLG, SpecialTutors_Compatibility_FRLG, species);
            // XD
            for (int i = 0; i < SpecialTutors_XD_Exclusive.Length; i++)
            {
                var allowed = SpecialTutors_Compatibility_XD_Exclusive[i].AsSpan();
                var index = allowed.IndexOf(species);
                if (index != -1)
                    moves.Add(SpecialTutors_XD_Exclusive[i]);
            }
            // XD (Mew)
            if (species == (int)Species.Mew)
                moves.AddRange(Tutor_3Mew);
        }

        private static void AddMovesTutor4(List<int> moves, int species, int form)
        {
            var pi = PersonalTable.HGSS[species, form];
            AddPermittedIndexes(moves, Tutors_4, pi.TypeTutors);
            for (int i = 0; i < SpecialTutors_4.Length; i++)
            {
                var allowed = SpecialTutors_Compatibility_4[i].AsSpan();
                var index = allowed.IndexOf(species);
                if (index != -1)
                    moves.Add(SpecialTutors_4[i]);
            }
        }

        private static void AddMovesTutor5(List<int> moves, int species, int form, PKM pkm, bool specialTutors)
        {
            var pi = PersonalTable.B2W2[species, form];
            AddPermittedIndexes(moves, TypeTutor6, pi.TypeTutors);
            if (pkm.InhabitedGeneration(5) && specialTutors)
                AddPermittedIndexes(moves, Tutors_B2W2, pi.SpecialTutors);
        }

        private static void AddMovesTutor6(List<int> moves, int species, int form, PKM pkm, bool specialTutors)
        {
            var pi = PersonalTable.AO[species, form];
            AddPermittedIndexes(moves, TypeTutor6, pi.TypeTutors);
            if (specialTutors && pkm.HasVisitedORAS(species))
                AddPermittedIndexes(moves, Tutors_AO, pi.SpecialTutors);
        }

        private static void AddMovesTutor7(List<int> moves, int species, int form, PKM pkm, bool specialTutors)
        {
            if (pkm.Version is (int)GameVersion.GO or (int)GameVersion.GP or (int)GameVersion.GE)
                return;
            var pi = PersonalTable.USUM[species, form];
            AddPermittedIndexes(moves, TypeTutor6, pi.TypeTutors);
            if (specialTutors && pkm.HasVisitedUSUM(species))
                AddPermittedIndexes(moves, Tutors_USUM, pi.SpecialTutors[0]);
        }

        private static void AddMovesTutor8(List<int> moves, int species, int form, PKM pkm, bool specialTutors)
        {
            if (pkm is PA8)
            {
                var pi = (PersonalInfoLA)PersonalTable.LA[species, form];
                if (!pi.IsPresentInGame)
                    return;
                var shop = MoveShop8_LA;
                var tutors = pi.SpecialTutors[0];
                for (int i = 0; i < shop.Length; i++)
                {
                    if (tutors[i])
                        moves.Add(shop[i]);
                }
                return;
            }
            if (pkm is PB8)
            {
                var pi = (PersonalInfoBDSP)PersonalTable.BDSP[species, form];
                if (pi.IsPresentInGame)
                    AddPermittedIndexes(moves, TypeTutor8b, pi.TypeTutors);
            }
            else // SWSH
            {
                var pi = (PersonalInfoSWSH)PersonalTable.SWSH[species, form];
                if (!pi.IsPresentInGame)
                    return;
                AddPermittedIndexes(moves, TypeTutor8, pi.TypeTutors);
                if (specialTutors)
                    AddPermittedIndexes(moves, Tutors_SWSH_1, pi.SpecialTutors[0]);
            }
        }

        private static void AddIfPermitted(List<int> moves, ReadOnlySpan<int> arrMoves, ReadOnlySpan<int> arrSpecies, int species)
        {
            var index = arrSpecies.IndexOf(species);
            if (index != -1)
                moves.Add(arrMoves[index]);
        }

        private static void AddPermittedIndexes(List<int> moves, int[][] tutors, bool[][] permit)
        {
            for (int i = 0; i < tutors.Length; i++)
            {
                var arr = tutors[i];
                var per = permit[i];
                AddPermittedIndexes(moves, arr, per);
            }
        }

        private static void AddPermittedIndexes(List<int> moves, int[] moveIDs, bool[] permit)
        {
            for (int i = 0; i < moveIDs.Length; i++)
            {
                if (permit[i])
                    moves.Add(moveIDs[i]);
            }
        }

        internal static void AddSpecialTutorMoves(List<int> r, PKM pkm, int Generation, int species)
        {
            switch (species)
            {
                case (int)Species.Keldeo:
                    r.Add((int)Move.SecretSword);
                    break;
                case (int)Species.Meloetta:
                    r.Add((int)Move.RelicSong);
                    break;

                case (int)Species.Pikachu when Generation == 7 && pkm.Form == 8:
                    r.AddRange(Tutor_StarterPikachu);
                    break;
                case (int)Species.Eevee when Generation == 7 && pkm.Form == 1:
                    r.AddRange(Tutor_StarterEevee);
                    break;

                case (int)Species.Pikachu or (int)Species.Raichu when Generation == 7 && !(pkm.LGPE || pkm.GO):
                    r.Add((int)Move.VoltTackle);
                    break;
            }
        }

        /// <summary> Rotom Moves that correspond to a specific form (form-0 ignored). </summary>
        private static readonly int[] RotomMoves = { (int)Move.Overheat, (int)Move.HydroPump, (int)Move.Blizzard, (int)Move.AirSlash, (int)Move.LeafStorm };

        internal static void AddSpecialFormChangeMoves(List<int> r, PKM pkm, int generation, int species)
        {
            switch (species)
            {
                case (int)Species.Rotom when generation >= 4:
                    var formMoves = RotomMoves;
                    var form = pkm.Form - 1;
                    if ((uint)form < formMoves.Length)
                        r.Add(RotomMoves[form]);
                    break;
                case (int)Species.Zygarde when generation == 7:
                    r.AddRange(ZygardeMoves);
                    break;
                case (int)Species.Necrozma when pkm.Form == 1: // Sun
                    r.Add((int)Move.SunsteelStrike);
                    break;
                case (int)Species.Necrozma when pkm.Form == 2: // Moon
                    r.Add((int)Move.MoongeistBeam);
                    break;
            }
        }
    }
}
