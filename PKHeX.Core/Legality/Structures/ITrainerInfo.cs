namespace PKHeX.Core;

/// <summary>
/// Minimal Trainer Information necessary for generating a <see cref="PKM"/>.
/// </summary>
public interface ITrainerInfo : ITrainerID32
{
    string OT { get; }
    int Gender { get; }
    int Game { get; }
    int Language { get; }

    int Generation { get; }
    EntityContext Context { get; }
}

public static class TrainerInfoExtensions
{
    public static void ApplyTo(this ITrainerInfo info, PKM pk)
    {
        pk.OT_Name = info.OT;
        pk.TID16 = info.TID16;
        pk.SID16 = pk.Format < 3 || pk.VC ? (ushort)0 : info.SID16;
        pk.OT_Gender = info.Gender;
        pk.Language = info.Language;
        pk.Version = info.Game;

        if (pk is not IRegionOrigin tr)
            return;

        if (info is not IRegionOrigin o)
            return;
        tr.Country = o.Country;
        tr.Region = o.Region;
        tr.ConsoleRegion = o.ConsoleRegion;
    }

    public static void ApplyHandlingTrainerInfo(this ITrainerInfo sav, PKM pk, bool force = false)
    {
        if (pk.Format == sav.Generation && !force)
            return;

        pk.HT_Name = sav.OT;
        pk.HT_Gender = sav.Gender;
        pk.HT_Friendship = pk.OT_Friendship;
        pk.CurrentHandler = 1;

        if (pk is PK6 pk6 && sav is IRegionOrigin o)
        {
            pk6.Geo1_Country = o.Country;
            pk6.Geo1_Region = o.Region;
            pk6.SetTradeMemoryHT6(true);
        }
        else if (pk is PK8 pk8)
        {
            pk8.SetTradeMemoryHT8();
        }
    }

    public static bool IsFromTrainer(this ITrainerInfo tr, PKM pk)
    {
        if (pk.IsEgg)
            return tr.IsFromTrainerEgg(pk);

        if (tr.Game == (int)GameVersion.Any)
            return true;

        if (!IsFromTrainerNoVersion(tr, pk))
            return false;

        return IsMatchVersion(tr, pk);
    }

    public static bool IsFromTrainerNoVersion(ITrainerInfo tr, PKM pk)
    {
        if (tr.ID32 != pk.ID32)
            return false;
        if (tr.OT != pk.OT_Name)
            return false;

        if (pk.Format == 3)
            return true; // Generation 3 does not check ot gender nor pokemon version

        if (tr.Gender != pk.OT_Gender)
        {
            if (pk.Format == 2)
                return pk is ICaughtData2 { CaughtData: 0 };
            return false;
        }
        return true;
    }

    /// <summary>
    /// Only call this if it is still an egg.
    /// </summary>
    public static bool IsFromTrainerEgg(this ITrainerInfo tr, PKM pk)
    {
        System.Diagnostics.Debug.Assert(pk.IsEgg);

        if (tr.Context != pk.Context)
            return false;
        if (tr.ID32 != pk.ID32)
            return false;
        if (tr.Gender != pk.OT_Gender)
            return false;

        if (tr.Game != pk.Version)
        {
            // PK9 does not store version for Picnic eggs.
            if (pk is PK9 { Version: 0 }) { }
            else { return false; }
        }

        if (tr.OT != pk.OT_Name)
            return false;

        return true;
    }

    private static bool IsMatchVersion(ITrainerInfo tr, PKM pk)
    {
        if (tr.Game == pk.Version)
            return true;
        if (pk.GO_LGPE)
            return tr.Game is (int)GameVersion.GP or (int)GameVersion.GE;
        return false;
    }
}
