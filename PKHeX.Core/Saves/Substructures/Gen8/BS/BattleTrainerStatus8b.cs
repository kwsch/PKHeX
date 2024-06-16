using System;
using System.Linq;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Defeated Status for all trainers (Dpr.Trainer.TrainerID)
/// </summary>
/// <remarks>size: 0x1618</remarks>
public sealed class BattleTrainerStatus8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw)
{
    // Structure:
    // (bool IsWin, bool IsBattleSearcher)[707];
    private const int COUNT_TRAINER = 707;
    private const int SIZE_TRAINER = 8; // bool,bool

    public bool AnyDefeated => Enumerable.Range(0, COUNT_TRAINER).Any(GetIsWin);
    public bool AnyUndefeated => Enumerable.Range(0, COUNT_TRAINER).Any(z => !GetIsWin(z));

    /// <summary>
    /// Don't use this unless you've finished the post-game.
    /// </summary>
    public void DefeatAll()
    {
        for (int i = 0; i < COUNT_TRAINER; i++)
        {
            SetIsWin(i, true);
            SetIsBattleSearcher(i, false);
        }
    }

    /// <summary>
    /// Don't use this unless you've finished the post-game.
    /// </summary>
    public void RebattleAll()
    {
        for (int i = 0; i < COUNT_TRAINER; i++)
        {
            SetIsWin(i, false);
            SetIsBattleSearcher(i, true);
        }
    }

    private static int GetTrainerOffset(int trainer)
    {
        if ((uint)trainer >= COUNT_TRAINER)
            throw new ArgumentOutOfRangeException(nameof(trainer));
        return (trainer * SIZE_TRAINER);
    }

    public bool GetIsWin(int trainer) => ReadUInt32LittleEndian(Data[GetTrainerOffset(trainer)..]) == 1;
    public bool GetIsBattleSearcher(int trainer) => ReadUInt32LittleEndian(Data[(GetTrainerOffset(trainer) + 4)..]) == 1;
    public void SetIsWin(int trainer, bool value) => WriteUInt32LittleEndian(Data[GetTrainerOffset(trainer)..], value ? 1u : 0u);
    public void SetIsBattleSearcher(int trainer, bool value) => WriteUInt32LittleEndian(Data[(GetTrainerOffset(trainer) + 4)..], value ? 1u : 0u);
}
