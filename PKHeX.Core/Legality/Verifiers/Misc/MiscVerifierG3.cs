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
        if (TrashTrainer3.IsPatternDefault(trash, version, language, gender))
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
            if (!TrashTrainer3.IsPatternDefault(trash, hatchVersion, language, gender))
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
            return false; // Hasn't evolved.

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
}
