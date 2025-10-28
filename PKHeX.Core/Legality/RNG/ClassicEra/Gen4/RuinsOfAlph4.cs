using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for <see cref="EntityContext.Gen4"/> Ruins of Alph Unown forms.
/// </summary>
public static class RuinsOfAlph4
{
    /// <summary>
    /// Met location for the Ruins of Alph.
    /// </summary>
    public const ushort Location = 209;

    /// <summary>
    /// Checks if the requested <see cref="form"/> is valid for the given seed.
    /// </summary>
    /// <param name="seed">Seed that originated the PID/IV.</param>
    /// <param name="form">Form to validate</param>
    /// <returns>True if the form is valid</returns>
    public static bool IsFormValid(uint seed, byte form)
    {
        // ABCD|E(Item)|F(Form) determination
        var f = LCRNG.Next6(seed);
        return IsFormValidFrame(f, form);
    }

    /// <inheritdoc cref="IsFormValid(uint,byte)"/>
    public static bool IsFormValid(PKM pk, byte form)
    {
        if (!MethodFinder.GetLCRNGMethod1Match(pk, out var seed))
            return true; // invalid anyway, don't care.
        return IsFormValid(seed, form);
    }

    /// <summary>
    /// Checks if the requested <see cref="form"/> is valid for the given frame seed that calculates form.
    /// </summary>
    /// <param name="seed">RNG state that determines the form</param>
    /// <param name="form">Form to validate</param>
    /// <returns>True if the form is valid</returns>
    public static bool IsFormValidFrame(uint seed, byte form)
    {
        if (form >= 26) // Entrance
            return form == GetEntranceForm(seed);

        var u16 = seed >> 16;
        var radio = (u16 % 100) < 50;
        if (radio)
        {
            if (IsFormValidRadio(form))
                return true;
            // Try without radio.
            if (IsFormValidInterior(form, u16))
                return true;
        }
        else
        {
            // Try without radio.
            if (IsFormValidInterior(form, u16))
                return true;
            var next = LCRNG.Next16(ref seed);
            return IsFormValidInterior(form, next);
        }
        return false;
    }

    /// <summary>
    /// Gets one of the two entry area forms (!?)
    /// </summary>
    /// <param name="seed">RNG state that determines which form is picked.</param>
    /// <returns>Form ID</returns>
    public static byte GetEntranceForm(uint seed) => (byte)(26 + ((seed >> 16) & 1));

    // give a random not-yet-seen form
    // this can be anything depending on the player's SaveFile progress
    private static bool IsFormValidRadio(byte _) => true;

    private const int MaxDepth = 4;

    private static bool IsFormValidInterior(byte form, uint rand)
    {
        // Let's have some fun: permute the combinations of unlocked forms & resulting form choice-s.
        Span<byte> forms = stackalloc byte[26]; // A-Z
        return Recurse(forms, form, rand, 0);
    }

    private static bool Recurse(in Span<byte> forms, in byte form, in uint rand, int count, int depth = 0)
    {
        if (depth == MaxDepth)
            return false;

        // Unlock the forms for this depth-set. Retain the count to keep both "ranges" available.
        // Try adding the forms for the entire depth and checking if it's valid.
        // Most players will unlock all forms anyway, so we go for that eager case.
        int newCount = AddForms(forms, count, depth);
        if (Recurse(forms, form, rand, newCount, depth + 1))
            return true;

        // Try with the forms we added in this stack frame.
        // Don't need to check the not-added case as earlier depths will check the same set when needed.
        var roll = rand % newCount;
        var expect = forms[(int)roll];
        if (expect == form)
            return true;

        // Try checking without the added forms. No need to clear what we just added.
        // Check this case before the nothing-else case, as players aren't likely to be missing many sets.
        if (Recurse(forms, form, rand, count, depth + 1))
            return true;

        return false;
    }

    private static int AddForms(Span<byte> forms, int count, int depth)
    {
        var (start, end) = GetForms(depth);
        for (var i = start; i <= end; i++)
            forms[count++] = i;
        return count;
    }

    private static (byte Start, byte End) GetForms(int index) => index switch
    {
        0 => (00, 09), // A-J = 10
        1 => (17, 21), // R-V = 5
        2 => (10, 16), // K-Q = 7
        3 => (22, 25), // W-Z = 4
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };
}
