using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Wild Encounter Slot data
    /// </summary>
    public class EncounterSlot : IEncounterable, IGenerationSet, ILocation, IVersionSet
    {
        public int Species { get; set; }
        public int Form { get; set; }
        public int LevelMin { get; set; }
        public int LevelMax { get; set; }
        public GameVersion Version { get; set; }
        public int Generation { get; set; } = -1;
        internal EncounterArea? Area { private get; set; }
        public int Location { get => Area?.Location ?? 0; set { } }
        public bool EggEncounter => false;
        public int EggLocation { get => 0; set { } }

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
        public bool IsLevelWithinRange(int min, int max) => LevelMin <= min && max <= LevelMax;

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
        public bool IsLevelWithinRange(int min, int max, int minDecrease, int maxIncrease) => LevelMin - minDecrease <= min && max <= LevelMax + maxIncrease;

        public SlotType Type { get; set; } = SlotType.Any;
        public EncounterType TypeEncounter { get; set; } = EncounterType.None;
        public int SlotNumber { get; set; }
        private EncounterSlotPermissions? _perm;
        public EncounterSlotPermissions Permissions => _perm ??= new EncounterSlotPermissions();

        public EncounterSlot Clone()
        {
            var slot = (EncounterSlot) MemberwiseClone();
            if (_perm != null)
                slot._perm = Permissions.Clone();
            return slot;
        }

        public bool FixedLevel => LevelMin == LevelMax;

        public bool IsMatchStatic(int index, int count) => index == Permissions.StaticIndex && count == Permissions.StaticCount;
        public bool IsMatchMagnet(int index, int count) => index == Permissions.MagnetPullIndex && count == Permissions.MagnetPullCount;

        private protected const string wild = "Wild Encounter";
        public string Name => wild;

        public virtual string LongName
        {
            get
            {
                if (Type == SlotType.Any)
                    return wild;
                return $"{wild} {Type.ToString().Replace('_', ' ')}";
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
            pk.Ball = (int)Type.GetBall();
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

        private void SetEncounterMoves(PKM pk, GameVersion version, int level)
        {
            var moves = this is IMoveset m ? m.Moves : MoveLevelUp.GetEncounterMoves(pk, level, version);
            pk.SetMoves(moves);
            pk.SetMaximumPPCurrent(moves);
        }

        private void SetFormatSpecificData(PKM pk)
        {
            if (pk is XK3 xk3)
            {
                xk3.FatefulEncounter = true; // PokeSpot
            }
            else if (pk is PK4 pk4)
            {
                pk4.EncounterType = TypeEncounter.GetIndex();
            }
            else if (pk is PK6 pk6)
            {
                if (Permissions.IsDexNav)
                {
                    var eggMoves = MoveEgg.GetEggMoves(pk, Species, Form, Version);
                    if (eggMoves.Length > 0)
                        pk6.RelearnMove1 = eggMoves[Util.Rand.Next(eggMoves.Length)];
                }
                pk6.SetRandomMemory6();
            }
        }

        private void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            int gender = criteria.GetGender(-1, pk.PersonalInfo);
            int nature = (int)criteria.GetNature(Nature.Random);

            var ability = Util.Rand.Next(2);
            if (Type == SlotType.HiddenGrotto) // don't force hidden for DexNav
                ability = 2;

            var pidtype = GetPIDType();
            if (pidtype == PIDType.PokeSpot)
                PIDGenerator.SetRandomPokeSpotPID(pk, nature, gender, ability, SlotNumber);
            else
                PIDGenerator.SetRandomWildPID(pk, pk.Format, nature, ability, gender, pidtype);
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

        private PIDType GetPIDType()
        {
            if (Version == GameVersion.XD)
                return PIDType.PokeSpot;
            return PIDType.None; // depends on format, let the program auto-detect.
        }
    }
}
