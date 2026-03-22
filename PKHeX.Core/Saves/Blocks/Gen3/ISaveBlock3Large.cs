using System;
using System.ComponentModel;

namespace PKHeX.Core;

[TypeConverter(typeof(ExpandableObjectConverter))]
public interface ISaveBlock3Large
{
    Memory<byte> Raw { get; }
    Span<byte> Data { get; }

    ushort X { get; set; }
    ushort Y { get; set; }
    byte PartyCount { get; set; }
    Span<byte> PartyBuffer { get; }
    uint Money { get; set; }
    ushort Coin { get; set; }
    ushort RegisteredItem { get; set; }
    Span<byte> EReaderBerry { get; }
    Gen3MysteryData MysteryData { get; set; }
    int DaycareOffset { get; }
    int DaycareSlotSize { get; }
    int BadgeFlagStart { get; }
    int EventFlagCount { get; }
    int EventWorkCount { get; }
    int EggEventFlag { get; }
    Memory<byte> RoamerData { get; }
    uint GetRecord(int record);
    void SetRecord(int record, uint value);

    Mail3 GetMail(int mailIndex);
    void SetMail(int mailIndex, Mail3 value);

    bool GetEventFlag(int flagNumber);
    void SetEventFlag(int flagNumber, bool value);
    ushort GetWork(int index);
    void SetWork(int index, ushort value);

    int SeenOffset2 { get; }
    int ExternalEventData { get; }
    int SeenOffset3 { get; }
    Span<byte> GiftRibbons { get; }
}
