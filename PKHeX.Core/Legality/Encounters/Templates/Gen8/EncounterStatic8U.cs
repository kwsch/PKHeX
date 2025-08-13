using System;
using static PKHeX.Core.Encounters8Nest;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 Nest Encounter (Max Raid) Underground
/// </summary>
/// <inheritdoc cref="EncounterStatic8Nest{T}"/>
public sealed record EncounterStatic8U : EncounterStatic8Nest<EncounterStatic8U>, ILocation
{
    ushort ILocation.Location => MaxLair;
    private const ushort Location = MaxLair;
    public override string Name => "Max Lair Encounter";

    public EncounterStatic8U(ushort species, byte form, byte level) : base(GameVersion.SWSH) // no difference in met location for hosted raids
    {
        Species = species;
        Form = form;
        Level = level;
        DynamaxLevel = 8;
        FlawlessIVCount = 4;
    }

    public static EncounterStatic8U Read(ReadOnlySpan<byte> data)
    {
        var spec = ReadUInt16LittleEndian(data);
        var move1 = ReadUInt16LittleEndian(data[4..]);
        var move2 = ReadUInt16LittleEndian(data[6..]);
        var move3 = ReadUInt16LittleEndian(data[8..]);
        var move4 = ReadUInt16LittleEndian(data[10..]);
        var moves = new Moveset(move1, move2, move3, move4);

        return new EncounterStatic8U(spec, data[2], data[3])
        {
            Ability = (AbilityPermission)data[12],
            CanGigantamax = data[13] != 0,
            Moves = moves,
        };
    }
    protected override ushort GetLocation() => Location;

    protected override void SetTrainerName(ReadOnlySpan<char> name, PK8 pk)
    {
        if (ShouldHaveScientistTrash)
            base.SetTrainerName(GetScientistName(pk.Language), pk);
        base.SetTrainerName(name, pk);
    }

    // These raids are always generated as Never-Shiny, and only at the choice screen are they possibly set shiny.
    private const Shiny ShinyMethod = Shiny.Never;
    private const byte ShinyXor = 1; // If forced shiny, the XOR is always 1.

    // Need to override to ensure the fallback also uses Never-Shiny.
    protected override void SetPINGA(PK8 pk, in EncounterCriteria criteria, PersonalInfo8SWSH pi)
    {
        Span<int> iv = stackalloc int[6];

        int ctr = 0;
        var rand = new Xoroshiro128Plus(Util.Rand.Rand64());
        var param = GetParam(pi);
        ulong seed;
        const int max = 100_000;
        do
        {
            if (TryApply(pk, seed = rand.Next(), iv, param, criteria))
                break;
        } while (++ctr < max);

        if (ctr == max) // fail
        {
            if (!TryApply(pk, seed = rand.Next(), iv, param, criteria.WithoutIVs()))
            {
                var tmp = EncounterCriteria.Unrestricted with { Shiny = ShinyMethod };
                while (!TryApply(pk, seed = rand.Next(), iv, param, tmp)) { }
            }
        }

        FinishCorrelation(pk, seed);
        if (criteria.IsSpecifiedNature() && criteria.Nature != pk.Nature && criteria.Nature.IsMint())
            pk.StatNature = criteria.Nature;
        if (criteria.Shiny.IsShiny())
            pk.PID = ShinyUtil.GetShinyPID(pk.TID16, pk.SID16, pk.PID, ShinyXor);
    }

    private GenerateParam8 GetParam(PersonalInfo8SWSH pi)
    {
        var ratio = RemapGenderToParam(Gender, pi);
        return new GenerateParam8(Species, ratio, FlawlessIVCount, Ability, ShinyMethod, Nature.Random, IVs);
    }

    // no downleveling, unlike all other raids
    protected override bool IsMatchLevel(PKM pk) => pk.MetLevel == Level;
    protected override bool IsMatchLocation(PKM pk) => Location == pk.MetLocation;

    public bool IsShinyXorValid(ushort pkShinyXor) => pkShinyXor is > 15 or ShinyXor; // not shiny, or shiny with XOR=1

    public bool ShouldHaveScientistTrash => Level != 70; // Level 65, not legendary/sub-legendary/ultra beast

    public static ReadOnlySpan<char> GetScientistName(int language) => language switch
    {
        (int)LanguageID.Japanese => "けんきゅういん",
        (int)LanguageID.English => "Scientist",
        (int)LanguageID.French => "Scientifique",
        (int)LanguageID.Italian => "Scienziata",
        (int)LanguageID.German => "Forscherin",
        (int)LanguageID.Spanish => "Científica",
        (int)LanguageID.Korean => "연구원",
        (int)LanguageID.ChineseS => "研究员",
        (int)LanguageID.ChineseT => "研究員",
        _ => [],
    };
}
