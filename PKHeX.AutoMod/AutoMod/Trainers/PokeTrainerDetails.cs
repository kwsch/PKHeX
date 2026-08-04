using System;

namespace PKHeX.Core.AutoMod;

/// <summary>
/// Wrapper for a <see cref="PKM"/> to provide details as if it were a <see cref="ITrainerInfo"/>
/// </summary>
public sealed record PokeTrainerDetails(PKM Entity) : ITrainerInfo, IRegionOrigin
{
    public ushort TID16
    {
        get => Entity.TID16;
        set => throw new ArgumentException("Setter for this object should never be called.");
    }

    public ushort SID16
    {
        get => Entity.SID16;
        set => throw new ArgumentException("Setter for this object should never be called.");
    }

    public uint ID32
    {
        get => (uint)(TID16 | (SID16 << 16));
        set => (TID16, SID16) = ((ushort)value, (ushort)(value >> 16));
    }

    public TrainerIDFormat TrainerIDDisplayFormat => this.GetTrainerIDFormat();

    public string OT
    {
        get => Entity.OriginalTrainerName;
        set => Entity.OriginalTrainerName = value;
    }

    public byte Gender => Entity.OriginalTrainerGender;
    public GameVersion Version => Entity.Version;

    public int Language
    {
        get => Entity.Language;
        set => Entity.Language = value;
    }

    public byte Country
    {
        get => Entity is IGeoTrack gt ? gt.Country : (byte)49;
        set
        {
            if (Entity is IGeoTrack gt)
                gt.Country = value;
        }
    }

    public byte Region
    {
        get => Entity is IGeoTrack gt ? gt.Region : (byte)7;
        set
        {
            if (Entity is IGeoTrack gt)
                gt.Region = value;
        }
    }

    public byte ConsoleRegion
    {
        get => Entity is IGeoTrack gt ? gt.ConsoleRegion : (byte)1;
        set
        {
            if (Entity is IGeoTrack gt)
                gt.ConsoleRegion = value;
        }
    }

    public byte Generation => Entity.Generation;
    public EntityContext Context => Entity.Context;
}
