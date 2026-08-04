using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core.AutoMod;

public static class SettingsUtil
{
    public static List<GameVersion> SanitizePriorityOrder(List<GameVersion> versionList) // Thank you Anubis and Koi
    {
        var validVersions = Enum.GetValues<GameVersion>()
            .Where(GameUtil.IsValidSavedVersion).Reverse()
            .ToList();

        foreach (var ver in validVersions)
        {
            if (!versionList.Contains(ver))
                versionList.Add(ver); // Add any missing versions.
        }

        // Remove any versions in cfg.PriorityOrder that are not in validVersions and clean up duplicates in the process.
        return [.. versionList.Intersect(validVersions)];
    }
}