using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Egg Encounter Data
    /// </summary>
    public sealed record EncounterEgg : IEncounterable
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
        public bool IsShiny => false;

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

            int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)sav.Language, version);
            pk.Species = Species;
            pk.Form = Form;
            pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, gen);
            pk.CurrentLevel = Level;
            pk.Version = (int)version;
            pk.Ball = (int)Ball.Poke;
            pk.OT_Friendship = pk.PersonalInfo.BaseFriendship;

            SetEncounterMoves(pk, version);
            pk.HealPP();
            SetPINGA(pk, criteria);

            if (gen <= 2)
            {
                if (version != GameVersion.C)
                {
                    pk.OT_Gender = 0;
                }
                else
                {
                    pk.Met_Location = Locations.HatchLocationC;
                    pk.Met_Level = 1;
                }
                return pk;
            }

            SetMetData(pk);

            if (gen >= 4)
                pk.SetEggMetData(version, (GameVersion)sav.Game);

            if (gen < 6)
                return pk;
            if (pk is PK6 pk6)
                pk6.SetHatchMemory6();

            SetForm(pk, sav);

            pk.SetRandomEC();
            pk.RelearnMove1 = pk.Move1;
            pk.RelearnMove2 = pk.Move2;
            pk.RelearnMove3 = pk.Move3;
            pk.RelearnMove4 = pk.Move4;

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

        private void SetEncounterMoves(PKM pk, GameVersion version)
        {
            var learnset = GameData.GetLearnset(version, Species, Form);
            var baseMoves = learnset.GetBaseEggMoves(Level);
            if (baseMoves.Length == 0) return; pk.Move1 = baseMoves[0];
            if (baseMoves.Length == 1) return; pk.Move2 = baseMoves[1];
            if (baseMoves.Length == 2) return; pk.Move3 = baseMoves[2];
            if (baseMoves.Length == 3) return; pk.Move4 = baseMoves[3];
        }
    }
}
