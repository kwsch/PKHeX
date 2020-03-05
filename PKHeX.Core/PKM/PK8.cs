using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary> Generation 8 <see cref="PKM"/> format. </summary>
    public sealed class PK8 : PKM,
        IRibbonSetEvent3, IRibbonSetEvent4, IRibbonSetCommon3, IRibbonSetCommon4, IRibbonSetCommon6, IRibbonSetCommon7, IRibbonSetCommon8, IRibbonSetMark8,
        IContestStats, IHyperTrain, IScaledSize, IGigantamax, IFavorite, IDynamaxLevel, IRibbonIndex, IHandlerLanguage, IFormArgument, IHomeTrack
    {
        private static readonly ushort[] Unused =
        {
            // Alignment bytes
            0x17, 0x1A, 0x1B, 0x23, 0x33, 0x3E, 0x3F,
            0xC5, 0x115, 0x11F,
        };

        public override IReadOnlyList<ushort> ExtraBytes => Unused;
        public override int Format => 8;
        public override PersonalInfo PersonalInfo => PersonalTable.SWSH.GetFormeEntry(Species, AltForm);

        public override byte[] Data { get; }

        public PK8()
        {
            Data = new byte[PokeCrypto.SIZE_8PARTY];
            AffixedRibbon = -1; // 00 would make it show Kalos Champion :)
        }

        protected override ushort CalculateChecksum()
        {
            ushort chk = 0;
            for (int i = 8; i < PokeCrypto.SIZE_8STORED; i += 2)
                chk += BitConverter.ToUInt16(Data, i);
            return chk;
        }

        // Simple Generated Attributes
        public override int CurrentFriendship
        {
            get => CurrentHandler == 0 ? OT_Friendship : HT_Friendship;
            set { if (CurrentHandler == 0) OT_Friendship = value; else HT_Friendship = value; }
        }

        public int OppositeFriendship
        {
            get => CurrentHandler == 1 ? OT_Friendship : HT_Friendship;
            set { if (CurrentHandler == 1) OT_Friendship = value; else HT_Friendship = value; }
        }

        public PK8(byte[] data)
        {
            PokeCrypto.DecryptIfEncrypted8(ref data);
            if (data.Length != PokeCrypto.SIZE_8PARTY)
                Array.Resize(ref data, PokeCrypto.SIZE_8PARTY);
            Data = data;
        }

        public override PKM Clone() => new PK8((byte[])Data.Clone()) { Identifier = Identifier };

        private string GetString(int Offset, int Count) => StringConverter.GetString7(Data, Offset, Count);
        private byte[] SetString(string value, int maxLength, bool chinese = false) => StringConverter.SetString7(value, maxLength, Language, chinese: chinese);

        public override int SIZE_PARTY => PokeCrypto.SIZE_8PARTY;
        public override int SIZE_STORED => PokeCrypto.SIZE_8STORED;

        // Trash Bytes
        public override byte[] Nickname_Trash { get => GetData(0x58, 24); set { if (value.Length == 24) value.CopyTo(Data, 0x58); } }
        public override byte[] HT_Trash { get => GetData(0xA8, 24); set { if (value.Length == 24) value.CopyTo(Data, 0xA8); } }
        public override byte[] OT_Trash { get => GetData(0xF8, 24); set { if (value.Length == 24) value.CopyTo(Data, 0xF8); } }
        public override bool WasLink => Met_Location == Locations.LinkGift6 && Gen6;
        public override bool WasEvent => Locations.IsEventLocation5(Met_Location) || FatefulEncounter;
        public override bool WasEventEgg => GenNumber < 5 ? base.WasEventEgg : (Locations.IsEventLocation5(Egg_Location) || (FatefulEncounter && Egg_Location == Locations.LinkTrade6)) && Met_Level == 1;

        // Maximums
        public override int MaxIV => 31;
        public override int MaxEV => 252;
        public override int OTLength => 12;
        public override int NickLength => 12;

        public override int PSV => (int)((PID >> 16 ^ (PID & 0xFFFF)) >> 4);
        public override int TSV => (TID ^ SID) >> 4;
        public override bool IsUntraded => Data[0xA8] == 0 && Data[0xA8 + 1] == 0 && Format == GenNumber; // immediately terminated HT_Name data (\0)

        // Complex Generated Attributes
        public override int Characteristic
        {
            get
            {
                int pm6 = (int)(EncryptionConstant % 6);
                int maxIV = MaximumIV;
                int pm6stat = 0;
                for (int i = 0; i < 6; i++)
                {
                    pm6stat = (pm6 + i) % 6;
                    if (GetIV(pm6stat) == maxIV)
                        break;
                }
                return (pm6stat * 5) + (maxIV % 5);
            }
        }

        // Methods
        protected override byte[] Encrypt()
        {
            RefreshChecksum();
            return PokeCrypto.EncryptArray8(Data);
        }

        public void FixRelearn()
        {
            while (true)
            {
                if (RelearnMove4 != 0 && RelearnMove3 == 0)
                {
                    RelearnMove3 = RelearnMove4;
                    RelearnMove4 = 0;
                }
                if (RelearnMove3 != 0 && RelearnMove2 == 0)
                {
                    RelearnMove2 = RelearnMove3;
                    RelearnMove3 = 0;
                    continue;
                }
                if (RelearnMove2 != 0 && RelearnMove1 == 0)
                {
                    RelearnMove1 = RelearnMove2;
                    RelearnMove2 = 0;
                    continue;
                }
                break;
            }
        }

        public void Trade(ITrainerInfo tr, int Day = 1, int Month = 1, int Year = 2015)
        {
            if (IsEgg)
            {
                // Eggs do not have any modifications done if they are traded
                // Apply link trade data, only if it left the OT (ignore if dumped & imported, or cloned, etc)
                if ((tr.OT != OT_Name) || (tr.TID != TID) || (tr.SID != SID) || (tr.Gender != OT_Gender))
                    SetLinkTradeEgg(Day, Month, Year, Locations.LinkTrade6);
                return;
            }

            // Process to the HT if the OT of the Pokémon does not match the SAV's OT info.
            if (!TradeOT(tr))
                TradeHT(tr);
        }

        public override uint EncryptionConstant { get => BitConverter.ToUInt32(Data, 0x00); set => BitConverter.GetBytes(value).CopyTo(Data, 0x00); }
        public override ushort Sanity { get => BitConverter.ToUInt16(Data, 0x04); set => BitConverter.GetBytes(value).CopyTo(Data, 0x04); }
        public override ushort Checksum { get => BitConverter.ToUInt16(Data, 0x06); set => BitConverter.GetBytes(value).CopyTo(Data, 0x06); }

        // Structure
        #region Block A
        public override int Species { get => BitConverter.ToUInt16(Data, 0x08); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x08); }
        public override int HeldItem { get => BitConverter.ToUInt16(Data, 0x0A); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x0A); }
        public override int TID { get => BitConverter.ToUInt16(Data, 0x0C); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x0C); }
        public override int SID { get => BitConverter.ToUInt16(Data, 0x0E); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x0E); }
        public override uint EXP { get => BitConverter.ToUInt32(Data, 0x10); set => BitConverter.GetBytes(value).CopyTo(Data, 0x10); }
        public override int Ability { get => BitConverter.ToUInt16(Data, 0x14); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x14); }
        public override int AbilityNumber { get => Data[0x16] & 7; set => Data[0x16] = (byte)((Data[0x16] & ~7) | (value & 7)); }
        public bool Favorite { get => (Data[0x16] & 8) != 0; set => Data[0x16] = (byte)((Data[0x16] & ~8) | ((value ? 1 : 0) << 3)); } // unused, was in LGPE but not in SWSH
        public bool CanGigantamax { get => (Data[0x16] & 16) != 0; set => Data[0x16] = (byte)((Data[0x16] & ~16) | (value ? 16 : 0)); }
        // 0x17 alignment unused
        public override int MarkValue { get => BitConverter.ToUInt16(Data, 0x18); protected set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x18); }
        // 0x1A alignment unused
        // 0x1B alignment unused
        public override uint PID { get => BitConverter.ToUInt32(Data, 0x1C); set => BitConverter.GetBytes(value).CopyTo(Data, 0x1C); }
        public override int Nature { get => Data[0x20]; set => Data[0x20] = (byte)value; }
        public override int StatNature { get => Data[0x21]; set => Data[0x21] = (byte)value; }
        public override bool FatefulEncounter { get => (Data[0x22] & 1) == 1; set => Data[0x22] = (byte)((Data[0x22] & ~0x01) | (value ? 1 : 0)); }
        public bool Flag2 { get => (Data[0x22] & 2) == 2; set => Data[0x22] = (byte)((Data[0x22] & ~0x02) | (value ? 2 : 0)); }
        public override int Gender { get => (Data[0x22] >> 2) & 0x3; set => Data[0x22] = (byte)((Data[0x22] & 0xF3) | (value << 2)); }
        // 0x23 alignment unused

        public override int AltForm { get => BitConverter.ToUInt16(Data, 0x24); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x24); }
        public override int EV_HP { get => Data[0x26]; set => Data[0x26] = (byte)value; }
        public override int EV_ATK { get => Data[0x27]; set => Data[0x27] = (byte)value; }
        public override int EV_DEF { get => Data[0x28]; set => Data[0x28] = (byte)value; }
        public override int EV_SPE { get => Data[0x29]; set => Data[0x29] = (byte)value; }
        public override int EV_SPA { get => Data[0x2A]; set => Data[0x2A] = (byte)value; }
        public override int EV_SPD { get => Data[0x2B]; set => Data[0x2B] = (byte)value; }
        public int CNT_Cool { get => Data[0x2C]; set => Data[0x2C] = (byte)value; }
        public int CNT_Beauty { get => Data[0x2D]; set => Data[0x2D] = (byte)value; }
        public int CNT_Cute { get => Data[0x2E]; set => Data[0x2E] = (byte)value; }
        public int CNT_Smart { get => Data[0x2F]; set => Data[0x2F] = (byte)value; }
        public int CNT_Tough { get => Data[0x30]; set => Data[0x30] = (byte)value; }
        public int CNT_Sheen { get => Data[0x31]; set => Data[0x31] = (byte)value; }
        private byte PKRS { get => Data[0x32]; set => Data[0x32] = value; }
        public override int PKRS_Days { get => PKRS & 0xF; set => PKRS = (byte)((PKRS & ~0xF) | value); }
        public override int PKRS_Strain { get => PKRS >> 4; set => PKRS = (byte)((PKRS & 0xF) | value << 4); }
        // 0x33 unused padding

        // ribbon u32
        public bool RibbonChampionKalos    { get => FlagUtil.GetFlag(Data, 0x34, 0); set => FlagUtil.SetFlag(Data, 0x34, 0, value); }
        public bool RibbonChampionG3Hoenn  { get => FlagUtil.GetFlag(Data, 0x34, 1); set => FlagUtil.SetFlag(Data, 0x34, 1, value); }
        public bool RibbonChampionSinnoh   { get => FlagUtil.GetFlag(Data, 0x34, 2); set => FlagUtil.SetFlag(Data, 0x34, 2, value); }
        public bool RibbonBestFriends      { get => FlagUtil.GetFlag(Data, 0x34, 3); set => FlagUtil.SetFlag(Data, 0x34, 3, value); }
        public bool RibbonTraining         { get => FlagUtil.GetFlag(Data, 0x34, 4); set => FlagUtil.SetFlag(Data, 0x34, 4, value); }
        public bool RibbonBattlerSkillful  { get => FlagUtil.GetFlag(Data, 0x34, 5); set => FlagUtil.SetFlag(Data, 0x34, 5, value); }
        public bool RibbonBattlerExpert    { get => FlagUtil.GetFlag(Data, 0x34, 6); set => FlagUtil.SetFlag(Data, 0x34, 6, value); }
        public bool RibbonEffort           { get => FlagUtil.GetFlag(Data, 0x34, 7); set => FlagUtil.SetFlag(Data, 0x34, 7, value); }

        public bool RibbonAlert            { get => FlagUtil.GetFlag(Data, 0x35, 0); set => FlagUtil.SetFlag(Data, 0x35, 0, value); }
        public bool RibbonShock            { get => FlagUtil.GetFlag(Data, 0x35, 1); set => FlagUtil.SetFlag(Data, 0x35, 1, value); }
        public bool RibbonDowncast         { get => FlagUtil.GetFlag(Data, 0x35, 2); set => FlagUtil.SetFlag(Data, 0x35, 2, value); }
        public bool RibbonCareless         { get => FlagUtil.GetFlag(Data, 0x35, 3); set => FlagUtil.SetFlag(Data, 0x35, 3, value); }
        public bool RibbonRelax            { get => FlagUtil.GetFlag(Data, 0x35, 4); set => FlagUtil.SetFlag(Data, 0x35, 4, value); }
        public bool RibbonSnooze           { get => FlagUtil.GetFlag(Data, 0x35, 5); set => FlagUtil.SetFlag(Data, 0x35, 5, value); }
        public bool RibbonSmile            { get => FlagUtil.GetFlag(Data, 0x35, 6); set => FlagUtil.SetFlag(Data, 0x35, 6, value); }
        public bool RibbonGorgeous         { get => FlagUtil.GetFlag(Data, 0x35, 7); set => FlagUtil.SetFlag(Data, 0x35, 7, value); }

        public bool RibbonRoyal            { get => FlagUtil.GetFlag(Data, 0x36, 0); set => FlagUtil.SetFlag(Data, 0x36, 0, value); }
        public bool RibbonGorgeousRoyal    { get => FlagUtil.GetFlag(Data, 0x36, 1); set => FlagUtil.SetFlag(Data, 0x36, 1, value); }
        public bool RibbonArtist           { get => FlagUtil.GetFlag(Data, 0x36, 2); set => FlagUtil.SetFlag(Data, 0x36, 2, value); }
        public bool RibbonFootprint        { get => FlagUtil.GetFlag(Data, 0x36, 3); set => FlagUtil.SetFlag(Data, 0x36, 3, value); }
        public bool RibbonRecord           { get => FlagUtil.GetFlag(Data, 0x36, 4); set => FlagUtil.SetFlag(Data, 0x36, 4, value); }
        public bool RibbonLegend           { get => FlagUtil.GetFlag(Data, 0x36, 5); set => FlagUtil.SetFlag(Data, 0x36, 5, value); }
        public bool RibbonCountry          { get => FlagUtil.GetFlag(Data, 0x36, 6); set => FlagUtil.SetFlag(Data, 0x36, 6, value); }
        public bool RibbonNational         { get => FlagUtil.GetFlag(Data, 0x36, 7); set => FlagUtil.SetFlag(Data, 0x36, 7, value); }

        public bool RibbonEarth            { get => FlagUtil.GetFlag(Data, 0x37, 0); set => FlagUtil.SetFlag(Data, 0x37, 0, value); }
        public bool RibbonWorld            { get => FlagUtil.GetFlag(Data, 0x37, 1); set => FlagUtil.SetFlag(Data, 0x37, 1, value); }
        public bool RibbonClassic          { get => FlagUtil.GetFlag(Data, 0x37, 2); set => FlagUtil.SetFlag(Data, 0x37, 2, value); }
        public bool RibbonPremier          { get => FlagUtil.GetFlag(Data, 0x37, 3); set => FlagUtil.SetFlag(Data, 0x37, 3, value); }
        public bool RibbonEvent            { get => FlagUtil.GetFlag(Data, 0x37, 4); set => FlagUtil.SetFlag(Data, 0x37, 4, value); }
        public bool RibbonBirthday         { get => FlagUtil.GetFlag(Data, 0x37, 5); set => FlagUtil.SetFlag(Data, 0x37, 5, value); }
        public bool RibbonSpecial          { get => FlagUtil.GetFlag(Data, 0x37, 6); set => FlagUtil.SetFlag(Data, 0x37, 6, value); }
        public bool RibbonSouvenir         { get => FlagUtil.GetFlag(Data, 0x37, 7); set => FlagUtil.SetFlag(Data, 0x37, 7, value); }

        // ribbon u32
        public bool RibbonWishing          { get => FlagUtil.GetFlag(Data, 0x38, 0); set => FlagUtil.SetFlag(Data, 0x38, 0, value); }
        public bool RibbonChampionBattle   { get => FlagUtil.GetFlag(Data, 0x38, 1); set => FlagUtil.SetFlag(Data, 0x38, 1, value); }
        public bool RibbonChampionRegional { get => FlagUtil.GetFlag(Data, 0x38, 2); set => FlagUtil.SetFlag(Data, 0x38, 2, value); }
        public bool RibbonChampionNational { get => FlagUtil.GetFlag(Data, 0x38, 3); set => FlagUtil.SetFlag(Data, 0x38, 3, value); }
        public bool RibbonChampionWorld    { get => FlagUtil.GetFlag(Data, 0x38, 4); set => FlagUtil.SetFlag(Data, 0x38, 4, value); }
        public bool HasContestMemoryRibbon { get => FlagUtil.GetFlag(Data, 0x38, 5); set => FlagUtil.SetFlag(Data, 0x38, 5, value); }
        public bool HasBattleMemoryRibbon  { get => FlagUtil.GetFlag(Data, 0x38, 6); set => FlagUtil.SetFlag(Data, 0x38, 6, value); }
        public bool RibbonChampionG6Hoenn  { get => FlagUtil.GetFlag(Data, 0x38, 7); set => FlagUtil.SetFlag(Data, 0x38, 7, value); }

        public bool RibbonContestStar      { get => FlagUtil.GetFlag(Data, 0x39, 0); set => FlagUtil.SetFlag(Data, 0x39, 0, value); }
        public bool RibbonMasterCoolness   { get => FlagUtil.GetFlag(Data, 0x39, 1); set => FlagUtil.SetFlag(Data, 0x39, 1, value); }
        public bool RibbonMasterBeauty     { get => FlagUtil.GetFlag(Data, 0x39, 2); set => FlagUtil.SetFlag(Data, 0x39, 2, value); }
        public bool RibbonMasterCuteness   { get => FlagUtil.GetFlag(Data, 0x39, 3); set => FlagUtil.SetFlag(Data, 0x39, 3, value); }
        public bool RibbonMasterCleverness { get => FlagUtil.GetFlag(Data, 0x39, 4); set => FlagUtil.SetFlag(Data, 0x39, 4, value); }
        public bool RibbonMasterToughness  { get => FlagUtil.GetFlag(Data, 0x39, 5); set => FlagUtil.SetFlag(Data, 0x39, 5, value); }
        public bool RibbonChampionAlola    { get => FlagUtil.GetFlag(Data, 0x39, 6); set => FlagUtil.SetFlag(Data, 0x39, 6, value); }
        public bool RibbonBattleRoyale     { get => FlagUtil.GetFlag(Data, 0x39, 7); set => FlagUtil.SetFlag(Data, 0x39, 7, value); }

        public bool RibbonBattleTreeGreat  { get => FlagUtil.GetFlag(Data, 0x3A, 0); set => FlagUtil.SetFlag(Data, 0x3A, 0, value); }
        public bool RibbonBattleTreeMaster { get => FlagUtil.GetFlag(Data, 0x3A, 1); set => FlagUtil.SetFlag(Data, 0x3A, 1, value); }
        public bool RibbonChampionGalar    { get => FlagUtil.GetFlag(Data, 0x3A, 2); set => FlagUtil.SetFlag(Data, 0x3A, 2, value); }
        public bool RibbonTowerMaster      { get => FlagUtil.GetFlag(Data, 0x3A, 3); set => FlagUtil.SetFlag(Data, 0x3A, 3, value); }
        public bool RibbonMasterRank       { get => FlagUtil.GetFlag(Data, 0x3A, 4); set => FlagUtil.SetFlag(Data, 0x3A, 4, value); }
        public bool RibbonMarkLunchtime    { get => FlagUtil.GetFlag(Data, 0x3A, 5); set => FlagUtil.SetFlag(Data, 0x3A, 5, value); }
        public bool RibbonMarkSleepyTime   { get => FlagUtil.GetFlag(Data, 0x3A, 6); set => FlagUtil.SetFlag(Data, 0x3A, 6, value); }
        public bool RibbonMarkDusk         { get => FlagUtil.GetFlag(Data, 0x3A, 7); set => FlagUtil.SetFlag(Data, 0x3A, 7, value); }

        public bool RibbonMarkDawn         { get => FlagUtil.GetFlag(Data, 0x3B, 0); set => FlagUtil.SetFlag(Data, 0x3B, 0, value); }
        public bool RibbonMarkCloudy       { get => FlagUtil.GetFlag(Data, 0x3B, 1); set => FlagUtil.SetFlag(Data, 0x3B, 1, value); }
        public bool RibbonMarkRainy        { get => FlagUtil.GetFlag(Data, 0x3B, 2); set => FlagUtil.SetFlag(Data, 0x3B, 2, value); }
        public bool RibbonMarkStormy       { get => FlagUtil.GetFlag(Data, 0x3B, 3); set => FlagUtil.SetFlag(Data, 0x3B, 3, value); }
        public bool RibbonMarkSnowy        { get => FlagUtil.GetFlag(Data, 0x3B, 4); set => FlagUtil.SetFlag(Data, 0x3B, 4, value); }
        public bool RibbonMarkBlizzard     { get => FlagUtil.GetFlag(Data, 0x3B, 5); set => FlagUtil.SetFlag(Data, 0x3B, 5, value); }
        public bool RibbonMarkDry          { get => FlagUtil.GetFlag(Data, 0x3B, 6); set => FlagUtil.SetFlag(Data, 0x3B, 6, value); }
        public bool RibbonMarkSandstorm    { get => FlagUtil.GetFlag(Data, 0x3B, 7); set => FlagUtil.SetFlag(Data, 0x3B, 7, value); }
        public int RibbonCountMemoryContest { get => Data[0x3C]; set => HasContestMemoryRibbon = (Data[0x3C] = (byte)value) != 0; }
        public int RibbonCountMemoryBattle { get => Data[0x3D]; set => HasBattleMemoryRibbon = (Data[0x3D] = (byte)value) != 0; }
        // 0x3E padding
        // 0x3F padding

        // 0x40 Ribbon 1
        public bool RibbonMarkMisty        { get => FlagUtil.GetFlag(Data, 0x40, 0); set => FlagUtil.SetFlag(Data, 0x40, 0, value); }
        public bool RibbonMarkDestiny      { get => FlagUtil.GetFlag(Data, 0x40, 1); set => FlagUtil.SetFlag(Data, 0x40, 1, value); }
        public bool RibbonMarkFishing      { get => FlagUtil.GetFlag(Data, 0x40, 2); set => FlagUtil.SetFlag(Data, 0x40, 2, value); }
        public bool RibbonMarkCurry        { get => FlagUtil.GetFlag(Data, 0x40, 3); set => FlagUtil.SetFlag(Data, 0x40, 3, value); }
        public bool RibbonMarkUncommon     { get => FlagUtil.GetFlag(Data, 0x40, 4); set => FlagUtil.SetFlag(Data, 0x40, 4, value); }
        public bool RibbonMarkRare         { get => FlagUtil.GetFlag(Data, 0x40, 5); set => FlagUtil.SetFlag(Data, 0x40, 5, value); }
        public bool RibbonMarkRowdy        { get => FlagUtil.GetFlag(Data, 0x40, 6); set => FlagUtil.SetFlag(Data, 0x40, 6, value); }
        public bool RibbonMarkAbsentMinded { get => FlagUtil.GetFlag(Data, 0x40, 7); set => FlagUtil.SetFlag(Data, 0x40, 7, value); }

        public bool RibbonMarkJittery      { get => FlagUtil.GetFlag(Data, 0x41, 0); set => FlagUtil.SetFlag(Data, 0x41, 0, value); }
        public bool RibbonMarkExcited      { get => FlagUtil.GetFlag(Data, 0x41, 1); set => FlagUtil.SetFlag(Data, 0x41, 1, value); }
        public bool RibbonMarkCharismatic  { get => FlagUtil.GetFlag(Data, 0x41, 2); set => FlagUtil.SetFlag(Data, 0x41, 2, value); }
        public bool RibbonMarkCalmness     { get => FlagUtil.GetFlag(Data, 0x41, 3); set => FlagUtil.SetFlag(Data, 0x41, 3, value); }
        public bool RibbonMarkIntense      { get => FlagUtil.GetFlag(Data, 0x41, 4); set => FlagUtil.SetFlag(Data, 0x41, 4, value); }
        public bool RibbonMarkZonedOut     { get => FlagUtil.GetFlag(Data, 0x41, 5); set => FlagUtil.SetFlag(Data, 0x41, 5, value); }
        public bool RibbonMarkJoyful       { get => FlagUtil.GetFlag(Data, 0x41, 6); set => FlagUtil.SetFlag(Data, 0x41, 6, value); }
        public bool RibbonMarkAngry        { get => FlagUtil.GetFlag(Data, 0x41, 7); set => FlagUtil.SetFlag(Data, 0x41, 7, value); }

        public bool RibbonMarkSmiley       { get => FlagUtil.GetFlag(Data, 0x42, 0); set => FlagUtil.SetFlag(Data, 0x42, 0, value); }
        public bool RibbonMarkTeary        { get => FlagUtil.GetFlag(Data, 0x42, 1); set => FlagUtil.SetFlag(Data, 0x42, 1, value); }
        public bool RibbonMarkUpbeat       { get => FlagUtil.GetFlag(Data, 0x42, 2); set => FlagUtil.SetFlag(Data, 0x42, 2, value); }
        public bool RibbonMarkPeeved       { get => FlagUtil.GetFlag(Data, 0x42, 3); set => FlagUtil.SetFlag(Data, 0x42, 3, value); }
        public bool RibbonMarkIntellectual { get => FlagUtil.GetFlag(Data, 0x42, 4); set => FlagUtil.SetFlag(Data, 0x42, 4, value); }
        public bool RibbonMarkFerocious    { get => FlagUtil.GetFlag(Data, 0x42, 5); set => FlagUtil.SetFlag(Data, 0x42, 5, value); }
        public bool RibbonMarkCrafty       { get => FlagUtil.GetFlag(Data, 0x42, 6); set => FlagUtil.SetFlag(Data, 0x42, 6, value); }
        public bool RibbonMarkScowling     { get => FlagUtil.GetFlag(Data, 0x42, 7); set => FlagUtil.SetFlag(Data, 0x42, 7, value); }

        public bool RibbonMarkKindly       { get => FlagUtil.GetFlag(Data, 0x43, 0); set => FlagUtil.SetFlag(Data, 0x43, 0, value); }
        public bool RibbonMarkFlustered    { get => FlagUtil.GetFlag(Data, 0x43, 1); set => FlagUtil.SetFlag(Data, 0x43, 1, value); }
        public bool RibbonMarkPumpedUp     { get => FlagUtil.GetFlag(Data, 0x43, 2); set => FlagUtil.SetFlag(Data, 0x43, 2, value); }
        public bool RibbonMarkZeroEnergy   { get => FlagUtil.GetFlag(Data, 0x43, 3); set => FlagUtil.SetFlag(Data, 0x43, 3, value); }
        public bool RibbonMarkPrideful     { get => FlagUtil.GetFlag(Data, 0x43, 4); set => FlagUtil.SetFlag(Data, 0x43, 4, value); }
        public bool RibbonMarkUnsure       { get => FlagUtil.GetFlag(Data, 0x43, 5); set => FlagUtil.SetFlag(Data, 0x43, 5, value); }
        public bool RibbonMarkHumble       { get => FlagUtil.GetFlag(Data, 0x43, 6); set => FlagUtil.SetFlag(Data, 0x43, 6, value); }
        public bool RibbonMarkThorny       { get => FlagUtil.GetFlag(Data, 0x43, 7); set => FlagUtil.SetFlag(Data, 0x43, 7, value); }
        // 0x44 Ribbon 2

        public bool RibbonMarkVigor { get => FlagUtil.GetFlag(Data, 0x44, 0); set => FlagUtil.SetFlag(Data, 0x44, 0, value); }
        public bool RibbonMarkSlump { get => FlagUtil.GetFlag(Data, 0x44, 1); set => FlagUtil.SetFlag(Data, 0x44, 1, value); }
        public bool RIB44_2 { get => FlagUtil.GetFlag(Data, 0x44, 2); set => FlagUtil.SetFlag(Data, 0x44, 2, value); }
        public bool RIB44_3 { get => FlagUtil.GetFlag(Data, 0x44, 3); set => FlagUtil.SetFlag(Data, 0x44, 3, value); }
        public bool RIB44_4 { get => FlagUtil.GetFlag(Data, 0x44, 4); set => FlagUtil.SetFlag(Data, 0x44, 4, value); }
        public bool RIB44_5 { get => FlagUtil.GetFlag(Data, 0x44, 5); set => FlagUtil.SetFlag(Data, 0x44, 5, value); }
        public bool RIB44_6 { get => FlagUtil.GetFlag(Data, 0x44, 6); set => FlagUtil.SetFlag(Data, 0x44, 6, value); }
        public bool RIB44_7 { get => FlagUtil.GetFlag(Data, 0x44, 7); set => FlagUtil.SetFlag(Data, 0x44, 7, value); }

        public bool RIB45_0 { get => FlagUtil.GetFlag(Data, 0x45, 0); set => FlagUtil.SetFlag(Data, 0x45, 0, value); }
        public bool RIB45_1 { get => FlagUtil.GetFlag(Data, 0x45, 1); set => FlagUtil.SetFlag(Data, 0x45, 1, value); }
        public bool RIB45_2 { get => FlagUtil.GetFlag(Data, 0x45, 2); set => FlagUtil.SetFlag(Data, 0x45, 2, value); }
        public bool RIB45_3 { get => FlagUtil.GetFlag(Data, 0x45, 3); set => FlagUtil.SetFlag(Data, 0x45, 3, value); }
        public bool RIB45_4 { get => FlagUtil.GetFlag(Data, 0x45, 4); set => FlagUtil.SetFlag(Data, 0x45, 4, value); }
        public bool RIB45_5 { get => FlagUtil.GetFlag(Data, 0x45, 5); set => FlagUtil.SetFlag(Data, 0x45, 5, value); }
        public bool RIB45_6 { get => FlagUtil.GetFlag(Data, 0x45, 6); set => FlagUtil.SetFlag(Data, 0x45, 6, value); }
        public bool RIB45_7 { get => FlagUtil.GetFlag(Data, 0x45, 7); set => FlagUtil.SetFlag(Data, 0x45, 7, value); }

        public bool RIB46_0 { get => FlagUtil.GetFlag(Data, 0x41, 0); set => FlagUtil.SetFlag(Data, 0x41, 0, value); }
        public bool RIB46_1 { get => FlagUtil.GetFlag(Data, 0x46, 1); set => FlagUtil.SetFlag(Data, 0x46, 1, value); }
        public bool RIB46_2 { get => FlagUtil.GetFlag(Data, 0x46, 2); set => FlagUtil.SetFlag(Data, 0x46, 2, value); }
        public bool RIB46_3 { get => FlagUtil.GetFlag(Data, 0x46, 3); set => FlagUtil.SetFlag(Data, 0x46, 3, value); }
        public bool RIB46_4 { get => FlagUtil.GetFlag(Data, 0x46, 4); set => FlagUtil.SetFlag(Data, 0x46, 4, value); }
        public bool RIB46_5 { get => FlagUtil.GetFlag(Data, 0x46, 5); set => FlagUtil.SetFlag(Data, 0x46, 5, value); }
        public bool RIB46_6 { get => FlagUtil.GetFlag(Data, 0x46, 6); set => FlagUtil.SetFlag(Data, 0x46, 6, value); }
        public bool RIB46_7 { get => FlagUtil.GetFlag(Data, 0x46, 7); set => FlagUtil.SetFlag(Data, 0x46, 7, value); }

        public bool RIB47_0 { get => FlagUtil.GetFlag(Data, 0x47, 0); set => FlagUtil.SetFlag(Data, 0x47, 0, value); }
        public bool RIB47_1 { get => FlagUtil.GetFlag(Data, 0x47, 1); set => FlagUtil.SetFlag(Data, 0x47, 1, value); }
        public bool RIB47_2 { get => FlagUtil.GetFlag(Data, 0x47, 2); set => FlagUtil.SetFlag(Data, 0x47, 2, value); }
        public bool RIB47_3 { get => FlagUtil.GetFlag(Data, 0x47, 3); set => FlagUtil.SetFlag(Data, 0x47, 3, value); }
        public bool RIB47_4 { get => FlagUtil.GetFlag(Data, 0x47, 4); set => FlagUtil.SetFlag(Data, 0x47, 4, value); }
        public bool RIB47_5 { get => FlagUtil.GetFlag(Data, 0x47, 5); set => FlagUtil.SetFlag(Data, 0x47, 5, value); }
        public bool RIB47_6 { get => FlagUtil.GetFlag(Data, 0x47, 6); set => FlagUtil.SetFlag(Data, 0x47, 6, value); }
        public bool RIB47_7 { get => FlagUtil.GetFlag(Data, 0x47, 7); set => FlagUtil.SetFlag(Data, 0x47, 7, value); }

        public uint U48 { get => BitConverter.ToUInt32(Data, 0x48); set => BitConverter.GetBytes(value).CopyTo(Data, 0x48); }

        public byte GetFromArrayA1(int index)
        {
            if ((uint)index >= 4)
                throw new ArgumentException(nameof(index));
            return Data[0x4C + index];
        }

        public void SetFromArrayA1(int index, byte value)
        {
            if ((uint)index >= 4)
                throw new ArgumentException(nameof(index));
            Data[0x4C + index] = value;
        }

        public int HeightScalar { get => Data[0x50]; set => Data[0x50] = (byte)value; }
        public int WeightScalar { get => Data[0x51]; set => Data[0x51] = (byte)value; }

        public byte GetFromArrayA2(int index)
        {
            if ((uint)index >= 6)
                throw new ArgumentException(nameof(index));
            return Data[0x52 + index];
        }

        public void SetFromArrayA2(int index, byte value)
        {
            if ((uint)index >= 6)
                throw new ArgumentException(nameof(index));
            Data[0x52 + index] = value;
        }
        #endregion
        #region Block B
        public override string Nickname
        {
            get => GetString(0x58, 24);
            set => SetString(value, 12).CopyTo(Data, 0x58);
        }

        // 2 bytes for \0, automatically handled above

        public override int Move1 { get => BitConverter.ToUInt16(Data, 0x72); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x72); }
        public override int Move2 { get => BitConverter.ToUInt16(Data, 0x74); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x74); }
        public override int Move3 { get => BitConverter.ToUInt16(Data, 0x76); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x76); }
        public override int Move4 { get => BitConverter.ToUInt16(Data, 0x78); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x78); }

        public override int Move1_PP    { get => Data[0x7A]; set => Data[0x7A] = (byte)value; }
        public override int Move2_PP    { get => Data[0x7B]; set => Data[0x7B] = (byte)value; }
        public override int Move3_PP    { get => Data[0x7C]; set => Data[0x7C] = (byte)value; }
        public override int Move4_PP    { get => Data[0x7D]; set => Data[0x7D] = (byte)value; }
        public override int Move1_PPUps { get => Data[0x7E]; set => Data[0x7E] = (byte)value; }
        public override int Move2_PPUps { get => Data[0x7F]; set => Data[0x7F] = (byte)value; }
        public override int Move3_PPUps { get => Data[0x80]; set => Data[0x80] = (byte)value; }
        public override int Move4_PPUps { get => Data[0x81]; set => Data[0x81] = (byte)value; }

        public override int RelearnMove1 { get => BitConverter.ToUInt16(Data, 0x82); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x82); }
        public override int RelearnMove2 { get => BitConverter.ToUInt16(Data, 0x84); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x84); }
        public override int RelearnMove3 { get => BitConverter.ToUInt16(Data, 0x86); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x86); }
        public override int RelearnMove4 { get => BitConverter.ToUInt16(Data, 0x88); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x88); }

        public override int Stat_HPCurrent { get => BitConverter.ToUInt16(Data, 0x8A); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x8A); }

        private uint IV32 { get => BitConverter.ToUInt32(Data, 0x8C); set => BitConverter.GetBytes(value).CopyTo(Data, 0x8C); }
        public override int IV_HP { get => (int)(IV32 >> 00) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 00)) | ((value > 31 ? 31u : (uint)value) << 00); }
        public override int IV_ATK { get => (int)(IV32 >> 05) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 05)) | ((value > 31 ? 31u : (uint)value) << 05); }
        public override int IV_DEF { get => (int)(IV32 >> 10) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 10)) | ((value > 31 ? 31u : (uint)value) << 10); }
        public override int IV_SPE { get => (int)(IV32 >> 15) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 15)) | ((value > 31 ? 31u : (uint)value) << 15); }
        public override int IV_SPA { get => (int)(IV32 >> 20) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 20)) | ((value > 31 ? 31u : (uint)value) << 20); }
        public override int IV_SPD { get => (int)(IV32 >> 25) & 0x1F; set => IV32 = (IV32 & ~(0x1Fu << 25)) | ((value > 31 ? 31u : (uint)value) << 25); }
        public override bool IsEgg { get => ((IV32 >> 30) & 1) == 1; set => IV32 = (IV32 & ~0x40000000u) | (value ? 0x40000000u : 0u); }
        public override bool IsNicknamed { get => ((IV32 >> 31) & 1) == 1; set => IV32 = (IV32 & 0x7FFFFFFFu) | (value ? 0x80000000u : 0u); }

        public byte DynamaxLevel { get => Data[0x90]; set => Data[0x90] = value; }

        public byte GetFromArrayB1(int index)
        {
            if ((uint)index >= 3)
                throw new ArgumentException(nameof(index));
            return Data[0x90 + index];
        }

        public void SetFromArrayB1(int index, byte value)
        {
            if ((uint)index >= 3)
                throw new ArgumentException(nameof(index));
            Data[0x90 + index] = value;
        }

        public override int Status_Condition { get => BitConverter.ToInt32(Data, 0x94); set => BitConverter.GetBytes(value).CopyTo(Data, 0x94); }
        public int Unk98 { get => BitConverter.ToInt32(Data, 0x98); set => BitConverter.GetBytes(value).CopyTo(Data, 0x98); }

        public byte GetFromArrayB2(int index)
        {
            if ((uint)index >= 14)
                throw new ArgumentException(nameof(index));
            return Data[0x9C + index];
        }

        public void SetFromArrayB2(int index, byte value)
        {
            if ((uint)index >= 14)
                throw new ArgumentException(nameof(index));
            Data[0x9C + index] = value;
        }
        #endregion
        #region Block C
        public override string HT_Name { get => GetString(0xA8, 24); set => SetString(value, 12).CopyTo(Data, 0xA8); }
        public override int HT_Gender { get => Data[0xC2]; set => Data[0xC2] = (byte)value; }
        public int HT_Language { get => Data[0xC3]; set => Data[0xC3] = (byte)value; }
        public override int CurrentHandler { get => Data[0xC4]; set => Data[0xC4] = (byte)value; }
        // 0xC5 unused (alignment)
        public int HT_TrainerID { get => BitConverter.ToUInt16(Data, 0xC6); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xC6); } // unused?
        public override int HT_Friendship { get => Data[0xC8]; set => Data[0xC8] = (byte)value; }
        public override int HT_Intensity { get => Data[0xC9]; set => Data[0xC9] = (byte)value; }
        public override int HT_Memory { get => Data[0xCA]; set => Data[0xCA] = (byte)value; }
        public override int HT_Feeling { get => Data[0xCB]; set => Data[0xCB] = (byte)value; }
        public override int HT_TextVar { get => BitConverter.ToUInt16(Data, 0xCC); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0xCC); }

        public byte GetFromArrayC1(int index)
        {
            if ((uint)index >= 14)
                throw new ArgumentException(nameof(index));
            return Data[0xCE + index];
        }

        public void SetFromArrayC1(int index, byte value)
        {
            if ((uint)index >= 14)
                throw new ArgumentException(nameof(index));
            Data[0xCE + index] = value;
        }

        public override byte Fullness { get => Data[0xDC]; set => Data[0xDC] = value; }
        public override byte Enjoyment { get => Data[0xDD]; set => Data[0xDD] = value; }
        public override int Version { get => Data[0xDE]; set => Data[0xDE] = (byte)value; }
        public override int Country { get => Data[0xDF]; set => Data[0xDF] = (byte)value; }
        public override int Region { get => Data[0xE0]; set => Data[0xE0] = (byte)value; }
        public override int ConsoleRegion { get => Data[0xE1]; set => Data[0xE1] = (byte)value; }
        public override int Language { get => Data[0xE2]; set => Data[0xE2] = (byte)value; }
        public int UnkE3 { get => Data[0xE3]; set => Data[0xE3] = (byte)value; }
        public uint FormArgument { get => BitConverter.ToUInt32(Data, 0xE4); set => BitConverter.GetBytes(value).CopyTo(Data, 0xE4); }
        public sbyte AffixedRibbon { get => (sbyte)Data[0xE8]; set => Data[0xE8] = (byte)value; } // selected ribbon

        public byte GetFromArrayC2(int index)
        {
            if ((uint)index >= 15)
                throw new ArgumentException(nameof(index));
            return Data[0xE9 + index];
        }

        public void SetFromArrayC2(int index, byte value)
        {
            if ((uint)index >= 15)
                throw new ArgumentException(nameof(index));
            Data[0xE9 + index] = value;
        }

        #endregion
        #region Block D
        public override string OT_Name { get => GetString(0xF8, 24); set => SetString(value, 12).CopyTo(Data, 0xF8); }
        public override int OT_Friendship { get => Data[0x112]; set => Data[0x112] = (byte)value; }
        public override int OT_Intensity { get => Data[0x113]; set => Data[0x113] = (byte)value; }
        public override int OT_Memory { get => Data[0x114]; set => Data[0x114] = (byte)value; }
        // 0x115 unused align
        public override int OT_TextVar { get => BitConverter.ToUInt16(Data, 0x116); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x116); }
        public override int OT_Feeling { get => Data[0x118]; set => Data[0x118] = (byte)value; }
        public override int Egg_Year { get => Data[0x119]; set => Data[0x119] = (byte)value; }
        public override int Egg_Month { get => Data[0x11A]; set => Data[0x11A] = (byte)value; }
        public override int Egg_Day { get => Data[0x11B]; set => Data[0x11B] = (byte)value; }
        public override int Met_Year { get => Data[0x11C]; set => Data[0x11C] = (byte)value; }
        public override int Met_Month { get => Data[0x11D]; set => Data[0x11D] = (byte)value; }
        public override int Met_Day { get => Data[0x11E]; set => Data[0x11E] = (byte)value; }
        // 0x11F unused align
        public override int Egg_Location { get => BitConverter.ToUInt16(Data, 0x120); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x120); }
        public override int Met_Location { get => BitConverter.ToUInt16(Data, 0x122); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x122); }
        public override int Ball { get => Data[0x124]; set => Data[0x124] = (byte)value; }
        public override int Met_Level { get => Data[0x125] & ~0x80; set => Data[0x125] = (byte)((Data[0x125] & 0x80) | value); }
        public override int OT_Gender { get => Data[0x125] >> 7; set => Data[0x125] = (byte)((Data[0x125] & ~0x80) | (value << 7)); }
        public int HyperTrainFlags { get => Data[0x126]; set => Data[0x126] = (byte)value; }
        public bool HT_HP { get => ((HyperTrainFlags >> 0) & 1) == 1; set => HyperTrainFlags = (HyperTrainFlags & ~(1 << 0)) | ((value ? 1 : 0) << 0); }
        public bool HT_ATK { get => ((HyperTrainFlags >> 1) & 1) == 1; set => HyperTrainFlags = (HyperTrainFlags & ~(1 << 1)) | ((value ? 1 : 0) << 1); }
        public bool HT_DEF { get => ((HyperTrainFlags >> 2) & 1) == 1; set => HyperTrainFlags = (HyperTrainFlags & ~(1 << 2)) | ((value ? 1 : 0) << 2); }
        public bool HT_SPA { get => ((HyperTrainFlags >> 3) & 1) == 1; set => HyperTrainFlags = (HyperTrainFlags & ~(1 << 3)) | ((value ? 1 : 0) << 3); }
        public bool HT_SPD { get => ((HyperTrainFlags >> 4) & 1) == 1; set => HyperTrainFlags = (HyperTrainFlags & ~(1 << 4)) | ((value ? 1 : 0) << 4); }
        public bool HT_SPE { get => ((HyperTrainFlags >> 5) & 1) == 1; set => HyperTrainFlags = (HyperTrainFlags & ~(1 << 5)) | ((value ? 1 : 0) << 5); }

        public bool GetMoveRecordFlag(int index)
        {
            if ((uint) index > 112) // 14 bytes, 8 bits
                throw new ArgumentException(nameof(index));
            int ofs = index >> 3;
            return FlagUtil.GetFlag(Data, 0x127 + ofs, index & 7);
        }

        public void SetMoveRecordFlag(int index, bool value)
        {
            if ((uint)index > 112) // 14 bytes, 8 bits
                throw new ArgumentException(nameof(index));
            int ofs = index >> 3;
            FlagUtil.SetFlag(Data, 0x127 + ofs, index & 7, value);
        }

        public bool HasAnyMoveRecordFlag()
        {
            for (int i = 0x127; i < 0x127 + 14; i++)
            {
                if (Data[i] != 0)
                    return true;
            }
            return false;
        }

        public ulong Tracker
        {
            get => BitConverter.ToUInt64(Data, 0x135);
            set => BitConverter.GetBytes(value).CopyTo(Data, 0x135);
        }

        public byte GetFromArrayD1(int index)
        {
            if ((uint)index >= 19)
                throw new ArgumentException(nameof(index));
            return Data[0x135 + index];
        }

        public void SetFromArrayD1(int index, byte value)
        {
            if ((uint)index >= 19)
                throw new ArgumentException(nameof(index));
            Data[0x135 + index] = value;
        }

        #endregion
        #region Battle Stats
        public override int Stat_Level { get => Data[0x148]; set => Data[0x148] = (byte)value; }
        // 0x149 unused alignment
        public override int Stat_HPMax { get => BitConverter.ToUInt16(Data, 0x14A); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x14A); }
        public override int Stat_ATK { get => BitConverter.ToUInt16(Data, 0x14C); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x14C); }
        public override int Stat_DEF { get => BitConverter.ToUInt16(Data, 0x14E); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x14E); }
        public override int Stat_SPE { get => BitConverter.ToUInt16(Data, 0x150); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x150); }
        public override int Stat_SPA { get => BitConverter.ToUInt16(Data, 0x152); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x152); }
        public override int Stat_SPD { get => BitConverter.ToUInt16(Data, 0x154); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x154); }
        public int DynamaxType { get => BitConverter.ToUInt16(Data, 0x156); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x156); }
        #endregion

        public override int[] Markings
        {
            get
            {
                int[] marks = new int[8];
                int val = MarkValue;
                for (int i = 0; i < marks.Length; i++)
                    marks[i] = ((val >> (i * 2)) & 3) % 3;
                return marks;
            }
            set
            {
                if (value.Length > 8)
                    return;
                int v = 0;
                for (int i = 0; i < value.Length; i++)
                    v |= (value[i] % 3) << (i * 2);
                MarkValue = v;
            }
        }

        public void FixMemories()
        {
            if (IsEgg) // No memories if is egg.
            {
                HT_Language = HT_Friendship = HT_TextVar = HT_Memory = HT_Intensity = HT_Feeling =
                /* OT_Friendship */ OT_TextVar = OT_Memory = OT_Intensity = OT_Feeling = 0;

                // Clear Handler
                HT_Name = string.Empty.PadRight(11, '\0');
                return;
            }

            if (IsUntraded)
                HT_Language = HT_Friendship = HT_TextVar = HT_Memory = HT_Intensity = HT_Feeling = 0;
            if (GenNumber < 6)
            {
                OT_TextVar = OT_Memory = OT_Intensity = OT_Feeling = 0;
            }

            if (GenNumber < 8) // must be transferred via HOME, and must have memories
            {
                TradeMemory();
            }
        }

        private bool TradeOT(ITrainerInfo tr)
        {
            // Check to see if the OT matches the SAV's OT info.
            if (!(tr.OT == OT_Name && tr.TID == TID && tr.SID == SID && tr.Gender == OT_Gender))
                return false;

            CurrentHandler = 0;
            return true;
        }

        private void TradeHT(ITrainerInfo tr)
        {
            if (tr.OT != HT_Name || tr.Gender != HT_Gender)
            {
                // No geolocations are set ingame -- except for bank transfers. Don't emulate bank transfers
                // this.TradeGeoLocation(tr.Country, tr.SubRegion);
            }

            if (HT_Name != tr.OT)
            {
                HT_Friendship = 50;
                HT_Name = tr.OT;
            }
            CurrentHandler = 1;
            HT_Gender = tr.Gender;
            HT_Language = tr.Language;
        }

        // Misc Updates
        public static void TradeMemory()
        {
        }

        // Maximums
        public override int MaxMoveID => Legal.MaxMoveID_8;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_8;
        public override int MaxAbilityID => Legal.MaxAbilityID_8;
        public override int MaxItemID => Legal.MaxItemID_8;
        public override int MaxBallID => Legal.MaxBallID_8;
        public override int MaxGameID => Legal.MaxGameID_8;

        public bool GetRibbon(int index) => FlagUtil.GetFlag(Data, GetRibbonByte(index), index & 7);
        public void SetRibbon(int index, bool value = true) => FlagUtil.SetFlag(Data, GetRibbonByte(index), index & 7, value);

        public int GetRibbonByte(int index)
        {
            if ((uint)index >= 128)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (index < 64)
                return 0x34 + (index >> 3);
            index -= 64;
            return 0x40 + (index >> 3);
        }
    }
}