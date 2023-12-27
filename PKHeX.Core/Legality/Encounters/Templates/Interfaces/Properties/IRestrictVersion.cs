namespace PKHeX.Core;

public interface IRestrictVersion
{
    bool CanBeReceivedByVersion(int version);
}
