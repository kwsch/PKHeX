namespace PKHeX.Core
{
    /// <summary>
    /// Interface for Accessing named blocks within a Generation 6 save file.
    /// </summary>
    public interface ISaveBlock6AO : ISaveBlock6Main
    {
        Misc6AO Misc { get; }
        Zukan6AO Zukan { get; }
        SecretBase6Block SecretBase { get; }
    }
}
