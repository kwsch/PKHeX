using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for generating a large amount of <see cref="PKM"/> data.
    /// </summary>
    public static class BulkGenerator
    {
        public static List<PKM> GetLivingDex(this SaveFile sav)
        {
            var speciesToGenerate = Enumerable.Range(1, sav.MaxSpeciesID);
            return GetLivingDex(sav, speciesToGenerate);
        }

        private static List<PKM> GetLivingDex(SaveFile sav, IEnumerable<int> speciesToGenerate)
        {
            return sav.GetLivingDex(speciesToGenerate, sav.BlankPKM);
        }

        public static List<PKM> GetLivingDex(this ITrainerInfo tr, IEnumerable<int> speciesToGenerate, PKM blank)
        {
            var result = new List<PKM>();
            var destType = blank.GetType();
            foreach (var s in speciesToGenerate)
            {
                var pk = blank.Clone();
                pk.Species = s;
                pk.Gender = pk.GetSaneGender();

                var pi = pk.PersonalInfo;
                for (int f = 0; f < pi.FormCount; f++)
                {
                    var entry = tr.GetLivingEntry(pk, s, f, destType);
                    if (entry == null)
                        continue;
                    result.Add(entry);
                }
            }

            return result;
        }

        public static PKM? GetLivingEntry(this ITrainerInfo tr, PKM template, int species, int form, Type destType)
        {
            template.Species = species;
            template.Form = form;
            template.Gender = template.GetSaneGender();

            var f = EncounterMovesetGenerator.GeneratePKMs(template, tr).FirstOrDefault();
            if (f == null)
                return null;

            var result = PKMConverter.ConvertToType(f, destType, out _);
            if (result == null)
                return null;

            result.CurrentLevel = 100;
            result.Species = species;
            result.Form = form;

            result.Heal();
            return result;
        }
    }
}
