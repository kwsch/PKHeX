namespace PKHeX.Core;

/// <summary>
/// Team listings for <see cref="GameVersion.CXD"/> that have a shadow Pokémon afterward.
/// </summary>
public static class Encounters3ColoShadow
{
    #region Colosseum

    public static readonly TeamLock CMakuhita = new(
        296, // Makuhita
        [
            new NPCLock(355, 24, 0, 127), // Duskull (M) (Quirky)
            new NPCLock(167, 00, 1, 127), // Spinarak (F) (Hardy)
        ]);

    public static readonly TeamLock CGligar = new(
        207, // Gligar
        [
            new NPCLock(216, 12, 0, 127), // Teddiursa (M) (Serious)
            new NPCLock(039, 06, 1, 191), // Jigglypuff (F) (Docile)
            new NPCLock(285, 18, 0, 127), // Shroomish (M) (Bashful)
        ]);

    public static readonly TeamLock CMurkrow = new(
        198, // Murkrow
        [
            new NPCLock(318, 06, 0, 127), // Carvanha (M) (Docile)
            new NPCLock(274, 12, 1, 127), // Nuzleaf (F) (Serious)
            new NPCLock(228, 18, 0, 127), // Houndour (M) (Bashful)
        ]);

    public static readonly TeamLock CHeracross = new(
        214, // Heracross
        [
            new NPCLock(284, 00, 0, 127), // Masquerain (M) (Hardy)
            new NPCLock(168, 00, 1, 127), // Ariados (F) (Hardy)
        ]);

    public static readonly TeamLock CUrsaring = new(
        217, // Ursaring
        [
            new NPCLock(067, 20, 1, 063), // Machoke (F) (Calm)
            new NPCLock(259, 16, 0, 031), // Marshtomp (M) (Mild)
            new NPCLock(275, 21, 1, 127), // Shiftry (F) (Gentle)
        ]);

    #endregion

    #region E-Reader

    public static readonly TeamLock ETogepi = new(
        175, // Togepi
        [
            new NPCLock(302, 23, 0, 127), // Sableye (M) (Careful)
            new NPCLock(088, 08, 0, 127), // Grimer (M) (Impish)
            new NPCLock(316, 24, 0, 127), // Gulpin (M) (Quirky)
            new NPCLock(175, 22, 1, 031), // Togepi (F) (Sassy) -- itself!
        ]);

    public static readonly TeamLock EMareep = new(
        179, // Mareep
        [
            new NPCLock(300, 04, 1, 191), // Skitty (F) (Naughty)
            new NPCLock(211, 10, 1, 127), // Qwilfish (F) (Timid)
            new NPCLock(355, 12, 1, 127), // Duskull (F) (Serious)
            new NPCLock(179, 16, 1, 127), // Mareep (F) (Mild) -- itself!
        ]);

    public static readonly TeamLock EScizor = new(
        212, // Scizor
        [
            new NPCLock(198, 13, 1, 191), // Murkrow (F) (Jolly)
            new NPCLock(344, 02, 2, 255), // Claydol (-) (Brave)
            new NPCLock(208, 03, 0, 127), // Steelix (M) (Adamant)
            new NPCLock(212, 11, 0, 127), // Scizor (M) (Hasty) -- itself!
        ]);

    #endregion
}
