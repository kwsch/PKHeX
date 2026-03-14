using System.ComponentModel.DataAnnotations;

namespace PKHeX.Core;

public interface ISaveBlock3SmallExpansion : ISaveBlock3Small
{
    ushort JoyfulJumpInRow { get; set; }
    ushort JoyfulJump5InRow { get; set; }
    ushort JoyfulJumpGamesMaxPlayers { get; set; }
    uint JoyfulJumpScore { get; set; }

    uint JoyfulBerriesScore { get; set; }
    ushort JoyfulBerriesInRow { get; set; }
    ushort JoyfulBerries5InRow { get; set; }

    ushort GetBerryPressSpeed([Range(0, 3)] int index);
    void SetBerryPressSpeed([Range(0, 3)] int index, ushort value);
    uint BerryPowder { get; set; }
    new uint SecurityKey { get; set; }
    uint LinkFlags { get; set; }
}
