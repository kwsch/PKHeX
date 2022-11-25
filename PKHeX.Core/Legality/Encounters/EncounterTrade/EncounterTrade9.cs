namespace PKHeX.Core;

/// <summary>
/// Generation 9 Trade Encounter
/// </summary>
/// <inheritdoc cref="EncounterTrade"/>
public sealed record EncounterTrade9 : EncounterTrade, IGemType
{
    public override int Generation => 9;
    public override EntityContext Context => EntityContext.Gen9;
    public override int Location => Locations.LinkTrade6NPC;
    public override Shiny Shiny { get; }

    public GemType TeraType => GemType.Default;

    public EncounterTrade9(GameVersion game, ushort species, byte level, Shiny shiny = Shiny.Never) : base(game)
    {
        Species = species;
        Level = level;
        Shiny = shiny;
    }

    protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(sav, criteria, pk);

        var pk9 = (PK9)pk;
        pk9.HT_Language = (byte)pk.Language;
        pk9.Obedience_Level = Level;
        pk9.SetRandomEC();
        if (TeraType is GemType.Default)
            pk9.TeraTypeOriginal = (MoveType)PersonalTable.SV.GetFormEntry(Species, Form).Type1;
        else if (TeraType.IsSpecified(out var type))
            pk9.TeraTypeOriginal = (MoveType)type;
        else
            pk9.TeraTypeOriginal = (MoveType)Util.Rand.Next(0, 18);
        pk9.Scale = PokeSizeUtil.GetRandomScalar();
    }

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (TeraType != GemType.Random && pk is ITeraType t && !Tera9RNG.IsMatchTeraType(TeraType, Species, Form, (byte)t.TeraTypeOriginal))
            return false;
        return base.IsMatchExact(pk, evo);
    }
}
