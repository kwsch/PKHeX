namespace PKHeX.Core
{
    /// <summary>
    /// Gender a <see cref="PKM"/> can have
    /// </summary>
    /// <remarks><see cref="Random"/> provided to function for Encounter template values</remarks>
#pragma warning disable CA1027 // Mark enums with FlagsAttribute
    public enum Gender : byte
    {
        Male = 0,
        Female = 1,

        Genderless = 2,
        Random = Genderless,
    }
}
