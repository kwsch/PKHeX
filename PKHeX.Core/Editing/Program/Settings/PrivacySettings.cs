namespace PKHeX.Core;

public sealed class PrivacySettings
{
    [LocalizedDescription("Hide Save File Details in Program Title")]
    public bool HideSAVDetails { get; set; }

    [LocalizedDescription("Hide Secret Details in Editors")]
    public bool HideSecretDetails { get; set; }
}
