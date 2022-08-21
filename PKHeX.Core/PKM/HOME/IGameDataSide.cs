namespace PKHeX.Core;

/// <summary>
/// Common properties stored by HOME's side game data formats.
/// </summary>
public interface IGameDataSide
{
    int Move1 { get; set; } int Move1_PP { get; set; } int Move1_PPUps { get; set; } int RelearnMove1 { get; set; }
    int Move2 { get; set; } int Move2_PP { get; set; } int Move2_PPUps { get; set; } int RelearnMove2 { get; set; }
    int Move3 { get; set; } int Move3_PP { get; set; } int Move3_PPUps { get; set; } int RelearnMove3 { get; set; }
    int Move4 { get; set; } int Move4_PP { get; set; } int Move4_PPUps { get; set; } int RelearnMove4 { get; set; }
    int Ball { get; set; }
    int Met_Location { get; set; }
    int Egg_Location { get; set; }

    /// <summary>
    /// Gets the personal info for the input arguments.
    /// </summary>
    PersonalInfo GetPersonalInfo(int species, int form);

    /// <summary>
    /// Converts the data to a <see cref="PKM"/>.
    /// </summary>
    /// <param name="pkh">HOME entity</param>
    PKM ConvertToPKM(PKH pkh);
}

public static class GameDataSideExtensions
{
    /// <summary>
    /// Copies the shared properties into a destination.
    /// </summary>
    /// <param name="data">Source side game data</param>
    /// <param name="pk">Destination entity</param>
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
