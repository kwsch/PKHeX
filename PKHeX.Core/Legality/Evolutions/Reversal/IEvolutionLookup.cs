namespace PKHeX.Core;

public interface IEvolutionLookup
{
    ref readonly EvolutionNode this[ushort species, byte form] { get; }
}
