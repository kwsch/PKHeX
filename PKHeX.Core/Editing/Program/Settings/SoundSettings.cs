namespace PKHeX.Core;

public sealed class SoundSettings
{
    [LocalizedDescription("Play Sound when loading a new Save File")]
    public bool PlaySoundSAVLoad { get; set; } = true;
    [LocalizedDescription("Play Sound when popping up Legality Report")]
    public bool PlaySoundLegalityCheck { get; set; } = true;
}
