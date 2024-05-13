using System.ComponentModel;

namespace PKHeX.Core;

/// <summary>
/// Settings object to contain all parameters that can be configured for legality checks.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class LegalitySettings
{
    public BulkAnalysisSettings Bulk { get; set; } = new();
    public FramePatternSettings FramePattern { get; set; } = new();
    public GameSpecificSettings Game { get; set; } = new();
    public HandlerSettings Handler { get; set; } = new();
    public HOMETransferSettings HOMETransfer { get; set; } = new();
    public NicknameSettings Nickname { get; set; } = new();
    public TradebackSettings Tradeback { get; set; } = new();
    public WordFilterSettings WordFilter { get; set; } = new();

    /// <summary>
    /// Adjusts the settings to disable all checks that reference the "current" save file.
    /// </summary>
    /// <remarks>Allows for quick disabling of others for use in unit tests.</remarks>
    /// <param name="checkHOME">If false, disable HOME Transfer checks. Useful for checking migration logic.</param>
    /// <param name="allowRNG">If true, allows special encounters to be nicknamed.</param>
    public void SetCheckWithoutSaveFile(bool checkHOME = true, bool allowRNG = false)
    {
        ParseSettings.ClearActiveTrainer();
        if (!checkHOME)
            HOMETransfer.Disable();
        if (allowRNG)
            Nickname.Disable();
    }
}
