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
    PKM ConvertToPKM(PKH pkh);
}

public static class GameDataSideExtensions
{
    public static void CopyTo(this IGameDataSide data, PKM pk)
    {
        pk.Move1 = data.Move1; pk.Move1_PP = data.Move1_PP; pk.Move1_PPUps = data.Move1_PPUps; pk.RelearnMove1 = data.RelearnMove1;
        pk.Move2 = data.Move2; pk.Move2_PP = data.Move1_PP; pk.Move2_PPUps = data.Move2_PPUps; pk.RelearnMove2 = data.RelearnMove2;
        pk.Move3 = data.Move3; pk.Move3_PP = data.Move1_PP; pk.Move3_PPUps = data.Move3_PPUps; pk.RelearnMove3 = data.RelearnMove3;
        pk.Move4 = data.Move4; pk.Move4_PP = data.Move1_PP; pk.Move4_PPUps = data.Move4_PPUps; pk.RelearnMove4 = data.RelearnMove4;
        pk.Ball = data.Ball;
        pk.Met_Location = data.Met_Location;
        pk.Egg_Location = data.Egg_Location;
    }
}
