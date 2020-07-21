namespace PKHeX.Core
{
    public interface ISaveBlock6XY : ISaveBlock6Main
    {
        Misc6XY Misc { get; }
        Zukan6XY Zukan { get; }
        Fashion6XY Fashion { get; }
    }
}