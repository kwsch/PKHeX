using System;

namespace PKHeX.Core
{
    public class WC3 : MysteryGift, IRibbonSetEvent3
    {
        // Template Properties

        /// <summary>
        /// Matched <see cref="PIDIV"/> Type
        /// </summary>
        public PIDType Method;

        public string OT_Name { get; set; }
        public int OT_Gender { get; set; } = 3;
        public int TID { get; set; }
        public int SID { get; set; }
        public int Met_Location { get; internal set; } = 255;
        public int Version { get; set; }
        public int Language { get; set; } = -1;
        public override int Species { get; set; }
        public override bool IsEgg { get; set; }
        public override int[] Moves { get; set; } = new int[0];
        public bool NotDistributed { get; set; }
        public bool? Shiny { get; set; } // null = allow, false = never, true = always
        public bool Fateful { get; set; } // Obedience Flag

        // Mystery Gift Properties
        public override int Format => 3;
        public override int Level { get; set; }
        public override int Ball { get; set; } = 4;

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

        // Synthetic
        private int? _metLevel;
        public int Met_Level
        {
            get => _metLevel ?? (IsEgg ? 0 : Level);
            set => _metLevel = value;
        }

        public override PKM ConvertToPKM(SaveFile SAV)
        {
            var pi = SAV.Personal.GetFormeEntry(Species, 0);
            PK3 pk = new PK3
            {
                Species = Species,
                Met_Level = Met_Level,
                Met_Location = Met_Location,
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

            if (Version == 0)
            {
                bool gen3 = SAV.Game <= 15 && GameVersion.Gen3.Contains((GameVersion)SAV.Game);
                pk.Version = gen3 ? SAV.Game : (int)GameVersion.R;
            }
            else
            {
                pk.Version = GetRandomVersion(Version);
            }
            int lang = GetSafeLanguage(SAV.Language, Language);
            bool hatchedEgg = IsEgg && SAV.Generation != 3;
            if (hatchedEgg) // ugly workaround for character table interactions
            {
                pk.Language = 2;
                pk.OT_Name = "PKHeX";
                pk.OT_Gender = SAV.Gender;
                pk.TID = SAV.TID;
                pk.SID = SAV.SID;
                pk.OT_Friendship = pi.BaseFriendship;
                pk.Met_Location = 32;
            }
            else
            {
                if (IsEgg)
                {
                    pk.IsEgg = true;
                    pk.IsNicknamed = true;
                    pk.Language = 1; // JPN
                    if (SID >= 0)
                        pk.SID = SID;
                    if (TID >= 0)
                        pk.SID = TID;
                }
                else
                    pk.Language = lang;

                pk.OT_Name = OT_Name ?? SAV.OT;
                if (string.IsNullOrWhiteSpace(pk.OT_Name))
                {
                    // Try again (only happens for eggs)
                    pk.IsEgg = false;
                    pk.Language = 2;
                    pk.OT_Name = SAV.OT;
                    pk.Language = 1;
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

            if (Moves == null) // not completely defined
                Moves = Legal.GetBaseEggMoves(pk, Species, (GameVersion)pk.Version, Level);
            if (Moves.Length != 4)
            {
                var moves = Moves;
                Array.Resize(ref moves, 4);
                Moves = moves;
            }

            pk.Moves = Moves;
            pk.Move1_PP = pk.GetMovePP(Moves[0], 0);
            pk.Move2_PP = pk.GetMovePP(Moves[1], 0);
            pk.Move3_PP = pk.GetMovePP(Moves[2], 0);
            pk.Move4_PP = pk.GetMovePP(Moves[3], 0);
            pk.HeldItem = 0; // clear, only random for Jirachis(?), no loss
            pk.RefreshChecksum();
            return pk;
        }

        private static int GetSafeLanguage(int hatchLang, int supplied)
        {
            if (supplied >= 1)
                return supplied;
            if (hatchLang < 0)
                return 2;
            return hatchLang;
        }
        private static int GetRandomVersion(int version)
        {
            if (version <= 15 && version > 0) // single game
                return version;

            int rand = Util.Rand.Next(1);
            switch (version)
            {
                case (int)GameVersion.FRLG:
                    return (int)GameVersion.FR + rand; // or LG
                case (int)GameVersion.RS:
                    return (int)GameVersion.R + rand; // or S
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
