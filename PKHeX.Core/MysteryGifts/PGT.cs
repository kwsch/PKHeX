using System;
using static System.Buffers.Binary.BinaryPrimitives;
using static PKHeX.Core.GiftType4;

namespace PKHeX.Core;

/// <summary>
/// Generation 4 Mystery Gift Template File (Inner Gift Data, no card data)
/// </summary>
public sealed class PGT(byte[] Data) : DataMysteryGift(Data), IRibbonSetEvent3, IRibbonSetEvent4, IRandomCorrelation
{
    public PGT() : this(new byte[Size]) { }

    public const int Size = 0x104; // 260
    public override byte Generation => 4;
    public override EntityContext Context => EntityContext.Gen4;
    public override bool FatefulEncounter => IsManaphyEgg || PK.FatefulEncounter;
    public override GameVersion Version => IsManaphyEgg ? GameVersion.Gen4 : PK.Version;

    public override byte Level
    {
        get => IsManaphyEgg ? (byte)1 : IsEntity ? PK.MetLevel : (byte)0;
        set { if (IsEntity) PK.MetLevel = value; }
    }

    public override byte Ball
    {
        get => IsManaphyEgg ? (byte)4 : IsEntity ? PK.Ball : (byte)0;
        set { if (IsEntity) PK.Ball = value; }
    }

    public override AbilityPermission Ability => IsManaphyEgg ? AbilityPermission.Any12 : (int)(PK.PID & 1) == 1 ? AbilityPermission.OnlySecond : AbilityPermission.OnlyFirst;

    public override string CardTitle { get => "Raw Gift (PGT)"; set { } }
    public override int CardID { get => -1; set { } }
    public override bool GiftUsed { get => false; set { } }
    public override Shiny Shiny => IsEgg ? Shiny.Random : PK.PID == 1 ? Shiny.Never : IsShiny ? Shiny.Always : Shiny.Never;

    public ushort CardType { get => ReadUInt16LittleEndian(Data.AsSpan(0x0)); set => WriteUInt16LittleEndian(Data.AsSpan(0x0), value); }
    public byte Slot { get => Data[2]; set => Data[2] = value; }
    public byte Detail { get => Data[3]; set => Data[3] = value; }
    public override int ItemID { get => ReadInt32LittleEndian(Data.AsSpan(0x4)); set => WriteInt32LittleEndian(Data.AsSpan(0x4), value); }
    public int ItemSubID { get => ReadInt32LittleEndian(Data.AsSpan(0x8)); set => WriteInt32LittleEndian(Data.AsSpan(0x8), value); }
    public int PokewalkerCourseID { get => Data[0x4]; set => Data[0x4] = (byte)value; }

    public PK4 PK
    {
        get => _pk ??= new PK4(Data.AsSpan(8, PokeCrypto.SIZE_4PARTY).ToArray());
        set
        {
            _pk = value;
            var data = value.Data;
            bool zero = Array.TrueForAll(data, static z => z == 0); // all zero
            if (!zero)
                data = PokeCrypto.EncryptArray45(data);
            data.CopyTo(Data, 8);
        }
    }

    public override byte[] Write()
    {
        // Ensure PGT content is encrypted
        var clone = new PGT((byte[])Data.Clone());
        clone.VerifyPKEncryption();
        return clone.Data;
    }

    private PK4? _pk;

    /// <summary>
    /// Double-checks the encryption of the gift data for Pokémon data.
    /// </summary>
    /// <returns>True if data was encrypted, false if the data was not modified.</returns>
    public bool VerifyPKEncryption()
    {
        if (GiftType is not (Pokémon or PokémonEgg))
            return false; // not encrypted
        if (ReadUInt32LittleEndian(Data.AsSpan(0x64 + 8)) != 0)
            return false; // already encrypted (unused PK4 field, zero)
        EncryptPK();
        return true;
    }

    private void EncryptPK()
    {
        var span = Data.AsSpan(8, PokeCrypto.SIZE_4PARTY);
        var ekdata = PokeCrypto.EncryptArray45(span);
        ekdata.CopyTo(span);
    }

    public GiftType4 GiftType { get => (GiftType4)CardType; set => CardType = (byte)value; }
    public GiftSubType4 GiftSubType { get => (GiftSubType4)ItemID; set => ItemID = (int)value; }
    public PoketchApp PoketchApp { get => (PoketchApp)ItemID; set => ItemID = (int)value; }
    public Seal4 Seal { get => (Seal4)(ItemSubID - 1); set => ItemSubID = (int)(value + 1); }
    public Accessory4 Accessory { get => (Accessory4)ItemSubID; set => ItemSubID = (int)value; }
    public Backdrop4 Backdrop { get => (Backdrop4)ItemSubID; set => ItemSubID = (int)value; }

    public bool IsHatched => GiftType is Pokémon or PokémonMovie;
    public override bool IsEgg { get => GiftType == PokémonEgg || IsManaphyEgg; set { if (value) { GiftType = PokémonEgg; PK.IsEgg = true; } } }
    public bool IsManaphyEgg { get => GiftType == ManaphyEgg; set { if (value) GiftType = ManaphyEgg; } }
    public override bool IsItem { get => GiftType == Item; set { if (value) GiftType = Item; } }
    public override bool IsEntity { get => GiftType is Pokémon or PokémonEgg or ManaphyEgg or PokémonMovie; set { } }

    public override ushort Species { get => IsManaphyEgg ? (ushort)490 : PK.Species; set => PK.Species = value; }
    public override Moveset Moves { get => new(PK.Move1, PK.Move2, PK.Move3, PK.Move4); set => PK.SetMoves(value); }
    public override int HeldItem { get => PK.HeldItem; set => PK.HeldItem = value; }
    public override bool IsShiny => PK.IsShiny;
    public override byte Gender { get => PK.Gender; set => PK.Gender = value; }
    public override byte Form { get => PK.Form; set => PK.Form = value; }
    public override uint ID32 { get => PK.ID32; set => PK.ID32= value; }
    public override ushort TID16 { get => PK.TID16; set => PK.TID16 = value; }
    public override ushort SID16 { get => PK.SID16; set => PK.SID16 = value; }
    public override string OriginalTrainerName { get => PK.OriginalTrainerName; set => PK.OriginalTrainerName = value; }
    public override ushort Location { get => PK.MetLocation; set => PK.MetLocation = value; }
    public override ushort EggLocation { get => PK.EggLocation; set => PK.EggLocation = value; }
    public override bool HasFixedIVs => (PK.IV32 & 0x3FFF_FFFFu) != 0;
    public override void GetIVs(Span<int> value)
    {
        if (HasFixedIVs)
            PK.GetIVs(value);
    }

    public override PK4 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        if (!IsEntity)
            throw new ArgumentException(nameof(IsEntity));

        // template is already filled out, only minor mutations required
        PK4 pk4 = new((byte[])PK.Data.Clone()) { Sanity = 0 };
        if (!IsHatched && Detail == 0)
        {
            pk4.OriginalTrainerName = tr.OT;
            pk4.TID16 = tr.TID16;
            pk4.SID16 = tr.SID16;
            pk4.OriginalTrainerGender = tr.Gender;
            pk4.Language = tr.Language;
        }

        if (IsManaphyEgg)
            SetDefaultManaphyEggDetails(pk4, tr);

        SetPINGA(pk4, criteria);
        SetMetData(pk4, tr);

        var pi = pk4.PersonalInfo;
        pk4.CurrentFriendship = pk4.IsEgg ? pi.HatchCycles : pi.BaseFriendship;

        pk4.RefreshChecksum();
        return pk4;
    }

    private void SetMetData(PK4 pk4, ITrainerInfo trainer)
    {
        if (!IsEgg)
        {
            pk4.MetLocation = (ushort)(pk4.EggLocation + 3000);
            pk4.EggLocation = 0;
            pk4.MetDate = EncounterDate.GetDateNDS();
            pk4.IsEgg = false;
        }
        else
        {
            pk4.EggLocation += 3000;
            if (trainer.Generation == 4)
                SetUnhatchedEggDetails(pk4);
            else
                SetHatchedEggDetails(pk4);
        }
    }

    private static void SetDefaultManaphyEggDetails(PK4 pk4, ITrainerInfo trainer)
    {
        // Since none of this data is populated, fill in default info.
        pk4.Species = (int)Core.Species.Manaphy;
        pk4.Gender = 2;
        // Level 1 Moves
        pk4.Move1 = 294; pk4.Move1_PP = 20;
        pk4.Move2 = 145; pk4.Move2_PP = 30;
        pk4.Move3 = 346; pk4.Move3_PP = 15;
        pk4.Ability = (int)Core.Ability.Hydration;
        pk4.FatefulEncounter = true;
        pk4.Ball = (int)Core.Ball.Poke;
        pk4.Version = GameVersion.Gen4.Contains(trainer.Version) ? trainer.Version : GameVersion.D;
        var lang = trainer.Language < (int)LanguageID.Korean ? trainer.Language : (int)LanguageID.English;
        pk4.Language = lang;
        pk4.EggLocation = 1; // Ranger (will be +3000 later)
        pk4.Nickname = SpeciesName.GetSpeciesNameGeneration((int)Core.Species.Manaphy, lang, 4);
        pk4.MetLocation = pk4.Version is GameVersion.HG or GameVersion.SS ? Locations.HatchLocationHGSS : Locations.HatchLocationDPPt;
        pk4.MetDate = EncounterDate.GetDateNDS();
    }

    private void SetPINGA(PK4 pk4, EncounterCriteria criteria)
    {
        // Ability is forced already, can't force anything

        // Generate PID
        var seed = SetPID(pk4, criteria);

        if (!IsManaphyEgg)
            seed = Util.Rand32(); // reseed, do not have method 1 correlation

        // Generate IVs
        if ((pk4.IV32 & 0x3FFF_FFFFu) == 0) // Ignore Nickname/Egg flag bits
        {
            uint iv1 = ((seed = LCRNG.Next(seed)) >> 16) & 0x7FFF;
            uint iv2 = (LCRNG.Next(seed) >> 16) & 0x7FFF;
            pk4.IV32 |= iv1 | (iv2 << 15);
        }
    }

    private uint SetPID(PK4 pk4, EncounterCriteria criteria)
    {
        uint seed = Util.Rand32();
        if (pk4.PID != 1 && !IsManaphyEgg)
            return seed; // PID is already set.

        // The games don't decide the Nature/Gender up-front, but we can try to honor requests.
        // Pre-determine the result values, and generate something.
        var n = criteria.GetNature();
        // Gender is already pre-determined in the template.
        while (true)
        {
            seed = GeneratePID(seed, pk4);
            if (pk4.Nature != n)
                continue;
            return seed;
        }
    }

    private static void SetHatchedEggDetails(PK4 pk4)
    {
        pk4.IsEgg = false;
        // Met Location & Date is modified when transferred to pk5; don't worry about it.
        pk4.EggMetDate = EncounterDate.GetDateNDS();
    }

    private void SetUnhatchedEggDetails(PK4 pk4)
    {
        pk4.IsEgg = true;
        pk4.IsNicknamed = false;
        pk4.Nickname = SpeciesName.GetEggName(pk4.Language, Generation);
        pk4.EggMetDate = EncounterDate.GetDateNDS();
    }

    private static uint GeneratePID(uint seed, PK4 pk4)
    {
        do
        {
            uint pid1 = (seed = LCRNG.Next(seed)) >> 16; // low
            uint pid2 = (seed = LCRNG.Next(seed)) & 0xFFFF0000; // hi
            pk4.PID = pid2 | pid1;
            while (pk4.IsShiny) // Call the ARNG to change the PID
                pk4.PID = ARNG.Next(pk4.PID);
            // sanity check gender for non-genderless PID cases
        } while (!pk4.IsGenderValid());

        return seed;
    }

    public static bool IsRangerManaphy(PKM pk)
    {
        if (pk.Language >= (int)LanguageID.Korean) // never korean
            return false;

        var egg = pk.EggLocation;
        if (!pk.IsEgg) // Link Trade Egg or Ranger
            return egg is Locations.LinkTrade4 or Locations.Ranger4;
        if (egg != Locations.Ranger4)
            return false;

        var met = pk.MetLocation;
        return met is Locations.LinkTrade4 or 0;
    }

    // Nothing is stored as a PGT besides Ranger Manaphy. Nothing should trigger these.
    public override bool IsMatchExact(PKM pk, EvoCriteria evo) => false;
    protected override bool IsMatchDeferred(PKM pk) => false;
    protected override bool IsMatchPartial(PKM pk) => false;

    public bool RibbonEarth { get => PK.RibbonEarth; set => PK.RibbonEarth = value; }
    public bool RibbonNational { get => PK.RibbonNational; set => PK.RibbonNational = value; }
    public bool RibbonCountry { get => PK.RibbonCountry; set => PK.RibbonCountry = value; }
    public bool RibbonChampionBattle { get => PK.RibbonChampionBattle; set => PK.RibbonChampionBattle = value; }
    public bool RibbonChampionRegional { get => PK.RibbonChampionRegional; set => PK.RibbonChampionRegional = value; }
    public bool RibbonChampionNational { get => PK.RibbonChampionNational; set => PK.RibbonChampionNational = value; }
    public bool RibbonClassic { get => PK.RibbonClassic; set => PK.RibbonClassic = value; }
    public bool RibbonWishing { get => PK.RibbonWishing; set => PK.RibbonWishing = value; }
    public bool RibbonPremier { get => PK.RibbonPremier; set => PK.RibbonPremier = value; }
    public bool RibbonEvent { get => PK.RibbonEvent; set => PK.RibbonEvent = value; }
    public bool RibbonBirthday { get => PK.RibbonBirthday; set => PK.RibbonBirthday = value; }
    public bool RibbonSpecial { get => PK.RibbonSpecial; set => PK.RibbonSpecial = value; }
    public bool RibbonWorld { get => PK.RibbonWorld; set => PK.RibbonWorld = value; }
    public bool RibbonChampionWorld { get => PK.RibbonChampionWorld; set => PK.RibbonChampionWorld = value; }
    public bool RibbonSouvenir { get => PK.RibbonSouvenir; set => PK.RibbonSouvenir = value; }

    public bool IsCompatible(PIDType val, PKM pk)
    {
        if (IsManaphyEgg)
            return IsG4ManaphyPIDValid(val, pk);
        if (PK.PID != 1 && val == PIDType.G5MGShiny)
            return true;
        return val == PIDType.None;
    }

    public PIDType GetSuggestedCorrelation()
    {
        if (IsManaphyEgg)
            return PIDType.Method_1;
        return PIDType.None;
    }

    private static bool IsG4ManaphyPIDValid(PIDType val, PKM pk)
    {
        // Unhatched: Can't trigger ARNG, so it must always be Method 1
        if (pk.IsEgg)
            return val == PIDType.Method_1;

        // Hatching: Code checks if the TID/SID yield a shiny, and re-roll until not shiny.
        // However, the TID/SID reference is stale (original OT, not hatching OT), so it's fallible.
        // Hatched: Can't be shiny for an un-traded egg.
        if (val == PIDType.Method_1)
            return pk.WasTradedEgg || !pk.IsShiny;

        // Hatched when the egg was shiny: PID needs to be from the ARNG.
        return val == PIDType.G4MGAntiShiny;
    }
}

public enum GiftType4 : byte
{
    None = 0,
    Pokémon = 1,
    PokémonEgg = 2,
    Item = 3,
    Rule = 4,
    Goods = 5,
    HasSubType = 6,
    ManaphyEgg = 7,
    MemberCard = 8,
    OaksLetter = 9,
    AzureFlute = 10,
    PokétchApp = 11,
    SecretKey = 12,
    PokémonMovie = 13,
    PokéwalkerCourse = 14,
    MemorialPhoto = 15,
}

public enum GiftSubType4 : byte
{
    Seal = 1,
    Accessory = 2,
    Backdrop = 3,
}
