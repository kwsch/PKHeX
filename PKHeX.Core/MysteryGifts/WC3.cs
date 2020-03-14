using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 3 Mystery Gift Template File
    /// </summary>
    /// <remarks>
    /// This is fabricated data built to emulate the future generation Mystery Gift objects.
    /// Data here is not stored in any save file and cannot be naturally exported.
    /// </remarks>
    public sealed class WC3 : MysteryGift, IRibbonSetEvent3, IVersion
    {
        public override MysteryGift Clone() => (WC3)MemberwiseClone();

        /// <summary>
        /// Matched <see cref="PIDIV"/> Type
        /// </summary>
        public PIDType Method;

        public override string OT_Name { get; set; } = string.Empty;
        public int OT_Gender { get; set; } = 3;
        public override int TID { get; set; }
        public override int SID { get; set; }
        public override int Location { get; set; } = 255;
        public override int EggLocation { get => 0; set {} }
        public GameVersion Version { get; set; }
        public int Language { get; set; } = -1;
        public override int Species { get; set; }
        public override bool IsEgg { get; set; }
        public override IReadOnlyList<int> Moves { get; set; } = Array.Empty<int>();
        public bool NotDistributed { get; set; }
        public Shiny Shiny { get; set; } = Shiny.Random;
        public bool Fateful { get; set; } // Obedience Flag

        // Mystery Gift Properties
        public override int Format => 3;
        public override int Level { get; set; }
        public override int Ball { get; set; } = 4;
        public override bool IsShiny => Shiny == Shiny.Always;
        public bool RibbonEarth { get; set; }
        public bool RibbonNational { get; set; }
        public bool RibbonCountry { get; set; }
        public bool RibbonChampionBattle { get; set; }
        public bool RibbonChampionRegional { get; set; }
        public bool RibbonChampionNational { get; set; }

        // Description
        public override string CardTitle { get; set; } = "Generation 3 Event";
        public override string CardHeader => CardTitle;

        // Unused
        public override bool GiftUsed { get; set; }
        public override int CardID { get; set; }
        public override bool IsItem { get; set; }
        public override int ItemID { get; set; }
        public override bool IsPokémon { get; set; } = true;
        public override bool Empty => false;
        public override int Gender { get; set; }
        public override int Form { get; set; }

        // Synthetic
        private int? _metLevel;

        public int Met_Level
        {
            get => _metLevel ?? (IsEgg ? 0 : Level);
            set => _metLevel = value;
        }

        public override PKM ConvertToPKM(ITrainerInfo SAV, EncounterCriteria criteria)
        {
            PK3 pk = new PK3
            {
                Species = Species,
                Met_Level = Met_Level,
                Met_Location = Location,
                Ball = 4,

                // Ribbons
                RibbonCountry = RibbonCountry,
                RibbonNational = RibbonNational,
                RibbonEarth = RibbonEarth,
                RibbonChampionBattle = RibbonChampionBattle,
                RibbonChampionRegional = RibbonChampionRegional,
                RibbonChampionNational = RibbonChampionNational,

                FatefulEncounter = Fateful,
                Version = GetVersion(SAV),
            };
            pk.EXP = Experience.GetEXP(Level, pk.PersonalInfo.EXPGrowth);
            SetMoves(pk);

            bool hatchedEgg = IsEgg && SAV.Generation != 3;
            if (hatchedEgg)
            {
                SetForceHatchDetails(pk, SAV);
            }
            else
            {
                pk.OT_Gender = OT_Gender != 3 ? OT_Gender & 1 : SAV.Gender;
                pk.TID = TID;
                pk.SID = SID;

                pk.Language = (int)GetSafeLanguage((LanguageID)SAV.Language);
                pk.OT_Name = !string.IsNullOrWhiteSpace(OT_Name) ? OT_Name : SAV.OT;
                if (IsEgg)
                    pk.IsEgg = true; // lang should be set to japanese already
            }
            pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, pk.Language, 3); // will be set to Egg nickname if appropriate by PK3 setter

            var pi = pk.PersonalInfo;
            pk.OT_Friendship = pk.IsEgg ? pi.HatchCycles : pi.BaseFriendship;

            // Generate PIDIV
            SetPINGA(pk, criteria);
            pk.HeldItem = 0; // clear, only random for Jirachis(?), no loss

            if (Version == GameVersion.XD)
                pk.FatefulEncounter = true; // pk3 is already converted from xk3

            pk.RefreshChecksum();
            return pk;
        }

        private static void SetForceHatchDetails(PK3 pk, ITrainerInfo SAV)
        {
            // ugly workaround for character table interactions
            pk.Language = (int)LanguageID.English;
            pk.OT_Name = "PKHeX";
            pk.OT_Gender = SAV.Gender;
            pk.TID = SAV.TID;
            pk.SID = SAV.SID;
            pk.Met_Location = pk.FRLG ? 146 /* Four Island */ : 32; // Route 117
            pk.FatefulEncounter &= pk.FRLG; // clear flag for RSE
            pk.Met_Level = 0; // hatched
        }

        private int GetVersion(ITrainerInfo SAV)
        {
            if (Version != 0)
                return (int) GetRandomVersion(Version);
            bool gen3 = SAV.Game <= 15 && GameVersion.Gen3.Contains((GameVersion)SAV.Game);
            return gen3 ? SAV.Game : (int)GameVersion.R;
        }

        private void SetMoves(PK3 pk)
        {
            if (Moves.Count == 0) // not completely defined
                Moves = Legal.GetBaseEggMoves(pk, Species, Form, (GameVersion)pk.Version, Level);
            if (Moves.Count != 4)
            {
                var moves = Moves.ToArray();
                Array.Resize(ref moves, 4);
                Moves = moves;
            }

            pk.SetMoves(Moves);
            pk.SetMaximumPPCurrent(Moves);
        }

        private void SetPINGA(PK3 pk, EncounterCriteria criteria)
        {
            var seed = Util.Rand32();
            seed = GetSaneSeed(seed);
            PIDGenerator.SetValuesFromSeed(pk, Method, seed);
        }

        private uint GetSaneSeed(uint seed)
        {
            return Method switch
            {
                PIDType.BACD_R => (seed & 0x0000FFFF),
                PIDType.BACD_R_S => (seed & 0x000000FF),
                _ => seed
            };
        }

        private LanguageID GetSafeLanguage(LanguageID hatchLang)
        {
            if (IsEgg)
                return LanguageID.Japanese;
            if (Language != -1)
                return (LanguageID)Language;
            if (hatchLang < LanguageID.Korean && hatchLang != LanguageID.Hacked)
                return hatchLang;
            return LanguageID.English; // fallback
        }

        private static GameVersion GetRandomVersion(GameVersion version)
        {
            if (version <= GameVersion.CXD && version > GameVersion.Unknown) // single game
                return version;

            int rand = Util.Rand.Next(2); // 0 or 1
            switch (version)
            {
                case GameVersion.FRLG:
                    return GameVersion.FR + rand; // or LG
                case GameVersion.RS:
                    return GameVersion.S + rand; // or R

                case GameVersion.COLO:
                case GameVersion.XD:
                    return GameVersion.CXD;
                default:
                    throw new Exception($"Unknown GameVersion: {version}");
            }
        }

        protected override bool IsMatchExact(PKM pkm)
        {
            // Gen3 Version MUST match.
            if (Version != 0 && !(Version).Contains((GameVersion)pkm.Version))
                return false;

            bool hatchedEgg = IsEgg && !pkm.IsEgg;
            if (!hatchedEgg)
            {
                if (SID != -1 && SID != pkm.SID) return false;
                if (TID != -1 && TID != pkm.TID) return false;
                if (OT_Gender < 3 && OT_Gender != pkm.OT_Gender) return false;
                var wcOT = OT_Name;
                if (!string.IsNullOrEmpty(wcOT))
                {
                    if (wcOT.Length > 7) // Colosseum Mattle Ho-Oh
                    {
                        if (!GetIsValidOTMattleHoOh(wcOT, pkm.OT_Name, pkm is CK3))
                            return false;
                    }
                    else if (wcOT != pkm.OT_Name)
                    {
                        return false;
                    }
                }
            }

            if (Language != -1 && Language != pkm.Language) return false;
            if (Ball != pkm.Ball) return false;
            if (Fateful != pkm.FatefulEncounter)
            {
                // XD Gifts only at level 20 get flagged after transfer
                if (Version == GameVersion.XD != pkm is XK3)
                    return false;
            }

            if (pkm.IsNative)
            {
                if (hatchedEgg)
                    return true; // defer egg specific checks to later.
                if (Met_Level != pkm.Met_Level)
                    return false;
                if (Location != pkm.Met_Location)
                    return false;
            }
            else
            {
                if (pkm.IsEgg)
                    return false;
                if (Level > pkm.Met_Level)
                    return false;
            }
            return true;
        }

        private static bool GetIsValidOTMattleHoOh(string wc, string ot, bool ck3)
        {
            if (ck3 && ot.Length == 10)
                return wc == ot;
            return ot.Length == 7 && wc.StartsWith(ot);
        }

        protected override bool IsMatchDeferred(PKM pkm)
        {
            return Species != pkm.Species;
        }
    }
}
