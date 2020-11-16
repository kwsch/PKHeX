namespace PKHeX.Core
{
    public interface IPogoSlot
    {
        Shiny Shiny { get; }
        PogoType Type { get; }
    }
}
