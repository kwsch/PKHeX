using System;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Egg Encounter Data
    /// </summary>
    public record EncounterEgg : IEncounterable
    {
        public int Species { get; }
        public int Form { get; }
        public string Name => "Egg";
        public string LongName => "Egg";

        public bool EggEncounter => true;
        public int LevelMin => Level;
        public int LevelMax => Level;
        public readonly int Level;
        public int Generation { get; }
        public GameVersion Version { get; }

        public EncounterEgg(int species, int form, int level, int gen, GameVersion game)
        {
            Species = species;
            Form = form;
            Level = level;
            Generation = gen;
            Version = game;
        }

        public PKM ConvertToPKM(ITrainerInfo sav) => ConvertToPKM(sav, EncounterCriteria.Unrestricted);

        public PKM ConvertToPKM(ITrainerInfo sav, EncounterCriteria criteria)
        {
            int gen = Generation;
            var version = Version;
            var pk = PKMConverter.GetBlank(gen, version);

            sav.ApplyTo(pk);

            pk.Species = Species;
            pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, sav.Language, gen);
            pk.CurrentLevel = Level;
            pk.Version = (int)version;
            pk.Ball = (int)Ball.Poke;
            pk.OT_Friendship = pk.PersonalInfo.BaseFriendship;

            int[] moves = SetEncounterMoves(pk, version);
            SetPINGA(pk, criteria);

            if (gen <= 2 && version != GameVersion.C)
                return pk;

            SetMetData(pk);

            if (gen < 3)
                return pk;

            if (gen >= 4)
                pk.SetEggMetData(version, (GameVersion)sav.Game);

            if (gen < 6)
                return pk;
            if (pk is PK6 pk6)
                pk6.SetHatchMemory6();

            SetForm(pk, sav);

            pk.SetRandomEC();
            pk.RelearnMoves = moves;

            return pk;
        }

        private void SetForm(PKM pk, ITrainerInfo sav)
        {
            switch (Species)
            {
                case (int)Core.Species.Minior:
                    pk.Form = Util.Rand.Next(7, 14);
                    break;
                case (int)Core.Species.Scatterbug or (int)Core.Species.Spewpa or (int)Core.Species.Vivillon:
                    if (sav is IRegionOrigin o)
                        pk.Form = Vivillon3DS.GetPattern((byte)o.Country, (byte)o.Region);
                    // else 0
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
            pk.StatNature = nature;
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
            var moves = MoveEgg.GetEggMoves(pk.PersonalInfo, Species, Form, version, Generation);
            if (moves.Length == 0)
                return MoveLevelUp.GetEncounterMoves(pk, Level, version);
            if (moves.Length >= 4 || pk.Format < 6)
                return moves;

            // Sprinkle in some default level up moves
            var lvl = MoveList.GetBaseEggMoves(pk, Species, Form, version, Level);
            return lvl.Concat(moves).ToArray();
        }
    }

    public sealed record EncounterEggSplit : EncounterEgg
    {
        public int OtherSpecies { get; }
        public EncounterEggSplit(int species, int form, int level, int gen, GameVersion game, int otherSpecies) : base(species, form, level, gen, game) => OtherSpecies = otherSpecies;
    }
}
