namespace PKHeX.Core
{
    public static class MoveBreed
    {
        public static bool Process(int generation, int species, int form, GameVersion version, int[] moves) => generation switch
        {
            2 => MoveBreed2.Process(species, version, moves),
            3 => MoveBreed3.Process(species, version, moves),
            4 => MoveBreed4.Process(species, version, moves),
            5 => MoveBreed5.Process(species, version, moves),
            _ => MoveBreed6.Process(generation, species, form, version, moves),
        };
    }
}
