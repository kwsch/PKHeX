namespace PKHeX.Core
{
    /// <summary>
    /// Level Up Learn Movepool Information (Generation 1/2)
    /// </summary>
    public sealed class Learnset1 : Learnset
    {
        private Learnset1(byte[] data, ref int offset)
        {
            int end = offset; // scan for count
            while (data[end] != 0)
                end += 2;
            Count = (end - offset) / 2;
            Moves = new int[Count];
            Levels = new int[Count];
            for (int i = 0; i < Moves.Length; i++)
            {
                Levels[i] = data[offset++];
                Moves[i] = data[offset++];
            }
            ++offset;
        }

        public static Learnset[] GetArray(byte[] input, int maxSpecies)
        {
            var data = new Learnset[maxSpecies + 1];

            int offset = 0;
            for (int s = 0; s < data.Length; s++)
                data[s] = new Learnset1(input, ref offset);

            return data;
        }
    }
}