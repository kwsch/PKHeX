namespace PKHeX.Core
{
    public static class MoveBreed
    {
        public static bool Process(int generation, int species, int form, GameVersion version, int[] moves)
        {
            _  = Process(generation, species, form, version, moves, out var valid);
            return valid;
        }

        public static object Process(int generation, int species, int form, GameVersion version, int[] moves, out bool valid) => generation switch
        {
            2 => MoveBreed2.Validate(species, version, moves, out valid),
            3 => MoveBreed3.Validate(species, version, moves, out valid),
            4 => MoveBreed4.Validate(species, version, moves, out valid),
            5 => MoveBreed5.Validate(species, version, moves, out valid),
            _ => MoveBreed6.Validate(generation, species, form, version, moves, out valid),
        };
    }
}
