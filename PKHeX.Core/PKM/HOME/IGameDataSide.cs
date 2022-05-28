namespace PKHeX.Core;

public interface IGameDataSide
{
    int Move1 { get; set; } int Move1_PP { get; set; } int Move1_PPUps { get; set; } int RelearnMove1 { get; set; }
    int Move2 { get; set; } int Move2_PP { get; set; } int Move2_PPUps { get; set; } int RelearnMove2 { get; set; }
    int Move3 { get; set; } int Move3_PP { get; set; } int Move3_PPUps { get; set; } int RelearnMove3 { get; set; }
    int Move4 { get; set; } int Move4_PP { get; set; } int Move4_PPUps { get; set; } int RelearnMove4 { get; set; }
    int Ball { get; set; }
    int Met_Location { get; set; }
    int Egg_Location { get; set; }

    PersonalInfo GetPersonalInfo(int species, int form);
}
