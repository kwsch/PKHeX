namespace PKHeX.Core
{
    /// <summary>
    /// Coordinate / Index Relationship for a Generation 2 Headbutt Tree
    /// </summary>
    internal readonly struct TreeCoordinates
    {
#if DEBUG
        private readonly int X;
        private readonly int Y;
#endif
        public readonly byte Index;

        public TreeCoordinates(in int x, in int y)
        {
#if DEBUG
            X = x;
            Y = y;
#endif
            Index = (byte)(((x * y) + x + y) / 5 % 10);
        }

#if DEBUG
        public override string ToString() => $"{Index} @ ({X:D2},{Y:D2})";
#endif
    }
}
