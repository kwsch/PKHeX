using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Environment for editing a <see cref="SaveFile"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class SaveDataEditor<T>
    {
        public readonly SaveFile SAV;
        public readonly SlotEditor<T> Slots;
        public readonly IPKMView PKMEditor;

        public SaveDataEditor() : this(FakeSaveFile.Default) { }

        public SaveDataEditor(SaveFile sav)
        {
            SAV = sav;
            Slots = new SlotEditor<T>(sav);
            PKMEditor = new FakePKMEditor(SAV.BlankPKM);
        }

        public SaveDataEditor(SaveFile sav, IPKMView editor)
        {
            SAV = sav;
            Slots = new SlotEditor<T>(sav);
            PKMEditor = editor;
        }
    }

    /// <summary>
    /// Fakes the <see cref="IPKMView"/> interface interactions.
    /// </summary>
    public sealed class FakePKMEditor : IPKMView
    {
        public FakePKMEditor(PKM template) => Data = template;

        public PKM Data { get; private set; }
        public bool Unicode => true;
        public bool HaX => false;
        public bool ChangingFields { get; set; }
        public bool EditsComplete => true;

        public PKM PreparePKM(bool click = true) => Data;
        public void PopulateFields(PKM pk, bool focus = true, bool skipConversionCheck = false) => Data = pk;
    }

    public sealed class FakeSaveFile : SaveFile
    {
        public static readonly FakeSaveFile Default = new();
        protected internal override string ShortSummary => "Fake Save File";
        protected override SaveFile CloneInternal() => this;
        public override string Extension => string.Empty;
        public override bool ChecksumsValid => true;
        public override string ChecksumInfo => string.Empty;
        public override int Generation => 3;
        public override string GetString(byte[] data, int offset, int length) => string.Empty;
        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0) => Array.Empty<byte>();
        public override PersonalTable Personal => PersonalTable.RS;
        public override int MaxEV => 0;
        public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_RS;
        public override int GetBoxOffset(int box) => -1;
        public override string GetBoxName(int box) => $"Box {box:00}";
        public override void SetBoxName(int box, string value) { }
        public override int OTLength => 5;
        public override int NickLength => 5;
        public override int MaxMoveID => 5;
        public override int MaxSpeciesID => 1;
        public override int MaxItemID => 5;
        public override int MaxBallID => 5;
        public override int MaxGameID => 5;
        public override int MaxAbilityID => 0;
        public override int BoxCount => 1;
        public override int GetPartyOffset(int slot) => -1;
        protected override void SetChecksums() { }

        public override Type PKMType => typeof(PK3);
        protected override PKM GetPKM(byte[] data) => BlankPKM;
        protected override byte[] DecryptPKM(byte[] data) => data;
        public override PKM BlankPKM => new PK3();
        protected override int SIZE_STORED => 0;
        protected override int SIZE_PARTY => 0;
    }
}