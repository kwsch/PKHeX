using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary> Generation 1 <see cref="PKM"/> format. </summary>
public sealed class PK1 : GBPKML, IPersonalType
{
    public override PersonalInfo1 PersonalInfo => PersonalTable.Y[Species];

    public override bool Valid => Species <= 151 && (Data[0] == 0 || Species != 0);

    public override int SIZE_PARTY => PokeCrypto.SIZE_1PARTY;
    public override int SIZE_STORED => PokeCrypto.SIZE_1STORED;
    public override bool Korean => false;

    public override EntityContext Context => EntityContext.Gen1;

    public PK1(bool jp = false) : base(PokeCrypto.SIZE_1PARTY, jp) { }
    public PK1(byte[] decryptedData, bool jp = false) : base(EnsurePartySize(decryptedData), jp) { }

    public PK1(ReadOnlySpan<byte> data, ReadOnlySpan<byte> ot, ReadOnlySpan<byte> nick)
        : this(ot.Length == StringLengthJapanese)
    {
        data.CopyTo(Data);
        ot.CopyTo(OriginalTrainerTrash);
        nick.CopyTo(NicknameTrash);
    }

    private static byte[] EnsurePartySize(byte[] data)
    {
        if (data.Length != PokeCrypto.SIZE_1PARTY)
            Array.Resize(ref data, PokeCrypto.SIZE_1PARTY);
        return data;
    }

    public override PK1 Clone()
    {
        PK1 clone = new((byte[])Data.Clone(), Japanese);
        OriginalTrainerTrash.CopyTo(clone.OriginalTrainerTrash);
        NicknameTrash.CopyTo(clone.NicknameTrash);
        return clone;
    }

    protected override byte[] Encrypt() => PokeList1.WrapSingle(this);

    #region Stored Attributes
    public byte SpeciesInternal { get => Data[0]; set => Data[0] = value; } // raw access
    public override ushort Species { get => SpeciesConverter.GetNational1(SpeciesInternal); set => SetSpeciesValues(value); }
    public override int Stat_HPCurrent { get => ReadUInt16BigEndian(Data.AsSpan(0x1)); set => WriteUInt16BigEndian(Data.AsSpan(0x1), (ushort)value); }
    public int Stat_LevelBox { get => Data[3]; set => Data[3] = (byte)value; }
    public override int Status_Condition { get => Data[4]; set => Data[4] = (byte)value; }
    public byte Type1 { get => Data[5]; set => Data[5] = value; }
    public byte Type2 { get => Data[6]; set => Data[6] = value; }
    public byte CatchRate { get => Data[7]; set => Data[7] = value; }
    public override ushort Move1 { get => Data[8]; set => Data[8] = (byte)value; }
    public override ushort Move2 { get => Data[9]; set => Data[9] = (byte)value; }
    public override ushort Move3 { get => Data[10]; set => Data[10] = (byte)value; }
    public override ushort Move4 { get => Data[11]; set => Data[11] = (byte)value; }
    public override ushort TID16 { get => ReadUInt16BigEndian(Data.AsSpan(0xC)); set => WriteUInt16BigEndian(Data.AsSpan(0xC), value); }
    public override uint EXP { get => ReadUInt32BigEndian(Data.AsSpan(0xE)) >> 8; set => WriteUInt32BigEndian(Data.AsSpan(0xE), (value << 8) | Data[0x11]); }
    public override int EV_HP { get => ReadUInt16BigEndian(Data.AsSpan(0x11)); set => WriteUInt16BigEndian(Data.AsSpan(0x11), (ushort)value); }
    public override int EV_ATK { get => ReadUInt16BigEndian(Data.AsSpan(0x13)); set => WriteUInt16BigEndian(Data.AsSpan(0x13), (ushort)value); }
    public override int EV_DEF { get => ReadUInt16BigEndian(Data.AsSpan(0x15)); set => WriteUInt16BigEndian(Data.AsSpan(0x15), (ushort)value); }
    public override int EV_SPE { get => ReadUInt16BigEndian(Data.AsSpan(0x17)); set => WriteUInt16BigEndian(Data.AsSpan(0x17), (ushort)value); }
    public override int EV_SPC { get => ReadUInt16BigEndian(Data.AsSpan(0x19)); set => WriteUInt16BigEndian(Data.AsSpan(0x19), (ushort)value); }
    public override ushort DV16 { get => ReadUInt16BigEndian(Data.AsSpan(0x1B)); set => WriteUInt16BigEndian(Data.AsSpan(0x1B), value); }
    public override int Move1_PP { get => Data[0x1D] & 0x3F; set => Data[0x1D] = (byte)((Data[0x1D] & 0xC0) | Math.Min(63, value)); }
    public override int Move2_PP { get => Data[0x1E] & 0x3F; set => Data[0x1E] = (byte)((Data[0x1E] & 0xC0) | Math.Min(63, value)); }
    public override int Move3_PP { get => Data[0x1F] & 0x3F; set => Data[0x1F] = (byte)((Data[0x1F] & 0xC0) | Math.Min(63, value)); }
    public override int Move4_PP { get => Data[0x20] & 0x3F; set => Data[0x20] = (byte)((Data[0x20] & 0xC0) | Math.Min(63, value)); }
    public override int Move1_PPUps { get => (Data[0x1D] & 0xC0) >> 6; set => Data[0x1D] = (byte)((Data[0x1D] & 0x3F) | ((value & 0x3) << 6)); }
    public override int Move2_PPUps { get => (Data[0x1E] & 0xC0) >> 6; set => Data[0x1E] = (byte)((Data[0x1E] & 0x3F) | ((value & 0x3) << 6)); }
    public override int Move3_PPUps { get => (Data[0x1F] & 0xC0) >> 6; set => Data[0x1F] = (byte)((Data[0x1F] & 0x3F) | ((value & 0x3) << 6)); }
    public override int Move4_PPUps { get => (Data[0x20] & 0xC0) >> 6; set => Data[0x20] = (byte)((Data[0x20] & 0x3F) | ((value & 0x3) << 6)); }
    #endregion

    #region Party Attributes
    public override byte Stat_Level { get => Data[0x21]; set => Stat_LevelBox = Data[0x21] = value; }
    public override int Stat_HPMax { get => ReadUInt16BigEndian(Data.AsSpan(0x22)); set => WriteUInt16BigEndian(Data.AsSpan(0x22), (ushort)value); }
    public override int Stat_ATK { get => ReadUInt16BigEndian(Data.AsSpan(0x24)); set => WriteUInt16BigEndian(Data.AsSpan(0x24), (ushort)value); }
    public override int Stat_DEF { get => ReadUInt16BigEndian(Data.AsSpan(0x26)); set => WriteUInt16BigEndian(Data.AsSpan(0x26), (ushort)value); }
    public override int Stat_SPE { get => ReadUInt16BigEndian(Data.AsSpan(0x28)); set => WriteUInt16BigEndian(Data.AsSpan(0x28), (ushort)value); }
    public int Stat_SPC { get => ReadUInt16BigEndian(Data.AsSpan(0x2A)); set => WriteUInt16BigEndian(Data.AsSpan(0x2A), (ushort)value); }
    // Leave SPA and SPD as alias for SPC
    public override int Stat_SPA { get => Stat_SPC; set => Stat_SPC = value; }
    public override int Stat_SPD { get => Stat_SPC; set { } }
    #endregion

    public static bool IsCatchRateHeldItem(byte rate) => rate == 0 || Array.IndexOf(Legal.HeldItems_GSC, rate) >= 0;

    private static bool IsCatchRatePreEvolutionRate(int baseSpecies, int finalSpecies, byte rate)
    {
        for (int species = baseSpecies; species <= finalSpecies; species++)
        {
            if (rate == PersonalTable.RB[species].CatchRate || rate == PersonalTable.Y[species].CatchRate)
                return true;
        }
        return false;
    }

    private void SetSpeciesValues(ushort species)
    {
        var internalID = SpeciesConverter.GetInternal1(species);
        if (SpeciesInternal == internalID)
            return;

        SpeciesInternal = internalID;

        var pi = PersonalTable.RB[species];
        Type1 = pi.Type1;
        Type2 = pi.Type2;

        // Before updating catch rate, check if non-standard
        if (IsValidCatchRateAnyPreEvo((byte)species, CatchRate))
            return;

        // Matches nothing possible; just reset to current Species' rate.
        CatchRate = pi.CatchRate;
    }

    private static bool IsValidCatchRateAnyPreEvo(byte species, byte rate)
    {
        if (IsCatchRateHeldItem(rate))
            return true;
        if (species == (int)Core.Species.Pikachu && rate == 0xA3) // Light Ball (starter)
            return true;

        // Get de-evolution steps we should check for.
        var stage = PersonalInfo1.GetEvolutionStage(species);
        // For Eevee-lutions, Eevee evolves to (134,135,136), which are (1,1,1). All 3 have the same catch rate as Eevee.
        // In the event the current species is 135 or 136, we'd never check Eevee with this logic, but they're all 45.
        var baby = species - stage;
        return IsCatchRatePreEvolutionRate(baby, species, rate);
    }

    public override GameVersion Version { get => GameVersion.RBY; set { } }
    public override int PokerusStrain { get => 0; set { } }
    public override int PokerusDays { get => 0; set { } }
    public override bool CanHoldItem(ReadOnlySpan<ushort> valid) => false;
    public override ushort MetLocation { get => 0; set { } }
    public override byte OriginalTrainerGender { get => 0; set { } }
    public override byte MetLevel { get => 0; set { } }
    public override byte CurrentFriendship { get => 0; set { } }
    public override bool IsEgg { get => false; set { } }
    public override int HeldItem { get => 0; set { } }
    public override byte OriginalTrainerFriendship { get => 0; set { } }

    // Maximums
    public override ushort MaxMoveID => Legal.MaxMoveID_1;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_1;
    public override int MaxAbilityID => Legal.MaxAbilityID_1;
    public override int MaxItemID => Legal.MaxItemID_1;

    // Extra
    public byte Gen2Item => ItemConverter.GetItemFuture1(CatchRate);

    public PK2 ConvertToPK2()
    {
        PK2 pk2 = new(Japanese) {Species = Species};
        Data.AsSpan(7, 0x1A).CopyTo(pk2.Data.AsSpan(1));
        OriginalTrainerTrash.CopyTo(pk2.OriginalTrainerTrash);
        NicknameTrash.CopyTo(pk2.NicknameTrash);

        pk2.HeldItem = Gen2Item;
        pk2.CurrentFriendship = pk2.PersonalInfo.BaseFriendship;
        pk2.Stat_Level = CurrentLevel;

        return pk2;
    }

    public PK7 ConvertToPK7()
    {
        var rnd = Util.Rand;
        var lang = TransferLanguage(RecentTrainerCache.Language);
        var version = EntityConverter.VirtualConsoleSourceGen1;
        if ((lang == 1) != Japanese)
            lang = Japanese ? 1 : 2;
        if (version == GameVersion.BU && !Japanese)
            version = GameVersion.RD;

        var pi = PersonalTable.SM[Species];
        int ability = TransporterLogic.IsHiddenDisallowedVC1(Species) ? 0 : 2; // Hidden
        var pk7 = new PK7
        {
            EncryptionConstant = rnd.Rand32(),
            Species = Species,
            TID16 = TID16,
            CurrentLevel = CurrentLevel,
            EXP = EXP,
            MetLevel = CurrentLevel,
            Nature = Experience.GetNatureVC(EXP),
            PID = rnd.Rand32(),
            Ball = 4,
            MetDate = EncounterDate.GetDate3DS(),
            Version = version,
            Move1 = Move1,
            Move2 = Move2,
            Move3 = Move3,
            Move4 = Move4,
            Move1_PPUps = Move1_PPUps,
            Move2_PPUps = Move2_PPUps,
            Move3_PPUps = Move3_PPUps,
            Move4_PPUps = Move4_PPUps,
            MetLocation = Locations.Transfer1, // "Kanto region", hardcoded.
            Gender = Gender,
            IsNicknamed = false,

            CurrentHandler = 1,
            HandlingTrainerName = RecentTrainerCache.OriginalTrainerName,
            HandlingTrainerGender = RecentTrainerCache.OriginalTrainerGender,

            Language = lang,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, 7),
            OriginalTrainerName = GetTransferTrainerName(lang),
            OriginalTrainerFriendship = pi.BaseFriendship,
            HandlingTrainerFriendship = pi.BaseFriendship,

            Ability = pi.GetAbilityAtIndex(ability),
            AbilityNumber = 1 << ability,
        };

        bool special = Species == (int)Core.Species.Mew;
        int flawless = special ? 5 : 3;
        pk7.SetTransferIVs(flawless, rnd);
        pk7.SetTransferPID(IsShiny);
        pk7.SetTransferLocale(lang);

        if (special) // Mew gets special treatment.
        {
            pk7.FatefulEncounter = true;
        }
        else if (IsNicknamedBank)
        {
            pk7.IsNicknamed = true;
            pk7.Nickname = StringConverter12Transporter.GetString(NicknameTrash, Japanese);
        }

        pk7.HealPP();
        pk7.RefreshChecksum();
        return pk7;
    }

    private string GetTransferTrainerName(int lang)
    {
        if (OriginalTrainerTrash[0] == StringConverter1.TradeOTCode) // In-game Trade
            return StringConverter12Transporter.GetTradeNameGen1(lang);
        return StringConverter12Transporter.GetString(OriginalTrainerTrash, Japanese);
    }

    public override string GetString(ReadOnlySpan<byte> data)
        => StringConverter1.GetString(data, Japanese);
    public override int LoadString(ReadOnlySpan<byte> data, Span<char> destBuffer)
        => StringConverter1.LoadString(data, destBuffer, Japanese);
    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter1.SetString(destBuffer, value, maxLength, Japanese, option);
    public override int GetStringTerminatorIndex(ReadOnlySpan<byte> data)
        => TrashBytesUTF16.GetTerminatorIndex(data, StringConverter4.Terminator);
    public override int GetStringLength(ReadOnlySpan<byte> data)
        => TrashBytesUTF16.GetStringLength(data, StringConverter4.Terminator);
    public override int GetBytesPerChar() => 2;

    /// <summary>
    /// Gets a checksum over all the entity's data using a single list to wrap all components.
    /// </summary>
    public ushort GetSingleListChecksum()
    {
        Span<byte> tmp = stackalloc byte[PokeList1.GetListLengthSingle(Japanese)];
        PokeList1.WrapSingle(this, tmp);
        return Checksums.CRC16_CCITT(tmp);
    }
}
