namespace PKHeX.Core;

public enum WordFilterType
{
    None,
    Gen5,
    Citra,
    NX,
}

public static class WordFilterTypeExtensions
{
    public static WordFilterType GetName(EntityContext type) => type.GetConsole() switch
    {
        GameConsole.NX => WordFilterType.NX,
        _ => type.Generation() switch
        {
            5 => WordFilterType.Gen5,
            6 or 7 => WordFilterType.Citra,
            _ => WordFilterType.None,
        },
    };

    private const int BitsUsedForType = 3; // 3 bits for type, 13 bits for index

    public static ushort GetPackedValue(WordFilterType type, int index)
    {
        if (type == WordFilterType.None || index == -1)
            return 0;
        return (ushort)(((int)type << (16 - BitsUsedForType)) | index);
    }

    public static (WordFilterType Type, int Index) GetUnpackedValue(ushort value)
    {
        if (value == 0)
            return (WordFilterType.None, -1);
        var type = (WordFilterType)(value >> (16 - BitsUsedForType));
        var index = value & 0x7FFF;
        return (type, index);
    }
}
