using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

internal sealed class MiscVerifierPK8 : Verifier
{
    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data)
    {
        if (data.Entity is PK8 pk)
            Verify(data, pk);
    }

    internal void Verify(LegalityAnalysis data, PK8 pk)
    {
        MiscVerifierHelpers.VerifyStatNature(data, pk);
        VerifyTechRecordFlags(data, pk);
        FullnessRules.Verify(data, pk);

        VerifySociability(data, pk);

        if (!pk.IsBattleVersionValid(data.Info.EvoChainsAllGens))
            data.AddLine(GetInvalid(StatBattleVersionInvalid));

        var enc = data.EncounterMatch;
        VerifyGigantamax(data, pk, enc);
        VerifyDynamax(data, pk);
    }

    private static void VerifySociability(LegalityAnalysis data, PK8 pk)
    {
        var social = pk.Sociability;
        if (pk.IsEgg)
        {
            if (social != 0)
                data.AddLine(GetInvalid(Encounter, MemorySocialZero));
        }
        else if (social > byte.MaxValue)
        {
            data.AddLine(GetInvalid(Encounter, MemoryStatSocialLEQ_0, 0));
        }
    }

    private void VerifyGigantamax(LegalityAnalysis data, PK8 pk, IEncounterTemplate enc)
    {
        bool originGMax = enc is IGigantamaxReadOnly { CanGigantamax: true };
        if (originGMax != pk.CanGigantamax)
        {
            bool ok = !pk.IsEgg && Gigantamax.CanToggle(pk.Species, pk.Form, enc.Species, enc.Form);
            var chk = ok ? GetValid(StatGigantamaxValid) : GetInvalid(StatGigantamaxInvalid);
            data.AddLine(chk);
        }
    }

    private void VerifyDynamax(LegalityAnalysis data, PK8 pk)
    {
        if (pk.DynamaxLevel != 0)
        {
            if (!pk.CanHaveDynamaxLevel(pk) || pk.DynamaxLevel > 10)
                data.AddLine(GetInvalid(StatDynamaxInvalid));
        }
    }

    private static ushort GetPermitIndex<T>(T permit, int index) where T : IPermitRecord => permit.RecordPermitIndexes[index];

    private static bool CanLearnTR(ushort species, byte form, int tr)
    {
        var pi = PersonalTable.SWSH.GetFormEntry(species, form);
        return pi.GetIsLearnTR(tr);
    }

    internal static void VerifyTechRecordFlags<T>(LegalityAnalysis data, T pk) where T : PKM, ITechRecord
    {
        // Flags for Gen8 format are transferred via HOME. S/V+ do not retain these flags.
        var permit = pk.Permit;
        var count = permit.RecordCountUsed;
        var evos = data.Info.EvoChainsAllGens.Gen8;
        if (evos.Length == 0)
        {
            VerifyTechRecordNone(data, pk, permit, count);
            return;
        }

        PersonalInfo8SWSH? pi = null;
        for (int i = 0; i < count; i++)
        {
            if (!pk.GetMoveRecordFlag(i))
                continue;
            if ((pi ??= GetPersonal(evos[0])).GetIsLearnTR(i))
                continue;

            // Calyrex-0 can have TR flags for Calyrex-1/2 after it has force unlearned them.
            // Re-fusing can reacquire the move via relearner, rather than needing another TR.
            // Calyrex-0 cannot reacquire the move via relearner, even though the TR is checked off in the TR list.
            if (pk.Species == (int)Species.Calyrex && CanCalyrexFormLearn(currentForm: pk.Form, i))
                continue;

            data.AddLine(GetInvalid(Misc, MoveTechRecordFlagMissing_0, GetPermitIndex(permit, i)));
        }
    }

    private static bool CanCalyrexFormLearn(byte currentForm, int index)
    {
        // Check if another alt form can learn the TR
        if (currentForm != 1 && CanLearnTR((int)Species.Calyrex, 1, index))
            return true;
        if (currentForm != 2 && CanLearnTR((int)Species.Calyrex, 2, index))
            return true;
        return false;
    }

    private static PersonalInfo8SWSH GetPersonal(EvoCriteria evo) => PersonalTable.SWSH.GetFormEntry(evo.Species, evo.Form);

    private static void VerifyTechRecordNone<T>(LegalityAnalysis data, T pk, IPermitRecord permit, int count)
        where T : PKM, ITechRecord
    {
        for (int i = 0; i < count; i++)
        {
            if (!pk.GetMoveRecordFlag(i))
                continue;
            data.AddLine(GetInvalid(Misc, MoveTechRecordFlagMissing_0, GetPermitIndex(permit, i)));
        }
    }
}
