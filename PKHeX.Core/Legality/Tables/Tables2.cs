namespace PKHeX.Core;

public static partial class Legal
{
    internal const int MaxSpeciesID_2 = 251;
    internal const int MaxMoveID_2 = 251;
    internal const int MaxItemID_2 = 255;
    internal const int MaxAbilityID_2 = 0;

    internal static readonly ushort[] HeldItems_GSC = ItemStorage2.GetAllHeld();

    internal static readonly bool[] ReleasedHeldItems_2 = GetPermitList(MaxItemID_2, HeldItems_GSC);
}
