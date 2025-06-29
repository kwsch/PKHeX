using System;

namespace PKHeX.Core;

/// <summary>
/// Represents a fake save file implementation for testing or placeholder purposes.
/// </summary>
/// <remarks>
/// This class provides a minimal implementation of the <see cref="SaveFile"/> base class, with default
/// or fixed values for all properties and methods. It is primarily intended for scenarios where a functional save file
/// is not required, such as testing or mock implementations.
/// </remarks>
public sealed class FakeSaveFile : SaveFile
{
    /// <summary>
    /// Represents the default instance of the <see cref="FakeSaveFile"/> class.
    /// </summary>
    /// <remarks>This static, read-only instance can be used as a default or fallback value when no specific
    /// <see cref="FakeSaveFile"/> instance is required.
    /// </remarks>
    public static readonly FakeSaveFile Default = new();
    private FakeSaveFile() { }

    protected internal override string ShortSummary => "Fake Save File";
    protected override FakeSaveFile CloneInternal() => this;
    public override string Extension => string.Empty;
    public override bool ChecksumsValid => true;
    public override string ChecksumInfo => string.Empty;
    public override byte Generation => 3;
    public override string GetString(ReadOnlySpan<byte> data) => string.Empty;
    public override int LoadString(ReadOnlySpan<byte> data, Span<char> destBuffer) => 0;
    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option) => 0;
    public override PersonalTable3 Personal => PersonalTable.RS;
    public override int MaxEV => 0;
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_RS;
    public override int GetBoxOffset(int box) => -1;
    public override int MaxStringLengthTrainer => 5;
    public override int MaxStringLengthNickname => 5;
    public override ushort MaxMoveID => 5;
    public override ushort MaxSpeciesID => 1;
    public override int MaxItemID => 5;
    public override int MaxBallID => 5;
    public override GameVersion MaxGameID => GameVersion.LG;
    public override int MaxAbilityID => 0;
    public override int BoxCount => 1;
    public override int GetPartyOffset(int slot) => -1;
    protected override void SetChecksums() { }
    public override GameVersion Version { get => GameVersion.R; set { } }
    public override Type PKMType => typeof(PK3);
    protected override PK3 GetPKM(byte[] data) => BlankPKM;
    protected override byte[] DecryptPKM(byte[] data) => data;
    public override PK3 BlankPKM => new();
    public override EntityContext Context => EntityContext.Gen3;
    protected override int SIZE_STORED => 0;
    protected override int SIZE_PARTY => 0;
}
