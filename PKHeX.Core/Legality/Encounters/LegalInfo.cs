using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public class LegalInfo
    {
        /// <summary>The <see cref="PKM"/> object used for comparisons.</summary>
        private readonly PKM pkm;

        /// <summary>The generation of games the <see cref="PKM"/> originated from.</summary>
        public int Generation { get; set; }

        /// <summary>The Game the <see cref="PKM"/> originated from.</summary>
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
        private IEncounterable _match;

        /// <summary>Indicates whether or not the <see cref="PKM"/> originated from <see cref="GameVersion.XD"/>.</summary>
        public bool WasXD => pkm?.Version == 15 && EncounterMatch != null && !Encounters3.Encounter_Colo.Contains(EncounterMatch);

        /// <summary>Base Relearn Moves for the <see cref="EncounterMatch"/>.</summary>
        public int[] RelearnBase { get; set; }

        /// <summary>Top level Legality Check result list for the <see cref="EncounterMatch"/>.</summary>
        public readonly List<CheckResult> Parse = new List<CheckResult>();

        public CheckResult[] Relearn { get; set; } = new CheckResult[4];
        public CheckMoveResult[] Moves { get; set; } = new CheckMoveResult[4];

        public ValidEncounterMoves EncounterMoves { get; set; }
        public DexLevel[][] EvoChainsAllGens => _evochains ?? (_evochains = Legal.GetEvolutionChainsAllGens(pkm, EncounterMatch));
        private DexLevel[][] _evochains;

        /// <summary><see cref="RNG"/> related information that generated the <see cref="PKM.PID"/>/<see cref="PKM.IVs"/> value(s).</summary>
        public PIDIV PIDIV { get; set; }

        /// <summary>Indicates whether or not the <see cref="PIDIV"/> can originate from the <see cref="EncounterMatch"/>.</summary>
        /// <remarks>This boolean is true until all valid <see cref="PIDIV"/> encounters are tested, at which time it is false.</remarks>
        public bool PIDIVMatches { get; set; } = true;

        public LegalInfo(PKM pk)
        {
            pkm = pk;

            // Store repeatedly accessed values
            Game = (GameVersion)pkm.Version;
            Generation = pkm.GenNumber;
        }

        /// <summary>List of all near-matches that were rejected for a given reason.</summary>
        public List<EncounterRejected> InvalidMatches;
        internal void Reject(CheckResult c)
        {
            if (InvalidMatches == null)
                InvalidMatches = new List<EncounterRejected>();
            InvalidMatches.Add(new EncounterRejected(EncounterMatch, c));
        }
    }
}
