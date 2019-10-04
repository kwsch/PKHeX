namespace PKHeX.Core
{
    /// <summary>
    /// Event variable used to determine game events.
    /// </summary>
    public abstract class EventVar
    {
        /// <summary>
        /// Name of event variable
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Type of event variable
        /// </summary>
        public readonly EventVarType Type;

        /// <summary>
        /// Raw index within the event variable (type) region.
        /// </summary>
        public int RawIndex;

        /// <summary>
        /// Unpacked structure's index.
        /// </summary>
        public readonly int RelativeIndex;

        protected EventVar(int index, EventVarType t, string name)
        {
            RelativeIndex = index;
            Type = t;
            Name = name;
        }
    }
}