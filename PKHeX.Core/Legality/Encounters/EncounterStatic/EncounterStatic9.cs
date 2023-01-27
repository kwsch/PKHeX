namespace PKHeX.Core;

/// <summary>
/// Generation 9 Static Encounter
/// </summary>
/// <inheritdoc cref="EncounterStatic"/>
public sealed record EncounterStatic9(GameVersion Version) : EncounterStatic(Version), IGemType
{
    public override int Generation => 9;
    public override EntityContext Context => EntityContext.Gen9;
    public byte Size { get; init; }
    public GemType TeraType { get; init; }
    public bool IsTitan { get; init; }

    private bool NoScalarsDefined => Size == 0;
    public bool GiftWithLanguage => Gift && !ScriptedYungoos; // Nice error by GameFreak -- all gifts (including eggs) set the HT_Language memory value in addition to OT_Language.
    public bool StarterBoxLegend => Gift && Species is (int)Core.Species.Koraidon or (int)Core.Species.Miraidon;
    public bool ScriptedYungoos => Species == (int)Core.Species.Yungoos && Level == 2;

    protected override bool IsMatchPartial(PKM pk)
    {
        if (pk is IScaledSize v && !NoScalarsDefined)
        {
            if (Gift)
            {
                if (v.HeightScalar != Size)
                    return true;
                if (v.WeightScalar != Size)
                    return true;
            }
            if (pk is PK9 pk9)
            {
                if (pk9.Scale != Size)
                    return true;
            }
        }
        return base.IsMatchPartial(pk);
    }

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (TeraType != GemType.Random && pk is ITeraType t && !Tera9RNG.IsMatchTeraType(TeraType, Species, Form, (byte)t.TeraTypeOriginal))
            return false;
        return base.IsMatchExact(pk, evo);
    }

    protected override void ApplyDetails(ITrainerInfo tr, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(tr, criteria, pk);
        var pk9 = (PK9)pk;
        if (Gift && !ScriptedYungoos)
            pk9.HT_Language = (byte)pk.Language;
        if (StarterBoxLegend)
            pk9.FormArgument = 1; // Not Ride Form.
        if (IsTitan)
            pk9.RibbonMarkTitan = true;
        pk9.Obedience_Level = (byte)pk9.Met_Level;

        const byte undefinedSize = 0;
        byte height, weight, scale;
        if (NoScalarsDefined)
        {
            height = weight = scale = undefinedSize;
        }
        else
        {
            // Gifts have a defined H/W/S, while capture-able only have scale.
            height = weight = Gift ? Size : undefinedSize;
            scale = Size;
        }

        const byte rollCount = 1;
        var pi = PersonalTable.SV.GetFormEntry(Species, Form);
        var param = new GenerateParam9(Species, pi.Gender, FlawlessIVCount, rollCount, height, weight, scale, Ability, Shiny);

        ulong init = Util.Rand.Rand64();
        var success = this.TryApply64(pk9, init, param, criteria, IVs.IsSpecified);
        if (!success)
            this.TryApply64(pk9, init, param, EncounterCriteria.Unrestricted, IVs.IsSpecified);
        if (IVs.IsSpecified)
        {
            pk.IV_HP = IVs.HP;
            pk.IV_ATK = IVs.ATK;
            pk.IV_DEF = IVs.DEF;
            pk.IV_SPA = IVs.SPA;
            pk.IV_SPD = IVs.SPD;
            pk.IV_SPE = IVs.SPE;
        }
        if (Nature != Nature.Random)
            pk.Nature = pk.StatNature = (int)Nature;
        if (Gender != -1)
            pk.Gender = (byte)Gender;
    }
}
