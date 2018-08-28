namespace PKHeX.Core
{
    /// <summary>
    /// Coordinate / Index Relationship for a Generation 2 Headbutt Tree
    /// </summary>
    internal sealed class TreeCoordinates
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Index;

        public TreeCoordinates(int x, int y)
        {
            X = x;
            Y = y;
            Index = ((X * Y) + X + Y) / 5 % 10;
        }
    }
}