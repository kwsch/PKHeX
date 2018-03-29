using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Egg Encounter Data
    /// </summary>
    public class EncounterEgg : IEncounterable
    {
        public int Species { get; set; }
        public string Name => "Egg";
        public bool EggEncounter => true;
        public int LevelMin => Level;
        public int LevelMax => Level;
        public int Level;

        public GameVersion Game;
        public bool SplitBreed;

        public PKM ConvertToPKM(ITrainerInfo SAV)
        {
            var pk = PKMConverter.GetBlank(SAV.Generation);
            SAV.ApplyToPKM(pk);

            pk.Species = Species;
            pk.Nickname = PKX.GetSpeciesNameGeneration(Species, pk.Language, SAV.Generation);
            pk.CurrentLevel = Level;
            pk.Version = (int)Game;

            var moves = Legal.GetEggMoves(pk, Species, pk.AltForm, Game);
            pk.Moves = moves;
            pk.SetMaximumPPCurrent(moves);
            pk.OT_Friendship = pk.PersonalInfo.BaseFriendship;

            pk.SetRandomIVs(flawless: 3);

            if (pk.Format <= 2 && Game != GameVersion.C)
                return pk;

            pk.PID = Util.Rand32();
            pk.RefreshAbility(Util.Rand.Next(2));
            pk.Ball = 4;

            pk.Met_Level = EncounterSuggestion.GetSuggestedEncounterEggMetLevel(pk);
            pk.Met_Location = EncounterSuggestion.GetSuggestedEggMetLocation(pk);

            if (pk.Format < 4)
                return pk;

            bool traded = (int)Game == SAV.Game;
            pk.Egg_Location = EncounterSuggestion.GetSuggestedEncounterEggLocationEgg(pk, traded);
            pk.EggMetDate = pk.MetDate = DateTime.Today;

            if (pk.Format < 6)
                return pk;

            if (pk.Format != SAV.Generation)
            {
                pk.HT_Name = SAV.OT;
                pk.HT_Gender = SAV.Gender;
                pk.HT_Friendship = pk.OT_Friendship;
                if (SAV.Generation == 6)
                {
                    pk.Geo1_Country = SAV.Country;
                    pk.Geo1_Region = SAV.SubRegion;
                }
            }
            if (SAV.Generation == 6)
                pk.SetHatchMemory6();

            pk.SetRandomEC();
            pk.RelearnMoves = moves;

            return pk;
        }
    }
}
