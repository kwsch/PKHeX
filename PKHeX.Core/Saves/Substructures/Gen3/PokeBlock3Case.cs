namespace PKHeX.Core
{
    public sealed class PokeBlock3Case
    {
        private const int Count = 40;
        public readonly PokeBlock3[] Blocks;

        public PokeBlock3Case(byte[] data, int offset)
        {
            Blocks = new PokeBlock3[Count];
            for (int i = 0; i < Blocks.Length; i++)
                Blocks[i] = PokeBlock3.GetBlock(data, offset + (i * PokeBlock3.SIZE));
        }

        public byte[] Write()
        {
            byte[] result = new byte[Count*PokeBlock3.SIZE];
            for (int i = 0; i < Blocks.Length; i++)
                Blocks[i].SetBlock(result, i * PokeBlock3.SIZE);
            return result;
        }

        public void DeleteAll()
        {
            foreach (var b in Blocks)
                b.Delete();
        }

        public void MaximizeAll(bool createMissing = false)
        {
            foreach (var b in Blocks)
                b.Maximize(createMissing);
        }
    }
}