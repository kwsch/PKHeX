using System;

namespace PKHeX.Core;

public static partial class Legal
{
    internal const int MaxSpeciesID_8a = (int)Species.Enamorus;
    internal const int MaxMoveID_8a = (int)Move.TakeHeart;
    internal const int MaxItemID_8a = 1828; // Legend Plate
    internal const int MaxBallID_8a = (int)Ball.LAOrigin;
    internal const int MaxGameID_8a = (int)GameVersion.SP;
    internal const int MaxAbilityID_8a = MaxAbilityID_8_R2;

    internal static readonly ushort[] HeldItems_LA = Array.Empty<ushort>();
}
