using System.ComponentModel;

namespace PKHeX.Core;

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class BulkAnalysisSettings
{
    [LocalizedDescription("Checks the save file data and Current Handler state to determine if the Pokémon's Current Handler does not match the expected value.")]
    public bool CheckActiveHandler { get; set; } = true;
}
