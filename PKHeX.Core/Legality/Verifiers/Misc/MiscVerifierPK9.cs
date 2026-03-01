using System;
using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

internal sealed class MiscVerifierPK9 : Verifier
{
    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data)
    {
        if (data.Entity is PK9 pk)
            Verify(data, pk);
    }

    internal void Verify(LegalityAnalysis data, PK9 pk)
    {
        MiscVerifierHelpers.VerifyStatNature(data, pk);
        VerifyTechRecordFlags(data, pk);

        if (!pk.IsBattleVersionValid(data.Info.EvoChainsAllGens))
            data.AddLine(GetInvalid(StatBattleVersionInvalid));

        if (!MiscVerifierHelpers.IsObedienceLevelValid(pk, pk.ObedienceLevel, pk.MetLevel))
            data.AddLine(GetInvalid(TransferObedienceLevel));

        VerifyTeraType(data, pk);

        var enc = data.EncounterOriginal;
        VerifyEncounter(data, pk, enc);

        if (!Locations9.IsAccessiblePreDLC(pk.MetLocation))
            VerifyPreDLC(data, pk, enc);
    }

    private void VerifyPreDLC(LegalityAnalysis data, PK9 pk, IEncounterTemplate enc)
    {
        if (enc is { Species: (int)Species.Larvesta, Form: 0 } and not EncounterEgg9)
            DisallowLevelUpMove(24, (ushort)Move.BugBite, pk, data);
        else if (enc is { Species: (int)Species.Zorua, Form: 1 } and not EncounterEgg9)
            DisallowLevelUpMove(28, (ushort)Move.Spite, pk, data);
        else
            return;

        // Safari and Sport are not obtainable in the base game.
        // For the learnset restricted cases, we need to check if the ball is available too.
        if (((BallUseLegality.WildPokeballs9PreDLC2 >> pk.Ball) & 1) != 1)
            data.AddLine(GetInvalid(BallUnavailable));
    }

    private void VerifyEncounter(LegalityAnalysis data, PK9 pk, IEncounterTemplate enc)
    {
        if (enc is EncounterEgg9 g)
        {
            if (!Tera9RNG.IsMatchTeraTypePersonalEgg(g.Species, g.Form, (byte)pk.TeraTypeOriginal))
                data.AddLine(GetInvalid(TeraTypeMismatch));
        }
        else if (enc is ITeraRaid9)
        {
            var seed = Tera9RNG.GetOriginalSeed(pk);
            data.Info.PIDIV = new PIDIV(PIDType.Tera9, seed);
        }
        else if (enc is not { Context: EntityContext.Gen9 } || pk is { GO_HOME: true })
        {
            if (pk.TeraTypeOverride == (MoveType)TeraTypeUtil.OverrideNone)
                data.AddLine(GetInvalid(TeraTypeIncorrect));
            else if (GetTeraImportMatch(data.Info.EvoChainsAllGens.Gen9, pk.TeraTypeOriginal, enc) == -1)
                data.AddLine(GetInvalid(TeraTypeIncorrect));
        }
        else if (enc is EncounterStatic9 { StarterBoxLegend: true })
        {
            // Ride legends cannot be traded or transferred.
            if (pk.CurrentHandler != 0 || pk.Tracker != 0 || !pk.IsUntraded)
                data.AddLine(GetInvalid(TransferBad));
        }
    }

    private void VerifyTeraType(LegalityAnalysis data, PK9 pk9)
    {
        if (pk9.IsEgg)
        {
            if (pk9.TeraTypeOverride != (MoveType)TeraTypeUtil.OverrideNone)
                data.AddLine(GetInvalid(TeraTypeIncorrect));
        }
        else if (pk9.Species == (int)Species.Terapagos)
        {
            if (!TeraTypeUtil.IsValidTerapagos((byte)pk9.TeraTypeOverride))
                data.AddLine(GetInvalid(TeraTypeIncorrect));
        }
        else if (pk9.Species == (int)Species.Ogerpon)
        {
            if (!TeraTypeUtil.IsValidOgerpon((byte)pk9.TeraTypeOverride, pk9.Form))
                data.AddLine(GetInvalid(TeraTypeIncorrect));
        }
        else
        {
            if (!TeraTypeUtil.IsValid((byte)pk9.TeraTypeOriginal))
                data.AddLine(GetInvalid(TeraTypeIncorrect));
        }
    }

    private static void DisallowLevelUpMove(byte level, ushort move, PK9 pk, LegalityAnalysis data)
    {
        if (pk.Tracker != 0)
            return;
        int index = pk.GetMoveIndex(move);
        if (index == -1)
            return;

        ref var m = ref data.Info.Moves[index];
        if (m.Info.Method != LearnMethod.LevelUp || m.Info.Argument != level)
            return;
        var flagIndex = pk.Permit.RecordPermitIndexes.IndexOf(move);
        ArgumentOutOfRangeException.ThrowIfNegative(flagIndex, nameof(move)); // Always expect it to match.
        if (pk.GetMoveRecordFlag(flagIndex))
            return;
        m = m with { Info = m.Info with { Method = LearnMethod.None } };
    }

    private static int GetTeraImportMatch(ReadOnlySpan<EvoCriteria> evos, MoveType actual, IEncounterTemplate enc)
    {
        // Sanitize out Form here for Arceus
        if (evos.Length == 0 || evos[0].Species is (int)Species.Arceus)
            return actual == MoveType.Normal ? 0 : -1;
        for (int i = evos.Length - 1; i >= 0; i--)
        {
            var evo = evos[i];
            if (FormInfo.IsFormChangeable(evo.Species, enc.Form, evo.Form, enc.Context, EntityContext.Gen9))
            {
                if (Tera9RNG.IsMatchTeraTypePersonalAnyFormImport(evo.Species, (byte)actual))
                    return i;
            }
            else
            {
                if (Tera9RNG.IsMatchTeraTypePersonalImport(evo.Species, evo.Form, (byte)actual))
                    return i;
            }
        }
        return -1;
    }

    private void VerifyTechRecordFlags(LegalityAnalysis data, PK9 pk)
    {
        var permit = pk.Permit;
        var count = permit.RecordCountUsed;
        var evos = data.Info.EvoChainsAllGens.Gen9;
        if (evos.Length == 0)
        {
            VerifyTechRecordNone(data, pk, permit, count);
            return;
        }

        PersonalInfo9SV? pi = null;
        for (int i = 0; i < count; i++)
        {
            var evo = evos[0];
            if (!pk.GetMoveRecordFlag(i))
                continue;
            if ((pi ??= GetPersonal(evo)).GetIsLearnTM(i))
                continue;

            if (evo.Species is (int)Species.Deoxys)
            {
                if (CanDeoxysFormLearn(pi, evo, i))
                    continue;
            }
            else
            {
                if (CanPreEvoLearn(evos, i))
                    continue;
            }
            data.AddLine(GetInvalid(MoveTechRecordFlagMissing_0, GetPermitIndex(permit, i)));
        }
    }

    private static bool CanPreEvoLearn(ReadOnlySpan<EvoCriteria> evos, int index)
    {
        // Zoroark-0 cannot learn Encore via TM, but the pre-evolution Zorua-0 can via TM.
        // Double check if any pre-evolutions can learn the TM.
        for (int p = 1; p < evos.Length; p++) // ignore head (final) evo, already checked.
        {
            if (GetPersonal(evos[p]).GetIsLearnTM(index))
                return true;
        }
        return false;
    }

    private static bool CanDeoxysFormLearn(PersonalInfo9SV pi, EvoCriteria evo, int index)
    {
        // Deoxys has different TM permissions depending on form.
        var fc = pi.FormCount;
        for (int p = 1; p < fc; p++) // Already checked form-0.
        {
            if (GetPersonal(evo.Species, (byte)p).GetIsLearnTM(index))
                return true;
        }
        return false;
    }

    private static PersonalInfo9SV GetPersonal(EvoCriteria evo) => GetPersonal(evo.Species, evo.Form);
    private static PersonalInfo9SV GetPersonal(ushort species, byte form) => PersonalTable.SV.GetFormEntry(species, form);

    private void VerifyTechRecordNone(LegalityAnalysis data, PK9 pk, IPermitRecord permit, int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (!pk.GetMoveRecordFlag(i))
                continue;
            data.AddLine(GetInvalid(MoveTechRecordFlagMissing_0, GetPermitIndex(permit, i)));
        }
    }

    private static ushort GetPermitIndex<T>(T permit, int index) where T : IPermitRecord => permit.RecordPermitIndexes[index];
}
