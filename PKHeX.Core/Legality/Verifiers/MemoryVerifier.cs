using System;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.MemoryPermissions;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the <see cref="IMemoryOT.OT_Memory"/>, <see cref="IMemoryHT.HT_Memory"/>, and associated values.
    /// </summary>
    public sealed class MemoryVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Memory;

        public override void Verify(LegalityAnalysis data)
        {
            VerifyOTMemory(data);
            VerifyHTMemory(data);
        }

        private CheckResult VerifyCommonMemory(PKM pkm, int handler, int gen, LegalInfo info)
        {
            var memory = MemoryVariableSet.Read((ITrainerMemories)pkm, handler);

            // Actionable HM moves
            int matchingMoveMemory = Array.IndexOf(Memories.MoveSpecificMemories[0], memory.MemoryID);
            if (matchingMoveMemory != -1)
            {
                // Gen8 has no HMs, so this memory can never exist.
                if (gen != 6 || (pkm.Species != (int)Species.Smeargle && !GetCanLearnMachineMove(pkm, Memories.MoveSpecificMemories[1][matchingMoveMemory], 6)))
                    return GetInvalid(string.Format(LMemoryArgBadMove, memory.Handler));
            }

            switch (memory.MemoryID)
            {
                // {0} saw {2} carrying {1} on its back. {4} that {3}.
                case 21 when gen != 6 || !GetCanLearnMachineMove(new PK6 {Species = memory.Variable, EXP = Experience.GetEXP(100, PersonalTable.XY.GetFormIndex(memory.Variable, 0))}, 19, 6):
                    return GetInvalid(string.Format(LMemoryArgBadMove, memory.Handler));

                case 16 or 48 when !CanKnowMove(pkm, memory, gen, info, memory.MemoryID == 16):
                    return GetInvalid(string.Format(LMemoryArgBadMove, memory.Handler));

                case 49 when memory.Variable == 0 || !GetCanRelearnMove(pkm, memory.Variable, gen, info.EvoChainsAllGens[gen]):
                    return GetInvalid(string.Format(LMemoryArgBadMove, memory.Handler));

                // Dynamaxing
                // {0} battled at {1}’s side against {2} that Dynamaxed. {4} that {3}.
                case 71 when !GetCanBeCaptured(memory.Variable, 8, handler == 0 ? (GameVersion)pkm.Version : GameVersion.Any):
                // {0} battled {2} and Dynamaxed upon {1}’s instruction. {4} that {3}.
                case 72 when !((PersonalInfoSWSH)PersonalTable.SWSH[memory.Variable]).IsPresentInGame:
                    return GetInvalid(string.Format(LMemoryArgBadSpecies, handler == 0 ? L_XOT : L_XHT));

                // Move
                // {0} studied about how to use {2} in a Box, thinking about {1}. {4} that {3}.
                // {0} practiced its cool pose for the move {2} in a Box, wishing to be praised by {1}. {4} that {3}.
                case 80 or 81 when !CanKnowMove(pkm, memory, gen, info):
                    return Get(string.Format(LMemoryArgBadMove, memory.Handler), gen == 8 ? Severity.Fishy : Severity.Invalid);

                // Species
                // {0} had a great chat about {1} with the {2} that it was in a Box with. {4} that {3}.
                // {0} became good friends with the {2} in a Box, practiced moves with it, and talked about the day that {0} would be praised by {1}. {4} that {3}.
                // {0} got in a fight with the {2} that it was in a Box with about {1}. {4} that {3}.
                case 82 or 83 or 87 when !((PersonalInfoSWSH)PersonalTable.SWSH[memory.Variable]).IsPresentInGame:
                    return GetInvalid(string.Format(LMemoryArgBadSpecies, handler == 0 ? L_XOT : L_XHT));

                // Item
                // {0} went to a Pokémon Center with {1} to buy {2}. {4} that {3}.
                case 5 when !CanBuyItem(gen, memory.Variable):
                // {1} used {2} when {0} was in trouble. {4} that {3}.
                case 15 when !CanUseItem(gen, memory.Variable, pkm.Species):
                // {0} saw {1} using {2}. {4} that {3}.
                case 26 when !CanUseItemGeneric(gen, memory.Variable):
                // {0} planted {2} with {1} and imagined a big harvest. {4} that {3}.
                case 34 when !CanPlantBerry(gen, memory.Variable):
                // {1} had {0} hold items like {2} to help it along. {4} that {3}.
                case 40 when !CanHoldItem(gen, memory.Variable):
                // {0} was excited when {1} won prizes like {2} through Loto-ID. {4} that {3}.
                case 51 when !CanWinRotoLoto(gen, memory.Variable):
                // {0} was worried if {1} was looking for the {2} that it was holding in a Box. {4} that {3}.
                // When {0} was in a Box, it thought about the reason why {1} had it hold the {2}. {4} that {3}.
                case 84 or 88 when !Legal.HeldItems_SWSH.Contains((ushort)memory.Variable) || pkm.IsEgg:
                    return GetInvalid(string.Format(LMemoryArgBadItem, memory.Handler));
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
            var pkm = (ITrainerMemories)data.pkm;
            if (pkm.OT_Memory != m)
                data.AddLine(GetInvalid(string.Format(LMemoryIndexID, L_XOT, m)));
            if (pkm.OT_Intensity != i)
                data.AddLine(GetInvalid(string.Format(LMemoryIndexIntensity, L_XOT, i)));
            if (pkm.OT_TextVar != t)
                data.AddLine(GetInvalid(string.Format(LMemoryIndexVar, L_XOT, t)));
            if (pkm.OT_Feeling != f)
                data.AddLine(GetInvalid(string.Format(LMemoryIndexFeel, L_XOT, f)));
        }

        private void VerifyHTMemoryNone(LegalityAnalysis data, ITrainerMemories pkm)
        {
            if (pkm.HT_Memory != 0 || pkm.HT_TextVar != 0 || pkm.HT_Intensity != 0 || pkm.HT_Feeling != 0)
                data.AddLine(GetInvalid(string.Format(LMemoryCleared, L_XHT)));
        }

        private void VerifyOTMemory(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var mem = (ITrainerMemories)pkm;
            var Info = data.Info;

            switch (data.EncounterMatch)
            {
                case WC6 {IsEgg: false} g when g.OTGender != 3:
                    VerifyOTMemoryIs(data, g.OT_Memory, g.OT_Intensity, g.OT_TextVar, g.OT_Feeling);
                    return;
                case WC7 {IsEgg: false} g when g.OTGender != 3:
                    VerifyOTMemoryIs(data, g.OT_Memory, g.OT_Intensity, g.OT_TextVar, g.OT_Feeling);
                    return;
                case WC8 {IsEgg: false} g when g.OTGender != 3:
                    VerifyOTMemoryIs(data, g.OT_Memory, g.OT_Intensity, g.OT_TextVar, g.OT_Feeling);
                    return;

                case IMemoryOT t when t is not MysteryGift: // Ignore Mystery Gift cases (covered above)
                    VerifyOTMemoryIs(data, t.OT_Memory, t.OT_Intensity, t.OT_TextVar, t.OT_Feeling);
                    return;
            }

            int memoryGen = Info.Generation;
            int memory = mem.OT_Memory;

            if (pkm.IsEgg)
            {
                // Traded unhatched eggs in Gen8 have OT link trade memory applied erroneously.
                // They can also have the box-inspect memory!
                if (memoryGen != 8 || !((pkm.Met_Location == Locations.LinkTrade6 && memory == 4) || memory == 85))
                {
                    VerifyOTMemoryIs(data, 0, 0, 0, 0); // empty
                    return;
                }
            }
            else if (!CanHaveMemoryForOT(pkm, memoryGen, memory))
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
                case 6 when memoryGen == 6 && !Memories.GetHasPokeCenterLocation((GameVersion)pkm.Version, mem.OT_TextVar):
                    data.AddLine(GetInvalid(string.Format(LMemoryArgBadLocation, L_XOT)));
                    return;
                case 6 when memoryGen == 8 && mem.OT_TextVar != 0:
                    data.AddLine(Get(string.Format(LMemoryArgBadLocation, L_XOT), ParseSettings.Gen8MemoryLocationTextVariable));
                    return;

                // {0} was with {1} when {1} caught {2}. {4} that {3}.
                case 14:
                    var result = GetCanBeCaptured(mem.OT_TextVar, Info.Generation, (GameVersion)pkm.Version) // Any Game in the Handling Trainer's generation
                        ? GetValid(string.Format(LMemoryArgSpecies, L_XOT))
                        : GetInvalid(string.Format(LMemoryArgBadSpecies, L_XOT));
                    data.AddLine(result);
                    return;
            }

            data.AddLine(VerifyCommonMemory(pkm, 0, Info.Generation, Info));
        }

        private static bool CanHaveMemoryForOT(PKM pkm, int origin, int memory)
        {
            switch (origin)
            {
                // Bank Memories only: Gen7 does not set memories.
                case 1 or 2 or 7 when memory != 4: // VC transfers

                // Memories don't exist
                case 7 when pkm.GG: // LGPE does not set memories.
                case 8 when pkm.GO_HOME: // HOME does not set memories.
                case 8 when pkm.Met_Location == Locations.HOME8: // HOME does not set memories.
                    return false;

                // Eggs cannot have memories
                // Cannot have memories if the OT was from a generation prior to Gen6.
                default:
                    return origin >= 6 && !pkm.IsEgg;
            }
        }

        private void VerifyHTMemory(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var mem = (ITrainerMemories)pkm;
            var Info = data.Info;

            var memory = mem.HT_Memory;

            if (pkm.IsUntraded)
            {
                if (memory == 4 && WasTradedSWSHEgg(pkm))
                {
                    // Untraded link trade eggs in Gen8 have HT link trade memory applied erroneously.
                    // Verify the link trade memory later.
                }
                else
                {
                    VerifyHTMemoryNone(data, mem);
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
                    VerifyHTMemoryNone(data, mem);
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
                case 6 when memoryGen == 6 && !Memories.GetHasPokeCenterLocation(GameVersion.Gen6, mem.HT_TextVar):
                    data.AddLine(GetInvalid(string.Format(LMemoryArgBadLocation, L_XOT)));
                    return;
                case 6 when memoryGen == 8 && mem.HT_TextVar != 0:
                    data.AddLine(Get(string.Format(LMemoryArgBadLocation, L_XOT), ParseSettings.Gen8MemoryLocationTextVariable));
                    return;

                // {0} was with {1} when {1} caught {2}. {4} that {3}.
                case 14:
                    var result = GetCanBeCaptured(mem.HT_TextVar, memoryGen, GameVersion.Any) // Any Game in the Handling Trainer's generation
                        ? GetValid(string.Format(LMemoryArgSpecies, L_XHT))
                        : GetInvalid(string.Format(LMemoryArgBadSpecies, L_XHT));
                    data.AddLine(result);
                    return;
            }

            var commonResult = VerifyCommonMemory(pkm, 1, memoryGen, Info);
            data.AddLine(commonResult);
        }

        private static bool WasTradedSWSHEgg(PKM pkm) => pkm.Gen8 && pkm.WasBredEgg;

        private void VerifyHTMemoryTransferTo7(LegalityAnalysis data, PKM pkm, LegalInfo Info)
        {
            var mem = (ITrainerMemories)pkm;
            // Bank Transfer adds in the Link Trade Memory.
            // Trading 7<->7 between games (not Bank) clears this data.
            if (mem.HT_Memory == 0)
            {
                VerifyHTMemoryNone(data, mem);
                return;
            }

            // Transfer 6->7 & withdraw to same HT => keeps past gen memory
            // Don't require link trade memory for these past gen cases
            int gen = Info.Generation;
            if (gen is >= 3 and < 7 && pkm.CurrentHandler == 1)
                return;

            if (mem.HT_Memory != 4)
                data.AddLine(Severity.Invalid, LMemoryIndexLinkHT, CheckIdentifier.Memory);
            if (mem.HT_TextVar != 0)
                data.AddLine(Severity.Invalid, LMemoryIndexArgHT, CheckIdentifier.Memory);
            if (mem.HT_Intensity != 1)
                data.AddLine(Severity.Invalid, LMemoryIndexIntensityHT1, CheckIdentifier.Memory);
            if (mem.HT_Feeling > 10)
                data.AddLine(Severity.Invalid, LMemoryIndexFeelHT09, CheckIdentifier.Memory);
        }
    }
}
