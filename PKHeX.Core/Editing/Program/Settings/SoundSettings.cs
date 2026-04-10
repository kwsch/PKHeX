namespace PKHeX.Core;

public sealed class SoundSettings
{
    [LocalizedDescription("Play Sound when loading a new Save File")]
    public bool PlaySoundSAVLoad { get; set; } = true;

    [LocalizedDescription("Play Sound when popping up Legality Report")]
    public bool PlaySoundLegalityCheck { get; set; } = true;

    [LocalizedDescription("Play Sound when performing any other action that would be reasonable to sound alert.")]
    public bool PlaySoundOther { get; set; } = true;
}
