using static PKHeX.Core.LegalityCheckResultCode;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="PKM.ID32"/>.
/// </summary>
public sealed class TrainerIDVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.Trainer;

    public override void Verify(LegalityAnalysis data)
    {
        var enc = data.EncounterMatch;
        if (!TrainerNameVerifier.IsPlayerOriginalTrainer(enc))
            return; // already verified

        var pk = data.Entity;
        if (enc.Context is EntityContext.Gen8b)
        {
            // Game loops to ensure a nonzero full-ID
            // int.MaxValue cannot be yielded by Unity's Random.Range[min, max)
            var id32 = pk.ID32;
            if (id32 is 0 or int.MaxValue)
            {
                data.AddLine(GetInvalid(OT_IDInvalid));
                return;
            }
        }
        else if (pk.Version == GameVersion.CXD)
        {
            VerifyTrainerID_CXD(data, pk);
        }
        else if (pk.Version is GameVersion.R or GameVersion.S)
        {
            // If the trainer ID is that of the player, verify it is possible
            // For eggs, the version does not update on hatch, so it truly is from R/S.

            // Eggs from R/S that are traded to Emerald can obtain the Emerald TID/SID without updating version.
            // Flag it as fishy for manual inspection. If it matches an Emerald trainer, it's fine.
            var severity = enc is EncounterEgg3 && !pk.IsEgg ? Severity.Fishy : Severity.Invalid;
            VerifyTrainerID_RS(data, pk, severity);
        }
        else if (pk.VC)
        {
            // Only TID is used for Gen 1/2 VC
            if (pk.SID16 != 0)
                data.AddLine(GetInvalid(OT_SID0Invalid));
            if (pk.TID16 == 0)
                data.AddLine(Get(Severity.Fishy, OT_TID0));
            return;
        }
        else if (pk.Format <= 2)
        {
            // Only TID is used for Gen 1/2
            if (pk.TID16 == 0)
                data.AddLine(Get(Severity.Fishy, OT_TID0));
            return;
        }

        if (pk is { ID32: 0 })
            data.AddLine(Get(Severity.Fishy, OT_IDs0));
        else if (pk.TID16 == pk.SID16)
            data.AddLine(Get(Severity.Fishy, OT_IDEqual));
        else if (pk.TID16 == 0)
            data.AddLine(Get(Severity.Fishy, OT_TID0));
        else if (pk.SID16 == 0)
            data.AddLine(Get(Severity.Fishy, OT_SID0));
        else if (IsOTIDSuspicious(pk.TID16, pk.SID16))
            data.AddLine(Get(Severity.Fishy, OTSuspicious));
    }

    public static bool TryGetShinySID(uint pid, ushort tid, GameVersion version, out ushort sid)
    {
        var xor = ((pid >> 16) ^ (pid & 0xFFFF) ^ tid) & 0xFFF8;
        uint bits = Util.Rand32();
        if (version is GameVersion.R or GameVersion.S)
            return MethodH.TryGetShinySID(tid, out sid, xor, bits);
        if (version is GameVersion.CXD)
            return MethodCXD.TryGetShinySID(tid, out sid, xor, bits);

        sid = (ushort)(xor ^ (bits & 7));
        return true;
    }

    private static void VerifyTrainerID_CXD<T>(LegalityAnalysis data, T tr) where T : ITrainerID32ReadOnly
    {
        var severity = Severity.Invalid;
        if (data.EncounterOriginal is (EncounterSlot3XD or EncounterShadow3XD or EncounterStatic3XD))
        {
            // XD does not do the random Poké Ball sequence on naming menu for PAL copies of the game.
            var lang = (LanguageID)data.Entity.Language;
            if (lang > LanguageID.English) // definitely PAL
                return;
            if (lang is LanguageID.English)
                severity = Severity.Fishy; // can be PAL or US, let user verify.
            // japanese will stay Invalid
        }
        if (!MethodCXD.TryGetSeedTrainerID(tr.TID16, tr.SID16, out _))
            data.AddLine(Get(CheckIdentifier.Trainer, severity, TrainerIDNoSeed));
    }

    private static void VerifyTrainerID_RS<T>(LegalityAnalysis data, T tr, Severity severity = Severity.Invalid) where T : ITrainerID32ReadOnly
    {
        if (!MethodH.TryGetSeedTrainerID(tr.TID16, tr.SID16, out _))
            data.AddLine(Get(CheckIdentifier.Trainer, severity, TrainerIDNoSeed));
    }

    public static bool IsOTIDSuspicious(ushort tid16, ushort sid16) => (tid16, sid16) switch
    {
        (12345, 54321) => true,
        (15040, 18831) => true, // 1234_123456 (SID7_TID7)
        _ => false,
    };
}
