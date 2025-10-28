namespace PKHeX.Core;

public interface IMysteryGiftFlags
{
    int MysteryGiftReceivedFlagMax { get; }
    bool GetMysteryGiftReceivedFlag(int index);
    void SetMysteryGiftReceivedFlag(int index, bool value);
    void ClearReceivedFlags();
}
