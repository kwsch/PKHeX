using PKHeX.Core;

namespace PKHeX.Web.State;

/// <summary>One form the editor offers for a species, from Core's form list for the entity's context.</summary>
/// <param name="Form">The form number, its index in the list.</param>
/// <param name="Name">Core's name of the form; empty when Core gives it none.</param>
/// <param name="BattleOnly">True when the form exists only during battle (<see cref="FormInfo.IsBattleOnlyForm"/>), such as a Mega Evolution.</param>
/// <param name="InGame">
/// True when the save's game has the form (<see cref="IPersonalTable.IsPresentInGame"/>). The list is the generation's, so an X or Y save is
/// also offered forms only Omega Ruby and Alpha Sapphire have, as the desktop editor offers them.
/// </param>
public sealed record FormChoice(byte Form, string Name, bool BattleOnly, bool InGame);

/// <summary>The values a species or form change can alter, as the editor shows them before and after the change.</summary>
/// <param name="Species">The species.</param>
/// <param name="Form">The form.</param>
/// <param name="Level">The level, as Core derives it from the experience points.</param>
/// <param name="Experience">The experience points.</param>
/// <param name="Ability">The ability.</param>
/// <param name="AbilityNumber">The stored ability slot number (1, 2 or 4).</param>
/// <param name="Gender">The gender.</param>
/// <param name="Nickname">The name.</param>
/// <param name="IsNicknamed">The nickname flag.</param>
/// <param name="Stats">
/// The six stats in the summary's order (HP, Attack, Defense, Sp. Atk, Sp. Def, Speed): a party member's stored stats, as an apply will store
/// them, or the stats Core calculates for a boxed Pokémon, which stores none, as the inspector shows them.
/// </param>
public sealed record SpeciesFormValues(
    ushort Species,
    byte Form,
    byte Level,
    uint Experience,
    int Ability,
    int AbilityNumber,
    byte Gender,
    string Nickname,
    bool IsNicknamed,
    IReadOnlyList<int> Stats)
{
    /// <summary>The values of <paramref name="pk"/>, with stored stats for a party member and calculated ones otherwise.</summary>
    internal static SpeciesFormValues Of(PK6 pk, bool party)
    {
        IReadOnlyList<int> stats;
        if (party)
        {
            stats = [pk.Stat_HPMax, pk.Stat_ATK, pk.Stat_DEF, pk.Stat_SPA, pk.Stat_SPD, pk.Stat_SPE];
        }
        else
        {
            // Core's stat order is HP, Atk, Def, Spe, SpA, SpD; the summary's is HP, Atk, Def, SpA, SpD, Spe.
            var core = pk.GetStats(pk.PersonalInfo);
            stats = [core[0], core[1], core[2], core[4], core[5], core[3]];
        }
        return new SpeciesFormValues(pk.Species, pk.Form, pk.CurrentLevel, pk.EXP, pk.Ability, pk.AbilityNumber, pk.Gender, pk.Nickname, pk.IsNicknamed, stats);
    }
}

/// <summary>
/// What a species or form change would do to the draft, shown before it is made: the dependent fields Core reports changed, the
/// values before and after, and what legality analysis will say about the form.
/// </summary>
/// <param name="Before">The drafted values now.</param>
/// <param name="After">The values the change would give.</param>
/// <param name="Changes">
/// The dependent fields the change alters, as Core's <see cref="SpeciesFormChange.ChangeSpeciesForm(PKM,ushort,byte,IPersonalTable)"/> reports
/// them; when the change returns to where a run of changes started (<paramref name="Restores"/>), the fields that differ from the draft now.
/// </param>
/// <param name="BattleOnly">True when the new form exists only during battle, which legality analysis reports for a stored Pokémon.</param>
/// <param name="InGame">True when the save's game has the new species and form.</param>
/// <param name="Restores">
/// True when the change returns to the species and form a run of species and form changes started from, with nothing they changed edited since,
/// so the values they changed are given back rather than set again (see <see cref="EditorDraft.EditSpeciesForm"/>).
/// </param>
/// <param name="HpChange">For a party member, how the change moves its current and maximum HP from the draft's, or null when neither moves.</param>
/// <param name="Forms">The forms of the new species (as <see cref="EditorDraft.FormChoices"/> will list them after the change), so a form can be chosen with it.</param>
public sealed record SpeciesFormPreview(
    SpeciesFormValues Before,
    SpeciesFormValues After,
    SpeciesFormChangeResult Changes,
    bool BattleOnly,
    bool InGame,
    bool Restores,
    PartyHpChange? HpChange,
    IReadOnlyList<FormChoice> Forms)
{
    /// <summary>True when the species changes.</summary>
    public bool SpeciesChanged => Before.Species != After.Species;

    /// <summary>True when the level changes, which a different growth rate can do at the same experience points.</summary>
    public bool LevelChanged => Before.Level != After.Level;

    /// <summary>The summary-order indexes of the stats that change.</summary>
    public IReadOnlyList<int> ChangedStats => [.. Enumerable.Range(0, Before.Stats.Count).Where(i => Before.Stats[i] != After.Stats[i])];

    /// <summary>True when the change alters nothing: the species and form are the drafted ones.</summary>
    public bool IsNoChange => !SpeciesChanged && Before.Form == After.Form;
}
