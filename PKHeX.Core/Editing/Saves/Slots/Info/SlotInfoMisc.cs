namespace PKHeX.Core;

/// <summary>
/// Miscellaneous origination <see cref="ISlotInfo"/>
/// </summary>
public sealed record SlotInfoMisc(byte[] Data, int Slot, int Offset, bool PartyFormat = false, bool Mutable = false) : ISlotInfo
{
    public SlotOrigin Origin => PartyFormat ? SlotOrigin.Party : SlotOrigin.Box;
    public bool CanWriteTo(SaveFile sav) => Mutable;
    public WriteBlockedMessage CanWriteTo(SaveFile sav, PKM pk) => Mutable ? WriteBlockedMessage.None : WriteBlockedMessage.InvalidDestination;
    public StorageSlotType Type { get; init; }

    public SlotInfoMisc(SaveFile sav, int slot, int offset, bool party = false) : this(GetBuffer(sav), slot, offset, party) { }

    private static byte[] GetBuffer(SaveFile sav) => sav switch
    {
        SAV4 s => s.General,
        SAV3 s3 => s3.Large,
        _ => sav.Data,
    };

    public bool WriteTo(SaveFile sav, PKM pk, PKMImportSetting setting = PKMImportSetting.UseDefault)
    {
        if (PartyFormat)
            sav.SetSlotFormatParty(pk, Data, Offset, setting, setting);
        else
            sav.SetSlotFormatStored(pk, Data, Offset, setting, setting);
        return true;
    }

    public PKM Read(SaveFile sav)
    {
        return PartyFormat ? sav.GetPartySlot(Data, Offset) : sav.GetStoredSlot(Data, Offset);
    }
}
