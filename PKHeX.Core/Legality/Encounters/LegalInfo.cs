using System.Collections.Generic;

namespace PKHeX.Core
{
    public class LegalInfo
    {
        /// <summary>The <see cref="PKM"/> object used for comparisons.</summary>
        private readonly PKM pkm;

        /// <summary>The generation of games the PKM originated from.</summary>
        public int Generation;

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

        public int[] RelearnBase;

        public readonly List<CheckResult> Parse = new List<CheckResult>();

        public CheckResult[] vRelearn = new CheckResult[4];
        public CheckResult[] vMoves = new CheckResult[4];

        public DexLevel[][] EvoChainsAllGens => _evochains ?? (_evochains = Legal.getEvolutionChainsAllGens(pkm, EncounterMatch));
        public ValidEncounterMoves EncounterMoves { get; set; }

        private DexLevel[][] _evochains;
        private IEncounterable _match;
        public PIDIV PIDIV;

        public LegalInfo(PKM pk)
        {
            pkm = pk;
            Game = (GameVersion) pkm.Version;
            Generation = pkm.GenNumber;
        }
    }
}
