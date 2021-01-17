using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    /// <summary>
    /// Object representing a <see cref="PKM"/>'s data and derived properties.
    /// </summary>
    public abstract class PKM : ISpeciesForm, ITrainerID, IGeneration, ILangNick, IGameValueLimit, INature
    {
        public static readonly string[] Extensions = PKX.GetPKMExtensions();
        public abstract int SIZE_PARTY { get; }
        public abstract int SIZE_STORED { get; }
        public string Extension => GetType().Name.ToLower();
        public abstract PersonalInfo PersonalInfo { get; }
        public virtual IReadOnlyList<ushort> ExtraBytes => Array.Empty<ushort>();

        // Internal Attributes set on creation
        public readonly byte[] Data; // Raw Storage
        public string? Identifier; // User or Form Custom Attribute
        public int Box { get; set; } = -1; // Batch Editor
        public int Slot { get; set; } = -1; // Batch Editor

        protected PKM(byte[] data) => Data = data;
        protected PKM(int size) => Data = new byte[size];

        public virtual byte[] EncryptedPartyData => ArrayUtil.Truncate(Encrypt(), SIZE_PARTY);
        public virtual byte[] EncryptedBoxData => ArrayUtil.Truncate(Encrypt(), SIZE_STORED);
        public virtual byte[] DecryptedPartyData => ArrayUtil.Truncate(Write(), SIZE_PARTY);
        public virtual byte[] DecryptedBoxData => ArrayUtil.Truncate(Write(), SIZE_STORED);

        public virtual bool Valid { get => ChecksumValid && Sanity == 0; set { if (!value) return; Sanity = 0; RefreshChecksum(); } }

        // Trash Bytes
        public abstract byte[] Nickname_Trash { get; set; }
        public abstract byte[] OT_Trash { get; set; }
        public virtual byte[] HT_Trash { get => Array.Empty<byte>(); set { } }

        protected byte[] GetData(int Offset, int Length) => Data.Slice(Offset, Length);

        protected virtual ushort CalculateChecksum() => PokeCrypto.GetCHK(Data, SIZE_STORED);

        protected abstract byte[] Encrypt();
        public abstract int Format { get; }

        private byte[] Write()
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
        public virtual int StatNature { get => Nature; set => Nature = value; }
        public abstract int Ability { get; set; }
        public abstract int CurrentFriendship { get; set; }
        public abstract int Form { get; set; }
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
        public abstract int Status_Condition { get; set; }
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
        public virtual bool Japanese => Language == (int)LanguageID.Japanese;
        public virtual bool Korean => Language == (int)LanguageID.Korean;

        // Future Properties
        public virtual int Met_Year { get => 0; set { } }
        public virtual int Met_Month { get => 0; set { } }
        public virtual int Met_Day { get => 0; set { } }
        public virtual string HT_Name { get => string.Empty; set { } }
        public virtual int HT_Gender { get => 0; set { } }
        public virtual int HT_Friendship { get => 0; set { } }
        public virtual byte Enjoyment { get => 0; set { } }
        public virtual byte Fullness { get => 0; set { } }
        public virtual int AbilityNumber { get => 0; set { } }

        /// <summary>
        /// The date the Pokémon was met.
        /// </summary>
        /// <returns>
        /// A DateTime representing the date the Pokémon was met.
        /// Returns null if either the <see cref="PKM"/> format does not support dates or the stored date is invalid.</returns>
        /// <remarks>
        /// Not all <see cref="PKM"/> types support the <see cref="MetDate"/> property.  In these cases, this property will return null.
        /// If null is assigned to this property, it will be cleared.
        /// </remarks>
        public DateTime? MetDate
        {
            get
            {
                // Check to see if date is valid
                if (!Util.IsDateValid(2000 + Met_Year, Met_Month, Met_Day))
                    return null;
                return new DateTime(2000 + Met_Year, Met_Month, Met_Day);
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

        public virtual int Egg_Year { get => 0; set { } }
        public virtual int Egg_Month { get => 0; set { } }
        public virtual int Egg_Day { get => 0; set { } }

        /// <summary>
        /// The date a Pokémon was met as an egg.
        /// </summary>
        /// <returns>
        /// A DateTime representing the date the Pokémon was met as an egg.
        /// Returns null if either the <see cref="PKM"/> format does not support dates or the stored date is invalid.</returns>
        /// <remarks>
        /// Not all <see cref="PKM"/> types support the <see cref="EggMetDate"/> property.  In these cases, this property will return null.
        /// If null is assigned to this property, it will be cleared.
        /// </remarks>
        public DateTime? EggMetDate
        {
            get
            {
                // Check to see if date is valid
                if (!Util.IsDateValid(2000 + Egg_Year, Egg_Month, Egg_Day))
                    return null;
                return new DateTime(2000 + Egg_Year, Egg_Month, Egg_Day);
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

        public virtual int RelearnMove1 { get => 0; set { } }
        public virtual int RelearnMove2 { get => 0; set { } }
        public virtual int RelearnMove3 { get => 0; set { } }
        public virtual int RelearnMove4 { get => 0; set { } }
        public virtual int EncounterType { get => 0; set { } }

        // Exposed but not Present in all
        public abstract int CurrentHandler { get; set; }

        // Maximums
        public abstract int MaxMoveID { get; }
        public abstract int MaxSpeciesID { get; }
        public abstract int MaxItemID { get; }
        public abstract int MaxAbilityID { get; }
        public abstract int MaxBallID { get; }
        public abstract int MaxGameID { get; }
        public virtual int MinGameID => 0;
        public abstract int MaxIV { get; }
        public abstract int MaxEV { get; }
        public abstract int OTLength { get; }
        public abstract int NickLength { get; }

        // Derived
        public int SpecForm { get => Species + (Form << 11); set { Species = value & 0x7FF; Form = value >> 11; } }
        public virtual int SpriteItem => HeldItem;
        public virtual bool IsShiny => TSV == PSV;
        public StorageSlotFlag StorageFlags { get; internal set; }
        public bool Locked => StorageFlags.HasFlagFast(StorageSlotFlag.Locked);
        public int TrainerID7 { get => (int)((uint)(TID | (SID << 16)) % 1000000); set => SetID7(TrainerSID7, value); }
        public int TrainerSID7 { get => (int)((uint)(TID | (SID << 16)) / 1000000); set => SetID7(value, TrainerID7); }

        public uint ShinyXor
        {
            get
            {
                var pid = PID;
                var upper = (pid >> 16) ^ (uint)SID;
                return (pid & 0xFFFF) ^ (uint)TID ^ upper;
            }
        }

        public int DisplayTID
        {
            get => Generation >= 7 ? TrainerID7 : TID;
            set { if (Generation >= 7) TrainerID7 = value; else TID = value; }
        }

        public int DisplaySID
        {
            get => Generation >= 7 ? TrainerSID7 : SID;
            set { if (Generation >= 7) TrainerSID7 = value; else SID = value; }
        }

        private void SetID7(int sid7, int tid7)
        {
            var oid = (sid7 * 1_000_000) + (tid7 % 1_000_000);
            TID = (ushort)oid;
            SID = oid >> 16;
        }

        public bool E => Version == (int)GameVersion.E;
        public bool FRLG => Version is (int)FR or (int)LG;
        public bool Pt => (int)GameVersion.Pt == Version;
        public bool HGSS => Version is (int)HG or (int)SS;
        public bool BW => Version is (int)B or (int)W;
        public bool B2W2 => Version is (int)B2 or (int)W2;
        public bool XY => Version is (int)X or (int)Y;
        public bool AO => Version is (int)AS or (int)OR;
        public bool SM => Version is (int)SN or (int)MN;
        public bool USUM => Version is (int)US or (int)UM;
        public bool GO => Version is (int)GameVersion.GO;
        public bool VC1 => Version is >= (int)RD and <= (int)YW;
        public bool VC2 => Version is >= (int)GD and <= (int)C;
        public bool LGPE => Version is (int)GP or (int)GE;
        public bool SWSH => Version is (int)SW or (int)SH;

        protected bool PtHGSS => Pt || HGSS;
        public bool GO_LGPE => GO && Met_Location == Locations.GO7;
        public bool GO_HOME => GO && Met_Location == Locations.GO8;
        public bool VC => VC1 || VC2;
        public bool GG => LGPE || GO_LGPE;
        public bool Gen8 => Version is >= 44 and <= 45 || GO_HOME;
        public bool Gen7 => Version is >= 30 and <= 33 || GG;
        public bool Gen6 => Version is >= 24 and <= 29;
        public bool Gen5 => Version is >= 20 and <= 23;
        public bool Gen4 => Version is >= 7 and <= 12 and not 9;
        public bool Gen3 => Version is >= 1 and <= 5 or 15;
        public bool Gen2 => Version == (int)GSC; // Fixed value set by the Gen2 PKM classes
        public bool Gen1 => Version == (int)RBY; // Fixed value set by the Gen1 PKM classes
        public bool GenU => Generation <= 0;

        public int Generation
        {
            get
            {
                if (Gen8) return 8;
                if (Gen7) return 7;
                if (Gen6) return 6;
                if (Gen5) return 5;
                if (Gen4) return 4;
                if (Gen3) return 3;
                if (Gen2) return Format; // 2
                if (Gen1) return Format; // 1
                if (VC1) return 1;
                if (VC2) return 2;
                return -1;
            }
        }

        public int DebutGeneration => Legal.GetDebutGeneration(Species);
        public bool PKRS_Infected { get => PKRS_Strain > 0; set => PKRS_Strain = value ? Math.Max(PKRS_Strain, 1) : 0; }

        public bool PKRS_Cured
        {
            get => PKRS_Days == 0 && PKRS_Strain > 0;
            set
            {
                PKRS_Days = value ? 0 : 1;
                PKRS_Infected = true;
            }
        }

        public virtual bool ChecksumValid => Checksum == CalculateChecksum();
        public int CurrentLevel { get => Experience.GetLevel(EXP, PersonalInfo.EXPGrowth); set => EXP = Experience.GetEXP(Stat_Level = value, PersonalInfo.EXPGrowth); }
        public int MarkCircle      { get => Markings[0]; set { var marks = Markings; marks[0] = value; Markings = marks; } }
        public int MarkTriangle    { get => Markings[1]; set { var marks = Markings; marks[1] = value; Markings = marks; } }
        public int MarkSquare      { get => Markings[2]; set { var marks = Markings; marks[2] = value; Markings = marks; } }
        public int MarkHeart       { get => Markings[3]; set { var marks = Markings; marks[3] = value; Markings = marks; } }
        public int MarkStar        { get => Markings[4]; set { var marks = Markings; marks[4] = value; Markings = marks; } }
        public int MarkDiamond     { get => Markings[5]; set { var marks = Markings; marks[5] = value; Markings = marks; } }
        public int IVTotal => IV_HP + IV_ATK + IV_DEF + IV_SPA + IV_SPD + IV_SPE;
        public int EVTotal => EV_HP + EV_ATK + EV_DEF + EV_SPA + EV_SPD + EV_SPE;
        public int MaximumIV => Math.Max(Math.Max(Math.Max(Math.Max(Math.Max(IV_HP, IV_ATK), IV_DEF), IV_SPA), IV_SPD), IV_SPE);

        public int FlawlessIVCount
        {
            get
            {
                int max = MaxIV;
                int ctr = 0;
                if (IV_HP == max) ++ctr;
                if (IV_ATK == max) ++ctr;
                if (IV_DEF == max) ++ctr;
                if (IV_SPA == max) ++ctr;
                if (IV_SPD == max) ++ctr;
                if (IV_SPE == max) ++ctr;
                return ctr;
            }
        }

        public string FileName => $"{FileNameWithoutExtension}.{Extension}";

        public virtual string FileNameWithoutExtension
        {
            get
            {
                string form = Form > 0 ? $"-{Form:00}" : string.Empty;
                string star = IsShiny ? " ★" : string.Empty;
                return $"{Species:000}{form}{star} - {Nickname} - {Checksum:X4}{EncryptionConstant:X8}";
            }
        }

        public int[] IVs
        {
            get => new[] { IV_HP, IV_ATK, IV_DEF, IV_SPE, IV_SPA, IV_SPD };
            set
            {
                if (value.Length != 6)
                    return;
                IV_HP = value[0]; IV_ATK = value[1]; IV_DEF = value[2];
                IV_SPE = value[3]; IV_SPA = value[4]; IV_SPD = value[5];
            }
        }

        public int[] EVs
        {
            get => new[] { EV_HP, EV_ATK, EV_DEF, EV_SPE, EV_SPA, EV_SPD };
            set
            {
                if (value.Length != 6)
                    return;
                EV_HP = value[0]; EV_ATK = value[1]; EV_DEF = value[2];
                EV_SPE = value[3]; EV_SPA = value[4]; EV_SPD = value[5];
            }
        }

        public int[] Stats
        {
            get => new[] { Stat_HPCurrent, Stat_ATK, Stat_DEF, Stat_SPE, Stat_SPA, Stat_SPD };
            set
            {
                if (value.Length != 6)
                    return;
                Stat_HPCurrent = value[0]; Stat_ATK = value[1]; Stat_DEF = value[2];
                Stat_SPE = value[3]; Stat_SPA = value[4]; Stat_SPD = value[5];
            }
        }

        public int[] Moves
        {
            get => new[] { Move1, Move2, Move3, Move4 };
            set => SetMoves(value);
        }

        public void SetMoves(IReadOnlyList<int> value)
        {
            Move1 = value.Count > 0 ? value[0] : 0;
            Move2 = value.Count > 1 ? value[1] : 0;
            Move3 = value.Count > 2 ? value[2] : 0;
            Move4 = value.Count > 3 ? value[3] : 0;
        }

        public int[] RelearnMoves
        {
            get => new[] { RelearnMove1, RelearnMove2, RelearnMove3, RelearnMove4 };
            set => SetRelearnMoves(value);
        }

        public void SetRelearnMoves(IReadOnlyList<int> value)
        {
            RelearnMove1 = value.Count > 0 ? value[0] : 0;
            RelearnMove2 = value.Count > 1 ? value[1] : 0;
            RelearnMove3 = value.Count > 2 ? value[2] : 0;
            RelearnMove4 = value.Count > 3 ? value[3] : 0;
        }

        public int PIDAbility
        {
            get
            {
                if (Generation > 5 || Format > 5)
                    return -1;

                if (Version == (int) CXD)
                    return PersonalInfo.GetAbilityIndex(Ability); // Can mismatch; not tied to PID
                return (int)((Gen5 ? PID >> 16 : PID) & 1);
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
                    b |= (byte)(Math.Min(value[i], 1) << i);
                MarkValue = b;
            }
        }

        private int HPBitValPower => ((IV_HP & 2) >> 1) | ((IV_ATK & 2) >> 0) | ((IV_DEF & 2) << 1) | ((IV_SPE & 2) << 2) | ((IV_SPA & 2) << 3) | ((IV_SPD & 2) << 4);
        public virtual int HPPower => Format < 6 ? ((40 * HPBitValPower) / 63) + 30 : 60;

        private int HPBitValType =>  ((IV_HP & 1) >> 0) | ((IV_ATK & 1) << 1) | ((IV_DEF & 1) << 2) | ((IV_SPE & 1) << 3) | ((IV_SPA & 1) << 4) | ((IV_SPD & 1) << 5);

        public virtual int HPType
        {
            get => 15 * HPBitValType / 63;
            set
            {
                var dlb = HiddenPower.DefaultLowBits;
                IV_HP =  (IV_HP  & ~1) + dlb[value, 0];
                IV_ATK = (IV_ATK & ~1) + dlb[value, 1];
                IV_DEF = (IV_DEF & ~1) + dlb[value, 2];
                IV_SPE = (IV_SPE & ~1) + dlb[value, 3];
                IV_SPA = (IV_SPA & ~1) + dlb[value, 4];
                IV_SPD = (IV_SPD & ~1) + dlb[value, 5];
            }
        }

        // Legality Extensions
        public TradebackType TradebackStatus { get; set; } = TradebackType.Any;
        public bool Gen1_NotTradeback => TradebackStatus == TradebackType.Gen1_NotTradeback;
        public bool Gen2_NotTradeback => TradebackStatus == TradebackType.Gen2_NotTradeback;
        public virtual bool WasLink => false;

        public bool WasEgg
        {
            get
            {
                int loc = Egg_Location;
                return Generation switch
                {
                    4 => (Legal.EggLocations4.Contains(loc) || (Species == (int) Core.Species.Manaphy && loc == Locations.Ranger4) || (loc == Locations.Faraway4 && PtHGSS)), // faraway
                    5 => Legal.EggLocations5.Contains(loc),
                    6 => Legal.EggLocations6.Contains(loc),
                    7 => Legal.EggLocations7.Contains(loc),
                    8 => Legal.EggLocations8.Contains(loc),
                    // Gen 1/2 and pal park Gen 3
                    _ => false
                };
            }
        }

        public bool WasBredEgg
        {
            get
            {
                int loc = Egg_Location;
                return Generation switch
                {
                    4 => loc is Locations.Daycare4 or Locations.LinkTrade4 || (loc == Locations.Faraway4 && PtHGSS),
                    5 => loc is Locations.Daycare5 or Locations.LinkTrade5,
                    6 or 7 or 8 => loc is Locations.Daycare5 or Locations.LinkTrade6,
                    _ => false,// Gen 1/2 and pal park Gen 3
                };
            }
        }

        public virtual bool WasGiftEgg
        {
            get
            {
                if (!WasEgg)
                    return false;
                int loc = Egg_Location;
                return Generation switch
                {
                    4 => Legal.GiftEggLocation4.Contains(loc) || (loc == Locations.Faraway4 && HGSS),
                    5 => loc == 60003,
                    6 or 7 or 8 => loc == 60004,
                    _ => false,
                };
            }
        }

        public virtual bool WasEvent => Locations.IsEventLocation5(Met_Location) || FatefulEncounter;

        public virtual bool WasEventEgg
        {
            get
            {
                if (Gen4)
                    return WasEgg && Species == (int) Core.Species.Manaphy;
                // Gen5+
                if (Met_Level != 1)
                    return false;
                int loc = Egg_Location;
                return Locations.IsEventLocation5(loc) || (FatefulEncounter && loc != 0);
            }
        }

        public bool WasTradedEgg => Egg_Location == GetTradedEggLocation();
        public bool IsTradedEgg => Met_Location == GetTradedEggLocation();
        private int GetTradedEggLocation() => Locations.TradedEggLocation(Generation);

        public virtual bool IsUntraded => false;
        public bool IsNative => Generation == Format;
        public bool IsOriginValid => Species <= Legal.GetMaxSpeciesOrigin(Format);

        /// <summary>
        /// Checks if the <see cref="PKM"/> could inhabit a set of games.
        /// </summary>
        /// <param name="generation">Set of games.</param>
        /// <param name="species"></param>
        /// <returns>True if could inhabit, False if not.</returns>
        public bool InhabitedGeneration(int generation, int species = -1)
        {
            if (species < 0)
                species = Species;

            var format = Format;
            if (format == generation)
                return true;

            if (!IsOriginValid)
                return false;

            // Sanity Check Species ID
            if (species > Legal.GetMaxSpeciesOrigin(generation) && !EvolutionLegality.GetFutureGenEvolutions(generation).Contains(species))
                return false;

            // Trade generation 1 -> 2
            if (format == 2 && generation == 1 && !Gen2_NotTradeback)
                return true;

            // Trade generation 2 -> 1
            if (format == 1 && generation == 2 && !Gen1_NotTradeback)
                return true;

            if (format < generation)
                return false; // Future

            int gen = Generation;
            return generation switch
            {
                1 => format == 1 || VC, // species compat checked via sanity above
                2 => format == 2 || VC,
                3 => Gen3,
                4 => gen is >= 3 and <= 4,
                5 => gen is >= 3 and <= 5,
                6 => gen is >= 3 and <= 6,
                7 => gen is >= 3 and <= 7 || VC,
                8 => gen is >= 3 and <= 8 || VC,
                _ => false
            };
        }

        /// <summary>
        /// Checks if the PKM has its original met location.
        /// </summary>
        /// <returns>Returns false if the Met Location has been overwritten via generational transfer.</returns>
        public virtual bool HasOriginalMetLocation => !(Format < 3 || VC || (Generation <= 4 && Format != Generation));

        /// <summary>
        /// Checks if the current <see cref="Gender"/> is valid.
        /// </summary>
        /// <returns>True if valid, False if invalid.</returns>
        public virtual bool IsGenderValid()
        {
            int gender = Gender;
            int gv = PersonalInfo.Gender;
            if (gv == 255)
                return gender == 2;
            if (gv == 254)
                return gender == 1;
            if (gv == 0)
                return gender == 0;

            int gen = Generation;
            if (gen is <= 2 or >= 6) // not 3-5
                return gender == (gender & 1);

            return gender == PKX.GetGenderFromPIDAndRatio(PID, gv);
        }

        /// <summary>
        /// Updates the checksum of the <see cref="PKM"/>.
        /// </summary>
        public virtual void RefreshChecksum() => Checksum = CalculateChecksum();

        /// <summary>
        /// Reorders moves and fixes PP if necessary.
        /// </summary>
        public void FixMoves()
        {
            ReorderMoves();

            if (Move1 == 0) Move1_PP = Move1_PPUps = 0;
            if (Move2 == 0) Move2_PP = Move2_PPUps = 0;
            if (Move3 == 0) Move3_PP = Move3_PPUps = 0;
            if (Move4 == 0) Move4_PP = Move4_PPUps = 0;
        }

        /// <summary>
        /// Reorders moves to put Empty entries last.
        /// </summary>
        private void ReorderMoves()
        {
            if (Move1 == 0 && Move2 != 0)
            {
                Move1 = Move2;
                Move1_PP = Move2_PP;
                Move1_PPUps = Move2_PPUps;
                Move2 = 0;
            }
            if (Move2 == 0 && Move3 != 0)
            {
                Move2 = Move3;
                Move2_PP = Move3_PP;
                Move2_PPUps = Move3_PPUps;
                Move3 = 0;
            }
            if (Move3 == 0 && Move4 != 0)
            {
                Move3 = Move4;
                Move3_PP = Move4_PP;
                Move3_PPUps = Move4_PPUps;
                Move4 = 0;
            }
        }

        /// <summary>
        /// Applies the desired Ability option.
        /// </summary>
        /// <param name="n">Ability Number (0/1/2)</param>
        public virtual void RefreshAbility(int n)
        {
            AbilityNumber = 1 << n;
            var abilities = PersonalInfo.Abilities;
            if ((uint)n < abilities.Count)
                Ability = abilities[n];
        }

        /// <summary>
        /// Gets the IV Judge Rating value.
        /// </summary>
        /// <remarks>IV Judge scales his response 0 (worst) to 3 (best).</remarks>
        public int PotentialRating
        {
            get
            {
                int ivTotal = IVTotal;
                if (ivTotal <= 90)
                    return 0;
                if (ivTotal <= 120)
                    return 1;
                return ivTotal <= 150 ? 2 : 3;
            }
        }

        /// <summary>
        /// Gets the current Battle Stats.
        /// </summary>
        /// <param name="p"><see cref="PersonalInfo"/> entry containing Base Stat Info</param>
        /// <returns>Battle Stats (H/A/B/S/C/D)</returns>
        public virtual ushort[] GetStats(PersonalInfo p)
        {
            int level = CurrentLevel;

            ushort[] stats = this is IHyperTrain t ? GetStats(p, t, level) : GetStats(p, level);
            // Account for nature
            PKX.ModifyStatsForNature(stats, StatNature);
            return stats;
        }

        private ushort[] GetStats(PersonalInfo p, IHyperTrain t, int level)
        {
            ushort[] stats = new ushort[6];
            stats[0] = (ushort)(p.HP == 1 ? 1 : (((t.HT_HP ? 31 : IV_HP) + (2 * p.HP) + (EV_HP / 4) + 100) * level / 100) + 10);
            stats[1] = (ushort)((((t.HT_ATK ? 31 : IV_ATK) + (2 * p.ATK) + (EV_ATK / 4)) * level / 100) + 5);
            stats[2] = (ushort)((((t.HT_DEF ? 31 : IV_DEF) + (2 * p.DEF) + (EV_DEF / 4)) * level / 100) + 5);
            stats[4] = (ushort)((((t.HT_SPA ? 31 : IV_SPA) + (2 * p.SPA) + (EV_SPA / 4)) * level / 100) + 5);
            stats[5] = (ushort)((((t.HT_SPD ? 31 : IV_SPD) + (2 * p.SPD) + (EV_SPD / 4)) * level / 100) + 5);
            stats[3] = (ushort)((((t.HT_SPE ? 31 : IV_SPE) + (2 * p.SPE) + (EV_SPE / 4)) * level / 100) + 5);
            return stats;
        }

        private ushort[] GetStats(PersonalInfo p, int level)
        {
            ushort[] stats = new ushort[6];
            stats[0] = (ushort)(p.HP == 1 ? 1 : ((IV_HP + (2 * p.HP) + (EV_HP / 4) + 100) * level / 100) + 10);
            stats[1] = (ushort)(((IV_ATK + (2 * p.ATK) + (EV_ATK / 4)) * level / 100) + 5);
            stats[2] = (ushort)(((IV_DEF + (2 * p.DEF) + (EV_DEF / 4)) * level / 100) + 5);
            stats[4] = (ushort)(((IV_SPA + (2 * p.SPA) + (EV_SPA / 4)) * level / 100) + 5);
            stats[5] = (ushort)(((IV_SPD + (2 * p.SPD) + (EV_SPD / 4)) * level / 100) + 5);
            stats[3] = (ushort)(((IV_SPE + (2 * p.SPE) + (EV_SPE / 4)) * level / 100) + 5);
            return stats;
        }

        /// <summary>
        /// Applies the specified stats to the <see cref="PKM"/>.
        /// </summary>
        /// <param name="stats">Battle Stats (H/A/B/S/C/D)</param>
        public void SetStats(ushort[] stats)
        {
            Stat_HPMax = Stat_HPCurrent = stats[0];
            Stat_ATK = stats[1];
            Stat_DEF = stats[2];
            Stat_SPE = stats[3];
            Stat_SPA = stats[4];
            Stat_SPD = stats[5];
        }

        /// <summary>
        /// Indicates if Party Stats are present. False if not initialized (from stored format).
        /// </summary>
        public bool PartyStatsPresent => Stat_HPMax != 0;

        /// <summary>
        /// Clears any status condition and refreshes the stats.
        /// </summary>
        public void ResetPartyStats()
        {
            SetStats(GetStats(PersonalInfo));
            Stat_Level = CurrentLevel;
            Status_Condition = 0;
        }

        public void Heal()
        {
            ResetPartyStats();
            HealPP();
        }

        /// <summary>
        /// Restores PP to maximum based on the current PP Ups for each move.
        /// </summary>
        public void HealPP()
        {
            Move1_PP = GetMovePP(Move1, Move1_PPUps);
            Move2_PP = GetMovePP(Move2, Move2_PPUps);
            Move3_PP = GetMovePP(Move3, Move3_PPUps);
            Move4_PP = GetMovePP(Move4, Move4_PPUps);
        }

        /// <summary>
        /// Enforces that Party Stat values are present.
        /// </summary>
        /// <returns>True if stats were refreshed, false if stats were already present.</returns>
        public bool ForcePartyData()
        {
            if (PartyStatsPresent)
                return false;
            ResetPartyStats();
            return true;
        }

        /// <summary>
        /// Checks if the <see cref="PKM"/> can hold its <see cref="HeldItem"/>.
        /// </summary>
        /// <param name="valid">Items that the <see cref="PKM"/> can hold.</param>
        /// <returns>True/False if the <see cref="PKM"/> can hold its <see cref="HeldItem"/>.</returns>
        public virtual bool CanHoldItem(IReadOnlyList<ushort> valid) => valid.Contains((ushort)HeldItem);

        /// <summary>
        /// Deep clones the <see cref="PKM"/> object. The clone will not have any shared resources with the source.
        /// </summary>
        /// <returns>Cloned <see cref="PKM"/> object</returns>
        public abstract PKM Clone();

        /// <summary>
        /// Sets Link Trade data for an <see cref="IsEgg"/>.
        /// </summary>
        /// <param name="day">Day the <see cref="PKM"/> was traded.</param>
        /// <param name="month">Month the <see cref="PKM"/> was traded.</param>
        /// <param name="y">Day the <see cref="PKM"/> was traded.</param>
        /// <param name="location">Link Trade location value.</param>
        protected void SetLinkTradeEgg(int day, int month, int y, int location)
        {
            Met_Day = day;
            Met_Month = month;
            Met_Year = y - 2000;
            Met_Location = location;
        }

        /// <summary>
        /// Gets the PP of a Move ID with consideration of the amount of PP Ups applied.
        /// </summary>
        /// <param name="move">Move ID</param>
        /// <param name="ppUpCount">PP Ups count</param>
        /// <returns>Current PP for the move.</returns>
        public virtual int GetMovePP(int move, int ppUpCount) => GetBasePP(move) * (5 + ppUpCount) / 5;

        /// <summary>
        /// Gets the base PP of a move ID depending on the <see cref="PKM"/>'s format.
        /// </summary>
        /// <param name="move">Move ID</param>
        /// <returns>Amount of PP the move has by default (no PP Ups).</returns>
        private int GetBasePP(int move)
        {
            var table = Legal.GetPPTable(this, Format);
            if (move >= table.Count)
                move = 0;
            return table[move];
        }

        /// <summary>
        /// Applies a shiny <see cref="PID"/> to the <see cref="PKM"/>.
        /// </summary>
        /// <remarks>
        /// If a <see cref="PKM"/> originated in a generation prior to Generation 6, the <see cref="EncryptionConstant"/> is updated.
        /// If a <see cref="PKM"/> is in the <see cref="GBPKM"/> format, it will update the <see cref="IVs"/> instead.
        /// </remarks>
        public virtual void SetShiny()
        {
            var rnd = Util.Rand;
            do { PID = PKX.GetRandomPID(rnd, Species, Gender, Version, Nature, Form, PID); }
            while (!IsShiny);
            if (Format >= 6 && (Gen3 || Gen4 || Gen5))
                EncryptionConstant = PID;
        }

        /// <summary>
        /// Applies a shiny <see cref="SID"/> to the <see cref="PKM"/>.
        /// </summary>
        public void SetShinySID(Shiny shiny = Shiny.Random)
        {
            if (IsShiny && shiny.IsValid(this))
                return;

            var xor = TID ^ (PID >> 16) ^ (PID & 0xFFFF);
            var bits = shiny switch
            {
                Shiny.AlwaysSquare => 0,
                Shiny.AlwaysStar => 1,
                _ => Util.Rand.Next(8)
            };

            SID = (int)xor ^ bits;
        }

        /// <summary>
        /// Applies a <see cref="PID"/> to the <see cref="PKM"/> according to the specified <see cref="Gender"/>.
        /// </summary>
        /// <param name="gender"><see cref="Gender"/> to apply</param>
        /// <remarks>
        /// If a <see cref="PKM"/> originated in a generation prior to Generation 6, the <see cref="EncryptionConstant"/> is updated.
        /// </remarks>
        public void SetPIDGender(int gender)
        {
            var rnd = Util.Rand;
            do PID = PKX.GetRandomPID(rnd, Species, gender, Version, Nature, Form, PID);
            while (IsShiny);
            if (Format >= 6 && (Gen3 || Gen4 || Gen5))
                EncryptionConstant = PID;
        }

        /// <summary>
        /// Applies a <see cref="PID"/> to the <see cref="PKM"/> according to the specified <see cref="Gender"/>.
        /// </summary>
        /// <param name="nature"><see cref="Nature"/> to apply</param>
        /// <remarks>
        /// If a <see cref="PKM"/> originated in a generation prior to Generation 6, the <see cref="EncryptionConstant"/> is updated.
        /// </remarks>
        public void SetPIDNature(int nature)
        {
            var rnd = Util.Rand;
            do PID = PKX.GetRandomPID(rnd, Species, Gender, Version, nature, Form, PID);
            while (IsShiny);
            if (Format >= 6 && (Gen3 || Gen4 || Gen5))
                EncryptionConstant = PID;
        }

        /// <summary>
        /// Applies a <see cref="PID"/> to the <see cref="PKM"/> according to the specified <see cref="Form"/>.
        /// </summary>
        /// <param name="form"><see cref="Form"/> to apply</param>
        /// <remarks>
        /// This method should only be used for Unown originating in Generation 3 games.
        /// If a <see cref="PKM"/> originated in a generation prior to Generation 6, the <see cref="EncryptionConstant"/> is updated.
        /// </remarks>
        public void SetPIDUnown3(int form)
        {
            do PID = Util.Rand32(); while (PKX.GetUnownForm(PID) != form);
            if (Format >= 6 && (Gen3 || Gen4 || Gen5))
                EncryptionConstant = PID;
        }

        /// <summary>
        /// Randomizes the IVs within game constraints.
        /// </summary>
        /// <param name="flawless">Count of flawless IVs to set. If none provided, a count will be detected.</param>
        /// <returns>Randomized IVs if desired.</returns>
        public int[] SetRandomIVs(int? flawless = null)
        {
            if (Version == (int)GameVersion.GO && flawless != 6)
                return SetRandomIVsGO();

            int[] ivs = new int[6];
            var rnd = Util.Rand;
            for (int i = 0; i < 6; i++)
                ivs[i] = rnd.Next(MaxIV + 1);

            int count = flawless ?? GetFlawlessIVCount();
            if (count != 0)
            {
                for (int i = 0; i < count; i++)
                    ivs[i] = MaxIV;
                Util.Shuffle(ivs); // Randomize IV order
            }
            return IVs = ivs;
        }

        public int[] SetRandomIVsGO(int minIV = 0)
        {
            int[] ivs = new int[6];
            var rnd = Util.Rand;
            ivs[0] = (rnd.Next(minIV, 16) << 1) | 1; // hp
            ivs[1] = ivs[4] = (rnd.Next(minIV, 16) << 1) | 1; // attack
            ivs[2] = ivs[5] = (rnd.Next(minIV, 16) << 1) | 1; // defense
            ivs[3] = rnd.Next(MaxIV + 1); // speed
            return IVs = ivs;
        }

        /// <summary>
        /// Randomizes the IVs within game constraints.
        /// </summary>
        /// <param name="template">IV template to generate from</param>
        /// <param name="flawless">Count of flawless IVs to set. If none provided, a count will be detected.</param>
        /// <returns>Randomized IVs if desired.</returns>
        public int[] SetRandomIVs(IReadOnlyList<int> template, int? flawless = null)
        {
            int count = flawless ?? GetFlawlessIVCount();
            int[] ivs = new int[6];
            var rnd = Util.Rand;
            do
            {
                for (int i = 0; i < 6; i++)
                    ivs[i] = template[i] < 0 ? rnd.Next(MaxIV + 1) : template[i];
            } while (ivs.Count(z => z == MaxIV) < count);

            IVs = ivs;
            return ivs;
        }

        /// <summary>
        /// Gets the amount of flawless IVs that the <see cref="PKM"/> should have.
        /// </summary>
        /// <returns>Count of IVs that should be max.</returns>
        public int GetFlawlessIVCount()
        {
            if (Generation >= 6 && (Legal.Legends.Contains(Species) || Legal.SubLegends.Contains(Species)))
                return 3;
            if (XY)
            {
                if (PersonalInfo.EggGroup1 == 15) // Undiscovered
                    return 3;
                if (Met_Location == 148 && Met_Level == 30) // Friend Safari
                    return 2;
            }
            if (VC)
                return Species is (int)Core.Species.Mew or (int)Core.Species.Celebi ? 5 : 3;
            return 0;
        }

        /// <summary>
        /// Applies all shared properties from the current <see cref="PKM"/> to <see cref="Destination"/> <see cref="PKM"/>.
        /// </summary>
        /// <param name="Destination"><see cref="PKM"/> that receives property values.</param>
        public void TransferPropertiesWithReflection(PKM Destination)
        {
            // Only transfer declared properties not defined in PKM.cs but in the actual type
            var srcType = GetType();
            var destType = Destination.GetType();
            var srcProperties = ReflectUtil.GetAllPropertyInfoPublic(srcType).Select(z => z.Name);
            var destProperties = ReflectUtil.GetAllPropertyInfoPublic(destType).Where(z => z.SetMethod != null).Select(z => z.Name);

            // Transfer properties in the order they are defined in the destination PKM format for best conversion
            var shared = destProperties.Intersect(srcProperties);
            foreach (string property in shared)
            {
                if (!BatchEditing.TryGetHasProperty(this, property, out var src))
                    continue;
                var prop = src.GetValue(this);
                if (prop is not byte[] && BatchEditing.TryGetHasProperty(Destination, property, out var pi))
                    ReflectUtil.SetValue(pi, Destination, prop);
            }
        }

        /// <summary>
        /// Checks if the <see cref="PKM"/> has the <see cref="move"/> in its current move list.
        /// </summary>
        public bool HasMove(int move) => Move1 == move || Move2 == move || Move3 == move || Move4 == move;

        public int GetMoveIndex(int move) => Move1 == move ? 0 : Move2 == move ? 1 : Move3 == move ? 2 : Move4 == move ? 3 : -1;

        public int GetMove(int index) => index switch
        {
            0 => Move1,
            1 => Move2,
            2 => Move3,
            3 => Move4,
            _ => throw new IndexOutOfRangeException(nameof(index)),
        };

        public int SetMove(int index, int value) => index switch
        {
            0 => Move1 = value,
            1 => Move2 = value,
            2 => Move3 = value,
            3 => Move4 = value,
            _ => throw new IndexOutOfRangeException(nameof(index))
        };

        /// <summary>
        /// Clears moves that a <see cref="PKM"/> may have, possibly from a future generation.
        /// </summary>
        public void ClearInvalidMoves()
        {
            uint invalid = 0;
            var moves = Moves;
            for (var i = 0; i < moves.Length; i++)
            {
                if (moves[i] <= MaxMoveID)
                    continue;

                invalid++;
                moves[i] = 0;
            }
            if (invalid == 0)
                return;
            if (invalid == 4) // no moves remain
            {
                moves[0] = 1; // Pound
                Move1_PP = GetMovePP(1, Move1_PPUps);
            }

            Moves = moves;
            FixMoves();
        }

        /// <summary>
        /// Gets one of the <see cref="EVs"/> based on its index within the array.
        /// </summary>
        /// <param name="index">Index to get</param>
        public int GetEV(int index) => index switch
        {
            0 => EV_HP,
            1 => EV_ATK,
            2 => EV_DEF,
            3 => EV_SPE,
            4 => EV_SPA,
            5 => EV_SPD,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };

        /// <summary>
        /// Gets one of the <see cref="IVs"/> based on its index within the array.
        /// </summary>
        /// <param name="index">Index to get</param>
        public int GetIV(int index) => index switch
        {
            0 => IV_HP,
            1 => IV_ATK,
            2 => IV_DEF,
            3 => IV_SPE,
            4 => IV_SPA,
            5 => IV_SPD,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };
    }
}
