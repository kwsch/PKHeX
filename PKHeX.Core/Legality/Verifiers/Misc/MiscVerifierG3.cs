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
        if (ParseSettings.AllowGBACrossTransferRSE(pk))
            return;

        // Only FR/LG are released. Only can originate from FR/LG.
        if (pk.Version is not (GameVersion.FR or GameVersion.LG))
            data.AddLine(GetInvalid(TradeNotAvailable));
        else if (Legal.IsForeignFRLG(pk.Species))
            data.AddLine(GetInvalid(TradeNotAvailable));

        if (ItemStorage3FRLG_VC.IsUnreleasedHeld(pk.HeldItem))
            data.AddLine(GetInvalid(ItemUnreleased));

        if (pk is PK3 pk3)
            VerifyTrash(data, pk3);
    }

    private void VerifyTrash(LegalityAnalysis data, PK3 pk)
    {
        var enc = data.EncounterOriginal;
        if (enc is EncounterTrade3)
            VerifyTrashTrade(data, pk);
        else if (pk.Japanese && !(pk.IsEgg && pk.OriginalTrainerTrash[^1] == 0x00))
            VerifyTrashJPN(data, pk);
        else
            VerifyTrashINT(data, pk);
    }

    private void VerifyTrashTrade(LegalityAnalysis data, PK3 pk)
    {
        // For in-game trades, zeroes after the first terminator.
        var trash = pk.OriginalTrainerTrash;
        int len = TrashBytes8.GetStringLength(trash);
        if (len == trash.Length)
            return; // OK
        if (trash[(len+1)..].ContainsAnyExcept<byte>(0))
            data.AddLine(GetInvalid(TrashBytesMissingTerminator));
    }

    private void VerifyTrashJPN(LegalityAnalysis data, PK3 pk)
    {
        var trash = pk.OriginalTrainerTrash;
        // OT name from save file is copied byte-for-byte. Byte 7 & 8 are always zero.

        // PK3 do not store the 8th byte. Check the 7th explicitly.
        if (trash[^1] != 0x00)
            data.AddLine(GetInvalid(TrashBytesMissingTerminator));

        int len = TrashBytes8.GetStringLength(trash);
        if (trash[len..^2].ContainsAnyExcept<byte>(0xFF))
            data.AddLine(GetInvalid(TrashBytesMissingTerminator));
    }

    private void VerifyTrashINT(LegalityAnalysis data, PK3 pk)
    {
        var trash = pk.OriginalTrainerTrash;
        // OT name from save file is copied byte-for-byte. All 8 bytes are initialized to FF on new game.

        int len = TrashBytes8.GetStringLength(trash);
        if (trash[len..].ContainsAnyExcept<byte>(0xFF))
            data.AddLine(GetInvalid(TrashBytesMissingTerminator));
    }
}
