namespace PKHeX.Core
{
    public sealed class SimpleTrainerInfo : ITrainerInfo
    {
        public string OT { get; set; } = "PKHeX";
        public int TID { get; set; } = 12345;
        public int SID { get; set; } = 54321;
        public int Gender { get; set; } = 0;
        public int Language { get; set; } = (int)LanguageID.English;

        public int ConsoleRegion { get; set; } = 1; // North America
        public int SubRegion { get; set; } = 7; // California
        public int Country { get; set; } = 49; // USA

        public int Game { get; }
        public int Generation { get; set; } = PKX.Generation;

        public SimpleTrainerInfo(GameVersion game = GameVersion.SW)
        {
            Game = (int) game;
            if (GameVersion.GG.Contains(game) || game.GetGeneration() >= 8)
                ConsoleRegion = SubRegion = Country = 0;
        }
    }
}
