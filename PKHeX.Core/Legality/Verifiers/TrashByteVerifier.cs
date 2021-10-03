using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.TrashBytes16;

namespace PKHeX.Core
{
    public sealed class TrashByteVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Trash;

        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            // Don't bother with previous generation formats yet.
            var format = pkm.Format;
            if (format < 6)
            {
                if (format < 3) // GB era
                    VerifyTrashVC(data, pkm);
                else if (format == 3)
                    VerifyTrashGBA(data, pkm);
                else
                    VerifyTrashDS(data, pkm);
                return;
            }

            VerifyFinalTerminator(data, pkm);
            if (pkm.IsEgg)
            {
                VerifyTrashIsEgg(data, pkm);
                return;
            }

            var enc = data.EncounterOriginal;
            if (IsTrashCleared(enc.Generation, pkm.Format))
            {
                VerifyTransferTrash(data, pkm);
                return;
            }

            VerifyTrashNickname(data, pkm, enc);
            VerifyTrashOT(data, pkm, enc);
            VerifyTrashHT(data, pkm);
        }

        private void VerifyTrashDS(LegalityAnalysis data, PKM pkm)
        {
            var enc = data.EncounterOriginal;
            if (IsTrashCleared(enc.Generation, pkm.Format))
            {
                VerifyTransferTrash(data, pkm);
                return;
            }

            if (enc is PCD { IsEgg: false } pcd)
            {
                // Can't nickname these, so they must retain the original trash.
                var original = pcd.Gift.PK;
                VerifyTrashPCD_Nickname(data, pkm, original);
                VerifyTrashPCD_OT(data, pkm, original);
                return;
            }

            var trashNick = pkm.Nickname_Trash;
            var trashNickIndex = FindTerminator(trashNick, 0xFF);
            if (trashNickIndex == -1)
            {
                data.AddLine(GetInvalid($"{nameof(PKM.Nickname_Trash)} is missing a terminator."));
                return;
            }

            if (!HasFinalTerminator(trashNick) && trashNickIndex + 2 != trashNick.Length)
            {
                // Allow nicknamed content to insert terminators and keyboard characters inside the mutable region.
                var littleEndian = pkm is not BK4;
                for (int i = trashNickIndex + 2; i < trashNick.Length; i+=2)
                {
                    var character = littleEndian
                        ? (trashNick[i] | (trashNick[i + 1] << 8))
                        : (trashNick[i + 1] | (trashNick[i] << 8));
                    if (character is 0)
                    {
                        data.AddLine(GetInvalid($"{nameof(PKM.Nickname_Trash)} has empty trash between terminators."));
                        return;
                    }
                }
            }

            var trashOT = pkm.OT_Trash;
            var trashOTIndex = FindTerminator(trashOT, 0xFF);
            if (trashOTIndex == -1)
            {
                data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} is missing a terminator."));
                return;
            }

            {
                var littleEndian = pkm is not BK4;
                for (int i = trashOTIndex + 2; i < trashOT.Length; i+=2)
                {
                    var character = littleEndian
                        ? (trashOT[i] | (trashOT[i + 1] << 8))
                        : (trashOT[i + 1] | (trashOT[i] << 8));
                    if (character is not 0)
                    {
                        data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} has nonzero trash after terminator."));
                        return;
                    }
                }
            }

            if (!HasFinalTerminator(trashOT) && trashOTIndex + 2 != trashOT.Length)
            {
                data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} detected at reserved terminator offset."));
            }
        }

        private void VerifyTrashPCD_Nickname(LegalityAnalysis data, PKM pkm, PKM original)
        {
            var finalRaw = pkm.Nickname_Trash;
            var firstTerminator = FindTerminator(finalRaw, 0xFF);
            if (firstTerminator == -1)
            {
                data.AddLine(GetInvalid($"{nameof(PKM.Nickname_Trash)} is missing a terminator."));
                return;
            }

            if (firstTerminator + 2 == finalRaw.Length)
            {
                // No trash bytes for Nickname
                data.AddLine(GetValid($"{nameof(PKM.Nickname_Trash)} is full."));
                return;
            }

            var originalRaw = original.Nickname_Trash;
            bool hasOriginalTrash = HasUnderlayer(finalRaw, originalRaw, firstTerminator + 2);
            if (!hasOriginalTrash)
                data.AddLine(GetInvalid($"{nameof(PKM.Nickname_Trash)} does not match the original trash bytes."));
        }

        private void VerifyTrashPCD_OT(LegalityAnalysis data, PKM pkm, PKM original)
        {
            var finalRaw = pkm.OT_Trash;
            var firstTerminator = FindTerminator(finalRaw, 0xFF);
            if (firstTerminator == -1)
            {
                data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} is missing a terminator."));
                return;
            }

            if (firstTerminator + 2 == finalRaw.Length)
            {
                // No trash bytes for Nickname
                data.AddLine(GetValid($"{nameof(PKM.OT_Trash)} is full."));
                return;
            }

            var originalRaw = original.OT_Trash;
            bool hasOriginalTrash = HasUnderlayer(finalRaw, originalRaw, firstTerminator + 2);
            if (!hasOriginalTrash)
                data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} does not match the original trash bytes."));
        }

        private void VerifyTrashGBA(LegalityAnalysis data, PKM pkm)
        {
            if (pkm is CK3 or XK3)
                return; // don't bother

            // PK3 strings can potentially not have a terminator in the span.
            // If a terminator is present, scan all following bytes.
            // Nicknamed Pokémon are all FF. Since RAM is heavily reused, trash can exist for anything (?)
            var ntrash = pkm.Nickname_Trash;
            var nterm = ntrash.IndexOf(StringConverter3.TerminatorByte);
            if (nterm != -1 && nterm != (ntrash.Length-1))
            {
                var slice = ntrash[(nterm+1)..];
                bool allTrashFF = !pkm.IsEgg && pkm.IsNicknamed && data.Info.EncounterMatch is not EncounterTrade { HasNickname: true };
                if (allTrashFF)
                {
                    foreach (var x in slice)
                    {
                        if (x == StringConverter3.TerminatorByte)
                            continue;
                        data.AddLine(GetInvalid($"{nameof(PKM.Nickname_Trash)} detected at reserved terminator offset."));
                        break;
                    }
                }
                else
                {
                    foreach (var x in slice)
                    {
                        if (x == 0)
                            continue;
                        data.AddLine(Get($"{nameof(PKM.Nickname_Trash)} detected.", Severity.Fishy));
                        break;
                    }
                }
            }

            // Check if all bytes after first terminator are terminators.
            var otrash = pkm.OT_Trash;
            var oterm = otrash.IndexOf(StringConverter3.TerminatorByte);
            if (oterm != -1 && oterm != (otrash.Length - 1))
            {
                var slice = otrash[(oterm+1)..];
                foreach (var x in slice)
                {
                    if (x == StringConverter3.TerminatorByte)
                        continue;
                    data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} detected at reserved terminator offset."));
                    break;
                }
            }
        }

        private void VerifyTrashVC(LegalityAnalysis data, PKM pkm)
        {
            if (!HasFinalTerminator(pkm.OT_Trash, StringConverter12.G1TerminatorCode))
                data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} detected at reserved terminator offset."));
        }

        /// <summary>
        /// Starting in generation 6, \0 is used as a terminator, and all trash byte sections are consistently clean. Flag anything that has nonzero values in them.
        /// </summary>
        private void VerifyFinalTerminator(LegalityAnalysis data, PKM pkm)
        {
            if (!HasFinalTerminator(pkm.Nickname_Trash))
                data.AddLine(GetInvalid($"{nameof(PKM.Nickname_Trash)} detected at reserved terminator offset."));
            if (!HasFinalTerminator(pkm.OT_Trash))
                data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} detected at reserved terminator offset."));
            if (!HasFinalTerminator(pkm.HT_Trash))
                data.AddLine(GetInvalid($"{nameof(PKM.HT_Trash)} detected at reserved terminator offset."));
        }

        private void VerifyTransferTrash(LegalityAnalysis data, PKM pkm)
        {
            var trashFormat = GetTransferFormat(data.Info.Generation, pkm.Format);
            switch (trashFormat)
            {
                case TransferTrashFormat.None:         VerifyTransferTrashNone        (data, pkm); break;
                case TransferTrashFormat.PalPark4:     VerifyTransferTrashPalPark4    (data, pkm); break;
                case TransferTrashFormat.Transporter5: VerifyTransferTrashTransporter5(data, pkm); break;
                case TransferTrashFormat.Bank6:        VerifyTransferTrashBank6       (data, pkm); break;
                case TransferTrashFormat.Virtual7:     VerifyTransferTrashVirtual7    (data, pkm); break;
                case TransferTrashFormat.Home8:        VerifyTransferTrashHome8       (data, pkm); break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            VerifyTrashHT(data, pkm);
        }

        private void VerifyTransferTrashPalPark4(LegalityAnalysis data, PKM pkm)
        {
            var finalRaw = pkm.Nickname_Trash;
            var firstTerminator = FindTerminator(finalRaw, 0xFF);
            if (firstTerminator == -1)
            {
                data.AddLine(GetInvalid($"{nameof(PKM.Nickname_Trash)} is missing a terminator."));
                return;
            }

            if (firstTerminator + 2 == finalRaw.Length)
            {
                // No trash bytes for Nickname
                data.AddLine(GetValid($"{nameof(PKM.Nickname_Trash)} is full."));
                return;
            }

            // Future: Check Pal Park Trash Bytes

            finalRaw = pkm.OT_Trash;
            firstTerminator = FindTerminator(finalRaw, 0xFF);
            if (firstTerminator == -1)
            {
                data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} is missing a terminator."));
                return;
            }

            if (firstTerminator + 2 == finalRaw.Length)
            {
                // No trash bytes for Nickname
                data.AddLine(GetValid($"{nameof(PKM.Nickname_Trash)} is full."));
            }
        }

        private void VerifyTransferTrashTransporter5(LegalityAnalysis data, PKM pkm)
        {
            if (HasTrash(pkm.OT_Trash))
                data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} detected."));
            VerifyTrashNickname(data, pkm, data.EncounterMatch);
        }

        private void VerifyTransferTrashBank6(LegalityAnalysis data, PKM pkm)
        {
            if (HasTrash(pkm.OT_Trash))
                data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} detected."));

            // Transferring 5->6 sets the localized current species name before copying over the current Nickname string, regardless of it being nicknamed.
            var evos = data.Info.EvoChainsAllGens[5];
            if (!HasUnderlayerAnyEvo(pkm, evos, 6))
                data.AddLine(GetInvalid($"{nameof(PKM.Nickname_Trash)} under-layer missing."));
        }

        private void VerifyTransferTrashVirtual7(LegalityAnalysis data, PKM pkm)
        {
            if (HasTrash(pkm.OT_Trash))
                data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} detected."));

            // Transferring VC->7 sets the localized current species name before copying over the current Nickname string, regardless of it being nicknamed.
            var evos = data.Info.EvoChainsAllGens[1].Concat(data.Info.EvoChainsAllGens[2]);
            if (!HasUnderlayerAnyEvo(pkm, evos, 7))
                data.AddLine(GetInvalid($"{nameof(PKM.Nickname_Trash)} under-layer missing."));
        }

        private static bool HasUnderlayerAnyEvo(PKM pkm, IEnumerable<EvoCriteria> evos, int generation)
        {
            var trash = pkm.Nickname_Trash;
            var firstTrash = FindTerminator(trash) + 2;
            foreach (var evo in evos)
            {
                if (HasUnderlayerAnySpecies(trash, firstTrash, evo.Species, generation))
                    return true;
            }
            return false;
        }

        private void VerifyTransferTrashHome8(LegalityAnalysis data, PKM pkm) => VerifyTransferTrashNone(data, pkm);

        private void VerifyTransferTrashNone(LegalityAnalysis data, PKM pkm)
        {
            if (HasTrash(pkm.Nickname_Trash))
                data.AddLine(GetInvalid($"{nameof(PKM.Nickname_Trash)} detected."));
            if (HasTrash(pkm.OT_Trash))
                data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} detected."));
        }

        private void VerifyTrashNickname(LegalityAnalysis data, PKM pkm, IEncounterTemplate enc)
        {
            var trash = pkm.Nickname_Trash;
            if (!HasTrash(trash))
                return;

            // As of generation 8, cannot nickname something from another language.
            // Nicknames can have trash up to the max for the language.
            var firstTrash = FindTerminator(trash) + 2;
            var lastTrash = FindLastTrash(trash, firstTrash);
            var expectedEnd = Legal.GetMaxLengthNickname(enc.Generation, (LanguageID)pkm.Language);
            var lastTrashCharIndex = (lastTrash / 2);

            var severity = Severity.Fishy;
            if (lastTrashCharIndex > expectedEnd)
            {
                // Some scenarios can set trash beyond, where the encounter is from someone else.
                // Find the uppermost trash beginnings that is within the mutable region.
                var extraTrash = FindNextTrashBackwards(trash, (expectedEnd * 2) + 2);
                if (IsExtraTrashValid(pkm, trash, enc, extraTrash))
                    severity = extraTrash / 2 == expectedEnd + 1 ? Severity.Valid : Severity.Fishy; // multiple nicknames
                else
                    severity = Severity.Invalid;
            }
            data.AddLine(Get($"{nameof(PKM.Nickname_Trash)} detected.", severity));
        }

        private void VerifyTrashHT(LegalityAnalysis data, PKM pkm)
        {
            var trash = pkm.HT_Trash;
            if (HasTrash(trash))
                data.AddLine(Get($"{nameof(PKM.HT_Trash)} detected.", Severity.Fishy));
        }

        private void VerifyTrashOT(LegalityAnalysis data, PKM pkm, IEncounterTemplate enc)
        {
            // Some encounters are first created with a fixed OT name, then when captured, the Trainer name is applied over top.
            if (enc is EncounterStatic8U { ShouldHaveScientistTrash: true })
            {
                if (EncounterStatic8U.HasScientistTrash(pkm) == false)
                    data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} does not match expected trash."));
                return;
            }

            var trash = pkm.OT_Trash;
            bool hasTrash = HasTrash(trash);
            if (!hasTrash)
                return;

            // Traded eggs can have trash from the original OT name.
            if (enc.EggEncounter && pkm.WasTradedEgg)
                data.AddLine(Get($"{nameof(PKM.OT_Trash)} detected.", Severity.Fishy));
            else if (enc is WC7 { TID: 18075, OT_Name: { Length: > 0 } }) // Ash Pikachu sets trainer name then overwrites it
                VerifyTrashAnyChar(data, pkm, enc, trash);
            else
                data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} detected."));
        }

        private void VerifyTrashAnyChar(LegalityAnalysis data, ILangNick pkm, IGeneration enc, ReadOnlySpan<byte> trash)
        {
            var firstTrash = FindTerminator(trash) + 2;
            var lastTrash = FindLastTrash(trash, firstTrash);
            var expectedEnd = Legal.GetMaxLengthOT(enc.Generation, (LanguageID)pkm.Language);
            if (lastTrash > expectedEnd * 2)
            {
                data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} detected."));
                return;
            }

            // Ensure there are no other trash breaks in the trash region.
            var nextTrash = FindNextTrashBackwards(trash, lastTrash);
            if (nextTrash != firstTrash)
            {
                data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} detected."));
                return;
            }

            // keyboard check?

            data.AddLine(GetValid($"Valid underlying {nameof(PKM.OT_Trash)} detected."));
        }

        /// <summary>
        /// Eggs are created first by creating a Box Pokémon, then put into an egg with the localized Egg name.
        /// The Nickname trash must have the species name beneath the egg name.
        /// </summary>
        private void VerifyTrashIsEgg(LegalityAnalysis data, PKM pkm)
        {
            if (HasTrash(pkm.Nickname_Trash))
            {
                // Eggs have the species name underneath the current egg name.
                var over = pkm.Nickname;
                var under = SpeciesName.GetSpeciesNameGeneration(pkm.Species, pkm.Language, data.Info.Generation);
                if (!HasUnderlayerSpecies(pkm.Nickname_Trash, under, over))
                    data.AddLine(GetInvalid($"{nameof(PKM.Nickname_Trash)} does not match expected trash."));
            }

            if (HasTrash(pkm.OT_Trash))
                data.AddLine(GetInvalid($"{nameof(PKM.OT_Trash)} detected."));
            if (!ArrayUtil.IsRangeEmpty(pkm.HT_Trash))
                data.AddLine(GetInvalid($"{nameof(PKM.HT_Trash)} detected."));
        }

        private static bool IsExtraTrashValid(PKM pkm, ReadOnlySpan<byte> trash, IEncounterTemplate enc, int firstTrash)
        {
            // Traded eggs inherit the language of the hatching OT, so the encounter species could be from any language.
            if (enc.EggEncounter && pkm.WasTradedEgg)
                return HasUnderlayerAnySpecies(trash, firstTrash, enc.Species, enc.Generation);

            // Shared raids use the language ID of the host, so the encounter species could be from any language.
            if (enc is EncounterStatic8N or EncounterStatic8ND or EncounterStatic8NC && pkm.Met_Location == Encounters8Nest.SharedNest)
                return HasUnderlayerAnySpecies(trash, firstTrash, enc.Species, enc.Generation);

            // Force nicknamed events apply the species name of the redeeming language, then slap on the forced Nickname.
            if (enc is WC8 w && w.GetIsNicknamed(pkm.Language))
            {
                if (!pkm.IsNicknamed)
                    return false;
                var nick = SpeciesName.GetSpeciesNameGeneration(enc.Species, pkm.Language, enc.Generation);
                if (nick != pkm.Nickname) // shouldn't be flagged
                    return false;
                return HasUnderlayerAnySpecies(trash, firstTrash, enc.Species, enc.Generation);
            }

            return false;
        }

        private static TransferTrashFormat GetTransferFormat(int origin, int format) => format switch
        {
            4 or 5 when origin == 3 => TransferTrashFormat.PalPark4,
            5 when origin < 5 => TransferTrashFormat.Transporter5,
            6 or 7 when origin is (3 or 4 or 5) => TransferTrashFormat.Bank6,
            7 when origin < 3 => TransferTrashFormat.Virtual7,
            8 => TransferTrashFormat.Home8,
            _ => TransferTrashFormat.None,
        };

        public static bool IsTrashCleared(int origin, int format) => origin != format && origin switch
        {
            (1 or 2) when format is (1 or 2) => false,
            4 when format is 5 => false,
            6 when format is 7 => false,
            _ => true,
        };

        private enum TransferTrashFormat
        {
            None,
            PalPark4,
            Transporter5,
            Bank6,
            Virtual7,
            Home8,
        }
    }
}
