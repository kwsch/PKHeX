using System;

namespace PKHeX.Core;

/// <summary>
/// Inter-format manipulation API for <see cref="IGameDataSide"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IGameDataSide<T> : IGameDataSide where T : PKM, new()
{
    /// <summary>
    /// Copies the data from the current instance into the input <see cref="PKM"/>.
    /// </summary>
    /// <param name="pk">Entity to copy to</param>
    /// <param name="pkh">Overall HOME data object</param>
    void CopyTo(T pk, PKH pkh);

    /// <summary>
    /// Copies the data from the input <see cref="PKM"/> into the current instance.
    /// </summary>
    /// <param name="pk">Entity to copy from</param>
    /// <param name="pkh">Overall HOME data object</param>
    void CopyFrom(T pk, PKH pkh);

    /// <summary>
    /// Converts the data to a <see cref="PKM"/>.
    /// </summary>
    /// <param name="pkh">Overall HOME data object</param>
    T ConvertToPKM(PKH pkh);

    /// <summary>
    /// Initializes the instance with data from the input <see cref="PKM"/>.
    /// </summary>
    /// <param name="other">Other game data to initialize from</param>
    /// <param name="pkh">Overall HOME data object</param>
    void InitializeFrom(IGameDataSide other, PKH pkh);
}

/// <summary>
/// Common properties stored by HOME's side game data formats.
/// </summary>
public interface IGameDataSide
{
    ushort Move1 { get; set; } int Move1_PP { get; set; } int Move1_PPUps { get; set; } ushort RelearnMove1 { get; set; }
    ushort Move2 { get; set; } int Move2_PP { get; set; } int Move2_PPUps { get; set; } ushort RelearnMove2 { get; set; }
    ushort Move3 { get; set; } int Move3_PP { get; set; } int Move3_PPUps { get; set; } ushort RelearnMove3 { get; set; }
    ushort Move4 { get; set; } int Move4_PP { get; set; } int Move4_PPUps { get; set; } ushort RelearnMove4 { get; set; }
    byte Ball { get; set; }
    ushort MetLocation { get; set; }
    ushort EggLocation { get; set; }

    /// <summary>
    /// Gets the personal info for the input arguments.
    /// </summary>
    PersonalInfo GetPersonalInfo(ushort species, byte form);
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
        pk.Move2 = data.Move2; pk.Move2_PP = data.Move2_PP; pk.Move2_PPUps = data.Move2_PPUps; pk.RelearnMove2 = data.RelearnMove2;
        pk.Move3 = data.Move3; pk.Move3_PP = data.Move3_PP; pk.Move3_PPUps = data.Move3_PPUps; pk.RelearnMove3 = data.RelearnMove3;
        pk.Move4 = data.Move4; pk.Move4_PP = data.Move4_PP; pk.Move4_PPUps = data.Move4_PPUps; pk.RelearnMove4 = data.RelearnMove4;
        pk.Ball = data.Ball;
        pk.MetLocation = data.MetLocation;
        pk.EggLocation = data.EggLocation;
    }

    /// <summary>
    /// Copies the shared properties into a destination.
    /// </summary>
    /// <param name="data">Source side game data</param>
    /// <param name="pk">Destination entity</param>
    public static void CopyTo(this IGameDataSide data, IGameDataSide pk)
    {
        pk.Move1 = data.Move1; pk.Move1_PP = data.Move1_PP; pk.Move1_PPUps = data.Move1_PPUps; pk.RelearnMove1 = data.RelearnMove1;
        pk.Move2 = data.Move2; pk.Move2_PP = data.Move2_PP; pk.Move2_PPUps = data.Move2_PPUps; pk.RelearnMove2 = data.RelearnMove2;
        pk.Move3 = data.Move3; pk.Move3_PP = data.Move3_PP; pk.Move3_PPUps = data.Move3_PPUps; pk.RelearnMove3 = data.RelearnMove3;
        pk.Move4 = data.Move4; pk.Move4_PP = data.Move4_PP; pk.Move4_PPUps = data.Move4_PPUps; pk.RelearnMove4 = data.RelearnMove4;
        pk.Ball = data.Ball;
        pk.MetLocation = data.MetLocation;
        pk.EggLocation = data.EggLocation;
    }

    /// <summary>
    /// Copies the shared properties into a destination.
    /// </summary>
    /// <param name="data">Source side game data</param>
    /// <param name="pk">Destination entity</param>
    public static void CopyFrom(this IGameDataSide data, PKM pk)
    {
        data.Move1 = pk.Move1; data.Move1_PP = pk.Move1_PP; data.Move1_PPUps = pk.Move1_PPUps; data.RelearnMove1 = pk.RelearnMove1;
        data.Move2 = pk.Move2; data.Move2_PP = pk.Move2_PP; data.Move2_PPUps = pk.Move2_PPUps; data.RelearnMove2 = pk.RelearnMove2;
        data.Move3 = pk.Move3; data.Move3_PP = pk.Move3_PP; data.Move3_PPUps = pk.Move3_PPUps; data.RelearnMove3 = pk.RelearnMove3;
        data.Move4 = pk.Move4; data.Move4_PP = pk.Move4_PP; data.Move4_PPUps = pk.Move4_PPUps; data.RelearnMove4 = pk.RelearnMove4;
        data.Ball = pk.Ball;
        data.MetLocation = pk.MetLocation;
        data.EggLocation = pk.EggLocation;
    }

    /// <summary>
    /// Resets the moves using the <see cref="source"/> for the given level.
    /// </summary>
    public static void ResetMoves(this IGameDataSide data, ushort species, byte form, int level, ILearnSource source, EntityContext context)
    {
        var learn = source.GetLearnset(species, form);
        Span<ushort> moves = stackalloc ushort[4];
        learn.SetEncounterMoves(level, moves);
        data.Move1 = moves[0];
        data.Move2 = moves[1];
        data.Move3 = moves[2];
        data.Move4 = moves[3];
        data.Move1_PP = MoveInfo.GetPP(context, moves[0]);
        data.Move2_PP = MoveInfo.GetPP(context, moves[1]);
        data.Move3_PP = MoveInfo.GetPP(context, moves[2]);
        data.Move4_PP = MoveInfo.GetPP(context, moves[3]);
    }
}
