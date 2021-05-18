using System;
using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    /// <summary>
    /// Wild Encounter Slot data
    /// </summary>
    /// <remarks>Wild encounter slots are found as random encounters in-game.</remarks>
    public abstract record EncounterSlot : IEncounterable, ILocation, IEncounterMatch
    {
        public int Species { get; }
        public int Form { get; }
        public int LevelMin { get; }
        public int LevelMax { get; }
        public abstract int Generation { get; }
        public bool EggEncounter => false;
        public virtual bool IsShiny => false;

        protected EncounterSlot(EncounterArea area, int species, int form, int min, int max)
        {
            Area = area;
            Species = species;
            Form = form;
            LevelMin = min;
            LevelMax = max;
        }

        internal readonly EncounterArea Area;
        public GameVersion Version => Area.Version;
        public int Location => Area.Location;
        public int EggLocation => 0;

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
            int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID) sav.Language, version);
            int level = LevelMin;
            pk.Species = Species;
            pk.Language = lang;
            pk.CurrentLevel = level;
            pk.Version = (int)version;
            pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation);

            var ball = Area.Type.GetRequiredBallValueWild(Generation, Location);
            pk.Ball = (int)(ball == Ball.None ? Ball.Poke : ball);
            pk.Language = lang;
            pk.Form = GetWildForm(pk, Form, sav);
            pk.OT_Friendship = pk.PersonalInfo.BaseFriendship;

            SetMetData(pk, level, Location);
            SetPINGA(pk, criteria);
            SetEncounterMoves(pk, version, level);

            SetFormatSpecificData(pk);

            if (pk.Format < 6)
                return;

            sav.ApplyHandlingTrainerInfo(pk);
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
            var pi = pk.PersonalInfo;
            int gender = criteria.GetGender(-1, pi);
            int nature = (int)criteria.GetNature(Nature.Random);
            var ability = criteria.GetAbilityFromNumber(Ability);

            if (Generation == 3 && Species == (int)Unown)
            {
                do
                {
                    PIDGenerator.SetRandomWildPID(pk, pk.Format, nature, ability, gender);
                    ability ^= 1; // some nature-forms cannot have a certain PID-ability set, so just flip it as Unown doesn't have dual abilities.
                } while (pk.Form != Form);
            }
            else
            {
                PIDGenerator.SetRandomWildPID(pk, pk.Format, nature, ability, gender);
            }

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

        private const int FormDynamic = FormVivillon;
        private const int FormVivillon = 30;
        private const int FormRandom = 31;

        private static int GetWildForm(PKM pk, int form, ITrainerInfo sav)
        {
            if (form < FormDynamic) // specified form
            {
                if (pk.Species == (int)Minior)
                    return Util.Rand.Next(7, 14);
                return form;
            }
            if (form == FormRandom) // flagged as totally random
                return Util.Rand.Next(pk.PersonalInfo.FormCount);

            int species = pk.Species;
            if (species is >= (int)Scatterbug and <= (int)Vivillon)
            {
                if (sav is IRegionOrigin o)
                    return Vivillon3DS.GetPattern((byte)o.Country, (byte)o.Region);
            }
            return 0;
        }

        public virtual string GetConditionString(out bool valid)
        {
            valid = true;
            return LegalityCheckStrings.LEncCondition;
        }

        public bool IsMatchExact(PKM pkm, DexLevel dl) => true; // Matched by Area

        public virtual EncounterMatchRating GetMatchRating(PKM pkm)
        {
            if (IsDeferredWurmple(pkm))
                return EncounterMatchRating.PartialMatch;

            if (pkm.Format >= 5)
            {
                bool isHidden = pkm.AbilityNumber == 4;
                if (isHidden && this.IsPartialMatchHidden(pkm.Species, Species))
                    return EncounterMatchRating.PartialMatch;
                if (IsDeferredHiddenAbility(isHidden))
                    return EncounterMatchRating.Deferred;
            }

            return EncounterMatchRating.Match;
        }

        protected virtual HiddenAbilityPermission IsHiddenAbilitySlot() => HiddenAbilityPermission.Never;

        public int Ability => IsHiddenAbilitySlot() switch
        {
            HiddenAbilityPermission.Never => 0,
            HiddenAbilityPermission.Always => 4,
            _ => -1,
        };

        private bool IsDeferredWurmple(PKM pkm) => Species == (int)Wurmple && pkm.Species != (int)Wurmple && !WurmpleUtil.IsWurmpleEvoValid(pkm);

        private bool IsDeferredHiddenAbility(bool IsHidden) => IsHiddenAbilitySlot() switch
        {
            HiddenAbilityPermission.Never => IsHidden,
            HiddenAbilityPermission.Always => !IsHidden,
            _ => false,
        };

        protected enum HiddenAbilityPermission
        {
            Always,
            Never,
            Possible,
        }
    }
}
