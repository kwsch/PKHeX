using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 3 Mystery Gift Template File
    /// </summary>
    /// <remarks>
    /// This is fabricated data built to emulate the future generation Mystery Gift objects.
    /// Data here is not stored in any save file and cannot be naturally exported.
    /// </remarks>
    public class WC3 : MysteryGift, IRibbonSetEvent3, IVersion
    {
        // Template Properties

        /// <summary>
        /// Matched <see cref="PIDIV"/> Type
        /// </summary>
        public PIDType Method;

        public override string OT_Name { get; set; }
        public int OT_Gender { get; set; } = 3;
        public override int TID { get; set; }
        public override int SID { get; set; }
        public override int Location { get; set; } = 255;
        public override int EggLocation { get => 0; set {} }
        public GameVersion Version { get; set; }
        public int Language { get; set; } = -1;
        public override int Species { get; set; }
        public override bool IsEgg { get; set; }
        public override int[] Moves { get; set; } = Array.Empty<int>();
        public bool NotDistributed { get; set; }
        public Shiny Shiny { get; set; } = Shiny.Random;
        public bool Fateful { get; set; } // Obedience Flag

        // Mystery Gift Properties
        public override int Format => 3;
        public override int Level { get; set; }
        public override int Ball { get; set; } = 4;
        public override bool IsShiny => Shiny == Shiny.Always;

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

        public override PKM ConvertToPKM(ITrainerInfo SAV)
        {
            PK3 pk = new PK3
            {
                Species = Species,
                Met_Level = Met_Level,
                Met_Location = Location,
                Ball = 4,

                EXP = PKX.GetEXP(Level, Species),

                // Ribbons
                RibbonCountry = RibbonCountry,
                RibbonNational = RibbonNational,
                RibbonEarth = RibbonEarth,
                RibbonChampionBattle = RibbonChampionBattle,
                RibbonChampionRegional = RibbonChampionRegional,
                RibbonChampionNational = RibbonChampionNational,

                FatefulEncounter = Fateful,
            };
            var pi = pk.PersonalInfo;

            if (Version == 0)
            {
                bool gen3 = SAV.Game <= 15 && GameVersion.Gen3.Contains((GameVersion)SAV.Game);
                pk.Version = gen3 ? SAV.Game : (int)GameVersion.R;
            }
            else
            {
                pk.Version = (int)GetRandomVersion(Version);
            }
            int lang = GetSafeLanguage(SAV.Language, Language);
            bool hatchedEgg = IsEgg && SAV.Generation != 3;
            if (hatchedEgg) // ugly workaround for character table interactions
            {
                pk.Language = (int)LanguageID.English;
                pk.OT_Name = "PKHeX";
                pk.OT_Gender = SAV.Gender;
                pk.TID = SAV.TID;
                pk.SID = SAV.SID;
                pk.OT_Friendship = pi.BaseFriendship;
                pk.Met_Location = pk.FRLG ? 146 /* Four Island */ : 32; // Route 117
                pk.FatefulEncounter &= pk.FRLG; // clear flag for RSE
                pk.Met_Level = 0; // hatched
            }
            else
            {
                if (IsEgg)
                {
                    pk.IsEgg = true;
                    pk.IsNicknamed = true;
                    pk.Language = (int)LanguageID.Japanese; // JPN
                    if (SID >= 0)
                        pk.SID = SID;
                    if (TID >= 0)
                        pk.SID = TID;
                }
                else
                {
                    pk.Language = lang;
                }

                pk.OT_Name = OT_Name ?? SAV.OT;
                if (string.IsNullOrWhiteSpace(pk.OT_Name))
                {
                    // Try again (only happens for eggs)
                    pk.IsEgg = false;
                    pk.Language = (int)LanguageID.English;
                    pk.OT_Name = SAV.OT;
                    pk.Language = (int)LanguageID.Japanese; // as egg is Japanese until hatched
                    pk.IsEgg = true;
                }
                pk.OT_Gender = OT_Gender != 3 ? OT_Gender & 1 : SAV.Gender;
                pk.TID = TID;
                pk.SID = SID;
                pk.OT_Friendship = IsEgg ? pi.HatchCycles : pi.BaseFriendship;
            }
            pk.Nickname = PKX.GetSpeciesNameGeneration(Species, pk.Language, 3); // will be set to Egg nickname if appropriate by PK3 setter

            // Generate PIDIV
            var seed = Util.Rand32();
            switch (Method)
            {
                case PIDType.BACD_R:
                    seed &= 0xFFFF;
                    break;
                case PIDType.BACD_R_S:
                    seed &= 0xFF;
                    break;
            }
            PIDGenerator.SetValuesFromSeed(pk, Method, seed);

            if (Version == GameVersion.XD)
                pk.FatefulEncounter = true; // pk3 is already converted from xk3

            if (Moves == null || Moves.Length == 0) // not completely defined
                Moves = Legal.GetBaseEggMoves(pk, Species, (GameVersion)pk.Version, Level);
            if (Moves.Length != 4)
            {
                var moves = Moves;
                Array.Resize(ref moves, 4);
                Moves = moves;
            }

            pk.Moves = Moves;
            pk.SetMaximumPPCurrent(Moves);
            pk.HeldItem = 0; // clear, only random for Jirachis(?), no loss
            pk.RefreshChecksum();
            return pk;
        }

        private static int GetSafeLanguage(int hatchLang, int supplied)
        {
            if (supplied >= 1)
                return supplied;
            if (hatchLang < 0 || hatchLang > 8) // ko
                return 2;
            return hatchLang;
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

        public bool RibbonEarth { get; set; }
        public bool RibbonNational { get; set; }
        public bool RibbonCountry { get; set; }
        public bool RibbonChampionBattle { get; set; }
        public bool RibbonChampionRegional { get; set; }
        public bool RibbonChampionNational { get; set; }
    }
}
