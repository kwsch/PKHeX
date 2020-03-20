using System;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Egg Encounter Data
    /// </summary>
    public class EncounterEgg : IEncounterable, IVersion
    {
        public int Species { get; }
        public int Form { get; }
        public string Name => "Egg";
        public string LongName => "Egg";
        public bool EggEncounter => true;
        public int LevelMin => Level;
        public int LevelMax => Level;
        public readonly int Level;

        public EncounterEgg(int species, int form, int level)
        {
            Species = species;
            Form = form;
            Level = level;
        }

        public GameVersion Version { get; set; }

        public PKM ConvertToPKM(ITrainerInfo SAV) => ConvertToPKM(SAV, EncounterCriteria.Unrestricted);

        public PKM ConvertToPKM(ITrainerInfo SAV, EncounterCriteria criteria)
        {
            int gen = Version.GetGeneration();
            var version = Version;
            if (gen < 2)
            {
                gen = 2;
                version = GameVersion.C;
            }
            var pk = PKMConverter.GetBlank(gen, version);

            SAV.ApplyToPKM(pk);

            pk.Species = Species;
            pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, SAV.Language, gen);
            pk.CurrentLevel = Level;
            pk.Version = (int)version;
            pk.Ball = 4;
            pk.OT_Friendship = pk.PersonalInfo.BaseFriendship;

            int[] moves = SetEncounterMoves(pk, version);
            SetPINGA(pk, criteria);

            if (pk.Format <= 2 && version != GameVersion.C)
                return pk;

            SetMetData(pk);

            if (pk.Format < 3)
                return pk;

            if (pk.GenNumber >= 4)
                pk.SetEggMetData(version, (GameVersion)SAV.Game);

            if (pk.Format < 6)
                return pk;
            if (pk.Format == 6)
                pk.SetHatchMemory6();

            SetAltForm(pk, SAV);

            pk.SetRandomEC();
            pk.RelearnMoves = moves;

            return pk;
        }

        private void SetAltForm(PKM pk, ITrainerInfo SAV)
        {
            switch (Species)
            {
                case (int)Core.Species.Minior:
                    pk.AltForm = Util.Rand.Next(7, 14);
                    break;
                case (int)Core.Species.Scatterbug:
                case (int)Core.Species.Spewpa:
                case (int)Core.Species.Vivillon:
                    pk.AltForm = Legal.GetVivillonPattern((byte)SAV.Country, (byte)SAV.SubRegion);
                    break;
            }
        }

        private static void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            pk.SetRandomIVs(flawless: 3);
            if (pk.Format <= 2)
                return;

            int gender = criteria.GetGender(-1, pk.PersonalInfo);
            int nature = (int)criteria.GetNature(Nature.Random);

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
        }

        private static void SetMetData(PKM pk)
        {
            pk.Met_Level = EncounterSuggestion.GetSuggestedEncounterEggMetLevel(pk);
            pk.Met_Location = Math.Max(0, EncounterSuggestion.GetSuggestedEggMetLocation(pk));
        }

        private int[] SetEncounterMoves(PKM pk, GameVersion version)
        {
            int[] moves = GetCurrentEggMoves(pk, version);
            pk.Moves = moves;
            pk.SetMaximumPPCurrent(moves);
            return moves;
        }

        private int[] GetCurrentEggMoves(PKM pk, GameVersion version)
        {
            var moves = MoveEgg.GetEggMoves(pk, Species, Form, version);
            if (moves.Length == 0)
                return MoveLevelUp.GetEncounterMoves(pk, Level, version);
            if (moves.Length >= 4 || pk.Format < 6)
                return moves;

            // Sprinkle in some default level up moves
            var lvl = Legal.GetBaseEggMoves(pk, Species, Form, version, Level);
            return lvl.Concat(moves).ToArray();
        }
    }

    public sealed class EncounterEggSplit : EncounterEgg
    {
        public int OtherSpecies { get; }
        public EncounterEggSplit(int species, int form, int level, int otherSpecies) : base(species, form, level) => OtherSpecies = otherSpecies;
    }
}
