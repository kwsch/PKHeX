using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for generating a large amount of <see cref="PKM"/> data.
    /// </summary>
    public static class BulkGenerator
    {
        public static IList<PKM> GetLivingDex(SaveFile sav)
        {
            var speciesToGenerate = Enumerable.Range(1, sav.MaxSpeciesID);
            return GetLivingDex(sav, speciesToGenerate, sav.BlankPKM);
        }

        public static List<PKM> GetLivingDex(ITrainerInfo tr, IEnumerable<int> speciesToGenerate, PKM blank)
        {
            var result = new List<PKM>();
            var destType = blank.GetType();
            foreach (var s in speciesToGenerate)
            {
                var pk = blank.Clone();
                pk.Species = s;
                pk.Gender = pk.GetSaneGender();

                var pi = pk.PersonalInfo;
                for (int i = 0; i < pi.FormeCount; i++)
                {
                    pk.AltForm = i;
                    if (s == (int) Species.Indeedee || s == (int) Species.Meowstic)
                        pk.Gender = i;

                    var f = EncounterMovesetGenerator.GeneratePKMs(pk, tr).FirstOrDefault();
                    if (f == null)
                        continue;
                    var converted = PKMConverter.ConvertToType(f, destType, out _);
                    if (converted == null)
                        continue;

                    converted.CurrentLevel = 100;
                    converted.Species = s;
                    converted.AltForm = i;

                    result.Add(converted);
                }
            }

            return result;
        }
    }
}
