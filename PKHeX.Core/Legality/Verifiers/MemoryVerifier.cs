using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.Encounters6;
using static PKHeX.Core.Encounters8;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the <see cref="PKM.OT_Memory"/>, <see cref="PKM.HT_Memory"/>, and associated values.
    /// </summary>
    public sealed class MemoryVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Memory;

        public override void Verify(LegalityAnalysis data)
        {
            VerifyOTMemory(data);
            VerifyHTMemory(data);
        }

        private CheckResult VerifyCommonMemory(PKM pkm, int handler, int gen)
        {
            var memory = MemoryVariableSet.Read(pkm, handler);

            // Actionable HM moves
            int matchingMoveMemory = Array.IndexOf(Memories.MoveSpecificMemories[0], memory.MemoryID);
            if (matchingMoveMemory != -1)
            {
                // Gen8 has no HMs, so this memory can never exist.
                if (gen != 6 || (pkm.Species != (int)Species.Smeargle && !Legal.GetCanLearnMachineMove(pkm, Memories.MoveSpecificMemories[1][matchingMoveMemory], 6)))
                    return GetInvalid(string.Format(LMemoryArgBadMove, memory.Handler));
            }

            switch (memory.MemoryID)
            {
                // {0} saw {2} carrying {1} on its back. {4} that {3}.
                case 21 when gen != 6 || !Legal.GetCanLearnMachineMove(new PK6 {Species = memory.Variable, EXP = Experience.GetEXP(100, PersonalTable.XY.GetFormeIndex(memory.Variable, 0))}, 19, 6):
                    return GetInvalid(string.Format(LMemoryArgBadMove, memory.Handler));

                case 16 when memory.Variable == 0 && !GetIsMoveKnowable(pkm, gen, memory.Variable):
                case 48 when memory.Variable == 0 && !GetIsMoveKnowable(pkm, gen, memory.Variable):
                    return GetInvalid(string.Format(LMemoryArgBadMove, memory.Handler));

                // {0} was able to remember {2} at {1}'s instruction. {4} that {3}.
                case 49 when memory.Variable == 0 && !GetIsMoveLearnable(pkm, gen, memory.Variable):
                    return GetInvalid(string.Format(LMemoryArgBadMove, memory.Handler));
            }

            if (gen == 6 && !Memories.CanHaveIntensity(memory.MemoryID, memory.Intensity))
            {
                if (pkm.Gen6 || (pkm.Gen7 && memory.MemoryID != 0)) // todo: memory intensity checks for gen8
                  return GetInvalid(string.Format(LMemoryIndexIntensityMin, memory.Handler, Memories.GetMinimumIntensity(memory.MemoryID)));
            }

            if (gen == 6 && memory.MemoryID != 4 && !Memories.CanHaveFeeling(memory.MemoryID, memory.Feeling))
            {
                if (pkm.Gen6 || (pkm.Gen7 && memory.MemoryID != 0)) // todo: memory feeling checks for gen8
                    return GetInvalid(string.Format(LMemoryFeelInvalid, memory.Handler));
            }

            return GetValid(string.Format(LMemoryF_0_Valid, memory.Handler));
        }

        private static bool GetIsMoveKnowable(PKM pkm, int gen, int move) => Legal.GetCanKnowMove(pkm, move, gen);
        private static bool GetIsMoveLearnable(PKM pkm, int gen, int move) => Legal.GetCanRelearnMove(pkm, move, gen);

        /// <summary>
        /// Used for enforcing a fixed memory detail.
        /// </summary>
        /// <param name="data">Output storage</param>
        /// <param name="m">Memory ID</param>
        /// <param name="i">Intensity</param>
        /// <param name="t">Text Variable</param>
        /// <param name="f">Feeling</param>
        private void VerifyOTMemoryIs(LegalityAnalysis data, int m, int i, int t, int f)
        {
            var pkm = data.pkm;
            if (pkm.OT_Memory != m)
                data.AddLine(GetInvalid(string.Format(LMemoryIndexID, L_XOT, m)));
            if (pkm.OT_Intensity != i)
                data.AddLine(GetInvalid(string.Format(LMemoryIndexIntensity, L_XOT, i)));
            if (pkm.OT_TextVar != t)
                data.AddLine(GetInvalid(string.Format(LMemoryIndexVar, L_XOT, t)));
            if (pkm.OT_Feeling != f)
                data.AddLine(GetInvalid(string.Format(LMemoryIndexFeel, L_XOT, f)));
        }

        private void VerifyHTMemoryNone(LegalityAnalysis data, PKM pkm)
        {
            if (pkm.HT_Memory != 0 || pkm.HT_TextVar != 0 || pkm.HT_Intensity != 0 || pkm.HT_Feeling != 0)
                data.AddLine(GetInvalid(string.Format(LMemoryCleared, L_XHT)));
        }

        private void VerifyOTMemory(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var Info = data.Info;

            switch (data.EncounterMatch)
            {
                case WC6 g when !g.IsEgg && g.OTGender != 3:
                    VerifyOTMemoryIs(data, g.OT_Memory, g.OT_Intensity, g.OT_TextVar, g.OT_Feeling);
                    return;
                case WC7 g when !g.IsEgg && g.OTGender != 3:
                    VerifyOTMemoryIs(data, g.OT_Memory, g.OT_Intensity, g.OT_TextVar, g.OT_Feeling);
                    return;
                case WC8 g when !g.IsEgg && g.OTGender != 3:
                    VerifyOTMemoryIs(data, g.OT_Memory, g.OT_Intensity, g.OT_TextVar, g.OT_Feeling);
                    return;

                case IMemoryOT t when !(t is MysteryGift): // Ignore Mystery Gift cases (covered above)
                    VerifyOTMemoryIs(data, t.OT_Memory, t.OT_Intensity, t.OT_TextVar, t.OT_Feeling);
                    return;
            }

            int memoryGen = Info.Generation;
            int memory = pkm.OT_Memory;

            if (pkm.IsEgg)
            {
                // Traded unhatched eggs in Gen8 have OT link trade memory applied erroneously.
                if (memoryGen != 8 || !(pkm.Met_Location == Locations.LinkTrade6 && memory == 4))
                {
                    VerifyOTMemoryIs(data, 0, 0, 0, 0); // empty
                    return;
                }
            }
            else if (!CanHaveMemory(pkm, memoryGen, memory))
            {
                VerifyOTMemoryIs(data, 0, 0, 0, 0); // empty
                return;
            }

            // Bounds checking
            switch (memoryGen)
            {
                case 6 when pkm.XY && (memory > Memories.MAX_MEMORY_ID_XY || Memories.Memory_NotXY.Contains(memory)):
                case 6 when pkm.AO && (memory > Memories.MAX_MEMORY_ID_AO || Memories.Memory_NotAO.Contains(memory)):
                case 8 when pkm.SWSH && (memory > Memories.MAX_MEMORY_ID_SWSH || Memories.Memory_NotSWSH.Contains(memory)):
                    data.AddLine(GetInvalid(string.Format(LMemoryArgBadID, L_XOT)));
                    break;
            }

            // Verify memory if specific to OT
            switch (memory)
            {
                // No Memory
                case 0: // SWSH trades don't set HT memories immediately, which is hilarious.
                    data.AddLine(Get(LMemoryMissingOT, memoryGen == 8 ? Severity.Fishy : Severity.Invalid));
                    VerifyOTMemoryIs(data, 0, 0, 0, 0);
                    return;

                // {0} hatched from an Egg and saw {1} for the first time at... {2}. {4} that {3}.
                case 2 when !Info.EncounterMatch.EggEncounter:
                    data.AddLine(GetInvalid(string.Format(LMemoryArgBadHatch, L_XOT)));
                    break;

                // {0} became {1}’s friend when it arrived via Link Trade at... {2}. {4} that {3}.
                case 4 when pkm.Gen6:
                    data.AddLine(GetInvalid(string.Format(LMemoryArgBadOTEgg, L_XOT)));
                    return;

                // {0} went to the Pokémon Center in {2} with {1} and had its tired body healed there. {4} that {3}.
                case 6 when memoryGen == 6 && !Memories.GetHasPokeCenterLocation((GameVersion)pkm.Version, pkm.OT_TextVar):
                    data.AddLine(GetInvalid(string.Format(LMemoryArgBadLocation, L_XOT)));
                    return;
                case 6 when memoryGen == 8 && pkm.OT_TextVar != 0:
                    data.AddLine(Get(string.Format(LMemoryArgBadLocation, L_XOT), ParseSettings.Gen8MemoryLocationTextVariable));
                    return;

                // {0} was with {1} when {1} caught {2}. {4} that {3}.
                case 14:
                    var result = GetCanBeCaptured(pkm.OT_TextVar, Info.Generation, (GameVersion)pkm.Version) // Any Game in the Handling Trainer's generation
                        ? GetValid(string.Format(LMemoryArgSpecies, L_XOT))
                        : GetInvalid(string.Format(LMemoryArgBadSpecies, L_XOT));
                    data.AddLine(result);
                    return;
            }

            data.AddLine(VerifyCommonMemory(pkm, 0, Info.Generation));
        }

        private static bool CanHaveMemory(PKM pkm, int origin, int memory)
        {
            if (pkm.GG) // LGPE never assigns memories
                return false;

            if ((pkm.VC || pkm.Gen7) && memory != 4) // Generation 7 - Trade memory or nothing
                return memory == 4;

            if (origin < 6) // NDS/3DS
                return false;

            if (pkm.IsEgg) // Eggs should not have memories
                return false;

            return true;
        }

        private void VerifyHTMemory(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var Info = data.Info;

            var memory = pkm.HT_Memory;

            if (pkm.IsUntraded)
            {
                if (memory == 4 && WasTradedSWSHEgg(pkm))
                {
                    // Untraded link trade eggs in Gen8 have HT link trade memory applied erroneously.
                    // Verify the link trade memory later.
                }
                else
                {
                    VerifyHTMemoryNone(data, pkm);
                    return;
                }
            }

            if (pkm.Format == 7)
            {
                VerifyHTMemoryTransferTo7(data, pkm, Info);
                return;
            }

            var memoryGen = pkm.Format >= 8 ? 8 : 6;

            // Bounds checking
            switch (memoryGen)
            {
                case 6 when memory > Memories.MAX_MEMORY_ID_AO:
                case 8 when memory > Memories.MAX_MEMORY_ID_SWSH || Memories.Memory_NotSWSH.Contains(memory):
                    data.AddLine(GetInvalid(string.Format(LMemoryArgBadID, L_XHT)));
                    break;
            }

            // Verify memory if specific to HT
            switch (memory)
            {
                // No Memory
                case 0: // SWSH trades don't set HT memories immediately, which is hilarious.
                    data.AddLine(Get(LMemoryMissingHT, memoryGen == 8 ? Severity.Fishy : Severity.Invalid));
                    VerifyHTMemoryNone(data, pkm);
                    return;

                // {0} met {1} at... {2}. {1} threw a Poké Ball at it, and they started to travel together. {4} that {3}.
                case 1:
                    data.AddLine(GetInvalid(string.Format(LMemoryArgBadCatch, L_XHT)));
                    return;

                // {0} hatched from an Egg and saw {1} for the first time at... {2}. {4} that {3}.
                case 2:
                    data.AddLine(GetInvalid(string.Format(LMemoryArgBadHatch, L_XHT)));
                    return;

                // {0} went to the Pokémon Center in {2} with {1} and had its tired body healed there. {4} that {3}.
                case 6 when memoryGen == 6 && !Memories.GetHasPokeCenterLocation(GameVersion.Gen6, pkm.HT_TextVar):
                    data.AddLine(GetInvalid(string.Format(LMemoryArgBadLocation, L_XOT)));
                    return;
                case 6 when memoryGen == 8 && pkm.HT_TextVar != 0:
                    data.AddLine(Get(string.Format(LMemoryArgBadLocation, L_XOT), ParseSettings.Gen8MemoryLocationTextVariable));
                    return;

                // {0} was with {1} when {1} caught {2}. {4} that {3}.
                case 14:
                    var result = GetCanBeCaptured(pkm.HT_TextVar, memoryGen, GameVersion.Any) // Any Game in the Handling Trainer's generation
                        ? GetValid(string.Format(LMemoryArgSpecies, L_XHT))
                        : GetInvalid(string.Format(LMemoryArgBadSpecies, L_XHT));
                    data.AddLine(result);
                    return;
            }

            var commonResult = VerifyCommonMemory(pkm, 1, memoryGen);
            data.AddLine(commonResult);
        }

        private static bool WasTradedSWSHEgg(PKM pkm) => pkm.Gen8 && (pkm.WasTradedEgg || pkm.WasBredEgg);

        private void VerifyHTMemoryTransferTo7(LegalityAnalysis data, PKM pkm, LegalInfo Info)
        {
            // Bank Transfer adds in the Link Trade Memory.
            // Trading 7<->7 between games (not Bank) clears this data.
            if (pkm.HT_Memory == 0)
            {
                VerifyHTMemoryNone(data, pkm);
                return;
            }

            // Transfer 6->7 & withdraw to same HT => keeps past gen memory
            // Don't require link trade memory for these past gen cases
            int gen = Info.Generation;
            if (3 <= gen && gen < 7 && pkm.CurrentHandler == 1)
                return;

            if (pkm.HT_Memory != 4)
                data.AddLine(Severity.Invalid, LMemoryIndexLinkHT, CheckIdentifier.Memory);
            if (pkm.HT_TextVar != 0)
                data.AddLine(Severity.Invalid, LMemoryIndexArgHT, CheckIdentifier.Memory);
            if (pkm.HT_Intensity != 1)
                data.AddLine(Severity.Invalid, LMemoryIndexIntensityHT1, CheckIdentifier.Memory);
            if (pkm.HT_Feeling > 10)
                data.AddLine(Severity.Invalid, LMemoryIndexFeelHT09, CheckIdentifier.Memory);
        }

        private static bool GetCanBeCaptured(int species, int gen, GameVersion version)
        {
            switch (gen)
            {
                // Capture Memory only obtainable via Gen 6.
                case 6:
                    switch (version)
                    {
                        case GameVersion.Any:
                            return Legal.FriendSafari.Contains(species)
                                   || GetCanBeCaptured(species, SlotsX, StaticX)
                                   || GetCanBeCaptured(species, SlotsY, StaticY)
                                   || GetCanBeCaptured(species, SlotsA, StaticA)
                                   || GetCanBeCaptured(species, SlotsO, StaticO);
                        case GameVersion.X:
                            return Legal.FriendSafari.Contains(species)
                                   || GetCanBeCaptured(species, SlotsX, StaticX);
                        case GameVersion.Y:
                            return Legal.FriendSafari.Contains(species)
                                   || GetCanBeCaptured(species, SlotsY, StaticY);

                        case GameVersion.AS:
                            return GetCanBeCaptured(species, SlotsA, StaticA);
                        case GameVersion.OR:
                            return GetCanBeCaptured(species, SlotsO, StaticO);
                    }
                    break;

                case 8:
                {
                    switch (version)
                    {
                        case GameVersion.Any:
                            return GetCanBeCaptured(species, SlotsSW.Concat(SlotsSH), StaticSW.Concat(StaticSH));
                        case GameVersion.SW:
                            return GetCanBeCaptured(species, SlotsSW, StaticSW);
                        case GameVersion.SH:
                            return GetCanBeCaptured(species, SlotsSH, StaticSH);
                    }
                    break;
                }
            }
            return false;
        }

        private static bool GetCanBeCaptured(int species, IEnumerable<EncounterArea> area, IEnumerable<EncounterStatic> statics)
        {
            if (area.Any(loc => loc.Slots.Any(slot => slot.Species == species)))
                return true;
            if (statics.Any(enc => enc.Species == species && !enc.Gift))
                return true;
            return false;
        }
    }
}
