using System;
using System.Drawing;
using System.Linq;

namespace PKHeX
{
    public abstract class PKM
    {
        public abstract int SIZE_PARTY { get; }
        public abstract int SIZE_STORED { get; }
        public virtual string Extension => "pk" + Format;
        public abstract PersonalInfo PersonalInfo { get; }

        // Internal Attributes set on creation
        public byte[] Data; // Raw Storage
        public string Identifier; // User or Form Custom Attribute
        public int Box { get; set; } = -1; // Batch Editor
        public int Slot { get; set; } = -1; // Batch Editor

        public virtual byte[] EncryptedPartyData => Encrypt().Take(SIZE_PARTY).ToArray();
        public virtual byte[] EncryptedBoxData => Encrypt().Take(SIZE_STORED).ToArray();
        public virtual byte[] DecryptedPartyData => Write().Take(SIZE_PARTY).ToArray();
        public virtual byte[] DecryptedBoxData => Write().Take(SIZE_STORED).ToArray();
        public virtual bool Valid { get { return ChecksumValid && Sanity == 0; } set { if (!value) return; Sanity = 0; RefreshChecksum(); } }

        protected virtual ushort CalculateChecksum()
        {
            ushort chk = 0;
            switch (Format)
            {
                case 3:
                    for (int i = 32; i < SIZE_STORED; i += 2)
                        chk += BitConverter.ToUInt16(Data, i);
                    return chk;
                default: // 4+
                    for (int i = 8; i < SIZE_STORED; i += 2)
                        chk += BitConverter.ToUInt16(Data, i);
                    return chk;
            }
        }
        public abstract byte[] Encrypt();
        public abstract int Format { get; }
        public byte[] Write()
        {
            RefreshChecksum();
            return Data;
        }

        // Surface Properties
        public abstract int Species { get; set; }
        public abstract string Nickname { get; set; }
        public abstract int HeldItem { get; set; }
        public abstract int Gender { get; set; }
        public abstract int Nature { get; set; }
        public abstract int Ability { get; set; }
        public abstract int CurrentFriendship { get; set; }
        public abstract int AltForm { get; set; }
        public abstract bool IsEgg { get; set; }
        public abstract bool IsNicknamed { get; set; }
        public abstract uint EXP { get; set; }
        public abstract int TID { get; set; }
        public abstract string OT_Name { get; set; }
        public abstract int OT_Gender { get; set; }
        public abstract int Ball { get; set; }
        public abstract int Met_Level { get; set; }

        // Battle
        public abstract int Move1 { get; set; }
        public abstract int Move2 { get; set; }
        public abstract int Move3 { get; set; }
        public abstract int Move4 { get; set; }
        public abstract int Move1_PP { get; set; }
        public abstract int Move2_PP { get; set; }
        public abstract int Move3_PP { get; set; }
        public abstract int Move4_PP { get; set; }
        public abstract int Move1_PPUps { get; set; }
        public abstract int Move2_PPUps { get; set; }
        public abstract int Move3_PPUps { get; set; }
        public abstract int Move4_PPUps { get; set; }
        public abstract int EV_HP { get; set; }
        public abstract int EV_ATK { get; set; }
        public abstract int EV_DEF { get; set; }
        public abstract int EV_SPE { get; set; }
        public abstract int EV_SPA { get; set; }
        public abstract int EV_SPD { get; set; }
        public abstract int IV_HP { get; set; }
        public abstract int IV_ATK { get; set; }
        public abstract int IV_DEF { get; set; }
        public abstract int IV_SPE { get; set; }
        public abstract int IV_SPA { get; set; }
        public abstract int IV_SPD { get; set; }
        public abstract int Stat_Level { get; set; }
        public abstract int Stat_HPMax { get; set; }
        public abstract int Stat_HPCurrent { get; set; }
        public abstract int Stat_ATK { get; set; }
        public abstract int Stat_DEF { get; set; }
        public abstract int Stat_SPE { get; set; }
        public abstract int Stat_SPA { get; set; }
        public abstract int Stat_SPD { get; set; }

        // Hidden Properties
        public abstract int Version { get; set; }
        public abstract int SID { get; set; }
        public abstract int PKRS_Strain { get; set; }
        public abstract int PKRS_Days { get; set; }
        public abstract int CNT_Cool { get; set; }
        public abstract int CNT_Beauty { get; set; }
        public abstract int CNT_Cute { get; set; }
        public abstract int CNT_Smart { get; set; }
        public abstract int CNT_Tough { get; set; }
        public abstract int CNT_Sheen { get; set; }

        public abstract uint EncryptionConstant { get; set; }
        public abstract uint PID { get; set; }
        public abstract ushort Sanity { get; set; }
        public abstract ushort Checksum { get; set; }

        // Misc Properties
        public abstract int Language { get; set; }
        public abstract bool FatefulEncounter { get; set; }
        public abstract int TSV { get; }
        public abstract int PSV { get; }
        public abstract int Characteristic { get; }
        public abstract int MarkValue { get; protected set; }
        public abstract int Met_Location { get; set; }
        public abstract int Egg_Location { get; set; }
        public abstract int OT_Friendship { get; set; }

        // Future Properties
        protected virtual int Met_Year { get { return 0; } set { } }
        protected virtual int Met_Month { get { return 0; } set { } }
        protected virtual int Met_Day { get { return 0; } set { } }
        public virtual string HT_Name { get; set; }
        public virtual int HT_Affection { get; set; }
        public virtual int HT_Friendship { get; set; }
        public virtual int HT_Memory { get; set; }
        public virtual int HT_TextVar { get; set; }
        public virtual int HT_Feeling { get; set; }
        public virtual int HT_Intensity { get; set; }
        public virtual int OT_Memory { get; set; }
        public virtual int OT_TextVar { get; set; }
        public virtual int OT_Feeling { get; set; }
        public virtual int OT_Intensity { get; set; }
        public virtual int Geo1_Region { get; set; }
        public virtual int Geo2_Region { get; set; }
        public virtual int Geo3_Region { get; set; }
        public virtual int Geo4_Region { get; set; }
        public virtual int Geo5_Region { get; set; }
        public virtual int Geo1_Country { get; set; }
        public virtual int Geo2_Country { get; set; }
        public virtual int Geo3_Country { get; set; }
        public virtual int Geo4_Country { get; set; }
        public virtual int Geo5_Country { get; set; }
        public virtual byte Enjoyment { get; set; }
        public virtual byte Fullness { get; set; }
        public virtual int AbilityNumber { get; set; }
        public virtual int Country { get; set; }
        public virtual int Region { get; set; }
        public virtual int ConsoleRegion { get; set; }

        /// <summary>
        /// The date the Pokémon was met.
        /// </summary>
        /// <returns>A DateTime representing the date the Pokémon was met, or null if either the <see cref="PKM"/> format does not support dates or the stored date is invalid.</returns>
        /// <remarks>Not all <see cref="PKM"/> types support the <see cref="MetDate"/> property.  In these cases, this property will return null.
        /// 
        /// If null is assigned to this property, it will be cleared.</remarks>
        public virtual DateTime? MetDate
        {
            get
            {
                // Check to see if date is valid
                if (!Util.IsDateValid(2000 + Met_Year, Met_Month, Met_Day))
                {
                    return null;
                }
                else
                {
                    return new DateTime(2000 + Met_Year, Met_Month, Met_Day);
                }
            }
            set
            {
                if (value.HasValue)
                {
                    // Only update the properties if a value is provided.
                    Met_Year = value.Value.Year - 2000;
                    Met_Month = value.Value.Month;
                    Met_Day = value.Value.Day;
                }
                else
                {
                    // Clear the Met Date.
                    // If code tries to access MetDate again, null will be returned.
                    Met_Year = 0;
                    Met_Month = 0;
                    Met_Day = 0;
                }
            }
        }

        protected virtual int Egg_Year { get { return 0; } set { } }
        protected virtual int Egg_Month { get { return 0; } set { } }
        protected virtual int Egg_Day { get { return 0; } set { } }

        /// <summary>
        /// The date a Pokémon was met as an egg.
        /// </summary>
        /// <returns>A DateTime representing the date the Pokémon was met as an egg, or null if the <see cref="PKM"/> format does not support dates.</returns>
        /// <remarks>Not all <see cref="PKM"/> types support the <see cref="EggMetDate"/> property.  In these cases, this property will return null.
        /// 
        /// If null is assigned to this property, it will be cleared.</remarks>
        public virtual DateTime? EggMetDate
        {
            get
            {
                // Check to see if date is valid
                if (!Util.IsDateValid(2000 + Egg_Year, Egg_Month, Egg_Day))
                {
                    return null;
                }
                else
                {
                    return new DateTime(2000 + Egg_Year, Egg_Month, Egg_Day);
                }
            }
            set
            {
                if (value.HasValue)
                {
                    // Only update the properties if a value is provided.
                    Egg_Year = value.Value.Year - 2000;
                    Egg_Month = value.Value.Month;
                    Egg_Day = value.Value.Day;
                }
                else
                {
                    // Clear the Met Date.
                    // If code tries to access MetDate again, null will be returned.
                    Egg_Year = 0;
                    Egg_Month = 0;
                    Egg_Day = 0;
                }
            }
        }

        public virtual int OT_Affection { get { return 0; } set { } }
        public virtual int RelearnMove1 { get { return 0; } set { } }
        public virtual int RelearnMove2 { get { return 0; } set { } }
        public virtual int RelearnMove3 { get { return 0; } set { } }
        public virtual int RelearnMove4 { get { return 0; } set { } }
        public virtual int EncounterType { get { return 0; } set { } }

        // Exposed but not Present in all
        public abstract int CurrentHandler { get; set; }

        // Derived
        public virtual int SpriteItem => HeldItem;
        public virtual bool IsShiny => TSV == PSV;
        public int TrainerID7 => (int)((uint)(TID | (SID << 16)) % 1000000);
        public bool Gen7 => Version >= 30 && Version <= 31;
        public bool Gen6 => Version >= 24 && Version <= 29;
        public bool XY => Version == (int)GameVersion.X || Version == (int)GameVersion.Y;
        public bool AO => Version == (int)GameVersion.AS || Version == (int)GameVersion.OR;
        public bool SM => Version == (int)GameVersion.SN || Version == (int)GameVersion.MN;
        protected bool PtHGSS => GameVersion.Pt == (GameVersion)Version || HGSS;
        public bool HGSS => new[] {GameVersion.HG, GameVersion.SS}.Contains((GameVersion)Version);
        public bool Gen5 => Version >= 20 && Version <= 23;
        public bool Gen4 => Version >= 7 && Version <= 12 && Version != 9;
        public bool Gen3 => Version >= 1 && Version <= 5 || Version == 15;
        public bool GenU => !(Gen6 || Gen5 || Gen4 || Gen3);
        public int GenNumber
        {
            get
            {
                if (Gen7) return 7;
                if (Gen6) return 6;
                if (Gen5) return 5;
                if (Gen4) return 4;
                if (Gen3) return 3;
                return -1;
            } 
        }
        public bool PKRS_Infected => PKRS_Strain > 0;
        public bool PKRS_Cured => PKRS_Days == 0 && PKRS_Strain > 0;
        public virtual bool ChecksumValid => Checksum == CalculateChecksum();
        public int CurrentLevel => PKX.getLevel(Species, EXP);
        public bool MarkCircle      { get { return (MarkValue & (1 << 0)) == 1 << 0; } set { MarkValue = (byte)(MarkValue & ~(1 << 0) | (value ? 1 << 0 : 0)); } }
        public bool MarkTriangle    { get { return (MarkValue & (1 << 1)) == 1 << 1; } set { MarkValue = (byte)(MarkValue & ~(1 << 0) | (value ? 1 << 0 : 0)); } }
        public bool MarkSquare      { get { return (MarkValue & (1 << 2)) == 1 << 2; } set { MarkValue = (byte)(MarkValue & ~(1 << 0) | (value ? 1 << 0 : 0)); } }
        public bool MarkHeart       { get { return (MarkValue & (1 << 3)) == 1 << 3; } set { MarkValue = (byte)(MarkValue & ~(1 << 0) | (value ? 1 << 0 : 0)); } }
        public bool MarkStar        { get { return (MarkValue & (1 << 4)) == 1 << 4; } set { MarkValue = (byte)(MarkValue & ~(1 << 0) | (value ? 1 << 0 : 0)); } }
        public bool MarkDiamond     { get { return (MarkValue & (1 << 5)) == 1 << 5; } set { MarkValue = (byte)(MarkValue & ~(1 << 0) | (value ? 1 << 0 : 0)); } }
        public Image Sprite => PKX.getSprite(this);
        public string ShowdownText => ShowdownSet.getShowdownText(this);
        public string[] QRText => PKX.getQRText(this);
        public virtual string FileName => $"{Species.ToString("000")}{(IsShiny ? " ★" : "")} - {Nickname} - {Checksum.ToString("X4")}{EncryptionConstant.ToString("X8")}.{Extension}";
        public int[] IVs
        {
            get { return new[] { IV_HP, IV_ATK, IV_DEF, IV_SPE, IV_SPA, IV_SPD }; }
            set
            {
                if (value?.Length != 6) return;
                IV_HP = value[0]; IV_ATK = value[1]; IV_DEF = value[2];
                IV_SPE = value[3]; IV_SPA = value[4]; IV_SPD = value[5];
            }
        }
        public int[] EVs
        {
            get { return new[] { EV_HP, EV_ATK, EV_DEF, EV_SPE, EV_SPA, EV_SPD }; }
            set
            {
                if (value?.Length != 6) return;
                EV_HP = value[0]; EV_ATK = value[1]; EV_DEF = value[2];
                EV_SPE = value[3]; EV_SPA = value[4]; EV_SPD = value[5];
            }
        }
        public int[] Moves
        {
            get { return new[] { Move1, Move2, Move3, Move4 }; }
            set { if (value?.Length != 4) return; Move1 = value[0]; Move2 = value[1]; Move3 = value[2]; Move4 = value[3]; }
        }
        public int[] RelearnMoves
        {
            get { return new[] { RelearnMove1, RelearnMove2, RelearnMove3, RelearnMove4 }; }
            set
            {
                if (value.Length > 0) RelearnMove1 = value[0];
                if (value.Length > 1) RelearnMove2 = value[1];
                if (value.Length > 2) RelearnMove3 = value[2];
                if (value.Length > 3) RelearnMove4 = value[3];
            }
        }
        public int PIDAbility
        {
            get
            {
                if (GenNumber > 5 || Format > 5)
                    return -1;
                if (GenNumber == 5)
                    return (int)((PID >> 16) & 1);
                return (int)(PID & 1);
            }
        }

        public virtual int[] Markings
        {
            get
            {
                int[] mark = new int[8];
                for (int i = 0; i < 8; i++)
                    mark[i] = (MarkValue >> i) & 1;
                return mark;
            }
            set
            {
                if (value.Length > 8)
                    return;
                byte b = 0;
                for (int i = 0; i < value.Length; i++)
                    b |= (byte)((value[i] & 1) << i);
                MarkValue = b;
            }
        }

        public int[] CNTs
        {
            get { return new[] { CNT_Cool, CNT_Beauty, CNT_Cute, CNT_Smart, CNT_Tough, CNT_Sheen }; }
            set { if (value?.Length != 6) return; CNT_Cool = value[0]; CNT_Beauty = value[1]; CNT_Cute = value[2]; CNT_Smart = value[3]; CNT_Tough = value[4]; CNT_Sheen = value[5]; }
        }
        public virtual int HPType
        {
            get { return 15 * ((IV_HP & 1) + 2 * (IV_ATK & 1) + 4 * (IV_DEF & 1) + 8 * (IV_SPE & 1) + 16 * (IV_SPA & 1) + 32 * (IV_SPD & 1)) / 63; }
            set
            {
                IV_HP = (IV_HP & ~1) + PKX.hpivs[value, 0];
                IV_ATK = (IV_ATK & ~1) + PKX.hpivs[value, 1];
                IV_DEF = (IV_DEF & ~1) + PKX.hpivs[value, 2];
                IV_SPE = (IV_SPE & ~1) + PKX.hpivs[value, 3];
                IV_SPA = (IV_SPA & ~1) + PKX.hpivs[value, 4];
                IV_SPD = (IV_SPD & ~1) + PKX.hpivs[value, 5];
            }
        }

        // Legality Extensions
        public virtual bool WasLink => false;
        public virtual bool WasEgg => Egg_Location > 0;
        public virtual bool WasEvent => Met_Location > 40000 && Met_Location < 50000 || FatefulEncounter;
        public virtual bool WasEventEgg => ((Egg_Location > 40000 && Egg_Location < 50000) || (FatefulEncounter && Egg_Location > 0)) && Met_Level == 1;
        public virtual bool WasTradedEgg => Egg_Location == 30002;
        public virtual bool WasIngameTrade => Met_Location == 30001;
        public virtual bool IsUntraded => string.IsNullOrWhiteSpace(HT_Name) && GenNumber == Format;
        public virtual bool IsNative => GenNumber == Format;
        public virtual bool SecretSuperTrainingUnlocked { get { return false; } set { } }
        public virtual bool SecretSuperTrainingComplete { get { return false; } set { } }

        public virtual int HyperTrainFlags { get { return 0; } set { } }
        public virtual bool HT_HP { get { return false; } set { } }
        public virtual bool HT_ATK { get { return false; } set { } }
        public virtual bool HT_DEF { get { return false; } set { } }
        public virtual bool HT_SPA { get { return false; } set { } }
        public virtual bool HT_SPD { get { return false; } set { } }
        public virtual bool HT_SPE { get { return false; } set { } }

        public bool InhabitedGeneration(int Generation)
        {
            if (Format < Generation)
                return false; // Future
            if (GenNumber > Generation)
                return false; // Past

            switch (Generation) // Sanity Check Species ID
            {
                case 1: return Species <= Legal.MaxSpeciesID_1;
                case 2: return Species <= Legal.MaxSpeciesID_2;
                case 3: return Species <= Legal.MaxSpeciesID_3;
                case 4: return Species <= Legal.MaxSpeciesID_4;
                case 5: return Species <= Legal.MaxSpeciesID_5;
                case 6: return Species <= Legal.MaxSpeciesID_6;
                case 7: return Species <= Legal.MaxSpeciesID_7;
                default:
                    return false;
            }
        }

        // Methods
        public abstract bool getGenderIsValid();
        public void RefreshChecksum() { Checksum = CalculateChecksum(); }
        public void FixMoves()
        {
            ReorderMoves();

            if (Move1 == 0) { Move1_PP = 0; Move1_PPUps = 0; }
            if (Move2 == 0) { Move2_PP = 0; Move2_PPUps = 0; }
            if (Move3 == 0) { Move3_PP = 0; Move3_PPUps = 0; }
            if (Move4 == 0) { Move4_PP = 0; Move4_PPUps = 0; }
        }

        private void ReorderMoves()
        {
            if (Move4 != 0 && Move3 == 0)
            {
                Move3 = Move4;
                Move3_PP = Move4_PP;
                Move3_PPUps = Move4_PPUps;
                Move4 = 0;
            }
            if (Move3 != 0 && Move2 == 0)
            {
                Move2 = Move3;
                Move2_PP = Move3_PP;
                Move2_PPUps = Move3_PPUps;
                Move3 = 0;
                ReorderMoves();
            }
            if (Move2 != 0 && Move1 == 0)
            {
                Move1 = Move2;
                Move1_PP = Move2_PP;
                Move1_PPUps = Move2_PPUps;
                Move2 = 0;
                ReorderMoves();
            }
        }

        public int PotentialRating
        {
            get
            {
                int ivTotal = IVs.Sum();
                if (ivTotal <= 90)
                    return 0;
                if (ivTotal <= 120)
                    return 1;
                return ivTotal <= 150 ? 2 : 3;
            }
        }

        public virtual ushort[] getStats(PersonalInfo p)
        {
            int level = CurrentLevel;
            ushort[] Stats = new ushort[6];
            Stats[0] = (ushort)(p.HP == 1 ? 1 : (((HT_HP ? 31 : IV_HP) + 2 * p.HP + EV_HP / 4 + 100) * level / 100 + 10));
            Stats[1] = (ushort)(((HT_ATK ? 31 : IV_ATK) + 2 * p.ATK + EV_ATK / 4) * level / 100 + 5);
            Stats[2] = (ushort)(((HT_DEF ? 31 : IV_DEF) + 2 * p.DEF + EV_DEF / 4) * level / 100 + 5);
            Stats[4] = (ushort)(((HT_SPA ? 31 : IV_SPA) + 2 * p.SPA + EV_SPA / 4) * level / 100 + 5);
            Stats[5] = (ushort)(((HT_SPD ? 31 : IV_SPD) + 2 * p.SPD + EV_SPD / 4) * level / 100 + 5);
            Stats[3] = (ushort)(((HT_SPE ? 31 : IV_SPE) + 2 * p.SPE + EV_SPE / 4) * level / 100 + 5);

            // Account for nature
            int incr = Nature / 5 + 1;
            int decr = Nature % 5 + 1;
            if (incr == decr) return Stats;
            Stats[incr] *= 11; Stats[incr] /= 10;
            Stats[decr] *= 9; Stats[decr] /= 10;
            return Stats;
        }
        public void setStats(ushort[] Stats)
        {
            Stat_HPMax = Stat_HPCurrent = Stats[0];
            Stat_ATK = Stats[1];
            Stat_DEF = Stats[2];
            Stat_SPE = Stats[3];
            Stat_SPA = Stats[4];
            Stat_SPD = Stats[5];
        }
        public virtual bool CanHoldItem(ushort[] ValidArray)
        {
            return ValidArray.Contains((ushort)HeldItem);
        }

        public abstract PKM Clone();

        public int getMovePP(int move, int ppup)
        {
            return getBasePP(move) * (5 + ppup) / 5;
        }

        private int getBasePP(int move)
        {
            int[] pptable;
            switch (Format)
            {
                case 1: pptable = Legal.MovePP_RBY; break;
                case 2: pptable = Legal.MovePP_GSC; break;
                case 3: pptable = Legal.MovePP_RS; break;
                case 4: pptable = Legal.MovePP_DP; break;
                case 5: pptable = Legal.MovePP_BW; break;
                case 6: pptable = Legal.MovePP_XY; break;
                case 7: pptable = Legal.MovePP_SM; break;
                default: pptable = new int[1]; break;
            }
            if (move >= pptable.Length)
                move = 0;
            return pptable[move];
        }

        public void setShinyPID()
        {
            do PID = PKX.getRandomPID(Species, Gender, Version, Nature, AltForm, PID); while (!IsShiny);
            if (GenNumber < 6)
                EncryptionConstant = PID;
        }
        public void setPIDGender(int gender)
        {
            do PID = PKX.getRandomPID(Species, gender, Version, Nature, AltForm, PID); while (IsShiny);
            if (GenNumber < 6)
                EncryptionConstant = PID;
        }
        public void setPIDNature(int nature)
        {
            do PID = PKX.getRandomPID(Species, Gender, Version, nature, AltForm, PID); while (IsShiny);
            if (GenNumber < 6)
                EncryptionConstant = PID;
        }
        public void setPIDUnown3(int form)
        {
            do PID = Util.rnd32(); while (PKX.getUnownForm(PID) != form);
        }

        // Gen3 Conversion -- do not use if format > 4
        public PKM convertToCK3()
        {
            if (Format != 3)
                return null;
            if (GetType() == typeof(CK3))
                return this;
            var pk = new CK3();
            TransferPropertiesWithReflection(this, pk);
            pk.setStats(getStats(PersonalTable.RS[pk.Species]));
            return pk;
        }
        public PKM convertToXK3()
        {
            if (Format != 3)
                return null;
            if (GetType() == typeof(XK3))
                return this;
            var pk = new XK3();
            TransferPropertiesWithReflection(this, pk);
            pk.setStats(getStats(PersonalTable.RS[pk.Species]));
            return pk;
        }
        public PKM convertToPK3()
        {
            if (Format != 3)
                return null;
            if (GetType() == typeof(PK3))
                return this;
            var pk = new PK3();
            TransferPropertiesWithReflection(this, pk);
            pk.RefreshChecksum();
            return pk;
        }

        public PKM convertPK1toPK7()
        {
            if (Format != 1)
                return null;
            if (Species > 151)
                return null;

            var pk = new PK7();
            TransferPropertiesWithReflection(this, pk);
            pk.EVs = new int[6];
            pk.Nature = IVs.Sum() % 25;
            pk.IVs = new[] {31,31,31,31,31,31};
            pk.RefreshChecksum();
            if (!IsNicknamed)
                pk.Nickname = Nickname.ToLower();
            pk.Version = -1;
            pk.Ability = PersonalTable.SM[Species].Abilities[0];
            do PID = PKX.getRandomPID(Species, Gender, Version, Nature, AltForm, PID); while (!IsShiny);
            return pk;
        }
        protected void TransferPropertiesWithReflection(PKM Source, PKM Destination)
        {
            var SourceProperties = ReflectUtil.getPropertiesCanWritePublic(Source.GetType());
            var DestinationProperties = ReflectUtil.getPropertiesCanWritePublic(Destination.GetType());

            foreach (string property in SourceProperties.Intersect(DestinationProperties).Where(prop => prop != nameof(Data)))
            {
                var prop = ReflectUtil.GetValue(this, property);
                if (prop == null)
                    continue;
                ReflectUtil.SetValue(Destination, property, prop);
            }
        }
    }
}
