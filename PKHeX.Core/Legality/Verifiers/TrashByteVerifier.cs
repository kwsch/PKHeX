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
                if (HasTrash2(pkm.Nickname_Trash))
                {
                    // Eggs have the species name underneath the current egg name.
                    var over = pkm.Nickname;
                    var under = SpeciesName.GetSpeciesNameGeneration(pkm.Species, pkm.Language, data.EncounterOriginal.Generation);
                    if (!HasTrashUnderlayer(over, under, pkm.Nickname_Trash))
                        data.AddLine(GetInvalid($"{nameof(PKM.Nickname_Trash)} does not match expected trash."));
                }

                if (!ArrayUtil.IsRangeEmpty(pkm.HT_Trash))
                    data.AddLine(GetInvalid($"{nameof(PKM.HT_Trash)} detected."));
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
                else if (enc.EggEncounter && pkm.WasTradedEgg)
                {
                    // Traded eggs can have trash from the original OT name.
                    if (HasTrash2(pkm.OT_Trash))
                        data.AddLine(Get($"{nameof(PKM.OT_Trash)} detected.", Severity.Fishy));
                }
                else if (HasTrash2(pkm.OT_Trash))
                {
                    data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} detected."));
                }

                if (HasTrash2(pkm.HT_Trash))
                    data.AddLine(Get($"{nameof(PKM.HT_Trash)} detected.", Severity.Fishy));
                if (HasTrash2(pkm.Nickname_Trash))
                {
                    var position = FindTerminator2(pkm.Nickname_Trash);
                    position = FindLastTrash2(pkm.Nickname_Trash, position);
                    var severity = Legal.GetMaxLengthNickname(enc.Generation, (LanguageID)pkm.Language) < position ? Severity.Invalid : Severity.Fishy;
                    data.AddLine(Get($"{nameof(PKM.Nickname_Trash)} detected.", severity));
                }
            }
        }

        private static int FindLastTrash2(ReadOnlySpan<byte> buffer, int start, byte terminator = 0)
        {
            for (int i = buffer.Length - 2; i > start + 2; i -= 2)
            {
                if (buffer[i + 1] == terminator && buffer[i] == terminator)
                    continue;
                return i;
            }
            return start;
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

        public static bool HasTrashUnderlayer(string top, string under, Span<byte> full_trash)
        {
            var trash = full_trash[((top.Length * 2) + 2)..];
            var nameBytes = StringConverter.SetString7b(under, under.Length, full_trash.Length / 2);
            var span = nameBytes.AsSpan((top.Length * 2) + 2);
            return trash.SequenceEqual(span);
        }
    }
}