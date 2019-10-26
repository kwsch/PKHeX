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
            var bd = sav.BoxData;
            var tr = sav;
            for (int i = 1; i <= sav.MaxSpeciesID; i++) // should really get a list of valid species IDs
            {
                var pk = sav.BlankPKM;
                pk.Species = i;
                pk.Gender = pk.GetSaneGender();
                if (i == (int)Species.Meowstic)
                    pk.AltForm = pk.Gender;

                var f = EncounterMovesetGenerator.GeneratePKMs(pk, tr).FirstOrDefault();
                if (f == null)
                    continue;

                var converted = PKMConverter.ConvertToType(f, sav.PKMType, out _);
                if (converted != null)
                    bd[i] = converted;
            }
            return bd;
        }
    }
}
