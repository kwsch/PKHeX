using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Base Class for Save Files
    /// </summary>
    public abstract class SaveFile : ITrainerInfo, IGameValueLimit
    {
        public static PKMImportSetting SetUpdateDex { protected get; set; } = PKMImportSetting.Update;
        public static PKMImportSetting SetUpdatePKM { protected get; set; } = PKMImportSetting.Update;

        // General Object Properties
        public byte[] Data;
        public bool Edited;
        public string FileName, FilePath, FileFolder;
        public string BAKName => $"{FileName} [{BAKText}].bak";
        protected abstract string BAKText { get; }
        public byte[] BAK { get; protected set; }
        public bool Exportable { get; protected set; }
        public abstract SaveFile Clone();
        public abstract string Filter { get; }
        public byte[] Footer { protected get; set; } = Array.Empty<byte>(); // .dsv
        public byte[] Header { protected get; set; } = Array.Empty<byte>(); // .gci
        public bool Japanese { get; protected set; }
        public virtual string PlayTimeString => $"{PlayedHours}ː{PlayedMinutes:00}ː{PlayedSeconds:00}"; // not :
        public bool IndeterminateGame => Version == GameVersion.Unknown;
        public abstract string Extension { get; }

        public virtual string[] PKMExtensions => PKM.Extensions.Where(f =>
        {
            int gen = f.Last() - 0x30;
            return 3 <= gen && gen <= Generation;
        }).ToArray();

        // General PKM Properties
        public abstract Type PKMType { get; }
        protected abstract PKM GetPKM(byte[] data);
        protected abstract byte[] DecryptPKM(byte[] data);
        public abstract PKM BlankPKM { get; }
        public abstract int SIZE_STORED { get; }
        protected abstract int SIZE_PARTY { get; }
        public abstract int MaxEV { get; }
        public virtual int MaxIV => 31;
        public ushort[] HeldItems { get; protected set; }

        // General SAV Properties
        public byte[] Write(ExportFlags flags = ExportFlags.None)
        {
            byte[] data = GetFinalData();
            if (Footer.Length > 0 && flags.HasFlagFast(ExportFlags.IncludeFooter))
                return data.Concat(Footer).ToArray();
            if (Header.Length > 0 && flags.HasFlagFast(ExportFlags.IncludeHeader))
                return Header.Concat(data).ToArray();
            return data;
        }

        protected virtual byte[] GetFinalData()
        {
            SetChecksums();
            return Data;
        }

        public virtual string MiscSaveChecks() => string.Empty;
        public virtual string MiscSaveInfo() => string.Empty;
        public virtual GameVersion Version { get; protected set; }
        public abstract bool ChecksumsValid { get; }
        public abstract string ChecksumInfo { get; }
        public abstract int Generation { get; }
        public PersonalTable Personal { get; set; }

        public bool GG => Data.Length == SaveUtil.SIZE_G7GG && GameVersion.GG.Contains(Version);
        public bool USUM => Data.Length == SaveUtil.SIZE_G7USUM;
        public bool SM => Data.Length == SaveUtil.SIZE_G7SM;
        public bool ORASDEMO => Data.Length == SaveUtil.SIZE_G6ORASDEMO;
        public bool ORAS => Data.Length == SaveUtil.SIZE_G6ORAS;
        public bool XY => Data.Length == SaveUtil.SIZE_G6XY;
        public bool B2W2 => Version == GameVersion.B2W2;
        public bool BW => Version == GameVersion.BW;
        public bool HGSS => Version == GameVersion.HGSS;
        public bool Pt => Version == GameVersion.Pt;
        public bool DP => Version == GameVersion.DP;
        public bool E => Version == GameVersion.E;
        public bool FRLG => Version == GameVersion.FRLG;
        public bool RS => Version == GameVersion.RS;
        public bool GSC => Version == GameVersion.GS || Version == GameVersion.C;
        public bool RBY => Version == GameVersion.RBY;
        public bool GameCube => new[] { GameVersion.COLO, GameVersion.XD, GameVersion.RSBOX }.Contains(Version);

        public abstract int MaxMoveID { get; }
        public abstract int MaxSpeciesID { get; }
        public abstract int MaxAbilityID { get; }
        public abstract int MaxItemID { get; }
        public abstract int MaxBallID { get; }
        public abstract int MaxGameID { get; }
        public virtual int MinGameID => 0;

        // Flags
        public bool HasWondercards => WondercardData > -1;
        public bool HasSuperTrain => SuperTrain > -1;
        public bool HasBerryField => BerryField > -1;
        public bool HasHoF => HoF > -1;
        public bool HasSecretBase => SecretBase > -1;
        public bool HasPSS => PSS > -1;
        public bool HasOPower => OPower > -1;
        public bool HasJPEG => JPEGData.Length > 0;
        public bool HasBox => Box > -1;
        public virtual bool HasParty => Party > -1;
        public bool HasBattleBox => BattleBox > -1;
        public bool HasFused => Fused > -1;
        public bool HasGTS => GTS > -1;
        public bool HasDaycare => Daycare > -1;
        public virtual bool HasPokeDex => PokeDex > -1;
        public virtual bool HasBoxWallpapers => GetBoxWallpaperOffset(0) > -1;
        public virtual bool HasNamableBoxes => HasBoxWallpapers;
        public bool HasPokeBlock => ORAS && !ORASDEMO;
        public virtual bool HasEvents => EventFlags.Length != 0;
        public bool HasLink => (ORAS && !ORASDEMO) || XY;

        // Counts
        protected virtual int GiftCountMax { get; } = int.MinValue;
        protected virtual int GiftFlagMax { get; } = 0x800;
        protected virtual int EventFlagMax { get; } = int.MinValue;
        protected virtual int EventConstMax { get; } = int.MinValue;
        public virtual int DaycareSeedSize { get; } = 0;
        public abstract int OTLength { get; }
        public abstract int NickLength { get; }
        public virtual int MaxMoney => 9999999;
        public virtual int MaxCoins => 9999;

        // Offsets
        protected int Box { get; set; } = int.MinValue;
        protected int Party { get; set; } = int.MinValue;
        protected int Trainer1 { get; set; } = int.MinValue;
        protected int Daycare { get; set; } = int.MinValue;
        protected int WondercardData { get; set; } = int.MinValue;
        protected int PCLayout { get; set; } = int.MinValue;
        protected int EventFlag { get; set; } = int.MinValue;
        protected int EventConst { get; set; } = int.MinValue;

        public int GTS { get; protected set; } = int.MinValue;
        public int BattleBox { get; protected set; } = int.MinValue;
        public int Fused { get; protected set; } = int.MinValue;
        public int SUBE { get; protected set; } = int.MinValue;
        public int PokeDex { get; protected set; } = int.MinValue;
        public int SuperTrain { get; protected set; } = int.MinValue;
        public int SecretBase { get; protected set; } = int.MinValue;
        public int PSS { get; protected set; } = int.MinValue;
        public int BerryField { get; protected set; } = int.MinValue;
        public int OPower { get; protected set; } = int.MinValue;
        public int HoF { get; protected set; } = int.MinValue;

        // SAV Properties
        public IList<PKM> BoxData
        {
            get
            {
                PKM[] data = new PKM[BoxCount*BoxSlotCount];
                for (int box = 0; box < BoxCount; box++)
                    AddBoxData(data, box, box * BoxSlotCount);
                return data;
            }
            set
            {
                if (value.Count != BoxCount*BoxSlotCount)
                    throw new ArgumentException($"Expected {BoxCount*BoxSlotCount}, got {value.Count}");
                if (value.Any(pk => PKMType != pk.GetType()))
                    throw new ArgumentException($"Not {PKMType} array.");

                for (int b = 0; b < BoxCount; b++)
                    SetBoxData(value, b, b * BoxSlotCount);
            }
        }

        public void SetBoxData(IList<PKM> value, int box, int index = 0)
        {
            int ofs = GetBoxOffset(box);
            for (int slot = 0; slot < BoxSlotCount; slot++, ofs += SIZE_STORED)
            {
                var pk = value[index + slot];
                if (!pk.StorageFlags.IsOverwriteProtected())
                    SetStoredSlot(pk, ofs);
            }
        }

        public PKM[] GetBoxData(int box)
        {
            var data = new PKM[BoxSlotCount];
            AddBoxData(data, box, 0);
            return data;
        }

        private void AddBoxData(IList<PKM> data, int box, int index)
        {
            int ofs = GetBoxOffset(box);
            var boxName = GetBoxName(box);
            for (int slot = 0; slot < BoxSlotCount; slot++, ofs += SIZE_STORED)
            {
                int i = slot + index;
                data[i] = GetStoredSlot(ofs);
                data[i].Identifier = $"{boxName}:{slot + 1:00}";
                data[i].Box = box + 1;
                data[i].Slot = slot + 1;
                data[i].StorageFlags = GetSlotFlags(box, slot);
            }
        }

        public IList<PKM> PartyData
        {
            get
            {
                PKM[] data = new PKM[PartyCount];
                for (int i = 0; i < data.Length; i++)
                    data[i] = GetPartySlot(GetPartyOffset(i));
                return data;
            }
            set
            {
                if (value.Count == 0 || value.Count > 6)
                    throw new ArgumentException($"Expected 1-6, got {value.Count}");
                if (value.Any(pk => PKMType != pk.GetType()))
                    throw new ArgumentException($"Not {PKMType} array.");
                if (value[0].Species == 0)
                    Debug.WriteLine($"Empty first slot, received {value.Count}.");

                int ctr = 0;
                foreach (var exist in value.Where(pk => pk.Species != 0))
                    SetPartySlot(exist, GetPartyOffset(ctr++));
                for (int i = ctr; i < 6; i++)
                    SetPartySlot(BlankPKM, GetPartyOffset(i));
            }
        }

        public IList<PKM> BattleBoxData
        {
            get
            {
                if (!HasBattleBox)
                    return Array.Empty<PKM>();

                PKM[] data = new PKM[6];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = GetStoredSlot(GetBattleBoxOffset(i));
                    if (BattleBoxLocked)
                        data[i].StorageFlags |= StorageSlotFlag.Locked;
                    if (data[i].Species != 0)
                        continue;
                    Array.Resize(ref data, i);
                    return data;
                }
                return data;
            }
        }

        public int GetBattleBoxOffset(int index) => BattleBox + (SIZE_STORED * index);

        /// <summary> All Event Flag values for the savegame </summary>
        public bool[] EventFlags
        {
            get
            {
                if (EventFlagMax < 0)
                    return Array.Empty<bool>();

                bool[] Flags = new bool[EventFlagMax];
                for (int i = 0; i < Flags.Length; i++)
                    Flags[i] = GetEventFlag(i);
                return Flags;
            }
            set
            {
                if (EventFlagMax < 0)
                    return;
                if (value.Length != EventFlagMax)
                    return;
                for (int i = 0; i < value.Length; i++)
                    SetEventFlag(i, value[i]);
            }
        }

        /// <summary> All Event Constant values for the savegame </summary>
        public virtual ushort[] EventConsts
        {
            get
            {
                if (EventConstMax <= 0)
                    return Array.Empty<ushort>();

                ushort[] Constants = new ushort[EventConstMax];
                for (int i = 0; i < Constants.Length; i++)
                    Constants[i] = BitConverter.ToUInt16(Data, EventConst + (i * 2));
                return Constants;
            }
            set
            {
                if (EventConstMax <= 0)
                    return;
                if (value.Length != EventConstMax)
                    return;

                for (int i = 0; i < value.Length; i++)
                    BitConverter.GetBytes(value[i]).CopyTo(Data, EventConst + (i * 2));
            }
        }

        /// <summary>
        /// Gets the <see cref="bool"/> status of a desired Event Flag
        /// </summary>
        /// <param name="flagNumber">Event Flag to check</param>
        /// <returns>Flag is Set (true) or not Set (false)</returns>
        public virtual bool GetEventFlag(int flagNumber)
        {
            if (flagNumber >= EventFlagMax)
                throw new ArgumentException($"Event Flag to get ({flagNumber}) is greater than max ({EventFlagMax}).");
            return GetFlag(EventFlag + (flagNumber >> 3), flagNumber & 7);
        }

        /// <summary>
        /// Sets the <see cref="bool"/> status of a desired Event Flag
        /// </summary>
        /// <param name="flagNumber">Event Flag to check</param>
        /// <param name="value">Event Flag status to set</param>
        /// <remarks>Flag is Set (true) or not Set (false)</remarks>
        public virtual void SetEventFlag(int flagNumber, bool value)
        {
            if (flagNumber >= EventFlagMax)
                throw new ArgumentException($"Event Flag to set ({flagNumber}) is greater than max ({EventFlagMax}).");
            SetFlag(EventFlag + (flagNumber >> 3), flagNumber & 7, value);
        }

        /// <summary>
        /// Gets the <see cref="bool"/> status of the Flag at the specified offset and index.
        /// </summary>
        /// <param name="offset">Offset to read from</param>
        /// <param name="bitIndex">Bit index to read</param>
        /// <returns>Flag is Set (true) or not Set (false)</returns>
        public bool GetFlag(int offset, int bitIndex)
        {
            bitIndex &= 7; // ensure bit access is 0-7
            return (Data[offset] >> bitIndex & 1) != 0;
        }

        /// <summary>
        /// Sets the <see cref="bool"/> status of the Flag at the specified offset and index.
        /// </summary>
        /// <param name="offset">Offset to read from</param>
        /// <param name="bitIndex">Bit index to read</param>
        /// <param name="value">Flag status to set</param>
        /// <remarks>Flag is Set (true) or not Set (false)</remarks>
        public void SetFlag(int offset, int bitIndex, bool value)
        {
            bitIndex &= 7; // ensure bit access is 0-7
            Data[offset] &= (byte)~(1 << bitIndex);
            Data[offset] |= (byte)((value ? 1 : 0) << bitIndex);
        }

        // Inventory
        public virtual InventoryPouch[] Inventory { get; set; }
        protected int OFS_PouchHeldItem { get; set; } = int.MinValue;
        protected int OFS_PouchKeyItem { get; set; } = int.MinValue;
        protected int OFS_PouchMedicine { get; set; } = int.MinValue;
        protected int OFS_PouchTMHM { get; set; } = int.MinValue;
        protected int OFS_PouchBerry { get; set; } = int.MinValue;
        protected int OFS_PouchBalls { get; set; } = int.MinValue;
        protected int OFS_BattleItems { get; set; } = int.MinValue;
        protected int OFS_MailItems { get; set; } = int.MinValue;
        protected int OFS_PCItem { get; set; } = int.MinValue;
        protected int OFS_PouchZCrystals { get; set; } = int.MinValue;

        // Mystery Gift
        protected virtual bool[] MysteryGiftReceivedFlags { get => Array.Empty<bool>(); set { } }
        protected virtual MysteryGift[] MysteryGiftCards { get => Array.Empty<MysteryGift>(); set { } }

        public virtual MysteryGiftAlbum GiftAlbum
        {
            get => new MysteryGiftAlbum
            {
                Flags = MysteryGiftReceivedFlags,
                Gifts = MysteryGiftCards
            };
            set
            {
                MysteryGiftReceivedFlags = value.Flags;
                MysteryGiftCards = value.Gifts;
            }
        }

        public virtual bool BattleBoxLocked { get => false; set { } }
        public virtual string JPEGTitle => string.Empty;
        public virtual byte[] JPEGData => Array.Empty<byte>();
        public virtual int Country { get => -1; set { } }
        public virtual int ConsoleRegion { get => -1; set { } }
        public virtual int SubRegion { get => -1; set { } }

        // Trainer Info
        public virtual int Gender { get; set; }
        public virtual int Language { get => -1; set { } }
        public virtual int Game { get => -1; set { } }
        public virtual int TID { get; set; }
        public virtual int SID { get; set; }
        public int TrainerID7 => (int)((uint)(TID | (SID << 16)) % 1000000);
        public int TrainerSID7 => (int)((uint)(TID | (SID << 16)) / 1000000);
        public virtual string OT { get; set; } = "PKHeX";
        public virtual int PlayedHours { get; set; }
        public virtual int PlayedMinutes { get; set; }
        public virtual int PlayedSeconds { get; set; }
        public virtual uint SecondsToStart { get; set; }
        public virtual uint SecondsToFame { get; set; }
        public virtual uint Money { get; set; }
        public abstract int BoxCount { get; }
        public virtual int SlotCount => BoxCount * BoxSlotCount;
        public virtual int PartyCount { get; protected set; }
        public virtual int MultiplayerSpriteID { get => 0; set { } }

        public bool IsPartyAllEggs(params int[] except)
        {
            if (!HasParty)
                return false;

            var party = PartyData;
            return party.Count == party.Where((t, i) => t.IsEgg || except.Contains(i)).Count();
        }

        // Varied Methods
        protected abstract void SetChecksums();
        public abstract int GetBoxOffset(int box);
        public abstract int GetPartyOffset(int slot);
        public abstract string GetBoxName(int box);
        public abstract void SetBoxName(int box, string value);
        public virtual int GameSyncIDSize { get; } = 8;
        public virtual string GameSyncID { get => null; set { } }

        // Daycare
        public int DaycareIndex = 0;
        public virtual bool HasTwoDaycares => false;
        public virtual int GetDaycareSlotOffset(int loc, int slot) => -1;
        public virtual uint? GetDaycareEXP(int loc, int slot) => null;
        public virtual string GetDaycareRNGSeed(int loc) => null;
        public virtual bool? IsDaycareHasEgg(int loc) => null;
        public virtual bool? IsDaycareOccupied(int loc, int slot) => null;

        public virtual void SetDaycareEXP(int loc, int slot, uint EXP) { }
        public virtual void SetDaycareRNGSeed(int loc, string seed) { }
        public virtual void SetDaycareHasEgg(int loc, bool hasEgg) { }
        public virtual void SetDaycareOccupied(int loc, int slot, bool occupied) { }

        // Storage
        public virtual int BoxSlotCount => 30;
        public virtual int BoxesUnlocked { get => -1; set { } }
        public virtual byte[] BoxFlags { get => Array.Empty<byte>(); set { } }
        public virtual int CurrentBox { get; set; }
        protected int[] TeamSlots = Array.Empty<int>();
        protected virtual IList<int>[] SlotPointers => new[] {TeamSlots};
        public virtual StorageSlotFlag GetSlotFlags(int index) => StorageSlotFlag.None;
        public StorageSlotFlag GetSlotFlags(int box, int slot) => GetSlotFlags((box * BoxSlotCount) + slot);
        public bool IsSlotLocked(int box, int slot) => GetSlotFlags(box, slot).HasFlagFast(StorageSlotFlag.Locked);
        public bool IsSlotLocked(int index) => GetSlotFlags(index).HasFlagFast(StorageSlotFlag.Locked);
        public bool IsSlotOverwriteProtected(int box, int slot) => GetSlotFlags(box, slot).IsOverwriteProtected();
        public bool IsSlotOverwriteProtected(int index) => GetSlotFlags(index).IsOverwriteProtected();
        public bool IsSlotOverwriteProtected(PKM pkm) => GetSlotFlags(pkm.Box, pkm.Slot).IsOverwriteProtected();

        public bool MoveBox(int box, int insertBeforeBox)
        {
            if (box == insertBeforeBox) // no movement required
                return true;
            if (box >= BoxCount || insertBeforeBox >= BoxCount) // invalid box positions
                return false;

            int pos1 = BoxSlotCount*box;
            int pos2 = BoxSlotCount*insertBeforeBox;
            int min = Math.Min(pos1, pos2);
            int max = Math.Max(pos1, pos2);

            int len = BoxSlotCount*SIZE_STORED;
            byte[] boxdata = GetData(GetBoxOffset(0), len*BoxCount); // get all boxes
            string[] boxNames = new int[BoxCount].Select((_, i) => GetBoxName(i)).ToArray();
            int[] boxWallpapers = new int[BoxCount].Select((_, i) => GetBoxWallpaper(i)).ToArray();

            min /= BoxSlotCount;
            max /= BoxSlotCount;

            // move all boxes within range to final spot
            for (int i = min, ctr = min; i < max; i++)
            {
                int b = insertBeforeBox; // if box is the moved box, move to insertion point, else move to unused box.
                if (i != box)
                {
                    if (insertBeforeBox == ctr)
                        ++ctr;
                    b = ctr++;
                }
                Buffer.BlockCopy(boxdata, len*i, Data, GetBoxOffset(b), len);
                SetBoxName(b, boxNames[i]);
                SetBoxWallpaper(b, boxWallpapers[i]);
            }
            SlotPointerUtil.UpdateMove(box, insertBeforeBox, BoxSlotCount, SlotPointers);

            return true;
        }

        public bool SwapBox(int box1, int box2)
        {
            if (box1 == box2) // no movement required
                return true;
            if (box1 >= BoxCount || box2 >= BoxCount) // invalid box positions
                return false;

            if (!IsBoxAbleToMove(box1) || !IsBoxAbleToMove(box2))
                return false;

            // Data
            int b1o = GetBoxOffset(box1);
            int b2o = GetBoxOffset(box2);
            int len = BoxSlotCount*SIZE_STORED;
            byte[] b1 = new byte[len];
            Buffer.BlockCopy(Data, b1o, b1, 0, len);
            Buffer.BlockCopy(Data, b2o, Data, b1o, len);
            Buffer.BlockCopy(b1, 0, Data, b2o, len);

            // Name
            string b1n = GetBoxName(box1);
            SetBoxName(box1, GetBoxName(box2));
            SetBoxName(box2, b1n);

            // Wallpaper
            int b1w = GetBoxWallpaper(box1);
            SetBoxWallpaper(box1, GetBoxWallpaper(box2));
            SetBoxWallpaper(box2, b1w);

            // Pointers
            SlotPointerUtil.UpdateSwap(box1, box2, BoxSlotCount, SlotPointers);

            return true;
        }

        private bool IsBoxAbleToMove(int box)
        {
            int min = BoxSlotCount * box;
            int max = min + BoxSlotCount;
            return !IsRegionOverwriteProtected(min, max);
        }

        protected virtual int GetBoxWallpaperOffset(int box) => -1;

        public virtual int GetBoxWallpaper(int box)
        {
            int offset = GetBoxWallpaperOffset(box);
            if (offset < 0 || box > BoxCount)
                return box;
            return Data[offset];
        }

        public virtual void SetBoxWallpaper(int box, int value)
        {
            int offset = GetBoxWallpaperOffset(box);
            if (offset < 0 || box > BoxCount)
                return;
            Data[offset] = (byte)value;
        }

        private void GetBoxSlotFromIndex(int index, out int box, out int slot)
        {
            box = index / BoxSlotCount;
            if (box >= BoxCount)
                throw new ArgumentOutOfRangeException(nameof(index));
            slot = index % BoxSlotCount;
        }

        public PKM GetPartySlotAtIndex(int index) => GetPartySlot(GetPartyOffset(index));
        public PKM GetBoxSlotAtIndex(int box, int slot) => GetStoredSlot(GetBoxSlotOffset(box, slot));

        public PKM GetBoxSlotAtIndex(int index)
        {
            GetBoxSlotFromIndex(index, out int box, out int slot);
            return GetBoxSlotAtIndex(box, slot);
        }

        public int GetBoxSlotOffset(int box, int slot) => GetBoxOffset(box) + (slot * SIZE_STORED);

        public int GetBoxSlotOffset(int index)
        {
            GetBoxSlotFromIndex(index, out int box, out int slot);
            return GetBoxSlotOffset(box, slot);
        }

        public void SetBoxSlotAtIndex(PKM pkm, int box, int slot, PKMImportSetting trade = PKMImportSetting.UseDefault, PKMImportSetting dex = PKMImportSetting.UseDefault)
            => SetStoredSlot(pkm, GetBoxSlotOffset(box, slot), trade, dex);

        public void SetBoxSlotAtIndex(PKM pkm, int index, PKMImportSetting trade = PKMImportSetting.UseDefault, PKMImportSetting dex = PKMImportSetting.UseDefault)
            => SetStoredSlot(pkm, GetBoxSlotOffset(index), trade, dex);

        public void SetPartySlotAtIndex(PKM pkm, int index, PKMImportSetting trade = PKMImportSetting.UseDefault, PKMImportSetting dex = PKMImportSetting.UseDefault)
            => SetPartySlot(pkm, GetPartyOffset(index), trade, dex);

        public virtual PKM GetPartySlot(int offset) => GetPKM(DecryptPKM(GetData(offset, SIZE_PARTY)));
        public virtual PKM GetStoredSlot(int offset) => GetPKM(DecryptPKM(GetData(offset, SIZE_STORED)));

        public void SetPartySlot(PKM pkm, int offset, PKMImportSetting trade = PKMImportSetting.UseDefault, PKMImportSetting dex = PKMImportSetting.UseDefault)
        {
            if (pkm == null) return;
            if (pkm.GetType() != PKMType)
                throw new ArgumentException($"PKM Format needs to be {PKMType} when setting to this Save File.");
            UpdatePKM(pkm, trade, dex);
            SetPartyValues(pkm, isParty: true);

            int i = GetPartyIndex(offset);
            if (i <= -1)
                throw new ArgumentException("Invalid Party offset provided; unable to resolve party slot index.");

            // update party count
            if (pkm.Species != 0)
            {
                if (PartyCount <= i)
                    PartyCount = i + 1;
            }
            else if (PartyCount > i)
            {
                PartyCount = i;
            }

            SetData(pkm.EncryptedPartyData, offset);
        }

        protected void UpdatePKM(PKM pkm, PKMImportSetting trade, PKMImportSetting dex)
        {
            if (GetTradeUpdateSetting(trade))
                SetPKM(pkm);
            if (GetDexUpdateSetting(dex))
                SetDex(pkm);
        }

        private static bool GetTradeUpdateSetting(PKMImportSetting trade = PKMImportSetting.UseDefault)
        {
            if (trade == PKMImportSetting.UseDefault)
                trade = SetUpdatePKM;
            return trade == PKMImportSetting.Update;
        }

        private static bool GetDexUpdateSetting(PKMImportSetting trade = PKMImportSetting.UseDefault)
        {
            if (trade == PKMImportSetting.UseDefault)
                trade = SetUpdateDex;
            return trade == PKMImportSetting.Update;
        }

        private int GetPartyIndex(int offset)
        {
            for (int i = 0; i < 6; i++)
            {
                if (GetPartyOffset(i) == offset)
                    return i;
            }
            return -1;
        }

        public virtual void SetStoredSlot(PKM pkm, int offset, PKMImportSetting trade = PKMImportSetting.UseDefault, PKMImportSetting dex = PKMImportSetting.UseDefault)
        {
            if (pkm == null) return;
            if (pkm.GetType() != PKMType)
                throw new ArgumentException($"PKM Format needs to be {PKMType} when setting to this Save File.");
            UpdatePKM(pkm, trade, dex);
            SetPartyValues(pkm, isParty: false);
            SetData(pkm.EncryptedBoxData, offset);
        }

        public void DeletePartySlot(int slot)
        {
            if (PartyCount <= slot) // beyond party range (or empty data already present)
                return;
            // Move all party slots down one
            for (int i = slot + 1; i < 6; i++) // Slide slots down
            {
                int slotTo = GetPartyOffset(i - 1);
                int slotFrom = GetPartyOffset(i);
                SetData(GetData(slotFrom, SIZE_PARTY), slotTo);
            }
            SetStoredSlot(BlankPKM, GetPartyOffset(5), PKMImportSetting.Skip, PKMImportSetting.Skip);
            PartyCount--;
        }

        protected virtual bool IsSlotSwapProtected(int box, int slot) => false;

        private bool IsRegionOverwriteProtected(int min, int max)
        {
            return SlotPointers.SelectMany(z => z)
                .Where(z => GetSlotFlags(z).IsOverwriteProtected())
                .Any(slot => WithinRange(slot, min, max));
        }

        private static bool WithinRange(int slot, int min, int max) => min <= slot && slot < max;

        public bool IsAnySlotLockedInBox(int BoxStart, int BoxEnd)
        {
            return SlotPointers.SelectMany(z => z)
                .Where(z => GetSlotFlags(z).HasFlagFast(StorageSlotFlag.Locked))
                .Any(slot => WithinRange(slot, BoxStart*BoxSlotCount, (BoxEnd + 1)*BoxSlotCount));
        }

        /// <summary>
        /// Sorts all <see cref="PKM"/> present within the range specified by <see cref="BoxStart"/> and <see cref="BoxEnd"/> with the provied <see cref="sortMethod"/>.
        /// </summary>
        /// <param name="BoxStart">Starting box; if not provided, will iterate from the first box.</param>
        /// <param name="BoxEnd">Ending box; if not provided, will iterate to the end.</param>
        /// <param name="sortMethod">Sorting logic required to order a <see cref="PKM"/> with respect to its peers; if not provided, will use a default sorting method.</param>
        /// <param name="reverse">Reverse the sorting order</param>
        /// <returns>Count of repositioned <see cref="PKM"/> slots.</returns>
        public int SortBoxes(int BoxStart = 0, int BoxEnd = -1, Func<IEnumerable<PKM>, IEnumerable<PKM>> sortMethod = null, bool reverse = false)
        {
            var BD = BoxData;
            int start = BoxSlotCount * BoxStart;
            var Section = BD.Skip(start);
            if (BoxEnd >= BoxStart)
                Section = Section.Take(BoxSlotCount * (BoxEnd - BoxStart + 1));

            Func<PKM, bool> skip = IsSlotOverwriteProtected;
            Section = Section.Where(z => !skip(z));
            var Sorted = (sortMethod ?? PKMSorting.OrderBySpecies)(Section);
            if (reverse)
                Sorted = Sorted.ReverseSort();

            var result = Sorted.ToArray();
            var boxclone = new PKM[BD.Count];
            BD.CopyTo(boxclone, 0);
            int count = result.CopyTo(boxclone, skip, start);

            SlotPointerUtil.UpdateRepointFrom(boxclone, BD, 0, SlotPointers);

            // clear storage flags to ensure all data is written back
            foreach (var pk in result)
                pk.StorageFlags = StorageSlotFlag.None;

            BoxData = boxclone;
            return count;
        }

        /// <summary>
        /// Removes all <see cref="PKM"/> present within the range specified by <see cref="BoxStart"/> and <see cref="BoxEnd"/> if the provied <see cref="deleteCriteria"/> is satisfied.
        /// </summary>
        /// <param name="BoxStart">Starting box; if not provided, will iterate from the first box.</param>
        /// <param name="BoxEnd">Ending box; if not provided, will iterate to the end.</param>
        /// <param name="deleteCriteria">Criteria required to be satisfied for a <see cref="PKM"/> to be deleted; if not provided, will clear if possible.</param>
        /// <returns>Count of deleted <see cref="PKM"/> slots.</returns>
        public int ClearBoxes(int BoxStart = 0, int BoxEnd = -1, Func<PKM, bool> deleteCriteria = null)
        {
            if (BoxEnd < 0)
                BoxEnd = BoxCount - 1;

            var blank = BlankPKM.EncryptedBoxData;
            int deleted = 0;
            for (int i = BoxStart; i <= BoxEnd; i++)
            {
                for (int p = 0; p < BoxSlotCount; p++)
                {
                    if (IsSlotOverwriteProtected(i, p))
                        continue;
                    var ofs = GetBoxSlotOffset(i, p);
                    if (!IsPKMPresent(ofs))
                        continue;
                    if (deleteCriteria != null)
                    {
                        var pk = GetStoredSlot(ofs);
                        if (!deleteCriteria(pk))
                            continue;
                    }

                    SetData(blank, ofs);
                    ++deleted;
                }
            }
            return deleted;
        }

        /// <summary>
        /// Modifies all <see cref="PKM"/> present within the range specified by <see cref="BoxStart"/> and <see cref="BoxEnd"/> with the modification routine provided by <see cref="action"/>.
        /// </summary>
        /// <param name="action">Modification to perform on a <see cref="PKM"/></param>
        /// <param name="BoxStart">Starting box; if not provided, will iterate from the first box.</param>
        /// <param name="BoxEnd">Ending box; if not provided, will iterate to the end.</param>
        /// <returns>Count of modified <see cref="PKM"/> slots.</returns>
        public int ModifyBoxes(Action<PKM> action, int BoxStart = 0, int BoxEnd = -1)
        {
            if (action == null)
                throw new ArgumentException(nameof(action));
            if (BoxEnd < 0)
                BoxEnd = BoxCount - 1;
            int modified = 0;
            for (int b = BoxStart; b <= BoxEnd; b++)
            {
                for (int s = 0; s < BoxSlotCount; s++)
                {
                    if (IsSlotOverwriteProtected(b, s))
                        continue;
                    var ofs = GetBoxSlotOffset(b, s);
                    if (!IsPKMPresent(ofs))
                        continue;
                    var pk = GetStoredSlot(ofs);
                    action(pk);
                    ++modified;
                    SetStoredSlot(pk, ofs, PKMImportSetting.Skip, PKMImportSetting.Skip);
                }
            }
            return modified;
        }

        public byte[] PCBinary => BoxData.SelectMany(pk => pk.EncryptedBoxData).ToArray();
        public byte[] GetBoxBinary(int box) => GetBoxData(box).SelectMany(pk => pk.EncryptedBoxData).ToArray();

        public bool SetPCBinary(byte[] data)
        {
            if (IsRegionOverwriteProtected(0, SlotCount))
                return false;
            if (data.Length != PCBinary.Length)
                return false;

            var BD = BoxData;
            var pkdata = PKX.GetPKMDataFromConcatenatedBinary(data, BlankPKM.EncryptedBoxData.Length);
            pkdata.Select(z => GetPKM(DecryptPKM(z))).CopyTo(BD, IsSlotOverwriteProtected);
            BoxData = BD;
            return true;
        }

        public bool SetBoxBinary(byte[] data, int box)
        {
            int start = box * BoxSlotCount;
            int end = start + BoxSlotCount;

            if (IsRegionOverwriteProtected(start, end))
                return false;
            if (data.Length != GetBoxBinary(box).Length)
                return false;

            var BD = BoxData;
            var pkdata = PKX.GetPKMDataFromConcatenatedBinary(data, BlankPKM.EncryptedBoxData.Length);
            pkdata.Select(z => GetPKM(DecryptPKM(z))).CopyTo(BD, IsSlotOverwriteProtected, start);
            BoxData = BD;
            return true;
        }

        protected virtual void SetPartyValues(PKM pkm, bool isParty)
        {
            if (!isParty)
                return;
            if (pkm.PartyStatsPresent) // Stats already present
                return;
            pkm.SetStats(pkm.GetStats(pkm.PersonalInfo));
            pkm.Stat_Level = pkm.CurrentLevel;
        }

        protected virtual void SetPKM(PKM pkm) { }
        protected virtual void SetDex(PKM pkm) { }
        public virtual bool GetSeen(int species) => false;
        public virtual void SetSeen(int species, bool seen) { }
        public virtual bool GetCaught(int species) => false;
        public virtual void SetCaught(int species, bool caught) { }
        public int SeenCount => HasPokeDex ? Enumerable.Range(1, MaxSpeciesID).Count(GetSeen) : 0;
        public int CaughtCount => HasPokeDex ? Enumerable.Range(1, MaxSpeciesID).Count(GetCaught) : 0;
        public decimal PercentSeen => (decimal) SeenCount / MaxSpeciesID;
        public decimal PercentCaught => (decimal)CaughtCount / MaxSpeciesID;

        public byte[] GetData(int Offset, int Length)
        {
            byte[] data = new byte[Length];
            Buffer.BlockCopy(Data, Offset, data, 0, Length);
            return data;
        }

        public void SetData(byte[] input, int Offset)
        {
            input.CopyTo(Data, Offset);
            Edited = true;
        }

        public bool IsRangeEmpty(int Offset, int Length) => IsRangeAll(Offset, Length, 0);

        public bool IsRangeAll(int Offset, int Length, int value)
        {
            for (int i = Offset + Length - 1; i >= Offset; i--)
            {
                if (Data[i] != value)
                    return false;
            }
            return true;
        }

        public virtual bool IsPKMPresent(int offset) => PKX.IsPKMPresent(Data, offset);

        public bool IsStorageFull => NextOpenBoxSlot() == StorageFullValue;
        private const int StorageFullValue = -1;

        public int NextOpenBoxSlot(int lastKnownOccupied = -1)
        {
            int count = BoxSlotCount * BoxCount;
            for (int i = lastKnownOccupied + 1; i < count; i++)
            {
                int offset = GetBoxSlotOffset(i);
                if (!IsPKMPresent(offset))
                    return i;
            }
            return StorageFullValue;
        }

        public abstract string GetString(byte[] data, int offset, int length);
        public string GetString(int offset, int length) => GetString(Data, offset, length);
        public abstract byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0);

        /// <summary>
        /// Compresses the <see cref="BoxData"/> by pulling out the empty storage slots and putting them at the end, retaining all existing data.
        /// </summary>
        /// <param name="storedCount">Count of actual <see cref="PKM"/> stored.</param>
        /// <param name="slotPointers">Important slot pointers that need to be repointed if a slot moves.</param>
        /// <returns>True if <see cref="BoxData"/> was updated, false if no update done.</returns>
        public bool CompressStorage(out int storedCount, params IList<int>[] slotPointers)
        {
            // keep track of empty slots, and only write them at the end if slots were shifted (no need otherwise).
            var empty = new List<byte[]>();
            bool shiftedSlots = false;

            ushort ctr = 0;
            int size = SIZE_STORED;
            int count = BoxSlotCount * BoxCount;
            for (int i = 0; i < count; i++)
            {
                int offset = GetBoxSlotOffset(i);
                if (IsPKMPresent(offset))
                {
                    if (ctr != i) // copy required
                    {
                        shiftedSlots = true; // appending empty slots afterwards is now required since a rewrite was done
                        int destOfs = GetBoxSlotOffset(ctr);
                        Buffer.BlockCopy(Data, offset, Data, destOfs, size);
                        SlotPointerUtil.UpdateRepointFrom(ctr, i, slotPointers);
                    }
                    ctr++;
                    continue;
                }

                // pop out an empty slot; save all unused data & preserve order
                byte[] data = new byte[size];
                Buffer.BlockCopy(Data, offset, data, 0, size);
                empty.Add(data);
            }

            storedCount = ctr;

            if (!shiftedSlots)
                return false;

            for (int i = ctr; i < count; i++)
            {
                var data = empty[i - ctr];
                int offset = GetBoxSlotOffset(i);
                data.CopyTo(Data, offset);
            }
            return true;
        }
    }
}
