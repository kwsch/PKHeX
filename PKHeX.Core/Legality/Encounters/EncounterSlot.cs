using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Wild Encounter Slot data
    /// </summary>
    public class EncounterSlot : IEncounterable, IGeneration, ILocation, IVersion
    {
        public int Species { get; set; }
        public int Form { get; set; }
        public int LevelMin { get; set; }
        public int LevelMax { get; set; }
        public SlotType Type { get; set; } = SlotType.Any;
        public EncounterType TypeEncounter { get; set; } = EncounterType.None;
        public int SlotNumber { get; set; }
        public int Generation { get; set; } = -1;
        internal EncounterSlotPermissions _perm;
        public EncounterSlotPermissions Permissions => _perm ?? (_perm = new EncounterSlotPermissions());
        public GameVersion Version { get; set; }

        internal EncounterArea Area { private get; set; }
        public int Location { get => Area.Location; set { } }
        public bool EggEncounter => false;
        public int EggLocation { get => 0; set { } }

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

        private const string wild = "Wild Encounter";
        public string Name => wild;

        public string LongName
        {
            get
            {
                if (Type == SlotType.Any)
                    return wild;
                return $"{wild} {Type.ToString().Replace('_', ' ')}";
            }
        }

        public PKM ConvertToPKM(ITrainerInfo SAV) => ConvertToPKM(SAV, EncounterCriteria.Unrestricted);

        public PKM ConvertToPKM(ITrainerInfo SAV, EncounterCriteria criteria)
        {
            var version = this.GetCompatibleVersion((GameVersion)SAV.Game);
            int lang = (int)Legal.GetSafeLanguage(Generation, (LanguageID)SAV.Language);
            int level = LevelMin;
            var pk = PKMConverter.GetBlank(Generation, Version);
            SAV.ApplyToPKM(pk);

            pk.Species = Species;
            pk.Language = lang;
            pk.CurrentLevel = level;
            pk.Version = (int)version;
            pk.Nickname = PKX.GetSpeciesNameGeneration(Species, lang, Generation);
            pk.Ball = (int)Type.GetBall();
            pk.Language = lang;
            pk.OT_Friendship = pk.PersonalInfo.BaseFriendship;
            pk.AltForm = GetWildAltForm(pk, Form, SAV);

            SetMetData(pk, level, Location);
            SetPINGA(pk, criteria);
            SetEncounterMoves(pk, version, level);

            SetFormatSpecificData(pk);

            if (pk.Format < 6)
                return pk;

            SAV.ApplyHandlingTrainerInfo(pk);
            pk.SetRandomEC();

            return pk;
        }

        private void SetEncounterMoves(PKM pk, GameVersion version, int level)
        {
            var moves = this is EncounterSlotMoves m ? m.Moves : MoveLevelUp.GetEncounterMoves(pk, level, version);
            pk.Moves = moves;
            pk.SetMaximumPPCurrent(moves);
        }

        private void SetFormatSpecificData(PKM pk)
        {
            if (pk is PK1 pk1)
            {
                if (Species == 64 && Version == GameVersion.YW) // Kadabra
                    pk1.Catch_Rate = 96;
                else if (Species == 148 && Version == GameVersion.YW) // Dragonair
                    pk1.Catch_Rate = 27;
                else
                    pk1.Catch_Rate = PersonalTable.RB[Species].CatchRate; // RB
            }
            else if (pk is PK2 pk2)
            {
                if (Version == GameVersion.C && this is EncounterSlot1 slot)
                    pk2.Met_TimeOfDay = slot.Time.RandomValidTime();
            }
            else if (pk is XK3 xk3)
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
                    var eggMoves = MoveEgg.GetEggMoves(pk, Species, pk.AltForm, Version);
                    if (eggMoves.Length > 0)
                        pk6.RelearnMove1 = eggMoves[Util.Rand.Next(eggMoves.Length)];
                }
                pk.SetRandomMemory6();
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

        private static int GetWildAltForm(PKM pk, int form, ITrainerInfo SAV)
        {
            if (form < 30)
            {
                switch (pk.Species)
                {
                    case 774: return Util.Rand.Next(7, 14); // Minior
                    default: return form;
                }
            }
            if (form == 31)
                return Util.Rand.Next(pk.PersonalInfo.FormeCount);
            if (pk.Species == 664 || pk.Species == 665 || pk.Species == 666)
                return Legal.GetVivillonPattern(SAV.Country, SAV.SubRegion);
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
