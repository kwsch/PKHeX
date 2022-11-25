namespace PKHeX.Core;

public enum HeldItemLumpImage
{
    NotLump,
    TechnicalMachine,
    TechnicalRecord,
}

public static class HeldItemLumpUtil
{
    public static bool IsLump(this HeldItemLumpImage image) => image != HeldItemLumpImage.NotLump;

    public static HeldItemLumpImage GetIsLump(int item, int generation) => generation switch
    {
        <= 4 when item is (>= 0328 and <= 0419) => HeldItemLumpImage.TechnicalMachine, // gen2/3/4 TM
        8 when item is (>= 0328 and <= 0427) => HeldItemLumpImage.TechnicalMachine, // BDSP TMs
        8 when item is (>= 1130 and <= 1229) => HeldItemLumpImage.TechnicalRecord, // Gen8 TR
        9 when item is (>= 0328 and <= 0419) // TM01-TM92
            or (>= 0618 and <= 0620) // TM93-TM95
            or (>= 0690 and <= 0693) // TM96-TM99
            or (>= 2160 and <= 2231) /* TM100-TM171 */ => HeldItemLumpImage.TechnicalMachine,
        _ => HeldItemLumpImage.NotLump,
    };
}
