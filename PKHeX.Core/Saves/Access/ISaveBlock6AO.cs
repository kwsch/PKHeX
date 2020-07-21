namespace PKHeX.Core
{
    public interface ISaveBlock6AO : ISaveBlock6Main
    {
        Misc6AO Misc { get; }
        Zukan6AO Zukan { get; }
    }
}