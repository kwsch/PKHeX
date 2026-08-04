namespace PKHeX.Core.AutoMod;

public readonly record struct LivingDexConfig
{
    public bool IncludeForms { get; init; }
    public bool SetShiny { get; init; }
    public bool SetAlpha { get; init; }

    public LivingDexConfig(byte bitValue)
    {
        IncludeForms = (bitValue & 1) != 0;
        SetShiny = (bitValue & 2) != 0;
        SetAlpha = (bitValue & 4) != 0;
    }

    public GameVersion TransferVersion { get; init; }

    public override string ToString()
    {
        return $"TransferVersion: {TransferVersion}\nIncludeForms: {IncludeForms}\nSetShiny: {SetShiny}\nSetAlpha: {SetAlpha}\n";
    }
}
