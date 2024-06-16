using System.ComponentModel;

namespace PKHeX.Core;

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class NicknameSettings
{
    [LocalizedDescription("Severity to flag a Legality Check if Pokémon has a Nickname matching another Species.")]
    public Severity NicknamedAnotherSpecies { get; set; } = Severity.Fishy;

    [LocalizedDescription("Nickname rules for Generation 1 and 2.")]
    public NicknameRestriction Nickname12 { get; set; } = new();

    [LocalizedDescription("Nickname rules for Generation 3.")]
    public NicknameRestriction Nickname3 { get; set; } = new();

    [LocalizedDescription("Nickname rules for Generation 4.")]
    public NicknameRestriction Nickname4 { get; set; } = new();

    [LocalizedDescription("Nickname rules for Generation 5.")]
    public NicknameRestriction Nickname5 { get; set; } = new();

    [LocalizedDescription("Nickname rules for Generation 6.")]
    public NicknameRestriction Nickname6 { get; set; } = new();

    [LocalizedDescription("Nickname rules for Generation 7.")]
    public NicknameRestriction Nickname7 { get; set; } = new();

    [LocalizedDescription("Nickname rules for Generation 7b.")]
    public NicknameRestriction Nickname7b { get; set; } = new();

    [LocalizedDescription("Nickname rules for Generation 8.")]
    public NicknameRestriction Nickname8 { get; set; } = new();

    [LocalizedDescription("Nickname rules for Generation 8a.")]
    public NicknameRestriction Nickname8a { get; set; } = new();

    [LocalizedDescription("Nickname rules for Generation 8b.")]
    public NicknameRestriction Nickname8b { get; set; } = new();

    [LocalizedDescription("Nickname rules for Generation 9.")]
    public NicknameRestriction Nickname9 { get; set; } = new();

    public void Disable()
    {
        var nick = new NicknameRestriction();
        nick.Disable();
        SetAllTo(nick);
    }

    public void SetAllTo(NicknameRestriction all)
    {
        Nickname12.CopyFrom(all);
        Nickname3.CopyFrom(all);
        Nickname4.CopyFrom(all);
        Nickname5.CopyFrom(all);
        Nickname6.CopyFrom(all);
        Nickname7.CopyFrom(all);
        Nickname7b.CopyFrom(all);
        Nickname8.CopyFrom(all);
        Nickname8a.CopyFrom(all);
        Nickname8b.CopyFrom(all);
        Nickname9.CopyFrom(all);
    }

    public Severity NicknamedMysteryGift(EntityContext encContext) => encContext switch
    {
        EntityContext.Gen1 => Nickname12.NicknamedMysteryGift,
        EntityContext.Gen2 => Nickname12.NicknamedMysteryGift,
        EntityContext.Gen3 => Nickname3.NicknamedMysteryGift,
        EntityContext.Gen4 => Nickname4.NicknamedMysteryGift,
        EntityContext.Gen5 => Nickname5.NicknamedMysteryGift,
        EntityContext.Gen6 => Nickname6.NicknamedMysteryGift,
        EntityContext.Gen7 => Nickname7.NicknamedMysteryGift,
        EntityContext.Gen7b => Nickname7b.NicknamedMysteryGift,
        EntityContext.Gen8 => Nickname8.NicknamedMysteryGift,
        EntityContext.Gen8a => Nickname8a.NicknamedMysteryGift,
        EntityContext.Gen8b => Nickname8b.NicknamedMysteryGift,
        EntityContext.Gen9 => Nickname9.NicknamedMysteryGift,
        _ => Severity.Valid,
    };

    public Severity NicknamedTrade(EntityContext encContext) => encContext switch
    {
        EntityContext.Gen1 => Nickname12.NicknamedTrade,
        EntityContext.Gen2 => Nickname12.NicknamedTrade,
        EntityContext.Gen3 => Nickname3.NicknamedTrade,
        EntityContext.Gen4 => Nickname4.NicknamedTrade,
        EntityContext.Gen5 => Nickname5.NicknamedTrade,
        EntityContext.Gen6 => Nickname6.NicknamedTrade,
        EntityContext.Gen7 => Nickname7.NicknamedTrade,
        EntityContext.Gen7b => Nickname7b.NicknamedTrade,
        EntityContext.Gen8 => Nickname8.NicknamedTrade,
        EntityContext.Gen8a => Nickname8a.NicknamedTrade,
        EntityContext.Gen8b => Nickname8b.NicknamedTrade,
        EntityContext.Gen9 => Nickname9.NicknamedTrade,
        _ => Severity.Valid,
    };
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed record NicknameRestriction
{
    [LocalizedDescription("Severity to flag a Legality Check if it is a nicknamed In-Game Trade the player cannot normally nickname.")]
    public Severity NicknamedTrade { get; set; } = Severity.Invalid;

    [LocalizedDescription("Severity to flag a Legality Check if it is a nicknamed Mystery Gift the player cannot normally nickname.")]
    public Severity NicknamedMysteryGift { get; set; } = Severity.Invalid;

    public void CopyFrom(NicknameRestriction other)
    {
        NicknamedTrade = other.NicknamedTrade;
        NicknamedMysteryGift = other.NicknamedMysteryGift;
    }

    public void Disable()
    {
        NicknamedTrade = Severity.Fishy;
        NicknamedMysteryGift = Severity.Fishy;
    }
}
