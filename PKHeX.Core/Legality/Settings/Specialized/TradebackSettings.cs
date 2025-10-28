using System.ComponentModel;

namespace PKHeX.Core;

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class TradebackSettings
{
    [LocalizedDescription("GB: Allow Generation 2 tradeback learnsets for PK1 formats. Disable when checking RBY Metagame rules.")]
    public bool AllowGen1Tradeback { get; set; } = true;
}
