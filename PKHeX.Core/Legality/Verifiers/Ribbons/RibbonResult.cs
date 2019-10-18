namespace PKHeX.Core
{
    /// <summary>
    /// Legality Check Parse object containing information about a single ribbon.
    /// </summary>
    internal class RibbonResult
    {
        /// <summary>Ribbon Display Name</summary>
        public string Name { get; private set; }

        /// <summary>Ribbon should not be present.</summary>
        /// <remarks>If this is false, the Ribbon is missing.</remarks>
        public bool Invalid { get; }

        public RibbonResult(string prop, bool invalid = true)
        {
            Name = RibbonStrings.GetName(prop);
            Invalid = invalid;
        }

        /// <summary>
        /// Merges the result name with another provided result.
        /// </summary>
        public void Combine(RibbonResult other)
        {
            Name += $" / {other.Name}";
        }
    }
}
