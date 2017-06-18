using System.Collections.Generic;

namespace PKHeX.Core
{
    public class LegalInfo
    {
        /// <summary>The <see cref="PKM"/> object used for comparisons.</summary>
        private readonly PKM pkm;

        /// <summary>The generation of games the PKM originated from.</summary>
        public int Generation { get; set; }

        /// <summary> The Game the PPKM originated from.</summary>
        public GameVersion Game { get; set; }

        /// <summary>The matched Encounter details for the <see cref="PKM"/>. </summary>
        public IEncounterable EncounterMatch
        {
            get => _match;
            set
            {
                if (EncounterMatch != null && (value.LevelMin != EncounterMatch.LevelMin || value.Species != EncounterMatch.Species))
                    _evochains = null;
                _match = value;
                Parse.Clear();
            }
        }

        public int[] RelearnBase { get; set; }

        public readonly List<CheckResult> Parse = new List<CheckResult>();

        public CheckResult[] Relearn { get; set; } = new CheckResult[4];
        public CheckMoveResult[] Moves { get; set; } = new CheckMoveResult[4];

        public DexLevel[][] EvoChainsAllGens => _evochains ?? (_evochains = Legal.GetEvolutionChainsAllGens(pkm, EncounterMatch));
        public ValidEncounterMoves EncounterMoves { get; set; }

        private DexLevel[][] _evochains;
        private IEncounterable _match;
        public PIDIV PIDIV { get; set; }
        public bool PIDIVMatches { get; set; } = true;

        public LegalInfo(PKM pk)
        {
            pkm = pk;
            Game = (GameVersion) pkm.Version;
            Generation = pkm.GenNumber;
        }
    }
}
