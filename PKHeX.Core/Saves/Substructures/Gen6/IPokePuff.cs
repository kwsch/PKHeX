namespace PKHeX.Core
{
    public interface IPokePuff
    {
        Puff6 PuffBlock { get; }
    }
    public interface IOPower
    {
        OPower6 OPowerBlock { get; }
    }

    public interface ILink
    {
        byte[] LinkBlock { get; set; }
    }
}