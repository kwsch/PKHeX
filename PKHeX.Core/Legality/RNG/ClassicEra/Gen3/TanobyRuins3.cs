using System;

namespace PKHeX.Core;

public static class TanobyRuins3
{
    /// <summary>
    /// Checks if the form is available in the location.
    /// </summary>
    public static bool IsFormInLocation(byte location, byte form) => IsLocationInRuins(location) && GetForms(location).Contains(form);

    /// <summary>
    /// If you're calling this method and know which template you are matched to, just check the form from the template directly.
    /// </summary>
    public static bool IsFormValid(byte location, byte form, byte slot)
    {
        if (!IsLocationInRuins(location))
            return false;

        var forms = GetForms(location);
        if (form > forms.Length)
            return false;
        return forms[slot] == form;
    }

    /// <summary>
    /// Gets the form for the location and slot.
    /// </summary>
    public static byte GetForm(byte location, byte slot)
    {
        if (!IsLocationInRuins(location))
            return 0;
        var forms = GetForms(location);
        if (slot > forms.Length)
            return 0;
        return forms[slot];
    }

    private static bool IsLocationInRuins(byte location) => location is >= 188 and <= 194;

    public static ReadOnlySpan<byte> GetForms(byte location) => location switch
    {
        188 => [00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 27], // 188 = Monean Chamber
        189 => [02, 02, 02, 03, 03, 03, 07, 07, 07, 20, 20, 14], // 189 = Liptoo Chamber
        190 => [13, 13, 13, 13, 18, 18, 18, 18, 08, 08, 04, 04], // 190 = Weepth Chamber
        191 => [15, 15, 11, 11, 09, 09, 17, 17, 17, 16, 16, 16], // 191 = Dilford Chamber
        192 => [24, 24, 19, 19, 06, 06, 06, 05, 05, 05, 10, 10], // 192 = Scufib Chamber
        193 => [21, 21, 21, 22, 22, 22, 23, 23, 12, 12, 01, 01], // 193 = Rixy Chamber
        194 => [25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 26], // 194 = Viapois Chamber
        _ => throw new ArgumentOutOfRangeException(nameof(location)),
    };
}
