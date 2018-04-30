using System;
using System.Linq;

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
            int nature = Nature == Nature.Random ? Util.Rand.Next(25) : (int)Nature;
            var today = DateTime.Today;
            SAV.ApplyToPKM(pk);

            pk.EncryptionConstant = Util.Rand32();
            pk.Species = Species;
            int gender = Gender < 0 ? pk.PersonalInfo.RandomGender : Gender;
            pk.Language = lang;
            pk.CurrentLevel = level;
            pk.Version = (int)version;
            pk.Nickname = PKX.GetSpeciesNameGeneration(Species, lang, Generation);
            pk.Ball = Ball;

            if (pk.Format > 2 || Version == GameVersion.C)
            {
                pk.Met_Location = Location;
                pk.Met_Level = level;
                if (Version == GameVersion.C && pk is PK2 pk2)
                    pk2.Met_TimeOfDay = EncounterTime.Any.RandomValidTime();

                if (pk.Format >= 4)
                    pk.MetDate = DateTime.Today;
            }
            if (EggEncounter)
            {
                pk.Egg_Location = EggLocation;
                pk.EggMetDate = today;
            }

            pk.AltForm = Form;
            pk.Language = lang;

            if (this is EncounterStaticPID pid)
            {
                pk.PID = pid.PID;
                pk.Gender = pk.GetSaneGender(gender);
                if (pk is PK5 pk5)
                    pk5.NPokémon = pid.NSparkle;
            }
            else
            {
                var pidtype = GetPIDType();
                PIDGenerator.SetRandomWildPID(pk, pk.Format, nature, Ability >> 1, gender, pidtype);
            }

            if (IVs != null)
                pk.SetRandomIVs(IVs, FlawlessIVCount);
            else if (FlawlessIVCount > 0)
                pk.SetRandomIVs(flawless: FlawlessIVCount);

            switch (pk.Format)
            {
                case 3:
                    if (this is EncounterStaticShadow)
                        ((PK3)pk).RibbonNational = true;
                    break;
                case 4:
                    if (this is EncounterStaticTyped t)
                        pk.EncounterType = t.TypeEncounter.GetIndex();
                    break;
                case 6:
                    pk.SetRandomMemory6();
                    break;
            }

            this.CopyContestStatsTo(pk);

            var moves = Moves ?? Legal.GetEncounterMoves(pk, level, version);
            if (pk.Format == 1 && moves.All(z => z == 0))
                moves = (PersonalTable.RB[Species] as PersonalInfoG1).Moves;
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

        private PIDType GetPIDType()
        {
            if (Roaming)
                return PIDType.Method_1_Roamer;
            if (Version == GameVersion.HGSS && Location == 233) // Pokéwalker
                return PIDType.Pokewalker;
            return PIDType.None;
        }
    }
}
