using System.ComponentModel;

namespace PKHeX.Core;

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class WordFilterSettings
{
    [LocalizedDescription("Checks player given Nicknames and Trainer Names for profanity. Bad words will be flagged using the 3DS console's regex lists.")]
    public bool CheckWordFilter { get; set; } = true;

    [LocalizedDescription("Disables the Word Filter check for formats prior to 3DS-era.")]
    public bool DisableWordFilterPastGen { get; set; }

    public bool IsEnabled(int gen) => CheckWordFilter && (!DisableWordFilterPastGen || gen >= 6);
}
