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
        public override int[] Moves { get; set; }
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

                Language = Language < 0 ? SAV.Language : Language,
                FatefulEncounter = Fateful,
            };
            if (IsEgg)
            {
                pk.IsEgg = true;
                pk.IsNicknamed = true;
            }

            bool hatchedEgg = IsEgg && SAV.Generation != 3;
            if (hatchedEgg) // ugly workaround for character table interactions
            {
                pk.IsEgg = false;
                pk.Language = SAV.Language;
                pk.OT_Name = "PKHeX";
                pk.OT_Gender = SAV.Gender;
                pk.TID = SAV.TID;
                pk.SID = SAV.SID;
                pk.OT_Friendship = pi.BaseFriendship;
                pk.IsEgg = true;
            }
            else
            {
                pk.OT_Name = OT_Name ?? SAV.OT;
                pk.OT_Gender = OT_Gender != 3 ? OT_Gender & 1 : SAV.Gender;
                pk.TID = TID;
                pk.SID = SID;
                pk.OT_Friendship = IsEgg ? pi.HatchCycles : pi.BaseFriendship;
            }
            pk.Nickname = PKX.GetSpeciesNameGeneration(Species, pk.Language, 3); // will be set to Egg nickname if appropriate by PK3 setter

            if (Version == 0)
            {
                if (SAV.Game > 15) // above CXD
                    pk.Version = (int) GameVersion.R;
                else
                    pk.Version = SAV.Game;
            }
            else
            {
                if (Version < 100) // single game
                    pk.Version = Version;
                else
                {
                    int rand = Util.Rand.Next(1);
                    switch (Version)
                    {
                        case (int) GameVersion.FRLG:
                            pk.Version = (int)GameVersion.FR + rand; // or LG
                            break;
                        case (int)GameVersion.RS:
                            pk.Version = (int)GameVersion.R + rand; // or S
                            break;
                        default:
                            throw new Exception($"Unknown GameVersion: {Version}");
                    }
                }
            }

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

            var moves = Moves ?? Legal.GetBaseEggMoves(pk, Species, (GameVersion)pk.Version, Level);
            if (moves.Length != 4)
                Array.Resize(ref moves, 4);
            pk.Moves = moves;
            pk.HeldItem = 0; // clear, only random for Jirachis(?), no loss
            pk.RefreshChecksum();
            return pk;
        }

        public bool RibbonEarth { get; set; }
        public bool RibbonNational { get; set; }
        public bool RibbonCountry { get; set; }
        public bool RibbonChampionBattle { get; set; }
        public bool RibbonChampionRegional { get; set; }
        public bool RibbonChampionNational { get; set; }
    }
}
