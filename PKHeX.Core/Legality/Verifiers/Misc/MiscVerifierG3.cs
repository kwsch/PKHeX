using System;
using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

public sealed class MiscVerifierG3 : Verifier
{
    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data)
    {
        if (data.Entity is G3PKM pk)
            Verify(data, pk);
    }

    internal void Verify(LegalityAnalysis data, G3PKM pk)
    {
        VerifyTrash(data, pk);

        if (ParseSettings.AllowGBACrossTransferRSE(pk))
            return;

        // Notes:
        // Nicknamed by player: all FF'd trash.
        // In-game trade: clean 00'd with a single FF on each.

        // Only FR/LG are released. Only can originate from FR/LG.
        if (pk.Version is not (GameVersion.FR or GameVersion.LG))
            data.AddLine(GetInvalid(TradeNotAvailable));
        else if (Legal.IsForeignFRLG(pk.Species))
            data.AddLine(GetInvalid(TradeNotAvailable));

        if (ItemStorage3FRLG_VC.IsUnreleasedHeld(pk.HeldItem))
            data.AddLine(GetInvalid(ItemUnreleased));

        if ((Ball)pk.Ball is Ball.Dive or Ball.Premier)
            data.AddLine(GetInvalid(BallUnavailable));
    }

    private void VerifyTrash(LegalityAnalysis data, G3PKM pk)
    {
        if (pk is PK3 pk3)
            VerifyTrash(data, pk3);
        else
            VerifyTrashCXD(data, pk);
    }

    private static void VerifyTrashCXD(LegalityAnalysis data, G3PKM pk)
    {
        // Buffers should be entirely clean.
        var ot = pk.OriginalTrainerTrash;
        var result = TrashBytesUTF16.IsTrashNone(ot);
        if (result.IsInvalid)
            data.AddLine(Get(Trainer, Severity.Invalid, TrashBytesShouldBeEmpty));

        var nick = pk.NicknameTrash;
        result = TrashBytesUTF16.IsTrashNone(nick);
        if (result.IsInvalid)
            data.AddLine(Get(Nickname, Severity.Invalid, TrashBytesShouldBeEmpty));
    }

    private void VerifyTrash(LegalityAnalysis data, PK3 pk)
    {
        if (!pk.IsEgg && TrashByteRules3.IsResetTrash(pk))
        {
            data.AddLine(GetValid(TrashBytesResetViaTransfer));
            return; // OK
        }

        var enc = data.EncounterOriginal;
        if (enc is EncounterTrade3)
            VerifyTrashTrade(data, pk);
        else if (enc is EncounterGift3 g3)
            VerifyTrashEvent3(data, pk, g3);
        else if (enc is EncounterGift3JPN jp)
            VerifyTrashEvent3(data, pk, jp);
        else if (enc is EncounterGift3NY ny)
            VerifyTrashEvent3(data, pk, ny);
        else if (pk.Japanese && !(pk.IsEgg && pk.OriginalTrainerTrash[^1] == 0xFF))
            VerifyTrashJPN(data, pk);
        else
            VerifyTrashINT(data, pk);
    }

    private static void VerifyTrashEvent3(LegalityAnalysis data, PK3 pk, EncounterGift3NY ny)
    {
        // todo
    }

    private static void VerifyTrashEvent3(LegalityAnalysis data, PK3 pk, EncounterGift3JPN jp)
    {
        // todo
    }

    private static void VerifyTrashEvent3(LegalityAnalysis data, PK3 pk, EncounterGift3 g3)
    {
        // todo
    }

    private static void VerifyTrashTrade(LegalityAnalysis data, PK3 pk)
    {
        if (!TrashByteRules3.IsTerminatedZero(pk.OriginalTrainerTrash))
            data.AddLine(GetInvalid(Trainer, TrashBytesShouldBeEmpty));
        if (!TrashByteRules3.IsTerminatedZero(pk.NicknameTrash))
            data.AddLine(GetInvalid(Nickname, TrashBytesShouldBeEmpty));
    }

    private static void VerifyTrashJPN(LegalityAnalysis data, PK3 pk)
    {
        var trash = pk.OriginalTrainerTrash;
        // OT name from save file is copied byte-for-byte. Byte 7 & 8 are always zero.
        if (!TrashByteRules3.IsTerminatedFFZero(trash, 6))
            data.AddLine(GetInvalid(Trainer, TrashBytesMissingTerminatorFinal));
        // Nickname can be all FF's (nicknamed) or whatever random garbage is in the buffer before filling. Unsure if we can reliably check this, but it should be "dirty" usually.
        // If it is clean, flag as fishy.
        FlagIsNicknameClean(data, pk);
    }

    private static void VerifyTrashINT(LegalityAnalysis data, PK3 pk)
    {
        var trash = pk.OriginalTrainerTrash;
        // OT name from save file is copied byte-for-byte. All 8 bytes are initialized to FF on new game.
        if (!TrashByteRules3.IsTerminatedFFZero(trash, 7))
        {
            var isDefaultVersion = IsDefaultTrainer(data, pk, trash);
            if (isDefaultVersion == NoDefaultTrainerNameMatch)
                data.AddLine(GetInvalid(Trainer, TrashBytesMissingTerminatorFinal));
        }
        // Nickname can be all FF's (nicknamed) or whatever random garbage is in the buffer before filling. Unsure if we can reliably check this, but it should be "dirty" usually.
        // If it is clean, flag as fishy.
        FlagIsNicknameClean(data, pk);
    }

    private const GameVersion NoDefaultTrainerNameMatch = 0;

    private static GameVersion IsDefaultTrainer(LegalityAnalysis data, PK3 pk, Span<byte> trash)
    {
        var language = (LanguageID)pk.Language;
        // Only non-Japanese are affected by trash patterns, so if Japanese, no match.
        if (language == LanguageID.Japanese)
            return NoDefaultTrainerNameMatch;

        var version = pk.Version;
        // FR/LG and others are not impacted.
        if (version is not (GameVersion.R or GameVersion.S or GameVersion.E))
            return NoDefaultTrainerNameMatch;

        // Check if it matches any of the default trainer name patterns for the original version and language.
        // If it does, then it's valid as a default trainer name with expected trash.
        var gender = pk.OriginalTrainerGender;
        var location = pk.MetLocation;
        var eggEncounter = data.EncounterOriginal.IsEgg;
        var isEgg = pk.IsEgg;
        if (TrashByteRules3.IsTrashPatternDefaultTrainer(trash, version, language, gender))
        {
            if (!eggEncounter || isEgg || EggHatchLocation3.IsValidMet3(location, version))
                return version;
        }

        // If it was a traded egg, possibly?
        // Unhatched eggs must match the original version, already checked above.
        // Non-eggs cannot change OT name. Must be hatched to change the OT name.
        if (!eggEncounter || isEgg)
            return NoDefaultTrainerNameMatch;

        // Check the other versions.
        for (var hatchVersion = GameVersion.S; hatchVersion <= GameVersion.E; hatchVersion++)
        {
            if (hatchVersion == version)
                continue; // already checked.
            if (!TrashByteRules3.IsTrashPatternDefaultTrainer(trash, hatchVersion, language, gender))
                continue; // doesn't match this version's pattern, skip.

            // Ensure it can be hatched at a valid location for this version. Probably don't need to do this...
            if (EggHatchLocation3.IsValidMet3(location, hatchVersion))
                return hatchVersion;
        }

        // None match.
        return NoDefaultTrainerNameMatch;
    }

    private static void FlagIsNicknameClean(LegalityAnalysis data, PK3 pk)
    {
        if (pk.IsEgg)
            return;

        if (IsNicknamedByPlayer(data, pk))
        {
            // Japanese only fills the first 5+1 bytes; everything else is trash.
            // International games are 10 chars (full buffer) max; implicit terminator if full.
            var nick = pk.GetNicknamePrefillRegion();
            if (TrashByteRules3.IsTerminatedFF(nick))
                return; // Matches the manually nicknamed pattern.

            if (!TrashByteRules3.IsTerminatedFFZero(nick)) // Wasn't cleared by transferring between C/XD.
                data.AddLine(GetInvalid(Trainer, TrashBytesMismatchInitial));
        }
    }

    private static bool IsNicknamedByPlayer(LegalityAnalysis data, PK3 pk)
    {
        Span<char> name = stackalloc char[10];
        var len = pk.LoadString(pk.NicknameTrash, name);
        name = name[..len];

        if (!SpeciesName.IsNicknamed(pk.Species, name, pk.Language, 3))
            return false;

        // Okay, we aren't matching the expected name.
        // Only way to do that is by manually nicknaming OR evolving in another language game.
        // Check for the language evolution.

        // Each game only contains the strings for its own localization, so it won't know of others.
        // The game evaluates "is nicknamed" based on the text not matching its species name.
        // Trade to another language then evolve: will treat it like a nickname, without actually filling with FF.
        if (IsNicknameLanguageEvolution(data, name))
            return false;

        // No other explanation; must be manually nicknamed.
        return true;
    }

    private static bool IsNicknameLanguageEvolution(LegalityAnalysis data, ReadOnlySpan<char> name)
    {
        var chain = data.Info.EvoChainsAllGens.Gen3;
        if (chain.Length == 1)
        {
            // Hasn't evolved.
            data.AddLine(GetInvalid(Trainer, TrashBytesMismatchInitial));
            return false;
        }

        // Check if the nickname matches any pre-evolution on any language.
        // Skip head (current species), check pre-evolutions.
        for (int i = 1; i < chain.Length; i++)
        {
            var isDefaultName = SpeciesName.IsNicknamedAnyLanguage(chain[i].Species, name, EntityContext.Gen3);
            if (isDefaultName)
                return true;
        }
        return false;
    }
}

public static class TrashByteRules3
{
    // PK3 stores u8[length] for OT name and Nickname.
    // Due to how the game initializes the buffer for each, specific patterns in the unused bytes (after the string, within the allocated max buffer) can arise.
    // When transferred to Colosseum/XD, the encoding method switches to u16[length], thus discarding the original buffer along with its "trash".
    // For original encounters from a mainline save file,
    // - OT Name: the game copies the entire buffer from the save file OT as the PK3's OT. Thus, that must match exactly.
    // - - Japanese OT names are 5 chars, international is 7 chars. Manually entered strings are FF terminated to max length + 1.
    // - - Default OT (Japanese) names were padded with FF to len=6, so they always match manually entered names (no trash).
    // - - Default OT (International) names from the character select screen can have trash bytes due to being un-padded (single FF end of string, saves ROM space).
    // - - verification of Default OTs
    // - Nickname: the buffer has garbage RAM data leftover in the nickname field, thus it should be "dirty" usually.
    // - Nicknamed: when nicknamed, the game fills the buffer with FFs then applies the nickname.
    // For event encounters from GameCube:
    // - OT Name: todo
    // - Nickname: todo
    // For event encounters directly injected into the save file via GBA multiboot:
    // - OT Name: todo
    // - Nickname: todo

    private const byte Terminator = StringConverter3.TerminatorByte;

    public static bool IsResetTrash(PK3 pk3)
    {
        if (!ParseSettings.AllowGBACrossTransferXD(pk3))
            return false;

        if (!IsTerminatedZero(pk3.OriginalTrainerTrash))
            return false;
        if (pk3.IsNicknamed)
            return true;
        if (!IsTerminatedZero(pk3.NicknameTrash))
            return false;
        return true;
    }

    public static bool IsTerminatedZero(ReadOnlySpan<byte> data)
    {
        var first = TrashBytes8.GetTerminatorIndex(data);
        if (first == -1 || first >= data.Length - 1)
            return true;
        return !data[(first+1)..].ContainsAnyExcept<byte>(0);
    }

    public static bool IsTerminatedFF(ReadOnlySpan<byte> data)
    {
        var first = TrashBytes8.GetTerminatorIndex(data);
        if (first == -1 || first >= data.Length - 1)
            return true;
        return !data[(first + 1)..].ContainsAnyExcept(Terminator);
    }

    /// <summary>
    /// Checks if the <see cref="data"/> matches the pattern of a pre-filled array with terminators of count <see cref="preFill"/>.
    /// </summary>
    /// <param name="data">Raw text string to check</param>
    /// <param name="preFill">Count of chars filled with terminator.</param>
    /// <returns><see langword="true"/> if the text matches the pre-fill pattern.</returns>
    public static bool IsTerminatedFFZero(ReadOnlySpan<byte> data, int preFill = 0)
    {
        if (preFill == 0)
            return IsTerminatedZero(data);

        var first = TrashBytes8.GetTerminatorIndex(data);
        if (first == -1 || first >= data.Length - 1)
            return true;

        first++;
        if (first < preFill)
        {
            var inner = data[first..preFill];
            if (inner.ContainsAnyExcept(Terminator))
                return false;
            first = preFill;
            if (first >= data.Length)
                return true;
        }
        return !data[first..].ContainsAnyExcept<byte>(0);
    }

    // TRASH BYTES: New Game Default OTs
    // Default OT names in International (not JPN) Gen3 mainline R/S/E games memcpy exactly 7 chars then FF from the "default OT name" table, regardless of the entry's strlen.
    // - Japanese has every default OT name padded with FF's (to strlen=6), and is thus not affected.
    // - FireRed/LeafGreen uses different logic (Oak Speech) which writes until EOS (0xFF) then filling the rest with 0xFF (thus is entirely clean).
    // Copied strings therefore contain "trash" from the next string entry encoded into the ROM's string table.
    // Below is a list of possible (version, language, trash) default OTs, as initialized by the game. An `*` is used to denote the terminator, with the following chars from next entry.

    // Sequential entries are provided for documentation purposes; entries that result in no difference from a manually entered name are commented out.
    // If it is a default name, it must match the associated gender; otherwise, it must have been manually entered for the other gender.

    // Potential optimization: Entries could be encoded into a single ulong via:
    // 7 bytes trash,
    // 1 byte: version (2bit) - gender (1bit), language (3bit).
    // Then, a simple sorted-array lookup could quickly check presence via binary search (147 entries, 5 pivot checks).
    // However, this is such a low-traffic method that such an optimization (sacrificing code documentation) isn't worth it.

    /// <summary>
    /// Checks if the specified trash byte pattern matches a default trainer name pattern for the given game <see cref="version"/> and <see cref="language"/>.
    /// </summary>
    /// <remarks>Default trainer names in certain Generation 3 Pokémon games may include trailing bytes ("trash") due to how names are stored in the game's ROM.
    /// This method checks if the provided pattern matches any of these known default patterns for the specified version and language.
    /// </remarks>
    public static bool IsTrashPatternDefaultTrainer(ReadOnlySpan<byte> trash, GameVersion version, LanguageID language, byte gender) => version switch
    {
        GameVersion.R => IsTrashPatternDefaultTrainerR(trash, language, gender),
        GameVersion.S => IsTrashPatternDefaultTrainerS(trash, language, gender),
        GameVersion.E => IsTrashPatternDefaultTrainerE(trash, language, gender),
        _ => false,
    };

    /// <inheritdoc cref="IsTrashPatternDefaultTrainer"/>
    /// <remarks>Default OT names present in <see cref="GameVersion.R"/> based on the language of the game.</remarks>
    public static bool IsTrashPatternDefaultTrainerR(ReadOnlySpan<byte> trash, LanguageID language, byte gender) => language switch
    {
        LanguageID.English => trash switch
        {
        //  [0xC6, 0xBB, 0xC8, 0xBE, 0xC9, 0xC8, 0xFF] => gender == 0, // LANDON*
            [0xCE, 0xBF, 0xCC, 0xCC, 0xD3, 0xFF, 0xCD] => gender == 0, // TERRY*S
            [0xCD, 0xBF, 0xCE, 0xC2, 0xFF, 0xCE, 0xC9] => gender == 0, // SETH*TO
            [0xCE, 0xC9, 0xC7, 0xFF, 0xCE, 0xBF, 0xCC] => gender == 0, // TOM*TER
            [0xCE, 0xBF, 0xCC, 0xCC, 0xBB, 0xFF, 0xC5] => gender == 1, // TERRA*K
            [0xC5, 0xC3, 0xC7, 0xC7, 0xD3, 0xFF, 0xC8] => gender == 1, // KIMMY*N
        //  [0xC8, 0xC3, 0xBD, 0xC9, 0xC6, 0xBB, 0xFF] => gender == 1, // NICOLA*
            [0xCD, 0xBB, 0xCC, 0xBB, 0xFF, 0xCE, 0xDC] => gender == 1, // SARA*Th
            _ => false,
        },  
        LanguageID.French => trash switch
        {   
        //  [0xCE, 0xC2, 0xC3, 0xBF, 0xCC, 0xCC, 0xD3] => gender == 0, // THIERRY
        //  [0xCE, 0xC2, 0xC9, 0xC7, 0xBB, 0xCD, 0xFF] => gender == 0, // THOMAS*
        //  [0xBE, 0xBB, 0xC8, 0xC3, 0xBF, 0xC6, 0xFF] => gender == 0, // DANIEL*
            [0xCD, 0xBF, 0xBC, 0xFF, 0xCD, 0xC9, 0xC6] => gender == 0, // SEB*SOL
        //  [0xCD, 0xC9, 0xC6, 0xBF, 0xC8, 0xBF, 0xFF] => gender == 1, // SOLENE*
            [0xBB, 0xC1, 0xC8, 0xBF, 0xCD, 0xFF, 0xBD] => gender == 1, // AGNES*C
        //  [0xBD, 0xC6, 0xBB, 0xC3, 0xCC, 0xBF, 0xFF] => gender == 1, // CLAIRE*
        //  [0xCD, 0xC9, 0xCA, 0xC2, 0xC3, 0xBF, 0xFF] => gender == 1, // SOPHIE*
            _ => false,
        },  
        LanguageID.Italian => trash switch
        {   
        //  [0xC6, 0xBB, 0xC8, 0xBE, 0xC9, 0xC8, 0xFF] => gender == 0, // LANDON*
            [0xC7, 0xBB, 0xCC, 0xBD, 0xC9, 0xFF, 0xCA] => gender == 0, // MARCO*P
            [0xCA, 0xBB, 0xC9, 0xC6, 0xC9, 0xFF, 0xC6] => gender == 0, // PAOLO*L
            [0xC6, 0xCF, 0xBD, 0xC3, 0xC9, 0xFF, 0xCE] => gender == 0, // LUCIO*T
        //  [0xCE, 0xBF, 0xCC, 0xBF, 0xCD, 0xBB, 0xFF] => gender == 1, // TERESA*
            [0xBB, 0xC8, 0xC8, 0xC3, 0xBF, 0xFF, 0xBF] => gender == 1, // ANNIE*E
            [0xBF, 0xC6, 0xC3, 0xCD, 0xBB, 0xFF, 0xCD] => gender == 1, // ELISA*S
            [0xCD, 0xBB, 0xCC, 0xBB, 0xFF, 0xCB, 0xE9] => gender == 1, // SARA*Qu
            _ => false,
        },  
        LanguageID.German => trash switch
        {   
        //  [0xCC, 0xC9, 0xC6, 0xBB, 0xC8, 0xBE, 0xFF] => gender == 0, // ROLAND*
        //  [0xBE, 0xBB, 0xC8, 0xC3, 0xBF, 0xC6, 0xFF] => gender == 0, // DANIEL*
            [0xC2, 0xBF, 0xC6, 0xC1, 0xBF, 0xFF, 0xC4] => gender == 0, // HELGE*J
            [0xC4, 0xBB, 0xC8, 0xFF, 0xCA, 0xBF, 0xCE] => gender == 0, // JAN*PET
            [0xCA, 0xBF, 0xCE, 0xCC, 0xBB, 0xFF, 0xCE] => gender == 1, // PETRA*T
            [0xCE, 0xBB, 0xC8, 0xC4, 0xBB, 0xFF, 0xBB] => gender == 1, // TANJA*A
        //  [0xBB, 0xC8, 0xBE, 0xCC, 0xBF, 0xBB, 0xFF] => gender == 1, // ANDREA*
            [0xCD, 0xBB, 0xCC, 0xBB, 0xFF, 0xBE, 0xDD] => gender == 1, // SARA*Di
            _ => false,
        },  
        LanguageID.Spanish => trash switch
        {   
            [0xCE, 0xBF, 0xCC, 0xBF, 0xC8, 0xFF, 0xCB] => gender == 0, // TEREN*Q
            [0xCB, 0xCF, 0xC3, 0xC7, 0xC3, 0xFF, 0xCC] => gender == 0, // QUIMI*R
            [0xCC, 0xCF, 0xC0, 0xC9, 0xFF, 0xBB, 0xCC] => gender == 0, // RUFO*AR
        //  [0xBB, 0xCC, 0xCE, 0xCF, 0xCC, 0xC9, 0xFF] => gender == 0, // ARTURO*
        //  [0xCE, 0xBF, 0xCC, 0xBF, 0xCD, 0xBB, 0xFF] => gender == 1, // TERESA*
        //  [0xCC, 0xBB, 0xCB, 0xCF, 0xBF, 0xC6, 0xFF] => gender == 1, // RAQUEL*
        //  [0xC7, 0xBB, 0xCC, 0xC3, 0xBB, 0xCF, 0xFF] => gender == 1, // MARIAU*
            [0xCD, 0xBB, 0xCC, 0xBB, 0xFF, 0xBB, 0xE5] => gender == 1, // SARA*Aq
            _ => false,
        },
        _ => false,
    };

    /// <inheritdoc cref="IsTrashPatternDefaultTrainer"/>
    /// <remarks>Default OT names present in <see cref="GameVersion.S"/> based on the language of the game.</remarks>
    public static bool IsTrashPatternDefaultTrainerS(ReadOnlySpan<byte> trash, LanguageID language, byte gender) => language switch
    {
        LanguageID.English => trash switch
        {
            [0xCD, 0xBF, 0xBB, 0xC8, 0xFF, 0xCE, 0xBF] => gender == 0, // SEAN*TE
            [0xCE, 0xBF, 0xCC, 0xCC, 0xD3, 0xFF, 0xCD] => gender == 0, // TERRY*S
            [0xCD, 0xBF, 0xCE, 0xC2, 0xFF, 0xCE, 0xC9] => gender == 0, // SETH*TO
            [0xCE, 0xC9, 0xC7, 0xFF, 0xC7, 0xBB, 0xCC] => gender == 0, // TOM*MAR
        //  [0xC7, 0xBB, 0xCC, 0xC3, 0xC8, 0xBB, 0xFF] => gender == 1, // MARINA*
            [0xC5, 0xC3, 0xC7, 0xC7, 0xD3, 0xFF, 0xC8] => gender == 1, // KIMMY*N
        //  [0xC8, 0xC3, 0xBD, 0xC9, 0xC6, 0xBB, 0xFF] => gender == 1, // NICOLA*
            [0xCD, 0xBB, 0xCC, 0xBB, 0xFF, 0xCE, 0xDC] => gender == 1, // SARA*Th
            _ => false,
        },  
        LanguageID.French => trash switch
        {   
        //  [0xC7, 0xBB, 0xCC, 0xCE, 0xC3, 0xBB, 0xC6] => gender == 0, // MARTIAL
        //  [0xCE, 0xC2, 0xC9, 0xC7, 0xBB, 0xCD, 0xFF] => gender == 0, // THOMAS*
        //  [0xBE, 0xBB, 0xC8, 0xC3, 0xBF, 0xC6, 0xFF] => gender == 0, // DANIEL*
            [0xCD, 0xBF, 0xBC, 0xFF, 0xC7, 0xBB, 0xCC] => gender == 0, // SEB*MAR
        //  [0xC7, 0xBB, 0xCC, 0xC3, 0xC8, 0xBF, 0xFF] => gender == 1, // MARINE*
            [0xBB, 0xC1, 0xC8, 0xBF, 0xCD, 0xFF, 0xBD] => gender == 1, // AGNES*C
        //  [0xBD, 0xC6, 0xBB, 0xC3, 0xCC, 0xBF, 0xFF] => gender == 1, // CLAIRE*
        //  [0xCD, 0xC9, 0xCA, 0xC2, 0xC3, 0xBF, 0xFF] => gender == 1, // SOPHIE*
            _ => false,
        },  
        LanguageID.Italian => trash switch
        {   
        //  [0xC7, 0xBB, 0xCC, 0xCE, 0xC3, 0xC8, 0xFF] => gender == 0, // MARTIN*
            [0xC7, 0xBB, 0xCC, 0xBD, 0xC9, 0xFF, 0xCA] => gender == 0, // MARCO*P
            [0xCA, 0xBB, 0xC9, 0xC6, 0xC9, 0xFF, 0xC6] => gender == 0, // PAOLO*L
            [0xC6, 0xCF, 0xBD, 0xC3, 0xC9, 0xFF, 0xC7] => gender == 0, // LUCIO*M
        //  [0xC7, 0xBB, 0xCC, 0xC3, 0xC8, 0xBB, 0xFF] => gender == 1, // MARINA*
            [0xBB, 0xC8, 0xC8, 0xC3, 0xBF, 0xFF, 0xBF] => gender == 1, // ANNIE*E
            [0xBF, 0xC6, 0xC3, 0xCD, 0xBB, 0xFF, 0xCD] => gender == 1, // ELISA*S
            [0xCD, 0xBB, 0xCC, 0xBB, 0xFF, 0xCB, 0xE9] => gender == 1, // SARA*Qu
            _ => false,
        },  
        LanguageID.German => trash switch
        {   
        //  [0xCD, 0xBF, 0xBC, 0xC9, 0xC6, 0xBE, 0xFF] => gender == 0, // SEBOLD*
        //  [0xBE, 0xBB, 0xC8, 0xC3, 0xBF, 0xC6, 0xFF] => gender == 0, // DANIEL*
            [0xC2, 0xBF, 0xC6, 0xC1, 0xBF, 0xFF, 0xC4] => gender == 0, // HELGE*J
            [0xC4, 0xBB, 0xC8, 0xFF, 0xC7, 0xBB, 0xCC] => gender == 0, // JAN*MAR
        //  [0xC7, 0xBB, 0xCC, 0xCE, 0xC3, 0xC8, 0xBB] => gender == 1, // MARTINA
            [0xCE, 0xBB, 0xC8, 0xC4, 0xBB, 0xFF, 0xBB] => gender == 1, // TANJA*A
        //  [0xBB, 0xC8, 0xBE, 0xCC, 0xBF, 0xBB, 0xFF] => gender == 1, // ANDREA*
            [0xCD, 0xBB, 0xCC, 0xBB, 0xFF, 0xBE, 0xDD] => gender == 1, // SARA*Di
            _ => false,
        },  
        LanguageID.Spanish => trash switch
        {   
        //  [0xC7, 0xBB, 0xCC, 0xC3, 0xC8, 0xC9, 0xFF] => gender == 0, // MARINO*
            [0xCB, 0xCF, 0xC3, 0xC7, 0xC3, 0xFF, 0xCC] => gender == 0, // QUIMI*R
            [0xCC, 0xCF, 0xC0, 0xC9, 0xFF, 0xBB, 0xCC] => gender == 0, // RUFO*AR
        //  [0xBB, 0xCC, 0xCE, 0xCF, 0xCC, 0xC9, 0xFF] => gender == 0, // ARTURO*
        //  [0xC7, 0xBB, 0xCC, 0xC3, 0xC8, 0xBB, 0xFF] => gender == 1, // MARINA*
        //  [0xCC, 0xBB, 0xCB, 0xCF, 0xBF, 0xC6, 0xFF] => gender == 1, // RAQUEL*
        //  [0xC7, 0xBB, 0xCC, 0xC3, 0xBB, 0xCF, 0xFF] => gender == 1, // MARIAU*
            [0xCD, 0xBB, 0xCC, 0xBB, 0xFF, 0xBB, 0xE5] => gender == 1, // SARA*Aq
            _ => false,
        },
        _ => false,
    };

    /// <inheritdoc cref="IsTrashPatternDefaultTrainer"/>
    /// <remarks>Default OT names present in <see cref="GameVersion.E"/> based on the language of the game.</remarks>
    public static bool IsTrashPatternDefaultTrainerE(ReadOnlySpan<byte> trash, LanguageID language, byte gender) => language switch
    {
        LanguageID.English => trash switch
        {
            [0xCD, 0xCE, 0xCF, 0xFF, 0xC7, 0xC3, 0xC6] => gender == 0, // STU*MIL
        //  [0xC7, 0xC3, 0xC6, 0xCE, 0xC9, 0xC8, 0xFF] => gender == 0, // MILTON*
            [0xCE, 0xC9, 0xC7, 0xFF, 0xC5, 0xBF, 0xC8] => gender == 0, // TOM*KEN
            [0xC5, 0xBF, 0xC8, 0xC8, 0xD3, 0xFF, 0xCC] => gender == 0, // KENNY*R
            [0xCC, 0xBF, 0xC3, 0xBE, 0xFF, 0xC4, 0xCF] => gender == 0, // REID*JU
            [0xC4, 0xCF, 0xBE, 0xBF, 0xFF, 0xC4, 0xBB] => gender == 0, // JUDE*JA
        //  [0xC4, 0xBB, 0xD2, 0xCD, 0xC9, 0xC8, 0xFF] => gender == 0, // JAXSON*
        //  [0xBF, 0xBB, 0xCD, 0xCE, 0xC9, 0xC8, 0xFF] => gender == 0, // EASTON*
        //  [0xD1, 0xBB, 0xC6, 0xC5, 0xBF, 0xCC, 0xFF] => gender == 0, // WALKER*
            [0xCE, 0xBF, 0xCC, 0xCF, 0xFF, 0xC4, 0xC9] => gender == 0, // TERU*JO
        //  [0xC4, 0xC9, 0xC2, 0xC8, 0xC8, 0xD3, 0xFF] => gender == 0, // JOHNNY*
            [0xBC, 0xCC, 0xBF, 0xCE, 0xCE, 0xFF, 0xCD] => gender == 0, // BRETT*S
            [0xCD, 0xBF, 0xCE, 0xC2, 0xFF, 0xCE, 0xBF] => gender == 0, // SETH*TE
            [0xCE, 0xBF, 0xCC, 0xCC, 0xD3, 0xFF, 0xBD] => gender == 0, // TERRY*C
            [0xBD, 0xBB, 0xCD, 0xBF, 0xD3, 0xFF, 0xBE] => gender == 0, // CASEY*D
        //  [0xBE, 0xBB, 0xCC, 0xCC, 0xBF, 0xC8, 0xFF] => gender == 0, // DARREN*
        //  [0xC6, 0xBB, 0xC8, 0xBE, 0xC9, 0xC8, 0xFF] => gender == 0, // LANDON*
        //  [0xBD, 0xC9, 0xC6, 0xC6, 0xC3, 0xC8, 0xFF] => gender == 0, // COLLIN*
        //  [0xCD, 0xCE, 0xBB, 0xC8, 0xC6, 0xBF, 0xD3] => gender == 0, // STANLEY
        //  [0xCB, 0xCF, 0xC3, 0xC8, 0xBD, 0xD3, 0xFF] => gender == 0, // QUINCY*
            [0xC5, 0xC3, 0xC7, 0xC7, 0xD3, 0xFF, 0xCE] => gender == 1, // KIMMY*T
            [0xCE, 0xC3, 0xBB, 0xCC, 0xBB, 0xFF, 0xBC] => gender == 1, // TIARA*B
            [0xBC, 0xBF, 0xC6, 0xC6, 0xBB, 0xFF, 0xC4] => gender == 1, // BELLA*J
            [0xC4, 0xBB, 0xD3, 0xC6, 0xBB, 0xFF, 0xBB] => gender == 1, // JAYLA*A
            [0xBB, 0xC6, 0xC6, 0xC3, 0xBF, 0xFF, 0xC6] => gender == 1, // ALLIE*L
        //  [0xC6, 0xC3, 0xBB, 0xC8, 0xC8, 0xBB, 0xFF] => gender == 1, // LIANNA*
            [0xCD, 0xBB, 0xCC, 0xBB, 0xFF, 0xC7, 0xC9] => gender == 1, // SARA*MO
        //  [0xC7, 0xC9, 0xC8, 0xC3, 0xBD, 0xBB, 0xFF] => gender == 1, // MONICA*
        //  [0xBD, 0xBB, 0xC7, 0xC3, 0xC6, 0xBB, 0xFF] => gender == 1, // CAMILA*
        //  [0xBB, 0xCF, 0xBC, 0xCC, 0xBF, 0xBF, 0xFF] => gender == 1, // AUBREE*
        //  [0xCC, 0xCF, 0xCE, 0xC2, 0xC3, 0xBF, 0xFF] => gender == 1, // RUTHIE*
            [0xC2, 0xBB, 0xD4, 0xBF, 0xC6, 0xFF, 0xC8] => gender == 1, // HAZEL*N
        //  [0xC8, 0xBB, 0xBE, 0xC3, 0xC8, 0xBF, 0xFF] => gender == 1, // NADINE*
            [0xCE, 0xBB, 0xC8, 0xC4, 0xBB, 0xFF, 0xD3] => gender == 1, // TANJA*Y
        //  [0xD3, 0xBB, 0xCD, 0xC7, 0xC3, 0xC8, 0xFF] => gender == 1, // YASMIN*
        //  [0xC8, 0xC3, 0xBD, 0xC9, 0xC6, 0xBB, 0xFF] => gender == 1, // NICOLA*
        //  [0xC6, 0xC3, 0xC6, 0xC6, 0xC3, 0xBF, 0xFF] => gender == 1, // LILLIE*
            [0xCE, 0xBF, 0xCC, 0xCC, 0xBB, 0xFF, 0xC6] => gender == 1, // TERRA*L
            [0xC6, 0xCF, 0xBD, 0xD3, 0xFF, 0xC2, 0xBB] => gender == 1, // LUCY*HA
            [0xC2, 0xBB, 0xC6, 0xC3, 0xBF, 0xFF, 0xCE] => gender == 1, // HALIE*T
            _ => false,
        },  
        LanguageID.French => trash switch
        {   
            [0xCD, 0xCE, 0xBF, 0xC0, 0xFF, 0xC7, 0xBB] => gender == 0, // STEF*MA
        //  [0xC7, 0xBB, 0xC8, 0xCF, 0xBF, 0xC6, 0xFF] => gender == 0, // MANUEL*
            [0xCD, 0xBF, 0xBC, 0xFF, 0xC1, 0xD1, 0xBF] => gender == 0, // SEB*GWE
            [0xC1, 0xD1, 0xBF, 0xC8, 0xC8, 0xFF, 0xBB] => gender == 0, // GWENN*A
            [0xBB, 0xCC, 0xC8, 0xC9, 0xFF, 0xC4, 0xCF] => gender == 0, // ARNO*JU
            [0xC4, 0xCF, 0xC6, 0xBF, 0xCD, 0xFF, 0xC4] => gender == 0, // JULES*J
        //  [0xC4, 0xC9, 0xC2, 0xBB, 0xC8, 0xC8, 0xFF] => gender == 0, // JOHANN*
        //  [0xCE, 0xC2, 0xC3, 0xBC, 0xBB, 0xCF, 0xBE] => gender == 0, // THIBAUD
            [0xBB, 0xC6, 0xBF, 0xBD, 0xFF, 0xC1, 0xC3] => gender == 0, // ALEC*GI
            [0xC1, 0xC3, 0xBC, 0xCF, 0xCD, 0xFF, 0xC4] => gender == 0, // GIBUS*J
        //  [0xC4, 0xC9, 0xC2, 0xC8, 0xC8, 0xD3, 0xFF] => gender == 0, // JOHNNY*
        //  [0xC0, 0xBB, 0xBC, 0xCC, 0xC3, 0xBD, 0xBF] => gender == 0, // FABRICE
        //  [0xBE, 0xBB, 0xC8, 0xC3, 0xBF, 0xC6, 0xFF] => gender == 0, // DANIEL*
        //  [0xCE, 0xC2, 0xC9, 0xC7, 0xBB, 0xCD, 0xFF] => gender == 0, // THOMAS*
            [0xC1, 0xBB, 0xCC, 0xD3, 0xFF, 0xCC, 0xCF] => gender == 0, // GARY*RU
            [0xCC, 0xCF, 0xBE, 0xBE, 0xD3, 0xFF, 0xCE] => gender == 0, // RUDDY*T
        //  [0xCE, 0xC2, 0xC3, 0xBF, 0xCC, 0xCC, 0xD3] => gender == 0, // THIERRY
            [0xBD, 0xC9, 0xC6, 0xC3, 0xC8, 0xFF, 0xCD] => gender == 0, // COLIN*S
            [0xCD, 0xCE, 0xBB, 0xC8, 0xFF, 0xCD, 0xBF] => gender == 0, // STAN*SE
        //  [0xCD, 0xBF, 0xD0, 0xBF, 0xCC, 0xC3, 0xC8] => gender == 0, // SEVERIN
            [0xBB, 0xC1, 0xC8, 0xBF, 0xCD, 0xFF, 0xBB] => gender == 1, // AGNES*A
        //  [0xBB, 0xCC, 0xC3, 0xBB, 0xC8, 0xBF, 0xFF] => gender == 1, // ARIANE*
            [0xBC, 0xBF, 0xC6, 0xC6, 0xBB, 0xFF, 0xC7] => gender == 1, // BELLA*M
            [0xC7, 0xBB, 0xBF, 0xD0, 0xBB, 0xFF, 0xCA] => gender == 1, // MAEVA*P
        //  [0xCA, 0xBB, 0xCF, 0xC6, 0xC3, 0xC8, 0xBF] => gender == 1, // PAULINE
            [0xBD, 0xC3, 0xC8, 0xBE, 0xD3, 0xFF, 0xCD] => gender == 1, // CINDY*S
        //  [0xCD, 0xC9, 0xCA, 0xC2, 0xC3, 0xBF, 0xFF] => gender == 1, // SOPHIE*
        //  [0xC7, 0xC9, 0xC8, 0xC3, 0xBD, 0xBB, 0xFF] => gender == 1, // MONICA*
            [0xBD, 0xBB, 0xCE, 0xC2, 0xD3, 0xFF, 0xC0] => gender == 1, // CATHY*F
            [0xC0, 0xBB, 0xC8, 0xC8, 0xD3, 0xFF, 0xCC] => gender == 1, // FANNY*R
        //  [0xCC, 0xC9, 0xD2, 0xBB, 0xC8, 0xBF, 0xFF] => gender == 1, // ROXANE*
            [0xBF, 0xBE, 0xC3, 0xCE, 0xC2, 0xFF, 0xC8] => gender == 1, // EDITH*N
        //  [0xC8, 0xBB, 0xBE, 0xC3, 0xC8, 0xBF, 0xFF] => gender == 1, // NADINE*
            [0xCE, 0xBB, 0xC8, 0xC3, 0xBB, 0xFF, 0xC4] => gender == 1, // TANIA*J
        //  [0xC4, 0xBB, 0xC8, 0xD3, 0xBD, 0xBF, 0xFF] => gender == 1, // JANYCE*
        //  [0xBD, 0xC6, 0xBB, 0xC3, 0xCC, 0xBF, 0xFF] => gender == 1, // CLAIRE*
            [0xC6, 0xC3, 0xC6, 0xC6, 0xD3, 0xFF, 0xCD] => gender == 1, // LILLY*S
        //  [0xCD, 0xC9, 0xC6, 0xBF, 0xC8, 0xBF, 0xFF] => gender == 1, // SOLENE*
        //  [0xBD, 0xD3, 0xC8, 0xCE, 0xC2, 0xC3, 0xBB] => gender == 1, // CYNTHIA
            [0xC7, 0xBB, 0xCF, 0xBE, 0xFF, 0xD0, 0xE3] => gender == 1, // MAUD*Vo
            _ => false,
        },  
        LanguageID.Italian => trash switch
        {   
        //  [0xC0, 0xCC, 0xBB, 0xC8, 0xBD, 0xD3, 0xFF] => gender == 0, // FRANCY*
        //  [0xC1, 0xC3, 0xC9, 0xCC, 0xC1, 0xC3, 0xC9] => gender == 0, // GIORGIO
            [0xC6, 0xCF, 0xBD, 0xC3, 0xC9, 0xFF, 0xC0] => gender == 0, // LUCIO*F
            [0xC0, 0xBB, 0xBC, 0xD3, 0xFF, 0xBB, 0xC8] => gender == 0, // FABY*AN
        //  [0xBB, 0xC8, 0xBE, 0xCC, 0xBF, 0xBB, 0xFF] => gender == 0, // ANDREA*
        //  [0xBE, 0xBB, 0xC8, 0xC3, 0xBF, 0xC6, 0xBF] => gender == 0, // DANIELE
        //  [0xC7, 0xC3, 0xBD, 0xC2, 0xBF, 0xC6, 0xBF] => gender == 0, // MICHELE
            [0xCC, 0xBF, 0xC8, 0xD4, 0xC9, 0xFF, 0xBF] => gender == 0, // RENZO*E
        //  [0xBF, 0xCF, 0xC1, 0xBF, 0xC8, 0xC3, 0xC9] => gender == 0, // EUGENIO
            [0xBF, 0xC6, 0xC3, 0xBB, 0xFF, 0xCD, 0xBB] => gender == 0, // ELIA*SA
        //  [0xCD, 0xBB, 0xC8, 0xBE, 0xCC, 0xC9, 0xFF] => gender == 0, // SANDRO*
        //  [0xCA, 0xC3, 0xBF, 0xCE, 0xCC, 0xC9, 0xFF] => gender == 0, // PIETRO*
            [0xCA, 0xBB, 0xC9, 0xC6, 0xC9, 0xFF, 0xC7] => gender == 0, // PAOLO*M
            [0xC7, 0xBB, 0xCC, 0xBD, 0xC9, 0xFF, 0xBB] => gender == 0, // MARCO*A
        //  [0xBB, 0xC6, 0xBC, 0xBF, 0xCC, 0xCE, 0xC9] => gender == 0, // ALBERTO
        //  [0xC0, 0xC3, 0xC6, 0xC3, 0xCA, 0xCA, 0xC9] => gender == 0, // FILIPPO
        //  [0xC6, 0xBB, 0xC8, 0xBE, 0xC9, 0xC8, 0xFF] => gender == 0, // LANDON*
            [0xC1, 0xC3, 0xC8, 0xC9, 0xFF, 0xBD, 0xBF] => gender == 0, // GINO*CE
            [0xBD, 0xBF, 0xBD, 0xBD, 0xC9, 0xFF, 0xC7] => gender == 0, // CECCO*M
            [0xC7, 0xBB, 0xCC, 0xC3, 0xC9, 0xFF, 0xBB] => gender == 0, // MARIO*A
            [0xBB, 0xC8, 0xC8, 0xC3, 0xBF, 0xFF, 0xBD] => gender == 1, // ANNIE*C
            [0xBD, 0xBB, 0xCE, 0xC3, 0xBB, 0xFF, 0xBC] => gender == 1, // CATIA*B
            [0xBC, 0xBF, 0xC6, 0xC6, 0xBB, 0xFF, 0xCA] => gender == 1, // BELLA*P
            [0xCA, 0xBB, 0xC9, 0xC6, 0xBB, 0xFF, 0xC6] => gender == 1, // PAOLA*L
            [0xC6, 0xCF, 0xC3, 0xCD, 0xBB, 0xFF, 0xC1] => gender == 1, // LUISA*G
        //  [0xC1, 0xCC, 0xBB, 0xD4, 0xC3, 0xBB, 0xFF] => gender == 1, // GRAZIA*
            [0xCD, 0xBB, 0xCC, 0xBB, 0xFF, 0xC7, 0xC9] => gender == 1, // SARA*MO
        //  [0xC7, 0xC9, 0xC8, 0xC3, 0xBD, 0xBB, 0xFF] => gender == 1, // MONICA*
            [0xC7, 0xBB, 0xCC, 0xCE, 0xBB, 0xFF, 0xCA] => gender == 1, // MARTA*P
            [0xCA, 0xC3, 0xBB, 0xFF, 0xCC, 0xC3, 0xCE] => gender == 1, // PIA*RIT
            [0xCC, 0xC3, 0xCE, 0xBB, 0xFF, 0xBF, 0xCC] => gender == 1, // RITA*ER
            [0xBF, 0xCC, 0xC3, 0xBD, 0xBB, 0xFF, 0xCC] => gender == 1, // ERICA*R
            [0xCC, 0xC9, 0xCD, 0xBB, 0xFF, 0xC7, 0xBF] => gender == 1, // ROSA*ME
        //  [0xC7, 0xBF, 0xC6, 0xC3, 0xCD, 0xCD, 0xBB] => gender == 1, // MELISSA
        //  [0xC7, 0xBB, 0xCC, 0xC3, 0xC8, 0xBB, 0xFF] => gender == 1, // MARINA*
            [0xBF, 0xC6, 0xC3, 0xCD, 0xBB, 0xFF, 0xC6] => gender == 1, // ELISA*L
            [0xC6, 0xC3, 0xC8, 0xBB, 0xFF, 0xCE, 0xBF] => gender == 1, // LINA*TE
        //  [0xCE, 0xBF, 0xCC, 0xBF, 0xCD, 0xBB, 0xFF] => gender == 1, // TERESA*
        //  [0xC6, 0xCF, 0xBD, 0xBF, 0xCE, 0xCE, 0xBB] => gender == 1, // LUCETTA
            [0xC6, 0xCF, 0xBD, 0xC3, 0xBB, 0xFF, 0xCB] => gender == 1, // LUCIA*Q
            _ => false,
        },  
        LanguageID.German => trash switch
        {   
        //  [0xCD, 0xCE, 0xBF, 0xC0, 0xBB, 0xC8, 0xFF] => gender == 0, // STEFAN*
        //  [0xC0, 0xC6, 0xC9, 0xCC, 0xC3, 0xBB, 0xC8] => gender == 0, // FLORIAN
            [0xC4, 0xBB, 0xC8, 0xFF, 0xBF, 0xCC, 0xC3] => gender == 0, // JAN*ERI
            [0xBF, 0xCC, 0xC3, 0xC5, 0xFF, 0xCE, 0xC2] => gender == 0, // ERIK*TH
        //  [0xCE, 0xC2, 0xC9, 0xC7, 0xBB, 0xCD, 0xFF] => gender == 0, // THOMAS*
        //  [0xC7, 0xBB, 0xCC, 0xCE, 0xC3, 0xC8, 0xFF] => gender == 0, // MARTIN*
        //  [0xC7, 0xBB, 0xCC, 0xC5, 0xCF, 0xCD, 0xFF] => gender == 0, // MARKUS*
            [0xC5, 0xC6, 0xBB, 0xCF, 0xCD, 0xFF, 0xCA] => gender == 0, // KLAUS*P
            [0xCA, 0xBB, 0xCF, 0xC6, 0xFF, 0xCC, 0xC9] => gender == 0, // PAUL*RO
            [0xCC, 0xC9, 0xC6, 0xC0, 0xFF, 0xC4, 0xF2] => gender == 0, // ROLF*JÖ
            [0xC4, 0xF2, 0xCC, 0xC1, 0xFF, 0xC2, 0xBB] => gender == 0, // JÖRG*HA
            [0xC2, 0xBB, 0xC3, 0xC5, 0xC9, 0xFF, 0xC2] => gender == 0, // HAIKO*H
            [0xC2, 0xBF, 0xC6, 0xC1, 0xBF, 0xFF, 0xBE] => gender == 0, // HELGE*D
        //  [0xBE, 0xBB, 0xC8, 0xC3, 0xBF, 0xC6, 0xFF] => gender == 0, // DANIEL*
        //  [0xC7, 0xC3, 0xBD, 0xC2, 0xBB, 0xBF, 0xC6] => gender == 0, // MICHAEL
            [0xBE, 0xBB, 0xD0, 0xC3, 0xBE, 0xFF, 0xCC] => gender == 0, // DAVID*R
        //  [0xCC, 0xC9, 0xC6, 0xBB, 0xC8, 0xBE, 0xFF] => gender == 0, // ROLAND*
        //  [0xC4, 0xC9, 0xC2, 0xBB, 0xC8, 0xC8, 0xFF] => gender == 0, // JOHANN*
        //  [0xBE, 0xC3, 0xBF, 0xCE, 0xBF, 0xCC, 0xFF] => gender == 0, // DIETER*
        //  [0xBB, 0xC8, 0xCD, 0xBF, 0xC6, 0xC7, 0xFF] => gender == 0, // ANSELM*
            [0xCE, 0xBB, 0xC8, 0xC4, 0xBB, 0xFF, 0xC7] => gender == 1, // TANJA*M
        //  [0xC7, 0xC3, 0xCC, 0xC4, 0xBB, 0xC7, 0xFF] => gender == 1, // MIRJAM*
        //  [0xC7, 0xBB, 0xCC, 0xCE, 0xC3, 0xC8, 0xBB] => gender == 1, // MARTINA
            [0xC4, 0xBB, 0xC7, 0xC3, 0xBF, 0xFF, 0xBD] => gender == 1, // JAMIE*C
        //  [0xBD, 0xBB, 0xCC, 0xC9, 0xC6, 0xC3, 0xC8] => gender == 1, // CAROLIN
        //  [0xCD, 0xC3, 0xC7, 0xC9, 0xC8, 0xBF, 0xFF] => gender == 1, // SIMONE*
            [0xCD, 0xBB, 0xCC, 0xBB, 0xFF, 0xBD, 0xC6] => gender == 1, // SARA*CL
        //  [0xBD, 0xC6, 0xBB, 0xCF, 0xBE, 0xC3, 0xBB] => gender == 1, // CLAUDIA
        //  [0xC4, 0xBB, 0xCD, 0xC7, 0xC3, 0xC8, 0xFF] => gender == 1, // JASMIN*
        //  [0xBE, 0xBF, 0xC8, 0xC3, 0xCD, 0xBF, 0xFF] => gender == 1, // DENISE*
        //  [0xC5, 0xBB, 0xCE, 0xCC, 0xC3, 0xC8, 0xFF] => gender == 1, // KATRIN*
        //  [0xC5, 0xBF, 0xCC, 0xCD, 0xCE, 0xC3, 0xC8] => gender == 1, // KERSTIN
        //  [0xCD, 0xD0, 0xBF, 0xC8, 0xC4, 0xBB, 0xFF] => gender == 1, // SVENJA*
            [0xBC, 0xBF, 0xBB, 0xCE, 0xBF, 0xFF, 0xC7] => gender == 1, // BEATE*M
            [0xC7, 0xBF, 0xC3, 0xC5, 0xBF, 0xFF, 0xBB] => gender == 1, // MEIKE*A
        //  [0xBB, 0xC8, 0xBE, 0xCC, 0xBF, 0xBB, 0xFF] => gender == 1, // ANDREA*
            [0xBF, 0xD0, 0xBB, 0xFF, 0xCA, 0xBF, 0xCE] => gender == 1, // EVA*PET
            [0xCA, 0xBF, 0xCE, 0xCC, 0xBB, 0xFF, 0xC1] => gender == 1, // PETRA*G
            [0xC1, 0xBB, 0xBC, 0xC3, 0xFF, 0xC8, 0xBB] => gender == 1, // GABI*NA
        //  [0xC8, 0xBB, 0xBE, 0xC3, 0xC8, 0xBF, 0xFF] => gender == 1, // NADINE*
            _ => false,
        },  
        LanguageID.Spanish => trash switch
        {   
            [0xBF, 0xC6, 0xBF, 0xC8, 0xC9, 0xFF, 0xC6] => gender == 0, // ELENO*L
            [0xC6, 0xBB, 0xCC, 0xBF, 0xC9, 0xFF, 0xBB] => gender == 0, // LAREO*A
        //  [0xBB, 0xCC, 0xCE, 0xCF, 0xCC, 0xC9, 0xFF] => gender == 0, // ARTURO*
            [0xBD, 0xBB, 0xCC, 0xC6, 0xC9, 0xFF, 0xC7] => gender == 0, // CARLO*M
            [0xC7, 0xBB, 0xCF, 0xCC, 0xC3, 0xFF, 0xBE] => gender == 0, // MAURI*D
        //  [0xBE, 0xBB, 0xC8, 0xC3, 0xBF, 0xC6, 0xFF] => gender == 0, // DANIEL*
        //  [0xC7, 0xBB, 0xCC, 0xBD, 0xBF, 0xC6, 0xC9] => gender == 0, // MARCELO
        //  [0xCC, 0xC9, 0xBC, 0xBF, 0xCC, 0xCE, 0xC9] => gender == 0, // ROBERTO
            [0xBB, 0xC3, 0xCE, 0xC9, 0xCC, 0xFF, 0xC4] => gender == 0, // AITOR*J
            [0xC4, 0xCF, 0xC6, 0xC3, 0xFF, 0xC8, 0xBB] => gender == 0, // JULI*NA
        //  [0xC8, 0xBB, 0xCC, 0xBD, 0xC3, 0xCD, 0xC9] => gender == 0, // NARCISO
            [0xC6, 0xCF, 0xC3, 0xCD, 0xFF, 0xCC, 0xCF] => gender == 0, // LUIS*RU
            [0xCC, 0xCF, 0xC0, 0xC9, 0xFF, 0xCB, 0xCF] => gender == 0, // RUFO*QU
            [0xCB, 0xCF, 0xC3, 0xC7, 0xC3, 0xFF, 0xC4] => gender == 0, // QUIMI*J
        //  [0xC4, 0xBF, 0xCD, 0xCF, 0xCD, 0xC9, 0xFF] => gender == 0, // JESUSO*
            [0xC7, 0xBB, 0xCC, 0xBD, 0xC9, 0xFF, 0xCE] => gender == 0, // MARCO*T
            [0xCE, 0xBF, 0xCC, 0xBF, 0xC8, 0xFF, 0xC7] => gender == 0, // TEREN*M
            [0xC7, 0xBB, 0xCC, 0xC3, 0xC9, 0xFF, 0xCA] => gender == 0, // MARIO*P
            [0xCA, 0xBF, 0xBE, 0xCC, 0xC9, 0xFF, 0xBF] => gender == 0, // PEDRO*E
        //  [0xBF, 0xC8, 0xCC, 0xC3, 0xCB, 0xCF, 0xBF] => gender == 0, // ENRIQUE
        //  [0xCC, 0xBB, 0xCB, 0xCF, 0xBF, 0xC6, 0xFF] => gender == 1, // RAQUEL*
            [0xBF, 0xC6, 0xBF, 0xC8, 0xBB, 0xFF, 0xCA] => gender == 1, // ELENA*P
            [0xCA, 0xBB, 0xC6, 0xC7, 0xBB, 0xFF, 0xC6] => gender == 1, // PALMA*L
            [0xC6, 0xBB, 0xCC, 0xBB, 0xFF, 0xBD, 0xBB] => gender == 1, // LARA*CA
        //  [0xBD, 0xBB, 0xCC, 0xC6, 0xC9, 0xCE, 0xBB] => gender == 1, // CARLOTA
            [0xC7, 0xC9, 0xC8, 0xBB, 0xFF, 0xCD, 0xBB] => gender == 1, // MONA*SA
            [0xCD, 0xBB, 0xCC, 0xBB, 0xFF, 0xBE, 0xBB] => gender == 1, // SARA*DA
        //  [0xBE, 0xBB, 0xC8, 0xC3, 0xBF, 0xC6, 0xBB] => gender == 1, // DANIELA
        //  [0xC9, 0xC6, 0xC3, 0xC7, 0xCA, 0xC3, 0xBB] => gender == 1, // OLIMPIA
        //  [0xC7, 0xBB, 0xCC, 0xBD, 0xBF, 0xC6, 0xBB] => gender == 1, // MARCELA
        //  [0xCC, 0xC9, 0xBC, 0xBF, 0xCC, 0xCE, 0xBB] => gender == 1, // ROBERTA
        //  [0xBB, 0xCC, 0xBB, 0xC8, 0xBD, 0xC2, 0xBB] => gender == 1, // ARANCHA
        //  [0xC4, 0xCF, 0xC6, 0xC3, 0xBF, 0xCE, 0xBB] => gender == 1, // JULIETA
        //  [0xC8, 0xC9, 0xBF, 0xC6, 0xC3, 0xBB, 0xFF] => gender == 1, // NOELIA*
        //  [0xC6, 0xCF, 0xBD, 0xC3, 0xCE, 0xBB, 0xFF] => gender == 1, // LUCITA*
        //  [0xC7, 0xBB, 0xCC, 0xC3, 0xBB, 0xCF, 0xFF] => gender == 1, // MARIAU*
            [0xCA, 0xBB, 0xC9, 0xC6, 0xBB, 0xFF, 0xCE] => gender == 1, // PAOLA*T
        //  [0xCE, 0xBF, 0xCC, 0xBF, 0xCD, 0xBB, 0xFF] => gender == 1, // TERESA*
            [0xC8, 0xCF, 0xCC, 0xC3, 0xBB, 0xFF, 0xC6] => gender == 1, // NURIA*L
            [0xC6, 0xC3, 0xC8, 0xBB, 0xFF, 0xBB, 0xE5] => gender == 1, // LINA*Aq
            _ => false,
        },
        _ => false,
    };
}
