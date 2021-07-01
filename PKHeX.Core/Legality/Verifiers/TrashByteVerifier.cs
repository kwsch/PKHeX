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

            // Data in this format has immutable trash bytes.
            // Flag anything that has nonzero values in them.
            if (!HasFinalTerminator(pkm.Nickname_Trash))
                data.AddLine(GetInvalid($"{nameof(PKM.Nickname_Trash)} detected in reserved terminator."));
            if (!HasFinalTerminator(pkm.OT_Trash))
                data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} detected in reserved terminator."));
            if (!HasFinalTerminator(pkm.HT_Trash))
                data.AddLine(GetInvalid($"{nameof(PKM.HT_Trash)} detected in reserved terminator."));

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
                    var firstTrash = FindTerminator2(pkm.Nickname_Trash) + 2;
                    var lastTrash = FindLastTrash2(pkm.Nickname_Trash, firstTrash);
                    var expectedEnd = Legal.GetMaxLengthNickname(enc.Generation, (LanguageID) pkm.Language);
                    var lastTrashCharIndex = (lastTrash / 2);
                    bool hasExtraTrash = expectedEnd < lastTrashCharIndex;
                    var allowed = !hasExtraTrash || IsExtraTrashValid(pkm, enc, firstTrash);
                    var severity = !allowed ? Severity.Invalid : Severity.Fishy;
                    data.AddLine(Get($"{nameof(PKM.Nickname_Trash)} detected.", severity));
                }
            }
        }

        private static bool IsExtraTrashValid(PKM pkm, IEncounterable enc, int firstTrash)
        {
            if (!AllowExtraTrash(pkm, enc))
                return false;

            // check all languages for original species
            var trash = pkm.Nickname_Trash;
            var species = enc.Species;
            var generation = enc.Generation;
            for (int language = 1; language <= (int) LanguageID.ChineseT; language++)
            {
                if (language == (int) LanguageID.Hacked)
                    continue;

                var name = SpeciesName.GetSpeciesNameGeneration(species, language, generation);
                if (HasTrashUnderlayer(firstTrash - 4, name, trash))
                    return true;
            }
            return false;
        }

        private static bool AllowExtraTrash(PKM pkm, IEncounterable enc)
        {
            if (enc.EggEncounter && pkm.WasTradedEgg)
                return true;
            if (enc is EncounterStatic8N or EncounterStatic8ND or EncounterStatic8NC && pkm.Met_Location == Encounters8Nest.SharedNest)
                return true;
            return false;
        }

        private static bool HasFinalTerminator(ReadOnlySpan<byte> buffer, byte terminator = 0) => buffer[^1] == terminator && buffer[^2] == terminator;

        private static int FindLastTrash2(ReadOnlySpan<byte> buffer, int start, byte terminator = 0)
        {
            for (int i = buffer.Length - 2; i > start; i -= 2)
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

        public static bool HasTrashUnderlayer(int topLength, string under, Span<byte> full_trash)
        {
            var trash = full_trash[((topLength * 2) + 2)..];
            var nameBytes = StringConverter.SetString7b(under, under.Length, full_trash.Length / 2);
            var span = nameBytes.AsSpan((topLength * 2) + 2);
            return trash.SequenceEqual(span);
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