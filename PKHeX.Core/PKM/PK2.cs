using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary> Generation 2 <see cref="PKM"/> format. </summary>
public sealed class PK2 : GBPKML, ICaughtData2
{
    public override PersonalInfo PersonalInfo => PersonalTable.C[Species];

    internal const byte EggSpeciesValue = 0xFD;
    public override bool Valid => Species is <= Legal.MaxSpeciesID_2 or EggSpeciesValue; // egg

    public override int SIZE_PARTY => PokeCrypto.SIZE_2PARTY;
    public override int SIZE_STORED => PokeCrypto.SIZE_2STORED;
    public override bool Korean => !Japanese && RawOT[0] <= 0xB;

    public override EntityContext Context => EntityContext.Gen2;

    public PK2(bool jp = false) : base(PokeCrypto.SIZE_2PARTY, jp) { }
    public PK2(byte[] decryptedData, bool jp = false) : base(EnsurePartySize(decryptedData), jp) { }

    private static byte[] EnsurePartySize(byte[] data)
    {
        if (data.Length != PokeCrypto.SIZE_2PARTY)
            Array.Resize(ref data, PokeCrypto.SIZE_2PARTY);
        return data;
    }

    public override PKM Clone()
    {
        var clone = new PK2((byte[])Data.Clone(), Japanese) { IsEgg = IsEgg };
        OT_Trash.CopyTo(clone.OT_Trash);
        Nickname_Trash.CopyTo(clone.Nickname_Trash);
        return clone;
    }

    protected override byte[] Encrypt() => new PokeList2(this).Write();

    #region Stored Attributes
    public override ushort Species { get => Data[0]; set => Data[0] = (byte)value; }
    public override int SpriteItem => ItemConverter.GetItemFuture2((byte)HeldItem);
    public override int HeldItem { get => Data[0x1]; set => Data[0x1] = (byte)value; }
    public override ushort Move1 { get => Data[2]; set => Data[2] = (byte)value; }
    public override ushort Move2 { get => Data[3]; set => Data[3] = (byte)value; }
    public override ushort Move3 { get => Data[4]; set => Data[4] = (byte)value; }
    public override ushort Move4 { get => Data[5]; set => Data[5] = (byte)value; }
    public override int TID { get => ReadUInt16BigEndian(Data.AsSpan(6)); set => WriteUInt16BigEndian(Data.AsSpan(6), (ushort)value); }
    public override uint EXP { get => ReadUInt32BigEndian(Data.AsSpan(0x08)) >> 8; set => WriteUInt32BigEndian(Data.AsSpan(8), (value << 8) | Data[0xB]); }
    public override int EV_HP  { get => ReadUInt16BigEndian(Data.AsSpan(0x0B)); set => WriteUInt16BigEndian(Data.AsSpan(0xB), (ushort)value); }
    public override int EV_ATK { get => ReadUInt16BigEndian(Data.AsSpan(0x0D)); set => WriteUInt16BigEndian(Data.AsSpan(0xD), (ushort)value); }
    public override int EV_DEF { get => ReadUInt16BigEndian(Data.AsSpan(0x0F)); set => WriteUInt16BigEndian(Data.AsSpan(0xF), (ushort)value); }
    public override int EV_SPE { get => ReadUInt16BigEndian(Data.AsSpan(0x11)); set => WriteUInt16BigEndian(Data.AsSpan(0x11), (ushort)value); }
    public override int EV_SPC { get => ReadUInt16BigEndian(Data.AsSpan(0x13)); set => WriteUInt16BigEndian(Data.AsSpan(0x13), (ushort)value); }
    public override ushort DV16 { get => ReadUInt16BigEndian(Data.AsSpan(0x15)); set => WriteUInt16BigEndian(Data.AsSpan(0x15), value); }
    public override int Move1_PP { get => Data[0x17] & 0x3F; set => Data[0x17] = (byte)((Data[0x17] & 0xC0) | Math.Min(63, value)); }
    public override int Move2_PP { get => Data[0x18] & 0x3F; set => Data[0x18] = (byte)((Data[0x18] & 0xC0) | Math.Min(63, value)); }
    public override int Move3_PP { get => Data[0x19] & 0x3F; set => Data[0x19] = (byte)((Data[0x19] & 0xC0) | Math.Min(63, value)); }
    public override int Move4_PP { get => Data[0x1A] & 0x3F; set => Data[0x1A] = (byte)((Data[0x1A] & 0xC0) | Math.Min(63, value)); }
    public override int Move1_PPUps { get => (Data[0x17] & 0xC0) >> 6; set => Data[0x17] = (byte)((Data[0x17] & 0x3F) | ((value & 0x3) << 6)); }
    public override int Move2_PPUps { get => (Data[0x18] & 0xC0) >> 6; set => Data[0x18] = (byte)((Data[0x18] & 0x3F) | ((value & 0x3) << 6)); }
    public override int Move3_PPUps { get => (Data[0x19] & 0xC0) >> 6; set => Data[0x19] = (byte)((Data[0x19] & 0x3F) | ((value & 0x3) << 6)); }
    public override int Move4_PPUps { get => (Data[0x1A] & 0xC0) >> 6; set => Data[0x1A] = (byte)((Data[0x1A] & 0x3F) | ((value & 0x3) << 6)); }
    public override int CurrentFriendship { get => Data[0x1B]; set => Data[0x1B] = (byte)value; }
    private byte PKRS { get => Data[0x1C]; set => Data[0x1C] = value; }
    public override int PKRS_Days { get => PKRS & 0xF; set => PKRS = (byte)((PKRS & ~0xF) | value); }
    public override int PKRS_Strain { get => PKRS >> 4; set => PKRS = (byte)((PKRS & 0xF) | (value << 4)); }
    // Crystal only Caught Data
    public ushort CaughtData { get => ReadUInt16BigEndian(Data.AsSpan(0x1D)); set => WriteUInt16BigEndian(Data.AsSpan(0x1D), value); }
    public int Met_TimeOfDay         { get => (CaughtData >> 14) & 0x3; set => CaughtData = (ushort)((CaughtData & 0x3FFF) | ((value & 0x3) << 14)); }
    public override int Met_Level    { get => (CaughtData >> 8) & 0x3F; set => CaughtData = (ushort)((CaughtData & 0xC0FF) | ((value & 0x3F) << 8)); }
    public override int OT_Gender    { get => (CaughtData >> 7) & 1;    set => CaughtData = (ushort)((CaughtData & 0xFF7F) | ((value & 1) << 7)); }
    public override int Met_Location { get => CaughtData & 0x7F;        set => CaughtData = (ushort)((CaughtData & 0xFF80) | (value & 0x7F)); }

    public override int Stat_Level
    {
        get => Data[0x1F];
        set => Data[0x1F] = (byte)value;
    }

    #endregion

    #region Party Attributes
    public override int Status_Condition { get => Data[0x20]; set => Data[0x20] = (byte)value; }

    public override int Stat_HPCurrent { get => ReadUInt16BigEndian(Data.AsSpan(0x22)); set => WriteUInt16BigEndian(Data.AsSpan(0x22), (ushort)value); }
    public override int Stat_HPMax     { get => ReadUInt16BigEndian(Data.AsSpan(0x24)); set => WriteUInt16BigEndian(Data.AsSpan(0x24), (ushort)value); }
    public override int Stat_ATK       { get => ReadUInt16BigEndian(Data.AsSpan(0x26)); set => WriteUInt16BigEndian(Data.AsSpan(0x26), (ushort)value); }
    public override int Stat_DEF       { get => ReadUInt16BigEndian(Data.AsSpan(0x28)); set => WriteUInt16BigEndian(Data.AsSpan(0x28), (ushort)value); }
    public override int Stat_SPE       { get => ReadUInt16BigEndian(Data.AsSpan(0x2A)); set => WriteUInt16BigEndian(Data.AsSpan(0x2A), (ushort)value); }
    public override int Stat_SPA       { get => ReadUInt16BigEndian(Data.AsSpan(0x2C)); set => WriteUInt16BigEndian(Data.AsSpan(0x2C), (ushort)value); }
    public override int Stat_SPD       { get => ReadUInt16BigEndian(Data.AsSpan(0x2E)); set => WriteUInt16BigEndian(Data.AsSpan(0x2E), (ushort)value); }
    #endregion

    public override bool IsEgg { get; set; }
    public override int OT_Friendship { get => CurrentFriendship; set => CurrentFriendship = value; }
    public override bool HasOriginalMetLocation => CaughtData != 0;
    public override int Version { get => (int)GameVersion.GSC; set { } }

    // Maximums
    public override ushort MaxMoveID => Legal.MaxMoveID_2;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_2;
    public override int MaxAbilityID => Legal.MaxAbilityID_2;
    public override int MaxItemID => Legal.MaxItemID_2;

    public PK1 ConvertToPK1()
    {
        PK1 pk1 = new(Japanese);
        Array.Copy(Data, 0x1, pk1.Data, 0x7, 0x1A);
        pk1.Species = Species; // This will take care of Typing :)

        var lvl = Stat_Level;
        if (lvl == 0) // no party stats (originated from box format), need to regenerate
        {
            pk1.Stat_HPCurrent = GetStat(PersonalInfo.HP, IV_ATK, EV_ATK, Stat_Level);
            pk1.Stat_Level = CurrentLevel;
        }
        else
        {
            pk1.Stat_HPCurrent = Stat_HPCurrent;
            pk1.Stat_Level = Stat_Level;
        }
        // Status = 0
        OT_Trash.CopyTo(pk1.OT_Trash);
        Nickname_Trash.CopyTo(pk1.Nickname_Trash);

        pk1.ClearInvalidMoves();

        return pk1;
    }

    public PK7 ConvertToPK7()
    {
        var rnd = Util.Rand;
        var pk7 = new PK7
        {
            EncryptionConstant = rnd.Rand32(),
            Species = Species,
            TID = TID,
            CurrentLevel = CurrentLevel,
            EXP = EXP,
            Met_Level = CurrentLevel,
            Nature = Experience.GetNatureVC(EXP),
            PID = rnd.Rand32(),
            Ball = 4,
            MetDate = DateTime.Now,
            Version = HasOriginalMetLocation ? (int)GameVersion.C : (int)GameVersion.SI,
            Move1 = Move1,
            Move2 = Move2,
            Move3 = Move3,
            Move4 = Move4,
            Move1_PPUps = Move1_PPUps,
            Move2_PPUps = Move2_PPUps,
            Move3_PPUps = Move3_PPUps,
            Move4_PPUps = Move4_PPUps,
            Met_Location = Locations.Transfer2, // "Johto region", hardcoded.
            Gender = Gender,
            IsNicknamed = false,
            Form = Form,

            CurrentHandler = 1,
            HT_Name = RecentTrainerCache.OT_Name,
            HT_Gender = RecentTrainerCache.OT_Gender,
        };
        RecentTrainerCache.SetConsoleRegionData3DS(pk7);
        RecentTrainerCache.SetFirstCountryRegion(pk7);
        pk7.HealPP();
        var lang = TransferLanguage(RecentTrainerCache.Language);
        pk7.Language = lang;
        pk7.Nickname = SpeciesName.GetSpeciesNameGeneration(pk7.Species, lang, pk7.Format);

        // IVs
        var special = Species is 151 or 251;
        Span<int> finalIVs = stackalloc int[6];
        int flawless = special ? 5 : 3;
        for (var i = 0; i < finalIVs.Length; i++)
            finalIVs[i] = rnd.Next(32);
        for (var i = 0; i < flawless; i++)
            finalIVs[i] = 31;
        Util.Shuffle(finalIVs);
        pk7.SetIVs(finalIVs);

        switch (IsShiny ? Shiny.Always : Shiny.Never)
        {
            case Shiny.Always when !pk7.IsShiny: // Force Square
                pk7.PID = (uint)(((pk7.TID ^ 0 ^ (pk7.PID & 0xFFFF) ^ 0) << 16) | (pk7.PID & 0xFFFF));
                break;
            case Shiny.Never when pk7.IsShiny: // Force Not Shiny
                pk7.PID ^= 0x1000_0000;
                break;
        }

        int abil = Legal.TransferSpeciesDefaultAbilityGen2(Species) ? 0 : 2; // Hidden
        pk7.RefreshAbility(abil); // 0/1/2 (not 1/2/4)

        if (special)
        {
            pk7.FatefulEncounter = true;
        }
        else if (IsNicknamedBank)
        {
            pk7.IsNicknamed = true;
            pk7.Nickname = Korean ? Nickname : StringConverter12Transporter.GetString(RawNickname, Japanese);
        }
        if (RawOT[0] == StringConverter12.G1TradeOTCode) // In-game Trade
            pk7.OT_Name = StringConverter12.G1TradeOTName[lang];
        else
            pk7.OT_Name = Korean ? OT_Name : StringConverter12Transporter.GetString(RawOT, Japanese);
        pk7.OT_Gender = OT_Gender; // Crystal
        pk7.OT_Friendship = pk7.HT_Friendship = PersonalTable.SM[Species].BaseFriendship;

        pk7.SetTradeMemoryHT6(bank: true); // oh no, memories on gen7 pk

        // Dizzy Punch cannot be transferred
        {
            var index = pk7.GetMoveIndex(146); // Dizzy Punch
            if (index != -1)
            {
                pk7.SetMove(index, 0);
                pk7.FixMoves();
            }
        }

        pk7.RefreshChecksum();
        return pk7;
    }

    public SK2 ConvertToSK2() => new(Japanese)
    {
        Species = Species,
        HeldItem = HeldItem,
        Move1 = Move1,
        Move2 = Move2,
        Move3 = Move3,
        Move4 = Move4,
        TID = TID,
        EXP = EXP,
        EV_HP = EV_HP,
        EV_ATK = EV_ATK,
        EV_DEF = EV_DEF,
        EV_SPE = EV_SPE,
        EV_SPC = EV_SPC,
        DV16 = DV16,
        Move1_PP = Move1_PP,
        Move2_PP = Move2_PP,
        Move3_PP = Move3_PP,
        Move4_PP = Move4_PP,
        Move1_PPUps = Move1_PPUps,
        Move2_PPUps = Move2_PPUps,
        Move3_PPUps = Move3_PPUps,
        Move4_PPUps = Move4_PPUps,
        CurrentFriendship = CurrentFriendship,
        IsEgg = IsEgg,
        Stat_Level = Stat_Level,
        PKRS_Days = PKRS_Days,
        PKRS_Strain = PKRS_Strain,
        CaughtData = CaughtData,

        // Only copies until first 0x50 terminator, but just copy everything
        Nickname = Nickname,
        OT_Name = OT_Name,
    };
}
