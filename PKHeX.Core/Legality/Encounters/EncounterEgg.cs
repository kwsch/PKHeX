using System;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Egg Encounter Data
    /// </summary>
    public class EncounterEgg : IEncounterable, IVersion
    {
        public int Species { get; set; }
        public string Name => "Egg";
        public bool EggEncounter => true;
        public int LevelMin => Level;
        public int LevelMax => Level;
        public int Level;

        public GameVersion Version { get; set; }

        public PKM ConvertToPKM(ITrainerInfo SAV)
        {
            int gen = Version.GetGeneration();
            if (gen < 2)
            {
                gen = 2;
                Version = GameVersion.C;
            }
            var pk = PKMConverter.GetBlank(gen);
            SAV.ApplyToPKM(pk);

            pk.Species = Species;
            pk.Nickname = PKX.GetSpeciesNameGeneration(Species, SAV.Language, gen);
            pk.CurrentLevel = Level;
            pk.Version = (int)Version;
            int[] moves = GetCurrentEggMoves(pk);
            pk.Moves = moves;
            pk.SetMaximumPPCurrent(moves);
            pk.OT_Friendship = pk.PersonalInfo.BaseFriendship;

            pk.SetRandomIVs(flawless: 3);

            if (pk.Format <= 2 && Version != GameVersion.C)
                return pk;

            pk.Met_Level = EncounterSuggestion.GetSuggestedEncounterEggMetLevel(pk);
            pk.Met_Location = Math.Max(0, EncounterSuggestion.GetSuggestedEggMetLocation(pk));

            if (pk.Format < 3)
                return pk;

            pk.Ball = 4;

            int gender = pk.PersonalInfo.RandomGender;
            int nature = Util.Rand.Next(25);

            if (pk.Format <= 5)
            {
                pk.SetPIDGender(gender);
                pk.Gender = gender;
                pk.SetPIDNature(nature);
                pk.Nature = nature;
                pk.RefreshAbility(pk.PIDAbility);
            }
            else
            {
                pk.PID = Util.Rand32();
                pk.Nature = nature;
                pk.Gender = gender;
                pk.RefreshAbility(Util.Rand.Next(2));
            }

            bool traded = (int)Version == SAV.Game;
            var today = pk.MetDate = DateTime.Today;
            if (pk.GenNumber >= 4)
            {
                pk.Egg_Location = EncounterSuggestion.GetSuggestedEncounterEggLocationEgg(pk, traded);
                pk.EggMetDate = today;
            }

            if (pk.Format < 6)
                return pk;
            if (pk.Gen6)
                pk.SetHatchMemory6();

            switch (Species)
            {
                case 774: // Minior
                    pk.AltForm = Util.Rand.Next(7, 14);
                    break;
                case 664: // Scatterbug
                    pk.AltForm = Legal.GetVivillonPattern(SAV.Country, SAV.SubRegion);
                    break;
            }

            pk.SetRandomEC();
            pk.RelearnMoves = moves;

            return pk;
        }

        private int[] GetCurrentEggMoves(PKM pk)
        {
            var moves = MoveEgg.GetEggMoves(pk, Species, pk.AltForm, Version);
            if (moves.Length == 0)
                return MoveLevelUp.GetEncounterMoves(pk, Level, Version);
            if (moves.Length >= 4 || pk.Format < 6)
                return moves;

            // Sprinkle in some default level up moves
            var lvl = Legal.GetBaseEggMoves(pk, Species, Version, Level);
            return lvl.Concat(moves).ToArray();
        }
    }

    public class EncounterEggSplit : EncounterEgg
    {
        public int OtherSpecies;
    }
}
