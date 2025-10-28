using System.ComponentModel;

namespace PKHeX.Core;

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class WordFilterSettings
{
    [LocalizedDescription("Checks player given Nicknames and Trainer Names for profanity. Bad words will be flagged using the appropriate console's lists.")]
    public bool CheckWordFilter { get; set; } = true;

    [LocalizedDescription("Disables retroactive Word Filter checks for earlier formats.")]
    public bool DisableWordFilterPastGen { get; set; }

    public bool IsEnabled(int gen) => CheckWordFilter && (!DisableWordFilterPastGen || gen >= 5);
}
