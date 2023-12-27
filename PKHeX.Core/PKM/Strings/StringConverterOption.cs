namespace PKHeX.Core;

/// <summary>
/// String Buffer pre-formatting option
/// </summary>
public enum StringConverterOption
{
    /// <summary> Does not do any operation on the buffer. </summary>
    None,

    /// <summary> Zeroes out the entire buffer. </summary>
    ClearZero,

    /// <summary> Fills the entire buffer with 0x50; used by Generation 1/2 string encoding. </summary>
    Clear50,

    /// <summary> Fills the entire buffer with 0x7F; used by Generation 1/2 Stadium to space over for the next line. </summary>
    Clear7F,

    /// <summary> Fills the entire buffer with 0xFF; used by Generation 3-5 which use 0xFF/0xFFFF as their terminator. </summary>
    ClearFF,
}
