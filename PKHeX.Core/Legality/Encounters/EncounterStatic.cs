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
        public int[] Relearn { get; set; } = Array.Empty<int>();
        public int Gender { get; set; } = -1;
        public int EggLocation { get; set; }
        public Nature Nature { get; set; } = Nature.Random;
        public bool Gift { get; set; }
        public int Ball { get; set; } = 4; // Only checked when is Gift
        public GameVersion Version { get; set; } = GameVersion.Any;
        public int[] IVs { get; set; }
        public int FlawlessIVCount { get; set; }

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
        public string Name => _name;
        public string LongName => Version == GameVersion.Any ? _name : $"{_name} ({Version})";

        public PKM ConvertToPKM(ITrainerInfo SAV) => ConvertToPKM(SAV, EncounterCriteria.Unrestricted);

        public PKM ConvertToPKM(ITrainerInfo SAV, EncounterCriteria criteria)
        {
            var pk = PKMConverter.GetBlank(Generation, Version);
            SAV.ApplyToPKM(pk);

            pk.EncryptionConstant = Util.Rand32();
            pk.Species = Species;
            pk.AltForm = Form;

            int lang = (int)Legal.GetSafeLanguage(Generation, (LanguageID)SAV.Language);
            int level = LevelMin;
            var version = this.GetCompatibleVersion((GameVersion)SAV.Game);
            SanityCheckVersion(ref version);

            pk.Language = lang = GetEdgeCaseLanguage(pk, lang);
            pk.Nickname = PKX.GetSpeciesNameGeneration(Species, lang, Generation);

            pk.CurrentLevel = level;
            pk.Version = (int)version;
            pk.Ball = Ball;
            pk.HeldItem = HeldItem;
            pk.OT_Friendship = pk.PersonalInfo.BaseFriendship;

            var today = DateTime.Today;
            SetMetData(pk, level, today);
            if (EggEncounter)
                SetEggMetData(pk, SAV, today);

            SetPINGA(pk, criteria);
            SetEncounterMoves(pk, version, level);

            switch (pk)
            {
                case PK3 pk3 when this is EncounterStaticShadow:
                    pk3.RibbonNational = true;
                    break;
                case PK4 pk4 when this is EncounterStaticTyped t:
                    pk4.EncounterType = t.TypeEncounter.GetIndex();
                    break;
                case PK6 pk6:
                    pk6.SetRandomMemory6();
                    break;
            }

            if (RibbonWishing && pk is IRibbonSetEvent4 e4)
                e4.RibbonWishing = true;
            if (this is EncounterStaticN n)
                n.SetNPokemonData((PK5)pk, lang);
            if (pk is IContestStats s)
                this.CopyContestStatsTo(s);
            if (Fateful)
                pk.FatefulEncounter = true;

            if (pk.Format < 6)
                return pk;

            if (Relearn.Length > 0)
                pk.RelearnMoves = Relearn;
            SAV.ApplyHandlingTrainerInfo(pk);
            pk.SetRandomEC();

            return pk;
        }

        protected virtual void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            int gender = criteria.GetGender(Gender, pk.PersonalInfo);
            int nature = (int)criteria.GetNature(Nature);
            int ability = Ability;

            var pidtype = GetPIDType();
            PIDGenerator.SetRandomWildPID(pk, pk.Format, nature, ability >> 1, gender, pidtype);
            SetIVs(pk);
        }

        private void SetEggMetData(PKM pk, ITrainerInfo tr, DateTime today)
        {
            pk.Met_Location = Math.Max(0, EncounterSuggestion.GetSuggestedEggMetLocation(pk));
            pk.Met_Level = EncounterSuggestion.GetSuggestedEncounterEggMetLevel(pk);

            if (Generation >= 4)
            {
                bool traded = (int)Version == tr.Game;
                pk.Egg_Location = EncounterSuggestion.GetSuggestedEncounterEggLocationEgg(pk, traded);
                pk.EggMetDate = today;
            }
            pk.Egg_Location = EggLocation;
            pk.EggMetDate = today;
        }

        private void SetMetData(PKM pk, int level, DateTime today)
        {
            if (pk.Format > 2 || Version == GameVersion.C)
            {
                pk.Met_Location = Location;
                pk.Met_Level = level;
                if (Version == GameVersion.C && pk is PK2 pk2)
                    pk2.Met_TimeOfDay = EncounterTime.Any.RandomValidTime();

                if (pk.Format >= 4)
                    pk.MetDate = today;
            }
        }

        private void SetEncounterMoves(PKM pk, GameVersion version, int level)
        {
            var moves = Moves?.Length > 0 ? Moves : MoveLevelUp.GetEncounterMoves(pk, level, version);
            pk.Moves = moves;
            pk.SetMaximumPPCurrent(moves);
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

        protected void SetIVs(PKM pk)
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

        public bool IsMatch(PKM pkm, int lvl)
        {
            if (Nature != Nature.Random && pkm.Nature != (int)Nature)
                return false;

            if (pkm.WasEgg != EggEncounter && pkm.Egg_Location == 0 && pkm.Format > 3 && Generation > 3 && !pkm.IsEgg)
                return false;
            if (this is EncounterStaticPID p && p.PID != pkm.PID)
                return false;

            if (Generation == 3 && EggLocation != 0) // Gen3 Egg
            {
                if (pkm.Format == 3 && pkm.IsEgg && EggLocation != pkm.Met_Location)
                    return false;
            }
            else if (Generation <= 2 && EggLocation != 0) // Gen2 Egg
            {
                if (pkm.Format <= 2)
                {
                    if (pkm.IsEgg)
                    {
                        if (pkm.Met_Location != 0 && pkm.Met_Level != 0)
                            return false;
                    }
                    else
                    {
                        switch (pkm.Met_Level)
                        {
                            case 0 when pkm.Met_Location != 0:
                                return false;
                            case 1 when pkm.Met_Location == 0:
                                return false;
                            default:
                                if (pkm.Met_Location == 0 && pkm.Met_Level != 0)
                                    return false;
                                break;
                        }
                    }
                    if (pkm.Met_Level == 1) // Gen2 Eggs are met at 1, and hatch at level 5.
                        lvl = 5;
                }
            }
            else if (EggLocation != pkm.Egg_Location)
            {
                if (pkm.IsEgg) // unhatched
                {
                    if (EggLocation != pkm.Met_Location)
                        return false;
                    if (pkm.Egg_Location != 0)
                        return false;
                }
                else if (Generation == 4)
                {
                    if (pkm.Egg_Location != Locations.LinkTrade4) // Link Trade
                    {
                        // check Pt/HGSS data
                        if (pkm.Format <= 4)
                            return false; // must match
                        if (!Locations.IsPtHGSSLocationEgg(EggLocation)) // non-Pt/HGSS egg gift
                            return false;
                        // transferring 4->5 clears pt/hgss location value and keeps Faraway Place
                        if (pkm.Egg_Location != 3002) // Faraway Place
                            return false;
                    }
                }
                else
                {
                    if (pkm.Egg_Location != Locations.LinkTrade6) // Link Trade
                        return false;
                }
            }
            else if (EggLocation != 0 && Generation == 4)
            {
                // Check the inverse scenario for 4->5 eggs
                if (Locations.IsPtHGSSLocationEgg(EggLocation)) // egg gift
                {
                    if (pkm.Format > 4)
                        return false; // locations match when it shouldn't
                }
            }

            if (pkm.HasOriginalMetLocation)
            {
                if (!EggEncounter && Location != 0 && Location != pkm.Met_Location)
                    return false;
                if (Level != lvl)
                {
                    if (!(pkm.Format == 3 && EggEncounter && lvl == 0))
                        return false;
                }
            }
            else if (Level > lvl)
            {
                return false;
            }

            if (Gender != -1 && Gender != pkm.Gender)
            {
                if (Species == (int)Core.Species.Azurill && Generation == 4 && Location == 233 && pkm.Gender == 0)
                {
                    if (PKX.GetGenderFromPIDAndRatio(pkm.PID, 0xBF) != 1)
                        return false;
                }
                else
                {
                    return false;
                }
            }
            if (Form != pkm.AltForm && !SkipFormCheck && !Legal.IsFormChangeable(pkm, Species))
                return false;
            if (EggLocation == Locations.Daycare5 && Relearn.Length == 0 && pkm.RelearnMoves.Any(z => z != 0)) // gen7 eevee edge case
                return false;

            if (IVs != null && (Generation > 2 || pkm.Format <= 2)) // 1,2->7 regenerates IVs, only check if original IVs still exist
            {
                if (!Legal.GetIsFixedIVSequenceValidSkipRand(IVs, pkm))
                    return false;
            }

            if (pkm is IContestStats s && s.IsContestBelow(this))
                return false;

            // Defer to EC/PID check
            // if (e.Shiny != null && e.Shiny != pkm.IsShiny)
            // continue;

            // Defer ball check to later
            // if (e.Gift && pkm.Ball != 4) // PokéBall
            // continue;

            return true;
        }
    }
}
