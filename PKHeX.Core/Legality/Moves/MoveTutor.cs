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
            for (int i = 0; i < Tutors_GSC.Length; i++)
            {
                if (Tutors_GSC[i] == move)
                    return info.TMHM[57 + i] ? GameVersion.C : NONE;
            }
            return GetIsTutor1(pkm, species, move);
        }

        private static GameVersion GetIsTutor3(int species, int move)
        {
            // E Tutors (Free)
            // E Tutors (BP)
            var info = PersonalTable.E[species];
            for (int i = 0; i < Tutor_E.Length; i++)
            {
                if (Tutor_E[i] == move && info.TypeTutors[i])
                    return GameVersion.E;
            }

            // FRLG Tutors
            // Only special tutor moves, normal tutor moves are already included in Emerald data
            for (int i = 0; i < SpecialTutors_FRLG.Length; i++)
            {
                if (Tutor_FRLG[i] == move && species == SpecialTutors_Compatibility_FRLG[i])
                    return GameVersion.FRLG;
            }

            // XD
            for (int i = 0; i < SpecialTutors_XD_Exclusive.Length; i++)
            {
                if (SpecialTutors_XD_Exclusive[i] == move && SpecialTutors_Compatibility_XD_Exclusive[i].Any(e => e == species))
                    return GameVersion.XD;
            }

            // XD (Mew)
            if (species == (int)Species.Mew && Tutor_3Mew.Contains(move))
                return GameVersion.XD;

            return NONE;
        }

        private static GameVersion GetIsTutor4(int species, int form, int move)
        {
            var pi = PersonalTable.HGSS.GetFormeEntry(species, form);
            for (int i = 0; i < Tutors_4.Length; i++)
            {
                if (Tutors_4[i] == move && pi.TypeTutors[i])
                    return GameVersion.Gen4;
            }

            for (int i = 0; i < SpecialTutors_4.Length; i++)
            {
                if (SpecialTutors_4[i] == move && SpecialTutors_Compatibility_4[i].Any(e => e == species))
                    return GameVersion.HGSS;
            }

            return NONE;
        }

        private static GameVersion GetIsTutor5(PKM pkm, int species, int form, bool specialTutors, int move)
        {
            var pi = PersonalTable.B2W2.GetFormeEntry(species, form);
            var arr = TypeTutor6;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == move && pi.TypeTutors[i])
                    return GameVersion.Gen5;
            }

            if (specialTutors && pkm.HasVisitedB2W2())
            {
                var tutors = Tutors_B2W2;
                for (int i = 0; i < tutors.Length; i++)
                {
                    for (int j = 0; j < tutors[i].Length; j++)
                    {
                        if (tutors[i][j] == move && pi.SpecialTutors[i][j])
                            return GameVersion.B2W2;
                    }
                }
            }

            return NONE;
        }

        private static GameVersion GetIsTutor6(PKM pkm, int species, int form, bool specialTutors, int move)
        {
            var pi = PersonalTable.AO.GetFormeEntry(species, form);
            var arr = TypeTutor6;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == move && pi.TypeTutors[i])
                    return GameVersion.Gen6;
            }

            if (specialTutors && pkm.HasVisitedORAS())
            {
                var tutors = Tutors_AO;
                for (int i = 0; i < tutors.Length; i++)
                {
                    for (int j = 0; j < tutors[i].Length; j++)
                    {
                        if (tutors[i][j] == move && pi.SpecialTutors[i][j])
                            return GameVersion.ORAS;
                    }
                }
            }

            return NONE;
        }

        private static GameVersion GetIsTutor7(PKM pkm, int species, int form, bool specialTutors, int move)
        {
            var pi = PersonalTable.USUM.GetFormeEntry(species, form);
            var arr = TypeTutor6;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == move && pi.TypeTutors[i])
                    return GameVersion.Gen7;
            }

            if (specialTutors && pkm.HasVisitedUSUM())
            {
                var tutors = Tutors_USUM;
                for (int i = 0; i < tutors.Length; i++)
                {
                    if (tutors[i] == move && pi.SpecialTutors[0][i])
                        return GameVersion.USUM;
                }
            }

            return NONE;
        }

        private static GameVersion GetIsTutor8(PKM pkm, int species, int form, bool specialTutors, int move)
        {
            var pi = (PersonalInfoSWSH)PersonalTable.SWSH.GetFormeEntry(species, form);
            var arr = TypeTutor8;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == move && pi.TypeTutors[i])
                    return GameVersion.Gen8;
            }

            return NONE;
        }

        public static IEnumerable<int> GetTutorMoves(PKM pkm, int species, int form, bool specialTutors, int generation)
        {
            List<int> moves = new List<int>();
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
            var pi = PersonalTable.HGSS.GetFormeEntry(species, form);
            moves.AddRange(Tutors_4.Where((_, i) => pi.TypeTutors[i]));
            moves.AddRange(SpecialTutors_4.Where((_, i) => SpecialTutors_Compatibility_4[i].Any(e => e == species)));
        }

        private static void AddMovesTutor5(List<int> moves, int species, int form, PKM pkm, bool specialTutors)
        {
            var pi = PersonalTable.B2W2[species];
            moves.AddRange(TypeTutor6.Where((_, i) => pi.TypeTutors[i]));
            if (pkm.InhabitedGeneration(5) && specialTutors)
                moves.AddRange(GetTutors(PersonalTable.B2W2.GetFormeEntry(species, form), Tutors_B2W2));
        }

        private static void AddMovesTutor6(List<int> moves, int species, int form, PKM pkm, bool specialTutors)
        {
            var pi = PersonalTable.AO[species];
            moves.AddRange(TypeTutor6.Where((_, i) => pi.TypeTutors[i]));
            if (specialTutors && pkm.HasVisitedORAS())
                moves.AddRange(GetTutors(PersonalTable.AO.GetFormeEntry(species, form), Tutors_AO));
        }

        private static void AddMovesTutor7(List<int> moves, int species, int form, PKM pkm, bool specialTutors)
        {
            if (pkm.GG)
                return;
            var pi = PersonalTable.USUM.GetFormeEntry(species, form);
            moves.AddRange(TypeTutor6.Where((_, i) => pi.TypeTutors[i]));
            if (specialTutors && pkm.HasVisitedUSUM())
                moves.AddRange(GetTutors(pi, Tutors_USUM));
        }

        private static void AddMovesTutor8(List<int> moves, int species, int form, PKM pkm, bool specialTutors)
        {
            var pi = (PersonalInfoSWSH)PersonalTable.SWSH.GetFormeEntry(species, form);
            if (!pi.IsPresentInGame)
                return;
            moves.AddRange(TypeTutor8.Where((_, i) => pi.TypeTutors[i]));
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
                case (int)Species.Keldeo: // Keldeo
                    r.Add(548); // Secret Sword
                    break;
                case (int)Species.Meloetta:
                    r.Add(547); // Relic Song
                    break;
                case (int)Species.Pikachu when Generation == 6 && pkm.Format == 6:
                    int index = pkm.AltForm - 1;
                    if (index >= 0 && index < CosplayPikachuMoves.Length)
                        r.Add(CosplayPikachuMoves[index]);
                    break;

                case (int)Species.Pikachu when Generation == 7 && pkm.AltForm == 8:
                    r.AddRange(Tutor_StarterPikachu);
                    break;
                case (int)Species.Eevee when Generation == 7 && pkm.AltForm == 1:
                    r.AddRange(Tutor_StarterEevee);
                    break;

                case (int)Species.Pikachu when Generation == 7 && !(pkm is PB7):
                case (int)Species.Raichu  when Generation == 7 && !(pkm is PB7):
                    r.Add(344); // Volt Tackle
                    break;
            }
        }

        internal static void AddSpecialFormChangeMoves(List<int> r, PKM pkm, int Generation, int species)
        {
            switch (species)
            {
                case (int)Species.Rotom when Generation >= 4: // rotom
                    r.Add(RotomMoves[pkm.AltForm]);
                    break;
                case (int)Species.Zygarde when Generation == 7: // zygarde
                    r.AddRange(ZygardeMoves);
                    break;
                case (int)Species.Necrozma when pkm.AltForm == 1: // Sun Necrozma
                    r.Add(713);
                    break;
                case (int)Species.Necrozma when pkm.AltForm == 2: // Moon Necrozma
                    r.Add(714);
                    break;
            }
        }
    }
}
