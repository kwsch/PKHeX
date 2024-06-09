using System;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

public sealed class EventWorkspace<TSave, TWork> where TSave : class, IEventFlagArray, IEventWorkArray<TWork> where TWork : unmanaged
{
    private readonly TSave SAV;
    public readonly bool[] Flags;
    public readonly TWork[] Values;
    public readonly EventLabelCollection Labels;

    public EventWorkspace(TSave obj, GameVersion version)
    {
        SAV = obj;
        Flags = obj.GetEventFlags();
        Values = obj.GetAllEventWork();

        var game = GetResourceSuffix(version);
        Labels = new EventLabelCollection(game, Flags.Length, Values.Length);
    }

    public void Save()
    {
        SAV.SetEventFlags(Flags);
        SAV.SetAllEventWork(Values);
        if (SAV is EventWork7 sm) // Ensure QR event flag has magic constant(s)
            sm.UpdateQrConstants();
    }

    private static string GetResourceSuffix(GameVersion version) => version switch
    {
        X or Y or XY => "xy",
        OR or AS or ORAS => "oras",
        SN or MN or SM => "sm",
        US or UM or USUM => "usum",
        D or P or DP => "dp",
        Pt or DPPt => "pt",
        HG or SS or HGSS => "hgss",
        B or W or BW => "bw",
        B2 or W2 or B2W2 => "b2w2",
        R or S or RS => "rs",
        E => "e",
        FR or LG or FRLG => "frlg",
        C => "c",
        GD or SI or GS => "gs",
        _ => throw new ArgumentOutOfRangeException(nameof(version), version, null),
    };
}
