namespace PKHeX.Core
{
    public class EncounterEgg : IEncounterable
    {
        public int Species { get; set; }
        public string Name => "Egg";
        public bool EggEncounter => true;
        public int LevelMin => Level;
        public int LevelMax => Level;
        public int Level;

        public GameVersion Game;
        public bool SplitBreed;
    }
}
