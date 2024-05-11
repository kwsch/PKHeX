namespace PKHeX.Core;

/// <summary>
/// Represents a known value for a <see cref="EventWork{T}"/> of type <see cref="int"/>.
/// </summary>
public sealed class EventWorkVal(string Text, int Value)
{
    public readonly string Text = Text;
    public readonly int Value = Value;

    public readonly bool Custom;

    public EventWorkVal() : this(nameof(Custom), int.MinValue)
    {
        Custom = true;
    }
}
