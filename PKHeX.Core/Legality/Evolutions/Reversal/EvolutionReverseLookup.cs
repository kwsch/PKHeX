using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Provides a reverse lookup mechanism for managing and retrieving evolution data based on species and form
/// identifiers.
/// </summary>
/// <remarks>This class is designed to efficiently store and retrieve evolution links for a large number of
/// species and forms. It supports registering evolution links and accessing them using species and form identifiers.
/// The lookup is optimized for performance and memory usage by leveraging internal indexing and key mapping.</remarks>
/// <param name="MaxSpecies">Maximum number of species supported by the lookup, matching the game context it provides for.</param>
public sealed class EvolutionReverseLookup(ushort MaxSpecies) : IEvolutionLookup
{
    private const int ExtraBufferFraction = 3; // magic number that gives a minimal-ish allocation
    private readonly EvolutionNode[] Nodes = new EvolutionNode[MaxSpecies + (MaxSpecies >> ExtraBufferFraction)];
    private readonly Dictionary<int, int> KeyLookup = new(MaxSpecies >> ExtraBufferFraction);

    private void Register(in EvolutionLink link, int index)
    {
        ref var node = ref Nodes[index];
        node.Add(link);
    }

    /// <summary>
    /// Registers an evolution link for a specific species and form.
    /// </summary>
    /// <remarks>
    /// If the specified form is 0, the method delegates to an overload that registers the link without considering the form.
    /// Otherwise, the species and form combination is used to  determine or create an index for registration.
    /// </remarks>
    /// <param name="link">The evolution link to register.</param>
    /// <param name="species">The species identifier associated with the evolution link.</param>
    /// <param name="form">The form identifier of the species.</param>
    public void Register(in EvolutionLink link, ushort species, byte form)
    {
        if (form == 0)
        {
            Register(link, species);
            return;
        }

        int index = GetOrAppendIndex(species, form);
        Register(link, index);
    }

    private int GetOrAppendIndex(ushort species, byte form)
    {
        int key = GetKey(species, form);
        if (KeyLookup.TryGetValue(key, out var index))
            return index;

        index = Nodes.Length - KeyLookup.Count - 1;
        KeyLookup.Add(key, index);
        return index;
    }

    private int GetIndex(ushort species, byte form)
    {
        if (species > MaxSpecies)
            return 0;
        if (form == 0)
            return species;
        int key = GetKey(species, form);
        return KeyLookup.GetValueOrDefault(key, 0);
    }

    private static int GetKey(ushort species, byte form) => species | form << 11;

    public ref readonly EvolutionNode this[ushort species, byte form] => ref Nodes[GetIndex(species, form)];
}
