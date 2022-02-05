namespace PKHeX.Core;

/// <summary>
/// Interface for Accessing named blocks within a Generation 8 <see cref="GameVersion.PLA"/> save file.
/// </summary>
public interface ISaveBlock8LA
{
    Box8 BoxInfo { get; }
    Party8a PartyInfo { get; }
    MyStatus8a MyStatus { get; }
    PokedexSave8a PokedexSave { get; }
    BoxLayout8a BoxLayout { get; }
    MyItem8a Items { get; }
    AdventureStart8a AdventureStart { get; }
    LastSaved8a LastSaved { get; }
    PlayTime8a Played { get; }
}
