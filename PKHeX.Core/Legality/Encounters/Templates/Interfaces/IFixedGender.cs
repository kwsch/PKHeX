namespace PKHeX.Core;

public interface IFixedGender
{
    sbyte Gender { get; }
    bool IsFixedGender => Gender != -1;
}
