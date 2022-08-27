using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 4 Mystery Gift Template File (Inner Gift Data, no card data)
/// </summary>
public sealed class PGT : DataMysteryGift, IRibbonSetEvent3, IRibbonSetEvent4
{
    public const int Size = 0x104; // 260
    public override int Generation => 4;
    public override EntityContext Context => EntityContext.Gen4;

    public override byte Level
    {
        get => IsManaphyEgg ? (byte)1 : IsEntity ? (byte)PK.Met_Level : (byte)0;
        set { if (IsEntity) PK.Met_Level = value; }
    }

    public override int Ball
    {
        get => IsEntity ? PK.Ball : 0;
        set { if (IsEntity) PK.Ball = value; }
    }

    public override AbilityPermission Ability => IsManaphyEgg ? AbilityPermission.Any12 : (int)(PK.PID & 1) == 1 ? AbilityPermission.OnlySecond : AbilityPermission.OnlyFirst;

    private enum GiftType
    {
        Pokémon = 1,
        PokémonEgg = 2,
        Item = 3,
        Rule = 4,
        Seal = 5,
        Accessory = 6,
        ManaphyEgg = 7,
        MemberCard = 8,
        OaksLetter = 9,
        AzureFlute = 10,
        PokétchApp = 11,
        Ribbon = 12,
        PokéWalkerArea = 14,
    }

    public override string CardTitle { get => "Raw Gift (PGT)"; set { } }
    public override int CardID { get => -1; set { } }
    public override bool GiftUsed { get => false; set { } }
    public override Shiny Shiny => IsEgg ? Shiny.Random : PK.PID == 1 ? Shiny.Never : IsShiny ? Shiny.Always : Shiny.Never;

    public PGT() : this(new byte[Size]) { }
    public PGT(byte[] data) : base(data) { }

    public byte CardType { get => Data[0]; set => Data[0] = value; }
    // Unused 0x01
    public byte Slot { get => Data[2]; set => Data[2] = value; }
    public byte Detail { get => Data[3]; set => Data[3] = value; }
    public override int ItemID { get => ReadUInt16LittleEndian(Data.AsSpan(0x4)); set => WriteUInt16LittleEndian(Data.AsSpan(0x4), (ushort)value); }

    public PK4 PK
    {
        get => _pk ??= new PK4(Data.Slice(8, PokeCrypto.SIZE_4PARTY));
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
    /// Double checks the encryption of the gift data for Pokemon data.
    /// </summary>
    /// <returns>True if data was encrypted, false if the data was not modified.</returns>
    public bool VerifyPKEncryption()
    {
        if (PGTGiftType is not (GiftType.Pokémon or GiftType.PokémonEgg))
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

    private GiftType PGTGiftType { get => (GiftType)Data[0]; set => Data[0] = (byte)value; }
    public bool IsHatched => PGTGiftType == GiftType.Pokémon;
    public override bool IsEgg { get => PGTGiftType == GiftType.PokémonEgg || IsManaphyEgg; set { if (value) { PGTGiftType = GiftType.PokémonEgg; PK.IsEgg = true; } } }
    public bool IsManaphyEgg { get => PGTGiftType == GiftType.ManaphyEgg; set { if (value) PGTGiftType = GiftType.ManaphyEgg; } }
    public override bool EggEncounter => IsEgg;
    public override bool IsItem { get => PGTGiftType == GiftType.Item; set { if (value) PGTGiftType = GiftType.Item; } }
    public override bool IsEntity { get => PGTGiftType is GiftType.Pokémon or GiftType.PokémonEgg or GiftType.ManaphyEgg; set { } }

    public override ushort Species { get => IsManaphyEgg ? (ushort)490 : PK.Species; set => PK.Species = value; }
    public override Moveset Moves { get => new(PK.Move1, PK.Move2, PK.Move3, PK.Move4); set => PK.SetMoves(value); }
    public override int HeldItem { get => PK.HeldItem; set => PK.HeldItem = value; }
    public override bool IsShiny => PK.IsShiny;
    public override int Gender { get => PK.Gender; set => PK.Gender = value; }
    public override byte Form { get => PK.Form; set => PK.Form = value; }
    public override int TID { get => (ushort)PK.TID; set => PK.TID = value; }
    public override int SID { get => (ushort)PK.SID; set => PK.SID = value; }
    public override string OT_Name { get => PK.OT_Name; set => PK.OT_Name = value; }
    public override int Location { get => PK.Met_Location; set => PK.Met_Location = value; }
    public override int EggLocation { get => PK.Egg_Location; set => PK.Egg_Location = value; }
    public override bool HasFixedIVs => (PK.IV32 & 0x3FFF_FFFFu) != 0;
    public override void GetIVs(Span<int> value)
    {
        if (HasFixedIVs)
            PK.GetIVs(value);
    }

    public override PKM ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        if (!IsEntity)
            throw new ArgumentException(nameof(IsEntity));

        // template is already filled out, only minor mutations required
        PK4 pk4 = new((byte[])PK.Data.Clone()) { Sanity = 0 };
        if (!IsHatched && Detail == 0)
        {
            pk4.OT_Name = tr.OT;
            pk4.TID = tr.TID;
            pk4.SID = tr.SID;
            pk4.OT_Gender = tr.Gender;
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
        if (!EggEncounter)
        {
            pk4.Met_Location = pk4.Egg_Location + 3000;
            pk4.Egg_Location = 0;
            pk4.MetDate = DateTime.Now;
            pk4.IsEgg = false;
        }
        else
        {
            pk4.Egg_Location += 3000;
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
        pk4.Version = GameVersion.Gen4.Contains(trainer.Game) ? trainer.Game : (int)GameVersion.D;
        var lang = trainer.Language < (int)LanguageID.Korean ? trainer.Language : (int)LanguageID.English;
        pk4.Language = lang;
        pk4.Egg_Location = 1; // Ranger (will be +3000 later)
        pk4.Nickname = SpeciesName.GetSpeciesNameGeneration((int)Core.Species.Manaphy, lang, 4);
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
            uint iv1 = ((seed = RNG.LCRNG.Next(seed)) >> 16) & 0x7FFF;
            uint iv2 = ((RNG.LCRNG.Next(seed)) >> 16) & 0x7FFF;
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
        var n = (int)criteria.GetNature(Nature.Random);
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
        pk4.EggMetDate = DateTime.Now;
    }

    private void SetUnhatchedEggDetails(PK4 pk4)
    {
        pk4.IsEgg = true;
        pk4.IsNicknamed = false;
        pk4.Nickname = SpeciesName.GetEggName(pk4.Language, Generation);
        pk4.EggMetDate = DateTime.Now;
    }

    private static uint GeneratePID(uint seed, PK4 pk4)
    {
        do
        {
            uint pid1 = (seed = RNG.LCRNG.Next(seed)) >> 16; // low
            uint pid2 = (seed = RNG.LCRNG.Next(seed)) & 0xFFFF0000; // hi
            pk4.PID = pid2 | pid1;
            // sanity check gender for non-genderless PID cases
        } while (!pk4.IsGenderValid());

        while (pk4.IsShiny) // Call the ARNG to change the PID
            pk4.PID = RNG.ARNG.Next(pk4.PID);
        return seed;
    }

    public static bool IsRangerManaphy(PKM pk)
    {
        var egg = pk.Egg_Location;
        if (!pk.IsEgg) // Link Trade Egg or Ranger
            return egg is Locations.LinkTrade4 or Locations.Ranger4;
        if (egg != Locations.Ranger4)
            return false;

        if (pk.Language == (int)LanguageID.Korean) // never korean
            return false;

        var met = pk.Met_Location;
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
}
