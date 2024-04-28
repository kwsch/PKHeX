using System;

namespace PKHeX.Core;

public sealed class OPower6 : SaveBlock<SAV6>
{
    public OPower6(SAV6XY sav, Memory<byte> raw) : base(sav, raw) => ArgumentOutOfRangeException.ThrowIfNotEqual(raw.Length, Size);
    public OPower6(SAV6AO sav, Memory<byte> raw) : base(sav, raw) => ArgumentOutOfRangeException.ThrowIfNotEqual(raw.Length, Size);

    // Structure:

    // u8[65] OPowerTypeFlags
    // u8 Points
    // u8[10] Field Level 1
    // u8[10] Field Level 2
    // u8[7] Battle Level 1
    // u8[7] Battle Level 2
  //public const int OffsetFlags = 0;
    public const int OffsetPoints = 65;
    public const int OffsetFieldLevel1 = 66;
    public const int OffsetFieldLevel2 = OffsetFieldLevel1 + (int)OPower6FieldType.Count;
    public const int OffsetBattleLevel1 = OffsetFieldLevel2 + (int)OPower6FieldType.Count;
    public const int OffsetBattleLevel2 = OffsetBattleLevel1 + (int)OPower6BattleType.Count;
    public const int Size = OffsetBattleLevel2 + (int)OPower6BattleType.Count; // 100

    private Span<byte> IndexFlags => Data[..OffsetPoints];
    private Span<byte> FieldLevels1 => Data[OffsetFieldLevel1..OffsetFieldLevel2];
    private Span<byte> FieldLevels2 => Data[OffsetFieldLevel2..OffsetBattleLevel1];
    private Span<byte> BattleLevels1 => Data[OffsetBattleLevel1..OffsetBattleLevel2];
    private Span<byte> BattleLevels2 => Data[OffsetBattleLevel2..Size];

    public byte Points { get => Data[OffsetPoints]; set => Data[OffsetPoints] = value; }

    public OPowerFlagState GetState(OPower6Index index) => (OPowerFlagState)IndexFlags[(int)index];
    public byte GetLevel1(OPower6FieldType type) => FieldLevels1[(int)type];
    public byte GetLevel2(OPower6FieldType type) => FieldLevels2[(int)type];
    public byte GetLevel1(OPower6BattleType type) => BattleLevels1[(int)type];
    public byte GetLevel2(OPower6BattleType type) => BattleLevels2[(int)type];

    public void SetState(OPower6Index index, OPowerFlagState state) => IndexFlags[(int)index] = (byte)state;
    public void SetLevel1(OPower6FieldType type, byte value) => FieldLevels1[(int)type] = value;
    public void SetLevel2(OPower6FieldType type, byte value) => FieldLevels2[(int)type] = value;
    public void SetLevel1(OPower6BattleType type, byte value) => BattleLevels1[(int)type] = value;
    public void SetLevel2(OPower6BattleType type, byte value) => BattleLevels2[(int)type] = value;

    public void UnlockAll()
    {
        IndexFlags.Fill(1);
        FieldLevels1.Fill(3);
        FieldLevels2.Fill(3);
        BattleLevels1.Fill(3);
        BattleLevels2.Fill(3);
    }

    public void ClearAll() => Data[1..].Clear(); // skip the unlock flag.
}
