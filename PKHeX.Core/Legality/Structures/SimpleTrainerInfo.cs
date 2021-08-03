namespace PKHeX.Core
{
    public sealed class SimpleTrainerInfo : ITrainerInfo, IRegionOrigin
    {
        public string OT { get; set; } = "PKHeX";
        public int TID { get; set; } = 12345;
        public int SID { get; set; } = 54321;
        public int Gender { get; set; } = 0;
        public int Language { get; set; } = (int)LanguageID.English;

        // IRegionOrigin for generation 6/7
        public byte ConsoleRegion { get; set; } = 1; // North America
        public byte Region { get; set; } = 7; // California
        public byte Country { get; set; } = 49; // USA

        public int Game { get; }
        public int Generation { get; set; } = PKX.Generation;

        public SimpleTrainerInfo(GameVersion game = GameVersion.SW)
        {
            Game = (int) game;
            if (GameVersion.Gen7b.Contains(game) || game.GetGeneration() >= 8)
                ConsoleRegion = Region = Country = 0;
        }
    }
}
