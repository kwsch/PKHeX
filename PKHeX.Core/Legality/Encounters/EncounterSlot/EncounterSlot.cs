using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Wild Encounter Slot data
    /// </summary>
    public abstract class EncounterSlot : IEncounterable, ILocation
    {
        public int Species { get; protected set; }
        public int Form { get; protected set; }
        public int LevelMin { get; protected set; }
        public int LevelMax { get; protected set; }
        public abstract int Generation { get; }
        public bool EggEncounter => false;
        public override string ToString() => $"{(Species) Species} @ {LevelMin}-{LevelMax}";

        internal readonly EncounterArea Area;
        public GameVersion Version => Area.Version;
        public int Location => Area.Location;
        public int EggLocation => 0;

        protected EncounterSlot(EncounterArea area) => Area = area;

        public EncounterSlot Clone() => (EncounterSlot)MemberwiseClone();

        public bool FixedLevel => LevelMin == LevelMax;

        private protected const string wild = "Wild Encounter";
        public string Name => wild;

        /// <summary>
        /// Gets if the specified level inputs are within range of the <see cref="LevelMin"/> and <see cref="LevelMax"/>
        /// </summary>
        /// <param name="lvl">Single level</param>
        /// <returns>True if within slot's range, false if impossible.</returns>
        public bool IsLevelWithinRange(int lvl) => LevelMin <= lvl && lvl <= LevelMax;

        /// <summary>
        /// Gets if the specified level inputs are within range of the <see cref="LevelMin"/> and <see cref="LevelMax"/>
        /// </summary>
        /// <param name="min">Highest value the low end of levels can be</param>
        /// <param name="max">Lowest value the high end of levels can be</param>
        /// <returns>True if within slot's range, false if impossible.</returns>
        public bool IsLevelWithinRange(int min, int max) => LevelMin <= max && min <= LevelMax;

        /// <summary>
        /// Gets if the specified level inputs are within range of the <see cref="LevelMin"/> and <see cref="LevelMax"/>
        /// </summary>
        /// <param name="lvl">Single level</param>
        /// <param name="minDecrease">Highest value the low end of levels can be</param>
        /// <param name="maxIncrease">Lowest value the high end of levels can be</param>
        /// <returns>True if within slot's range, false if impossible.</returns>
        public bool IsLevelWithinRange(int lvl, int minDecrease, int maxIncrease) => LevelMin - minDecrease <= lvl && lvl <= LevelMax + maxIncrease;

        /// <summary>
        /// Gets if the specified level inputs are within range of the <see cref="LevelMin"/> and <see cref="LevelMax"/>
        /// </summary>
        /// <param name="min">Lowest level allowed</param>
        /// <param name="max">Highest level allowed</param>
        /// <param name="minDecrease">Highest value the low end of levels can be</param>
        /// <param name="maxIncrease">Lowest value the high end of levels can be</param>
        /// <returns>True if within slot's range, false if impossible.</returns>
        public bool IsLevelWithinRange(int min, int max, int minDecrease, int maxIncrease) => LevelMin - minDecrease <= max && min <= LevelMax + maxIncrease;

        public virtual string LongName
        {
            get
            {
                if (Area!.Type == SlotType.Any)
                    return wild;
                return $"{wild} {Area!.Type.ToString().Replace('_', ' ')}";
            }
        }

        public PKM ConvertToPKM(ITrainerInfo sav) => ConvertToPKM(sav, EncounterCriteria.Unrestricted);

        public PKM ConvertToPKM(ITrainerInfo sav, EncounterCriteria criteria)
        {
            var pk = PKMConverter.GetBlank(Generation, Version);
            sav.ApplyTo(pk);
            ApplyDetails(sav, criteria, pk);
            return pk;
        }

        protected virtual void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            var version = this.GetCompatibleVersion((GameVersion) sav.Game);
            int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID) sav.Language);
            int level = LevelMin;
            pk.Species = Species;
            pk.Language = lang;
            pk.CurrentLevel = level;
            pk.Version = (int)version;
            pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation);

            var ball = BallExtensions.GetRequiredBallValueWild(Generation, Location);
            pk.Ball = (int)(ball == Ball.None ? Ball.Poke : ball);
            pk.Language = lang;
            pk.OT_Friendship = pk.PersonalInfo.BaseFriendship;
            pk.AltForm = GetWildAltForm(pk, Form, sav);

            SetMetData(pk, level, Location);
            SetPINGA(pk, criteria);
            SetEncounterMoves(pk, version, level);

            SetFormatSpecificData(pk);

            if (pk.Format < 6)
                return;

            sav.ApplyHandlingTrainerInfo(pk);
            pk.SetRandomEC();
        }

        protected virtual void SetEncounterMoves(PKM pk, GameVersion version, int level)
        {
            var moves = MoveLevelUp.GetEncounterMoves(pk, level, version);
            pk.SetMoves(moves);
            pk.SetMaximumPPCurrent(moves);
        }

        protected virtual void SetFormatSpecificData(PKM pk) { }

        protected virtual void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            int gender = criteria.GetGender(-1, pk.PersonalInfo);
            int nature = (int)criteria.GetNature(Nature.Random);

            var ability = Util.Rand.Next(2);
            if (Area!.Type == SlotType.HiddenGrotto) // don't force hidden for DexNav
                ability = 2;

            PIDGenerator.SetRandomWildPID(pk, pk.Format, nature, ability, gender);
            pk.Gender = gender;
            pk.StatNature = nature;
        }

        private void SetMetData(PKM pk, int level, int location)
        {
            if (pk.Format <= 2 && Version != GameVersion.C)
                return;

            pk.Met_Location = location;
            pk.Met_Level = level;

            if (pk.Format >= 4)
                pk.MetDate = DateTime.Today;
        }

        private static int GetWildAltForm(PKM pk, int form, ITrainerInfo sav)
        {
            if (form < 30) // specified form
            {
                if (pk.Species == (int) Core.Species.Minior)
                    return Util.Rand.Next(7, 14);
                return form;
            }
            if (form == 31) // flagged as totally random
                return Util.Rand.Next(pk.PersonalInfo.FormeCount);

            int spec = pk.Species;
            if ((int) Core.Species.Scatterbug <= spec && spec <= (int) Core.Species.Vivillon)
            {
                if (sav is IRegionOrigin o)
                    return Legal.GetVivillonPattern((byte)o.Country, (byte)o.Region);
            }
            return 0;
        }

        public virtual string GetConditionString(out bool valid)
        {
            valid = true;
            return LegalityCheckStrings.LEncCondition;
        }
    }
}
