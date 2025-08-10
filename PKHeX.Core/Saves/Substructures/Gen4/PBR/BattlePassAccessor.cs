using System;

namespace PKHeX.Core;

/// <summary>
/// Retrieves <see cref="BattlePass"/> from a <see cref="SAV4BR"/>.
/// </summary>
/// <param name="sav">The save file to read from.</param>
public sealed class BattlePassAccessor(SAV4BR sav)
{
    private const int PassBank1 = 0x13858;
    private const int PassBank2 = 0x58DB8;
    private const int PassBank3 = 0x5F578;

    public int PASS_COUNT_CUSTOM => sav.Japanese ? 32 : 37;
    public const int PASS_COUNT_RENTAL = 6;
    public const int PASS_COUNT_FRIEND = 61;
    public int PASS_COUNT_DOWNLOAD => sav.Japanese ? 30 : 25;
    public const int PASS_COUNT_OTHER1 = 30;
    public const int PASS_COUNT_OTHER2 = 12;
    public const int PASS_COUNT_OTHER3 = 16;

    public const int PASS_START_CUSTOM = 0;
    public int PASS_START_RENTAL => PASS_START_CUSTOM + PASS_COUNT_CUSTOM;
    public int PASS_START_FRIEND => PASS_START_RENTAL + PASS_COUNT_RENTAL;
    public int PASS_START_DOWNLOAD => PASS_START_FRIEND + PASS_COUNT_FRIEND;
    public const int PASS_START_OTHER1 = 129; // PASS_START_DOWNLOAD + PASS_COUNT_DOWNLOAD
    public const int PASS_START_OTHER2 = PASS_START_OTHER1 + PASS_COUNT_OTHER1;
    public const int PASS_START_OTHER3 = PASS_START_OTHER2 + PASS_COUNT_OTHER2;
    public const int PASS_COUNT = PASS_START_OTHER3 + PASS_COUNT_OTHER3;

    public BattlePass this[int index]
    {
        get => new(GetPassMemory(index));
        set => value.Data.CopyTo(GetPassMemory(index).Span);
    }

    private Memory<byte> GetPassMemory(int index)
    {
        int ofs = index switch
        {
            < PASS_START_OTHER2 => PassBank1 + (index * BattlePass.Size),
            < PASS_START_OTHER3 => PassBank2 + ((index - PASS_START_OTHER2) * BattlePass.Size),
            < PASS_COUNT => PassBank3 + ((index - PASS_START_OTHER3) * BattlePass.Size),
            _ => throw new ArgumentOutOfRangeException(nameof(index)),
        };
        return sav.Buffer.Slice(ofs, BattlePass.Size);
    }

    public BattlePassType GetPassType(int index) => index switch
    {
        _ when index < PASS_START_RENTAL => BattlePassType.Custom,
        _ when index < PASS_START_FRIEND => BattlePassType.Rental,
        _ when index < PASS_START_DOWNLOAD => BattlePassType.Friend,
        < PASS_START_OTHER1 => BattlePassType.Download,
        < PASS_START_OTHER2 => BattlePassType.Other1,
        < PASS_START_OTHER3 => BattlePassType.Other2,
        < PASS_COUNT => BattlePassType.Other3,
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };

    public void Swap(int index, int other)
    {
        Span<byte> temp = stackalloc byte[BattlePass.Size];
        var pass = GetPassMemory(index);
        var otherPass = GetPassMemory(other);
        pass.Span.CopyTo(temp);
        otherPass.CopyTo(pass);
        temp.CopyTo(otherPass.Span);
    }

    public void Delete(int index)
    {
        var type = GetPassType(index);
        switch (type)
        {
            case BattlePassType.Custom:
                // Preserve design and whether it's been unlocked
                var pass = this[index];
                int passDesign = pass.PassDesign;
                bool unlocked = pass.Available;

                pass.Data.Clear();
                pass.Unknown1EC = -1;
                pass.PassDesign = passDesign;
                pass.Available = unlocked;
                pass.Language = ((LanguageID)sav.Language).ToBattlePassLanguage();
                break;
            case BattlePassType.Rental:
                // Can't be deleted
                break;
            default:
                // All zeroes in new game
                GetPassMemory(index).Span.Clear();
                break;
        }

        // Shift up, moving the blank pass to the first empty slot
        int max;
        switch (type)
        {
            case BattlePassType.Custom:
                max = PASS_START_RENTAL;
                break;
            case BattlePassType.Friend:
                max = PASS_START_DOWNLOAD;
                break;
            case BattlePassType.Download:
                max = PASS_START_OTHER1;
                break;
            default:
                // No shift needed
                return;
        }
        for (int i = index; i < max - 1 && this[i + 1].Issued; i++)
            Swap(i, i + 1);
    }

    public void UnlockAllCustomPasses()
    {
        for (int i = PASS_START_CUSTOM; i < PASS_START_RENTAL; i++)
        {
            var pdata = this[i];
            pdata.Available = true;
        }
    }

    public void UnlockAllRentalPasses()
    {
        for (int i = PASS_START_RENTAL; i < PASS_START_FRIEND; i++)
        {
            var pdata = this[i];
            pdata.Available = pdata.Issued = true;
        }
    }

    public void CopyChangesFrom(BattlePassAccessor other)
    {
        for (int i = PASS_START_CUSTOM; i < PASS_COUNT; i++)
            other.GetPassMemory(i).CopyTo(GetPassMemory(i));
        sav.State.Edited = true;
    }
}
