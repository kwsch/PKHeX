namespace PKHeX.WinForms.Controls
{
    /// <summary>
    /// Indexes within the <see cref="SAVEditor"/>'s scope.
    /// </summary>
    /// <remarks>The save editor displays party/battlebox/daycare outside of a <see cref="SlotList"/>, and need to be managed.</remarks>
    internal enum SlotIndex
    {
        Party = 0,
        BattleBox = 6,
        Daycare = 12,
    }
}
