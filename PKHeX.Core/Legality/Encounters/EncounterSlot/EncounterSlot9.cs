using System.Collections.Generic;
using static PKHeX.Core.AreaWeather9;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.SWSH"/>.
/// </summary>
/// <inheritdoc cref="EncounterSlot"/>
public sealed record EncounterSlot9 : EncounterSlot
{
    public override int Generation => 9;
    public override EntityContext Context => EntityContext.Gen9;
    public sbyte Gender { get; }
    public byte Time { get; } // disallow at time bit flag

    public EncounterSlot9(EncounterArea9 area, ushort species, byte form, byte min, byte max, sbyte gender, byte time) : base(area, species, form, min, max)
    {
        Gender = gender;
        Time = time;
    }

    protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(sav, criteria, pk);

        var pk9 = (PK9)pk;
        pk9.Obedience_Level = (byte)pk.Met_Level;
        var rand = new Xoroshiro128Plus(Util.Rand.Rand64());
        var type = Tera9RNG.GetTeraTypeFromPersonal(Species, Form, rand.Next());
        pk9.TeraTypeOriginal = (MoveType)type;
        if (criteria.TeraType != -1 && type != criteria.TeraType)
            pk9.SetTeraType(type); // sets the override type
        if (Gender != -1)
            pk.Gender = (byte)Gender;
        pk9.Scale = PokeSizeUtil.GetRandomScalar();
        if (Species == (int)Core.Species.Toxtricity)
            pk.Nature = ToxtricityUtil.GetRandomNature(ref rand, Form);
        pk9.EncryptionConstant = Util.Rand32();
    }

    private static int GetTime(RibbonIndex mark) => mark switch
    {
        RibbonIndex.MarkLunchtime => 0,
        RibbonIndex.MarkSleepyTime => 1,
        RibbonIndex.MarkDusk => 2,
        RibbonIndex.MarkDawn => 3,
        _ => 4,
    };

    public bool CanSpawnAtTime(RibbonIndex mark) => (Time & (1 << GetTime(mark))) == 0;

    public bool CanSpawnInWeather(RibbonIndex mark)
    {
        if (AreaWeather.TryGetValue((byte)Area.Location, out var areaWeather))
            return areaWeather.IsMarkCompatible(mark);
        return false;
    }

    /// <summary>
    /// Location IDs matched with possible weather types.
    /// </summary>
    internal static readonly Dictionary<byte, AreaWeather9> AreaWeather = new()
    {
        {   6, Standard },                       // South Province (Area One)
        {  10, Standard },                       // Pokémon League
        {  12, Standard },                       // South Province (Area Two)
        {  14, Standard },                       // South Province (Area Four)
        {  16, Standard },                       // South Province (Area Six)
        {  18, Standard },                       // South Province (Area Five)
        {  20, Standard },                       // South Province (Area Three)
        {  22, Standard },                       // West Province (Area One)
        {  24, Sand },                           // Asado Desert
        {  26, Standard },                       // West Province (Area Two)
        {  28, Standard },                       // West Province (Area Three)
        {  30, Standard },                       // Tagtree Thicket
        {  32, Standard },                       // East Province (Area Three)
        {  34, Standard },                       // East Province (Area One)
        {  36, Standard },                       // East Province (Area Two)
        {  38, Snow },                           // Glaseado Mountain (1)
        {  40, Standard },                       // Casseroya Lake
        {  44, Standard },                       // North Province (Area Three)
        {  46, Standard },                       // North Province (Area One)
        {  48, Standard },                       // North Province (Area Two)
        {  50, Standard },                       // Great Crater of Paldea
        {  56, Standard },                       // South Paldean Sea
        {  58, Standard },                       // West Paldean Sea
        {  60, Standard },                       // East Paldean Sea
        {  62, Standard },                       // North Paldean Sea
        {  64, Inside },                         // Inlet Grotto
        {  67, Inside },                         // Alfornada Cavern
        {  69, Standard | Inside | Snow | Snow },// Dalizapa Passage (Near Medali, Tunnels, Near Pokémon Center, Near Zapico)
        {  70, Standard },                       // Poco Path
        {  80, Standard },                       // Cabo Poco
        { 109, Standard },                       // Socarrat Trail
        { 124, Inside },                         // Area Zero (5)
    };
}
