using System;

namespace PKHeX.Core;

/// <summary>
/// Miscellaneous origination <see cref="ISlotInfo"/>
/// </summary>
public sealed record SlotInfoMisc(Memory<byte> Data, int Slot, bool PartyFormat = false, bool Mutable = false, bool HideLegality = false) : ISlotInfo
{
    public required StorageSlotType Type { get; init; }
    public bool CanWriteTo(SaveFile sav) => Mutable;
    public WriteBlockedMessage CanWriteTo(SaveFile sav, PKM pk) => Mutable ? WriteBlockedMessage.None : WriteBlockedMessage.InvalidDestination;

    public bool WriteTo(SaveFile sav, PKM pk, EntityImportSettings settings = default)
    {
        var span = Data.Span;
        if (PartyFormat)
            sav.SetSlotFormatParty(pk, span, settings);
        else
            sav.SetSlotFormatStored(pk, span, settings);
        return true;
    }

    public PKM Read(SaveFile sav)
    {
        var span = Data.Span;
        return PartyFormat ? sav.GetPartySlot(span) : sav.GetStoredSlot(span);
    }
}
