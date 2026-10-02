using PKHeX.Core;
using PKHeX.Web.State;

namespace PKHeX.Web.Services;

/// <summary>
/// A stored value with Core's English name for it. Values are typed and carry no display text beyond the name; the UI formats them.
/// </summary>
/// <param name="Value">The stored value.</param>
/// <param name="Name">Core's name for the value, or null when Core has none, so the UI can label it as unknown without inventing a meaning.</param>
/// <param name="OutsideGameList">
/// True when the value is not in the session's Core list of values this game can hold (<see cref="SaveCapabilities.Lists"/>),
/// e.g. an item the game cannot have a Pokémon hold. Always false for values that have no such list.
/// </param>
public readonly record struct NamedValue(int Value, string? Name, bool OutsideGameList = false);

/// <summary>Which battle stats <see cref="StatsFacts.Stats"/> shows.</summary>
public enum StatsSource
{
    /// <summary>The battle stats stored with a party member, as the game last calculated them.</summary>
    Stored,

    /// <summary>Calculated by Core from base stats, IVs, EVs, level and nature; boxed Pokémon do not store battle stats.</summary>
    Calculated,

    /// <summary>
    /// A party member's battle stats as an unapplied stat edit recalculated them (<see cref="PartyStatPolicy"/>): what applying the draft
    /// stores.
    /// </summary>
    Recalculated,
}

/// <summary>One stat of the six, in the games' summary order (HP, Attack, Defense, Sp. Atk, Sp. Def, Speed).</summary>
/// <param name="Base">The species' base stat for the stored form.</param>
/// <param name="Iv">The individual value.</param>
/// <param name="Ev">The effort value.</param>
/// <param name="Value">The battle stat, from <see cref="StatsFacts.Source"/>.</param>
public sealed record StatRow(int Base, int Iv, int Ev, int Value);

/// <summary>One of the four move slots.</summary>
/// <param name="Move">The stored move; value 0 is an empty slot.</param>
/// <param name="Pp">Current PP.</param>
/// <param name="PpUps">PP Ups applied.</param>
/// <param name="MaxPp">The move's PP with those PP Ups, as Core calculates it; 0 for an empty slot.</param>
public sealed record MoveRow(NamedValue Move, int Pp, int PpUps, int MaxPp);

/// <summary>Who the Pokémon is: species, names and the values shown on its summary's first page.</summary>
/// <param name="Species">The stored species, with its national dex number as the value.</param>
/// <param name="Form">The stored form. The name is null when the form is outside Core's list for the species.</param>
/// <param name="HasForms">True when Core lists more than one form for the species.</param>
/// <param name="Nickname">The stored nickname (an egg's is the game's egg name).</param>
/// <param name="IsNicknamed">The stored nickname flag.</param>
/// <param name="IsEgg">True for an egg.</param>
/// <param name="Gender">The stored gender: 0 male, 1 female, 2 genderless.</param>
/// <param name="IsShiny">True when the PID is shiny for the stored trainer IDs.</param>
/// <param name="Language">The stored language.</param>
/// <param name="Level">The level Core derives from the experience points.</param>
/// <param name="Experience">The stored experience points.</param>
/// <param name="Nature">The stored nature. In Generation 6 it is stored apart from the PID.</param>
/// <param name="Ability">The stored ability.</param>
/// <param name="AbilitySlot">The stored ability number: 1 first, 2 second, 4 hidden; any other value is not a valid slot.</param>
/// <param name="HeldItem">The held item; value 0 is none.</param>
/// <param name="Friendship">The friendship value for the current handler.</param>
public sealed record IdentityFacts(
    NamedValue Species,
    NamedValue Form,
    bool HasForms,
    string Nickname,
    bool IsNicknamed,
    bool IsEgg,
    byte Gender,
    bool IsShiny,
    NamedValue Language,
    byte Level,
    uint Experience,
    NamedValue Nature,
    NamedValue Ability,
    int AbilitySlot,
    NamedValue HeldItem,
    byte Friendship);

/// <summary>The Pokémon's stats.</summary>
/// <param name="Source">Whether <paramref name="Stats"/> are stored or calculated.</param>
/// <param name="Stats">Six rows in the games' summary order.</param>
/// <param name="CurrentHp">
/// Current HP stored with a party member whose battle stats are stored; null for a boxed Pokémon, and for a party member without stored
/// stats, whose HP could not be read against the calculated maximum.
/// </param>
/// <param name="Status">
/// The status condition stored with a party member, as a Generation 5+ <see cref="StatusType"/> value (the low byte, as the desktop reads
/// it); null for a boxed Pokémon. The name is null when the value is not a status.
/// </param>
/// <param name="Characteristic">The characteristic (the phrase on the summary), from the IVs and encryption constant.</param>
/// <param name="HiddenPowerType">The type Hidden Power has, from the IVs.</param>
/// <param name="Nature">The stats the stored nature raises and lowers, which Core's calculation applies.</param>
public sealed record StatsFacts(
    StatsSource Source,
    IReadOnlyList<StatRow> Stats,
    int? CurrentHp,
    NamedValue? Status,
    NamedValue Characteristic,
    NamedValue HiddenPowerType,
    NatureEffect Nature)
{
    /// <summary>The sum of the six IVs.</summary>
    public int IvTotal => Stats.Sum(s => s.Iv);

    /// <summary>The sum of the six EVs.</summary>
    public int EvTotal => Stats.Sum(s => s.Ev);
}

/// <summary>The Pokémon's moves.</summary>
/// <param name="Moves">The four move slots, in order.</param>
/// <param name="Relearn">The four relearn move slots, in order; value 0 is empty.</param>
public sealed record MovesFacts(IReadOnlyList<MoveRow> Moves, IReadOnlyList<NamedValue> Relearn);

/// <summary>Where the Pokémon comes from and who has handled it.</summary>
/// <param name="TrainerName">The original trainer's name.</param>
/// <param name="TrainerGender">The original trainer's gender: 0 male, 1 female.</param>
/// <param name="IdFormat">How the origin game displays trainer IDs.</param>
/// <param name="DisplayTid">The original trainer's ID as the game displays it.</param>
/// <param name="DisplaySid">The original trainer's secret ID as the game displays it.</param>
/// <param name="OriginGame">The game the Pokémon was obtained in.</param>
/// <param name="MetLocation">Where it was met; the name is null when Core has none for the value.</param>
/// <param name="MetLevel">The level it was met at.</param>
/// <param name="MetDate">The date it was met, or null when the stored date is not a valid date.</param>
/// <param name="Ball">The ball it is in.</param>
/// <param name="EggLocation">Where the egg was received, or null when it did not hatch from an egg.</param>
/// <param name="EggDate">The date the egg was received, or null when there is none or it is not a valid date.</param>
/// <param name="FatefulEncounter">The fateful encounter flag (event Pokémon).</param>
/// <param name="TrainerFriendship">Friendship towards the original trainer.</param>
/// <param name="HandlerName">The latest handling trainer's name, or null when none is stored.</param>
/// <param name="HandlerGender">The handling trainer's gender: 0 male, 1 female.</param>
/// <param name="HandlerFriendship">Friendship towards the handling trainer.</param>
/// <param name="CurrentHandler">Who holds it now: 0 the original trainer, 1 the handling trainer.</param>
public sealed record OriginFacts(
    string TrainerName,
    byte TrainerGender,
    TrainerIDFormat IdFormat,
    uint DisplayTid,
    uint DisplaySid,
    NamedValue OriginGame,
    NamedValue MetLocation,
    byte MetLevel,
    DateOnly? MetDate,
    NamedValue Ball,
    NamedValue? EggLocation,
    DateOnly? EggDate,
    bool FatefulEncounter,
    byte TrainerFriendship,
    string? HandlerName,
    byte HandlerGender,
    byte HandlerFriendship,
    byte CurrentHandler);

/// <summary>Low-level values: identifiers, counts and flags not edited in this release.</summary>
/// <param name="Pid">The PID.</param>
/// <param name="EncryptionConstant">The encryption constant.</param>
/// <param name="RibbonCount">Ribbons held, as Core counts them for the format.</param>
/// <param name="PokerusStrain">The Pokérus strain; 0 with 0 days is never infected.</param>
/// <param name="PokerusDays">Days of Pokérus left; 0 with a strain means cured.</param>
/// <param name="Markings">The box markings, in the game's order (circle, triangle, square, heart, star, diamond).</param>
/// <param name="Format">Core's entity format name, e.g. PK6.</param>
/// <param name="ChecksumValid">True when the stored checksum matches the data.</param>
public sealed record AdvancedFacts(
    uint Pid,
    uint EncryptionConstant,
    int RibbonCount,
    int PokerusStrain,
    int PokerusDays,
    IReadOnlyList<bool> Markings,
    string Format,
    bool ChecksumValid);

/// <summary>
/// Read-only view of one PK6, grouped as the inspector shows it. Every value is read through a typed Core member; nothing is
/// enumerated by reflection, so the view is safe under trimming and cannot show a field this format does not have.
/// </summary>
/// <param name="Slot">The position the entity was read from.</param>
/// <param name="Identity">Species, names and summary values.</param>
/// <param name="Stats">Stats, IVs and EVs.</param>
/// <param name="Moves">Moves and relearn moves.</param>
/// <param name="Origin">Origin and trainers.</param>
/// <param name="Advanced">Identifiers, counts and flags.</param>
public sealed record EntityInspection(SlotRef Slot, IdentityFacts Identity, StatsFacts Stats, MovesFacts Moves, OriginFacts Origin, AdvancedFacts Advanced)
{
    /// <summary>
    /// Reads <paramref name="pk"/>, which was taken from <paramref name="slot"/>. The entity is not changed.
    /// </summary>
    /// <remarks>
    /// A party member's battle stats, HP and status are shown as stored. A boxed Pokémon stores none, so its stats are calculated
    /// by Core without writing them to the entity.
    /// </remarks>
    /// <param name="statsRecalculated">
    /// True when <paramref name="pk"/> is a drafted party member whose stored stats an unapplied edit recalculated, so they are shown as
    /// what an apply stores rather than as stored.
    /// </param>
    /// <param name="pk">The entity to read; pass a copy, as the draft's <see cref="EditorDraft.Preview"/> does.</param>
    /// <param name="slot">The position it was read from, which decides whether stored party stats apply.</param>
    /// <param name="capabilities">The session's capabilities, whose lists mark values this game cannot hold.</param>
    public static EntityInspection From(PK6 pk, SlotRef slot, SaveCapabilities capabilities, bool statsRecalculated = false)
    {
        var strings = GameInfo.Strings;
        var lists = capabilities.Lists;
        return new EntityInspection(slot, ReadIdentity(pk, strings, lists), ReadStats(pk, slot, strings, statsRecalculated), ReadMoves(pk, strings, lists), ReadOrigin(pk, lists), ReadAdvanced(pk));
    }

    private static IdentityFacts ReadIdentity(PK6 pk, GameStrings strings, FilteredGameDataSource lists)
    {
        var forms = FormConverter.GetFormList(pk.Species, strings.types, strings.forms, GameInfo.GenderSymbolUnicode, pk.Context);
        return new IdentityFacts(
            Listed(pk.Species, lists.Species, strings.specieslist),
            new NamedValue(pk.Form, pk.Form < forms.Length && forms[pk.Form].Length != 0 ? forms[pk.Form] : null),
            forms.Length > 1,
            pk.Nickname,
            pk.IsNicknamed,
            pk.IsEgg,
            pk.Gender,
            pk.IsShiny,
            Listed(pk.Language, lists.Languages, []),
            pk.CurrentLevel,
            pk.EXP,
            Named((int)pk.Nature, strings.natures),
            Listed(pk.Ability, lists.Abilities, strings.abilitylist),
            pk.AbilityNumber,
            Listed(pk.HeldItem, lists.Items, strings.itemlist),
            pk.CurrentFriendship);
    }

    private static StatsFacts ReadStats(PK6 pk, SlotRef slot, GameStrings strings, bool recalculated)
    {
        var personal = pk.PersonalInfo;
        // A party member stores the stats the game calculated; a boxed one has none, and calculating them here writes nothing to pk.
        var stored = slot.IsParty && pk.PartyStatsPresent;
        // Core's stat order is HP, Atk, Def, Spe, SpA, SpD; the summary's is HP, Atk, Def, SpA, SpD, Spe.
        ReadOnlySpan<int> values = stored
            ? [pk.Stat_HPMax, pk.Stat_ATK, pk.Stat_DEF, pk.Stat_SPA, pk.Stat_SPD, pk.Stat_SPE]
            : Calculated(pk.GetStats(personal));
        StatRow[] rows =
        [
            new(personal.HP, pk.IV_HP, pk.EV_HP, values[0]),
            new(personal.ATK, pk.IV_ATK, pk.EV_ATK, values[1]),
            new(personal.DEF, pk.IV_DEF, pk.EV_DEF, values[2]),
            new(personal.SPA, pk.IV_SPA, pk.EV_SPA, values[3]),
            new(personal.SPD, pk.IV_SPD, pk.EV_SPD, values[4]),
            new(personal.SPE, pk.IV_SPE, pk.EV_SPE, values[5]),
        ];
        var hiddenPower = strings.HiddenPowerTypes;
        return new StatsFacts(
            !stored ? StatsSource.Calculated : recalculated ? StatsSource.Recalculated : StatsSource.Stored,
            rows,
            stored ? pk.Stat_HPCurrent : null,
            slot.IsParty ? Status(pk.Status_Condition) : null,
            Named(pk.Characteristic, strings.characteristics),
            new NamedValue(pk.HPType, (uint)pk.HPType < (uint)hiddenPower.Length ? hiddenPower[pk.HPType] : null),
            NatureEffect.Of(pk.Nature));

        static int[] Calculated(ushort[] core) => [core[0], core[1], core[2], core[4], core[5], core[3]];
    }

    private static MovesFacts ReadMoves(PK6 pk, GameStrings strings, FilteredGameDataSource lists)
    {
        MoveRow Row(ushort move, int pp, int ppUps) =>
            new(Listed(move, lists.Moves, strings.movelist), pp, ppUps, move == 0 ? 0 : pk.GetMovePP(move, ppUps));
        NamedValue Relearn(ushort move) => Listed(move, lists.Relearn, strings.movelist);
        return new MovesFacts(
            [Row(pk.Move1, pk.Move1_PP, pk.Move1_PPUps), Row(pk.Move2, pk.Move2_PP, pk.Move2_PPUps), Row(pk.Move3, pk.Move3_PP, pk.Move3_PPUps), Row(pk.Move4, pk.Move4_PP, pk.Move4_PPUps)],
            [Relearn(pk.RelearnMove1), Relearn(pk.RelearnMove2), Relearn(pk.RelearnMove3), Relearn(pk.RelearnMove4)]);
    }

    private static OriginFacts ReadOrigin(PK6 pk, FilteredGameDataSource lists)
    {
        var hatched = pk.EggLocation != 0;
        return new OriginFacts(
            pk.OriginalTrainerName,
            pk.OriginalTrainerGender,
            pk.TrainerIDDisplayFormat,
            pk.DisplayTID,
            pk.DisplaySID,
            Listed((int)pk.Version, lists.Games, []),
            Location(pk, false, pk.MetLocation),
            pk.MetLevel,
            pk.MetDate,
            Listed(pk.Ball, lists.Balls, []),
            hatched ? Location(pk, true, pk.EggLocation) : null,
            hatched ? pk.EggMetDate : null,
            pk.FatefulEncounter,
            pk.OriginalTrainerFriendship,
            string.IsNullOrEmpty(pk.HandlingTrainerName) ? null : pk.HandlingTrainerName,
            pk.HandlingTrainerGender,
            pk.HandlingTrainerFriendship,
            pk.CurrentHandler);
    }

    private static AdvancedFacts ReadAdvanced(PK6 pk)
    {
        var markings = new bool[pk.MarkingCount];
        for (var i = 0; i < markings.Length; i++)
        {
            markings[i] = pk.GetMarking(i);
        }
        return new AdvancedFacts(pk.PID, pk.EncryptionConstant, pk.RibbonCount, pk.PokerusStrain, pk.PokerusDays, markings, nameof(PK6), pk.ChecksumValid);
    }

    /// <summary>
    /// The status in Generation 5+ formats: the low byte holds a <see cref="StatusType"/> value, not the Generation 1-4 flags of
    /// <see cref="StatusCondition"/>, as the desktop's status view reads it.
    /// </summary>
    private static NamedValue Status(int stored) => Named(stored & 0xFF, StatusNames);

    /// <summary>Names of the <see cref="StatusType"/> values, indexed by value. Core has no display strings for them.</summary>
    private static readonly string[] StatusNames = ["None", "Paralysis", "Sleep", "Freeze", "Burn", "Poison"];

    /// <summary>Core's location name for the origin game, or a null name when Core has none.</summary>
    private static NamedValue Location(PK6 pk, bool egg, ushort location)
    {
        var name = GameInfo.GetLocationName(egg, location, pk.Format, pk.Generation, pk.Version);
        return new NamedValue(location, string.IsNullOrEmpty(name) ? null : name);
    }

    /// <summary>The name at <paramref name="value"/> in <paramref name="names"/>, or a null name when it is out of range or blank.</summary>
    private static NamedValue Named(int value, IReadOnlyList<string> names) =>
        new(value, (uint)value < (uint)names.Count && names[value].Length != 0 ? names[value] : null);

    /// <summary>
    /// The name from the session's list when <paramref name="value"/> is in it; otherwise Core's general name, if
    /// <paramref name="names"/> has one, marked as outside the game's list.
    /// </summary>
    internal static NamedValue Listed(int value, IReadOnlyList<ComboItem> list, IReadOnlyList<string> names)
    {
        foreach (var item in list)
        {
            if (item.Value == value)
            {
                return new NamedValue(value, item.Text);
            }
        }
        return Named(value, names) with { OutsideGameList = true };
    }
}
