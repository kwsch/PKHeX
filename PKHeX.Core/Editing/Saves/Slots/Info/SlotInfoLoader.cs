using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace PKHeX.Core;

/// <summary>
/// Loads <see cref="SlotCache"/> from sources into a storage collection.
/// </summary>
public static class SlotInfoLoader
{
    // The "Add" method isn't shared for any interface... so we'll just do this twice.

    #region ConcurrentBag Implementation
    public static void AddFromSaveFile(SaveFile sav, ConcurrentBag<SlotCache> db)
    {
        if (sav.HasBox)
            AddBoxData(sav, db);

        if (sav.HasParty)
            AddPartyData(sav, db);

        AddExtraData(sav, db);
    }

    public static void AddFromLocalFile(string file, ConcurrentBag<SlotCache> db, ITrainerInfo dest, ICollection<string> validExtensions)
    {
        var fi = new FileInfo(file);
        if (!validExtensions.Contains(fi.Extension) || !EntityDetection.IsSizePlausible(fi.Length))
            return;

        var data = File.ReadAllBytes(file);
        if (!FileUtil.TryGetPKM(data, out var pk, fi.Extension, dest))
            return;
        if (pk.Species is 0)
            return;

        var info = new SlotInfoFile(file);
        var entry = new SlotCache(info, pk);
        db.Add(entry);
    }

    private static void AddBoxData(SaveFile sav, ConcurrentBag<SlotCache> db)
    {
        var bc = sav.BoxCount;
        var sc = sav.BoxSlotCount;
        for (int box = 0; box < bc; box++)
        {
            for (int slot = 0; slot < sc; slot++)
            {
                var ident = new SlotInfoBox(box, slot);
                var pk = sav.GetBoxSlotAtIndex(box, slot);
                var result = new SlotCache(ident, pk, sav);
                db.Add(result);
            }
        }
    }

    private static void AddPartyData(SaveFile sav, ConcurrentBag<SlotCache> db)
    {
        var count = sav.PartyCount;
        if ((uint)count > 6)
            count = 6;
        for (var index = 0; index < count; index++)
        {
            var pk = sav.GetPartySlotAtIndex(index);
            if (pk.Species == 0)
                continue;

            var ident = new SlotInfoParty(index);
            var result = new SlotCache(ident, pk, sav);
            db.Add(result);
        }
    }

    private static void AddExtraData(SaveFile sav, ConcurrentBag<SlotCache> db)
    {
        var extra = sav.GetExtraSlots(true);
        foreach (var x in extra)
        {
            var pk = x.Read(sav);
            if (pk.Species == 0)
                continue;

            var result = new SlotCache(x, pk, sav);
            db.Add(result);
        }
    }
    #endregion

    #region ICollection Implementation
    public static void AddFromSaveFile(SaveFile sav, ICollection<SlotCache> db)
    {
        if (sav.HasBox)
            AddBoxData(sav, db);

        if (sav.HasParty)
            AddPartyData(sav, db);

        AddExtraData(sav, db);
    }

    public static void AddFromLocalFile(string file, ICollection<SlotCache> db, ITrainerInfo dest, ICollection<string> validExtensions)
    {
        var fi = new FileInfo(file);
        if (!validExtensions.Contains(fi.Extension) || !EntityDetection.IsSizePlausible(fi.Length))
            return;

        var data = File.ReadAllBytes(file);
        _ = FileUtil.TryGetPKM(data, out var pk, fi.Extension, dest);
        if (pk?.Species is not > 0)
            return;

        var info = new SlotInfoFile(file);
        var entry = new SlotCache(info, pk);
        db.Add(entry);
    }

    public static void AddBoxData(SaveFile sav, ICollection<SlotCache> db)
    {
        var bc = sav.BoxCount;
        var sc = sav.BoxSlotCount;
        for (int box = 0; box < bc; box++)
        {
            for (int slot = 0; slot < sc; slot++)
            {
                var ident = new SlotInfoBox(box, slot);
                var pk = sav.GetBoxSlotAtIndex(box, slot);
                var result = new SlotCache(ident, pk, sav);
                db.Add(result);
            }
        }
    }

    public static void AddPartyData(SaveFile sav, ICollection<SlotCache> db)
    {
        var count = sav.PartyCount;
        for (var index = 0; index < count; index++)
        {
            var pk = sav.GetPartySlotAtIndex(index);
            if (pk.Species == 0)
                continue;

            var ident = new SlotInfoParty(index);
            var result = new SlotCache(ident, pk, sav);
            db.Add(result);
        }
    }

    private static void AddExtraData(SaveFile sav, ICollection<SlotCache> db)
    {
        var extra = sav.GetExtraSlots(true);
        foreach (var x in extra)
        {
            var pk = x.Read(sav);
            if (pk.Species == 0)
                continue;

            var result = new SlotCache(x, pk, sav);
            db.Add(result);
        }
    }
    #endregion
}
