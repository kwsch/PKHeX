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

        private CheckResult VerifyCommonMemory(PKM pkm, int handler)
        {
            var memory = MemoryVariableSet.Read(pkm, handler);
            int matchingMoveMemory = Array.IndexOf(Memories.MoveSpecificMemories[0], memory.MemoryID);
            if (matchingMoveMemory != -1 && pkm.Species != 235 && !Legal.GetCanLearnMachineMove(pkm, Memories.MoveSpecificMemories[1][matchingMoveMemory], 6))
                return GetInvalid(string.Format(LMemoryArgBadMove, memory.Handler));

            switch (memory.MemoryID)
            {
                case 6 when !Memories.LocationsWithPKCenter.Contains(memory.Variable):
                    return GetInvalid(string.Format(LMemoryArgBadPokecenter, memory.Handler));

                // {0} saw {2} carrying {1} on its back. {4} that {3}.
                case 21 when !Legal.GetCanLearnMachineMove(new PK6 {Species = memory.Variable, EXP = Experience.GetEXP(100, PersonalTable.XY.GetFormeIndex(memory.Variable, 0))}, 19, 6):
                    return GetInvalid(string.Format(LMemoryArgBadMove, memory.Handler));

                case 16 when memory.Variable == 0 && !GetIsMoveKnowable(pkm, handler, memory.Variable):
                case 48 when memory.Variable == 0 && !GetIsMoveKnowable(pkm, handler, memory.Variable):
                    return GetInvalid(string.Format(LMemoryArgBadMove, memory.Handler));

                // {0} was able to remember {2} at {1}'s instruction. {4} that {3}.
                case 49 when memory.Variable == 0 && !GetIsMoveLearnable(pkm, handler, memory.Variable):
                    return GetInvalid(string.Format(LMemoryArgBadMove, memory.Handler));
            }

            if (!Memories.CanHaveIntensity(memory.MemoryID, memory.Intensity))
            {
                if (pkm.Gen6 || (pkm.Gen7 && memory.MemoryID != 0)) // todo: memory intensity checks for gen8
                  return GetInvalid(string.Format(LMemoryIndexIntensityMin, memory.Handler, Memories.GetMinimumIntensity(memory.MemoryID)));
            }

            if (memory.MemoryID != 4 && !Memories.CanHaveFeeling(memory.MemoryID, memory.Feeling))
            {
                if (pkm.Gen6 || (pkm.Gen7 && memory.MemoryID != 0)) // todo: memory feeling checks for gen8
                    return GetInvalid(string.Format(LMemoryFeelInvalid, memory.Handler));
            }

            return GetValid(string.Format(LMemoryF_0_Valid, memory.Handler));
        }

        /// <summary>
        /// Gets the Generation the Memory ID was obtained in.
        /// </summary>
        /// <param name="pkm">Entity data</param>
        /// <param name="handler">OT/HT</param>
        private static int GetMemoryObtainedGeneration(PKM pkm, int handler) => handler == 0 ? pkm.GenNumber : pkm.Format >= 8 ? 8 : 6;
        private static bool GetIsMoveKnowable(PKM pkm, int handler, int move) => Legal.GetCanKnowMove(pkm, move, GetMemoryObtainedGeneration(pkm, handler));
        private static bool GetIsMoveLearnable(PKM pkm, int handler, int move) => Legal.GetCanRelearnMove(pkm, move, GetMemoryObtainedGeneration(pkm, handler));

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

        private void VerifyOTMemory(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var Info = data.Info;
            if (Info.Generation < 6 || pkm.IsEgg)
            {
                VerifyOTMemoryIs(data, 0, 0, 0, 0); // empty
                return;
            }

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

                case IMemoryOT t when !(t is MysteryGift):
                    VerifyOTMemoryIs(data, t.OT_Memory, t.OT_Intensity, t.OT_TextVar, t.OT_Feeling);
                    return;
            }

            switch (pkm.OT_Memory)
            {
                case 2: // {0} hatched from an Egg and saw {1} for the first time at... {2}. {4} that {3}.
                    if (pkm.Egg_Location == 0)
                        data.AddLine(Severity.Invalid, string.Format(LMemoryArgBadHatch, L_XOT), CheckIdentifier.Memory);
                    break;

                case 4 when pkm.Gen6: // {0} became {1}’s friend when it arrived via Link Trade at... {2}. {4} that {3}.
                    data.AddLine(Severity.Invalid, string.Format(LMemoryArgBadOTEgg, L_XOT), CheckIdentifier.Memory);
                    return;

                case 6: // {0} went to the Pokémon Center in {2} with {1} and had its tired body healed there. {4} that {3}.
                    int matchingOriginGame = Array.IndexOf(Memories.LocationsWithPKCenter, pkm.OT_TextVar);
                    if (matchingOriginGame != -1)
                    {
                        var gameID = Memories.GetGameVersionForPokeCenterIndex(matchingOriginGame);
                        if (!gameID.Contains((GameVersion)pkm.Version))
                            data.AddLine(Severity.Invalid, string.Format(LMemoryArgBadLocation, L_XOT), CheckIdentifier.Memory);
                    }
                    data.AddLine(VerifyCommonMemory(pkm, 0));
                    return;

                case 14:
                    if (!GetCanBeCaptured(pkm.OT_TextVar, Info.Generation, (GameVersion)pkm.Version))
                        data.AddLine(Severity.Invalid, string.Format(LMemoryArgBadSpecies, L_XOT), CheckIdentifier.Memory);
                    else
                        data.AddLine(Severity.Valid, string.Format(LMemoryArgSpecies, L_XOT), CheckIdentifier.Memory);
                    return;
            }
            if (pkm.XY && Memories.Memory_NotXY.Contains(pkm.OT_Memory))
                data.AddLine(Severity.Invalid, string.Format(LMemoryArgBadID, L_XOT), CheckIdentifier.Memory);
            if (pkm.AO && Memories.Memory_NotAO.Contains(pkm.OT_Memory))
                data.AddLine(Severity.Invalid, string.Format(LMemoryArgBadID, L_XOT), CheckIdentifier.Memory);

            data.AddLine(VerifyCommonMemory(pkm, 0));
        }

        private void VerifyHTMemory(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm.Format < 6)
                return;

            var Info = data.Info;
            if (pkm.Format == 7)
            {
                /*
                *  Bank Transfer adds in the Link Trade Memory.
                *  Trading 7<->7 between games (not Bank) clears this data.
                */
                if (pkm.HT_Memory == 0)
                {
                    if (pkm.HT_TextVar != 0 || pkm.HT_Intensity != 0 || pkm.HT_Feeling != 0)
                        data.AddLine(Severity.Invalid, LMemoryCleared, CheckIdentifier.Memory);
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
                return;
            }

            switch (pkm.HT_Memory)
            {
                case 0:
                    if (string.IsNullOrEmpty(pkm.HT_Name))
                        return;
                    data.AddLine(Severity.Invalid, LMemoryMissingHT, CheckIdentifier.Memory); return;
                case 1: // {0} met {1} at... {2}. {1} threw a Poké Ball at it, and they started to travel together. {4} that {3}.
                    data.AddLine(Severity.Invalid, string.Format(LMemoryArgBadCatch, L_XHT), CheckIdentifier.Memory); return;

                case 2: // {0} hatched from an Egg and saw {1} for the first time at... {2}. {4} that {3}.
                    data.AddLine(Severity.Invalid, string.Format(LMemoryArgBadHatch, L_XHT), CheckIdentifier.Memory); return;

                case 14:
                    if (GetCanBeCaptured(pkm.HT_TextVar, 6))
                        data.AddLine(Severity.Valid, string.Format(LMemoryArgSpecies, L_XHT), CheckIdentifier.Memory);
                    else
                        data.AddLine(Severity.Invalid, string.Format(LMemoryArgBadSpecies, L_XHT), CheckIdentifier.Memory);
                    return;
            }
            data.AddLine(VerifyCommonMemory(pkm, 1));
        }

        private static bool GetCanBeCaptured(int species, int gen, GameVersion version = GameVersion.Any)
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
