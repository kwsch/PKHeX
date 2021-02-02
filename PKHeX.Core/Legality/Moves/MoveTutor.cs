using System;
using System.Collections.Generic;
using System.Linq;

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
                _ => NONE
            };
        }

        private static GameVersion GetIsTutor1(PKM pkm, int species, int move)
        {
            // Surf Pikachu via Stadium
            if (move != 57 || ParseSettings.AllowGBCartEra)
                return NONE;
            if (pkm.Format < 3 && (species == (int)Species.Pikachu || species == (int)Species.Raichu))
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
            if (xd != -1 && SpecialTutors_Compatibility_XD_Exclusive[xd].Contains(species))
                return GameVersion.XD;

            // XD (Mew)
            if (species == (int)Species.Mew && Tutor_3Mew.Contains(move))
                return GameVersion.XD;

            return NONE;
        }

        private static GameVersion GetIsTutor4(int species, int form, int move)
        {
            var pi = PersonalTable.HGSS.GetFormEntry(species, form);
            var type = Array.IndexOf(Tutors_4, move);
            if (type != -1 && pi.TypeTutors[type])
                return GameVersion.Gen4;

            var special = Array.IndexOf(SpecialTutors_4, move);
            if (special != -1 && SpecialTutors_Compatibility_4[special].Contains(species))
                return GameVersion.HGSS;

            return NONE;
        }

        private static GameVersion GetIsTutor5(PKM pkm, int species, int form, bool specialTutors, int move)
        {
            var pi = PersonalTable.B2W2.GetFormEntry(species, form);
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
            var pi = PersonalTable.AO.GetFormEntry(species, form);
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
            var pi = PersonalTable.USUM.GetFormEntry(species, form);
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
            var pi = (PersonalInfoSWSH)PersonalTable.SWSH.GetFormEntry(species, form);
            var type = Array.IndexOf(TypeTutor8, move);
            if (type != -1 && pi.TypeTutors[type])
                return GameVersion.Gen8;

            if (!specialTutors)
                return NONE;

            var tutor = Array.IndexOf(Tutors_SWSH_1, move);
            if (tutor != -1 && pi.SpecialTutors[0][tutor])
                return GameVersion.USUM;

            return NONE;
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
            return moves.Distinct();
        }

        private static void AddMovesTutor1(List<int> moves, int species, int format)
        {
            if (ParseSettings.AllowGBCartEra && format < 3 && (species == (int)Species.Pikachu || species == (int)Species.Raichu)) // Surf Pikachu via Stadium
                moves.Add(57);
        }

        private static void AddMovesTutor2(List<int> moves, int species, int format, bool korean = false)
        {
            if (korean)
                return;
            var pi = PersonalTable.C[species];
            moves.AddRange(Tutors_GSC.Where((_, i) => pi.TMHM[57 + i]));
            AddMovesTutor1(moves, species, format);
        }

        private static void AddMovesTutor3(List<int> moves, int species)
        {
            // E Tutors (Free)
            // E Tutors (BP)
            var pi = PersonalTable.E[species];
            moves.AddRange(Tutor_E.Where((_, i) => pi.TypeTutors[i]));
            // FRLG Tutors
            // Only special tutor moves, normal tutor moves are already included in Emerald data
            moves.AddRange(SpecialTutors_FRLG.Where((_, i) => SpecialTutors_Compatibility_FRLG[i] == species));
            // XD
            moves.AddRange(SpecialTutors_XD_Exclusive.Where((_, i) => SpecialTutors_Compatibility_XD_Exclusive[i].Any(e => e == species)));
            // XD (Mew)
            if (species == (int)Species.Mew)
                moves.AddRange(Tutor_3Mew);
        }

        private static void AddMovesTutor4(List<int> moves, int species, int form)
        {
            var pi = PersonalTable.HGSS.GetFormEntry(species, form);
            moves.AddRange(Tutors_4.Where((_, i) => pi.TypeTutors[i]));
            moves.AddRange(SpecialTutors_4.Where((_, i) => SpecialTutors_Compatibility_4[i].Any(e => e == species)));
        }

        private static void AddMovesTutor5(List<int> moves, int species, int form, PKM pkm, bool specialTutors)
        {
            var pi = PersonalTable.B2W2[species];
            moves.AddRange(TypeTutor6.Where((_, i) => pi.TypeTutors[i]));
            if (pkm.InhabitedGeneration(5) && specialTutors)
                moves.AddRange(GetTutors(PersonalTable.B2W2.GetFormEntry(species, form), Tutors_B2W2));
        }

        private static void AddMovesTutor6(List<int> moves, int species, int form, PKM pkm, bool specialTutors)
        {
            var pi = PersonalTable.AO[species];
            moves.AddRange(TypeTutor6.Where((_, i) => pi.TypeTutors[i]));
            if (specialTutors && pkm.HasVisitedORAS(species))
                moves.AddRange(GetTutors(PersonalTable.AO.GetFormEntry(species, form), Tutors_AO));
        }

        private static void AddMovesTutor7(List<int> moves, int species, int form, PKM pkm, bool specialTutors)
        {
            if (pkm.Version is (int)GameVersion.GO or (int)GameVersion.GP or (int)GameVersion.GE)
                return;
            var pi = PersonalTable.USUM.GetFormEntry(species, form);
            moves.AddRange(TypeTutor6.Where((_, i) => pi.TypeTutors[i]));
            if (specialTutors && pkm.HasVisitedUSUM(species))
                moves.AddRange(GetTutors(pi, Tutors_USUM));
        }

        private static void AddMovesTutor8(List<int> moves, int species, int form, PKM pkm, bool specialTutors)
        {
            var pi = (PersonalInfoSWSH)PersonalTable.SWSH.GetFormEntry(species, form);
            if (!pi.IsPresentInGame)
                return;
            moves.AddRange(TypeTutor8.Where((_, i) => pi.TypeTutors[i]));
            if (specialTutors)
                moves.AddRange(GetTutors(pi, Tutors_SWSH_1));
        }

        private static IEnumerable<int> GetTutors(PersonalInfo pi, params int[][] tutors)
        {
            for (int i = 0; i < tutors.Length; i++)
            {
                for (int b = 0; b < tutors[i].Length; b++)
                {
                    if (pi.SpecialTutors[i][b])
                        yield return tutors[i][b];
                }
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

                case (int)Species.Pikachu or (int)Species.Raichu when Generation == 7 && !pkm.GG:
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
                    r.Add(713);
                    break;
                case (int)Species.Necrozma when pkm.Form == 2: // Moon
                    r.Add(714);
                    break;
            }
        }
    }
}
