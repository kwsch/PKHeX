namespace PKHeX.Core
{
    /// <summary>
    /// Stat/misc data for individual species or their associated alternate forme data.
    /// </summary>
    public abstract class PersonalInfo
    {
        protected byte[] Data;
        public abstract byte[] Write();
        public abstract int HP { get; set; }
        public abstract int ATK { get; set; }
        public abstract int DEF { get; set; }
        public abstract int SPE { get; set; }
        public abstract int SPA { get; set; }
        public abstract int SPD { get; set; }

        public int[] Stats => new[] { HP, ATK, DEF, SPE, SPA, SPD };

        public abstract int EV_HP { get; set; }
        public abstract int EV_ATK { get; set; }
        public abstract int EV_DEF { get; set; }
        public abstract int EV_SPE { get; set; }
        public abstract int EV_SPA { get; set; }
        public abstract int EV_SPD { get; set; }
        public abstract int Type1 { get; set; }
        public abstract int Type2 { get; set; }
        public abstract int EggGroup1 { get; set; }
        public abstract int EggGroup2 { get; set; }
        public abstract int CatchRate { get; set; }
        public virtual int EvoStage { get; set; }
        public abstract int[] Items { get; set; }
        public abstract int Gender { get; set; }
        public abstract int HatchCycles { get; set; }
        public abstract int BaseFriendship { get; set; }
        public abstract int EXPGrowth { get; set; }
        public abstract int [] Abilities { get; set; }
        public abstract int EscapeRate { get; set; }
        public virtual int FormeCount { get; set; } = 1;
        protected internal virtual int FormStatsIndex { get; set; }
        public virtual int FormeSprite { get; set; }
        public abstract int BaseEXP { get; set; }
        public abstract int Color { get; set; }

        public virtual int Height { get; set; } = 0;
        public virtual int Weight { get; set; } = 0;

        public int[] Types
        {
            get => new[] { Type1, Type2 };
            set
            {
                if (value?.Length != 2) return;
                Type1 = value[0];
                Type2 = value[1];
            }
        }
        public int[] EggGroups
        {
            get => new[] { EggGroup1, EggGroup2 };
            set
            {
                if (value?.Length != 2) return;
                EggGroup1 = (byte)value[0];
                EggGroup2 = (byte)value[1];
            }
        }

        /// <summary>
        /// TM/HM learn compatibility flags for individual moves.
        /// </summary>
        public bool[] TMHM { get; protected set; }
        /// <summary>
        /// Grass-Fire-Water-Etc typed learn compatibility flags for individual moves.
        /// </summary>
        public bool[] TypeTutors { get; protected set; }
        /// <summary>
        /// Special tutor learn compatibility flags for individual moves.
        /// </summary>
        public bool[][] SpecialTutors { get; protected set; } = new bool[0][];

        protected static bool[] GetBits(byte[] data)
        {
            bool[] r = new bool[data.Length<<3];
            for (int i = 0; i < r.Length; i++)
                r[i] = (data[i>>3] >> (i&7) & 0x1) == 1;
            return r;
        }
        protected static byte[] SetBits(bool[] bits)
        {
            byte[] data = new byte[bits.Length>>3];
            for (int i = 0; i < bits.Length; i++)
                data[i>>3] |= (byte)(bits[i] ? 1 << (i&0x7) : 0);
            return data;
        }

        /// <summary>
        /// Injects supplementary TM/HM compatibility which is not present in the generation specific <see cref="PersonalInfo"/> format.
        /// </summary>
        /// <param name="data"></param>
        internal void AddTMHM(byte[] data) => TMHM = GetBits(data);
        /// <summary>
        /// Injects supplementary Type Tutor compatibility which is not present in the generation specific <see cref="PersonalInfo"/> format.
        /// </summary>
        internal void AddTypeTutors(byte[] data) => TypeTutors = GetBits(data);

        /// <summary>
        /// Gets the <see cref="PersonalTable"/> <see cref="PKM.AltForm"/> entry index for the input criteria, with fallback for the original species entry.
        /// </summary>
        /// <param name="species"><see cref="PKM.Species"/> to retrieve for</param>
        /// <param name="forme"><see cref="PKM.AltForm"/> to retrieve for</param>
        /// <returns>Index the <see cref="PKM.AltForm"/> exists as in the <see cref="PersonalTable"/>.</returns>
        public int FormeIndex(int species, int forme)
        {
            if (forme <= 0) // no forme requested
                return species;
            if (FormStatsIndex <= 0) // no formes present
                return species;
            if (forme >= FormeCount) // beyond range of species' formes
                return species;

            return FormStatsIndex + forme - 1;
        }
        public int RandomGender
        {
            get
            {
                switch (Gender)
                {
                    case 255: // Genderless
                        return 2;
                    case 254: // Female
                        return 1;
                    case 0: // Male
                        return 0;
                    default:
                        return (int)(Util.Rand32() & 1);
                }
            }
        }
        public bool HasFormes => FormeCount > 1;
        public int BST => HP + ATK + DEF + SPE + SPA + SPD;

        public bool IsFormeWithinRange(int forme)
        {
            if (forme == 0)
                return true;
            return forme < FormeCount;
        }
    }
}
