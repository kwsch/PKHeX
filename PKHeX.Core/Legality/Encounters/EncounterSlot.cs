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
            int gender = Util.Rand.Next(2);
            pk.Gender = pk.GetSaneGender(gender);

            int nature = Util.Rand.Next(25);
            pk.Nature = nature;
            pk.EncryptionConstant = Util.Rand32();
            pk.Species = Species;
            pk.Language = lang;
            pk.CurrentLevel = level;
            pk.Version = (int) version;
            pk.PID = Util.Rand32();
            pk.Nickname = PKX.GetSpeciesNameGeneration(Species, lang, Generation);
            pk.Ball = 4;
            pk.Met_Level = level;
            pk.Met_Location = Location;
            pk.MetDate = DateTime.Today;

            SAV.ApplyToPKM(pk);
            pk.Language = lang;

            pk.SetRandomIVs(flawless: 3);

            if (Permissions.IsDexNav)
            {
                pk.RefreshAbility(2);
                var eggMoves = Legal.GetEggMoves(pk, Species, pk.AltForm, Version);
                if (eggMoves.Length > 0)
                    pk.RelearnMove1 = eggMoves[Util.Rand.Next(eggMoves.Length)];
            }
            else
            {
                pk.RefreshAbility(Util.Rand.Next(2));
            }

            switch (pk.Format)
            {
                case 3:
                case 4:
                    PIDGenerator.SetValuesFromSeed(pk, PIDType.Method_1, Util.Rand32());
                    if (pk.Format == 4)
                        pk.EncounterType = TypeEncounter.GetIndex();
                    break;
                case 6:
                    pk.SetRandomMemory6();
                    break;
            }

            var moves = this is EncounterSlotMoves m ? m.Moves : Legal.GetEncounterMoves(pk, level, version);
            pk.Moves = moves;
            pk.SetMaximumPPCurrent(moves);
            pk.OT_Friendship = pk.PersonalInfo.BaseFriendship;
            if (pk.Format < 6)
                return pk;

            SAV.ApplyHandlingTrainerInfo(pk);
            pk.SetRandomEC();

            return pk;
        }
    }
}
