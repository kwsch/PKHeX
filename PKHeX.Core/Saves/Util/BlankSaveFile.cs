using System;
using static PKHeX.Core.LanguageID;
using static PKHeX.Core.SaveFileType;

namespace PKHeX.Core;

/// <summary>
/// Logic for blank save files or default values if not exposed via data.
/// </summary>
public static class BlankSaveFile
{
    private const string DefaultTrainer = TrainerName.ProgramINT;
    private const LanguageID DefaultLanguage = English;

    /// <summary>
    /// Returns a <see cref="LanguageID"/> that feels best for the save file's language.
    /// </summary>
    public static LanguageID GetSafeLanguage(SaveFile? sav) => sav switch
    {
        null => English,
        ILangDeviantSave s => s.Japanese ? Japanese : s.Korean ? Korean : English,
        _ => (uint)sav.Language <= Legal.GetMaxLanguageID(sav.Generation) ? (LanguageID)sav.Language : English,
    };

    /// <summary>
    /// Returns a Trainer Name that feels best for the save file's language.
    /// </summary>
    public static string GetSafeTrainerName(SaveFile? sav, LanguageID lang) => lang switch
    {
        Japanese => sav?.Generation >= 3 ? TrainerName.ProgramJPN : TrainerName.GameFreakJPN,
        _ => TrainerName.ProgramINT,
    };

    /// <inheritdoc cref="Get(SaveFileType,LanguageID,GameVersion)"/>
    public static SaveFile Get(GameVersion game, string trainerName = DefaultTrainer, LanguageID language = DefaultLanguage)
    {
        var type = game.GetSaveFileType();
        return Get(type, game, trainerName, language);
    }

    /// <summary>
    /// Creates an instance of a SaveFile with a blank base.
    /// </summary>
    /// <param name="context">Context of the Save File.</param>
    /// <param name="trainerName">Trainer Name</param>
    /// <param name="language">Save file language to initialize for</param>
    /// <returns>Save File for that generation.</returns>
    public static SaveFile Get(EntityContext context, string trainerName = DefaultTrainer, LanguageID language = DefaultLanguage)
    {
        var version = context.GetSingleGameVersion();
        var type = version.GetSaveFileType();
        return Get(type, version, trainerName, language);
    }

    /// <summary>
    /// Creates an instance of a SaveFile with a blank base.
    /// </summary>
    /// <param name="type">Requested save file type.</param>
    /// <param name="game">Version to create the save file for.</param>
    /// <param name="trainerName">Trainer Name</param>
    /// <param name="language">Language to initialize with</param>
    /// <returns>Blank save file from the requested game, null if no game exists for that <see cref="GameVersion"/>.</returns>
    public static SaveFile Get(SaveFileType type, GameVersion game, string trainerName = DefaultTrainer, LanguageID language = DefaultLanguage)
    {
        var sav = Get(type, language, game);
        sav.Version = game;
        sav.OT = trainerName;
        if (sav.Generation >= 4)
            sav.Language = (int)language;

        // Secondary Properties may not be used but can be filled in as template.
        (uint tid, uint sid) = sav.Generation >= 7 ? (123456u, 1234u) : (12345u, 54321u);
        sav.SetDisplayID(tid, sid);
        sav.Language = (int)language;

        // Only set geolocation data for 3DS titles
        if (sav is IRegionOrigin o)
            o.SetDefaultRegionOrigins((int)language);

        return sav;
    }

    /// <summary>
    /// Creates an instance of a SaveFile with a blank base.
    /// </summary>
    /// <param name="type">Requested save file type.</param>
    /// <param name="language">Save file language to initialize for</param>
    /// <param name="game">Version to create the save file for, if a specific version is requested within the <see cref="type"/>.</param>
    /// <returns>Blank save file from the requested game, null if no game exists for that <see cref="GameVersion"/>.</returns>
    private static SaveFile Get(SaveFileType type, LanguageID language, GameVersion game = default) => type switch
    {
        RBY => new SAV1(game == GameVersion.BU ? Japanese : language, version: game),
        Stadium1J => new SAV1StadiumJ(),
        Stadium1 => new SAV1Stadium(language == Japanese),

        GSC => new SAV2(language, language == Korean ? GameVersion.GS : game),
        Stadium2 => new SAV2Stadium(language == Japanese),

        RS => new SAV3RS(language == Japanese),
        Emerald => new SAV3E(language == Japanese),
        FRLG => new SAV3FRLG(language == Japanese),

        Colosseum => new SAV3Colosseum(),
        XD => new SAV3XD(),
        RSBox => new SAV3RSBox(),

        DP => new SAV4DP(),
        Pt => new SAV4Pt(),
        HGSS => new SAV4HGSS(),
        BattleRevolution => new SAV4BR(),

        BW => new SAV5BW(),
        B2W2 => new SAV5B2W2(),

        XY => new SAV6XY(),
        AODemo => new SAV6AODemo(),
        AO => new SAV6AO(),

        SM => new SAV7SM(),
        USUM => new SAV7USUM(),
        LGPE => new SAV7b(),

        SWSH => new SAV8SWSH(),
        BDSP => new SAV8BS(),
        LA => new SAV8LA(),

        SV => new SAV9SV(),

        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
