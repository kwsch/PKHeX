using System;
using System.ComponentModel;

namespace PKHeX.Core;

[TypeConverter(typeof(ExpandableObjectConverter))]
public interface ISaveBlock3Small
{
    Memory<byte> Raw { get; }
    Span<byte> Data { get; }

    Span<byte> OriginalTrainerTrash { get; }
    byte Gender { get; set; }
    uint ID32 { get; set; }
    ushort TID16 { get; set; }
    ushort SID16 { get; set; }
    int PlayedHours { get; set; }
    int PlayedMinutes { get; set; }
    int PlayedSeconds { get; set; }
    byte PlayedFrames { get; set; }
    byte OptionsButtonMode { get; set; }
    int TextSpeed { get; set; }
    byte OptionWindowFrame { get; set; }
    bool OptionSound { get; set; }
    bool OptionBattleStyle { get; set; }
    bool OptionBattleScene { get; set; }
    bool OptionIsRegionMapZoom { get; set; }
    byte PokedexSort { get; set; }
    byte PokedexMode { get; set; }
    byte PokedexNationalMagicRSE { get; set; }
    byte PokedexNationalMagicFRLG { get; set; }
    uint DexPIDUnown { get; set; }
    uint DexPIDSpinda { get; set; }
    Span<byte> EReaderTrainer { get; }
    uint SecurityKey { get; }
}
