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
            if (!TrashByteRules3.IsTrashPatternDefaultTrainer(trash, pk.Version, (LanguageID)pk.Language))
                data.AddLine(GetInvalid(Trainer, TrashBytesMissingTerminatorFinal));
        }
        // Nickname can be all FF's (nicknamed) or whatever random garbage is in the buffer before filling. Unsure if we can reliably check this, but it should be "dirty" usually.
        // If it is clean, flag as fishy.
        FlagIsNicknameClean(data, pk);
    }

    private static void FlagIsNicknameClean(LegalityAnalysis data, PK3 pk)
    {
        if (!pk.IsNicknamed || pk.IsEgg)
            return;
        // Japanese only fills the first 5+1 bytes; everything else is trash.
        // International games are 10 chars (full buffer) max; implicit terminator if full.
        var nick = pk.GetNicknamePrefillRegion();
        if (!TrashByteRules3.IsTerminatedFF(nick))
            data.AddLine(GetInvalid(Trainer, TrashBytesMismatchInitial));
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
    // - - verification of Default OTs todo (if OT dirty, check if is default with expected trash pattern)
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
        return !data[(first + 1)..].ContainsAnyExcept<byte>(0xFF);
    }

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
            first++;
            if (first >= data.Length - 1)
                return true;
        }
        return !data[first..].ContainsAnyExcept<byte>(0);
    }

    // TRASH BYTES: New Game Default OTs
    // Default OT names in International (not JPN) Gen3 mainline games memcpy 7 chars and FF from the "default OT name" table, regardless of strlen.
    // Copied strings therefore contain trash from the next string entry that is encoded into the rom's string table.
    // Below is a list of possible (version, language, trash) default OTs, as initialized by the game. An `*` is used to denote the terminator.

    /// <summary>
    /// Checks if the specified trash byte pattern matches a default trainer name pattern for the given game <see cref="version"/> and <see cref="language"/>.
    /// </summary>
    /// <remarks>Default trainer names in certain Generation 3 Pokémon games may include trailing bytes ("trash") due to how names are stored in the game's ROM.
    /// This method checks if the provided pattern matches any of these known default patterns for the specified version and language.
    /// </remarks>
    public static bool IsTrashPatternDefaultTrainer(ReadOnlySpan<byte> trash, GameVersion version, LanguageID language) => version switch
    {
        GameVersion.R or GameVersion.S => IsTrashPatternDefaultTrainerRS(trash, language),
        GameVersion.E => IsTrashPatternDefaultTrainerE(trash, language),
        GameVersion.FR => IsTrashPatternDefaultTrainerFR(trash, language),
        GameVersion.LG => IsTrashPatternDefaultTrainerLG(trash, language),
        _ => false,
    };

    /// <inheritdoc cref="IsTrashPatternDefaultTrainer"/>
    /// <remarks>Default OT names present in <see cref="GameVersion.R"/> and <see cref="GameVersion.S"/> based on the language of the game.</remarks>
    public static bool IsTrashPatternDefaultTrainerRS(ReadOnlySpan<byte> trash, LanguageID language) => language switch
    {
        // TODO
        LanguageID.English => trash switch
        {
            [0xCD, 0xBB, 0xCC, 0xBB, 0xFF, 0xCE, 0xDC] => true, // SARA*Th
            _ => false,
        },
        _ => false,
    };

    /// <inheritdoc cref="IsTrashPatternDefaultTrainer"/>
    /// <remarks>Default OT names present in <see cref="GameVersion.E"/> based on the language of the game.</remarks>
    public static bool IsTrashPatternDefaultTrainerE(ReadOnlySpan<byte> trash, LanguageID language) => language switch
    {
        // TODO
        _ => false,
    };

    /// <inheritdoc cref="IsTrashPatternDefaultTrainer"/>
    /// <remarks>Default OT names present in <see cref="GameVersion.FR"/> based on the language of the game.</remarks>
    public static bool IsTrashPatternDefaultTrainerFR(ReadOnlySpan<byte> trash, LanguageID language) => language switch
    {
        // TODO
        _ => false,
    };

    /// <inheritdoc cref="IsTrashPatternDefaultTrainer"/>
    /// <remarks>Default OT names present in <see cref="GameVersion.LG"/> based on the language of the game.</remarks>
    public static bool IsTrashPatternDefaultTrainerLG(ReadOnlySpan<byte> trash, LanguageID language) => language switch
    {
        // TODO
        _ => false,
    };
}
