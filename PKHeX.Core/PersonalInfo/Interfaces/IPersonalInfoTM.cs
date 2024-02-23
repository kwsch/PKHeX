namespace PKHeX.Core;

public interface IPersonalInfoTM
{
    bool GetIsLearnTM(int index);
    void SetIsLearnTM(int index, bool value);
}
