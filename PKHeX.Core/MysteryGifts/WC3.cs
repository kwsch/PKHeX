using System;

namespace PKHeX.Core
{
    public class WC3 : MysteryGift
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
        public override bool IsPokémon { get; set; }

        // Synthetic
        private int _metLevel = -1;
        public int Met_Level
        {
            get
            {
                if (IsEgg)
                    return 0;
                return _metLevel < 0 ? Level : _metLevel;
            }
            set => _metLevel = value;
        }

        public override PKM ConvertToPKM(SaveFile SAV)
        {
            throw new NotImplementedException();
        }
    }
}
