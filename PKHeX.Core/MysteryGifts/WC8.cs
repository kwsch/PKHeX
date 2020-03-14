using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static PKHeX.Core.RibbonIndex;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 8 Mystery Gift Template File
    /// </summary>
    public sealed class WC8 : DataMysteryGift, ILangNick, INature, IGigantamax, IDynamaxLevel, IRibbonIndex, IMemoryOT,
        IRibbonSetEvent3, IRibbonSetEvent4, IRibbonSetCommon3, IRibbonSetCommon4, IRibbonSetCommon6, IRibbonSetCommon7, IRibbonSetCommon8, IRibbonSetMark8
    {
        public const int Size = 0x2D0;
        public const int CardStart = 0x0;

        public override int Format => 8;

        public enum GiftType : byte
        {
            None = 0,
            Pokemon = 1,
            Item = 2,
            BP = 3,
            Clothing = 4,
        }

        public WC8() : this(new byte[Size]) { }
        public WC8(byte[] data) : base(data) { }

        // TODO: public byte RestrictVersion?

        public bool CanBeReceivedByVersion(int v)
        {
            if (v < (int)GameVersion.SW || v > (int)GameVersion.SH)
                return false;
            return true;
        }

        // General Card Properties
        public override int CardID
        {
            get => BitConverter.ToUInt16(Data, CardStart + 0x8);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x8);
        }

        public byte CardFlags { get => Data[CardStart + 0x10]; set => Data[CardStart + 0x10] = value; }
        public GiftType CardType { get => (GiftType)Data[CardStart + 0x11]; set => Data[CardStart + 0x11] = (byte)value; }
        public bool GiftRepeatable { get => (CardFlags & 1) == 0; set => CardFlags = (byte)((CardFlags & ~1) | (value ? 0 : 1)); }
        public override bool GiftUsed { get => false; set { }  }

        public int CardTitleIndex
        {
            get => Data[CardStart + 0x15];
            set => Data[CardStart + 0x15] = (byte) value;
        }

        public override string CardTitle
        {
            get => "Mystery Gift"; // TODO: Use text string from CardTitleIndex
            set => throw new Exception();
        }

        // Item Properties
        public override bool IsItem { get => CardType == GiftType.Item; set { if (value) CardType = GiftType.Item; } }

        public override int ItemID
        {
            get => GetItem(0);
            set => SetItem(0, (ushort)value);
        }

        public override int Quantity
        {
            get => GetQuantity(0);
            set => SetQuantity(0, (ushort)value);
        }

        public int GetItem(int index) => BitConverter.ToUInt16(Data, CardStart + 0x20 + (0x4 * index));
        public void SetItem(int index, ushort item) => BitConverter.GetBytes(item).CopyTo(Data, CardStart + 0x20 + (4 * index));
        public int GetQuantity(int index) => BitConverter.ToUInt16(Data, CardStart + 0x22 + (0x4 * index));
        public void SetQuantity(int index, ushort quantity) => BitConverter.GetBytes(quantity).CopyTo(Data, CardStart + 0x22 + (4 * index));

        // Pokémon Properties
        public override bool IsPokémon { get => CardType == GiftType.Pokemon; set { if (value) CardType = GiftType.Pokemon; } }
        public override bool IsShiny => PIDType == Shiny.Always;

        public override int TID
        {
            get => BitConverter.ToUInt16(Data, CardStart + 0x20);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x20);
        }

        public override int SID {
            get => BitConverter.ToUInt16(Data, CardStart + 0x22);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x22);
        }

        public int OriginGame
        {
            get => BitConverter.ToInt32(Data, CardStart + 0x24);
            set => BitConverter.GetBytes(value).CopyTo(Data, CardStart + 0x24);
        }

        public uint EncryptionConstant
        {
            get => BitConverter.ToUInt32(Data, CardStart + 0x28);
            set => BitConverter.GetBytes(value).CopyTo(Data, CardStart + 0x28);
        }

        public uint PID
        {
            get => BitConverter.ToUInt32(Data, CardStart + 0x2C);
            set => BitConverter.GetBytes(value).CopyTo(Data, CardStart + 0x2C);
        }

        // Nicknames, OT Names 0x30 - 0x228
        public override int EggLocation { get => BitConverter.ToUInt16(Data, CardStart + 0x228); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x228); }
        public int MetLocation { get => BitConverter.ToUInt16(Data, CardStart + 0x22A); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x22A); }

        public override int Ball
        {
            get => BitConverter.ToUInt16(Data, CardStart + 0x22C);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x22C);
        }

        public override int HeldItem
        {
            get => BitConverter.ToUInt16(Data, CardStart + 0x22E);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x22E);
        }

        public int Move1 { get => BitConverter.ToUInt16(Data, CardStart + 0x230); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x230); }
        public int Move2 { get => BitConverter.ToUInt16(Data, CardStart + 0x232); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x232); }
        public int Move3 { get => BitConverter.ToUInt16(Data, CardStart + 0x234); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x234); }
        public int Move4 { get => BitConverter.ToUInt16(Data, CardStart + 0x236); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x236); }
        public int RelearnMove1 { get => BitConverter.ToUInt16(Data, CardStart + 0x238); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x238); }
        public int RelearnMove2 { get => BitConverter.ToUInt16(Data, CardStart + 0x23A); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x23A); }
        public int RelearnMove3 { get => BitConverter.ToUInt16(Data, CardStart + 0x23C); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x23C); }
        public int RelearnMove4 { get => BitConverter.ToUInt16(Data, CardStart + 0x23E); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x23E); }

        public override int Species { get => BitConverter.ToUInt16(Data, CardStart + 0x240); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x240); }
        public override int Form { get => Data[CardStart + 0x242]; set => Data[CardStart + 0x242] = (byte)value; }
        public override int Gender { get => Data[CardStart + 0x243]; set => Data[CardStart + 0x243] = (byte)value; }
        public override int Level { get => Data[CardStart + 0x244]; set => Data[CardStart + 0x244] = (byte)value; }
        public override bool IsEgg { get => Data[CardStart + 0x245] == 1; set => Data[CardStart + 0x245] = (byte)(value ? 1 : 0); }
        public int Nature { get => (sbyte)Data[CardStart + 0x246]; set => Data[CardStart + 0x246] = (byte)value; }
        public override int AbilityType { get => Data[CardStart + 0x247]; set => Data[CardStart + 0x247] = (byte)value; }

        public Shiny PIDType
        {
            get
            {
                return Data[CardStart + 0x248] switch
                {
                    0 => Shiny.Never,
                    1 => Shiny.Random,
                    2 => Shiny.AlwaysStar,
                    3 => Shiny.AlwaysSquare,
                    4 => Shiny.FixedValue,
                    _ => throw new ArgumentException()
                };
            }
        }

        public int MetLevel { get => Data[CardStart + 0x249]; set => Data[CardStart + 0x249] = (byte)value; }
        public byte DynamaxLevel { get => Data[CardStart + 0x24A]; set => Data[CardStart + 0x24A] = value; }
        public bool CanGigantamax { get => Data[CardStart + 0x24B] != 0; set => Data[CardStart + 0x24B] = (byte)(value ? 1 : 0); }

        // Ribbons 0x24C-0x26C
        private const int RibbonBytesOffset = 0x24C;
        private const int RibbonBytesCount = 0x20;
        private const int RibbonByteNone = 0xFF; // signed -1

        public byte GetRibbonAtIndex(int byteIndex)
        {
            if ((uint)byteIndex >= RibbonBytesCount)
                throw new IndexOutOfRangeException();
            return Data[RibbonBytesOffset + byteIndex];
        }

        public void SetRibbonAtIndex(int byteIndex, byte ribbonIndex)
        {
            if ((uint)byteIndex >= RibbonBytesCount)
                throw new IndexOutOfRangeException();
            Data[RibbonBytesOffset + byteIndex] = ribbonIndex;
        }

        public int IV_HP { get => Data[CardStart + 0x26C]; set => Data[CardStart + 0x26C] = (byte)value; }
        public int IV_ATK { get => Data[CardStart + 0x26D]; set => Data[CardStart + 0x26D] = (byte)value; }
        public int IV_DEF { get => Data[CardStart + 0x26E]; set => Data[CardStart + 0x26E] = (byte)value; }
        public int IV_SPE { get => Data[CardStart + 0x26F]; set => Data[CardStart + 0x26F] = (byte)value; }
        public int IV_SPA { get => Data[CardStart + 0x270]; set => Data[CardStart + 0x270] = (byte)value; }
        public int IV_SPD { get => Data[CardStart + 0x271]; set => Data[CardStart + 0x271] = (byte)value; }

        public int OTGender { get => Data[CardStart + 0x272]; set => Data[CardStart + 0x272] = (byte)value; }

        public int EV_HP {  get => Data[CardStart + 0x273]; set => Data[CardStart + 0x273] = (byte)value; }
        public int EV_ATK { get => Data[CardStart + 0x274]; set => Data[CardStart + 0x274] = (byte)value; }
        public int EV_DEF { get => Data[CardStart + 0x275]; set => Data[CardStart + 0x275] = (byte)value; }
        public int EV_SPE { get => Data[CardStart + 0x276]; set => Data[CardStart + 0x276] = (byte)value; }
        public int EV_SPA { get => Data[CardStart + 0x277]; set => Data[CardStart + 0x277] = (byte)value; }
        public int EV_SPD { get => Data[CardStart + 0x278]; set => Data[CardStart + 0x278] = (byte)value; }

        public int OT_Intensity { get => Data[CardStart + 0x279]; set => Data[CardStart + 0x279] = (byte)value; }
        public int OT_Memory { get => Data[CardStart + 0x27A]; set => Data[CardStart + 0x27A] = (byte)value; }
        public int OT_Feeling { get => Data[CardStart + 0x27B]; set => Data[CardStart + 0x27B] = (byte)value; }
        public int OT_TextVar { get => BitConverter.ToUInt16(Data, CardStart + 0x27C); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, CardStart + 0x27C); }

        // Meta Accessible Properties
        public override int[] IVs
        {
            get => new[] { IV_HP, IV_ATK, IV_DEF, IV_SPE, IV_SPA, IV_SPD };
            set
            {
                if (value.Length != 6) return;
                IV_HP = value[0]; IV_ATK = value[1]; IV_DEF = value[2];
                IV_SPE = value[3]; IV_SPA = value[4]; IV_SPD = value[5];
            }
        }

        public int[] EVs
        {
            get => new[] { EV_HP, EV_ATK, EV_DEF, EV_SPE, EV_SPA, EV_SPD };
            set
            {
                if (value.Length != 6) return;
                EV_HP = value[0]; EV_ATK = value[1]; EV_DEF = value[2];
                EV_SPE = value[3]; EV_SPA = value[4]; EV_SPD = value[5];
            }
        }

        public bool GetIsNicknamed(int language) => BitConverter.ToUInt16(Data, GetNicknameOffset(language)) != 0;

        public int GetNicknameLanguage(int language) => Data[GetNicknameOffset(language) + 0x1A];

        public bool GetHasOT(int language) => BitConverter.ToUInt16(Data, GetOTOffset(language)) != 0;

        private static int GetLanguageIndex(int language)
        {
            var lang = (LanguageID) language;
            if (lang < LanguageID.Japanese || lang == LanguageID.UNUSED_6 || lang > LanguageID.ChineseT)
                return (int) LanguageID.English; // fallback
            return lang < LanguageID.UNUSED_6 ? language - 1 : language - 2;
        }

        public override int Location { get => MetLocation; set => MetLocation = (ushort)value; }

        public override IReadOnlyList<int> Moves
        {
            get => new[] { Move1, Move2, Move3, Move4 };
            set
            {
                if (value.Count > 0) Move1 = value[0];
                if (value.Count > 1) Move2 = value[1];
                if (value.Count > 2) Move3 = value[2];
                if (value.Count > 3) Move4 = value[3];
            }
        }

        public override IReadOnlyList<int> Relearn
        {
            get => new[] { RelearnMove1, RelearnMove2, RelearnMove3, RelearnMove4 };
            set
            {
                if (value.Count > 0) RelearnMove1 = value[0];
                if (value.Count > 1) RelearnMove2 = value[1];
                if (value.Count > 2) RelearnMove3 = value[2];
                if (value.Count > 3) RelearnMove4 = value[3];
            }
        }

        public override string OT_Name { get; set; } = string.Empty;
        public string Nickname => string.Empty;
        public bool IsNicknamed => false;
        public int Language => 2;

        public string GetNickname(int language) => Util.TrimFromZero(Encoding.Unicode.GetString(Data, GetNicknameOffset(language), 0x1A));
        public void SetNickname(int language, string value) => Encoding.Unicode.GetBytes(value.PadRight(0x1A / 2, '\0')).CopyTo(Data, GetNicknameOffset(language));

        public string GetOT(int language) => Util.TrimFromZero(Encoding.Unicode.GetString(Data, GetOTOffset(language), 0x1A));
        public void SetOT(int language, string value) => Encoding.Unicode.GetBytes(value.PadRight(0x1A / 2, '\0')).CopyTo(Data, GetOTOffset(language));

        private static int GetNicknameOffset(int language)
        {
            int index = GetLanguageIndex(language);
            return 0x30 + (index * 0x1C);
        }

        private static int GetOTOffset(int language)
        {
            int index = GetLanguageIndex(language);
            return 0x12C + (index * 0x1C);
        }

        private bool IsHOMEGift => PIDType == Shiny.FixedValue && PID == 0 && EncryptionConstant == 0;

        public override PKM ConvertToPKM(ITrainerInfo SAV, EncounterCriteria criteria)
        {
            if (!IsPokémon)
                throw new ArgumentException(nameof(IsPokémon));

            int currentLevel = Level > 0 ? Level : Util.Rand.Next(1, 101);
            int metLevel = MetLevel > 0 ? MetLevel : currentLevel;
            var pi = PersonalTable.SWSH.GetFormeEntry(Species, Form);
            var OT = GetOT(SAV.Language);

            var pk = new PK8
            {
                EncryptionConstant = EncryptionConstant != 0 || IsHOMEGift ? EncryptionConstant : Util.Rand32(),
                TID = TID,
                SID = SID,
                Species = Species,
                AltForm = Form,
                CurrentLevel = currentLevel,
                Ball = Ball != 0 ? Ball : 4, // Default is Pokeball
                Met_Level = metLevel,
                HeldItem = HeldItem,

                EXP = Experience.GetEXP(currentLevel, pi.EXPGrowth),

                Move1 = Move1,
                Move2 = Move2,
                Move3 = Move3,
                Move4 = Move4,
                RelearnMove1 = RelearnMove1,
                RelearnMove2 = RelearnMove2,
                RelearnMove3 = RelearnMove3,
                RelearnMove4 = RelearnMove4,

                Version = OriginGame != 0 ? OriginGame : SAV.Game,

                OT_Name = OT.Length > 0 ? OT : SAV.OT,
                OT_Gender = OTGender < 2 ? OTGender : SAV.Gender,
                HT_Name = GetHasOT(Language) ? SAV.OT : string.Empty,
                HT_Gender = GetHasOT(Language) ? SAV.Gender : 0,
                HT_Language = GetHasOT(Language) ? SAV.Language : 0,
                CurrentHandler = GetHasOT(Language) ? 1 : 0,
                OT_Friendship = pi.BaseFriendship,

                OT_Intensity = OT_Intensity,
                OT_Memory = OT_Memory,
                OT_TextVar = OT_TextVar,
                OT_Feeling = OT_Feeling,
                FatefulEncounter = true,

                EVs = EVs,

                CanGigantamax = CanGigantamax,
                DynamaxLevel = DynamaxLevel,

                Met_Location = MetLocation,
                Egg_Location = EggLocation,
            };
            pk.SetMaximumPPCurrent();

            if ((SAV.Generation > Format && OriginGame == 0) || !CanBeReceivedByVersion(pk.Version))
            {
                // give random valid game
                var rnd = Util.Rand;
                do { pk.Version = (int)GameVersion.SW + rnd.Next(2); }
                while (!CanBeReceivedByVersion(pk.Version));
            }

            if (OTGender >= 2)
            {
                pk.TID = SAV.TID;
                pk.SID = SAV.SID;
            }

            // Official code explicitly corrects for meowstic
            if (pk.Species == (int)Core.Species.Meowstic)
                pk.AltForm = pk.Gender;

            pk.MetDate = DateTime.Now;

            var nickname_language = GetNicknameLanguage(SAV.Language);
            pk.Language = nickname_language != 0 ? nickname_language : SAV.Language;
            pk.IsNicknamed = GetIsNicknamed(pk.Language);
            pk.Nickname = pk.IsNicknamed ? Nickname : SpeciesName.GetSpeciesNameGeneration(Species, pk.Language, Format);

            for (var i = 0; i < RibbonBytesCount; i++)
            {
                var ribbon = GetRibbonAtIndex(i);
                if (ribbon != RibbonByteNone)
                    pk.SetRibbon(ribbon);
            }

            SetPINGA(pk, SAV, criteria);

            if (IsEgg)
                SetEggMetData(pk);
            pk.CurrentFriendship = pk.IsEgg ? pi.HatchCycles : pi.BaseFriendship;

            pk.HeightScalar = PokeSizeExtensions.GetRandomScalar();
            pk.WeightScalar = PokeSizeExtensions.GetRandomScalar();

            pk.RefreshChecksum();
            return pk;
        }

        private void SetEggMetData(PKM pk)
        {
            pk.IsEgg = true;
            pk.EggMetDate = DateTime.Now;
            pk.Nickname = SpeciesName.GetSpeciesNameGeneration(0, pk.Language, Format);
            pk.IsNicknamed = true;
        }

        private void SetPINGA(PKM pk, ITrainerInfo SAV, EncounterCriteria criteria)
        {
            var pi = PersonalTable.SWSH.GetFormeEntry(Species, Form);
            pk.Nature = (int)criteria.GetNature(Nature == -1 ? Core.Nature.Random : (Nature)Nature);
            pk.StatNature = pk.Nature;
            pk.Gender = criteria.GetGender(Gender, pi);
            var av = GetAbilityIndex(criteria, pi);
            pk.RefreshAbility(av);
            SetPID(pk, SAV);
            SetIVs(pk);
        }

        private int GetAbilityIndex(EncounterCriteria criteria, PersonalInfo pi)
        {
            switch (AbilityType)
            {
                case 00: // 0 - 0
                case 01: // 1 - 1
                case 02: // 2 - H
                    return AbilityType;
                case 03: // 0/1
                case 04: // 0/1/H
                    return criteria.GetAbility(AbilityType, pi); // 3 or 2
                default:
                    throw new ArgumentException(nameof(AbilityType));
            }
        }

        private uint GetFixedPID(ITrainerInfo SAV)
        {
            uint pid = PID;
            var val = Data[CardStart + 0x248];
            if (val == 4)
                return pid;
            return (uint)((pid & 0xFFFF) | ((SAV.SID ^ SAV.TID ^ (pid & 0xFFFF) ^ (val == 2 ? 1 : 0)) << 16));
        }

        private void SetPID(PKM pk, ITrainerInfo SAV)
        {
            switch (PIDType)
            {
                case Shiny.FixedValue: // Specified
                    pk.PID = GetFixedPID(SAV);
                    break;
                case Shiny.Random: // Random
                    pk.PID = Util.Rand32();
                    break;
                case Shiny.AlwaysStar: // Random Shiny
                    pk.PID = Util.Rand32();
                    pk.PID = (uint)(((pk.TID ^ pk.SID ^ (pk.PID & 0xFFFF) ^ 1) << 16) | (pk.PID & 0xFFFF));
                    break;
                case Shiny.AlwaysSquare: // Random Shiny
                    pk.PID = Util.Rand32();
                    pk.PID = (uint)(((pk.TID ^ pk.SID ^ (pk.PID & 0xFFFF) ^ 0) << 16) | (pk.PID & 0xFFFF));
                    break;
                case Shiny.Never: // Random Nonshiny
                    pk.PID = Util.Rand32();
                    if (pk.IsShiny) pk.PID ^= 0x10000000;
                    break;
            }
        }

        private void SetIVs(PKM pk)
        {
            int[] finalIVs = new int[6];
            var ivflag = Array.Find(IVs, iv => (byte)(iv - 0xFC) < 3);
            var rng = Util.Rand;
            if (ivflag == 0) // Random IVs
            {
                for (int i = 0; i < 6; i++)
                    finalIVs[i] = IVs[i] > 31 ? rng.Next(32) : IVs[i];
            }
            else // 1/2/3 perfect IVs
            {
                int IVCount = ivflag - 0xFB;
                do { finalIVs[rng.Next(6)] = 31; }
                while (finalIVs.Count(iv => iv == 31) < IVCount);
                for (int i = 0; i < 6; i++)
                    finalIVs[i] = finalIVs[i] == 31 ? 31 : rng.Next(32);
            }
            pk.IVs = finalIVs;
        }

        protected override bool IsMatchExact(PKM pkm)
        {
            if (pkm.Egg_Location == 0) // Not Egg
            {
                if (OTGender < 2)
                {
                    if (SID != pkm.SID) return false;
                    if (TID != pkm.TID) return false;
                    if (OTGender != pkm.OT_Gender) return false;
                }
                var OT = GetOT(pkm.Language); // May not be guaranteed to work.
                if (!string.IsNullOrEmpty(OT) && OT != pkm.OT_Name) return false;
                if (OriginGame != 0 && OriginGame != pkm.Version) return false;
                if (EncryptionConstant != 0)
                {
                    if (EncryptionConstant != pkm.EncryptionConstant)
                        return false;
                }
                else if (IsHOMEGift)// 0
                {
                    // HOME gifts -- PID and EC are zeroes...
                    if (EncryptionConstant != pkm.EncryptionConstant)
                        return false;
                    if (IsShiny)
                        return false;
                }
            }

            if (Form != pkm.AltForm && !Legal.IsFormChangeable(pkm, Species))
                return false;

            if (IsEgg)
            {
                if (EggLocation != pkm.Egg_Location) // traded
                {
                    if (pkm.Egg_Location != Locations.LinkTrade6)
                        return false;
                    if (PIDType == Shiny.Random && pkm.IsShiny && pkm.ShinyXor > 1)
                        return false; // shiny traded egg will always have xor0/1.
                }
                if (!PIDType.IsValid(pkm))
                {
                    return false; // can't be traded away for unshiny
                }

                if (pkm.IsEgg && !pkm.IsNative)
                    return false;
            }
            else
            {
                if (!PIDType.IsValid(pkm)) return false;
                if (EggLocation != pkm.Egg_Location) return false;
                if (MetLocation != pkm.Met_Location) return false;
            }

            if (MetLevel != 0 && MetLevel != pkm.Met_Level) return false;
            if (Ball != 0 && Ball != pkm.Ball) return false;
            if (Ball == 0 && pkm.Ball != 4) return false;
            if (OTGender < 2 && OTGender != pkm.OT_Gender) return false;
            if (Nature != -1 && pkm.Nature != Nature) return false;
            if (Gender != 3 && Gender != pkm.Gender) return false;

            if (!(pkm is IGigantamax g && g.CanGigantamax == CanGigantamax))
                return false;

            if (!(pkm is IDynamaxLevel dl && dl.DynamaxLevel >= DynamaxLevel))
                return false;

            return PIDType != 0 || pkm.PID == PID;
        }

        protected override bool IsMatchDeferred(PKM pkm)
        {
            return pkm.Species == Species;
        }

        #region Lazy Ribbon Implementation
        public bool RibbonEarth { get => this.GetRibbonIndex(Earth); set => this.SetRibbonIndex(Earth, value); }
        public bool RibbonNational { get => this.GetRibbonIndex(National); set => this.SetRibbonIndex(National, value); }
        public bool RibbonCountry { get => this.GetRibbonIndex(Country); set => this.SetRibbonIndex(Country, value); }
        public bool RibbonChampionBattle { get => this.GetRibbonIndex(ChampionBattle); set => this.SetRibbonIndex(ChampionBattle, value); }
        public bool RibbonChampionRegional { get => this.GetRibbonIndex(ChampionRegional); set => this.SetRibbonIndex(ChampionRegional, value); }
        public bool RibbonChampionNational { get => this.GetRibbonIndex(ChampionNational); set => this.SetRibbonIndex(ChampionNational, value); }
        public bool RibbonClassic { get => this.GetRibbonIndex(Classic); set => this.SetRibbonIndex(Classic, value); }
        public bool RibbonWishing { get => this.GetRibbonIndex(Wishing); set => this.SetRibbonIndex(Wishing, value); }
        public bool RibbonPremier { get => this.GetRibbonIndex(Premier); set => this.SetRibbonIndex(Premier, value); }
        public bool RibbonEvent { get => this.GetRibbonIndex(Event); set => this.SetRibbonIndex(Event, value); }
        public bool RibbonBirthday { get => this.GetRibbonIndex(Birthday); set => this.SetRibbonIndex(Birthday, value); }
        public bool RibbonSpecial { get => this.GetRibbonIndex(Special); set => this.SetRibbonIndex(Special, value); }
        public bool RibbonWorld { get => this.GetRibbonIndex(World); set => this.SetRibbonIndex(World, value); }
        public bool RibbonChampionWorld { get => this.GetRibbonIndex(ChampionWorld); set => this.SetRibbonIndex(ChampionWorld, value); }
        public bool RibbonSouvenir { get => this.GetRibbonIndex(Souvenir); set => this.SetRibbonIndex(Souvenir, value); }
        public bool RibbonChampionG3Hoenn { get => this.GetRibbonIndex(ChampionG3Hoenn); set => this.SetRibbonIndex(ChampionG3Hoenn, value); }
        public bool RibbonArtist { get => this.GetRibbonIndex(Artist); set => this.SetRibbonIndex(Artist, value); }
        public bool RibbonEffort { get => this.GetRibbonIndex(Effort); set => this.SetRibbonIndex(Effort, value); }
        public bool RibbonChampionSinnoh { get => this.GetRibbonIndex(ChampionSinnoh); set => this.SetRibbonIndex(ChampionSinnoh, value); }
        public bool RibbonAlert { get => this.GetRibbonIndex(Alert); set => this.SetRibbonIndex(Alert, value); }
        public bool RibbonShock { get => this.GetRibbonIndex(Shock); set => this.SetRibbonIndex(Shock, value); }
        public bool RibbonDowncast { get => this.GetRibbonIndex(Downcast); set => this.SetRibbonIndex(Downcast, value); }
        public bool RibbonCareless { get => this.GetRibbonIndex(Careless); set => this.SetRibbonIndex(Careless, value); }
        public bool RibbonRelax { get => this.GetRibbonIndex(Relax); set => this.SetRibbonIndex(Relax, value); }
        public bool RibbonSnooze { get => this.GetRibbonIndex(Snooze); set => this.SetRibbonIndex(Snooze, value); }
        public bool RibbonSmile { get => this.GetRibbonIndex(Smile); set => this.SetRibbonIndex(Smile, value); }
        public bool RibbonGorgeous { get => this.GetRibbonIndex(Gorgeous); set => this.SetRibbonIndex(Gorgeous, value); }
        public bool RibbonRoyal { get => this.GetRibbonIndex(Royal); set => this.SetRibbonIndex(Royal, value); }
        public bool RibbonGorgeousRoyal { get => this.GetRibbonIndex(GorgeousRoyal); set => this.SetRibbonIndex(GorgeousRoyal, value); }
        public bool RibbonFootprint { get => this.GetRibbonIndex(Footprint); set => this.SetRibbonIndex(Footprint, value); }
        public bool RibbonRecord { get => this.GetRibbonIndex(Record); set => this.SetRibbonIndex(Record, value); }
        public bool RibbonLegend { get => this.GetRibbonIndex(Legend); set => this.SetRibbonIndex(Legend, value); }
        public bool RibbonChampionKalos { get => this.GetRibbonIndex(ChampionKalos); set => this.SetRibbonIndex(ChampionKalos, value); }
        public bool RibbonChampionG6Hoenn { get => this.GetRibbonIndex(ChampionG6Hoenn); set => this.SetRibbonIndex(ChampionG6Hoenn, value); }
        public bool RibbonBestFriends { get => this.GetRibbonIndex(BestFriends); set => this.SetRibbonIndex(BestFriends, value); }
        public bool RibbonTraining { get => this.GetRibbonIndex(Training); set => this.SetRibbonIndex(Training, value); }
        public bool RibbonBattlerSkillful { get => this.GetRibbonIndex(BattlerSkillful); set => this.SetRibbonIndex(BattlerSkillful, value); }
        public bool RibbonBattlerExpert { get => this.GetRibbonIndex(BattlerExpert); set => this.SetRibbonIndex(BattlerExpert, value); }
        public bool RibbonContestStar { get => this.GetRibbonIndex(ContestStar); set => this.SetRibbonIndex(ContestStar, value); }
        public bool RibbonMasterCoolness { get => this.GetRibbonIndex(MasterCoolness); set => this.SetRibbonIndex(MasterCoolness, value); }
        public bool RibbonMasterBeauty { get => this.GetRibbonIndex(MasterBeauty); set => this.SetRibbonIndex(MasterBeauty, value); }
        public bool RibbonMasterCuteness { get => this.GetRibbonIndex(MasterCuteness); set => this.SetRibbonIndex(MasterCuteness, value); }
        public bool RibbonMasterCleverness { get => this.GetRibbonIndex(MasterCleverness); set => this.SetRibbonIndex(MasterCleverness, value); }
        public bool RibbonMasterToughness { get => this.GetRibbonIndex(MasterToughness); set => this.SetRibbonIndex(MasterToughness, value); }

        public int RibbonCountMemoryContest { get => 0; set { } }
        public int RibbonCountMemoryBattle { get => 0; set { } }

        public bool RibbonChampionAlola { get => this.GetRibbonIndex(ChampionAlola); set => this.SetRibbonIndex(ChampionAlola, value); }
        public bool RibbonBattleRoyale { get => this.GetRibbonIndex(BattleRoyale); set => this.SetRibbonIndex(BattleRoyale, value); }
        public bool RibbonBattleTreeGreat { get => this.GetRibbonIndex(BattleTreeGreat); set => this.SetRibbonIndex(BattleTreeGreat, value); }
        public bool RibbonBattleTreeMaster { get => this.GetRibbonIndex(BattleTreeMaster); set => this.SetRibbonIndex(BattleTreeMaster, value); }
        public bool RibbonChampionGalar { get => this.GetRibbonIndex(ChampionGalar); set => this.SetRibbonIndex(ChampionGalar, value); }
        public bool RibbonTowerMaster { get => this.GetRibbonIndex(TowerMaster); set => this.SetRibbonIndex(TowerMaster, value); }
        public bool RibbonMasterRank { get => this.GetRibbonIndex(MasterRank); set => this.SetRibbonIndex(MasterRank, value); }
        public bool RibbonMarkLunchtime { get => this.GetRibbonIndex(MarkLunchtime); set => this.SetRibbonIndex(MarkLunchtime, value); }
        public bool RibbonMarkSleepyTime { get => this.GetRibbonIndex(MarkSleepyTime); set => this.SetRibbonIndex(MarkSleepyTime, value); }
        public bool RibbonMarkDusk { get => this.GetRibbonIndex(MarkDusk); set => this.SetRibbonIndex(MarkDusk, value); }
        public bool RibbonMarkDawn { get => this.GetRibbonIndex(MarkDawn); set => this.SetRibbonIndex(MarkDawn, value); }
        public bool RibbonMarkCloudy { get => this.GetRibbonIndex(MarkCloudy); set => this.SetRibbonIndex(MarkCloudy, value); }
        public bool RibbonMarkRainy { get => this.GetRibbonIndex(MarkRainy); set => this.SetRibbonIndex(MarkRainy, value); }
        public bool RibbonMarkStormy { get => this.GetRibbonIndex(MarkStormy); set => this.SetRibbonIndex(MarkStormy, value); }
        public bool RibbonMarkSnowy { get => this.GetRibbonIndex(MarkSnowy); set => this.SetRibbonIndex(MarkSnowy, value); }
        public bool RibbonMarkBlizzard { get => this.GetRibbonIndex(MarkBlizzard); set => this.SetRibbonIndex(MarkBlizzard, value); }
        public bool RibbonMarkDry { get => this.GetRibbonIndex(MarkDry); set => this.SetRibbonIndex(MarkDry, value); }
        public bool RibbonMarkSandstorm { get => this.GetRibbonIndex(MarkSandstorm); set => this.SetRibbonIndex(MarkSandstorm, value); }
        public bool RibbonMarkMisty { get => this.GetRibbonIndex(MarkMisty); set => this.SetRibbonIndex(MarkMisty, value); }
        public bool RibbonMarkDestiny { get => this.GetRibbonIndex(MarkDestiny); set => this.SetRibbonIndex(MarkDestiny, value); }
        public bool RibbonMarkFishing { get => this.GetRibbonIndex(MarkFishing); set => this.SetRibbonIndex(MarkFishing, value); }
        public bool RibbonMarkCurry { get => this.GetRibbonIndex(MarkCurry); set => this.SetRibbonIndex(MarkCurry, value); }
        public bool RibbonMarkUncommon { get => this.GetRibbonIndex(MarkUncommon); set => this.SetRibbonIndex(MarkUncommon, value); }
        public bool RibbonMarkRare { get => this.GetRibbonIndex(MarkRare); set => this.SetRibbonIndex(MarkRare, value); }
        public bool RibbonMarkRowdy { get => this.GetRibbonIndex(MarkRowdy); set => this.SetRibbonIndex(MarkRowdy, value); }
        public bool RibbonMarkAbsentMinded { get => this.GetRibbonIndex(MarkAbsentMinded); set => this.SetRibbonIndex(MarkAbsentMinded, value); }
        public bool RibbonMarkJittery { get => this.GetRibbonIndex(MarkJittery); set => this.SetRibbonIndex(MarkJittery, value); }
        public bool RibbonMarkExcited { get => this.GetRibbonIndex(MarkExcited); set => this.SetRibbonIndex(MarkExcited, value); }
        public bool RibbonMarkCharismatic { get => this.GetRibbonIndex(MarkCharismatic); set => this.SetRibbonIndex(MarkCharismatic, value); }
        public bool RibbonMarkCalmness { get => this.GetRibbonIndex(MarkCalmness); set => this.SetRibbonIndex(MarkCalmness, value); }
        public bool RibbonMarkIntense { get => this.GetRibbonIndex(MarkIntense); set => this.SetRibbonIndex(MarkIntense, value); }
        public bool RibbonMarkZonedOut { get => this.GetRibbonIndex(MarkZonedOut); set => this.SetRibbonIndex(MarkZonedOut, value); }
        public bool RibbonMarkJoyful { get => this.GetRibbonIndex(MarkJoyful); set => this.SetRibbonIndex(MarkJoyful, value); }
        public bool RibbonMarkAngry { get => this.GetRibbonIndex(MarkAngry); set => this.SetRibbonIndex(MarkAngry, value); }
        public bool RibbonMarkSmiley { get => this.GetRibbonIndex(MarkSmiley); set => this.SetRibbonIndex(MarkSmiley, value); }
        public bool RibbonMarkTeary { get => this.GetRibbonIndex(MarkTeary); set => this.SetRibbonIndex(MarkTeary, value); }
        public bool RibbonMarkUpbeat { get => this.GetRibbonIndex(MarkUpbeat); set => this.SetRibbonIndex(MarkUpbeat, value); }
        public bool RibbonMarkPeeved { get => this.GetRibbonIndex(MarkPeeved); set => this.SetRibbonIndex(MarkPeeved, value); }
        public bool RibbonMarkIntellectual { get => this.GetRibbonIndex(MarkIntellectual); set => this.SetRibbonIndex(MarkIntellectual, value); }
        public bool RibbonMarkFerocious { get => this.GetRibbonIndex(MarkFerocious); set => this.SetRibbonIndex(MarkFerocious, value); }
        public bool RibbonMarkCrafty { get => this.GetRibbonIndex(MarkCrafty); set => this.SetRibbonIndex(MarkCrafty, value); }
        public bool RibbonMarkScowling { get => this.GetRibbonIndex(MarkScowling); set => this.SetRibbonIndex(MarkScowling, value); }
        public bool RibbonMarkKindly { get => this.GetRibbonIndex(MarkKindly); set => this.SetRibbonIndex(MarkKindly, value); }
        public bool RibbonMarkFlustered { get => this.GetRibbonIndex(MarkFlustered); set => this.SetRibbonIndex(MarkFlustered, value); }
        public bool RibbonMarkPumpedUp { get => this.GetRibbonIndex(MarkPumpedUp); set => this.SetRibbonIndex(MarkPumpedUp, value); }
        public bool RibbonMarkZeroEnergy { get => this.GetRibbonIndex(MarkZeroEnergy); set => this.SetRibbonIndex(MarkZeroEnergy, value); }
        public bool RibbonMarkPrideful { get => this.GetRibbonIndex(MarkPrideful); set => this.SetRibbonIndex(MarkPrideful, value); }
        public bool RibbonMarkUnsure { get => this.GetRibbonIndex(MarkUnsure); set => this.SetRibbonIndex(MarkUnsure, value); }
        public bool RibbonMarkHumble { get => this.GetRibbonIndex(MarkHumble); set => this.SetRibbonIndex(MarkHumble, value); }
        public bool RibbonMarkThorny { get => this.GetRibbonIndex(MarkThorny); set => this.SetRibbonIndex(MarkThorny, value); }
        public bool RibbonMarkVigor { get => this.GetRibbonIndex(MarkVigor); set => this.SetRibbonIndex(MarkVigor, value); }
        public bool RibbonMarkSlump { get => this.GetRibbonIndex(MarkSlump); set => this.SetRibbonIndex(MarkSlump, value); }

        public int GetRibbonByte(int index) => Array.FindIndex(Data, RibbonBytesOffset, RibbonBytesCount, z => z == index);
        public bool GetRibbon(int index) => GetRibbonByte(index) >= 0;

        public void SetRibbon(int index, bool value = true)
        {
            if ((uint)index > (uint)MarkSlump)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (value)
            {
                if (GetRibbon(index))
                    return;
                var openIndex = Array.FindIndex(Data, RibbonBytesOffset, RibbonBytesCount, z => z != RibbonByteNone);
                if (openIndex < 0)
                    throw new ArgumentOutOfRangeException(nameof(index));
                SetRibbonAtIndex(openIndex, (byte)index);
            }
            else
            {
                var ofs = GetRibbonByte(index);
                if (ofs < 0)
                    return;
                SetRibbonAtIndex(ofs, RibbonByteNone);
            }
        }
        #endregion
    }
}
