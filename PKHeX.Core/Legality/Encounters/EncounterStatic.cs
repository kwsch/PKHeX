using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Static Encounter Data
    /// </summary>
    /// <remarks>
    /// Static Encounters are fixed position encounters with properties that are not subject to Wild Encounter conditions.
    /// </remarks>
    public class EncounterStatic : IEncounterable, IMoveset, IGeneration, ILocation, IContestStats, IVersion
    {
        public int Species { get; set; }
        public int[] Moves { get; set; }
        public int Level { get; set; }

        public int LevelMin => Level;
        public int LevelMax => Level;
        public int Generation { get; set; } = -1;
        public int Location { get; set; }
        public int Ability { get; set; }
        public int Form { get; set; }
        public virtual Shiny Shiny { get; set; } = Shiny.Random;
        public int[] Relearn { get; set; } = new int[4];
        public int Gender { get; set; } = -1;
        public int EggLocation { get; set; }
        public Nature Nature { get; set; } = Nature.Random;
        public bool Gift { get; set; }
        public int Ball { get; set; } = 4; // Only checked when is Gift
        public GameVersion Version { get; set; } = GameVersion.Any;
        public int[] IVs { get; set; }
        public int FlawlessIVCount { get; internal set; }
        public bool IV3 { set => FlawlessIVCount = value ? 3 : 0; }

        public int[] Contest { set => this.SetContestStats(value); }
        public int CNT_Cool { get; set; }
        public int CNT_Beauty { get; set; }
        public int CNT_Cute { get; set; }
        public int CNT_Smart { get; set; }
        public int CNT_Tough { get; set; }
        public int CNT_Sheen { get; set; }

        public int HeldItem { get; set; }
        public int EggCycles { get; set; }

        public bool Fateful { get; set; }
        public bool RibbonWishing { get; set; }
        public bool SkipFormCheck { get; set; }
        public bool Roaming { get; set; }
        public bool EggEncounter => EggLocation > 0;

        private void CloneArrays()
        {
            // dereference original arrays with new copies
            Moves = (int[])Moves?.Clone();
            Relearn = (int[])Relearn.Clone();
            IVs = (int[])IVs?.Clone();
        }
        internal virtual EncounterStatic Clone()
        {
            var result = (EncounterStatic)MemberwiseClone();
            result.CloneArrays();
            return result;
        }

        private const string _name = "Static Encounter";
        public string Name => Version == GameVersion.Any ? _name : $"{_name} ({Version})";

        public PKM ConvertToPKM(ITrainerInfo SAV)
        {
            var version = this.GetCompatibleVersion((GameVersion)SAV.Game);
            int lang = (int)Legal.GetSafeLanguage(Generation, (LanguageID)SAV.Language);
            int level = LevelMin;
            var pk = PKMConverter.GetBlank(Generation);
            int gender = Gender < 0 ? Util.Rand.Next(2) : Gender;
            int nature = Nature == Nature.Random ? Util.Rand.Next(25) : (int)Nature;
            var today = DateTime.Today;
            SAV.ApplyToPKM(pk);

            pk.EncryptionConstant = Util.Rand32();
            pk.Species = Species;
            pk.Language = lang;
            pk.CurrentLevel = level;
            pk.Version = (int)version;

            if (3 <= pk.Format && pk.Format <= 5)
            {
                pk.SetPIDGender(gender);
                pk.Gender = pk.GetSaneGender(gender);
            }
            else
            {
                pk.PID = Util.Rand32();
                pk.Gender = pk.GetSaneGender(gender);
                pk.Nature = nature;
            }
            pk.Nickname = PKX.GetSpeciesNameGeneration(Species, lang, Generation);
            pk.Ball = Ball;
            pk.Met_Level = level;
            pk.Met_Location = Location;
            pk.MetDate = today;
            if (EggEncounter)
            {
                pk.Egg_Location = EggLocation;
                pk.EggMetDate = today;
            }

            pk.AltForm = Form;

            pk.Language = lang;

            pk.RefreshAbility(Ability >> 1);

            if (IVs != null)
                pk.SetRandomIVs(IVs, FlawlessIVCount);
            else
                pk.SetRandomIVs(flawless: FlawlessIVCount);

            switch (pk.Format)
            {
                case 3:
                case 4:
                    PIDGenerator.SetValuesFromSeed(pk, Roaming ? PIDType.Method_1_Roamer : PIDType.Method_1, Util.Rand32());
                    if (this is EncounterStaticTyped t)
                        pk.EncounterType = t.TypeEncounter.GetIndex();
                    pk.Gender = pk.GetSaneGender(gender);
                    break;
                case 6:
                    pk.SetRandomMemory6();
                    break;
            }

            if (this is EncounterStaticPID pid)
            {
                pk.PID = pid.PID;
                pk.Gender = pk.GetSaneGender(gender);
                if (pk is PK5 pk5)
                    pk5.NPokémon = pid.NSparkle;
            }

            this.CopyContestStatsTo(pk);

            var moves = Moves ?? Legal.GetEncounterMoves(pk, level, version);
            pk.Moves = moves;
            pk.SetMaximumPPCurrent(moves);
            if (pk.Format >= 6 && Relearn != null)
                pk.RelearnMoves = Relearn;
            pk.OT_Friendship = pk.PersonalInfo.BaseFriendship;
            if (Fateful)
                pk.FatefulEncounter = true;

            if (pk.Format < 6)
                return pk;
            if (RibbonWishing && pk is IRibbonSetEvent4 e4)
                e4.RibbonWishing = true;

            SAV.ApplyHandlingTrainerInfo(pk);
            pk.SetRandomEC();

            return pk;
        }
    }
}
