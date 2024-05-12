using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 4 Mystery Gift Template File
/// </summary>
/// <remarks>
/// Big thanks to Grovyle91's Pokémon Mystery Gift Editor, from which the structure was referenced.
/// https://projectpokemon.org/home/profile/859-grovyle91/
/// https://projectpokemon.org/home/forums/topic/5870-pok%C3%A9mon-mystery-gift-editor-v143-now-with-bw-support/
/// See also: http://tccphreak.shiny-clique.net/debugger/pcdfiles.htm
/// </remarks>
public sealed class PCD(byte[] Data)
    : DataMysteryGift(Data), IRibbonSetEvent3, IRibbonSetEvent4, IRestrictVersion, IRandomCorrelation
{
    public PCD() : this(new byte[Size]) { }

    public const int Size = 0x358; // 856
    public override byte Generation => 4;
    public override EntityContext Context => EntityContext.Gen4;
    public override bool FatefulEncounter => Gift.PK.FatefulEncounter;
    public override GameVersion Version => Gift.Version;

    public override byte Level
    {
        get => Gift.Level;
        set => Gift.Level = value;
    }

    public override byte Ball
    {
        get => Gift.Ball;
        set => Gift.Ball = value;
    }

    public override byte[] Write()
    {
        // Ensure PGT content is encrypted
        var clone = new PCD((byte[])Data.Clone());
        if (clone.Gift.VerifyPKEncryption())
            clone.Gift = clone.Gift;
        return clone.Data;
    }

    public PGT Gift
    {
        get => _gift ??= new PGT(Data[..PGT.Size]);
        set => (_gift = value).Data.CopyTo(Data, 0);
    }

    private PGT? _gift;
    public GiftType4 GiftType => Gift.GiftType;

    public Span<byte> GetMetadata() => Data.AsSpan(PGT.Size);
    public void SetMetadata(ReadOnlySpan<byte> data) => data.CopyTo(Data.AsSpan(PGT.Size));

    public override bool GiftUsed { get => Gift.GiftUsed; set => Gift.GiftUsed = value; }
    public override bool IsEntity { get => Gift.IsEntity; set => Gift.IsEntity = value; }
    public override bool IsItem { get => Gift.IsItem; set => Gift.IsItem = value; }
    public override int ItemID { get => Gift.ItemID; set => Gift.ItemID = value; }
    public bool IsLockCapsule => IsItem && ItemID == 533; // 0x215
    public bool CanConvertToPGT => !IsLockCapsule;

    public override int CardID
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x150));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x150), (ushort)value);
    }

    private const int TitleLength = 0x48;

    private Span<byte> CardTitleSpan => Data.AsSpan(0x104, TitleLength);

    public override string CardTitle
    {
        get => StringConverter4.GetString(CardTitleSpan);
        set => StringConverter4.SetString(CardTitleSpan, value, TitleLength / 2, 0, StringConverterOption.ClearFF);
    }

    public ushort CardCompatibility => ReadUInt16LittleEndian(Data.AsSpan(0x14C)); // rest of bytes we don't really care about

    public override ushort Species { get => Gift.IsManaphyEgg ? (ushort)490 : Gift.Species; set => Gift.Species = value; }
    public override Moveset Moves { get => Gift.Moves; set => Gift.Moves = value; }
    public override int HeldItem { get => Gift.HeldItem; set => Gift.HeldItem = value; }
    public override bool IsShiny => Gift.IsShiny;
    public override Shiny Shiny => Gift.Shiny;
    public override bool IsEgg { get => Gift.IsEgg; set => Gift.IsEgg = value; }
    public override byte Gender { get => Gift.Gender; set => Gift.Gender = value; }
    public override byte Form { get => Gift.Form; set => Gift.Form = value; }
    public override uint ID32 { get => Gift.ID32; set => Gift.ID32 = value; }
    public override ushort TID16 { get => Gift.TID16; set => Gift.TID16 = value; }
    public override ushort SID16 { get => Gift.SID16; set => Gift.SID16 = value; }
    public override string OriginalTrainerName { get => Gift.OriginalTrainerName; set => Gift.OriginalTrainerName = value; }
    public override AbilityPermission Ability => Gift.Ability;
    public override bool HasFixedIVs => Gift.HasFixedIVs;
    public override void GetIVs(Span<int> value) => Gift.GetIVs(value);

    // ILocation overrides
    public override ushort Location { get => (ushort)(IsEgg ? 0 : Gift.EggLocation + 3000); set { } }
    public override ushort EggLocation { get => (ushort)(IsEgg ? Gift.EggLocation + 3000 : 0); set { } }

    public bool IsCompatible(PIDType val, PKM pk) => Gift.IsCompatible(val, pk);
    public PIDType GetSuggestedCorrelation() => Gift.GetSuggestedCorrelation();

    public bool GiftEquals(PGT pgt)
    {
        // Skip over the PGT's "Corresponding PCD Slot" @ 0x02
        byte[] g = pgt.Data;
        byte[] c = Gift.Data;
        if (g.Length != c.Length || g.Length < 3)
            return false;
        for (int i = 0; i < 2; i++)
        {
            if (g[i] != c[i])
                return false;
        }

        for (int i = 3; i < g.Length; i++)
        {
            if (g[i] != c[i])
                return false;
        }

        return true;
    }

    public override PK4 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        return Gift.ConvertToPKM(tr, criteria);
    }

    public bool CanBeReceivedByVersion(GameVersion version) => Version == version;

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        var wc = Gift.PK;
        if (!wc.IsEgg)
        {
            if (wc.TID16 != pk.TID16) return false;
            if (wc.SID16 != pk.SID16) return false;
            if (wc.OriginalTrainerName != pk.OriginalTrainerName) return false;
            if (wc.OriginalTrainerGender != pk.OriginalTrainerGender) return false;
            if (wc.Language != 0 && wc.Language != pk.Language) return false;

            if (pk.Format != 4) // transferred
            {
                // met location: deferred to general transfer check
                if (wc.CurrentLevel > pk.MetLevel) return false;
                if (!IsMatchEggLocation(pk))
                    return false;
            }
            else
            {
                if (wc.EggLocation + 3000 != pk.MetLocation) return false;
                if (wc.CurrentLevel != pk.MetLevel) return false;
            }
        }
        else // Egg
        {
            if (wc.EggLocation + 3000 != pk.EggLocation && pk.EggLocation != Locations.LinkTrade4) // traded
                return false;
            if (wc.CurrentLevel != pk.MetLevel)
                return false;
            if (pk is { IsEgg: true, Format: not 4 })
                return false;
        }

        if (wc.Form != evo.Form && !FormInfo.IsFormChangeable(wc.Species, wc.Form, pk.Form, Context, pk.Context))
            return false;

        if (wc.Ball != pk.Ball) return false;
        if (wc.OriginalTrainerGender < 3 && wc.OriginalTrainerGender != pk.OriginalTrainerGender) return false;

        // Milotic is the only gift to come with Contest stats.
        if (wc.Species == (int)Core.Species.Milotic && pk is IContestStatsReadOnly s && s.IsContestBelow(wc))
            return false;

        if (IsRandomPID())
        {
            // Random PID, never shiny
            // PID=0 was never used (pure random).
            if (pk.IsShiny)
                return false;

            // Don't check gender. All gifts that have PID=1 are genderless, with one exception.
            // The KOR Arcanine can end up with either gender, even though the template may have a specified gender.
        }
        else
        {
            // Fixed PID
            if (wc.Gender != pk.Gender)
                return false;
        }

        return true;
    }

    protected override bool IsMatchPartial(PKM pk) => !CanBeReceivedByVersion(pk.Version);
    protected override bool IsMatchDeferred(PKM pk) => false;

    public bool RibbonEarth { get => Gift.RibbonEarth; set => Gift.RibbonEarth = value; }
    public bool RibbonNational { get => Gift.RibbonNational; set => Gift.RibbonNational = value; }
    public bool RibbonCountry { get => Gift.RibbonCountry; set => Gift.RibbonCountry = value; }
    public bool RibbonChampionBattle { get => Gift.RibbonChampionBattle; set => Gift.RibbonChampionBattle = value; }
    public bool RibbonChampionRegional { get => Gift.RibbonChampionRegional; set => Gift.RibbonChampionRegional = value; }
    public bool RibbonChampionNational { get => Gift.RibbonChampionNational; set => Gift.RibbonChampionNational = value; }
    public bool RibbonClassic { get => Gift.RibbonClassic; set => Gift.RibbonClassic = value; }
    public bool RibbonWishing { get => Gift.RibbonWishing; set => Gift.RibbonWishing = value; }
    public bool RibbonPremier { get => Gift.RibbonPremier; set => Gift.RibbonPremier = value; }
    public bool RibbonEvent { get => Gift.RibbonEvent; set => Gift.RibbonEvent = value; }
    public bool RibbonBirthday { get => Gift.RibbonBirthday; set => Gift.RibbonBirthday = value; }
    public bool RibbonSpecial { get => Gift.RibbonSpecial; set => Gift.RibbonSpecial = value; }
    public bool RibbonWorld { get => Gift.RibbonWorld; set => Gift.RibbonWorld = value; }
    public bool RibbonChampionWorld { get => Gift.RibbonChampionWorld; set => Gift.RibbonChampionWorld = value; }
    public bool RibbonSouvenir { get => Gift.RibbonSouvenir; set => Gift.RibbonSouvenir = value; }

    public bool IsFixedPID() => Gift.PK.PID != 1;
    public bool IsRandomPID() => Gift.PK.PID == 1; // nothing used 0 (full random), always anti-shiny
}
