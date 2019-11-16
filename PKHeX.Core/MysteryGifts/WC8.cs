using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 8 Mystery Gift Template File
    /// </summary>
    public sealed class WC8 : DataMysteryGift, ILangNick, INature, IGigantamax, IDynamaxLevel
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
        public override bool GiftUsed { get => false; set => throw new Exception(); }

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
                switch (Data[CardStart + 0x248])
                {
                    case 0:
                        return Shiny.Never;
                    case 1:
                        return Shiny.Random;
                    case 2: // Fixed never shiny
                    case 3: // Fixed always shiny
                    case 4:
                        return Shiny.FixedValue;
                    default:
                        throw new ArgumentException();
                }
            }
        }

        public int MetLevel { get => Data[CardStart + 0x249]; set => Data[CardStart + 0x249] = (byte)value; }
        public byte DynamaxLevel { get => Data[CardStart + 0x24A]; set => Data[CardStart + 0x24A] = value; }
        public bool CanGigantamax { get => Data[CardStart + 0x24B] != 0; set => Data[CardStart + 0x24B] = (byte)(value ? 1 : 0); }

        // Ribbons 0x24C-0x26C
        public byte GetRibbon(int index)
        {
            if (index >= 0x20) throw new IndexOutOfRangeException();
            return Data[0x24C + index];
        }

        public void SetRibbon(int index, byte value)
        {
            if (index >= 0x20) throw new IndexOutOfRangeException();
            Data[0x24C + index] = value;
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

        private int GetLanguageIndex(int language)
        {
            var lang = (LanguageID) language;
            if (lang < LanguageID.Japanese || lang == LanguageID.UNUSED_6)
                return (int) LanguageID.English; // fallback
            return lang < LanguageID.UNUSED_6 ? language - 1 : language - 2;
        }

        public override int Location { get => MetLocation; set => MetLocation = (ushort)value; }

        public override int[] Moves
        {
            get => new[] { Move1, Move2, Move3, Move4 };
            set
            {
                if (value.Length > 0) Move1 = value[0];
                if (value.Length > 1) Move2 = value[1];
                if (value.Length > 2) Move3 = value[2];
                if (value.Length > 3) Move4 = value[3];
            }
        }

        public override int[] RelearnMoves
        {
            get => new[] { RelearnMove1, RelearnMove2, RelearnMove3, RelearnMove4 };
            set
            {
                if (value.Length > 0) RelearnMove1 = value[0];
                if (value.Length > 1) RelearnMove2 = value[1];
                if (value.Length > 2) RelearnMove3 = value[2];
                if (value.Length > 3) RelearnMove4 = value[3];
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

        private int GetNicknameOffset(int language)
        {
            int index = GetLanguageIndex(language);
            return 0x30 + (index * 0x1C);
        }

        private int GetOTOffset(int language)
        {
            int index = GetLanguageIndex(language);
            return 0x12C + (index * 0x1C);
        }

        public override PKM ConvertToPKM(ITrainerInfo SAV, EncounterCriteria criteria)
        {
            if (!IsPokémon)
                throw new ArgumentException(nameof(IsPokémon));

            int currentLevel = Level > 0 ? Level : Util.Rand.Next(100) + 1;
            int metLevel = MetLevel > 0 ? MetLevel : currentLevel;
            var pi = PersonalTable.SWSH.GetFormeEntry(Species, Form);
            var OT = GetOT(SAV.Language);

            var pk = new PK8
            {
                EncryptionConstant = EncryptionConstant != 0 ? EncryptionConstant : Util.Rand32(),
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
                do { pk.Version = (int)GameVersion.SW + Util.Rand.Next(2); }
                while (!CanBeReceivedByVersion(pk.Version));
            }

            if (OTGender >= 2)
            {
                pk.TID = SAV.TID;
                pk.SID = SAV.SID;
            }

            // Official code explicitly corrects for meowstic
            if (pk.Species == 678)
                pk.AltForm = pk.Gender;

            pk.MetDate = DateTime.Now;

            var nickname_language = GetNicknameLanguage(SAV.Language);
            pk.Language = nickname_language != 0 ? nickname_language : SAV.Language;
            pk.IsNicknamed = GetIsNicknamed(SAV.Language);
            pk.Nickname = pk.IsNicknamed ? Nickname : SpeciesName.GetSpeciesNameGeneration(Species, pk.Language, Format);

            for (var i = 0; i < 0x20; i++)
            {
                var ribbon = GetRibbon(i);
                if (ribbon != 0xFF)
                    pk.SetRibbon(ribbon);
            }

            SetPINGA(pk, SAV, criteria);

            if (IsEgg)
                SetEggMetData(pk);
            pk.CurrentFriendship = pk.IsEgg ? pi.HatchCycles : pi.BaseFriendship;

            pk.HeightScalar = PokeSizeExtensions.GetRandomPokeSize();
            pk.WeightScalar = PokeSizeExtensions.GetRandomPokeSize();

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
            var pi = PersonalTable.USUM.GetFormeEntry(Species, Form);
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
                case Shiny.Always: // Random Shiny
                    pk.PID = Util.Rand32();
                    pk.PID = (uint)(((pk.TID ^ pk.SID ^ (pk.PID & 0xFFFF)) << 16) | (pk.PID & 0xFFFF));
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
            if (ivflag == 0) // Random IVs
            {
                for (int i = 0; i < 6; i++)
                    finalIVs[i] = IVs[i] > 31 ? Util.Rand.Next(pk.MaxIV + 1) : IVs[i];
            }
            else // 1/2/3 perfect IVs
            {
                int IVCount = ivflag - 0xFB;
                do { finalIVs[Util.Rand.Next(6)] = 31; }
                while (finalIVs.Count(iv => iv == 31) < IVCount);
                for (int i = 0; i < 6; i++)
                    finalIVs[i] = finalIVs[i] == 31 ? pk.MaxIV : Util.Rand.Next(pk.MaxIV + 1);
            }
            pk.IVs = finalIVs;
        }

        protected override bool IsMatchExact(PKM pkm, IEnumerable<DexLevel> vs)
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
                if (EncryptionConstant != 0 && EncryptionConstant != pkm.EncryptionConstant) return false;
            }

            if (Form != pkm.AltForm && vs.All(z => !Legal.IsFormChangeable(pkm, z.Species)))
                return false;

            if (IsEgg)
            {
                if (EggLocation != pkm.Egg_Location) // traded
                {
                    if (pkm.Egg_Location != Locations.LinkTrade6)
                        return false;
                }
                else if (PIDType == 0 && pkm.IsShiny)
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
    }
}
