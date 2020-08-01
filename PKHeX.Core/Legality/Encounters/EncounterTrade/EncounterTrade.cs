using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Trade Encounter data
    /// </summary>
    /// <remarks>
    /// Trade data is fixed level in all cases except for the first few generations of games.
    /// </remarks>
    public class EncounterTrade : IEncounterable, IGenerationSet, IMoveset, ILocation, IContestStats, IVersionSet
    {
        public int Species { get; set; }
        public IReadOnlyList<int> Moves { get; set; } = Array.Empty<int>();
        public int Level { get; set; }
        public int LevelMin => Level;
        public int LevelMax => 100;
        public int Generation { get; set; } = -1;

        public int Location { get; set; } = -1;
        public int Ability { get; set; }
        public Nature Nature = Nature.Random;
        public int TID { get; set; }
        public int SID { get; set; }
        public GameVersion Version { get; set; } = GameVersion.Any;
        public IReadOnlyList<int> IVs { get; set; } = Array.Empty<int>();
        public int FlawlessIVCount { get; set; }
        public int Form { get; set; }
        public virtual Shiny Shiny { get; set; } = Shiny.Never;
        public int Gender { get; set; } = -1;
        public int OTGender { get; set; } = -1;
        public bool EggEncounter => false;
        public int EggLocation { get; set; }
        public bool EvolveOnTrade { get; set; }
        public int Ball { get; set; } = 4;
        public int CurrentLevel { get; set; } = -1;

        internal IReadOnlyList<int> Contest { set => this.SetContestStats(value); }
        public int CNT_Cool { get; set; }
        public int CNT_Beauty { get; set; }
        public int CNT_Cute { get; set; }
        public int CNT_Smart { get; set; }
        public int CNT_Tough { get; set; }
        public int CNT_Sheen { get; set; }

        public int TID7
        {
            set
            {
                TID = (ushort) value;
                SID = value >> 16;
            }
        }

        private const string _name = "In-game Trade";
        public string Name => _name;
        public string LongName => _name;
        public bool Fateful { get; set; }
        public bool IsNicknamed { get; set; } = true;

        public IReadOnlyList<string> Nicknames { get; internal set; } = Array.Empty<string>();
        public IReadOnlyList<string> TrainerNames { get; internal set; } = Array.Empty<string>();
        public string GetNickname(int language) => (uint)language < Nicknames.Count ? Nicknames[language] : string.Empty;
        public string GetOT(int language) => (uint)language < TrainerNames.Count ? TrainerNames[language] : string.Empty;
        public bool HasNickname => Nicknames.Count != 0;
        public bool HasTrainerName => TrainerNames.Count != 0;

        private static readonly int[] DefaultMetLocation =
        {
            0,
            Locations.LinkTrade2NPC,
            Locations.LinkTrade3NPC,
            Locations.LinkTrade4NPC,
            Locations.LinkTrade5NPC,
            Locations.LinkTrade6NPC,
            Locations.LinkTrade6NPC, // 7 is same as 6
            Locations.LinkTrade6NPC, // 8 is same as 6
        };

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
            var version = this.GetCompatibleVersion((GameVersion)sav.Game);
            int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)sav.Language);
            int level = CurrentLevel > 0 ? CurrentLevel : LevelMin;
            if (level == 0)
                level = Math.Max(1, LevelMin);

            int species = Species;
            if (EvolveOnTrade)
                species++;

            pk.EncryptionConstant = Util.Rand32();
            pk.Species = species;
            pk.AltForm = Form;
            pk.Language = lang;
            pk.OT_Name = pk.Format == 1 ? StringConverter12.G1TradeOTStr : HasTrainerName ? GetOT(lang) : sav.OT;
            pk.OT_Gender = HasTrainerName ? Math.Max(0, OTGender) : sav.Gender;
            pk.SetNickname(GetNickname(lang));

            pk.CurrentLevel = level;
            pk.Version = (int) version;
            pk.TID = TID;
            pk.SID = SID;
            pk.Ball = Ball;
            pk.OT_Friendship = pk.PersonalInfo.BaseFriendship;

            SetPINGA(pk, criteria);
            SetMoves(pk, version, level);

            var time = DateTime.Now;
            if (pk.Format != 2 || version == GameVersion.C)
            {
                var location = Location > 0 ? Location : DefaultMetLocation[Generation - 1];
                SetMetData(pk, level, location, time);
            }

            if (EggLocation != 0)
                SetEggMetData(pk, time);

            if (pk is IContestStats s)
                this.CopyContestStatsTo(s);

            if (Fateful)
                pk.FatefulEncounter = true;

            UpdateEdgeCase(pk);

            if (pk.Format < 6)
                return;

            sav.ApplyHandlingTrainerInfo(pk, force: true);
            pk.SetRandomEC();

            if (pk is PK6 pk6)
                pk6.SetRandomMemory6();
        }

        protected virtual void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            var pi = pk.PersonalInfo;
            int gender = criteria.GetGender(Gender, pi);
            int nature = (int)criteria.GetNature(Nature);
            int ability = criteria.GetAbilityFromNumber(Ability, pi);

            PIDGenerator.SetRandomWildPID(pk, Generation, nature, ability, gender);
            pk.Nature = pk.StatNature = nature;
            pk.Gender = gender;
            pk.RefreshAbility(ability);

            SetIVs(pk);
        }

        protected void SetIVs(PKM pk)
        {
            if (IVs.Count != 0)
                pk.SetRandomIVs(IVs, 0);
            else
                pk.SetRandomIVs(flawless: 3);
        }

        private void SetMoves(PKM pk, GameVersion version, int level)
        {
            var moves = Moves.Count != 0 ? Moves : MoveLevelUp.GetEncounterMoves(pk, level, version);
            if (pk.Format == 1 && moves.All(z => z == 0))
                moves = ((PersonalInfoG1)PersonalTable.RB[Species]).Moves;
            pk.SetMoves(moves);
            pk.SetMaximumPPCurrent(moves);
        }

        private void SetEggMetData(PKM pk, DateTime time)
        {
            pk.Egg_Location = EggLocation;
            pk.EggMetDate = time;
        }

        private static void SetMetData(PKM pk, int level, int location, DateTime time)
        {
            pk.Met_Level = level;
            pk.Met_Location = location;
            pk.MetDate = time;
        }

        private void UpdateEdgeCase(PKM pkm)
        {
            switch (Generation)
            {
                case 3 when Species == (int)Core.Species.Jynx && pkm.Version == (int) GameVersion.LG && pkm.Language == (int) LanguageID.Italian:
                    // Italian LG Jynx untranslated from English name
                    pkm.OT_Name = GetOT((int)LanguageID.English);
                    pkm.SetNickname(GetNickname((int)LanguageID.English));
                    break;

                case 4 when Version == GameVersion.DPPt && Species == (int)Core.Species.Magikarp: // Meister Magikarp
                    // Has German Language ID for all except German origin, which is English
                    pkm.Language = (int)(pkm.Language == (int)LanguageID.German ? LanguageID.English : LanguageID.German);
                    break;

                case 4 when Version == GameVersion.DPPt && (pkm.Version == (int)GameVersion.D || pkm.Version == (int)GameVersion.P):
                    // DP English origin are Japanese lang
                    pkm.Language = (int)LanguageID.Japanese;
                    break;

                case 4 when Version == GameVersion.HGSS && Species == (int)Core.Species.Pikachu: // Pikachu
                    // Has English Language ID for all except English origin, which is French
                    pkm.Language = (int)(pkm.Language == (int)LanguageID.English ? LanguageID.French : LanguageID.English);
                    break;

                case 5 when Version == GameVersion.BW && pkm.Language == (int)LanguageID.Japanese:
                    // Trades for JPN games have language ID of 0, not 1.
                    pkm.Language = 0;
                    break;
            }
        }

        public virtual bool IsMatch(PKM pkm, DexLevel evo, int lvl)
        {
            if (IVs.Count != 0)
            {
                if (!Legal.GetIsFixedIVSequenceValidSkipRand(IVs, pkm))
                    return false;
            }

            if (!IsMatchNatureGenderShiny(pkm))
                return false;
            if (TID != pkm.TID)
                return false;
            if (SID != pkm.SID)
                return false;

            if (!IsMatchLevel(pkm, lvl))
                return false;

            if (CurrentLevel != -1 && CurrentLevel > pkm.CurrentLevel)
                return false;

            if (Form != evo.Form && !Legal.IsFormChangeable(pkm, Species, Form))
                return false;
            if (OTGender != -1 && OTGender != pkm.OT_Gender)
                return false;
            if (EggLocation != pkm.Egg_Location)
                return false;
            // if (z.Ability == 4 ^ pkm.AbilityNumber == 4) // defer to Ability
            //    continue;
            if (!Version.Contains((GameVersion)pkm.Version))
                return false;

            if (pkm is IContestStats s && s.IsContestBelow(this))
                return false;

            return true;
        }

        private bool IsMatchLevel(PKM pkm, int lvl)
        {
            if (pkm.HasOriginalMetLocation)
            {
                var loc = Location > 0 ? Location : DefaultMetLocation[Generation - 1];
                if (loc != pkm.Met_Location)
                    return false;

                if (pkm.Format < 5)
                {
                    if (Level > lvl)
                        return false;
                }
                else if (Level != lvl)
                {
                    return false;
                }
            }
            else if (Level > lvl)
            {
                return false;
            }

            return true;
        }

        protected virtual bool IsMatchNatureGenderShiny(PKM pkm)
        {
            if (!Shiny.IsValid(pkm))
                return false;
            if (Gender != -1 && Gender != pkm.Gender)
                return false;

            if (Nature != Nature.Random && pkm.Nature != (int)Nature)
                return false;

            return true;
        }
    }
}
