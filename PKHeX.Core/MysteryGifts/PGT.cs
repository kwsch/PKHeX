using System;
using static System.Buffers.Binary.BinaryPrimitives;
using static PKHeX.Core.GiftType4;
using static PKHeX.Core.RandomCorrelationRating;

namespace PKHeX.Core;

/// <summary>
/// Generation 4 Mystery Gift Template File (Inner Gift Data, no card data)
/// </summary>
public sealed class PGT : DataMysteryGift, IRibbonSetEvent3, IRibbonSetEvent4, IRandomCorrelation
{
    public PGT() : this(new byte[Size]) { }
    public PGT(Memory<byte> raw) : base(raw) { }

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

    public ushort CardType { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }
    public byte Slot { get => Data[2]; set => Data[2] = value; }
    public byte Detail { get => Data[3]; set => Data[3] = value; }
    public override int ItemID { get => ReadInt32LittleEndian(Data[0x4..]); set => WriteInt32LittleEndian(Data[0x4..], value); }
    public int ItemSubID { get => ReadInt32LittleEndian(Data[0x8..]); set => WriteInt32LittleEndian(Data[0x8..], value); }
    public int PokewalkerCourseID { get => Data[0x4]; set => Data[0x4] = (byte)value; }

    public PK4 PK
    {
        get => _pk ??= new PK4(Data.Slice(8, PokeCrypto.SIZE_4PARTY).ToArray());
        set
        {
            _pk = value;
            var data = value.Data;
            bool zero = Array.TrueForAll(data, static z => z == 0); // all zero
            if (!zero)
                data = PokeCrypto.EncryptArray45(data);
            data.CopyTo(Data[8..]);
        }
    }

    public override ReadOnlySpan<byte> Write()
    {
        // Ensure PGT content is encrypted
        var clone = new PGT(Data.ToArray());
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
        if (ReadUInt32LittleEndian(Data[(0x64 + 8)..]) != 0)
            return false; // already encrypted (unused PK4 field, zero)
        EncryptPK();
        return true;
    }

    private void EncryptPK()
    {
        var span = Data.Slice(8, PokeCrypto.SIZE_4PARTY);
        var ekdata = PokeCrypto.EncryptArray45(span);
        ekdata.CopyTo(span);
    }

    public GiftType4 GiftType { get => (GiftType4)CardType; set => CardType = (byte)value; }
    public GiftSubType4 GiftSubType { get => (GiftSubType4)ItemID; set => ItemID = (int)value; }
    public PoketchApp PoketchApp { get => (PoketchApp)ItemID; set => ItemID = (int)value; }
    public Seal4 Seal { get => (Seal4)(ItemSubID - 1); set => ItemSubID = (int)(value + 1); }
    public Accessory4 Accessory { get => (Accessory4)ItemSubID; set => ItemSubID = (int)value; }
    public Backdrop4 Backdrop { get => (Backdrop4)ItemSubID; set => ItemSubID = (int)value; }

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

        // Template is already filled out, only minor mutations required
        byte[] clone = [.. PK.Data]; // disassociate from gift template
        PK4 pk4 = new(clone) { Sanity = 0 }; // clear for the bad WORLD08 Lucario (0x0100)
        var pi = pk4.PersonalInfo;

        if (IsEgg)
        {
            // Only hits here for the Manaphy gift. No gift eggs (type 2) were officially distributed.
            pk4.OriginalTrainerName = tr.OT;
            pk4.TID16 = tr.TID16;
            pk4.SID16 = tr.SID16;
            pk4.OriginalTrainerGender = tr.Gender;
            // Set Language and Version later for Manaphy, as we need to sanity check the input Trainer info.
            // No need to worry about regular egg gifts, because again, none exist.

            pk4.OriginalTrainerFriendship = pi.HatchCycles;
        }
        else
        {
            pk4.OriginalTrainerFriendship = pi.BaseFriendship;
        }

        if (IsManaphyEgg)
        {
            SetDefaultManaphyEggDetails(pk4, tr);
            SetPINGAManaphy(pk4, criteria, tr);
        }
        else
        {
            SetPINGA(pk4, pi, criteria);
            SetMetData(pk4, tr, criteria);
        }

        pk4.RefreshChecksum();
        return pk4;
    }

    private static void SetPINGAManaphy(PK4 pk4, EncounterCriteria criteria, ITrainerInfo tr)
    {
        if (criteria.IsSpecifiedIVsAll() && TrySetManaphyFromIVs(pk4, criteria, tr))
            return;

        var seed = Util.Rand32();
        bool filterIVs = criteria.IsSpecifiedIVs(2);
        while (true)
        {
            // Generate PID
            var a = LCRNG.Next16(ref seed);
            var b = LCRNG.Next16(ref seed);
            var pid = (b << 16) | a;
            if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature((Nature)(pid % 25)))
                continue;
            var c = LCRNG.Next15(ref seed);
            var d = LCRNG.Next15(ref seed);
            var iv32 = d << 15 | c;
            if (criteria.IsSpecifiedHiddenPower() && !criteria.IsSatisfiedHiddenPower(iv32))
                continue;
            if (filterIVs && !criteria.IsSatisfiedIVs(iv32))
                continue;

            var xor = (a ^ b) >> 3;
            var trXor = (tr.TID16 ^ tr.SID16) >> 3;
            bool shiny = xor == trXor;
            if (criteria.Shiny.IsShiny() != shiny)
                continue;

            if (shiny)
                SetAsTradedManaphy(pk4, shiny);
            pk4.PID = pid;
            pk4.IV32 |= iv32;
            break;
        }
    }

    private static bool TrySetManaphyFromIVs(PK4 pk4, EncounterCriteria criteria, ITrainerInfo tr)
    {
        Span<uint> seeds = stackalloc uint[LCRNG.MaxCountSeedsIV];
        criteria.GetCombinedIVs(out var iv1, out var iv2);
        int count = LCRNGReversal.GetSeedsIVs(seeds, iv1 << 16, iv2 << 16);
        foreach (var seed in seeds[..count])
        {
            var b = seed >> 16;
            var a = LCRNG.Prev(seed) >> 16;
            var pid = (b << 16) | a;

            // Check for anti-shiny.
            var xor = (a ^ b) >> 3;
            bool arng = false;
            if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature((Nature)(pid % 25)))
            {
                while (true)
                {
                    pid = ARNG.Next(pid);
                    var newXor = (pid >> 16 ^ pid) >> 3;
                    if (newXor == xor)
                        continue;
                    xor = newXor;
                    arng = true;
                    break;
                }
                if (!criteria.IsSatisfiedNature((Nature)(pid % 25)))
                    continue;
            }

            var trXor = (tr.TID16 ^ tr.SID16) >> 3;
            bool shiny = xor == trXor;
            if (criteria.Shiny.IsShiny() != shiny)
                continue;

            if (shiny || arng) // set even for ARNG to ensure was shiny for the original recipient
                SetAsTradedManaphy(pk4, shiny);
            pk4.PID = pid;
            pk4.IV32 |= iv2 << 15 | iv1;
            return true;
        }
        return false;
    }

    private static void SetAsTradedManaphy(PK4 pk4, bool shiny)
    {
        // Must be traded; can't be shiny as an egg.
        if (pk4.IsEgg)
        {
            if (!shiny)
            {
                pk4.MetLocation = Locations.LinkTrade4;
                pk4.EggLocation = Locations.Ranger4;
                return;
            }

            // Force hatch, we want to retain the trainer IDs from the request rather than leave the egg unhatched.
            pk4.IsEgg = false;
            pk4.IsNicknamed = false;
            pk4.Nickname = SpeciesName.GetSpeciesNameGeneration((ushort)Core.Species.Manaphy, pk4.Language, 4);
        }
        pk4.MetLocation = pk4.Version is GameVersion.HG or GameVersion.SS ? Locations.HatchLocationHGSS : Locations.HatchLocationDPPt;
        pk4.EggLocation = Locations.LinkTrade4;
    }

    private void SetMetData(PK4 pk4, ITrainerInfo trainer, EncounterCriteria criteria)
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
            // Never reaches here because no egg PGTs were distributed besides Manaphy.
            if (criteria.Shiny.IsShiny())
                pk4.EggLocation = Locations.LinkTrade4;
            else
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

        // Manaphy is the only PGT gift egg! (and the only gift that needs a version to be set)
        var version = trainer.Version;
        if (!version.IsGen4())
            version = GameVersion.D;
        pk4.Version = version;

        var lang = trainer.Language;
        if ((uint)lang is >= (uint)LanguageID.Korean or (0 or (int)LanguageID.UNUSED_6))
            lang = (int)LanguageID.English; // Don't allow Korean or unused languages
        pk4.Language = lang;

        pk4.IsEgg = true;
        pk4.Nickname = SpeciesName.GetEggName(lang, 4);
        pk4.EggLocation = Locations.Ranger4;
        pk4.EggMetDate = pk4.MetDate = EncounterDate.GetDateNDS();
    }

    public bool HasPID => PK.PID > 1; // 0=Random, 1=Random (Anti-Shiny). 0 was never used in any Gen4 gift (all non-shiny).
    public bool HasIVs => (PK.IV32 & 0x3FFF_FFFFu) != 0; // ignore Nickname/Egg flag bits

    private static void SetPINGA(PK4 pk4, PersonalInfo4 pi, EncounterCriteria criteria)
    {
        // Ability is forced already, can't force anything

        // Generate PID
        pk4.PID = GetPID(pk4, pi, criteria);

        // Ignore Nickname/Egg flag bits
        if ((pk4.IV32 & 0x3FFF_FFFFu) != 0)
            return; // IVs are already set

        // Generate IVs
        if (criteria.IsSpecifiedIVsAll())
        {
            pk4.IV32 |= criteria.GetCombinedIVs();
            return;
        }
        var seed = Util.Rand32(); // reseed, do not have method 1 correlation
        bool filterIVs = criteria.IsSpecifiedIVs(2);
        uint iv32;
        while (true)
        {
            iv32 = ClassicEraRNG.GetSequentialIVs(ref seed);
            if (criteria.IsSpecifiedHiddenPower() && !criteria.IsSatisfiedHiddenPower(iv32))
                continue;
            if (filterIVs && !criteria.IsSatisfiedIVs(iv32))
                continue;
            break;
        }
        pk4.IV32 |= iv32;
    }

    private static uint GetPID(PK4 pk4, PersonalInfo4 pi, EncounterCriteria criteria)
    {
        var template = pk4.PID;
        if (template > 1) // 0=Random, 1=Random (Anti-Shiny). 0 was never used in any Gen4 gift (all non-shiny).
            return template; // PID is already set.

        // The games don't decide the Nature/Gender up-front, but we can try to honor requests.
        // Gender is already pre-determined in the template.
        uint seed = Util.Rand32();
        var gender = pk4.Gender;
        var gr = pi.Gender;
        var trXor = (pk4.TID16 ^ pk4.SID16) >> 3;
        while (true)
        {
            var pid = seed = LCRNG.Next(seed);
            while (true)
            {
                var xor = (pid >> 16 ^ pid) >> 3;
                if (xor != trXor)
                    break;
                pid = ARNG.Next(pid);
            }
            if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature((Nature)(pid % 25)))
                continue;
            if (EntityGender.GetFromPIDAndRatio(pid, gr) != gender)
                continue;
            return pid;
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

    /// <summary>
    /// Rough check to see if the <see cref="pk"/> matches the expected Ranger Manaphy details.
    /// </summary>
    public static bool IsRangerManaphy(PKM pk)
    {
        if (pk.Species is not (ushort)Core.Species.Manaphy)
            return false;

        // Never Korean or future language values; others (0, 6) will get flagged by other checks.
        if (pk.Language >= (int)LanguageID.Korean)
            return false;

        var egg = pk.EggLocation;
        if (!pk.IsEgg) // Link Trade Egg or Ranger must be present in the Egg Met field. Met location is hatch location or Transfer (Gen5+).
            return egg is Locations.LinkTrade4 or Locations.Ranger4;

        if (pk.Format != 4)
            return false; // Can't be an Egg if it must be transferred.

        // Logic below only applies to Eggs.
        // While an egg, Egg Location must still be Ranger.
        // If traded while an egg, the Met Location will be updated from 0 => Link Trade.
        // When hatched, the Met Location will move to Egg Location, if non-zero (aka, Traded).
        if (egg != Locations.Ranger4)
            return false;
        var met = pk.MetLocation;
        return met is Locations.LinkTrade4 or 0;
    }

    /// <summary>
    /// Nothing should call these methods. Should be checked via the static method <see cref="IsRangerManaphy"/> prior to yielding the match.
    /// </summary>
    /// <remarks><see cref="PCD"/> objects will instead call their respective methods.</remarks>
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

    public RandomCorrelationRating IsCompatible(PIDType type, PKM pk)
    {
        if (IsManaphyEgg)
            return IsG4ManaphyPIDValid(type, pk) ? Match : Mismatch;
        if (PK.PID != 1 && type == PIDType.G5MGShiny)
            return Match;
        return type is PIDType.None ? Match : Mismatch;
    }

    public PIDType GetSuggestedCorrelation()
    {
        if (IsManaphyEgg)
            return PIDType.Method_1;
        return PIDType.None;
    }

    private static bool IsG4ManaphyPIDValid(PIDType val, PKM pk)
    {
        // Can trigger ARNG immediately on receipt, but only if it was shiny.
        // Can trigger ARNG on hatch, but only if it was un-traded.
        if (val is PIDType.Method_1)
        {
            if (pk.IsEgg)
                return !pk.IsShiny;
            return pk.WasTradedEgg || !pk.IsShiny;
        }

        if (val is not PIDType.G4MGAntiShiny)
            return false;

        // Un-hatched Traded eggs will retain the recipient OT data, and must still match.
        // Hatched Traded eggs can be disassociated from the original trainer data.
        if (pk is { IsEgg: false, WasTradedEgg: true })
            return true; // don't bother checking if it is shiny or not; game doesn't re-roll if it was traded.

        // To be anti-shiny, it must be shiny for the current OT, or a traded egg.
        var pid = pk.PID;
        pid = ARNG.Prev(pid);
        var tsv = (pk.TID16 ^ pk.SID16) >> 3;
        var psv = ((pid & 0xFFFF) ^ (pid >> 16)) >> 3;
        return tsv == psv;
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
