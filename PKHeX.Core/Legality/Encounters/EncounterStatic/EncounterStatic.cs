using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Static Encounter Data
    /// </summary>
    /// <remarks>
    /// Static Encounters are fixed position encounters with properties that are not subject to Wild Encounter conditions.
    /// </remarks>
    public class EncounterStatic : IEncounterable, IMoveset, IGeneration, ILocation, IContestStats, IVersion, IRelearn
    {
        public int Species { get; set; }
        public IReadOnlyList<int> Moves { get; set; } = Array.Empty<int>();
        public virtual int Level { get; set; }

        public virtual int LevelMin => Level;
        public virtual int LevelMax => Level;
        public int Generation { get; set; } = -1;
        public virtual int Location { get; set; }
        public int Ability { get; set; }
        public int Form { get; set; }
        public virtual Shiny Shiny { get; set; } = Shiny.Random;
        public IReadOnlyList<int> Relearn { get; set; } = Array.Empty<int>();
        public int Gender { get; set; } = -1;
        public int EggLocation { get; set; }
        public Nature Nature { get; set; } = Nature.Random;
        public bool Gift { get; set; }
        public int Ball { get; set; } = 4; // Only checked when is Gift
        public GameVersion Version { get; set; } = GameVersion.Any;
        public IReadOnlyList<int> IVs { get; set; } = Array.Empty<int>();
        public int FlawlessIVCount { get; set; }

        internal IReadOnlyList<int> Contest { set => this.SetContestStats(value); }
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

        internal EncounterStatic Clone() => (EncounterStatic)MemberwiseClone();

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

            int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)SAV.Language);
            int level = GetMinimalLevel();
            var version = this.GetCompatibleVersion((GameVersion)SAV.Game);
            SanityCheckVersion(ref version);

            pk.Language = lang = GetEdgeCaseLanguage(pk, lang);
            pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation);

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
            if (this is EncounterStatic5N n)
                n.SetNPokemonData((PK5)pk, lang);
            if (pk is IContestStats s)
                this.CopyContestStatsTo(s);
            if (Fateful)
                pk.FatefulEncounter = true;

            if (pk.Format < 6)
                return pk;

            pk.SetRelearnMoves(Relearn);
            SAV.ApplyHandlingTrainerInfo(pk);
            pk.SetRandomEC();

            if (this is IGigantamax g && pk is IGigantamax pg)
                pg.CanGigantamax = g.CanGigantamax;
            if (this is IDynamaxLevel d && pk is IDynamaxLevel pd)
                pd.DynamaxLevel = d.DynamaxLevel;

            return pk;
        }

        protected virtual int GetMinimalLevel() => LevelMin;

        protected virtual void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            int gender = criteria.GetGender(Gender, pk.PersonalInfo);
            int nature = (int)criteria.GetNature(Nature);
            int ability = GetRandomAbility(); // use criteria?

            var pidtype = GetPIDType();
            PIDGenerator.SetRandomWildPID(pk, pk.Format, nature, ability, gender, pidtype);
            SetIVs(pk);
        }

        private int GetRandomAbility()
        {
            return Ability switch
            {
                 0 => Util.Rand.Next(2),
                -1 => Util.Rand.Next(3),
                _ => (Ability >> 1)
            };
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
            var moves = Moves.Count > 0 ? Moves : MoveLevelUp.GetEncounterMoves(pk, level, version);
            pk.SetMoves(moves);
            pk.SetMaximumPPCurrent(moves);
        }

        private void SanityCheckVersion(ref GameVersion version)
        {
            if (Generation != 4 || version == GameVersion.Pt)
                return;
            switch (Species)
            {
                case (int)Core.Species.Darkrai when Location == 079: // DP Darkrai
                case (int)Core.Species.Shaymin when Location == 063: // DP Shaymin
                    version = GameVersion.Pt;
                    return;
            }
        }

        protected void SetIVs(PKM pk)
        {
            if (IVs.Count != 0)
                pk.SetRandomIVs(IVs, FlawlessIVCount);
            else if (FlawlessIVCount > 0)
                pk.SetRandomIVs(flawless: FlawlessIVCount);
        }

        private int GetEdgeCaseLanguage(PKM pk, int lang)
        {
            switch (pk.Format)
            {
                case 1 when Species == (int)Core.Species.Mew && Version == GameVersion.VCEvents: // VC Mew
                    pk.TID = 22796;
                    pk.OT_Name = Legal.GetG1OT_GFMew(lang);
                    return lang;
                case 1 when Version == GameVersion.EventsGBGen1:
                case 2 when Version == GameVersion.EventsGBGen2:
                case 3 when this is EncounterStaticShadow s && s.EReader:
                case 3 when Species == (int)Core.Species.Mew:
                    pk.OT_Name = "ゲーフリ";
                    return (int)LanguageID.Japanese; // Old Sea Map was only distributed to Japanese games.

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
                case 4 when Species == (int)Core.Species.Pichu: // Spiky Eared Pichu
                case 4 when Location == Locations.PokeWalker4: // Pokéwalker
                    return PIDType.Pokewalker;
                case 5 when Shiny == Shiny.Always:
                    return PIDType.G5MGShiny;

                default: return PIDType.None;
            }
        }

        public virtual bool IsMatch(PKM pkm, int lvl)
        {
            if (Nature != Nature.Random && pkm.Nature != (int) Nature)
                return false;

            if (Generation > 3 && pkm.Format > 3 && pkm.WasEgg != EggEncounter && pkm.Egg_Location == 0 && !pkm.IsEgg)
                return false;

            if (!IsMatchEggLocation(pkm, ref lvl))
                return false;
            if (!IsMatchLocation(pkm))
                return false;
            if (!IsMatchLevel(pkm, lvl))
                return false;
            if (!IsMatchGender(pkm))
                return false;
            if (!IsMatchForm(pkm))
                return false;

            if (EggLocation == Locations.Daycare5 && Relearn.Count == 0 && pkm.RelearnMoves.Any(z => z != 0)) // gen7 eevee edge case
                return false;

            if (!IsMatchIVs(pkm))
                return false;

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

        private bool IsMatchIVs(PKM pkm)
        {
            if (IVs.Count == 0)
                return true; // nothing to check, IVs are random
            if (Generation <= 2 && pkm.Format > 2)
                return true; // IVs are regenerated on VC transfer upward

            return Legal.GetIsFixedIVSequenceValidSkipRand(IVs, pkm);
        }

        private bool IsMatchForm(PKM pkm)
        {
            if (SkipFormCheck)
                return true;
            if (FormConverter.IsTotemForm(Species, Form, Generation))
            {
                var expectForm = pkm.Format == 7 ? Form : FormConverter.GetTotemBaseForm(Species, Form);
                return expectForm == pkm.AltForm;
            }
            if (Form != pkm.AltForm && !Legal.IsFormChangeable(pkm, Species))
                return false;
            return true;
        }

        private bool IsMatchEggLocation(PKM pkm, ref int lvl)
        {
            if (Generation == 3 && EggLocation != 0) // Gen3 Egg
            {
                if (pkm.Format == 3 && pkm.IsEgg && EggLocation != pkm.Met_Location)
                    return false;
            }
            else if (Generation <= 2 && EggLocation != 0) // Gen2 Egg
            {
                if (pkm.Format > 2)
                    return true;

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
                            return false;
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
                        return false;
                }
            }

            return true;
        }

        private bool IsMatchGender(PKM pkm)
        {
            if (Gender == -1 || Gender == pkm.Gender)
                return true;

            if (Species == (int) Core.Species.Azurill && Generation == 4 && Location == 233 && pkm.Gender == 0)
                return PKX.GetGenderFromPIDAndRatio(pkm.PID, 0xBF) == 1;

            return false;
        }

        protected virtual bool IsMatchLocation(PKM pkm)
        {
            if (EggEncounter)
                return true;
            if (Location == 0)
                return true;
            if (!pkm.HasOriginalMetLocation)
                return true;
            return Location == pkm.Met_Location;
        }

        protected virtual bool IsMatchLevel(PKM pkm, int lvl)
        {
            if (!pkm.HasOriginalMetLocation)
                return lvl >= Level;

            if (lvl == Level)
                return true;
            if (!(pkm.Format == 3 && EggEncounter && lvl == 0))
                return false;

            return true;
        }

        public virtual bool IsMatchDeferred(PKM pkm)
        {
            if (pkm.FatefulEncounter != Fateful)
                return true;
            if (Ability == 4 && pkm.AbilityNumber != 4) // BW/2 Jellicent collision with wild surf slot, resolved by duplicating the encounter with any abil
                return true;
            return false;
        }
    }
}
