namespace PKHeX.Core;

/// <summary>
/// Modifies a property to have a "correct" value based on derived legality.
/// </summary>
public interface ISuggestModification
{
    public bool IsMatch(string name, string value, BatchInfo info);
    public ModifyResult Modify(string name, string value, BatchInfo info);
}
