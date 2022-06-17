namespace PKHeX.Core;

/// <summary>
/// Complex filter of data based on a string value.
/// </summary>
public interface IComplexFilterMeta
{
    bool IsMatch(string prop);
    bool IsFiltered(object cache, StringInstruction value);
}
