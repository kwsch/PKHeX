using System;

namespace PKHeX.Core;

/// <summary>
/// Minimal Trainer Information necessary for generating a <see cref="PKM"/>.
/// </summary>
public interface ITrainerInfo : ITrainerID32ReadOnly, IVersion, IGeneration
{
    string OT { get; }
    byte Gender { get; }
    new GameVersion Version { get; }
    int Language { get; }

    new byte Generation { get; }
    EntityContext Context { get; }
}

/// <summary>
/// Extension methods for <see cref="ITrainerInfo"/>.
/// </summary>
public static class TrainerInfoExtensions
{
    extension(ITrainerInfo info)
    {
        /// <summary>
        /// Copies the <see cref="ITrainerInfo"/> data to the <see cref="PKM"/> object.
        /// </summary>
        /// <param name="pk">Pokémon to copy to</param>
        public void ApplyTo(PKM pk)
        {
            pk.OriginalTrainerName = info.OT;
            pk.TID16 = info.TID16;
            pk.SID16 = pk.Format < 3 || pk.VC ? default : info.SID16;
            pk.OriginalTrainerGender = info.Gender;
            pk.Language = info.Language;
            pk.Version = info.Version;

            if (pk is not IRegionOrigin tr)
                return;

            if (info is not IRegionOriginReadOnly o)
                return;
            o.CopyRegionOrigin(tr);
        }

        /// <summary>
        /// Checks if the <see cref="ITrainerInfo"/> data matches the <see cref="PKM"/> object's Original Trainer data.
        /// </summary>
        /// <param name="pk">Pokémon to compare to</param>
        /// <returns>True if the data matches.</returns>
        public bool IsFromTrainer(PKM pk)
        {
            if (pk.IsEgg)
                return info.IsFromTrainerEgg(pk);

            if (!IsFromTrainerNoVersion(info, pk))
                return false;

            return IsMatchVersion(info, pk);
        }
    }

    /// <summary>
    /// Checks if the <see cref="ITrainerInfo"/> data matches the <see cref="PKM"/> object's Original Trainer data, ignoring the version.
    /// </summary>
    /// <param name="tr">Trainer Information</param>
    /// <param name="pk">Pokémon to compare to</param>
    /// <returns>True if the data matches.</returns>
    public static bool IsFromTrainerNoVersion(ITrainerInfo tr, PKM pk)
    {
        if (tr.ID32 != pk.ID32)
            return false;

        Span<char> ot = stackalloc char[pk.MaxStringLengthTrainer];
        int len = pk.LoadString(pk.OriginalTrainerTrash, ot);
        ot = ot[..len];
        if (!ot.SequenceEqual(tr.OT))
            return false;

        if (pk.Format == 3)
            return true; // Generation 3 does not check ot gender nor version

        if (tr.Gender != pk.OriginalTrainerGender)
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
        if (tr.Gender != pk.OriginalTrainerGender)
            return false;

        if (tr.Version != pk.Version)
        {
            if (!IsVersionlessState(pk))
                return false;
        }

        Span<char> ot = stackalloc char[pk.MaxStringLengthTrainer];
        int len = pk.LoadString(pk.OriginalTrainerTrash, ot);
        ot = ot[..len];
        if (!ot.SequenceEqual(tr.OT))
            return false;

        return true;
    }

    private static bool IsVersionlessState(PKM pk)
    {
        // PK9 does not store version for Picnic eggs.
        if (pk is PK9 { Version: 0 }) // IsEgg is already true for all calls
            return true;
        return false;
    }

    private static bool IsMatchVersion(ITrainerInfo tr, PKM pk)
    {
        if (tr.Version == pk.Version)
            return true;
        if (pk.GO_LGPE)
            return tr.Version is GameVersion.GP or GameVersion.GE;
        if (pk.Format <= 2)
            return true; // No version stored.
        return false;
    }
}
