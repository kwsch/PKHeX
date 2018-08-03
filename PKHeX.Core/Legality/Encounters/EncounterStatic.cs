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
        public int[] Relearn { get; set; } = Array.Empty<int>();
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
            SanityCheckVersion(ref version);

            int lang = (int)Legal.GetSafeLanguage(Generation, (LanguageID)SAV.Language);
            int level = LevelMin;
            var pk = PKMConverter.GetBlank(Generation);
            int nature = Nature == Nature.Random ? Util.Rand.Next(25) : (int)Nature;
            var today = DateTime.Today;
            SAV.ApplyToPKM(pk);

            pk.EncryptionConstant = Util.Rand32();
            pk.Species = Species;
            int gender = Gender < 0 ? pk.PersonalInfo.RandomGender : Gender;
            pk.Language = lang = GetEdgeCaseLanguage(pk, lang);
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
                bool traded = (int)Version == SAV.Game;
                pk.Met_Location = Math.Max(0, EncounterSuggestion.GetSuggestedEggMetLocation(pk));
                pk.Met_Level = EncounterSuggestion.GetSuggestedEncounterEggMetLevel(pk);
                if (pk.GenNumber >= 4)
                {
                    pk.Egg_Location = EncounterSuggestion.GetSuggestedEncounterEggLocationEgg(pk, traded);
                    pk.EggMetDate = today;
                }
                pk.Egg_Location = EggLocation;
                pk.EggMetDate = today;
            }

            pk.AltForm = Form;

            if (this is EncounterStaticPID p)
            {
                pk.PID = p.PID;
                pk.Gender = PKX.GetGenderFromPID(Species, p.PID);
                if (pk is PK5 pk5)
                {
                    pk5.IVs = new[] {30, 30, 30, 30, 30, 30};
                    pk5.NPokémon = p.NSparkle;
                    pk5.OT_Name = Legal.GetG5OT_NSparkle(lang);
                    pk5.TID = 00002;
                    pk5.SID = 00000;
                }
                else
                {
                    SetIVs(pk);
                }
                if (Generation >= 5)
                    pk.Nature = nature;
                pk.RefreshAbility(Ability >> 1);
            }
            else
            {
                var pidtype = GetPIDType();
                PIDGenerator.SetRandomWildPID(pk, pk.Format, nature, Ability >> 1, gender, pidtype);
                SetIVs(pk);
            }

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

            if (pk is IContestStats s)
                this.CopyContestStatsTo(s);

            var moves = Moves ?? MoveLevelUp.GetEncounterMoves(pk, level, version);
            pk.HeldItem = HeldItem;
            pk.Moves = moves;
            pk.SetMaximumPPCurrent(moves);
            if (pk.Format >= 6 && Relearn.Length > 0)
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

        private void SanityCheckVersion(ref GameVersion version)
        {
            if (Generation != 4 || version == GameVersion.Pt)
                return;
            switch (Species)
            {
                case 491 when Location == 079: // DP Darkrai
                case 492 when Location == 063: // DP Shaymin
                    version = GameVersion.Pt;
                    return;
            }
        }

        private void SetIVs(PKM pk)
        {
            if (IVs != null)
                pk.SetRandomIVs(IVs, FlawlessIVCount);
            else if (FlawlessIVCount > 0)
                pk.SetRandomIVs(flawless: FlawlessIVCount);
        }

        private int GetEdgeCaseLanguage(PKM pk, int lang)
        {
            switch (pk.Format)
            {
                case 1 when Species == 151 && Version == GameVersion.VCEvents: // VC Mew
                    pk.TID = 22796;
                    pk.OT_Name = Legal.GetG1OT_GFMew(lang);
                    return lang;
                case 1 when Version == GameVersion.EventsGBGen1:
                case 2 when Version == GameVersion.EventsGBGen2:
                case 3 when this is EncounterStaticShadow s && s.EReader:
                case 3 when Species == 151:
                    pk.OT_Name = "ゲーフリ";
                    return 1; // Old Sea Map was only distributed to Japanese games.

                default:
                    return lang;
            }
        }

        private PIDType GetPIDType()
        {
            switch (Generation)
            {
                case 3 when Roaming && Version != GameVersion.E: // Roamer IV glitch was fixed in Emerald
                    return PIDType.Method_1_Roamer;
                case 4 when Shiny == Shiny.Always: // Lake of Rage Gyarados
                    return PIDType.ChainShiny;
                case 4 when Species == 172: // Spiky Eared Pichu
                case 4 when Location == 233: // Pokéwalker
                    return PIDType.Pokewalker;
                case 5 when Shiny == Shiny.Always:
                    return PIDType.G5MGShiny;

                default: return PIDType.None;
            }
        }
    }
}
