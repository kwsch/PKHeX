namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.Gen7"/> (GO Park, <seealso cref="GameVersion.GG"/>).
/// <inheritdoc cref="EncounterSlotGO" />
/// </summary>
public sealed record EncounterSlot7GO : EncounterSlotGO
{
    public override int Generation => 7;
    public override EntityContext Context => EntityContext.Gen7b;
    public override Ball FixedBall => Ball.None; // GO Park can override the ball; obey capture rules for LGP/E

    public EncounterSlot7GO(EncounterArea7g area, ushort species, byte form, int start, int end, Shiny shiny, Gender gender, PogoType type)
        : base(area, species, form, start, end, shiny, gender, type)
    {
    }

    protected override PKM GetBlank() => new PB7();

    protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(sav, criteria, pk);
        var pb = (PB7) pk;
        pb.AwakeningSetAllTo(2);
        pk.SetRandomEC();
        pb.HeightScalar = PokeSizeUtil.GetRandomScalar();
        pb.WeightScalar = PokeSizeUtil.GetRandomScalar();
        pb.ResetHeight();
        pb.ResetWeight();
        pb.ResetCP();
    }

    protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
    {
        var pi = pk.PersonalInfo;
        int gender = criteria.GetGender(-1, pi);
        int nature = (int)criteria.GetNature(Nature.Random);
        var ability = criteria.GetAbilityFromNumber(Ability);

        pk.PID = Util.Rand32();
        pk.Nature = pk.StatNature = nature;
        pk.Gender = gender;
        pk.RefreshAbility(ability);
        pk.SetRandomIVsGO();
        base.SetPINGA(pk, criteria);
    }

    protected override void SetEncounterMoves(PKM pk, GameVersion version, int level)
    {
        var moves = MoveLevelUp.GetEncounterMoves(pk, level, GameVersion.GG);
        pk.SetMoves(moves);
        pk.SetMaximumPPCurrent(moves);
    }
}
