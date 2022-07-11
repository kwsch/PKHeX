using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Level-Up Lookup object
/// </summary>
public sealed class LearnLookup
{
    private readonly PersonalTable Table;
    private readonly Learnset[] Learn;

    public LearnLookup(PersonalTable table, Learnset[] learn)
    {
        Table = table;
        Learn = learn;
    }

    public List<int> AddMovesIndex(List<int> moves, int index, int max, int min)
    {
        if (index <= 0)
            return moves;
        return Learn[index].AddMoves(moves, max, min);
    }

    public List<int> AddMoves(List<int> moves, int species, int form, int max, int min = 0)
    {
        int index = Table.GetFormIndex(species, form);
        return AddMovesIndex(moves, index, max, min);
    }

    public List<int> AddMoves1(List<int> moves, int species, int form, int max, int min)
    {
        int index = Table.GetFormIndex(species, form);
        return AddMovesIndex1(moves, index, max, min);
    }

    public List<int> AddMovesIndex1(List<int> moves, int index, int max, int min)
    {
        if (min == 1)
            moves.AddRange(((PersonalInfoG1)Table[index]).Moves);
        return AddMovesIndex(moves, index, max, min);
    }
}
