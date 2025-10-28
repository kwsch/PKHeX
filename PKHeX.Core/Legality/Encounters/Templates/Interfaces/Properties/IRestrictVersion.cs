namespace PKHeX.Core;

public interface IRestrictVersion
{
    bool CanBeReceivedByVersion(GameVersion version);
}
