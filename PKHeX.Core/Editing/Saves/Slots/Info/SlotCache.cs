using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Contains slot data and metadata indicating where the <see cref="PKM"/> originated from.
    /// </summary>
    public class SlotCache
    {
        /// <summary>
        /// Information regarding how the <see cref="Entity"/> was obtained.
        /// </summary>
        public readonly ISlotInfo Source;

        /// <summary>
        /// Save File reference that obtained the <see cref="Entity"/>.
        /// </summary>
        public readonly SaveFile SAV;

        /// <summary>
        /// Data that was loaded.
        /// </summary>
        public readonly PKM Entity;

        private static readonly FakeSaveFile NoSaveFile = new();

        public SlotCache(SlotInfoFile source, PKM entity)
        {
            Source = source;
            Entity = entity;
            SAV = NoSaveFile;
        }

        public SlotCache(ISlotInfo source, PKM entity, SaveFile sav)
        {
            Source = source;
            Entity = entity;
            SAV = sav;
        }

        public string Identify() => GetFileName() + Source switch
        {
            SlotInfoBox box => $"[{box.Box + 1:00}] ({SAV.GetBoxName(box.Box)})-{box.Slot + 1:00}: {Entity.FileName}",
            SlotInfoFile file => $"File: {file.Path}",
            SlotInfoMisc misc => $"{misc.Type}-{misc.Slot}: {Entity.FileName}",
            SlotInfoParty party => $"Party: {party.Slot}: {Entity.FileName}",
            _ => throw new ArgumentOutOfRangeException(nameof(Source))
        };

        private string GetFileName()
        {
            var fn = SAV.Metadata.FileName;
            if (fn is null)
                return string.Empty;
            return $"{fn} @ ";
        }

        public bool IsDataValid()
        {
            var e = Entity;
            return e.Species != 0 && e.ChecksumValid && (e.Sanity == 0 || e is BK4);
        }
    }
}
