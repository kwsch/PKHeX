namespace PKHeX.Core
{
    public interface IGen3Joyful
    {
        ushort JoyfulJumpInRow { get; set; }
        ushort JoyfulJump5InRow { get; set; }
        ushort JoyfulJumpGamesMaxPlayers { get; set; }
        uint JoyfulJumpScore { get; set; }

        uint JoyfulBerriesScore { get; set; }
        ushort JoyfulBerriesInRow { get; set; }
        ushort JoyfulBerries5InRow { get; set; }
    }
}
