using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for filling in template data for <see cref="PKM"/> objects.
    /// </summary>
    public static class EntityTemplates
    {
        /// <summary>
        /// Applies junk data to a <see cref="SaveFile.BlankPKM"/>, which is preferable to a completely empty entity.
        /// </summary>
        /// <param name="pk">Blank data</param>
        /// <param name="tr">Trainer info to apply</param>
        public static void TemplateFields(PKM pk, ITrainerInfo tr)
        {
            pk.Move1 = (int)Move.Pound;
            pk.HealPP();
            pk.Ball = 4;
            pk.MetDate = DateTime.Today;

            if (tr.Game >= 0)
                pk.Version = tr.Game;

            pk.Species = GetTemplateSpecies(pk, tr);
            pk.Language = GetTemplateLanguage(tr);
            pk.Gender = pk.GetSaneGender();

            pk.ClearNickname();

            pk.OT_Name = tr.OT;
            pk.OT_Gender = tr.Gender;
            pk.TID = tr.TID;
            pk.SID = tr.SID;
            if (tr is IRegionOrigin o && pk is IRegionOrigin gt)
            {
                gt.ConsoleRegion = o.ConsoleRegion;
                gt.Country = o.Country;
                gt.Region = o.Region;
            }

            ApplyTrashBytes(pk, tr);
            pk.RefreshChecksum();
        }

        private static void ApplyTrashBytes(PKM pk, ITrainerInfo tr)
        {
            // Copy OT trash bytes for sensitive games (Gen1/2)
            if (pk is not GBPKM pk12)
                return;
            switch (tr)
            {
                case SAV1 s1:
                    pk12.OT_Trash = s1.OT_Trash;
                    break;
                case SAV2 s2:
                    pk12.OT_Trash = s2.OT_Trash;
                    break;
            }
        }

        private static int GetTemplateSpecies(PKM pk, ITrainerInfo tr)
        {
            int species = tr is IGameValueLimit s ? s.MaxSpeciesID : ((GameVersion)pk.Version).GetMaxSpeciesID();
            if (species <= 0)
                species = pk.MaxSpeciesID;
            return species;
        }

        private static int GetTemplateLanguage(ITrainerInfo tr)
        {
            var lang = tr.Language;
            if (lang <= 0)
                lang = (int)LanguageID.English;
            return lang;
        }
    }
}
