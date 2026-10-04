using PKHeX.Core;

namespace PKHeX.Web.Tests;

/// <summary>
/// A synthetic X/Y save whose stored names are hostile: markup, and a right-to-left override that would reverse the text after it. Used to check
/// that names render as inert text and cannot reorder the page.
/// </summary>
internal static class HostileFixtures
{
    /// <summary>Right-to-left override (U+202E).</summary>
    public const string Override = "\u202E";

    /// <summary>The save's trainer name, also given to the boxed Pokémon as its original trainer: markup within the 12 characters Generation 6 stores.</summary>
    public const string TrainerName = "<i>T</i>";

    /// <summary>The first box's name: markup.</summary>
    public const string BoxName = "<b>B</b>";

    /// <summary>The first boxed Pokémon's nickname: an override, then markup.</summary>
    public const string Nickname = Override + "<b>x</b>";

    /// <summary>What a nickname shows once its bidirectional controls are removed.</summary>
    public const string ShownNickname = "<b>x</b>";

    /// <summary>A file name with markup, an override and characters Windows reserves.</summary>
    public const string FileName = "<img src=x onerror=alert(1)>" + Override + "main";

    /// <summary>The same name as the app shows it: reserved characters replaced, the override removed (written out, not computed).</summary>
    public const string ShownFileName = "_img src=x onerror=alert(1)_main";

    /// <summary>The save, built with Core. The Pokémon's names are set after the synthetic save stores it in box 1, slot 1.</summary>
    public static byte[] Save()
    {
        var bytes = SaveFixtures.Synthetic(false, customize: save =>
        {
            save.OT = TrainerName;
            ((IBoxDetailName)save).SetBoxName(0, BoxName);
        });
        var save = SaveUtil.GetSaveFile(bytes)!;
        var pk = save.GetBoxSlotAtIndex(0, 0);
        pk.IsNicknamed = true;
        pk.Nickname = Nickname;
        pk.OriginalTrainerName = TrainerName;
        save.SetBoxSlotAtIndex(pk, 0, 0, EntityImportSettings.None);
        return save.Write().ToArray();
    }
}
