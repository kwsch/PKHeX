namespace PKHeX.Core;

public interface IEncounterArea<out T> where T : IEncounterTemplate, IVersion
{
    T[] Slots { get; }
}
