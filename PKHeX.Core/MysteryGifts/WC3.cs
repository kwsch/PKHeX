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

        public string OT_Name;
        public int OT_Gender = 3;
        public int TID = -1;
        public int SID = -1;
        public int Met_Location = 255;
        public int Version;
        public int Language;
        public override int Species { get; set; }
        public override bool IsEgg { get; set; }
        public override int[] Moves { get; set; }
        public bool NotDistributed = false;
        public bool? Shiny = null; // null = allow, false = never, true = always

        // Mystery Gift Properties
        public override int Format => 3;
        public override int Level { get; set; }
        public override int Ball { get; set; } = 4;

        // Description
        public override string CardTitle { get; set; } = "Generation 3 Event";
        public override string getCardHeader() => CardTitle;

        // Unused
        public override bool GiftUsed { get; set; }
        public override int CardID { get; set; }
        public override bool IsItem { get; set; }
        public override int Item { get; set; }
        public override bool IsPokémon { get; set; }

        public override PKM convertToPKM(SaveFile SAV)
        {
            throw new NotImplementedException();
        }
    }
}
