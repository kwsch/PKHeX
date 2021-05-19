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
    public abstract record EncounterTrade : IEncounterable, IMoveset, ILocation, IEncounterMatch
    {
        public int Species { get; init; }
        public int Form { get; init; }
        public int Level { get; init; }
        public virtual int LevelMin => Level;
        public int LevelMax => 100;
        public IReadOnlyList<int> Moves { get; init; } = Array.Empty<int>();
        public abstract int Generation { get; }
        public GameVersion Version { get; }

        public int CurrentLevel { get; init; } = -1;
        public abstract int Location { get; }
        public int Ability { get; init; }
        public int Gender { get; init; } = -1;
        public Nature Nature { get; init; } = Nature.Random;
        public Shiny Shiny { get; init; } = Shiny.Never;
        public int Ball { get; init; } = 4;

        public int TID { get; init; }
        public int SID { get; init; }
        public int OTGender { get; init; } = -1;

        public IReadOnlyList<int> IVs { get; init; } = Array.Empty<int>();

        public bool EggEncounter => false;
        public int EggLocation { get; init; }
        public bool EvolveOnTrade { get; init; }

        public int TID7
        {
            init
            {
                TID = (ushort) value;
                SID = value >> 16;
            }
        }

        private const string _name = "In-game Trade";
        public string Name => _name;
        public string LongName => _name;
        public bool IsNicknamed { get; init; } = true;
        public bool IsShiny => Shiny.IsShiny();

        public IReadOnlyList<string> Nicknames { get; internal set; } = Array.Empty<string>();
        public IReadOnlyList<string> TrainerNames { get; internal set; } = Array.Empty<string>();
        public string GetNickname(int language) => (uint)language < Nicknames.Count ? Nicknames[language] : string.Empty;
        public string GetOT(int language) => (uint)language < TrainerNames.Count ? TrainerNames[language] : string.Empty;
        public bool HasNickname => Nicknames.Count != 0 && IsNicknamed;
        public bool HasTrainerName => TrainerNames.Count != 0;

        protected EncounterTrade(GameVersion game) => Version = game;

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
            int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)sav.Language, version);
            int level = CurrentLevel > 0 ? CurrentLevel : LevelMin;
            if (level == 0)
                level = Math.Max(1, LevelMin);

            int species = Species;
            if (EvolveOnTrade)
                species++;

            pk.EncryptionConstant = Util.Rand32();
            pk.Species = species;
            pk.Form = Form;
            pk.Language = lang;
            pk.OT_Name = pk.Format == 1 ? StringConverter12.G1TradeOTStr : HasTrainerName ? GetOT(lang) : sav.OT;
            pk.OT_Gender = HasTrainerName ? Math.Max(0, OTGender) : sav.Gender;
            pk.SetNickname(HasNickname ? GetNickname(lang) : string.Empty);

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
                SetMetData(pk, level, Location, time);
            }
            else
            {
                pk.OT_Gender = 0;
            }

            if (EggLocation != 0)
                SetEggMetData(pk, time);

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
            int ability = criteria.GetAbilityFromNumber(Ability);

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

        public virtual bool IsMatchExact(PKM pkm, DexLevel evo)
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

            if (!IsMatchLevel(pkm, evo))
                return false;

            if (CurrentLevel != -1 && CurrentLevel > pkm.CurrentLevel)
                return false;

            if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pkm.Form, pkm.Format))
                return false;
            if (OTGender != -1 && OTGender != pkm.OT_Gender)
                return false;
            if (EggLocation != pkm.Egg_Location)
                return false;
            // if (z.Ability == 4 ^ pkm.AbilityNumber == 4) // defer to Ability
            //    continue;
            if (!Version.Contains((GameVersion)pkm.Version))
                return false;

            return true;
        }

        private bool IsMatchLevel(PKM pkm, DexLevel evo)
        {
            if (!pkm.HasOriginalMetLocation)
                return evo.Level >= Level;

            if (Location != pkm.Met_Location)
                return false;

            if (pkm.Format < 5)
                return evo.Level >= Level;

            return pkm.Met_Level == Level;
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

        public EncounterMatchRating GetMatchRating(PKM pkm)
        {
            if (IsMatchPartial(pkm))
                return EncounterMatchRating.PartialMatch;
            if (IsMatchDeferred(pkm))
                return EncounterMatchRating.Deferred;
            return EncounterMatchRating.Match;
        }

        protected virtual bool IsMatchDeferred(PKM pkm) => false;
        protected virtual bool IsMatchPartial(PKM pkm) => false;
    }
}
