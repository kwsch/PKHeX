using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Substructure of LegalInfo related to generation evolution legal checks <see cref="LegalInfo"/>.
    /// </summary>
    public class LegalEvoGenerationsInfo
    {
        public LegalEvoGenerationsInfo()
        {

        }
        public LegalEvoGenerationsInfo(LegalEvoGenerationsInfo obj)
        {
            EvoChainsAllGens = obj.EvoChainsAllGens;
            EvoGenerations = obj.EvoGenerations;
        }

        public IReadOnlyList<EvoCriteria>[]? EvoChainsAllGens
        {
            get => _evochainsallgen;
            set => _evochainsallgen = value;
        }
        private IReadOnlyList<EvoCriteria>[]? _evochainsallgen;
        
        private IEnumerable<int>? _evogenerations;
        public IEnumerable<int> EvoGenerations
        {
            get => _evogenerations ?? new List<int>();
            set => _evogenerations = value;
        }
        public CheckResult Evolution { get; set; } = new CheckResult(Severity.Invalid, LEvoInvalid, CheckIdentifier.Evolution);
        public bool ValidAbility { get; set; }
        public bool ValidMemory { get; set; }
        public CheckMoveResult[] Moves { get; internal set; } = new CheckMoveResult[4];

        private int ValidationScore
        {
            get
            {
                if (Moves.Any(z => z == null)) // No checks done
                    return 0;

                if (!Moves.All(z => z?.Valid ?? false)) //Some invalid moves
                    return 1;

                if (!ValidAbility) 
                    return 2;

                if (Evolution == null || !Evolution.Valid)
                    return 3;

                if (!ValidMemory)
                    return 4;

                return 5;
            }
        }

        public bool IsPreferredEvoGeneration(LegalEvoGenerationsInfo info) => ValidationScore >= info.ValidationScore;
    }
}
