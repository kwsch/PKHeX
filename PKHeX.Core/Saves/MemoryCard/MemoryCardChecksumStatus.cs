using System;

namespace PKHeX.Core;

[Flags]
public enum MemoryCardChecksumStatus
{
    None = 0,
    HeaderBad = 1 << 0,
    DirectoryBad = 1 << 1,
    DirectoryBackupBad = 1 << 2,
    BlockAllocBad = 1 << 3,
    BlockAllocBackupBad = 1 << 4,
}
