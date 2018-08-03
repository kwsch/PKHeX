using System;

namespace PKHeX.Core
{
    public class EncounterSlotPermissions
    {
        public int StaticIndex { get; set; } = -1;
        public int MagnetPullIndex { get; set; } = -1;
        public int StaticCount { get; set; }
        public int MagnetPullCount { get; set; }

        public bool AllowDexNav { get; set; }
        public bool Pressure { get; set; }
        public bool DexNav { get; set; }
        public bool WhiteFlute { get; set; }
        public bool BlackFlute { get; set; }
        public bool IsNormalLead => !(WhiteFlute || BlackFlute || DexNav);
        public bool IsDexNav => AllowDexNav && DexNav;
        public EncounterSlotPermissions Clone() => (EncounterSlotPermissions)MemberwiseClone();
    }

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

        public string Name
        {
            get
            {
                const string wild = "Wild Encounter";
                if (Type == SlotType.Any)
                    return wild;
                return $"{wild} {Type.ToString().Replace("_", " ")}";
            }
        }

        public PKM ConvertToPKM(ITrainerInfo SAV)
        {
            var version = this.GetCompatibleVersion((GameVersion)SAV.Game);
            int lang = (int)Legal.GetSafeLanguage(Generation, (LanguageID)SAV.Language);
            int level = LevelMin;
            var pk = PKMConverter.GetBlank(Generation);
            int nature = Util.Rand.Next(25);
            SAV.ApplyToPKM(pk);

            pk.Species = Species;
            int gender = pk.PersonalInfo.RandomGender;
            pk.Language = lang;
            pk.CurrentLevel = level;
            pk.Version = (int)version;
            pk.Nickname = PKX.GetSpeciesNameGeneration(Species, lang, Generation);
            pk.Ball = GetBall();
            pk.AltForm = GetWildAltForm(Form, pk, SAV);

            if (pk.Format > 2 || Version == GameVersion.C)
            {
                pk.Met_Location = Location;
                pk.Met_Level = level;
                if (Version == GameVersion.C && pk is PK2 pk2 && this is EncounterSlot1 slot)
                    pk2.Met_TimeOfDay = slot.Time.RandomValidTime();

                if (pk.Format >= 4)
                    pk.MetDate = DateTime.Today;
            }
            pk.Language = lang;

            var ability = Util.Rand.Next(0, 2);
            var pidtype = GetPIDType();
            if (pidtype == PIDType.PokeSpot)
                PIDGenerator.SetRandomPokeSpotPID(pk, nature, gender, ability, SlotNumber);
            else
                PIDGenerator.SetRandomWildPID(pk, pk.Format, nature, ability, gender, pidtype);

            if (Permissions.IsDexNav)
            {
                pk.RefreshAbility(2);
                var eggMoves = MoveEgg.GetEggMoves(pk, Species, pk.AltForm, Version);
                if (eggMoves.Length > 0)
                    pk.RelearnMove1 = eggMoves[Util.Rand.Next(eggMoves.Length)];
            }

            switch (pk.Format)
            {
                case 1 when Species == 64 && Version == GameVersion.YW: // Kadabra
                    ((PK1)pk).Catch_Rate = 96;
                    break;
                case 1 when Species == 148 && Version == GameVersion.YW: // Dragonair
                    ((PK1)pk).Catch_Rate = 27;
                    break;
                case 3:
                case 4:
                    if (pk.Format == 4)
                        pk.EncounterType = TypeEncounter.GetIndex();
                    pk.Gender = gender;
                    break;
                case 5:
                    if (Type == SlotType.HiddenGrotto)
                        pk.RefreshAbility(2);
                    break;
                case 6:
                    pk.SetRandomMemory6();
                    break;
            }

            var moves = this is EncounterSlotMoves m ? m.Moves : MoveLevelUp.GetEncounterMoves(pk, level, version);
            if (Version == GameVersion.XD)
                pk.FatefulEncounter = true;
            pk.Moves = moves;
            pk.SetMaximumPPCurrent(moves);
            pk.OT_Friendship = pk.PersonalInfo.BaseFriendship;
            if (pk.Format < 6)
                return pk;

            SAV.ApplyHandlingTrainerInfo(pk);
            pk.SetRandomEC();

            return pk;
        }

        private static int GetWildAltForm(int form, PKM pk, ITrainerInfo SAV)
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

        private int GetBall()
        {
            if (Type == SlotType.BugContest)
                return 24; // Sport
            if (Type.IsSafariType())
                return 5; // Safari
            return 4; // Poké Ball
        }
    }
}
