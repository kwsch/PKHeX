using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Pokémon Link Encounter Data
    /// </summary>
    public class EncounterLink : IEncounterable, IRibbonSetEvent4, IMoveset, ILocation, IVersion
    {
        public int Species { get; set; }
        public int Level { get; set; }
        public int LevelMin => Level;
        public int LevelMax => Level;
        public int Location { get; set; } = 30011;
        public int Ability { get; set; } = 1;
        public int Ball { get; set; } = 4; // Pokéball
        public int[] RelearnMoves { get; set; } = new int[4];
        public bool OT { get; set; } = true; // Receiver is OT?

        public bool EggEncounter => false;
        public int EggLocation { get => 0; set { } }
        public GameVersion Version { get; set; } = GameVersion.Gen6;

        public int[] Moves { get; set; } = new int[0];

        public string Name => "Pokémon Link Gift";

        public bool RibbonClassic { get; set; } = true;

        // Unused
        public bool RibbonWishing { get; set; }
        public bool RibbonPremier { get; set; }
        public bool RibbonEvent { get; set; }
        public bool RibbonBirthday { get; set; }
        public bool RibbonSpecial { get; set; }
        public bool RibbonWorld { get; set; }
        public bool RibbonChampionWorld { get; set; }
        public bool RibbonSouvenir { get; set; }

        public PKM ConvertToPKM(ITrainerInfo SAV)
        {
            const int gen = 6;
            var version = this.GetCompatibleVersion((GameVersion)SAV.Game);
            int lang = (int)Legal.GetSafeLanguage(6, (LanguageID)SAV.Language);
            var pk = new PK6
            {
                EncryptionConstant = Util.Rand32(),
                Species = Species,
                Language = lang,
                CurrentLevel = Level,
                Version = (int)version,
                PID = Util.Rand32(),
                Nickname = PKX.GetSpeciesNameGeneration(Species, lang, gen),
                Ball = Ball,
                Met_Level = Level,
                Met_Location = Location,
                MetDate = DateTime.Today
            };

            SAV.ApplyToPKM(pk);
            pk.Language = lang;

            var moves = Moves.Length != 0 ? Moves : Legal.GetEncounterMoves(pk, Level, (GameVersion)version);
            pk.Moves = moves;
            pk.SetMaximumPPCurrent(moves);
            pk.OT_Friendship = pk.PersonalInfo.BaseFriendship;
            pk.SetRandomIVs(flawless: 3);
            pk.RefreshAbility(Ability);
            SAV.ApplyHandlingTrainerInfo(pk);
            if (RelearnMoves != null)
                pk.RelearnMoves = RelearnMoves;
            if (RibbonClassic)
                pk.RibbonClassic = true;
            if (!OT)
                SAV.ApplyHandlingTrainerInfo(pk);

            return pk;
        }
    }
}
