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
    int ILocation.Location => MaxLair;
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

    // no downleveling, unlike all other raids
    protected override bool IsMatchLevel(PKM pk) => pk.Met_Level == Level;
    protected override bool IsMatchLocation(PKM pk) => Location == pk.Met_Location;

    public bool IsShinyXorValid(ushort pkShinyXor) => pkShinyXor is > 15 or 1;

    public bool ShouldHaveScientistTrash => !SpeciesCategory.IsLegendary(Species)
                                         && !SpeciesCategory.IsSubLegendary(Species);

    protected override void FinishCorrelation(PK8 pk, ulong seed)
    {
        if (!ShouldHaveScientistTrash)
            return;

        ApplyTrashBytes(pk);
    }

    public void ApplyTrashBytes(PKM pk)
    {
        // Normally we would apply the trash before applying the OT, but we already did.
        // Just add in the expected trash after the OT.
        var ot = pk.OT_Trash;
        var language = pk.Language;
        var scientist = GetScientistName(language);
        StringConverter8.ApplyTrashBytes(ot, scientist);
    }

    public static TrashMatch HasScientistTrash(PKM pk)
    {
        var language = pk.Language;
        var name = GetScientistName(language);
        return StringConverter8.GetTrashState(pk.OT_Trash, name);
    }

    private static ReadOnlySpan<char> GetScientistName(int language) => language switch
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
        _ => ReadOnlySpan<char>.Empty,
    };
}
