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
    public class EncounterTrade : IEncounterable, IMoveset, IGeneration, ILocation, IContestStats, IVersion
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

        public static readonly int[] DefaultMetLocation =
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

        public PKM ConvertToPKM(ITrainerInfo SAV) => ConvertToPKM(SAV, EncounterCriteria.Unrestricted);

        public PKM ConvertToPKM(ITrainerInfo SAV, EncounterCriteria criteria)
        {
            var pk = PKMConverter.GetBlank(Generation, Version);
            SAV.ApplyToPKM(pk);

            ApplyDetails(SAV, criteria, pk);
            return pk;
        }

        protected virtual void ApplyDetails(ITrainerInfo SAV, EncounterCriteria criteria, PKM pk)
        {
            var version = this.GetCompatibleVersion((GameVersion)SAV.Game);
            int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)SAV.Language);
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
            pk.OT_Name = pk.Format == 1 ? StringConverter12.G1TradeOTStr : HasTrainerName ? GetOT(lang) : SAV.OT;
            pk.OT_Gender = HasTrainerName ? Math.Max(0, OTGender) : SAV.Gender;
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

            if (pk is PK1 pk1 && this is EncounterTradeCatchRate c)
                pk1.Catch_Rate = (int) c.Catch_Rate;

            if (pk is IContestStats s)
                this.CopyContestStatsTo(s);

            if (Fateful)
                pk.FatefulEncounter = true;

            UpdateEdgeCase(pk);

            if (pk.Format < 6)
                return;

            SAV.ApplyHandlingTrainerInfo(pk, force: true);
            pk.SetRandomEC();

            if (pk.Format == 6)
                pk.SetRandomMemory6();
        }

        protected virtual void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            int gender = criteria.GetGender(Gender, pk.PersonalInfo);
            int nature = (int)criteria.GetNature(Nature);
            int ability = Ability >> 1;

            PIDGenerator.SetRandomWildPID(pk, Generation, nature, ability, gender);
            pk.Nature = nature;
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

        public virtual bool IsMatch(PKM pkm, int lvl)
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

            if (Form != pkm.AltForm && !Legal.IsFormChangeable(pkm, Species))
                return false;
            if (OTGender != -1 && OTGender != pkm.OT_Gender)
                return false;
            if (EggLocation != pkm.Egg_Location)
                return false;
            // if (z.Ability == 4 ^ pkm.AbilityNumber == 4) // defer to Ability
            //    countinue;
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

        public bool IsMatchVC1(PKM pkm)
        {
            if (Level > pkm.CurrentLevel) // minimum required level
                return false;
            if (pkm.Format != 1 || !pkm.Gen1_NotTradeback)
                return true;

            // Even if the in game trade uses the tables with source pokemon allowing generation 2 games, the traded pokemon could be a non-tradeback pokemon
            var rate = (pkm as PK1)?.Catch_Rate;
            if (this is EncounterTradeCatchRate r)
            {
                if (rate != r.Catch_Rate)
                    return false;
            }
            else
            {
                if (Version == GameVersion.YW && rate != PersonalTable.Y[Species].CatchRate)
                    return false;
                if (Version != GameVersion.YW && rate != PersonalTable.RB[Species].CatchRate)
                    return false;
            }
            return true;
        }

        public bool IsMatchVC2(PKM pkm)
        {
            if (Level > pkm.CurrentLevel) // minimum required level
                return false;
            if (TID != pkm.TID)
                return false;
            if (pkm.Format <= 2)
            {
                if (Gender >= 0 && Gender != pkm.Gender)
                    return false;
                if (IVs.Count != 0 && !Legal.GetIsFixedIVSequenceValidNoRand(IVs, pkm))
                    return false;
            }
            if (pkm.Met_Location != 0 && pkm.Format == 2 && pkm.Met_Location != 126)
                return false;

            if (!IsValidTradeOT12Gender(pkm))
                return false;
            return IsValidTradeOT12(pkm);
        }

        private bool IsValidTradeOT12Gender(PKM pkm)
        {
            if (OTGender == 1)
            {
                // Female, can be cleared if traded to RBY (clears met location)
                if (pkm.Format <= 2)
                    return pkm.OT_Gender == (pkm.Met_Location != 0 ? 1 : 0);
                return pkm.OT_Gender == 0 || !pkm.VC1; // require male except if transferred from GSC
            }
            return pkm.OT_Gender == 0;
        }

        private bool IsValidTradeOT12(PKM pkm)
        {
            var OT = pkm.OT_Name;
            if (pkm.Japanese)
                return TrainerNames[(int)LanguageID.Japanese] == OT;
            if (pkm.Korean)
                return TrainerNames[(int)LanguageID.Korean] == OT;

            if (pkm.Format >= 7)
            {
                switch (Species)
                {
                    case (int)Core.Species.Voltorb:
                        // Spanish FALCÁN trade loses the accented A on transfer
                        if (OT == "FALCÁN")
                            return false;
                        if (OT == "FALC N")
                            return true;
                        break;
                    case (int)Core.Species.Shuckle:
                        // Spanish MANÍA trade loses the accented I on transfer
                        if (OT == "MANÍA")
                            return false;
                        if (OT == "MAN A")
                            return true;
                        break;
                }
            }

            const int start = (int)LanguageID.English;
            const int end = (int)LanguageID.Spanish;

            for (int i = start; i <= end; i++)
            {
                if (TrainerNames[i] == OT)
                    return true;
            }
            return false;
        }
    }

    public sealed class EncounterTrade6 : EncounterTrade, IMemoryOT
    {
        public int OT_Memory { get; }
        public int OT_Intensity { get; }
        public int OT_Feeling { get; }
        public int OT_TextVar { get; }

        public EncounterTrade6(int m, int i, int f, int v)
        {
            OT_Memory = m;
            OT_Intensity = i;
            OT_Feeling = f;
            OT_TextVar = v;
        }

        protected override void ApplyDetails(ITrainerInfo SAV, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(SAV, criteria, pk);
            pk.OT_Memory = OT_Memory;
            pk.OT_Intensity = OT_Intensity;
            pk.OT_Feeling = OT_Feeling;
            pk.OT_TextVar = OT_TextVar;
        }
    }

    public sealed class EncounterTrade7 : EncounterTrade, IMemoryOT
    {
        public int OT_Memory => 1;
        public int OT_Intensity => 3;
        public int OT_Feeling => 5;
        public int OT_TextVar => 40;

        protected override void ApplyDetails(ITrainerInfo SAV, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(SAV, criteria, pk);
            pk.OT_Memory = OT_Memory;
            pk.OT_Intensity = OT_Intensity;
            pk.OT_Feeling = OT_Feeling;
            pk.OT_TextVar = OT_TextVar;
        }
    }
}
