using System;

namespace PKHeX.Core
{
    public sealed class TrashByteVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Trash;

        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm.Format < 6)
                return;

            if (pkm.IsEgg)
            {
                if (!ArrayUtil.IsRangeEmpty(pkm.HT_Trash))
                    data.AddLine(GetInvalid($"{nameof(PKM.HT_Trash)} detected."));
                if (HasTrash2(pkm.Nickname_Trash))
                    data.AddLine(GetInvalid($"{nameof(PKM.Nickname_Trash)} detected."));
                if (HasTrash2(pkm.OT_Trash))
                    data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} detected."));
            }
            else
            {
                var enc = data.EncounterOriginal;
                if (enc is EncounterStatic8U { ShouldHaveScientistTrash: true})
                {
                    if (EncounterStatic8U.HasScientistTrash(pkm) == false)
                        data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} does not match expected trash."));
                }
                else if (HasTrash2(pkm.OT_Trash))
                {
                    data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} detected."));
                }

                if (HasTrash2(pkm.HT_Trash))
                    data.AddLine(Get($"{nameof(PKM.HT_Trash)} detected.", Severity.Fishy));
                if (HasTrash2(pkm.Nickname_Trash))
                    data.AddLine(Get($"{nameof(PKM.Nickname_Trash)} detected.", Severity.Fishy));
            }
        }

        private static bool HasTrash2(ReadOnlySpan<byte> buffer)
        {
            var terminator = FindTerminator2(buffer);
            return terminator == -1 || !buffer[(terminator+2)..].IsRangeEmpty();
        }

        private static int FindTerminator2(ReadOnlySpan<byte> buffer, byte terminator = 0)
        {
            for (int i = 0; i < buffer.Length; i += 2)
            {
                if (buffer[i + 1] == terminator && buffer[i] == terminator)
                    return i;
            }
            return -1;
        }

        private static int FindTerminator1(ReadOnlySpan<byte> buffer, byte terminator = 0)
        {
            for (int i = 0; i < buffer.Length; i += 2)
            {
                if (buffer[i] == terminator)
                    return i;
            }
            return -1;
        }
    }
}