namespace PKHeX.Core
{
    /// <summary>
    /// Represents a known value for a <see cref="EventWork{T}"/> of type <see cref="int"/>.
    /// </summary>
    public sealed class EventWorkVal
    {
        public readonly bool Custom;
        public readonly string Text;
        public readonly int Value;

        public EventWorkVal()
        {
            Custom = true;
            Text = nameof(Custom);
            Value = int.MinValue;
        }

        public EventWorkVal(string text, int val)
        {
            Text = text;
            Value = val;
        }
    }
}