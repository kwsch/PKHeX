using System;
using System.Linq;

namespace PKHeX
{
    // Base Class for Save Files
    public abstract class SaveFile
    {
        internal static bool SetUpdateDex = true;
        internal static bool SetUpdatePKM = true;

        // General Object Properties
        public byte[] Data;
        public bool Edited;
        public string FileName, FilePath;
        public abstract string BAKName { get; }
        public byte[] BAK { get; protected set; }
        public bool Exportable { get; protected set; }
        public abstract SaveFile Clone();
        public abstract string Filter { get; }
        public byte[] Footer { protected get; set; } = new byte[0]; // .dsv
        public byte[] Header { protected get; set; } = new byte[0]; // .gci
        public bool Japanese { get; set; }
        public string PlayTimeString => $"{PlayedHours}ː{PlayedMinutes.ToString("00")}ː{PlayedSeconds.ToString("00")}"; // not :
        public virtual bool IndeterminateGame => false;
        public virtual bool IndeterminateLanguage => false;
        public virtual bool IndeterminateSubVersion => false;

        // General PKM Properties
        public abstract Type PKMType { get; }
        public abstract PKM getPKM(byte[] data);
        public abstract PKM BlankPKM { get; }
        public abstract byte[] decryptPKM(byte[] data);
        public abstract int SIZE_STORED { get; }
        public abstract int SIZE_PARTY { get; }
        public abstract int MaxEV { get; }
        public virtual int MaxIV => 31;
        public ushort[] HeldItems { get; protected set; }

        // General SAV Properties
        public virtual byte[] Write(bool DSV)
        {
            setChecksums();
            if (Footer.Length > 0 && DSV)
                return Data.Concat(Footer).ToArray();
            if (Header.Length > 0)
                return Header.Concat(Data).ToArray();
            return Data;
        }
        public virtual string MiscSaveChecks() { return ""; }
        public virtual string MiscSaveInfo() { return ""; }
        public virtual GameVersion Version { get; protected set; }
        public abstract bool ChecksumsValid { get; }
        public abstract string ChecksumInfo { get; }
        public abstract int Generation { get; }
        public PersonalTable Personal { get; set; }

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
        public bool RBY => Version == GameVersion.RBY;

        public bool GSC => Version == GameVersion.GS || Version == GameVersion.C;

        public virtual int MaxMoveID => int.MaxValue;
        public virtual int MaxSpeciesID => int.MaxValue;
        public virtual int MaxAbilityID => int.MaxValue;
        public virtual int MaxItemID => int.MaxValue;
        public virtual int MaxBallID => int.MaxValue;
        public virtual int MaxGameID => int.MaxValue;

        // Flags
        public bool HasWondercards => WondercardData > -1;
        public bool HasSuperTrain => SuperTrain > -1;
        public bool HasBerryField => BerryField > -1;
        public bool HasHoF => HoF > -1;
        public bool HasSecretBase => SecretBase > -1;
        public bool HasPuff => Puff > -1;
        public bool HasPSS => PSS > -1;
        public bool HasOPower => OPower > -1;
        public bool HasJPEG => JPEGData != null;
        public bool HasBox => Box > -1;
        public virtual bool HasParty => Party > -1;
        public bool HasBattleBox => BattleBox > -1;
        public bool HasFused => Fused > -1;
        public bool HasGTS => GTS > -1;
        public bool HasDaycare => Daycare > -1;
        public virtual bool HasPokeDex => PokeDex > -1;
        public virtual bool HasBoxWallpapers => getBoxWallpaperOffset(0) > -1;
        public virtual bool HasSUBE => SUBE > -1 && !ORAS;
        public virtual bool HasGeolocation => false;
        public bool HasPokeBlock => ORAS && !ORASDEMO;
        public bool HasEvents => EventFlags != null;
        public bool HasLink => ORAS && !ORASDEMO || XY;

        // Counts
        protected virtual int GiftCountMax { get; } = int.MinValue;
        protected virtual int GiftFlagMax { get; } = 0x800;
        protected virtual int EventFlagMax { get; } = int.MinValue;
        protected virtual int EventConstMax { get; } = int.MinValue;
        public virtual int DaycareSeedSize { get; } = 0;
        public abstract int OTLength { get; }
        public abstract int NickLength { get; }
        public virtual int MaxMoney { get; } = 9999999;
        public virtual int MaxShadowID { get; } = 0;

        // Offsets
        protected virtual int Box { get; set; } = int.MinValue;
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
        public int Puff { get; protected set; } = int.MinValue;
        public int PSS { get; protected set; } = int.MinValue;
        public int BerryField { get; protected set; } = int.MinValue;
        public int OPower { get; protected set; } = int.MinValue;
        public int HoF { get; protected set; } = int.MinValue;

        // SAV Properties
        public PKM[] BoxData
        {
            get
            {
                PKM[] data = new PKM[BoxCount*BoxSlotCount];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = getStoredSlot(getBoxOffset(i/BoxSlotCount) + SIZE_STORED*(i%BoxSlotCount));
                    data[i].Identifier = $"{getBoxName(i/BoxSlotCount)}:{(i%BoxSlotCount + 1).ToString("00")}";
                    data[i].Box = i/BoxSlotCount + 1;
                    data[i].Slot = i%BoxSlotCount + 1;
                }
                return data;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                if (value.Length != BoxCount*BoxSlotCount)
                    throw new ArgumentException($"Expected {BoxCount*BoxSlotCount}, got {value.Length}");
                if (value.Any(pk => PKMType != pk.GetType()))
                    throw new ArgumentException($"Not {PKMType} array.");

                for (int i = 0; i < value.Length; i++)
                    setStoredSlot(value[i], getBoxOffset(i/BoxSlotCount) + SIZE_STORED*(i%BoxSlotCount));
            }
        }
        public PKM[] PartyData
        {
            get
            {
                PKM[] data = new PKM[PartyCount];
                for (int i = 0; i < data.Length; i++)
                    data[i] = getPartySlot(getPartyOffset(i));
                return data;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                if (value.Length == 0 || value.Length > 6)
                    throw new ArgumentException("Expected 1-6, got " + value.Length);
                if (value.Any(pk => PKMType != pk.GetType()))
                    throw new ArgumentException($"Not {PKMType} array.");
                if (value[0].Species == 0)
                    throw new ArgumentException("Can't have an empty first slot." + value.Length);

                PKM[] newParty = value.Where(pk => pk.Species != 0).ToArray();

                PartyCount = newParty.Length;
                Array.Resize(ref newParty, 6);

                for (int i = PartyCount; i < newParty.Length; i++)
                    newParty[i] = BlankPKM;
                for (int i = 0; i < newParty.Length; i++)
                    setPartySlot(newParty[i], getPartyOffset(i));
            }
        }
        public PKM[] BattleBoxData
        {
            get
            {
                if (!HasBattleBox)
                    return new PKM[0];

                PKM[] data = new PKM[6];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = getStoredSlot(BattleBox + SIZE_STORED * i);
                    if (data[i].Species == 0)
                        return data.Take(i).ToArray();
                }
                return data;
            }
        }

        public bool[] EventFlags
        {
            get
            {
                if (EventFlagMax < 0)
                    return null;

                bool[] Flags = new bool[EventFlagMax];
                for (int i = 0; i < Flags.Length; i++)
                    Flags[i] = (Data[EventFlag + i / 8] >> i % 8 & 0x1) == 1;
                return Flags;
            }
            set
            {
                if (EventFlagMax < 0)
                    return;
                if (value.Length != EventFlagMax)
                    return;

                byte[] data = new byte[value.Length / 8];
                for (int i = 0; i < value.Length; i++)
                    if (value[i])
                        data[i >> 3] |= (byte)(1 << (i & 7));

                data.CopyTo(Data, EventFlag);
            }
        }
        public ushort[] EventConsts
        {
            get
            {
                if (EventConstMax < 0)
                    return null;

                ushort[] Constants = new ushort[EventConstMax];
                for (int i = 0; i < Constants.Length; i++)
                    Constants[i] = BitConverter.ToUInt16(Data, EventConst + i * 2);
                return Constants;
            }
            set
            {
                if (EventConstMax < 0)
                    return;
                if (value.Length != EventConstMax)
                    return;

                for (int i = 0; i < value.Length; i++)
                    BitConverter.GetBytes(value[i]).CopyTo(Data, EventConst + i * 2);
            }
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
        protected virtual bool[] MysteryGiftReceivedFlags { get { return null; } set { } }
        protected virtual MysteryGift[] MysteryGiftCards { get { return null; } set { } }
        public virtual MysteryGiftAlbum GiftAlbum
        {
            get
            {
                return new MysteryGiftAlbum
                {
                    Flags = MysteryGiftReceivedFlags,
                    Gifts = MysteryGiftCards
                };
            }
            set
            {
                MysteryGiftReceivedFlags = value.Flags;
                MysteryGiftCards = value.Gifts;
            }
        }

        public virtual bool BattleBoxLocked { get { return false; } set { } }
        public virtual string JPEGTitle => null;
        public virtual byte[] JPEGData => null;
        public virtual int Country { get { return -1; } set { } }
        public virtual int ConsoleRegion { get { return -1; } set { } }
        public virtual int SubRegion { get { return -1; } set { } }

        // Trainer Info
        public virtual int Gender { get; set; }
        public virtual int Language { get { return -1; } set { } }
        public virtual int Game { get { return -1; } set { } }
        public virtual ushort TID { get; set; }
        public virtual ushort SID { get; set; }
        public int TrainerID7 => (int)((uint)(TID | (SID << 16)) % 1000000);
        public virtual string OT { get; set; }
        public virtual int PlayedHours { get; set; }
        public virtual int PlayedMinutes { get; set; }
        public virtual int PlayedSeconds { get; set; }
        public virtual int SecondsToStart { get; set; }
        public virtual int SecondsToFame { get; set; }
        public virtual uint Money { get; set; }
        public abstract int BoxCount { get; }
        public virtual int PartyCount { get; protected set; }
        public abstract string Extension { get; }

        // Varied Methods
        protected abstract void setChecksums();
        public abstract int getBoxOffset(int box);
        public abstract int getPartyOffset(int slot);
        public abstract string getBoxName(int box);
        public abstract void setBoxName(int box, string val);
        public virtual ulong? GameSyncID { get { return null; } set { } }
        public virtual ulong? Secure1 { get { return null; } set { } }
        public virtual ulong? Secure2 { get { return null; } set { } }

        // Daycare
        public int DaycareIndex = 0;
        public virtual bool HasTwoDaycares => false;
        public virtual int getDaycareSlotOffset(int loc, int slot) { return -1; }
        public virtual uint? getDaycareEXP(int loc, int slot) { return null; }
        public virtual string getDaycareRNGSeed(int loc) { return null; }
        public virtual bool? getDaycareHasEgg(int loc) { return null; }
        public virtual bool? getDaycareOccupied(int loc, int slot) { return null; }

        public virtual void setDaycareEXP(int loc, int slot, uint EXP) { }
        public virtual void setDaycareRNGSeed(int loc, string seed) { }
        public virtual void setDaycareHasEgg(int loc, bool hasEgg) { }
        public virtual void setDaycareOccupied(int loc, int slot, bool occupied) { }

        // Storage
        public virtual int BoxSlotCount => 30;
        public virtual int BoxesUnlocked { get { return -1; } set { } }
        public virtual byte[] BoxFlags { get { return null; } set { } }
        public virtual int CurrentBox { get { return 0; } set { } }

        protected virtual int getBoxWallpaperOffset(int box) { return -1; }
        public int getBoxWallpaper(int box)
        {
            int offset = getBoxWallpaperOffset(box);
            if (offset < 0 || box > BoxCount)
                return box;
            return Data[offset];
        }
        public virtual void setBoxWallpaper(int box, int val)
        {
            int offset = getBoxWallpaperOffset(box);
            if (offset < 0 || box > BoxCount)
                return;
            Data[offset] = (byte)val;
        }

        public virtual PKM getPartySlot(int offset)
        {
            return getPKM(decryptPKM(getData(offset, SIZE_PARTY)));
        }
        public virtual PKM getStoredSlot(int offset)
        {
            return getPKM(decryptPKM(getData(offset, SIZE_STORED)));
        }
        public void setPartySlot(PKM pkm, int offset, bool? trade = null, bool? dex = null)
        {
            if (pkm == null) return;
            if (pkm.GetType() != PKMType)
                throw new InvalidCastException($"PKM Format needs to be {PKMType} when setting to a Gen{Generation} Save File.");
            if (trade ?? SetUpdatePKM)
                setPKM(pkm);
            if (dex ?? SetUpdateDex)
                setDex(pkm);

            for (int i = 0; i < 6; i++)
                if (getPartyOffset(i) == offset)
                    if (PartyCount <= i)
                        PartyCount = i + 1;

            setData(pkm.EncryptedPartyData, offset);
            Edited = true;
        }
        public virtual void setStoredSlot(PKM pkm, int offset, bool? trade = null, bool? dex = null)
        {
            if (pkm == null) return;
            if (pkm.GetType() != PKMType)
                throw new InvalidCastException($"PKM Format needs to be {PKMType} when setting to a {GetType().Name.Last()} Save File.");
            if (trade ?? SetUpdatePKM)
                setPKM(pkm);
            if (dex ?? SetUpdateDex)
                setDex(pkm);

            setData(pkm.EncryptedBoxData, offset);
            Edited = true;
        }
        public void setPartySlot(byte[] data, int offset, bool? trade = null, bool? dex = null)
        {
            if (data == null) return;
            PKM pkm = getPKM(decryptPKM(data));
            if (trade ?? SetUpdatePKM)
                setPKM(pkm);
            if (dex ?? SetUpdateDex)
                setDex(pkm);

            for (int i = 0; i < 6; i++)
                if (getPartyOffset(i) == offset)
                    if (PartyCount <= i)
                        PartyCount = i + 1;

            setData(pkm.EncryptedPartyData, offset);
            Edited = true;
        }
        public void setStoredSlot(byte[] data, int offset, bool? trade = null, bool? dex = null)
        {
            if (data == null) return;
            PKM pkm = getPKM(decryptPKM(data));
            if (trade ?? SetUpdatePKM)
                setPKM(pkm);
            if (dex ?? SetUpdateDex)
                setDex(pkm);

            setData(pkm.EncryptedBoxData, offset);
            Edited = true;
        }
        public void deletePartySlot(int slot)
        {
            if (PartyCount <= slot) // beyond party range (or empty data already present)
                return;
            // Move all party slots down one
            for (int i = slot + 1; i < 6; i++) // Slide slots down
            {
                int slotTo = getPartyOffset(i - 1);
                int slotFrom = getPartyOffset(i);
                setData(getData(slotFrom, SIZE_PARTY), slotTo);
            }
            setStoredSlot(BlankPKM, getPartyOffset(5), false, false);
            PartyCount -= 1;
        }

        public void sortBoxes(int BoxStart = 0, int BoxEnd = -1)
        {
            PKM[] BD = BoxData;
            var Section = BD.Skip(BoxStart*BoxSlotCount);
            if (BoxEnd > BoxStart)
                Section = Section.Take(BoxSlotCount*(BoxEnd - BoxStart));

            var Sorted = Section
                .OrderBy(p => p.Species == 0) // empty slots at end
                .ThenBy(p => p.IsEgg) // eggs to the end
                .ThenBy(p => p.Species) // species sorted
                .ThenBy(p => p.AltForm) // altforms sorted
                .ThenBy(p => p.IsNicknamed).ToArray();

            Array.Copy(Sorted, 0, BD, BoxStart*BoxSlotCount, Sorted.Length);
            BoxData = BD;
        }
        public void resetBoxes(int BoxStart = 0, int BoxEnd = -1)
        {
            if (BoxEnd < 0)
                BoxEnd = BoxCount;
            for (int i = BoxStart; i < BoxEnd; i++)
            {
                int offset = getBoxOffset(i);
                for (int p = 0; p < BoxSlotCount; p++)
                    setStoredSlot(BlankPKM, offset + SIZE_STORED * p);
            }
        }

        public byte[] getPCBin() { return BoxData.SelectMany(pk => pk.EncryptedBoxData).ToArray(); }
        public byte[] getBoxBin(int box) { return BoxData.Skip(box*BoxSlotCount).Take(BoxSlotCount).SelectMany(pk => pk.EncryptedBoxData).ToArray(); }
        public bool setPCBin(byte[] data)
        {
            if (data.Length != getPCBin().Length)
                return false;

            int len = BlankPKM.EncryptedBoxData.Length;

            // split up data to individual pkm
            byte[][] pkdata = new byte[data.Length/len][];
            for (int i = 0; i < data.Length; i += len)
            {
                pkdata[i/len] = new byte[len];
                Array.Copy(data, i, pkdata[i/len], 0, len);
            }
            
            PKM[] pkms = BoxData;
            for (int i = 0; i < pkms.Length; i++)
                pkms[i] = getPKM(decryptPKM(pkdata[i]));
            BoxData = pkms;
            return true;
        }
        public bool setBoxBin(byte[] data, int box)
        {
            if (data.Length != getBoxBin(box).Length)
                return false;

            int len = BlankPKM.EncryptedBoxData.Length;

            // split up data to individual pkm
            byte[][] pkdata = new byte[data.Length/len][];
            for (int i = 0; i < data.Length; i += len)
            {
                pkdata[i/len] = new byte[len];
                Array.Copy(data, i, pkdata[i/len], 0, len);
            }

            PKM[] pkms = BoxData;
            for (int i = 0; i < BoxSlotCount; i++)
                pkms[box*BoxSlotCount + i] = getPKM(decryptPKM(pkdata[i]));
            BoxData = pkms;
            return true;
        }

        protected virtual void setPKM(PKM pkm) { }
        protected virtual void setDex(PKM pkm)
        {
            setSeen(pkm);
            setCaught(pkm);
        }

        public virtual bool getSeen(PKM pkm) { throw new NotImplementedException(); }
        public virtual bool getCaught(PKM pkm) { throw new NotImplementedException(); }
        protected internal virtual void setSeen(PKM pkm, bool seen = true) { }
        protected internal virtual void setCaught(PKM pkm, bool caught = true) { }
        
        public byte[] getData(int Offset, int Length)
        {
            if (Offset + Length > Data.Length)
                return null;

            byte[] data = new byte[Length];
            Array.Copy(Data, Offset, data, 0, Length);
            return data;
        }
        public void setData(byte[] input, int Offset)
        {
            input.CopyTo(Data, Offset);
            Edited = true;
        }

        public virtual bool RequiresMemeCrypto { get { return false; } }
    }
}
