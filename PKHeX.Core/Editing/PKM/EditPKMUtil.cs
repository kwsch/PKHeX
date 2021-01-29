using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Utility for editing a <see cref="PKM"/>
    /// </summary>
    public static class EditPKMUtil
    {
        public static List<string> GetSuggestionMessage(PKM pkm, int level, int location, int minimumLevel)
        {
            var suggestion = new List<string> { MsgPKMSuggestionStart };
            if (pkm.Format >= 3)
            {
                var metList = GameInfo.GetLocationList((GameVersion)pkm.Version, pkm.Format, egg: false);
                var locationName = metList.First(loc => loc.Value == location).Text;
                suggestion.Add($"{MsgPKMSuggestionMetLocation} {locationName}");
                suggestion.Add($"{MsgPKMSuggestionMetLevel} {level}");
            }
            if (pkm.CurrentLevel < minimumLevel)
                suggestion.Add($"{MsgPKMSuggestionLevel} {minimumLevel}");
            return suggestion;
        }

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
            int species = tr is IGameValueLimit s ? s.MaxSpeciesID : ((GameVersion)pk.Version).GetMaxSpeciesID();
            if (species <= 0)
                species = pk.MaxSpeciesID;
            pk.Species = species;
            var lang = tr.Language;
            if (lang <= 0)
                lang = (int)LanguageID.English;
            pk.Language = lang;
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

            // Copy OT trash bytes for sensitive games (Gen1/2)
            if (pk is GBPKM pk12)
            {
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

            pk.RefreshChecksum();
        }
    }
}
