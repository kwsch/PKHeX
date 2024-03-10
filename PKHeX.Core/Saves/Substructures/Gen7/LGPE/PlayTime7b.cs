using System;

namespace PKHeX.Core;

/// <summary>
/// PlayTime object with a 1900-epoch Last Saved timestamp.
/// </summary>
public sealed class PlayTime7b : PlayTimeLastSaved<SaveFile, Epoch1900DateTimeValue>
{
    public PlayTime7b(SAV7b sav, Memory<byte> raw) : base(sav, raw) { }
    public PlayTime7b(SAV8SWSH sav, SCBlock block) : base(sav, block.Data) { }

    protected override Epoch1900DateTimeValue LastSaved => new(Raw.Slice(0x4, 4));
}
